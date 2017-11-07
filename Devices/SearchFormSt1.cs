using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Devices
{
	public partial class SearchFormSt1 : Form
	{
		private DevicesDatabase Db { get; set; }
		public SearchParametersGroup Spg { get; set; }
		public bool CancelSearch { get; set; }
		private SearchParametersGroup SpgNew { get; set; }
		public SearchFormSt1()
		{
			InitializeComponent();
			CancelSearch = true;
            foreach (Control control in splitContainer1.Panel1.Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        buttonSearch_Click(sender, e);
                    else
                        if (e.KeyCode == Keys.Escape)
                            buttonClose_Click(sender, e);
                };
            foreach (Control control in splitContainer1.Panel2.Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        buttonSearch_Click(sender, e);
                    else
                        if (e.KeyCode == Keys.Escape)
                            buttonClose_Click(sender, e);
                };
		}

		private void InitDataGridView()
		{
		    var bs = new BindingSource {DataSource = SpgNew.NodeParameters.Distinct()};
		    dataGridViewNodeProperties.DataSource = bs;
		    if (bs.Count > 0)
		    {
		        dataGridViewNodeProperties.Columns[0].Visible = false;
		        dataGridViewNodeProperties.Columns[1].HeaderText = @"Устройство";
		        dataGridViewNodeProperties.Columns[1].Width = 150;
		        dataGridViewNodeProperties.Columns[2].HeaderText = @"Параметр";
		        dataGridViewNodeProperties.Columns[2].Width = 150;
		        dataGridViewNodeProperties.Columns[3].HeaderText = @"Операция";
		        dataGridViewNodeProperties.Columns[4].HeaderText = @"Значение";
		        dataGridViewNodeProperties.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		        dataGridViewNodeProperties.Columns[5].HeaderText = @"Тип параметра";
		    }

		    bs = new BindingSource { DataSource = SpgNew.MonitoringParameters.Distinct()};
		    dataGridViewMonitoringProperties.DataSource = bs;
		    if (bs.Count > 0)
		    {
		        dataGridViewMonitoringProperties.Columns[0].Visible = false;
		        dataGridViewMonitoringProperties.Columns[1].HeaderText = @"Параметр";
		        dataGridViewMonitoringProperties.Columns[1].Width = 150;
		        dataGridViewMonitoringProperties.Columns[2].HeaderText = @"Операция";
		        dataGridViewMonitoringProperties.Columns[3].HeaderText = @"Значение";
		        dataGridViewMonitoringProperties.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		    }
		}

		private void SearchFormSt1_Load(object sender, EventArgs e)
		{
            CancelSearch = true;
			//Загрузка департаментов в дерево
			treeViewDepartments.Nodes.Clear();
			Db = new DevicesDatabase();
			SpgNew = new SearchParametersGroup();
			var list = Db.GetDepartments(SpgNew);
			foreach (var department in list)
			{
			    var node = new TreeNode
			    {
			        Text = department.NodeName,
			        Tag = new NodeProperty(department.NodeId, NodeTypeEnum.DepartmentNode)
			    };
			    TreeNodesHelper.AddNode(node, treeViewDepartments.Nodes, treeViewDepartments.Nodes, department.ParentNodeId);
			}

			//Загрузка типов узлов в combobox
			var view = Db.GetDeviceTypes()	;
			comboBoxDevTypes.DisplayMember = "DeviceType";
			for (var i = 0; i < view.Table.Rows.Count; i++)
			{
				comboBoxDevTypes.Items.Add(new DeviceTypeComboboxItem(view.Table.Rows[i]["Type"].ToString(),
					Convert.ToInt32(view.Table.Rows[i]["ID Device Type"])));
			}
		}

		private void treeViewDepartments_AfterCheck(object sender, TreeViewEventArgs e)
		{
			foreach (TreeNode child in e.Node.Nodes)
			{
				child.Checked = e.Node.Checked;
			}
		}

		private void comboBoxDevTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			SpgNew = new SearchParametersGroup();
			InitDataGridView();
		}

		private void buttonAddNodeProperty_Click(object sender, EventArgs e)
		{
		    var sfs2 = new SearchFormSt2
		    {
		        DeviceTypeId = ((DeviceTypeComboboxItem) comboBoxDevTypes.SelectedItem).DeviceTypeId,
		        ParamList = SpgNew.NodeParameters
		    };
		    sfs2.ShowDialog();
			InitDataGridView();
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void FillDepartmentIDs(List<int> departmentsIDs, TreeNodeCollection currentNode)
		{
			foreach (TreeNode node in currentNode)
			{
				if (node.Checked)
					departmentsIDs.Add(((NodeProperty)node.Tag).NodeId);
				FillDepartmentIDs(departmentsIDs, node.Nodes);
			}
		}

		private void buttonSearch_Click(object sender, EventArgs e)
		{
			SpgNew.DeviceTypeId = ((DeviceTypeComboboxItem)comboBoxDevTypes.SelectedItem).DeviceTypeId;
			SpgNew.DeviceName = textBoxDeviceName.Text.Trim();
            SpgNew.SerialNumber = textBoxSerialNumber.Text.Trim();
            SpgNew.InventoryNumber = textBoxInventoryNumber.Text.Trim();
			FillDepartmentIDs(SpgNew.DepartmentIDs, treeViewDepartments.Nodes);
			Spg.DepartmentIDs = SpgNew.DepartmentIDs;
			Spg.DeviceTypeId = SpgNew.DeviceTypeId;
			Spg.NodeParameters = SpgNew.NodeParameters;
			Spg.MonitoringParameters = SpgNew.MonitoringParameters;
			Spg.DeviceName = SpgNew.DeviceName;
            Spg.SerialNumber = SpgNew.SerialNumber;
            Spg.InventoryNumber = SpgNew.InventoryNumber;
			CancelSearch = false;
			Close();
		}

		private void buttonRemoveNodeProperty_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(@"Вы действительно хотите удалить этот параметр?", @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
				== DialogResult.No)
				return;
			foreach(DataGridViewRow dgvr in dataGridViewNodeProperties.SelectedRows)
			{
				for (var i = 0; i < SpgNew.NodeParameters.Count; i++)
				{
					var sp = SpgNew.NodeParameters[i];
				    if ((sp.ParameterId.ToString() != dgvr.Cells[0].Value.ToString()) ||
				        (sp.ParameterName != dgvr.Cells[2].Value.ToString()) || 
                        (sp.Operation != dgvr.Cells[3].Value.ToString()) ||
				        (sp.ParameterValue != dgvr.Cells[4].Value.ToString()) ||
                        (sp.ParameterType != dgvr.Cells[5].Value.ToString())) continue;
				    SpgNew.NodeParameters.RemoveAt(i);
				    InitDataGridView();
				    return;
				}
			}
		}

		private void dataGridViewNodeProperties_SelectionChanged(object sender, EventArgs e)
		{
			buttonRemoveNodeProperty.Enabled = dataGridViewNodeProperties.SelectedRows.Count > 0;
		}

        private void dataGridViewNodeProperties_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				buttonRemoveNodeProperty_Click(sender, new EventArgs());
		}

        private void dataGridViewMonitoringProperties_SelectionChanged(object sender, EventArgs e)
        {
            buttonRemoveMonitoringProperty.Enabled = dataGridViewMonitoringProperties.SelectedRows.Count > 0;
        }

        private void buttonAddMonitoringProperty_Click(object sender, EventArgs e)
        {
            var sfs3 = new SearchFormSt3
            {
                ParamList = SpgNew.MonitoringParameters
            };
            sfs3.ShowDialog();
            InitDataGridView();
        }

        private void buttonRemoveMonitoringProperty_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить этот параметр?", @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.No)
                return;
            foreach (DataGridViewRow dgvr in dataGridViewMonitoringProperties.SelectedRows)
            {
                for (var i = 0; i < SpgNew.MonitoringParameters.Count; i++)
                {
                    var sp = SpgNew.MonitoringParameters[i];
                    if ((sp.ParameterName != dgvr.Cells[0].Value.ToString()) ||
                        (sp.Operation != dgvr.Cells[2].Value.ToString()) ||
                        (sp.ParameterValue != dgvr.Cells[3].Value.ToString())) continue;
                    SpgNew.MonitoringParameters.RemoveAt(i);
                    InitDataGridView();
                    return;
                }
            }
        }

        private void SearchFormSt1_Shown(object sender, EventArgs e)
        {
            comboBoxDevTypes.SelectedIndex = 0;
            InitDataGridView();
        }
	}
}
