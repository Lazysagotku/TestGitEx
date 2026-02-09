using System;

namespace TimeReportV3
{
    public class FullFieldsTaskInfo
    {
        public FieldsDetailInfo[] FieldsTaskInfos { get; set; }

        /// <summary>
        /// Доступно обновление
        /// </summary>
        public bool IsPossiblyMakeRead { get; set; }
    }

    /// <summary>
    /// Если нужно менять поля в этом классе, то также необходимо менять соответствующие поля в sql-запросах.
    /// Иначе Dapper не распознает измененные поля.
    /// </summary>
    public class FieldsDetailInfo
    {
        /// <summary>
        /// Номер задачи
        /// </summary>
        public string TaskId { get; set; }


        public string System { get; set; }
        public string KeyTaskId { get; set; }

        /// <summary>
        /// Заголовок задачи
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Списанное время в минутах
        /// В настоящий момент используется только при детализации параметра ParamTimeSpentByUser
        /// Если значение < 0, то не используется в таблице детализации
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Id инициатора задачи
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// Исполнители
        /// </summary>
        public string Executors { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime Changed { get; set; }

        /// <summary>
        /// Статус заявки
        /// </summary>
        public string StatusValue { get; set; }
        /// <summary>
        /// Приоритет заявки
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        /// Приоритеты:
        /// SortOrder = 1 => Низкий;
        /// SortOrder = 2 => Средний;
        /// SortOrder = 3 => Высокий;
        /// SortOrder = 4 => Критический;
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Это поле вспомогательное, в БД отсутствует. 
        /// Означает новые записи, которые нужно выделить жирным шрифтом
        /// </summary>
        public int IsSelected { get; set; }
    }
}
