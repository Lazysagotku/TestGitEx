using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace TimeReportV3
{
    internal sealed class MainTable
    {

        private readonly DataGridView DgvMainTable;
        private readonly MainForm MainForm;

        private IParamMainForm[] MainFormParams;
        private List<ParamResult> ParamResults;
        public FieldsDetailInfo[] Details { get; set; }
        public bool IsPossiblyMakeRead { get; set; }

        public bool IsDetailFormShow;

        //private readonly UserTasksRepo userTasksRepo = new UserTasksRepo();

        public MainTable(DataGridView dgvMainTable, IParamMainForm[] mainFormParams, MainForm mainForm)
        {
            
            MainForm = mainForm;
            DgvMainTable = dgvMainTable;
            InitGrid();
            DgvMainTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; 
            DgvMainTable.AutoGenerateColumns = false;

            MainFormParams = mainFormParams;
            ParamResults = MainFormParams.Select(p => p.ParamResult).ToList();

            /*foreach (var param in MainFormParams)
            {
                param.ParamResult.GetDetailedInfo = userTasksRepo.GetDetailedInfo;

                if (param.ParamResult.SetAsRead != null)
                {
                    param.ParamResult.SetAsRead = userTasksRepo.SetTasksReadByStatus;
                }
            }*/

            
        }

        public void Dispose()
        {
            DgvMainTable.SelectionChanged -= DgvMainTable_SelectionChanged;
            DgvMainTable.CellClick -= DgvMainTable_CellClick;
            DgvMainTable.CellDoubleClick -= DgvMainTable_CellDoubleClick;
        }

        /// <summary>
        /// Показать состояние "Загрузка..." в таблице
        /// </summary>
        public void ShowLoadingState()
        {
            if (DgvMainTable.InvokeRequired)
            {
                DgvMainTable.Invoke(new Action(ShowLoadingState));
                return;
            }

            DgvMainTable.Rows.Clear();
            DgvMainTable.Rows.Add("Загрузка данных...", "...");
            DgvMainTable.Enabled = false;
        }

        /// <summary>
        /// Скрыть состояние "Загрузка..."
        /// </summary>
        public void HideLoadingState()
        {
            if (DgvMainTable.InvokeRequired)
            {
                DgvMainTable.Invoke(new Action(HideLoadingState));
                return;
            }

            DgvMainTable.Enabled = true;
        }
        public bool IsChanged(List<int> newCounts)
        {
            if (ParamResults == null || ParamResults.Count != newCounts.Count)
                return true;

            for (int i = 0; i < newCounts.Count; i++)
            {
                if (ParamResults[i].Count != newCounts[i])
                    return true;
            }
            return false;
        }
        private void InitParams(IParamMainForm[] parameters)
        {
            MainFormParams = parameters;
            ParamResults = parameters.Select(p => p.ParamResult).ToList();
        }
        /*public void HandleCellClick(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 1)
                return;

            if (e.RowIndex >= ParamResults.Count)
                return;

            var param = ParamResults[e.RowIndex];
            if (!param.IsDetailsAvailable)
                return;

            IsDetailFormShow = true;
            using (var form = new DetailsForm(param, MainForm))
            {
                form.ShowDialog();
            }
            MainForm.RefreshData(null, null);
            IsDetailFormShow = false;
        }

        public void HandleCellDoubleClick(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 4 && e.ColumnIndex == 0)
            {
                MainForm.IsTimeUserTableVisible = !MainForm.IsTimeUserTableVisible;
                MainForm.NeedRender = true;
                MainForm.RefreshData(null, null);
            }
        }*/

        private void InitGrid()
        {
            DgvMainTable.AutoGenerateColumns = true; 
            DgvMainTable.DataSource = null;
            DgvMainTable.Columns.Clear();
            DgvMainTable.Rows.Clear();

            DgvMainTable.RowHeadersVisible = false; // вот она
            DgvMainTable.ScrollBars = ScrollBars.None;
            DgvMainTable.AllowUserToResizeColumns = true;
            DgvMainTable.AllowUserToResizeRows = false; // и вот эта строка убирают стрелочку в таблице
            DgvMainTable.AllowUserToAddRows = false;

            var column1 = new DataGridViewColumn
            {
                HeaderText = "Название",
                Width = 380,
                ReadOnly = true,
                Name = "name",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };

            var column2 = new DataGridViewColumn
            {
                HeaderText = "Значение",
                Width = 380,
                ReadOnly = true,
                Name = "Value",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //column1.Width = 520;
            //column2.Width = 240;
            DgvMainTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            DgvMainTable.Columns.Add(column1);
            DgvMainTable.Columns.Add(column2);
            //DgvMainTable.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //DgvMainTable.Columns["Value"].Width = 380;

            DgvMainTable.SelectionChanged -= DgvMainTable_SelectionChanged;
            DgvMainTable.CellClick -= DgvMainTable_CellClick;
            DgvMainTable.CellDoubleClick -= DgvMainTable_CellDoubleClick;

            DgvMainTable.SelectionChanged += DgvMainTable_SelectionChanged;
            DgvMainTable.CellClick += DgvMainTable_CellClick;
            DgvMainTable.CellDoubleClick += DgvMainTable_CellDoubleClick;
            //DgvMainTable.CellClick += (s, e) => { MessageBox.Show("CellClick подписан и жив"); };
        }


        public bool GetActualTaskCounts(out List<int> tasksCounts)
        {
            var prevCounts = ParamResults.Select(p => p.Count).ToArray();

            //tasksCounts = userTasksRepo.GetTasksCounts(); 
            tasksCounts = MainFormParams.SelectMany(p => p.Get()).Select(r => r.Count).ToList();
            while (tasksCounts.Count < ParamResults.Count) 
            { 
                tasksCounts.Add(0);
            }

            bool isChanged = false;
            for (int i = 0; i < prevCounts.Length && i < tasksCounts.Count; i++)
            {
                if (prevCounts[i] != tasksCounts[i])
                {
                    isChanged = true;
                    break;
                }
            }

            return isChanged;
        }

        public bool GetActualTaskCounts(List<ParamResult> newResults, out List<int> tasksCounts)
        {
            tasksCounts = newResults?.Select(r => r.Count).ToList() ?? new List<int>();
            var prevCounts = ParamResults?.Select(p => p.Count).ToArray() ?? Array.Empty<int>();

            while (tasksCounts.Count < prevCounts.Length)
            {
                tasksCounts.Add(0);
            }

            for (int i = 0; i < prevCounts.Length && i < tasksCounts.Count; i++)
            {
                if (prevCounts[i] != tasksCounts[i])
                {
                    return true;
                }
            }

            return tasksCounts.Count != prevCounts.Length;
        }

        public void RefreshMainTableRows(IParamMainForm[] parameters)
        {
            /*foreach (var p in parameters) 
            { 
                if (string.IsNullOrEmpty(p.ParamResult.ShowValue))
                    p.ParamResult.ShowValue = p.ParamResult.Count.ToString();
            }*/
            /*ParamResults = parameters.Select(p => p.ParamResult).ToList();

            DgvMainTable.Rows.Clear();
            DgvMainTable.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DgvMainTable.Columns["Value"].Width = 120;
            DgvMainTable.Rows.Add(ParamResults.Count);*/
            //MainForm.Visible = false;
            MainForm.SuspendLayout();
            ParamResults = parameters.SelectMany(p => p.Get()).ToList(); 
            DgvMainTable.Rows.Clear();
            DgvMainTable.Rows.Add(ParamResults.Count);

            for (int i = 0; i < ParamResults.Count; i++)
            {
                var param = ParamResults[i];
                DgvMainTable.Rows[i].Cells[0].Value = param.ParameterName;
                DgvMainTable.Rows[i].Cells[1].Value = param.ShowValue;

                if (param.IsDetailsAvailable)
                {
                    DgvMainTable.Rows[i].Cells[1].Style.ForeColor = Color.Blue;
                    DgvMainTable.Rows[i].Cells[1].Style.Font =
                      new Font(DgvMainTable.Font, FontStyle.Underline);
                }
            }

            DgvMainTable.ClearSelection();
            ResizeGrid();
            DgvMainTable.ResumeLayout(false);
            MainForm.ResumeLayout(true);
            //MainForm.Visible = true;

            //DgvMainTable.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //DgvMainTable.Columns["Value"].Width = 120;
        }

        public void RefreshMainTableRows(List<ParamResult> paramResults)
        {
            MainForm.SuspendLayout();
            ParamResults = paramResults ?? new List<ParamResult>();
            DgvMainTable.Rows.Clear();
            DgvMainTable.Rows.Add(ParamResults.Count);

            for (int i = 0; i < ParamResults.Count; i++)
            {
                var param = ParamResults[i];
                DgvMainTable.Rows[i].Cells[0].Value = param.ParameterName;
                DgvMainTable.Rows[i].Cells[1].Value = param.ShowValue;

                if (param.IsDetailsAvailable)
                {
                    DgvMainTable.Rows[i].Cells[1].Style.ForeColor = Color.Blue;
                    DgvMainTable.Rows[i].Cells[1].Style.Font =
                      new Font(DgvMainTable.Font, FontStyle.Underline);
                }
            }

            DgvMainTable.ClearSelection();
            ResizeGrid();
            DgvMainTable.ResumeLayout(false);
            MainForm.ResumeLayout(true);
        }

        private void ResizeGrid()
        {
            int height = DgvMainTable.ColumnHeadersHeight;
            foreach (DataGridViewRow row in DgvMainTable.Rows)
                height += row.Height;

            int width = DgvMainTable.Columns.Cast<DataGridViewColumn>().Sum(c => c.Width);



            //DgvMainTable.Height = height + 2;
            //DgvMainTable.Width = width + 2 ;
            DgvMainTable.Height = GetDgvMainTableHeight();
            DgvMainTable.Width = GetDgvMainTableWidht();

            //DgvMainTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            //DgvMainTable.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            //DgvMainTable.Columns[1].Width = 380;
            //DgvMainTable.Columns[1].MinimumWidth = 280;




        }
        private int GetDgvMainTableHeight()
        {
            int totalRowHeight = DgvMainTable.ColumnHeadersHeight;
            foreach (DataGridViewRow row in DgvMainTable.Rows)
            {
                totalRowHeight += row.Height;
            }

            return totalRowHeight + 2;
        }

        private int GetDgvMainTableWidht()
        {
            int totalColWidth = 0;
            foreach (DataGridViewColumn col in DgvMainTable.Columns)
            {
                totalColWidth += col.Width;
            }

            return totalColWidth + 3;
        }

        private void DgvMainTable_SelectionChanged(object sender, EventArgs e)
        {
            DgvMainTable.ClearSelection();
        }

        private void DgvMainTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 1)
                return;

            if (IsDetailFormShow)
                return;

            if (e.RowIndex >= ParamResults.Count)
                return;

            var param = ParamResults[e.RowIndex];
            if (param == null || !param.IsDetailsAvailable)
                return;

            IsDetailFormShow = true;
            try
            {
                using (var form = new DetailsForm(param, MainForm))
                {
                    form.ShowDialog();
                }
            }
            finally
            {
                IsDetailFormShow = false;
            }
        }

        private async void DgvMainTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            DgvMainTable.ClearSelection();
            //MainForm.LoadTimeUserDataAsync();
            //MainForm.RefreshData1(null, null);
            //MainForm.Refresh();
            await Task.Run(() =>
            {
                MainForm.LoadTimeUserAll();
            });
            if (e.RowIndex < 0 || e.RowIndex >= ParamResults.Count)
                return;

            if (e.RowIndex > 4 && e.ColumnIndex == 0) // && e.RowIndex < ParamResults.Count)
            {
                MainForm.IsTimeUserTableVisible = !MainForm.IsTimeUserTableVisible;
                if (MainForm.IsTimeUserTableVisible)
                {
                    MainForm.NeedRender = true;
                }
                MainForm.RefreshData(null, null);
            }

            MainForm.RebuildLayout();


            /*if (e.ColumnIndex == 0 && ParamResults[e.RowIndex].IsMinutesUsed)
            {

                //MainForm.IsTimeUserTableVisible = !MainForm.IsTimeUserTableVisible;
                
                //MainForm.NeedRender = true;
                
                MainForm.SetTimeTablesVisible(!MainForm.IsTimeUserTableVisible);
                MainForm.RefreshData1(null, null);
                //ResizeGrid();

                //




                //    = !MainForm.IsTimeUserTableVisible;
                //MainForm.NeedRender = true;
                //MainForm.RefreshData1(null, null);
            }*/
            //MainForm.RefreshData1(null, null);
        }
    }
}
