using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Linq;

namespace TimeReportV3.Params
{
    public sealed class CombinedSumTimeParam : IParamMainForm
    {
        private readonly Param6TimeSpentByUser _isParam;
        private readonly JiraParam6TimeSpentByUser _jiraParam;

        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }

        public CombinedSumTimeParam()
        {
            _isParam = new Param6TimeSpentByUser();
            _jiraParam = new JiraParam6TimeSpentByUser();

            ParamResult = new ParamResult
            {
                ParameterName = "Сумма по времени (IS / Jira)",
                DetailsParameterName = "Суммарное списанное время",
                Index = 7,
                IsMinutesUsed = true,
                GetFullFieldsTaskInfo = GetDetails
            };

            ParamResults = new[] { ParamResult };
        }

        public ParamResult[] Get()
        {
            var isResult = _isParam.Get().First();
            var jiraResult = _jiraParam.Get().First();

            int totalMinutes = isResult.Count + jiraResult.Count;

            ParamResult.Count = totalMinutes;
            ParamResult.ShowValue = $"{totalMinutes / 60:D2}:{totalMinutes % 60:D2}";
            ParamResult.IsDetailsAvailable =
              isResult.IsDetailsAvailable || jiraResult.IsDetailsAvailable;

            ParamResult.Details = GetDetails(null).FieldsTaskInfos;
            ParamResult.IsPossiblyMakeRead = false;

            var remainingParam = GetRemainingTimeParam(totalMinutes);

            return remainingParam != null
              ? new[] { ParamResult, remainingParam }
              : new[] { ParamResult };
        }
        public static ParamResult GetRemainingTimeParam(int spentMinutes)
        {
            var spentTime = new TimeSpan(0, spentMinutes, 0);
            var workingDay = new TimeSpan(8, 0, 0);

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
            var isDetails = _isParam.ParamResult.Details
                    ?? System.Array.Empty<FieldsDetailInfo>();

            var jiraDetails = _jiraParam.ParamResult.Details
                     ?? System.Array.Empty<FieldsDetailInfo>();

            var combined = isDetails
              .Concat(jiraDetails)
              .OrderByDescending(x => x.Created)
              .ToArray();

            return new FullFieldsTaskInfo
            {
                FieldsTaskInfos = combined,
                IsPossiblyMakeRead = false
            };
        }
    }
}