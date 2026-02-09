using System.Collections.Generic;
using System.Linq;

namespace TimeReportV3.Params
{
    public sealed class JiraParam4UnreadCommentsInUserTasks : IParamMainForm
    {
        private readonly UserTasksRepo _repo = new UserTasksRepo();
        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }

        public JiraParam4UnreadCommentsInUserTasks()
        {
            ParamResult = new ParamResult
            {
                ParameterName = "Количество непрочитанных комментариев в задачах пользователя (Jira)   ",
                DetailsParameterName = "Непрочитанные комментарии (Jira)",
                Statuses = new List<TaskStatuses> { },
                Index = 4,
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