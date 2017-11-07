using System;
using System.Windows.Forms;

namespace Devices
{
	public partial class ArchiveInfo : Form
	{
		public DisplayArchive DisplayArchiveType { get; set; }
		public  ArchiveInfo()
		{
			InitializeComponent();
		}

		public void InitializeForm()
		{
			toolStripStatusLabel1.Text = "Загрузка данных...";
			statusStrip1.Refresh();
			DevicesDatabase dd = new DevicesDatabase();
			switch (DisplayArchiveType)
			{
				case DisplayArchive.DeviceChangesArchive:
					dataGridView1.DataSource = dd.GetArchiveDeviceInfo();
					dataGridView1.Columns["ID Device"].Visible = false;
					dataGridView1.Columns["Department"].HeaderText = "Подразделение";
					dataGridView1.Columns["Type"].HeaderText = "Тип устройства";
					dataGridView1.Columns["Device Name"].HeaderText = "Системное имя";
					dataGridView1.Columns["SerialNumber"].HeaderText = "Серийный номер";
					dataGridView1.Columns["InventoryNumber"].HeaderText = "Инвентарный номер";
					dataGridView1.Columns["Description"].HeaderText = "Комментарий";
					dataGridView1.Columns["Owner"].HeaderText = "Оператор, добавивший устройство";
					dataGridView1.Columns["Operation"].HeaderText = "Операция";
					dataGridView1.Columns["Date"].HeaderText = "Дата операции";
					dataGridView1.Columns["Operator"].HeaderText = "Оператор, внесший изменения";
					dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
					break;
				case DisplayArchive.NodeChangesArchive:
					dataGridView1.DataSource = dd.GetArchiveNodesInfo();
					dataGridView1.Columns["ID Node"].Visible = false;
					dataGridView1.Columns["Device Name"].HeaderText = "Системное имя";
					dataGridView1.Columns["Parameter Name"].HeaderText = "Характеристика";
					dataGridView1.Columns["Value"].HeaderText = "Значение";
					dataGridView1.Columns["Operation"].HeaderText = "Операция";
					dataGridView1.Columns["Date"].HeaderText = "Дата операции";
					dataGridView1.Columns["Operator"].HeaderText = "Оператор, внесший изменения";
					dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
					break;
				case DisplayArchive.DeletedDeviceArchive:
					dataGridView1.DataSource = dd.GetArchiveDeletedDevicesInfoInfo();
					dataGridView1.Columns["ID Device"].Visible = false;
					dataGridView1.Columns["Department"].HeaderText = "Подразделение";
					dataGridView1.Columns["Type"].HeaderText = "Тип устройства";
					dataGridView1.Columns["Device Name"].HeaderText = "Системное имя";
					dataGridView1.Columns["SerialNumber"].HeaderText = "Серийный номер";
					dataGridView1.Columns["InventoryNumber"].HeaderText = "Инвентарный номер";
					dataGridView1.Columns["Description"].HeaderText = "Комментарий";
					dataGridView1.Columns["Owner"].HeaderText = "Оператор, добавивший устройство";
					dataGridView1.Columns["Date"].HeaderText = "Дата операции";
					dataGridView1.Columns["Operator"].HeaderText = "Оператор, внесший изменения";
					dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
					break;
			}
			dd.Dispose();
			toolStripStatusLabel1.Text = "Выполнено";
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			InitializeForm();
		}

		private void OpenDeviceDetails()
		{
			if (DisplayArchiveType == DisplayArchive.DeletedDeviceArchive)
			{
				ComputerInfo compForm = new ComputerInfo();
				compForm.DeviceID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID Device"].Value.ToString());
				compForm.searchInArchive = true;
				compForm.FillInfoTree();
				compForm.ShowDialog();
			}
		}

		private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			OpenDeviceDetails();
		}

		private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
			else
				if (e.KeyCode == Keys.Enter)
				{
					OpenDeviceDetails();
					e.Handled = true;
				}
				else
					if (e.KeyCode == Keys.F5)
						InitializeForm();
		}
	}

	public enum DisplayArchive { DeviceChangesArchive, NodeChangesArchive, DeletedDeviceArchive }
}
