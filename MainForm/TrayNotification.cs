using System;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;
using TimeReportV3.Params;
using TimeReportV3.Repository;
using ToastNotifications;
using static TimeReportV3.MainForm;

namespace TimeReportV3
{
    public enum TrayIconStatus { NotConnection, ErrorConnection, ShowTasksWithoutExecutor }
    internal class TrayNotification
    {
        private NotifyIcon NotifyIcon;
        private bool IsAllowShowDetailsFormWithTasksWithoutExecutor;
        private readonly MainForm MainForm;
        private Param3TasksWithoutExecutor Param3TasksWithoutExecutor;
        private JiraParam3TasksWithoutExecutor JiraParam3TasksWithoutExecutor;
        private CombinedParam3TasksWithoutExecutor CombinedParam3TasksWithoutExecutor;
        private DetailsForm DetailsForm;
        private SettingsForm SettingsForm;
        private ParamResult param;
        private readonly SoundAlert SoundAlert;

        private const int DISPLAY_TIME_SECOND = 10;
        private const string CAPTION_SQL_ERROR = "Cоединение с SQL сервером";
        private const string MESSAGE_SQL_ERROR = "Проверте, что SQL-пользователь создан!";

        private const string CAPTION = "Внимание!";

        public TrayNotification(MainForm mainForm, NotifyIcon notifyIcon, Param3TasksWithoutExecutor param3TasksWithoutExecutor, JiraParam3TasksWithoutExecutor jiraparam3TasksWithoutExecutor, CombinedParam3TasksWithoutExecutor comboparam3TasksWithoutExecutor, SystemMode sysMd)
        {
            MainForm = mainForm;
            //Properties.Settings.Default.Upgrade();
            NotifyIcon = notifyIcon;
            NotifyIcon.BalloonTipClicked += new System.EventHandler(NotifyIcon_BalloonTipClicked);
            //SettingsForm = SetForm;
            if (Properties.Settings.Default.NotifyIS)
            //(sysMd == SystemMode.IS) ()
            {
                Param3TasksWithoutExecutor = param3TasksWithoutExecutor;
            }
            if (Properties.Settings.Default.NotifyJira)
            //(sysMd == SystemMode.Jira)
            {
                JiraParam3TasksWithoutExecutor = jiraparam3TasksWithoutExecutor;
            }
            if (Properties.Settings.Default.NotifyAll)//SettingsForm.RadioButton3.Checked// All
            {
                CombinedParam3TasksWithoutExecutor = comboparam3TasksWithoutExecutor;
            }



            SoundAlert = new SoundAlert();
        }
        private readonly UserTasksRepo _repo = new UserTasksRepo();
        private readonly Param3TasksWithoutExecutor _isParam = new Param3TasksWithoutExecutor();
        private readonly JiraParam3TasksWithoutExecutor _jiraParam = new JiraParam3TasksWithoutExecutor();
        public void Show(TrayIconStatus status, int tasksWithoutExecutorCount)
        {
            string title;
            string body;
            ToolTipIcon toolTipIcon;


            if (status == TrayIconStatus.NotConnection)
            {
                title = CAPTION_SQL_ERROR;
                body = MESSAGE_SQL_ERROR;
                toolTipIcon = ToolTipIcon.Error;
                IsAllowShowDetailsFormWithTasksWithoutExecutor = false;
            }
            else if (status == TrayIconStatus.ErrorConnection)
            {
                title = CAPTION_SQL_ERROR;
                body = $"Запрос для параметра {Properties.Settings.Default.SelectionIndexDisplayInSystemTray} не удался!";
                toolTipIcon = ToolTipIcon.Error;
                IsAllowShowDetailsFormWithTasksWithoutExecutor = false;
            }
            else
            {
                if (tasksWithoutExecutorCount == 0 || !Properties.Settings.Default.NotifyAboutTasksWithoutЕxecutor)
                {
                    return;
                }
                if (Properties.Settings.Default.NotifyIS)
                {

                    var counts = _repo.GetCountTasksByStatus();
                    var count = counts[2];
                    tasksWithoutExecutorCount = count;
                }
                else if (Properties.Settings.Default.NotifyJira)
                {
                    var counts = JiraTasksRepo.GetTasksCounts(MainForm.UserName);
                    var value = counts.ElementAtOrDefault(2);
                    tasksWithoutExecutorCount = value;
                }
                else
                {
                    var isResult = _isParam.Get().First();
                    var jiraResult = _jiraParam.Get().First();
                    tasksWithoutExecutorCount = isResult.Count + jiraResult.Count;
                }
                //var tasksWithoutExecutorCount = Param3TasksWithoutExecutor.ParamResult.Count;
                //if (tasksWithoutExecutorCount == 0)
                //    return;

                //SoundAlert.Play(Properties.Settings.Default.PathToSoundAlertFile, false);
                title = CAPTION;
                body = $"Имеются задачи без исполнителя в количестве {tasksWithoutExecutorCount}";
                toolTipIcon = ToolTipIcon.Info;
                IsAllowShowDetailsFormWithTasksWithoutExecutor = true;
            }

            SoundAlert.Play(Properties.Settings.Default.PathToSoundAlertFile, false);
            if (Properties.Settings.Default.StandardPopupNotification)
            {
                NotifyIcon.ShowBalloonTip(DISPLAY_TIME_SECOND * 3000, title, body, toolTipIcon);
            }
            else
            {
                var toastNotification = new Notification
                (
                    title,
                    body,
                    DISPLAY_TIME_SECOND,
                    FormAnimator.AnimationMethod.Slide,
                    FormAnimator.AnimationDirection.Left,
                    ShowDetailsFormWithTasksWithoutExecutor
                );
                if (tasksWithoutExecutorCount != 0)
                {
                    toastNotification.Show();
                }
            }
        }


