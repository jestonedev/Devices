using System;
using System.Linq;
using System.Windows.Forms;

namespace Devices.ReportsForms
{
    public partial class PeripheryDevicesForm : Form
    {
        private readonly DevicesDatabase _db = new DevicesDatabase();

        public PeripheryDevicesForm()
        {
            InitializeComponent();
            PeripheryTypes.DataSource = _db.GetPeripheryType();
            PeripheryTypes.DisplayMember = "Value";
            PeripheryTypes.ValueMember = "Id";
        }

        public string GetFilterIds()
        {
            var idsPerTypes = string.Empty;
            if (PeripheryTypes.CheckedItems.Count <= 0) return "";
            idsPerTypes = PeripheryTypes.CheckedItems.Cast<PeripheryType>().
                Aggregate(idsPerTypes, (current, row) => current + ("," + row.Id));
            return "(0" + idsPerTypes + ")";
        }
            
        private void allCheck_CheckedChanged(object sender, EventArgs e)
        {
            for (var i = 0; i < PeripheryTypes.Items.Count; i++)
            {
                PeripheryTypes.SetItemChecked(i, allCheck.Checked);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (PeripheryTypes.CheckedItems.Count == 0)
            {
                MessageBox.Show(@"Выберите хотя бы один тип переферийного оборудования", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        
    }
}
