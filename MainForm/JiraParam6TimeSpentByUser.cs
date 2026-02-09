using System;
using System.Linq;
using TimeReportV3.Repository;

namespace TimeReportV3.Params
{
    public sealed class JiraParam6TimeSpentByUser : IParamMainForm
    {
        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }

        public JiraParam6TimeSpentByUser()
        {
            ParamResult = new ParamResult
            {
                ParameterName = "Время, списанное пользователем за текущий день (Jira)",
                DetailsParameterName = "Время списанное (Jira)",
                Index = 6,
                IsMinutesUsed = true,
                GetFullFieldsTaskInfo = GetDetails
            };

            ParamResults = new[] { ParamResult };
        }

        public ParamResult[] Get()
        {
            int minutes = 0;

            try
            {
                var counts = JiraTasksRepo.GetTasksCounts(MainForm.UserName);
                minutes = counts.ElementAtOrDefault(5);
            }
            catch
            {
                // Jira может быть недоступна — не валим форму
                minutes = 0;
            }

            ParamResult.Count = minutes;
            ParamResult.ShowValue = $"{minutes / 60:D2}:{minutes % 60:D2}";
            ParamResult.IsDetailsAvailable = minutes > 0;

            if (minutes > 0)
            {
                var details = GetDetails(null);
                ParamResult.Details = details.FieldsTaskInfos;
                ParamResult.IsPossiblyMakeRead = false;
            }
            else
            {
                ParamResult.Details = System.Array.Empty<FieldsDetailInfo>();
                ParamResult.IsPossiblyMakeRead = false;
            }

            var remainingParam = GetRemainingTimeParam(minutes);

            return remainingParam != null
              ? new[] { ParamResult, remainingParam }
              : new[] { ParamResult };
        }
        public static ParamResult GetRemainingTimeParam(int spentMinutes)
        {
            var spentTime = new TimeSpan(0, spentMinutes, 0);
            var workingDay = new TimeSpan(6, 0, 0);

            if (spentTime == workingDay)
                return null;

            var ts = workingDay - spentTime;

            return new ParamResult
            {
                ParameterName = spentTime < workingDay
        ? "Время, оставшееся до завершения рабочего дня"
        : "Время переработки текущего дня",

                ShowValue = $"{ts:hh\\:mm}",

                IsDetailsAvailable = false,
                //Index = -1 // 🔥 КРИТИЧНО: не участвует в кликах
            };
            /*var spentTime = new TimeSpan(0, spentMinutes, 0);

            if (WorkingDay == spentTime)
            {
                return null; // ParamResults;
            }
            var parameterName = spentTime < WorkingDay
                ? "Время, оставшееся до завершения рабочего дня"
                : "Время переработки текущего дня";
            var ts = WorkingDay - spentTime;
            return
                new ParamResult
                {
                    ParameterName = parameterName,
                    ShowValue = $"{ts:hh\\:mm}",
                    IsDetailsAvailable = false,
                };
            var spentTime = new TimeSpan(0, spentMinutes, 0);

            if (spentTime == WorkingDay)
                return null;

            var ts = WorkingDay - spentTime;

            return spentTime < WorkingDay
              ? $"Время, оставшееся до завершения рабочего дня: {ts:hh\\:mm}"
              : $"Время переработки текущего дня: {ts:hh\\:mm}";*/
        }
        public FullFieldsTaskInfo GetDetails(System.Collections.Generic.IEnumerable<FieldsDetailInfo> _)
        {
            var list = JiraTasksRepo.GetDetailDatas(6, MainForm.UserName);

            return new FullFieldsTaskInfo
            {
                FieldsTaskInfos = list ?? System.Array.Empty<FieldsDetailInfo>(),
                IsPossiblyMakeRead = false
            };
        }
    }
}