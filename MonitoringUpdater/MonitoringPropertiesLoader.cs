using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MonitoringUpdater
{
    internal class MonitoringPropertiesLoader
    {
        private readonly string _vplogPath;

        public MonitoringPropertiesLoader(string vplogPath)
        {
            if (string.IsNullOrEmpty(vplogPath))
            {
                throw new ArgumentNullException("vplogPath");
            }
            if (!Directory.Exists(vplogPath))
            {
                throw new DirectoryNotFoundException(string.Format("Каталог {0} не существует", vplogPath));
            }
            _vplogPath = vplogPath;
        }

        public IEnumerable<MonitoringDevice> LoadMonitoringDevices()
        {
            var files = Directory.GetFiles(_vplogPath);
            var monitoringDevices = new List<MonitoringDevice>();
            foreach (var file in files)
            {
                var fileParts = Path.GetFileNameWithoutExtension(file).Split('-');
                if (fileParts.Length < 2) continue;
                var computerName = fileParts[1];
                var fileInfo = new FileInfo(file);
                monitoringDevices.Add(new MonitoringDevice
                {
                    DeviceName = computerName,
                    Properties = LoadProperties(file),
                    UpdateDate = fileInfo.LastWriteTime
                });
            }
            var resultMonitoringDevices = new List<MonitoringDevice>();
            foreach (var monitoringDevice in monitoringDevices)
            {
                var resultMonitoringDevice =
                    resultMonitoringDevices.FirstOrDefault(r => r.DeviceName == monitoringDevice.DeviceName);
                if (resultMonitoringDevice != null)
                {
                    if (resultMonitoringDevice.UpdateDate >= monitoringDevice.UpdateDate) continue;
                    resultMonitoringDevices.Remove(resultMonitoringDevice);
                    resultMonitoringDevices.Add(monitoringDevice);
                }
                else
                {
                    resultMonitoringDevices.Add(monitoringDevice);
                }
            }
            return resultMonitoringDevices;
        }

        private static IEnumerable<MonitoringProperty> LoadProperties(string fileName)
        {
            var properties = new List<MonitoringProperty>();
            using (var reader = new StreamReader(fileName, Encoding.GetEncoding(866)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    var lineParts = line.Split(new[] {':'}, 2);
                    if (lineParts.Length == 2)
                    {
                        // Если произошел сбой при получении какого-либо свойства, то не обновлять статистику по устройству
                        var value = lineParts[1].Trim();
                        if (value == "~0,-3" || value == "~0")
                        {
                            continue;
                        }
                        var property = new MonitoringProperty
                        {
                            Name = lineParts[0],
                            Value = value
                        };
                        if (lineParts[0] == "SysDiskFree")
                        {
                            long valueParse;
                            if (long.TryParse(value, out valueParse))
                            {
                                property.Value = Math.Round((double) valueParse/(1024*1024)).ToString(CultureInfo.InvariantCulture);
                            }
                        }
                        properties.Add(property);
                    }
                    else
                    {
                        var value = lineParts[0].Trim();
                        if (value == "~0,-3" || value == "~0")
                        {
                            continue;
                        }
                        properties.Add(new MonitoringProperty
                        {
                            Value = value
                        });
                    }
                }
            }
            return properties;
        }
    }
}
