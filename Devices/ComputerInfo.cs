using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Devices
{
	public partial class ComputerInfo : Form, IDisposable
	{
		public int DeviceId { get; set; }
		//Если переменная установлена, то поиск производится по архиву
		public bool SearchInArchive { get; set; }
		private readonly DevicesDatabase _db = new DevicesDatabase();
        private MoveComputersForm _mcf;

		/// <summary>
		/// Заполнить дерево устройств
		/// </summary>
		public void FillInfoTree()
		{
			List<Node> list;
			if (SearchInArchive)
			{
				list = _db.GetDeviceInfoMetaInArchive(DeviceId);
				toolStrip1.Enabled = false;
				добавитьToolStripMenuItem.Enabled = false;
				изменитьToolStripMenuItem.Enabled = false;
				удалитьToolStripMenuItem.Enabled = false;
			}
			else
				list = _db.GetDeviceInfoMeta(DeviceId);
			foreach (var deviceInfoMeta in list)
			{
			    var node = new TreeNode
			    {
			        Text = deviceInfoMeta.NodeName,
			        Tag = new NodeProperty(deviceInfoMeta.NodeId, NodeTypeEnum.DeviceComplexParameter)
			    };
			    TreeNodesHelper.AddNode(node, treeViewDeviceInfo.Nodes,
					treeViewDeviceInfo.Nodes, deviceInfoMeta.ParentNodeId);
			}
			list.Clear();	
			list = SearchInArchive ? _db.GetDeviceInfoFromArchive(DeviceId) : _db.GetDeviceInfo(DeviceId);
			foreach (var deviceInfo in list)
			{
			    var node = new TreeNode
			    {
			        Text = deviceInfo.NodeName,
			        Tag = new NodeProperty(deviceInfo.NodeId, NodeTypeEnum.DeviceSimpleParameter)
			    };
			    TreeNodesHelper.AddNode(node, treeViewDeviceInfo.Nodes,
					treeViewDeviceInfo.Nodes, deviceInfo.ParentNodeId);
			}
			treeViewDeviceInfo.ExpandAll();
		}

		/// <summary>
		/// Инициализация форму ComputerInfo
		/// </summary>
		public ComputerInfo()
		{
			InitializeComponent();
			SearchInArchive = false;
		}

		private void treeViewDeviceInfo_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var id = ((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeId;
			var nodeType = ((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeType;
			DataView dv;
			if (nodeType == NodeTypeEnum.DeviceSimpleParameter)
			{
				dv = !SearchInArchive ? _db.GetDetailDeviceInfo(id) : _db.GetDetailDeviceInfoFromArchive(id);
			}
			else
				dv = _db.GetDetailDeviceInfo(-1);
			dataGridView1.DataSource = dv;
			dataGridView1.Columns["ID Node"].Visible = false;
			dataGridView1.Columns["ID Parent Node"].Visible = false;
			dataGridView1.Columns["Parameter Type"].Visible = false;
			dataGridView1.Columns["NodeRealID"].Visible = false;
			dataGridView1.Columns["Parameter Name"].HeaderText = @"Характеристика";
			dataGridView1.Columns["Parameter Name"].MinimumWidth = 200;
			dataGridView1.Columns["Value"].HeaderText = @"Значение";
			dataGridView1.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (treeViewDeviceInfo.SelectedNode.Parent == null ||
                treeViewDeviceInfo.SelectedNode.Parent.Text != @"Периферийные устройства")
                toolStripButton4.Enabled = false;
            else
                toolStripButton4.Enabled = true;
            toolStripButton5.Enabled = (nodeType == NodeTypeEnum.DeviceSimpleParameter);
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			_db.Dispose();
		}

		#endregion

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
		    var emptyNode = new TreeNode {Tag = new NodeProperty(0, NodeTypeEnum.DeviceSimpleParameter)};
		    if (((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceComplexParameter)
			{
				emptyNode.Text = treeViewDeviceInfo.SelectedNode.Text;
				treeViewDeviceInfo.SelectedNode.Nodes.Add(emptyNode);
				treeViewDeviceInfo.SelectedNode = emptyNode;
			}
			else
				if (((NodeProperty)treeViewDeviceInfo.SelectedNode.Parent.Tag).NodeType == NodeTypeEnum.DeviceComplexParameter)
				{
					emptyNode.Text = treeViewDeviceInfo.SelectedNode.Parent.Text;
					treeViewDeviceInfo.SelectedNode.Parent.Nodes.Add(emptyNode);
					treeViewDeviceInfo.SelectedNode = emptyNode;
				}
				else
					throw new Exception();
			treeViewDeviceInfo.LabelEdit = true;
			treeViewDeviceInfo.SelectedNode.BeginEdit();
		}

		private void treeViewDeviceInfo_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			//Сохранить информацию об изменениях узла дерева в БД
			if (((NodeProperty)e.Node.Tag).NodeId != 0)
			{
				if (!_db.UpdateDeviceNode(((NodeProperty)e.Node.Tag).NodeId, e.Label ?? e.Node.Text))
					e.CancelEdit = true;
				treeViewDeviceInfo.LabelEdit = false;
			}
			else
			{
				var nodeId = _db.InsertDeviceNode(((NodeProperty)e.Node.Parent.Tag).NodeId, DeviceId, e.Label ?? e.Node.Text);
				if (nodeId == -1)
					e.Node.Remove();
				else
					((NodeProperty)e.Node.Tag).NodeId = nodeId;
				treeViewDeviceInfo.LabelEdit = false;
			}
			treeViewDeviceInfo_AfterSelect(treeViewDeviceInfo, new TreeViewEventArgs(treeViewDeviceInfo.SelectedNode));
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			if (((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceComplexParameter)
			{
				MessageBox.Show(@"Вы не можете удалить группу", @"Внимание",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (MessageBox.Show(@"Вы уверены, что хотите удалить запись", @"Внимание",
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				if (_db.DeleteDeviceNode(((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeId))
					treeViewDeviceInfo.SelectedNode.Remove();
			}
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			if (((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceComplexParameter)
			{
				MessageBox.Show(@"Вы не можете менять название группы параметров", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			treeViewDeviceInfo.LabelEdit = true;
			treeViewDeviceInfo.SelectedNode.BeginEdit();
		}

		private void dataGridView1_DoubleClick(object sender, EventArgs e)
		{
			if (SearchInArchive)
				return;
			//Отобразить форму редактирования выбранного параметра
		    var cpcf = new ComputerParamChangeForm
		    {
		        AssocMetaNodeId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID Node"].Value.ToString())
		    };
		    int nodeRealId;
			if (!int.TryParse(dataGridView1.SelectedRows[0].Cells["NodeRealID"].Value.ToString(), out nodeRealId))
				cpcf.NodeRealId = -1;
			else
			{
				cpcf.Value = dataGridView1.SelectedRows[0].Cells["Value"].Value;
				cpcf.NodeRealId = nodeRealId;
			}
			cpcf.ParamType = dataGridView1.SelectedRows[0].Cells["Parameter Type"].Value.ToString();
			cpcf.ParamName = dataGridView1.SelectedRows[0].Cells["Parameter Name"].Value.ToString();
			cpcf.ParentNodeId = ((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeId;
			cpcf.DeviceId = DeviceId;
			cpcf.InitForm();
			cpcf.ShowDialog();
			if (cpcf.IsChanged)
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

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (treeViewDeviceInfo.SelectedNode.Parent == null ||
                treeViewDeviceInfo.SelectedNode.Parent.Text != @"Периферийные устройства")
                e.Cancel = true;
        }

        private void открытьСписокЗаявокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewDeviceInfo.SelectedNode.Parent == null ||
                treeViewDeviceInfo.SelectedNode.Parent.Text != @"Периферийные устройства")
                return;
            var rf = new RequestsForm();
            var id = ((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeId;
            rf.SerialNumber = _db.GetSerialNumberBy(id, true);
            rf.InventoryNumber = _db.GetInventoryNumberBy(id, true);
            rf.InitializeForm(_db);
            rf.ShowDialog();
            rf.Dispose();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeType != NodeTypeEnum.DeviceSimpleParameter)
                return;
            var np = ((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag);
            if (_mcf == null)
                _mcf = new MoveComputersForm(np);
            else
                _mcf.Np = np;
            _mcf.Text = @"Перемещение узла " + treeViewDeviceInfo.SelectedNode.Text;
            _mcf.Moved = false;
            _mcf.ShowDialog();
            if (!_mcf.Moved) return;
            //Удалить характеристику в старом ПК, если перемещали не в него же
            if (DeviceId != _mcf.NewId)
                treeViewDeviceInfo.SelectedNode.Remove();
        }
	}
}
