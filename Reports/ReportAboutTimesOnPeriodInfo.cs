using System;

namespace TimeReportV3
{
    public class ReportAboutTimesOnPeriodInfo
    {
        public string Service { get; set; }
        public string Project { get; set; }

        public string Customer { get; set; }

        public int TaskId { get; set; }

        public string TaskName { get; set; }

        public string Status { get; set; }
        public string Priority { get; set; }

        public DateTime Created { get; set; }
        public DateTime Deadline { get; set; }

        public DateTime Completed { get; set; }

        public DateTime Closed { get; set; }

        public string Author { get; set; }

        public string Executors { get; set; }

        public int Minutes { get; set; }
    }
}
