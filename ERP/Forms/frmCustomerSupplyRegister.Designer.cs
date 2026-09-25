namespace ERP
{
    partial class frmCustomerSupplyRegister
    {
        private System.ComponentModel.IContainer components = null;

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
            System.Windows.Forms.DataGridViewCellStyle dgvHeaderStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvAltRowStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvDefaultStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colDateStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colVoucherStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colUnitStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colQtyStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colSecQtyStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colRateStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colSecRateStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colDiscountStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colAddLessStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colAmountStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colSaveStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle colDeleteStyle = new System.Windows.Forms.DataGridViewCellStyle();

            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnSaveAll = new System.Windows.Forms.Button();
            this.btnAddSupplyEntry = new System.Windows.Forms.Button();
            this.btnPrintCustomerBill = new System.Windows.Forms.Button();

            this.pnlFilters = new System.Windows.Forms.Panel();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.lblFromDate = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.lblToDate = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.btnPreset1to10 = new System.Windows.Forms.Button();
            this.btnPreset1to15 = new System.Windows.Forms.Button();
            this.btnPreset1to20 = new System.Windows.Forms.Button();
            this.btnPresetMonth = new System.Windows.Forms.Button();
            this.btnPresetLastMonth = new System.Windows.Forms.Button();
            this.lblItem = new System.Windows.Forms.Label();
            this.cmbItem = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();

            this.pnlKpis = new System.Windows.Forms.Panel();
            this.cardTotalRecords = new System.Windows.Forms.Panel();
            this.lblKpiRecordsTitle = new System.Windows.Forms.Label();
            this.lblKpiRecordsVal = new System.Windows.Forms.Label();
            this.cardTotalQty = new System.Windows.Forms.Panel();
            this.lblKpiQtyTitle = new System.Windows.Forms.Label();
            this.lblKpiQtyVal = new System.Windows.Forms.Label();
            this.cardTotalAmount = new System.Windows.Forms.Panel();
            this.lblKpiAmountTitle = new System.Windows.Forms.Label();
            this.lblKpiAmountVal = new System.Windows.Forms.Label();
            this.cardPending = new System.Windows.Forms.Panel();
            this.lblKpiPendingTitle = new System.Windows.Forms.Label();
            this.lblKpiPendingVal = new System.Windows.Forms.Label();

            this.pnlGridContainer = new System.Windows.Forms.Panel();
            this.dgvRecords = new System.Windows.Forms.DataGridView();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVoucher = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSecQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSecRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDiscount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAddLess = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSave = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();

            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.prgProgress = new System.Windows.Forms.ToolStripProgressBar();

            this.pnlHeader.SuspendLayout();
            this.pnlFilters.SuspendLayout();
            this.pnlKpis.SuspendLayout();
            this.cardTotalRecords.SuspendLayout();
            this.cardTotalQty.SuspendLayout();
            this.cardTotalAmount.SuspendLayout();
            this.cardPending.SuspendLayout();
            this.pnlGridContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();

            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.btnReload);
            this.pnlHeader.Controls.Add(this.btnSaveAll);
            this.pnlHeader.Controls.Add(this.btnAddSupplyEntry);
            this.pnlHeader.Controls.Add(this.btnPrintCustomerBill);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height = 64;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlHeader.Size = new System.Drawing.Size(1264, 64);
            this.pnlHeader.TabIndex = 0;

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(16, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(326, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "CUSTOMER SUPPLY & BILL REGISTER";

            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblSubtitle.Location = new System.Drawing.Point(17, 34);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(460, 15);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Audit, update, and manage daily customer supply deliveries and print customer bills";

            // 
            // btnReload
            // 
            this.btnReload.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnReload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReload.FlatAppearance.BorderSize = 0;
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnReload.ForeColor = System.Drawing.Color.White;
            this.btnReload.Location = new System.Drawing.Point(740, 16);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(75, 32);
            this.btnReload.TabIndex = 5;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = false;

            // 
            // btnSaveAll
            // 
            this.btnSaveAll.BackColor = System.Drawing.Color.FromArgb(217, 119, 6);
            this.btnSaveAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveAll.Enabled = false;
            this.btnSaveAll.FlatAppearance.BorderSize = 0;
            this.btnSaveAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveAll.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnSaveAll.ForeColor = System.Drawing.Color.White;
            this.btnSaveAll.Location = new System.Drawing.Point(823, 16);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(160, 32);
            this.btnSaveAll.TabIndex = 4;
            this.btnSaveAll.Text = "Save All Changes (0)";
            this.btnSaveAll.UseVisualStyleBackColor = false;

            // 
            // btnAddSupplyEntry
            // 
            this.btnAddSupplyEntry.BackColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.btnAddSupplyEntry.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddSupplyEntry.FlatAppearance.BorderSize = 0;
            this.btnAddSupplyEntry.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddSupplyEntry.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnAddSupplyEntry.ForeColor = System.Drawing.Color.White;
            this.btnAddSupplyEntry.Location = new System.Drawing.Point(991, 16);
            this.btnAddSupplyEntry.Name = "btnAddSupplyEntry";
            this.btnAddSupplyEntry.Size = new System.Drawing.Size(105, 32);
            this.btnAddSupplyEntry.TabIndex = 3;
            this.btnAddSupplyEntry.Text = "+ Add Entry";
            this.btnAddSupplyEntry.UseVisualStyleBackColor = false;

            // 
            // btnPrintCustomerBill
            // 
            this.btnPrintCustomerBill.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnPrintCustomerBill.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrintCustomerBill.FlatAppearance.BorderSize = 0;
            this.btnPrintCustomerBill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrintCustomerBill.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnPrintCustomerBill.ForeColor = System.Drawing.Color.White;
            this.btnPrintCustomerBill.Location = new System.Drawing.Point(1104, 16);
            this.btnPrintCustomerBill.Name = "btnPrintCustomerBill";
            this.btnPrintCustomerBill.Size = new System.Drawing.Size(150, 32);
            this.btnPrintCustomerBill.TabIndex = 2;
            this.btnPrintCustomerBill.Text = "Print Customer Bill";
            this.btnPrintCustomerBill.UseVisualStyleBackColor = false;

            // 
            // pnlFilters
            // 
            this.pnlFilters.BackColor = System.Drawing.Color.White;
            this.pnlFilters.Controls.Add(this.lblCustomer);
            this.pnlFilters.Controls.Add(this.cmbCustomer);
            this.pnlFilters.Controls.Add(this.lblFromDate);
            this.pnlFilters.Controls.Add(this.dtpFromDate);
            this.pnlFilters.Controls.Add(this.lblToDate);
            this.pnlFilters.Controls.Add(this.dtpToDate);
            this.pnlFilters.Controls.Add(this.btnPreset1to10);
            this.pnlFilters.Controls.Add(this.btnPreset1to15);
            this.pnlFilters.Controls.Add(this.btnPreset1to20);
            this.pnlFilters.Controls.Add(this.btnPresetMonth);
            this.pnlFilters.Controls.Add(this.btnPresetLastMonth);
            this.pnlFilters.Controls.Add(this.lblItem);
            this.pnlFilters.Controls.Add(this.cmbItem);
            this.pnlFilters.Controls.Add(this.btnSearch);
            this.pnlFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilters.Height = 76;
            this.pnlFilters.Location = new System.Drawing.Point(0, 64);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlFilters.Size = new System.Drawing.Size(1264, 76);
            this.pnlFilters.TabIndex = 1;

            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblCustomer.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblCustomer.Location = new System.Drawing.Point(16, 10);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(122, 12);
            this.lblCustomer.TabIndex = 0;
            this.lblCustomer.Text = "CUSTOMER ACCOUNT *";

            // cmbCustomer
            // 
            this.cmbCustomer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbCustomer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbCustomer.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(16, 28);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(240, 23);
            this.cmbCustomer.TabIndex = 1;

            // 
            // lblFromDate
            // 
            this.lblFromDate.AutoSize = true;
            this.lblFromDate.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblFromDate.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblFromDate.Location = new System.Drawing.Point(266, 10);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(65, 12);
            this.lblFromDate.TabIndex = 2;
            this.lblFromDate.Text = "FROM DATE";

            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd-MMM-yyyy";
            this.dtpFromDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(266, 28);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(110, 23);
            this.dtpFromDate.TabIndex = 3;

            // 
            // lblToDate
            // 
            this.lblToDate.AutoSize = true;
            this.lblToDate.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblToDate.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblToDate.Location = new System.Drawing.Point(386, 10);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(49, 12);
            this.lblToDate.TabIndex = 4;
            this.lblToDate.Text = "TO DATE";

            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "dd-MMM-yyyy";
            this.dtpToDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(386, 28);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(110, 23);
            this.dtpToDate.TabIndex = 5;

            // 
            // btnPreset1to10
            // 
            this.btnPreset1to10.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.btnPreset1to10.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreset1to10.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnPreset1to10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreset1to10.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.btnPreset1to10.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnPreset1to10.Location = new System.Drawing.Point(506, 28);
            this.btnPreset1to10.Name = "btnPreset1to10";
            this.btnPreset1to10.Size = new System.Drawing.Size(38, 24);
            this.btnPreset1to10.TabIndex = 6;
            this.btnPreset1to10.Text = "1-10";
            this.btnPreset1to10.UseVisualStyleBackColor = false;

            // 
            // btnPreset1to15
            // 
            this.btnPreset1to15.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.btnPreset1to15.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreset1to15.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnPreset1to15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreset1to15.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.btnPreset1to15.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnPreset1to15.Location = new System.Drawing.Point(548, 28);
            this.btnPreset1to15.Name = "btnPreset1to15";
            this.btnPreset1to15.Size = new System.Drawing.Size(38, 24);
            this.btnPreset1to15.TabIndex = 7;
            this.btnPreset1to15.Text = "1-15";
            this.btnPreset1to15.UseVisualStyleBackColor = false;

            // 
            // btnPreset1to20
            // 
            this.btnPreset1to20.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.btnPreset1to20.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreset1to20.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnPreset1to20.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreset1to20.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.btnPreset1to20.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnPreset1to20.Location = new System.Drawing.Point(590, 28);
            this.btnPreset1to20.Name = "btnPreset1to20";
            this.btnPreset1to20.Size = new System.Drawing.Size(38, 24);
            this.btnPreset1to20.TabIndex = 8;
            this.btnPreset1to20.Text = "1-20";
            this.btnPreset1to20.UseVisualStyleBackColor = false;

            // 
            // btnPresetMonth
            // 
            this.btnPresetMonth.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.btnPresetMonth.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPresetMonth.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnPresetMonth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPresetMonth.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.btnPresetMonth.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnPresetMonth.Location = new System.Drawing.Point(632, 28);
            this.btnPresetMonth.Name = "btnPresetMonth";
            this.btnPresetMonth.Size = new System.Drawing.Size(52, 24);
            this.btnPresetMonth.TabIndex = 9;
            this.btnPresetMonth.Text = "Month";
            this.btnPresetMonth.UseVisualStyleBackColor = false;

            // 
            // btnPresetLastMonth
            // 
            this.btnPresetLastMonth.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.btnPresetLastMonth.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPresetLastMonth.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnPresetLastMonth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPresetLastMonth.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.btnPresetLastMonth.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnPresetLastMonth.Location = new System.Drawing.Point(688, 28);
            this.btnPresetLastMonth.Name = "btnPresetLastMonth";
            this.btnPresetLastMonth.Size = new System.Drawing.Size(48, 24);
            this.btnPresetLastMonth.TabIndex = 10;
            this.btnPresetLastMonth.Text = "Prev";
            this.btnPresetLastMonth.UseVisualStyleBackColor = false;

            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblItem.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblItem.Location = new System.Drawing.Point(746, 10);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(95, 12);
            this.lblItem.TabIndex = 11;
            this.lblItem.Text = "PRODUCT FILTER";

            // cmbItem
            // 
            this.cmbItem.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbItem.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Location = new System.Drawing.Point(746, 28);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(180, 23);
            this.cmbItem.TabIndex = 12;

            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(936, 27);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 25);
            this.btnSearch.TabIndex = 13;
            this.btnSearch.Text = "Load Records";
            this.btnSearch.UseVisualStyleBackColor = false;

            // 
            // pnlKpis
            // 
            this.pnlKpis.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlKpis.Controls.Add(this.cardTotalRecords);
            this.pnlKpis.Controls.Add(this.cardTotalQty);
            this.pnlKpis.Controls.Add(this.cardTotalAmount);
            this.pnlKpis.Controls.Add(this.cardPending);
            this.pnlKpis.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlKpis.Height = 62;
            this.pnlKpis.Location = new System.Drawing.Point(0, 140);
            this.pnlKpis.Name = "pnlKpis";
            this.pnlKpis.Padding = new System.Windows.Forms.Padding(16, 6, 16, 6);
            this.pnlKpis.Size = new System.Drawing.Size(1264, 62);
            this.pnlKpis.TabIndex = 2;

            // 
            // cardTotalRecords
            // 
            this.cardTotalRecords.BackColor = System.Drawing.Color.White;
            this.cardTotalRecords.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardTotalRecords.Controls.Add(this.lblKpiRecordsTitle);
            this.cardTotalRecords.Controls.Add(this.lblKpiRecordsVal);
            this.cardTotalRecords.Location = new System.Drawing.Point(16, 6);
            this.cardTotalRecords.Name = "cardTotalRecords";
            this.cardTotalRecords.Size = new System.Drawing.Size(220, 50);
            this.cardTotalRecords.TabIndex = 0;

            // 
            // lblKpiRecordsTitle
            // 
            this.lblKpiRecordsTitle.AutoSize = true;
            this.lblKpiRecordsTitle.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold);
            this.lblKpiRecordsTitle.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblKpiRecordsTitle.Location = new System.Drawing.Point(8, 5);
            this.lblKpiRecordsTitle.Name = "lblKpiRecordsTitle";
            this.lblKpiRecordsTitle.Size = new System.Drawing.Size(123, 12);
            this.lblKpiRecordsTitle.TabIndex = 0;
            this.lblKpiRecordsTitle.Text = "TOTAL SUPPLY RECORDS";

            // 
            // lblKpiRecordsVal
            // 
            this.lblKpiRecordsVal.AutoSize = true;
            this.lblKpiRecordsVal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblKpiRecordsVal.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblKpiRecordsVal.Location = new System.Drawing.Point(8, 20);
            this.lblKpiRecordsVal.Name = "lblKpiRecordsVal";
            this.lblKpiRecordsVal.Size = new System.Drawing.Size(77, 20);
            this.lblKpiRecordsVal.TabIndex = 1;
            this.lblKpiRecordsVal.Text = "0 Records";

            // 
            // cardTotalQty
            // 
            this.cardTotalQty.BackColor = System.Drawing.Color.White;
            this.cardTotalQty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardTotalQty.Controls.Add(this.lblKpiQtyTitle);
            this.cardTotalQty.Controls.Add(this.lblKpiQtyVal);
            this.cardTotalQty.Location = new System.Drawing.Point(248, 6);
            this.cardTotalQty.Name = "cardTotalQty";
            this.cardTotalQty.Size = new System.Drawing.Size(220, 50);
            this.cardTotalQty.TabIndex = 1;

            // 
            // lblKpiQtyTitle
            // 
            this.lblKpiQtyTitle.AutoSize = true;
            this.lblKpiQtyTitle.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold);
            this.lblKpiQtyTitle.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblKpiQtyTitle.Location = new System.Drawing.Point(8, 5);
            this.lblKpiQtyTitle.Name = "lblKpiQtyTitle";
            this.lblKpiQtyTitle.Size = new System.Drawing.Size(89, 12);
            this.lblKpiQtyTitle.TabIndex = 0;
            this.lblKpiQtyTitle.Text = "TOTAL QUANTITY";

            // 
            // lblKpiQtyVal
            // 
            this.lblKpiQtyVal.AutoSize = true;
            this.lblKpiQtyVal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblKpiQtyVal.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lblKpiQtyVal.Location = new System.Drawing.Point(8, 20);
            this.lblKpiQtyVal.Name = "lblKpiQtyVal";
            this.lblKpiQtyVal.Size = new System.Drawing.Size(39, 20);
            this.lblKpiQtyVal.TabIndex = 1;
            this.lblKpiQtyVal.Text = "0.00";

            // 
            // cardTotalAmount
            // 
            this.cardTotalAmount.BackColor = System.Drawing.Color.White;
            this.cardTotalAmount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardTotalAmount.Controls.Add(this.lblKpiAmountTitle);
            this.cardTotalAmount.Controls.Add(this.lblKpiAmountVal);
            this.cardTotalAmount.Location = new System.Drawing.Point(480, 6);
            this.cardTotalAmount.Name = "cardTotalAmount";
            this.cardTotalAmount.Size = new System.Drawing.Size(220, 50);
            this.cardTotalAmount.TabIndex = 2;

            // 
            // lblKpiAmountTitle
            // 
            this.lblKpiAmountTitle.AutoSize = true;
            this.lblKpiAmountTitle.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold);
            this.lblKpiAmountTitle.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblKpiAmountTitle.Location = new System.Drawing.Point(8, 5);
            this.lblKpiAmountTitle.Name = "lblKpiAmountTitle";
            this.lblKpiAmountTitle.Size = new System.Drawing.Size(126, 12);
            this.lblKpiAmountTitle.TabIndex = 0;
            this.lblKpiAmountTitle.Text = "TOTAL BILLING AMOUNT";

            // 
            // lblKpiAmountVal
            // 
            this.lblKpiAmountVal.AutoSize = true;
            this.lblKpiAmountVal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblKpiAmountVal.ForeColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.lblKpiAmountVal.Location = new System.Drawing.Point(8, 20);
            this.lblKpiAmountVal.Name = "lblKpiAmountVal";
            this.lblKpiAmountVal.Size = new System.Drawing.Size(61, 20);
            this.lblKpiAmountVal.TabIndex = 1;
            this.lblKpiAmountVal.Text = "Rs. 0.00";

            // 
            // cardPending
            // 
            this.cardPending.BackColor = System.Drawing.Color.White;
            this.cardPending.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardPending.Controls.Add(this.lblKpiPendingTitle);
            this.cardPending.Controls.Add(this.lblKpiPendingVal);
            this.cardPending.Location = new System.Drawing.Point(712, 6);
            this.cardPending.Name = "cardPending";
            this.cardPending.Size = new System.Drawing.Size(220, 50);
            this.cardPending.TabIndex = 3;

            // 
            // lblKpiPendingTitle
            // 
            this.lblKpiPendingTitle.AutoSize = true;
            this.lblKpiPendingTitle.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold);
            this.lblKpiPendingTitle.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblKpiPendingTitle.Location = new System.Drawing.Point(8, 5);
            this.lblKpiPendingTitle.Name = "lblKpiPendingTitle";
            this.lblKpiPendingTitle.Size = new System.Drawing.Size(116, 12);
            this.lblKpiPendingTitle.TabIndex = 0;
            this.lblKpiPendingTitle.Text = "MODIFIED (UNSAVED)";

            // 
            // lblKpiPendingVal
            // 
            this.lblKpiPendingVal.AutoSize = true;
            this.lblKpiPendingVal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblKpiPendingVal.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblKpiPendingVal.Location = new System.Drawing.Point(8, 20);
            this.lblKpiPendingVal.Name = "lblKpiPendingVal";
            this.lblKpiPendingVal.Size = new System.Drawing.Size(76, 20);
            this.lblKpiPendingVal.TabIndex = 1;
            this.lblKpiPendingVal.Text = "0 Pending";

            // 
            // pnlGridContainer
            // 
            this.pnlGridContainer.BackColor = System.Drawing.Color.White;
            this.pnlGridContainer.Controls.Add(this.dgvRecords);
            this.pnlGridContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridContainer.Location = new System.Drawing.Point(0, 202);
            this.pnlGridContainer.Name = "pnlGridContainer";
            this.pnlGridContainer.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlGridContainer.Size = new System.Drawing.Size(1264, 549);
            this.pnlGridContainer.TabIndex = 3;

            // 
            // dgvRecords
            // 
            this.dgvRecords.AllowUserToAddRows = false;
            this.dgvRecords.AllowUserToDeleteRows = false;
            this.dgvRecords.AllowUserToResizeRows = false;
            dgvAltRowStyle.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.dgvRecords.AlternatingRowsDefaultCellStyle = dgvAltRowStyle;
            this.dgvRecords.BackgroundColor = System.Drawing.Color.White;
            this.dgvRecords.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dgvHeaderStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvHeaderStyle.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            dgvHeaderStyle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            dgvHeaderStyle.ForeColor = System.Drawing.Color.White;
            dgvHeaderStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRecords.ColumnHeadersDefaultCellStyle = dgvHeaderStyle;
            this.dgvRecords.ColumnHeadersHeight = 36;
            this.dgvRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvRecords.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDate,
            this.colVoucher,
            this.colItem,
            this.colUnit,
            this.colQty,
            this.colSecQty,
            this.colRate,
            this.colSecRate,
            this.colDiscount,
            this.colAddLess,
            this.colAmount,
            this.colSave,
            this.colDelete});
            dgvDefaultStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvDefaultStyle.BackColor = System.Drawing.SystemColors.Window;
            dgvDefaultStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            dgvDefaultStyle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dgvDefaultStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            dgvDefaultStyle.SelectionForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            dgvDefaultStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRecords.DefaultCellStyle = dgvDefaultStyle;
            this.dgvRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecords.EnableHeadersVisualStyles = false;
            this.dgvRecords.GridColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.dgvRecords.Location = new System.Drawing.Point(16, 8);
            this.dgvRecords.Name = "dgvRecords";
            this.dgvRecords.RowHeadersVisible = false;
            this.dgvRecords.RowTemplate.Height = 30;
            this.dgvRecords.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvRecords.Size = new System.Drawing.Size(1232, 533);
            this.dgvRecords.TabIndex = 0;

            // 
            // colDate
            // 
            colDateStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colDate.DefaultCellStyle = colDateStyle;
            this.colDate.HeaderText = "Date";
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            this.colDate.Width = 95;

            // 
            // colVoucher
            // 
            colVoucherStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colVoucherStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            colVoucherStyle.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.colVoucher.DefaultCellStyle = colVoucherStyle;
            this.colVoucher.HeaderText = "Voucher #";
            this.colVoucher.Name = "colVoucher";
            this.colVoucher.ReadOnly = true;
            this.colVoucher.Width = 90;

            // 
            // colItem
            // 
            this.colItem.HeaderText = "Item Supplied";
            this.colItem.Name = "colItem";
            this.colItem.ReadOnly = true;
            this.colItem.Width = 210;

            // 
            // colUnit
            // 
            colUnitStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colUnit.DefaultCellStyle = colUnitStyle;
            this.colUnit.HeaderText = "Unit";
            this.colUnit.Name = "colUnit";
            this.colUnit.ReadOnly = true;
            this.colUnit.Width = 90;

            // 
            // colQty
            // 
            colQtyStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            colQtyStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.colQty.DefaultCellStyle = colQtyStyle;
            this.colQty.HeaderText = "Qty";
            this.colQty.Name = "colQty";
            this.colQty.Width = 85;

            // 
            // colSecQty
            // 
            colSecQtyStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colSecQty.DefaultCellStyle = colSecQtyStyle;
            this.colSecQty.HeaderText = "Sec Qty";
            this.colSecQty.Name = "colSecQty";
            this.colSecQty.Width = 85;

            // 
            // colRate
            // 
            colRateStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colRate.DefaultCellStyle = colRateStyle;
            this.colRate.HeaderText = "Rate";
            this.colRate.Name = "colRate";
            this.colRate.Width = 85;

            // 
            // colSecRate
            // 
            colSecRateStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colSecRate.DefaultCellStyle = colSecRateStyle;
            this.colSecRate.HeaderText = "Sec Rate";
            this.colSecRate.Name = "colSecRate";
            this.colSecRate.Width = 85;

            // 
            // colDiscount
            // 
            colDiscountStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colDiscount.DefaultCellStyle = colDiscountStyle;
            this.colDiscount.HeaderText = "Discount";
            this.colDiscount.Name = "colDiscount";
            this.colDiscount.Width = 80;

            // 
            // colAddLess
            // 
            colAddLessStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colAddLess.DefaultCellStyle = colAddLessStyle;
            this.colAddLess.HeaderText = "Add / Less";
            this.colAddLess.Name = "colAddLess";
            this.colAddLess.Width = 85;

            // 
            // colAmount
            // 
            colAmountStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            colAmountStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            colAmountStyle.ForeColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.colAmount.DefaultCellStyle = colAmountStyle;
            this.colAmount.HeaderText = "Total Amount";
            this.colAmount.Name = "colAmount";
            this.colAmount.ReadOnly = true;
            this.colAmount.Width = 115;

            // 
            // colSave
            // 
            colSaveStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colSaveStyle.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            colSaveStyle.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.colSave.DefaultCellStyle = colSaveStyle;
            this.colSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colSave.HeaderText = "Save";
            this.colSave.Name = "colSave";
            this.colSave.Text = "Save";
            this.colSave.UseColumnTextForButtonValue = true;
            this.colSave.Width = 65;

            // 
            // colDelete
            // 
            colDeleteStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colDeleteStyle.BackColor = System.Drawing.Color.FromArgb(254, 242, 242);
            colDeleteStyle.ForeColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.colDelete.DefaultCellStyle = colDeleteStyle;
            this.colDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colDelete.HeaderText = "Delete";
            this.colDelete.Name = "colDelete";
            this.colDelete.Text = "Delete";
            this.colDelete.UseColumnTextForButtonValue = true;
            this.colDelete.Width = 65;

            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.prgProgress});
            this.statusStrip.Location = new System.Drawing.Point(0, 751);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1264, 22);
            this.statusStrip.TabIndex = 4;

            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1249, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "Ready. Select a customer to load daily supply records.";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // prgProgress
            // 
            this.prgProgress.Name = "prgProgress";
            this.prgProgress.Size = new System.Drawing.Size(150, 16);
            this.prgProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.prgProgress.Visible = false;

            // 
            // frmCustomerSupplyRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.ClientSize = new System.Drawing.Size(1264, 773);
            this.Controls.Add(this.pnlGridContainer);
            this.Controls.Add(this.pnlKpis);
            this.Controls.Add(this.pnlFilters);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.MinimumSize = new System.Drawing.Size(1080, 650);
            this.Name = "frmCustomerSupplyRegister";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Supply & Bill Register";

            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            this.pnlKpis.ResumeLayout(false);
            this.cardTotalRecords.ResumeLayout(false);
            this.cardTotalRecords.PerformLayout();
            this.cardTotalQty.ResumeLayout(false);
            this.cardTotalQty.PerformLayout();
            this.cardTotalAmount.ResumeLayout(false);
            this.cardTotalAmount.PerformLayout();
            this.cardPending.ResumeLayout(false);
            this.cardPending.PerformLayout();
            this.pnlGridContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnSaveAll;
        private System.Windows.Forms.Button btnAddSupplyEntry;
        private System.Windows.Forms.Button btnPrintCustomerBill;

        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.ComboBox cmbCustomer;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Button btnPreset1to10;
        private System.Windows.Forms.Button btnPreset1to15;
        private System.Windows.Forms.Button btnPreset1to20;
        private System.Windows.Forms.Button btnPresetMonth;
        private System.Windows.Forms.Button btnPresetLastMonth;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.ComboBox cmbItem;
        private System.Windows.Forms.Button btnSearch;

        private System.Windows.Forms.Panel pnlKpis;
        private System.Windows.Forms.Panel cardTotalRecords;
        private System.Windows.Forms.Label lblKpiRecordsTitle;
        private System.Windows.Forms.Label lblKpiRecordsVal;
        private System.Windows.Forms.Panel cardTotalQty;
        private System.Windows.Forms.Label lblKpiQtyTitle;
        private System.Windows.Forms.Label lblKpiQtyVal;
        private System.Windows.Forms.Panel cardTotalAmount;
        private System.Windows.Forms.Label lblKpiAmountTitle;
        private System.Windows.Forms.Label lblKpiAmountVal;
        private System.Windows.Forms.Panel cardPending;
        private System.Windows.Forms.Label lblKpiPendingTitle;
        private System.Windows.Forms.Label lblKpiPendingVal;

        private System.Windows.Forms.Panel pnlGridContainer;
        private System.Windows.Forms.DataGridView dgvRecords;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVoucher;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSecQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSecRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDiscount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAddLess;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAmount;
        private System.Windows.Forms.DataGridViewButtonColumn colSave;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar prgProgress;
    }
}
