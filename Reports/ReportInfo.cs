using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TimeReportV3.Reports;

namespace TimeReportV3
{
    public class ReportInfo
    {
        private static readonly Type reportInfoType = typeof(ReportInfo);
        public string SystemName { get; set; }
        //public int Minutes { get; set; }
        public decimal Hours { get; set; }
        public string Time { get; set; }
        internal static PropertyInfo[] props = reportInfoType.GetProperties();
        public static Report Get(ReportType reportType)
        {
            switch (reportType)
            {
                case ReportType.Consolid_Report:
                    return new Report
                    {
                        Columns = new List<Column>
                        {
                            DefaultColumns.Executor,
                            new Column("SystemName", "Система"),
                            DefaultColumns.TaskId,
                            DefaultColumns.TaskName,
                            DefaultColumns.Author,
                            DefaultColumns.Created,
                            DefaultColumns.Creator,
                            DefaultColumns.Deadline,
                            DefaultColumns.Completed,
                            DefaultColumns.Closed,
                            DefaultColumns.Service,
                            DefaultColumns.CFO,
                            DefaultColumns.Project,
                            DefaultColumns.EpicID,
                            DefaultColumns.Status,
                            DefaultColumns.Priority,
                            DefaultColumns.Minutes,
                            DefaultColumns.Hours,
                            DefaultColumns.Time,
                            DefaultColumns.JiraProject,
                            DefaultColumns.JiraEpicProject
                        },
                        ReportType = reportType,
                        Header = "Консолидированный отчет"
                        

                    };
                case ReportType.OpenTasks:
                    return new Report
                    {
                        Columns = new List<Column>
                        {
                            DefaultColumns.Service,
                            DefaultColumns.CFO,
                            DefaultColumns.Project,
                            DefaultColumns.TaskId,
                            DefaultColumns.TaskName,
                            DefaultColumns.Status,
                            DefaultColumns.Priority,
                            DefaultColumns.Author,
                            DefaultColumns.Executors,
                            DefaultColumns.Created,
                            new Column(nameof(Deadline), "Крайний срок"
                                , conditionalBackColor: (row) => { //=iif(IsDate(Fields!Deadline.Value) and Fields!Deadline.Value < DateTime.Now, "Pink", "White")
                                    if (row.Deadline > DateTime.MinValue && row.Deadline < DateTime.Now) return Color.FromArgb(255, 192, 203); return null; }),
                            new Column(nameof(Commentator), "Комментатор"),
                            new Column(nameof(LastChanged), "Добавлен"),
                            new Column(nameof(LastComment), "Комментарий (первые 100 символов)", new Style { WrapText = true, Width_mm = 65 }),
                        },
                        ReportType = reportType,
                        Header = "Задачи со статусом \"Открыта\""
                    };
                case ReportType.TaskList: //Учет времени по задачам
                    return new Report
                    {
                        Columns = new List<Column>
                        {
                            DefaultColumns.TaskId,
                            DefaultColumns.TaskName,
                            DefaultColumns.Priority,
                            DefaultColumns.Service,
                            DefaultColumns.CFO,
                            DefaultColumns.Project,
                            DefaultColumns.Customer,
                            DefaultColumns.Status,
                            DefaultColumns.Author,
                            DefaultColumns.Executors,
                            DefaultColumns.Created,
                            DefaultColumns.Deadline,
                            DefaultColumns.Completed,
                            DefaultColumns.Closed,
                        new Column(nameof(AllTime), "Часы"), //?  new Column("", "Часы", getDecimalValueFunc: (clmn) => { return Math.Round((decimal)clmn.Minutes / 60, 2); }),
                            DefaultColumns.Evaluation
                        },
                        ReportType = reportType,
                        Header = "Список задач за период"
                    };
                case ReportType.Time_By_User:
                    return new Report
                    {
                        Columns = new List<Column>
                        {
                            DefaultColumns.TaskId,
                            DefaultColumns.TaskName,
                            DefaultColumns.Author,
                            DefaultColumns.Customer,
                            DefaultColumns.Created,
                            DefaultColumns.Deadline,
                            DefaultColumns.Completed,
                            DefaultColumns.Closed,
                            DefaultColumns.Service,
                            DefaultColumns.CFO,
                            DefaultColumns.Project,
                            DefaultColumns.Status,
                            DefaultColumns.Priority,
                            DefaultColumns.Minutes,
                            DefaultColumns.Hours,
                            DefaultColumns.Time,
                            DefaultColumns.Evaluation
                        },
                        ReportType = reportType,
                        Header = string.Empty,
                        GetHeader = (comboBox) => { return $"Отчет по задачам по: {((ComboBox)comboBox).Text}"; }
                    };
                case ReportType.Time_By_Users:
                    return new Report
                    {
                        Columns = new List<Column>
                        {
                            DefaultColumns.Executor,
                            DefaultColumns.TaskId,
                            DefaultColumns.TaskName,
                            DefaultColumns.Author,
                            DefaultColumns.Customer,
                            DefaultColumns.Created,
                            DefaultColumns.Deadline,
                            DefaultColumns.Completed,
                            DefaultColumns.Closed,
                            DefaultColumns.Service,
                            DefaultColumns.CFO,
                            DefaultColumns.Project,
                            DefaultColumns.Status,
                            DefaultColumns.Priority,
                            DefaultColumns.Minutes,
                            DefaultColumns.Hours,
                            DefaultColumns.Time,
                            DefaultColumns.Evaluation
                        },
                        ReportType = reportType,
                        Header = "Отчет по задачам по всем пользователям"
                    };
                case ReportType.Times_On_Tasks:
                    return new Report
                    {
                        Columns = new List<Column>
                        {
                            DefaultColumns.TaskId,
                            DefaultColumns.TaskName,
                            DefaultColumns.Author,
                            DefaultColumns.Executor,
                            DefaultColumns.Customer,
                            DefaultColumns.Created,
                            DefaultColumns.Deadline,
                            DefaultColumns.Completed,
                            DefaultColumns.Closed,
                            DefaultColumns.Service,
                            DefaultColumns.CFO,
                            DefaultColumns.Project,
                            DefaultColumns.Status,
                            DefaultColumns.Priority,
                            DefaultColumns.Minutes,
                            DefaultColumns.Hours,
                            DefaultColumns.Time,
                        },
                        ReportType = reportType,
                        Header = "Учет времени по задачам"
                    };
                default:
                    return null;
            }
        }

