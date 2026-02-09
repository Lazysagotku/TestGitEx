using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TimeReportV3
{
    //private string AddressOfServerIntraService = "alefsupport.trinfico.ru";
    internal sealed class ParamAddressOfServerIntraService : IParamSettings
    {
        private readonly TextBox TbxAddressOfServerIntraService;
        public ParamAddressOfServerIntraService(TextBox tbxAddressOfServerIntraService)
        {
            TbxAddressOfServerIntraService = tbxAddressOfServerIntraService;
        }
        public void SaveValue()
        {
            if (TbxAddressOfServerIntraService.Text.Length > 0)
            {
                Properties.Settings.Default.AddressOfServerIntraService = TbxAddressOfServerIntraService.Text;
            }
        }

        public void SetStartValue()
        {
            TbxAddressOfServerIntraService.Text = Properties.Settings.Default.AddressOfServerIntraService;
        }
    }
}
