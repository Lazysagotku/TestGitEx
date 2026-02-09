using System.Collections.Generic;
using System.Linq;

namespace TimeReportV3.Params
{
    public sealed class JiraParam5UnreadCommentsInAllTasks : IParamMainForm
    {
        private readonly UserTasksRepo _repo = new UserTasksRepo();
        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }

        public JiraParam5UnreadCommentsInAllTasks()
        {
            ParamResult = new ParamResult
            {
                ParameterName = "Непрочитанные комментарии во всех задачах (Jira)",
                DetailsParameterName = "Непрочитанные комментарии (все) (Jira)",
                Statuses = new List<TaskStatuses> { },
                Index = 5,
                GetDetailedInfo = _repo.GetDetailedInfo,
                GetFullFieldsTaskInfo = GetDetails,
                SetAsRead = _repo.SetTasksReadByStatus
            };

            ParamResults = new[] { ParamResult };
        }

        public ParamResult[] Get()
        {

            ParamResult.Count = 0;
            ParamResult.ShowValue = "-";
            ParamResult.IsDetailsAvailable = false;

            ParamResult.Details = System.Array.Empty<FieldsDetailInfo>();
            ParamResult.IsPossiblyMakeRead = true;

            return ParamResults;
        }

        public FullFieldsTaskInfo GetDetails(System.Collections.Generic.IEnumerable<FieldsDetailInfo> fieldsDetails)
        {
            return new FullFieldsTaskInfo
            {
                FieldsTaskInfos = fieldsDetails?.ToArray() ?? System.Array.Empty<FieldsDetailInfo>(),
                IsPossiblyMakeRead = true
            };
        }
    }
}