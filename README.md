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
