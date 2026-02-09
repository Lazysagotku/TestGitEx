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

        public FieldsIdTasksUserInfo[] GetData(string curDate)
        {
            if (_mf.RadioButton1.Checked)
                return JiraTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray();

            if (_mf.RadioButton2.Checked)
            {
                var jira = JiraTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray() ?? Array.Empty<FieldsIdTasksUserInfo>(); ;
                var isTasks = userTasksRepo.GetIdTasksUserOnCurDay(curDate) ?? Array.Empty<FieldsIdTasksUserInfo>();

                return jira.Concat(isTasks).ToArray();
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


            DgvIdTasksUserTable.DefaultCellStyle.WrapMode = DataGridViewTriState.True; //  несколько строк в ячейке
            DgvIdTasksUserTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            DgvIdTasksUserTable.AllowUserToAddRows = false; //запрещаем пользователю самому добавлять строки
            ShowFormHeaders();
            //dgvIdTasksUserTable.ResumeLayout(false);
            //_mf.ResumeLayout(true);
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
                Width = 70,
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
                Width = 200, //ширина колонки
                ReadOnly = true, //значение в этой колонке нельзя править
                Name = "Name", //текстовое имя колонки, его можно использовать вместо обращений по индексу
                Frozen = true, //флаг, что данная колонка всегда отображается на своем месте
                CellTemplate = new DataGridViewTextBoxCell(), //тип нашей колонки
            };
            DgvIdTasksUserTable.Columns.Add(column4);

            /*var column5 = new DataGridViewColumn
            {
                HeaderText = "Jira задача", //текст в шапке
                Width = 70, //ширина колонки
                ReadOnly = true, //значение в этой колонке нельзя править
                Name = "TaskJira", //текстовое имя колонки, его можно использовать вместо обращений по индексу
                Frozen = true, //флаг, что данная колонка всегда отображается на своем месте
                CellTemplate = new DataGridViewTextBoxCell(), //тип нашей колонки
            };
            DgvIdTasksUserTable.Columns.Add(column5);*/


            DgvIdTasksUserTable.AllowUserToAddRows = false; //запрещаем пользователю самому добавлять строки

            // Высота таблицы будет установлена в MainForm равной высоте основной таблицы;
            //DgvIdTasksUserTable.ScrollBars = ScrollBars.Vertical;
            DgvIdTasksUserTable.Width = GetDgvIdTasksUserTableWidht() + SystemInformation.VerticalScrollBarWidth;
            
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

            var  cell = DgvIdTasksUserTable.Rows[row].Cells[0];
            cell.Style.ForeColor = System.Drawing.Color.Blue;
            cell.Style.Font = new System.Drawing.Font(DgvIdTasksUserTable.Font, FontStyle.Underline);
        }
        public void Refresh(string curDate)
        {
            DgvIdTasksUserTable.Visible = false;
            _mf.SuspendLayout();
            DgvIdTasksUserTable.SuspendLayout();
            //MainForm.LoadIdTasksDataAsync(curDate);
            try
            {
                DgvIdTasksUserTable.Rows.Clear();
                if (_mf.RadioButton1.Checked)
                {
                    var items = JiraTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray() ?? Array.Empty<FieldsIdTasksUserInfo>();
                    foreach (var item in items)
                        AddRow(item);
                }
                else if (_mf.RadioButton2.Checked)
                {
                    var jira = JiraTasksRepo.GetIdTasksUserOnCurDay(curDate) ?? Enumerable.Empty<FieldsIdTasksUserInfo>();
                    var isys = userTasksRepo.GetIdTasksUserOnCurDay(curDate) ?? Enumerable.Empty<FieldsIdTasksUserInfo>();
                    foreach (var item in jira.Concat(isys))
                        AddRow(item);
                }
                else
                {
                    var items = userTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray() ?? Array.Empty<FieldsIdTasksUserInfo>();
                    foreach (var item in items)
                        AddRow(item);
                }

                /*if (items == null)
                        return;

                    DgvIdTasksUserTable.Rows.Clear();
                    for (int i = 0; i < items.Length; ++i)
                    {
                        var item = items[i];
                        DgvIdTasksUserTable.Rows.Add(item.TaskJira, item.System, item.Minutes, item.Name, item.IdTask);
                        DgvIdTasksUserTable.Rows[i].Cells["Id"].ToolTipText = item.Name;

                        var font = DgvIdTasksUserTable.Font;
                        FontStyle fontStyle = font.Style;
                        DgvIdTasksUserTable.Rows[i].Cells[0].Style.Font = new System.Drawing.Font(font, fontStyle | FontStyle.Underline);
                        DgvIdTasksUserTable.Rows[i].Cells[0].Style.ForeColor = System.Drawing.Color.Blue;
                    }
                }
                else if (_mf.RadioButton2.Checked)
                {
                    var btnJira = JiraTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray();
                    //?? Array.Empty<FullFieldsTaskInfo>(); // добавил
                    var btnIs = userTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray();
                    //?? Array.Empty<FullFieldsTaskInfo>(); // добавил

                    DgvIdTasksUserTable.SuspendLayout(); // добавил
                    DgvIdTasksUserTable.Rows.Clear();

                    var allTasks = btnJira
                      .Concat(btnIs)
                      .GroupBy(x => x.System == "Jira" ? x.TaskJira : x.IdTask) // добавил
                          .Select(g => g.First()) // добавил
                          .ToList();

                    var font = DgvIdTasksUserTable.Font;

                    foreach (var item in allTasks) // добавил
                    {
                        int rowIndex = DgvIdTasksUserTable.Rows.Add( // добавил
                                item.System == "Jira" ? item.TaskJira : item.IdTask,
                          item.System,
                          item.Minutes,
                          item.Name,
                          item.System == "Jira" ? item.IdTask : item.TaskJira
                        );

                        var cell = DgvIdTasksUserTable.Rows[rowIndex].Cells[0]; // добавил
                        cell.ToolTipText = item.Name;
                        cell.Style.Font = new System.Drawing.Font(font, FontStyle.Underline); // добавил
                        cell.Style.ForeColor = System.Drawing.Color.Blue; // добавил
                    }

                    DgvIdTasksUserTable.ClearSelection();
                }
                else
                {
                    //  MainForm.UserName jiraTasksRepo.GetIdTasksUserOnCurDay SystemMode sysMd
                    var items = userTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray();
                    if (items == null)
                        return;

                    DgvIdTasksUserTable.Rows.Clear();
                    for (int i = 0; i < items.Length; ++i)
                    {
                        var item = items[i];
                        DgvIdTasksUserTable.Rows.Add(item.IdTask, item.System, item.Minutes, item.Name);
                        DgvIdTasksUserTable.Rows[i].Cells["Id"].ToolTipText = item.Name;

                        var font = DgvIdTasksUserTable.Font;
                        FontStyle fontStyle = font.Style;
                        DgvIdTasksUserTable.Rows[i].Cells[0].Style.Font = new System.Drawing.Font(font, fontStyle | FontStyle.Underline);
                        DgvIdTasksUserTable.Rows[i].Cells[0].Style.ForeColor = System.Drawing.Color.Blue;
                    }
                }*/
            }
            finally
            {
                DgvIdTasksUserTable.ResumeLayout(false);
                _mf.ResumeLayout(true);
                DgvIdTasksUserTable.Visible = true;
            }

            //_mf = new MainForm();
            /*var curBtn = _mf.RadioButton1;
            var curBtn2 = _mf.RadioButton2;

            //_ = MainForm.CurrentSystemMode; // mainform null
            //var curSys = radioButton1;

            //SystemMode curSystem = new MainForm.SystemMode();
            //var curSys = curSystem;
            if (curBtn.Checked)
            {
                var items = JiraTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray();
                if (items == null)
                    return;

                DgvIdTasksUserTable.Rows.Clear();
                for (int i = 0; i < items.Length; ++i)
                {
                    var item = items[i];
                    DgvIdTasksUserTable.Rows.Add(item.TaskJira, item.System, item.Minutes, item.Name, item.IdTask);
                    DgvIdTasksUserTable.Rows[i].Cells["Id"].ToolTipText = item.Name;

                    var font = DgvIdTasksUserTable.Font;
                    FontStyle fontStyle = font.Style;
                    DgvIdTasksUserTable.Rows[i].Cells[0].Style.Font = new System.Drawing.Font(font, fontStyle | FontStyle.Underline);
                    DgvIdTasksUserTable.Rows[i].Cells[0].Style.ForeColor = System.Drawing.Color.Blue;
                }
            }
            else if (curBtn2.Checked)
            {
                var btnJira = JiraTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray();
                       //?? Array.Empty<FullFieldsTaskInfo>(); // добавил
                var btnIs = userTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray();
                    //?? Array.Empty<FullFieldsTaskInfo>(); // добавил

                DgvIdTasksUserTable.SuspendLayout(); // добавил
                DgvIdTasksUserTable.Rows.Clear();

                var allTasks = btnJira
                  .Concat(btnIs)
                  .GroupBy(x => x.System == "Jira" ? x.TaskJira : x.IdTask) // добавил
                      .Select(g => g.First()) // добавил
                      .ToList();

                var font = DgvIdTasksUserTable.Font;

                foreach (var item in allTasks) // добавил
                {
                    int rowIndex = DgvIdTasksUserTable.Rows.Add( // добавил
                            item.System == "Jira" ? item.TaskJira : item.IdTask,
                      item.System,
                      item.Minutes,
                      item.Name,
                      item.System == "Jira" ? item.IdTask : item.TaskJira
                    );

                    var cell = DgvIdTasksUserTable.Rows[rowIndex].Cells[0]; // добавил
                    cell.ToolTipText = item.Name;
                    cell.Style.Font = new System.Drawing.Font(font, FontStyle.Underline); // добавил
                    cell.Style.ForeColor = System.Drawing.Color.Blue; // добавил
                }

                DgvIdTasksUserTable.ClearSelection();
            }
            else
            {
                //  MainForm.UserName jiraTasksRepo.GetIdTasksUserOnCurDay SystemMode sysMd
                var items = userTasksRepo.GetIdTasksUserOnCurDay(curDate)?.ToArray();
                if (items == null)
                    return;

                DgvIdTasksUserTable.Rows.Clear();
                for (int i = 0; i < items.Length; ++i)
                {
                    var item = items[i];
                    DgvIdTasksUserTable.Rows.Add(item.IdTask, item.System, item.Minutes, item.Name);
                    DgvIdTasksUserTable.Rows[i].Cells["Id"].ToolTipText = item.Name;

                    var font = DgvIdTasksUserTable.Font;
                    FontStyle fontStyle = font.Style;
                    DgvIdTasksUserTable.Rows[i].Cells[0].Style.Font = new System.Drawing.Font(font, fontStyle | FontStyle.Underline);
                    DgvIdTasksUserTable.Rows[i].Cells[0].Style.ForeColor = System.Drawing.Color.Blue;
                }
            }*/

        }


        private int GetDgvIdTasksUserTableWidht()
        {
            int totalColWidth = 1;
            foreach (DataGridViewColumn col in DgvIdTasksUserTable.Columns)
            {
                totalColWidth += col.Width;
            }

            return totalColWidth;
        }

        private void DgvIdTasksUserTable_SelectionChanged(object sender, EventArgs e)
        {
            DgvIdTasksUserTable.SelectionChanged -= DgvIdTasksUserTable_SelectionChanged;
            DgvIdTasksUserTable.ClearSelection();
            DgvIdTasksUserTable.SelectionChanged += DgvIdTasksUserTable_SelectionChanged;
        }
    }
}

