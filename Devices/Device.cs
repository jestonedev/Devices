namespace Devices
{
    internal class Device
    {
        public string Department { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string InventoryNumber { get; set; }

        public Device()
        {
        }

        public Device(string department, int deviceId, string name, string serialNumber, string inventoryNumber)
        {
            Department = department;
            DeviceId = deviceId;
            Name = name;
            SerialNumber = serialNumber;
            InventoryNumber = inventoryNumber;
        }
    }
}
