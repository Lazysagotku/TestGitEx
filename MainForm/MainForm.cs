using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TimeReportV3.Properties;
using TimeReportV3.Repository;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;
using TrfCommonUtility;
using System.Reflection;
using Org.BouncyCastle.Asn1.Cmp;
using MathNet.Numerics.Providers.SparseSolver;
using System.Runtime.Remoting.Messaging;
using TimeReportV3.Params;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.ComponentModel.Design;
using NPOI.SS.Formula.Functions;

namespace TimeReportV3
{
    public partial class MainForm : Form
    {


        //to do: 2 мелких замечания:
        // 1.	Окно отчетов не запоминает предыдущие координаты
        // 2.	Окно отчетов не запоминает выбранного в прошлый раз пользователя
        //http://itdesk.trinfico.ru/browse/TRF-1300

        internal static string SharedDir { get; } = ConfigurationManager.AppSettings["SharedFolder"];
        internal static string ResourcesSubdir { get; } = $"{SharedDir}{ConfigurationManager.AppSettings["ResourcesFolder"]}";
        internal static string TaskViewUrl { get; } = $"https://{Properties.Settings.Default.AddressOfServerIntraService}/Task/View/";
        internal static string TaskJiraUrl { get; } = $"http://{Properties.Settings.Default.AddressOfServerJira}/browse/";
        internal static string UserLogin { get; private set; }
        internal static string UserName { get; private set; }

        private bool _isLoadingData = false; // Флаг для отслеживания загрузки
        internal static bool NeedRestart { get; set; } = false;
        internal static IEnumerable<User> Users { get; set; }
        private readonly Param1ActiveUserTask _isParam1;
        private readonly JiraParam1ActiveUserTask _jiraParam1;
        private readonly Param2TaskUserNotInProgress _isParam2;
        private readonly JiraParam2TaskUserNotInProgress _jiraParam2;
        private readonly Param3TasksWithoutExecutor _isParam3;
        private readonly JiraParam3TasksWithoutExecutor _jiraParam3;
        private readonly Param4UnreadCommentsInUserTasks _isParam4;
        private readonly JiraParam4UnreadCommentsInUserTasks _jiraParam4;
        private readonly Param5UnreadCommentsInAllTasks _isParam5;
        private readonly JiraParam5UnreadCommentsInAllTasks _jiraParam5;
        private TimeUserTable TimeUserTable;
        private IdTasksUserTable IdTasksUserTable;
        public ParamResult CurrTasksFor3 { get; set; }
        private NotifyIcon _currNotify;
        bool isBalloonVisible = false;
        private SimpleLock SimpleLock;

        private string AddressOfServerIntraService;
        private readonly ToolStripMenuItem HideShowMenuItem;
        internal IParamMainForm[] Parameters { get; set; }

        private TrayNotification TrayNotification;

        private readonly string header;

        private Icon[] Icons;
        private Icon IconConnectionBreak;
        private MainTable MainTable;
        public bool IsTimeUserTableVisible = false;//{ get; set; }
        public bool NeedRender { get; set; }

        private bool IsAllowFormClose = false;
        private bool _isRealExit = false;

        //private SmsProvider SmsProvider;

        private ReportForm ActiveReportForm;
        private MassTimestampForm ActiveMassTimestampForm;
        private SettingsForm ActiveSettingsForm;

        private bool IsTestLocation { get; set; }

        private Timer Timer;
        private readonly Action<bool> RefreshTable;
        private bool IsPossiblyMakeRead;
        private List<ParamResult> ParamResults;
        public RadioButton RadioButton1 => radioButton1;
        public RadioButton RadioButton2 => radioButton2;
        private bool _isVisible;
        private bool _dataLoaded = false;
        private bool _isShowFromTray = false;
        private FieldsIdTasksUserInfo _cachedTasksAll;
        private bool _cacheValid = false;
        private bool _allowRealClose = false;
        private FieldsTimeUserInfo[] _timeCache;
        private FieldsIdTasksUserInfo[] _idTasksCache;
        private readonly UserTasksRepo _repo = new UserTasksRepo();
        public bool IsDataLoaded { get; private set; }


        private void EnableDoubleBuffer(DataGridView dgv)
        {
            typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(dgv, true, null);
        }
        public MainForm()
        {

            InitializeComponent();
            EnableDoubleBuffer(dgvMainTable);
            GetPrm();
            //EnableDoubleBuffer(dgvTimeUserTable); 
            //EnableDoubleBuffer(dgvIdTasksTable);
            //dgvIdTasksTable.Visible = false;
            //dgvTimeUserTable.Visible = false;
            Users = BaseRepository.InitRepository();
            InitJira();
            SuspendLayout();
            dgvMainTable.SuspendLayout();
            InitParamsAndMainTable();
            //dgvIdTasksTable.SuspendLayout();
            dgvTimeUserTable.SuspendLayout();
            _lastSystemMode = _systemMode;
            dgvMainTable.ResumeLayout();
            //dgvIdTasksTable.ResumeLayout();
            dgvTimeUserTable.ResumeLayout();
            ResumeLayout(true);
            string currWinUser = WindowsIdentity.GetCurrent().Name.Replace($"{Environment.UserDomainName}\\", "");
            //string currEnvUser = Environment.UserName;
            UserLogin = Users.FirstOrDefault(u => string.Compare(u.Login, currWinUser, true) == 0)?.Login ?? currWinUser;
            UserName = Users.FirstOrDefault(u => string.Compare(u.Login, currWinUser, true) == 0)?.Name ?? null;

            header = $"{Application.ProductName} [{Application.ProductVersion /*Utils.VersionLabel*/}]";

            SimpleLock = new SimpleLock();

            // прячем наше окно из панели
            this.ShowInTaskbar = false;

            // создаем элементы меню
            HideShowMenuItem = new ToolStripMenuItem("Показать", Image.FromFile($"{ResourcesSubdir}show_icon.jpg"));// Resources.show_icon);
            var reportsMenuItem = new ToolStripMenuItem("Отчёты", Image.FromFile($"{ResourcesSubdir}report_icon.jpg"));
            var massTimestampMenuItem = new ToolStripMenuItem("Массовая отметка времени", Image.FromFile($"{ResourcesSubdir}mass_timestamp_icon.jpg"));
            var settingsMenuItem = new ToolStripMenuItem("Настройки", Image.FromFile($"{ResourcesSubdir}options_icon.jpg"));
            var exitMenuItem = new ToolStripMenuItem("Выход", Image.FromFile($"{ResourcesSubdir}exit_icon.jpg"));

            // добавляем элементы в меню
            contextMenuStrip1.Items.AddRange(new[] { HideShowMenuItem, reportsMenuItem, massTimestampMenuItem, settingsMenuItem, exitMenuItem });
            // ассоциируем контекстное меню с текстовым полем
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            // устанавливаем обработчики событий для меню
            HideShowMenuItem.Click += HideShowMenuItem_Click;
            reportsMenuItem.Click += ReportsMenuItem_Click;
            massTimestampMenuItem.Click += MassTimestampMenuItem_Click;
            settingsMenuItem.Click += SettingsMenuItem_Click;
            exitMenuItem.Click += ExitMenuItem_Click;

            SetNewIcons();
            //notifyIcon1.Visible = false;
            //notifyIcon1.Dispose();
            timerRefresh.Tick += new EventHandler(RefreshData);

            if (Properties.Settings.Default.UpgradeSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeSettings = false;
                Properties.Settings.Default.Save();
            }
            IsDataLoaded = true;

        }

