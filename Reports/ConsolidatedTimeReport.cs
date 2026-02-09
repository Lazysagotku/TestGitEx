using System;
using System.Data;
using TimeReportV3.Repository;

namespace TimeReportV3.Reports
{
    internal sealed class ConsolidatedTimeReport
    {
        /* public string Name => "Консолидированный отчёт (IS / Jira)";

         public DataTable Build(DateTime from, DateTime to)
         {
             var dtJira = LoadJira(from, to);
             var dtIs = LoadIntraService(from, to);

             var result = dtIs.Clone();

             foreach (DataRow r in dtJira.Rows)
                 result.ImportRow(r);

             foreach (DataRow r in dtIs.Rows)
                 result.ImportRow(r);

             Sort(result);
             return result;
         }

         private DataTable LoadJira(DateTime from,
                                    DateTime to)
         {
             string sql = SqlQueries.Jira
               .Replace("@begin", $"'{from:yyyy-MM-dd}'")
               .Replace("@end", $"'{to:yyyy-MM-dd}'");

             //return JiraRepository.QueryDataTable(sql);
         }

         private DataTable LoadIntraService(DateTime from, DateTime to)
         {
             string sql = SqlQueries.IntraService
               .Replace("@begin", $"'{from:yyyy-MM-dd}'")
               .Replace("@end", $"'{to:yyyy-MM-dd}'");

             return IntraServiceRepository.Query(sql);
         }

         private void Sort(DataTable dt)
         {
             dt.DefaultView.Sort =
               "Исполнитель ASC, Система учёта ASC, Номер задачи ASC";
         }*/
    }
}