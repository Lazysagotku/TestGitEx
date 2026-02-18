public bool SetTasksReadByIds(List<string> taskIds)
        {
            if (taskIds == null || taskIds.Count == 0)
                return false;

            var ids = string.Join(",", taskIds.Select(id => $"{id}"));

            select "Id" from "User" where "Login" = 'IArkhipov'
            select "Id" from "VisitedTask" where "TaskId" in (34418) and "UserId" = 601
            update "VisitedTask" set "NewComments" = true where "Id" in (135206, 152986);

            var query = $@"
        update [VisitedTask]
        set 
            [NewComments] = cast(0 as {GetSqlTypeName(System.Data.SqlDbType.Bit)}),
            [TaskView] = {GetDateTimeNowSqlFunc()}
        from [VisitedTask] vt
        inner join [User] u 
            on u.[Login] = '{MainForm.UserLogin}' 
            and u.[Id] = vt.[UserId]
        where vt.[TaskId] in ({ids})
    ";

            QueryUpsert(query);

            return true;
        }