        public void RestartApplication()
        {
            Properties.Settings.Default.MainFormLocationX = Location.X;
            Properties.Settings.Default.MainFormLocationY = Location.Y;
            Properties.Settings.Default.Save();

            notifyIcon1.Visible = false;
            notifyIcon1.Dispose();

            //Application.Restart();
            Process.Start(Application.ExecutablePath, "show");
            //Environment.Exit(0 );
            Application.Exit();
        }
        private void LoadIdTasksData(string date)
        {
            _idTasksCache = IdTasksUserTable.GetData(date);
        }

        private void RenderIdTasksTable()
        {
            IdTasksUserTable.Render(_idTasksCache);
        }

        private async void LoadIdTaskDataAsync(string date)
        {
            dgvIdTasksTable.Visible = false;

            await Task.Run(() => LoadIdTasksData(date));

            BeginInvoke(new Action(() =>
            {
                RenderIdTasksTable();
                dgvIdTasksTable.Visible = true;
            }));
        }
        private void LoadTimeUserData()
        {
            string _timeCache = Convert.ToString(TimeUserTable.GetTimeUserOnLastWorkingdays());
        }

        private void RenderTimeUserTable()
        {

            //LoadIdTasksDataAsync(_timeCache);
        }

        public async void LoadTimeUserDataAsync()
        {
            dgvTimeUserTable.Visible = false;

            await Task.Run(() => LoadTimeUserData());

            BeginInvoke(new Action(() =>
            {
                RenderTimeUserTable();
                //RenderIdTasksTable();
                dgvTimeUserTable.Visible = true;
            }));

        }

        private void DgvMainTable_SelectionChanged(object sender, EventArgs e)
        {
            dgvMainTable.ClearSelection();
        }
        private void DgvMainTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (MainTable == null)
                return;

            SetFormSize();
            GetMainFormPosition();
            //MainTable.HandleCellClick(e); 
        }
        private void DgvMainTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            //SuspendLayout();
            //dgvMainTable.SuspendLayout();
            //dgvIdTasksTable.SuspendLayout();
            //dgvTimeUserTable.SuspendLayout();
            if (MainTable == null)
                return;

            SetFormSize();
            //GetMainFormPosition();
            //MainTable.HandleCellDoubleClick(e); 
            if (e.RowIndex < 0)
                return;

            if (e.RowIndex >= ParamResults.Count) return;


            var param = ParamResults[e.RowIndex];
            if (!param.IsMinutesUsed)
                return;

