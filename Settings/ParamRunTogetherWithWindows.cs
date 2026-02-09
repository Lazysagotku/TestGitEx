using System.IO;
using System.Reflection;
using System.Windows.Forms;
using TrfCommonUtility;

namespace TimeReportV3
{
    internal sealed class ParamRunTogetherWithWindows : IParamSettings
    {
        //private bool RunTogetherWithWindows = false;
        private readonly CheckBox CbxRunTogetherWithWindows;

        public ParamRunTogetherWithWindows(CheckBox cbxRunTogetherWithWindows)
        {
            CbxRunTogetherWithWindows = cbxRunTogetherWithWindows;
        }
            
        public void SaveValue()
        {
            const string PATH_STARTER = @"\\ALEF\Stockroute\TimeReport v3\TRv3TimeReportV3Starter.exe";
            const string APLICATION_STARTER_NAME = "TimeReportV3Starter";
            
            var aswwSTARTER = new AutostartWithWindows(PATH_STARTER, APLICATION_STARTER_NAME);
            
            var pathToMainApplication = Assembly.GetExecutingAssembly().Location;
            var applicationName = Application.ProductName;
            var aswwLocal = new AutostartWithWindows(pathToMainApplication, applicationName);

            if (CbxRunTogetherWithWindows.Checked)
            {
                if (File.Exists(PATH_STARTER))
                {
                    aswwSTARTER.Start();
                }
                else
                {
                    aswwLocal.Start();
                }
            }
            else
            {
                aswwSTARTER.Stop();
                aswwLocal.Stop();
            }

            Properties.Settings.Default.RunTogetherWithWindows = CbxRunTogetherWithWindows.Checked;
        }

        public void SetStartValue()
        {
            CbxRunTogetherWithWindows.Checked = Properties.Settings.Default.RunTogetherWithWindows;
        }
    }
}
