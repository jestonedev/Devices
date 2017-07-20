using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Devices.ReportsForms;
using Reporting;

namespace Devices
{
	public partial class MainForm : Form, IDisposable
	{
		private DevicesDatabase db { get; set; }
        private SearchParametersGroup spg { get; set; }

        private MoveComputersForm mcf;
        private readonly string _deviceNameCommandLineArg;

		public MainForm(string[] args)
		{
			InitializeComponent();
            spg = new SearchParametersGroup();
            var arguments = args.Select(r => r.Split(new[] { '=' }, 2));
            var deviceCommandLineArg = arguments.FirstOrDefault(r => r.Length > 1 && r[0] == "--computer");
            if (deviceCommandLineArg != null)
		    {
                _deviceNameCommandLineArg = deviceCommandLineArg[1];
		    }
		}

        private void MainForm_Load(object sender, EventArgs e)
		{
            if (!string.IsNullOrEmpty(_deviceNameCommandLineArg))
            {
                spg.deviceName = _deviceNameCommandLineArg;
            }
            Reload(_deviceNameCommandLineArg != null);
            toolStripButton6.Checked = _deviceNameCommandLineArg != null;
		}

		private void Reload(bool autoOpenFirstFoundDevice = false)
		{
			treeViewComputers.Nodes.Clear();
			db = new DevicesDatabase();
			var list = db.GetDepartments(spg);
		    var listFull = db.GetDepartments(new SearchParametersGroup());
            var cache = new List<Node>();
            int cacheCount;
            do
            {
                cacheCount = cache.Count;
                cache.Clear();
                foreach (var department in listFull)
                {
                    if (!HasChildDepartmentsFromActiveList(department, listFull, list))
                        continue;
                    var node = new TreeNode
                    {
                        Text = department.NodeName,
                        Tag = new NodeProperty(department.NodeID, NodeTypeEnum.DepartmentNode)
                    };
                    var inserted = TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, department.ParentNodeID);
                    if (!inserted)
                        cache.Add(department);
                }
                listFull.Clear();
                listFull.AddRange(cache);
            }
            while (cache.Count != cacheCount);
			list = db.GetDevices(spg);
		    TreeNode selectNode = null;
			foreach (var device in list)
			{
			    var node = new TreeNode
			    {
			        Text = device.NodeName,
			        Tag = new NodeProperty(device.NodeID, NodeTypeEnum.DeviceNode)
			    };
			    TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, device.ParentNodeID);
                if (selectNode == null && autoOpenFirstFoundDevice)
                {
                    selectNode = node;
			    }
			}
			treeViewComputers.Sort();
		    if (treeViewComputers.Nodes.Count > 0)
		    {
                treeViewComputers.SelectedNode = selectNode ?? treeViewComputers.Nodes[0];
		    }
		}

	    private bool HasChildDepartmentsFromActiveList(Node department, List<Node> listFull, List<Node> list)
	    {
	        return list.Any(v => v.NodeID == department.NodeID) ||
                   listFull.Where(v => v.ParentNodeID == department.NodeID).
                   Any(v => HasChildDepartmentsFromActiveList(v, listFull, list));
	    }

	    private void OpenDeviceInfo()
		{
	        if (((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeType != NodeTypeEnum.DeviceNode) return;
	        var compForm = new ComputerInfo {DeviceID = ((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeID};
	        compForm.FillInfoTree();
	        compForm.ShowDialog();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			var addForm = new AddDepartmentAndPCForm();
			var empty_node = new TreeNode();
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
			OpenDeviceInfo();
		}

		private void toolStripButtonDelete_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Вы уверены, что хотите удалить запись", "Внимание",
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				var db = new DevicesDatabase();
				if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DepartmentNode)
				{
					if (db.DeleteDepartment(((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID))
						treeViewComputers.SelectedNode.Remove();
				}
				else
				{
					if (db.DeleteDevice(((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID))
					{
						var tmp_node = treeViewComputers.SelectedNode.Parent;
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
			var addForm = new AddDepartmentAndPCForm();
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
			var list = db.GetDeviceInfoMeta(DeviceID);
			foreach (var DeviceInfoMeta in list)
			{
				var node = new TreeNode();
				node.Text = DeviceInfoMeta.NodeName;
				node.Tag = new NodeProperty(DeviceInfoMeta.NodeID, NodeTypeEnum.DeviceComplexParameter);
				TreeNodesHelper.AddNode(node, treeViewDeviceInfo.Nodes,
					treeViewDeviceInfo.Nodes, DeviceInfoMeta.ParentNodeID);
			}
			list.Clear();
			list = db.GetDeviceInfo(DeviceID);
			foreach (var DeviceInfo in list)
			{
				var node = new TreeNode();
				node.Text = DeviceInfo.NodeName;
				node.Tag = new NodeProperty(DeviceInfo.NodeID, NodeTypeEnum.DeviceSimpleParameter);
				TreeNodesHelper.AddNode(node, treeViewDeviceInfo.Nodes,
					treeViewDeviceInfo.Nodes, DeviceInfo.ParentNodeID);
			}
			treeViewDeviceInfo.ExpandAll();
		}

		private void treeViewComputers_AfterSelect(object sender, TreeViewEventArgs e)
		{
            groupBoxPereferial.Visible = false;
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
            if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DepartmentNode)
            {
                textBoxName.Text = db.GetDepartmentInfo(((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID);
                textBoxType.Text = "Департамент (отдел)";
            }
            else
            {
                var dv = db.GetDeviceGeneralInfo(((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID);
                textBoxName.Text = dv[0]["Device Name"].ToString();
                textBoxInventoryNumber.Text = dv[0]["InventoryNumber"].ToString();
                textBoxSerialNumber.Text = dv[0]["SerialNumber"].ToString();
                textBoxType.Text = dv[0]["Type"].ToString();
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
		    var selectedNodeProperty = (NodeProperty)treeViewComputers.SelectedNode.Tag;
            Reload(); 
            SelectNodeByNodeId(treeViewComputers.Nodes, selectedNodeProperty.NodeID);
		}

		private void измененияТекущеToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var ai = new ArchiveInfo();
			ai.DisplayArchiveType = DisplayArchive.NodeChangesArchive;
			ai.InitializeForm();
			ai.ShowDialog();
			ai.Dispose();
		}

		private void удаленныеКомпьютерыToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var ai = new ArchiveInfo();
			ai.DisplayArchiveType = DisplayArchive.DeviceChangesArchive;
			ai.InitializeForm();
			ai.ShowDialog();
			ai.Dispose();
		}

		private void удаленныеУстройстваToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var ai = new ArchiveInfo();
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
				var iif = new InstallationsInfoForm();
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
				var rf = new RequestsForm();
                var DeviceID = ((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeID;
                rf.SerialNumber = db.GetSerialNumberBy(DeviceID, false);
                rf.InventoryNumber = db.GetInventoryNumberBy(DeviceID, false);
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
			    var selectedNodeProperty = (NodeProperty)treeViewComputers.SelectedNode.Tag;
				spg = new SearchParametersGroup();
				Reload();
			    SelectNodeByNodeId(treeViewComputers.Nodes, selectedNodeProperty.NodeID);
			}
			else
			{
			    var sfs = new SearchFormSt1 {spg = spg};
			    sfs.ShowDialog();
				if (!sfs.CancelSearch)
				{
					Reload(true);
					toolStripButton6.Checked = true;
				}
				sfs.Dispose();
			}
		}

        private bool SelectNodeByNodeId(TreeNodeCollection nodes, int nodeId)
        {
            foreach (TreeNode node in nodes)
            {
                var nodeProperty = (NodeProperty) node.Tag;
                if (nodeProperty.NodeID == nodeId)
                {
                    treeViewComputers.SelectedNode = node;
                    return true;
                }
                if (SelectNodeByNodeId(node.Nodes, nodeId))
                {
                    return true;
                }
            }
            return false;
        }

		private void GetDevicesInDepartment(TreeNode current_node, string Department, List<Device> devices)
		{
			foreach (TreeNode node in current_node.Nodes)
			{
				if (((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DeviceNode)
				{
					var dev = new Device();
					dev.DeviceID = ((NodeProperty)node.Tag).NodeID;
					dev.Name = node.Text;
					dev.Department = Department;
					devices.Add(dev);
				} else
					if (((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DepartmentNode)
					{
                        GetDevicesInDepartment(node, BuildDepartmentPath(node), devices);
				    }
			}
		}

	    private string BuildDepartmentPath(TreeNode department)
	    {
	        if (department == null || ((NodeProperty) department.Tag).NodeType != NodeTypeEnum.DepartmentNode)
	            return "";
            var departmentStr = department.Text;
	        while (department.Parent != null)
	        {
	            department = department.Parent;
	            departmentStr = department.Text + " / " + departmentStr;
	        }
	        return departmentStr;
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
			var node = (TreeNode)treeViewComputers.SelectedNode.Clone();
            var NP = ((NodeProperty)treeViewComputers.SelectedNode.Tag);
            if (mcf == null)
                mcf = new MoveComputersForm(NP);
            else
                mcf.NP = NP;
			mcf.Text = "Перемещение узла " + treeViewComputers.SelectedNode.Text;
            mcf.Moved = false;
			mcf.ShowDialog();
			if (mcf.Moved)
			{
				//Удалить в старом департаменте узел и добавить в новый
				treeViewComputers.SelectedNode.Remove();
				//Добавить узел в новое место
				TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, mcf.NewID);
				treeViewComputers.Sort();
			}
		}

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (treeViewDeviceInfo.SelectedNode.Parent == null ||
                treeViewDeviceInfo.SelectedNode.Parent.Text != "Периферийные устройства")
                e.Cancel = true;
            if (treeViewDeviceInfo.SelectedNode.Text == "Системный блок")
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
        }

        private void contextMenuStrip2_Click(object sender, EventArgs e)
        {
            if (treeViewDeviceInfo.SelectedNode.Parent == null ||
                treeViewDeviceInfo.SelectedNode.Parent.Text != "Периферийные устройства")
                return;
            var rf = new RequestsForm();
            var ID = ((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeID;
            rf.SerialNumber = db.GetSerialNumberBy(ID, true);
            rf.InventoryNumber = db.GetInventoryNumberBy(ID, true);
            rf.InitializeForm(db);
            rf.ShowDialog();
            rf.Dispose();
        }

        private void treeViewDeviceInfo_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewDeviceInfo.SelectedNode.Parent == null ||
                treeViewDeviceInfo.SelectedNode.Parent.Text != "Периферийные устройства")
                groupBoxPereferial.Visible = false;
            else
            {
                var ID = ((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeID;
                var nodeType = ((NodeProperty)treeViewDeviceInfo.SelectedNode.Tag).NodeType;
                var dv =  db.GetDetailDeviceInfo(ID);
                foreach (DataRowView row in dv)
                {
                    switch ((int)row["ID Node"])
                    {
                        case 25:
                            textBoxPereferialType.Text = row["Value"].ToString();
                            break;
                        case 26:
                            textBoxPereferialName.Text = row["Value"].ToString();
                            break;
                        case 44:
                            textBoxPereferialSerialNumber.Text = row["Value"].ToString();
                            break;
                        case 45:
                            textBoxPereferialInventoryNumber.Text = row["Value"].ToString();
                            break;
                    }
                }
                groupBoxPereferial.Visible = true;
            }
        }

        //Генерация отчета "Список компьютеров"
        private void ComputersDevices_Click(object sender, EventArgs e)
        {
            var list = new List<Device>();
            var root = treeViewComputers.Nodes;
            foreach (TreeNode node in root)
            {
                string Department;
                if (((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DepartmentNode)
                    Department = node.Text;
                else
                    continue;
                GetDevicesInDepartment(node, Department, list);
            }
            //Получаем дополнительную информацию об устройствах
            db.GetExInfoByDeviceIdList(list);
            //Сгенерировать отчет
            var rep = new Reporter();
            rep.DevicesReport(list);
        }

        // Генерация отчета "Список периф. оборудования"
        private void PeripheryDevices_Click(object sender, EventArgs e)
        {            
            var repForm2 = new PeripheryDevicesForm();            
            if (repForm2.ShowDialog() != DialogResult.OK)
                return;
            var idsTypes = repForm2.GetFilterIds();           
            var devicesRep = new PeripheryListReporter();
            devicesRep.ReportTitle = "Список периферийных устройств";
            devicesRep.Arguments.Add("ids_types", idsTypes);

            var list = new List<Device>();
            var root = treeViewComputers.Nodes;
            foreach (TreeNode node in root)
            {
                string Department;
                if (((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DepartmentNode)
                    Department = node.Text;
                else
                    continue;
                GetDevicesInDepartment(node, Department, list);
            }
            var where = "";
            foreach (var device in list)
            {
                where += "," + device.DeviceID;
            }
            devicesRep.Arguments.Add("where_devices", where);
            devicesRep.Run();                                      
        }

        private void ManualMenuItem_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                @"Documentation\Manual.odt");
            if (!File.Exists(fileName))
            {
                MessageBox.Show(
                    string.Format("Не удалось найти руководство пользователя по пути {0}", fileName),
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var process = new Process())
            {
                var psi = new ProcessStartInfo(fileName);
                process.StartInfo = psi;
                process.Start();
            }
        }

        private void devicesFeaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var devicesRep = new DevicesFeaturesReporter {ReportTitle = "Характеристики оборудования"};

            var list = new List<Device>();
            var root = treeViewComputers.Nodes;
            foreach (TreeNode node in root)
            {
                string Department;
                if (((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DepartmentNode)
                    Department = node.Text;
                else
                    continue;
                GetDevicesInDepartment(node, Department, list);
            }
            var where = "";
            foreach (var device in list)
            {
                where += "," + device.DeviceID;
            }
            devicesRep.Arguments.Add("where_devices", where);
            devicesRep.Run();  
        }

        private void treeViewComputers_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) treeViewComputers.SelectedNode = e.Node;
        }

        private void treeViewComputers_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (toolStripButton1.Enabled && e.Node == treeViewComputers.SelectedNode)
            {
                toolStripButton1_Click_1(sender, new EventArgs());
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

    internal class PeripheryType
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