            ToggleTimeUserTable();
            //dgvMainTable.ResumeLayout();
            //dgvIdTasksTable.ResumeLayout();
            //dgvTimeUserTable.ResumeLayout();
            //ResumeLayout(true);
        }

        public void ToggleTimeUserTable()
        {
            IsTimeUserTableVisible = !IsTimeUserTableVisible;

            dgvTimeUserTable.Visible = IsTimeUserTableVisible;
            dgvIdTasksTable.Visible = false;

            /*if (IsTimeUserTableVisible)
            {
                TimeUserTable.Refresh(this);
            }*/
            SetFormSize();
            ShowIdTasksTable();
            RebuildLayout();
        }

        private void ShowIdTasksTable()
        {
            dgvIdTasksTable.Visible = true;
            dgvTimeUserTable.Visible = true;
            RebuildLayout();
        }
        public void RebuildLayout()
        {
            if (!_dataLoaded) return;
            int rightX = dgvMainTable.Right + 10;

            if (dgvTimeUserTable.Visible)
            {
                dgvTimeUserTable.Location = new Point(rightX, dgvMainTable.Top);
                rightX = dgvTimeUserTable.Right + 10;
            }

            if (dgvIdTasksTable.Visible)
            {
                dgvIdTasksTable.Location = new Point(rightX, dgvMainTable.Top);
                //rightX = dgvIdTasksTable.Left + 25;
            }

            int newWidth = rightX + (dgvIdTasksTable.Visible ? dgvIdTasksTable.Width : dgvTimeUserTable.Visible ? dgvTimeUserTable.Width : 0) + 25;
            Width = Math.Max(newWidth, MinimumSize.Width);
        }
        private void InitJira()
        {
            var jiraConn = ConfigurationManager.ConnectionStrings["JiraDB"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(jiraConn))
            {
                MessageBox.Show("Не найдена строка подключения JiraDB в App.config", "Ошибка Jira", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            JiraRepository.Init(jiraConn);
        }

        private void UpdateTrayText()
        {
            HideShowMenuItem.Text = _isVisible ? "Скрыть" : "Показать";
            HideShowMenuItem.Image = Image.FromFile(_isVisible ? $"{ResourcesSubdir}hide_icon.jpg" : $"{ResourcesSubdir}show_icon.jpg");

            //notifyIcon1.Text = _isVisible ? "TimeReport - Скрыть" : "TimeReport - Показать";
        }

        private Panel panelLeft;
        private void MainForm_Load(object sender, EventArgs e)
        {

            //typeof(DataGridView).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(dgvTimeUserTable, null, null);
            //notifyIcon1.Visible = true;
            SuspendLayout();
            dgvMainTable.SuspendLayout();
            //RefreshData1(null, null);
            //ShowDialog(); 
            this.Location = GetVisibleLocation(Properties.Settings.Default.MainFormLocation);
            //SuspendLayout();
            //ResumeLayout();
            dgvMainTable.Visible = false;
            dgvIdTasksTable.Visible = false;
            dgvTimeUserTable.Visible = false;


            Task.Run(() =>
            {
                LoadTimeUserData();
            });

            BeginInvoke(new Action(() =>
            {
                //InitJira();
                //SuspendLayout();
                // dgvMainTable.SuspendLayout();
                //InitParamsAndMainTable();
                // _lastSystemMode = _systemMode;
                // dgvMainTable.ResumeLayout();
                // ResumeLayout(true);
                //Activate();
                SetTimeTablesVisible1(!dgvMainTable.Visible);

                //RebuildLayout();
                //RefreshData1(null, null);
            }



            ));

            //if(Properties.Settings.Default.MainFormLocationX !=0 || Properties.Settings.Default.MainFormLocationY != 0)
            //{
            //    Location = new Point(Properties.Settings.Default.MainFormLocationX, Properties.Settings.Default.MainFormLocationY);
            //}

            radioButton2.Checked = true;
            //FillTimeUserTable(data);
            dgvMainTable.ResumeLayout();
            ResumeLayout(true);

            RefreshData1(null, null);
        }

        public void ResizeMainForm()
        {
            SuspendLayout();
            int width = dgvMainTable.Right;
            if (IsTimeUserTableVisible)
                width = dgvIdTasksTable.Right + 10;
            //width=dgvIdTasksTable.Right + 10;

            Width = width;
            ResumeLayout(true);
            RebuildLayout();
        }

        public async void LoadIdTasksDataAsync(string date)
        {
            dgvIdTasksTable.Visible = false;

            var data = await Task.Run(() =>
                IdTasksUserTable.GetData(date));

            IdTasksUserTable.Render(data);

            dgvIdTasksTable.Visible = true;

            SetFormSize();
        }
        public void SetTimeTablesVisible(bool visible)
        {

            SuspendLayout();
            dgvTimeUserTable.SuspendLayout();
            dgvIdTasksTable.SuspendLayout();
            IsTimeUserTableVisible = visible;
            //dgvTimeUserTable.Visible = false;
            //dgvIdTasksTable.Visible = false;
            //dgvTimeUserTable.Visible = visible;
            //dgvIdTasksTable.Visible = visible;
            if (visible)
            {
                TimeUserTable.Refresh(this);
                //LoadTimeUserDataAsync();
                //RenderIdTasksTable();
            }
            RebuildLayout();
            GetAutoSizeMode();
            dgvIdTasksTable.ResumeLayout();
            dgvTimeUserTable.ResumeLayout();
            ResumeLayout(true);
            IsTimeUserTableVisible = visible;
            dgvTimeUserTable.Visible = visible;
            dgvIdTasksTable.Visible = visible;
            NeedRender = true;



        }

        public void SetTimeTablesVisible1(bool visible)
        {
            //Visible = false;
            SuspendLayout();
            //////dgvTimeUserTable.SuspendLayout();
            //dgvIdTasksTable.SuspendLayout();
            //dgvTimeUserTable.Visible = false;
            //dgvIdTasksTable.Visible = false;
            //dgvTimeUserTable.Visible = visible;
            //dgvIdTasksTable.Visible = visible;
            if (visible)
            {
                TimeUserTable.Refresh(this);
            }
            RebuildLayout();
            GetAutoSizeMode();
            //dgvIdTasksTable.ResumeLayout();
            //dgvTimeUserTable.ResumeLayout();
            ResumeLayout(true);
            //IsTimeUserTableVisible = visible;
            //dgvTimeUserTable.Visible = visible;
            //dgvIdTasksTable.Visible = visible;
            NeedRender = true;


        }

        private WorkingDays WorkingDays;

        private readonly UserTasksRepo userTasksRepo = new UserTasksRepo();
        public FieldsTimeUserInfo[] LoadTimeUserAll()
        {

            WorkingDays = WorkingDays.GetInstance();
            var workingDays = WorkingDays.Get();
            if (workingDays?.Count() == 0)
                return null;
            string firstDay = workingDays.Last();
            string line = string.Join(", ", workingDays.Select(wd => $"(cast('{wd}' as date))"));
            var qJira = JiraTasksRepo.GetTimeUserOnLastWorkingDays()?.ToArray()
                      ?? Array.Empty<FieldsTimeUserInfo>();
            var qIs = userTasksRepo.GetTimeUserOnLastWorkingdays(line, firstDay)?.ToArray()
               ?? Array.Empty<FieldsTimeUserInfo>();
            return qJira
                .Concat(qIs)
                .GroupBy(x => x.Date)
                .Select(g => new FieldsTimeUserInfo
                {
                    Date = g.Key,
                    Minutes = g.Sum(x => x.Minutes),
                    //System = "All" 
                })
                .OrderByDescending(x => x.Date)
                .ToArray();
        }

        private void FillTimeUserTable(FieldsTimeUserInfo[] data)
        {
            SuspendLayout();
            dgvTimeUserTable.SuspendLayout();
            dgvIdTasksTable.SuspendLayout();
            dgvTimeUserTable.Rows.Clear();
            foreach (var item in data)
            {
                dgvTimeUserTable.Rows.Add(item.Date.ToString("yyyy-MM-dd"),
                    TimeSpan.FromMinutes(item.Minutes).ToString(@"mm"));
            }
            var result = data
                .Concat(data)
                    .GroupBy(x => x.Date)
                    .Select(g => new FieldsTimeUserInfo
                    {
                        Date = g.Key,
                        Minutes = g.Sum(x => x.Minutes)
                    })
                .OrderByDescending(x => x.Date)
                .ToArray();

            data = result;
            dgvTimeUserTable.ClearSelection();
            dgvIdTasksTable.ClearSelection();
            dgvTimeUserTable.ResumeLayout(false);
            dgvIdTasksTable.ResumeLayout(false);
            ResumeLayout(true);

        }
        private Point GetVisibleLocation(Point location)
        {
            var rectangle = new Rectangle(location, this.RestoreBounds.Size);
            if (IsVisibleOnAnyScreen(rectangle))
            {
                return new Point(rectangle.X, rectangle.Y);
            }

            return new Point((Screen.PrimaryScreen.WorkingArea.Width - RestoreBounds.Width) / 2,
                             (Screen.PrimaryScreen.WorkingArea.Height - RestoreBounds.Height) / 2);
        }


        private void InitParamsAndMainTable()
        {
            //IsTimeUserTableVisible = false;
            NeedRender = false;
            if (_systemMode == SystemMode.IS || Properties.Settings.Default.NotifyIS)
            {
                var param3TasksWithoutExecutor = new Param3TasksWithoutExecutor();
                var jiraparam3TasksWithoutExecutor = new JiraParam3TasksWithoutExecutor();
                var comboparam3TasksWithoutExecutor = new CombinedParam3TasksWithoutExecutor();
                TrayNotification = new TrayNotification(this, notifyIcon1, param3TasksWithoutExecutor, jiraparam3TasksWithoutExecutor, comboparam3TasksWithoutExecutor, _systemMode);
            }
            else if (_systemMode == SystemMode.Jira || Properties.Settings.Default.NotifyJira)
            {
                var param3TasksWithoutExecutor = new Param3TasksWithoutExecutor();
                var jiraparam3TasksWithoutExecutor = new JiraParam3TasksWithoutExecutor();
                var comboparam3TasksWithoutExecutor = new CombinedParam3TasksWithoutExecutor();
                TrayNotification = new TrayNotification(this, notifyIcon1, param3TasksWithoutExecutor, jiraparam3TasksWithoutExecutor, comboparam3TasksWithoutExecutor, _systemMode);
            }
            else // All
            {
                var param3TasksWithoutExecutor = new Param3TasksWithoutExecutor();
                var jiraparam3TasksWithoutExecutor = new JiraParam3TasksWithoutExecutor();
                var comboparam3TasksWithoutExecutor = new CombinedParam3TasksWithoutExecutor();
                TrayNotification = new TrayNotification(this, notifyIcon1, param3TasksWithoutExecutor, jiraparam3TasksWithoutExecutor, comboparam3TasksWithoutExecutor, _systemMode);
            }
            //GetPrm();


            //SmsProvider = new SmsProvider(param3TasksWithoutExecutor, false);

            if (_systemMode == SystemMode.IS)
            {
                Parameters = new IParamMainForm[]
                                {
                    new Param1ActiveUserTask(),
                    new Param2TaskUserNotInProgress(),
                    new Param3TasksWithoutExecutor(),
                    new Param4UnreadCommentsInUserTasks(),
                    new Param5UnreadCommentsInAllTasks(),
                    new CombinedSumTimeParam()

                                };
            }
            else if (_systemMode == SystemMode.Jira)
            {
                Parameters = new IParamMainForm[]
                {
            new Params.JiraParam1ActiveUserTask(),
            new Params.JiraParam2TaskUserNotInProgress(),
            new Params.JiraParam3TasksWithoutExecutor(),
            new Params.JiraParam4UnreadCommentsInUserTasks(),
            new Params.JiraParam5UnreadCommentsInAllTasks(),
            new Params.CombinedSumTimeParam()
                };
            }
            else // All
            {
                Parameters = new IParamMainForm[]
                {
            new Params.CombinedParam1ActiveUserTask(),
            new Params.CombinedParam2TaskUserNotInProgress(),
            new Params.CombinedParam3TasksWithoutExecutor(),
            new Params.CombinedParam4UnreadCommentsInUserTasks(),
            new Params.CombinedParam5UnreadCommentsInAllTasks(),
            //new Params.CombinedParam6TimeSpentByUser(),
            new Params.CombinedSumTimeParam()
                };
            }

            //MainTable = new MainTable(dgvMainTable, Parameters, this);
            //TimeUserTable = new TimeUserTable(dgvTimeUserTable, dgvIdTasksTable);
            MainTable?.Dispose();
            MainTable = new MainTable(dgvMainTable, Parameters, this);
            //AdjustLayout();
            _ = RadioButton1;
            //TimeUserTable = null;
            if (TimeUserTable == null)
            {

                TimeUserTable = new TimeUserTable(dgvTimeUserTable, dgvIdTasksTable, ShowIdTasksTable, this);
                AdjustLayout();
            }
            /*if (IdTasksUserTable == null)
            {                                                                                                                                    

                IdTasksUserTable = new IdTasksUserTable(dgvIdTasksTable, this);
                AdjustLayout();
            }*/
        }

        private void ApplySystemFilter(SystemMode system)
        {
            dgvIdTasksTable.Visible = false;
            dgvTimeUserTable.Visible = false;
            AdjustLayout();

        }

        private void AdjustLayout()
        {
            int baseWidth = 600;
            int tablesWidth = 100;

            if (dgvTimeUserTable.Visible)
                tablesWidth += dgvTimeUserTable.Width;

            //if (dgvIdTasksTable.Visible)
            //    tablesWidth += dgvIdTasksTable.Width;


            //if (dgvMainTable.Visible)
            //    tablesWidth += dgvMainTable.Width;

            //RefreshData1(null,null);

            Width = baseWidth + tablesWidth + 20;
        }

        public void RefreshData(object myObject, EventArgs myEventArgs)
        {
            if (IsTimeUserTableVisible)
            {
                if (TimeUserTable == null)
                {
                    TimeUserTable = new TimeUserTable(dgvTimeUserTable, dgvIdTasksTable, ShowIdTasksTable, this);
                }
                //TimeUserTable.Refresh(this);
            }
            if (!SimpleLock.IsAllowed())
                return;

            try
            {
                if (timerRefresh.Enabled)
                    timerRefresh.Stop();

                UpdateAll();
                //CheckVersion();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                SimpleLock.Release();
                if (!timerRefresh.Enabled)
                    timerRefresh.Start();
            }
            //ResizeMainForm();
        }

        public void RefreshData1(object myObject, EventArgs myEventArgs)
        {

            //SuspendLayout();
            //dgvTimeUserTable.SuspendLayout();
            //dgvIdTasksTable.SuspendLayout();
            if (IsTimeUserTableVisible)
            {
                if (TimeUserTable == null)
                {
                    TimeUserTable = new TimeUserTable(dgvTimeUserTable, dgvIdTasksTable, ShowIdTasksTable, this);
                }
                TimeUserTable.Refresh(this);
            }
            if (!SimpleLock.IsAllowed())
                return;

            try
            {
                if (timerRefresh.Enabled)
                    timerRefresh.Stop();

                UpdateAll1();
                //CheckVersion();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                SimpleLock.Release();
                if (!timerRefresh.Enabled)
                    timerRefresh.Start();
            }
            //ResizeMainForm();

            //dgvTimeUserTable.ResumeLayout();
            //dgvIdTasksTable.ResumeLayout();
            //ResumeLayout(true);
        }

        public SystemMode CurrentSystemMode => _systemMode;

        public void SetSystemMode(SystemMode mode)
        {
            if (mode == _systemMode) return;

            _systemMode = mode;
            _cacheValid = false;

            // Сразу показываем "Загрузка..." в таблицах
            MainTable?.ShowLoadingState();
            TimeUserTable?.InvalidateCache();

            // Асинхронное обновление без блокировки UI
            RefreshDataAsync();
        }

        /// <summary>
        /// Принудительное обновление данных (после действий типа "Сделать прочитанным")
        /// </summary>
        public void ForceRefreshData()
        {
            _cacheValid = false;
            TimeUserTable?.InvalidateCache();
            RefreshData(null, null);
        }
        /// <summary>
        /// Асинхронное обновление данных с показом старых данных
        /// </summary>
        private async void RefreshDataAsync()
        {
            if (_isLoadingData) return;
            _isLoadingData = true;

            try
            {
                // Показываем "Загрузка..." только если нет старых данных
                if (MainTable == null || dgvMainTable.Rows.Count == 0)
                {
                    MainTable?.ShowLoadingState();
                }

                // Загружаем данные в фоновом потоке
                await Task.Run(() =>
                {
                    UpdateAllData();
                });

                // Обновляем UI в главном потоке
                BeginInvoke(new Action(() =>
                {
                    MainTable?.HideLoadingState();
                    RefreshData(null, null);
                }));
            }
            finally
            {
                _isLoadingData = false;
            }
        }

        /// <summary>
        /// Обновление данных в фоновом потоке
        /// </summary>
        private void UpdateAllData()
        {
            if (_lastSystemMode != _systemMode)
            {
                BeginInvoke(new Action(() =>
                {
                    InitParamsAndMainTable();
                }));
                _lastSystemMode = _systemMode;
            }

            // Предзагрузка данных параметров параллельно
            if (Parameters != null)
            {
                var tasks = Parameters.Select(p => Task.Run(() => p.Get())).ToArray();
                Task.WaitAll(tasks);
            }
        }
        private void ShowMainForm()
        {
            if (_isVisible) return;
            SuspendLayout();
            ResizeMainForm();
            if (!_dataLoaded)
            {
                RefreshData1(null, null);
                _dataLoaded = true;
            }


            _isVisible = true;
            // Восстанавливаем видимость таблиц из настроек
            IsTimeUserTableVisible = Properties.Settings.Default.TimeUserTableVisible;
            dgvTimeUserTable.Visible = IsTimeUserTableVisible;
            dgvIdTasksTable.Visible = IsTimeUserTableVisible;
            RebuildLayout();
            UpdateTrayText();
            ResumeLayout(true);
            BringToFront();
            Activate();
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void HideMainForm()
        {
            if (!_isVisible) return;
            SuspendLayout();
            // Сохраняем состояние таблиц перед скрытием
            Properties.Settings.Default.TimeUserTableVisible = IsTimeUserTableVisible;
            Properties.Settings.Default.Save();

            // Скрываем таблицы
            dgvTimeUserTable.Visible = false;
            dgvIdTasksTable.Visible = false;
            ResizeMainForm();
            RefreshData1(null, null);
            Hide();
            WindowState = FormWindowState.Minimized;
            _isVisible = false;

            UpdateTrayText();
            ResumeLayout(true);

        }

        public SystemMode GetCurrentSystemMode()
        {
            return CurrentSystemMode;
        }
        private void UpdateAll()
        {
            //SuspendLayout();

            //ResumeLayout(true);
            //RebuildLayout();

            if (_lastSystemMode != _systemMode)
            {
                InitParamsAndMainTable();
                _lastSystemMode = _systemMode;
            }

            timerRefresh.Interval = Properties.Settings.Default.FrequencyUpdatingValues * 60000;
            /*#if DEBUG
                        timerRefresh.Interval = 30000;
            #endif*/
            notifyIcon1.Text = $"Актуально на {DateTime.Now:HH\\:mm\\:ss}";
            Text = $"{header} {DateTime.Now.ToLongDateString()} {DateTime.Now:HH\\:mm}";// {new string(' ', 10)}

            bool isChanged = MainTable.GetActualTaskCounts(out List<int> tasksCounts);


            TrayNotification.Show(TrayIconStatus.ShowTasksWithoutExecutor
            , tasksCounts[(int)TrayIconStatus.ShowTasksWithoutExecutor]);
            TryUpdateNotifyIcon(tasksCounts);




            //notifyIcon1.BalloonTipHidden += (s, e) => isBalloonVisible = false;
            NeedRender = NeedRender || this.WindowState == FormWindowState.Normal;
            if (isChanged && !NeedRender && !IsTimeUserTableVisible)
            {
                return;
            }
            Parameters.SelectMany(p => p.Get()).ToList();
            if (NeedRender)
            {
                RefreshData();
                NeedRender = false;
            }
        }

        private void UpdateAll1()
        {
            //SuspendLayout();

            //ResumeLayout(true);
            //RebuildLayout();

            if (_lastSystemMode != _systemMode)
            {
                InitParamsAndMainTable();
                _lastSystemMode = _systemMode;
            }

            timerRefresh.Interval = Properties.Settings.Default.FrequencyUpdatingValues * 60000;
            /*#if DEBUG
                        timerRefresh.Interval = 30000;
            #endif*/
            notifyIcon1.Text = $"Актуально на {DateTime.Now:HH\\:mm\\:ss}";
            Text = $"{header} {DateTime.Now.ToLongDateString()} {DateTime.Now:HH\\:mm}";// {new string(' ', 10)}

            bool isChanged = MainTable.GetActualTaskCounts(out List<int> tasksCounts);
            //TrayNotification.Show(TrayIconStatus.ShowTasksWithoutExecutor
            //    , tasksCounts[(int)TrayIconStatus.ShowTasksWithoutExecutor]);
            //TryUpdateNotifyIcon(tasksCounts);

            NeedRender = NeedRender || this.WindowState == FormWindowState.Normal;
            if (isChanged && !NeedRender && !IsTimeUserTableVisible)
            {
                return;
            }
            Parameters.SelectMany(p => p.Get()).ToList();
            if (NeedRender)
            {
                RefreshData();
                NeedRender = false;
            }
        }

        public async void PreloadData()
        {
            await Task.Run(() =>
            {
                foreach (var p in Parameters)
                {
                    p.Get();
                }
            });

        }

        /*public async void LoadTimeUserDataAsync()
        {
            dgvTimeUserTable.Visible = false;
            dgvIdTasksTable.Visible = false;

            var data = await Task.Run(() =>
            {
                return TimeUserTable.GetTimeUserOnLastWorkingdays();
            });

            BeginInvoke(new Action(() =>
            {
                dgvTimeUserTable.Visible = true;
                dgvIdTasksTable.Visible = true;

            }));
        }*/

        /*public ParamResult GetParamByIndex(byte index, byte system)
        {
            List<ParamResult> list;

            switch ((SystemMode)system)
            {
                case SystemMode.Jira:
                    list = GetJiraParam.SelectMany(p => p.Get()).ToList();
            }
        }*/

        /*public ParamResult GetParamByIndex(byte index, byte system)
        {
            var mode = (SystemMode)system;
            var list = Parameters.Where(p => p.SystemMode == mode).SelectMany(p => p.Get()).ToList();

            if (index -1 < 0 || index - 1 >= list.Count) return null;
            return list[index -1];
        }*/
        private bool TryUpdateNotifyIcon(List<int> taskCounts)
        {
            int indexParam = Properties.Settings.Default.SelectionIndexDisplayInSystemTray;
            byte system = Properties.Settings.Default.TraySystemMode;

            int count = 0;

            if (system == 1) // IS
            {
                var counts = _repo.GetCountTasksByStatus(); // ВЫЗЫВАЕМ 1 РАЗ

                if (indexParam >= 1 && indexParam <= counts.Count)
                    count = counts[indexParam - 1];
            }
            else if (system == 2) // Jira
            {
                var counts = JiraTasksRepo.GetTasksCounts(MainForm.UserName)?.ToList(); // 1 РАЗ

                if (counts != null && indexParam >= 1 && indexParam <= counts.Count)
                    count = counts[indexParam - 1];
            }
            else if (system == 3) // All
            {
                var countsIS = _repo.GetCountTasksByStatus(); // 1 РАЗ
                var countsJira = JiraTasksRepo.GetTasksCounts(MainForm.UserName)?.ToList(); // 1 РАЗ

                if (countsIS != null && countsJira != null &&
                    indexParam >= 1 &&
                    indexParam <= countsIS.Count &&
                    indexParam <= countsJira.Count)
                {
                    count = countsIS[indexParam - 1] + countsJira[indexParam - 1];
                }
            }

            notifyIcon1.Icon = Icons[count <= 10 ? count : Icons.Length - 1];
            notifyIcon1.Visible = true;
            HideShowMenuItem.Visible = true;

            return true;
        }

        private void SetNewIcons()
        {

            Icons = new[]
            {
                    new Icon($"{ResourcesSubdir}0_16х16.ico"),
                    new Icon($"{ResourcesSubdir}1_16х16.ico"),
                    new Icon($"{ResourcesSubdir}2_16х16.ico"),
                    new Icon($"{ResourcesSubdir}3_16х16.ico"),
                    new Icon($"{ResourcesSubdir}4_16х16.ico"),
                    new Icon($"{ResourcesSubdir}5_16х16.ico"),
                    new Icon($"{ResourcesSubdir}6_16х16.ico"),
                    new Icon($"{ResourcesSubdir}7_16х16.ico"),
                    new Icon($"{ResourcesSubdir}8_16х16.ico"),
                    new Icon($"{ResourcesSubdir}9_16х16.ico"),
                    new Icon($"{ResourcesSubdir}10_16х16.ico"),
                    new Icon($"{ResourcesSubdir}10+_16х16.ico"),
                };

            IconConnectionBreak = new Icon($"{ResourcesSubdir}ConnectionBreak_16х16.ico");
        }

        public void CheckVersion()
        {
            try
            {
                //string fileversion = FileVersionInfo.GetVersionInfo(Path.Combine(System.IO.Directory.GetCurrentDirectory(), Assembly.GetExecutingAssembly().GetName().Name + ".exe")).FileVersion;
                string fileversion = FileVersionInfo.GetVersionInfo(Path.Combine(@"\\alef\Stockroute\TimeReport v2", Assembly.GetExecutingAssembly().GetName().Name + ".exe")).FileVersion;
                string runningversion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                if (fileversion.Length > 0 && runningversion.Length > 0)
                    if (fileversion != runningversion)
                    {
                        Process[] runningProcesses = Process.GetProcesses();
                        foreach (Process process in runningProcesses)
                        {
                            if (process.ProcessName == $"{Assembly.GetExecutingAssembly().GetName().Name}")
                            {
                                Process.Start(Path.Combine(@"\\alef\Stockroute\TimeReport v2\TimeReportV3Starter.exe"));
                                //Process.Start(Path.Combine(System.IO.Directory.GetCurrentDirectory(), "TimeReportV3Starter.exe"));
                                process.Kill();
                                return;
                            }
                        }
                    }
            }
            catch (Exception)
            {
                // на случай неполадок в сети, недоступности сетевой папки timereport
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            _allowRealClose = true;
            _isRealExit = true;

            SaveCurrentLocation();
            notifyIcon1.Visible = false;
            try
            {
                Application.Exit();
                Environment.Exit(0);
            }
            catch { }
            finally
            {
                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveSettingsForm != null)
            {
                ActiveSettingsForm.Activate();
                return;
            }

            ActiveSettingsForm = new SettingsForm(this);
            if (!Properties.Settings.Default.NotifyIS &&
            !Properties.Settings.Default.NotifyJira &&
            !Properties.Settings.Default.NotifyAll)
            {
                Properties.Settings.Default.NotifyAll = true;
                Properties.Settings.Default.Save();
                //RefreshData1(null, null);
            }
            //ActiveSettingsForm.RadioButton3.Checked = true;
            var result = ActiveSettingsForm.ShowDialog();
            ActiveSettingsForm = null;
            if (result == DialogResult.OK && !NeedRestart)
            {
                RefreshData1(null, null);
            }
            if (NeedRestart)
            {
                _allowRealClose = true;
                RefreshData1(null, null);
                Close();
            }
        }

        private void ReportsMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveReportForm != null)
            {
                ActiveReportForm.Activate();
                return;
            }

            ActiveReportForm = new ReportForm();
            var result = ActiveReportForm.ShowDialog();
            if (result == DialogResult.Abort)
            {
                // скорее всего нет соединения, и надо обновить иконку
                RefreshData1(null, null);
            }
            ActiveReportForm = null;
        }

        private void MassTimestampMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMassTimestampForm != null)
            {
                ActiveMassTimestampForm.Activate();
                return;
            }

            ActiveMassTimestampForm = new MassTimestampForm();
            var result = ActiveMassTimestampForm.ShowDialog();
            if (result == DialogResult.Abort)
            {
                // скорее всего нет соединения, и надо обновить иконку
                RefreshData(null, null);
            }
            ActiveMassTimestampForm = null;
        }


        private void HideShowMenuItem_Click(object sender, EventArgs e)
        {
            //RefreshData();
            //RefreshData1(null, null);
            //_systemMode = SystemMode.Jira;
            _isShowFromTray = true;
            if (!_isVisible)
            {
                //RefreshData1(null, null);
                ShowMainForm();
                /*//ShowMainForm();
                //_systemMode = SystemMode.All;
                SuspendLayout();

                //RefreshData();
                RebuildLayout();
                ResumeLayout(true);

                WindowState = FormWindowState.Normal;
                
                BringToFront();
                Activate();

                _isVisible = true;
                UpdateTrayText();
                RefreshData();
                //Show();*/
            }
            else
            {

                // this.WindowState = FormWindowState.Normal;

                //RefreshData();
                //UpdateTrayText();
                HideMainForm();
                //Show();
            }
            //RefreshData();
        }

        private void RefreshData()
        {
            //SuspendLayout();
            MainTable.RefreshMainTableRows(Parameters);
            if (IsTimeUserTableVisible)
            {
                TimeUserTable.Refresh(this);
                //_dataLoaded = true;
                //dgvMainTable.Visible = true;
                //dgvTimeUserTable.Visible = IsTimeUserTableVisible;
                //dgvIdTasksTable.Visible = IsTimeUserTableVisible;
            }
            //_dataLoaded = true;
            dgvMainTable.Visible = true;
            dgvTimeUserTable.Visible = IsTimeUserTableVisible;
            dgvIdTasksTable.Visible = IsTimeUserTableVisible;

            SetFormSize();
            //GetMainFormPosition();
            //ResumeLayout(true);
        }

        private void SetFormSize()
        {
            dgvTimeUserTable.Location = new Point { X = 2 * dgvMainTable.Location.X + dgvMainTable.Width, Y = dgvMainTable.Location.Y };
            dgvTimeUserTable.Height = dgvMainTable.Height;

            dgvIdTasksTable.Location = new Point { X = dgvTimeUserTable.Location.X + dgvTimeUserTable.Width + dgvMainTable.Location.X, Y = dgvTimeUserTable.Location.Y };
            dgvIdTasksTable.Width = Math.Min(dgvIdTasksTable.Width, ClientRectangle.Width + dgvIdTasksTable.Left - 5);
            dgvIdTasksTable.Height = dgvTimeUserTable.Height;


            //Rectangle screenRectangle = this.RectangleToScreen(this.ClientRectangle);
            //int titleHeight = screenRectangle.Top - this.Top;

            var titleHeight = Height - ClientRectangle.Height;
            var titleWidth = Width - ClientRectangle.Width;

            var height = titleHeight + 2 * dgvMainTable.Location.Y + (dgvMainTable.Height-15);
            var width = titleWidth - 3 * dgvMainTable.Location.X - dgvMainTable.Width;

            if (IsTimeUserTableVisible)
            {
                width += 2 * dgvMainTable.Location.X + dgvTimeUserTable.Width + dgvIdTasksTable.Width;
            }

            //Width = width; //ширина
            //Height = height; //высота
            //MaximumSize = new Size(width, height);
            //MinimumSize = new Size(width, height);
            //MaximumSize = Size.Empty;
            MinimumSize = new Size(500, height);
            //MaximumSize = new Size(485, height);
            if (_dataLoaded) RebuildLayout();
        }

        public Rectangle GetMainFormPosition()
        {
            return new Rectangle(Location, Size);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                HideMainForm();
            }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Сохраняем состояние таблиц
            Properties.Settings.Default.TimeUserTableVisible = IsTimeUserTableVisible;
            Properties.Settings.Default.MainFormLocation = Location;
            Properties.Settings.Default.Save();

            if (!_allowRealClose && !_isRealExit)
            {
                //Отменяем закрытие формы
                e.Cancel = true;
                HideMainForm();
                return;
            }

            // Реальное закрытие - освобождаем ресурсы
            notifyIcon1.Visible = false;
            notifyIcon1?.Dispose();
            base.OnFormClosing(e);
        }

        /*protected override void OnResize(EventArgs e)
        { 
            base.OnResize(e);
            if(WindowState == FormWindowState.Minimized)
                {
                HideMainForm();
                }
            
        }*/


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowRealClose)
            {
                //Отменяем закрытие формы
                e.Cancel = true;
                HideMainForm();
                return;
            }
            SaveCurrentLocation();
            //if (notifyIcon1 != null)
            //{
            //    notifyIcon1.Visible = false;
            //    notifyIcon1.Dispose();
            //}
        }

        public void SaveCurrentLocation()
        {
            Point point = this.WindowState != FormWindowState.Minimized
                ? this.Location
                : RestoreBounds.Location;

            // this.DesktopBounds - содержит Rectangle формы, если она не свёрнута
            var rect = new Rectangle(point, this.Size);
            if (!IsVisibleOnAnyScreen(rect))
            {
                return;
            }

            if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.MainFormLocation = Location;
            }
            else
            {
                Properties.Settings.Default.MainFormLocation = RestoreBounds.Location;
            }
            //Properties.Settings.Default.MainFormLocation = point;
            Properties.Settings.Default.Save();
        }

        private bool IsVisibleOnAnyScreen(Rectangle rect)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(rect))
                {
                    var newRectangle = Rectangle.Intersect(screen.WorkingArea, rect);
                    if (newRectangle.Width >= this.Size.Width && newRectangle.Height >= this.Size.Height)
                        return true;
                }
            }

            return false;
        }

        private Rectangle ConstrainToScreen(Rectangle bounds)
        {
            Screen screen = Screen.FromRectangle(bounds);
            Rectangle workingArea = screen.WorkingArea;
            int width = Math.Min(bounds.Width, workingArea.Width);
            int height = Math.Min(bounds.Height, workingArea.Height);
            // mmm....minimax            
            int left = Math.Min(workingArea.Right - width, Math.Max(bounds.Left, workingArea.Left));
            int top = Math.Min(workingArea.Bottom - height, Math.Max(bounds.Top, workingArea.Top));
            return new Rectangle(left, top, width, height);
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            if (!_isShowFromTray) return;
            //Hide();
            IsTestLocation = true;
            if (Properties.Settings.Default.DontLostFocusCollapse)
            {
                ShowInTaskbar = true;
                return;
            }
            if (WindowState != FormWindowState.Minimized && !MainTable.IsDetailFormShow)
            {
                HideShowMenuItem_Click(sender, e);
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            HideShowMenuItem_Click(sender, e);
        }

        public void GetPrm()
        {
            if (!IsDataLoaded) return;
            InitParamsAndMainTable();
            if (Properties.Settings.Default.NotifyIS)
            //(sysMd == SystemMode.IS) ()
            {
                var param3TasksWithoutExecutor = new Param3TasksWithoutExecutor();
                CurrTasksFor3 = param3TasksWithoutExecutor.Get()[0];
                //var param3 = Param3TasksWithoutExecutor.Get()[0];
                //var jiraparam3TasksWithoutExecutor = new JiraParam3TasksWithoutExecutor();
                //var comboparam3TasksWithoutExecutor = new CombinedParam3TasksWithoutExecutor();
                TrayNotification = new TrayNotification(this, notifyIcon1, param3TasksWithoutExecutor, null, null, _systemMode);

            }
            else if (Properties.Settings.Default.NotifyJira)
            //(sysMd == SystemMode.Jira)
            {

                //var param3TasksWithoutExecutor = new Param3TasksWithoutExecutor();
                var jiraparam3TasksWithoutExecutor = new JiraParam3TasksWithoutExecutor();
                CurrTasksFor3 = jiraparam3TasksWithoutExecutor.Get()[0];
                //var comboparam3TasksWithoutExecutor = new CombinedParam3TasksWithoutExecutor();
                TrayNotification = new TrayNotification(this, notifyIcon1, null, jiraparam3TasksWithoutExecutor, null, _systemMode);
            }
            else  //SettingsForm.RadioButton3.Checked// All
            {
                //var param3TasksWithoutExecutor = new Param3TasksWithoutExecutor();
                //var jiraparam3TasksWithoutExecutor = new JiraParam3TasksWithoutExecutor();
                var comboparam3TasksWithoutExecutor = new CombinedParam3TasksWithoutExecutor();

                CurrTasksFor3 = comboparam3TasksWithoutExecutor.Get()[0];
                TrayNotification = new TrayNotification(this, notifyIcon1, null, null, comboparam3TasksWithoutExecutor, _systemMode);
            }

            //radioButton2.Checked = true;
            RefreshData1(null, null);
        }
        /*public ParamResult GetParamResult(ParamResult key)
        {
            return Parameters
              .SelectMany(p => p.Get()).FirstOrDefault(r => r.ParamResult == key);
        }*/
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {

            /*if (!_dataLoaded) return;
            RebuildLayout();*/
            if (IsTestLocation)
            {
                Location = GetVisibleLocation(Location);
                IsTestLocation = false;
            }

        }

        private void dgvIdTasksTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            //typeof(DataGridView).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(dgvTimeUserTable, null, null);
            /*for (int i = 0; i < dgvIdTasksTable.Rows.Count; i++)
            {
                if (dgvIdTasksTable.Rows[i].Cells["System"].Value != null)
                {
                    if (dgvIdTasksTable.Rows[i].Cells["Id"].Value != null)
                    {
                        string taskId = (string)dgvIdTasksTable.Rows[i].Cells["Id"].Value;
                        string sys = (string)dgvIdTasksTable.Rows[i].Cells["System"].Value;
                        var process = OpenInToBrowser(taskId, sys);
                        if (process != null && IsPossiblyMakeRead)
                        {
                            Timer = new Timer
                            {
                                Interval = 1500
                            };
                            Timer.Tick += new EventHandler(Refresh);
                            Timer.Enabled = true;
                            Timer.Start();
                        }
                    }
                }
            }*/
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                // открываем задачу в браузере
                //var item = (FieldsDetailInfo)DgvDetailTable.Rows[e.RowIndex].Tag;
                //var process = OpenInToBrowser(item);

                string taskId = (string)dgvIdTasksTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                string sys = (string)dgvIdTasksTable.Rows[e.RowIndex].Cells["System"].Value;
                var process = OpenInToBrowser(taskId, sys);
                if (process != null && IsPossiblyMakeRead)
                {
                    Timer = new Timer
                    {
                        Interval = 1500
                    };
                    Timer.Tick += new EventHandler(Refresh);
                    Timer.Enabled = true;
                    Timer.Start();
                }
            }
            dgvIdTasksTable.ClearSelection();
        }

        private Process OpenInToBrowser(string taskId, string sys)
        {
            string url;
            if (sys == "Jira")
            {
                url = $"http://{Properties.Settings.Default.AddressOfServerJira}/browse/{taskId}";
            }
            else
            {
                url = $"https://{Properties.Settings.Default.AddressOfServerIntraService}/Task/View/{taskId}";
            }

            var pathToBrowser = GetPathToBrowser();
            return Process.Start(pathToBrowser, url);
        }

        private SystemMode _lastSystemMode;
        private string GetPathToBrowser()
        {
            var pathToBrowser = Properties.Settings.Default.PathToBrowser;
            if (File.Exists(pathToBrowser) && Path.GetFileName(pathToBrowser).EndsWith(".exe"))
            {
                // на моём компьютере Chrome находится:
                //const string path = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                return pathToBrowser;
            }

            // открываем в браузере по умолчанию
            return "explorer";
        }
        private void Refresh(object sender, EventArgs eventArgs)
        {
            Timer.Stop();
            RefreshTable(true);
        }

        private void RefreshTbl()
        {
            RefreshTable(true);
        }

        private void rbAllUsers_CheckedChanged(object sender, EventArgs e)
        {

            //if (!((RadioButton)sender).Checked) return;
            //_systemMode = SystemMode.IS; 
            //UpdateMainCounters();

            if (rbAllUsers.Checked)
            {
                _systemMode = SystemMode.IS;
                //NeedRender = false;
                //ResizeMainForm();
                RefreshData(null, null);
            }
        }

        private void dgvMainTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            //typeof(DataGridView).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(dgvTimeUserTable, null, null);
        }


        private void timerRefresh_Tick(object sender, EventArgs e)
        {

        }

        public void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                _systemMode = SystemMode.Jira;
                //NeedRender = false;
                //ResizeMainForm();
                RefreshData(null, null);
            }
        }

        public void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                _systemMode = SystemMode.All;
                //NeedRender = false;
                //ResizeMainForm();
                RefreshData(null, null);
            }
        }

        public enum SystemMode
        {
            IS,
            Jira,
            All
        }

        private SystemMode _systemMode = SystemMode.All;

        private void dgvTimeUserTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            //typeof(DataGridView).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(dgvTimeUserTable, null, null);
        }
    }
}
