using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ClosedXML.Excel;
using NPOI.XSSF.UserModel;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using Org.BouncyCastle.Tls;

namespace TimeReportV3.Repository
{
    internal class ConsolidatedExcelBuilder
    {
        private static readonly Dictionary<string, string> ColumnTitles =
            new Dictionary<string, string>
            {
                            ["Executor"] = "Исполнитель",
                            ["SystemName"] = "Система",
                            ["TaskId"] = "Номер задачи",
                            ["TaskName"] = "Наименование задачи",
                            ["Author"] = "Автор",
                            ["Created"] = "Создано",
                            ["Creator"] = "Создатель",
                            ["Deadline"] = "Срок",
                            ["Completed"] = "Завершена",
                            ["Closed"] = "Закрыта",
                            ["Service"] = "Сервис",
                            ["CFO"] = "ЦФО",
                            ["Project"] = "Проект",
                            ["EpicID"] = "Эпик",
                            ["Status"] = "Статус",
                            ["Priority"] = "Приоритет",
                            ["Minutes"] = "Минуты",
                            ["Hours"] = "Часы",
                            ["Time"] = "Время",
                            ["JiraProject"] = "ПроектЖира",
                            ["JiraEpicProject"] = "ЭпикПроектЖира"
            };
        public void Build(string fileName, DataTable isData, DataTable jiraData, DataTable consolidated, DateTime begin, DateTime end)
        {
            var wb = new XSSFWorkbook();

            CreateSheet(wb, "IntraService", isData, withTotals: false);
            CreateSheet(wb, "Jira", jiraData, withTotals: false);
            CreateSheet(wb, "Consolidated", consolidated, withTotals: true);

            if (File.Exists(fileName))
                File.Delete(fileName);

            using (var fs = System.IO.File.Create(fileName))
            {
                wb.Write(fs);
            }

            Process.Start(fileName);
        }

        private void CreateSheet(XSSFWorkbook wb, string name, DataTable dt, bool withTotals)
        {
            var sheet = wb.CreateSheet(name);

            var headerStyle = wb.CreateCellStyle();
            var headerFont = wb.CreateFont();
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);

            var linkStyle = wb.CreateCellStyle();
            var linkFont = wb.CreateFont();
            linkFont.Underline = FontUnderlineType.Single;
            linkFont.Color = NPOI.SS.UserModel.IndexedColors.Blue.Index;
            linkStyle.SetFont(linkFont);


            //header

            var header = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                //header.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                var colName = dt.Columns[i].ColumnName;

                var title = ColumnTitles.TryGetValue(colName, out var ru)
                    ? ru
                    : colName;

                var cell = header.CreateCell(i);
                cell.SetCellValue(title);
                cell.CellStyle = headerStyle;   


               
            }
            //data

            string currentExecutor = null; 
            double sumMinutes = 0;
            double sumHours = 0;
            TimeSpan sumTime = TimeSpan.Zero;

            int excelRow = 1;

            foreach (DataRow dr in dt.Rows)
            {
                var executor = dr.Table.Columns.Contains("Executor")
                    ? dr["Executor"]?.ToString() : null;

                if (withTotals && currentExecutor != null && executor != currentExecutor)
                {
                    WriteTotalRow(sheet, excelRow++, dt, sumMinutes, sumHours, sumTime);
                    sumMinutes = 0;
                    sumHours = 0;
                    sumTime = TimeSpan.Zero;
                }

                currentExecutor = executor;

                var row = sheet.CreateRow(excelRow++);

                for (int c = 0;c< dt.Columns.Count;c++)
                {
                    var colName = dt.Columns[c].ColumnName;
                    var cell = row.CreateCell(c);
                    var val = dr[c];

                    cell.SetCellValue(val?.ToString() ?? "");

                    //hyperlinks

                    if (colName == "TaskId")
                    {
                        var system = dr["SystemName"]?.ToString();

                        if (system == "JIRA")
                        {
                            var project = dr["JiraProject"]?.ToString();
                            cell.Hyperlink = new XSSFHyperlink(HyperlinkType.Url)
                            {
                                Address = $"http://itdesk.trinfico.ru/browse/{project}-{val}"
                            };
                            cell.CellStyle = linkStyle;
                        }
                        else
                        {

                            cell.Hyperlink = new XSSFHyperlink(HyperlinkType.Url)
                            {
                                Address = $"https://alefsupport.trinfico.ru/Task/View/{val}"
                            };
                            cell.CellStyle = linkStyle;
                        }
                    }
                    if (colName == "EpicID")
                    {
                        var system = dr["SystemName"]?.ToString();

                        if (system == "JIRA")
                        {
                            var proj = dr["JiraEpicProject"]?.ToString();
                            cell.Hyperlink = new XSSFHyperlink(HyperlinkType.Url)
                            {
                                Address = $"http://itdesk.trinfico.ru/browse/{proj}-{val}"
                            };
                            cell.CellStyle = linkStyle;
                        }
                    }

                    if (withTotals)
                    {
                        if (colName == "Minutes")
                            sumMinutes += Convert.ToDouble(val);
                        if (colName == "Hours")
                            sumHours += Convert.ToDouble(val);
                        if (colName == "Time" && TimeSpan.TryParse(val?.ToString(), out var ts))
                            sumTime += ts;
                }
                }
                
            }

            if (withTotals && currentExecutor != null)
            {
                WriteTotalRow(sheet, excelRow, dt, sumMinutes, sumHours, sumTime);
            }

            

            for(int i=0; i < dt.Columns.Count;i++)
                sheet.AutoSizeColumn(i);

            sheet.CreateFreezePane(0, 1);
            sheet.SetAutoFilter(new NPOI.SS.Util.CellRangeAddress(0,sheet.LastRowNum,0,dt.Columns.Count-1));

        }

        private void WriteTotalRow(ISheet sheet, int rowIndex, DataTable dt, double minutes, double hours, TimeSpan time)
        {
            var row = sheet.CreateRow(rowIndex);

            row.CreateCell(0).SetCellValue("ИТОГО:");

            for (int i = 0; i < dt.Columns.Count;i++)
            {
                var col = dt.Columns[i].ColumnName;

                if (col == "Minutes")
                    row.CreateCell(i).SetCellValue(minutes);
                if (col =="Hours")
                    row.CreateCell(i).SetCellValue(hours);
                if (col =="Time")
                    row.CreateCell(i).SetCellValue(time.ToString(@"DD\:hh\:mm"));


            }
        }
    }
}
