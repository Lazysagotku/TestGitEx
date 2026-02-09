using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TimeReportV3.Properties;

namespace TimeReportV3
{
    public partial class DetailsForm : Form
    {
        private readonly ParamResult ParamResult;
        public FieldsDetailInfo[] Details { get; set; }
        public bool IsPossiblyMakeRead { get; set; }
        private readonly MainForm MainForm;
        private DetailTableForTaskInfo DetailTable;
        private int _initialHeight;
        private bool _locationInitialized = false;
        private bool _reSize = false;
        //private bool IsPossiblyMakeRead = false;
        public DetailsForm(ParamResult paramResult, MainForm mainForm)
        {
            InitializeComponent();

            // по умолчанию это будет означать, что никаких действий после закрытия этой формы не требуется
            DialogResult = DialogResult.None;
            ParamResult = paramResult;
            MainForm = mainForm;
            //DetailsForm.AutoSize = true;
            //DetailsForm.FormBorderStyle = FormBorderStyle.Sizable;
            //Height = 380;


            //if (MainForm.WindowState == FormWindowState.Normal)
            //{
            //    Height = MainForm.Height;
            //}
            //else
            //{
            //    Height = MainForm.RestoreBounds.Height;
            //}

        }

        private void DetailsForm_Load(object sender, EventArgs e)
        {
            Text = ParamResult.DetailsParameterName;
            
            if(!_reSize)
            {
                DetailTable = new DetailTableForTaskInfo(dgvDetailTable, ParamResult.IsMinutesUsed, RefreshTable);
                RefreshTable(true);
                _reSize = true;
            }
            _initialHeight = Height;
        }

        private void RefreshTable(bool isMainFormRefresh)
        {
            

            //var detailDatas = ParamResult.GetDetailedInfo(ParamResult);
            var details = ParamResult.Details;
            if (details == null || details.Length == 0)
            {
                Close();
                return;
            }
            var detailDatas = new FullFieldsTaskInfo { FieldsTaskInfos = details, IsPossiblyMakeRead = ParamResult.IsPossiblyMakeRead };
            if (detailDatas == null)
            {
                MessageBox.Show("Не удалось получить данные из БД!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            if (detailDatas.FieldsTaskInfos.Length == 0)
            {
                Close();
                MainForm.RefreshData1(null, null);
                return;
            }


            MainForm.RefreshData1(null, null);
            IsPossiblyMakeRead = detailDatas.IsPossiblyMakeRead;
            SetFormSize();
            if (!isMainFormRefresh)
                MainForm.RefreshData1(null, null);

            DetailTable.Show(detailDatas);
        }


        private void SetFormSize()
        {
            var oldButton = Controls.OfType<Button>().FirstOrDefault(b => b.Name == "btnSetRead");
            if (oldButton != null)
            {
                Controls.Remove(oldButton);
                oldButton.Dispose();
            }
            var titleWidth = Width - ClientRectangle.Width;
            var width = titleWidth - 2 * dgvDetailTable.Location.X + dgvDetailTable.Width-70;
            //int width = Width;
            Height =Math.Max(_initialHeight, Height);
            // запрещаем изменение размеров формы
            MaximumSize = new Size(width, Height);
            MinimumSize = new Size(width, Height);

            if (MainForm.WindowState == FormWindowState.Minimized)
            {
                Activate();
                return;
            }

            if(!_locationInitialized)
            {
                var x = MainForm.Location.X + MainForm.Width - Width;
                if (x < 0)
                    x = 0;

                var y = MainForm.Location.Y + MainForm.Height - Height;
                if (y < 0)
                    y = 0;

                Location = new Point(x, y);
                _locationInitialized = true;
            }
            

            if (ParamResult.SetAsRead != null && IsPossiblyMakeRead)
            {
                var newButtion = new Button
                {
                    Name = "btnSetRead",
                    Text = "Сделать всё прочитанными", // ParamResult.NameDo,
                    AutoSize = true,
                    Visible = true
                };
                newButtion.Click += new EventHandler(NewButtion_Click);
                Controls.Add(newButtion);
                var xLocation = ClientRectangle.Width - newButtion.Width - dgvDetailTable.Location.Y;
                var yLocation = ClientRectangle.Height - newButtion.Height - dgvDetailTable.Location.X;
                newButtion.Location = new Point(xLocation, yLocation);
                dgvDetailTable.Height = newButtion.Location.Y - 2 * dgvDetailTable.Location.Y;
            }
            else
            {
                //Rectangle screenRectangle = this.RectangleToScreen(this.ClientRectangle);
                //int titleHeight = screenRectangle.Top - this.Top;
                dgvDetailTable.Height = ClientRectangle.Height - 2 * dgvDetailTable.Location.Y;
            }
        }

        private void NewButtion_Click(object sender, EventArgs e)
        {
            var ids = DetailTable.GetVisibleTaskIds();
            //var isOk = ParamResult.SetAsRead(ParamResult);

            bool isOk;
            if(ParamResult.SetAsReadIds != null) isOk=ParamResult.SetAsReadIds(ids);
            else isOk= ParamResult.SetAsRead(ParamResult);
            if (isOk)
            {
                // это будет означать, что после закрытия этой формы произведены обновления БД и требуется обновить таблицу главной формы
                DialogResult = DialogResult.Yes;
                Close();
                MainForm.BeginInvoke(new Action(() => { MainForm.RefreshData1(null, null); }));
                return;
            }

            var dialogResult = MessageBox.Show("Не удалось обновить данные в БД! Повторить попытку?", "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (dialogResult == DialogResult.OK)
            {
                NewButtion_Click(sender, e);
            }
        }

        private void DetailsForm_Deactivate(object sender, EventArgs e)
        {
            //if (!Properties.Settings.Default.DontLostFocusCollapse)
            //{
            //    Close();
            //}
        }

        private void dgvDetailTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

