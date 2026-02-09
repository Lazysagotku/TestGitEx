using System.Windows.Forms;

namespace TimeReportV3
{
    internal sealed class ParamDisplayInSystemTray : IParamSettings
    {
        private readonly RadioButton[] DisplayInSystemTrayRadioButtions;
        private const byte DefaultActiveIndex = 2;

        public ParamDisplayInSystemTray(RadioButton[] displayInSystemTrayRadioButtions)
        {
            DisplayInSystemTrayRadioButtions = displayInSystemTrayRadioButtions;
        }
        public void SaveValue()
        {
            Properties.Settings.Default.SelectionIndexDisplayInSystemTray = GetSelectionRadioButtonActiveIndex();
        }

        public void SetStartValue()
        {
            var activeIndex = Properties.Settings.Default.SelectionIndexDisplayInSystemTray;
            if (activeIndex < 1 || activeIndex > DisplayInSystemTrayRadioButtions.Length)
            {
                activeIndex = DefaultActiveIndex;
            }

            DisplayInSystemTrayRadioButtions[activeIndex - 1].Checked = true;
        }
        private byte GetSelectionRadioButtonActiveIndex()
        {
            for (byte i = 0; i < DisplayInSystemTrayRadioButtions.Length; i++)
            {
                if (DisplayInSystemTrayRadioButtions[i].Checked)
                {
                    return (byte)(i + 1);
                }
            }

            return DefaultActiveIndex;
        }
    }
}
