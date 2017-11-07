using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Devices
{
	public partial class SearchFormSt3 : Form
	{
		private DevicesDatabase Db { get; set; }
		public int DeviceTypeId { get; set; }
        public List<SearchMonitoringParameter> ParamList { get; set; }

        public SearchFormSt3()
		{
			InitializeComponent();
		}

		private void SearchFormSt3_Load(object sender, EventArgs e)
		{
		    Db = new DevicesDatabase();
            var list = Db.GetMonitoringExistsProperties();
		    comboBoxDeviceParameters.DataSource = list;
			comboBoxDeviceParameters.DisplayMember = "DisplayName";
            comboBoxDeviceParameters.ValueMember = "Name";
            if (comboBoxDeviceParameters.Items.Count > 0)
                comboBoxDeviceParameters.SelectedIndex = 0;
            else
                buttonAccept.Enabled = false;
            comboBoxOperator.SelectedIndex = 0;
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void textBoxValue_TextChanged(object sender, EventArgs e)
		{
			if ((comboBoxDeviceParameters.SelectedIndex < 0) || (textBoxValue.Text.Length <= 0))
				buttonAccept.Enabled = false;
			else
				buttonAccept.Enabled = true;
		}

		private void buttonAccept_Click(object sender, EventArgs e)
		{
            var operation = comboBoxOperator.Text;
            var parameterName = comboBoxDeviceParameters.SelectedValue.ToString();
            var displayName = ((MonitoringProperty)comboBoxDeviceParameters.SelectedItem).DisplayName;
            var value = textBoxValue.Text; 
            ParamList.Add(new SearchMonitoringParameter(parameterName, displayName, operation, value));
			Close();
		}
	}
}
