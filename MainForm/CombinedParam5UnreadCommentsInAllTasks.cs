using System.Collections.Generic;
using System.Linq;

namespace TimeReportV3.Params
{
    public sealed class CombinedParam5UnreadCommentsInAllTasks : IParamMainForm
    {
        private readonly UserTasksRepo _repo = new UserTasksRepo();
        private readonly Param5UnreadCommentsInAllTasks _isParam;
        private readonly JiraParam5UnreadCommentsInAllTasks _jiraParam;

        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }

        public CombinedParam5UnreadCommentsInAllTasks()
        {
            _isParam = new Param5UnreadCommentsInAllTasks();
            _jiraParam = new JiraParam5UnreadCommentsInAllTasks();

            ParamResult = new ParamResult
            {
                ParameterName = "Количество непрочитанных комментариев во всех задачах (IS / -)",
                DetailsParameterName = "Все задачи с непрочитанными комментариями (IS / Jira)",
                Statuses = new List<TaskStatuses> { },
                Index = 5,
                GetFullFieldsTaskInfo = GetDetails,
                SetAsReadIds = ids => _repo.SetTasksReadByIds(ids)
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
            ParamResult.IsPossiblyMakeRead =
              (isResult.IsPossiblyMakeRead || jiraResult.IsPossiblyMakeRead);
            //ParamResult.SetAsRead = (isResult.SetAsRead);

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
                IsPossiblyMakeRead = combined.Any(),
            };
        }
    }
}