        public int TaskId { get; set; }
        public string TaskStatus { get; set; }
        public string Priority { get; set; }
        public string Service { get; set; }
        public string ProjectName { get; set; }
        public string Customer { get; set; }
        public string TaskName { get; set; }
        public string Author { get; set; }
        public string Executor { get; set; }
        public string Executors { get; set; }
        public DateTime Created { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime Completed { get; set; }
        public DateTime Closed { get; set; }
        public int AllTime { get; set; }
        public int Minutes { get; set; }
        public string Evaluation { get; set; }
        public string CFO { get; set; }
        public string JiraProject { get; set; }
        public string EpicID { get; set; }
        public string JiraEpicProject { get; set; }
        public string Project { get; set; }
        public string Status { get; set; }
        public string Commentator { get; set; }
        public DateTime LastChanged { get; set; }
        public string LastComment { get; set; }
        public void CalculatedTime()
        {
            Hours = Math.Round(Minutes / 60m, 2);

            var ts =TimeSpan.FromMinutes(Minutes);
            Time = $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}";

        }
    }

    
    public class Column
    {
        public string Header { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DataTypeName { get; private set; }
        public Style Style { get; set; } = null;
        public Func<ReportInfo, string> GetStringValueFunc { get; private set; } = null;
        public Func<ReportInfo, int> GetInt32ValueFunc { get; private set; } = null;
        public Func<ReportInfo, DateTime> GetDateTimeValueFunc { get; set; } = null;
        public Func<ReportInfo, decimal> GetDecimalValueFunc { get; set; } = null;
        public Func<ReportInfo, Color?> ConditionalBackColor { get; set; } = null;
        public PropertyInfo ReportColumnInfo { get; set; } = null;

        public Column(
            string name
            , string header
            , Style styleInfo = null
            , Func<ReportInfo, string> getStringValueFunc = null
            , Func<ReportInfo, int> getInt32ValueFunc = null
            , Func<ReportInfo, DateTime> getDateTimeValueFunc = null
            , Func<ReportInfo, decimal> getDecimalValueFunc = null
            , Func<ReportInfo, Color?> conditionalBackColor = null)
        {
            Header = header;
            Name = name;
            if (styleInfo != null)
            {
                Style = styleInfo;
            }
            else
            {
                Style = new Style {};
            }
            if (getStringValueFunc != null)
            {
                GetStringValueFunc = getStringValueFunc;
                DataTypeName = "String";
            }
            else if (getInt32ValueFunc != null)
            {
                GetInt32ValueFunc = getInt32ValueFunc;
                DataTypeName = "Int32";
            }
            else if (getDateTimeValueFunc != null)
            {
                GetDateTimeValueFunc = getDateTimeValueFunc;
                DataTypeName = "DateTime";
            }
            else if (getDecimalValueFunc != null)
            {
                GetDecimalValueFunc = getDecimalValueFunc;
                DataTypeName = "Decimal";
            }
            else
            {
                var prop = ReportInfo.props.FirstOrDefault(p => p.Name == name);
                DataTypeName = prop?.PropertyType.Name ?? string.Empty;
                ReportColumnInfo = prop;
            }
            if (conditionalBackColor != null)
            {
                ConditionalBackColor = conditionalBackColor;
            }
            string dataTypeDefaultCellFormat = GetDefaultCellFormat(DataTypeName, out Aligns dataTypeDefaultHorisontal, out int width);
            if (string.IsNullOrEmpty(Style.CellFormat))
            {
                Style.CellFormat = dataTypeDefaultCellFormat;
            }
            if (Style.Width_mm == 0 && width > 0)
            {
                Style.Width_mm = width;
            }
            if (Style.Horizontal == Aligns.Default)
            {
                Style.Horizontal = dataTypeDefaultHorisontal;
            }
            if (Style.Vertical == Aligns.Default)
            {
                Style.Vertical = Aligns.C;
            }
        }

        private string GetDefaultCellFormat(string dataTypeName, out Aligns horisontal, out int width)
        {
            horisontal = Aligns.C;
            width = 0;
            switch (dataTypeName)
            {
                case "DateTime":
                    width = 16;
                    return "dd.mm.yyyy hh:mm";
                case "Int32":
                    return "###";
                case "Decimal":
                    return "# ##0.00";
                default:
                    horisontal = Aligns.L;
                    return "";
            }
        }

    }
    public enum ReportType { Time_By_User, TaskList, Times_On_Tasks, OpenTasks, Time_By_Users, Consolid_Report }

