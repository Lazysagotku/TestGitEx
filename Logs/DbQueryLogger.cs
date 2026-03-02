using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeReportV3;

namespace TimeReportV2.Logs
{
    public static class DbQueryLogger
    {
        private static readonly string LogDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        private static readonly string LogFile =
            Path.Combine(LogDir, "db_log.txt");

        public static void Log(string dbSystem, string queryId, double seconds)
        {
            try
            {
                if (!Directory.Exists(LogDir))
                    Directory.CreateDirectory(LogDir);

                var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {dbSystem} | {queryId} | {seconds:F3} sec";

                File.AppendAllText(LogFile, line + Environment.NewLine);
            }
            catch
            {
                // не валим программу из-за логирования
            }
        }
    }

    internal static class Logger
    {
        private static readonly object _lock = new object();
        private static readonly string LogsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        // Ротация: каждый час новый файл
        private static string GetLogFilePath()
        {
            if (!Directory.Exists(LogsDirectory))
                Directory.CreateDirectory(LogsDirectory);

            // Формат: Log_2024-01-15_14.txt (по часам)
            string fileName = $"Log_{DateTime.Now:yyyy-MM-dd_HH}.txt";
            return Path.Combine(LogsDirectory, fileName);
        }

        /// <summary>
        /// Запись запроса в лог
        /// </summary>
        /// <param name="system">IS / Jira</param>
        /// <param name="queryId">ID запроса из справочника</param>
        /// <param name="durationMs">Время выполнения в миллисекундах</param>
        public static void LogQuery(string system, string queryId, long durationMs)
        {
            string userName = MainForm.UserLogin ?? "Unknown";
            string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {system} | {queryId} | {durationMs}ms | User: {userName}{Environment.NewLine}";

            lock (_lock)
            {
                File.AppendAllText(GetLogFilePath(), logLine);
            }
        }

        /// <summary>
        /// Очистка старых логов (старше N дней)
        /// </summary>
        public static void CleanupOldLogs(int daysToKeep = 7)
        {
            try
            {
                if (!Directory.Exists(LogsDirectory))
                    return;

                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

                foreach (var file in Directory.GetFiles(LogsDirectory, "Log_*.txt"))
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        fileInfo.Delete();
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки очистки
            }
        }

        /// <summary>
        /// Открыть папку с логами
        /// </summary>
        public static void OpenLogsFolder()
        {
            if (!Directory.Exists(LogsDirectory))
                Directory.CreateDirectory(LogsDirectory);

            System.Diagnostics.Process.Start("explorer.exe", LogsDirectory);
        }
    }
}
