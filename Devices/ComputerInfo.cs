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
	public partial class ComputerInfo : Form, IDisposable
	{
		public int DeviceID { get; set; }
		//Если переменная установлена, то поиск производится по архиву
		public bool searchInArchive { get; set; }
		DevicesDatabase db = new DevicesDatabase();

		/// <summary>
		/// Заполнить дерево устройств
		/// </summary>
		public void FillInfoTree()
		{
			List<Node> list;
			if (searchInArchive)
			{
				list = db.GetDeviceInfoMetaInArchive(DeviceID);
				toolStrip1.Enabled = false;
				добавитьToolStripMenuItem.Enabled = false;
				изменитьToolStripMenuItem.Enabled = false;
				удалитьToolStripMenuItem.Enabled = false;
			}
			else
				list = db.GetDeviceInfoMeta(DeviceID);
			foreach (Node DeviceInfoMeta in list)
			{
				TreeNode node = new TreeNode();
				node.Text = DeviceInfoMeta.NodeName;
				node.Tag = new NodeProperty(DeviceInfoMeta.NodeID, NodeTypeEnum.DeviceComplexParameter);
				TreeNodesHelper.AddNode(node, treeViewDeviceInfo.Nodes,
					treeViewDeviceInfo.Nodes, DeviceInfoMeta.ParentNodeID);
			}
			list.Clear();	
			if (searchInArchive)
				list = db.GetDeviceInfoFromArchive(DeviceID);
			else
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

		/// <summary>
		/// Инициализация форму ComputerInfo
		/// </summary>
		public ComputerInfo()
		{
			InitializeComponent();
			searchInArchive = false;
		}

		private void treeViewDeviceInfo_AfterSelect(object sender, TreeViewEventArgs e)
		{
			int ID = ((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeID;
			NodeTypeEnum nodeType = ((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeType;
			DataView dv;
			if (nodeType == NodeTypeEnum.DeviceSimpleParameter)
			{
				if (!searchInArchive)
					dv = db.GetDetailDeviceInfo(ID);
				else
					dv = db.GetDetailDeviceInfoFromArchive(ID);
			}
			else
				dv = db.GetDetailDeviceInfo(-1);
			dataGridView1.DataSource = dv;
			dataGridView1.Columns["ID Node"].Visible = false;
			dataGridView1.Columns["ID Parent Node"].Visible = false;
			dataGridView1.Columns["Parameter Type"].Visible = false;
			dataGridView1.Columns["NodeRealID"].Visible = false;
			dataGridView1.Columns["Parameter Name"].HeaderText = "Характеристика";
			dataGridView1.Columns["Parameter Name"].MinimumWidth = 200;
			dataGridView1.Columns["Value"].HeaderText = "Значение";
			dataGridView1.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			db.Dispose();
		}

		#endregion

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			TreeNode empty_node = new TreeNode();
			empty_node.Tag = new NodeProperty(0, NodeTypeEnum.DeviceSimpleParameter);
			if (((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceComplexParameter)
			{
				empty_node.Text = treeViewDeviceInfo.SelectedNode.Text;
				treeViewDeviceInfo.SelectedNode.Nodes.Add(empty_node);
				treeViewDeviceInfo.SelectedNode = empty_node;
			}
			else
				if (((NodeProperty)treeViewDeviceInfo.SelectedNode.Parent.Tag).NodeType == NodeTypeEnum.DeviceComplexParameter)
				{
					empty_node.Text = treeViewDeviceInfo.SelectedNode.Parent.Text;
					treeViewDeviceInfo.SelectedNode.Parent.Nodes.Add(empty_node);
					treeViewDeviceInfo.SelectedNode = empty_node;
				}
				else
					throw new Exception();
			treeViewDeviceInfo.LabelEdit = true;
			treeViewDeviceInfo.SelectedNode.BeginEdit();
		}

		private void treeViewDeviceInfo_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			//Сохранить информацию об изменениях узла дерева в БД
			if (((NodeProperty)e.Node.Tag).NodeID != 0)
			{
				if (!db.UpdateDeviceNode(((NodeProperty)e.Node.Tag).NodeID, e.Label == null ? e.Node.Text : e.Label))
					e.CancelEdit = true;
				treeViewDeviceInfo.LabelEdit = false;
			}
			else
			{
				int NodeID = db.InsertDeviceNode(((NodeProperty)e.Node.Parent.Tag).NodeID, DeviceID, e.Label == null ? e.Node.Text : e.Label);
				if (NodeID == -1)
					e.Node.Remove();
				else
					((NodeProperty)e.Node.Tag).NodeID = NodeID;
				treeViewDeviceInfo.LabelEdit = false;
			}
			treeViewDeviceInfo_AfterSelect(treeViewDeviceInfo, new TreeViewEventArgs(treeViewDeviceInfo.SelectedNode));
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			if (((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceComplexParameter)
			{
				MessageBox.Show("Вы не можете удалить группу", "Внимание",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (MessageBox.Show("Вы уверены, что хотите удалить запись", "Внимание",
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				if (db.DeleteDeviceNode(((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeID))
					treeViewDeviceInfo.SelectedNode.Remove();
			}
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			if (((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceComplexParameter)
			{
				MessageBox.Show("Вы не можете менять название группы параметров", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			treeViewDeviceInfo.LabelEdit = true;
			treeViewDeviceInfo.SelectedNode.BeginEdit();
		}

		private void dataGridView1_DoubleClick(object sender, EventArgs e)
		{
			if (searchInArchive)
				return;
			//Отобразить форму редактирования выбранного параметра
			ComputerParamChangeForm cpcf = new ComputerParamChangeForm();
			cpcf.AssocMetaNodeID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID Node"].Value.ToString());
			int NodeRealID = -1;
			if (!Int32.TryParse(dataGridView1.SelectedRows[0].Cells["NodeRealID"].Value.ToString(), out NodeRealID))
				cpcf.NodeRealID = -1;
			else
			{
				cpcf.Value = dataGridView1.SelectedRows[0].Cells["Value"].Value;
				cpcf.NodeRealID = NodeRealID;
			}
			cpcf.ParamType = dataGridView1.SelectedRows[0].Cells["Parameter Type"].Value.ToString();
			cpcf.ParamName = dataGridView1.SelectedRows[0].Cells["Parameter Name"].Value.ToString();
			cpcf.ParentNodeID = ((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeID;
			cpcf.DeviceID = DeviceID;
			cpcf.InitForm();
			cpcf.ShowDialog();
			if (cpcf.isChanged)
				treeViewDeviceInfo_AfterSelect(dataGridView1, new TreeViewEventArgs(treeViewDeviceInfo.SelectedNode));
			cpcf.Dispose();
		}

		private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				dataGridView1_DoubleClick(sender, e);
				e.Handled = true;
			}
			else
				if (e.KeyCode == Keys.Escape)
					Close();
		}

		private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStripButton3_Click(sender, e);
		}

		private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStripButton1_Click(sender, e);
		}

		private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStripButton2_Click(sender, e);
		}

		private void выходToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void treeViewDeviceInfo_KeyDown_1(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}
	}
}
