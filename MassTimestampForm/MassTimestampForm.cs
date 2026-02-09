using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace TimeReportV3
{
    public partial class MassTimestampForm : Form
    {
        private bool IsTaskIdExistsInDgvMassTasks;
        private readonly UserTasksRepo userTasksRepo = new UserTasksRepo();
        public MassTimestampForm()
        {
            InitializeComponent();

            dgvMassTasks.SelectionChanged += new EventHandler(dgvMassTasks_SelectionChanged);
            dgvMassTasks.RowHeadersVisible = false;

            dgvMassTasks.AllowUserToResizeColumns = true;
            dgvMassTasks.AllowUserToResizeRows = false;
            dgvMassTasks.ScrollBars = ScrollBars.Vertical;

            dgvMassTasks.DefaultCellStyle.WrapMode = DataGridViewTriState.True; // разрешить несколько строк в ячейке
            dgvMassTasks.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            dgvMassTasks.AllowUserToAddRows = false; //запрешаем пользователю самому добавлять строки
            dgvMassTasks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            //DgvDetailTable.VerticalScrollBar.VisibleChanged += new EventHandler(VerticalScrollBar_VisibleChanged);
            //DgvDetailTable.Resize += new System.EventHandler(DgvDetailTable_Resize);

            tbxTaskId.KeyPress += TbxTaskId_KeyPress;
            tbxTaskId.TextChanged += TbxTaskId_TextChanged;
        }

        private void TbxTaskId_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TbxTaskId_TextChanged(object sender, EventArgs e)
        {
            dgvMassTasks.SelectionChanged -= dgvMassTasks_SelectionChanged;

            var rowIndex = GetRowIndex(int.Parse(tbxTaskId.Text));
            if (rowIndex >= 0)
            {
                dgvMassTasks.Rows[rowIndex].Selected = true;
                dgvMassTasks.FirstDisplayedScrollingRowIndex = rowIndex;
                IsTaskIdExistsInDgvMassTasks = true;
            }
            else
            {
                dgvMassTasks.ClearSelection();
                IsTaskIdExistsInDgvMassTasks = false;
            }

            dgvMassTasks.SelectionChanged += dgvMassTasks_SelectionChanged;
        }

        private int GetRowIndex(int taskId)
        {
            DataGridViewRow row = dgvMassTasks.Rows
                .Cast<DataGridViewRow>()
                .Where(r => (int)r.Cells["TaskId"].Value == taskId)
                .FirstOrDefault();

            var rowIndex = row == null ? -1: row.Index;
            return rowIndex;
        }

        private void MassTimestampForm_Load(object sender, EventArgs e)
        {
            ShowFormHeaders();
            FillTable();
        }

        private void dgvMassTasks_SelectionChanged(object sender, EventArgs e)
        {
            tbxTaskId.TextChanged -= TbxTaskId_TextChanged;
            tbxTaskId.Text = dgvMassTasks.SelectedRows[0].Cells["TaskId"].Value.ToString();
            IsTaskIdExistsInDgvMassTasks = true;
            tbxTaskId.TextChanged += TbxTaskId_TextChanged;
        }

        private void ShowFormHeaders()
        {
            var column1 = new DataGridViewColumn
            {
                HeaderText = "№ задачи", //текст в шапке
                Width = 65, //ширина колонки
                ReadOnly = true, //значение в этой колонке нельзя править
                Name = "TaskId", //текстовое имя колонки, его можно использовать вместо обращений по индексу
                Frozen = true, //флаг, что данная колонка всегда отображается на своем месте
                CellTemplate = new DataGridViewTextBoxCell(), //тип нашей колонки
            };
            column1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMassTasks.Columns.Add(column1);

            var column2 = new DataGridViewColumn
            {
                HeaderText = "Система",
                Width = 50,
                ReadOnly = true,
                Name = "System",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvMassTasks.Columns.Add(column2);

            var column3 = new DataGridViewColumn
            {
                HeaderText = "Задача",
                Width = 250,
                ReadOnly = true,
                Name = "Name",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvMassTasks.Columns.Add(column3);

            var column4 = new DataGridViewColumn
            {
                HeaderText = "Сервис",
                Width = 130,
                ReadOnly = true,
                Name = "Service",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvMassTasks.Columns.Add(column4);

            var column5 = new DataGridViewColumn
            {
                HeaderText = "Статус",
                Width = 65,
                ReadOnly = true,
                Name = "StatusId",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column5.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMassTasks.Columns.Add(column5);

            var column6 = new DataGridViewColumn
            {
                HeaderText = "Инициатор",
                Width = 120,
                ReadOnly = true,
                Name = "CreatorID",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column6.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvMassTasks.Columns.Add(column6);

            var column7 = new DataGridViewColumn
            {
                HeaderText = "Исполн-ли",
                Width = 70,
                ReadOnly = true,
                Name = "ExecutorsCount",
                Frozen = true,
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column7.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvMassTasks.Columns.Add(column7);

            var column8 = new DataGridViewColumn
            {
                HeaderText = "Дата создания",
                Width = 115,
                ReadOnly = true,
                Name = "Created",
                Frozen = true,

                
                CellTemplate = new DataGridViewTextBoxCell()
            };
            column8.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column8.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            dgvMassTasks.Columns.Add(column8);
            dgvMassTasks.AllowUserToAddRows = false; //запрещаем пользователю самому добавлять строки
        }

        private void FillTable()
        {
            dgvMassTasks.Rows.Clear();

            var massTimestampInfos = userTasksRepo.GetMassTimestampInfos().ToArray();

            for (int i = 0; i < massTimestampInfos.Length; ++i)
            {
                var item = massTimestampInfos[i];

                dgvMassTasks.Rows.Add(item.TaskId, item.TaskName, item.Service, item.Status, item.Author, item.ExecutorsCount, item.Created);

                var executors = item.Executors.Replace("  ", " ").Replace(", ", ",").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var sortExecutors = executors.OrderBy(e => e);
                var columnExecutors = string.Join(Environment.NewLine, sortExecutors);
                dgvMassTasks.Rows[i].Cells["ExecutorsCount"].ToolTipText = columnExecutors;
            }

            //DgvDetailTable.Height = GetDgvMainTableHeight();
            //dgvMassTasks.Width = GetDgvMainTableWidht() + SystemInformation.VerticalScrollBarWidth;
            //dgvMassTasks.Refresh();
            //var height = GetDgvMainTableHeight();
            //if (height > DgvDetailTable.Height)
            //{
            //    DgvDetailTable.Width += SystemInformation.VerticalScrollBarWidth;
            //}
            //DgvDetailTable.ClearSelection();
            //IsPossiblyMakeRead = detailDatas.IsPossiblyMakeRead;
        }


        private void btnSaveAndExit_Click(object sender, EventArgs e)
        {
            if (!IsTaskIdExistsInDgvMassTasks && !userTasksRepo.IsTaskIdExistsInDB(tbxTaskId.Text))
            {
                MessageBox.Show($"Задачи с номером {tbxTaskId.Text} не существует!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsTimeCorrect(out int totalMinutes))
            {
                return;
            }

            var taskId = tbxTaskId.Text;
            
            // В таблице Task нужно также обновить поле Hours
            int number = userTasksRepo.FillTaskExpenses(taskId, totalMinutes);
            if (number <= 0)
            {
                MessageBox.Show("Данные не вставлены в талицу TaskExpenses. Возможно, данные были добавлены ранее",
                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            };

            //if (cbxSetTaskStatusCompleted.Checked)
            //{
            // проверяем была ли закрыта закрыта задача ранее
            // если нет, то закрываем ее 
            // см. commit Task #19369 Доработать TimeReport от 2024-09-06

            Close();
        }

        private bool IsTimeCorrect(out int totalMinutes)
        {
            var hours = Convert.ToInt32(tbxHours.Text);
            var minutes = Convert.ToInt32(tbxMinutes.Text);
            totalMinutes = 60 * hours + minutes;
            if (totalMinutes == 0)
            {
                MessageBox.Show("Необходимо ввести время!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (totalMinutes > 24*60)
            {
                MessageBox.Show("За текущую дату невозможно списать более 24 часов!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tbxHours_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void tbxMinutes_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
