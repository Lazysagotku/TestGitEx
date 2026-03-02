using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TimeReportV3.Properties;
using TimeReportV3.Reports;

namespace TimeReportV3
{
    public partial class ReportForm : Form
    {
        // Версия с построением через SSRS см. в master до 2024-09-06

        private readonly ReportFile ReportFile;
        private bool IsReportCreating;
        private static DateTime date19000101 = new DateTime(1900, 1, 1);

        public ReportForm()
        {
            InitializeComponent();
            ReportFile = new ReportFile(dtpBegin, dtpEnd, rbTempFolder, rbMyDocuments, cmbUser);

            nudYear.Maximum = DateTime.Now.Year;
            if (DateTime.Now.Month == 1)
            {
                nudYear.Value = DateTime.Now.Year - 1;
            }
            else
            {
                nudYear.Value = DateTime.Now.Year;
            }
            dtpBegin.Value = DateTime.Now;
            dtpEnd.Value = DateTime.Now;
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToResizeRows = false; // и вот эта строка убирают стрелочку в таблице
            dataGridView1.AllowUserToAddRows = false;
            // Ensure columns exist
            if (dataGridView1.Columns.Count == 0)
            {
                var colName = new DataGridViewTextBoxColumn
                {
                    Name = "ReportName",
                    HeaderText = "Название отчёта",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };
                dataGridView1.Columns.Add(colName);

                var colType = new DataGridViewTextBoxColumn
                {
                    Name = "ReportType",
                    HeaderText = "ReportType",
                    Visible = false
                };
                dataGridView1.Columns.Add(colType);
            }

            dataGridView1.AutoSizeColumnsMode= DataGridViewAutoSizeColumnsMode.Fill;


            AddReportRow("Консолидированный отчет", ReportType.Consolid_Report);
            AddReportRow("Отчёт по времени пользователя", ReportType.Time_By_User);
            AddReportRow("Тех. отчёт", ReportType.Times_On_Tasks);
            AddReportRow("Список задач", ReportType.TaskList);
            AddReportRow("Открытые задачи", ReportType.OpenTasks);
            AddReportRow("По всем пользователям", ReportType.Time_By_Users);

            rbWeek_CheckedChanged(sender, e);
            initUsers();
        }

        private void AddReportRow(string displayName, ReportType repType)
        {
            int idx = dataGridView1.Rows.Add();
            dataGridView1.Rows[idx].Cells["ReportName"].Value = repType.ToString();
            dataGridView1.Rows[idx].Cells["ReportName"].Value = displayName;
            dataGridView1.Rows[idx].Tag = repType;
        }


        private void initUsers()
        {
            //cmbUser.DataSource = new BindingSource(MainForm.Users.OrderBy(user => user.Name), null);

            if (rbAllUsers.Checked) //|| radioButton1.Checked  || radioButton2.Checked )
            {
                cmbUser.DataSource = new BindingSource(MainForm.Users.OrderBy(user => user.Name), null);
            }
            else
            {
                cmbUser.DataSource = new BindingSource(UserTasksRepo.GetActiveUsers(dtpBegin.Value, dtpEnd.Value), null);
            }
            cmbUser.DisplayMember = "Name";
            cmbUser.ValueMember = "Id";

            if (MainForm.UserName != null)
                cmbUser.SelectedIndex = cmbUser.FindStringExact(MainForm.UserName);
            if (cmbUser.SelectedIndex < 0)
                if (cmbUser.Items.Count > 0)
                    cmbUser.SelectedIndex = 0;
        }

        private void rbActiveUsers_CheckedChanged(object sender, EventArgs e)
        {
            initUsers();
        }
        private void rbAllUsers_CheckedChanged(object sender, EventArgs e)
        {
            initUsers();
        }
        private void dtpEnd_ValueChanged(object sender, EventArgs e)
        {
            initUsers();
        }
        private void dtpBegin_ValueChanged(object sender, EventArgs e)
        {
            initUsers();
        }


