namespace Devices
{
    public class SearchNodeParameter
    {
        public int ParameterId { get; set; }
        public string DeviceName { get; set; }
        public string ParameterName { get; set; }
        public string Operation { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterType { get; set; }

        public SearchNodeParameter()
        {
        }

        public SearchNodeParameter(int parameterId, string deviceName, string parameterName, string parameterType, string operation, string parameterValue)
        {
            ParameterId = parameterId;
            Operation = operation;
            ParameterValue = parameterValue;
            ParameterName = parameterName;
            DeviceName = deviceName;
            ParameterType = parameterType;
        }
    }
}
