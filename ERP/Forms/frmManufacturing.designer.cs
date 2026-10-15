namespace ERP.Forms
{
    partial class frmManufacturing
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlLeftFilters = new System.Windows.Forms.Panel();
            this.lblFilterDates = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblStatusFilter = new System.Windows.Forms.Label();
            this.cmbStatusFilter = new System.Windows.Forms.ComboBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnFilter = new System.Windows.Forms.Button();
            this.dgvProductionList = new System.Windows.Forms.DataGridView();
            this.clnListVoucher = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListPlanned = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpMaster = new System.Windows.Forms.GroupBox();
            this.lblVoucherNo = new System.Windows.Forms.Label();
            this.txtVoucherNo = new System.Windows.Forms.TextBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lblBom = new System.Windows.Forms.Label();
            this.cmbBom = new System.Windows.Forms.ComboBox();
            this.lblFinishedItem = new System.Windows.Forms.Label();
            this.cmbFinishedItem = new System.Windows.Forms.ComboBox();
            this.lblBatchNo = new System.Windows.Forms.Label();
            this.txtBatchNo = new System.Windows.Forms.TextBox();
            this.lblPlannedQty = new System.Windows.Forms.Label();
            this.txtPlannedQty = new System.Windows.Forms.TextBox();
            this.lblProducedQty = new System.Windows.Forms.Label();
            this.txtProducedQty = new System.Windows.Forms.TextBox();
            this.lblRejectedQty = new System.Windows.Forms.Label();
            this.txtRejectedQty = new System.Windows.Forms.TextBox();
            this.lblStatusTitle = new System.Windows.Forms.Label();
            this.lblStatusDisplay = new System.Windows.Forms.Label();
            this.lblStage = new System.Windows.Forms.Label();
            this.txtStage = new System.Windows.Forms.TextBox();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.grpCosts = new System.Windows.Forms.GroupBox();
            this.lblRawCost = new System.Windows.Forms.Label();
            this.txtRawCost = new System.Windows.Forms.TextBox();
            this.lblLaborCost = new System.Windows.Forms.Label();
            this.txtLaborCost = new System.Windows.Forms.TextBox();
            this.lblOverheadCost = new System.Windows.Forms.Label();
            this.txtOverheadCost = new System.Windows.Forms.TextBox();
            this.lblTotalCost = new System.Windows.Forms.Label();
            this.txtTotalCost = new System.Windows.Forms.TextBox();
            this.lblUnitCost = new System.Windows.Forms.Label();
            this.txtUnitCost = new System.Windows.Forms.TextBox();
            this.grpDetails = new System.Windows.Forms.GroupBox();
            this.dgvRawMaterials = new System.Windows.Forms.DataGridView();
            this.clnRawItem = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clnPlannedQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnActualQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnWastageQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnUnitCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnTotalCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnDetailNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlDetailButtons = new System.Windows.Forms.Panel();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.btnDeleteRow = new System.Windows.Forms.Button();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnIssueMaterials = new System.Windows.Forms.Button();
            this.btnComplete = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();

            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlLeftFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductionList)).BeginInit();
            this.grpMaster.SuspendLayout();
            this.grpCosts.SuspendLayout();
            this.grpDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRawMaterials)).BeginInit();
            this.pnlDetailButtons.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();

            // pnlHeader
            this.pnlHeader.BackColor = System.Drawing.Color.DarkSlateGray;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1200, 48);
            this.pnlHeader.TabIndex = 0;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(365, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Manufacturing & Production Voucher (WIP)";

            // splitContainer1
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 48);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.dgvProductionList);
            this.splitContainer1.Panel1.Controls.Add(this.pnlLeftFilters);
            this.splitContainer1.Panel2.Controls.Add(this.grpDetails);
            this.splitContainer1.Panel2.Controls.Add(this.grpCosts);
            this.splitContainer1.Panel2.Controls.Add(this.grpMaster);
            this.splitContainer1.Panel2.Controls.Add(this.pnlActions);
            this.splitContainer1.Size = new System.Drawing.Size(1200, 672);
            this.splitContainer1.SplitterDistance = 350;
            this.splitContainer1.TabIndex = 1;

            // pnlLeftFilters
            this.pnlLeftFilters.Controls.Add(this.lblFilterDates);
            this.pnlLeftFilters.Controls.Add(this.dtpFrom);
            this.pnlLeftFilters.Controls.Add(this.dtpTo);
            this.pnlLeftFilters.Controls.Add(this.lblStatusFilter);
            this.pnlLeftFilters.Controls.Add(this.cmbStatusFilter);
            this.pnlLeftFilters.Controls.Add(this.lblSearch);
            this.pnlLeftFilters.Controls.Add(this.txtSearch);
            this.pnlLeftFilters.Controls.Add(this.btnFilter);
            this.pnlLeftFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLeftFilters.Location = new System.Drawing.Point(0, 0);
            this.pnlLeftFilters.Name = "pnlLeftFilters";
            this.pnlLeftFilters.Size = new System.Drawing.Size(350, 105);
            this.pnlLeftFilters.TabIndex = 0;

            // lblFilterDates
            this.lblFilterDates.AutoSize = true;
            this.lblFilterDates.Location = new System.Drawing.Point(8, 12);
            this.lblFilterDates.Name = "lblFilterDates";
            this.lblFilterDates.Size = new System.Drawing.Size(38, 13);
            this.lblFilterDates.Text = "Dates:";

            // dtpFrom
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(52, 9);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(100, 20);

            // dtpTo
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(158, 9);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(100, 20);

            // lblStatusFilter
            this.lblStatusFilter.AutoSize = true;
            this.lblStatusFilter.Location = new System.Drawing.Point(8, 42);
            this.lblStatusFilter.Name = "lblStatusFilter";
            this.lblStatusFilter.Size = new System.Drawing.Size(40, 13);
            this.lblStatusFilter.Text = "Status:";

            // cmbStatusFilter
            this.cmbStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatusFilter.FormattingEnabled = true;
            this.cmbStatusFilter.Items.AddRange(new object[] { "All", "Planned", "InProgress", "Completed", "Cancelled" });
            this.cmbStatusFilter.Location = new System.Drawing.Point(52, 38);
            this.cmbStatusFilter.Name = "cmbStatusFilter";
            this.cmbStatusFilter.Size = new System.Drawing.Size(120, 21);
            this.cmbStatusFilter.SelectedIndex = 0;

            // btnFilter
            this.btnFilter.BackColor = System.Drawing.Color.Teal;
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.ForeColor = System.Drawing.Color.White;
            this.btnFilter.Location = new System.Drawing.Point(180, 36);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(78, 25);
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = false;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);

            // lblSearch
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(8, 74);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(44, 13);
            this.lblSearch.Text = "Search:";

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(52, 71);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(280, 20);
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);

            // dgvProductionList
            this.dgvProductionList.AllowUserToAddRows = false;
            this.dgvProductionList.AllowUserToDeleteRows = false;
            this.dgvProductionList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProductionList.BackgroundColor = System.Drawing.Color.White;
            this.dgvProductionList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductionList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnListVoucher,
            this.clnListDate,
            this.clnListItem,
            this.clnListPlanned,
            this.clnListStatus});
            this.dgvProductionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductionList.Location = new System.Drawing.Point(0, 105);
            this.dgvProductionList.MultiSelect = false;
            this.dgvProductionList.Name = "dgvProductionList";
            this.dgvProductionList.ReadOnly = true;
            this.dgvProductionList.RowHeadersVisible = false;
            this.dgvProductionList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductionList.Size = new System.Drawing.Size(350, 567);
            this.dgvProductionList.TabIndex = 1;
            this.dgvProductionList.SelectionChanged += new System.EventHandler(this.dgvProductionList_SelectionChanged);

            // Columns for dgvProductionList
            this.clnListVoucher.DataPropertyName = "VoucherNo";
            this.clnListVoucher.HeaderText = "Voucher #";
            this.clnListVoucher.Name = "clnListVoucher";
            this.clnListVoucher.ReadOnly = true;
            this.clnListVoucher.FillWeight = 65F;

            this.clnListDate.DataPropertyName = "Date";
            this.clnListDate.HeaderText = "Date";
            this.clnListDate.Name = "clnListDate";
            this.clnListDate.ReadOnly = true;
            this.clnListDate.FillWeight = 55F;

            this.clnListItem.DataPropertyName = "FinishedItemName";
            this.clnListItem.HeaderText = "Output Product";
            this.clnListItem.Name = "clnListItem";
            this.clnListItem.ReadOnly = true;
            this.clnListItem.FillWeight = 90F;

            this.clnListPlanned.DataPropertyName = "PlannedQuantity";
            this.clnListPlanned.HeaderText = "Qty";
            this.clnListPlanned.Name = "clnListPlanned";
            this.clnListPlanned.ReadOnly = true;
            this.clnListPlanned.FillWeight = 40F;

            this.clnListStatus.DataPropertyName = "Status";
            this.clnListStatus.HeaderText = "Status";
            this.clnListStatus.Name = "clnListStatus";
            this.clnListStatus.ReadOnly = true;
            this.clnListStatus.FillWeight = 55F;

            // grpMaster
            this.grpMaster.Controls.Add(this.lblVoucherNo);
            this.grpMaster.Controls.Add(this.txtVoucherNo);
            this.grpMaster.Controls.Add(this.lblDate);
            this.grpMaster.Controls.Add(this.dtpDate);
            this.grpMaster.Controls.Add(this.lblBom);
            this.grpMaster.Controls.Add(this.cmbBom);
            this.grpMaster.Controls.Add(this.lblFinishedItem);
            this.grpMaster.Controls.Add(this.cmbFinishedItem);
            this.grpMaster.Controls.Add(this.lblBatchNo);
            this.grpMaster.Controls.Add(this.txtBatchNo);
            this.grpMaster.Controls.Add(this.lblPlannedQty);
            this.grpMaster.Controls.Add(this.txtPlannedQty);
            this.grpMaster.Controls.Add(this.lblProducedQty);
            this.grpMaster.Controls.Add(this.txtProducedQty);
            this.grpMaster.Controls.Add(this.lblRejectedQty);
            this.grpMaster.Controls.Add(this.txtRejectedQty);
            this.grpMaster.Controls.Add(this.lblStatusTitle);
            this.grpMaster.Controls.Add(this.lblStatusDisplay);
            this.grpMaster.Controls.Add(this.lblStage);
            this.grpMaster.Controls.Add(this.txtStage);
            this.grpMaster.Controls.Add(this.lblRemarks);
            this.grpMaster.Controls.Add(this.txtRemarks);
            this.grpMaster.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpMaster.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grpMaster.Location = new System.Drawing.Point(0, 0);
            this.grpMaster.Name = "grpMaster";
            this.grpMaster.Size = new System.Drawing.Size(846, 175);
            this.grpMaster.TabIndex = 0;
            this.grpMaster.TabStop = false;
            this.grpMaster.Text = "Production Batch Header";

            // Row 1
            this.lblVoucherNo.AutoSize = true;
            this.lblVoucherNo.Location = new System.Drawing.Point(12, 22);
            this.lblVoucherNo.Text = "Voucher #:";
            this.txtVoucherNo.Location = new System.Drawing.Point(85, 19);
            this.txtVoucherNo.Size = new System.Drawing.Size(120, 23);
            this.txtVoucherNo.ReadOnly = true;

            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(220, 22);
            this.lblDate.Text = "Date:";
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(265, 19);
            this.dtpDate.Size = new System.Drawing.Size(100, 23);

            this.lblStatusTitle.AutoSize = true;
            this.lblStatusTitle.Location = new System.Drawing.Point(385, 22);
            this.lblStatusTitle.Text = "Status:";
            this.lblStatusDisplay.AutoSize = true;
            this.lblStatusDisplay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusDisplay.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblStatusDisplay.Location = new System.Drawing.Point(435, 22);
            this.lblStatusDisplay.Text = "Planned";

            this.lblStage.AutoSize = true;
            this.lblStage.Location = new System.Drawing.Point(530, 22);
            this.lblStage.Text = "WIP Stage:";
            this.txtStage.Location = new System.Drawing.Point(600, 19);
            this.txtStage.Size = new System.Drawing.Size(160, 23);

            // Row 2
            this.lblBom.AutoSize = true;
            this.lblBom.Location = new System.Drawing.Point(12, 53);
            this.lblBom.Text = "Select BOM:";
            this.cmbBom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBom.FormattingEnabled = true;
            this.cmbBom.Location = new System.Drawing.Point(85, 50);
            this.cmbBom.Size = new System.Drawing.Size(280, 23);
            this.cmbBom.SelectedIndexChanged += new System.EventHandler(this.cmbBom_SelectedIndexChanged);

            this.lblFinishedItem.AutoSize = true;
            this.lblFinishedItem.Location = new System.Drawing.Point(385, 53);
            this.lblFinishedItem.Text = "Output Item:";
            this.cmbFinishedItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFinishedItem.FormattingEnabled = true;
            this.cmbFinishedItem.Location = new System.Drawing.Point(465, 50);
            this.cmbFinishedItem.Size = new System.Drawing.Size(295, 23);

            // Row 3
            this.lblPlannedQty.AutoSize = true;
            this.lblPlannedQty.Location = new System.Drawing.Point(12, 85);
            this.lblPlannedQty.Text = "Planned Qty:";
            this.txtPlannedQty.Location = new System.Drawing.Point(85, 82);
            this.txtPlannedQty.Size = new System.Drawing.Size(85, 23);
            this.txtPlannedQty.Text = "1";
            this.txtPlannedQty.TextChanged += new System.EventHandler(this.txtPlannedQty_TextChanged);

            this.lblProducedQty.AutoSize = true;
            this.lblProducedQty.Location = new System.Drawing.Point(180, 85);
            this.lblProducedQty.Text = "Produced Qty:";
            this.txtProducedQty.Location = new System.Drawing.Point(265, 82);
            this.txtProducedQty.Size = new System.Drawing.Size(85, 23);
            this.txtProducedQty.Text = "0";
            this.txtProducedQty.TextChanged += new System.EventHandler(this.CalculateUnitCost);

            this.lblRejectedQty.AutoSize = true;
            this.lblRejectedQty.Location = new System.Drawing.Point(365, 85);
            this.lblRejectedQty.Text = "Rejected Qty:";
            this.txtRejectedQty.Location = new System.Drawing.Point(445, 82);
            this.txtRejectedQty.Size = new System.Drawing.Size(75, 23);
            this.txtRejectedQty.Text = "0";

            this.lblBatchNo.AutoSize = true;
            this.lblBatchNo.Location = new System.Drawing.Point(535, 85);
            this.lblBatchNo.Text = "Batch #:";
            this.txtBatchNo.Location = new System.Drawing.Point(600, 82);
            this.txtBatchNo.Size = new System.Drawing.Size(160, 23);

            // Row 4
            this.lblRemarks.AutoSize = true;
            this.lblRemarks.Location = new System.Drawing.Point(12, 118);
            this.lblRemarks.Text = "Remarks:";
            this.txtRemarks.Location = new System.Drawing.Point(85, 115);
            this.txtRemarks.Size = new System.Drawing.Size(675, 23);

            // grpCosts
            this.grpCosts.Controls.Add(this.lblRawCost);
            this.grpCosts.Controls.Add(this.txtRawCost);
            this.grpCosts.Controls.Add(this.lblLaborCost);
            this.grpCosts.Controls.Add(this.txtLaborCost);
            this.grpCosts.Controls.Add(this.lblOverheadCost);
            this.grpCosts.Controls.Add(this.txtOverheadCost);
            this.grpCosts.Controls.Add(this.lblTotalCost);
            this.grpCosts.Controls.Add(this.txtTotalCost);
            this.grpCosts.Controls.Add(this.lblUnitCost);
            this.grpCosts.Controls.Add(this.txtUnitCost);
            this.grpCosts.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpCosts.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grpCosts.Location = new System.Drawing.Point(0, 175);
            this.grpCosts.Name = "grpCosts";
            this.grpCosts.Size = new System.Drawing.Size(846, 60);
            this.grpCosts.TabIndex = 1;
            this.grpCosts.TabStop = false;
            this.grpCosts.Text = "Cost Summary (PKR)";

            this.lblRawCost.AutoSize = true;
            this.lblRawCost.Location = new System.Drawing.Point(12, 25);
            this.lblRawCost.Text = "Raw Mat. Cost:";
            this.txtRawCost.Location = new System.Drawing.Point(95, 22);
            this.txtRawCost.Size = new System.Drawing.Size(80, 23);
            this.txtRawCost.ReadOnly = true;
            this.txtRawCost.Text = "0.00";

            this.lblLaborCost.AutoSize = true;
            this.lblLaborCost.Location = new System.Drawing.Point(185, 25);
            this.lblLaborCost.Text = "Labor:";
            this.txtLaborCost.Location = new System.Drawing.Point(225, 22);
            this.txtLaborCost.Size = new System.Drawing.Size(75, 23);
            this.txtLaborCost.Text = "0.00";
            this.txtLaborCost.TextChanged += new System.EventHandler(this.CalculateUnitCost);

            this.lblOverheadCost.AutoSize = true;
            this.lblOverheadCost.Location = new System.Drawing.Point(310, 25);
            this.lblOverheadCost.Text = "Overhead:";
            this.txtOverheadCost.Location = new System.Drawing.Point(375, 22);
            this.txtOverheadCost.Size = new System.Drawing.Size(75, 23);
            this.txtOverheadCost.Text = "0.00";
            this.txtOverheadCost.TextChanged += new System.EventHandler(this.CalculateUnitCost);

            this.lblTotalCost.AutoSize = true;
            this.lblTotalCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalCost.Location = new System.Drawing.Point(460, 25);
            this.lblTotalCost.Text = "Total Cost:";
            this.txtTotalCost.Location = new System.Drawing.Point(525, 22);
            this.txtTotalCost.Size = new System.Drawing.Size(95, 23);
            this.txtTotalCost.ReadOnly = true;
            this.txtTotalCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtTotalCost.Text = "0.00";

            this.lblUnitCost.AutoSize = true;
            this.lblUnitCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblUnitCost.Location = new System.Drawing.Point(630, 25);
            this.lblUnitCost.Text = "Unit Cost:";
            this.txtUnitCost.Location = new System.Drawing.Point(695, 22);
            this.txtUnitCost.Size = new System.Drawing.Size(85, 23);
            this.txtUnitCost.ReadOnly = true;
            this.txtUnitCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtUnitCost.ForeColor = System.Drawing.Color.DarkGreen;
            this.txtUnitCost.Text = "0.00";

            // grpDetails
            this.grpDetails.Controls.Add(this.dgvRawMaterials);
            this.grpDetails.Controls.Add(this.pnlDetailButtons);
            this.grpDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDetails.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grpDetails.Location = new System.Drawing.Point(0, 235);
            this.grpDetails.Name = "grpDetails";
            this.grpDetails.Size = new System.Drawing.Size(846, 385);
            this.grpDetails.TabIndex = 2;
            this.grpDetails.TabStop = false;
            this.grpDetails.Text = "Raw Material Issuance & Consumption";

            // pnlDetailButtons
            this.pnlDetailButtons.Controls.Add(this.btnAddRow);
            this.pnlDetailButtons.Controls.Add(this.btnDeleteRow);
            this.pnlDetailButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDetailButtons.Location = new System.Drawing.Point(3, 19);
            this.pnlDetailButtons.Name = "pnlDetailButtons";
            this.pnlDetailButtons.Size = new System.Drawing.Size(840, 32);
            this.pnlDetailButtons.TabIndex = 0;

            // btnAddRow
            this.btnAddRow.BackColor = System.Drawing.Color.ForestGreen;
            this.btnAddRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddRow.ForeColor = System.Drawing.Color.White;
            this.btnAddRow.Location = new System.Drawing.Point(6, 3);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(90, 26);
            this.btnAddRow.Text = "+ Add Line";
            this.btnAddRow.UseVisualStyleBackColor = false;
            this.btnAddRow.Click += new System.EventHandler(this.btnAddRow_Click);

            // btnDeleteRow
            this.btnDeleteRow.BackColor = System.Drawing.Color.Firebrick;
            this.btnDeleteRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteRow.ForeColor = System.Drawing.Color.White;
            this.btnDeleteRow.Location = new System.Drawing.Point(102, 3);
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(90, 26);
            this.btnDeleteRow.Text = "- Remove";
            this.btnDeleteRow.UseVisualStyleBackColor = false;
            this.btnDeleteRow.Click += new System.EventHandler(this.btnDeleteRow_Click);

            // dgvRawMaterials
            this.dgvRawMaterials.AllowUserToAddRows = false;
            this.dgvRawMaterials.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRawMaterials.BackgroundColor = System.Drawing.Color.White;
            this.dgvRawMaterials.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRawMaterials.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnRawItem,
            this.clnPlannedQty,
            this.clnActualQty,
            this.clnWastageQty,
            this.clnUnitCost,
            this.clnTotalCost,
            this.clnDetailNotes});
            this.dgvRawMaterials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRawMaterials.Location = new System.Drawing.Point(3, 51);
            this.dgvRawMaterials.Name = "dgvRawMaterials";
            this.dgvRawMaterials.RowHeadersVisible = false;
            this.dgvRawMaterials.Size = new System.Drawing.Size(840, 331);
            this.dgvRawMaterials.TabIndex = 1;
            this.dgvRawMaterials.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRawMaterials_CellValueChanged);

            // Columns
            this.clnRawItem.HeaderText = "Raw Material / Component";
            this.clnRawItem.Name = "clnRawItem";
            this.clnRawItem.FillWeight = 140F;

            this.clnPlannedQty.HeaderText = "Planned Qty";
            this.clnPlannedQty.Name = "clnPlannedQty";
            this.clnPlannedQty.FillWeight = 50F;

            this.clnActualQty.HeaderText = "Actual Qty";
            this.clnActualQty.Name = "clnActualQty";
            this.clnActualQty.FillWeight = 50F;

            this.clnWastageQty.HeaderText = "Wastage Qty";
            this.clnWastageQty.Name = "clnWastageQty";
            this.clnWastageQty.FillWeight = 45F;

            this.clnUnitCost.HeaderText = "Unit Cost";
            this.clnUnitCost.Name = "clnUnitCost";
            this.clnUnitCost.FillWeight = 50F;

            this.clnTotalCost.HeaderText = "Line Cost";
            this.clnTotalCost.Name = "clnTotalCost";
            this.clnTotalCost.ReadOnly = true;
            this.clnTotalCost.FillWeight = 55F;

            this.clnDetailNotes.HeaderText = "Notes";
            this.clnDetailNotes.Name = "clnDetailNotes";
            this.clnDetailNotes.FillWeight = 70F;

            // pnlActions
            this.pnlActions.Controls.Add(this.btnNew);
            this.pnlActions.Controls.Add(this.btnSave);
            this.pnlActions.Controls.Add(this.btnIssueMaterials);
            this.pnlActions.Controls.Add(this.btnComplete);
            this.pnlActions.Controls.Add(this.btnCancel);
            this.pnlActions.Controls.Add(this.btnDelete);
            this.pnlActions.Controls.Add(this.btnClose);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActions.Location = new System.Drawing.Point(0, 620);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(846, 52);
            this.pnlActions.TabIndex = 3;

            // Buttons
            this.btnNew.BackColor = System.Drawing.Color.LightGray;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNew.Location = new System.Drawing.Point(12, 10);
            this.btnNew.Size = new System.Drawing.Size(75, 32);
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            this.btnSave.BackColor = System.Drawing.Color.SteelBlue;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(95, 10);
            this.btnSave.Size = new System.Drawing.Size(85, 32);
            this.btnSave.Text = "Save Plan";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnIssueMaterials.BackColor = System.Drawing.Color.DarkOrange;
            this.btnIssueMaterials.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIssueMaterials.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnIssueMaterials.ForeColor = System.Drawing.Color.White;
            this.btnIssueMaterials.Location = new System.Drawing.Point(188, 10);
            this.btnIssueMaterials.Size = new System.Drawing.Size(140, 32);
            this.btnIssueMaterials.Text = "Issue Materials (WIP)";
            this.btnIssueMaterials.Click += new System.EventHandler(this.btnIssueMaterials_Click);

            this.btnComplete.BackColor = System.Drawing.Color.SeaGreen;
            this.btnComplete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComplete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnComplete.ForeColor = System.Drawing.Color.White;
            this.btnComplete.Location = new System.Drawing.Point(336, 10);
            this.btnComplete.Size = new System.Drawing.Size(140, 32);
            this.btnComplete.Text = "Complete Batch";
            this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);

            this.btnCancel.BackColor = System.Drawing.Color.DimGray;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(484, 10);
            this.btnCancel.Size = new System.Drawing.Size(80, 32);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.btnDelete.BackColor = System.Drawing.Color.Crimson;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(572, 10);
            this.btnDelete.Size = new System.Drawing.Size(75, 32);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.btnClose.BackColor = System.Drawing.Color.Gray;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(655, 10);
            this.btnClose.Size = new System.Drawing.Size(75, 32);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 720);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlHeader);
            this.Name = "frmManufacturing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manufacturing / Production Voucher";
            this.Load += new System.EventHandler(this.frmManufacturing_Load);

            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnlLeftFilters.ResumeLayout(false);
            this.pnlLeftFilters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductionList)).EndInit();
            this.grpMaster.ResumeLayout(false);
            this.grpMaster.PerformLayout();
            this.grpCosts.ResumeLayout(false);
            this.grpCosts.PerformLayout();
            this.grpDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRawMaterials)).EndInit();
            this.pnlDetailButtons.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlLeftFilters;
        private System.Windows.Forms.Label lblFilterDates;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblStatusFilter;
        private System.Windows.Forms.ComboBox cmbStatusFilter;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.DataGridView dgvProductionList;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListVoucher;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListPlanned;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListStatus;
        private System.Windows.Forms.GroupBox grpMaster;
        private System.Windows.Forms.Label lblVoucherNo;
        private System.Windows.Forms.TextBox txtVoucherNo;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label lblBom;
        private System.Windows.Forms.ComboBox cmbBom;
        private System.Windows.Forms.Label lblFinishedItem;
        private System.Windows.Forms.ComboBox cmbFinishedItem;
        private System.Windows.Forms.Label lblBatchNo;
        private System.Windows.Forms.TextBox txtBatchNo;
        private System.Windows.Forms.Label lblPlannedQty;
        private System.Windows.Forms.TextBox txtPlannedQty;
        private System.Windows.Forms.Label lblProducedQty;
        private System.Windows.Forms.TextBox txtProducedQty;
        private System.Windows.Forms.Label lblRejectedQty;
        private System.Windows.Forms.TextBox txtRejectedQty;
        private System.Windows.Forms.Label lblStatusTitle;
        private System.Windows.Forms.Label lblStatusDisplay;
        private System.Windows.Forms.Label lblStage;
        private System.Windows.Forms.TextBox txtStage;
        private System.Windows.Forms.Label lblRemarks;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.GroupBox grpCosts;
        private System.Windows.Forms.Label lblRawCost;
        private System.Windows.Forms.TextBox txtRawCost;
        private System.Windows.Forms.Label lblLaborCost;
        private System.Windows.Forms.TextBox txtLaborCost;
        private System.Windows.Forms.Label lblOverheadCost;
        private System.Windows.Forms.TextBox txtOverheadCost;
        private System.Windows.Forms.Label lblTotalCost;
        private System.Windows.Forms.TextBox txtTotalCost;
        private System.Windows.Forms.Label lblUnitCost;
        private System.Windows.Forms.TextBox txtUnitCost;
        private System.Windows.Forms.GroupBox grpDetails;
        private System.Windows.Forms.Panel pnlDetailButtons;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.Button btnDeleteRow;
        private System.Windows.Forms.DataGridView dgvRawMaterials;
        private System.Windows.Forms.DataGridViewComboBoxColumn clnRawItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnPlannedQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnActualQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnWastageQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnUnitCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnTotalCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnDetailNotes;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnIssueMaterials;
        private System.Windows.Forms.Button btnComplete;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClose;
    }
}
