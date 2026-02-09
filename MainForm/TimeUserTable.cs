using DocumentFormat.OpenXml.Drawing.Charts;
using Org.BouncyCastle.Cms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TimeReportV3.Params;
using TimeReportV3.Repository;
using TrfCommonUtility;
using static TimeReportV3.MainForm;

namespace TimeReportV3
{
    public class FieldsTimeUserInfo
    {
        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Время в минутах
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Подкрашиваем красным цветом те задачи, время на которые пользователь списал в выходные дни
        /// </summary>
        public bool IsRed { get; set; }
    }

    internal sealed class TimeUserTable
    {
        private readonly Action _onShowIdTasks;
        private CustomDataGridView DgvTimeUserTable;
        private IdTasksUserTable IdTasksUserTable;
        private WorkingDays WorkingDays;
        private readonly UserTasksRepo userTasksRepo = new UserTasksRepo();
        private readonly JiraTasksRepo jiraTasksRepo;
        private MainForm _mf;
        private SystemMode curSys { get; set; }

        public void OnDateSelected(string date)
        {
            IdTasksUserTable.Refresh(date);
        }

        public TimeUserTable(CustomDataGridView dgvTimeUserTable, CustomDataGridView dvgIdTasksUserTable, Action onShowIdTasks, MainForm mf)
        {
            _mf = mf;
            DgvTimeUserTable = dgvTimeUserTable;
            IdTasksUserTable = new IdTasksUserTable(dvgIdTasksUserTable, mf);
            DgvTimeUserTable.Visible = false;
            _mf.SuspendLayout();
            DgvTimeUserTable.SuspendLayout();
            _onShowIdTasks = onShowIdTasks;
            WorkingDays = WorkingDays.GetInstance();

            DgvTimeUserTable.RowHeadersVisible = false;
            DgvTimeUserTable.RowEnter -= new DataGridViewCellEventHandler(DgvTimeUserTable_RowEnter);
            DgvTimeUserTable.RowEnter += new DataGridViewCellEventHandler(DgvTimeUserTable_RowEnter);

            DgvTimeUserTable.AllowUserToResizeRows = false;
            DgvTimeUserTable.ScrollBars = ScrollBars.Vertical;

            //DgvDetailTable.AllowUserToResizeColumns = true;
            DgvTimeUserTable.DefaultCellStyle.WrapMode = DataGridViewTriState.True; // разрешить несколько строк в ячейке
            DgvTimeUserTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            DgvTimeUserTable.AllowUserToAddRows = false; //запрещаем пользователю самому добавлять строки

            //DgvTimeUserTable.CellClick += new DataGridViewCellEventHandler(DgvDetailTable_CellClick);
            ShowFormHeaders();
            DgvTimeUserTable.ResumeLayout(false);
            _mf.ResumeLayout(true);
            DgvTimeUserTable.Visible = true;
        }

        public void DgvTimeUserTable_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            /*if (e.RowIndex >= 0)
            {
                var colName = DgvTimeUserTable.Columns[0].Name;
                // получаем дату в формате yyyy - MM - dd
                var curDate = DgvTimeUserTable.Rows[e.RowIndex].Cells[colName].Value.ToString();
                IdTasksUserTable.Refresh(curDate);
                dgvIdTasksUserTable.Visible = true;
                MainForm.RebuildLayout();


            }*/
            if (e.RowIndex < 0)
                return;

            var curDate = DgvTimeUserTable.Rows[e.RowIndex].Cells["Date"].Value.ToString();
            //DgvTimeUserTable.Visible = false;
            IdTasksUserTable.Refresh(curDate);
            _onShowIdTasks?.Invoke();
            //DgvTimeUserTable.Visible = true;
        }

