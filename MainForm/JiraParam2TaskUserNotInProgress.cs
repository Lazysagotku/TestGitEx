using System;
using System.Linq;
using TimeReportV3.Repository;

namespace TimeReportV3.Params
{
    public sealed class JiraParam2TaskUserNotInProgress : IParamMainForm
    {
        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }

        public JiraParam2TaskUserNotInProgress()
        {
            ParamResult = new ParamResult
            {
                ParameterName = "Количество задач пользователя не в работе (Jira)",
                DetailsParameterName = "Задачи пользователя не в работе (Jira)",
                Index = 2,

                // 👇 важно: единый механизм
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
                var value = counts.ElementAtOrDefault(1); // 👈 индекс для Param2

                ParamResult.Count = value;
                ParamResult.ShowValue = value.ToString();
                ParamResult.IsDetailsAvailable = value > 0;

                if (value >= 0)
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
                    ParamResult.Details = Array.Empty<FieldsDetailInfo>();
                    ParamResult.IsPossiblyMakeRead = false;
                }
            }
            catch (Exception ex)
            {
                // ❗ КРИТИЧНО: Jira может быть недоступна
                ParamResult.Count = 0;
                ParamResult.ShowValue = "0";
                ParamResult.IsDetailsAvailable = false;
                ParamResult.Details = Array.Empty<FieldsDetailInfo>();
                ParamResult.IsPossiblyMakeRead = false;

                System.Diagnostics.Debug.WriteLine(ex);
            }

            return ParamResults;
        }

        // используется MainTable при клике
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

        // используется, если данные уже подгружены
        public FullFieldsTaskInfo GetDetails(
      System.Collections.Generic.IEnumerable<FieldsDetailInfo> fieldsDetails)
        {
            var arr = fieldsDetails?.ToArray()
                 ?? Array.Empty<FieldsDetailInfo>();

            return new FullFieldsTaskInfo
            {
                FieldsTaskInfos = arr,
                IsPossiblyMakeRead = false
            };
        }
    }
}