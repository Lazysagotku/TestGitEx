private object QueryScalar(string query)
{
    using (var connection = CreateConnection())
    {
        connection.Open();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = query;
            return command.ExecuteScalar();
        }
    }
}



private List<string> QueryList(string query)
{
    var result = new List<string>();

    using (var connection = CreateConnection())
    {
        connection.Open();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = query;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader[0].ToString());
                }
            }
        }
    }

    return result;
}






 protected static bool QueryUpsert(string query)
        {
            try
            {
                using (var dbConn = BaseConnMaker.Invoke())
                {
                    dbConn.Open();
                    string handledQuery = SetTargetChars(query);
                    var result = dbConn.Execute(SetTargetChars(query));
                    return true;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.txt", $"{ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
