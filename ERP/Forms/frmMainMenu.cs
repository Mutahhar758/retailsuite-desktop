using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using ERP.Classes;
namespace ERP
{
    public partial class frmMainMenu : Form
    {
        public frmMainMenu()
        {
            InitializeComponent();
        }

        private void chartOfAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChartOfAcc chart = new frmChartOfAcc();
            chart.MdiParent = this;
            chart.Show();
        }
        Form frmLogin;
        Thread th;
        void INISetting()
        {
            //if (!Directory.Exists("Settings.ini"))
            //File.Create("Settings.ini");
            //if (!Directory.Exists(@"C:\\Con.ini"))
            // File.Create("Prii.ini");
            //INIFile MyINI = new INIFile("Settings.ini");
            ConfigInfo.ThermalPrinterName = INIFile.ReadValue("PrinterSetting", "ThermalPrinter");
            // bool a =  INIFile.WriteValue("PrinterSetting", "ThermalPrinter", "ABC");


        }
        private void CompanyCetreLoc()
        {
            lblCompanyName.Location = new Point((this.Size.Width - lblCompanyName.Size.Width) / 2, lblCompanyName.Location.Y);
            lblUrCompanyName.Location = new Point((this.Size.Width - lblUrCompanyName.Size.Width) / 2, lblUrCompanyName.Location.Y);
            picLogo.Location = new Point((this.Size.Width - picLogo.Size.Width) / 2, picLogo.Location.Y);
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            INISetting();
            StlblUserId.Text = UserInfo.UserName;
            this.Text = CompanyInfo.CompanyName;
            lblCompanyName.Text = CompanyInfo.CompanyName;
            lblUrCompanyName.Text = CompanyInfo.UrCompanyName;
            AccountHead.Customer = "001002001001";
            AccountHead.Farmer = "002001001001";
            CompanyCetreLoc();

            frmLogin = this.Owner;
            if (frmLogin != null)
                frmLogin.Hide();

            StConStatus.Text = Application.ProductVersion;

            ApplyPermissions();
        }

