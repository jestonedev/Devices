using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Devices
{
	public enum NodeTypeEnum { DepartmentNode, DeviceNode, DeviceComplexParameter, DeviceSimpleParameter }
	
	public class NodeProperty
	{
		public int NodeID { get; set; }
		public NodeTypeEnum NodeType { get; set; }

		public NodeProperty(int NodeID, NodeTypeEnum NodeType)
		{
			this.NodeID = NodeID;
			this.NodeType = NodeType;
		}
	}

	public class TreeNodesHelper
	{
		public static bool AddNode(TreeNode new_node, TreeNodeCollection current, TreeNodeCollection root, int ParentNodeID)
		{
			if (ParentNodeID == 0)
			{
				root.Add(new_node);
				return true;
			}
			foreach (TreeNode node in current)
			{
				if ((((NodeProperty)node.Tag).NodeID == ParentNodeID) &&
				   ((((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DepartmentNode) ||
					(((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DeviceComplexParameter)))
				{
					node.Nodes.Add(new_node);
				    if (((NodeProperty) new_node.Tag).NodeType != NodeTypeEnum.DeviceNode) return true;
				    var tmp_node = new_node.Parent;
				    while (tmp_node != null)
				    {
				        tmp_node.ForeColor = Color.DarkBlue;
				        tmp_node = tmp_node.Parent;
				    }
				    return true;
				}
			    var ok = AddNode(new_node, node.Nodes, root, ParentNodeID);
			    if (ok)
			        return true;
			}
			return false;
		}
	}
}
