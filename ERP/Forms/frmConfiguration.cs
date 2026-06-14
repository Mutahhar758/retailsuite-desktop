using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace ERP.Forms
{
    public partial class frmConfiguration : Form
    {
        public frmConfiguration()
        {
            InitializeComponent();
        }

        private void frmConfiguration_Load(object sender, EventArgs e)
        {
            foreach (String strPrinter in PrinterSettings.InstalledPrinters)
            {
                cmbPrinter.Items.Add(strPrinter);                
            }
            cmbPrinter.SelectedItem = ConfigInfo.ThermalPrinterName; 
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            INIFile.WriteValue("PrinterSetting", "ThermalPrinter", cmbPrinter.SelectedItem.ToString());
            ConfigInfo.ThermalPrinterName = cmbPrinter.SelectedItem.ToString();

        }
    }
}
