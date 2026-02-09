using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TimeReportV3.Repository;
using System.Linq;

namespace TimeReportV3.Reports
{
    internal class ConsolidatedReportService
    {
        private readonly DateTime _begin;
        private readonly DateTime _end;

        public ConsolidatedReportService(DateTime begin, DateTime end)
        {
            _begin = begin;
            _end = end;
        }

        private DataTable SortByExecutor(DataTable source)
        {
            var view = source.DefaultView;
            view.Sort = "Executor ASC, TaskId ASC";
            return view.ToTable();
        }

        public void BuildAndOpen(string fileName)
        {
            var isRepo = new ConsolidatedIsRepo();
            var jiraRepo = new ConsolidatedJiraRepo();

            DataTable isData = SortByExecutor(isRepo.GetData(_begin, _end));
            DataTable jiraData = SortByExecutor(jiraRepo.GetData(_begin, _end));

            DataTable consolidated = isData.Clone();

            foreach (DataRow r in isData.Rows)
                consolidated.Rows.Add(r.ItemArray);

            foreach (DataRow r in jiraData.Rows)
                consolidated.Rows.Add(r.ItemArray);

            consolidated = SortByExecutor(consolidated);




            var builder = new ConsolidatedExcelBuilder();
            builder.Build(fileName,isData,jiraData,consolidated,_begin, _end);

           


        }

       
    }
}