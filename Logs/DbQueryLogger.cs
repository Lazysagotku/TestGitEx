using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeReportV3;

namespace TimeReportV2.Logs
{
    /// <summary>
    /// Класс для логирования запросов к БД с ротацией по часам и архивированием
    /// </summary>
    public static class DbQueryLogger
    {
        private static readonly object _lock = new object();
        private static readonly string LogsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static DateTime _lastArchiveCheck = DateTime.MinValue;

        /// <summary>
        /// Получить путь к файлу лога для текущего часа
        /// </summary>
        private static string GetLogFilePath()
        {
            if (!Directory.Exists(LogsDirectory))
                Directory.CreateDirectory(LogsDirectory);

            // Формат: Log_2024-01-15_14.txt (по часам)
            string fileName = $"Log_{DateTime.Now:yyyy-MM-dd_HH}.txt";
            return Path.Combine(LogsDirectory, fileName);
        }

        /// <summary>
        /// Запись запроса в лог (совместимость со старым API)
        /// </summary>
        public static void Log(string dbSystem, string queryId, double seconds)
        {
            long durationMs = (long)(seconds * 1000);
            LogQuery(dbSystem, queryId, durationMs);
        }

        /// <summary>
        /// Запись запроса в лог с временем в миллисекундах
        /// </summary>
        public static void LogQuery(string system, string queryId, long durationMs)
        {
            try
            {
                string userName = "Unknown";
                try { userName = MainForm.UserLogin ?? "Unknown"; } catch { }

                string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {system} | {queryId} | {durationMs}ms | User: {userName}{Environment.NewLine}";

                lock (_lock)
                {
                    File.AppendAllText(GetLogFilePath(), logLine);
                }

                TryArchiveOldLogs();
            }
            catch { }
        }

        /// <summary>
        /// Архивирование логов за прошлые дни
        /// </summary>
        private static void TryArchiveOldLogs()
        {
            try
            {
                if (DateTime.Now.Hour == _lastArchiveCheck.Hour &&
                    DateTime.Now.Date == _lastArchiveCheck.Date)
                    return;

                _lastArchiveCheck = DateTime.Now;

                if (!Directory.Exists(LogsDirectory))
                    return;

                var today = DateTime.Today;
                var logFiles = Directory.GetFiles(LogsDirectory, "Log_*.txt");

                var filesByDate = logFiles
                    .Select(f => new FileInfo(f))
                    .Where(f => f.Name.Length >= 14)
                    .GroupBy(f => f.Name.Substring(4, 10))
                    .Where(g => g.Key != today.ToString("yyyy-MM-dd"))
                    .ToList();

                foreach (var group in filesByDate)
                {
                    string archiveName = Path.Combine(LogsDirectory, $"Logs_{group.Key}.zip");

                    if (File.Exists(archiveName))
                        continue;

                    using (var archive = System.IO.Compression.ZipFile.Open(archiveName, ZipArchiveMode.Create))
                    {
                        foreach (var file in group)
                        {
                            archive.CreateEntryFromFile(file.FullName, file.Name);
                        }
                    }

                    foreach (var file in group)
                    {
                        try { file.Delete(); } catch { }
                    }
                }

                CleanupOldArchives(30);
            }
            catch { }
        }

        public static void CleanupOldArchives(int daysToKeep = 30)
        {
            try
            {
                if (!Directory.Exists(LogsDirectory))
                    return;

                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

                foreach (var file in Directory.GetFiles(LogsDirectory, "Logs_*.zip"))
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        try { fileInfo.Delete(); } catch { }
                    }
                }
            }
            catch { }
        }

        public static void OpenLogsFolder()
        {
            try
            {
                if (!Directory.Exists(LogsDirectory))
                    Directory.CreateDirectory(LogsDirectory);

                System.Diagnostics.Process.Start("explorer.exe", LogsDirectory);
            }
            catch { }
        }
    }

    public class QueryTimer : IDisposable
    {
        private readonly string _system;
        private readonly string _queryId;
        private readonly System.Diagnostics.Stopwatch _stopwatch;

        public QueryTimer(string system, string queryId)
        {
            _system = system;
            _queryId = queryId;
            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            DbQueryLogger.LogQuery(_system, _queryId, _stopwatch.ElapsedMilliseconds);
        }
    }
}
