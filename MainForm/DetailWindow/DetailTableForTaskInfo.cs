using MathNet.Numerics.LinearAlgebra.Factorization;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TrfCommonUtility;
using static TimeReportV3.MainForm;

namespace TimeReportV3
{
    internal sealed class DetailTableForTaskInfo
    {
        private CustomDataGridView DgvDetailTable;
        private readonly bool IsMinutesUsed;
        private bool IsPossiblyMakeRead;

        private Timer Timer;
        private readonly Action<bool> RefreshTable;

        public DetailTableForTaskInfo(CustomDataGridView dgvDetailTable, bool isMinutesUsed, Action<bool> refreshTable)
        {
            IsMinutesUsed = isMinutesUsed;
            RefreshTable = refreshTable;

            DgvDetailTable = dgvDetailTable;
            DgvDetailTable.SelectionChanged += new EventHandler(DgvDetailTable_SelectionChanged);
            DgvDetailTable.RowHeadersVisible = false;

            DgvDetailTable.AllowUserToResizeColumns = true;
            DgvDetailTable.AllowUserToResizeRows = false;
            DgvDetailTable.ScrollBars = ScrollBars.Both;

            //DgvDetailTable.AllowUserToResizeColumns = true;
            DgvDetailTable.DefaultCellStyle.WrapMode = DataGridViewTriState.True; // разрешить несколько строк в ячейке
            DgvDetailTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            DgvDetailTable.AllowUserToAddRows = false; //запрешаем пользователю самому добавлять строки

            //DgvDetailTable.VerticalScrollBar.VisibleChanged += new EventHandler(VerticalScrollBar_VisibleChanged);
            DgvDetailTable.CellClick += new DataGridViewCellEventHandler(DgvDetailTable_CellClick);
            //DgvDetailTable.Resize += new System.EventHandler(DgvDetailTable_Resize);

            ShowFormHeaders();
        }

