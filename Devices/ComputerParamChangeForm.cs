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
	public partial class ComputerParamChangeForm : Form
	{
		public string ParamType { get; set; }
		public int AssocMetaNodeID { get; set; }
		public int NodeRealID { get; set; }
		public object Value { get; set; }
		public string ParamName { get; set; }
		public bool isChanged { get; set; }
		public int ParentNodeID { get; set; }
		public int DeviceID { get; set; }

		public ComputerParamChangeForm()
		{
			InitializeComponent();
			isChanged = false;
		}

		public void InitForm()
		{
			label1.Text = ParamName;
			switch (ParamType)
			{
				case "text":
					textBoxParam.Visible = true;
					if (NodeRealID != -1)
						textBoxParam.Text = (string)Value;
					break;
				case "combobox": 
					comboBoxParam.Visible = true;
					DevicesDatabase db = new DevicesDatabase();
					comboBoxParam.DataSource = db.GetValuesByMetaNodeID(AssocMetaNodeID);
					comboBoxParam.DisplayMember = "Value";
					if (NodeRealID != -1)
						comboBoxParam.SelectedIndex = comboBoxParam.FindString(Value.ToString());
					break;
				case "int":
					numericUpDownParam.Visible = true;
					if (NodeRealID != -1)
						numericUpDownParam.Value = Convert.ToInt32(Value);
					break;
				case "float":
					numericUpDownParam.Visible = true;
					numericUpDownParam.DecimalPlaces = 2;
					if (NodeRealID != -1)
						numericUpDownParam.Value = Convert.ToDecimal(Value);
					break;
				default:
					textBoxParam.Visible = true;
					if (NodeRealID != -1)
						textBoxParam.Text = (string)Value;
					break;
			}
		}

		private void buttonQuit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			DevicesDatabase db = new DevicesDatabase();
			switch (ParamType)
			{
				case "text":
					Value = textBoxParam.Text;
					break;
				case "combobox":
					Value = comboBoxParam.Text;
					break;
				case "int":
					Value = numericUpDownParam.Value;
					break;
				case "float":
					Value = numericUpDownParam.Value;
					break;
				default: ;
					break;
			}
			if (NodeRealID == -1)
				db.InsertDeviceNodeValue(AssocMetaNodeID, ParentNodeID, DeviceID, Value.ToString());
			else
				db.UpdateDeviceNodeValue(NodeRealID, Value.ToString());
			db.Dispose();
			isChanged = true;
			Close();
		}

		private void numericUpDownParam_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				buttonSave_Click(buttonSave, new EventArgs());
			else
				if (e.KeyCode == Keys.Escape)
					Close();
		}

	}
}
