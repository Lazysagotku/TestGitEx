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
        where vt.[Id] in (
            select vt2.[Id]
            from [VisitedTask] vt2
            inner join [User] u 
                on u.[Login] = '{MainForm.UserLogin}'
                and u.[Id] = vt2.[UserId]
            where vt2.[TaskId] in ({ids})
        )
    ";

    QueryUpsert(query);

    return true;
}
