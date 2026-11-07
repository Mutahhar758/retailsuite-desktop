using System.Drawing;
using System.Windows.Forms;

namespace ERP.Forms
{
    partial class frmDailyStockClosing
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblStatus;
        private Label lblHeaderShortcuts;

        private TabControl tbClosingQuery;
        private TabPage tbDetail;
        private TabPage tbQuery;

        private GroupBox grpHeader;
        private Label lblDate;
        private DateTimePicker dtpDate;
        private Label lblCustomer;
        private ComboBox cmbCustomer;
        private Label lblVendor;
        private ComboBox cmbVendor;
        private Label lblCashAcc;
        private ComboBox cmbCashAccount;

        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblSearch;
        private TextBox txtSearch;
        private Label lblRemarks;
        private TextBox txtRemarks;
        private CheckBox chkChangedOnly;

        private DataGridView dgvStock;
        private DataGridViewTextBoxColumn colItemId;
        private DataGridViewTextBoxColumn colTitle;
        private DataGridViewTextBoxColumn colUnit;
        private DataGridViewTextBoxColumn colOpeningQty;
        private DataGridViewTextBoxColumn colClosingQty;
        private DataGridViewTextBoxColumn colPurchasedQty;
        private DataGridViewTextBoxColumn colSoldQty;
        private DataGridViewTextBoxColumn colPurchaseRate;
        private DataGridViewTextBoxColumn colSaleRate;
        private DataGridViewTextBoxColumn colCalcBuyAmt;
        private DataGridViewTextBoxColumn colCalcSaleAmt;

        private TableLayoutPanel tblBottom;
        private GroupBox grpSalesSummary;
        private FlowLayoutPanel flpSalesSummary;
        private Label lblCalcSaleTitle;
        private TextBox txtCalcSaleVal;
        private Label lblActualSaleTitle;
        private TextBox txtActualSale;
        private Button btnResetSaleTotal;
        private Label lblSaleVarianceTitle;
        private TextBox txtSaleVarianceVal;

        private GroupBox grpBuySummary;
        private FlowLayoutPanel flpBuySummary;
        private Label lblCalcBuyTitle;
        private TextBox txtCalcBuyVal;
        private Label lblActualBuyTitle;
        private TextBox txtActualBuy;
        private Button btnResetBuyTotal;
        private Label lblBuyVarianceTitle;
        private TextBox txtBuyVarianceVal;

        private Panel panel1;
        private Button btnSave;
        private Button btnNew;
        private Button btnReload;
        private Button btnHistory;
        private Button btnDelete;
        private Button btnClose;
        private Label lblShortcutsBar;

        // Search Closings Tab Controls
        private GroupBox grpQueryFilter;
        private Label lblFromDate;
        private DateTimePicker dtpFromDate;
        private Label lblToDate;
        private DateTimePicker dtpToDate;
        private Button btnFind;
        private DataGridView dgvQuery;
        private DataGridViewTextBoxColumn colQDate;
        private DataGridViewTextBoxColumn colQPuNo;
        private DataGridViewTextBoxColumn colQPuAmt;
        private DataGridViewTextBoxColumn colQSlNo;
        private DataGridViewTextBoxColumn colQSlAmt;
        private DataGridViewTextBoxColumn colQRemarks;
        private DataGridViewTextBoxColumn colQCreatedBy;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new Panel();
            this.lblTitle = new Label();
            this.lblSubtitle = new Label();
            this.lblStatus = new Label();
            this.lblHeaderShortcuts = new Label();

            this.tbClosingQuery = new TabControl();
            this.tbDetail = new TabPage();
            this.tbQuery = new TabPage();

            this.grpHeader = new GroupBox();
            this.lblDate = new Label();
            this.dtpDate = new DateTimePicker();
            this.lblCustomer = new Label();
            this.cmbCustomer = new ComboBox();
            this.lblVendor = new Label();
            this.cmbVendor = new ComboBox();
            this.lblCashAcc = new Label();
            this.cmbCashAccount = new ComboBox();
            this.lblCategory = new Label();
            this.cmbCategory = new ComboBox();
            this.lblSearch = new Label();
            this.txtSearch = new TextBox();
            this.lblRemarks = new Label();
            this.txtRemarks = new TextBox();
            this.chkChangedOnly = new CheckBox();

