using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace Devices
{
	public partial class SearchFormSt2 : Form
	{
		private DevicesDatabase Db { get; set; }
		public int DeviceTypeId { get; set; }
		public List<SearchParameter> ParamList { get; set; }

		public SearchFormSt2()
		{
			InitializeComponent();
		}

		private void SearchFormSt2_Load(object sender, EventArgs e)
		{
		    Db = new DevicesDatabase();
			var list = Db.GetDeviceInfoMetaByType(DeviceTypeId);
			foreach (var deviceInfoMeta in list)
			{
			    var node = new TreeNode
			    {
			        Text = deviceInfoMeta.NodeName,
			        Tag = new NodeProperty(deviceInfoMeta.NodeId, NodeTypeEnum.DeviceComplexParameter)
			    };
			    TreeNodesHelper.AddNode(node, treeViewDeviceInfo.Nodes,
					treeViewDeviceInfo.Nodes, deviceInfoMeta.ParentNodeId);
			}
			treeViewDeviceInfo.ExpandAll();
			comboBoxDeviceParameters.DisplayMember = "ParameterName";
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void treeViewDeviceInfo_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var list = Db.GetDeviceParameters(((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeID);
			comboBoxDeviceParameters.Items.Clear();
			foreach (var item in list)
			{
				comboBoxDeviceParameters.Items.Add(item);
			}
			if (comboBoxDeviceParameters.Items.Count > 0)
				comboBoxDeviceParameters.SelectedIndex = 0;
			else
				buttonAccept.Enabled = false;
		}

		private void comboBoxDeviceParameters_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (((DeviceParametersComboboxItem)comboBoxDeviceParameters.SelectedItem).ParameterType.ToLower())
			{
				case "int":
					numericUpDownValue.DecimalPlaces = 0;
					numericUpDownValue.Visible = true;
					textBoxValue.Visible = false;
					comboBoxValue.Visible = false;
					buttonAccept.Enabled = true;
					comboBoxOperator.Items.Clear();
					comboBoxOperator.Items.AddRange(new object[] { "=", "<>", ">", ">=", "<", "<=" });
					break;
				case "float": 
					numericUpDownValue.DecimalPlaces = 2;
					numericUpDownValue.Visible = true;
					textBoxValue.Visible = false;
					comboBoxValue.Visible = false;
					buttonAccept.Enabled = true;
					comboBoxOperator.Items.Clear();
					comboBoxOperator.Items.AddRange(new object[] { "=", "<>", ">", ">=", "<", "<=" });
					break;
				case "combobox":
					comboBoxValue.DataSource = Db.GetValuesByMetaNodeId(
						((DeviceParametersComboboxItem)comboBoxDeviceParameters.SelectedItem).MetaNodeId);
					comboBoxValue.DisplayMember = "Value";
					numericUpDownValue.Visible = false;
					textBoxValue.Visible = false;
					comboBoxValue.Visible = true;
					buttonAccept.Enabled = true;
					comboBoxOperator.Items.Clear();
					comboBoxOperator.Items.AddRange(new object[] { "=", "<>" });
					break;
				default:
					numericUpDownValue.Visible = false;
					textBoxValue.Visible = true;
					comboBoxValue.Visible = false;
					textBoxValue_TextChanged(sender, e);
					comboBoxOperator.Items.Clear();
					comboBoxOperator.Items.AddRange(new object[] { "=", "<>", "Содержит", "Не содержит", "Начинается с", "Начинается не с" });
					break;
			}
			comboBoxOperator.SelectedIndex = 0;
		}

		private void textBoxValue_TextChanged(object sender, EventArgs e)
		{
			if ((comboBoxDeviceParameters.SelectedIndex < 0) || (textBoxValue.Text.Length <= 0))
				buttonAccept.Enabled = false;
			else
				buttonAccept.Enabled = true;
		}

		private string ConvertOperation(string operation)
		{
			switch (operation)
			{
				case "Содержит": 
				case "Начинается с": return "LIKE";
				case "Не содержит": 
				case "Начинается не с": return "NOT LIKE";
				default: return operation;
			}
		}

		private static string ConvertValue(string value, string operation, string type)
		{
			switch (operation)
			{
				case "Начинается не с":
				case "Начинается с": return "\'" + value + "%\'";
				case "Содержит":
				case "Не содержит": return "\'%" + value + "%\'";
			    default: 
					if ((type == "int") || (type == "float"))
						return value;
			        return "\'" + value + "\'";
			}
		}

		private void buttonAccept_Click(object sender, EventArgs e)
		{
			var parameterId = ((DeviceParametersComboboxItem)comboBoxDeviceParameters.SelectedItem).MetaNodeId;
			var operation = ConvertOperation(comboBoxOperator.Text);
			var parameterName = ((DeviceParametersComboboxItem)comboBoxDeviceParameters.SelectedItem).ParameterName;
			var parameterType = ((DeviceParametersComboboxItem)comboBoxDeviceParameters.SelectedItem).ParameterType.ToLower();
			var deviceName = treeViewDeviceInfo.SelectedNode.Text;
			string value;
            switch (parameterType)
			{
				case "int":
					value = numericUpDownValue.Value.ToString(CultureInfo.InvariantCulture);
					break;
				case "float": 
					value = numericUpDownValue.Value.ToString(CultureInfo.InvariantCulture);
					break;
				case "combobox":
					value = comboBoxValue.Text;
					break;
				default: 
					value = textBoxValue.Text;
					break;
			}
			value = ConvertValue(value, comboBoxOperator.Text,
				((DeviceParametersComboboxItem)comboBoxDeviceParameters.SelectedItem).ParameterType.ToLower());
            ParamList.Add(new SearchParameter(parameterId, deviceName, parameterName, parameterType, operation, value));
			Close();
		}
	}

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

	public class SearchParameter
	{
		public int ParameterId { get; set; }
		public string DeviceName { get; set; }
		public string ParameterName { get; set; }
		public string Operation { get; set; }
		public string ParameterValue { get; set; }
        public string ParameterType { get; set; }

		public SearchParameter()
		{
		}

        public SearchParameter(int parameterId, string deviceName, string parameterName, string parameterType, string operation, string parameterValue)
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