        private void cmbKvartal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbKvartal.SelectedIndex == -1) return;
            int b = 0, f = 0, y = 0;
            switch (cmbKvartal.SelectedIndex)
            {
                case 0:
                    b = 1;
                    f = 4;
                    break;
                case 1:
                    b = 4;
                    f = 7;
                    break;
                case 2:
                    b = 7;
                    f = 10;
                    break;
                case 3:
                    b = 10;
                    f = 1;
                    break;
            };
            y = (int)nudYear.Value;
            dtpBegin.Value = DateTime.ParseExact($"{y}-{b:0#}-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (f == 1)
            {
                y++;
            };
            dtpEnd.Value = DateTime.ParseExact($"{y}-{f:0#}-01", "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(-1);
        }

        private void rbWeek_CheckedChanged(object sender, EventArgs e)
        {
            fDisabled();
            dtpBegin.Value = GetFirstDayOfWeek(DateTime.Now);
            dtpEnd.Value = DateTime.Now;
            initUsers();
        }
        private void fDisabled()
        {
            dtpBegin.Enabled = false;
            dtpEnd.Enabled = false;
            cmbMonth.Enabled = false;
            cmbMonth.SelectedIndex = -1;
            cmbKvartal.Enabled = false;
            cmbKvartal.SelectedIndex = -1;
            nudYear.Enabled = false;
        }

        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek)
        {
            DayOfWeek firstDay = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            return firstDayInWeek;
        }

        private void rbToDay_CheckedChanged(object sender, EventArgs e)
        {
            fDisabled();
            dtpBegin.Enabled = rbPeriod.Checked;
            dtpEnd.Enabled = rbPeriod.Checked;
            dtpBegin.Value = DateTime.Now;
            dtpEnd.Value = DateTime.Now;
            initUsers();
        }