        //private readonly JiraParam3TasksWithoutExecutor _jiraParam;
        private void ShowDetailsFormWithTasksWithoutExecutorIS()
        {
            if (IsAllowShowDetailsFormWithTasksWithoutExecutor)
            {
                DetailsForm = new DetailsForm(Param3TasksWithoutExecutor.Get()[0], MainForm);
                MainForm.BeginInvoke(new Action(() =>
                {
                    DetailsForm.ShowDialog();
                    DetailsForm = null;
                    MainForm.RefreshData(null, null);
                }));
                /*if (MainForm.IsHandleCreated)
                {
                    MainForm.BeginInvoke(new Action(() =>
                    {
                        MainForm.RefreshData(null, null);
                    }));
                }*/
            }
        }

        private void ShowDetailsFormWithTasksWithoutExecutorJira()
        {
            if (IsAllowShowDetailsFormWithTasksWithoutExecutor)
            {
                DetailsForm = new DetailsForm(JiraParam3TasksWithoutExecutor.Get()[0], MainForm);
                MainForm.BeginInvoke(new Action(() =>
                {
                    DetailsForm.ShowDialog();
                    DetailsForm = null;
                    MainForm.RefreshData(null, null);
                }));
                /*if (MainForm.IsHandleCreated)
                {
                    MainForm.BeginInvoke(new Action(() =>
                    {
                        MainForm.RefreshData(null, null);
                    }));
                }*/
            }
        }
        private void ShowDetailsFormWithTasksWithoutExecutorAll()
        {
            if (IsAllowShowDetailsFormWithTasksWithoutExecutor)
            {
                DetailsForm = new DetailsForm(CombinedParam3TasksWithoutExecutor.Get()[0], MainForm);
                MainForm.BeginInvoke(new Action(() =>
                {
                    DetailsForm.ShowDialog();
                    DetailsForm = null;
                    MainForm.RefreshData(null, null);
                }));
            }
        }


        private void ShowDetailsFormWithTasksWithoutExecutor()
        {
            MainForm.GetPrm();

            
            if (IsAllowShowDetailsFormWithTasksWithoutExecutor)
            {

                if (Param3TasksWithoutExecutor != null)//&& SettingsForm.checkBox1) (!Properties.Settings.Default.NotifyIS)
                {
                    //DetailsForm = new DetailsForm(Param3TasksWithoutExecutor.Get()[0], MainForm);
                    MainForm.BeginInvoke(new Action(() =>
                    {
                        using(var form = new DetailsForm(Param3TasksWithoutExecutor.Get()[0], MainForm)) form.ShowDialog(MainForm);
                        //DetailsForm.ShowDialog();
                        DetailsForm = null;
                        MainForm.RefreshData1(null, null);
                    }));
                    if (MainForm.IsHandleCreated)
                    {
                        MainForm.BeginInvoke(new Action(() =>
                        {
                            MainForm.RefreshData1(null, null);
                        }));
                    }

                }
            
            if (JiraParam3TasksWithoutExecutor != null)//(!Properties.Settings.Default.NotifyJira)
            {

                //DetailsForm = new DetailsForm(JiraParam3TasksWithoutExecutor.Get()[0], MainForm);
                    MainForm.BeginInvoke(new Action(() =>
                    {
                        using (var form = new DetailsForm(JiraParam3TasksWithoutExecutor.Get()[0], MainForm)) form.ShowDialog(MainForm);
                        //DetailsForm.ShowDialog();
                        DetailsForm = null;
                        MainForm.RefreshData1(null, null);
                    }));
                    if (MainForm.IsHandleCreated)
                {
                    MainForm.BeginInvoke(new Action(() =>
                    {
                        MainForm.RefreshData1(null, null);
                    }));
                }
            }
            if (CombinedParam3TasksWithoutExecutor != null)// (!Properties.Settings.Default.NotifyAll)
            {
                //DetailsForm = new DetailsForm(CombinedParam3TasksWithoutExecutor.Get()[0], MainForm);
                    MainForm.BeginInvoke(new Action(() =>
                    {
                        using (var form = new DetailsForm(CombinedParam3TasksWithoutExecutor.Get()[0], MainForm)) form.ShowDialog(MainForm);
                        //DetailsForm.ShowDialog();
                        DetailsForm = null;
                        MainForm.RefreshData1(null, null);
                    }));

                    if (MainForm.IsHandleCreated)
                {
                    MainForm.BeginInvoke(new Action(() =>
                   {
                        MainForm.RefreshData1(null, null);
                   }));
                }
            }
               }

            }

    

        private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            ShowDetailsFormWithTasksWithoutExecutor();
        }
    }
}

