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
		public int NodeID { get; set; }
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
			if (db.MoveDevice(NodeID, NewDepartmentID))
				Moved = true;
			Close();
		}
	}
}
