using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeReportV3
{
    /// <summary>
    /// Эти значения взяты из БД intraservice из таблицы [Priority], поле SortOrder
    /// </summary>
    public enum TaskPriorityIndex { Low = 1, Normal = 2, High = 3, Critical = 4 }

    public struct CountInfo
    {
        public DateTime Date {get; set;}
        public int Count { get; set; }
    }

    public struct DetailInfo
    {
        public DateTime Date { get; set; }
        public FieldsDetailInfo[] FieldsDetailInfos { get; set; }
    }

    internal sealed class Param3TasksWithoutExecutor : IParamMainForm
    {
        private readonly UserTasksRepo _repo = new UserTasksRepo();

        private readonly object lockerCountInfo = new object();
        private readonly object lockerDetailInfo = new object();

        private CountInfo LastCountInfo;
        private DetailInfo LastDdetailInfo;

        public ParamResult ParamResult { get; set; }
        public ParamResult[] ParamResults { get; set; }
        public Param3TasksWithoutExecutor()
        {
            ParamResult = new ParamResult
            {
                ParameterName = "Количество задач без исполнителя",
                DetailsParameterName = "Задачи без исполнителя",
                Statuses = new List<TaskStatuses>
        {
          TaskStatuses.InWork,
          TaskStatuses.NeedsDetails,
          TaskStatuses.Suspended,
          TaskStatuses.WaitExecutor
        },
                Index = 3,
                GetDetailedInfo = _repo.GetDetailedInfo,
                GetFullFieldsTaskInfo = GetDetails,
                SetAsRead = null
            };

            ParamResults = new[] { ParamResult };
        }

        public CountInfo GetLastCountInfo 
        { 
            get
            {
                lock (lockerCountInfo)
                {
                    return LastCountInfo;
                }
            }
        }

        public DetailInfo GetLastDetailInfo
        {
            get
            {
                lock (lockerDetailInfo)
                {
                    return LastDdetailInfo;
                }
            }
        }

        public ParamResult[] Get()
        {

            var counts = _repo.GetCountTasksByStatus();
            var count = counts[2];

            ParamResult.Count = count;
            ParamResult.ShowValue = count.ToString();
            ParamResult.IsDetailsAvailable = count > 0;

            if (count > 0)
            {
                var details = _repo.GetDetailedInfo(ParamResult);
                ParamResult.Details = details?.FieldsTaskInfos;
                ParamResult.IsPossiblyMakeRead = false;

                lock (lockerDetailInfo)
                {
                    LastDdetailInfo = new DetailInfo
                    {
                        Date = DateTime.Now,
                        FieldsDetailInfos = ParamResult.Details
                    };
                }
            }
            else
            {
                ParamResult.Details = Array.Empty<FieldsDetailInfo>();
                ParamResult.IsPossiblyMakeRead = false;
            }

            lock (lockerCountInfo)
            {
                LastCountInfo = new CountInfo
                {
                    Date = DateTime.Now,
                    Count = count
                };
            }

            return ParamResults;
        }

        public FullFieldsTaskInfo GetDetails(IEnumerable<FieldsDetailInfo> fieldsDetails)
        {
            var arr = fieldsDetails?.ToArray() ?? Array.Empty<FieldsDetailInfo>();

            lock (lockerDetailInfo)
            {
                LastDdetailInfo = new DetailInfo
                {
                    Date = DateTime.Now,
                    FieldsDetailInfos = arr
                };
            }

            return new FullFieldsTaskInfo
            {
                FieldsTaskInfos = arr,
                IsPossiblyMakeRead = false
            };
        }

    }
}
