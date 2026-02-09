using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeReportV3
{
    internal sealed class Param6TimeSpentByUser : IParamMainForm
    {
        private static readonly TimeSpan WorkingDay = new TimeSpan(8, 0, 0);
        private readonly UserTasksRepo _repo = new UserTasksRepo();

        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }

        public Param6TimeSpentByUser()
        {
            ParamResult =
                new ParamResult
                {
                    ParameterName = "Время, списанное пользователем на задачи в течение текущего дня",
                    DetailsParameterName = "Задачи, на которые списанно время в течение текущего дня",
                    SetAsRead = _repo.SetTasksReadByStatus,
                    NameDo = "",
                    Index = 6,
                    IsMinutesUsed = true,
                    GetDetailedInfo = _repo.GetDetailedInfo,
                    GetFullFieldsTaskInfo = GetDetails,
                };
            ParamResults = new[] { ParamResult };
        }

        public ParamResult[] Get()
        {
            var counts = _repo.GetCountTasksByStatus();
            var spentMinutes = counts[5];

            // ОСНОВНОЙ параметр
            ParamResult.Count = spentMinutes;
            ParamResult.ShowValue = $"{spentMinutes / 60:D2}:{spentMinutes % 60:D2}";
            ParamResult.IsDetailsAvailable = spentMinutes > 0;

            if (spentMinutes > 0)
            {
                var details = _repo.GetDetailedInfo(ParamResult);
                ParamResult.Details = details?.FieldsTaskInfos;
                ParamResult.IsPossiblyMakeRead = false;
            }
            else
            {
                ParamResult.Details = System.Array.Empty<FieldsDetailInfo>();
                ParamResult.IsPossiblyMakeRead = false;
            }

            // ВТОРОЙ параметр — remaining / overtime
            var remainingParam = GetRemainingTimeParam(spentMinutes);

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

        public FullFieldsTaskInfo GetDetails(IEnumerable<FieldsDetailInfo> fieldsDetails)
        {
            var arr = fieldsDetails?
              .OrderByDescending(f => f.Created)
              .ToArray()
              ?? System.Array.Empty<FieldsDetailInfo>();

            return new FullFieldsTaskInfo
            {
                FieldsTaskInfos = arr,
                IsPossiblyMakeRead = false
            };
        }
    }
}
