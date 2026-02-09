using System.Linq;
using TimeReportV3.Repository;

namespace TimeReportV3.Params
{
    public sealed class JiraParam3TasksWithoutExecutor : IParamMainForm
    {
        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }

        public JiraParam3TasksWithoutExecutor()
        {
            ParamResult = new ParamResult
            {
                ParameterName = "Количество задач без исполнителя (Jira)",
                DetailsParameterName = "Задачи без исполнителя (Jira)",
                Index = 3,
                GetDetailedInfo = GetDetailedInfo,
                GetFullFieldsTaskInfo = GetDetails
            };
            ParamResults = new[] { ParamResult };
        }

        public ParamResult[] Get()
        {
            try
            {
                var counts = JiraTasksRepo.GetTasksCounts(MainForm.UserName);
                var value = counts.ElementAtOrDefault(2);

                ParamResult.Count = value;
                ParamResult.ShowValue = value.ToString();
                ParamResult.IsDetailsAvailable = value > 0;

                if (value > 0)
                {
                    var details = JiraTasksRepo.GetDetailDatas(
                      ParamResult.Index,
                      MainForm.UserName
                    );

                    ParamResult.Details = details;
                    ParamResult.IsPossiblyMakeRead = false;
                }
                else
                {
                    ParamResult.Details = System.Array.Empty<FieldsDetailInfo>();
                    ParamResult.IsPossiblyMakeRead = false;
                }
            }
            catch
            {
                ParamResult.Count = 0;
                ParamResult.ShowValue = "0";
                ParamResult.IsDetailsAvailable = false;
                ParamResult.Details = System.Array.Empty<FieldsDetailInfo>();
                ParamResult.IsPossiblyMakeRead = false;
            }

            return ParamResults;
        }

        public FullFieldsTaskInfo GetDetailedInfo(ParamResult paramResult)
        {
            var list = JiraTasksRepo.GetDetailDatas(
              paramResult.Index,
              MainForm.UserName
            );

            return new FullFieldsTaskInfo
            {
                FieldsTaskInfos = list,
                IsPossiblyMakeRead = false
            };
        }

        public FullFieldsTaskInfo GetDetails(System.Collections.Generic.IEnumerable<FieldsDetailInfo> fieldsDetails)
        {
            return new FullFieldsTaskInfo
            {
                FieldsTaskInfos = fieldsDetails?.ToArray() ?? System.Array.Empty<FieldsDetailInfo>(),
                IsPossiblyMakeRead = false
            };
        }
    }
}
