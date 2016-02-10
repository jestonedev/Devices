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
    public partial class PeripheryDevicesForm : Form
    {
        private DevicesDatabase _db = new DevicesDatabase();

        public PeripheryDevicesForm()
        {
            InitializeComponent();
            peripheryTypes.DataSource = _db.GetPeripheryType();
            peripheryTypes.DisplayMember = "Value";
            peripheryTypes.ValueMember = "Id";
            var node = new Node("Все департаменты", 0, 0);
            Organizations.DataSource = new BindingSource((new List<Node> { node}).Concat(_db.GetDepartments(null,"[ID Parent Department] IS NULL")), null);
            Organizations.ValueMember = "NodeID";
            Organizations.DisplayMember = "NodeName";
        }

        public Dictionary<string,string> GetDepartmentsId()
        {
            var dict = new Dictionary<string, string>();
            string idsDeps = "";
            if(Organizations.SelectedIndex == 0)
            {
                foreach(var dep in Organizations.Items)
                {
                    Node node = (Node)dep;
                    if (node.NodeID == 0)
                        continue;
                    idsDeps += Organizations.Items.IndexOf(dep) == Organizations.Items.Count - 1 ?
                        node.NodeID.ToString() : node.NodeID.ToString() + ", ";
                }
                dict.Add(((Node)Organizations.Items[0]).NodeName,  "(" + idsDeps + ")");
                return dict;
            }
            else
            {
                Node node =(Node) Organizations.SelectedItem;
                dict.Add(node.NodeName,"(" + node.NodeID + ")");
                return dict;
            }
        }

        public string GetFilterIds()
        {
            string idsPerTypes = string.Empty;
            if (peripheryTypes.CheckedItems.Count > 0)
            {
                for (int i = 0; i < peripheryTypes.CheckedItems.Count; i++)
                {
                    var row = (PeripheryType)peripheryTypes.CheckedItems[i];
                    idsPerTypes += peripheryTypes.CheckedItems.IndexOf(row) == peripheryTypes.CheckedItems.Count - 1 ? row.Id.ToString() : row.Id.ToString() + ", ";
                }
                return idsPerTypes = "(" + idsPerTypes + ")";
            }
            else
            {
                MessageBox.Show(@"Выберите хотя бы один тип устройства", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }            
        }      
            
        private void allCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (allCheck.Checked)
            {
                for(int i=0; i < peripheryTypes.Items.Count; i++)
                {
                    peripheryTypes.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < peripheryTypes.Items.Count; i++)
                {
                    peripheryTypes.SetItemChecked(i, false);
                } 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        
    }
}
