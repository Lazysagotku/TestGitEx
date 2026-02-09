using System.Collections.Generic;
using System.Linq;
using System;

namespace TimeReportV3
{
    internal class UserTasksRepo : BaseRepository
    {
        public static IEnumerable<User> GetUsers()
        {
            return Query<User>($@"
                select u.[Id]
                    , 'IS' [System],'' [KeyTaskId], u.[Name], u.[Login], u.[Email], u.[ADGuid], u.[IsArchive], {GetXmlSqlFunc("r.[NameXml]", "/Language/Ru", "[Role]", "varchar(50)")}
                from [User] u
                inner join [Role] r on r.[Id] = u.[RoleId] and r.[TypeId] = 1 ");
        }
        public static IEnumerable<User> GetActiveUsers(DateTime beginDate, DateTime endDate)
        {
            return Query<User>($@"
                select distinct  te.[UserId] as [Id]
                    , 'IS' [System],'' [KeyTaskId],u.[Name], u.[Login], u.[Email], u.[ADGuid], u.[IsArchive], {GetXmlSqlFunc("r.[NameXml]", "/Language/Ru", "[Role]", "varchar(50)")}
                from [TaskExpenses] te
                inner join [User] u on u.[Id] = te.[UserId]
                inner join [Role] r on r.[Id] = u.[RoleId] and r.[TypeId] = 1
		        where cast([Date] as date) between '{beginDate:yyyy-MM-dd}' and '{endDate:yyyy-MM-dd}'
                order by u.[Name]");
        }

        public IEnumerable<MassTimestampInfo> GetMassTimestampInfos()
        {
            return Query<MassTimestampInfo>($@"
                select 
                    t.[Id] [TaskId]
                    , 'IS' [System]
                    ,'' [KeyTaskId]
                    , t.[Name] [TaskName]
                    , {GetXmlSqlFunc("s.[NameXml]", "/Language/Ru", "[Service]", "varchar(20)")}
                    , {GetXmlSqlFunc("st.[NameXml]", "/Language/Ru", "[Status]", "varchar(20)")}
                    , au.[Name] [Author]
                    , t.[Executors] [Executors]
                    , {GetLengthSqlName()}(t.[Executors]) 
                        - {GetLengthSqlName()}(replace(t.[Executors], ',', '')) + 1 as [ExecutorsCount]
                    , t.[Created]
                 from [Task] t
                 inner join [Service] s on s.[Id] = t.[ServiceId]
                 inner join [Status] st on st.[Id] = t.[StatusId]
                 inner join [User] au on au.[Id] = t.[CreatorId]
                 where t.[StatusId] in (31, 27) 
                    and {GetCharindexSqlFunc("t.[Executors]", "','")} > 0
                 order by case
                    when s.[Id] = 36 then 9999999999 + t.[Id] else t.[Id] end desc").ToArray();
            // сначала ОргМероприятия (31, 27) -- Задача открыта или в работе
        }


        public int FillTaskExpenses(string taskId, int totalMinutes)
        {
            string sqlInsert = $@"
            insert into [TaskExpenses] ([TaskId], [UserId], [EditorId], [Date], [Rate], [Comments], [Minutes], [IsFromApi], [Created], [Changed], [CreatorId])
                select {taskId},'IS' [System] , '' [KeyTaskId], ex.[UserId], t.[CreatorId], {GetDateTimeNowSqlFunc(true)}, 0, null, {totalMinutes}, null
                                                                            , {GetDateTimeNowSqlFunc()}, {GetDateTimeNowSqlFunc()}, t.[CreatorId]
                from [TaskExecutor] ex
                inner join [Task] t on t.[Id] = ex.[TaskId]
                where ex.[TaskId] = {taskId}";
            return Query(sqlInsert);
        }

        public bool IsTaskIdExistsInDB(string taskId)
        {
            return Query($"select count(1) from [Task] where [Id] = {taskId}") > 0;
        }

        public IEnumerable<FieldsIdTasksUserInfo> GetIdTasksUserOnCurDay(string curDate)
        {
            return Query<FieldsIdTasksUserInfo>(
                $@"
                    select t.[Id] [IdTask]
                    , 'IS' [System] , te.[Minutes], t.[Name]
                    from [TaskExpenses] te
                    inner join [Task] t on t.[Id] = te.[TaskId]
                    inner join [User] u on u.[Login] = '{MainForm.UserLogin}' and u.[Id] = te.[UserId]
                    where te.[Date] = '{curDate}'
                    order by [IdTask]"
                );
        }

        public IEnumerable<FieldsTimeUserInfo> GetTimeUserOnLastWorkingdays(string line, string firstDay)
        {
            string QUERY_TIME_USER_ON_LAST_30_DAYS =
                $@"select
                    case when table1.wd is null then table2.date2 else table1.wd end as [Date]
                    , table2.[Minutes] as [Minutes]
                    , case when table1.wd is null then 1 else 0 end as [IsRed]
                from (Values {line}) as table1(wd)
                full join
                   (select cast(te.[Date] as date) as [date2], sum(case when u.[Id] is null then 0 else te.[Minutes] end) as [Minutes]
                   from [TaskExpenses] te
                   left join [User] u on u.[Login] = '{MainForm.UserLogin}' and u.[Id] = te.[UserId]
                   where cast(te.[Date] as date) >= '{firstDay}'
                   group by cast(te.[Date] as date)) as [table2] on table1.wd = table2.date2
                where table1.wd is not null or table2.[Minutes] > 0
                order by 1 desc";
            return Query<FieldsTimeUserInfo>(QUERY_TIME_USER_ON_LAST_30_DAYS);
        }

        private const string AND_Executors_LIKE_USER_NAME = "and t.[Executors] like concat('%', u.[Name], '%')";
        private static string GetDiffColumnsByParamNumber(int paramNumber)
        {
            if (paramNumber == 4 || paramNumber == 5) // needVisitedTask
            {
                return string.Empty;
            }
            if (paramNumber == (int)TaskQueries.WithoutExecutor)
            {
                return $@", {GetXmlSqlFunc("pr.[NameXml]", "/Language/Ru", "[Priority]", "varchar(50)")}, pr.[SortOrder]";
            }
            if (paramNumber == 6) // spentTime 
            {
                return ", te.[Minutes]";
            }
            return ", case when vt.[TaskView] is null then 1 else 0 end as [IsSelected]";
        }
        private static string GetTaskJoinsOnUser_ByParamNumber(int paramNumber)
        {
            switch (paramNumber)
            {
                case 1: // нужны все задачи + инфо про VisitedTask
                case 2:
                    return $@"
                [Task] t 
                    inner join [User] u on u.[Login] = '{MainForm.UserLogin}' {AND_Executors_LIKE_USER_NAME} 
                    left outer join [VisitedTask] vt on t.[Id] = vt.[TaskId] and u.[Id] = vt.[UserId]";
                case 3: // для 3 (noExecutor) нужны все задачи + инфо про Приоритет
                    return @"
                [Task] t 
                    inner join [Priority] pr on pr.[Id] = t.[PriorityId]";
                case 4:// нужны все VisitedTask
                case 5:
                    return $@"
                [VisitedTask] vt 
                    inner join [User] u on u.[Login] = '{MainForm.UserLogin}' and u.[Id] = vt.[UserId] 
                    inner join [Task] t on t.[Id] = vt.[TaskId] {(paramNumber == 4 ? AND_Executors_LIKE_USER_NAME : string.Empty)}";
                case 6:
                    return $@"
                [TaskExpenses] te 
                    inner join [User] u on u.[Login] = '{MainForm.UserLogin}' and u.[Id] = te.[UserId] 
                    left join [Task] t on t.[Id] = te.[TaskId] ";
                default:
                    return string.Empty;
            }
        }
        private static string GetWhereByParamNumber(int paramNumber)
        {
            string statusIds = string.Empty;
            
            bool noExecutor = false;
            switch (paramNumber)
            {
                case 4://VisitedTask
                case 5:
                    return "cast(vt.[NewComments] as int) > 0";
                case 6://@"from TaskExpenses
                    return $"{GetDateFormatSqlFunc("te.[Date]", "yyyy-MM-dd")} = {GetDateFormatSqlFunc(GetDateTimeNowSqlFunc(), "yyyy-MM-dd")}";
                case 1:
                    statusIds = string.Join(",", 26, 27, 35); // "Отложена", "В работе", "Требует уточнения"
                    break;
                case 2:
                    statusIds = "31"; // NotInProgress
                    break;
                case 3:
                    statusIds = string.Join(",", 26, 27, 31, 35);
                    noExecutor = true;
                    break;
                default:
                    break;
            }

            return $@"t.[StatusId] in ({statusIds}) {(noExecutor ? "and case when t.[Executors] is null then '' else t.[Executors] end = '' and t.[ExecutorGroupId] is null" : string.Empty)}";
        }
        public FullFieldsTaskInfo GetDetailedInfo(ParamResult paramResult)
        {
            string statusIds = paramResult.Statuses != null ? string.Join(",", paramResult.Statuses) : "";
            var fieldsDetailInfos = GetDetailDatas(statusIds, paramResult.Index);

            return paramResult.GetFullFieldsTaskInfo(fieldsDetailInfos);
        }

        public IEnumerable<FieldsDetailInfo> GetDetailDatas(IEnumerable<TaskStatuses> statuses, int paramNumber)
        {
            string query = $@"
            select 
                    t.[Id] [TaskId]
                    , 'IS' [System]
                    , '' [KeyTaskId]
                    , t.[Name]
                    , u2.[Name] [CreatorName]
                    , t.[Executors]
                    , t.[Created]
                    , t.[Changed]
                    , {GetXmlSqlFunc("s.[NameXml]", "/Language/Ru", "[StatusValue]", "varchar(50)")}
                    {GetDiffColumnsByParamNumber(paramNumber)} 
            from 
                {GetTaskJoinsOnUser_ByParamNumber(paramNumber)}
                inner join [User] u2 on u2.[Id] = t.[CreatorId]
                inner join [Status] s on s.[Id] = t.[StatusId]
            where {GetWhereByParamNumber(paramNumber)}";
            return Query<FieldsDetailInfo>(query);
        }
        public FieldsDetailInfo[] GetDetailDatas(string statusIds, int paramNumber)
        {
            string query = $@"
            select 
                    t.[Id] [TaskId]
                    , 'IS' [System]
                    , '' [KeyTaskId]
                    , t.[Name]
                    , u2.[Name] [CreatorName]
                    , t.[Executors]
                    , t.[Created]
                    , t.[Changed]
                    , {GetXmlSqlFunc("s.[NameXml]", "/Language/Ru", "[StatusValue]", "varchar(50)")}
                    {GetDiffColumnsByParamNumber(paramNumber)} 
            from 
                {GetTaskJoinsOnUser_ByParamNumber(paramNumber)}
                inner join [User] u2 on u2.[Id] = t.[CreatorId]
                inner join [Status] s on s.[Id] = t.[StatusId]
            where {GetWhereByParamNumber(paramNumber)}";
            return Query<FieldsDetailInfo>(query).ToArray();
        }
        public int GetCountTasksByStatus(ParamResult paramResult)
        {
            string dquery = $@"
            select
                count(1)
            from {GetTaskJoinsOnUser_ByParamNumber(paramResult.Index)}
            where {GetWhereByParamNumber(paramResult.Index)}";

            return Query(dquery);
        }
        public List<int> GetCountTasksByStatus()
        {
            List<string> queries = new List<string>();
            for (int paramNumber = 1; paramNumber < 7; paramNumber++)
            {
                queries.Add($@"
                select
                {(paramNumber != 6 ? "count(1)" : "sum(case when te.[Minutes] is null then 0 else te.[Minutes] end)")}
                from {GetTaskJoinsOnUser_ByParamNumber(paramNumber)}
                where {GetWhereByParamNumber(paramNumber)}"
                );
            }
            return ExecScalarQueries(queries);
        }
        public bool SetTasksReadByStatus(ParamResult paramResult)
        {
            return SetTasksReadByStatus(paramResult.Index, string.Join(",", paramResult.Statuses));
        }
        public bool SetTasksReadByStatus(int paramNumber, string statusIds = "")
        {
            string query;

            if (paramNumber == 4 || paramNumber == 5)
            {
                query = $@"
                update [VisitedTask]
                set 
                    [NewComments] = cast(0 as {GetSqlTypeName(System.Data.SqlDbType.Bit)}),
                    [TaskView] = {GetDateTimeNowSqlFunc()}
                from [VisitedTask] vt
                inner join [User] u on u.[Login] = '{MainForm.UserLogin}' and u.[Id] = vt.[UserId]
                inner join [Task] t on t.[Id] = vt.[TaskId] {(paramNumber == 5 ? string.Empty : AND_Executors_LIKE_USER_NAME)}
                where vt.[NewComments] != cast(0 as {GetSqlTypeName(System.Data.SqlDbType.Bit)})";
            }
            else
            {
                query = $@"
                insert into [VisitedTask] ([TaskId], [UserId], [NewComments], [TaskView]) 
                select t.[Id] [TaskId], u.[Id] [UserId], cast(0 as {GetSqlTypeName(System.Data.SqlDbType.Bit)}), {GetDateTimeNowSqlFunc()}
                from [Task] t inner join [User] u on u.[Login] = '{MainForm.UserLogin}' {AND_Executors_LIKE_USER_NAME}
                left outer join [VisitedTask] vt on t.[Id] = vt.[TaskId] and u.[Id] = vt.[UserId]
                where t.[StatusId] in ({statusIds}) and vt.[TaskView] is null";
            }
            QueryUpsert(query);

            return true;
        }

    }
}
