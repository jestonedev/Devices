namespace Devices
{
    partial class PeripheryDevicesForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.peripheryTypes = new System.Windows.Forms.CheckedListBox();
            this.allCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(277, 332);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 39);
            this.button1.TabIndex = 4;
            this.button1.Text = "Ок";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(415, 332);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 39);
            this.button2.TabIndex = 5;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(45, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(211, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Периферийное оборудование:";
            // 
            // peripheryTypes
            // 
            this.peripheryTypes.FormattingEnabled = true;
            this.peripheryTypes.Location = new System.Drawing.Point(275, 82);
            this.peripheryTypes.Name = "peripheryTypes";
            this.peripheryTypes.Size = new System.Drawing.Size(251, 225);
            this.peripheryTypes.TabIndex = 7;
            // 
            // allCheck
            // 
            this.allCheck.AutoSize = true;
            this.allCheck.Location = new System.Drawing.Point(277, 55);
            this.allCheck.Name = "allCheck";
            this.allCheck.Size = new System.Drawing.Size(54, 21);
            this.allCheck.TabIndex = 8;
            this.allCheck.Text = "Все";
            this.allCheck.UseVisualStyleBackColor = true;
            this.allCheck.CheckedChanged += new System.EventHandler(this.allCheck_CheckedChanged);
            // 
            // PeripheryDevicesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 395);
            this.Controls.Add(this.allCheck);
            this.Controls.Add(this.peripheryTypes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "PeripheryDevicesForm";
            this.Text = "Формирование отчета";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox peripheryTypes;
        private System.Windows.Forms.CheckBox allCheck;
    }
}