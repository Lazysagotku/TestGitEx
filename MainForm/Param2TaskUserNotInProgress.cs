using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using System.Linq;

namespace TimeReportV3
{
    public sealed class Param2TaskUserNotInProgress : IParamMainForm
    {
        private readonly UserTasksRepo _repo = new UserTasksRepo();
        public ParamResult ParamResult { get; set; }
        public FieldsDetailInfo[] Details { get; set; }
        public bool IsPossiblyMakeRead { get; set; }
        public ParamResult[] ParamResults { get; set; }
        public Param2TaskUserNotInProgress()
        {
            ParamResult =
                new ParamResult
                {
                    ParameterName = "Количество задач пользователя не в работе",
                    DetailsParameterName = "Задачи пользователя не в работе",
                    Statuses = new List<TaskStatuses> { TaskStatuses.WaitExecutor },
                    Index = 2,
                    GetFullFieldsTaskInfo = GetDetails,
                    GetDetailedInfo = _repo.GetDetailedInfo,
                    SetAsRead = _repo.SetTasksReadByStatus
                };
            ParamResults = new [] { ParamResult };
        }

        public ParamResult[] Get()
        {
            var counts = _repo.GetCountTasksByStatus();
            var count = counts[1];
            ParamResult.Count = count;
            ParamResult.ShowValue = count.ToString();
            ParamResult.IsDetailsAvailable = count > 0;
            if (count > 0)
            {
                var details = _repo.GetDetailedInfo(ParamResult);
                ParamResult.Details = details?.FieldsTaskInfos;
                ParamResult.IsPossiblyMakeRead = details?.IsPossiblyMakeRead ?? false;
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
            return new FullFieldsTaskInfo { FieldsTaskInfos = fieldsDetails.ToArray(), IsPossiblyMakeRead = fieldsDetails.Any(item => item.IsSelected == 1) };
        }

    }
}
