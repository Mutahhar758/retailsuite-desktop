using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing ;
using  System.Windows .Forms;
using System.Data;

namespace ERP
{
    public partial class Component1 : System.Windows.Forms.ComboBox
    {
        DataGridView dgv = new DataGridView(); 
        public Component1()
        {
            InitializeComponent();
           
        }
        public string a = "";
        public Component1(IContainer container)
        {
            this.DroppedDown = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToOrderColumns = true;
            dgv.AllowUserToResizeColumns = false;
            dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.Location = new System.Drawing.Point(304, 33);
            dgv.Name = "dataGridView1";
            dgv.ReadOnly = true;
            dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgv.Size = new System.Drawing.Size(309, 150);
            
            container.Add(this);
            
            InitializeComponent();

           
            
        }


        private void Component1_Click(object sender, EventArgs e)
        {
            
           
        }
        bool Flogin = true;
        private void Component1_DropDown(object sender, EventArgs e)
        {
            if (Flogin)
            {
                DataTable dt = new DataTable();
                dt = (DataTable)this.DataSource;
                dgv.DataSource = dt;
                this.FindForm().Controls.Add(dgv);
                Flogin = false;
            }
           
            dgv.Visible = true;
        }

        private void Component1_DropDownClosed(object sender, EventArgs e)
        {
            dgv.Visible = false ;
        }
    }
}
