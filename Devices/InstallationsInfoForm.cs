﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Devices
{
	public partial class InstallationsInfoForm : Form
	{
		public int DeviceID { get; set; }

		public InstallationsInfoForm()
		{
			InitializeComponent();
		}

		internal void InitializeForm(DevicesDatabase db)
		{	
			DataView dv = db.GetInstallationsByComputerID(DeviceID);
			if (dv.Table == null)
				return;
			dataGridView1.DataSource = dv;
			dataGridView1.Columns["ID Installation"].Visible = false;
			dataGridView1.Columns["ID Computer"].Visible = false;
			dataGridView1.Columns["Software"].HeaderText = "Программное обеспечение";
			dataGridView1.Columns["Version"].HeaderText = "Версия";
			dataGridView1.Columns["installationDate"].HeaderText = "Дата установки ПО";
			dataGridView1.Columns["LicenseType"].HeaderText = "Вид лицензии";
			dataGridView1.Columns["BuyLicenseDate"].HeaderText = "Дата приобретения лицензии";
			dataGridView1.Columns["ExpireLicenseDate"].HeaderText = "Дата окончания лицензии";
			dataGridView1.Columns["LicenseKey"].HeaderText = "Лицензионный ключ";
			dataGridView1.Columns["SoftwareMaker"].HeaderText = "Разработчик ПО";
			dataGridView1.Columns["Supplier"].HeaderText = "Поставщик ПО";
			dataGridView1.Columns["SoftwareType"].HeaderText = "Вид ПО";
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			toolStripStatusLabel1.Text = "Всего установок: " + dataGridView1.RowCount.ToString();
		}

		private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}
	}
}