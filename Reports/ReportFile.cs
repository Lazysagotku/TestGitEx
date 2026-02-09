using System;
using System.IO;
using System.Windows.Forms;

namespace TimeReportV3
{
    internal class ReportFile
    {
        private readonly DateTimePicker DtpBegin;
        private readonly DateTimePicker DtpEnd;

        private readonly RadioButton RbTempFolder;
        private readonly RadioButton RbMyDocuments;
        private readonly ComboBox CmbUser;

        private string TempFolder;
        private string MyDocuments;

        public ReportFile(DateTimePicker dtpBegin, DateTimePicker dtpEnd, RadioButton rbTempFolder, RadioButton rbMyDocuments, ComboBox cmbUser)
        {
            DtpBegin = dtpBegin;
            DtpEnd = dtpEnd;

            RbTempFolder = rbTempFolder;
            RbMyDocuments = rbMyDocuments;

            RbTempFolder.Checked = true;
            //RbTempFolder.Text = Environment.GetEnvironmentVariable("TEMP");
            //RbMyDocuments.Text = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            const string reportFolder = "Отчеты";
            TempFolder = $"{Environment.GetEnvironmentVariable("TEMP")}{Path.DirectorySeparatorChar}{reportFolder}";
            MyDocuments = $"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}{Path.DirectorySeparatorChar}{reportFolder}";

            ToolTip toolTipRbTempFolder = new ToolTip();
            toolTipRbTempFolder.SetToolTip(RbTempFolder, TempFolder);

            ToolTip toolTipRbMyDocuments = new ToolTip();
            toolTipRbMyDocuments.SetToolTip(RbMyDocuments, MyDocuments);

            CmbUser = cmbUser;
        }

        public string FileName(ReportType typeReport)
        {
            string sFileName = RbTempFolder.Checked ? TempFolder : MyDocuments;
            
            //sFileName += $"{Path.DirectorySeparatorChar}Отчеты";
            if (!Directory.Exists(sFileName))
            {
                Directory.CreateDirectory(sFileName);
            };
            sFileName += Path.DirectorySeparatorChar;

            switch (typeReport)
            {
                case ReportType.Consolid_Report:
                    sFileName += ReportType.Consolid_Report.ToString();
                    break;
                case ReportType.OpenTasks:
                    sFileName += ReportType.OpenTasks.ToString();
                    break;
                case ReportType.Time_By_User:
                    sFileName += $"{CmbUser.Text}{Path.DirectorySeparatorChar}";
                    if (!Directory.Exists(sFileName))
                    {
                        Directory.CreateDirectory(sFileName);
                    };
                    //if (CbxTempFile.Checked)
                    //    sFileName += "tmp_";

                    sFileName += CmbUser.Text;
                    break;
                case ReportType.TaskList:
                    sFileName += ReportType.TaskList.ToString();
                    break;
                case ReportType.Times_On_Tasks:
                    sFileName += ReportType.Times_On_Tasks.ToString();
                    break;
                case ReportType.Time_By_Users:
                    sFileName += ReportType.Time_By_Users.ToString();
                    break;
            }

            if (typeReport == ReportType.OpenTasks)
            {
                sFileName += $"_за {DateTime.Now:dd.MM.yyyy}.xlsx";
            }
            else if (DtpBegin.Value != DtpEnd.Value)
            {
                sFileName += $"_с {DtpBegin.Value:dd.MM.yyyy} по {DtpEnd.Value:dd.MM.yyyy}.xlsx";
            }
            else
            {
                sFileName += $"_за {DtpBegin.Value:dd.MM.yyyy}.xlsx";
            }

            sFileName = GetFreeFileName(sFileName, 0);
            return sFileName;
        }

        private string GetFreeFileName(string tmpFullName, int i)
        {
            string testFileName;
            if (i == 0)
            {
                testFileName = tmpFullName;
            }
            else
            {
                var path = Path.GetDirectoryName(tmpFullName); 
                var tmpNameWithoutExtension = Path.GetFileNameWithoutExtension(tmpFullName);
                testFileName = $"{path}{Path.DirectorySeparatorChar}{tmpNameWithoutExtension}({i}).xlsx";
            }

            if (!File.Exists(testFileName))
                return testFileName;
            
            if (RbTempFolder.Checked)
            {
                try
                {
                    File.Delete(testFileName);
                    return testFileName;
                }
                catch
                {
                    return GetFreeFileName(tmpFullName, ++i);
                }
            }
            
            return GetFreeFileName(tmpFullName, ++i);
        }
    }
}
