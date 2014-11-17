using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Devices
{
	public partial class SearchFormSt2 : Form
	{
		private DevicesDatabase db { get; set; }
		public int DeviceTypeID { get; set; }
		public List<SearchParameter> paramList { get; set; }

		public SearchFormSt2()
		{
			InitializeComponent();
		}

		private void SearchFormSt2_Load(object sender, EventArgs e)
		{
			List<Node> list;
			db = new DevicesDatabase();
			list = db.GetDeviceInfoMetaByType(DeviceTypeID);
			foreach (Node DeviceInfoMeta in list)
			{
				TreeNode node = new TreeNode();
				node.Text = DeviceInfoMeta.NodeName;
				node.Tag = new NodeProperty(DeviceInfoMeta.NodeID, NodeTypeEnum.DeviceComplexParameter);
				TreeNodesHelper.AddNode(node, treeViewDeviceInfo.Nodes,
					treeViewDeviceInfo.Nodes, DeviceInfoMeta.ParentNodeID);
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
			List<DeviceParametersComboboxItem> list = db.GetDeviceParameters(((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeID);
			comboBoxDeviceParameters.Items.Clear();
			foreach (DeviceParametersComboboxItem item in list)
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
					comboBoxOperator.Items.AddRange(new string[] { "=", "<>", ">", ">=", "<", "<=" });
					break;
				case "float": 
					numericUpDownValue.DecimalPlaces = 2;
					numericUpDownValue.Visible = true;
					textBoxValue.Visible = false;
					comboBoxValue.Visible = false;
					buttonAccept.Enabled = true;
					comboBoxOperator.Items.Clear();
					comboBoxOperator.Items.AddRange(new string[] { "=", "<>", ">", ">=", "<", "<=" });
					break;
				case "combobox":
					comboBoxValue.DataSource = db.GetValuesByMetaNodeID(
						((DeviceParametersComboboxItem)comboBoxDeviceParameters.SelectedItem).MetaNodeID);
					comboBoxValue.DisplayMember = "Value";
					numericUpDownValue.Visible = false;
					textBoxValue.Visible = false;
					comboBoxValue.Visible = true;
					buttonAccept.Enabled = true;
					comboBoxOperator.Items.Clear();
					comboBoxOperator.Items.AddRange(new string[] { "=", "<>" });
					break;
				default:
					numericUpDownValue.Visible = false;
					textBoxValue.Visible = true;
					comboBoxValue.Visible = false;
					textBoxValue_TextChanged(sender, e);
					comboBoxOperator.Items.Clear();
					comboBoxOperator.Items.AddRange(new string[] { "=", "<>", "Содержит", "Не содержит", "Начинается с", "Начинается не с" });
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

		private string ConvertOperation(string Operation)
		{
			switch (Operation)
			{
				case "Содержит": 
				case "Начинается с": return "LIKE";
				case "Не содержит": 
				case "Начинается не с": return "NOT LIKE";
				default: return Operation;
			}
		}

		private string ConvertValue(string Value, string Operation, string Type)
		{
			switch (Operation)
			{
				case "Начинается не с":
				case "Начинается с": return "\'" + Value + "%\'";
				case "Содержит":
				case "Не содержит": return "\'%" + Value + "%\'"; ;
				default: 
					if ((Type == "int") || (Type == "float"))
						return Value;
					else 
						return "\'" + Value + "\'"; ;
			}
		}

		private void buttonAccept_Click(object sender, EventArgs e)
		{
			int ParameterID = ((DeviceParametersComboboxItem)comboBoxDeviceParameters.SelectedItem).MetaNodeID;
			string Operation = ConvertOperation(comboBoxOperator.Text);
			string ParameterName = ((DeviceParametersComboboxItem)comboBoxDeviceParameters.SelectedItem).ParameterName;
			string DeviceName = treeViewDeviceInfo.SelectedNode.Text;
			string Value = "";
			switch (((DeviceParametersComboboxItem)comboBoxDeviceParameters.SelectedItem).ParameterType.ToLower())
			{
				case "int":
					Value = numericUpDownValue.Value.ToString();
					break;
				case "float": 
					Value = numericUpDownValue.Value.ToString();
					break;
				case "combobox":
					Value = comboBoxValue.Text;
					break;
				default: 
					Value = textBoxValue.Text;
					break;
			}
			Value = ConvertValue(Value, comboBoxOperator.Text,
				((DeviceParametersComboboxItem)comboBoxDeviceParameters.SelectedItem).ParameterType.ToLower());
			paramList.Add(new SearchParameter(ParameterID, DeviceName, ParameterName, Operation, Value));
			Close();
		}
	}

	public class DeviceParametersComboboxItem
	{
		public int MetaNodeID { get; set; }
		public string ParameterName { get; set; }
		public string ParameterType { get; set; }

		public DeviceParametersComboboxItem(int MetaNodeID, string ParameterName, string ParameterType)
		{
			this.MetaNodeID = MetaNodeID;
			this.ParameterName = ParameterName;
			this.ParameterType = ParameterType;
		}
	}

	public class SearchParameter
	{
		public int ParameterID { get; set; }
		public string DeviceName { get; set; }
		public string ParameterName { get; set; }
		public string Operation { get; set; }
		public string ParameterValue { get; set; }

		public SearchParameter()
		{
		}

		public SearchParameter(int ParameterID, string DeviceName, string ParameterName, string Operation, string ParameterValue)
		{
			this.ParameterID = ParameterID;
			this.Operation = Operation;
			this.ParameterValue = ParameterValue;
			this.ParameterName = ParameterName;
			this.DeviceName = DeviceName;
		}
	}
}
