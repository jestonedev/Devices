namespace Devices
{
    public class SearchMonitoringParameter
    {
        public string ParameterName { get; set; }
        public string DisplayName { get; set; }
        public string Operation { get; set; }
        public string ParameterValue { get; set; }

        public SearchMonitoringParameter()
        {
        }

        public SearchMonitoringParameter(string parameterName, string displayName, string operation, string parameterValue)
        {
            Operation = operation;
            ParameterValue = parameterValue;
            ParameterName = parameterName;
            DisplayName = displayName;
        }
    }
}
