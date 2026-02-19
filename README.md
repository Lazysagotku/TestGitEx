protected static List<string> ExecList(string query)
{
    using (var dbConn = BaseConnMaker.Invoke())
    {
        dbConn.Open();
        return dbConn.Query<string>(query).ToList();
    }
}
var idTasksList = ExecList(idTasksQuery);
