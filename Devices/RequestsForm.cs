using System.Data;
using System.Windows.Forms;

namespace Devices
{
	public partial class RequestsForm : Form
	{
        public string SerialNumber { get; set; }
        public string InventoryNumber { get; set; }

		public RequestsForm()
		{
			InitializeComponent();
		}

		internal void InitializeForm(DevicesDatabase db)
		{
			DataView dv = db.GetRequests(SerialNumber, InventoryNumber);
			if (dv.Table == null)
				return;
            dataGridView1.AutoGenerateColumns = false;
			dataGridView1.DataSource = dv;

            NumRequestColumn.DataPropertyName = "num";
            ExecutorColumn.DataPropertyName = "Sotr";
            FixDateColumn.DataPropertyName = "D_Rem";
            HoursDurColumn.DataPropertyName = "hh";
            MinutesDurColumn.DataPropertyName = "mm";
            JobTypeColumn.DataPropertyName = "j_name";
            ActColumn.DataPropertyName = "act";
            StikerColumn.DataPropertyName = "Stiker";
            textBoxFault.DataBindings.Add("Text", dv, "Fault");
            textBoxRecom.DataBindings.Add("Text", dv, "Recom");
            textBoxActionRem.DataBindings.Add("Text", dv, "Action_Rem");
            textBoxDescription.DataBindings.Add("Text", dv, "Prim");

			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			toolStripStatusLabel1.Text = "Всего заявок: " + dataGridView1.RowCount.ToString();
		}
	}
}
