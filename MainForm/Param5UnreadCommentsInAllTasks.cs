using System.Collections.Generic;
using System.Linq;

namespace TimeReportV3
{
    internal sealed class Param5UnreadCommentsInAllTasks : IParamMainForm
    {
        private readonly UserTasksRepo _repo = new UserTasksRepo();

        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }
        public Param5UnreadCommentsInAllTasks()
        {
            ParamResult =
                new ParamResult
                {
                    ParameterName = "Количество непрочитанных комментариев во всех задачах",
                    DetailsParameterName = "Все задачи с непрочитанными комментариями",
                    Statuses = new List<TaskStatuses> { },
                    Index = 5,
                    GetDetailedInfo = _repo.GetDetailedInfo,
                    GetFullFieldsTaskInfo = GetDetails,
                    SetAsRead = _repo.SetTasksReadByStatus
                };
            ParamResults = new ParamResult[] { ParamResult };
        }
        public ParamResult[] Get()
        {
            var counts = _repo.GetCountTasksByStatus();
            var count = counts[4];

            ParamResult.Count = count;
            ParamResult.ShowValue = count.ToString();
            ParamResult.IsDetailsAvailable = count > 0;

            if (count > 0)
            {
                var details = _repo.GetDetailedInfo(ParamResult);
                ParamResult.Details = details?.FieldsTaskInfos;
                ParamResult.IsPossiblyMakeRead = true;
            }
            else
            {
                ParamResult.Details = System.Array.Empty<FieldsDetailInfo>();
                ParamResult.IsPossiblyMakeRead = false;
            }

            return ParamResults;
        }
         public FullFieldsTaskInfo GetDetails(IEnumerable<FieldsDetailInfo> fieldsDetails)
        {
            var arr = fieldsDetails?.ToArray() ?? System.Array.Empty<FieldsDetailInfo>();

            return new FullFieldsTaskInfo
            {
                FieldsTaskInfos = arr,
                IsPossiblyMakeRead = arr.Any()
            };
        }
    }
}