    public enum Aligns { L = 1, C = 2, R = 3, Default = 0, }

    public class Style
    {
        public int Width_mm { get; set; } = 0;
        public double FontHeightInPoints { get; set; } = 10;
        public string FontName { get; set; } = "Arial";
        public bool FontIsBold { get; set; } = false;
        public Aligns Horizontal { get; set; } = Aligns.Default;
        public Aligns Vertical { get; set; } = Aligns.Default;
        public Color? BackColor { get; set; } = null;
        public bool WrapText { get; set; } = false;
        public string CellFormat { get; set; } = string.Empty;
        public Func<ReportInfo, string> Hyperlink { get; set; } = null;
    }

    public class Report
    {
        public List<Column> Columns { get; set; }
        public ReportType ReportType { get; set; }
        public string Header { get; set; }
        public Func<object, string> GetHeader { get; set; } = null;
    }

    class DefaultColumns
    {
        internal static Column TaskName { get; } = new Column(nameof(TaskName), "Наименование задачи", new Style { WrapText = true, Width_mm = 55 });
        internal static Column TaskId { get; } = new Column(nameof(TaskId), "Id задачи", new Style { 
            Hyperlink = (clmn) => { 
            if (clmn.SystemName == "Jira")
                return $"{MainForm.TaskJiraUrl}{clmn.JiraProject}-{clmn.TaskId}";
            
            else
                return $"{MainForm.TaskViewUrl}{clmn.TaskId}"; 
        }
        });
        internal static Column Priority { get; } = new Column(nameof(Priority), "Приоритет", new Style { Horizontal = Aligns.C });
        internal static Column Service { get; } = new Column(nameof(Service), "Сервис");
        internal static Column CFO { get; } = new Column(nameof(CFO), "ЦФО", new Style { WrapText = true, Width_mm = 30 });
        internal static Column Project { get; } = new Column(nameof(Project), "Проект", new Style { WrapText = true, Width_mm = 23 });
        internal static Column EpicID { get; } = new Column(nameof(EpicID), "EpicID", new Style
        {
            Hyperlink = (clmn) =>
            {
                return $"{MainForm.TaskJiraUrl}{clmn.JiraEpicProject}-{clmn.EpicID}";
            }
        });
        internal static Column JiraProject { get; } = new Column(nameof(JiraProject), "JiraProject", new Style { WrapText = true, Width_mm = 23 });
        internal static Column JiraEpicProject { get; } = new Column(nameof(JiraEpicProject), "JiraEpicProject", new Style { WrapText = true, Width_mm = 23 });
        internal static Column Creator { get; } = new Column(nameof(Creator), "Создатель", new Style { WrapText = true, Width_mm = 30 });
        internal static Column Customer { get; } = new Column(nameof(Customer), "Заказчик");
        internal static Column Status { get; } = new Column(nameof(Status), "Статус", new Style { Horizontal = Aligns.C });
        internal static Column Author { get; } = new Column(nameof(Author), "Автор");
        internal static Column Executor { get; } = new Column(nameof(Executor), "Исполнитель");
        internal static Column Executors { get; } = new Column("", "Исполнители", new Style { WrapText = true, Width_mm = 30 }
            , (row) => { return row.Executors; } //?todo regex
        ); //=Trim(Fields!Executors.Value).Replace(", ", System.Environment.NewLine).Replace(System.Environment.NewLine + " ", System.Environment.NewLine)
        //(row) => { return row.Executors.Replace(",", Environment.NewLine).Replace($"{Environment.NewLine} ", Environment.NewLine).Trim(); } //?todo regex
        internal static Column Created { get; } = new Column(nameof(Created), "Создана");
        internal static Column Deadline { get; } = new Column(nameof(Deadline), "Крайний срок");
        internal static Column Completed { get; } = new Column(nameof(Completed), "Завершена"
            , conditionalBackColor: (row) => {
                if ((!row.Completed.Equals(DateTime.MinValue) && row.Deadline < row.Completed) || (row.Completed.Equals(DateTime.MinValue) && row.Deadline < DateTime.Now))
                        return Color.FromArgb(255, 192, 203);
                    return null; 
                }); //=iif(IsDate(Fields!Completed.Value) and Fields!Completed.Value > Fields!Deadline.Value or (Not(IsDate(Fields!Completed.Value)) and Fields!Deadline.Value < DateTime.Now), "Pink", "White")
        internal static Column Closed { get; } = new Column(nameof(Closed), "Закрыта");
        internal static Column Minutes { get; } = new Column(nameof(Minutes), "Минуты");
        internal static Column Hours { get; } = new Column(nameof(Hours), "Часы", getDecimalValueFunc: (clmn) => { return Math.Round((decimal)clmn.Minutes / 60, 2); });
        internal static Column Time { get; } = new Column(nameof(Time), "Время", getStringValueFunc: (clmn) => { return $"{clmn.Minutes / 60:00}:{clmn.Minutes % 60:00}"; }
            , styleInfo: new Style { Horizontal = Aligns.C });
       internal static Column Evaluation { get; } = new Column(nameof(Evaluation), "Оценка");
    }

}