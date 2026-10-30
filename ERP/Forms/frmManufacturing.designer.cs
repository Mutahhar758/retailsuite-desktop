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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeaderShortcuts = new System.Windows.Forms.Label();
            this.lblHeaderSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabDetailQuery = new System.Windows.Forms.TabControl();
            this.tabpgDetail = new System.Windows.Forms.TabPage();
            this.grpMaster = new System.Windows.Forms.GroupBox();
            this.lblVoucherNo = new System.Windows.Forms.Label();
            this.txtVoucherNo = new System.Windows.Forms.TextBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lblStatusTitle = new System.Windows.Forms.Label();
            this.lblStatusDisplay = new System.Windows.Forms.Label();
            this.lblBatchNo = new System.Windows.Forms.Label();
            this.txtBatchNo = new System.Windows.Forms.TextBox();
            this.lblStage = new System.Windows.Forms.Label();
            this.txtStage = new System.Windows.Forms.TextBox();
            this.lblBom = new System.Windows.Forms.Label();
            this.cmbBom = new System.Windows.Forms.ComboBox();
            this.lblFinishedItem = new System.Windows.Forms.Label();
            this.cmbFinishedItem = new System.Windows.Forms.ComboBox();
            this.lblPlannedQty = new System.Windows.Forms.Label();
            this.txtPlannedQty = new System.Windows.Forms.TextBox();
            this.lblProducedQty = new System.Windows.Forms.Label();
            this.txtProducedQty = new System.Windows.Forms.TextBox();
            this.lblRejectedQty = new System.Windows.Forms.Label();
            this.txtRejectedQty = new System.Windows.Forms.TextBox();
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
            this.pnlDetailButtons = new System.Windows.Forms.Panel();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.btnDeleteRow = new System.Windows.Forms.Button();
            this.dgvRawMaterials = new System.Windows.Forms.DataGridView();
            this.clnRawItem = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clnPlannedQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnActualQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnWastageQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnUnitCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnTotalCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnDetailNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.lblShortcutsBar = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnIssueMaterials = new System.Windows.Forms.Button();
            this.btnComplete = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabpgQuery = new System.Windows.Forms.TabPage();
            this.dgvProductionList = new System.Windows.Forms.DataGridView();
            this.clnListVoucher = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListPlanned = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpSearchFilter = new System.Windows.Forms.GroupBox();
            this.lblFilterDates = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblStatusFilter = new System.Windows.Forms.Label();
            this.cmbStatusFilter = new System.Windows.Forms.ComboBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnFilter = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabDetailQuery.SuspendLayout();
            this.tabpgDetail.SuspendLayout();
            this.grpMaster.SuspendLayout();
            this.grpCosts.SuspendLayout();
            this.grpDetails.SuspendLayout();
            this.pnlDetailButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRawMaterials)).BeginInit();
            this.pnlActions.SuspendLayout();
            this.tabpgQuery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductionList)).BeginInit();
            this.grpSearchFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHeader.Controls.Add(this.lblHeaderShortcuts);
            this.pnlHeader.Controls.Add(this.lblHeaderSubtitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.pictureBox1);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1214, 54);
            this.pnlHeader.TabIndex = 218;
            // 
            // lblHeaderShortcuts
            // 
            this.lblHeaderShortcuts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHeaderShortcuts.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderShortcuts.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblHeaderShortcuts.Location = new System.Drawing.Point(750, 16);
            this.lblHeaderShortcuts.Name = "lblHeaderShortcuts";
            this.lblHeaderShortcuts.Size = new System.Drawing.Size(450, 20);
            this.lblHeaderShortcuts.TabIndex = 219;
            this.lblHeaderShortcuts.Text = "⌨  [F3] New Batch     [F5] Save Plan     [Esc] Close";
            this.lblHeaderShortcuts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHeaderSubtitle
            // 
            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblHeaderSubtitle.Location = new System.Drawing.Point(62, 32);
            this.lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            this.lblHeaderSubtitle.Size = new System.Drawing.Size(370, 13);
            this.lblHeaderSubtitle.TabIndex = 218;
            this.lblHeaderSubtitle.Text = "BOM recipe execution, raw materials consumption & batch costing (WIP)";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblTitle.Location = new System.Drawing.Point(60, 6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(355, 25);
            this.lblTitle.TabIndex = 216;
            this.lblTitle.Text = "Manufacturing / Production Voucher";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ERP.Properties.Resources.logoPNG;
            this.pictureBox1.Location = new System.Drawing.Point(14, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 40);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 217;
            this.pictureBox1.TabStop = false;
            // 
            // tabDetailQuery
            // 
            this.tabDetailQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabDetailQuery.Controls.Add(this.tabpgDetail);
            this.tabDetailQuery.Controls.Add(this.tabpgQuery);
            this.tabDetailQuery.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabDetailQuery.Location = new System.Drawing.Point(6, 60);
            this.tabDetailQuery.Name = "tabDetailQuery";
            this.tabDetailQuery.SelectedIndex = 0;
            this.tabDetailQuery.Size = new System.Drawing.Size(1202, 650);
            this.tabDetailQuery.TabIndex = 0;
            // 
            // tabpgDetail
            // 
            this.tabpgDetail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.tabpgDetail.Controls.Add(this.grpDetails);
            this.tabpgDetail.Controls.Add(this.grpCosts);
            this.tabpgDetail.Controls.Add(this.grpMaster);
            this.tabpgDetail.Controls.Add(this.pnlActions);
            this.tabpgDetail.Location = new System.Drawing.Point(4, 26);
            this.tabpgDetail.Name = "tabpgDetail";
            this.tabpgDetail.Padding = new System.Windows.Forms.Padding(6);
            this.tabpgDetail.Size = new System.Drawing.Size(1194, 620);
            this.tabpgDetail.TabIndex = 0;
            this.tabpgDetail.Text = "Production Batch";
            // 
            // grpMaster
            // 
            this.grpMaster.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMaster.BackColor = System.Drawing.Color.White;
            this.grpMaster.Controls.Add(this.lblRemarks);
            this.grpMaster.Controls.Add(this.txtRemarks);
            this.grpMaster.Controls.Add(this.lblRejectedQty);
            this.grpMaster.Controls.Add(this.txtRejectedQty);
            this.grpMaster.Controls.Add(this.lblProducedQty);
            this.grpMaster.Controls.Add(this.txtProducedQty);
            this.grpMaster.Controls.Add(this.lblPlannedQty);
            this.grpMaster.Controls.Add(this.txtPlannedQty);
            this.grpMaster.Controls.Add(this.lblFinishedItem);
            this.grpMaster.Controls.Add(this.cmbFinishedItem);
            this.grpMaster.Controls.Add(this.lblBom);
            this.grpMaster.Controls.Add(this.cmbBom);
            this.grpMaster.Controls.Add(this.lblStage);
            this.grpMaster.Controls.Add(this.txtStage);
            this.grpMaster.Controls.Add(this.lblBatchNo);
            this.grpMaster.Controls.Add(this.txtBatchNo);
            this.grpMaster.Controls.Add(this.lblStatusDisplay);
            this.grpMaster.Controls.Add(this.lblStatusTitle);
            this.grpMaster.Controls.Add(this.lblDate);
            this.grpMaster.Controls.Add(this.dtpDate);
            this.grpMaster.Controls.Add(this.lblVoucherNo);
            this.grpMaster.Controls.Add(this.txtVoucherNo);
            this.grpMaster.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpMaster.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpMaster.Location = new System.Drawing.Point(8, 6);
            this.grpMaster.Name = "grpMaster";
            this.grpMaster.Size = new System.Drawing.Size(1178, 142);
            this.grpMaster.TabIndex = 0;
            this.grpMaster.TabStop = false;
            this.grpMaster.Text = " Production Batch Details ";
            // 
            // lblVoucherNo
            // 
            this.lblVoucherNo.AutoSize = true;
            this.lblVoucherNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVoucherNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblVoucherNo.Location = new System.Drawing.Point(12, 23);
            this.lblVoucherNo.Name = "lblVoucherNo";
            this.lblVoucherNo.Size = new System.Drawing.Size(64, 15);
            this.lblVoucherNo.TabIndex = 0;
            this.lblVoucherNo.Text = "Voucher #:";
            // 
            // txtVoucherNo
            // 
            this.txtVoucherNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtVoucherNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVoucherNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtVoucherNo.Location = new System.Drawing.Point(80, 20);
            this.txtVoucherNo.Name = "txtVoucherNo";
            this.txtVoucherNo.ReadOnly = true;
            this.txtVoucherNo.Size = new System.Drawing.Size(105, 23);
            this.txtVoucherNo.TabIndex = 157;
            this.txtVoucherNo.TabStop = false;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblDate.Location = new System.Drawing.Point(200, 23);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(34, 15);
            this.lblDate.TabIndex = 1;
            this.lblDate.Text = "Date:";
            // 
            // dtpDate
            // 
            this.dtpDate.CustomFormat = "dd-MMM-yyyy";
            this.dtpDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(240, 20);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(125, 23);
            this.dtpDate.TabIndex = 0;
            this.dtpDate.TabStop = true;
            // 
            // lblStatusTitle
            // 
            this.lblStatusTitle.AutoSize = true;
            this.lblStatusTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblStatusTitle.Location = new System.Drawing.Point(380, 23);
            this.lblStatusTitle.Name = "lblStatusTitle";
            this.lblStatusTitle.Size = new System.Drawing.Size(42, 15);
            this.lblStatusTitle.TabIndex = 2;
            this.lblStatusTitle.Text = "Status:";
            // 
            // lblStatusDisplay
            // 
            this.lblStatusDisplay.AutoSize = true;
            this.lblStatusDisplay.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusDisplay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(64)))), ((int)(((byte)(175)))));
            this.lblStatusDisplay.Location = new System.Drawing.Point(426, 22);
            this.lblStatusDisplay.Name = "lblStatusDisplay";
            this.lblStatusDisplay.Size = new System.Drawing.Size(58, 17);
            this.lblStatusDisplay.TabIndex = 3;
            this.lblStatusDisplay.Text = "Planned";
            // 
            // lblBatchNo
            // 
            this.lblBatchNo.AutoSize = true;
            this.lblBatchNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBatchNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblBatchNo.Location = new System.Drawing.Point(535, 23);
            this.lblBatchNo.Name = "lblBatchNo";
            this.lblBatchNo.Size = new System.Drawing.Size(50, 15);
            this.lblBatchNo.TabIndex = 4;
            this.lblBatchNo.Text = "Batch #:";
            // 
            // txtBatchNo
            // 
            this.txtBatchNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBatchNo.Location = new System.Drawing.Point(590, 20);
            this.txtBatchNo.Name = "txtBatchNo";
            this.txtBatchNo.Size = new System.Drawing.Size(155, 23);
            this.txtBatchNo.TabIndex = 4;
            this.txtBatchNo.TabStop = true;
            // 
            // lblStage
            // 
            this.lblStage.AutoSize = true;
            this.lblStage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblStage.Location = new System.Drawing.Point(765, 23);
            this.lblStage.Name = "lblStage";
            this.lblStage.Size = new System.Drawing.Size(64, 15);
            this.lblStage.TabIndex = 5;
            this.lblStage.Text = "WIP Stage:";
            // 
            // txtStage
            // 
            this.txtStage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStage.Location = new System.Drawing.Point(835, 20);
            this.txtStage.Name = "txtStage";
            this.txtStage.Size = new System.Drawing.Size(170, 23);
            this.txtStage.TabIndex = 160;
            this.txtStage.TabStop = false;
            // 
            // lblBom
            // 
            this.lblBom.AutoSize = true;
            this.lblBom.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblBom.Location = new System.Drawing.Point(12, 59);
            this.lblBom.Name = "lblBom";
            this.lblBom.Size = new System.Drawing.Size(68, 15);
            this.lblBom.TabIndex = 6;
            this.lblBom.Text = "Select BOM:";
            // 
            // cmbBom
            // 
            this.cmbBom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBom.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBom.FormattingEnabled = true;
            this.cmbBom.Location = new System.Drawing.Point(80, 56);
            this.cmbBom.Name = "cmbBom";
            this.cmbBom.Size = new System.Drawing.Size(285, 23);
            this.cmbBom.TabIndex = 1;
            this.cmbBom.TabStop = true;
            this.cmbBom.SelectedIndexChanged += new System.EventHandler(this.cmbBom_SelectedIndexChanged);
            // 
            // lblFinishedItem
            // 
            this.lblFinishedItem.AutoSize = true;
            this.lblFinishedItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFinishedItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblFinishedItem.Location = new System.Drawing.Point(380, 59);
            this.lblFinishedItem.Name = "lblFinishedItem";
            this.lblFinishedItem.Size = new System.Drawing.Size(76, 15);
            this.lblFinishedItem.TabIndex = 7;
            this.lblFinishedItem.Text = "Output Item:";
            // 
            // cmbFinishedItem
            // 
            this.cmbFinishedItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFinishedItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbFinishedItem.FormattingEnabled = true;
            this.cmbFinishedItem.Location = new System.Drawing.Point(462, 56);
            this.cmbFinishedItem.Name = "cmbFinishedItem";
            this.cmbFinishedItem.Size = new System.Drawing.Size(283, 23);
            this.cmbFinishedItem.TabIndex = 2;
            this.cmbFinishedItem.TabStop = true;
            // 
            // lblPlannedQty
            // 
            this.lblPlannedQty.AutoSize = true;
            this.lblPlannedQty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlannedQty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblPlannedQty.Location = new System.Drawing.Point(765, 59);
            this.lblPlannedQty.Name = "lblPlannedQty";
            this.lblPlannedQty.Size = new System.Drawing.Size(75, 15);
            this.lblPlannedQty.TabIndex = 8;
            this.lblPlannedQty.Text = "Planned Qty:";
            // 
            // txtPlannedQty
            // 
            this.txtPlannedQty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPlannedQty.Location = new System.Drawing.Point(845, 56);
            this.txtPlannedQty.Name = "txtPlannedQty";
            this.txtPlannedQty.Size = new System.Drawing.Size(80, 23);
            this.txtPlannedQty.TabIndex = 3;
            this.txtPlannedQty.TabStop = true;
            this.txtPlannedQty.Text = "1";
            this.txtPlannedQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPlannedQty.TextChanged += new System.EventHandler(this.txtPlannedQty_TextChanged);
            // 
            // lblProducedQty
            // 
            this.lblProducedQty.AutoSize = true;
            this.lblProducedQty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProducedQty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblProducedQty.Location = new System.Drawing.Point(940, 59);
            this.lblProducedQty.Name = "lblProducedQty";
            this.lblProducedQty.Size = new System.Drawing.Size(82, 15);
            this.lblProducedQty.TabIndex = 9;
            this.lblProducedQty.Text = "Produced Qty:";
            // 
            // txtProducedQty
            // 
            this.txtProducedQty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProducedQty.Location = new System.Drawing.Point(1028, 56);
            this.txtProducedQty.Name = "txtProducedQty";
            this.txtProducedQty.Size = new System.Drawing.Size(75, 23);
            this.txtProducedQty.TabIndex = 162;
            this.txtProducedQty.TabStop = false;
            this.txtProducedQty.Text = "0";
            this.txtProducedQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtProducedQty.TextChanged += new System.EventHandler(this.CalculateUnitCost);
            // 
            // lblRejectedQty
            // 
            this.lblRejectedQty.AutoSize = true;
            this.lblRejectedQty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRejectedQty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblRejectedQty.Location = new System.Drawing.Point(12, 98);
            this.lblRejectedQty.Name = "lblRejectedQty";
            this.lblRejectedQty.Size = new System.Drawing.Size(76, 15);
            this.lblRejectedQty.TabIndex = 10;
            this.lblRejectedQty.Text = "Rejected Qty:";
            // 
            // txtRejectedQty
            // 
            this.txtRejectedQty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRejectedQty.Location = new System.Drawing.Point(90, 95);
            this.txtRejectedQty.Name = "txtRejectedQty";
            this.txtRejectedQty.Size = new System.Drawing.Size(70, 23);
            this.txtRejectedQty.TabIndex = 163;
            this.txtRejectedQty.TabStop = false;
            this.txtRejectedQty.Text = "0";
            this.txtRejectedQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblRemarks
            // 
            this.lblRemarks.AutoSize = true;
            this.lblRemarks.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRemarks.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblRemarks.Location = new System.Drawing.Point(180, 98);
            this.lblRemarks.Name = "lblRemarks";
            this.lblRemarks.Size = new System.Drawing.Size(55, 15);
            this.lblRemarks.TabIndex = 11;
            this.lblRemarks.Text = "Remarks:";
            // 
            // txtRemarks
            // 
            this.txtRemarks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemarks.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRemarks.Location = new System.Drawing.Point(240, 95);
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new System.Drawing.Size(863, 23);
            this.txtRemarks.TabIndex = 164;
            this.txtRemarks.TabStop = false;
            // 
            // grpCosts
            // 
            this.grpCosts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCosts.BackColor = System.Drawing.Color.White;
            this.grpCosts.Controls.Add(this.txtUnitCost);
            this.grpCosts.Controls.Add(this.lblUnitCost);
            this.grpCosts.Controls.Add(this.txtTotalCost);
            this.grpCosts.Controls.Add(this.lblTotalCost);
            this.grpCosts.Controls.Add(this.txtOverheadCost);
            this.grpCosts.Controls.Add(this.lblOverheadCost);
            this.grpCosts.Controls.Add(this.txtLaborCost);
            this.grpCosts.Controls.Add(this.lblLaborCost);
            this.grpCosts.Controls.Add(this.txtRawCost);
            this.grpCosts.Controls.Add(this.lblRawCost);
            this.grpCosts.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpCosts.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpCosts.Location = new System.Drawing.Point(8, 154);
            this.grpCosts.Name = "grpCosts";
            this.grpCosts.Size = new System.Drawing.Size(1178, 54);
            this.grpCosts.TabIndex = 1;
            this.grpCosts.TabStop = false;
            this.grpCosts.Text = " Cost Summary (PKR) ";
            // 
            // lblRawCost
            // 
            this.lblRawCost.AutoSize = true;
            this.lblRawCost.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRawCost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblRawCost.Location = new System.Drawing.Point(12, 23);
            this.lblRawCost.Name = "lblRawCost";
            this.lblRawCost.Size = new System.Drawing.Size(86, 15);
            this.lblRawCost.TabIndex = 0;
            this.lblRawCost.Text = "Raw Mat. Cost:";
            // 
            // txtRawCost
            // 
            this.txtRawCost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtRawCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRawCost.Location = new System.Drawing.Point(100, 20);
            this.txtRawCost.Name = "txtRawCost";
            this.txtRawCost.ReadOnly = true;
            this.txtRawCost.Size = new System.Drawing.Size(95, 23);
            this.txtRawCost.TabIndex = 170;
            this.txtRawCost.TabStop = false;
            this.txtRawCost.Text = "0.00";
            this.txtRawCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblLaborCost
            // 
            this.lblLaborCost.AutoSize = true;
            this.lblLaborCost.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLaborCost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblLaborCost.Location = new System.Drawing.Point(215, 23);
            this.lblLaborCost.Name = "lblLaborCost";
            this.lblLaborCost.Size = new System.Drawing.Size(40, 15);
            this.lblLaborCost.TabIndex = 1;
            this.lblLaborCost.Text = "Labor:";
            // 
            // txtLaborCost
            // 
            this.txtLaborCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLaborCost.Location = new System.Drawing.Point(258, 20);
            this.txtLaborCost.Name = "txtLaborCost";
            this.txtLaborCost.Size = new System.Drawing.Size(85, 23);
            this.txtLaborCost.TabIndex = 171;
            this.txtLaborCost.TabStop = false;
            this.txtLaborCost.Text = "0.00";
            this.txtLaborCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLaborCost.TextChanged += new System.EventHandler(this.CalculateUnitCost);
            // 
            // lblOverheadCost
            // 
            this.lblOverheadCost.AutoSize = true;
            this.lblOverheadCost.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOverheadCost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblOverheadCost.Location = new System.Drawing.Point(360, 23);
            this.lblOverheadCost.Name = "lblOverheadCost";
            this.lblOverheadCost.Size = new System.Drawing.Size(61, 15);
            this.lblOverheadCost.TabIndex = 2;
            this.lblOverheadCost.Text = "Overhead:";
            // 
            // txtOverheadCost
            // 
            this.txtOverheadCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOverheadCost.Location = new System.Drawing.Point(425, 20);
            this.txtOverheadCost.Name = "txtOverheadCost";
            this.txtOverheadCost.Size = new System.Drawing.Size(85, 23);
            this.txtOverheadCost.TabIndex = 172;
            this.txtOverheadCost.TabStop = false;
            this.txtOverheadCost.Text = "0.00";
            this.txtOverheadCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtOverheadCost.TextChanged += new System.EventHandler(this.CalculateUnitCost);
            // 
            // lblTotalCost
            // 
            this.lblTotalCost.AutoSize = true;
            this.lblTotalCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblTotalCost.Location = new System.Drawing.Point(535, 23);
            this.lblTotalCost.Name = "lblTotalCost";
            this.lblTotalCost.Size = new System.Drawing.Size(65, 15);
            this.lblTotalCost.TabIndex = 3;
            this.lblTotalCost.Text = "Total Cost:";
            // 
            // txtTotalCost
            // 
            this.txtTotalCost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtTotalCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotalCost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtTotalCost.Location = new System.Drawing.Point(605, 20);
            this.txtTotalCost.Name = "txtTotalCost";
            this.txtTotalCost.ReadOnly = true;
            this.txtTotalCost.Size = new System.Drawing.Size(105, 23);
            this.txtTotalCost.TabIndex = 173;
            this.txtTotalCost.TabStop = false;
            this.txtTotalCost.Text = "0.00";
            this.txtTotalCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblUnitCost
            // 
            this.lblUnitCost.AutoSize = true;
            this.lblUnitCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUnitCost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(128)))), ((int)(((byte)(61)))));
            this.lblUnitCost.Location = new System.Drawing.Point(730, 23);
            this.lblUnitCost.Name = "lblUnitCost";
            this.lblUnitCost.Size = new System.Drawing.Size(61, 15);
            this.lblUnitCost.TabIndex = 4;
            this.lblUnitCost.Text = "Unit Cost:";
            // 
            // txtUnitCost
            // 
            this.txtUnitCost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(253)))), ((int)(((byte)(244)))));
            this.txtUnitCost.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUnitCost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(128)))), ((int)(((byte)(61)))));
            this.txtUnitCost.Location = new System.Drawing.Point(795, 19);
            this.txtUnitCost.Name = "txtUnitCost";
            this.txtUnitCost.ReadOnly = true;
            this.txtUnitCost.Size = new System.Drawing.Size(105, 24);
            this.txtUnitCost.TabIndex = 174;
            this.txtUnitCost.TabStop = false;
            this.txtUnitCost.Text = "0.00";
            this.txtUnitCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // grpDetails
            // 
            this.grpDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDetails.BackColor = System.Drawing.Color.White;
            this.grpDetails.Controls.Add(this.dgvRawMaterials);
            this.grpDetails.Controls.Add(this.pnlDetailButtons);
            this.grpDetails.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDetails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpDetails.Location = new System.Drawing.Point(8, 214);
            this.grpDetails.Name = "grpDetails";
            this.grpDetails.Size = new System.Drawing.Size(1178, 280);
            this.grpDetails.TabIndex = 2;
            this.grpDetails.TabStop = false;
            this.grpDetails.Text = " Raw Material Issuance & Consumption ";
            // 
            // pnlDetailButtons
            // 
            this.pnlDetailButtons.Controls.Add(this.btnDeleteRow);
            this.pnlDetailButtons.Controls.Add(this.btnAddRow);
            this.pnlDetailButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDetailButtons.Location = new System.Drawing.Point(3, 19);
            this.pnlDetailButtons.Name = "pnlDetailButtons";
            this.pnlDetailButtons.Size = new System.Drawing.Size(1172, 34);
            this.pnlDetailButtons.TabIndex = 0;
            // 
            // btnAddRow
            // 
            this.btnAddRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnAddRow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddRow.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnAddRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddRow.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.btnAddRow.Location = new System.Drawing.Point(4, 3);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(130, 27);
            this.btnAddRow.TabIndex = 0;
            this.btnAddRow.TabStop = false;
            this.btnAddRow.Text = "+ Add Material Line";
            this.btnAddRow.UseVisualStyleBackColor = false;
            this.btnAddRow.Click += new System.EventHandler(this.btnAddRow_Click);
            // 
            // btnDeleteRow
            // 
            this.btnDeleteRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.btnDeleteRow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeleteRow.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.btnDeleteRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteRow.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDeleteRow.Location = new System.Drawing.Point(140, 3);
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(110, 27);
            this.btnDeleteRow.TabIndex = 1;
            this.btnDeleteRow.TabStop = false;
            this.btnDeleteRow.Text = "- Remove Line";
            this.btnDeleteRow.UseVisualStyleBackColor = false;
            this.btnDeleteRow.Click += new System.EventHandler(this.btnDeleteRow_Click);
            // 
            // dgvRawMaterials
            // 
            this.dgvRawMaterials.AllowUserToAddRows = false;
            this.dgvRawMaterials.AllowUserToDeleteRows = false;
            this.dgvRawMaterials.AllowUserToOrderColumns = true;
            this.dgvRawMaterials.AllowUserToResizeColumns = true;
            this.dgvRawMaterials.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRawMaterials.BackgroundColor = System.Drawing.Color.White;
            this.dgvRawMaterials.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgvRawMaterials.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRawMaterials.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvRawMaterials.ColumnHeadersHeight = 32;
            this.dgvRawMaterials.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvRawMaterials.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnRawItem,
            this.clnPlannedQty,
            this.clnActualQty,
            this.clnWastageQty,
            this.clnUnitCost,
            this.clnTotalCost,
            this.clnDetailNotes});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRawMaterials.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvRawMaterials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRawMaterials.EnableHeadersVisualStyles = false;
            this.dgvRawMaterials.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dgvRawMaterials.Location = new System.Drawing.Point(3, 53);
            this.dgvRawMaterials.MultiSelect = false;
            this.dgvRawMaterials.Name = "dgvRawMaterials";
            this.dgvRawMaterials.RowHeadersWidth = 24;
            this.dgvRawMaterials.RowTemplate.Height = 28;
            this.dgvRawMaterials.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvRawMaterials.Size = new System.Drawing.Size(1172, 224);
            this.dgvRawMaterials.TabIndex = 5;
            this.dgvRawMaterials.TabStop = true;
            this.dgvRawMaterials.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRawMaterials_CellValueChanged);
            // 
            // clnRawItem
            // 
            this.clnRawItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clnRawItem.HeaderText = "Raw Material / Component";
            this.clnRawItem.MinimumWidth = 180;
            this.clnRawItem.Name = "clnRawItem";
            this.clnRawItem.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clnRawItem.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.clnRawItem.FillWeight = 140F;
            // 
            // clnPlannedQty
            // 
            this.clnPlannedQty.HeaderText = "Planned Qty";
            this.clnPlannedQty.MinimumWidth = 85;
            this.clnPlannedQty.Name = "clnPlannedQty";
            this.clnPlannedQty.FillWeight = 50F;
            // 
            // clnActualQty
            // 
            this.clnActualQty.HeaderText = "Actual Qty";
            this.clnActualQty.MinimumWidth = 85;
            this.clnActualQty.Name = "clnActualQty";
            this.clnActualQty.FillWeight = 50F;
            // 
            // clnWastageQty
            // 
            this.clnWastageQty.HeaderText = "Wastage Qty";
            this.clnWastageQty.MinimumWidth = 85;
            this.clnWastageQty.Name = "clnWastageQty";
            this.clnWastageQty.FillWeight = 50F;
            // 
            // clnUnitCost
            // 
            this.clnUnitCost.HeaderText = "Unit Cost";
            this.clnUnitCost.MinimumWidth = 85;
            this.clnUnitCost.Name = "clnUnitCost";
            this.clnUnitCost.FillWeight = 50F;
            // 
            // clnTotalCost
            // 
            this.clnTotalCost.HeaderText = "Line Cost";
            this.clnTotalCost.MinimumWidth = 95;
            this.clnTotalCost.Name = "clnTotalCost";
            this.clnTotalCost.ReadOnly = true;
            this.clnTotalCost.FillWeight = 60F;
            // 
            // clnDetailNotes
            // 
            this.clnDetailNotes.HeaderText = "Notes / Batch Details";
            this.clnDetailNotes.MinimumWidth = 140;
            this.clnDetailNotes.Name = "clnDetailNotes";
            this.clnDetailNotes.FillWeight = 85F;
            // 
            // pnlActions
            // 
            this.pnlActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlActions.BackColor = System.Drawing.Color.White;
            this.pnlActions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlActions.Controls.Add(this.lblShortcutsBar);
            this.pnlActions.Controls.Add(this.btnSave);
            this.pnlActions.Controls.Add(this.btnNew);
            this.pnlActions.Controls.Add(this.btnIssueMaterials);
            this.pnlActions.Controls.Add(this.btnComplete);
            this.pnlActions.Controls.Add(this.btnCancel);
            this.pnlActions.Controls.Add(this.btnDelete);
            this.pnlActions.Controls.Add(this.btnClose);
            this.pnlActions.Location = new System.Drawing.Point(8, 500);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1178, 114);
            this.pnlActions.TabIndex = 3;
            // 
            // lblShortcutsBar
            // 
            this.lblShortcutsBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblShortcutsBar.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShortcutsBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblShortcutsBar.Location = new System.Drawing.Point(10, 60);
            this.lblShortcutsBar.Name = "lblShortcutsBar";
            this.lblShortcutsBar.Size = new System.Drawing.Size(1156, 45);
            this.lblShortcutsBar.TabIndex = 171;
            this.lblShortcutsBar.Text = @"⌨ Quick Reference:

