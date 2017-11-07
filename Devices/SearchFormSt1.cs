using System;
using System.Collections.Generic;
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

		private void InitDataGridView()
		{
		    var bs = new BindingSource {DataSource = SpgNew.Parameters};
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
			comboBoxDevTypes.SelectedIndex = 0;
			InitDataGridView();
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

		private void button3_Click(object sender, EventArgs e)
		{
		    var sfs2 = new SearchFormSt2
		    {
		        DeviceTypeId = ((DeviceTypeComboboxItem) comboBoxDevTypes.SelectedItem).DeviceTypeId,
		        ParamList = SpgNew.Parameters
		    };
		    sfs2.ShowDialog();
			InitDataGridView();
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
					departmentsIDs.Add(((NodeProperty)node.Tag).NodeId);
				FillDepartmentIDs(departmentsIDs, node.Nodes);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			SpgNew.DeviceTypeId = ((DeviceTypeComboboxItem)comboBoxDevTypes.SelectedItem).DeviceTypeId;
			SpgNew.DeviceName = textBoxDeviceName.Text.Trim();
            SpgNew.SerialNumber = textBoxSerialNumber.Text.Trim();
            SpgNew.InventoryNumber = textBoxInventoryNumber.Text.Trim();
			FillDepartmentIDs(SpgNew.DepartmentIDs, treeViewDepartments.Nodes);
			Spg.DepartmentIDs = SpgNew.DepartmentIDs;
			Spg.DeviceTypeId = SpgNew.DeviceTypeId;
			Spg.Parameters = SpgNew.Parameters;
			Spg.DeviceName = SpgNew.DeviceName;
            Spg.SerialNumber = SpgNew.SerialNumber;
            Spg.InventoryNumber = SpgNew.InventoryNumber;
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
				for (var i = 0; i < SpgNew.Parameters.Count; i++)
				{
					var sp = SpgNew.Parameters[i];
				    if ((sp.ParameterId.ToString() != dgvr.Cells[0].Value.ToString()) ||
				        (sp.ParameterName != dgvr.Cells[2].Value.ToString()) || 
                        (sp.Operation != dgvr.Cells[3].Value.ToString()) ||
				        (sp.ParameterValue != dgvr.Cells[4].Value.ToString()) ||
                        (sp.ParameterType != dgvr.Cells[5].Value.ToString())) continue;
				    SpgNew.Parameters.RemoveAt(i);
				    InitDataGridView();
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
		public List<SearchParameter> Parameters { get; set; }
		public List<int> DepartmentIDs { get; set; }
		public int DeviceTypeId { get; set; }
        public string DeviceName { get; set; }
        public string SerialNumber { get; set; }
        public string InventoryNumber { get; set; }

		public SearchParametersGroup()
		{
			Parameters = new List<SearchParameter>();
			DepartmentIDs = new List<int>();
			DeviceName = "";
            SerialNumber = "";
            InventoryNumber = "";
		}
	}
}
