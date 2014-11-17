namespace Devices
{
	partial class ComputerParamChangeForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComputerParamChangeForm));
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxParam = new System.Windows.Forms.TextBox();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonQuit = new System.Windows.Forms.Button();
			this.comboBoxParam = new System.Windows.Forms.ComboBox();
			this.numericUpDownParam = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownParam)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// textBoxParam
			// 
			resources.ApplyResources(this.textBoxParam, "textBoxParam");
			this.textBoxParam.Name = "textBoxParam";
			this.textBoxParam.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDownParam_KeyDown);
			// 
			// buttonSave
			// 
			resources.ApplyResources(this.buttonSave, "buttonSave");
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// buttonQuit
			// 
			resources.ApplyResources(this.buttonQuit, "buttonQuit");
			this.buttonQuit.Name = "buttonQuit";
			this.buttonQuit.UseVisualStyleBackColor = true;
			this.buttonQuit.Click += new System.EventHandler(this.buttonQuit_Click);
			// 
			// comboBoxParam
			// 
			this.comboBoxParam.FormattingEnabled = true;
			resources.ApplyResources(this.comboBoxParam, "comboBoxParam");
			this.comboBoxParam.Name = "comboBoxParam";
			this.comboBoxParam.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDownParam_KeyDown);
			// 
			// numericUpDownParam
			// 
			resources.ApplyResources(this.numericUpDownParam, "numericUpDownParam");
			this.numericUpDownParam.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
			this.numericUpDownParam.Name = "numericUpDownParam";
			this.numericUpDownParam.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDownParam_KeyDown);
			// 
			// ComputerParamChangeForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.numericUpDownParam);
			this.Controls.Add(this.comboBoxParam);
			this.Controls.Add(this.buttonQuit);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.textBoxParam);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ComputerParamChangeForm";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownParam)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxParam;
		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.Button buttonQuit;
		private System.Windows.Forms.ComboBox comboBoxParam;
		private System.Windows.Forms.NumericUpDown numericUpDownParam;
	}
}