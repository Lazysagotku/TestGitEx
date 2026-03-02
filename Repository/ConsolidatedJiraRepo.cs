using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Npgsql;
using System.Runtime.Remoting.Messaging;

namespace TimeReportV3.Repository
{
	internal class ConsolidatedJiraRepo : BaseRepository
	{
		public DataTable GetData(DateTime begin, DateTime end)
		{
			var sql = GetQuery(begin,end);
			var conn = GetJiraConnection();
			var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@begin", begin);
            cmd.Parameters.AddWithValue("@end", end);

			var dt = new DataTable();
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
			return $@";with 
	t as (
		SELECT wkl.issueid												as TaskID -- ID задачи
			, wkl.AUTHOR												as Worker -- ID исполнителя
			, sum(wkl.timeworked)/60									as WorkMinutes -- сумма времени, сипсанного исполнителем на задачу в минутах
			, cast(dateadd(second, sum(wkl.timeworked)/60, 0) as time)	as WorkTime -- сумма времени, сипсанного исполнителем на задачу
		FROM worklog wkl
		-- WHERE wkl.startdate BETWEEN @begin AND @end        -- Период
		WHERE cast(wkl.startdate as date) >= '{begin:yyyy-MM-dd}' AND wkl.startdate <= '{end:yyyy-MM-dd}'
		GROUP BY wkl.issueid, wkl.AUTHOR
	) -- 192 rows

	, usr as (
		SELECT au.user_key, cu.display_name
		FROM app_user au
			INNER JOIN cwd_user cu ON cu.lower_user_name = au.lower_user_name and cu.directory_id = 10000 -- is#39052 --
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
	, cast(t.WorkMinutes as DECIMAL(10,2))/60	AS 'Hours'
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
	}
}

