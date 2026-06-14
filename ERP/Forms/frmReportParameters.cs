using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ERP.Classes;
using System.IO;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmReportParameters : Form
    {
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly ItemCategoryApiService _itemCategoryApiService;
        private readonly InventoryApiService _inventoryApiService;
        private readonly CustomerApiService _customerApiService;
        private readonly SupplyOrderApiService _supplyOrderApiService;

        public frmReportParameters()
        {
            InitializeComponent();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _itemCategoryApiService = new ItemCategoryApiService();
            _inventoryApiService = new InventoryApiService();
            _customerApiService = new CustomerApiService();
            _supplyOrderApiService = new SupplyOrderApiService();
        }
        internal string Reportname = "";
        internal string Paramname = "";
        bool FLogIn = true;
        private void ManageControls(Control[] ctrl)
        {
            int LocX = 12, LocY = 60;
            pnlControl.Location = new Point(235, LocY);
            LocY += 50;
            foreach (Control item in ctrl)
            {
                item.Visible = true;
                item.Location = new Point(LocX, LocY);
                LocY += item.Height + 3;
            }
            this.Size = new Size(430, LocY + 50);

        }
        private void ManageControlsLocation(Control[] ctrl)
        {
            int LocX = 12, LocY = 60;
            pnlControl.Location = new Point(235, LocY);
            LocY += 50;
            foreach (Control item in ctrl)
            {
                item.Location = new Point(LocX, LocY);
                LocY += item.Height + 3;
            }
            this.Size = new Size(430, LocY + 50);
        }

        private static DataTable ToAccountDataTable(List<ChartOfAccountHeadDto> source, string codeColumnName)
        {
            var dt = new DataTable();
            dt.Columns.Add(codeColumnName, typeof(string));
            dt.Columns.Add("Title", typeof(string));

            foreach (var item in source)
                dt.Rows.Add(item.Account, item.Title);

            return dt;
        }

        private static DataTable ToCategoryDataTable(List<ItemCategoryDto> source)
        {
            var dt = new DataTable();
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Title", typeof(string));

            foreach (var item in source)
                dt.Rows.Add(item.Code, item.Title);

            return dt;
        }

        private static DataTable ToItemDataTable(List<InventoryItemDto> source)
        {
            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Title", typeof(string));

            foreach (var item in source)
                dt.Rows.Add(item.Id, item.Title);

            return dt;
        }

        private async System.Threading.Tasks.Task FillAccouontAsync()
        {
            var accounts = await _chartOfAccountApiService.GetDetailAccountsAsync();
            cmbAccount.DataSource = ToAccountDataTable(accounts, "Account");
            cmbAccount.DisplayMember = "Title";
            cmbAccount.ValueMember = "Account";
        }

        private async System.Threading.Tasks.Task FillLvl4AccountHeadsAsync()
        {
            var accounts = await _chartOfAccountApiService.GetHeadsAsync(4);
            cmbAccount.DataSource = ToAccountDataTable(accounts, "Account");
            cmbAccount.DisplayMember = "Title";
            cmbAccount.ValueMember = "Account";
            cmbAccount.SelectedIndex = -1;
        }

        private async System.Threading.Tasks.Task FillCustomersAsync()
        {
            var customers = await _chartOfAccountApiService.GetCustomerAccountsAsync();
            cmbAccount.DataSource = ToAccountDataTable(customers, "Code");
            cmbAccount.DisplayMember = "Title";
            cmbAccount.ValueMember = "Code";
            cmbAccount.SelectedIndex = -1;
        }

        private async System.Threading.Tasks.Task FillCheckedAccountsAsync(bool customers)
        {
            if (customers)
            {
                var customerAccounts = await _chartOfAccountApiService.GetCustomerAccountsAsync();
                ((ListBox)chklstAccounts).DataSource = ToAccountDataTable(customerAccounts, "Code");
                ((ListBox)chklstAccounts).DisplayMember = "Title";
                ((ListBox)chklstAccounts).ValueMember = "Code";
                return;
            }

            var accounts = await _chartOfAccountApiService.GetDetailAccountsAsync();
            ((ListBox)chklstAccounts).DataSource = ToAccountDataTable(accounts, "Account");
            ((ListBox)chklstAccounts).DisplayMember = "Title";
            ((ListBox)chklstAccounts).ValueMember = "Account";
        }

        private async System.Threading.Tasks.Task FillItemCategoriesAsync()
        {
            var categories = await _itemCategoryApiService.GetActiveAsync();
            cmbItemCatagory.DataSource = ToCategoryDataTable(categories);
            cmbItemCatagory.DisplayMember = "Title";
            cmbItemCatagory.ValueMember = "Code";
            cmbItemCatagory.SelectedIndex = -1;
        }

        private async System.Threading.Tasks.Task FillItemsAsync()
        {
            var items = await _inventoryApiService.GetItemsAsync(null);
            cmbItem.DataSource = ToItemDataTable(items);
            cmbItem.DisplayMember = "Title";
            cmbItem.ValueMember = "ID";
            cmbItem.SelectedIndex = -1;
        }

        private async System.Threading.Tasks.Task FillSupplyOrdersAsync()
        {
            var supplyOrders = await _supplyOrderApiService.GetAsync();
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            
            // Add an empty row for default
            dt.Rows.Add("", "--- Select Profile ---");

            foreach (var item in supplyOrders)
                dt.Rows.Add(item.Id.ToString(), item.Title);

            cmbSupplyOrder.DataSource = dt;
            cmbSupplyOrder.DisplayMember = "Title";
            cmbSupplyOrder.ValueMember = "Id";
            cmbSupplyOrder.SelectedIndex = 0;
        }
        
        private void btnPrint_Click(object sender, EventArgs e)
        {
            frmReportView frm = new frmReportView();
            #region Account Statement
            if (Reportname == "Account Statement")
            {
                string CompanyName = CompanyInfo.CompanyName;
                if (rdoViewReport.Checked)
                {
                    
                    Reports.AccountStatement rpt = new Reports.AccountStatement();
                    DataTable dt = ReportQuery.AccountStatement((string)cmbAccount.SelectedValue, dtpFDate.Value, dtpTDate.Value);
                    rpt.SetDataSource(dt);
                    rpt.SetParameterValue("@companyname", CompanyName);
                    rpt.SetParameterValue("@Account", cmbAccount.Text);
                    rpt.SetParameterValue("@Fdate", dtpFDate.Value);
                    rpt.SetParameterValue("@Tdate", dtpTDate.Value);
                    frm.rptViewer.ReportSource = rpt;
                }
                else
                {
                    foreach (DataRowView item in chklstAccounts.CheckedItems)
                    {
                        
                        
                        Reports.AccountStatement rpt = new Reports.AccountStatement();
                        DataTable dt = ReportQuery.AccountStatement(item["Account"].ToString() , dtpFDate.Value, dtpTDate.Value);
                        rpt.SetDataSource(dt);
                        rpt.SetParameterValue("@companyname", CompanyName);
                        rpt.SetParameterValue("@Account", item["Title"].ToString());
                        rpt.SetParameterValue("@Fdate", dtpFDate.Value);
                        rpt.SetParameterValue("@Tdate", dtpTDate.Value);
                        rpt.PrintToPrinter(1, true, 0, 0);
                    }
                    return;
                }
            }
            #endregion
            #region Ur Account Statement
            else if (Reportname == "Account Statement (Urdu)")
            {
                UrduReports.CrUrAccountStatement rpt = new UrduReports.CrUrAccountStatement();
                DataTable dt = ReportQuery.AccountStatement((string)cmbAccount.SelectedValue, dtpFDate.Value, dtpTDate.Value);
                rpt.SetDataSource(dt);
                rpt.SetParameterValue("@companyname", CompanyInfo.UrCompanyName);
                rpt.SetParameterValue("@Account", cmbAccount.Text);
                rpt.SetParameterValue("@Fdate", dtpFDate.Value);
                rpt.SetParameterValue("@Tdate", dtpTDate.Value);
                frm.rptViewer.ReportSource = rpt;

            }
            #endregion
            #region Account Statement With Due Days
            if (Reportname == "Account Statement With Due Days")
            {                
                string CompanyName = CompanyInfo.CompanyName;
                
                if (rdoViewReport.Checked)
                {
                    Reports.AccountStatementWithdue rpt = new Reports.AccountStatementWithdue();
                    DataTable dt = ReportQuery.AccountStatementWithDue((string)cmbAccount.SelectedValue, dtpFDate.Value, dtpTDate.Value);
                    rpt.SetDataSource(dt);
                    rpt.SetParameterValue("@companyname", CompanyName);
                    rpt.SetParameterValue("@Account", cmbAccount.Text);
                    rpt.SetParameterValue("@Fdate", dtpFDate.Value);
                    rpt.SetParameterValue("@Tdate", dtpTDate.Value);
                    frm.rptViewer.ReportSource = rpt;
                }
                else
                {
                    foreach (DataRowView item in chklstAccounts.CheckedItems)
                    {
                        Reports.AccountStatementWithdue rpt = new Reports.AccountStatementWithdue();
                        DataTable dt = ReportQuery.AccountStatementWithDue(item["Account"].ToString(), dtpFDate.Value, dtpTDate.Value);
                        rpt.SetDataSource(dt);
                        rpt.SetParameterValue("@companyname", CompanyName);
                        rpt.SetParameterValue("@Account", item["Title"].ToString());
                        rpt.SetParameterValue("@Fdate", dtpFDate.Value);
                        rpt.SetParameterValue("@Tdate", dtpTDate.Value);
                        rpt.PrintToPrinter(1, true, 0, 0);
                    }
                    return;
                }
            }
            #endregion
            #region Stock Balance
            if (Reportname == "Stock Balance")
            {
                Reports.StockBalace rpt = new Reports.StockBalace();
                DataTable dt = ReportQuery.StockBalance(dtpFDate.Value, dtpTDate.Value, cmbFilter.Text, decimal.Parse(txtQty.Text), (string)cmbItemCatagory.SelectedValue, (string)cmbType.SelectedValue);                
                rpt.SetDataSource(dt);
                rpt.SetParameterValue("@companyname", CompanyInfo.CompanyName);
                rpt.SetParameterValue("@Fdate", dtpFDate.Value);
                rpt.SetParameterValue("@Tdate", dtpTDate.Value);
                frm.rptViewer.ReportSource = rpt;
            }
            #endregion
            #region Stock Ledger
            if (Reportname == "Stock Ledger")
            {
                Reports.StockLedger rpt = new Reports.StockLedger();
                DataTable dt = ReportQuery.StockLedger( (string)cmbItem.SelectedValue, dtpFDate.Value, dtpTDate.Value);
                rpt.SetDataSource(dt);
                rpt.SetParameterValue("@companyname", CompanyInfo.CompanyName);
                rpt.SetParameterValue("@item", cmbItem.Text);
                rpt.SetParameterValue("@Fdate", dtpFDate.Value);
                rpt.SetParameterValue("@Tdate", dtpTDate.Value);
                frm.rptViewer.ReportSource = rpt;

            }
            #endregion
            #region Trial Balance
            if (Reportname == "Trial Balance")
            {
                Reports.CrTrialBalance rpt = new Reports.CrTrialBalance();
                DataTable dt = ReportQuery.TrialBalance (dtpFDate.Value, dtpTDate.Value);
                rpt.SetDataSource(dt);
                rpt.SetParameterValue("@companyname", CompanyInfo.CompanyName);
                rpt.SetParameterValue("@Fdate", dtpFDate.Value);
                rpt.SetParameterValue("@Tdate", dtpTDate.Value);
                frm.rptViewer.ReportSource = rpt;

            }
            #endregion
            #region Balance Sheet
            if (Reportname == "Balance Sheet")
            {
                Reports.BalanceSheet rpt = new Reports.BalanceSheet();
                DataTable dt = ReportQuery.BalanceSheet(dtpAsOn.Value);
                rpt.SetDataSource(dt);
                rpt.SetParameterValue("@companyname", CompanyInfo.CompanyName);
                rpt.SetParameterValue("@Tdate", dtpAsOn.Value);
                frm.rptViewer.ReportSource = rpt;

            }
            #endregion
            #region Ur Balance Sheet
            if (Reportname == "Balance Sheet (Urdu)")
            {
                UrduReports.CrUrBalanceSheet rpt = new UrduReports.CrUrBalanceSheet();
                DataTable dt = ReportQuery.BalanceSheet(dtpAsOn.Value);
                rpt.SetDataSource(dt);
                rpt.SetParameterValue("@companyname", CompanyInfo.UrCompanyName);
                rpt.SetParameterValue("@Tdate", dtpAsOn.Value);
                frm.rptViewer.ReportSource = rpt;

            }
            #endregion
            #region Income Summery
            if (Reportname == "Income Summery")
            {
                Reports.IncomeSummery rpt = new Reports.IncomeSummery();
                DataTable dt = ReportQuery.IncomeSummery(dtpFDate.Value, dtpTDate.Value);
                rpt.SetDataSource(dt); 
                rpt.SetParameterValue("@companyname", CompanyInfo.CompanyName);
                rpt.SetParameterValue("@Fdate", dtpFDate.Value);
                rpt.SetParameterValue("@Tdate", dtpTDate.Value);
                frm.rptViewer.ReportSource = rpt;

            }
            #endregion
            #region Balance Detail
            else if (Reportname == "Balance Detail")
            {
                Reports.CrBalance rpt = new Reports.CrBalance();
                DataTable dt = ReportQuery.Balance((string)cmbAccount.SelectedValue, dtpAsOn.Value);
                rpt.SetDataSource(dt);
                rpt.SetParameterValue("@tdate", dtpAsOn.Value);
                rpt.SetParameterValue("@Account", ((DataRowView)cmbAccount.SelectedItem)["Title"].ToString());
                rpt.SetParameterValue("@CompanyName", CompanyInfo.CompanyName);
                frm.rptViewer.ReportSource = rpt;

            }
            #endregion
            #region Ur Customer Bill
            else if (Reportname == "Customer Bill (Urdu)")
            {
                UrduReports.CrUrSaleBill rpt = new UrduReports.CrUrSaleBill();
                DataTable dt = ReportQuery.CustomerBill((string)cmbAccount.SelectedValue, dtpFDate.Value, dtpTDate.Value).Tables[0];
                rpt.SetDataSource(dt);
                rpt.SetParameterValue("@companyname", CompanyInfo.UrCompanyName);
                rpt.SetParameterValue("@Account", cmbAccount.Text);
                rpt.SetParameterValue("@Fdate", dtpFDate.Value);
                rpt.SetParameterValue("@Tdate", dtpTDate.Value);
                frm.rptViewer.ReportSource = rpt;

            }
            #endregion
            #region Customer Bill Date Range
            else if (Reportname == "Customer Bill Date Range")
            {
                if (rdoViewReport.Checked)
                {
                    if (cmbAccount.SelectedValue == null)
                    {
                        MessageBox.Show("Please select account..");
                        return;
                    }
                    DataSet ds = ReportQuery.CustomerBill((string)cmbAccount.SelectedValue, dtpFDate.Value, dtpTDate.Value);
                    DataTable dt = ds.Tables[0];
                    Reports.SaleReceiptBill rpt = new Reports.SaleReceiptBill();
                    rpt.SetDataSource(dt);
                    rpt.SetParameterValue("@CompanyName", CompanyInfo.CompanyName);
                    rpt.SetParameterValue("@ServerDate", DateTime.Now);
                    rpt.SetParameterValue("@User", UserInfo.UserName);
                    rpt.SetParameterValue("@Address", CompanyInfo.Address);
                    rpt.SetParameterValue("@ContactHeader", CompanyInfo.ContactHead);
                    rpt.SetParameterValue("@Account", cmbAccount.Text);
                    rpt.SetParameterValue("@FromDate", dtpFDate.Value);
                    rpt.SetParameterValue("@ToDate", dtpTDate.Value);
                    rpt.SetParameterValue("@PreviousBalance", (decimal)ds.Tables[1].Rows[0]["PreviousBalance"]);
                    rpt.SetParameterValue("@Payment", (decimal)ds.Tables[1].Rows[0]["Payment"]);
                    rpt.SetParameterValue("@NetBalance", (decimal)ds.Tables[1].Rows[0]["Balance"]);
                    frm.rptViewer.ReportSource = rpt;
                }
                else
                {
                    if (chklstAccounts.CheckedItems.Count == 0)
                    {
                        MessageBox.Show("Please select at least one customer from the list.");
                        return;
                    }
                    
                    Cursor.Current = Cursors.WaitCursor;
                    int printedCount = 0;
                    
                    foreach (DataRowView item in chklstAccounts.CheckedItems)
                    {
                        DataSet ds = ReportQuery.CustomerBill(item["Code"].ToString(), dtpFDate.Value, dtpTDate.Value);
                        DataTable dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            Reports.SaleReceiptBill rpt = new Reports.SaleReceiptBill();
                            rpt.SetDataSource(dt);
                            rpt.SetParameterValue("@CompanyName", CompanyInfo.CompanyName);
                            rpt.SetParameterValue("@ServerDate", DateTime.Now);
                            rpt.SetParameterValue("@User", UserInfo.UserName);
                            rpt.SetParameterValue("@Address", CompanyInfo.Address);
                            rpt.SetParameterValue("@ContactHeader", CompanyInfo.ContactHead);
                            rpt.SetParameterValue("@Account", item["Title"].ToString());
                            rpt.SetParameterValue("@FromDate", dtpFDate.Value);
                            rpt.SetParameterValue("@ToDate", dtpTDate.Value);
                            rpt.SetParameterValue("@PreviousBalance", ds.Tables[1].Rows[0]["PreviousBalance"]);
                            rpt.SetParameterValue("@Payment", ds.Tables[1].Rows[0]["Payment"]);
                            rpt.SetParameterValue("@NetBalance", ds.Tables[1].Rows[0]["Balance"]);

                            rpt.PrintOptions.PrinterName = ConfigInfo.ThermalPrinterName;
                            rpt.PrintToPrinter(1, true, 0, 0);
                            printedCount++;
                        }
                    }
                    
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show(string.Format("{0} customer bill(s) printed successfully!", printedCount));
                    return;
                }
            }
            #endregion
            #region Envelope
            else if (Reportname == "Envelope")
            {
                Reports.Envelope rpt = new Reports.Envelope();
                //DataRow[] a=  chklstAccounts.CheckedItems.OfType<DataRow >().ToArray();
                string[] Accounts = new string[chklstAccounts.CheckedItems.Count];
                for (int i = 0; i < chklstAccounts.CheckedItems.Count; i++)
                {
                    Accounts[i] = ((DataRowView )chklstAccounts.CheckedItems[i])["Code"].ToString() ;
                }
                DataTable dt = ReportQuery.EnvelopeDetail(Accounts);
                rpt.SetDataSource(dt);
                frm.rptViewer.ReportSource = rpt;

            }
            #endregion
            #region Barcode
            else if (Reportname == "Barcode")
            {
                if (cmbItem.SelectedValue == null)
                {
                    MessageBox.Show("Please select item..");
                    return;
                }

                string id = cmbItem.SelectedValue.ToString();
                var item = System.Threading.Tasks.Task.Run(async () =>
                {
                    var items = await _inventoryApiService.GetItemsAsync(null);
                    return items.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
                }).GetAwaiter().GetResult();

                if (item == null)
                {
                    MessageBox.Show("Selected item not found.");
                    return;
                }

                string path = Application.StartupPath + @"\\barcode" + id + ".png";
                File.Delete(path);
                Barcode.GetBarcode(85, 120, item.Barcode ?? string.Empty, path);
                Reports.rpt_itemLabel rpt = new Reports.rpt_itemLabel();

                rpt.SetParameterValue("@Location", path);
                rpt.SetParameterValue("@Label", item.Title ?? string.Empty);
                rpt.SetParameterValue("@Barcode", item.Barcode ?? string.Empty);
                rpt.SetParameterValue("@Rate", "Cd:" + item.PriRate.ToString("N0"));
                frm.rptViewer.ReportSource = rpt;

            }
            #endregion
            #region Shipment Label Tag
            else if (Reportname == "Shipment Label Tag")
            {
                if (cmbAccount.SelectedValue == null)
                {
                    MessageBox.Show("Please select account..");
                    return;
                }

                string id = cmbAccount.SelectedValue.ToString();
                var customer = System.Threading.Tasks.Task.Run(async () =>
                {
                    var customers = await _customerApiService.GetAsync();
                    return customers.FirstOrDefault(x => string.Equals(x.Account, id, StringComparison.OrdinalIgnoreCase));
                }).GetAwaiter().GetResult();

                Reports.rpt_ShipmentTag rpt = new Reports.rpt_ShipmentTag();

                rpt.SetParameterValue("@CustomerName", cmbAccount.Text);
                rpt.SetParameterValue("@CustomerAddress", customer != null ? (customer.Address ?? string.Empty) : string.Empty);
                rpt.SetParameterValue("@CustomerPhone", customer != null ? (customer.Phone1 ?? string.Empty) : string.Empty);
                rpt.SetParameterValue("@CompanyName", CompanyInfo.CompanyName);
                rpt.SetParameterValue("@CompanyAddress", CompanyInfo.Address);
                rpt.SetParameterValue("@CompanyPhone", CompanyInfo.Cell);
                frm.rptViewer.ReportSource = rpt;

            }
            #endregion
            frm.Show();
        }

        private async void frmReportParameters_Load(object sender, EventArgs e)
        {
            try
            {
                lblReport.Text = Reportname;
                Paramname = Paramname == "" ? Reportname : Paramname;
                #region Account Statement
                if (Paramname == "Account Statement")
                {
                    await FillAccouontAsync();
                    ManageControls(new Control[] { grpDateRange, grpPrintOption, grpAccounts });
                    await FillCheckedAccountsAsync(false);
                }
                #endregion
                #region Account Statement With Due Days
                if (Paramname == "Account Statement With Due Days")
                {
                    await FillAccouontAsync();
                    ManageControls(new Control[] { grpDateRange, grpPrintOption, grpAccounts });
                    await FillCheckedAccountsAsync(false);
                }

                #endregion
                #region Stock Balance
                if (Paramname == "Stock Balance")
                {
                    await FillItemCategoriesAsync(); 
                    ManageControls(new Control[] { grpDateRange,grpItemCatagory,grpFilter });
                    cmbFilter.SelectedIndex = 0; 
                }
                #endregion
                #region Stock Ledger
                if (Paramname == "Stock Ledger")
                {
                    await FillItemsAsync();
                    ManageControls(new Control[] { grpDateRange, grpItem });
                }
                #endregion
                #region Trial Balance
                if (Paramname == "Trial Balance")
                {
                    ManageControls(new Control[] { grpDateRange});
                }
                #endregion
                #region Balance Sheet
                if (Paramname == "Balance Sheet")
                {
                    ManageControls(new Control[] { grpAsOn});                
                }
                #endregion
                #region Income Summery
                if (Paramname == "Income Summery")
                {
                    ManageControls(new Control[] { grpDateRange});
                    }
                #endregion          
                #region Customer Bill
                if (Paramname == "Customer Bill")
                {
                    await FillCustomersAsync();
                    grpDateRange.Visible = true;
                    grpDateRange.Location = new Point(12, 60);
                    grpAccounts.Visible = true;
                    grpAccounts.Location = new Point(12, 120);
                    btnPrint.Location = new Point(btnPrint.Location.X, 180);
                    this.Size = new Size(this.Size.Width, 250);
                }
                #endregion
                #region Customer Bill Date Range
                if (Paramname == "Customer Bill Date Range")
                {
                    await FillCustomersAsync();
                    await FillCheckedAccountsAsync(true);
                    await FillSupplyOrdersAsync();
                    ManageControls(new Control[] { grpDateRange, grpPrintOption, grpAccounts });
                }
                #endregion
                #region Balance Detail
                else if (Reportname == "Balance Detail")
                {
                    await FillLvl4AccountHeadsAsync();
                    grpAccounts.Text = "Accounts Head";
                    ManageControls(new Control[] { grpAsOn, grpAccounts });
                }
                #endregion
                #region Envelope
                else if (Reportname == "Envelope")
                {
                    await FillCheckedAccountsAsync(true);
                    ManageControls(new Control[] { grpSelectedAccounts });
                }
                #endregion
                #region Barcode
                else if (Reportname == "Barcode")
                {
                    await FillItemCategoriesAsync();
                    await FillItemsAsync();
                    ManageControls(new Control[] { grpItem });
                }
                #endregion
                #region Shipment Label Tag
                else if (Reportname == "Shipment Label Tag")
                {
                    await FillCustomersAsync();
                    ManageControls(new Control[] { grpAccounts });
                }
                #endregion
                FLogIn = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFilter .SelectedValue == "All")
            {
                txtQty.Enabled = false;
                txtQty.Text = "";
            }
            else 
                txtQty.Enabled = true;            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rdoViewReport_CheckedChanged(object sender, EventArgs e)
        {
            if (Reportname == "Account Statement" )
            {
                if (!rdoViewReport.Checked)
                {
                    grpSelectedAccounts.Visible = true;
                    grpAccounts.Visible = false;                 
                    ManageControls(new Control[] { grpDateRange, grpPrintOption , grpSelectedAccounts });
                }
                else
                {
                    grpSelectedAccounts.Visible = false;
                    grpAccounts.Visible = true;                 
                    ManageControls(new Control[] { grpDateRange, grpPrintOption, grpAccounts });
                }

            }
            else if (Reportname == "Customer Bill Date Range")
            {
                if (!rdoViewReport.Checked)
                {
                    grpSelectedAccounts.Visible = true;
                    grpAccounts.Visible = false;
                    ManageControls(new Control[] { grpDateRange, grpPrintOption, grpSelectedAccounts });
                }
                else
                {
                    grpSelectedAccounts.Visible = false;
                    grpAccounts.Visible = true;
                    ManageControls(new Control[] { grpDateRange, grpPrintOption, grpAccounts });
                }
            }
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < chklstAccounts.Items.Count; i++)
            {
                chklstAccounts.SetItemChecked(i, chkSelectAll.Checked);

            }
        }

        private async void cmbSupplyOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSupplyOrder.SelectedIndex <= 0 || cmbSupplyOrder.SelectedValue == null)
                return;

            string idStr = cmbSupplyOrder.SelectedValue.ToString();
            int id = 0;
            if (!int.TryParse(idStr, out id)) return;

            // Uncheck all first
            for (int i = 0; i < chklstAccounts.Items.Count; i++)
            {
                chklstAccounts.SetItemChecked(i, false);
            }
            chkSelectAll.Checked = false;

            var order = await _supplyOrderApiService.GetByIdAsync(id);
            if (order != null && order.Details != null)
            {
                var customerIds = order.Details.Select(d => d.CustomerId).ToList();

                for (int i = 0; i < chklstAccounts.Items.Count; i++)
                {
                    var rowView = chklstAccounts.Items[i] as DataRowView;
                    if (rowView != null)
                    {
                        string accountCode = rowView["Code"].ToString();
                        if (customerIds.Contains(accountCode))
                        {
                            chklstAccounts.SetItemChecked(i, true);
                        }
                    }
                }
            }
        }


    
        

        
    }
}
