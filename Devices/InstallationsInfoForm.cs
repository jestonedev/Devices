using System.Windows.Forms;

namespace Devices
{
	public partial class InstallationsInfoForm : Form
	{
		public int DeviceId { get; set; }

		public InstallationsInfoForm()
		{
			InitializeComponent();
		}

		internal void InitializeForm(DevicesDatabase db)
		{	
			var dv = db.GetInstallationsByComputerId(DeviceId);
			if (dv.Table == null)
				return;
			dataGridView1.DataSource = dv;
			dataGridView1.Columns["ID Installation"].Visible = false;
			dataGridView1.Columns["ID Computer"].Visible = false;
			dataGridView1.Columns["Software"].HeaderText = @"Программное обеспечение";
			dataGridView1.Columns["Version"].HeaderText = @"Версия";
			dataGridView1.Columns["InstallationDate"].HeaderText = @"Дата установки ПО";
			dataGridView1.Columns["LicType"].HeaderText = @"Вид лицензии";
			dataGridView1.Columns["BuyLicenseDate"].HeaderText = @"Дата приобретения лицензии";
			dataGridView1.Columns["ExpireLicenseDate"].HeaderText = @"Дата окончания лицензии";
			dataGridView1.Columns["LicKey"].HeaderText = @"Лицензионный ключ";
			dataGridView1.Columns["SoftMaker"].HeaderText = @"Разработчик ПО";
			dataGridView1.Columns["Supplier"].HeaderText = @"Поставщик ПО";
			dataGridView1.Columns["SoftType"].HeaderText = @"Вид ПО";
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			toolStripStatusLabel1.Text = @"Всего установок: " + dataGridView1.RowCount;
		}

		private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}
	}
}