            this.dgvStock = new DataGridView();
            this.colItemId = new DataGridViewTextBoxColumn();
            this.colTitle = new DataGridViewTextBoxColumn();
            this.colUnit = new DataGridViewTextBoxColumn();
            this.colOpeningQty = new DataGridViewTextBoxColumn();
            this.colClosingQty = new DataGridViewTextBoxColumn();
            this.colPurchasedQty = new DataGridViewTextBoxColumn();
            this.colSoldQty = new DataGridViewTextBoxColumn();
            this.colPurchaseRate = new DataGridViewTextBoxColumn();
            this.colSaleRate = new DataGridViewTextBoxColumn();
            this.colCalcBuyAmt = new DataGridViewTextBoxColumn();
            this.colCalcSaleAmt = new DataGridViewTextBoxColumn();

            this.tblBottom = new TableLayoutPanel();
            this.grpSalesSummary = new GroupBox();
            this.flpSalesSummary = new FlowLayoutPanel();
            this.lblCalcSaleTitle = new Label();
            this.txtCalcSaleVal = new TextBox();
            this.lblActualSaleTitle = new Label();
            this.txtActualSale = new TextBox();
            this.btnResetSaleTotal = new Button();
            this.lblSaleVarianceTitle = new Label();
            this.txtSaleVarianceVal = new TextBox();

            this.grpBuySummary = new GroupBox();
            this.flpBuySummary = new FlowLayoutPanel();
            this.lblCalcBuyTitle = new Label();
            this.txtCalcBuyVal = new TextBox();
            this.lblActualBuyTitle = new Label();
            this.txtActualBuy = new TextBox();
            this.btnResetBuyTotal = new Button();
            this.lblBuyVarianceTitle = new Label();
            this.txtBuyVarianceVal = new TextBox();

            this.panel1 = new Panel();
            this.btnSave = new Button();
            this.btnNew = new Button();
            this.btnReload = new Button();
            this.btnHistory = new Button();
            this.btnDelete = new Button();
            this.btnClose = new Button();
            this.lblShortcutsBar = new Label();

            this.grpQueryFilter = new GroupBox();
            this.lblFromDate = new Label();
            this.dtpFromDate = new DateTimePicker();
            this.lblToDate = new Label();
            this.dtpToDate = new DateTimePicker();
            this.btnFind = new Button();
            this.dgvQuery = new DataGridView();
            this.colQDate = new DataGridViewTextBoxColumn();
            this.colQPuNo = new DataGridViewTextBoxColumn();
            this.colQPuAmt = new DataGridViewTextBoxColumn();
            this.colQSlNo = new DataGridViewTextBoxColumn();
            this.colQSlAmt = new DataGridViewTextBoxColumn();
            this.colQRemarks = new DataGridViewTextBoxColumn();
            this.colQCreatedBy = new DataGridViewTextBoxColumn();

            this.SuspendLayout();

            // 
            // pnlHeader (matches frmNormalSale white top header)
            // 
            this.pnlHeader.BackColor = Color.White;
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 54;
            this.pnlHeader.Location = new Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";

