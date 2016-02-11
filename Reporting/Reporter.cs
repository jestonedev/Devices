using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace Devices.Reporting
{
    public class Reporter
    {
        public string ReportTitle { get; set; }
        public Dictionary<string, string> Arguments { get; set; }
        
        public Reporter()
        {
            ReportTitle = "Unknown report";
            Arguments = new Dictionary<string, string>();
        }      

        public virtual void Run()
        {
            if (!File.Exists(Reporting.Settings.Default.ActivityManagerPath))
            {
                MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                    "Не удалось найти генератор отчетов ActivityManager. Возможно указанный путь {0} является некорректным.",
                    Reporting.Settings.Default.ActivityManagerPath),
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            SynchronizationContext context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem((args) =>
            {
                using (Process process = new Process())
                {
                    ProcessStartInfo psi = new ProcessStartInfo(Reporting.Settings.Default.ActivityManagerPath,
                        GetArguments());
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                }
            }, Arguments);
        }

        private string GetArguments()
        {
            string argumentsString = "";
            foreach (var argument in Arguments)
                argumentsString += String.Format(CultureInfo.InvariantCulture, "{0}=\"{1}\" ",
                    argument.Key.Replace("\"", "\\\""),
                    argument.Value.Replace("\"", "\\\""));
            return argumentsString; ;
        }
    }

    public class ReportOutputStreamEventArgs : EventArgs
    {
        public string Text { get; set; }

        public ReportOutputStreamEventArgs(string text)
        {
            Text = text;
        }
    }
}