• [F3] New Batch  •  [F5] Save Plan  •  [Esc] Close

• Use Enter to advance from Header fields into Raw Materials Grid, then directly onto Save.";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(10, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(125, 42);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "&Save Plan [F5]";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNew.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnNew.Location = new System.Drawing.Point(143, 10);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(95, 42);
            this.btnNew.TabIndex = 1;
            this.btnNew.TabStop = false;
            this.btnNew.Text = "&New [F3]";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnIssueMaterials
            // 
            this.btnIssueMaterials.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.btnIssueMaterials.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIssueMaterials.FlatAppearance.BorderSize = 0;
            this.btnIssueMaterials.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIssueMaterials.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIssueMaterials.ForeColor = System.Drawing.Color.White;
            this.btnIssueMaterials.Location = new System.Drawing.Point(246, 10);
            this.btnIssueMaterials.Name = "btnIssueMaterials";
            this.btnIssueMaterials.Size = new System.Drawing.Size(155, 42);
            this.btnIssueMaterials.TabIndex = 2;
            this.btnIssueMaterials.TabStop = false;
            this.btnIssueMaterials.Text = "Issue &Materials (WIP)";
            this.btnIssueMaterials.UseVisualStyleBackColor = false;
            this.btnIssueMaterials.Click += new System.EventHandler(this.btnIssueMaterials_Click);
            // 
            // btnComplete
            // 
            this.btnComplete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnComplete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnComplete.FlatAppearance.BorderSize = 0;
            this.btnComplete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComplete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnComplete.ForeColor = System.Drawing.Color.White;
            this.btnComplete.Location = new System.Drawing.Point(409, 10);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(145, 42);
            this.btnComplete.TabIndex = 3;
            this.btnComplete.TabStop = false;
            this.btnComplete.Text = "&Complete Batch";
            this.btnComplete.UseVisualStyleBackColor = false;
            this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnCancel.Location = new System.Drawing.Point(562, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 42);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.TabStop = false;
            this.btnCancel.Text = "Cance&l";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.Location = new System.Drawing.Point(655, 10);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(85, 42);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.TabStop = false;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnClose.Location = new System.Drawing.Point(748, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(85, 42);
            this.btnClose.TabIndex = 6;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "Close [Esc]";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tabpgQuery
            // 
            this.tabpgQuery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.tabpgQuery.Controls.Add(this.dgvProductionList);
            this.tabpgQuery.Controls.Add(this.grpSearchFilter);
            this.tabpgQuery.Location = new System.Drawing.Point(4, 26);
            this.tabpgQuery.Name = "tabpgQuery";
            this.tabpgQuery.Padding = new System.Windows.Forms.Padding(6);
            this.tabpgQuery.Size = new System.Drawing.Size(1194, 620);
            this.tabpgQuery.TabIndex = 1;
            this.tabpgQuery.Text = "Search Batches";
            // 
            // dgvProductionList
            // 
            this.dgvProductionList.AllowUserToAddRows = false;
            this.dgvProductionList.AllowUserToDeleteRows = false;
            this.dgvProductionList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProductionList.BackgroundColor = System.Drawing.Color.White;
            this.dgvProductionList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgvProductionList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvProductionList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvProductionList.ColumnHeadersHeight = 32;
            this.dgvProductionList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvProductionList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnListVoucher,
            this.clnListDate,
            this.clnListItem,
            this.clnListPlanned,
            this.clnListStatus});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvProductionList.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvProductionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductionList.EnableHeadersVisualStyles = false;
            this.dgvProductionList.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dgvProductionList.Location = new System.Drawing.Point(6, 92);
            this.dgvProductionList.MultiSelect = false;
            this.dgvProductionList.Name = "dgvProductionList";
            this.dgvProductionList.ReadOnly = true;
            this.dgvProductionList.RowHeadersWidth = 24;
            this.dgvProductionList.RowTemplate.Height = 28;
            this.dgvProductionList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductionList.Size = new System.Drawing.Size(1182, 522);
            this.dgvProductionList.TabIndex = 1;
            // 
            // clnListVoucher
            // 
            this.clnListVoucher.DataPropertyName = "VoucherNo";
            this.clnListVoucher.FillWeight = 65F;
            this.clnListVoucher.HeaderText = "Voucher #";
            this.clnListVoucher.Name = "clnListVoucher";
            this.clnListVoucher.ReadOnly = true;
            // 
            // clnListDate
            // 
            this.clnListDate.DataPropertyName = "Date";
            this.clnListDate.FillWeight = 55F;
            this.clnListDate.HeaderText = "Date";
            this.clnListDate.Name = "clnListDate";
            this.clnListDate.ReadOnly = true;
            // 
            // clnListItem
            // 
            this.clnListItem.DataPropertyName = "FinishedItemName";
            this.clnListItem.FillWeight = 110F;
            this.clnListItem.HeaderText = "Output Product";
            this.clnListItem.Name = "clnListItem";
            this.clnListItem.ReadOnly = true;
            // 
            // clnListPlanned
            // 
            this.clnListPlanned.DataPropertyName = "PlannedQuantity";
            this.clnListPlanned.FillWeight = 45F;
            this.clnListPlanned.HeaderText = "Planned Qty";
            this.clnListPlanned.Name = "clnListPlanned";
            this.clnListPlanned.ReadOnly = true;
            // 
            // clnListStatus
            // 
            this.clnListStatus.DataPropertyName = "Status";
            this.clnListStatus.FillWeight = 55F;
            this.clnListStatus.HeaderText = "Status";
            this.clnListStatus.Name = "clnListStatus";
            this.clnListStatus.ReadOnly = true;
            // 
            // grpSearchFilter
            // 
            this.grpSearchFilter.BackColor = System.Drawing.Color.White;
            this.grpSearchFilter.Controls.Add(this.btnFilter);
            this.grpSearchFilter.Controls.Add(this.txtSearch);
            this.grpSearchFilter.Controls.Add(this.lblSearch);
            this.grpSearchFilter.Controls.Add(this.cmbStatusFilter);
            this.grpSearchFilter.Controls.Add(this.lblStatusFilter);
            this.grpSearchFilter.Controls.Add(this.dtpTo);
            this.grpSearchFilter.Controls.Add(this.lblTo);
            this.grpSearchFilter.Controls.Add(this.dtpFrom);
            this.grpSearchFilter.Controls.Add(this.lblFilterDates);
            this.grpSearchFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSearchFilter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpSearchFilter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpSearchFilter.Location = new System.Drawing.Point(6, 6);
            this.grpSearchFilter.Name = "grpSearchFilter";
            this.grpSearchFilter.Size = new System.Drawing.Size(1182, 86);
            this.grpSearchFilter.TabIndex = 0;
            this.grpSearchFilter.TabStop = false;
            this.grpSearchFilter.Text = " Filter & Search Production Batches ";
            // 
            // lblFilterDates
            // 
            this.lblFilterDates.AutoSize = true;
            this.lblFilterDates.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterDates.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblFilterDates.Location = new System.Drawing.Point(12, 28);
            this.lblFilterDates.Name = "lblFilterDates";
            this.lblFilterDates.Size = new System.Drawing.Size(38, 15);
            this.lblFilterDates.TabIndex = 0;
            this.lblFilterDates.Text = "From:";
            // 
            // dtpFrom
            // 
            this.dtpFrom.CustomFormat = "dd-MMM-yyyy";
            this.dtpFrom.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(54, 25);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(115, 23);
            this.dtpFrom.TabIndex = 0;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblTo.Location = new System.Drawing.Point(180, 28);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(22, 15);
            this.lblTo.TabIndex = 1;
            this.lblTo.Text = "To:";
            // 
            // dtpTo
            // 
            this.dtpTo.CustomFormat = "dd-MMM-yyyy";
            this.dtpTo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(208, 25);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(115, 23);
            this.dtpTo.TabIndex = 1;
            // 
            // lblStatusFilter
            // 
            this.lblStatusFilter.AutoSize = true;
            this.lblStatusFilter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusFilter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblStatusFilter.Location = new System.Drawing.Point(340, 28);
            this.lblStatusFilter.Name = "lblStatusFilter";
            this.lblStatusFilter.Size = new System.Drawing.Size(42, 15);
            this.lblStatusFilter.TabIndex = 2;
            this.lblStatusFilter.Text = "Status:";
            // 
            // cmbStatusFilter
            // 
            this.cmbStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatusFilter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbStatusFilter.FormattingEnabled = true;
            this.cmbStatusFilter.Items.AddRange(new object[] {
            "All",
            "Planned",
            "InProgress",
            "Completed",
            "Cancelled"});
            this.cmbStatusFilter.Location = new System.Drawing.Point(388, 25);
            this.cmbStatusFilter.Name = "cmbStatusFilter";
            this.cmbStatusFilter.Size = new System.Drawing.Size(130, 23);
            this.cmbStatusFilter.TabIndex = 2;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblSearch.Location = new System.Drawing.Point(535, 28);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(45, 15);
            this.lblSearch.TabIndex = 3;
            this.lblSearch.Text = "Search:";
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(585, 25);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(260, 23);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // btnFilter
            // 
            this.btnFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilter.FlatAppearance.BorderSize = 0;
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilter.ForeColor = System.Drawing.Color.White;
            this.btnFilter.Location = new System.Drawing.Point(860, 22);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(100, 28);
            this.btnFilter.TabIndex = 4;
            this.btnFilter.Text = "&Filter";
            this.btnFilter.UseVisualStyleBackColor = false;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // frmManufacturing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(1214, 715);
            this.Controls.Add(this.tabDetailQuery);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Name = "frmManufacturing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manufacturing / Production Voucher";
            this.Load += new System.EventHandler(this.frmManufacturing_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmManufacturing_KeyDown);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabDetailQuery.ResumeLayout(false);
            this.tabpgDetail.ResumeLayout(false);
            this.grpMaster.ResumeLayout(false);
            this.grpMaster.PerformLayout();
            this.grpCosts.ResumeLayout(false);
            this.grpCosts.PerformLayout();
            this.grpDetails.ResumeLayout(false);
            this.pnlDetailButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRawMaterials)).EndInit();
            this.pnlActions.ResumeLayout(false);
            this.tabpgQuery.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductionList)).EndInit();
            this.grpSearchFilter.ResumeLayout(false);
            this.grpSearchFilter.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHeaderShortcuts;
        private System.Windows.Forms.Label lblHeaderSubtitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabControl tabDetailQuery;
        private System.Windows.Forms.TabPage tabpgDetail;
        private System.Windows.Forms.GroupBox grpMaster;
        private System.Windows.Forms.Label lblVoucherNo;
        private System.Windows.Forms.TextBox txtVoucherNo;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label lblStatusTitle;
        private System.Windows.Forms.Label lblStatusDisplay;
        private System.Windows.Forms.Label lblBatchNo;
        private System.Windows.Forms.TextBox txtBatchNo;
        private System.Windows.Forms.Label lblStage;
        private System.Windows.Forms.TextBox txtStage;
        private System.Windows.Forms.Label lblBom;
        private System.Windows.Forms.ComboBox cmbBom;
        private System.Windows.Forms.Label lblFinishedItem;
        private System.Windows.Forms.ComboBox cmbFinishedItem;
        private System.Windows.Forms.Label lblPlannedQty;
        private System.Windows.Forms.TextBox txtPlannedQty;
        private System.Windows.Forms.Label lblProducedQty;
        private System.Windows.Forms.TextBox txtProducedQty;
        private System.Windows.Forms.Label lblRejectedQty;
        private System.Windows.Forms.TextBox txtRejectedQty;
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
        private System.Windows.Forms.Label lblShortcutsBar;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnIssueMaterials;
        private System.Windows.Forms.Button btnComplete;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TabPage tabpgQuery;
        private System.Windows.Forms.GroupBox grpSearchFilter;
        private System.Windows.Forms.Label lblFilterDates;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblTo;
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
    }
}
