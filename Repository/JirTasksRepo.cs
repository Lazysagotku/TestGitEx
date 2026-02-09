// Repository/JiraTasksRepo.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeReportV3.Repository
{
    internal  class JiraTasksRepo
    {
        internal static List<int> GetTasksCounts(string displayName)
        {
            var results = new List<int>();

            const string q1 = @"
SELECT COUNT(*)
FROM [JiraSoftwareDB].[dbo].[jiraissue] ji
JOIN app_user au ON au.user_key = ji.ASSIGNEE
JOIN cwd_user cw ON cw.lower_user_name = au.lower_user_name
WHERE ji.issuestatus  NOT IN(10002,10008,10013,10016,5,6)
  AND cw.display_name = @DisplayName;
";
            results.Add(JiraRepository.ExecuteScalarInt(q1, new { DisplayName = displayName }));

            const string q2 = @"
SELECT COUNT(*)
FROM [JiraSoftwareDB].[dbo].[jiraissue] ji
JOIN app_user au ON au.user_key = ji.ASSIGNEE
JOIN cwd_user cw ON cw.lower_user_name = au.lower_user_name
WHERE ji.issuestatus  in (1,10000,10007,10009,10010) 
  AND cw.display_name = @DisplayName;
";
            results.Add(JiraRepository.ExecuteScalarInt(q2, new { DisplayName = displayName }));

            const string q3 = @"
SELECT COUNT(*)
FROM [JiraSoftwareDB].[dbo].[jiraissue] ji
WHERE ji.issuestatus NOT IN('10002','10008','10013','10016','5','6') 
  AND ji.ASSIGNEE IS NULL;
";
            results.Add(JiraRepository.ExecuteScalarInt(q3, null));

            // 4 and 5 - comments not used -> return 0
            results.Add(0);
            results.Add(0);

            const string q6 = @"
SELECT FLOOR(SUM(wl.timeworked) / 60.0) AS Minutes
FROM [JiraSoftwareDB].[dbo].[worklog] wl
JOIN app_user au ON au.user_key = wl.AUTHOR
JOIN cwd_user cw ON au.lower_user_name = cw.lower_user_name
WHERE cw.display_name = @DisplayName AND CONVERT(date, wl.STARTDATE) = CONVERT(date, GETDATE())
";
            var mn = JiraRepository.QuerySingleOrDefault<int?>(q6, new { DisplayName = displayName }) ?? 0;
            results.Add(mn);

            return results;
        }

        internal static FieldsDetailInfo[] GetDetailDatas(int paramNumber, string displayName)
        {
            switch (paramNumber)
            {
                case 1:
                    {
                        var q = @"
SELECT  
    ji.issuenum AS TaskId,
    'Jira' AS System,
    ji.SUMMARY AS Name,
    cw1.display_name AS CreatorName,
    cw2.display_name AS Executors,
    ji.CREATED AS Created,
    ji.UPDATED AS Changed,
    ist.pname AS StatusValue,
CONVERT(varchar(60),j_proj.pkey)+  '-' + CONVERT(varchar(100),ji.issuenum) AS KeyTaskId
FROM [JiraSoftwareDB].[dbo].[jiraissue] ji
JOIN app_user au1 ON au1.user_key = ji.REPORTER
JOIN cwd_user cw1 ON cw1.lower_user_name = au1.lower_user_name
JOIN app_user au2 ON au2.user_key = ji.ASSIGNEE
JOIN cwd_user cw2 ON cw2.lower_user_name = au2.lower_user_name
JOIN issuestatus ist ON ji.issuestatus = ist.ID
INNER JOIN project j_proj ON j_proj.ID = ji.PROJECT
WHERE cw2.display_name = @DisplayName
  AND ji.issuestatus NOT IN(10002,10008,10013,10016,5,6)
";
                        return JiraRepository.Query<FieldsDetailInfo>(q, new { DisplayName = displayName }).ToArray();
                    }
                case 2:
                    {
                        var q = @"
SELECT  
    ji.issuenum AS TaskId,
    'Jira' AS System,
    ji.SUMMARY AS Name,
    cw1.display_name AS CreatorName,
    cw2.display_name AS Executors,
    ji.CREATED AS Created,
    ji.UPDATED AS Changed,
    ist.pname AS StatusValue,
CONVERT(varchar(60),j_proj.pkey)+  '-' + CONVERT(varchar(100),ji.issuenum) AS KeyTaskId
FROM [JiraSoftwareDB].[dbo].[jiraissue] ji
JOIN app_user au1 ON au1.user_key = ji.REPORTER
JOIN cwd_user cw1 ON cw1.lower_user_name = au1.lower_user_name
JOIN app_user au2 ON au2.user_key = ji.ASSIGNEE
JOIN cwd_user cw2 ON cw2.lower_user_name = au2.lower_user_name
JOIN issuestatus ist ON ji.issuestatus = ist.ID
INNER JOIN project j_proj ON j_proj.ID = ji.PROJECT
WHERE cw2.display_name = @DisplayName
  AND ji.issuestatus IN(1,10000,10007,10009,10010)
";
                        return JiraRepository.Query<FieldsDetailInfo>(q, new { DisplayName = displayName }).ToArray();
                    }
                case 3:
                    {
                        var q = @"
SELECT  
    ji.issuenum AS TaskId,
    'Jira' AS System,
    ji.SUMMARY AS Name,
    cw1.display_name AS CreatorName,
    '' AS Executors,
    ji.CREATED AS Created,
    ji.UPDATED AS Changed,
    ist.pname AS StatusValue,
CONVERT(varchar(60),j_proj.pkey)+  '-' + CONVERT(varchar(100),ji.issuenum) AS KeyTaskId
FROM [JiraSoftwareDB].[dbo].[jiraissue] ji
JOIN app_user au1 ON au1.user_key = ji.REPORTER
JOIN cwd_user cw1 ON cw1.lower_user_name = au1.lower_user_name
JOIN issuestatus ist ON ji.issuestatus = ist.ID
INNER JOIN project j_proj ON j_proj.ID = ji.PROJECT
WHERE ji.issuestatus NOT IN(10002,10008,10013,10016,5,6) AND ji.ASSIGNEE IS NULL
";
                        return JiraRepository.Query<FieldsDetailInfo>(q, null).ToArray();
                    }
                case 4:
                case 5:
                    return new FieldsDetailInfo[] { };
                case 6:
                    {
                        var q = @"
SELECT  
    ji.issuenum AS TaskId,
    'Jira' AS System,
    ji.SUMMARY AS Name,
    cw.display_name AS CreatorName,
    cw2.display_name AS Executors,
    ji.CREATED AS Created,
    ji.UPDATED AS Changed,
    ist.pname AS StatusValue,
    FLOOR(SUM(wl.timeworked) / 60.0) AS Minutes,
CONVERT(varchar(60),j_proj.pkey)+ '-' + CONVERT(varchar(100),ji.issuenum) AS KeyTaskId
FROM [JiraSoftwareDB].[dbo].[worklog] wl
JOIN jiraissue ji ON wl.issueid = ji.ID
JOIN app_user au ON au.user_key = wl.AUTHOR
JOIN cwd_user cw ON cw.lower_user_name = au.lower_user_name
JOIN issuestatus ist ON ji.issuestatus = ist.ID
JOIN app_user au2 ON au2.user_key = ji.ASSIGNEE
JOIN cwd_user cw2 ON cw2.lower_user_name = au2.lower_user_name
INNER JOIN project j_proj ON j_proj.ID = ji.PROJECT
WHERE ji.issuestatus NOT IN(10002,10008,10013,10016,5,6) AND cw.display_name = @DisplayName AND CONVERT(date, wl.STARTDATE) = CONVERT(date, GETDATE())
GROUP BY j_proj.pkey,ji.issuenum, ji.SUMMARY,cw.display_name,cw2.display_name,ji.CREATED,ji.UPDATED,ist.pname
ORDER BY ji.issuenum
";
                        return JiraRepository.Query<FieldsDetailInfo>(q, new { DisplayName = displayName }).ToArray();
                    }
                default:
                    return new FieldsDetailInfo[] { };
            }
        }

        internal static FieldsIdTasksUserInfo[] GetIdTasksUserOnCurDay(string curDate)
        {
            var q = @"
SELECT ji.issuenum AS IdTask,
    'Jira' AS System,
FLOOR(SUM(wl.timeworked) / 60.0) AS Minutes, ji.SUMMARY AS Name,
CONVERT(varchar(60),j_proj.pkey)+ '-' + CONVERT(varchar(100),ji.issuenum) AS TaskJira
FROM [JiraSoftwareDB].[dbo].[worklog] wl
JOIN jiraissue ji ON wl.issueid = ji.ID
JOIN app_user au ON au.user_key = wl.AUTHOR
JOIN cwd_user cw ON cw.lower_user_name = au.lower_user_name
INNER JOIN project j_proj ON j_proj.ID = ji.PROJECT
WHERE cw.display_name = @DisplayName AND CAST(wl.STARTDATE AS date) = CAST(@CurDate AS date)
GROUP BY ji.issuenum, ji.SUMMARY,j_proj.pkey
ORDER BY ji.issuenum
";
            return JiraRepository.Query<FieldsIdTasksUserInfo>(q, new { DisplayName = MainForm.UserName, CurDate = curDate }).ToArray();
        }

        internal static FieldsTimeUserInfo[] GetTimeUserOnLastWorkingDays()
        {
            var q =
                $@"SELECT 
       CAST(wl.[STARTDATE] AS date)                                         AS Date
      ,FLOOR(SUM(wl.[timeworked]) / 60)                                     AS Minutes
  FROM [JiraSoftwareDB].[dbo].[worklog] wl
	JOIN app_user au
		ON au.user_key = wl.AUTHOR
	JOIN cwd_user cw
		ON cw.lower_user_name = au.lower_user_name
WHERE cw.display_name = @DisplayName
GROUP BY CAST(wl.[STARTDATE] AS date) 
ORDER BY 1 DESC";
            return JiraRepository.Query<FieldsTimeUserInfo>(q, new { DisplayName = MainForm.UserName }).ToArray();
        }

        internal static (string Service, string Role)[] GetServicesAndRoles(string displayName)
        {
            var q = @"
SELECT p.pname AS Service, r.pname AS Role
FROM project p
JOIN projectroleactor pra ON pra.pid = p.id
JOIN projectrole r ON r.ID = pra.projectroleid
JOIN cwd_user cu ON cu.lower_user_name = pra.actor
INNER JOIN project j_proj ON j_proj.ID = ji.PROJECT
WHERE cu.display_name = @DisplayName
";
            var rows = JiraRepository.Query<dynamic>(q, new { DisplayName = displayName }).ToArray();
            return rows.Select(r => ((string)r.Service, (string)r.Role)).ToArray();
        }


        
    }
}
