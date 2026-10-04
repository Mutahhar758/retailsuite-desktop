namespace ERP.Forms
{
    partial class frmRepairJob
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnBillJob = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpJobCard = new System.Windows.Forms.TabPage();
            this.gbTotals = new System.Windows.Forms.GroupBox();
            this.lblSaleVoucher = new System.Windows.Forms.Label();
            this.txtBalanceDue = new System.Windows.Forms.TextBox();
            this.lblBalanceDue = new System.Windows.Forms.Label();
            this.cmbAdvanceAccount = new System.Windows.Forms.ComboBox();
            this.lblAdvanceAccount = new System.Windows.Forms.Label();
            this.txtAdvanceReceived = new System.Windows.Forms.TextBox();
            this.lblAdvance = new System.Windows.Forms.Label();
            this.txtNetTotal = new System.Windows.Forms.TextBox();
            this.lblNetTotal = new System.Windows.Forms.Label();
            this.txtServicesTotal = new System.Windows.Forms.TextBox();
            this.lblServicesTotal = new System.Windows.Forms.Label();
            this.txtPartsTotal = new System.Windows.Forms.TextBox();
            this.lblPartsTotal = new System.Windows.Forms.Label();
            this.txtEstimatedCost = new System.Windows.Forms.TextBox();
            this.lblEstimatedCost = new System.Windows.Forms.Label();
            this.tcDetails = new System.Windows.Forms.TabControl();
            this.tpParts = new System.Windows.Forms.TabPage();
            this.dgvParts = new System.Windows.Forms.DataGridView();
            this.clnPartItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnPartItemTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnPartQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnPartPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnPartTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpServices = new System.Windows.Forms.TabPage();
            this.dgvServices = new System.Windows.Forms.DataGridView();
            this.clnServiceDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnServiceAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbDevice = new System.Windows.Forms.GroupBox();
            this.txtTechnicianNotes = new System.Windows.Forms.TextBox();
            this.lblTechnicianNotes = new System.Windows.Forms.Label();
            this.cmbTechnician = new System.Windows.Forms.ComboBox();
            this.lblTechnician = new System.Windows.Forms.Label();
            this.txtAccessories = new System.Windows.Forms.TextBox();
            this.lblAccessories = new System.Windows.Forms.Label();
            this.txtPhysicalCondition = new System.Windows.Forms.TextBox();
            this.lblCondition = new System.Windows.Forms.Label();
            this.txtProblem = new System.Windows.Forms.TextBox();
            this.lblProblem = new System.Windows.Forms.Label();
            this.txtPasscode = new System.Windows.Forms.TextBox();
            this.lblPasscode = new System.Windows.Forms.Label();
            this.txtImei = new System.Windows.Forms.TextBox();
            this.lblImei = new System.Windows.Forms.Label();
            this.txtColor = new System.Windows.Forms.TextBox();
            this.lblColor = new System.Windows.Forms.Label();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.lblModel = new System.Windows.Forms.Label();
            this.cmbBrand = new System.Windows.Forms.ComboBox();
            this.lblBrand = new System.Windows.Forms.Label();
            this.gbJob = new System.Windows.Forms.GroupBox();
            this.txtCustomerPhone = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.dtpJobDate = new System.Windows.Forms.DateTimePicker();
            this.lblDate = new System.Windows.Forms.Label();
            this.txtJobNo = new System.Windows.Forms.TextBox();
            this.lblJobNo = new System.Windows.Forms.Label();
            this.tpJobList = new System.Windows.Forms.TabPage();
            this.dgvJobList = new System.Windows.Forms.DataGridView();
            this.clnListJobNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListCustomer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListPhone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListDevice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListImei = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListNetTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListAdvance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListBalance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlListFilter = new System.Windows.Forms.Panel();
            this.btnListRefresh = new System.Windows.Forms.Button();
            this.txtListSearch = new System.Windows.Forms.TextBox();
            this.lblListSearch = new System.Windows.Forms.Label();
            this.cmbListStatus = new System.Windows.Forms.ComboBox();
            this.lblListStatus = new System.Windows.Forms.Label();
            this.dtpListTo = new System.Windows.Forms.DateTimePicker();
            this.lblListTo = new System.Windows.Forms.Label();
            this.dtpListFrom = new System.Windows.Forms.DateTimePicker();
            this.lblListFrom = new System.Windows.Forms.Label();
            this.pnlTop.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tpJobCard.SuspendLayout();
            this.gbTotals.SuspendLayout();
            this.tcDetails.SuspendLayout();
            this.tpParts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParts)).BeginInit();
            this.tpServices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).BeginInit();
            this.gbDevice.SuspendLayout();
            this.gbJob.SuspendLayout();
            this.tpJobList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvJobList)).BeginInit();
            this.pnlListFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.SteelBlue;
            this.pnlTop.Controls.Add(this.btnClose);
            this.pnlTop.Controls.Add(this.btnBillJob);
            this.pnlTop.Controls.Add(this.btnPrint);
            this.pnlTop.Controls.Add(this.btnSave);
            this.pnlTop.Controls.Add(this.btnNew);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1064, 48);
            this.pnlTop.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 11);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(206, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Mobile Repair Job Card";
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.BackColor = System.Drawing.Color.White;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNew.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnNew.Location = new System.Drawing.Point(520, 9);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(95, 30);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "New (F2)";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(623, 9);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save (F3)";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.BackColor = System.Drawing.Color.White;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnPrint.Location = new System.Drawing.Point(731, 9);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(115, 30);
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "Print Slip (F5)";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnBillJob
            // 
            this.btnBillJob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBillJob.BackColor = System.Drawing.Color.Goldenrod;
            this.btnBillJob.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBillJob.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnBillJob.ForeColor = System.Drawing.Color.White;
            this.btnBillJob.Location = new System.Drawing.Point(854, 9);
            this.btnBillJob.Name = "btnBillJob";
            this.btnBillJob.Size = new System.Drawing.Size(115, 30);
            this.btnBillJob.TabIndex = 4;
            this.btnBillJob.Text = "Bill to Invoice";
            this.btnBillJob.UseVisualStyleBackColor = false;
            this.btnBillJob.Click += new System.EventHandler(this.btnBillJob_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.IndianRed;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(977, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 30);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpJobCard);
            this.tcMain.Controls.Add(this.tpJobList);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.tcMain.Location = new System.Drawing.Point(0, 48);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(1064, 633);
            this.tcMain.TabIndex = 1;
            // 
            // tpJobCard
            // 
            this.tpJobCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.tpJobCard.Controls.Add(this.gbTotals);
            this.tpJobCard.Controls.Add(this.tcDetails);
            this.tpJobCard.Controls.Add(this.gbDevice);
            this.tpJobCard.Controls.Add(this.gbJob);
            this.tpJobCard.Location = new System.Drawing.Point(4, 26);
            this.tpJobCard.Name = "tpJobCard";
            this.tpJobCard.Padding = new System.Windows.Forms.Padding(8);
            this.tpJobCard.Size = new System.Drawing.Size(1056, 603);
            this.tpJobCard.TabIndex = 0;
            this.tpJobCard.Text = "Job Entry";
            // 
            // gbTotals
            // 
            this.gbTotals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTotals.Controls.Add(this.lblSaleVoucher);
            this.gbTotals.Controls.Add(this.txtBalanceDue);
            this.gbTotals.Controls.Add(this.lblBalanceDue);
            this.gbTotals.Controls.Add(this.cmbAdvanceAccount);
            this.gbTotals.Controls.Add(this.lblAdvanceAccount);
            this.gbTotals.Controls.Add(this.txtAdvanceReceived);
            this.gbTotals.Controls.Add(this.lblAdvance);
            this.gbTotals.Controls.Add(this.txtNetTotal);
            this.gbTotals.Controls.Add(this.lblNetTotal);
            this.gbTotals.Controls.Add(this.txtServicesTotal);
            this.gbTotals.Controls.Add(this.lblServicesTotal);
            this.gbTotals.Controls.Add(this.txtPartsTotal);
            this.gbTotals.Controls.Add(this.lblPartsTotal);
            this.gbTotals.Controls.Add(this.txtEstimatedCost);
            this.gbTotals.Controls.Add(this.lblEstimatedCost);
            this.gbTotals.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.gbTotals.Location = new System.Drawing.Point(778, 303);
            this.gbTotals.Name = "gbTotals";
            this.gbTotals.Size = new System.Drawing.Size(269, 292);
            this.gbTotals.TabIndex = 3;
            this.gbTotals.TabStop = false;
            this.gbTotals.Text = "Summary & Charges";
            // 
            // lblSaleVoucher
            // 
            this.lblSaleVoucher.AutoSize = true;
            this.lblSaleVoucher.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSaleVoucher.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblSaleVoucher.Location = new System.Drawing.Point(12, 263);
            this.lblSaleVoucher.Name = "lblSaleVoucher";
            this.lblSaleVoucher.Size = new System.Drawing.Size(0, 15);
            this.lblSaleVoucher.TabIndex = 14;
            // 
            // txtBalanceDue
            // 
            this.txtBalanceDue.BackColor = System.Drawing.Color.MistyRose;
            this.txtBalanceDue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtBalanceDue.ForeColor = System.Drawing.Color.DarkRed;
            this.txtBalanceDue.Location = new System.Drawing.Point(122, 230);
            this.txtBalanceDue.Name = "txtBalanceDue";
            this.txtBalanceDue.ReadOnly = true;
            this.txtBalanceDue.Size = new System.Drawing.Size(135, 25);
            this.txtBalanceDue.TabIndex = 13;
            this.txtBalanceDue.Text = "0.00";
            this.txtBalanceDue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblBalanceDue
            // 
            this.lblBalanceDue.AutoSize = true;
            this.lblBalanceDue.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblBalanceDue.ForeColor = System.Drawing.Color.DarkRed;
            this.lblBalanceDue.Location = new System.Drawing.Point(12, 234);
            this.lblBalanceDue.Name = "lblBalanceDue";
            this.lblBalanceDue.Size = new System.Drawing.Size(89, 17);
            this.lblBalanceDue.TabIndex = 12;
            this.lblBalanceDue.Text = "Balance Due:";
            // 
            // cmbAdvanceAccount
            // 
            this.cmbAdvanceAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAdvanceAccount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbAdvanceAccount.FormattingEnabled = true;
            this.cmbAdvanceAccount.Location = new System.Drawing.Point(122, 198);
            this.cmbAdvanceAccount.Name = "cmbAdvanceAccount";
            this.cmbAdvanceAccount.Size = new System.Drawing.Size(135, 23);
            this.cmbAdvanceAccount.TabIndex = 11;
            // 
            // lblAdvanceAccount
            // 
            this.lblAdvanceAccount.AutoSize = true;
            this.lblAdvanceAccount.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblAdvanceAccount.Location = new System.Drawing.Point(12, 201);
            this.lblAdvanceAccount.Name = "lblAdvanceAccount";
            this.lblAdvanceAccount.Size = new System.Drawing.Size(78, 15);
            this.lblAdvanceAccount.TabIndex = 10;
            this.lblAdvanceAccount.Text = "Adv. Account:";
            // 
            // txtAdvanceReceived
            // 
            this.txtAdvanceReceived.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtAdvanceReceived.Location = new System.Drawing.Point(122, 166);
            this.txtAdvanceReceived.Name = "txtAdvanceReceived";
            this.txtAdvanceReceived.Size = new System.Drawing.Size(135, 24);
            this.txtAdvanceReceived.TabIndex = 9;
            this.txtAdvanceReceived.Text = "0.00";
            this.txtAdvanceReceived.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAdvanceReceived.TextChanged += new System.EventHandler(this.CalculateTotals);
            // 
            // lblAdvance
            // 
            this.lblAdvance.AutoSize = true;
            this.lblAdvance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAdvance.Location = new System.Drawing.Point(12, 169);
            this.lblAdvance.Name = "lblAdvance";
            this.lblAdvance.Size = new System.Drawing.Size(84, 15);
            this.lblAdvance.TabIndex = 8;
            this.lblAdvance.Text = "Adv. Received:";
            // 
            // txtNetTotal
            // 
            this.txtNetTotal.BackColor = System.Drawing.Color.Honeydew;
            this.txtNetTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtNetTotal.ForeColor = System.Drawing.Color.DarkGreen;
            this.txtNetTotal.Location = new System.Drawing.Point(122, 131);
            this.txtNetTotal.Name = "txtNetTotal";
            this.txtNetTotal.ReadOnly = true;
            this.txtNetTotal.Size = new System.Drawing.Size(135, 25);
            this.txtNetTotal.TabIndex = 7;
            this.txtNetTotal.Text = "0.00";
            this.txtNetTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblNetTotal
            // 
            this.lblNetTotal.AutoSize = true;
            this.lblNetTotal.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblNetTotal.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblNetTotal.Location = new System.Drawing.Point(12, 135);
            this.lblNetTotal.Name = "lblNetTotal";
            this.lblNetTotal.Size = new System.Drawing.Size(69, 17);
            this.lblNetTotal.TabIndex = 6;
            this.lblNetTotal.Text = "Net Total:";
            // 
            // txtServicesTotal
            // 
            this.txtServicesTotal.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtServicesTotal.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtServicesTotal.Location = new System.Drawing.Point(122, 98);
            this.txtServicesTotal.Name = "txtServicesTotal";
            this.txtServicesTotal.ReadOnly = true;
            this.txtServicesTotal.Size = new System.Drawing.Size(135, 23);
            this.txtServicesTotal.TabIndex = 5;
            this.txtServicesTotal.Text = "0.00";
            this.txtServicesTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblServicesTotal
            // 
            this.lblServicesTotal.AutoSize = true;
            this.lblServicesTotal.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblServicesTotal.Location = new System.Drawing.Point(12, 102);
            this.lblServicesTotal.Name = "lblServicesTotal";
            this.lblServicesTotal.Size = new System.Drawing.Size(81, 15);
            this.lblServicesTotal.TabIndex = 4;
            this.lblServicesTotal.Text = "Labour/Serv.:";
            // 
            // txtPartsTotal
            // 
            this.txtPartsTotal.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtPartsTotal.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPartsTotal.Location = new System.Drawing.Point(122, 65);
            this.txtPartsTotal.Name = "txtPartsTotal";
            this.txtPartsTotal.ReadOnly = true;
            this.txtPartsTotal.Size = new System.Drawing.Size(135, 23);
            this.txtPartsTotal.TabIndex = 3;
            this.txtPartsTotal.Text = "0.00";
            this.txtPartsTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblPartsTotal
            // 
            this.lblPartsTotal.AutoSize = true;
            this.lblPartsTotal.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblPartsTotal.Location = new System.Drawing.Point(12, 69);
            this.lblPartsTotal.Name = "lblPartsTotal";
            this.lblPartsTotal.Size = new System.Drawing.Size(65, 15);
            this.lblPartsTotal.TabIndex = 2;
            this.lblPartsTotal.Text = "Parts Total:";
            // 
            // txtEstimatedCost
            // 
            this.txtEstimatedCost.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtEstimatedCost.Location = new System.Drawing.Point(122, 30);
            this.txtEstimatedCost.Name = "txtEstimatedCost";
            this.txtEstimatedCost.Size = new System.Drawing.Size(135, 24);
            this.txtEstimatedCost.TabIndex = 1;
            this.txtEstimatedCost.Text = "0.00";
            this.txtEstimatedCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblEstimatedCost
            // 
            this.lblEstimatedCost.AutoSize = true;
            this.lblEstimatedCost.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblEstimatedCost.Location = new System.Drawing.Point(12, 34);
            this.lblEstimatedCost.Name = "lblEstimatedCost";
            this.lblEstimatedCost.Size = new System.Drawing.Size(89, 15);
            this.lblEstimatedCost.TabIndex = 0;
            this.lblEstimatedCost.Text = "Estimated Cost:";
            // 
            // tcDetails
            // 
            this.tcDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcDetails.Controls.Add(this.tpParts);
            this.tcDetails.Controls.Add(this.tpServices);
            this.tcDetails.Location = new System.Drawing.Point(9, 303);
            this.tcDetails.Name = "tcDetails";
            this.tcDetails.SelectedIndex = 0;
            this.tcDetails.Size = new System.Drawing.Size(763, 292);
            this.tcDetails.TabIndex = 2;
            // 
            // tpParts
            // 
            this.tpParts.Controls.Add(this.dgvParts);
            this.tpParts.Location = new System.Drawing.Point(4, 26);
            this.tpParts.Name = "tpParts";
            this.tpParts.Padding = new System.Windows.Forms.Padding(3);
            this.tpParts.Size = new System.Drawing.Size(755, 262);
            this.tpParts.TabIndex = 0;
            this.tpParts.Text = "Parts Used / Replaced";
            this.tpParts.UseVisualStyleBackColor = true;
            // 
            // dgvParts
            // 
            this.dgvParts.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvParts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvParts.ColumnHeadersHeight = 28;
            this.dgvParts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnPartItemId,
            this.clnPartItemTitle,
            this.clnPartQty,
            this.clnPartPrice,
            this.clnPartTotal});
            this.dgvParts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvParts.EnableHeadersVisualStyles = false;
            this.dgvParts.Location = new System.Drawing.Point(3, 3);
            this.dgvParts.Name = "dgvParts";
            this.dgvParts.RowTemplate.Height = 25;
            this.dgvParts.Size = new System.Drawing.Size(749, 256);
            this.dgvParts.TabIndex = 0;
            this.dgvParts.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvParts_CellEndEdit);
            this.dgvParts.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvParts_RowsRemoved);
            // 
            // clnPartItemId
            // 
            this.clnPartItemId.HeaderText = "Item Code";
            this.clnPartItemId.Name = "clnPartItemId";
            this.clnPartItemId.Width = 110;
            // 
            // clnPartItemTitle
            // 
            this.clnPartItemTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clnPartItemTitle.HeaderText = "Item Description";
            this.clnPartItemTitle.Name = "clnPartItemTitle";
            // 
            // clnPartQty
            // 
            this.clnPartQty.HeaderText = "Qty";
            this.clnPartQty.Name = "clnPartQty";
            this.clnPartQty.Width = 80;
            // 
            // clnPartPrice
            // 
            this.clnPartPrice.HeaderText = "Unit Price";
            this.clnPartPrice.Name = "clnPartPrice";
            this.clnPartPrice.Width = 100;
            // 
            // clnPartTotal
            // 
            this.clnPartTotal.HeaderText = "Total";
            this.clnPartTotal.Name = "clnPartTotal";
            this.clnPartTotal.ReadOnly = true;
            this.clnPartTotal.Width = 110;
            // 
            // tpServices
            // 
            this.tpServices.Controls.Add(this.dgvServices);
            this.tpServices.Location = new System.Drawing.Point(4, 26);
            this.tpServices.Name = "tpServices";
            this.tpServices.Padding = new System.Windows.Forms.Padding(3);
            this.tpServices.Size = new System.Drawing.Size(755, 262);
            this.tpServices.TabIndex = 1;
            this.tpServices.Text = "Services & Labour Charges";
            this.tpServices.UseVisualStyleBackColor = true;
            // 
            // dgvServices
            // 
            this.dgvServices.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvServices.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvServices.ColumnHeadersHeight = 28;
            this.dgvServices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnServiceDesc,
            this.clnServiceAmount});
            this.dgvServices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvServices.EnableHeadersVisualStyles = false;
            this.dgvServices.Location = new System.Drawing.Point(3, 3);
            this.dgvServices.Name = "dgvServices";
            this.dgvServices.RowTemplate.Height = 25;
            this.dgvServices.Size = new System.Drawing.Size(749, 256);
            this.dgvServices.TabIndex = 0;
            this.dgvServices.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvServices_CellEndEdit);
            this.dgvServices.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvServices_RowsRemoved);
            // 
            // clnServiceDesc
            // 
            this.clnServiceDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clnServiceDesc.HeaderText = "Service Description / Labour";
            this.clnServiceDesc.Name = "clnServiceDesc";
            // 
            // clnServiceAmount
            // 
            this.clnServiceAmount.HeaderText = "Amount";
            this.clnServiceAmount.Name = "clnServiceAmount";
            this.clnServiceAmount.Width = 140;
            // 
            // gbDevice
            // 
            this.gbDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbDevice.Controls.Add(this.txtTechnicianNotes);
            this.gbDevice.Controls.Add(this.lblTechnicianNotes);
            this.gbDevice.Controls.Add(this.cmbTechnician);
            this.gbDevice.Controls.Add(this.lblTechnician);
            this.gbDevice.Controls.Add(this.txtAccessories);
            this.gbDevice.Controls.Add(this.lblAccessories);
            this.gbDevice.Controls.Add(this.txtPhysicalCondition);
            this.gbDevice.Controls.Add(this.lblCondition);
            this.gbDevice.Controls.Add(this.txtProblem);
            this.gbDevice.Controls.Add(this.lblProblem);
            this.gbDevice.Controls.Add(this.txtPasscode);
            this.gbDevice.Controls.Add(this.lblPasscode);
            this.gbDevice.Controls.Add(this.txtImei);
            this.gbDevice.Controls.Add(this.lblImei);
            this.gbDevice.Controls.Add(this.txtColor);
            this.gbDevice.Controls.Add(this.lblColor);
            this.gbDevice.Controls.Add(this.txtModel);
            this.gbDevice.Controls.Add(this.lblModel);
            this.gbDevice.Controls.Add(this.cmbBrand);
            this.gbDevice.Controls.Add(this.lblBrand);
            this.gbDevice.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.gbDevice.Location = new System.Drawing.Point(9, 107);
            this.gbDevice.Name = "gbDevice";
            this.gbDevice.Size = new System.Drawing.Size(1038, 190);
            this.gbDevice.TabIndex = 1;
            this.gbDevice.TabStop = false;
            this.gbDevice.Text = "Device Details & Problem";
            // 
            // txtTechnicianNotes
            // 
            this.txtTechnicianNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTechnicianNotes.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtTechnicianNotes.Location = new System.Drawing.Point(623, 141);
            this.txtTechnicianNotes.Name = "txtTechnicianNotes";
            this.txtTechnicianNotes.Size = new System.Drawing.Size(401, 38);
            this.txtTechnicianNotes.TabIndex = 19;
            // 
            // lblTechnicianNotes
            // 
            this.lblTechnicianNotes.AutoSize = true;
            this.lblTechnicianNotes.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblTechnicianNotes.Location = new System.Drawing.Point(527, 144);
            this.lblTechnicianNotes.Name = "lblTechnicianNotes";
            this.lblTechnicianNotes.Size = new System.Drawing.Size(89, 15);
            this.lblTechnicianNotes.TabIndex = 18;
            this.lblTechnicianNotes.Text = "Tech. Remarks:";
            // 
            // cmbTechnician
            // 
            this.cmbTechnician.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTechnician.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbTechnician.FormattingEnabled = true;
            this.cmbTechnician.Location = new System.Drawing.Point(344, 141);
            this.cmbTechnician.Name = "cmbTechnician";
            this.cmbTechnician.Size = new System.Drawing.Size(165, 23);
            this.cmbTechnician.TabIndex = 17;
            // 
            // lblTechnician
            // 
            this.lblTechnician.AutoSize = true;
            this.lblTechnician.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblTechnician.Location = new System.Drawing.Point(269, 144);
            this.lblTechnician.Name = "lblTechnician";
            this.lblTechnician.Size = new System.Drawing.Size(66, 15);
            this.lblTechnician.TabIndex = 16;
            this.lblTechnician.Text = "Technician:";
            // 
            // txtAccessories
            // 
            this.txtAccessories.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtAccessories.Location = new System.Drawing.Point(100, 141);
            this.txtAccessories.Name = "txtAccessories";
            this.txtAccessories.Size = new System.Drawing.Size(155, 23);
            this.txtAccessories.TabIndex = 15;
            // 
            // lblAccessories
            // 
            this.lblAccessories.AutoSize = true;
            this.lblAccessories.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblAccessories.Location = new System.Drawing.Point(14, 144);
            this.lblAccessories.Name = "lblAccessories";
            this.lblAccessories.Size = new System.Drawing.Size(72, 15);
            this.lblAccessories.TabIndex = 14;
            this.lblAccessories.Text = "Accessories:";
            // 
            // txtPhysicalCondition
            // 
            this.txtPhysicalCondition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPhysicalCondition.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPhysicalCondition.Location = new System.Drawing.Point(623, 98);
            this.txtPhysicalCondition.Name = "txtPhysicalCondition";
            this.txtPhysicalCondition.Size = new System.Drawing.Size(401, 23);
            this.txtPhysicalCondition.TabIndex = 13;
            // 
            // lblCondition
            // 
            this.lblCondition.AutoSize = true;
            this.lblCondition.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblCondition.Location = new System.Drawing.Point(527, 101);
            this.lblCondition.Name = "lblCondition";
            this.lblCondition.Size = new System.Drawing.Size(91, 15);
            this.lblCondition.TabIndex = 12;
            this.lblCondition.Text = "Phys. Condition:";
            // 
            // txtProblem
            // 
            this.txtProblem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtProblem.Location = new System.Drawing.Point(100, 98);
            this.txtProblem.Name = "txtProblem";
            this.txtProblem.Size = new System.Drawing.Size(409, 23);
            this.txtProblem.TabIndex = 11;
            // 
            // lblProblem
            // 
            this.lblProblem.AutoSize = true;
            this.lblProblem.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblProblem.Location = new System.Drawing.Point(14, 101);
            this.lblProblem.Name = "lblProblem";
            this.lblProblem.Size = new System.Drawing.Size(54, 15);
            this.lblProblem.TabIndex = 10;
            this.lblProblem.Text = "Problem:";
            // 
            // txtPasscode
            // 
            this.txtPasscode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPasscode.Location = new System.Drawing.Point(865, 59);
            this.txtPasscode.Name = "txtPasscode";
            this.txtPasscode.Size = new System.Drawing.Size(159, 23);
            this.txtPasscode.TabIndex = 9;
            // 
            // lblPasscode
            // 
            this.lblPasscode.AutoSize = true;
            this.lblPasscode.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblPasscode.Location = new System.Drawing.Point(800, 62);
            this.lblPasscode.Name = "lblPasscode";
            this.lblPasscode.Size = new System.Drawing.Size(59, 15);
            this.lblPasscode.TabIndex = 8;
            this.lblPasscode.Text = "Passcode:";
            // 
            // txtImei
            // 
            this.txtImei.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtImei.Location = new System.Drawing.Point(623, 59);
            this.txtImei.Name = "txtImei";
            this.txtImei.Size = new System.Drawing.Size(165, 23);
            this.txtImei.TabIndex = 7;
            // 
            // lblImei
            // 
            this.lblImei.AutoSize = true;
            this.lblImei.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblImei.Location = new System.Drawing.Point(544, 62);
            this.lblImei.Name = "lblImei";
            this.lblImei.Size = new System.Drawing.Size(73, 15);
            this.lblImei.TabIndex = 6;
            this.lblImei.Text = "IMEI/Serial:";
            // 
            // txtColor
            // 
            this.txtColor.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtColor.Location = new System.Drawing.Point(433, 59);
            this.txtColor.Name = "txtColor";
            this.txtColor.Size = new System.Drawing.Size(100, 23);
            this.txtColor.TabIndex = 5;
            // 
            // lblColor
            // 
            this.lblColor.AutoSize = true;
            this.lblColor.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblColor.Location = new System.Drawing.Point(389, 62);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(39, 15);
            this.lblColor.TabIndex = 4;
            this.lblColor.Text = "Color:";
            // 
            // txtModel
            // 
            this.txtModel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtModel.Location = new System.Drawing.Point(232, 59);
            this.txtModel.Name = "txtModel";
            this.txtModel.Size = new System.Drawing.Size(145, 23);
            this.txtModel.TabIndex = 3;
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblModel.Location = new System.Drawing.Point(184, 62);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(44, 15);
            this.lblModel.TabIndex = 2;
            this.lblModel.Text = "Model:";
            // 
            // cmbBrand
            // 
            this.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBrand.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbBrand.FormattingEnabled = true;
            this.cmbBrand.Location = new System.Drawing.Point(58, 59);
            this.cmbBrand.Name = "cmbBrand";
            this.cmbBrand.Size = new System.Drawing.Size(115, 23);
            this.cmbBrand.TabIndex = 1;
            // 
            // lblBrand
            // 
            this.lblBrand.AutoSize = true;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblBrand.Location = new System.Drawing.Point(14, 62);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(41, 15);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "Brand:";
            // 
            // gbJob
            // 
            this.gbJob.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbJob.Controls.Add(this.txtCustomerPhone);
            this.gbJob.Controls.Add(this.lblPhone);
            this.gbJob.Controls.Add(this.cmbCustomer);
            this.gbJob.Controls.Add(this.lblCustomer);
            this.gbJob.Controls.Add(this.cmbStatus);
            this.gbJob.Controls.Add(this.lblStatus);
            this.gbJob.Controls.Add(this.dtpJobDate);
            this.gbJob.Controls.Add(this.lblDate);
            this.gbJob.Controls.Add(this.txtJobNo);
            this.gbJob.Controls.Add(this.lblJobNo);
            this.gbJob.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.gbJob.Location = new System.Drawing.Point(9, 8);
            this.gbJob.Name = "gbJob";
            this.gbJob.Size = new System.Drawing.Size(1038, 93);
            this.gbJob.TabIndex = 0;
            this.gbJob.TabStop = false;
            this.gbJob.Text = "Job Info";
            // 
            // txtCustomerPhone
            // 
            this.txtCustomerPhone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomerPhone.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCustomerPhone.Location = new System.Drawing.Point(865, 56);
            this.txtCustomerPhone.Name = "txtCustomerPhone";
            this.txtCustomerPhone.Size = new System.Drawing.Size(159, 23);
            this.txtCustomerPhone.TabIndex = 9;
            // 
            // lblPhone
            // 
            this.lblPhone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPhone.AutoSize = true;
            this.lblPhone.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblPhone.Location = new System.Drawing.Point(798, 59);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(61, 15);
            this.lblPhone.TabIndex = 8;
            this.lblPhone.Text = "Phone No:";
            // 
            // cmbCustomer
            // 
            this.cmbCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCustomer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbCustomer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbCustomer.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(74, 56);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(714, 23);
            this.cmbCustomer.TabIndex = 7;
            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblCustomer.Location = new System.Drawing.Point(11, 59);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(62, 15);
            this.lblCustomer.TabIndex = 6;
            this.lblCustomer.Text = "Customer:";
            // 
            // cmbStatus
            // 
            this.cmbStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Items.AddRange(new object[] {
            "Received",
            "In Progress",
            "Waiting for Parts",
            "Repaired",
            "Delivered",
            "Cancelled"});
            this.cmbStatus.Location = new System.Drawing.Point(865, 23);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(159, 23);
            this.cmbStatus.TabIndex = 5;
            this.cmbStatus.SelectedIndexChanged += new System.EventHandler(this.cmbStatus_SelectedIndexChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblStatus.Location = new System.Drawing.Point(816, 26);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(43, 15);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Status:";
            // 
            // dtpJobDate
            // 
            this.dtpJobDate.CustomFormat = "dd/MM/yyyy";
            this.dtpJobDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpJobDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpJobDate.Location = new System.Drawing.Point(269, 23);
            this.dtpJobDate.Name = "dtpJobDate";
            this.dtpJobDate.Size = new System.Drawing.Size(120, 23);
            this.dtpJobDate.TabIndex = 3;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDate.Location = new System.Drawing.Point(228, 26);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(35, 15);
            this.lblDate.TabIndex = 2;
            this.lblDate.Text = "Date:";
            // 
            // txtJobNo
            // 
            this.txtJobNo.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtJobNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtJobNo.Location = new System.Drawing.Point(74, 23);
            this.txtJobNo.Name = "txtJobNo";
            this.txtJobNo.ReadOnly = true;
            this.txtJobNo.Size = new System.Drawing.Size(135, 23);
            this.txtJobNo.TabIndex = 1;
            // 
            // lblJobNo
            // 
            this.lblJobNo.AutoSize = true;
            this.lblJobNo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblJobNo.Location = new System.Drawing.Point(14, 26);
            this.lblJobNo.Name = "lblJobNo";
            this.lblJobNo.Size = new System.Drawing.Size(47, 15);
            this.lblJobNo.TabIndex = 0;
            this.lblJobNo.Text = "Job No:";
            // 
            // tpJobList
            // 
            this.tpJobList.Controls.Add(this.dgvJobList);
            this.tpJobList.Controls.Add(this.pnlListFilter);
            this.tpJobList.Location = new System.Drawing.Point(4, 26);
            this.tpJobList.Name = "tpJobList";
            this.tpJobList.Padding = new System.Windows.Forms.Padding(3);
            this.tpJobList.Size = new System.Drawing.Size(1056, 603);
            this.tpJobList.TabIndex = 1;
            this.tpJobList.Text = "Job Search / History";
            this.tpJobList.UseVisualStyleBackColor = true;
            // 
            // dgvJobList
            // 
            this.dgvJobList.AllowUserToAddRows = false;
            this.dgvJobList.AllowUserToDeleteRows = false;
            this.dgvJobList.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvJobList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvJobList.ColumnHeadersHeight = 28;
            this.dgvJobList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnListJobNo,
            this.clnListDate,
            this.clnListStatus,
            this.clnListCustomer,
            this.clnListPhone,
            this.clnListDevice,
            this.clnListImei,
            this.clnListNetTotal,
            this.clnListAdvance,
            this.clnListBalance});
            this.dgvJobList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvJobList.EnableHeadersVisualStyles = false;
            this.dgvJobList.Location = new System.Drawing.Point(3, 49);
            this.dgvJobList.MultiSelect = false;
            this.dgvJobList.Name = "dgvJobList";
            this.dgvJobList.ReadOnly = true;
            this.dgvJobList.RowHeadersVisible = false;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvJobList.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvJobList.RowTemplate.Height = 25;
            this.dgvJobList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvJobList.Size = new System.Drawing.Size(1050, 551);
            this.dgvJobList.TabIndex = 1;
            this.dgvJobList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvJobList_CellDoubleClick);
            // 
            // clnListJobNo
            // 
            this.clnListJobNo.HeaderText = "Job No";
            this.clnListJobNo.Name = "clnListJobNo";
            this.clnListJobNo.ReadOnly = true;
            this.clnListJobNo.Width = 90;
            // 
            // clnListDate
            // 
            this.clnListDate.HeaderText = "Date";
            this.clnListDate.Name = "clnListDate";
            this.clnListDate.ReadOnly = true;
            this.clnListDate.Width = 90;
            // 
            // clnListStatus
            // 
            this.clnListStatus.HeaderText = "Status";
            this.clnListStatus.Name = "clnListStatus";
            this.clnListStatus.ReadOnly = true;
            this.clnListStatus.Width = 110;
            // 
            // clnListCustomer
            // 
            this.clnListCustomer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clnListCustomer.HeaderText = "Customer";
            this.clnListCustomer.Name = "clnListCustomer";
            this.clnListCustomer.ReadOnly = true;
            // 
            // clnListPhone
            // 
            this.clnListPhone.HeaderText = "Phone";
            this.clnListPhone.Name = "clnListPhone";
            this.clnListPhone.ReadOnly = true;
            this.clnListPhone.Width = 110;
            // 
            // clnListDevice
            // 
            this.clnListDevice.HeaderText = "Device";
            this.clnListDevice.Name = "clnListDevice";
            this.clnListDevice.ReadOnly = true;
            this.clnListDevice.Width = 130;
            // 
            // clnListImei
            // 
            this.clnListImei.HeaderText = "IMEI";
            this.clnListImei.Name = "clnListImei";
            this.clnListImei.ReadOnly = true;
            this.clnListImei.Width = 120;
            // 
            // clnListNetTotal
            // 
            this.clnListNetTotal.HeaderText = "Net Total";
            this.clnListNetTotal.Name = "clnListNetTotal";
            this.clnListNetTotal.ReadOnly = true;
            this.clnListNetTotal.Width = 90;
            // 
            // clnListAdvance
            // 
            this.clnListAdvance.HeaderText = "Advance";
            this.clnListAdvance.Name = "clnListAdvance";
            this.clnListAdvance.ReadOnly = true;
            this.clnListAdvance.Width = 85;
            // 
            // clnListBalance
            // 
            this.clnListBalance.HeaderText = "Balance";
            this.clnListBalance.Name = "clnListBalance";
            this.clnListBalance.ReadOnly = true;
            this.clnListBalance.Width = 90;
            // 
            // pnlListFilter
            // 
            this.pnlListFilter.BackColor = System.Drawing.Color.AliceBlue;
            this.pnlListFilter.Controls.Add(this.btnListRefresh);
            this.pnlListFilter.Controls.Add(this.txtListSearch);
            this.pnlListFilter.Controls.Add(this.lblListSearch);
            this.pnlListFilter.Controls.Add(this.cmbListStatus);
            this.pnlListFilter.Controls.Add(this.lblListStatus);
            this.pnlListFilter.Controls.Add(this.dtpListTo);
            this.pnlListFilter.Controls.Add(this.lblListTo);
            this.pnlListFilter.Controls.Add(this.dtpListFrom);
            this.pnlListFilter.Controls.Add(this.lblListFrom);
            this.pnlListFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlListFilter.Location = new System.Drawing.Point(3, 3);
            this.pnlListFilter.Name = "pnlListFilter";
            this.pnlListFilter.Size = new System.Drawing.Size(1050, 46);
            this.pnlListFilter.TabIndex = 0;
            // 
            // btnListRefresh
            // 
            this.btnListRefresh.BackColor = System.Drawing.Color.SteelBlue;
            this.btnListRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnListRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnListRefresh.ForeColor = System.Drawing.Color.White;
            this.btnListRefresh.Location = new System.Drawing.Point(825, 9);
            this.btnListRefresh.Name = "btnListRefresh";
            this.btnListRefresh.Size = new System.Drawing.Size(80, 27);
            this.btnListRefresh.TabIndex = 8;
            this.btnListRefresh.Text = "Filter";
            this.btnListRefresh.UseVisualStyleBackColor = false;
            this.btnListRefresh.Click += new System.EventHandler(this.btnListRefresh_Click);
            // 
            // txtListSearch
            // 
            this.txtListSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtListSearch.Location = new System.Drawing.Point(620, 11);
            this.txtListSearch.Name = "txtListSearch";
            this.txtListSearch.Size = new System.Drawing.Size(190, 23);
            this.txtListSearch.TabIndex = 7;
            this.txtListSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtListSearch_KeyDown);
            // 
            // lblListSearch
            // 
            this.lblListSearch.AutoSize = true;
            this.lblListSearch.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblListSearch.Location = new System.Drawing.Point(571, 15);
            this.lblListSearch.Name = "lblListSearch";
            this.lblListSearch.Size = new System.Drawing.Size(45, 15);
            this.lblListSearch.TabIndex = 6;
            this.lblListSearch.Text = "Search:";
            // 
            // cmbListStatus
            // 
            this.cmbListStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbListStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbListStatus.FormattingEnabled = true;
            this.cmbListStatus.Items.AddRange(new object[] {
            "All",
            "Received",
            "In Progress",
            "Waiting for Parts",
            "Repaired",
            "Delivered",
            "Cancelled"});
            this.cmbListStatus.Location = new System.Drawing.Point(422, 11);
            this.cmbListStatus.Name = "cmbListStatus";
            this.cmbListStatus.Size = new System.Drawing.Size(130, 23);
            this.cmbListStatus.TabIndex = 5;
            // 
            // lblListStatus
            // 
            this.lblListStatus.AutoSize = true;
            this.lblListStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblListStatus.Location = new System.Drawing.Point(375, 15);
            this.lblListStatus.Name = "lblListStatus";
            this.lblListStatus.Size = new System.Drawing.Size(42, 15);
            this.lblListStatus.TabIndex = 4;
            this.lblListStatus.Text = "Status:";
            // 
            // dtpListTo
            // 
            this.dtpListTo.CustomFormat = "dd/MM/yyyy";
            this.dtpListTo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpListTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpListTo.Location = new System.Drawing.Point(238, 11);
            this.dtpListTo.Name = "dtpListTo";
            this.dtpListTo.Size = new System.Drawing.Size(115, 23);
            this.dtpListTo.TabIndex = 3;
            // 
            // lblListTo
            // 
            this.lblListTo.AutoSize = true;
            this.lblListTo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblListTo.Location = new System.Drawing.Point(209, 15);
            this.lblListTo.Name = "lblListTo";
            this.lblListTo.Size = new System.Drawing.Size(23, 15);
            this.lblListTo.TabIndex = 2;
            this.lblListTo.Text = "To:";
            // 
            // dtpListFrom
            // 
            this.dtpListFrom.CustomFormat = "dd/MM/yyyy";
            this.dtpListFrom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpListFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpListFrom.Location = new System.Drawing.Point(78, 11);
            this.dtpListFrom.Name = "dtpListFrom";
            this.dtpListFrom.Size = new System.Drawing.Size(115, 23);
            this.dtpListFrom.TabIndex = 1;
            // 
            // lblListFrom
            // 
            this.lblListFrom.AutoSize = true;
            this.lblListFrom.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblListFrom.Location = new System.Drawing.Point(34, 15);
            this.lblListFrom.Name = "lblListFrom";
            this.lblListFrom.Size = new System.Drawing.Size(38, 15);
            this.lblListFrom.TabIndex = 0;
            this.lblListFrom.Text = "From:";
            // 
            // frmRepairJob
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 681);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.pnlTop);
            this.KeyPreview = true;
            this.Name = "frmRepairJob";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mobile Repair Job Card";
            this.Load += new System.EventHandler(this.frmRepairJob_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmRepairJob_KeyDown);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.tcMain.ResumeLayout(false);
            this.tpJobCard.ResumeLayout(false);
            this.gbTotals.ResumeLayout(false);
            this.gbTotals.PerformLayout();
            this.tcDetails.ResumeLayout(false);
            this.tpParts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParts)).EndInit();
            this.tpServices.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).EndInit();
            this.gbDevice.ResumeLayout(false);
            this.gbDevice.PerformLayout();
            this.gbJob.ResumeLayout(false);
            this.gbJob.PerformLayout();
            this.tpJobList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvJobList)).EndInit();
            this.pnlListFilter.ResumeLayout(false);
            this.pnlListFilter.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnBillJob;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpJobCard;
        private System.Windows.Forms.GroupBox gbJob;
        private System.Windows.Forms.TextBox txtCustomerPhone;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.ComboBox cmbCustomer;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.DateTimePicker dtpJobDate;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.TextBox txtJobNo;
        private System.Windows.Forms.Label lblJobNo;
        private System.Windows.Forms.GroupBox gbDevice;
        private System.Windows.Forms.TextBox txtTechnicianNotes;
        private System.Windows.Forms.Label lblTechnicianNotes;
        private System.Windows.Forms.ComboBox cmbTechnician;
        private System.Windows.Forms.Label lblTechnician;
        private System.Windows.Forms.TextBox txtAccessories;
        private System.Windows.Forms.Label lblAccessories;
        private System.Windows.Forms.TextBox txtPhysicalCondition;
        private System.Windows.Forms.Label lblCondition;
        private System.Windows.Forms.TextBox txtProblem;
        private System.Windows.Forms.Label lblProblem;
        private System.Windows.Forms.TextBox txtPasscode;
        private System.Windows.Forms.Label lblPasscode;
        private System.Windows.Forms.TextBox txtImei;
        private System.Windows.Forms.Label lblImei;
        private System.Windows.Forms.TextBox txtColor;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.ComboBox cmbBrand;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.TabControl tcDetails;
        private System.Windows.Forms.TabPage tpParts;
        private System.Windows.Forms.DataGridView dgvParts;
        private System.Windows.Forms.TabPage tpServices;
        private System.Windows.Forms.DataGridView dgvServices;
        private System.Windows.Forms.GroupBox gbTotals;
        private System.Windows.Forms.TextBox txtBalanceDue;
        private System.Windows.Forms.Label lblBalanceDue;
        private System.Windows.Forms.ComboBox cmbAdvanceAccount;
        private System.Windows.Forms.Label lblAdvanceAccount;
        private System.Windows.Forms.TextBox txtAdvanceReceived;
        private System.Windows.Forms.Label lblAdvance;
        private System.Windows.Forms.TextBox txtNetTotal;
        private System.Windows.Forms.Label lblNetTotal;
        private System.Windows.Forms.TextBox txtServicesTotal;
        private System.Windows.Forms.Label lblServicesTotal;
        private System.Windows.Forms.TextBox txtPartsTotal;
        private System.Windows.Forms.Label lblPartsTotal;
        private System.Windows.Forms.TextBox txtEstimatedCost;
        private System.Windows.Forms.Label lblEstimatedCost;
        private System.Windows.Forms.Label lblSaleVoucher;
        private System.Windows.Forms.TabPage tpJobList;
        private System.Windows.Forms.Panel pnlListFilter;
        private System.Windows.Forms.DateTimePicker dtpListFrom;
        private System.Windows.Forms.Label lblListFrom;
        private System.Windows.Forms.DateTimePicker dtpListTo;
        private System.Windows.Forms.Label lblListTo;
        private System.Windows.Forms.ComboBox cmbListStatus;
        private System.Windows.Forms.Label lblListStatus;
        private System.Windows.Forms.TextBox txtListSearch;
        private System.Windows.Forms.Label lblListSearch;
        private System.Windows.Forms.Button btnListRefresh;
        private System.Windows.Forms.DataGridView dgvJobList;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnPartItemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnPartItemTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnPartQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnPartPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnPartTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnServiceDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnServiceAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListJobNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListCustomer;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListPhone;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListDevice;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListImei;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListNetTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListAdvance;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListBalance;
    }
}
