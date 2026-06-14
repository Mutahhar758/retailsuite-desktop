using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERP
{
    public partial class frmReportView : Form
    {
        public frmReportView()
        {
            InitializeComponent();
        }

        private void frmReportView_Load(object sender, EventArgs e)
        {

        }

     

        private void rptViewer_DoubleClickPage(object sender, CrystalDecisions.Windows.Forms.PageMouseEventArgs e)
        {
            if (e.ObjectInfo.Name.ToLower() == "vno1")
            {
                if (e.ObjectInfo.Text.Substring(0, 2) == "SL")
                {
                    frmSale frmSL = new frmSale();
                    frmSL.FillSale(e.ObjectInfo.Text.Substring(3));
                    frmSL.ShowDialog();
                }
                if (e.ObjectInfo.Text.Substring(0, 2) == "SR")
                {
                    frmSaleReturn frmSL = new frmSaleReturn();
                    frmSL.FillSale(e.ObjectInfo.Text.Substring(3));
                    frmSL.ShowDialog();
                }
                else if (e.ObjectInfo.Text.Substring(0, 2) == "PU")
                {
                    frmPurchase frmPU = new frmPurchase();
                    frmPU.FillPurchase(e.ObjectInfo.Text.Substring(3));
                    frmPU.ShowDialog();
                }
                else if (e.ObjectInfo.Text.Substring(0, 2) == "PR")
                {
                    frmPurchaseReturn frmPU = new frmPurchaseReturn();
                    frmPU.FillPurchase(e.ObjectInfo.Text.Substring(3));
                    frmPU.ShowDialog();
                }
                else if (e.ObjectInfo.Text.Substring(0, 2) == "PV")
                {
                    frmPayments frmPV = new frmPayments();
                    frmPV.FillPayment(e.ObjectInfo.Text.Substring(3));
                    frmPV.ShowDialog();
                }
                else if (e.ObjectInfo.Text.Substring(0, 2) == "RV")
                {
                    frmReceipt frmRV = new frmReceipt();
                    _ = frmRV.FillReceiptAsync(e.ObjectInfo.Text.Substring(3));
                    frmRV.ShowDialog();
                }
                else if (e.ObjectInfo.Text.Substring(0, 2) == "JV")
                {
                    frmJournalVoucher frmJV = new frmJournalVoucher();
                    frmJV.FillJournal(e.ObjectInfo.Text.Substring(3));
                    frmJV.ShowDialog();
                }
                else if (e.ObjectInfo.Text.Substring(0, 2) == "PL")
                {
                    frmPayRoll frmPL = new frmPayRoll();
                    frmPL.FillPayroll(e.ObjectInfo.Text.Substring(3));
                    frmPL.ShowDialog();
                }
            }
        }

       
    }
}