        private void ShowFormHeaders()
        {
            var column1 = new DataGridViewColumn
            {
                HeaderText = "№ задачи", //текст в шапке
                Width = 70, //ширина колонки
                ReadOnly = true, //значение в этой колонке нельзя править
                Name = "TaskId", //текстовое имя колонки, его можно использовать вместо обращений по индексу
                Frozen = true, //флаг, что данная колонка всегда отображается на своем месте
                CellTemplate = new DataGridViewTextBoxCell(), //тип нашей колонки
            };
            column1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column1.SortMode = DataGridViewColumnSortMode.Automatic;
            DgvDetailTable.Columns.Add(column1);

            var column2 = new DataGridViewColumn
            {
                HeaderText = "Система",
                Width = 40,
                ReadOnly = true,
                Name = "System",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column2.SortMode = DataGridViewColumnSortMode.Automatic;
            DgvDetailTable.Columns.Add(column2);

            var column3 = new DataGridViewColumn
            {
                HeaderText = "Заголовок",
                Width = 350,
                ReadOnly = true,
                Name = "Name",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column3.SortMode = DataGridViewColumnSortMode.Automatic;
            DgvDetailTable.Columns.Add(column3);

            if (IsMinutesUsed)
            {
                var column4 = new DataGridViewColumn
                {
                    HeaderText = "Списано",
                    Width = 100,
                    ReadOnly = true,
                    Name = "Minutes",
                    Frozen = true,
                    CellTemplate = new DataGridViewTextBoxCell()
                };
                column4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column4.SortMode = DataGridViewColumnSortMode.Automatic;
                DgvDetailTable.Columns.Add(column4);
            }

            var column5 = new DataGridViewColumn
            {
                HeaderText = "Инициатор",
                Width = 100,
                ReadOnly = true,
                Name = "CreatorID",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column5.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column5.SortMode = DataGridViewColumnSortMode.Automatic;
            DgvDetailTable.Columns.Add(column5);

            var column6 = new DataGridViewColumn
            {
                HeaderText = "Исполнители",
                Width = 310,
                ReadOnly = true,
                Name = "Executors",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column6.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column6.SortMode = DataGridViewColumnSortMode.Automatic;
            DgvDetailTable.Columns.Add(column6);

            var column7 = new DataGridViewColumn
            {
                HeaderText = "Дата создания",
                Width = 135,
                ReadOnly = true,
                Name = "Created",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column7.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column7.SortMode = DataGridViewColumnSortMode.Automatic;
            DgvDetailTable.Columns.Add(column7);

            var column8 = new DataGridViewColumn
            {
                HeaderText = "Дата изменения",
                Width = 135,
                ReadOnly = true,
                Name = "Changed",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column8.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column8.SortMode = DataGridViewColumnSortMode.Automatic;
            DgvDetailTable.Columns.Add(column8);

            var column9 = new DataGridViewColumn
            {
                HeaderText = "Статус",
                Width = 85,
                ReadOnly = true,
                Name = "StatusId",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column9.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column9.SortMode = DataGridViewColumnSortMode.Automatic;
            DgvDetailTable.Columns.Add(column9);

            /*var column10 = new DataGridViewColumn
            {
                HeaderText = "Номер Jira",
                Width = 85,
                ReadOnly = true,
                Name = "KeyTaskId",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column10.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column10.SortMode = DataGridViewColumnSortMode.Automatic;
            DgvDetailTable.Columns.Add(column10);*/
            DgvDetailTable.AllowUserToOrderColumns = true;
            DgvDetailTable.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            DgvDetailTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            DgvDetailTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            DgvDetailTable.AllowUserToAddRows = false; //запрещаем пользователю самому добавлять строки
        }

        private void DgvDetailTable_SelectionChanged(object sender, EventArgs e)
        {
            DgvDetailTable.SelectionChanged -= DgvDetailTable_SelectionChanged;
            DgvDetailTable.SelectionChanged += DgvDetailTable_SelectionChanged;
            DgvDetailTable.ClearSelection();

            //if (Control.MouseButtons != MouseButtons.None)
            //    ((DataGridView)sender).CurrentCell = null;
        }

        public List<int> GetVisibleTaskIds()
        {
            var result = new List<int>();

            foreach(DataGridViewRow row in DgvDetailTable.Rows)
            {
                if (row.Tag is FieldsDetailInfo item) //result.Add(item.TaskId);
                    if(int.TryParse(item.TaskId, out var id)) 
                        result.Add(id);

                //if (row.Cells["TaskId"].Value is int id) result.Add(id);
            }
            return result;
        }
        private int GetDgvMainTableHeight()
        {
            int totalRowHeight = DgvDetailTable.ColumnHeadersHeight;
            foreach (DataGridViewRow row in DgvDetailTable.Rows)
            {
                totalRowHeight += row.Height;
            }

            return totalRowHeight + 2;
        }

        private int GetDgvMainTableWidht()
        {
            int totalColWidth = 0;
            foreach (DataGridViewColumn col in DgvDetailTable.Columns)
            {
                totalColWidth += col.Width;
            }

            return totalColWidth + 3;
        }

        public void Show(FullFieldsTaskInfo detailDatas)
        {
            //DgvDetailTable.Rows.Clear();

            if (DgvDetailTable.Columns.Count == 0)
            {
                ShowFormHeaders();
            }


            //DgvDetailTable.Rows.Clear();


            for (int i = 0; i < detailDatas.FieldsTaskInfos.Length; ++i)
            {
                var item = detailDatas.FieldsTaskInfos[i];
                int rowIndex;
                if (IsMinutesUsed)
                {
                    var spentTime = new TimeSpan(0, item.Minutes, 0);
                    if (item.System == "Jira")
                    {
                        rowIndex = DgvDetailTable.Rows.Add(item.KeyTaskId, item.System, item.Name, $"{spentTime:hh\\:mm}", item.CreatorName, item.Executors, $"{item.Created:dd.MM.yyyy HH:mm}", $"{item.Changed:dd.MM.yyyy HH:mm}", item.StatusValue);
                    }
                    else
                    {
                        rowIndex = DgvDetailTable.Rows.Add(item.TaskId, item.System, item.Name, $"{spentTime:hh\\:mm}", item.CreatorName, item.Executors, $"{item.Created:dd.MM.yyyy HH:mm}", $"{item.Changed:dd.MM.yyyy HH:mm}", item.StatusValue);
                    }

                }
                else
                {
                    if (item.System == "Jira")
                    {
                        //item.TaskId = item.KeyTaskId;
                        rowIndex = DgvDetailTable.Rows.Add(item.KeyTaskId, item.System, item.Name, item.CreatorName, item.Executors, $"{item.Created:dd.MM.yyyy HH:mm}", $"{item.Created:dd.MM.yyyy HH:mm}", item.StatusValue);
                    }
                    else
                    {
                        rowIndex = DgvDetailTable.Rows.Add(item.TaskId, item.System, item.Name, item.CreatorName, item.Executors, $"{item.Created:dd.MM.yyyy HH:mm}", $"{item.Created:dd.MM.yyyy HH:mm}", item.StatusValue);
                    }

                }

                DgvDetailTable.Rows[rowIndex].Tag = item;
                var font = DgvDetailTable.Font;

                FontStyle fontStyle;
                if (item.IsSelected == 1)
                {
                    fontStyle = FontStyle.Bold;
                    var fontLine = new Font(font, fontStyle);
                    for (int j = 1; j < DgvDetailTable.Columns.Count; j++)
                    {
                        DgvDetailTable.Rows[i].Cells[j].Style.Font = fontLine;
                    }
                }
                else
                {
                    fontStyle = font.Style;
                }

                DgvDetailTable.Rows[i].Cells[0].Style.Font = new Font(font, fontStyle | FontStyle.Underline);
                DgvDetailTable.Rows[i].Cells[0].Style.ForeColor = Color.Blue;
                //DgvDetailTable.Rows[rowIndex].Tag= item;
            }

            //DgvDetailTable.Dock = DockStyle.Fill;
            //DgvDetailTable.ScrollBars = ScrollBars.Vertical;
           //DgvDetailTable.AutoSize = true;
           DgvDetailTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
           DgvDetailTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            //DgvDetailTable.ScrollBars = ScrollBars.Vertical;

            //DgvDetailTable.Height = GetDgvMainTableHeight();
           DgvDetailTable.Width = GetDgvMainTableWidht() + SystemInformation.VerticalScrollBarWidth;

            IsPossiblyMakeRead = detailDatas.IsPossiblyMakeRead;
            DgvDetailTable.Refresh();
            //var height = GetDgvMainTableHeight();
            //if (height > DgvDetailTable.Height)
            //{
            //    DgvDetailTable.Width += SystemInformation.VerticalScrollBarWidth;
            //}
            DgvDetailTable.ClearSelection();
        }

        private void Refresh(object sender, EventArgs eventArgs)
        {
            Timer.Stop();
            RefreshTable(true);
        }

        private void DgvDetailTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            /*for(int i = 0;i< DgvDetailTable.Rows.Count;i++)
            {
                if (DgvDetailTable.Rows[i].Cells["System"].Value !=null)
                {
                    if (DgvDetailTable.Rows[i].Cells["Id"].Value !=null)
                    {
                        string taskId = (string)DgvDetailTable.Rows[i].Cells["Id"].Value;
                        string sys = (string)DgvDetailTable.Rows[i].Cells["System"].Value;
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

                string taskId = (string)DgvDetailTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                string sys = (string)DgvDetailTable.Rows[e.RowIndex].Cells["System"].Value;//ставим условие если jira то либо сплитуем задачу либо условием либо так же вэлью помещаем в мтеод ниже
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
                    RefreshTable(false);
                }
            }
            /*if (e.RowIndex >= 0 && e.ColumnIndex == 1)
            {
                var sys = (string)DgvDetailTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                var process = OpenInToBrowser(null, sys);
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
            */

            //DgvDetailTable.ClearSelection();
            //((DataGridView)sender).CurrentCell = null;
        }



        private Process OpenInToBrowser(string taskId, string sys)//FieldsDetailInfo item)//, string system, string jiraProject)
        {
            //var sys = item.System;
            //_systemMode = sys ;
            string url;
            if (sys == "Jira")
            {
                url = $"http://{Properties.Settings.Default.AddressOfServerJira}/browse/{taskId}";
                //var pathToBrowser = GetPathToBrowser();
                //return Process.Start(pathToBrowser, url);
            }
            else
            {
                url = $"https://{Properties.Settings.Default.AddressOfServerIntraService}/Task/View/{taskId}";
                //var pathToBrowser = GetPathToBrowser();
                //eturn  url;
            }


            /*switch (_systemMode)
            {
                case SystemMode.Jira:
                    url = $"http://{Properties.Settings.Default.AddressOfServerJira}/browse/{taskId}";
                    break;
                case SystemMode.All:
                    if (item.System == "IS")
                    {
                        url = $"https://{Properties.Settings.Default.AddressOfServerIntraService}/Task/View/{taskId}";
                        break;
                    }
                    else
                    {
                        url = $"http://{Properties.Settings.Default.AddressOfServerJira}/browse/{taskId}";
                        break;
                    }
                case SystemMode.IS:
                default:
                    url = $"https://{Properties.Settings.Default.AddressOfServerIntraService}/Task/View/{taskId}";//$"https://{Properties.Settings.Default.AddressOfServerIntraService}/Task/View/{taskId}";
                    break;
            }*/


            /*var intraService = Properties.Settings.Default.AddressOfServerIntraService;
            url = $"https://{intraService}/Task/View/{taskId}";
            var pathToBrowser = GetPathToBrowser();
            if (item.System == "Jira")
            {
                var jira = Properties.Settings.Default.AddressOfServerJira;
                url = $"http://{jira}/browse/{item.KeyTaskId}";
            }
            else
            {
                var intraService = Properties.Settings.Default.AddressOfServerIntraService;
                url = $"https://{intraService}/Task/View/{item.TaskId}";
                
            }*/
            return Process.Start(GetPathToBrowser(), url);
        }

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
    }
}
