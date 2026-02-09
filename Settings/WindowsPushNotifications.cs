using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace TimeReportV3
{
    internal sealed class WindowsPushNotifications
    {
        private const string PATH_NOTIFICATION_REGECTRY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\PushNotifications";
        private const string NAME_PARAMETER = "ToastEnabled";
        public bool IsPushNotificationsEnable()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(PATH_NOTIFICATION_REGECTRY, false);
            object value;
            try
            {
                value = key.GetValue(NAME_PARAMETER);
                key.Close();
            }
            catch (Exception)
            {
                return false;
            }

            if (value == null)
                return false;

            var isEnable = (int)value == 1;
            return isEnable;
        }

        public bool SetPushNotificationsEnable()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(PATH_NOTIFICATION_REGECTRY, true);
            try
            {
                key.SetValue(NAME_PARAMETER, 1);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                key.Close();
            }
           
            return true;
        }
    }
}
