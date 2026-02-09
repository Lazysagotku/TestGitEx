using System.Collections.Generic;
using System.Linq;
using TimeReportV3.Repository;

namespace TimeReportV3.Params
{
    public sealed class JiraParam1ActiveUserTask : IParamMainForm
    {
        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }

        public JiraParam1ActiveUserTask()
        {
            ParamResult = new ParamResult
            {
                ParameterName = "Количество активных задач пользователя (Jira)",
                DetailsParameterName = "Активные задачи пользователя (Jira)",
                Index = 1,
                GetFullFieldsTaskInfo = GetDetails
            };

            ParamResults = new[] { ParamResult };
        }

        public ParamResult[] Get()
        {
            try
            {
                var counts = JiraTasksRepo.GetTasksCounts(MainForm.UserName);
                var count = counts.FirstOrDefault();

                ParamResult.Count = count;
                ParamResult.ShowValue = count.ToString();
                ParamResult.IsDetailsAvailable = count > 0;

                if (count > 0)
                {
                    var details = JiraTasksRepo.GetDetailDatas(
                      ParamResult.Index,
                      MainForm.UserName
                    );

                    ParamResult.Details = details ?? System.Array.Empty<FieldsDetailInfo>();
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

        public FullFieldsTaskInfo GetDetails(IEnumerable<FieldsDetailInfo> fieldsDetails)
        {
            return new FullFieldsTaskInfo
            {
                FieldsTaskInfos = fieldsDetails.ToArray(),
                IsPossiblyMakeRead = false
            };
        }
        /*public FullFieldsTaskInfo GetDetailedInfo(ParamResult paramResult) 
        { 
            var list = JiraTasksRepo.GetDetailDatas(paramResult.Index, MainForm.UserName);
            return new FullFieldsTaskInfo { FieldsTaskInfos = list, IsPossiblyMakeRead = false }; 
        }*/
    }
}
