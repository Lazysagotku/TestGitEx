using System.Windows.Forms;

namespace TimeReportV3
{
    internal sealed class ParamNotifyAboutTasksWithoutЕxecutor : IParamSettings
    {
        //private bool NotifyAboutTasksWithoutЕxecutor = false;
        private readonly CheckBox CbxNotifyAboutTasksWithoutЕxecutor;
        public ParamNotifyAboutTasksWithoutЕxecutor(CheckBox cbxNotifyAboutTasksWithoutЕxecutor)
        {
            CbxNotifyAboutTasksWithoutЕxecutor = cbxNotifyAboutTasksWithoutЕxecutor;
        }

        public void SaveValue()
        {
            Properties.Settings.Default.NotifyAboutTasksWithoutЕxecutor = CbxNotifyAboutTasksWithoutЕxecutor.Checked;
        }

        public void SetStartValue()
        {
            CbxNotifyAboutTasksWithoutЕxecutor.Checked = Properties.Settings.Default.NotifyAboutTasksWithoutЕxecutor;
            
        }
    }
}
