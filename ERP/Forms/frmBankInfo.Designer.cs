namespace ERP
{
    partial class frmBankInfo
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tpList = new System.Windows.Forms.TabPage();
            this.dgvBankAccounts = new System.Windows.Forms.DataGridView();
            this.colAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBankName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAccountTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAccountNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBranchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOpeningBalance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCurrency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlListHeader = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnRefreshList = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.tpDetails = new System.Windows.Forms.TabPage();
            this.pnlDetailBanner = new System.Windows.Forms.Panel();
            this.lblBannerSubtitle = new System.Windows.Forms.Label();
            this.lblBannerTitle = new System.Windows.Forms.Label();
            this.lblBankName = new System.Windows.Forms.Label();
            this.txtBankName = new System.Windows.Forms.TextBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.lblAccountTitle = new System.Windows.Forms.Label();
            this.txtAccountTitle = new System.Windows.Forms.TextBox();
            this.lblAccountNumber = new System.Windows.Forms.Label();
            this.txtAccountNumber = new System.Windows.Forms.TextBox();
            this.lblCurrency = new System.Windows.Forms.Label();
            this.cmbCurrency = new System.Windows.Forms.ComboBox();
            this.lblIBAN = new System.Windows.Forms.Label();
            this.txtIBAN = new System.Windows.Forms.TextBox();
            this.lblBranchName = new System.Windows.Forms.Label();
            this.txtBranchName = new System.Windows.Forms.TextBox();
            this.lblBranchCode = new System.Windows.Forms.Label();
            this.txtBranchCode = new System.Windows.Forms.TextBox();
            this.lblSwiftCode = new System.Windows.Forms.Label();
            this.txtSwiftCode = new System.Windows.Forms.TextBox();
            this.lblOpeningBalance = new System.Windows.Forms.Label();
            this.txtOpeningBalance = new ERP.DecimalTextbox();
            this.lblOpeningBalanceHint = new System.Windows.Forms.Label();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.lblCreatedByTitle = new System.Windows.Forms.Label();
            this.lblCreatedBy = new System.Windows.Forms.Label();
            this.lblEditByTitle = new System.Windows.Forms.Label();
            this.lblEditBy = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnStatement = new System.Windows.Forms.Button();
            this.btnReconcile = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tpList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankAccounts)).BeginInit();
            this.pnlListHeader.SuspendLayout();
            this.tpDetails.SuspendLayout();
            this.pnlDetailBanner.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tpList);
            this.tabControl.Controls.Add(this.tpDetails);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(860, 560);
            this.tabControl.TabIndex = 0;
            // 
            // tpList
            // 
            this.tpList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(251)))), ((int)(((byte)(253)))));
            this.tpList.Controls.Add(this.dgvBankAccounts);
            this.tpList.Controls.Add(this.pnlListHeader);
            this.tpList.Location = new System.Drawing.Point(4, 25);
            this.tpList.Name = "tpList";
            this.tpList.Size = new System.Drawing.Size(852, 531);
            this.tpList.TabIndex = 0;
            this.tpList.Text = "Bank Accounts Directory";
            // 
            // dgvBankAccounts
            // 
            this.dgvBankAccounts.AllowUserToAddRows = false;
            this.dgvBankAccounts.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            this.dgvBankAccounts.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBankAccounts.BackgroundColor = System.Drawing.Color.White;
            this.dgvBankAccounts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBankAccounts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvBankAccounts.ColumnHeadersHeight = 32;
            this.dgvBankAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvBankAccounts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAccount,
            this.colBankName,
            this.colAccountTitle,
            this.colAccountNumber,
            this.colBranchName,
            this.colOpeningBalance,
            this.colCurrency,
            this.colActive});
            this.dgvBankAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBankAccounts.EnableHeadersVisualStyles = false;
            this.dgvBankAccounts.Location = new System.Drawing.Point(0, 52);
            this.dgvBankAccounts.MultiSelect = false;
            this.dgvBankAccounts.Name = "dgvBankAccounts";
            this.dgvBankAccounts.ReadOnly = true;
            this.dgvBankAccounts.RowHeadersVisible = false;
            this.dgvBankAccounts.RowTemplate.Height = 26;
            this.dgvBankAccounts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBankAccounts.Size = new System.Drawing.Size(852, 479);
            this.dgvBankAccounts.TabIndex = 0;
            this.dgvBankAccounts.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvBankAccounts_CellDoubleClick);
            // 
            // colAccount
            // 
            this.colAccount.HeaderText = "Code";
            this.colAccount.Name = "colAccount";
            this.colAccount.ReadOnly = true;
            this.colAccount.Visible = false;
            // 
            // colBankName
            // 
            this.colBankName.HeaderText = "Bank Name";
            this.colBankName.Name = "colBankName";
            this.colBankName.ReadOnly = true;
            this.colBankName.Width = 160;
            // 
            // colAccountTitle
            // 
            this.colAccountTitle.HeaderText = "Account Title";
            this.colAccountTitle.Name = "colAccountTitle";
            this.colAccountTitle.ReadOnly = true;
            this.colAccountTitle.Width = 200;
            // 
            // colAccountNumber
            // 
            this.colAccountNumber.HeaderText = "Account #";
            this.colAccountNumber.Name = "colAccountNumber";
            this.colAccountNumber.ReadOnly = true;
            this.colAccountNumber.Width = 140;
            // 
            // colBranchName
            // 
            this.colBranchName.HeaderText = "Branch";
            this.colBranchName.Name = "colBranchName";
            this.colBranchName.ReadOnly = true;
            this.colBranchName.Width = 140;
            // 
            // colOpeningBalance
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "N2";
            this.colOpeningBalance.DefaultCellStyle = dataGridViewCellStyle3;
            this.colOpeningBalance.HeaderText = "Opening Balance";
            this.colOpeningBalance.Name = "colOpeningBalance";
            this.colOpeningBalance.ReadOnly = true;
            this.colOpeningBalance.Width = 130;
            // 
            // colCurrency
            // 
            this.colCurrency.HeaderText = "Currency";
            this.colCurrency.Name = "colCurrency";
            this.colCurrency.ReadOnly = true;
            this.colCurrency.Width = 70;
            // 
            // colActive
            // 
            this.colActive.HeaderText = "Active";
            this.colActive.Name = "colActive";
            this.colActive.ReadOnly = true;
            this.colActive.Width = 65;
            // 
            // pnlListHeader
            // 
            this.pnlListHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlListHeader.Controls.Add(this.lblSearch);
            this.pnlListHeader.Controls.Add(this.txtSearch);
            this.pnlListHeader.Controls.Add(this.btnRefreshList);
            this.pnlListHeader.Controls.Add(this.btnAddNew);
            this.pnlListHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlListHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlListHeader.Name = "pnlListHeader";
            this.pnlListHeader.Size = new System.Drawing.Size(852, 52);
            this.pnlListHeader.TabIndex = 1;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblSearch.Location = new System.Drawing.Point(14, 18);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(52, 15);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search :";
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtSearch.Location = new System.Drawing.Point(74, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(260, 21);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            // 
            // btnRefreshList
            // 
            this.btnRefreshList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnRefreshList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshList.FlatAppearance.BorderSize = 0;
            this.btnRefreshList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnRefreshList.ForeColor = System.Drawing.Color.White;
            this.btnRefreshList.Location = new System.Drawing.Point(348, 11);
            this.btnRefreshList.Name = "btnRefreshList";
            this.btnRefreshList.Size = new System.Drawing.Size(90, 30);
            this.btnRefreshList.TabIndex = 2;
            this.btnRefreshList.Text = "Refresh";
            this.btnRefreshList.UseVisualStyleBackColor = false;
            this.btnRefreshList.Click += new System.EventHandler(this.BtnRefreshList_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnAddNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddNew.FlatAppearance.BorderSize = 0;
            this.btnAddNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnAddNew.ForeColor = System.Drawing.Color.White;
            this.btnAddNew.Location = new System.Drawing.Point(448, 11);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(150, 30);
            this.btnAddNew.TabIndex = 3;
            this.btnAddNew.Text = "+ New Bank Account";
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.BtnAddNew_Click);
            // 
            // tpDetails
            // 
            this.tpDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(251)))), ((int)(((byte)(253)))));
            this.tpDetails.Controls.Add(this.pnlDetailBanner);
            this.tpDetails.Controls.Add(this.lblBankName);
            this.tpDetails.Controls.Add(this.txtBankName);
            this.tpDetails.Controls.Add(this.chkActive);
            this.tpDetails.Controls.Add(this.lblAccountTitle);
            this.tpDetails.Controls.Add(this.txtAccountTitle);
            this.tpDetails.Controls.Add(this.lblAccountNumber);
            this.tpDetails.Controls.Add(this.txtAccountNumber);
            this.tpDetails.Controls.Add(this.lblCurrency);
            this.tpDetails.Controls.Add(this.cmbCurrency);
            this.tpDetails.Controls.Add(this.lblIBAN);
            this.tpDetails.Controls.Add(this.txtIBAN);
            this.tpDetails.Controls.Add(this.lblBranchName);
            this.tpDetails.Controls.Add(this.txtBranchName);
            this.tpDetails.Controls.Add(this.lblBranchCode);
            this.tpDetails.Controls.Add(this.txtBranchCode);
            this.tpDetails.Controls.Add(this.lblSwiftCode);
            this.tpDetails.Controls.Add(this.txtSwiftCode);
            this.tpDetails.Controls.Add(this.lblOpeningBalance);
            this.tpDetails.Controls.Add(this.txtOpeningBalance);
            this.tpDetails.Controls.Add(this.lblOpeningBalanceHint);
            this.tpDetails.Controls.Add(this.lblNotes);
            this.tpDetails.Controls.Add(this.txtNotes);
            this.tpDetails.Controls.Add(this.lblCreatedByTitle);
            this.tpDetails.Controls.Add(this.lblCreatedBy);
            this.tpDetails.Controls.Add(this.lblEditByTitle);
            this.tpDetails.Controls.Add(this.lblEditBy);
            this.tpDetails.Controls.Add(this.panel2);
            this.tpDetails.Location = new System.Drawing.Point(4, 25);
            this.tpDetails.Name = "tpDetails";
            this.tpDetails.Size = new System.Drawing.Size(852, 531);
            this.tpDetails.TabIndex = 1;
            this.tpDetails.Text = "Bank Account Details";
            // 
            // pnlDetailBanner
            // 
            this.pnlDetailBanner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.pnlDetailBanner.Controls.Add(this.lblBannerSubtitle);
            this.pnlDetailBanner.Controls.Add(this.lblBannerTitle);
            this.pnlDetailBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDetailBanner.Location = new System.Drawing.Point(0, 0);
            this.pnlDetailBanner.Name = "pnlDetailBanner";
            this.pnlDetailBanner.Size = new System.Drawing.Size(852, 50);
            this.pnlDetailBanner.TabIndex = 0;
            // 
            // lblBannerSubtitle
            // 
            this.lblBannerSubtitle.AutoSize = true;
            this.lblBannerSubtitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblBannerSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            this.lblBannerSubtitle.Location = new System.Drawing.Point(14, 28);
            this.lblBannerSubtitle.Name = "lblBannerSubtitle";
            this.lblBannerSubtitle.Size = new System.Drawing.Size(325, 13);
            this.lblBannerSubtitle.TabIndex = 1;
            this.lblBannerSubtitle.Text = "Manage company bank accounts, branches, and opening balances.";
            // 
            // lblBannerTitle
            // 
            this.lblBannerTitle.AutoSize = true;
            this.lblBannerTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblBannerTitle.ForeColor = System.Drawing.Color.White;
            this.lblBannerTitle.Location = new System.Drawing.Point(13, 8);
            this.lblBannerTitle.Name = "lblBannerTitle";
            this.lblBannerTitle.Size = new System.Drawing.Size(199, 18);
            this.lblBannerTitle.TabIndex = 0;
            this.lblBannerTitle.Text = "Bank Account Information";
            // 
            // lblBankName
            // 
            this.lblBankName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.lblBankName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblBankName.Location = new System.Drawing.Point(12, 67);
            this.lblBankName.Name = "lblBankName";
            this.lblBankName.Size = new System.Drawing.Size(140, 20);
            this.lblBankName.TabIndex = 1;
            this.lblBankName.Text = "Bank Name * :";
            this.lblBankName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBankName
            // 
            this.txtBankName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtBankName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtBankName.BackColor = System.Drawing.Color.White;
            this.txtBankName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBankName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtBankName.Location = new System.Drawing.Point(158, 66);
            this.txtBankName.Name = "txtBankName";
            this.txtBankName.Size = new System.Drawing.Size(260, 21);
            this.txtBankName.TabIndex = 2;
            // 
            // chkActive
            // 
            this.chkActive.Checked = true;
            this.chkActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
            this.chkActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.chkActive.Location = new System.Drawing.Point(440, 67);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(80, 20);
            this.chkActive.TabIndex = 3;
            this.chkActive.Text = "Active";
            // 
            // lblAccountTitle
            // 
            this.lblAccountTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.lblAccountTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblAccountTitle.Location = new System.Drawing.Point(12, 99);
            this.lblAccountTitle.Name = "lblAccountTitle";
            this.lblAccountTitle.Size = new System.Drawing.Size(140, 20);
            this.lblAccountTitle.TabIndex = 4;
            this.lblAccountTitle.Text = "Account Title * :";
            this.lblAccountTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAccountTitle
            // 
            this.txtAccountTitle.BackColor = System.Drawing.Color.White;
            this.txtAccountTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAccountTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtAccountTitle.Location = new System.Drawing.Point(158, 98);
            this.txtAccountTitle.Name = "txtAccountTitle";
            this.txtAccountTitle.Size = new System.Drawing.Size(420, 21);
            this.txtAccountTitle.TabIndex = 5;
            // 
            // lblAccountNumber
            // 
            this.lblAccountNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.lblAccountNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblAccountNumber.Location = new System.Drawing.Point(12, 131);
            this.lblAccountNumber.Name = "lblAccountNumber";
            this.lblAccountNumber.Size = new System.Drawing.Size(140, 20);
            this.lblAccountNumber.TabIndex = 6;
            this.lblAccountNumber.Text = "Account Number :";
            this.lblAccountNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.BackColor = System.Drawing.Color.White;
            this.txtAccountNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAccountNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtAccountNumber.Location = new System.Drawing.Point(158, 130);
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.Size = new System.Drawing.Size(200, 21);
            this.txtAccountNumber.TabIndex = 7;
            // 
            // lblCurrency
            // 
            this.lblCurrency.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.lblCurrency.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblCurrency.Location = new System.Drawing.Point(365, 131);
            this.lblCurrency.Name = "lblCurrency";
            this.lblCurrency.Size = new System.Drawing.Size(75, 20);
            this.lblCurrency.TabIndex = 8;
            this.lblCurrency.Text = "Currency :";
            this.lblCurrency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbCurrency
            // 
            this.cmbCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCurrency.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cmbCurrency.FormattingEnabled = true;
            this.cmbCurrency.Items.AddRange(new object[] {
            "PKR",
            "USD",
            "EUR",
            "GBP",
            "AED",
            "SAR"});
            this.cmbCurrency.Location = new System.Drawing.Point(445, 129);
            this.cmbCurrency.Name = "cmbCurrency";
            this.cmbCurrency.Size = new System.Drawing.Size(133, 23);
            this.cmbCurrency.TabIndex = 9;
            // 
            // lblIBAN
            // 
            this.lblIBAN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.lblIBAN.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblIBAN.Location = new System.Drawing.Point(12, 163);
            this.lblIBAN.Name = "lblIBAN";
            this.lblIBAN.Size = new System.Drawing.Size(140, 20);
            this.lblIBAN.TabIndex = 10;
            this.lblIBAN.Text = "IBAN :";
            this.lblIBAN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtIBAN
            // 
            this.txtIBAN.BackColor = System.Drawing.Color.White;
            this.txtIBAN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIBAN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtIBAN.Location = new System.Drawing.Point(158, 162);
            this.txtIBAN.Name = "txtIBAN";
            this.txtIBAN.Size = new System.Drawing.Size(420, 21);
            this.txtIBAN.TabIndex = 11;
            // 
            // lblBranchName
            // 
            this.lblBranchName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.lblBranchName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblBranchName.Location = new System.Drawing.Point(12, 195);
            this.lblBranchName.Name = "lblBranchName";
            this.lblBranchName.Size = new System.Drawing.Size(140, 20);
            this.lblBranchName.TabIndex = 12;
            this.lblBranchName.Text = "Branch Name :";
            this.lblBranchName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBranchName
            // 
            this.txtBranchName.BackColor = System.Drawing.Color.White;
            this.txtBranchName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBranchName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtBranchName.Location = new System.Drawing.Point(158, 194);
            this.txtBranchName.Name = "txtBranchName";
            this.txtBranchName.Size = new System.Drawing.Size(420, 21);
            this.txtBranchName.TabIndex = 13;
            // 
            // lblBranchCode
            // 
            this.lblBranchCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.lblBranchCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblBranchCode.Location = new System.Drawing.Point(12, 227);
            this.lblBranchCode.Name = "lblBranchCode";
            this.lblBranchCode.Size = new System.Drawing.Size(140, 20);
            this.lblBranchCode.TabIndex = 14;
            this.lblBranchCode.Text = "Branch Code :";
            this.lblBranchCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBranchCode
            // 
            this.txtBranchCode.BackColor = System.Drawing.Color.White;
            this.txtBranchCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBranchCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtBranchCode.Location = new System.Drawing.Point(158, 226);
            this.txtBranchCode.Name = "txtBranchCode";
            this.txtBranchCode.Size = new System.Drawing.Size(130, 21);
            this.txtBranchCode.TabIndex = 15;
            // 
            // lblSwiftCode
            // 
            this.lblSwiftCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.lblSwiftCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblSwiftCode.Location = new System.Drawing.Point(295, 227);
            this.lblSwiftCode.Name = "lblSwiftCode";
            this.lblSwiftCode.Size = new System.Drawing.Size(95, 20);
            this.lblSwiftCode.TabIndex = 16;
            this.lblSwiftCode.Text = "SWIFT / BIC :";
            this.lblSwiftCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSwiftCode
            // 
            this.txtSwiftCode.BackColor = System.Drawing.Color.White;
            this.txtSwiftCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSwiftCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtSwiftCode.Location = new System.Drawing.Point(395, 226);
            this.txtSwiftCode.Name = "txtSwiftCode";
            this.txtSwiftCode.Size = new System.Drawing.Size(183, 21);
            this.txtSwiftCode.TabIndex = 17;
            // 
            // lblOpeningBalance
            // 
            this.lblOpeningBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
            this.lblOpeningBalance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.lblOpeningBalance.Location = new System.Drawing.Point(12, 259);
            this.lblOpeningBalance.Name = "lblOpeningBalance";
            this.lblOpeningBalance.Size = new System.Drawing.Size(140, 20);
            this.lblOpeningBalance.TabIndex = 18;
            this.lblOpeningBalance.Text = "Opening Balance :";
            this.lblOpeningBalance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOpeningBalance
            // 
            this.txtOpeningBalance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(252)))), ((int)(((byte)(232)))));
            this.txtOpeningBalance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOpeningBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtOpeningBalance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtOpeningBalance.Location = new System.Drawing.Point(158, 258);
            this.txtOpeningBalance.Name = "txtOpeningBalance";
            this.txtOpeningBalance.Size = new System.Drawing.Size(180, 22);
            this.txtOpeningBalance.TabIndex = 19;
            this.txtOpeningBalance.Text = "0.00";
            this.txtOpeningBalance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblOpeningBalanceHint
            // 
            this.lblOpeningBalanceHint.AutoSize = true;
            this.lblOpeningBalanceHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic);
            this.lblOpeningBalanceHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblOpeningBalanceHint.Location = new System.Drawing.Point(345, 262);
            this.lblOpeningBalanceHint.Name = "lblOpeningBalanceHint";
            this.lblOpeningBalanceHint.Size = new System.Drawing.Size(225, 13);
            this.lblOpeningBalanceHint.TabIndex = 20;
            this.lblOpeningBalanceHint.Text = "(Initial ledger balance when onboarding account)";
            // 
            // lblNotes
            // 
            this.lblNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.lblNotes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblNotes.Location = new System.Drawing.Point(12, 291);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(140, 20);
            this.lblNotes.TabIndex = 21;
            this.lblNotes.Text = "Notes / Description :";
            this.lblNotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNotes
            // 
            this.txtNotes.BackColor = System.Drawing.Color.White;
            this.txtNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtNotes.Location = new System.Drawing.Point(158, 290);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(420, 50);
            this.txtNotes.TabIndex = 22;
            // 
            // lblCreatedByTitle
            // 
            this.lblCreatedByTitle.AutoSize = true;
            this.lblCreatedByTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblCreatedByTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblCreatedByTitle.Location = new System.Drawing.Point(85, 360);
            this.lblCreatedByTitle.Name = "lblCreatedByTitle";
            this.lblCreatedByTitle.Size = new System.Drawing.Size(65, 13);
            this.lblCreatedByTitle.TabIndex = 23;
            this.lblCreatedByTitle.Text = "Created By :";
            // 
            // lblCreatedBy
            // 
            this.lblCreatedBy.AutoSize = true;
            this.lblCreatedBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblCreatedBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblCreatedBy.Location = new System.Drawing.Point(158, 360);
            this.lblCreatedBy.Name = "lblCreatedBy";
            this.lblCreatedBy.Size = new System.Drawing.Size(10, 13);
            this.lblCreatedBy.TabIndex = 24;
            this.lblCreatedBy.Text = "-";
            // 
            // lblEditByTitle
            // 
            this.lblEditByTitle.AutoSize = true;
            this.lblEditByTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblEditByTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblEditByTitle.Location = new System.Drawing.Point(72, 385);
            this.lblEditByTitle.Name = "lblEditByTitle";
            this.lblEditByTitle.Size = new System.Drawing.Size(76, 13);
            this.lblEditByTitle.TabIndex = 25;
            this.lblEditByTitle.Text = "Last Modified :";
            // 
            // lblEditBy
            // 
            this.lblEditBy.AutoSize = true;
            this.lblEditBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblEditBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblEditBy.Location = new System.Drawing.Point(158, 385);
            this.lblEditBy.Name = "lblEditBy";
            this.lblEditBy.Size = new System.Drawing.Size(10, 13);
            this.lblEditBy.TabIndex = 26;
            this.lblEditBy.Text = "-";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.panel2.Controls.Add(this.btnNew);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.btnDelete);
            this.panel2.Controls.Add(this.btnStatement);
            this.panel2.Controls.Add(this.btnReconcile);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 476);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(852, 55);
            this.panel2.TabIndex = 27;
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.btnNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnNew.ForeColor = System.Drawing.Color.White;
            this.btnNew.Location = new System.Drawing.Point(16, 12);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(80, 32);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "&New";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.BtnNew_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(103, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 32);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(190, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(80, 32);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnStatement
            // 
            this.btnStatement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnStatement.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStatement.FlatAppearance.BorderSize = 0;
            this.btnStatement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStatement.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnStatement.ForeColor = System.Drawing.Color.White;
            this.btnStatement.Location = new System.Drawing.Point(285, 12);
            this.btnStatement.Name = "btnStatement";
            this.btnStatement.Size = new System.Drawing.Size(125, 32);
            this.btnStatement.TabIndex = 3;
            this.btnStatement.Text = "Ledger / Statement";
            this.btnStatement.UseVisualStyleBackColor = false;
            this.btnStatement.Click += new System.EventHandler(this.BtnStatement_Click);
            // 
            // btnReconcile
            // 
            this.btnReconcile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(58)))), ((int)(((byte)(237)))));
            this.btnReconcile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReconcile.FlatAppearance.BorderSize = 0;
            this.btnReconcile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReconcile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnReconcile.ForeColor = System.Drawing.Color.White;
            this.btnReconcile.Location = new System.Drawing.Point(417, 12);
            this.btnReconcile.Name = "btnReconcile";
            this.btnReconcile.Size = new System.Drawing.Size(100, 32);
            this.btnReconcile.TabIndex = 4;
            this.btnReconcile.Text = "Reconcile";
            this.btnReconcile.UseVisualStyleBackColor = false;
            this.btnReconcile.Click += new System.EventHandler(this.BtnReconcile_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(525, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 32);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // frmBankInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(860, 560);
            this.Controls.Add(this.tabControl);
            this.Name = "frmBankInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bank Account Information";
            this.Load += new System.EventHandler(this.FrmBankInfo_Load);
            this.tabControl.ResumeLayout(false);
            this.tpList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankAccounts)).EndInit();
            this.pnlListHeader.ResumeLayout(false);
            this.pnlListHeader.PerformLayout();
            this.tpDetails.ResumeLayout(false);
            this.tpDetails.PerformLayout();
            this.pnlDetailBanner.ResumeLayout(false);
            this.pnlDetailBanner.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tpList;
        private System.Windows.Forms.DataGridView dgvBankAccounts;
        private System.Windows.Forms.Panel pnlListHeader;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnRefreshList;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.TabPage tpDetails;
        private System.Windows.Forms.Panel pnlDetailBanner;
        private System.Windows.Forms.Label lblBannerTitle;
        private System.Windows.Forms.Label lblBannerSubtitle;
        private System.Windows.Forms.Label lblBankName;
        private System.Windows.Forms.TextBox txtBankName;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Label lblAccountTitle;
        private System.Windows.Forms.TextBox txtAccountTitle;
        private System.Windows.Forms.Label lblAccountNumber;
        private System.Windows.Forms.TextBox txtAccountNumber;
        private System.Windows.Forms.Label lblCurrency;
        private System.Windows.Forms.ComboBox cmbCurrency;
        private System.Windows.Forms.Label lblIBAN;
        private System.Windows.Forms.TextBox txtIBAN;
        private System.Windows.Forms.Label lblBranchName;
        private System.Windows.Forms.TextBox txtBranchName;
        private System.Windows.Forms.Label lblBranchCode;
        private System.Windows.Forms.TextBox txtBranchCode;
        private System.Windows.Forms.Label lblSwiftCode;
        private System.Windows.Forms.TextBox txtSwiftCode;
        private System.Windows.Forms.Label lblOpeningBalance;
        private ERP.DecimalTextbox txtOpeningBalance;
        private System.Windows.Forms.Label lblOpeningBalanceHint;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label lblCreatedByTitle;
        private System.Windows.Forms.Label lblCreatedBy;
        private System.Windows.Forms.Label lblEditByTitle;
        private System.Windows.Forms.Label lblEditBy;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnStatement;
        private System.Windows.Forms.Button btnReconcile;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBankName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccountTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccountNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBranchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOpeningBalance;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurrency;
        private System.Windows.Forms.DataGridViewTextBoxColumn colActive;
    }
}
