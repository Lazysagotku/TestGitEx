using System.Collections.Generic;
using System.Linq;

namespace TimeReportV3.Params
{
    public sealed class CombinedParam6TimeSpentByUser : IParamMainForm
    {
        private readonly Param6TimeSpentByUser _isParam;
        private readonly JiraParam6TimeSpentByUser _jiraParam;

        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }

        public CombinedParam6TimeSpentByUser()
        {
            _isParam = new Param6TimeSpentByUser();
            _jiraParam = new JiraParam6TimeSpentByUser();

            ParamResult = new ParamResult
            {
                ParameterName = "Время, списанное пользователем за текущий день (IS / Jira)",
                DetailsParameterName = "Время списанное (IS / Jira)",
                Index = 6,
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

            return ParamResults;
        }

        public FullFieldsTaskInfo GetDetails(IEnumerable<FieldsDetailInfo> _)
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