using System;
using System.Windows.Forms;

namespace Devices
{
	public partial class ComputerParamChangeForm : Form
	{
		public string ParamType { get; set; }
		public int AssocMetaNodeId { get; set; }
		public int NodeRealId { get; set; }
		public object Value { get; set; }
		public string ParamName { get; set; }
		public bool IsChanged { get; set; }
		public int ParentNodeId { get; set; }
		public int DeviceId { get; set; }

		public ComputerParamChangeForm()
		{
			InitializeComponent();
			IsChanged = false;
		}

		public void InitForm()
		{
			label1.Text = ParamName;
			switch (ParamType)
			{
				case "text":
					textBoxParam.Visible = true;
					if (NodeRealId != -1)
						textBoxParam.Text = (string)Value;
					break;
				case "combobox": 
					comboBoxParam.Visible = true;
					var db = new DevicesDatabase();
					comboBoxParam.DataSource = db.GetValuesByMetaNodeId(AssocMetaNodeId);
					comboBoxParam.DisplayMember = "Value";
					if (NodeRealId != -1)
						comboBoxParam.SelectedIndex = comboBoxParam.FindString(Value.ToString());
					break;
				case "int":
					numericUpDownParam.Visible = true;
					if (NodeRealId != -1)
						numericUpDownParam.Value = Convert.ToInt32(Value);
					break;
				case "float":
					numericUpDownParam.Visible = true;
					numericUpDownParam.DecimalPlaces = 2;
					if (NodeRealId != -1)
						numericUpDownParam.Value = Convert.ToDecimal(Value);
					break;
				default:
					textBoxParam.Visible = true;
					if (NodeRealId != -1)
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
			var db = new DevicesDatabase();
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
			}
			if (NodeRealId == -1)
				db.InsertDeviceNodeValue(AssocMetaNodeId, ParentNodeId, DeviceId, Value.ToString());
			else
				db.UpdateDeviceNodeValue(NodeRealId, Value.ToString());
			db.Dispose();
			IsChanged = true;
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
