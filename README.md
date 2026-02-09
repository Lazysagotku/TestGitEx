1. DeetailForm отображение данных 4 и 5 строки. А) Производит дубли задач в списке, которые уже есть, после клика на одну из них Б)  Доп.окно со списком непрочитанных комментариев. Теперь меняется горизонтальный размер окна/таблицы. После перехода в (любую из списка) задачу таблица дополняется лишними строками, а выбранная задача из списка не убирается. После переоткртия окна нормализуется до следующего перехода в задачу.
2. "Сделать прочитанным" опять делает все прочитанным, а не по-одному
3. При первом запуске приложения основное окно всё так же открывается по центру экрана. 
4. При клике на оповещение о задачах без исполнителя доп.окно со списком задач открывается не всегда.
5. Ассинхронность 
int firstDataRow = 2;
int lastDataRow = RowsToExcel + 1;

for (int row = firstDataRow; row <= lastDataRow; row++)
{
    // Проверяем, не итоговая ли строка
    var firstCell = workSheet.Cells[row, 1].Value;

    if (firstCell != null)
    {
        string text = firstCell.ToString();

        if (text.Contains("ИТОГО") || text.Contains("СРЕДНЕЕ"))
            continue;
    }

    // ---------- Столбец H (8) ----------
    Excel.Range cellH = workSheet.Cells[row, 8];

    if (cellH.Value != null &&
        double.TryParse(cellH.Value.ToString(), out double valH))
    {
        if (valH <= -15)
        {
            cellH.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightPink);
        }
    }

    // ---------- Столбец J (10) ----------
    Excel.Range cellJ = workSheet.Cells[row, 10];

    if (cellJ.Value != null &&
        double.TryParse(cellJ.Value.ToString(), out double valJ))
    {
        if (valJ <= -15)
        {
            cellJ.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightPink);
        }
    }
}
