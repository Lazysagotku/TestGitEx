
using System.Windows.Forms;

namespace TimeReportV3
{
    internal sealed class ParamDontLostFocusCollapse : IParamSettings
    {
        private CheckBox CbxIsDontLostFocusCollapse;

        public ParamDontLostFocusCollapse(CheckBox cbxIsDontLostFocusCollapse)
        {
            CbxIsDontLostFocusCollapse = cbxIsDontLostFocusCollapse;
        }
        public void SaveValue()
        {
            Properties.Settings.Default.DontLostFocusCollapse = CbxIsDontLostFocusCollapse.Checked;
        }

        public void SetStartValue()
        {
            CbxIsDontLostFocusCollapse.Checked = Properties.Settings.Default.DontLostFocusCollapse;
        }
    }
}
