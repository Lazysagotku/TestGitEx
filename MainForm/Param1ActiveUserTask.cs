using System.Collections.Generic;
using System.Linq;
using TimeReportV3;

public sealed class Param1ActiveUserTask : IParamMainForm
{
    private readonly UserTasksRepo _repo = new UserTasksRepo();

    public ParamResult ParamResult { get; set; }
    public ParamResult[] ParamResults { get; set; }

    public Param1ActiveUserTask()
    {
        ParamResult = new ParamResult
        {
            ParameterName = "Количество активных задач пользователя",
            DetailsParameterName = "Активные задачи пользователя",
            Index = 1,
            GetFullFieldsTaskInfo=GetDetails,
            GetDetailedInfo = _repo.GetDetailedInfo,
            SetAsRead = _repo.SetTasksReadByStatus
        };

        ParamResults = new[] { ParamResult };
    }

    public ParamResult[] Get() 
    {
        var counts = _repo.GetCountTasksByStatus();
        var count = counts[0];
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
        return new FullFieldsTaskInfo
        {
            FieldsTaskInfos = fieldsDetails.ToArray(),
            IsPossiblyMakeRead = false
        };
    }
}