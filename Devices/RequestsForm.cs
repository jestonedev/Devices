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
	public partial class RequestsForm : Form
	{
		public int DeviceID { get; set; }

		public RequestsForm()
		{
			InitializeComponent();
		}

		internal void InitializeForm(DevicesDatabase db)
		{
			DataView dv = db.GetRequestsBySerialNumber(DeviceID);
			if (dv.Table == null)
				return;
			dataGridView1.DataSource = dv;
			dataGridView1.Columns["Kod_Sotr"].Visible = false;
			dataGridView1.Columns["rec_date"].Visible = false;
			dataGridView1.Columns["id_job"].Visible = false;
			dataGridView1.Columns["IsOtv"].Visible = false;
			dataGridView1.Columns["login"].Visible = false;
			dataGridView1.Columns["amount"].Visible = false;
			dataGridView1.Columns["act"].Visible = false;
			dataGridView1.Columns["num"].HeaderText = "№";
			dataGridView1.Columns["Sotr"].HeaderText = "Сотрудник";
			dataGridView1.Columns["Fault"].HeaderText = "Неисправность";
			dataGridView1.Columns["Recom"].HeaderText = "Рекомендация";
			dataGridView1.Columns["Action_Rem"].HeaderText = "Выполненные работы";
			dataGridView1.Columns["D_Rem"].HeaderText = "Дата выполнения";
			dataGridView1.Columns["Prim"].HeaderText = "Примечание";
			dataGridView1.Columns["hh"].HeaderText = "Часы";
			dataGridView1.Columns["mm"].HeaderText = "Минуты";
			dataGridView1.Columns["j_name"].HeaderText = "Вид работы";
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			toolStripStatusLabel1.Text = "Всего заявок: " + dataGridView1.RowCount.ToString();
		}
	}
}
