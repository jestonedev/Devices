namespace Devices
{
	partial class RequestsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RequestsForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxDescription = new System.Windows.Forms.GroupBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.groupBoxActionRem = new System.Windows.Forms.GroupBox();
            this.textBoxActionRem = new System.Windows.Forms.TextBox();
            this.groupBoxRecom = new System.Windows.Forms.GroupBox();
            this.textBoxRecom = new System.Windows.Forms.TextBox();
            this.groupBoxFault = new System.Windows.Forms.GroupBox();
            this.textBoxFault = new System.Windows.Forms.TextBox();
            this.NumRequestColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExecutorColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JobTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FixDateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HoursDurColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinutesDurColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StikerColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxDescription.SuspendLayout();
            this.groupBoxActionRem.SuspendLayout();
            this.groupBoxRecom.SuspendLayout();
            this.groupBoxFault.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 540);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabel1.Text = "Всего установок: 0";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NumRequestColumn,
            this.ExecutorColumn,
            this.JobTypeColumn,
            this.FixDateColumn,
            this.HoursDurColumn,
            this.MinutesDurColumn,
            this.StikerColumn,
            this.ActColumn});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new System.Drawing.Size(784, 300);
            this.dataGridView1.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(784, 540);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 5;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBoxDescription, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxActionRem, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxRecom, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxFault, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 236);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBoxDescription
            // 
            this.groupBoxDescription.Controls.Add(this.textBoxDescription);
            this.groupBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDescription.Location = new System.Drawing.Point(395, 121);
            this.groupBoxDescription.Name = "groupBoxDescription";
            this.groupBoxDescription.Size = new System.Drawing.Size(386, 112);
            this.groupBoxDescription.TabIndex = 4;
            this.groupBoxDescription.TabStop = false;
            this.groupBoxDescription.Text = "Примечание";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(3, 16);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.Size = new System.Drawing.Size(380, 93);
            this.textBoxDescription.TabIndex = 0;
            // 
            // groupBoxActionRem
            // 
            this.groupBoxActionRem.Controls.Add(this.textBoxActionRem);
            this.groupBoxActionRem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxActionRem.Location = new System.Drawing.Point(3, 121);
            this.groupBoxActionRem.Name = "groupBoxActionRem";
            this.groupBoxActionRem.Size = new System.Drawing.Size(386, 112);
            this.groupBoxActionRem.TabIndex = 3;
            this.groupBoxActionRem.TabStop = false;
            this.groupBoxActionRem.Text = "Выполненные работы";
            // 
            // textBoxActionRem
            // 
            this.textBoxActionRem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxActionRem.Location = new System.Drawing.Point(3, 16);
            this.textBoxActionRem.Multiline = true;
            this.textBoxActionRem.Name = "textBoxActionRem";
            this.textBoxActionRem.ReadOnly = true;
            this.textBoxActionRem.Size = new System.Drawing.Size(380, 93);
            this.textBoxActionRem.TabIndex = 0;
            // 
            // groupBoxRecom
            // 
            this.groupBoxRecom.Controls.Add(this.textBoxRecom);
            this.groupBoxRecom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxRecom.Location = new System.Drawing.Point(395, 3);
            this.groupBoxRecom.Name = "groupBoxRecom";
            this.groupBoxRecom.Size = new System.Drawing.Size(386, 112);
            this.groupBoxRecom.TabIndex = 2;
            this.groupBoxRecom.TabStop = false;
            this.groupBoxRecom.Text = "Рекомендации";
            // 
            // textBoxRecom
            // 
            this.textBoxRecom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxRecom.Location = new System.Drawing.Point(3, 16);
            this.textBoxRecom.Multiline = true;
            this.textBoxRecom.Name = "textBoxRecom";
            this.textBoxRecom.ReadOnly = true;
            this.textBoxRecom.Size = new System.Drawing.Size(380, 93);
            this.textBoxRecom.TabIndex = 0;
            // 
            // groupBoxFault
            // 
            this.groupBoxFault.Controls.Add(this.textBoxFault);
            this.groupBoxFault.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxFault.Location = new System.Drawing.Point(3, 3);
            this.groupBoxFault.Name = "groupBoxFault";
            this.groupBoxFault.Size = new System.Drawing.Size(386, 112);
            this.groupBoxFault.TabIndex = 1;
            this.groupBoxFault.TabStop = false;
            this.groupBoxFault.Text = "Выявленные неисправности";
            // 
            // textBoxFault
            // 
            this.textBoxFault.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxFault.Location = new System.Drawing.Point(3, 16);
            this.textBoxFault.Multiline = true;
            this.textBoxFault.Name = "textBoxFault";
            this.textBoxFault.ReadOnly = true;
            this.textBoxFault.Size = new System.Drawing.Size(380, 93);
            this.textBoxFault.TabIndex = 0;
            // 
            // NumRequestColumn
            // 
            this.NumRequestColumn.HeaderText = "№";
            this.NumRequestColumn.MinimumWidth = 50;
            this.NumRequestColumn.Name = "NumRequestColumn";
            this.NumRequestColumn.ReadOnly = true;
            this.NumRequestColumn.Width = 50;
            // 
            // ExecutorColumn
            // 
            this.ExecutorColumn.HeaderText = "Исполнитель";
            this.ExecutorColumn.MinimumWidth = 150;
            this.ExecutorColumn.Name = "ExecutorColumn";
            this.ExecutorColumn.ReadOnly = true;
            this.ExecutorColumn.Width = 150;
            // 
            // JobTypeColumn
            // 
            this.JobTypeColumn.HeaderText = "Вид работы";
            this.JobTypeColumn.MinimumWidth = 200;
            this.JobTypeColumn.Name = "JobTypeColumn";
            this.JobTypeColumn.ReadOnly = true;
            this.JobTypeColumn.Width = 200;
            // 
            // FixDateColumn
            // 
            this.FixDateColumn.HeaderText = "Дата ремонта";
            this.FixDateColumn.MinimumWidth = 150;
            this.FixDateColumn.Name = "FixDateColumn";
            this.FixDateColumn.ReadOnly = true;
            this.FixDateColumn.Width = 150;
            // 
            // HoursDurColumn
            // 
            this.HoursDurColumn.HeaderText = "Часы";
            this.HoursDurColumn.MinimumWidth = 100;
            this.HoursDurColumn.Name = "HoursDurColumn";
            this.HoursDurColumn.ReadOnly = true;
            // 
            // MinutesDurColumn
            // 
            this.MinutesDurColumn.HeaderText = "Минуты";
            this.MinutesDurColumn.MinimumWidth = 100;
            this.MinutesDurColumn.Name = "MinutesDurColumn";
            this.MinutesDurColumn.ReadOnly = true;
            // 
            // StikerColumn
            // 
            this.StikerColumn.HeaderText = "Стикер";
            this.StikerColumn.Name = "StikerColumn";
            this.StikerColumn.ReadOnly = true;
            this.StikerColumn.Width = 68;
            // 
            // ActColumn
            // 
            this.ActColumn.HeaderText = "№ акта";
            this.ActColumn.MinimumWidth = 100;
            this.ActColumn.Name = "ActColumn";
            this.ActColumn.ReadOnly = true;
            // 
            // RequestsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RequestsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Заявки";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBoxDescription.ResumeLayout(false);
            this.groupBoxDescription.PerformLayout();
            this.groupBoxActionRem.ResumeLayout(false);
            this.groupBoxActionRem.PerformLayout();
            this.groupBoxRecom.ResumeLayout(false);
            this.groupBoxRecom.PerformLayout();
            this.groupBoxFault.ResumeLayout(false);
            this.groupBoxFault.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBoxFault;
        private System.Windows.Forms.GroupBox groupBoxRecom;
        private System.Windows.Forms.TextBox textBoxRecom;
        private System.Windows.Forms.TextBox textBoxFault;
        private System.Windows.Forms.GroupBox groupBoxActionRem;
        private System.Windows.Forms.TextBox textBoxActionRem;
        private System.Windows.Forms.GroupBox groupBoxDescription;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumRequestColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExecutorColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn JobTypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FixDateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn HoursDurColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinutesDurColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StikerColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActColumn;
	}
}