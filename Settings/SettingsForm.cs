using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace TimeReportV3
{
    public partial class SettingsForm : Form
    {
        private List<IParamSettings> Params;

        private readonly MainForm _mainForm;
        public RadioButton RadiooButton1 => radioButton1;
        public RadioButton RadiooButton2 => radioButton2;
        public RadioButton RadiooButton3 => radioButton3;
        private bool _isInitialized = false;
        public SettingsForm(MainForm mainForm)
        {

            InitializeComponent();
            _mainForm = mainForm;
            InitParameters();
            SetStartValues();
            SetTooltipsTextBoxForPhoneNumbersForSms();
            SetTooltipsLabelBoxForPhoneNumbersForSms();
            Text = $"Настройки. Пользователь: {MainForm.UserLogin}";

            if (!Properties.Settings.Default.NotifyIS &&
            !Properties.Settings.Default.NotifyJira &&
            !Properties.Settings.Default.NotifyAll)
            {
                Properties.Settings.Default.NotifyAll = true;
                Properties.Settings.Default.Save();
            }

            radioButton1.Checked = Properties.Settings.Default.NotifyIS;
            radioButton2.Checked = Properties.Settings.Default.NotifyJira;
            radioButton3.Checked = Properties.Settings.Default.NotifyAll;
        }

        private void SetTooltipsTextBoxForPhoneNumbersForSms()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip toolTipTextBoxForPhoneNumbersForSms = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 100,
                ReshowDelay = 500,
                ShowAlways = true  // Force the ToolTip text to be displayed whether or not the form is active.
            };

            // Set up the ToolTip text for the Button and Checkbox.
            //toolTipTextBoxForPhoneNumbersForSms.SetToolTip(this.tbxPhoneNumbersForSms, "+7 987 654-32-10, +7 901 234-56-78");
        }

        private void SetTooltipsLabelBoxForPhoneNumbersForSms()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip toolTipTLabelForPhoneNumbersForSms = new ToolTip
            {
                AutoPopDelay = 10000,
                InitialDelay = 100,
                ReshowDelay = 500,
                ShowAlways = true  // Force the ToolTip text to be displayed whether or not the form is active.
            };

            // Set up the ToolTip text for the Button and Checkbox.
            //toolTipTLabelForPhoneNumbersForSms.SetToolTip(this.lblPhoneNumbersForSms, "Смс отправляются только для новых задач с приоритетом высокий и критический\nво вне рабочее время с 20-00 до 8-00 и в нерабочие дни");
        }


        private void InitParameters()
        {
            Params = new List<IParamSettings>
            {
                new ParamRunTogetherWithWindows(cbxRunTogetherWithWindows),
                new ParamFrequencyUpdatingValues(tbxFrequencyUpdatingValues),
                new ParamDisplayInSystemTray(new[]
                    {   rbnNumberActiveUserTasks_1,
                        rbnNumberTaskUserNotInProgress_2,
                        rbnNumberTasksWithoutExecutor_3,
                        rbnNumberUnreadCommentsInUserTasks_4,
                        rbnNumberUnreadCommentsInAllTasks_5
                    }),
                new ParamPathToBrowser(tbxPathToBrowser),
                new ParamAddressOfServerIntraService(tbxAddressOfServerIntraService),
                new ParamNotifyAboutTasksWithoutЕxecutor(cbxNotifyAboutTasksWithoutЕxecutor),
                //new ParamPhoneNumbersForSms(tbxPhoneNumbersForSms),
                new ParamPathToSoundAlertFile(tbxPathToSoundAlertFile),
                new ParamStandardPopupNotification(cbxIsStandardPopupNotification),
                new ParamDontLostFocusCollapse(cbxIsDontLostFocusCollapse),
                new ParamTargetDB(cmbTargetDB),
                new ParamReportFont(cmbFontName, cmbFontSize, cmbThickness),
            };
        }

        private void SetStartValues()
        {
            foreach (var item in Params)
            {
                item.SetStartValue();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (var item in Params)
            {
                item.SaveValue();
            }

            Properties.Settings.Default.Save();
            Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSetPathSoundFile_Click(object sender, EventArgs e)
        {
            var soundAlert = new SoundAlert();
            tbxPathToSoundAlertFile.Text = soundAlert.GetPathSelectFile();
        }

        private void btnPathToBrowser_Click(object sender, EventArgs e)
        {
            const string DEFAULT_FILTER_TYPE_EXE_FILE = "exe files|*.exe";
            const string DEFAULT_FILE_EXTENSION = ".exe";
            OpenFileDialog dlg = new OpenFileDialog
            {
                CheckFileExists = true, // Make sure the dialog checks for existence of the selected file.
                Filter = DEFAULT_FILTER_TYPE_EXE_FILE, // Allow selection of .wav files only.
                DefaultExt = DEFAULT_FILE_EXTENSION
            };

            // Activate the file selection dialog.
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbxPathToBrowser.Text = dlg.FileName;
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void tbxAddressOfServerIntraService_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void cmbTargetDB_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click_1(object sender, EventArgs e)
        {

        }


        private void gbxSettings_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isInitialized) return;
            //if (radioButton1.Checked)
            //{
            Properties.Settings.Default.TraySystemMode = 1;
            MessageBox.Show("При изменении выбора системы требуется либо перезапустить программу, либо сменить систему в главной форме", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            Properties.Settings.Default.NotifyIS = true;
            Properties.Settings.Default.NotifyJira = false;
            Properties.Settings.Default.NotifyAll = false;
            Properties.Settings.Default.Save();


        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isInitialized) return;

            //if (radioButton2.Checked)
            //{
            Properties.Settings.Default.TraySystemMode = 2;
            MessageBox.Show("При изменении выбора системы требуется либо перезапустить программу, либо сменить систему в главной форме", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            Properties.Settings.Default.NotifyIS = false;
            Properties.Settings.Default.NotifyJira = true;
            Properties.Settings.Default.NotifyAll = false;
            Properties.Settings.Default.Save();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

            if (!_isInitialized) return;

            //if (radioButton3.Checked)
            //{
            Properties.Settings.Default.TraySystemMode = 3;
            MessageBox.Show("При изменении выбора системы требуется либо перезапустить программу, либо сменить систему в главной форме", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            Properties.Settings.Default.NotifyIS = false;
            Properties.Settings.Default.NotifyJira = false;
            Properties.Settings.Default.NotifyAll = true;
            Properties.Settings.Default.Save();
        }

        private void tbxFrequencyUpdatingValues_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var item in Params)
            {
                item.SaveValue();
            }
            //Properties.Settings.Default.MainFormLocation;
            //MainForm.SaveCurrentLocation();
            //MainForm.notifyIcon1.Visible = false;
            //MainForm.notifyIcon1.Dispose();
            _mainForm.RestartApplication();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            _isInitialized = true;
        }

        private void rbnNumberActiveUserTasks_1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tbxPathToSoundAlertFile_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            /*var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs","db_log.txt");

            if (File.Exists(filePath))
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });*/
            // Открываем папку с логами вместо отдельного файла
            TimeReportV2.Logs.DbQueryLogger.OpenLogsFolder();

        }
    }
}
