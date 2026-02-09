using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TimeReportV3
{
    /// <summary>
    /// Не на все номера телефонов доходят sms-сообщения!!
    /// У меня на номер МГТС сообщения отправляются, но не доходят. Пишут в статусе - неизвестная причина!
    /// Лучше всего указывать номера телефонов основных операторов: МТС, Билайн и Мегафон.
    /// </summary>
    internal sealed class ParamPhoneNumbersForSms : IParamSettings
    {
        private TextBox TbxPhoneNumbersForSms;
        public static string[] Numbers
        {
            get
            {
                var collections = Properties.Settings.Default.PhoneNumbersForSms;
                if (collections != null && collections.Count > 0)
                {
                    var numberList = collections.Cast<string>().ToArray();
                    return numberList;
                }

                return new string[0];
            }
        }

        public ParamPhoneNumbersForSms(TextBox tbxPhoneNumbersForSms)
        {
            TbxPhoneNumbersForSms = tbxPhoneNumbersForSms;
        }

        public void SaveValue()
        {
            string pattern = @"[+]{1}[7]{1}[\s]{1}[9]{1}[0-9]{2}[\s]{1}[0-9]{3}[-]{1}[0-9]{2}[-]{1}[0-9]{2}";

            var matches = Regex.Matches(TbxPhoneNumbersForSms.Text, pattern);
            var numbers = matches.Cast<Match>().Select(m => m.Value).ToArray();

            if (numbers.Length > 0)
            {
                var collections = new System.Collections.Specialized.StringCollection();
                collections.AddRange(numbers);
                Properties.Settings.Default.PhoneNumbersForSms = collections;
            }
        }

        public void SetStartValue()
        {
            var collections = Properties.Settings.Default.PhoneNumbersForSms;
            if(collections != null && collections.Count > 0)
            {
                var numberList = collections.Cast<string>().ToArray();
                TbxPhoneNumbersForSms.Text = string.Join(", ", numberList);
            }
        }
    }
}
