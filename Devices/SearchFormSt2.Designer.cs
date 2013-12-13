namespace Devices
{
	partial class SearchFormSt2
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
			this.treeViewDeviceInfo = new System.Windows.Forms.TreeView();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxDeviceParameters = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBoxOperator = new System.Windows.Forms.ComboBox();
			this.textBoxValue = new System.Windows.Forms.TextBox();
			this.buttonAccept = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.numericUpDownValue = new System.Windows.Forms.NumericUpDown();
			this.comboBoxValue = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue)).BeginInit();
			this.SuspendLayout();
			// 
			// treeViewDeviceInfo
			// 
			this.treeViewDeviceInfo.Location = new System.Drawing.Point(12, 25);
			this.treeViewDeviceInfo.Name = "treeViewDeviceInfo";
			this.treeViewDeviceInfo.Size = new System.Drawing.Size(488, 321);
			this.treeViewDeviceInfo.TabIndex = 0;
			this.treeViewDeviceInfo.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDeviceInfo_AfterSelect);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Устройство";
			// 
			// comboBoxDeviceParameters
			// 
			this.comboBoxDeviceParameters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDeviceParameters.FormattingEnabled = true;
			this.comboBoxDeviceParameters.Location = new System.Drawing.Point(12, 368);
			this.comboBoxDeviceParameters.Name = "comboBoxDeviceParameters";
			this.comboBoxDeviceParameters.Size = new System.Drawing.Size(190, 21);
			this.comboBoxDeviceParameters.TabIndex = 1;
			this.comboBoxDeviceParameters.SelectedIndexChanged += new System.EventHandler(this.comboBoxDeviceParameters_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(66, 352);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Характеристика";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(226, 352);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(57, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Операция";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(369, 352);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(55, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Значение";
			// 
			// comboBoxOperator
			// 
			this.comboBoxOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxOperator.FormattingEnabled = true;
			this.comboBoxOperator.Location = new System.Drawing.Point(208, 368);
			this.comboBoxOperator.Name = "comboBoxOperator";
			this.comboBoxOperator.Size = new System.Drawing.Size(96, 21);
			this.comboBoxOperator.TabIndex = 2;
			// 
			// textBoxValue
			// 
			this.textBoxValue.Location = new System.Drawing.Point(310, 368);
			this.textBoxValue.Name = "textBoxValue";
			this.textBoxValue.Size = new System.Drawing.Size(190, 20);
			this.textBoxValue.TabIndex = 7;
			this.textBoxValue.TextChanged += new System.EventHandler(this.textBoxValue_TextChanged);
			// 
			// buttonAccept
			// 
			this.buttonAccept.Enabled = false;
			this.buttonAccept.Location = new System.Drawing.Point(344, 394);
			this.buttonAccept.Name = "buttonAccept";
			this.buttonAccept.Size = new System.Drawing.Size(75, 23);
			this.buttonAccept.TabIndex = 4;
			this.buttonAccept.Text = "Добавить";
			this.buttonAccept.UseVisualStyleBackColor = true;
			this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(425, 395);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 5;
			this.button2.Text = "Отменить";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// numericUpDownValue
			// 
			this.numericUpDownValue.Location = new System.Drawing.Point(310, 368);
			this.numericUpDownValue.Maximum = new decimal(new int[] {
            -1794967296,
            0,
            0,
            0});
			this.numericUpDownValue.Name = "numericUpDownValue";
			this.numericUpDownValue.Size = new System.Drawing.Size(190, 20);
			this.numericUpDownValue.TabIndex = 10;
			this.numericUpDownValue.Visible = false;
			// 
			// comboBoxValue
			// 
			this.comboBoxValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxValue.FormattingEnabled = true;
			this.comboBoxValue.Location = new System.Drawing.Point(310, 368);
			this.comboBoxValue.Name = "comboBoxValue";
			this.comboBoxValue.Size = new System.Drawing.Size(190, 21);
			this.comboBoxValue.TabIndex = 3;
			this.comboBoxValue.Visible = false;
			// 
			// SearchFormSt2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(512, 424);
			this.Controls.Add(this.comboBoxValue);
			this.Controls.Add(this.numericUpDownValue);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.buttonAccept);
			this.Controls.Add(this.textBoxValue);
			this.Controls.Add(this.comboBoxOperator);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxDeviceParameters);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.treeViewDeviceInfo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "SearchFormSt2";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Добавить характеристику узла";
			this.Load += new System.EventHandler(this.SearchFormSt2_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView treeViewDeviceInfo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxDeviceParameters;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboBoxOperator;
		private System.Windows.Forms.TextBox textBoxValue;
		private System.Windows.Forms.Button buttonAccept;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.NumericUpDown numericUpDownValue;
		private System.Windows.Forms.ComboBox comboBoxValue;
	}
}