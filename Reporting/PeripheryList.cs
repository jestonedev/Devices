using System.IO;

namespace Reporting
{
    public sealed class PeripheryListReporter : Reporter
    {
        public override void Run()
        {
            ReportTitle = "Список периферийных устройств";
            Arguments.Add("config", Path.Combine(Settings.Default.ActivityManagerConfigsPath, "PeripheryList.xml"));
            Arguments.Add("connectionString", Settings.Default.DevicesConnectionString);           
            base.Run();                           
        }
    }
}
