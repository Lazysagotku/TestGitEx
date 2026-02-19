public bool SetTasksReadByIds(List<string> taskIds)
{
    if (taskIds == null || taskIds.Count == 0)
        return false;

    try
    {
        using (var dbConn = BaseConnMaker.Invoke())
        {
            dbConn.Open();

            // 1️⃣ Получаем UserId
            var idUsr = dbConn.ExecuteScalar<int>(
                @"select [Id] 
                  from [User] 
                  where [Login] = @login",
                new { login = MainForm.UserLogin }
            );

            // 2️⃣ Получаем Id записей VisitedTask
            var idTasks = dbConn.Query<int>(
                $@"select [Id]
                   from [VisitedTask]
                   where [TaskId] in @taskIds
                   and [UserId] = @userId",
                new { taskIds, userId = idUsr }
            ).ToList();

            if (idTasks.Count == 0)
                return false;

            // 3️⃣ Обновляем строго их
            dbConn.Execute(
                @"update [VisitedTask]
                  set [NewComments] = 0,
                      [TaskView] = GETDATE()
                  where [Id] in @ids",
                new { ids = idTasks }
            );

            return true;
        }
    }
    catch (Exception ex)
    {
        File.AppendAllText("debug.txt", $"{ex.Message}\n{ex.StackTrace}");
        return false;
    }
}
