using System;
using System.Windows.Forms;
using TrfCommonUtility;
using System.Threading.Tasks;
using System.Linq;

namespace TimeReportV3
{
    // предыдущий проект находится в хранилище и называется TimeReport2009
    public static class Program
    {
        /// 
        /// Главная точка входа для приложения.
        /// 
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new MainForm();
            if (Environment.GetCommandLineArgs().Contains("show"))
            {
                form.Shown += (s, e) =>
                {
                    form.Show();
                    form.WindowState = FormWindowState.Normal;

                };
            }

            Application.Run(form);

            /*var mainForm = new MainForm();
            Task.Run(() =>
            {
                mainForm.PreloadData();
            });

            Application.Run(mainForm);*/
            // Нужно запретить пользователям запускать исполняемый из сетевой папки, так как его иначе не удастся обновлять!
            /*if (ProgramUtility.IsRunningFromNetwork(Application.StartupPath))
            {
                AutoClosingMessageBox.Show($"Для запуска приложения нужно использовать {Application.ProductName}Starter.exe!", "Предупреждение!", 5000, MessageBoxIcon.Warning);
                return;
            }

            using (var instance = new ProgramInstance())
            {
                if (instance.IsRun())
                    return;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }*/
        }
    }
}
