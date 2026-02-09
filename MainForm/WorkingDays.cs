using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeReportV3
{
    internal class WorkingDays
    {
        private static WorkingDays Instance;
        private DateTime DateLastRequest;
        private IEnumerable<string> ActualWorkingDays;

        private WorkingDays()
        {
        }

        public static WorkingDays GetInstance()
        {
            if (Instance == null)
                Instance = new WorkingDays();

            return Instance;
        }

        public IEnumerable<string> Get()
        {
            var dateRequest = DateTime.Now.Date;
            if (dateRequest == DateLastRequest)
            {
                return ActualWorkingDays;
            }

            var wds = BaseRepository.GetWorkingDayFromDbAlef();
            if (wds?.Count() > 0)
            {
                ActualWorkingDays = wds;
                DateLastRequest = dateRequest;
            }

            return wds;
        }

    }
}
