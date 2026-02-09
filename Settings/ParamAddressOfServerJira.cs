using TimeReportV3.Properties; 

namespace TimeReportV3 
{ internal sealed class ParamAddressOfServerJira : IParamSettings 
    { 
        public string Name => "itdesk.trinfico.ru"; 
        public string Value 
        { 
          get => Properties.Settings.Default.AddressOfServerJira; 
          set => Properties.Settings.Default.AddressOfServerJira = value; 
        } 
        public void SaveValue()
        {
            //if (TbxAddressOfServerJira.Text.Length > 0)
            //{
            //    Properties.Settings.Default.AddressOfServerJira = TbxAddressOfServerJira.Text;
            //}
        }

        public void SetStartValue()
        {
            //TbxAddressOfServerJira.Text = Properties.Settings.Default.AddressOfServerJira;
        }
    } 
}
