using System;

namespace TimeReportV3
{
    public class MassTimestampInfo
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string Service { get; set; }
        public string Status { get; set; }
        public string Author { get; set; }
        public string Executors { get; set; }
        public int ExecutorsCount { get; set; }
        public string Priority { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime Created { get; set; }
    }
}
