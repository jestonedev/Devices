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
	public partial class MoveComputersForm : Form
	{
        public NodeProperty NP;
        public int NewID { get; set; }
		public bool Moved { get; set; }

		private DevicesDatabase db { get; set; }
		private SearchParametersGroup spg { get; set; }

		public MoveComputersForm(NodeProperty NP)
		{
			InitializeComponent();
            this.NP = NP;
			Moved = false;
			spg = new SearchParametersGroup();
			db = new DevicesDatabase();
            LoadNodes();
		}

        private void LoadNodes()
        {
            List<Node> list = db.GetDepartments(spg);
            List<Node> cache = new List<Node>();
            int cacheCount = 0;
            do
            {
                cacheCount = cache.Count;
                cache.Clear();
                foreach (Node department in list)
                {
                    TreeNode node = new TreeNode();
                    node.Text = department.NodeName;
                    node.Tag = new NodeProperty(department.NodeID, NodeTypeEnum.DepartmentNode);
                    bool inserted = TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, department.ParentNodeID);
                    if (!inserted)
                        cache.Add(department);
                }
                list.Clear();
                list.AddRange(cache);
            }
            while (cache.Count != cacheCount);
            if (NP.NodeType != NodeTypeEnum.DeviceSimpleParameter)
            {
                treeViewComputers.Sort();
                if (treeViewComputers.Nodes.Count > 0)
                    treeViewComputers.SelectedNode = treeViewComputers.Nodes[0];
                return;
            }
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

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			NewID = ((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID;
            if (NP.NodeType == NodeTypeEnum.DeviceNode)
            {
                if (db.MoveDevice(NP.NodeID, NewID))
                    Moved = true;
            } else
                if (NP.NodeType == NodeTypeEnum.DepartmentNode)
            {
                if (IDInSubNodes(NP.NodeID, treeViewComputers.SelectedNode))
                {
                    MessageBox.Show("Вы пытаетесь переместить департамент сам в себя или в дочернее подразделение, образовав циклическую зависимость", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (db.MoveDepartment(NP.NodeID, NewID))
                    Moved = true;
            } else
                    if (NP.NodeType == NodeTypeEnum.DeviceSimpleParameter)
            {
                if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType != NodeTypeEnum.DeviceNode)
                {
                    MessageBox.Show("Для переноса характеристики устройства необходимо выбрать другое устройство, а не департамент", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (db.MoveParameter(NP.NodeID, NewID))
                    Moved = true;
            }
			Close();
		}

        private bool IDInSubNodes(int id, TreeNode node)
        {
            if (id == ((NodeProperty)node.Tag).NodeID)
                return true;
            if (node.Parent != null)
                return IDInSubNodes(id, node.Parent);
            return false;
        }

        private void MoveComputersForm_Shown(object sender, EventArgs e)
        {
            NodeProperty nodeProperty = (NodeProperty)treeViewComputers.SelectedNode.Tag;
            treeViewComputers.Nodes.Clear();
            LoadNodes();
            SelectNode(treeViewComputers.Nodes, nodeProperty);
            treeViewComputers.Focus();
        }

        private bool SelectNode(TreeNodeCollection nodes, NodeProperty nodeProperty)
        {
            foreach (TreeNode currentNode in nodes)
            {
                NodeProperty currentNodeProperty = (NodeProperty)(currentNode.Tag);
                if (nodeProperty.NodeType == currentNodeProperty.NodeType && nodeProperty.NodeID == currentNodeProperty.NodeID)
                {
                    treeViewComputers.SelectedNode = currentNode;
                    return true;
                }
                if (SelectNode(currentNode.Nodes, nodeProperty) == true)
                    return true;
            }
            return false;
        }
	}
}
