using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ERP
{
    public partial class DecimalTextbox : System .Windows .Forms .TextBox 
    {
        public DecimalTextbox()
        {
            InitializeComponent();
        }

        public DecimalTextbox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private void NumericTextbox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
           
                if (this.SelectedText.Length > 0)
                {
                    int selind = this.SelectionStart;
                    this.Text = this.Text.Replace(this.SelectedText, "");
                    this.SelectionStart = selind;
                    this.SelectionLength = 0;
                }
                if (!char.IsControl(e.KeyChar)
           && !char.IsDigit(e.KeyChar)
           && !(this.Text.Count(a => a == '.') == 0 && e.KeyChar == '.')
                    )
                {
                    e.Handled = true;
                }
            
        }

        private void DecimalTextbox_Validated(object sender, EventArgs e)
        {
            if (this.Text == "")this.Text = "0";
        }

        public decimal Value
        {
            get
            {
                decimal result;
                return decimal.TryParse(this.Text, out result) ? result : 0m;
            }
            set
            {
                this.Text = value.ToString();
            }
        }
    }
}
