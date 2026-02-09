//using NLog;
using SmsRu;
using SmsRu.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

/*namespace TimeReportV3
{
    public class MessageInfo 
    {
        public int TaskId { get; set; }
        public string PhoneNumber { get; set; }
        public string SmsId { get; set; }
    }

    public class MessageInfoComparer : IEqualityComparer<MessageInfo>
    {
        public bool Equals(MessageInfo x, MessageInfo y)
        {
            return x.TaskId == y.TaskId && x.PhoneNumber == y.PhoneNumber;
        }

        public int GetHashCode(MessageInfo obj)
        {
            return $"{obj.TaskId}_{obj.PhoneNumber}".GetHashCode();
        }
    }

    internal sealed class SmsProvider
    {
        //public Logger MyLogger { get; private set; }

        private readonly Param3TasksWithoutExecutor Param3TasksWithoutExecutor;
        private readonly PeriodForSendingSms PeriodForSendingSms;
        private List<MessageInfo> LocalSentTasks = new List<MessageInfo>();
        private readonly SmsRepo smsRepo = new SmsRepo();

        // временный лимит, пока приложение не будет отлажено. Установлен, чтобы исключить непрерывное отправление смс из-за возможных ошибок.
        // неудачные отправления также входят в этот лимит
        private const int MaxDailyLimitSms = 5;
        private int CurDailyLimitSms;
        private readonly bool IsTest;
        public SmsProvider(Param3TasksWithoutExecutor param3TasksWithoutExecutor, bool isTest)
        {
            Param3TasksWithoutExecutor = param3TasksWithoutExecutor;
            
            // без этой строки decimal некорректно преобразуется в текстовое значение (с разделителем запятая), но это меняет формат даты и др. региональные настройки
            //CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            PeriodForSendingSms = new PeriodForSendingSms();
            IsTest = isTest;
            CurDailyLimitSms = MaxDailyLimitSms;
            //MyLogger = Logging.MyLogger;
        }

        public void SendSmsIfNeed()
        {
            //WriteToDB(DateTime.Now, "79034567897", 2234, "Rhbnbxtcrbq", 3.15m, "JJJJJJJJ", "99999999-77777", "ttttttttt");
            TaskPriorityIndex taskPriorityIndex;

            if (!Properties.Settings.Default.NotifyAboutTasksWithoutЕxecutor)
                return;

            var phoneNumbers = ParamPhoneNumbersForSms.Numbers;
            if (phoneNumbers.Length == 0)
                return;

            if (IsTest)
            {
                taskPriorityIndex = TaskPriorityIndex.Low;
            }
            else
            {
                var answer = PeriodForSendingSms.IsNeedToSend(DateTime.Now);
                if (answer == Answer.No)
                {
                    LocalSentTasks.Clear();
                    CurDailyLimitSms = MaxDailyLimitSms;
                    return;
                }

                if (answer == Answer.Error)
                {
                    // to-do
                    // залогировать ошибку об невозможности понять нужно ли отправлять смс
                    // для этого нужно создать в БД хранимую процедуру, которая бы сохраняла текст ошибки
                    return;
                }

                taskPriorityIndex = TaskPriorityIndex.High;
            }

            var criticalTasks = GetCriticalTask(taskPriorityIndex);
            if (criticalTasks.Length == 0)
                return;

            // находим задачи, по которым мы ранее не отправляли смс:
            // синхронизируем локальные данные об отправленных смс с БД
            var listTasks = criticalTasks.Select(t => t.TaskId).ToList();
            var dbSentTasks = smsRepo.GetSentSms(listTasks).ToList();
            if (dbSentTasks != null && dbSentTasks.Count > 0)
            {
                // объединяем с локальной структурой
                var comparer = new MessageInfoComparer();
                LocalSentTasks = (LocalSentTasks.Union(dbSentTasks, comparer)).ToList();
            }

            // находим задачи, по которым мы ранее не отправляли смс (или отправляли смс не на все номера):
            var dictTaskIdNumberPhones = LocalSentTasks.GroupBy(mi => mi.TaskId).ToDictionary(g => g.Key, g => g.Select(item => item.PhoneNumber).ToList());
            var newTasks = criticalTasks.Where(item =>
            {
                if(!dictTaskIdNumberPhones.TryGetValue(item.TaskId, out List<string> mis))
                    return true;

                return phoneNumbers.Any(phoneNumber => !mis.Contains(phoneNumber));
            }).ToArray();

            if (newTasks.Length == 0)
                return;

            var limit = new LimitSms();
            if (!limit.CheckLimit(out int remSms, out DataForLogging checkLimitLogging))
            {
                //Logging.Log(checkLimitLogging);
                return;
            }

            var balanceInfo = new Balance();
            if (!balanceInfo.CheckBalance(out decimal balance, out DataForLogging balanceLogging))
            {
                //Logging.Log(balanceLogging);
                return;
            }

            // преобразуем формат номера телефона, принятого в приложении, к формату телефона для SmsRu
            var smsRuPhoneNumbers = phoneNumbers.Select(p => string.Concat(p.Where(ch => char.IsDigit(ch))));
            var newSms = new NewSms();

            foreach (var newTask in newTasks)
            {
                var taskInfo = new TaskInfo { TaskId = newTask.TaskId.ToString(), PriorityTask = newTask.Priority, BalanceBefore = balance, TaskName = newTask.Name };
                var text = newSms.GetSmsText(taskInfo);
                var costInfo = new CostSms();
                
                foreach (var to in smsRuPhoneNumbers)
                {
                    if (LocalSentTasks.Any(sm => sm.TaskId == newTask.TaskId && sm.PhoneNumber == to))
                        continue;

                    if (!ValidNumber.IsPhoneNumberValid(to))
                    {
                        //MyLogger.Error($"Номер телефона {to} имеет неверный формат! Не удалось на него отправить sms.");
                        continue;
                    }

                    if (!costInfo.CheckCostSms(to, text, out decimal cost, out int smsCount, out DataForLogging costLogging))
                    {
                        //Logging.Log(costLogging);
                        continue;
                    }

                    if (balance >= cost && remSms >= smsCount && CurDailyLimitSms > 0)
                    {
                        CurDailyLimitSms--;
                        if (!newSms.SendSms(to, taskInfo, out string smsId, out DataForLogging newSmsLogging,  IsTest))
                        {
                            //Logging.Log(newSmsLogging);
                            continue;
                        }

                        var newMessageInfo = new MessageInfo { TaskId = newTask.TaskId, PhoneNumber = to, SmsId = smsId };
                        LocalSentTasks.Add(newMessageInfo);

                        var smsText = newSms.GetSmsText(taskInfo);
                        WriteToDB(DateTime.Now, to, newTask.TaskId, newTask.Priority, balance, newTask.Name, smsId, smsText);

                        balance -= cost;
                        remSms -= smsCount;
                    }
                }
            }
        }

        //public bool Test()
        //{
        //    var tasks1 = new List<MessageInfo>
        //    {
        //        new MessageInfo {TaskId = 2222, PhoneNumber = "79035556677"},
        //        new MessageInfo {TaskId = 3333, PhoneNumber = "79035556677"},
        //        new MessageInfo {TaskId = 3333, PhoneNumber = "79035558899"},
        //    };

        //    var tasks2 = new List<MessageInfo>
        //    {
        //        new MessageInfo {TaskId = 2222, PhoneNumber = "79035556677"},
        //        new MessageInfo {TaskId = 3333, PhoneNumber = "79035550011"},
        //        new MessageInfo {TaskId = 3333, PhoneNumber = "79035558899"},
        //    };

        //    var comparer = new MessageInfoComparer();
        //    tasks1 = (tasks1.Union(tasks2, comparer)).ToList();
        //    return tasks1.Count == 4;
        //}

        private FieldsDetailInfo[] GetCriticalTask(TaskPriorityIndex taskPriorityIndex)
        {
            var FieldsDetailInfos = new FieldsDetailInfo[0];

            // сначала пробуем получить данные из кэша
            var lastCountInfo = Param3TasksWithoutExecutor.GetLastCountInfo;
            var timeSpan = DateTime.Now - lastCountInfo.Date;
            if (timeSpan.TotalMinutes < 1.0 && lastCountInfo.Count == 0)
            {
                return FieldsDetailInfos;
            }

            var lastDetailInfo = Param3TasksWithoutExecutor.GetLastDetailInfo;
            timeSpan = DateTime.Now - lastDetailInfo.Date;
            if (timeSpan.TotalMinutes < 1.0)
            {
                if (lastDetailInfo.FieldsDetailInfos.Length == 0)
                    return FieldsDetailInfos;

                FieldsDetailInfos = lastDetailInfo.FieldsDetailInfos.Where(fi => fi.SortOrder >= (int)taskPriorityIndex).ToArray();
                return FieldsDetailInfos;
            }

            var detailDatas = Param3TasksWithoutExecutor.ParamResult.GetDetailedInfo(Param3TasksWithoutExecutor.ParamResult);
            if (detailDatas == null)
            {
                // ошибка выполнения запроса
                return FieldsDetailInfos;
            }

            if (detailDatas.FieldsTaskInfos.Length == 0)
                return FieldsDetailInfos;

            FieldsDetailInfos = detailDatas.FieldsTaskInfos.Where(fi => fi.SortOrder >= (int)taskPriorityIndex).ToArray();
            return FieldsDetailInfos;
        }

        public bool WriteToDB(DateTime sendDate, string phoneNumber, int taskId, string priorityTask, decimal balanceBefore, string taskName, string smsId, string smsText)
        {
            var strBalanceBefore = balanceBefore.ToString("#0.00", new CultureInfo("en-US"));
            smsRepo.InsertRowToSmsAboutTasks(sendDate, phoneNumber, taskId, priorityTask, strBalanceBefore, taskName, smsId, smsText);
            return true;
        }
        // --truncate table [dbo].[SmsAboutTasks] // очистка таблицы


    }
}
*/