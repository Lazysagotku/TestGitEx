using System.Windows.Forms;

namespace TimeReportV3
{
    internal sealed class ParamTargetDB : IParamSettings
    {
        private ComboBox CmbTargetDB;

        public ParamTargetDB(ComboBox cmbTargetDB)
        {
            CmbTargetDB = cmbTargetDB;
        }
        public void SaveValue()
        {
            if (CmbTargetDB.Text.Equals(Properties.Settings.Default.TargetDB))
                return;
            Properties.Settings.Default.TargetDB = CmbTargetDB.Text;
            MessageBox.Show("Необходим перезапуск. Программа будет закрыта");
            MainForm.NeedRestart = true;
        }
        public void SetStartValue()
        {
            CmbTargetDB.Text = Properties.Settings.Default.TargetDB;
        }
    }
}
