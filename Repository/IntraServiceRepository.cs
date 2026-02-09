using Npgsql;
using System.Data;

namespace TimeReportV3.Repository
{
    internal static class IntraServiceRepository
    {
        private static string _cs;

        internal static void Init(string connectionString)
        {
            _cs = connectionString;
        }

        internal static DataTable Query(string sql)
        {
            var conn = new NpgsqlConnection(_cs);
            conn.Open();

            var cmd = new NpgsqlCommand(sql, conn);
            var reader = cmd.ExecuteReader();

            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }
    }
}