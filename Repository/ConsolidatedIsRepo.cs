using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;


namespace TimeReportV3.Repository
{
    internal class ConsolidatedIsRepo: BaseRepository
    {

		public DataTable GetData(DateTime begin,DateTime end) 
        {
			var sql = GetQuery(begin, end);
				//.Replace("{begin:yyyy-MM-dd}",begin.ToString("yyyy-MM-dd")).Replace("{end:yyyy-MM-dd}",end.ToString("yyyy-MM-dd"));

			var conn = GetConnection() ;
			var cmd = new NpgsqlCommand(sql, conn) ;

			var dt = new DataTable() ;
			try
			{
                conn.Open();
                dt.Load(cmd.ExecuteReader());
                
            }
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally { conn.Close(); }
            return dt;

        }

		private string GetQuery(DateTime begin, DateTime end)
		{
			return $@"
WITH
params AS (
     SELECT
         DATE '{begin:yyyy-MM-dd}' AS begin_date,
         DATE '{end:yyyy-MM-dd}' AS end_date
),
times AS (
	select t.""TaskId"" AS taskid, t.""UserId"" AS userid, sum(t.""Minutes"") as minutes
	from ""TaskExpenses"" t
		--inner join params p on t.""Date"" between p.begin_date and p.end_date
		inner join params p on t.""Date"" >= p.begin_date and t.""Date"" <= p.end_date
	group by t.""TaskId"", t.""UserId""
)

SELECT e.""Name""															AS ""Executor""
	, 'IntraService'													AS ""SystemName""
	, t.taskid															AS ""TaskId""
	, tk.""Name""															AS ""TaskName""
	, COALESCE(ini.""Name"", a.""Name"")									AS ""Author"" -- is#36555 если автора нет, то автор - инициатор
	, a.""Name""															AS ""Creator""
    , tk.""Created""::Date												AS ""Created""
    , tk.""Deadline""::Date												AS ""Deadline""
    , tk.""ResolutionDateFact""::Date										AS ""Completed""
    , tk.""Closed""::Date													AS ""Closed""
    , (xpath('/Language/Ru/text()', sr.""NameXml""))[1]::text				AS ""Service""
    -- is#36782?LifeTimeId=222552&IsShort=true#link222552 -- для 1С Документооборот ЦФО всегда 08, а для остальных берём из задачи, а не из автора
    -- , a.""SearchStringComboBox""											AS ""CFO""
	, CASE
		WHEN tk.""ServiceId"" = 41
			THEN (xpath('/Language/Ru/text()', (SELECT ""NameXml"" 
												FROM ""TaskTypeComboBox"" 
													WHERE ""Id"" = 109 AND ""TaskTypeFieldId""=1017 )))[1]::text
		ELSE (xpath('/Language/Ru/text()', (SELECT ""NameXml"" 
		 										FROM ""TaskTypeComboBox"" 
												 	WHERE ""Id"" = ((xpath('/data/field/text()', tk.""Data""))[1]::text)::int 
													 		AND ""TaskTypeFieldId""=1015 )))[1]::text
	END																	AS ""CFO""
	-- end of is#36782?LifeTimeId=222552&IsShort=true#link222552
	, CASE
         WHEN tk.""ServiceId"" = 42 THEN '1C'
         ELSE (xpath('/Language/Ru/text()', (SELECT ""NameXml"" 
		 										FROM ""TaskTypeComboBox"" 
												 	WHERE ""Id"" = ((xpath('/data/field/text()', tk.""Data""))[2]::text)::int 
													 		AND ""TaskTypeFieldId""=1014 )))[1]::text
     END 																AS ""Project""
	, -1::Integer														AS ""EpicID""
	, (xpath('/Language/Ru/text()', st.""NameXml""))[1]::text				AS ""Status""
	, (xpath('/Language/Ru/text()', p.""NameXml""))[1]::text				AS ""Priority""
	, COALESCE(t.minutes,0)::numeric(20,2)														AS ""Minutes""
	, COALESCE(t.minutes,0)::numeric(20,2) / 60													AS ""Hours""
	--, make_interval(mins => t.minutes::int)::time without time zone	
	,LPAD(FLOOR(COALESCE(t.minutes,0)/1440)::text,2,'0') || ':' ||
        LPAD(FLOOR((COALESCE(t.minutes,0)%1440)/60)::text,2,'0') || ':' ||
        LPAD((COALESCE(t.minutes,0)%60)::text,2,'0') AS ""Time""
	, ''																AS ""JiraProject""
	, ''																AS ""JiraEpicProject""
FROM times t
	INNER JOIN ""Task"" tk ON tk.""Id"" = t.""taskid""
	INNER JOIN ""User"" e ON e.""Id"" = t.userid
	INNER JOIN ""User"" a ON a.""Id"" = tk.""CreatorId""
	LEFT JOIN ""User"" ini ON ini.""Id"" = tk.""EditorId"" -- is#36555 inner → left
	INNER JOIN ""Service"" sr ON sr.""Id"" = tk.""ServiceId""
	INNER JOIN ""Priority"" p ON p.""Id"" = tk.""PriorityId""
	INNER JOIN ""Status"" st  ON st.""Id"" = tk.""StatusId""";
		}
    }
}
