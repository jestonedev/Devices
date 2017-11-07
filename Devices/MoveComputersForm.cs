using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Devices
{
	public partial class MoveComputersForm : Form
	{
        public NodeProperty Np;
        public int NewId { get; set; }
		public bool Moved { get; set; }

		private DevicesDatabase Db { get; set; }
		private SearchParametersGroup Spg { get; set; }

		public MoveComputersForm(NodeProperty np)
		{
			InitializeComponent();
            Np = np;
			Moved = false;
			Spg = new SearchParametersGroup();
			Db = new DevicesDatabase();
            LoadNodes();
		}

        private void LoadNodes()
        {
            var list = Db.GetDepartments(Spg);
            var cache = new List<Node>();
            int cacheCount;
            do
            {
                cacheCount = cache.Count;
                cache.Clear();
                foreach (var department in list)
                {
                    var node = new TreeNode
                    {
                        Text = department.NodeName,
                        Tag = new NodeProperty(department.NodeId, NodeTypeEnum.DepartmentNode)
                    };
                    var inserted = TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, department.ParentNodeId);
                    if (!inserted)
                        cache.Add(department);
                }
                list.Clear();
                list.AddRange(cache);
            }
            while (cache.Count != cacheCount);
            if (Np.NodeType != NodeTypeEnum.DeviceSimpleParameter)
            {
                treeViewComputers.Sort();
                if (treeViewComputers.Nodes.Count > 0)
                    treeViewComputers.SelectedNode = treeViewComputers.Nodes[0];
                return;
            }
            list = Db.GetDevices(Spg);
            foreach (var device in list)
            {
                var node = new TreeNode
                {
                    Text = device.NodeName,
                    Tag = new NodeProperty(device.NodeId, NodeTypeEnum.DeviceNode)
                };
                TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, device.ParentNodeId);
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
			NewId = ((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeId;
            switch (Np.NodeType)
            {
                case NodeTypeEnum.DeviceNode:
                    if (Db.MoveDevice(Np.NodeId, NewId))
                        Moved = true;
                    break;
                case NodeTypeEnum.DepartmentNode:
                    if (IDInSubNodes(Np.NodeId, treeViewComputers.SelectedNode))
                    {
                        MessageBox.Show(@"Вы пытаетесь переместить департамент сам в себя или в дочернее подразделение, образовав циклическую зависимость", 
                            @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (Db.MoveDepartment(Np.NodeId, NewId))
                        Moved = true;
                    break;
                case NodeTypeEnum.DeviceSimpleParameter:
                    if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType != NodeTypeEnum.DeviceNode)
                    {
                        MessageBox.Show(@"Для переноса характеристики устройства необходимо выбрать другое устройство, а не департамент", @"Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (Db.MoveParameter(Np.NodeId, NewId))
                        Moved = true;
                    break;
            }
			Close();
		}

        private bool IDInSubNodes(int id, TreeNode node)
        {
            if (id == ((NodeProperty)node.Tag).NodeId)
                return true;
            if (node.Parent != null)
                return IDInSubNodes(id, node.Parent);
            return false;
        }

        private void MoveComputersForm_Shown(object sender, EventArgs e)
        {
            var nodeProperty = (NodeProperty)treeViewComputers.SelectedNode.Tag;
            treeViewComputers.Nodes.Clear();
            LoadNodes();
            SelectNode(treeViewComputers.Nodes, nodeProperty);
            treeViewComputers.Focus();
        }

        private bool SelectNode(TreeNodeCollection nodes, NodeProperty nodeProperty)
        {
            foreach (TreeNode currentNode in nodes)
            {
                var currentNodeProperty = (NodeProperty)(currentNode.Tag);
                if (nodeProperty.NodeType == currentNodeProperty.NodeType && nodeProperty.NodeId == currentNodeProperty.NodeId)
                {
                    treeViewComputers.SelectedNode = currentNode;
                    return true;
                }
                if (SelectNode(currentNode.Nodes, nodeProperty))
                    return true;
            }
            return false;
        }
	}
}
