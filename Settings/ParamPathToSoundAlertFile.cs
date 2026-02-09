using System.IO;
using System.Windows.Forms;

namespace TimeReportV3
{
    internal sealed class ParamPathToSoundAlertFile : IParamSettings
    {
        

        private TextBox TbxPathToSoundAlertFile;
        private SoundAlert SoundAlert;
        public ParamPathToSoundAlertFile(TextBox tbxPathToSoundAlertFile)
        {
            TbxPathToSoundAlertFile = tbxPathToSoundAlertFile;
            TbxPathToSoundAlertFile.Enabled = false;
            SoundAlert = new SoundAlert();
        }

        public void SaveValue()
        {
            Properties.Settings.Default.PathToSoundAlertFile = TbxPathToSoundAlertFile.Text;
        }

        public void SetStartValue()
        {
            TbxPathToSoundAlertFile.Text = Properties.Settings.Default.PathToSoundAlertFile;
        }

        //private bool IsPathToSoundFileCorrect(string pathToSoundFile)
        //{
        //    return File.Exists(pathToSoundFile) && Path.GetFileName(pathToSoundFile).EndsWith(".wav");
        //}
    }
}
