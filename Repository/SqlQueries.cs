namespace TimeReportV3.Repository
{
    internal static class SqlQueries
    {
        internal const string Jira = @"
;with 
	t as (
		SELECT wkl.issueid												as TaskID -- ID задачи
			, wkl.AUTHOR												as Worker -- ID исполнителя
			, sum(wkl.timeworked)/60									as WorkMinutes -- сумма времени, сипсанного исполнителем на задачу в минутах
			, cast(dateadd(second, sum(wkl.timeworked)/60, 0) as time)	as WorkTime -- сумма времени, сипсанного исполнителем на задачу
		FROM worklog wkl
		-- WHERE wkl.startdate BETWEEN @begin AND @end        -- Период
		WHERE cast(wkl.startdate as date) > @begin AND wkl.startdate < @end
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

SELECT usr_w.display_name						AS 'Исполнитель'
	, 'JIRA'									AS 'Система учёта'
	, ji.issuenum								AS 'Номер задачи'
	, ji.SUMMARY								AS 'Наименование задачи'
	, usr_cr.display_name						AS 'Автор'
	, usr_a.display_name						AS 'Инициатор'
	, cast(ji.CREATED as date)					AS 'Создана'
	, cast(ji.DUEDATE as date)					AS 'Срок'
	, cast(ji.RESOLUTIONDATE as date)			AS 'Завершена'
	, cast(ji.RESOLUTIONDATE as date)			AS 'Закрыта'
	, comp.srv									AS 'Сервис' -- Компонент
	, lab.lb									AS 'ЦФО' -- Метки
	, IsNull(epic.SUMMARY, parent_e.SUMMARY)	AS 'Проект' -- ЭПИК
	, IsNull(epic.issuenum, parent_e.issuenum)	AS 'EpicID'
	, stat.pname								AS 'Статус'
	, pri.pname									AS 'Приоритет'
	, t.WorkMinutes								AS 'Минуты'
	, round(t.WorkMinutes/60, 2)				AS 'Часы'
	, t.WorkTime								AS 'Затраченное время'
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
ORDER BY usr_w.display_name desc, ji.issuenum desc
";

        internal const string IntraService = @"
WITH
params AS (
     SELECT
         DATE @begin AS begin_date,
         DATE @end AS end_date
),
times AS (
	select t.""TaskId"" AS taskid, t.""UserId"" AS userid, sum(t.""Minutes"") as minutes
	from ""TaskExpenses"" t
		--inner join params p on t.""Date"" between p.begin_date and p.end_date
		inner join params p on t.""Date"" > p.begin_date and t.""Date"" < p.end_date
	group by t.""TaskId"", t.""UserId""
)

SELECT e.""Name""															AS ""Ispolnitel""
	, 'IntraService'													AS ""Sistema ucheta""
	, t.taskid															AS ""Nomer zadachi""
	, tk.""Name""															AS ""Naimenovanie zadachi""
	, COALESCE(ini.""Name"", a.""Name"")									AS ""Avtor"" -- is#36555 если автора нет, то автор - инициатор
	, a.""Name""															AS ""Initziator""
    , tk.""Created""::Date												AS ""Sozdana""
    , tk.""Deadline""::Date												AS ""Srok""
    , tk.""ResolutionDateFact""::Date										AS ""Zavershena""
    , tk.""Closed""::Date													AS ""Zakrita""
    , (xpath('/Language/Ru/text()', sr.""NameXml""))[1]::text				AS ""Servis""
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
     END 																AS ""Proect""
	, -1::Integer														AS ""EpicID""
	, (xpath('/Language/Ru/text()', st.""NameXml""))[1]::text				AS ""Status""
	, (xpath('/Language/Ru/text()', p.""NameXml""))[1]::text				AS ""Prioritet""
	, t.minutes															AS ""Minuti""
	, round(t.minutes / 60)												AS ""Thasi""
	, make_interval(mins => t.minutes::int)::time without time zone	AS ""Zatrachennoe vremya""
	, ''																AS ""JiraProject""
	, ''																AS ""JiraEpicProject""
FROM times t
	INNER JOIN ""Task"" tk ON tk.""Id"" = t.""taskid""
	INNER JOIN ""User"" e ON e.""Id"" = t.userid
	INNER JOIN ""User"" a ON a.""Id"" = tk.""CreatorId""
	LEFT JOIN ""User"" ini ON ini.""Id"" = tk.""EditorId"" -- is#36555 inner → left
	INNER JOIN ""Service"" sr ON sr.""Id"" = tk.""ServiceId""
	INNER JOIN ""Priority"" p ON p.""Id"" = tk.""PriorityId""
	INNER JOIN ""Status"" st  ON st.""Id"" = tk.""StatusId""
";
    }
}