            this.lblTitle.AutoSize = true;
            this.lblTitle.UseMnemonic = false;
            this.lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(15, 23, 42);
            this.lblTitle.Location = new Point(16, 7);
            this.lblTitle.Text = "Daily Stock & Cash Closing";

            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.UseMnemonic = false;
            this.lblSubtitle.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular);
            this.lblSubtitle.ForeColor = Color.FromArgb(100, 116, 139);
            this.lblSubtitle.Location = new Point(18, 31);
            this.lblSubtitle.Text = "Physical Stock Count & Day-End Cash Summary";

            this.lblStatus.AutoSize = true;
            this.lblStatus.UseMnemonic = false;
            this.lblStatus.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
            this.lblStatus.Location = new Point(290, 12);
            this.lblStatus.Text = "New Daily Closing";

            this.lblHeaderShortcuts.Dock = DockStyle.Right;
            this.lblHeaderShortcuts.AutoSize = false;
            this.lblHeaderShortcuts.Width = 480;
            this.lblHeaderShortcuts.Padding = new Padding(0, 0, 16, 0);
            this.lblHeaderShortcuts.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
            this.lblHeaderShortcuts.ForeColor = Color.FromArgb(71, 85, 105);
            this.lblHeaderShortcuts.Text = "Shortcuts: [F3] New / Today  •  [F5] Save Closing  •  [F9] Search Closings";
            this.lblHeaderShortcuts.TextAlign = ContentAlignment.MiddleRight;

            this.pnlHeader.Controls.Add(this.lblHeaderShortcuts);
            this.pnlHeader.Controls.Add(this.lblStatus);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblTitle);

            // 
            // tbClosingQuery (matches frmNormalSale TabControl)
            // 
            this.tbClosingQuery.Dock = DockStyle.Fill;
            this.tbClosingQuery.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.tbClosingQuery.Name = "tbClosingQuery";
            this.tbClosingQuery.SelectedIndex = 0;

            // 
            // tbDetail (Tab 1: Closing Sheet Entry)
            // 
            this.tbDetail.BackColor = Color.FromArgb(248, 250, 252);
            this.tbDetail.Padding = new Padding(8);
            this.tbDetail.Text = "Closing Sheet Entry";

            // 
            // grpHeader (matches grpInvoiceDetail in frmNormalSale)
            // 
            this.grpHeader.Dock = DockStyle.Top;
            this.grpHeader.Height = 104;
            this.grpHeader.BackColor = Color.White;
            this.grpHeader.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.grpHeader.ForeColor = Color.FromArgb(30, 41, 59);
            this.grpHeader.Text = " Closing Details && Filters ";

            var labelFont = new Font("Segoe UI", 9F, FontStyle.Regular);
            var labelColor = Color.FromArgb(71, 85, 105);
            var inputFont = new Font("Segoe UI", 9F, FontStyle.Regular);

            this.lblDate.Text = "Date:";
            this.lblDate.Font = labelFont;
            this.lblDate.ForeColor = labelColor;
            this.lblDate.Location = new Point(14, 28);
            this.lblDate.AutoSize = true;
            this.dtpDate.Format = DateTimePickerFormat.Short;
            this.dtpDate.Font = inputFont;
            this.dtpDate.Location = new Point(58, 24);
            this.dtpDate.Size = new Size(125, 23);

            this.lblCustomer.Text = "Walk-In Customer:";
            this.lblCustomer.Font = labelFont;
            this.lblCustomer.ForeColor = labelColor;
            this.lblCustomer.Location = new Point(198, 28);
            this.lblCustomer.AutoSize = true;
            this.cmbCustomer.Font = inputFont;
            this.cmbCustomer.Location = new Point(310, 24);
            this.cmbCustomer.Size = new Size(250, 23);
            this.cmbCustomer.DropDownStyle = ComboBoxStyle.DropDownList;

            this.lblVendor.Text = "Default Supplier:";
            this.lblVendor.Font = labelFont;
            this.lblVendor.ForeColor = labelColor;
            this.lblVendor.Location = new Point(576, 28);
            this.lblVendor.AutoSize = true;
            this.cmbVendor.Font = inputFont;
            this.cmbVendor.Location = new Point(678, 24);
            this.cmbVendor.Size = new Size(250, 23);
            this.cmbVendor.DropDownStyle = ComboBoxStyle.DropDownList;

            this.lblCashAcc.Text = "Cash Account:";
            this.lblCashAcc.Font = labelFont;
            this.lblCashAcc.ForeColor = labelColor;
            this.lblCashAcc.Location = new Point(944, 28);
            this.lblCashAcc.AutoSize = true;
            this.cmbCashAccount.Font = inputFont;
            this.cmbCashAccount.Location = new Point(1032, 24);
            this.cmbCashAccount.Size = new Size(250, 23);
            this.cmbCashAccount.DropDownStyle = ComboBoxStyle.DropDownList;

            this.lblCategory.Text = "Category:";
            this.lblCategory.Font = labelFont;
            this.lblCategory.ForeColor = labelColor;
            this.lblCategory.Location = new Point(14, 64);
            this.lblCategory.AutoSize = true;
            this.cmbCategory.Font = inputFont;
            this.cmbCategory.Location = new Point(78, 60);
            this.cmbCategory.Size = new Size(210, 23);
            this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            this.lblSearch.Text = "Search:";
            this.lblSearch.Font = labelFont;
            this.lblSearch.ForeColor = labelColor;
            this.lblSearch.Location = new Point(308, 64);
            this.lblSearch.AutoSize = true;
            this.txtSearch.Font = inputFont;
            this.txtSearch.Location = new Point(358, 60);
            this.txtSearch.Size = new Size(250, 23);

            this.lblRemarks.Text = "Description:";
            this.lblRemarks.Font = labelFont;
            this.lblRemarks.ForeColor = labelColor;
            this.lblRemarks.Location = new Point(624, 64);
            this.lblRemarks.AutoSize = true;
            this.txtRemarks.Font = inputFont;
            this.txtRemarks.Location = new Point(698, 60);
            this.txtRemarks.Size = new Size(230, 23);

            this.chkChangedOnly.Text = "Show Changed Items Only";
            this.chkChangedOnly.Font = labelFont;
            this.chkChangedOnly.ForeColor = labelColor;
            this.chkChangedOnly.Location = new Point(948, 62);
            this.chkChangedOnly.AutoSize = true;

            this.grpHeader.Controls.AddRange(new Control[] {
                lblDate, dtpDate, lblCustomer, cmbCustomer, lblVendor, cmbVendor,
                lblCashAcc, cmbCashAccount, lblCategory, cmbCategory, lblSearch, txtSearch,
                lblRemarks, txtRemarks, chkChangedOnly
            });

            // 
            // tblBottom (Bill Summary style matching groupBox3 in frmNormalSale)
            // 
            this.tblBottom.Dock = DockStyle.Bottom;
            this.tblBottom.Height = 82;
            this.tblBottom.ColumnCount = 2;
            this.tblBottom.RowCount = 1;
            this.tblBottom.Padding = new Padding(0, 6, 0, 6);
            this.tblBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tblBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tblBottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Left Panel: Daily Sales Summary
            this.grpSalesSummary.Dock = DockStyle.Fill;
            this.grpSalesSummary.BackColor = Color.White;
            this.grpSalesSummary.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.grpSalesSummary.ForeColor = Color.FromArgb(30, 41, 59);
            this.grpSalesSummary.Text = " Daily Sales Summary ";
            this.grpSalesSummary.Margin = new Padding(0, 0, 6, 0);

            this.flpSalesSummary.Dock = DockStyle.Fill;
            this.flpSalesSummary.FlowDirection = FlowDirection.LeftToRight;
            this.flpSalesSummary.WrapContents = false;
            this.flpSalesSummary.Padding = new Padding(8, 8, 8, 4);

            this.lblCalcSaleTitle.Text = "Calculated Sale:";
            this.lblCalcSaleTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblCalcSaleTitle.ForeColor = Color.FromArgb(71, 85, 105);
            this.lblCalcSaleTitle.AutoSize = true;
            this.lblCalcSaleTitle.Margin = new Padding(0, 6, 4, 0);

            this.txtCalcSaleVal.ReadOnly = true;
            this.txtCalcSaleVal.TabStop = false;
            this.txtCalcSaleVal.BackColor = Color.FromArgb(241, 245, 249);
            this.txtCalcSaleVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.txtCalcSaleVal.ForeColor = Color.FromArgb(15, 23, 42);
            this.txtCalcSaleVal.Size = new Size(125, 26);
            this.txtCalcSaleVal.TextAlign = HorizontalAlignment.Right;
            this.txtCalcSaleVal.Text = "0.00";
            this.txtCalcSaleVal.Margin = new Padding(0, 2, 16, 0);

            this.lblActualSaleTitle.Text = "Actual Sale:";
            this.lblActualSaleTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblActualSaleTitle.ForeColor = Color.FromArgb(22, 101, 52);
            this.lblActualSaleTitle.AutoSize = true;
            this.lblActualSaleTitle.Margin = new Padding(0, 6, 4, 0);

            this.txtActualSale.BackColor = Color.FromArgb(240, 253, 244);
            this.txtActualSale.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.txtActualSale.ForeColor = Color.FromArgb(22, 101, 52);
            this.txtActualSale.Size = new Size(125, 26);
            this.txtActualSale.TextAlign = HorizontalAlignment.Right;
            this.txtActualSale.Text = "0.00";
            this.txtActualSale.Margin = new Padding(0, 2, 4, 0);

            this.btnResetSaleTotal.Text = "Sync";
            this.btnResetSaleTotal.BackColor = Color.FromArgb(241, 245, 249);
            this.btnResetSaleTotal.FlatStyle = FlatStyle.Flat;
            this.btnResetSaleTotal.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            this.btnResetSaleTotal.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular);
            this.btnResetSaleTotal.ForeColor = Color.FromArgb(30, 41, 59);
            this.btnResetSaleTotal.Size = new Size(48, 26);
            this.btnResetSaleTotal.Margin = new Padding(0, 2, 16, 0);

            this.lblSaleVarianceTitle.Text = "Disc / Var:";
            this.lblSaleVarianceTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblSaleVarianceTitle.ForeColor = Color.FromArgb(185, 28, 28);
            this.lblSaleVarianceTitle.AutoSize = true;
            this.lblSaleVarianceTitle.Margin = new Padding(0, 6, 4, 0);

            this.txtSaleVarianceVal.ReadOnly = true;
            this.txtSaleVarianceVal.TabStop = false;
            this.txtSaleVarianceVal.BackColor = Color.FromArgb(254, 242, 242);
            this.txtSaleVarianceVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.txtSaleVarianceVal.ForeColor = Color.FromArgb(185, 28, 28);
            this.txtSaleVarianceVal.Size = new Size(105, 26);
            this.txtSaleVarianceVal.TextAlign = HorizontalAlignment.Right;
            this.txtSaleVarianceVal.Text = "0.00";
            this.txtSaleVarianceVal.Margin = new Padding(0, 2, 0, 0);

            this.flpSalesSummary.Controls.AddRange(new Control[] {
                lblCalcSaleTitle, txtCalcSaleVal, lblActualSaleTitle, txtActualSale,
                btnResetSaleTotal, lblSaleVarianceTitle, txtSaleVarianceVal
            });
            this.grpSalesSummary.Controls.Add(this.flpSalesSummary);
            this.tblBottom.Controls.Add(this.grpSalesSummary, 0, 0);

            // Right Panel: Daily Buying Summary
            this.grpBuySummary.Dock = DockStyle.Fill;
            this.grpBuySummary.BackColor = Color.White;
            this.grpBuySummary.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.grpBuySummary.ForeColor = Color.FromArgb(30, 41, 59);
            this.grpBuySummary.Text = " Daily Buying Summary ";
            this.grpBuySummary.Margin = new Padding(6, 0, 0, 0);

            this.flpBuySummary.Dock = DockStyle.Fill;
            this.flpBuySummary.FlowDirection = FlowDirection.LeftToRight;
            this.flpBuySummary.WrapContents = false;
            this.flpBuySummary.Padding = new Padding(8, 8, 8, 4);

            this.lblCalcBuyTitle.Text = "Calculated Buy:";
            this.lblCalcBuyTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblCalcBuyTitle.ForeColor = Color.FromArgb(71, 85, 105);
            this.lblCalcBuyTitle.AutoSize = true;
            this.lblCalcBuyTitle.Margin = new Padding(0, 6, 4, 0);

            this.txtCalcBuyVal.ReadOnly = true;
            this.txtCalcBuyVal.TabStop = false;
            this.txtCalcBuyVal.BackColor = Color.FromArgb(241, 245, 249);
            this.txtCalcBuyVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.txtCalcBuyVal.ForeColor = Color.FromArgb(15, 23, 42);
            this.txtCalcBuyVal.Size = new Size(125, 26);
            this.txtCalcBuyVal.TextAlign = HorizontalAlignment.Right;
            this.txtCalcBuyVal.Text = "0.00";
            this.txtCalcBuyVal.Margin = new Padding(0, 2, 16, 0);

            this.lblActualBuyTitle.Text = "Actual Buy:";
            this.lblActualBuyTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblActualBuyTitle.ForeColor = Color.FromArgb(37, 99, 235);
            this.lblActualBuyTitle.AutoSize = true;
            this.lblActualBuyTitle.Margin = new Padding(0, 6, 4, 0);

            this.txtActualBuy.BackColor = Color.FromArgb(239, 246, 255);
            this.txtActualBuy.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.txtActualBuy.ForeColor = Color.FromArgb(29, 78, 216);
            this.txtActualBuy.Size = new Size(125, 26);
            this.txtActualBuy.TextAlign = HorizontalAlignment.Right;
            this.txtActualBuy.Text = "0.00";
            this.txtActualBuy.Margin = new Padding(0, 2, 4, 0);

            this.btnResetBuyTotal.Text = "Sync";
            this.btnResetBuyTotal.BackColor = Color.FromArgb(241, 245, 249);
            this.btnResetBuyTotal.FlatStyle = FlatStyle.Flat;
            this.btnResetBuyTotal.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            this.btnResetBuyTotal.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular);
            this.btnResetBuyTotal.ForeColor = Color.FromArgb(30, 41, 59);
            this.btnResetBuyTotal.Size = new Size(48, 26);
            this.btnResetBuyTotal.Margin = new Padding(0, 2, 16, 0);

            this.lblBuyVarianceTitle.Text = "Add / Less:";
            this.lblBuyVarianceTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblBuyVarianceTitle.ForeColor = Color.FromArgb(37, 99, 235);
            this.lblBuyVarianceTitle.AutoSize = true;
            this.lblBuyVarianceTitle.Margin = new Padding(0, 6, 4, 0);

            this.txtBuyVarianceVal.ReadOnly = true;
            this.txtBuyVarianceVal.TabStop = false;
            this.txtBuyVarianceVal.BackColor = Color.FromArgb(241, 245, 249);
            this.txtBuyVarianceVal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.txtBuyVarianceVal.ForeColor = Color.FromArgb(37, 99, 235);
            this.txtBuyVarianceVal.Size = new Size(105, 26);
            this.txtBuyVarianceVal.TextAlign = HorizontalAlignment.Right;
            this.txtBuyVarianceVal.Text = "0.00";
            this.txtBuyVarianceVal.Margin = new Padding(0, 2, 0, 0);

            this.flpBuySummary.Controls.AddRange(new Control[] {
                lblCalcBuyTitle, txtCalcBuyVal, lblActualBuyTitle, txtActualBuy,
                btnResetBuyTotal, lblBuyVarianceTitle, txtBuyVarianceVal
            });
            this.grpBuySummary.Controls.Add(this.flpBuySummary);
            this.tblBottom.Controls.Add(this.grpBuySummary, 1, 0);

            // 
            // panel1 (Bottom Action Bar matching frmNormalSale)
            // 
            this.panel1.Dock = DockStyle.Bottom;
            this.panel1.Height = 58;
            this.panel1.BackColor = Color.White;
            this.panel1.BorderStyle = BorderStyle.FixedSingle;

            this.btnSave.BackColor = Color.FromArgb(22, 163, 74);
            this.btnSave.Cursor = Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.Location = new Point(10, 7);
            this.btnSave.Size = new Size(145, 42);
            this.btnSave.Text = "&Save Closing [F5]";
            this.btnSave.UseVisualStyleBackColor = false;

            this.btnNew.BackColor = Color.FromArgb(241, 245, 249);
            this.btnNew.Cursor = Cursors.Hand;
            this.btnNew.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            this.btnNew.FlatStyle = FlatStyle.Flat;
            this.btnNew.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnNew.ForeColor = Color.FromArgb(30, 41, 59);
            this.btnNew.Location = new Point(161, 7);
            this.btnNew.Size = new Size(125, 42);
            this.btnNew.Text = "&New / Today [F3]";
            this.btnNew.UseVisualStyleBackColor = false;

            this.btnReload.BackColor = Color.FromArgb(241, 245, 249);
            this.btnReload.Cursor = Cursors.Hand;
            this.btnReload.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            this.btnReload.FlatStyle = FlatStyle.Flat;
            this.btnReload.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.btnReload.ForeColor = Color.FromArgb(30, 41, 59);
            this.btnReload.Location = new Point(292, 7);
            this.btnReload.Size = new Size(115, 42);
            this.btnReload.Text = "&Reload Sheet";
            this.btnReload.UseVisualStyleBackColor = false;

            this.btnHistory.BackColor = Color.FromArgb(241, 245, 249);
            this.btnHistory.Cursor = Cursors.Hand;
            this.btnHistory.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            this.btnHistory.FlatStyle = FlatStyle.Flat;
            this.btnHistory.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.btnHistory.ForeColor = Color.FromArgb(30, 41, 59);
            this.btnHistory.Location = new Point(413, 7);
            this.btnHistory.Size = new Size(130, 42);
            this.btnHistory.Text = "Search &History [F9]";
            this.btnHistory.UseVisualStyleBackColor = false;

            this.btnDelete.BackColor = Color.FromArgb(254, 242, 242);
            this.btnDelete.Cursor = Cursors.Hand;
            this.btnDelete.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
            this.btnDelete.FlatStyle = FlatStyle.Flat;
            this.btnDelete.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnDelete.ForeColor = Color.FromArgb(220, 38, 38);
            this.btnDelete.Location = new Point(549, 7);
            this.btnDelete.Size = new Size(90, 42);
            this.btnDelete.Text = "&Delete";
            this.btnDelete.Enabled = false;
            this.btnDelete.UseVisualStyleBackColor = false;

            this.btnClose.BackColor = Color.FromArgb(241, 245, 249);
            this.btnClose.Cursor = Cursors.Hand;
            this.btnClose.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.btnClose.ForeColor = Color.FromArgb(71, 85, 105);
            this.btnClose.Location = new Point(645, 7);
            this.btnClose.Size = new Size(85, 42);
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;

            this.lblShortcutsBar.Dock = DockStyle.Right;
            this.lblShortcutsBar.AutoSize = false;
            this.lblShortcutsBar.Width = 420;
            this.lblShortcutsBar.Padding = new Padding(0, 8, 12, 0);
            this.lblShortcutsBar.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular);
            this.lblShortcutsBar.ForeColor = Color.FromArgb(100, 116, 139);
            this.lblShortcutsBar.Text = "⌨ [F3] Today  •  [F5] Save Closing  •  [F9] Search Closings\r\n• [Enter / ↑↓] Navigate Closing Stock Count";

            this.panel1.Controls.AddRange(new Control[] {
                btnSave, btnNew, btnReload, btnHistory, btnDelete, btnClose, lblShortcutsBar
            });

            // 
            // dgvStock (matches dgvSale styling)
            // 
            this.dgvStock.Dock = DockStyle.Fill;
            this.dgvStock.AllowUserToAddRows = false;
            this.dgvStock.AllowUserToDeleteRows = false;
            this.dgvStock.BackgroundColor = Color.White;
            this.dgvStock.BorderStyle = BorderStyle.Fixed3D;
            this.dgvStock.GridColor = Color.FromArgb(226, 232, 240);
            this.dgvStock.RowHeadersVisible = false;
            this.dgvStock.RowTemplate.Height = 26;
            this.dgvStock.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.colItemId.Name = "colItemId";
            this.colItemId.Visible = false;

            this.colTitle.Name = "colTitle";
            this.colTitle.HeaderText = "Product";
            this.colTitle.ReadOnly = true;
            this.colTitle.FillWeight = 220;

            this.colUnit.Name = "colUnit";
            this.colUnit.HeaderText = "Unit";
            this.colUnit.ReadOnly = true;
            this.colUnit.FillWeight = 65;

            this.colOpeningQty.Name = "colOpeningQty";
            this.colOpeningQty.HeaderText = "Opening Stock";
            this.colOpeningQty.ReadOnly = true;
            this.colOpeningQty.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.colOpeningQty.DefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            this.colOpeningQty.FillWeight = 90;

            this.colClosingQty.Name = "colClosingQty";
            this.colClosingQty.HeaderText = "Closing Stock (Count)";
            this.colClosingQty.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.colClosingQty.DefaultCellStyle.BackColor = Color.FromArgb(254, 249, 195);
            this.colClosingQty.FillWeight = 110;

            this.colPurchasedQty.Name = "colPurchasedQty";
            this.colPurchasedQty.HeaderText = "Purchased Qty";
            this.colPurchasedQty.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.colPurchasedQty.FillWeight = 95;

            this.colSoldQty.Name = "colSoldQty";
            this.colSoldQty.HeaderText = "Sold Qty";
            this.colSoldQty.ReadOnly = true;
            this.colSoldQty.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.colSoldQty.DefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            this.colSoldQty.FillWeight = 85;

            this.colPurchaseRate.Name = "colPurchaseRate";
            this.colPurchaseRate.HeaderText = "Buy Rate";
            this.colPurchaseRate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.colPurchaseRate.FillWeight = 85;

            this.colSaleRate.Name = "colSaleRate";
            this.colSaleRate.HeaderText = "Sale Rate";
            this.colSaleRate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.colSaleRate.FillWeight = 85;

            this.colCalcBuyAmt.Name = "colCalcBuyAmt";
            this.colCalcBuyAmt.HeaderText = "Calc. Buying";
            this.colCalcBuyAmt.ReadOnly = true;
            this.colCalcBuyAmt.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.colCalcBuyAmt.DefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            this.colCalcBuyAmt.FillWeight = 95;

            this.colCalcSaleAmt.Name = "colCalcSaleAmt";
            this.colCalcSaleAmt.HeaderText = "Calc. Sale";
            this.colCalcSaleAmt.ReadOnly = true;
            this.colCalcSaleAmt.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.colCalcSaleAmt.DefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            this.colCalcSaleAmt.FillWeight = 95;

            this.dgvStock.Columns.AddRange(new DataGridViewColumn[] {
                colItemId, colTitle, colUnit, colOpeningQty, colClosingQty,
                colPurchasedQty, colSoldQty, colPurchaseRate, colSaleRate,
                colCalcBuyAmt, colCalcSaleAmt
            });

            this.tbDetail.Controls.Add(this.dgvStock);
            this.tbDetail.Controls.Add(this.tblBottom);
            this.tbDetail.Controls.Add(this.panel1);
            this.tbDetail.Controls.Add(this.grpHeader);

            // 
            // tbQuery (Tab 2: Search Closings — matches tbQuery in frmNormalSale)
            // 
            this.tbQuery.BackColor = Color.FromArgb(248, 250, 252);
            this.tbQuery.Padding = new Padding(8);
            this.tbQuery.Text = "Search Closings";

            this.grpQueryFilter.Dock = DockStyle.Top;
            this.grpQueryFilter.Height = 68;
            this.grpQueryFilter.BackColor = Color.White;
            this.grpQueryFilter.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.grpQueryFilter.ForeColor = Color.FromArgb(30, 41, 59);
            this.grpQueryFilter.Text = " Filter && Search Daily Closings ";

            this.lblFromDate.Text = "From Date:";
            this.lblFromDate.Font = labelFont;
            this.lblFromDate.ForeColor = labelColor;
            this.lblFromDate.Location = new Point(16, 28);
            this.lblFromDate.AutoSize = true;
            this.dtpFromDate.Format = DateTimePickerFormat.Short;
            this.dtpFromDate.Font = inputFont;
            this.dtpFromDate.Location = new Point(88, 24);
            this.dtpFromDate.Size = new Size(125, 23);

            this.lblToDate.Text = "To Date:";
            this.lblToDate.Font = labelFont;
            this.lblToDate.ForeColor = labelColor;
            this.lblToDate.Location = new Point(232, 28);
            this.lblToDate.AutoSize = true;
            this.dtpToDate.Format = DateTimePickerFormat.Short;
            this.dtpToDate.Font = inputFont;
            this.dtpToDate.Location = new Point(288, 24);
            this.dtpToDate.Size = new Size(125, 23);

            this.btnFind.BackColor = Color.FromArgb(37, 99, 235);
            this.btnFind.Cursor = Cursors.Hand;
            this.btnFind.FlatAppearance.BorderSize = 0;
            this.btnFind.FlatStyle = FlatStyle.Flat;
            this.btnFind.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnFind.ForeColor = Color.White;
            this.btnFind.Location = new Point(432, 21);
            this.btnFind.Size = new Size(95, 30);
            this.btnFind.Text = "&Find";
            this.btnFind.UseVisualStyleBackColor = false;

            this.grpQueryFilter.Controls.AddRange(new Control[] {
                lblFromDate, dtpFromDate, lblToDate, dtpToDate, btnFind
            });

            this.dgvQuery.Dock = DockStyle.Fill;
            this.dgvQuery.AllowUserToAddRows = false;
            this.dgvQuery.AllowUserToDeleteRows = false;
            this.dgvQuery.ReadOnly = true;
            this.dgvQuery.BackgroundColor = Color.White;
            this.dgvQuery.BorderStyle = BorderStyle.Fixed3D;
            this.dgvQuery.GridColor = Color.FromArgb(226, 232, 240);
            this.dgvQuery.RowHeadersVisible = false;
            this.dgvQuery.RowTemplate.Height = 26;
            this.dgvQuery.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvQuery.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.colQDate.Name = "colQDate";
            this.colQDate.HeaderText = "Closing Date";
            this.colQDate.FillWeight = 110;

            this.colQPuNo.Name = "colQPuNo";
            this.colQPuNo.HeaderText = "Purchase Voucher";
            this.colQPuNo.FillWeight = 110;

            this.colQPuAmt.Name = "colQPuAmt";
            this.colQPuAmt.HeaderText = "Total Buying";
            this.colQPuAmt.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.colQPuAmt.FillWeight = 120;

            this.colQSlNo.Name = "colQSlNo";
            this.colQSlNo.HeaderText = "Sale Voucher";
            this.colQSlNo.FillWeight = 110;

            this.colQSlAmt.Name = "colQSlAmt";
            this.colQSlAmt.HeaderText = "Total Sales";
            this.colQSlAmt.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.colQSlAmt.FillWeight = 120;

            this.colQRemarks.Name = "colQRemarks";
            this.colQRemarks.HeaderText = "Description";
            this.colQRemarks.FillWeight = 180;

            this.colQCreatedBy.Name = "colQCreatedBy";
            this.colQCreatedBy.HeaderText = "Created By";
            this.colQCreatedBy.FillWeight = 100;

            this.dgvQuery.Columns.AddRange(new DataGridViewColumn[] {
                colQDate, colQPuNo, colQPuAmt, colQSlNo, colQSlAmt, colQRemarks, colQCreatedBy
            });

            this.tbQuery.Controls.Add(this.dgvQuery);
            this.tbQuery.Controls.Add(this.grpQueryFilter);

            this.tbClosingQuery.Controls.Add(this.tbDetail);
            this.tbClosingQuery.Controls.Add(this.tbQuery);

            // 
            // frmDailyStockClosing
            // 
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.ClientSize = new Size(1280, 720);
            this.Controls.Add(this.tbClosingQuery);
            this.Controls.Add(this.pnlHeader);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.KeyPreview = true;
            this.Name = "frmDailyStockClosing";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Daily Stock & Cash Closing";
            this.WindowState = FormWindowState.Maximized;
            this.ResumeLayout(false);
        }
    }
}