        private void OpenFile(string sFileName)
        {
            try
            {
                Process.Start(sFileName);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Файл на данный момент не доступен. Попробуйте повторить попытку позже или обратитесь к разработчикам.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Ошибка открытия файла отчета. Текст ошибки: " + Ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            };
        }

        private void rbPeriod_CheckedChanged(object sender, EventArgs e)
        {
            fDisabled();
            dtpBegin.Enabled = true;
            dtpEnd.Enabled = true;
            initUsers();
        }

        private void rbMonth_CheckedChanged(object sender, EventArgs e)
        {
            fDisabled();
            cmbMonth.Enabled = true;
            nudYear.Enabled = true;
            if (DateTime.Now.Month == 1)
            {
                cmbMonth.SelectedIndex = 11;
            }
            else
            {
                cmbMonth.SelectedIndex = DateTime.Now.Month - 2;
            };
            if (DateTime.Now.Month > 1)
                nudYear.Value = DateTime.Now.Year;
            else
                nudYear.Value = DateTime.Now.Year - 1;
            initUsers();
        }

        private void rbKvartal_CheckedChanged(object sender, EventArgs e)
        {
            int q = 0;
            fDisabled();
            cmbKvartal.Enabled = true;
            nudYear.Enabled = true;
            switch (DateTime.Now.Month)
            {
                case 1:
                case 2:
                case 3:
                    q = 3;
                    nudYear.Value--;
                    break;
                case 4:
                case 5:
                case 6:
                    q = 0;
                    break;
                case 7:
                case 8:
                case 9:
                    q = 1;
                    break;
                case 10:
                case 11:
                case 12:
                    q = 2;
                    break;
            };
            cmbKvartal.SelectedIndex = q;
            initUsers();
        }

        private void cmbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMonth.SelectedIndex == -1) return;
            int m = cmbMonth.SelectedIndex + 1;
            int y = (int)nudYear.Value;
            dtpBegin.Value = DateTime.ParseExact($"{y}-{m:0#}-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (m == 12)
            {
                m = 1;
                y++;
            }
            else m++;
            dtpEnd.Value = DateTime.ParseExact($"{y}-{m:0#}-01", "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(-1);
        }

        private void rbPrevWeek_CheckedChanged(object sender, EventArgs e)
        {
            fDisabled();
            dtpBegin.Value = GetFirstDayOfWeek(DateTime.Now).AddDays(-7);
            dtpEnd.Value = GetFirstDayOfWeek(DateTime.Now).AddDays(-3);
            initUsers();
        }

        private void nudYear_ValueChanged(object sender, EventArgs e)
        {
            var begin = $"{dtpBegin.Value.Day:0#}-{dtpBegin.Value.Month:0#}-{nudYear.Value}";
            dtpBegin.Value = DateTime.ParseExact(begin, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var end = $"{dtpEnd.Value.Day:0#}-{dtpEnd.Value.Month:0#}-{nudYear.Value}";
            dtpEnd.Value = DateTime.ParseExact(end, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        }

        private void cbReportTimeByUser_Click(object sender, EventArgs e)
        {
            CreateExcelReport(ReportType.Time_By_User);
        }

        private void cbTechReport_Click(object sender, EventArgs e)
        {
            CreateExcelReport(ReportType.Times_On_Tasks);
        }

        private void cbTasksReport_Click(object sender, EventArgs e)
        {
            CreateExcelReport(ReportType.TaskList);
        }

        private void cbTasksOpen_Click(object sender, EventArgs e)
        {
            CreateExcelReport(ReportType.OpenTasks);
        }

        private void cbReportTimeByUsers_Click(object sender, EventArgs e)
        {
            CreateExcelReport(ReportType.Time_By_Users);
        }

        private void BuildConsolidatedReport(string fileName)
        {
            var service = new ConsolidatedReportService(
                dtpBegin.Value,
                dtpEnd.Value
                );
            service.BuildAndOpen(fileName);
        }

        private void CreateExcelReport(ReportType repType)
        {
            if (repType == ReportType.Consolid_Report)
            {
                BuildConsolidatedReport(ReportFile.FileName(ReportType.Consolid_Report));
                return;
               
            }

            // создание отчета
            if (cmbUser.SelectedIndex < 0)
                return;

            string sFileName = ReportFile.FileName(repType);
            Report report = ReportInfo.Get(repType);
            List<Column> columnSet = report.Columns;
            try
            {
                IsReportCreating = true;
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;

                XSSFWorkbook workbook = new XSSFWorkbook();
                XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet($"{repType}");
                sheet.DisplayGridlines = false;
                IEnumerable<ReportInfo> repRows = new ReportRepo().GetReportInfos(repType, cmbUser.SelectedValue.ToString(), dtpBegin.Value, dtpEnd.Value)
                    ?? new List<ReportInfo>();
                int rowNumber = 0;
                int columnNumber = 0;
                int rowSumNumber = 1; // строка суммовки
                XSSFCell headerReportCell = (XSSFCell)(sheet.CreateRow(rowNumber).CreateCell(columnNumber));
                XSSFRow headerRow = (XSSFRow)sheet.CreateRow(++rowNumber);
                Dictionary<string, XSSFCellStyle> styles = new Dictionary<string, XSSFCellStyle>();
                Dictionary<string, int> maxCharLength = new Dictionary<string, int>();

                int colNumMinutes = 0;
                int colNumHours = 0;

                foreach (Column column in columnSet)
                {
                    // запоминаем номера столбцов минут и часов для последующей суммовки
                    if (column.Name == "Minutes")
                    {
                        colNumMinutes = columnNumber;
                    }
                    else if (column.Name == "Hours")
                    {
                        colNumHours = columnNumber;
                    }

                    var headerCell = headerRow.CreateCell(columnNumber++);
                    headerCell.CellStyle = GetCellFormats(null, workbook, nameof(headerRow));
                    headerCell.SetCellValue(column.Header);
                    styles.Add(column.Name, GetCellFormats(column, workbook));
                    maxCharLength.Add(column.Name, column.Header.Length);
                }

                if (rdGroup.Checked == true && dtpBegin.Value != dtpEnd.Value && repType == ReportType.Time_By_User)
                {
                    GRcolNumMinutes = "";
                    GRcolNumHours = "";
                    // Группировка по дням отчета Time_By_User
                    for (DateTime dDay = dtpBegin.Value; dDay <= dtpEnd.Value; dDay = dDay.AddDays(1))
                    {
                        IEnumerable<ReportInfo> repGroupRows = new ReportRepo().GetReportInfos(repType, cmbUser.SelectedValue.ToString(), dDay, dDay)
                        ?? new List<ReportInfo>();
                        foreach (ReportInfo repRow in repGroupRows)
                        {
                            XSSFRow row = (XSSFRow)sheet.CreateRow(++rowNumber);
                            columnNumber = 0;
                            foreach (var column in columnSet)
                            {
                                XSSFCell cell = (XSSFCell)row.CreateCell(columnNumber++);
                                int charCount = SetCellValue(cell, repRow, column);
                                if (column.ConditionalBackColor != null)
                                {
                                    column.Style.BackColor = column.ConditionalBackColor.Invoke(repRow);
                                }
                                cell.CellStyle = styles[column.Name];

                                if (charCount > maxCharLength[column.Name])
                                {
                                    maxCharLength[column.Name] = charCount;
                                }
                            }
                        }
                        if (rowNumber != rowSumNumber && SetSumColumns(colNumMinutes, colNumHours, rowNumber, rowSumNumber, sheet, workbook, dDay.ToShortDateString(), 1, true))
                        {
                            ++rowNumber;
                            rowSumNumber = rowNumber;
                        }
                    }
                    SetFinalSumColumn(colNumMinutes, colNumHours, rowNumber, sheet, workbook);
                }
                else
                {
                    // Общим списком
                    foreach (ReportInfo repRow in repRows)
                    {
                        XSSFRow row = (XSSFRow)sheet.CreateRow(++rowNumber);
                        columnNumber = 0;
                        foreach (var column in columnSet)
                        {
                            XSSFCell cell = (XSSFCell)row.CreateCell(columnNumber++);
                            int charCount = SetCellValue(cell, repRow, column);
                            if (column.ConditionalBackColor != null)
                            {
                                column.Style.BackColor = column.ConditionalBackColor.Invoke(repRow);
                            }
                            cell.CellStyle = styles[column.Name];

                            if (charCount > maxCharLength[column.Name])
                            {
                                maxCharLength[column.Name] = charCount;
                            }
                        }
                    }
                    if (SetSumColumns(colNumMinutes, colNumHours, rowNumber, rowSumNumber, sheet, workbook, null, 2, false)) ++rowNumber;
                }

                // Ширина столбцов
                int[] maxCharLengths = maxCharLength.Values.ToArray();
                for (int i = 0; i < columnSet.Count; i++)
                {
                    if (columnSet[i].Style?.Width_mm > 0)
                    {
                        sheet.SetColumnWidth(i, columnSet[i].Style.Width_mm * 256);
                    }
                    else
                    {
                        sheet.SetColumnWidth(i, (maxCharLengths[i] + 4) * 256);
                    }
                }
                // Заголовок Отчета
                string dateStringHeader = " (" + dtpBegin.Value.ToShortDateString() + "-" + dtpEnd.Value.ToShortDateString() + ")";
                if (dtpBegin.Value.ToShortDateString() == dtpEnd.Value.ToShortDateString())
                    dateStringHeader = " (" + dtpBegin.Value.ToShortDateString() + ")";
                headerReportCell.SetCellValue($"{(report.GetHeader != null ? report.GetHeader.Invoke(cmbUser) + dateStringHeader : report.Header + dateStringHeader)}");
                headerReportCell.CellStyle = GetCellFormats(null, workbook, nameof(headerReportCell));
                CellRangeAddress cellRangeAddress = new CellRangeAddress(0, 0, 0, 8);
                sheet.AddMergedRegion(cellRangeAddress);
                sheet.SetAutoFilter(new CellRangeAddress(1, sheet.LastRowNum, 0, columnSet.Count - 1));

                using (FileStream fileStream = new FileStream(sFileName, FileMode.Create))
                {
                    workbook.Write(fileStream, false);
                }
                OpenFile(sFileName);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            finally
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
            };
        }

        string GRcolNumMinutes = "";
        string GRcolNumHours = "";

        private bool SetSumColumns(int colNumMinutes, int colNumHours, int rowNumber, int rowSumNumber, XSSFSheet sheet, XSSFWorkbook wb, string dd, byte formulaType, bool group)
        {
            rowSumNumber = rowSumNumber + 2;
            // суммовка минут/часов, если такие столбцы есть в отчете и существует хотя бы одна запись
            if (checkBoxSum.Checked || group)
            if (colNumMinutes > 0 && rowNumber > 1)
            {
                XSSFFont hFont = (XSSFFont)wb.CreateFont();
                hFont.FontHeightInPoints = 10;
                hFont.FontName = "Arial";
                hFont.IsBold = true;
                XSSFCellStyle hStyle = (XSSFCellStyle)wb.CreateCellStyle();
                hStyle.SetFont(hFont);

                XSSFRow rowSum = (XSSFRow)sheet.CreateRow(++rowNumber);
                XSSFCell cellSumMinutes = (XSSFCell)rowSum.CreateCell(colNumMinutes);
                string colNM = ColumnIndexToColumnLetter(colNumMinutes);
                string colNH = ColumnIndexToColumnLetter(colNumHours);

                string formulaString = "sum(";
                if (formulaType == 2)
                    formulaString = "subtotal(9,";

                cellSumMinutes.SetCellFormula(string.Format("{3}{0}{1}:{0}{2})", colNM, rowSumNumber, rowNumber, formulaString)); //"sum({0}{1}:{0}{2})"
                if (GRcolNumMinutes.Length > 0) GRcolNumMinutes = GRcolNumMinutes + ",";
                GRcolNumMinutes = GRcolNumMinutes + colNM + (rowNumber + 1);
                cellSumMinutes.CellStyle = hStyle;
                if (colNumHours > 0)
                {
                    XSSFCell cellSumHours = (XSSFCell)rowSum.CreateCell(colNumHours);
                    cellSumHours.SetCellFormula(string.Format("{3}{0}{1}:{0}{2})", colNH, rowSumNumber, rowNumber, formulaString));
                    if (GRcolNumHours.Length > 0) GRcolNumHours = GRcolNumHours + ",";
                    GRcolNumHours = GRcolNumHours + colNH + (rowNumber + 1);
                    cellSumHours.CellStyle = hStyle;
                }
                if(dd != null)
                {
                    XSSFCell cellDate = (XSSFCell)rowSum.CreateCell(0);
                    cellDate.SetCellValue("Итого за " + dd + ": ");
                    cellDate.CellStyle = hStyle;
                }
                else
                {
                    XSSFCell cellDate = (XSSFCell)rowSum.CreateCell(0);
                    cellDate.SetCellValue("Итого: ");
                    cellDate.CellStyle = hStyle;
                }

                for (int c = 0; c < rowSum.Cells.Count; c++)
                {
                    // заполнение светло-серым фоном
                    if (c > 0 && c != colNumMinutes && c != colNumHours)
                    {
                        XSSFCell cellColor = (XSSFCell)rowSum.CreateCell(c);
                        cellColor.CellStyle = hStyle;
                    }
                    rowSum.Cells[c].CellStyle.FillForegroundColor = 67;
                    rowSum.Cells[c].CellStyle.FillPattern = FillPattern.SolidForeground;
                }
                return true;
            }
            return false;
        }
        private bool SetFinalSumColumn(int colNumMinutes, int colNumHours, int rowNumber, XSSFSheet sheet, XSSFWorkbook wb)
        {
            // суммовка суммовок
            if (GRcolNumMinutes.Length > 0 || GRcolNumHours.Length > 0)
            {
                XSSFFont hFont = (XSSFFont)wb.CreateFont();
                hFont.FontHeightInPoints = 11;
                hFont.FontName = "Arial";
                hFont.IsBold = true;
                XSSFCellStyle hStyle = (XSSFCellStyle)wb.CreateCellStyle();
                hStyle.SetFont(hFont);

                XSSFRow rowSum = (XSSFRow)sheet.CreateRow(++rowNumber);
                XSSFCell cellSumMinutes = (XSSFCell)rowSum.CreateCell(colNumMinutes);
                if (GRcolNumMinutes.Length > 0)
                    cellSumMinutes.SetCellFormula(string.Format("sum({0})", GRcolNumMinutes));

                cellSumMinutes.CellStyle = hStyle;
                if (colNumHours > 0)
                {
                    XSSFCell cellSumHours = (XSSFCell)rowSum.CreateCell(colNumHours);
                    if (GRcolNumHours.Length > 0)
                        cellSumHours.SetCellFormula(string.Format("sum({0})", GRcolNumHours));
                    cellSumHours.CellStyle = hStyle;
                }
                XSSFCell cellDate = (XSSFCell)rowSum.CreateCell(0);
                    cellDate.SetCellValue("Итого за период");
                    cellDate.CellStyle = hStyle;

                for (int c = 0; c < rowSum.Cells.Count; c++)
                {
                    // заполнение светло-серым фоном
                    if (c > 0 && c != colNumMinutes && c != colNumHours)
                    {
                        XSSFCell cellColor = (XSSFCell)rowSum.CreateCell(c);
                        cellColor.CellStyle = hStyle;
                    }
                    rowSum.Cells[c].CellStyle.FillForegroundColor = 67;
                    rowSum.Cells[c].CellStyle.FillPattern = FillPattern.SolidForeground;
                }
                return true;
            }
            return false;
        }

        // преобразование номера столбца в его букву
        static string ColumnIndexToColumnLetter(int colIndex)
        {
            int div = colIndex + 1;
            string colLetter = String.Empty;
            int mod = 0;

            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        }

        private void ReportForm_Deactivate(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.DontLostFocusCollapse)
            {
                return;
            }
            if (IsReportCreating)
            {
                // произошла потеря фокуса из-за открытия Excel
                IsReportCreating = false;
                return;
            }
            Close();
        }

        private XSSFCellStyle GetCellFormats(Column repColumn, IWorkbook workbook, string targetRow = "")
        {
            string key;
            Style style;
            //bool isHeaderRow = false;
            //bool isHeaderReportRow = false;
            if (repColumn == null)
            {
                key = targetRow;
                if (targetRow == "headerRow")
                {
                    //isHeaderRow = true;
                    style = new Style
                    {
                        Horizontal = Aligns.C,
                        FontHeightInPoints = 11,
                        FontIsBold = true,
                    };

                }
                else if (targetRow == "headerReportCell")
                {
                    //isHeaderReportRow= true;
                    style = new Style
                    {
                        Horizontal = Aligns.L,
                        FontHeightInPoints = 24,
                    };
                }
                else
                {
                    style = new Style();
                }
            }
            else
            {
                key = repColumn.Header;
                style = repColumn.Style;
            }
            XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();

            cellStyle.Alignment = (NPOI.SS.UserModel.HorizontalAlignment)style.Horizontal;
            cellStyle.VerticalAlignment = (NPOI.SS.UserModel.VerticalAlignment)((int)style.Vertical - 1);

            cellStyle.WrapText = style.WrapText;

            if (!string.IsNullOrEmpty(style.CellFormat))
            {
                cellStyle.DataFormat = format.GetFormat(style.CellFormat);
            }
            if (style.BackColor.HasValue)
            {
                cellStyle.SetFillForegroundColor(new XSSFColor(new byte[] { style.BackColor.Value.R, style.BackColor.Value.G, style.BackColor.Value.B }));
                cellStyle.FillPattern = FillPattern.SolidForeground;
            }
            IFont font = workbook.CreateFont();
            NPOI.SS.UserModel.BorderStyle border;
            string[] settArr = Properties.Settings.Default.ReportFont.Split(';');
            if (settArr.Length == 3)
            {
                font.FontName = settArr[0];
                font.FontHeightInPoints = (targetRow != "headerReportCell") ? double.Parse(settArr[1]) : style.FontHeightInPoints;
                switch (settArr[2])
                {
                    case "тонкая":
                        border = NPOI.SS.UserModel.BorderStyle.Thin;
                        break;
                    case "толстая":
                        border = NPOI.SS.UserModel.BorderStyle.Medium;
                        break;
                    default:
                        border = NPOI.SS.UserModel.BorderStyle.Hair;
                        break;
                        //нет тонкая толстая
                }
            }
            else
            {
                font.FontName = style.FontName;
                border = NPOI.SS.UserModel.BorderStyle.Thin;
                font.FontHeightInPoints = style.FontHeightInPoints;
            }

            font.IsBold = style.FontIsBold;
            if (style.Hyperlink != null)
            {
                font.Underline = FontUnderlineType.Single;
                font.Color = HSSFColor.Blue.Index;
            }
            cellStyle.SetFont(font);

            cellStyle.BorderTop = border;
            cellStyle.BorderBottom = border;
            cellStyle.BorderLeft = border;
            cellStyle.BorderRight = border;

            return cellStyle;
        }

        private int SetCellValue(XSSFCell cell, ReportInfo repRow, Column column)
        {
            int charCount = 0;
            object propVal = column.ReportColumnInfo?.GetValue(repRow) ?? null;
            switch (column.DataTypeName)
            {
                case "Int32":
                    int? intVal = null;
                    if (propVal != null)
                    {
                        intVal = (int)propVal;
                    }
                    else if (column.GetInt32ValueFunc != null)
                    {
                        intVal = column.GetInt32ValueFunc.Invoke(repRow);
                    }
                    if (!intVal.HasValue)
                    {
                        return 0;
                    }
                    cell.SetCellValue(intVal.Value);
                    charCount = intVal.Value.ToString().Length;
                    break;
                case "DateTime":
                    DateTime? dateValue = null;
                    if (propVal != null)
                    {
                        dateValue = (DateTime?)propVal;
                    }
                    else if (column.GetInt32ValueFunc != null)
                    {
                        dateValue = column.GetDateTimeValueFunc.Invoke(repRow);
                    }
                    if (!dateValue.HasValue || dateValue.Value.Equals(DateTime.MinValue) || dateValue.Value.Equals(date19000101))
                    {
                        return 0;
                    }
                    cell.SetCellValue(dateValue.Value);
                    break;
                case "Decimal":
                    double? doubleVal = null;
                    if (propVal != null)
                    {
                        doubleVal = (double?)propVal;
                    }
                    else if (column.GetDecimalValueFunc != null)
                    {
                        var r = (double)column.GetDecimalValueFunc.Invoke(repRow);
                        cell.SetCellValue((double)column.GetDecimalValueFunc.Invoke(repRow));
                    }
                    if (!doubleVal.HasValue)
                    {
                        return 0;
                    }
                    cell.SetCellValue(doubleVal.Value);
                    charCount = doubleVal.Value.ToString("0.00").Length;
                    break;
                default:
                    string strVal = "";
                    if (propVal != null)
                    {
                        strVal = (string)propVal;
                        cell.SetCellValue(strVal);
                    }
                    else if (column.GetStringValueFunc != null)
                    {
                        strVal = column.GetStringValueFunc.Invoke(repRow);
                        cell.SetCellValue(strVal);
                    }
                    if (strVal == null) strVal = "";
                    charCount = strVal.Length;
                    break;
            }
            if (column.Style.Hyperlink != null)
            {
                XSSFHyperlink link = new XSSFHyperlink(HyperlinkType.Url)
                {
                    Address = column.Style.Hyperlink.Invoke(repRow)
                };
                cell.Hyperlink = (link);
            }
            return charCount;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cmbUser_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dataGridView1.Rows[e.RowIndex];
            if (row.Tag is ReportType repType)
            {
                CreateExcelReport(repType);
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
    }

}
