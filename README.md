query


public bool SetTasksReadByIds(List<string> taskIds)
{
    if (taskIds == null || taskIds.Count == 0)
        return false;

    var ids = string.Join(",", taskIds.Select(id => $"'{id}'"));

    var idUsrQuery = $@"select [Id] from [User] where [Login] = '{MainForm.UserLogin}'";
    var idUsr = QueryScalar(idUsrQuery)?.ToString();

    if (string.IsNullOrEmpty(idUsr))
        return false;

    var idTasksQuery = $@"
        select [Id] 
        from [VisitedTask] 
        where [TaskId] in ({ids}) 
        and [UserId] = {idUsr}";

    var idTasksList = QueryList(idTasksQuery);

    if (idTasksList == null || idTasksList.Count == 0)
        return false;

    var idTasks = string.Join(",", idTasksList);

    var updateQuery = $@"
        update [VisitedTask]
        set 
            [NewComments] = cast(0 as {GetSqlTypeName(System.Data.SqlDbType.Bit)}),
            [TaskView] = {GetDateTimeNowSqlFunc()}
        where [Id] in ({idTasks})";

    QueryUpsert(updateQuery);

    return true;
}
