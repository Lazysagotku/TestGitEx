public bool SetTasksReadByIds(List<string> taskIds)
{
    if (taskIds == null || taskIds.Count == 0)
        return false;

    var ids = string.Join(",", taskIds.Select(id => $"'{id}'"));

    var query = $@"
        update vt
        set 
            vt.[NewComments] = cast(0 as {GetSqlTypeName(System.Data.SqlDbType.Bit)}),
            vt.[TaskView] = {GetDateTimeNowSqlFunc()}
        from [VisitedTask] vt
        inner join [User] u 
            on u.[Login] = '{MainForm.UserLogin}' 
            and u.[Id] = vt.[UserId]
        where vt.[TaskId] in ({ids})
    ";

    QueryUpsert(query);

    return true;
}