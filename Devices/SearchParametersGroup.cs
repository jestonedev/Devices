using System.Collections.Generic;

namespace Devices
{
    public class SearchParametersGroup
    {
        public List<SearchNodeParameter> NodeParameters { get; set; }
        public List<SearchMonitoringParameter> MonitoringParameters { get; set; }
        public List<int> DepartmentIDs { get; set; }
        public int DeviceTypeId { get; set; }
        public string DeviceName { get; set; }
        public string SerialNumber { get; set; }
        public string InventoryNumber { get; set; }

        public SearchParametersGroup()
        {
            NodeParameters = new List<SearchNodeParameter>();
            MonitoringParameters = new List<SearchMonitoringParameter>();
            DepartmentIDs = new List<int>();
            DeviceName = "";
            SerialNumber = "";
            InventoryNumber = "";
        }
    }
}