using System.Collections.Generic;
using System.Linq;

namespace TimeReportV3.Params
{
    public sealed class CombinedParam3TasksWithoutExecutor : IParamMainForm
    {
        private readonly Param3TasksWithoutExecutor _isParam;
        private readonly JiraParam3TasksWithoutExecutor _jiraParam;

        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }

        public CombinedParam3TasksWithoutExecutor()
        {
            _isParam = new Param3TasksWithoutExecutor();
            _jiraParam = new JiraParam3TasksWithoutExecutor();

            ParamResult = new ParamResult
            {
                ParameterName = "Количество задач без исполнителя (IS / Jira)",
                DetailsParameterName = "Задачи без исполнителя (IS / Jira)",
                Index = 3,
                GetFullFieldsTaskInfo = GetDetails
            };

            ParamResults = new[] { ParamResult };
        }

        public ParamResult[] Get()
        {
            var isResult = _isParam.Get().First();
            var jiraResult = _jiraParam.Get().First();

            ParamResult.Count = isResult.Count + jiraResult.Count;
            ParamResult.ShowValue = $"{isResult.Count}/{jiraResult.Count}";
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
