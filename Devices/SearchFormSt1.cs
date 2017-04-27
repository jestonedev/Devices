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
	public partial class SearchFormSt1 : Form
	{
		private DevicesDatabase db { get; set; }
		public SearchParametersGroup spg { get; set; }
		public bool CancelSearch { get; set; }
		private SearchParametersGroup spgNew { get; set; }
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
                        button1_Click(sender, e);
                    else
                        if (e.KeyCode == Keys.Escape)
                            button2_Click(sender, e);
                };
            foreach (Control control in splitContainer1.Panel2.Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        button1_Click(sender, e);
                    else
                        if (e.KeyCode == Keys.Escape)
                            button2_Click(sender, e);
                };
		}

		private void InitDataGridView(object DataSource)
		{
		    var bs = new BindingSource {DataSource = spgNew.parameters};
		    dataGridView1.DataSource = bs;
			dataGridView1.Columns[0].Visible = false;
			dataGridView1.Columns[1].HeaderText = @"Устройство";
			dataGridView1.Columns[1].Width = 150;
			dataGridView1.Columns[2].HeaderText = @"Параметр";
			dataGridView1.Columns[2].Width = 150;
			dataGridView1.Columns[3].HeaderText = @"Операция";
            dataGridView1.Columns[4].HeaderText = @"Значение";
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[5].HeaderText = @"Тип параметра";
		}

		private void SearchFormSt1_Load(object sender, EventArgs e)
		{
            CancelSearch = true;
			//Загрузка департаментов в дерево
			treeViewDepartments.Nodes.Clear();
			db = new DevicesDatabase();
			spgNew = new SearchParametersGroup();
			var list = db.GetDepartments(spgNew);
			foreach (var department in list)
			{
				var node = new TreeNode();
				node.Text = department.NodeName;
				node.Tag = new NodeProperty(department.NodeID, NodeTypeEnum.DepartmentNode);
				TreeNodesHelper.AddNode(node, treeViewDepartments.Nodes, treeViewDepartments.Nodes, department.ParentNodeID);
			}

			//Загрузка типов узлов в combobox
			var view = db.GetDeviceTypes()	;
			comboBoxDevTypes.DisplayMember = "DeviceType";
			for (var i = 0; i < view.Table.Rows.Count; i++)
			{
				comboBoxDevTypes.Items.Add(new DeviceTypeComboboxItem(view.Table.Rows[i]["Type"].ToString(),
					Convert.ToInt32(view.Table.Rows[i]["ID Device Type"])));
			}
			comboBoxDevTypes.SelectedIndex = 0;
			InitDataGridView(spgNew.parameters);
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
			spgNew = new SearchParametersGroup();
			InitDataGridView(spgNew.parameters);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			var sfs2 = new SearchFormSt2();
			sfs2.DeviceTypeId = ((DeviceTypeComboboxItem)comboBoxDevTypes.SelectedItem).DeviceTypeID;
			sfs2.ParamList = spgNew.parameters;
			sfs2.ShowDialog();
			InitDataGridView(spgNew.parameters);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void FillDepartmentIDs(List<int> departmentsIDs, TreeNodeCollection currentNode)
		{
			foreach (TreeNode node in currentNode)
			{
				if (node.Checked)
					departmentsIDs.Add(((NodeProperty)node.Tag).NodeID);
				FillDepartmentIDs(departmentsIDs, node.Nodes);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			spgNew.deviceTypeID = ((DeviceTypeComboboxItem)comboBoxDevTypes.SelectedItem).DeviceTypeID;
			spgNew.deviceName = textBoxDeviceName.Text.Trim();
            spgNew.serialNumber = textBoxSerialNumber.Text.Trim();
            spgNew.inventoryNumber = textBoxInventoryNumber.Text.Trim();
			FillDepartmentIDs(spgNew.departmentIDs, treeViewDepartments.Nodes);
			spg.departmentIDs = spgNew.departmentIDs;
			spg.deviceTypeID = spgNew.deviceTypeID;
			spg.parameters = spgNew.parameters;
			spg.deviceName = spgNew.deviceName;
            spg.serialNumber = spgNew.serialNumber;
            spg.inventoryNumber = spgNew.inventoryNumber;
			CancelSearch = false;
			Close();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(@"Вы действительно хотите удалить этот параметр?", @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
				== DialogResult.No)
				return;
			foreach(DataGridViewRow dgvr in dataGridView1.SelectedRows)
			{
				for (var i = 0; i < spgNew.parameters.Count; i++)
				{
					var sp = spgNew.parameters[i];
				    if ((sp.ParameterId.ToString() != dgvr.Cells[0].Value.ToString()) ||
				        (sp.ParameterName != dgvr.Cells[2].Value.ToString()) || 
                        (sp.Operation != dgvr.Cells[3].Value.ToString()) ||
				        (sp.ParameterValue != dgvr.Cells[4].Value.ToString()) ||
                        (sp.ParameterType != dgvr.Cells[5].Value.ToString())) continue;
				    spgNew.parameters.RemoveAt(i);
				    InitDataGridView(spgNew.parameters);
				    return;
				}
			}
		}

		private void dataGridView1_SelectionChanged(object sender, EventArgs e)
		{
			button4.Enabled = (dataGridView1.SelectedRows.Count > 0);

		}

		private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				button4_Click(sender, new EventArgs());
		}
	}

	public class SearchParametersGroup
	{
		public List<SearchParameter> parameters { get; set; }
		public List<int> departmentIDs { get; set; }
		public int deviceTypeID { get; set; }
        public string deviceName { get; set; }
        public string serialNumber { get; set; }
        public string inventoryNumber { get; set; }

		public SearchParametersGroup()
		{
			parameters = new List<SearchParameter>();
			departmentIDs = new List<int>();
			deviceName = "";
            serialNumber = "";
            inventoryNumber = "";
		}
	}
}
