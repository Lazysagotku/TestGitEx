using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using Dapper;
using Microsoft.Extensions.Logging;
using TimeReportV2.Logs;

namespace TimeReportV3.Repository
{
    internal static class JiraRepository
    {
        // 🔥 КРИТИЧНО: храним строку подключения в одном месте
        private static string _connectionString;
        private static bool _isInitialized;

        /// <summary>
        /// Инициализация репозитория Jira (вызывать ОДИН раз при старте)
        /// </summary>
        internal static void Init(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Jira connection string is empty");

            _connectionString = connectionString;
            _isInitialized = true;
        }

        private static SqlConnection CreateConnection()
        {
            if (!_isInitialized || string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("JiraRepository is not initialized. Call JiraRepository.Init(...)");

            return new SqlConnection(_connectionString);
        }

        internal static int ExecuteScalarInt(string sql, object param,
    string queryId,
    string dbSystem)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    var sw = Stopwatch.StartNew();
                    var result = conn.ExecuteScalar<int>(sql, param);  // Сначала запрос
                    sw.Stop();  // Потом остановить

                    DbQueryLogger.Log(dbSystem, queryId, sw.Elapsed.TotalSeconds);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Log(sql, param, "ExecuteScalarInt", ex);
                return 0;
            }
        }


        internal static T QuerySingleOrDefault<T>(string sql, object param,
    string queryId,
    string dbSystem)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    var sw = Stopwatch.StartNew();
                    var result = conn.QuerySingleOrDefault<T>(sql, param);  // Сначала запрос
                    sw.Stop();  // Потом остановить

                    DbQueryLogger.Log(dbSystem, queryId, sw.Elapsed.TotalSeconds);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Log(sql, param, "QuerySingleOrDefault", ex);
                return default;
            }
        }

        internal static IEnumerable<T> Query<T>(string sql, object param,
    string queryId,
    string dbSystem)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    var sw = Stopwatch.StartNew();
                    var result = conn.Query<T>(sql, param);  // Сначала запрос
                    sw.Stop();  // Потом остановить

                    DbQueryLogger.Log(dbSystem, queryId, sw.Elapsed.TotalSeconds);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Log(sql, param, "Query", ex);
                return Array.Empty<T>();
            }
        }

        private static void Log(string sql, object param, string method, Exception ex)
        {
            MessageBox.Show(
              $"{method} error: {ex.Message}\nSQL:\n{sql}\nParams: {param}"
            );
        }
    }
}