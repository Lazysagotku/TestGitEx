using System;

namespace TimeReportV3
{
    internal sealed class ScheduleDay
    {
        public DateTime Date { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public bool IsWorkingDay { get; set; }
    }

    public enum Answer { Yes, No, Error}
    internal sealed class PeriodForSendingSms
    {
        private ScheduleDay ScheduleDay;
        private readonly SmsRepo smsRepo = new SmsRepo();

        public PeriodForSendingSms()
        {
        }

        /*public Answer IsNeedToSend(DateTime date)
        {
            // Один раз в сутки сохраняем данные о расписании рабочего дня
            if (ScheduleDay == null || ScheduleDay.Date != date.Date)
            {
                var answer =  UpdateSheduleDay(date);
                if (answer == Answer.Error)
                    return answer;
            }

            if (ScheduleDay.IsWorkingDay)
            {
                if(!TimeSpan.TryParse(ScheduleDay.TimeStart, out TimeSpan timeBegin) ||
                   !TimeSpan.TryParse(ScheduleDay.TimeEnd, out TimeSpan timeEnd))
                {
                    return Answer.Error;
                }

                if (date.TimeOfDay >= timeBegin && date.TimeOfDay <= timeEnd)
                {
                    return Answer.No;
                }
            }

            return Answer.Yes;
        }

        private Answer UpdateSheduleDay(DateTime date)
        {
            var answer = IsScheduleDayException(date, out ScheduleDay scheduleDay);
            if (answer == Answer.No)
            {
                answer = GetScheduleDay(date, out scheduleDay);
            }

            ScheduleDay = scheduleDay;
            return answer;
        }

          private Answer IsScheduleDayException(DateTime date, out ScheduleDay scheduleDay)
        {
            scheduleDay = null;
            var scheduleException = smsRepo.GetScheduleException(date);
            if (scheduleException == null)
            {
                return Answer.No;
            }
            scheduleDay = scheduleException;
            scheduleDay.IsWorkingDay = !string.IsNullOrEmpty(scheduleDay.TimeStart);
            return Answer.Yes;
        }

        /// <summary>
        /// Определяет расписание рабочего дня (для выходных дней выдаст ошибку!)
        /// </summary>
        /// <returns></returns>
        private Answer GetScheduleDay(DateTime date, out ScheduleDay scheduleDay)
        {
            try
            {
                scheduleDay = smsRepo.GetScheduleDay((int)date.DayOfWeek);
                if (scheduleDay == null)
                {
                    scheduleDay = new ScheduleDay { Date = date.Date, IsWorkingDay = false };
                    return Answer.Yes;
                }
                scheduleDay.Date = date.Date;
                scheduleDay.IsWorkingDay = !string.IsNullOrEmpty(scheduleDay.TimeStart);
                return Answer.Yes;
            }
            catch
            {
                scheduleDay = null;
                return Answer.Error;
            }
        }*/
    }
}
