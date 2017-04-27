using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reporting
{
    public sealed class DevicesFeaturesReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Характеристики оборудования";
            Arguments.Add("config", Path.Combine(Settings.Default.ActivityManagerConfigsPath, "DevicesFeatures.xml"));
            Arguments.Add("connectionString", Settings.Default.DevicesConnectionString);
            base.Run();
        }
    }
}
