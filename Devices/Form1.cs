using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Devices
{
	public partial class Form1 : Form, IDisposable
	{
		private DevicesDatabase db { get; set; }
		private SearchParametersGroup spg { get; set; }

		public Form1()
		{
			InitializeComponent();
            spg = new SearchParametersGroup();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Reload();
		}

		private void Reload()
		{
			treeViewComputers.Nodes.Clear();
			db = new DevicesDatabase();
			List<Node> list = db.GetDepartments(spg);
            List<Node> cache = new List<Node>();
            int cacheCount = 0;
            do
            {
                cacheCount = cache.Count;
                foreach (Node department in list)
                {
                    TreeNode node = new TreeNode();
                    node.Text = department.NodeName;
                    node.Tag = new NodeProperty(department.NodeID, NodeTypeEnum.DepartmentNode);
                    bool inserted = TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, department.ParentNodeID);
                    if (!inserted)
                        cache.Add(department);
                }
            }
            while (cache.Count != cacheCount);
			list.Clear();
			list = db.GetDevices(spg);
			foreach (Node device in list)
			{
				TreeNode node = new TreeNode();
				node.Text = device.NodeName;
				node.Tag = new NodeProperty(device.NodeID, NodeTypeEnum.DeviceNode);
				TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, device.ParentNodeID);
			}
			treeViewComputers.Sort();
			if (treeViewComputers.Nodes.Count > 0)
				treeViewComputers.SelectedNode = treeViewComputers.Nodes[0];
		}

		private void openDeviceInfo()
		{
			if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceNode)
			{
				ComputerInfo compForm = new ComputerInfo();
				compForm.DeviceID = ((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID;
				compForm.FillInfoTree();
				compForm.ShowDialog();
			}
		}

		private void treeViewComputers_DoubleClick(object sender, EventArgs e)
		{
			toolStripButton1_Click_1(sender, new EventArgs());
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			AddDepartmentAndPCForm addForm = new AddDepartmentAndPCForm();
			TreeNode empty_node = new TreeNode();
			empty_node.Tag = new NodeProperty(-1, NodeTypeEnum.DeviceNode);
			addForm.currentNode = empty_node;
			if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DepartmentNode)
				addForm.parentNode = treeViewComputers.SelectedNode;
			else
				addForm.parentNode = treeViewComputers.SelectedNode.Parent;
			addForm.InitializeForm();
			addForm.ShowDialog();
		}

		private void toolStripButton1_Click_1(object sender, EventArgs e)
		{
			openDeviceInfo();
		}

		private void toolStripButtonDelete_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Вы уверены, что хотите удалить запись", "Внимание",
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				DevicesDatabase db = new DevicesDatabase();
				if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DepartmentNode)
				{
					if (db.DeleteDepartment(((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID))
						treeViewComputers.SelectedNode.Remove();
				}
				else
				{
					if (db.DeleteDevice(((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID))
					{
						TreeNode tmp_node = treeViewComputers.SelectedNode.Parent;
						if (tmp_node.Nodes.Count == 1)
						{
							while (tmp_node != null)
							{
								tmp_node.ForeColor = Color.Black;
								tmp_node = tmp_node.Parent;
							}
						}
						treeViewComputers.SelectedNode.Remove();
					}
				}
				db.Dispose();
			}
		}

		private void treeViewComputers_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				выходToolStripMenuItem_Click(sender, new EventArgs());
			else
				if (e.KeyCode == Keys.Enter)
					toolStripButton1_Click_1(sender, new EventArgs());

		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			if (treeViewComputers.SelectedNode.Index < 0)
				return;
			AddDepartmentAndPCForm addForm = new AddDepartmentAndPCForm();
			addForm.currentNode = treeViewComputers.SelectedNode;
			if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DepartmentNode)
				addForm.parentNode = treeViewComputers.SelectedNode;
			else
				addForm.parentNode = treeViewComputers.SelectedNode.Parent;
			addForm.InitializeForm();
			addForm.ShowDialog();
		}

		public void FillInfoTree(int DeviceID)
		{
			List<Node> list = db.GetDeviceInfoMeta(DeviceID);
			foreach (Node DeviceInfoMeta in list)
			{
				TreeNode node = new TreeNode();
				node.Text = DeviceInfoMeta.NodeName;
				node.Tag = new NodeProperty(DeviceInfoMeta.NodeID, NodeTypeEnum.DeviceComplexParameter);
				TreeNodesHelper.AddNode(node, treeViewDeviceInfo.Nodes,
					treeViewDeviceInfo.Nodes, DeviceInfoMeta.ParentNodeID);
			}
			list.Clear();
			list = db.GetDeviceInfo(DeviceID);
			foreach (Node DeviceInfo in list)
			{
				TreeNode node = new TreeNode();
				node.Text = DeviceInfo.NodeName;
				node.Tag = new NodeProperty(DeviceInfo.NodeID, NodeTypeEnum.DeviceSimpleParameter);
				TreeNodesHelper.AddNode(node, treeViewDeviceInfo.Nodes,
					treeViewDeviceInfo.Nodes, DeviceInfo.ParentNodeID);
			}
			treeViewDeviceInfo.ExpandAll();
		}

		private void treeViewComputers_AfterSelect(object sender, TreeViewEventArgs e)
		{
			treeViewDeviceInfo.Nodes.Clear();
			if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceNode)
			{
				FillInfoTree(((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID);
				открытьToolStripMenuItem.Enabled = true;
				открытьСписокУстановленногоПОToolStripMenuItem.Enabled = true;
				открытьСписокЗаявокToolStripMenuItem.Enabled = true;
				toolStripButton1.Enabled = true;
				toolStripButton4.Enabled = true;
				toolStripButton5.Enabled = true;
			}
			else
			{
				открытьToolStripMenuItem.Enabled = false;
				открытьСписокУстановленногоПОToolStripMenuItem.Enabled = false;
				открытьСписокЗаявокToolStripMenuItem.Enabled = false;
				toolStripButton1.Enabled = false;
				toolStripButton4.Enabled = false;
				toolStripButton5.Enabled = false;
			}

		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			db.Dispose();
		}

		#endregion

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			Reload();
		}

		private void измененияТекущеToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ArchiveInfo ai = new ArchiveInfo();
			ai.DisplayArchiveType = DisplayArchive.NodeChangesArchive;
			ai.InitializeForm();
			ai.ShowDialog();
			ai.Dispose();
		}

		private void удаленныеКомпьютерыToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ArchiveInfo ai = new ArchiveInfo();
			ai.DisplayArchiveType = DisplayArchive.DeviceChangesArchive;
			ai.InitializeForm();
			ai.ShowDialog();
			ai.Dispose();
		}

		private void удаленныеУстройстваToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ArchiveInfo ai = new ArchiveInfo();
			ai.DisplayArchiveType = DisplayArchive.DeletedDeviceArchive;
			ai.InitializeForm();
			ai.ShowDialog();
			ai.Dispose();
		}

		private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStripButton1_Click_1(sender, e);
		}

		private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStripButton2_Click(sender, e);
		}

		private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStripButton1_Click(sender, e);
		}

		private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStripButtonDelete_Click(sender, e);
		}

		private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStripButton3_Click(sender, e);
		}

		private void выходToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Вы действительно хотите выйти из программы?", "Выход", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				Close();
		}

		private void splitContainer1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				выходToolStripMenuItem_Click(sender, new EventArgs());
		}

		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceNode)
			{
				InstallationsInfoForm iif = new InstallationsInfoForm();
				iif.DeviceID = ((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID;
				iif.InitializeForm(db);
				iif.ShowDialog();
				iif.Dispose();
			}
			else
				MessageBox.Show("Выбранный узел не является устройством. Выберете устройство (ПК), на котором вы хотели бы просмотреть список установленного ПО",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void открытьСписокУстановленногоПОToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStripButton4_Click(sender, e);
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceNode)
			{
				открытьХарактеристикиУстройстваToolStripMenuItem.Enabled = true;
				открытьСписокУстановленногоПОToolStripMenuItem1.Enabled = true;
				открытьСписокЗаявокToolStripMenuItem1.Enabled = true;
			}
			else
			{
				открытьХарактеристикиУстройстваToolStripMenuItem.Enabled = false;
				открытьСписокУстановленногоПОToolStripMenuItem1.Enabled = false;
				открытьСписокЗаявокToolStripMenuItem1.Enabled = false;
			}
		}

		private void toolStripButton5_Click(object sender, EventArgs e)
		{
			if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceNode)
			{
				RequestsForm rf = new RequestsForm();
				rf.DeviceID = ((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID;
				rf.InitializeForm(db);
				rf.ShowDialog();
				rf.Dispose();
			}
			else
				MessageBox.Show("Выбранный узел не является устройством. Выберете устройство (ПК), на котором вы хотели бы просмотреть список заявок",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void toolStripButton6_Click(object sender, EventArgs e)
		{
			if (toolStripButton6.Checked)
			{
				toolStripButton6.Checked = false;
				spg = new SearchParametersGroup();
				Reload();
			}
			else
			{
				SearchFormSt1 sfs = new SearchFormSt1();
				sfs.spg = spg;
				sfs.ShowDialog();
				if (!sfs.CancelSearch)
				{
					Reload();
					toolStripButton6.Checked = true;

				}
				sfs.Dispose();
			}
		}

		private void GetDevicesInDepartment(TreeNode current_node, string Department, List<Device> devices)
		{
			foreach (TreeNode node in current_node.Nodes)
			{
				if (((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DeviceNode)
				{
					Device dev = new Device();
					dev.DeviceID = ((NodeProperty)node.Tag).NodeID;
					dev.Name = node.Text;
					dev.Department = Department;
					devices.Add(dev);
				} else
					if (((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DepartmentNode)
				{
					GetDevicesInDepartment(node, Department, devices);
				}
			}
		}

		private void toolStripButton7_Click(object sender, EventArgs e)
		{
			List<Device> list = new List<Device>();
			TreeNodeCollection root = treeViewComputers.Nodes;
			foreach (TreeNode node in root)
			{
				string Department = "";
				if (((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DepartmentNode)
					Department = node.Text;
				else
					continue;
				GetDevicesInDepartment(node, Department, list);
			}
			//Получаем дополнительную информацию об устройствах
			db.GetExInfoByDeviceIdList(list);
			//Сгенерировать отчет
			Reporter rep = new Reporter();
			rep.DevicesReport(list);
		}

		private void toolStripButton8_Click(object sender, EventArgs e)
		{
			if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType != NodeTypeEnum.DeviceNode &&
                ((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType != NodeTypeEnum.DepartmentNode)
				return;
			if (treeViewComputers.SelectedNode.Parent == null)
				return;
			if (((NodeProperty)treeViewComputers.SelectedNode.Parent.Tag).NodeID <= 0)
				return;
			TreeNode node = (TreeNode)treeViewComputers.SelectedNode.Clone();
			MoveComputersForm mcf = new MoveComputersForm();
			mcf.Text = "Перемещение узла " + treeViewComputers.SelectedNode.Text;
			mcf.NP = ((NodeProperty)treeViewComputers.SelectedNode.Tag);
			mcf.ShowDialog();
			if (mcf.Moved)
			{
				//Удалить в старом департаменте узел и добавить в новый
				treeViewComputers.SelectedNode.Remove();
				//Добавить узел в новое место
				TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, mcf.NewDepartmentID);
				treeViewComputers.Sort();
			}
		}
	}

	internal class Device
	{
		public string Department { get; set; }
		public int DeviceID { get; set; }
		public string Name { get; set; }
		public string SerialNumber { get; set; }
		public string InventoryNumber { get; set; }

		public Device()
		{
		}

		public Device(string Department, int DeviceID, string Name, string SerialNumber, string InventoryNumber)
		{
			this.Department = Department;
			this.DeviceID = DeviceID;
			this.Name = Name;
			this.SerialNumber = SerialNumber;
			this.InventoryNumber = InventoryNumber;
		}
	}
}
