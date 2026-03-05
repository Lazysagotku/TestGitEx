using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

namespace TimeReportV3.Params
{
    public sealed class CombinedParam2TaskUserNotInProgress : IParamMainForm
    {
        private readonly Param2TaskUserNotInProgress _isParam;
        private readonly JiraParam2TaskUserNotInProgress _jiraParam;

        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }

        public CombinedParam2TaskUserNotInProgress()
        {
            _isParam = new Param2TaskUserNotInProgress();
            _jiraParam = new JiraParam2TaskUserNotInProgress();

            ParamResult = new ParamResult
            {
                ParameterName = "Количество задач пользователя не в работе (IS / Jira)",
                DetailsParameterName = "Задачи пользователя не в работе (IS / Jira)",
                Index = 2,
                GetFullFieldsTaskInfo = GetDetails
            };

            ParamResults = new[] { ParamResult };
        }

        public ParamResult[] Get()
        {
            var isTask = Task.Run(() => _isParam.Get().First());
            var jiraTask = Task.Run(() => _jiraParam.Get().First());

            //Task.WaitAll(isTask, jiraTask);  // Параллельное выполнение!

            Task task = Task.WhenAll(isTask, jiraTask);
            task.Wait();

            var isResult = isTask.Result;
            var jiraResult = jiraTask.Result;

            ParamResult.Count = isResult.Count + jiraResult.Count;
            ParamResult.ShowValue = $"{isResult.Count}/{jiraResult.Count}";
            ParamResult.IsDetailsAvailable =
              isResult.IsDetailsAvailable || jiraResult.IsDetailsAvailable;

            ParamResult.Details = GetDetails(null).FieldsTaskInfos;
            ParamResult.IsPossiblyMakeRead =
              isResult.IsPossiblyMakeRead || jiraResult.IsPossiblyMakeRead;

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
                IsPossiblyMakeRead = combined.Any(x => x.IsSelected == 1)
            };
        }
    }
}