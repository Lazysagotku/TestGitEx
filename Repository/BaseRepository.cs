using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TimeReportV2.Logs;

namespace TimeReportV3
{
    internal class BaseRepository
    {
        private static bool isPostgre { get; } = Properties.Settings.Default.TargetDB == "Postgre SQL";
        private static Func<DbConnection> BaseConnMaker { get; set; }
        private static Func<DbConnection> AlfConnMaker { get; set; }

        protected static SqlConnection JiraConnection;

        protected static NpgsqlConnection IsConnection;


        public static NpgsqlConnection GetConnection()
        {
            if (IsConnection == null)
            {
                var settings = Properties.Settings.Default;
                var connectionString = $"Server={settings.AddressOfServerIS};" + $"Port=5432;User Id = alefsupport; Password = 2r8laTI.omE; Database = intraservice5;";
                IsConnection = new NpgsqlConnection(connectionString);

                //string connString = $"Server={settings.AddressOfServerIS};" +$"Port=5432;"User Id=alefsupport;Password=2r8laTI.omE;Database=intraservice5;";
            }
            return IsConnection;
        }

        public static SqlConnection GetJiraConnection() 
        {
            if (JiraConnection == null) 
            { 
                var settings = Properties.Settings.Default; 
                var connectionString = $"Server={settings.AddressOfServerJira};" + $"Database=JiraSoftwareDB;" + $"User Id={settings.LoginJira};" + $"Password={settings.PasswordJira};"; 
                JiraConnection = new SqlConnection(connectionString); } 
            return JiraConnection; }

        internal static IEnumerable<User> InitRepository() 
        {
            foreach (ConnectionStringSettings conn in ConfigurationManager.ConnectionStrings)
            {
                if (conn.Name.StartsWith("Local"))
                {
                    continue;
                }
                
                if (conn.Name == "al")
                {
                    string result = $"{conn.ConnectionString}{HandleTail(Properties.Settings.Default[$"Cr{conn.Name}"].ToString())}";
                    AlfConnMaker = () => { return new SqlConnection(result); };
                }
                else if (!isPostgre && conn.ProviderName == "System.Data.Client")
                {
                    string result = $"{conn.ConnectionString}{HandleTail(Properties.Settings.Default[$"Cr{conn.Name}"].ToString())}";
                    BaseConnMaker = () => { return new SqlConnection(result); };
                }
                else if (isPostgre && conn.ProviderName == "Npgsql")
                {
                    string result = $"{conn.ConnectionString}{HandleTail(Properties.Settings.Default[$"Cr{conn.Name}"].ToString())}";
                    BaseConnMaker = () => { return new NpgsqlConnection(result); };
                }
            }
            return UserTasksRepo.GetUsers();
        }

        protected static IEnumerable<T> Query<T>(
    string query,
    string queryId,
    string dbSystem,
    bool needReplaceChars = true)
        {
            string handledQuery = "";

            try
            {
                using (var dbConn = BaseConnMaker.Invoke())
                {
                    dbConn.Open();

                    handledQuery = needReplaceChars ? SetTargetChars(query) : query;

                    var sw = Stopwatch.StartNew();

                    var result = dbConn.Query<T>(handledQuery).ToArray();

                    sw.Stop();

                    DbQueryLogger.Log(dbSystem, queryId, sw.Elapsed.TotalSeconds);

                    return result;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.txt",
                    $"{DateTime.Now}\n{ex.Message}\n{handledQuery}\n\n");

                return new T[] { };
            }
        }

