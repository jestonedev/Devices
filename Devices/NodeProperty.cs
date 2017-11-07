using System.Windows.Forms;
using System.Drawing;

namespace Devices
{
	public enum NodeTypeEnum { DepartmentNode, DeviceNode, DeviceComplexParameter, DeviceSimpleParameter }
	
	public class NodeProperty
	{
		public int NodeId { get; set; }
		public NodeTypeEnum NodeType { get; set; }

		public NodeProperty(int nodeId, NodeTypeEnum nodeType)
		{
			NodeId = nodeId;
			NodeType = nodeType;
		}
	}

    //здесь были изменения
	public class TreeNodesHelper
	{
		public static bool AddNode(TreeNode newNode, TreeNodeCollection current, TreeNodeCollection root, int parentNodeId)
		{           
            //if (ParentNodeID == 0 || (root.Count == 0 || !ContainsParentAmongChilds(root, ParentNodeID)))
            if (parentNodeId == 0)
			{
				root.Add(newNode);                
				return true;
			}
			foreach (TreeNode node in current)
			{
				if ((((NodeProperty)node.Tag).NodeId == parentNodeId) &&
				   ((((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DepartmentNode) ||
					(((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DeviceComplexParameter)))
				{                    
					node.Nodes.Add(newNode);
				    if (((NodeProperty) newNode.Tag).NodeType != NodeTypeEnum.DeviceNode) return true;
				    var tmpNode = newNode.Parent;
				    while (tmpNode != null)
				    {
				        tmpNode.ForeColor = Color.DarkBlue;
				        tmpNode = tmpNode.Parent;
				    }
				    return true;
				}
			    var ok = AddNode(newNode, node.Nodes, root, parentNodeId);
			    if (ok)
			        return true;
			}
			return false;
		}

        //определяет есть ли любой родительский узел для текущего узла(т.е делать ли тек. узел корневым)
        public static bool ContainsParentAmongChilds(TreeNodeCollection roots, int parentNodeId)
        {
            foreach (TreeNode node in roots)
            {
                if ((((NodeProperty)node.Tag).NodeId == parentNodeId))
                    return true;
                if (ContainsParentAmongChilds(node.Nodes, parentNodeId))
                    return true;
            }
            return false;
        }
         
	}
}
