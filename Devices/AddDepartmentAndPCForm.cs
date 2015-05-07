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
	/// <summary>
	/// Класс формы добавления департаментов и устройств
	/// </summary>
	public partial class AddDepartmentAndPCForm : Form, IDisposable
	{
		public TreeNode parentNode { get; set; }
		private DevicesDatabase db = new DevicesDatabase();
		public TreeNode currentNode { get; set; }

		public void InitializeForm()
		{
			DataView view = db.GetDeviceTypes();
			DeviceTypeComboboxItem item = new DeviceTypeComboboxItem("Департамент (отдел)", 0);
			comboBoxDevType.Items.Add(item);
			comboBoxDevType.DisplayMember = "DeviceType";
			for (int i = 0; i < view.Table.Rows.Count; i++)
			{
				comboBoxDevType.Items.Add(new DeviceTypeComboboxItem(view.Table.Rows[i]["Type"].ToString(),
					Convert.ToInt32(view.Table.Rows[i]["ID Device Type"])));
			}
			comboBoxDevType.SelectedIndex = 0;
			//Инициализация формы на проведение изменений данных
			if (((NodeProperty)currentNode.Tag).NodeID >= 0)
			{
				this.Text = "Изменить";
				buttonAdd.Text = "Изменить";
				if (((NodeProperty)currentNode.Tag).NodeType == NodeTypeEnum.DepartmentNode)
				{
					textBoxName.Text = db.GetDepartmentInfo(((NodeProperty)currentNode.Tag).NodeID);
					comboBoxDevType.SelectedIndex = 0;
				}
				else
				{
					DataView dv = db.GetDeviceGeneralInfo(((NodeProperty)currentNode.Tag).NodeID);
					textBoxName.Text = dv[0]["Device Name"].ToString();
					textBoxDescription.Text = dv[0]["Description"].ToString();
					textBoxInvenotryNumber.Text = dv[0]["InventoryNumber"].ToString();
					textBoxSerialNumber.Text = dv[0]["SerialNumber"].ToString();
					comboBoxDevType.SelectedIndex = comboBoxDevType.FindString(dv[0]["Type"].ToString());
				}
				comboBoxDevType.Enabled = false;
			}
		}

		public AddDepartmentAndPCForm()
		{
			InitializeComponent();			
		}

		/// <summary>
		/// Преобразование индекса в combobox в NodeTypeEnum
		/// </summary>
		private NodeTypeEnum ConvertNodeTypeIDToEnumID(int ID)
		{
			switch (ID)
			{
				case 0: return NodeTypeEnum.DepartmentNode;
				default: return NodeTypeEnum.DeviceNode;
			}
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			if (((NodeProperty)currentNode.Tag).NodeID == -1)
			{
				//Добавление нового узла
				TreeNode node = new TreeNode();
				node.Text = textBoxName.Text;
				int id = -1;
				if (ConvertNodeTypeIDToEnumID(((DeviceTypeComboboxItem)comboBoxDevType.SelectedItem).DeviceTypeID) == NodeTypeEnum.DepartmentNode)
					id = db.InsertDepartment(textBoxName.Text, ((NodeProperty)parentNode.Tag).NodeID);
				else
					id = db.InsertDevice(textBoxName.Text, textBoxInvenotryNumber.Text, textBoxSerialNumber.Text, textBoxDescription.Text,
						((NodeProperty)parentNode.Tag).NodeID, ((DeviceTypeComboboxItem)comboBoxDevType.SelectedItem).DeviceTypeID);
				if (id == -1)
				{
					return;
				}
				node.Tag = new NodeProperty(id, ConvertNodeTypeIDToEnumID(((DeviceTypeComboboxItem)comboBoxDevType.SelectedItem).DeviceTypeID));
				parentNode.Nodes.Add(node);
				if (((NodeProperty)node.Tag).NodeType == NodeTypeEnum.DeviceNode)
				{
					TreeNode tmp_node = node.Parent;
					while (tmp_node != null)
					{
						tmp_node.ForeColor = Color.DarkBlue;
						tmp_node = tmp_node.Parent;
					}
				}
				parentNode.Expand(); 
			}
			else
			{
				//Обновление существующего узла
				bool is_complete = false;
				if (ConvertNodeTypeIDToEnumID(((DeviceTypeComboboxItem)comboBoxDevType.SelectedItem).DeviceTypeID) == NodeTypeEnum.DepartmentNode)
					is_complete = db.UpdateDepartment(((NodeProperty)currentNode.Tag).NodeID, textBoxName.Text);
				else
					is_complete = db.UpdateDevice(((NodeProperty)currentNode.Tag).NodeID, textBoxName.Text,
						textBoxInvenotryNumber.Text, textBoxSerialNumber.Text, textBoxDescription.Text);
				if (is_complete)
					currentNode.Text = textBoxName.Text;
			}
            DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
			db.Dispose();
		}

		#endregion

		private void comboBoxDevType_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool vis = comboBoxDevType.SelectedIndex != 0;
			label3.Visible = vis;
			label4.Visible = vis;
			label5.Visible = vis;
			textBoxSerialNumber.Visible = vis;
			textBoxInvenotryNumber.Visible = vis;
			textBoxDescription.Visible = vis;
			if (vis) {
				this.Height = 330;
				label2.Text = "Сетевое имя узла";
			}
			else {
				this.Height = 145;
				label2.Text = "Наименование департамента";
			}
		}

		private void comboBoxDevType_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				this.Close();
			else
				if ((e.KeyCode == Keys.Enter) && (buttonAdd.Enabled == true))
					buttonAdd_Click(sender, new EventArgs());
		}

		private void textBoxDescription_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				this.Close();
		}
	}

	/// <summary>
	/// Специализированный класс элемента combobox
	/// </summary>
	public class DeviceTypeComboboxItem
	{
		public string DeviceType { get; set; }
		public int DeviceTypeID { get; set; }

		public DeviceTypeComboboxItem(string DeviceType, int DeviceTypeID)
		{
			this.DeviceType = DeviceType;
			this.DeviceTypeID = DeviceTypeID;
		}
	}
}
