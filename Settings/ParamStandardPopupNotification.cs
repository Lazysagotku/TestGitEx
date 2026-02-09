using System.Windows.Forms;

namespace TimeReportV3
{
    internal sealed class ParamStandardPopupNotification : IParamSettings
    {
        private CheckBox CbxIsStandardPopupNotification;

        public ParamStandardPopupNotification(CheckBox cbxIsStandardPopupNotification)
        {
            CbxIsStandardPopupNotification = cbxIsStandardPopupNotification;
        }
        public void SaveValue()
        {
            if (CbxIsStandardPopupNotification.Checked)
            {
                var windowsPushNotifications = new WindowsPushNotifications();
                if (windowsPushNotifications.IsPushNotificationsEnable())
                {
                    Properties.Settings.Default.StandardPopupNotification = CbxIsStandardPopupNotification.Checked;
                }
                else
                {
                    //if (windowsPushNotifications.SetPushNotificationsEnable())
                    //{
                    //    // установить значение реестра - мало. Нужно ещё перезагрузить компьютер.
                    //    // Или в настройках (Система -> Настройки -> Система -> Уведомления и действия включить разрешения вручную.
                    //    Properties.Settings.Default.StandardPopupNotification = CbxIsStandardPopupNotification.Checked;
                    //}

                    MessageBox.Show("У Вас запрещен показ стандартных всплывающих уведомлений! \nДля включения разрешения нужно зайти в \nНастройки -> Сиситема -> Уведомления и действия", 
                        "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    CbxIsStandardPopupNotification.Checked = false;
                    
                }
            }
            else
            {
                Properties.Settings.Default.StandardPopupNotification = CbxIsStandardPopupNotification.Checked;
            }
        }

        public void SetStartValue()
        {
            CbxIsStandardPopupNotification.Checked = Properties.Settings.Default.StandardPopupNotification;
        }
    }
}
