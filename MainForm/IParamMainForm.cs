using System;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace TimeReportV3
{
    interface IParamMainForm
    {
        // Логика программы сейчас построена так, что каждый параметр должен содержать не более одной строки,
        // которая содержит возможность детализации.
        // Предполагается, что что если появится параметр, содержащий несколько строк и для каждой нужно делать детализацию, то
        // такой параметр нужно разбить на несколько.
        // Предполагается, что у параметра могут быть кроме главной строки с детализацией и второстепенные строки, но они не должны иметь детализаций.

        //SupportedType GetSupportedType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexParamMainForm">нужен для дальнейшего поиска соответствующего параметра для получения окна детализации</param>
        /// <returns></returns>
        ParamResult[] Get();
        ParamResult ParamResult { get; set; }
        //MainForm.SystemMode SystemMode { get; set; }

        FullFieldsTaskInfo GetDetails(IEnumerable<FieldsDetailInfo> fieldsDetails);
    }

    public class ParamResult : IEquatable<ParamResult>
    {
        public int Index { get; set; }
        public FieldsDetailInfo[] Details { get; set; }
        public bool IsPossiblyMakeRead { get; set; }
        public IEnumerable<TaskStatuses> Statuses { get; set; }
        public string ParameterName { get; set; }
        public string DetailsParameterName { get; set; }
        public bool IsCorrectly { get; set; } = false;
        public string ShowValue { get; set; }
        public int Count { get; set; } = 0;
        /// <summary>
        /// Нужно ли использовать минуты в таблице детализации
        /// </summary>
        public bool IsMinutesUsed { get; set; } = false;
        public bool IsDetailsAvailable { get; set; } = false;
        public Func<IEnumerable<FieldsDetailInfo>, FullFieldsTaskInfo> GetFullFieldsTaskInfo { get; set; }
        public string NameDo { get; set; } = "Сделать всё прочитанными";
        public Func<ParamResult, bool> SetAsRead { get; set; } = null;

        public Func<List<string>, bool> SetAsReadIds;
        public Func<ParamResult, FullFieldsTaskInfo> GetDetailedInfo { get; set; }


        public bool Equals(ParamResult other)
        {
            return other != null &&
                ParameterName == other.ParameterName &&
                ShowValue == other.ShowValue;
        }

        public void RefreshDetails()
        {
            //Details = GetDetailedInfo(this);
        }
    }

    public enum TaskQueries
    {
        Active = 1,
        NotInProgress = 2,
        WithoutExecutor = 3,
        UnreadUserComments = 4,
        UnreadAllComments = 5,
        SpentTime = 6,
    }
    public enum TaskStatuses
    {
        /// <summary>
        /// В работе
        /// </summary>
        InWork = 27,
        /// <summary>
        /// Требует уточнения
        /// </summary>
        NeedsDetails = 35,
        /// <summary>
        /// Отложена
        /// </summary>
        Suspended = 26,
        /// <summary>
        /// В ожидании назначения Исполнитель
        /// </summary>
        WaitExecutor = 31,
//26	Отложена
//27	В работе
//28	Закрыта
//29	Выполнена
//30	Отменена
//31	Открыта
//35	Требует уточнения
//36	Согласование
//37	Во внешней организац
//38	Переоткрыта
    }
}
