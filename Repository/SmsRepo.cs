using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeReportV3
{
    internal class SmsRepo : BaseRepository
    {
        public ScheduleDay GetScheduleDay(int dayofweek)
        {
            // style 8 -> из даты берем только время
            return Query<ScheduleDay>($@"
                select 
                    {GetDateFormatSqlFunc("sd.[TimeStart]", "HH:mm:ss")} as [TimeStart]
                    {GetDateFormatSqlFunc("sd.[TimeEnd]", "HH:mm:ss")} as [TimeEnd]
                from [ScheduleDay] sd
                where sd.[TimeStart] is not null and sd.[Day] = {dayofweek}")
                .FirstOrDefault();
        }
        public ScheduleDay GetScheduleException(DateTime date)
        {
            return Query<ScheduleDay>($@"
                select cast(se.[Date] as date) as [Date], se.[TimeStart], se.[TimeEnd] 
                from [ScheduleDayException] se 
                where se.[Date] = '{date:yyyy-MM-dd}'")
                .FirstOrDefault();
        }

       /* public IEnumerable<MessageInfo> GetSentSms(List<int> taskIds)
        {
            return Query<MessageInfo>($@"
                select t.[TaskId], t.[PhoneNumber], t.[SmsId]
                from [SmsAboutTasks] t 
                where t.[TaskId] in ({string.Join(", ", taskIds)})");
        }*/

        public void InsertRowToSmsAboutTasks(DateTime sendDate, string phoneNumber, int taskId, string priorityTask, string balanceBefore, string taskName, string smsId, string smsText)
        {
            QueryUpsert($@"
            insert into [SmsAboutTasks] ([SendDate], [PhoneNumber], [TaskId], [PriorityTask], [BalanceBefore], [TaskName], [SmsId], [SmsText])
            values ('{sendDate:yyyy-MM-dd HH:mm:ss}', '{phoneNumber}', {taskId}, '{priorityTask}', {balanceBefore}, '{taskName}', '{smsId}', '{smsText}')");
        }
    }
}
