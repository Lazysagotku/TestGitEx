postgresql var idTasks = string.Join(",", $@"select [Id] from [VisitedTask] where [TaskId] in ({ids}) and [UserId]={idUsr}"); исправь чтобы запрос вывел в переменную через запятую
