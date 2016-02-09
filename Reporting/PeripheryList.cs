using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Devices.Reporting
{
    public sealed class PeripheryListReporter : Reporter
    {
        public override void Run()
        {
            ReportTitle = "Список периферийных устройств";
            Arguments.Add("config", Path.Combine(Reporting.Settings.Default.ActivityManagerConfigsPath, "PeripheryList.xml"));
            Arguments.Add("connectionString", Reporting.Settings.Default.DevicesConnectionString);           
            base.Run();                           
        }
    }
}