        private void ShowFormHeaders()
        {
            var column1 = new DataGridViewColumn
            {
                HeaderText = "Дата", //текст в шапке
                Width = 70, //ширина колонки
                ReadOnly = true, //значение в этой колонке нельзя править
                Name = "Date", //текстовое имя колонки, его можно использовать вместо обращений по индексу
                Frozen = true, //флаг, что данная колонка всегда отображается на своем месте
                CellTemplate = new DataGridViewTextBoxCell(), //тип нашей колонки
            };
            column1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvTimeUserTable.Columns.Add(column1);

            var column2 = new DataGridViewColumn
            {
                HeaderText = "Минуты",
                Width = 70,
                ReadOnly = true,
                Name = "Minutes",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvTimeUserTable.Columns.Add(column2);

            DgvTimeUserTable.AllowUserToAddRows = false; //запрещаем пользователю самому добавлять строки

            // Высота таблицы будет установлена в MainForm равной высоте основной таблицы;
            //DgvTimeUserTable.Width = GetDgvTimeUserTableWidht() + SystemInformation.VerticalScrollBarWidth;
            DgvTimeUserTable.Height = 200;// - SystemInformation.VerticalScrollBarWidth;
            DgvTimeUserTable.Refresh();
            DgvTimeUserTable.ClearSelection();
        }

        private FieldsTimeUserInfo[] _cachedTimeUserAll;
        private FieldsIdTasksUserInfo _cachedTasksAll;
        private bool _cacheValid = false;
        public void Refresh(MainForm mnfrm)
        {
            FieldsTimeUserInfo[] items; 

            if (mnfrm.CurrentSystemMode == SystemMode.All)
            {
                if (!_cacheValid) 
                {
                    _cachedTimeUserAll = mnfrm.LoadTimeUserAll(); 
                    _cacheValid = true;                     
                }

                items = _cachedTimeUserAll;                 
            }
            else
            {
                items = GetTimeUserOnLastWorkingdays();     
            }

            if (items == null || items.Length == 0)
                return;

            mnfrm.SuspendLayout();                                
            DgvTimeUserTable.SuspendLayout();               

            DgvTimeUserTable.Rows.Clear();

            for (int i = 0; i < items.Length; ++i)
            {
                var item = items[i];
                DgvTimeUserTable.Rows.Add(
                  $"{item.Date:yyyy-MM-dd}",
                  item.Minutes
                );

                var color = item.IsRed ? Color.Red : Color.Black;

                for (int j = 0; j < DgvTimeUserTable.Columns.Count; j++)
                {
                    DgvTimeUserTable.Rows[i].Cells[j].Style.ForeColor = color;
                }
            }

            DgvTimeUserTable.ClearSelection();

            DgvTimeUserTable.ResumeLayout(false);            
            mnfrm.ResumeLayout(true);                              
        }
         public void Refresh1(MainForm mnfrm)
        {
            mnfrm.SuspendLayout();
            DgvTimeUserTable.SuspendLayout();
            //add checked system IS||Jira||All
            var items = GetTimeUserOnLastWorkingdays();
             // первоначально использовалась функция GetTimeUserOnLast30days();
             if (items == null)
                 return;

             DgvTimeUserTable.Rows.Clear();
             //DgvTimeUserTable.Columns.Clear();
             // если по мимо цвета у выделенных строк нужно ещё поменять стиль, то достаточно раскомментировать строки в этом методе
             //var font = DgvTimeUserTable.Font;

             for (int i = 0; i < items.Length; ++i)
             {
                 //FontStyle fontStyle;
                 Color color;
                 var item = items[i];
                 DgvTimeUserTable.Rows.Add($"{item.Date:yyyy-MM-dd}", item.Minutes);
                 if (item.IsRed)
                 {
                     //font = Font.Italic;
                     color = Color.Red;
                 }
                 else
                 {
                     //font = System.Drawing.FontStyle.Regular;
                     color = Color.Black;
                 }
                 //var fontLine = new Font(font, fontStyle);

                 for (int j = 0; j < DgvTimeUserTable.Columns.Count; j++)
                 {
                     //DgvTimeUserTable.Rows[i].Cells[j].Style.Font = fontLine;
                     DgvTimeUserTable.Rows[i].Cells[j].Style.ForeColor = color;
                 }
             }
            DgvTimeUserTable.ClearSelection();

            DgvTimeUserTable.ResumeLayout(false);
            mnfrm.ResumeLayout(true);
        }


        public FieldsTimeUserInfo[] GetTimeUserOnLastWorkingdays()
        {
            var curBtn = _mf.RadioButton1;
            var curBtn2 = _mf.RadioButton2;
            var workingDays = WorkingDays.Get();
            if (workingDays?.Count() == 0)
                return null;

            string firstDay = workingDays.Last();
            string line = string.Join(", ", workingDays.Select(wd => $"(cast('{wd}' as date))"));
            if (curBtn.Checked)
            {
                return JiraTasksRepo.GetTimeUserOnLastWorkingDays().ToArray();
            }
            else if (curBtn2.Checked)
            {
                var qJira = JiraTasksRepo.GetTimeUserOnLastWorkingDays()?.ToArray()
                      ?? Array.Empty<FieldsTimeUserInfo>(); 
                var qIs = userTasksRepo.GetTimeUserOnLastWorkingdays(line, firstDay)?.ToArray()
                   ?? Array.Empty<FieldsTimeUserInfo>(); 

                var result = qJira
                .Concat(qIs) 
                    .GroupBy(x => x.Date)
                    .Select(g => new FieldsTimeUserInfo 
                    {
                        Date = g.Key,
                        Minutes = g.Sum(x => x.Minutes) 
                    })
                .OrderByDescending(x => x.Date)
                .ToArray();

                return result;
            }
            /*else if (curBtn2.Checked)
            {
                var qJira = JiraTasksRepo.GetTimeUserOnLastWorkingDays().ToArray();
                var qIs = userTasksRepo.GetTimeUserOnLastWorkingdays(line, firstDay).ToArray();


                List<FieldsTimeUserInfo> resultList = new List<FieldsTimeUserInfo>();
                if (qJira.Length > qIs.Length)
                {
                    for (int i = 0; i < qJira.Length; i++)
                    {
                        var item = qJira[i];
                        for (int j = 0; j < qIs.Length; j++)
                        {
                            var item1 = qIs[j];
                            if (item1.Date == item.Date)
                            {
                                var sumMin = item1.Minutes + item.Minutes;
                                item1.Minutes = sumMin;
                                resultList.Add(item1);
                            }
                        }
                        resultList.Add(item);

                    }
                }
                else
                {
                    for (int i = 0; i < qIs.Length; i++)
                    {
                        var item = qIs[i];
                        for (int j = 0; j < qJira.Length; j++)
                        {
                            var item1 = qJira[j];
                            if (item1.Date == item.Date)
                            {
                                var sumMin = item1.Minutes + item.Minutes;
                                item1.Minutes = sumMin;
                            }

                            resultList.Add(item1);
                        }
                        resultList.Add(item);

                    }
                }



                foreach ( var j in qIs)
                {
                    foreach ( var j2 in qJira)
                    {
                        if (j.Date.Date == j2.Date.Date)
                        {
                            var sumMin = j.Minutes + j2.Minutes;
                            j2.Minutes= sumMin;

                        }

                    }
                    resultList.Add(j);
                }*/
            //return resultList.ToArray();

            //}
            else
            {
                return userTasksRepo.GetTimeUserOnLastWorkingdays(line, firstDay).ToArray();
            }

        }

        private int GetDgvTimeUserTableWidht()
        {
            int totalColWidth = 0;
            foreach (DataGridViewColumn col in DgvTimeUserTable.Columns)
            {
                totalColWidth += col.Width;
            }

            return totalColWidth + 3;
        }
    }
}

