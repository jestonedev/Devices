using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Devices.ReportsForms;
using Reporting;

namespace Devices
{
	public partial class MainForm : Form, IDisposable
	{
		private DevicesDatabase Db { get; set; }
        private SearchParametersGroup Spg { get; set; }

        private MoveComputersForm _mcf;
        private readonly string _deviceNameCommandLineArg;

		public MainForm(string[] args)
		{
			InitializeComponent();
            Spg = new SearchParametersGroup();
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
                Spg.DeviceName = _deviceNameCommandLineArg;
            }
            Reload(_deviceNameCommandLineArg != null);
            toolStripButton6.Checked = _deviceNameCommandLineArg != null;
		}

		private void Reload(bool autoOpenFirstFoundDevice = false)
		{
			treeViewComputers.Nodes.Clear();
			Db = new DevicesDatabase();
			var list = Db.GetDepartments(Spg);
		    var listFull = Db.GetDepartments(new SearchParametersGroup());
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
                        Tag = new NodeProperty(department.NodeId, NodeTypeEnum.DepartmentNode)
                    };
                    var inserted = TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, department.ParentNodeId);
                    if (!inserted)
                        cache.Add(department);
                }
                listFull.Clear();
                listFull.AddRange(cache);
            }
            while (cache.Count != cacheCount);
            list = Db.GetDevices(Spg);
		    TreeNode selectNode = null;
			foreach (var device in list)
			{
			    var node = new TreeNode
			    {
			        Text = device.NodeName,
			        Tag = new NodeProperty(device.NodeId, NodeTypeEnum.DeviceNode)
			    };
			    TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, device.ParentNodeId);
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
            toolStripStatusLabelDeviceCount.Text = string.Format("Сетевых узлов: {0}", list.Count);

		    var monitoringWarnings = Db.GetMonitoringWarnings().ToList();
		    if (!monitoringWarnings.Any())
		    {
		        toolStripStatusLabelWarning.Visible = false;
		    }
		    else
		    {
                toolStripStatusLabelWarning.Visible = true;
                toolStripStatusLabelWarning.ToolTipText = string.Join(Environment.NewLine, monitoringWarnings.ToArray());
		    }
		}

	    private bool HasChildDepartmentsFromActiveList(Node department, List<Node> listFull, List<Node> list)
	    {
	        return list.Any(v => v.NodeId == department.NodeId) ||
                   listFull.Where(v => v.ParentNodeId == department.NodeId).
                   Any(v => HasChildDepartmentsFromActiveList(v, listFull, list));
	    }

	    private void OpenDeviceInfo()
		{
	        if (((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeType != NodeTypeEnum.DeviceNode) return;
	        var compForm = new ComputerInfo {DeviceId = ((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeId};
	        compForm.FillInfoTree();
	        compForm.ShowDialog();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			var addForm = new AddDepartmentAndPcForm();
		    var emptyNode = new TreeNode {Tag = new NodeProperty(-1, NodeTypeEnum.DeviceNode)};
		    addForm.CurrentNode = emptyNode;
			addForm.ParentNode = ((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DepartmentNode ? 
                treeViewComputers.SelectedNode : treeViewComputers.SelectedNode.Parent;
			addForm.InitializeForm();
            addForm.ShowDialog();
		}

		private void toolStripButton1_Click_1(object sender, EventArgs e)
		{
			OpenDeviceInfo();
		}

		private void toolStripButtonDelete_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(@"Вы уверены, что хотите удалить запись", @"Внимание",
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				var db = new DevicesDatabase();
				if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DepartmentNode)
				{
					if (db.DeleteDepartment(((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeId))
						treeViewComputers.SelectedNode.Remove();
				}
				else
				{
					if (db.DeleteDevice(((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeId))
					{
						var tmpNode = treeViewComputers.SelectedNode.Parent;
						if (tmpNode.Nodes.Count == 1)
						{
							while (tmpNode != null)
							{
								tmpNode.ForeColor = Color.Black;
								tmpNode = tmpNode.Parent;
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
		    var addForm = new AddDepartmentAndPcForm
		    {
		        CurrentNode = treeViewComputers.SelectedNode,
		        ParentNode = ((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DepartmentNode
		            ? treeViewComputers.SelectedNode
		            : treeViewComputers.SelectedNode.Parent
		    };
		    addForm.InitializeForm();
			addForm.ShowDialog();
		}

		public void FillInfoTree(int deviceId)
		{
			var list = Db.GetDeviceInfoMeta(deviceId);
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
			list = Db.GetDeviceInfo(deviceId);
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

		private void treeViewComputers_AfterSelect(object sender, TreeViewEventArgs e)
		{
            groupBoxPereferial.Visible = false;
            treeViewDeviceInfo.Nodes.Clear();
            dataGridViewMonitoring.Rows.Clear();
			if (((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceNode)
			{
				FillInfoTree(((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeId);
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
                textBoxName.Text = Db.GetDepartmentInfo(((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeId);
                textBoxType.Text = @"Департамент (отдел)";
            }
            else
            {
                var dv = Db.GetDeviceGeneralInfo(((NodeProperty)treeViewComputers.SelectedNode.Tag).NodeId);
                textBoxName.Text = dv[0]["Device Name"].ToString();
                textBoxInventoryNumber.Text = dv[0]["InventoryNumber"].ToString();
                textBoxSerialNumber.Text = dv[0]["SerialNumber"].ToString();
                textBoxType.Text = dv[0]["Type"].ToString();

                var monitoring =
                    Db.GetDeviceMonitoringPropertiesInfo(((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeId);
                var monitoringConditions = Db.GetMonitoringWarningConditions().ToList();
                for (var i = 0; i < monitoring.Count; i++)
                {
                    var propertyName = monitoring[i].Row["Property Name"] == DBNull.Value ? null : (string) monitoring[i].Row["Property Name"];
                    var propertyValue = monitoring[i].Row["Property Value"] == DBNull.Value ? null : (string)monitoring[i].Row["Property Value"];
                    var rowIndex = dataGridViewMonitoring.Rows.Add(
                        monitoring[i].Row["Display Name"] == DBNull.Value ? "" : monitoring[i].Row["Display Name"],
                        (propertyValue + " " + monitoring[i].Row["Units"]).Trim(),
                        monitoring[i].Row["Update Date"]);
                    var relevantConditions = monitoringConditions.Where(c => c.PropertyName == propertyName).ToList();
                    if (!relevantConditions.Any()) continue;
                    var conditionFailed = CheckMonitoringConditions(relevantConditions, propertyValue);
                    dataGridViewMonitoring.Rows[rowIndex].DefaultCellStyle.BackColor = conditionFailed ? Color.LightPink : Color.White;
                }
            }
		}

	    private static bool CheckMonitoringConditions(IEnumerable<MonitoringWarningCondition> relevantConditions, string propertyValue)
	    {
	        var conditionSuccess = true;
	        foreach (var condition in relevantConditions)
	        {
	            double propertyValueParsed, conditionBoundParsed;
	            switch (condition.ConditionType)
	            {
	                case ConditionType.Equal:
	                    if (condition.ConditionBound != propertyValue)
                            conditionSuccess = false;
	                    break;
	                case ConditionType.GreaterThen:
	                    if (double.TryParse(propertyValue, out propertyValueParsed) &&
	                        double.TryParse(condition.ConditionBound, out conditionBoundParsed))
	                    {
                            if (propertyValueParsed <= conditionBoundParsed)
                                conditionSuccess = false;
	                    }
	                    break;
	                case ConditionType.LessThan:
	                    if (double.TryParse(propertyValue, out propertyValueParsed) &&
	                        double.TryParse(condition.ConditionBound, out conditionBoundParsed))
	                    {
                            if (propertyValueParsed >= conditionBoundParsed)
                                conditionSuccess = false;
	                    }
	                    break;
	                case ConditionType.GreaterOrEqual:
	                    if (double.TryParse(propertyValue, out propertyValueParsed) &&
	                        double.TryParse(condition.ConditionBound, out conditionBoundParsed))
	                    {
                            if (propertyValueParsed < conditionBoundParsed)
                                conditionSuccess = false;
	                    }
	                    break;
	                case ConditionType.LessOrEqual:
	                    if (double.TryParse(propertyValue, out propertyValueParsed) &&
	                        double.TryParse(condition.ConditionBound, out conditionBoundParsed))
	                    {
                            if (propertyValueParsed > conditionBoundParsed)
                                conditionSuccess = false;
	                    }
	                    break;
	                default:
	                    throw new ArgumentOutOfRangeException();
	            }
	        }
	        return conditionSuccess;
	    }

	    #region IDisposable Members

	    void IDisposable.Dispose()
	    {
	        Db.Dispose();
	    }

	    #endregion

	    private void toolStripButton3_Click(object sender, EventArgs e)
	    {
	        var selectedNodeProperty = (NodeProperty) treeViewComputers.SelectedNode.Tag;
	        Reload();
	        SelectNodeByNodeId(treeViewComputers.Nodes, selectedNodeProperty.NodeId);
	    }

	    private void измененияТекущеToolStripMenuItem_Click(object sender, EventArgs e)
	    {
	        var ai = new ArchiveInfo {DisplayArchiveType = DisplayArchive.NodeChangesArchive};
	        ai.InitializeForm();
	        ai.ShowDialog();
	        ai.Dispose();
	    }

	    private void удаленныеКомпьютерыToolStripMenuItem_Click(object sender, EventArgs e)
	    {
	        var ai = new ArchiveInfo {DisplayArchiveType = DisplayArchive.DeviceChangesArchive};
	        ai.InitializeForm();
	        ai.ShowDialog();
	        ai.Dispose();
	    }

	    private void удаленныеУстройстваToolStripMenuItem_Click(object sender, EventArgs e)
	    {
	        var ai = new ArchiveInfo {DisplayArchiveType = DisplayArchive.DeletedDeviceArchive};
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
	        if (MessageBox.Show(@"Вы действительно хотите выйти из программы?", @"Выход", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
	            Close();
	    }

	    private void splitContainer1_KeyDown(object sender, KeyEventArgs e)
	    {
	        if (e.KeyCode == Keys.Escape)
	            выходToolStripMenuItem_Click(sender, new EventArgs());
	    }

	    private void toolStripButton4_Click(object sender, EventArgs e)
	    {
	        if (((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceNode)
	        {
	            var iif = new InstallationsInfoForm {DeviceId = ((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeId};
	            iif.InitializeForm(Db);
	            iif.ShowDialog();
	            iif.Dispose();
	        }
	        else
	            MessageBox.Show(@"Выбранный узел не является устройством. Выберете устройство (ПК), на котором вы хотели бы просмотреть список установленного ПО", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
	    }

	    private void открытьСписокУстановленногоПОToolStripMenuItem_Click(object sender, EventArgs e)
	    {
	        toolStripButton4_Click(sender, e);
	    }

	    private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	    {
	        if (((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceNode)
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
	        if (((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeType == NodeTypeEnum.DeviceNode)
	        {
	            var rf = new RequestsForm();
	            var deviceId = ((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeId;
	            rf.SerialNumber = Db.GetSerialNumberBy(deviceId, false);
	            rf.InventoryNumber = Db.GetInventoryNumberBy(deviceId, false);
	            rf.InitializeForm(Db);
	            rf.ShowDialog();
	            rf.Dispose();
	        }
	        else
	            MessageBox.Show(@"Выбранный узел не является устройством. Выберете устройство (ПК), на котором вы хотели бы просмотреть список заявок", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
	    }

	    private void toolStripButton6_Click(object sender, EventArgs e)
	    {
	        if (toolStripButton6.Checked)
	        {
	            toolStripButton6.Checked = false;
	            var selectedNodeProperty = (NodeProperty) treeViewComputers.SelectedNode.Tag;
	            Spg = new SearchParametersGroup();
	            Reload();
	            SelectNodeByNodeId(treeViewComputers.Nodes, selectedNodeProperty.NodeId);
	        }
	        else
	        {
	            var sfs = new SearchFormSt1 {Spg = Spg};
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
	            if (nodeProperty.NodeId == nodeId)
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

	    private void GetDevicesInDepartment(TreeNode currentNode, string department, List<Device> devices)
	    {
	        foreach (TreeNode node in currentNode.Nodes)
	        {
	            if (((NodeProperty) node.Tag).NodeType == NodeTypeEnum.DeviceNode)
	            {
	                var dev = new Device
	                {
	                    DeviceId = ((NodeProperty) node.Tag).NodeId, Name = node.Text, Department = department
	                };
	                devices.Add(dev);
	            }
	            else if (((NodeProperty) node.Tag).NodeType == NodeTypeEnum.DepartmentNode)
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
	        if (((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeType != NodeTypeEnum.DeviceNode && ((NodeProperty) treeViewComputers.SelectedNode.Tag).NodeType != NodeTypeEnum.DepartmentNode)
	            return;
	        if (treeViewComputers.SelectedNode.Parent == null)
	            return;
	        if (((NodeProperty) treeViewComputers.SelectedNode.Parent.Tag).NodeId <= 0)
	            return;
	        var node = (TreeNode) treeViewComputers.SelectedNode.Clone();
	        var np = ((NodeProperty) treeViewComputers.SelectedNode.Tag);
	        if (_mcf == null)
	            _mcf = new MoveComputersForm(np);
	        else
	            _mcf.Np = np;
	        _mcf.Text = @"Перемещение узла " + treeViewComputers.SelectedNode.Text;
	        _mcf.Moved = false;
	        _mcf.ShowDialog();
	        if (_mcf.Moved)
	        {
	            if (toolStripButton6.Checked)
	            {
	                Reload(true);
	            }
	            else
	            {
	                //Удалить в старом департаменте узел и добавить в новый
	                treeViewComputers.SelectedNode.Remove();
	                //Добавить узел в новое место
	                TreeNodesHelper.AddNode(node, treeViewComputers.Nodes, treeViewComputers.Nodes, _mcf.NewId);
	                treeViewComputers.Sort();
	            }
	        }
	    }

	    private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
	    {
	        if (treeViewDeviceInfo.SelectedNode.Parent == null || treeViewDeviceInfo.SelectedNode.Parent.Text != @"Периферийные устройства")
	            e.Cancel = true;
	        if (treeViewDeviceInfo.SelectedNode.Text == @"Системный блок")
	            contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
	    }

	    private void contextMenuStrip2_Click(object sender, EventArgs e)
	    {
	        if (treeViewDeviceInfo.SelectedNode.Parent == null || treeViewDeviceInfo.SelectedNode.Parent.Text != @"Периферийные устройства")
	            return;
	        var rf = new RequestsForm();
	        var id = ((NodeProperty) treeViewDeviceInfo.SelectedNode.Tag).NodeId;
	        rf.SerialNumber = Db.GetSerialNumberBy(id, true);
	        rf.InventoryNumber = Db.GetInventoryNumberBy(id, true);
	        rf.InitializeForm(Db);
	        rf.ShowDialog();
	        rf.Dispose();
	    }

	    private void treeViewDeviceInfo_AfterSelect(object sender, TreeViewEventArgs e)
	    {
	        if (treeViewDeviceInfo.SelectedNode.Parent == null || treeViewDeviceInfo.SelectedNode.Parent.Text != @"Периферийные устройства")
	            groupBoxPereferial.Visible = false;
	        else
	        {
	            var id = ((NodeProperty) treeViewDeviceInfo.SelectedNode.Tag).NodeId;
	            var dv = Db.GetDetailDeviceInfo(id);
	            foreach (DataRowView row in dv)
	            {
	                switch ((int) row["ID Node"])
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
	            string department;
	            if (((NodeProperty) node.Tag).NodeType == NodeTypeEnum.DepartmentNode)
	                department = node.Text;
	            else
	                continue;
	            GetDevicesInDepartment(node, department, list);
	        }
	        //Получаем дополнительную информацию об устройствах
	        Db.GetExInfoByDeviceIdList(list);
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
	        var devicesRep = new PeripheryListReporter {ReportTitle = "Список периферийных устройств"};
	        devicesRep.Arguments.Add("ids_types", idsTypes);

	        var list = new List<Device>();
	        var root = treeViewComputers.Nodes;
	        foreach (TreeNode node in root)
	        {
	            string department;
	            if (((NodeProperty) node.Tag).NodeType == NodeTypeEnum.DepartmentNode)
	                department = node.Text;
	            else
	                continue;
	            GetDevicesInDepartment(node, department, list);
	        }
	        var where = "";
	        foreach (var device in list)
	        {
	            where += "," + device.DeviceId;
	        }
	        devicesRep.Arguments.Add("where_devices", where);
	        devicesRep.Run();
	    }

	    private void ManualMenuItem_Click(object sender, EventArgs e)
	    {
	        var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Documentation\Manual.odt");
	        if (!File.Exists(fileName))
	        {
	            MessageBox.Show(string.Format("Не удалось найти руководство пользователя по пути {0}", fileName), @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
	            string department;
	            if (((NodeProperty) node.Tag).NodeType == NodeTypeEnum.DepartmentNode)
	                department = node.Text;
	            else
	                continue;
	            GetDevicesInDepartment(node, department, list);
	        }
	        var where = "";
	        foreach (var device in list)
	        {
	            where += "," + device.DeviceId;
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

        private void toolStripStatusLabelWarning_MouseHover(object sender, EventArgs e)
        {
            toolTipWarning.Show(toolStripStatusLabelWarning.ToolTipText, statusStrip1,
                new Point(toolStripStatusLabelWarning.Bounds.Right,
                    toolStripStatusLabelWarning.Bounds.Top), 5000);
        }
	}
}
