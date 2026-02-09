using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Data.SqlClient;

namespace TimeReportV3
{
    internal class ReportRepo : BaseRepository
    {

        public IEnumerable<ReportInfo> GetReportInfos(ReportType repType, string user, DateTime beginDate, DateTime endDate)
        {
            if(repType==ReportType.Consolid_Report)
            {
                var result = new List<ReportInfo>();

                var isSql = GetISConsolidQuery(beginDate, endDate);

                var isData = QueryIS<ReportInfo>(isSql);
                foreach (var r in isData)
                {
                    r.SystemName = "IS";
                    result.Add(r);
                }

                var jiraSql = GetJiraConsolidQuery(beginDate, endDate);
                var jiraData = QueryJira<ReportInfo>(jiraSql);

                foreach(var r in jiraData)
                {
                    r.SystemName = "Jira";
                    result.Add(r);
                }
                return result;
            }
            return Query<ReportInfo>(GetQueryByRepType(repType, user, beginDate, endDate));
        }

        private string GetJiraConsolidQuery(DateTime beginDate, DateTime endDate)
        {
            return $@";with 
	t as (
		SELECT wkl.issueid												as TaskID -- ID задачи
			, wkl.AUTHOR												as Worker -- ID исполнителя
			, sum(wkl.timeworked)/60									as WorkMinutes -- сумма времени, сипсанного исполнителем на задачу в минутах
			, cast(dateadd(second, sum(wkl.timeworked)/60, 0) as time)	as WorkTime -- сумма времени, сипсанного исполнителем на задачу
		FROM worklog wkl
		-- WHERE wkl.startdate BETWEEN @begin AND @end        -- Период
		WHERE cast(wkl.startdate as date) > '{beginDate:yyyy-MM-dd}' AND wkl.startdate < '{endDate:yyyy-MM-dd}'
		GROUP BY wkl.issueid, wkl.AUTHOR
	) -- 192 rows

	, usr as (
		SELECT au.user_key, cu.display_name
		FROM app_user au
			INNER JOIN cwd_user cu ON cu.lower_user_name = au.lower_user_name
	)

	, comp as (
		select  na.SOURCE_NODE_ID as TaskID, STRING_AGG(comp.cname, ', ') as srv
		from nodeassociation na
			inner join component comp on comp.ID = na.SINK_NODE_ID
		where na.SINK_NODE_ENTITY = 'Component'
		group by na.SOURCE_NODE_ID
	)
	
	, labels as (
		SELECT wkl.issueid as TaskID, STRING_AGG(l.LABEL, ', ') as lb
		FROM (
				select distinct issueid from worklog
			) as wkl
			LEFT JOIN (select DISTINCT ISSUE, LABEL from label) l ON l.ISSUE = wkl.issueid
		GROUP BY wkl.issueid
	)

SELECT usr_w.display_name						AS 'Executor'
	, 'JIRA'									AS 'SystemName'
	, ji.issuenum								AS 'TaskId'
	, ji.SUMMARY								AS 'TaskName'
	, usr_cr.display_name						AS 'Author'
	, usr_a.display_name						AS 'Creator'
	, cast(ji.CREATED as date)					AS 'Created'
	, cast(ji.DUEDATE as date)					AS 'Deadline'
	, cast(ji.RESOLUTIONDATE as date)			AS 'Completed'
	, cast(ji.RESOLUTIONDATE as date)			AS 'Closed'
	, comp.srv									AS 'Service' -- Компонент
	, lab.lb									AS 'CFO' -- Метки
	, IsNull(epic.SUMMARY, parent_e.SUMMARY)	AS 'Project' -- ЭПИК
	, IsNull(epic.issuenum, parent_e.issuenum)	AS 'EpicID'
	, stat.pname								AS 'Status'
	, pri.pname									AS 'Priority'
	, t.WorkMinutes								AS 'Minutes'
	, round(t.WorkMinutes/60, 2)				AS 'Hours'
	, CONVERT(varchar(8), DATEADD(second,t.WorkMinutes	*60,0),108)									AS 'Time'
	, j_proj.pkey								AS 'JiraProject' -- проект JIRA для правильной ссылки is#36427?LifeTimeId=219969&IsShort=true#link219969
	, IsNull(epic_pj.pkey, parent_e_pj.pkey)	AS 'JiraEpicProject' -- проект JIRA для правильной ссылки на Эпик is#36427?LifeTimeId=219969&IsShort=true#link219969
FROM t
	INNER JOIN jiraissue			ji			ON ji.ID = t.TaskID
	INNER JOIN usr					usr_cr		ON usr_cr.user_key = ji.CREATOR
	INNER JOIN usr					usr_a		ON usr_a.user_key = ji.REPORTER
	INNER JOIN usr					usr_w		ON usr_w.user_key = t.Worker--ji.ASSIGNEE
	INNER JOIN issuestatus			stat		ON stat.ID = ji.issuestatus
	INNER JOIN [priority]			pri			ON pri.ID = ji.PRIORITY
	-- ЭПИК - это у нас проект
	LEFT JOIN jiraissue				epic_i		ON epic_i.ID	= t.TaskID
	LEFT JOIN issuelink				epic_l		ON epic_l.DESTINATION = epic_i.ID AND epic_l.LINKTYPE = 10201 -- issuelinktype.Epic-Story Link
	LEFT JOIN jiraissue				epic		ON epic.ID = epic_l.SOURCE
		-- проект JIRA для правильной ссылки is#36427?LifeTimeId=219969&IsShort=true#link219969
		LEFT JOIN project			epic_pj		ON epic_pj.ID = epic.PROJECT
		-- end проект JIRA
	-- end ЭПИК
	-- ЭПИК из parent
	LEFT JOIN issuelink				parent_l	ON parent_l.DESTINATION = ji.ID AND parent_l.LINKTYPE = 10100 -- issuelinktype.Epic-Story Link
	LEFT JOIN issuelink				parent_el	ON parent_el.DESTINATION = parent_l.SOURCE AND parent_el.LINKTYPE = 10201 -- issuelinktype.Epic-Story Link
	LEFT JOIN jiraissue				parent_e	on parent_e.ID = parent_el.SOURCE
		-- проект JIRA для правильной ссылки is#36427?LifeTimeId=219969&IsShort=true#link219969
		LEFT JOIN project			parent_e_pj	ON	parent_e_pj.ID = parent_e.PROJECT
		-- end проект JIRA
	-- end ЭПИК из parent
	-- Компонент - это у нас Сервис
	LEFT JOIN comp					comp		ON comp.TaskID = t.TaskID
	-- end Компонент
	-- Метки - это у нас ЦФО
	LEFT JOIN labels				lab			ON lab.TaskID = t.TaskID
	-- end Метки
	-- проект JIRA для правильной ссылки is#36427?LifeTimeId=219969&IsShort=true#link219969
	INNER JOIN project				j_proj		ON j_proj.ID = ji.PROJECT
	-- end проект JIRA
ORDER BY usr_w.display_name desc, ji.issuenum desc";
        }

