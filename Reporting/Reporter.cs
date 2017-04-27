using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Reporting
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
            if (!File.Exists(Settings.Default.ActivityManagerPath))
            {
                MessageBox.Show(string.Format(CultureInfo.InvariantCulture,
                    "Не удалось найти генератор отчетов ActivityManager. Возможно указанный путь {0} является некорректным.",
                    Settings.Default.ActivityManagerPath),
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ThreadPool.QueueUserWorkItem((args) =>
            {
                using (var process = new Process())
                {
                    var psi = new ProcessStartInfo(Settings.Default.ActivityManagerPath,
                        GetArguments())
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                }
            }, Arguments);
        }

        private string GetArguments()
        {
            var argumentsString = "";
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
