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
        public NodeProperty NP { get; set; }
		public int NewDepartmentID { get; set; }
		public bool Moved { get; set; }

		private DevicesDatabase db { get; set; }
		private SearchParametersGroup spg { get; set; }

		public MoveComputersForm()
		{
			InitializeComponent();
			Moved = false;
			spg = new SearchParametersGroup();
			db = new DevicesDatabase();
			List<Node> list = db.GetDepartments(spg);
			foreach (Node department in list)
			{
				TreeNode node = new TreeNode();
				node.Text = department.NodeName;
				node.Tag = new NodeProperty(department.NodeID, NodeTypeEnum.DepartmentNode);
				TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, department.ParentNodeID);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			NewDepartmentID = ((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID;
            if (NP.NodeType == NodeTypeEnum.DeviceNode)
            {
                if (db.MoveDevice(NP.NodeID, NewDepartmentID))
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
                if (db.MoveDepartment(NP.NodeID, NewDepartmentID))
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
	}
}
