using System;
using System.Linq;
using System.Windows.Forms;
using TrfCommonUtility;
using System.Data;
using System.Drawing;
using TimeReportV3.Repository;
using static TimeReportV3.MainForm;
using NPOI.OpenXmlFormats.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace TimeReportV3
{
    public class FieldsIdTasksUserInfo
    {
        /// <summary>
        /// Дата
        /// </summary>
        public string IdTask { get; set; }

        /// <summary>
        /// Время в минутах
        /// </summary>
        public int Minutes { get; set; }

        public string Name { get; set; }

        public string System { get; set; }
        public string TaskJira { get; set; }

    }

    internal class IdTasksUserTable
    {
        private CustomDataGridView DgvIdTasksUserTable;
        private readonly UserTasksRepo userTasksRepo = new UserTasksRepo();
        private readonly JiraTasksRepo jiraTasksRepo;
        private SystemMode curSys { get; set; }
        private MainForm _mf;
        private bool _isLoading = false;

        public FieldsIdTasksUserInfo[] GetData(string curDate)
        {

            if (_mf.RadioButton1.Checked)
                return JiraTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray();

            if (_mf.RadioButton2.Checked)
            {
                var jiraTask = Task.Run(() => JiraTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray() ?? Array.Empty<FieldsIdTasksUserInfo>()); ;
                var isTasks = Task.Run(() => userTasksRepo.GetIdTasksUserOnCurDay(curDate) ?? Array.Empty<FieldsIdTasksUserInfo>());
               

                Task.WaitAll(isTasks, jiraTask);  // Параллельное выполнение!

                var isResult = isTasks.Result;
                var jiraResult = jiraTask.Result;

                return jiraResult.Concat(isResult).ToArray();
            }

            return userTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray();
        }


        public void Render(FieldsIdTasksUserInfo[] items)
        {
            if (items == null)
                return;

            DgvIdTasksUserTable.Rows.Clear();

            var font = DgvIdTasksUserTable.Font;

            foreach (var item in items)
            {
                int rowIndex = DgvIdTasksUserTable.Rows.Add(
                    item.System == "Jira" ? item.TaskJira : item.IdTask,
                    item.System,
                    item.Minutes,
                    item.Name
                );

                DgvIdTasksUserTable.Rows[rowIndex].Tag = item;

                var cell = DgvIdTasksUserTable.Rows[rowIndex].Cells[0];
                cell.Style.Font = new System.Drawing.Font(font, FontStyle.Underline);
                cell.Style.ForeColor = System.Drawing.Color.Blue;
            }

            DgvIdTasksUserTable.ClearSelection();
        }
        public IdTasksUserTable(CustomDataGridView dgvIdTasksUserTable, MainForm mf)
        {
            _mf = mf;
            DgvIdTasksUserTable = dgvIdTasksUserTable;
            dgvIdTasksUserTable.Visible = false;
            //_mf.SuspendLayout();
            //dgvIdTasksUserTable.SuspendLayout();
            DgvIdTasksUserTable.RowHeadersVisible = false;
            DgvIdTasksUserTable.SelectionChanged += new EventHandler(DgvIdTasksUserTable_SelectionChanged);


            DgvIdTasksUserTable.AllowUserToResizeRows = true;
            DgvIdTasksUserTable.ScrollBars = ScrollBars.Vertical;

            //DgvIdTasksUserTable.Dock = DockStyle.Fill;
            DgvIdTasksUserTable.DefaultCellStyle.WrapMode = DataGridViewTriState.True; //  несколько строк в ячейке
            DgvIdTasksUserTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            DgvIdTasksUserTable.AutoResizeColumns();
            DgvIdTasksUserTable.AllowUserToAddRows = false; //запрещаем пользователю самому добавлять строки
            ShowFormHeaders();
            //dgvIdTasksUserTable.ResumeLayout(false);
            //_mf.ResumeLayout(true);
            _mf.RebuildLayout();
            dgvIdTasksUserTable.Visible = true;

        }

        private void ShowFormHeaders()
        {
            var column1 = new DataGridViewColumn
            {
                HeaderText = "Задача", //текст в шапке
                Width = 70, //ширина колонки
                ReadOnly = true, //значение в этой колонке нельзя править
                Name = "Id", //текстовое имя колонки, его можно использовать вместо обращений по индексу
                Frozen = true, //флаг, что данная колонка всегда отображается на своем месте
                CellTemplate = new DataGridViewTextBoxCell(), //тип нашей колонки
            };
            column1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvIdTasksUserTable.Columns.Add(column1);

            var column2 = new DataGridViewColumn
            {
                HeaderText = "Sys",
                Width = 30,
                ReadOnly = true,
                Name = "System",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvIdTasksUserTable.Columns.Add(column2);

            var column3 = new DataGridViewColumn
            {
                HeaderText = "Минуты",
                Width = 50,
                ReadOnly = true,
                Name = "Minutes",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvIdTasksUserTable.Columns.Add(column3);

            var column4 = new DataGridViewColumn
            {
                HeaderText = "Наименование", //текст в шапке
                Width = 185, //ширина колонки
                ReadOnly = true, //значение в этой колонке нельзя править
                Name = "Name", //текстовое имя колонки, его можно использовать вместо обращений по индексу
                Frozen = true, //флаг, что данная колонка всегда отображается на своем месте
                CellTemplate = new DataGridViewTextBoxCell(), //тип нашей колонки
            };
            DgvIdTasksUserTable.Columns.Add(column4);
            DgvIdTasksUserTable.Columns["Name"].Width -= SystemInformation.VerticalScrollBarWidth; 


            DgvIdTasksUserTable.AllowUserToAddRows = false; //запрещаем пользователю самому добавлять строки

            // Высота таблицы будет установлена в MainForm равной высоте основной таблицы;
            //DgvIdTasksUserTable.Width = GetDgvIdTasksUserTableWidht() + SystemInformation.VerticalScrollBarWidth;
            DgvIdTasksUserTable.ScrollBars = ScrollBars.Vertical;
            //DgvIdTasksUserTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            DgvIdTasksUserTable.BackgroundColor = System.Drawing.Color.White; // Белый фон
            //DgvIdTasksUserTable.Anchor = AnchorStyles.Bottom;
            //DgvIdTasksUserTable.ScrollButton = ScrollButton.Left;
            //DgvIdTasksUserTable.Height = 200;
            DgvIdTasksUserTable.Refresh();
            DgvIdTasksUserTable.ClearSelection();
        }

        private void AddRow(FieldsIdTasksUserInfo item)
        {
            int row = DgvIdTasksUserTable.Rows.Add(
                item.System == "Jira" ? item.TaskJira : item.IdTask,
                item.System,
                item.Minutes,
                item.Name,
                item.System == "Jira" ? item.IdTask : item.TaskJira);

            var cell = DgvIdTasksUserTable.Rows[row].Cells[0];
            cell.Style.ForeColor = System.Drawing.Color.Blue;
            cell.Style.Font = new System.Drawing.Font(DgvIdTasksUserTable.Font, FontStyle.Underline);
        }
        public void Refresh(string curDate)
        {
            if (_isLoading) return;

            // Проверяем, что curDate - это валидная дата, а не "Загрузка..."
            if (string.IsNullOrEmpty(curDate) || curDate == "Загрузка..." || !DateTime.TryParse(curDate, out _))
            {
                return;
            }

            RefreshAsync(curDate);
        }

        private async void RefreshAsync(string curDate)
        {
            _isLoading = true;
            DgvIdTasksUserTable.Visible = false;


            try
            {
                if (DgvIdTasksUserTable.Rows.Count == 0)
                {
                    ShowLoadingState();
                }

                _mf.SuspendLayout();

                FieldsIdTasksUserInfo[] items = await Task.Run(() => GetData(curDate));

                DgvIdTasksUserTable.Rows.Clear();

                if (items != null)
                {
                    foreach (var item in items)
                        AddRow(item);
                }

                _mf.ResumeLayout(true);
                DgvIdTasksUserTable.Visible = true;
                HideLoadingState();
            }
            finally
            {
                _isLoading = false;
            }
        }


        private int GetDgvIdTasksUserTableWidht()
        {
            int totalColWidth = 0;
            foreach (DataGridViewColumn col in DgvIdTasksUserTable.Columns)
            {
                totalColWidth += col.Width;
            }

            return totalColWidth + 3;
        }


        private void DgvIdTasksUserTable_SelectionChanged(object sender, EventArgs e)
        {
            DgvIdTasksUserTable.SelectionChanged -= DgvIdTasksUserTable_SelectionChanged;
            DgvIdTasksUserTable.ClearSelection();
            DgvIdTasksUserTable.SelectionChanged += DgvIdTasksUserTable_SelectionChanged;
        }

        private void ShowLoadingState()
        {
            if (DgvIdTasksUserTable.InvokeRequired)
            {
                DgvIdTasksUserTable.Invoke(new Action(ShowLoadingState));
                return;
            }

            DgvIdTasksUserTable.Rows.Clear();
            DgvIdTasksUserTable.Rows.Add("Загрузка...", "...", "", "");
            DgvIdTasksUserTable.Enabled = false;
        }

        private void HideLoadingState()
        {
            if (DgvIdTasksUserTable.InvokeRequired)
            {
                DgvIdTasksUserTable.Invoke(new Action(HideLoadingState));
                return;
            }

            DgvIdTasksUserTable.Enabled = true;
        }

    }
}