        private void ApplyPermissions()
        {
            chartOfAccountToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.ChartOfAccounts);
            detailAccountsToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.DetailAccounts);
            customerInformationToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.Customers);
            supplierInformationToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.Vendors);
            narrationToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.Narrations);
            itemDetailToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.InventoryItems);
            unitIndexToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.Units);
            itemCatagoryToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.ItemCategories);
            hRInfoToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.HRInfo);
            supplyOrderToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.SupplyOrders);
            
            paymentVoucherToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.PaymentVouchers);
            receiptVoucherToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.ReceiptVouchers);
            journalVoucherToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.JournalVouchers);
            purchaseToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.Purchases);
            purchaseReturnToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.PurchaseReturns);
            toolStripMenuItem1.Visible = UserInfo.HasPermission(AppAction.View, AppResource.SaleSupplies);
            customerSupplyRegisterToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.SaleSupplies);
            saleOrderToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.Sales);
            saleReturnToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.SaleReturns);
            stockAdjustmentToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.StockAdjustments);
            bankReconcilationToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.BankReconciliations);
            payrollToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.Payrolls);
            
            // Reports (Urdu / Classic)
            accountStatementToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.AccountStatement);
            accountStatementWithDueToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.AccountStatementWithDue);
            accountBalanceToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.AccountBalance);
            trialBalanceToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.TrialBalance);
            stockBalanceToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.StockBalance);
            itemLedgerToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.StockLedger);
            incomeSummaryToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.IncomeSummary);
            balanceSheetToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.BalanceSheet);
            customerBillToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.CustomerBill);
            enToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.EnvelopeReport);
            barcodeToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.BarcodeReport);
            shipmentLabelTagToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.ShipmentLabelReport);
            reportsToolStripMenuItem.Visible = UserInfo.IsOwner ||
                accountStatementToolStripMenuItem.Visible || accountStatementWithDueToolStripMenuItem.Visible ||
                accountBalanceToolStripMenuItem.Visible || trialBalanceToolStripMenuItem.Visible ||
                stockBalanceToolStripMenuItem.Visible || itemLedgerToolStripMenuItem.Visible ||
                incomeSummaryToolStripMenuItem.Visible || balanceSheetToolStripMenuItem.Visible ||
                customerBillToolStripMenuItem.Visible || enToolStripMenuItem.Visible ||
                barcodeToolStripMenuItem.Visible || shipmentLabelTagToolStripMenuItem.Visible;

            // Reports 2 (English / Modern)
            accountStatement2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.AccountStatement);
            accountStatementWithDue2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.AccountStatementWithDue);
            stockBalance2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.StockBalance);
            trialBalance2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.TrialBalance);
            accountBalance2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.AccountBalance);
            itemLedger2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.StockLedger);
            incomeSummary2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.IncomeSummary);
            balanceSheet2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.BalanceSheet);
            customerBill2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.CustomerBill);
            milkComparison2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.MilkComparison);
            customerBalanceRecovery2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.CustomerBalanceRecovery);
            envelope2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.EnvelopeReport);
            barcode2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.BarcodeReport);
            shipmentLabelTag2ToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.ShipmentLabelReport);
            miscReports2ToolStripMenuItem.Visible = UserInfo.IsOwner || UserInfo.HasPermission(AppAction.View, AppResource.MiscReports) || envelope2ToolStripMenuItem.Visible || barcode2ToolStripMenuItem.Visible || shipmentLabelTag2ToolStripMenuItem.Visible;
            reports2ToolStripMenuItem.Visible = UserInfo.IsOwner ||
                accountStatement2ToolStripMenuItem.Visible || accountStatementWithDue2ToolStripMenuItem.Visible ||
                stockBalance2ToolStripMenuItem.Visible || trialBalance2ToolStripMenuItem.Visible ||
                accountBalance2ToolStripMenuItem.Visible || itemLedger2ToolStripMenuItem.Visible ||
                incomeSummary2ToolStripMenuItem.Visible || balanceSheet2ToolStripMenuItem.Visible ||
                customerBill2ToolStripMenuItem.Visible || milkComparison2ToolStripMenuItem.Visible ||
                customerBalanceRecovery2ToolStripMenuItem.Visible || miscReports2ToolStripMenuItem.Visible;
            
            configurationToolStripMenuItem.Visible = UserInfo.HasPermission(AppAction.View, AppResource.PrinterSettings);
        }

        private void customerInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCustomerInfo cus = new frmCustomerInfo();
            cus.MdiParent = this;
            cus.Show();
        }

        private void paymentVoucherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPayments frm = new frmPayments();
            frm.MdiParent = this;
            frm.Show();

        }

        private void narrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNarration narr = new frmNarration();
            narr.MdiParent = this;
            narr.Show();
        }

        private void receiptVoucherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReceipt frm = new frmReceipt();
            frm.MdiParent = this;
            frm.Show();
        }

        private void supplyOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSupplyOrder frm = new frmSupplyOrder();
            frm.MdiParent = this;
            frm.Show();
        }



        private void unitsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void itemDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmItemDetail frm = new frmItemDetail();
            frm.MdiParent = this;
            frm.Show();
        }



        private void saleOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSale frm = new frmSale();
            frm.MdiParent = this;
            frm.Show();
        }




        private void unitIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUnits frm = new frmUnits();
            frm.MdiParent = this;
            frm.Show();
        }

        private void accountStatementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Account Statement";
            frm.MdiParent = this;
            frm.Show();

        }

        private void accountStatement2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.ReportViewer viewer = new ERP.Reporting.ReportViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void accountStatementWithDue2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.ReportViewer viewer = new ERP.Reporting.ReportViewer(ERP.Reporting.ReportViewerMode.AccountStatementWithDue);
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void stockBalance2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.StockBalanceViewer viewer = new ERP.Reporting.StockBalanceViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void trialBalance2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.TrialBalanceViewer viewer = new ERP.Reporting.TrialBalanceViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void accountBalance2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.AccountBalanceViewer viewer = new ERP.Reporting.AccountBalanceViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void itemLedger2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.ItemLedgerViewer viewer = new ERP.Reporting.ItemLedgerViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void incomeSummary2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.IncomeSummaryViewer viewer = new ERP.Reporting.IncomeSummaryViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void balanceSheet2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.BalanceSheetViewer viewer = new ERP.Reporting.BalanceSheetViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void customerBill2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.CustomerBillViewer viewer = new ERP.Reporting.CustomerBillViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void milkComparison2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.MilkComparisonViewer viewer = new ERP.Reporting.MilkComparisonViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void customerBalanceRecovery2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.CustomerBalanceRecoveryViewer viewer = new ERP.Reporting.CustomerBalanceRecoveryViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void envelope2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.EnvelopeViewer viewer = new ERP.Reporting.EnvelopeViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void barcode2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.BarcodeViewer viewer = new ERP.Reporting.BarcodeViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void shipmentLabelTag2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ERP.Reporting.ShipmentTagViewer viewer = new ERP.Reporting.ShipmentTagViewer();
            viewer.MdiParent = this;
            viewer.Show();
        }

        private void purchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPurchase frm = new frmPurchase();
            frm.MdiParent = this;
            frm.Show();
        }

        private void stockBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Stock Balance";
            frm.MdiParent = this;
            frm.Show();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (th != null && th.IsAlive)
                th.Abort();
            Application.Exit();
        }



        private void journalVoucherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmJournalVoucher frm = new frmJournalVoucher();
            frm.MdiParent = this;
            frm.Show();
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.X)
            {
                if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to exit this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
            else if (e.KeyCode == Keys.F12)
            {
                if ((Application.OpenForms["frmSale"]) != null)
                    Application.OpenForms["frmSale"].Focus();
                else
                {
                    frmSale frm = new frmSale();
                    frm.MdiParent = this;
                    frm.Show();
                }
            }
            else if (e.KeyCode == Keys.F11)
            {
                if ((Application.OpenForms["frmPurchase"]) != null)
                    Application.OpenForms["frmPurchase"].Focus();
                else
                {
                    frmPurchase frm = new frmPurchase();
                    frm.MdiParent = this;
                    frm.Show();
                }
            }
            else if (e.KeyCode == Keys.F8)
            {
                if ((Application.OpenForms["frmChartOfAcc"]) != null)
                    Application.OpenForms["frmChartOfAcc"].Focus();
                else
                {
                    frmChartOfAcc frm = new frmChartOfAcc();
                    frm.MdiParent = this;
                    frm.Show();
                }
            }
            //////////Calculator/////////////////////// 
            else if (e.KeyCode == Keys.F1)
            {
                System.Diagnostics.Process calcprc = System.Diagnostics.Process.Start("calc.exe");
                calcprc.WaitForInputIdle();

            }
        }

        private void itemLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Stock Ledger";
            frm.MdiParent = this;
            frm.Show();
        }

        private void incomeSummeryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Income Summery";
            frm.MdiParent = this;
            frm.Show();
        }

        private void trialBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Trial Balance";
            frm.MdiParent = this;
            frm.Show();
        }

        private void balanceSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Balance Sheet";
            frm.MdiParent = this;
            frm.Show();
        }

        private void supplierInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSupplierInfo supp = new frmSupplierInfo();
            supp.MdiParent = this;
            supp.Show();
        }

        private void monthlyInstallmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Monthly Installments";
            frm.MdiParent = this;
            frm.Show();
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {


        }

        private void bankReconcilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBankReconcilation frm = new frmBankReconcilation();
            frm.MdiParent = this;
            frm.Show();
        }

        private void accountStatementToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Account Statement (Urdu)";
            frm.Paramname = "Account Statement";
            frm.MdiParent = this;
            frm.Show();
        }

        private void balanceSheetToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Balance Sheet (Urdu)";
            frm.Paramname = "Balance Sheet";
            frm.MdiParent = this;
            frm.Show();
        }

        private void customerBillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Customer Bill (Urdu)";
            frm.Paramname = "Customer Bill";
            frm.MdiParent = this;
            frm.Show();
        }

        private void itemCatagoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmItemCatagory frm = new frmItemCatagory();
            frm.MdiParent = this;
            frm.Show();
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.frmConfiguration frm = new Forms.frmConfiguration();
            frm.MdiParent = this;
            frm.Show();
        }





        private void detailAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChartOfAccLvl5 frm = new frmChartOfAccLvl5();
            frm.MdiParent = this;
            frm.Show();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            frmChartOfAccLvl5 frm = new frmChartOfAccLvl5();
            frm.MdiParent = this;
            frm.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            frmSale frm = new frmSale();
            frm.MdiParent = this;
            frm.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            frmPurchase frm = new frmPurchase();
            frm.MdiParent = this;
            frm.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            frmReceipt frm = new frmReceipt();
            frm.MdiParent = this;
            frm.Show();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            frmPayments frm = new frmPayments();
            frm.MdiParent = this;
            frm.Show();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            frmJournalVoucher frm = new frmJournalVoucher();
            frm.MdiParent = this;
            frm.Show();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            frmBankReconcilation frm = new frmBankReconcilation();
            frm.MdiParent = this;
            frm.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.bizgrip.somee.com");
        }

        private void pictureBox7_Click_1(object sender, EventArgs e)
        {
            frmItemDetail frm = new frmItemDetail();
            frm.MdiParent = this;
            frm.Show();
        }

        private void stockAdjustmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStockAdjustment frm = new frmStockAdjustment();
            frm.MdiParent = this;
            frm.Show();
        }


        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.frmChangepassword frm = new Forms.frmChangepassword();
            frm.MdiParent = this;
            frm.Show();
        }

        private void accountBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Balance Detail";
            frm.Paramname = "Balance Detail";
            frm.MdiParent = this;
            frm.Show();
        }

        private void itemLedgerToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Stock Ledger";
            frm.Paramname = "Stock Ledger";
            frm.MdiParent = this;
            frm.Show();
        }

        private void saleReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleReturn frm = new frmSaleReturn();
            frm.MdiParent = this;
            frm.Show();
        }

        private void purchaseReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPurchaseReturn frm = new frmPurchaseReturn();
            frm.MdiParent = this;
            frm.Show();
        }

        private void incomeSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Income Summery";
            frm.Paramname = "Income Summery";
            frm.MdiParent = this;
            frm.Show();

        }

        private void balanceSheetToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Balance Sheet";
            frm.Paramname = "Balance Sheet";
            frm.MdiParent = this;
            frm.Show();

        }

        private void hRInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.frmHRInfo frm = new Forms.frmHRInfo();
            frm.MdiParent = this;
            frm.Show();
        }

        private void payrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPayRoll frm = new frmPayRoll();
            frm.MdiParent = this;
            frm.Show();
        }


        private void enToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Envelope";
            frm.Paramname = "Envelope";
            frm.MdiParent = this;
            frm.Show();
        }


        private void frmMainMenu_Shown(object sender, EventArgs e)
        {

            MainPait();

        }
        Bitmap Mainbmp;
        private void MainPait()
        {
            Mainbmp = new Bitmap(panel1.Width, panel1.Height);
            Graphics g = Graphics.FromImage(Mainbmp);
            g.CopyFromScreen(PointToScreen(panel1.Location), new Point(0, 0), panel1.Size);
            panel1.DrawToBitmap(Mainbmp, new Rectangle(Point.Empty, Mainbmp.Size));
            this.Controls.Remove(panel1);
            this.BackgroundImage = Mainbmp;

        }
        private void accountStatementWithDueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Account Statement With Due Days";
            frm.MdiParent = this;
            frm.Show();
        }

        private void barcodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Barcode";
            frm.MdiParent = this;
            frm.Show();

        }

        private void shipmentLabelTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Shipment Label Tag";
            frm.MdiParent = this;
            frm.Show();

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmSaleSupply frm = new frmSaleSupply();
            frm.MdiParent = this;
            frm.Show();
        }

        private void customerSupplyRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCustomerSupplyRegister frm = new frmCustomerSupplyRegister();
            frm.MdiParent = this;
            frm.Show();
        }

        private void customerBillToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            frmReportParameters frm = new frmReportParameters();
            frm.Reportname = "Customer Bill Date Range";
            frm.Paramname = "Customer Bill Date Range";
            frm.MdiParent = this;
            frm.Show();
        }

        private void supplyOrderToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            frmSupplyOrder frm = new frmSupplyOrder();
            frm.MdiParent = this;
            frm.Show();
        }
    }






}

