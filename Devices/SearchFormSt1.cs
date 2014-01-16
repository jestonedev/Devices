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
		}

		private void InitDataGridView(object DataSource)
		{
			BindingSource bs = new BindingSource();
			bs.DataSource = spgNew.parameters;
			dataGridView1.DataSource = bs;
			dataGridView1.Columns[0].Visible = false;
			dataGridView1.Columns[1].HeaderText = "Устройство";
			dataGridView1.Columns[1].Width = 150;
			dataGridView1.Columns[2].HeaderText = "Параметр";
			dataGridView1.Columns[2].Width = 150;
			dataGridView1.Columns[3].HeaderText = "Операция";
			dataGridView1.Columns[4].HeaderText = "Значение";
			dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		}

		private void SearchFormSt1_Load(object sender, EventArgs e)
		{
			//Загрузка департаментов в дерево
			treeViewDepartments.Nodes.Clear();
			db = new DevicesDatabase();
			spgNew = new SearchParametersGroup();
			List<Node> list = db.GetDepartments(spgNew);
			foreach (Node department in list)
			{
				TreeNode node = new TreeNode();
				node.Text = department.NodeName;
				node.Tag = new NodeProperty(department.NodeID, NodeTypeEnum.DepartmentNode);
				TreeNodesHelper.AddNode(node, treeViewDepartments.Nodes, treeViewDepartments.Nodes, department.ParentNodeID);
			}
			treeViewDepartments.ExpandAll();

			//Загрузка типов узлов в combobox
			DataView view = db.GetDeviceTypes()	;
			comboBoxDevTypes.DisplayMember = "DeviceType";
			for (int i = 0; i < view.Table.Rows.Count; i++)
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
			SearchFormSt2 sfs2 = new SearchFormSt2();
			sfs2.DeviceTypeID = ((DeviceTypeComboboxItem)comboBoxDevTypes.SelectedItem).DeviceTypeID;
			sfs2.paramList = spgNew.parameters;
			sfs2.ShowDialog();
			InitDataGridView(spgNew.parameters);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			CancelSearch = true;
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
			FillDepartmentIDs(spgNew.departmentIDs, treeViewDepartments.Nodes);
			spg.departmentIDs = spgNew.departmentIDs;
			spg.deviceTypeID = spgNew.deviceTypeID;
			spg.parameters = spgNew.parameters;
			spg.deviceName = spgNew.deviceName;
            spg.serialNumber = spgNew.serialNumber;
			CancelSearch = false;
			Close();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Вы действительно хотите удалить этот параметр?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
				== DialogResult.No)
				return;
			foreach(DataGridViewRow dgvr in dataGridView1.SelectedRows)
			{
				for (int i = 0; i < spgNew.parameters.Count; i++)
				{
					SearchParameter sp = spgNew.parameters[i];
					if ((sp.ParameterID.ToString() == dgvr.Cells[0].Value.ToString()) &&
						(sp.ParameterName == dgvr.Cells[1].Value.ToString()) && 
						(sp.Operation == dgvr.Cells[2].Value.ToString()) && 
						(sp.ParameterValue == dgvr.Cells[3].Value.ToString()))
					{
						spgNew.parameters.RemoveAt(i);
						InitDataGridView(spgNew.parameters);
						return;
					}
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

		public SearchParametersGroup()
		{
			parameters = new List<SearchParameter>();
			departmentIDs = new List<int>();
			deviceName = "";
            serialNumber = "";
		}
	}
}