        protected IEnumerable<T> QueryJira<T>(string sql)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["JiraDB"]?.ConnectionString))
            {
                conn.Open();
                return conn.Query<T>(sql);
            }
        }

        private string GetISConsolidQuery(DateTime beginDate, DateTime endDate)
        {
            return $@"WITH
params AS (
     SELECT
         DATE '{beginDate:yyyy-MM-dd}' AS begin_date,
         DATE '{endDate:yyyy-MM-dd}' AS end_date
),
times AS (
	select t.""TaskId"" AS taskid, t.""UserId"" AS userid, sum(t.""Minutes"") as minutes
	from ""TaskExpenses"" t
		--inner join params p on t.""Date"" between p.begin_date and p.end_date
		inner join params p on t.""Date"" > p.begin_date and t.""Date"" < p.end_date
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
	, COALESCE(t.minutes,0)::numeric(20,6)														AS ""Minutes""
	, round(t.minutes / 60)												AS ""Hours""
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

		private IEnumerable<T> QueryIS<T>(string sql)
		{
			using (var conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["ISdb"]?.ConnectionString))
			{
				conn.Open();
				return conn.Query<T>(sql);
			}
		}
        private string GetQueryByRepType(ReportType typeReport, string user, DateTime beginDate, DateTime endDate)
        {
            bool needEvaluation = typeReport == ReportType.TaskList || typeReport == ReportType.Time_By_User || typeReport == ReportType.Time_By_Users;
            bool needMinutes = typeReport == ReportType.Time_By_User || typeReport == ReportType.Times_On_Tasks || typeReport == ReportType.Time_By_Users;
            bool needExecutors = typeReport == ReportType.OpenTasks || typeReport == ReportType.TaskList;
            string query = $@"
	SELECT distinct
	    t.[Id] [TaskId]
      , t.[Name] [TaskName]
      , {GetXmlSqlFunc("s.[NameXml]", "/Language/Ru", "[Service]", "varchar(20)")}
      , array_to_string(ARRAY(SELECT {GetXmlSqlFunc("ttcb_cfo.[NameXml]", "/Language/Ru", "[CFO]", "varchar(50)")} 
            FROM [TaskTypeComboBox] ttcb_cfo where ttcb_cfo.[Id] in (select cfo.[ComboboxId] FROM [TaskFieldValues] cfo where cfo.[EntityId] = t.[Id] and cfo.[FieldId] = 1015)), ', ') as [CFO]
      , array_to_string(ARRAY(SELECT {GetXmlSqlFunc("ttcb_prj.[NameXml]", "/Language/Ru", "[Project]", "varchar(50)")} 
            FROM [TaskTypeComboBox] ttcb_prj where ttcb_prj.[Id] in (select prj.[ComboboxId] FROM [TaskFieldValues] prj where prj.[EntityId] = t.[Id] and prj.[FieldId] = 1014)), ', ') as [Project]
      , case when ast.[Name] is null then '' else ast.[Name] end as [Customer]
	  , {GetXmlSqlFunc("st.[NameXml]", "/Language/Ru", "[Status]", "varchar(20)")}
	  , {GetXmlSqlFunc("pr.[NameXml]", "/Language/Ru", "[Priority]", "varchar(20)")}
	  , au.[Name] as [Author]
          {(needExecutors
      ? @", case when t.[Executors] is null then
  				    (case when eg.[Name] is null then 'Не назначен' else ast.[Name] end)
			    else t.[Executors]
            end	as [Executors]"
          : string.Empty)}
          {(typeReport == ReportType.Times_On_Tasks
      ? ", u.[Name] as [Executor] "
          : string.Empty)}
	  , t.[Created]
	  , t.[Deadline]
          {(typeReport == ReportType.OpenTasks
      ? @", case when th.[Editir] is null then '' else th.[Editir] end as [Commentator]
	      , case when th.[Changed] is null then t.[Created] else th.[Changed] end as [LastChanged]
          , LEFT(LTRIM(case when th.[Comment] is null then '' else th.[Comment] end), 100) as [LastComment]"
      : @", t.[ResolutionDateFact] as [Completed]
          , t.[Closed] ")}
          {(typeReport == ReportType.TaskList
      ? ", case when t.[Hours] is null then 0 else t.[Hours] end as [AllTime]"
        : string.Empty)}
          {(needMinutes
      ? ", te.[Minutes]"
        : string.Empty)}
        {(needEvaluation
      ? $", {GetXmlSqlFunc("ev.[NameXml]", "/Language/Ru", "[Evaluation]", "varchar(50)")}"
        : string.Empty)}
        {(typeReport == ReportType.Time_By_Users ? @", au2.[Name] as [Executor]" : string.Empty)}
   FROM 
    {(needMinutes
        ? $@"
          (select [TaskId], [UserId], sum([Minutes]) as [Minutes]
		  from [TaskExpenses]
		  where cast([Date] as date) between '{beginDate:yyyy-MM-dd}' and '{endDate:yyyy-MM-dd}'
                {(typeReport == ReportType.Time_By_User ? $" and [UserId] = {user}" : string.Empty)}
		  group by [TaskId], [UserId] ) as te
          inner join [Task] t on t.[Id] = te.[TaskId] "
        : "[Task] t ")}
    {(typeReport == ReportType.Time_By_Users ? "inner join [User] au2 on au2.[Id] = te.[UserId]" : string.Empty)}
   inner join [Service] s on s.[Id] = t.[ServiceId]
   inner join [Status] st on st.[Id] = t.[StatusId]
   inner join [Priority] pr on pr.[Id] = t.[PriorityId]
   inner join [User] au on au.[Id] = t.[CreatorId]
       {(needMinutes ?
   $" inner join [User] u on u.[Id] = {(typeReport == ReportType.TaskList ? user : "te.[UserId] ")}"
   : " left join [ExecutorGroup] eg on eg.[Id] = t.[ExecutorGroupId] ")}
    {(typeReport == ReportType.Time_By_Users ? "left join [ExecutorGroup] eg on eg.[Id] = t.[ExecutorGroupId]" : string.Empty)}
   left join [TaskAsset] ta on ta.[TaskId] = t.[Id]
   left join [Asset] ast on ast.[Id] = ta.[AssetId]
        {(needEvaluation ?
    " left join [Evaluation] ev on ev.[Id] = t.[EvaluationId]"
        : string.Empty)}
        {(typeReport == ReportType.OpenTasks ?
    $@" left join 
	(select h.[TaskId], u.[Name] as [Editir], h.[Changed], h.[Comment] from [TaskHistory] h inner join [User] u on u.[Id] = h.[EditorId]
		where h.[Changed] = (select max([Changed]) from [TaskHistory] where h.[TaskId] = [TaskId] and [Comment] is not null)) th on th.[TaskId] = t.[Id]
    where t.[StatusId] = 31"
        : string.Empty)}
        {(typeReport == ReportType.TaskList ?
    $"where cast(t.[Created] as date) between '{beginDate:yyyy-MM-dd}' and '{endDate:yyyy-MM-dd}'"
        : string.Empty)}
        {(typeReport == ReportType.OpenTasks ?
    " order by [LastChanged], [Deadline] desc" : typeReport == ReportType.Time_By_Users ? " order by au2.[Name], t.[Id]"
    : " order by t.[Id]")}
";
            return query;
        }

    }
}
