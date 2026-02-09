using System.Windows.Forms;

namespace TimeReportV3
{
    
    internal sealed class ParamReportFont : IParamSettings
    {
        private ComboBox CmbFontName;
        private ComboBox CmbFontSize;
        private ComboBox CmbThickness;
        public ParamReportFont(ComboBox cmbFontName, ComboBox cmbFontSize, ComboBox cmbThickness)
        {
            CmbFontName = cmbFontName;
            CmbFontSize = cmbFontSize;
            CmbThickness = cmbThickness;
        }
        public void SaveValue()
        {
            Properties.Settings.Default.ReportFont = $"{CmbFontName.Text};{CmbFontSize.Text};{CmbThickness.Text}";
        }
        public void SetStartValue()
        {
            string[] settArr = Properties.Settings.Default.ReportFont.Split(';');
            if (settArr.Length != 3)
                return;
            CmbFontName.Text = settArr[0];
            CmbFontSize.Text = settArr[1];
            CmbThickness.Text = settArr[2];
        }

    }
}

