1.SettingsForm -
При загрузке формы foreach (var param in Params)
                      param.SetStartValue();

В кнопке "ОК" SettingsForm

foreach (var param in Params)
    param.SaveValue();

Properties.Settings.Default.Save();

2.
public List<string> GetVisibleTaskIds()
{
    var result = new List<string>();

    foreach (DataGridViewRow row in DgvDetailTable.Rows)
    {
        if (row.Tag is FieldsDetailInfo item && !string.IsNullOrEmpty(item.TaskId))
            result.Add(item.TaskId);
    }

    return result;
}
private void NewButtion_Click(object sender, EventArgs e)
{
    var ids = DetailTable.GetVisibleTaskIds();

    if (ids == null || ids.Count == 0)
        return;

    bool isOk = false;

    if (ParamResult.SetAsReadIds != null)
        isOk = ParamResult.SetAsReadIds(ids);

    if (!isOk)
    {
        MessageBox.Show("Не удалось обновить данные!", "Ошибка");
        return;
    }

    // 🔥 Перезапрашиваем данные
    ParamResult.RefreshDetails();

    // 🔥 Перерисовываем таблицу
    RefreshTable(false);

    // 🔥 Обновляем MainForm
    MainForm.BeginInvoke(new Action(() =>
    {
        MainForm.RefreshData1(null, null);
    }));
}
_readButton.Visible =
    ParamResult.SetAsReadIds != null &&
    IsPossiblyMakeRead;
В Show() перед циклом добавь:

DgvDetailTable.SuspendLayout();


В конце:

DgvDetailTable.ResumeLayout();
