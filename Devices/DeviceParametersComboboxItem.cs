namespace Devices
{
    public class DeviceParametersComboboxItem
    {
        public int MetaNodeId { get; set; }
        public string ParameterName { get; set; }
        public string ParameterType { get; set; }

        public DeviceParametersComboboxItem(int metaNodeId, string parameterName, string parameterType)
        {
            MetaNodeId = metaNodeId;
            ParameterName = parameterName;
            ParameterType = parameterType;
        }
    }
}