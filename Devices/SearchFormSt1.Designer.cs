namespace Devices
{
	partial class SearchFormSt1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchFormSt1));
            this.comboBoxDevTypes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.treeViewDepartments = new System.Windows.Forms.TreeView();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageNodeProperties = new System.Windows.Forms.TabPage();
            this.buttonAddNodeProperty = new System.Windows.Forms.Button();
            this.dataGridViewNodeProperties = new System.Windows.Forms.DataGridView();
            this.buttonRemoveNodeProperty = new System.Windows.Forms.Button();
            this.tabPageMonitoringProperties = new System.Windows.Forms.TabPage();
            this.buttonAddMonitoringProperty = new System.Windows.Forms.Button();
            this.dataGridViewMonitoringProperties = new System.Windows.Forms.DataGridView();
            this.buttonRemoveMonitoringProperty = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxInventoryNumber = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxSerialNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxDeviceName = new System.Windows.Forms.TextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageNodeProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNodeProperties)).BeginInit();
            this.tabPageMonitoringProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMonitoringProperties)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxDevTypes
            // 
            this.comboBoxDevTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDevTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDevTypes.FormattingEnabled = true;
            this.comboBoxDevTypes.Location = new System.Drawing.Point(3, 25);
            this.comboBoxDevTypes.Name = "comboBoxDevTypes";
            this.comboBoxDevTypes.Size = new System.Drawing.Size(347, 21);
            this.comboBoxDevTypes.TabIndex = 0;
            this.comboBoxDevTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxDevTypes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Тип узла";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Департамент";
            // 
            // treeViewDepartments
            // 
            this.treeViewDepartments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewDepartments.CheckBoxes = true;
            this.treeViewDepartments.Location = new System.Drawing.Point(3, 25);
            this.treeViewDepartments.Name = "treeViewDepartments";
            this.treeViewDepartments.Size = new System.Drawing.Size(351, 424);
            this.treeViewDepartments.TabIndex = 3;
            this.treeViewDepartments.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDepartments_AfterCheck);
            // 
            // buttonSearch
            // 
            this.buttonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearch.Location = new System.Drawing.Point(563, 465);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(75, 23);
            this.buttonSearch.TabIndex = 6;
            this.buttonSearch.Text = "Поиск";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(644, 465);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(8, 7);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewDepartments);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxInventoryNumber);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxSerialNumber);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxDeviceName);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxDevTypes);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(714, 452);
            this.splitContainer1.SplitterDistance = 357;
            this.splitContainer1.TabIndex = 8;
            this.splitContainer1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageNodeProperties);
            this.tabControl1.Controls.Add(this.tabPageMonitoringProperties);
            this.tabControl1.Location = new System.Drawing.Point(3, 176);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(347, 273);
            this.tabControl1.TabIndex = 14;
            // 
            // tabPageNodeProperties
            // 
            this.tabPageNodeProperties.Controls.Add(this.buttonAddNodeProperty);
            this.tabPageNodeProperties.Controls.Add(this.dataGridViewNodeProperties);
            this.tabPageNodeProperties.Controls.Add(this.buttonRemoveNodeProperty);
            this.tabPageNodeProperties.Location = new System.Drawing.Point(4, 22);
            this.tabPageNodeProperties.Name = "tabPageNodeProperties";
            this.tabPageNodeProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNodeProperties.Size = new System.Drawing.Size(339, 247);
            this.tabPageNodeProperties.TabIndex = 0;
            this.tabPageNodeProperties.Text = "Характеристики";
            this.tabPageNodeProperties.UseVisualStyleBackColor = true;
            // 
            // buttonAddNodeProperty
            // 
            this.buttonAddNodeProperty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddNodeProperty.Location = new System.Drawing.Point(179, 218);
            this.buttonAddNodeProperty.Name = "buttonAddNodeProperty";
            this.buttonAddNodeProperty.Size = new System.Drawing.Size(75, 23);
            this.buttonAddNodeProperty.TabIndex = 5;
            this.buttonAddNodeProperty.Text = "+";
            this.buttonAddNodeProperty.UseVisualStyleBackColor = true;
            this.buttonAddNodeProperty.Click += new System.EventHandler(this.buttonAddNodeProperty_Click);
            // 
            // dataGridViewNodeProperties
            // 
            this.dataGridViewNodeProperties.AllowUserToAddRows = false;
            this.dataGridViewNodeProperties.AllowUserToDeleteRows = false;
            this.dataGridViewNodeProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewNodeProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewNodeProperties.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewNodeProperties.Name = "dataGridViewNodeProperties";
            this.dataGridViewNodeProperties.ReadOnly = true;
            this.dataGridViewNodeProperties.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewNodeProperties.Size = new System.Drawing.Size(333, 211);
            this.dataGridViewNodeProperties.TabIndex = 4;
            this.dataGridViewNodeProperties.SelectionChanged += new System.EventHandler(this.dataGridViewNodeProperties_SelectionChanged);
            this.dataGridViewNodeProperties.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewNodeProperties_KeyDown);
            // 
            // buttonRemoveNodeProperty
            // 
            this.buttonRemoveNodeProperty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemoveNodeProperty.Enabled = false;
            this.buttonRemoveNodeProperty.Location = new System.Drawing.Point(258, 218);
            this.buttonRemoveNodeProperty.Name = "buttonRemoveNodeProperty";
            this.buttonRemoveNodeProperty.Size = new System.Drawing.Size(75, 23);
            this.buttonRemoveNodeProperty.TabIndex = 6;
            this.buttonRemoveNodeProperty.Text = "-";
            this.buttonRemoveNodeProperty.UseVisualStyleBackColor = true;
            this.buttonRemoveNodeProperty.Click += new System.EventHandler(this.buttonRemoveNodeProperty_Click);
            // 
            // tabPageMonitoringProperties
            // 
            this.tabPageMonitoringProperties.Controls.Add(this.buttonAddMonitoringProperty);
            this.tabPageMonitoringProperties.Controls.Add(this.dataGridViewMonitoringProperties);
            this.tabPageMonitoringProperties.Controls.Add(this.buttonRemoveMonitoringProperty);
            this.tabPageMonitoringProperties.Location = new System.Drawing.Point(4, 22);
            this.tabPageMonitoringProperties.Name = "tabPageMonitoringProperties";
            this.tabPageMonitoringProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMonitoringProperties.Size = new System.Drawing.Size(339, 247);
            this.tabPageMonitoringProperties.TabIndex = 1;
            this.tabPageMonitoringProperties.Text = "Мониторинг";
            this.tabPageMonitoringProperties.UseVisualStyleBackColor = true;
            // 
            // buttonAddMonitoringProperty
            // 
            this.buttonAddMonitoringProperty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddMonitoringProperty.Location = new System.Drawing.Point(179, 218);
            this.buttonAddMonitoringProperty.Name = "buttonAddMonitoringProperty";
            this.buttonAddMonitoringProperty.Size = new System.Drawing.Size(75, 23);
            this.buttonAddMonitoringProperty.TabIndex = 8;
            this.buttonAddMonitoringProperty.Text = "+";
            this.buttonAddMonitoringProperty.UseVisualStyleBackColor = true;
            this.buttonAddMonitoringProperty.Click += new System.EventHandler(this.buttonAddMonitoringProperty_Click);
            // 
            // dataGridViewMonitoringProperties
            // 
            this.dataGridViewMonitoringProperties.AllowUserToAddRows = false;
            this.dataGridViewMonitoringProperties.AllowUserToDeleteRows = false;
            this.dataGridViewMonitoringProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewMonitoringProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMonitoringProperties.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewMonitoringProperties.Name = "dataGridViewMonitoringProperties";
            this.dataGridViewMonitoringProperties.ReadOnly = true;
            this.dataGridViewMonitoringProperties.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewMonitoringProperties.Size = new System.Drawing.Size(333, 211);
            this.dataGridViewMonitoringProperties.TabIndex = 7;
            this.dataGridViewMonitoringProperties.SelectionChanged += new System.EventHandler(this.dataGridViewMonitoringProperties_SelectionChanged);
            // 
            // buttonRemoveMonitoringProperty
            // 
            this.buttonRemoveMonitoringProperty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemoveMonitoringProperty.Enabled = false;
            this.buttonRemoveMonitoringProperty.Location = new System.Drawing.Point(258, 218);
            this.buttonRemoveMonitoringProperty.Name = "buttonRemoveMonitoringProperty";
            this.buttonRemoveMonitoringProperty.Size = new System.Drawing.Size(75, 23);
            this.buttonRemoveMonitoringProperty.TabIndex = 9;
            this.buttonRemoveMonitoringProperty.Text = "-";
            this.buttonRemoveMonitoringProperty.UseVisualStyleBackColor = true;
            this.buttonRemoveMonitoringProperty.Click += new System.EventHandler(this.buttonRemoveMonitoringProperty_Click);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 134);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Инвентарный номер";
            // 
            // textBoxInventoryNumber
            // 
            this.textBoxInventoryNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInventoryNumber.Location = new System.Drawing.Point(3, 150);
            this.textBoxInventoryNumber.MaxLength = 100;
            this.textBoxInventoryNumber.Name = "textBoxInventoryNumber";
            this.textBoxInventoryNumber.Size = new System.Drawing.Size(346, 20);
            this.textBoxInventoryNumber.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Серийный номер";
            // 
            // textBoxSerialNumber
            // 
            this.textBoxSerialNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSerialNumber.Location = new System.Drawing.Point(3, 108);
            this.textBoxSerialNumber.MaxLength = 100;
            this.textBoxSerialNumber.Name = "textBoxSerialNumber";
            this.textBoxSerialNumber.Size = new System.Drawing.Size(346, 20);
            this.textBoxSerialNumber.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Сетевое имя";
            // 
            // textBoxDeviceName
            // 
            this.textBoxDeviceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDeviceName.Location = new System.Drawing.Point(3, 68);
            this.textBoxDeviceName.MaxLength = 100;
            this.textBoxDeviceName.Name = "textBoxDeviceName";
            this.textBoxDeviceName.Size = new System.Drawing.Size(346, 20);
            this.textBoxDeviceName.TabIndex = 1;
            // 
            // SearchFormSt1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 500);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSearch);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SearchFormSt1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Поиск";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.SearchFormSt1_Load);
            this.Shown += new System.EventHandler(this.SearchFormSt1_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageNodeProperties.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNodeProperties)).EndInit();
            this.tabPageMonitoringProperties.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMonitoringProperties)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxDevTypes;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TreeView treeViewDepartments;
		private System.Windows.Forms.Button buttonSearch;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button buttonRemoveNodeProperty;
		private System.Windows.Forms.Button buttonAddNodeProperty;
		private System.Windows.Forms.DataGridView dataGridViewNodeProperties;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxDeviceName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxSerialNumber;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxInventoryNumber;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageNodeProperties;
        private System.Windows.Forms.TabPage tabPageMonitoringProperties;
        private System.Windows.Forms.Button buttonAddMonitoringProperty;
        private System.Windows.Forms.DataGridView dataGridViewMonitoringProperties;
        private System.Windows.Forms.Button buttonRemoveMonitoringProperty;
	}
}