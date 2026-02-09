using System.IO;
using System.Windows.Forms;

namespace TimeReportV3
{
    internal sealed class ParamPathToBrowser : IParamSettings
    {
        //private string PathToBrowser = string.Empty;
        private TextBox TbxPathToBrowser;

        public ParamPathToBrowser(TextBox tbxPathToBrowser)
        {
            TbxPathToBrowser = tbxPathToBrowser;
        }

        public void SaveValue()
        {
            if (IsPathToBrowserCorrect(TbxPathToBrowser.Text))
            {
                Properties.Settings.Default.PathToBrowser = TbxPathToBrowser.Text;
            }
        }

        public void SetStartValue()
        {
            TbxPathToBrowser.Text = Properties.Settings.Default.PathToBrowser;
        }

        private bool IsPathToBrowserCorrect(string pathToBrowser)
        {
            return File.Exists(pathToBrowser) && Path.GetFileName(pathToBrowser).EndsWith(".exe");
        }
    }
}