        protected static int Query(
    string query,
    string queryId,
    string dbSystem)
        {
            try
            {
                using (var dbConn = BaseConnMaker.Invoke())
                {
                    dbConn.Open();

                    string handledQuery = SetTargetChars(query);

                   

                    int result = Convert.ToInt32(dbConn.ExecuteScalar(handledQuery));
                    var sw = Stopwatch.StartNew();
                    sw.Stop();

                    DbQueryLogger.Log(dbSystem, queryId, sw.Elapsed.TotalSeconds);

                    return result;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.txt",
                    $"{DateTime.Now}\n{ex.Message}\n{ex.StackTrace}\n\n");

                return -222;
            }
        }
        protected static List<int> ExecScalarQueries(List<string> queries,
    string queryId,
    string dbSystem)
        {
            List<int> results = new List<int>();
            try
            {
                using (var dbConn = BaseConnMaker.Invoke())
                {
                    dbConn.Open(); 
                    var sw = Stopwatch.StartNew();
                    foreach (var query in queries)
                    {
                        string handledQuery = SetTargetChars(query);
                        int result = Convert.ToInt32(dbConn.ExecuteScalar(handledQuery));
                        results.Add(result);
                    }

                    sw.Stop();

                    DbQueryLogger.Log(dbSystem, queryId, sw.Elapsed.TotalSeconds);
                    return results;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.txt", $"{ex.Message}\n{ex.StackTrace}");
                return results;
            }
        }
        protected static List<string> ExecList(string query)
        {
            using (var dbConn = BaseConnMaker.Invoke())
            {
                dbConn.Open();
                return dbConn.Query<string>(query).ToList();
            }
        }

        protected static bool QueryUpsert(string query, string queryId, string dbSystem)
        {
            try
            {
                using (var dbConn = BaseConnMaker.Invoke())
                {
                    dbConn.Open();

                    string handledQuery = SetTargetChars(query);

                    var sw = Stopwatch.StartNew();

                    dbConn.Execute(SetTargetChars(query));

                    sw.Stop();

                    DbQueryLogger.Log(dbSystem, queryId, sw.Elapsed.TotalSeconds);

                    var result = dbConn.Execute(SetTargetChars(query));
                    return true;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.txt", $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
        

        internal static IEnumerable<string> GetWorkingDayFromDbAlef()
        {
            var dateBegin = DateTime.Now.AddDays(-180);
            // DocID = 1720040351 - выбираем календарь рабочих дней (В похожей по названию таблице указано, какие DocId содержат рабочие дни, какие торговые дни и т.п.)
            string query =
                $@"select [uf_l_date]
                from [DocALF_Calendar_list_1532]
                where [uf_l_date] between '{dateBegin:yyyy-MM-dd}' and getdate() and DocID = 1720040351 and uf_l_is_workday = 1
                order by [uf_l_date] desc";
            string handledQuery;
            try
            {
                using (var dbConn = AlfConnMaker.Invoke())
                {
                    dbConn.Open();
                    handledQuery = query;
                    var result = dbConn.Query<object>(query)?.Select(dapperRow => $"{((IDictionary<string, object>)dapperRow)["uf_l_date"]:yyyy-MM-dd}");
                    dbConn.Close();
                    return result;
                    //return Query<object>(query, false, true)?.Select(dapperRow => $"{((IDictionary<string, object>)dapperRow)["uf_l_date"]:yyyy-MM-dd}");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.txt", $"{ex.Message}\n{ex.StackTrace}");
                return new string[] { };
            }
        }
        protected static string GetLengthSqlName()
        {
            if (isPostgre)
            {
                return "length";
            }
            return "len";
        }
        protected static string GetXmlSqlFunc(string fieldName, string pathToNode, string alias, string outSqlType)
        {
            if (isPostgre)
            {
                return $"(xpath('{pathToNode}/text()', {fieldName}))&#91;1&#93;::{outSqlType} as {alias}";
            }
            return $"{fieldName}.value('({pathToNode})&#91;1&#93;', '{outSqlType}') as {alias}";
        }
        protected static string GetCharindexSqlFunc(string fieldName, string substr, string alias = "", string outSqlType = "")
        {
            if (isPostgre)
            {
                return $"position({substr} in {fieldName}) {alias}";
            }
            return $"CHARINDEX({substr}, {fieldName}) {alias}";
        }
        protected static string GetDateFormatSqlFunc(string fieldName, string format)
        {
            if (isPostgre)
            {
                return $"to_char({fieldName}, '{format}')";
            }
            return $"format({fieldName}, '{format}')";
        }
        protected static string GetDateTimeNowSqlFunc(bool isDateOnly = false, string alias = "")
        {
            string func = isPostgre ? "now" : "getdate";
            string asAlias = !string.IsNullOrEmpty(alias) ? $" as {alias}" : "";
            return $"{(isDateOnly ? "cast(" : "")}{func}(){(isDateOnly ? " as date)" : "")} {asAlias}";
        }
        protected static string GetSqlTypeName(SqlDbType dbType)
        {
            switch (dbType)
            {
                case SqlDbType.BigInt:
                    break;
                case SqlDbType.Binary:
                    break;
                case SqlDbType.Bit:
                    return (isPostgre ? "boolean" : "bit");
                case SqlDbType.Char:
                    break;
                case SqlDbType.DateTime:
                    break;
                case SqlDbType.Decimal:
                    break;
                case SqlDbType.Float:
                    break;
                case SqlDbType.Image:
                    break;
                case SqlDbType.Int:
                    break;
                case SqlDbType.Money:
                    break;
                case SqlDbType.NChar:
                    break;
                case SqlDbType.NText:
                    break;
                case SqlDbType.NVarChar:
                    break;
                case SqlDbType.Real:
                    break;
                case SqlDbType.UniqueIdentifier:
                    break;
                case SqlDbType.SmallDateTime:
                    break;
                case SqlDbType.SmallInt:
                    break;
                case SqlDbType.SmallMoney:
                    break;
                case SqlDbType.Text:
                    break;
                case SqlDbType.Timestamp:
                    break;
                case SqlDbType.TinyInt:
                    break;
                case SqlDbType.VarBinary:
                    break;
                case SqlDbType.VarChar:
                    break;
                case SqlDbType.Variant:
                    break;
                case SqlDbType.Xml:
                    break;
                case SqlDbType.Udt:
                    break;
                case SqlDbType.Structured:
                    break;
                case SqlDbType.Date:
                    break;
                case SqlDbType.Time:
                    break;
                case SqlDbType.DateTime2:
                    break;
                case SqlDbType.DateTimeOffset:
                    break;
                default:
                    break;
            }
            return "";
        }

        private static Dictionary<string, string> specSymbols = new Dictionary<string, string>
        {
            { "&#91;", "[" },
            { "&#93;", "]" },

        };

        private static string SetTargetChars(string query)
        {
            if (isPostgre)
            {
                query = query.Replace("[", "\"").Replace("]", "\"");
            }

            foreach (var symbol in specSymbols)
            {
                query = query.Replace(symbol.Key, symbol.Value);
            }
            return query;
        }
        private static string HandleTail(string tail)
        {
            string EncryptionKey = nameof(MainForm.UserLogin);
            tail = tail.Replace(" ", "+");
            //byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            byte[] cipherBytes = Convert.FromBase64String(tail);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    //using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        //cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    tail = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return tail;
        }



    }
}
