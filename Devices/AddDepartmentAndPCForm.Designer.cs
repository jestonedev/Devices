namespace Devices
{
	partial class AddDepartmentAndPCForm
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
			this.comboBoxDevType = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxSerialNumber = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxInvenotryNumber = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxDescription = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// comboBoxDevType
			// 
			this.comboBoxDevType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxDevType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDevType.FormattingEnabled = true;
			this.comboBoxDevType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.comboBoxDevType.Location = new System.Drawing.Point(11, 21);
			this.comboBoxDevType.Name = "comboBoxDevType";
			this.comboBoxDevType.Size = new System.Drawing.Size(296, 21);
			this.comboBoxDevType.TabIndex = 0;
			this.comboBoxDevType.SelectedIndexChanged += new System.EventHandler(this.comboBoxDevType_SelectedIndexChanged);
			this.comboBoxDevType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxDevType_KeyDown);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Тип добавляемого узла";
			// 
			// textBoxName
			// 
			this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxName.Location = new System.Drawing.Point(11, 63);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(296, 20);
			this.textBoxName.TabIndex = 1;
			this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
			this.textBoxName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxDevType_KeyDown);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Наименование";
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonAdd.Enabled = false;
			this.buttonAdd.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.buttonAdd.Location = new System.Drawing.Point(85, 277);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(75, 23);
			this.buttonAdd.TabIndex = 5;
			this.buttonAdd.Text = "Добавить";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			this.buttonAdd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxDescription_KeyDown);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonCancel.Location = new System.Drawing.Point(166, 277);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Отмена";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			this.buttonCancel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxDescription_KeyDown);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 86);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(93, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Серийный номер";
			// 
			// textBoxSerialNumber
			// 
			this.textBoxSerialNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxSerialNumber.Location = new System.Drawing.Point(11, 102);
			this.textBoxSerialNumber.Name = "textBoxSerialNumber";
			this.textBoxSerialNumber.Size = new System.Drawing.Size(296, 20);
			this.textBoxSerialNumber.TabIndex = 2;
			this.textBoxSerialNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxDevType_KeyDown);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 126);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(117, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Инвернтарный номер";
			// 
			// textBoxInvenotryNumber
			// 
			this.textBoxInvenotryNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxInvenotryNumber.Location = new System.Drawing.Point(11, 142);
			this.textBoxInvenotryNumber.Name = "textBoxInvenotryNumber";
			this.textBoxInvenotryNumber.Size = new System.Drawing.Size(296, 20);
			this.textBoxInvenotryNumber.TabIndex = 3;
			this.textBoxInvenotryNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxDevType_KeyDown);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 168);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(144, 13);
			this.label5.TabIndex = 11;
			this.label5.Text = "Дополнительное описание";
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxDescription.Location = new System.Drawing.Point(11, 184);
			this.textBoxDescription.Multiline = true;
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.Size = new System.Drawing.Size(296, 87);
			this.textBoxDescription.TabIndex = 4;
			this.textBoxDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxDescription_KeyDown);
			// 
			// AddDepartmentAndPCForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(319, 306);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textBoxDescription);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBoxInvenotryNumber);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxSerialNumber);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxDevType);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "AddDepartmentAndPCForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Добавить";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxDevType;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxSerialNumber;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxInvenotryNumber;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxDescription;
	}
}