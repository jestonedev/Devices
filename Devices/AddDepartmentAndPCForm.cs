using System;
using System.Drawing;
using System.Windows.Forms;

namespace Devices
{
	/// <summary>
	/// Класс формы добавления департаментов и устройств
	/// </summary>
	public partial class AddDepartmentAndPcForm : Form, IDisposable
	{
		public TreeNode ParentNode { get; set; }
		private readonly DevicesDatabase _db = new DevicesDatabase();
		public TreeNode CurrentNode { get; set; }

		public void InitializeForm()
		{
			var view = _db.GetDeviceTypes();
			var item = new DeviceTypeComboboxItem("Департамент (отдел)", 0);
			comboBoxDevType.Items.Add(item);
			comboBoxDevType.DisplayMember = "DeviceType";
			for (var i = 0; i < view.Table.Rows.Count; i++)
			{
				comboBoxDevType.Items.Add(new DeviceTypeComboboxItem(view.Table.Rows[i]["Type"].ToString(),
					Convert.ToInt32(view.Table.Rows[i]["ID Device Type"])));
			}
			comboBoxDevType.SelectedIndex = 0;
			//Инициализация формы на проведение изменений данных
		    if (((NodeProperty) CurrentNode.Tag).NodeId < 0) return;
		    Text = @"Изменить";
		    buttonAdd.Text = @"Изменить";
		    if (((NodeProperty)CurrentNode.Tag).NodeType == NodeTypeEnum.DepartmentNode)
		    {
		        textBoxName.Text = _db.GetDepartmentInfo(((NodeProperty)CurrentNode.Tag).NodeId);
		        comboBoxDevType.SelectedIndex = 0;
		    }
		    else
		    {
		        var dv = _db.GetDeviceGeneralInfo(((NodeProperty)CurrentNode.Tag).NodeId);
		        textBoxName.Text = dv[0]["Device Name"].ToString();
		        textBoxDescription.Text = dv[0]["Description"].ToString();
		        textBoxInvenotryNumber.Text = dv[0]["InventoryNumber"].ToString();
		        textBoxSerialNumber.Text = dv[0]["SerialNumber"].ToString();
		        comboBoxDevType.SelectedIndex = comboBoxDevType.FindString(dv[0]["Type"].ToString());
		    }
		    comboBoxDevType.Enabled = false;
		}

		public AddDepartmentAndPcForm()
		{
			InitializeComponent();			
		}

		/// <summary>
		/// Преобразование индекса в combobox в NodeTypeEnum
		/// </summary>
		private NodeTypeEnum ConvertNodeTypeIDToEnumID(int id)
		{
			switch (id)
			{
				case 0: return NodeTypeEnum.DepartmentNode;
				default: return NodeTypeEnum.DeviceNode;
			}
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			if (((NodeProperty)CurrentNode.Tag).NodeId == -1)
			{
				//Добавление нового узла
			    var node = new TreeNode {Text = textBoxName.Text};
			    int id;
				if (ConvertNodeTypeIDToEnumID(((DeviceTypeComboboxItem)comboBoxDevType.SelectedItem).DeviceTypeId) == NodeTypeEnum.DepartmentNode)
					id = _db.InsertDepartment(textBoxName.Text, ((NodeProperty)ParentNode.Tag).NodeId);
				else
					id = _db.InsertDevice(textBoxName.Text, textBoxInvenotryNumber.Text, textBoxSerialNumber.Text, textBoxDescription.Text,
						((NodeProperty)ParentNode.Tag).NodeId, ((DeviceTypeComboboxItem)comboBoxDevType.SelectedItem).DeviceTypeId);
				if (id == -1)
				{
					return;
				}
				node.Tag = new NodeProperty(id, ConvertNodeTypeIDToEnumID(((DeviceTypeComboboxItem)comboBoxDevType.SelectedItem).DeviceTypeId));
				ParentNode.Nodes.Add(node);
				if (((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DeviceNode)
				{
					var tmpNode = node.Parent;
					while (tmpNode != null)
					{
						tmpNode.ForeColor = Color.DarkBlue;
						tmpNode = tmpNode.Parent;
					}
				}
				ParentNode.Expand(); 
			}
			else
			{
				//Обновление существующего узла
				bool isComplete;
				if (ConvertNodeTypeIDToEnumID(((DeviceTypeComboboxItem)comboBoxDevType.SelectedItem).DeviceTypeId) == NodeTypeEnum.DepartmentNode)
					isComplete = _db.UpdateDepartment(((NodeProperty)CurrentNode.Tag).NodeId, textBoxName.Text);
				else
					isComplete = _db.UpdateDevice(((NodeProperty)CurrentNode.Tag).NodeId, textBoxName.Text,
						textBoxInvenotryNumber.Text, textBoxSerialNumber.Text, textBoxDescription.Text);
				if (isComplete)
					CurrentNode.Text = textBoxName.Text;
			}
            DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
            DialogResult = DialogResult.Cancel;
			Close();
		}

		private void textBoxName_TextChanged(object sender, EventArgs e)
		{
			buttonAdd.Enabled = textBoxName.Text.Length > 0;
		}

		#region IDisposable Members

		/// <summary>
		/// Дисконект от базы данных при закрытии формы
		/// </summary>
		void IDisposable.Dispose()
		{
			_db.Dispose();
		}

		#endregion

		private void comboBoxDevType_SelectedIndexChanged(object sender, EventArgs e)
		{
			var vis = comboBoxDevType.SelectedIndex != 0;
			label3.Visible = vis;
			label4.Visible = vis;
			label5.Visible = vis;
			textBoxSerialNumber.Visible = vis;
			textBoxInvenotryNumber.Visible = vis;
			textBoxDescription.Visible = vis;
			if (vis) {
				Height = 330;
				label2.Text = @"Сетевое имя узла";
			}
			else {
				Height = 145;
				label2.Text = @"Наименование департамента";
			}
		}

		private void comboBoxDevType_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
			else
				if ((e.KeyCode == Keys.Enter) && buttonAdd.Enabled)
					buttonAdd_Click(sender, new EventArgs());
		}

		private void textBoxDescription_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}
	}

	/// <summary>
	/// Специализированный класс элемента combobox
	/// </summary>
	public class DeviceTypeComboboxItem
	{
		public string DeviceType { get; set; }
		public int DeviceTypeId { get; set; }

		public DeviceTypeComboboxItem(string deviceType, int deviceTypeId)
		{
			DeviceType = deviceType;
			DeviceTypeId = deviceTypeId;
		}
	}
}
