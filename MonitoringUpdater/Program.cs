using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace MonitoringUpdater
{
    internal class Program
    {
        private static void Main()
        {
            var vplogPath = ConfigurationManager.AppSettings["VplogPath"];
            if (!Directory.Exists(vplogPath))
            {
                var bgColor = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Не передана ссылка на каталог с файлами мониторинга");
                Console.ForegroundColor = bgColor;
                return;
            }
            var propertiesLoader = new MonitoringPropertiesLoader(vplogPath);

            IEnumerable<MonitoringDevice> monitoringDevices;
            try
            {
                monitoringDevices = propertiesLoader.LoadMonitoringDevices();
            }
            catch (IOException e)
            {
                var bgColor = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка загрузки файлов мониторинга. Подробнее: {0}", e.Message);
                Console.ForegroundColor = bgColor;
                return;
            }
            try
            {
                var propertiesDbSaver = new MonitoringPropertiesDbSaver(ConfigurationManager.ConnectionStrings["Default"].ConnectionString);
                propertiesDbSaver.SaveMonitoringDevices(monitoringDevices);
            }
            catch (SqlException e)
            {
                var bgColor = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка сохранения информации мониторинга. Подробнее: {0}", e.Message);
                Console.ForegroundColor = bgColor;
            }
        }
    }
}
