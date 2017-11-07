using System;
using System.Collections.Generic;

namespace MonitoringUpdater
{
    internal class MonitoringDevice
    {
        public string DeviceName { get; set; }
        public DateTime UpdateDate { get; set; }
        public IEnumerable<MonitoringProperty> Properties { get; set; }
    }
}
