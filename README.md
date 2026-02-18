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
