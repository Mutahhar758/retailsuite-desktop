namespace ERP
{
    partial class frmCustomerInfo
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCustomerInfo));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeaderShortcuts = new System.Windows.Forms.Label();
            this.lblHeaderSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tpList = new System.Windows.Forms.TabPage();
            this.dgvCustomers = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnCustomerCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlListHeader = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnRefreshList = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.tpDetails = new System.Windows.Forms.TabPage();
            this.grpCustomerInfo = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAccountCode = new System.Windows.Forms.TextBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPhone1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPhone2 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtNic = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtFax = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtQualification = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtSMSNumber = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtIBAN = new System.Windows.Forms.TextBox();
            this.lblCustomerCategory = new System.Windows.Forms.Label();
            this.cmbCustomerCategory = new System.Windows.Forms.ComboBox();
            this.chkIsSubAccount = new System.Windows.Forms.CheckBox();
            this.cmbParentCustomer = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSMSAlert = new System.Windows.Forms.CheckBox();
            this.chkEmailAlert = new System.Windows.Forms.CheckBox();
            this.profileImage1 = new ERP.Controls.ProfileImage();
            this.cmbCustomerCode = new System.Windows.Forms.ComboBox();
            this.grpSupplyDefaults = new System.Windows.Forms.GroupBox();
            this.lblSupplyCustomerInfo = new System.Windows.Forms.Label();
            this.btnAddSupplyItem = new System.Windows.Forms.Button();
            this.btnDeleteSupplyItem = new System.Windows.Forms.Button();
            this.dgvSupplyItems = new System.Windows.Forms.DataGridView();
            this.clnSupplyItemId = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clnSupplyQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnSupplySecQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnSupplyRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnSupplyDiscount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnSupplyCarriage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnSupplyAddLess = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCreatedBy = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblEditBy = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tpList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
            this.pnlListHeader.SuspendLayout();
            this.tpDetails.SuspendLayout();
            this.grpCustomerInfo.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpSupplyDefaults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSupplyItems)).BeginInit();
            this.panel2.SuspendLayout();
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
            this.pnlHeader.Size = new System.Drawing.Size(984, 54);
            this.pnlHeader.TabIndex = 200;
            // 
            // lblHeaderShortcuts
            // 
            this.lblHeaderShortcuts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHeaderShortcuts.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderShortcuts.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblHeaderShortcuts.Location = new System.Drawing.Point(520, 16);
            this.lblHeaderShortcuts.Name = "lblHeaderShortcuts";
            this.lblHeaderShortcuts.Size = new System.Drawing.Size(450, 20);
            this.lblHeaderShortcuts.TabIndex = 3;
            this.lblHeaderShortcuts.Text = "⌨  [F3] New Customer     [F5] Save Customer     [Esc] Close";
            this.lblHeaderShortcuts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHeaderSubtitle
            // 
            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblHeaderSubtitle.Location = new System.Drawing.Point(62, 31);
            this.lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            this.lblHeaderSubtitle.Size = new System.Drawing.Size(418, 13);
            this.lblHeaderSubtitle.TabIndex = 2;
            this.lblHeaderSubtitle.Text = "Create, edit and manage customer accounts, contact details & default supply orders";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblTitle.Location = new System.Drawing.Point(60, 6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(217, 25);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Customer Management";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ERP.Properties.Resources.logoPNG;
            this.pictureBox1.Location = new System.Drawing.Point(14, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 40);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tpList);
            this.tabControl.Controls.Add(this.tpDetails);
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(6, 60);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(972, 614);
            this.tabControl.TabIndex = 0;
            // 
            // tpList
            // 
            this.tpList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.tpList.Controls.Add(this.dgvCustomers);
            this.tpList.Controls.Add(this.pnlListHeader);
            this.tpList.Location = new System.Drawing.Point(4, 26);
            this.tpList.Name = "tpList";
            this.tpList.Size = new System.Drawing.Size(964, 584);
            this.tpList.TabIndex = 0;
            this.tpList.Text = "Search Customers";
            // 
            // dgvCustomers
            // 
            this.dgvCustomers.AllowUserToAddRows = false;
            this.dgvCustomers.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.dgvCustomers.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCustomers.BackgroundColor = System.Drawing.Color.White;
            this.dgvCustomers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCustomers.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCustomers.ColumnHeadersHeight = 32;
            this.dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCustomers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.clnCustomerCategory,
            this.dataGridViewTextBoxColumn6});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCustomers.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomers.EnableHeadersVisualStyles = false;
            this.dgvCustomers.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dgvCustomers.Location = new System.Drawing.Point(0, 48);
            this.dgvCustomers.MultiSelect = false;
            this.dgvCustomers.Name = "dgvCustomers";
            this.dgvCustomers.ReadOnly = true;
            this.dgvCustomers.RowHeadersVisible = false;
            this.dgvCustomers.RowTemplate.Height = 28;
            this.dgvCustomers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomers.Size = new System.Drawing.Size(964, 536);
            this.dgvCustomers.TabIndex = 2;
            this.dgvCustomers.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvCustomers_CellDoubleClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Account Code";
            this.dataGridViewTextBoxColumn1.Name = "Account";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 130;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Customer Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 220;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Phone # 1";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 120;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Email";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 160;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Address";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 200;
            // 
            // clnCustomerCategory
            // 
            this.clnCustomerCategory.HeaderText = "Category";
            this.clnCustomerCategory.Name = "clnCustomerCategory";
            this.clnCustomerCategory.ReadOnly = true;
            this.clnCustomerCategory.Width = 110;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Active";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 70;
            // 
            // pnlListHeader
            // 
            this.pnlListHeader.BackColor = System.Drawing.Color.White;
            this.pnlListHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlListHeader.Controls.Add(this.lblSearch);
            this.pnlListHeader.Controls.Add(this.txtSearch);
            this.pnlListHeader.Controls.Add(this.btnRefreshList);
            this.pnlListHeader.Controls.Add(this.btnAddNew);
            this.pnlListHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlListHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlListHeader.Name = "pnlListHeader";
            this.pnlListHeader.Size = new System.Drawing.Size(964, 48);
            this.pnlListHeader.TabIndex = 1;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblSearch.Location = new System.Drawing.Point(14, 15);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(51, 15);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search :";
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(72, 11);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(280, 24);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            // 
            // btnRefreshList
            // 
            this.btnRefreshList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnRefreshList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshList.FlatAppearance.BorderSize = 0;
            this.btnRefreshList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshList.Font = new System.Drawing.Font("Segoe UI", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshList.ForeColor = System.Drawing.Color.White;
            this.btnRefreshList.Location = new System.Drawing.Point(365, 9);
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
            this.btnAddNew.Font = new System.Drawing.Font("Segoe UI", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddNew.ForeColor = System.Drawing.Color.White;
            this.btnAddNew.Location = new System.Drawing.Point(465, 9);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(155, 30);
            this.btnAddNew.TabIndex = 3;
            this.btnAddNew.Text = "+ New Customer [F3]";
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.BtnAddNew_Click);
            // 
            // tpDetails
            // 
            this.tpDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.tpDetails.Controls.Add(this.grpSupplyDefaults);
            this.tpDetails.Controls.Add(this.grpCustomerInfo);
            this.tpDetails.Controls.Add(this.panel2);
            this.tpDetails.Location = new System.Drawing.Point(4, 26);
            this.tpDetails.Name = "tpDetails";
            this.tpDetails.Padding = new System.Windows.Forms.Padding(6);
            this.tpDetails.Size = new System.Drawing.Size(964, 584);
            this.tpDetails.TabIndex = 1;
            this.tpDetails.Text = "Customer Details";
            // 
            // grpCustomerInfo
            // 
            this.grpCustomerInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCustomerInfo.BackColor = System.Drawing.Color.White;
            this.grpCustomerInfo.Controls.Add(this.cmbCustomerCode);
            this.grpCustomerInfo.Controls.Add(this.profileImage1);
            this.grpCustomerInfo.Controls.Add(this.groupBox1);
            this.grpCustomerInfo.Controls.Add(this.cmbParentCustomer);
            this.grpCustomerInfo.Controls.Add(this.chkIsSubAccount);
            this.grpCustomerInfo.Controls.Add(this.cmbCustomerCategory);
            this.grpCustomerInfo.Controls.Add(this.lblCustomerCategory);
            this.grpCustomerInfo.Controls.Add(this.txtIBAN);
            this.grpCustomerInfo.Controls.Add(this.label1);
            this.grpCustomerInfo.Controls.Add(this.txtSMSNumber);
            this.grpCustomerInfo.Controls.Add(this.label11);
            this.grpCustomerInfo.Controls.Add(this.txtQualification);
            this.grpCustomerInfo.Controls.Add(this.label13);
            this.grpCustomerInfo.Controls.Add(this.txtAddress);
            this.grpCustomerInfo.Controls.Add(this.label12);
            this.grpCustomerInfo.Controls.Add(this.txtFax);
            this.grpCustomerInfo.Controls.Add(this.label20);
            this.grpCustomerInfo.Controls.Add(this.txtNic);
            this.grpCustomerInfo.Controls.Add(this.label15);
            this.grpCustomerInfo.Controls.Add(this.txtPhone2);
            this.grpCustomerInfo.Controls.Add(this.label7);
            this.grpCustomerInfo.Controls.Add(this.txtPhone1);
            this.grpCustomerInfo.Controls.Add(this.label8);
            this.grpCustomerInfo.Controls.Add(this.txtEmail);
            this.grpCustomerInfo.Controls.Add(this.label21);
            this.grpCustomerInfo.Controls.Add(this.txtTitle);
            this.grpCustomerInfo.Controls.Add(this.lblCustomerName);
            this.grpCustomerInfo.Controls.Add(this.chkActive);
            this.grpCustomerInfo.Controls.Add(this.txtAccountCode);
            this.grpCustomerInfo.Controls.Add(this.label2);
            this.grpCustomerInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpCustomerInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpCustomerInfo.Location = new System.Drawing.Point(8, 6);
            this.grpCustomerInfo.Name = "grpCustomerInfo";
            this.grpCustomerInfo.Size = new System.Drawing.Size(948, 275);
            this.grpCustomerInfo.TabIndex = 0;
            this.grpCustomerInfo.TabStop = false;
            this.grpCustomerInfo.Text = " Customer Profile & Contact Details ";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label2.Location = new System.Drawing.Point(12, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 20);
            this.label2.TabIndex = 100;
            this.label2.Text = "Account Code :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAccountCode
            // 
            this.txtAccountCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtAccountCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAccountCode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccountCode.Location = new System.Drawing.Point(112, 22);
            this.txtAccountCode.Name = "txtAccountCode";
            this.txtAccountCode.ReadOnly = true;
            this.txtAccountCode.Size = new System.Drawing.Size(140, 23);
            this.txtAccountCode.TabIndex = 101;
            this.txtAccountCode.TabStop = false;
            // 
            // chkActive
            // 
            this.chkActive.Checked = true;
            this.chkActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActive.Font = new System.Drawing.Font("Segoe UI", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.chkActive.Location = new System.Drawing.Point(260, 23);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(75, 20);
            this.chkActive.TabIndex = 102;
            this.chkActive.Text = "Active";
            this.chkActive.UseVisualStyleBackColor = true;
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblCustomerName.Location = new System.Drawing.Point(12, 51);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(95, 20);
            this.lblCustomerName.TabIndex = 103;
            this.lblCustomerName.Text = "Customer Name :";
            this.lblCustomerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTitle
            // 
            this.txtTitle.BackColor = System.Drawing.Color.White;
            this.txtTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTitle.Location = new System.Drawing.Point(112, 50);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(518, 23);
            this.txtTitle.TabIndex = 1;
            // 
            // label21
            // 
            this.label21.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label21.Location = new System.Drawing.Point(12, 79);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(95, 20);
            this.label21.TabIndex = 110;
            this.label21.Text = "Email Address :";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtEmail
            // 
            this.txtEmail.BackColor = System.Drawing.Color.White;
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEmail.Location = new System.Drawing.Point(112, 78);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(518, 23);
            this.txtEmail.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label8.Location = new System.Drawing.Point(12, 107);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 20);
            this.label8.TabIndex = 111;
            this.label8.Text = "Phone # 1 :";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPhone1
            // 
            this.txtPhone1.BackColor = System.Drawing.Color.White;
            this.txtPhone1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPhone1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPhone1.Location = new System.Drawing.Point(112, 106);
            this.txtPhone1.Name = "txtPhone1";
            this.txtPhone1.Size = new System.Drawing.Size(195, 23);
            this.txtPhone1.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label7.Location = new System.Drawing.Point(325, 107);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 20);
            this.label7.TabIndex = 112;
            this.label7.Text = "Phone # 2 :";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPhone2
            // 
            this.txtPhone2.BackColor = System.Drawing.Color.White;
            this.txtPhone2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPhone2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPhone2.Location = new System.Drawing.Point(415, 106);
            this.txtPhone2.Name = "txtPhone2";
            this.txtPhone2.Size = new System.Drawing.Size(215, 23);
            this.txtPhone2.TabIndex = 4;
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label15.Location = new System.Drawing.Point(12, 135);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(95, 20);
            this.label15.TabIndex = 113;
            this.label15.Text = "CNIC / NIC :";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNic
            // 
            this.txtNic.BackColor = System.Drawing.Color.White;
            this.txtNic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNic.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNic.Location = new System.Drawing.Point(112, 134);
            this.txtNic.Name = "txtNic";
            this.txtNic.Size = new System.Drawing.Size(195, 23);
            this.txtNic.TabIndex = 5;
            // 
            // label20
            // 
            this.label20.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label20.Location = new System.Drawing.Point(325, 135);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(85, 20);
            this.label20.TabIndex = 114;
            this.label20.Text = "Fax :";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFax
            // 
            this.txtFax.BackColor = System.Drawing.Color.White;
            this.txtFax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFax.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFax.Location = new System.Drawing.Point(415, 134);
            this.txtFax.Name = "txtFax";
            this.txtFax.Size = new System.Drawing.Size(215, 23);
            this.txtFax.TabIndex = 6;
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label12.Location = new System.Drawing.Point(12, 163);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(95, 20);
            this.label12.TabIndex = 115;
            this.label12.Text = "Address :";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAddress
            // 
            this.txtAddress.BackColor = System.Drawing.Color.White;
            this.txtAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAddress.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress.Location = new System.Drawing.Point(112, 162);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(518, 23);
            this.txtAddress.TabIndex = 7;
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label13.Location = new System.Drawing.Point(12, 191);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(95, 20);
            this.label13.TabIndex = 116;
            this.label13.Text = "Qualification :";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtQualification
            // 
            this.txtQualification.BackColor = System.Drawing.Color.White;
            this.txtQualification.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtQualification.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQualification.Location = new System.Drawing.Point(112, 190);
            this.txtQualification.Name = "txtQualification";
            this.txtQualification.Size = new System.Drawing.Size(195, 23);
            this.txtQualification.TabIndex = 8;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label11.Location = new System.Drawing.Point(325, 191);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(85, 20);
            this.label11.TabIndex = 117;
            this.label11.Text = "SMS Number :";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSMSNumber
            // 
            this.txtSMSNumber.BackColor = System.Drawing.Color.White;
            this.txtSMSNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSMSNumber.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSMSNumber.Location = new System.Drawing.Point(415, 190);
            this.txtSMSNumber.Name = "txtSMSNumber";
            this.txtSMSNumber.Size = new System.Drawing.Size(215, 23);
            this.txtSMSNumber.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label1.Location = new System.Drawing.Point(12, 219);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 20);
            this.label1.TabIndex = 118;
            this.label1.Text = "IBAN :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtIBAN
            // 
            this.txtIBAN.BackColor = System.Drawing.Color.White;
            this.txtIBAN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIBAN.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIBAN.Location = new System.Drawing.Point(112, 218);
            this.txtIBAN.Name = "txtIBAN";
            this.txtIBAN.Size = new System.Drawing.Size(195, 23);
            this.txtIBAN.TabIndex = 10;
            // 
            // lblCustomerCategory
            // 
            this.lblCustomerCategory.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblCustomerCategory.Location = new System.Drawing.Point(325, 219);
            this.lblCustomerCategory.Name = "lblCustomerCategory";
            this.lblCustomerCategory.Size = new System.Drawing.Size(85, 20);
            this.lblCustomerCategory.TabIndex = 119;
            this.lblCustomerCategory.Text = "Category :";
            this.lblCustomerCategory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbCustomerCategory
            // 
            this.cmbCustomerCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomerCategory.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCustomerCategory.FormattingEnabled = true;
            this.cmbCustomerCategory.Location = new System.Drawing.Point(415, 218);
            this.cmbCustomerCategory.Name = "cmbCustomerCategory";
            this.cmbCustomerCategory.Size = new System.Drawing.Size(215, 23);
            this.cmbCustomerCategory.TabIndex = 11;
            // 
            // chkIsSubAccount
            // 
            this.chkIsSubAccount.AutoSize = true;
            this.chkIsSubAccount.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkIsSubAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.chkIsSubAccount.Location = new System.Drawing.Point(12, 246);
            this.chkIsSubAccount.Name = "chkIsSubAccount";
            this.chkIsSubAccount.Size = new System.Drawing.Size(126, 19);
            this.chkIsSubAccount.TabIndex = 12;
            this.chkIsSubAccount.Text = "Is Sub-Account of :";
            this.chkIsSubAccount.UseVisualStyleBackColor = true;
            // 
            // cmbParentCustomer
            // 
            this.cmbParentCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParentCustomer.Enabled = false;
            this.cmbParentCustomer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbParentCustomer.FormattingEnabled = true;
            this.cmbParentCustomer.Location = new System.Drawing.Point(145, 244);
            this.cmbParentCustomer.Name = "cmbParentCustomer";
            this.cmbParentCustomer.Size = new System.Drawing.Size(162, 23);
            this.cmbParentCustomer.TabIndex = 13;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSMSAlert);
            this.groupBox1.Controls.Add(this.chkEmailAlert);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.groupBox1.Location = new System.Drawing.Point(325, 238);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(305, 32);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Alerts :";
            // 
            // chkSMSAlert
            // 
            this.chkSMSAlert.AutoSize = true;
            this.chkSMSAlert.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSMSAlert.Location = new System.Drawing.Point(60, 10);
            this.chkSMSAlert.Name = "chkSMSAlert";
            this.chkSMSAlert.Size = new System.Drawing.Size(49, 19);
            this.chkSMSAlert.TabIndex = 0;
            this.chkSMSAlert.Text = "SMS";
            this.chkSMSAlert.UseVisualStyleBackColor = true;
            // 
            // chkEmailAlert
            // 
            this.chkEmailAlert.AutoSize = true;
            this.chkEmailAlert.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEmailAlert.Location = new System.Drawing.Point(125, 10);
            this.chkEmailAlert.Name = "chkEmailAlert";
            this.chkEmailAlert.Size = new System.Drawing.Size(55, 19);
            this.chkEmailAlert.TabIndex = 1;
            this.chkEmailAlert.Text = "Email";
            this.chkEmailAlert.UseVisualStyleBackColor = true;
            // 
            // profileImage1
            // 
            this.profileImage1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.profileImage1.Image = null;
            this.profileImage1.Location = new System.Drawing.Point(740, 20);
            this.profileImage1.MediaId = null;
            this.profileImage1.MediaUrl = null;
            this.profileImage1.Name = "profileImage1";
            this.profileImage1.Size = new System.Drawing.Size(190, 245);
            this.profileImage1.TabIndex = 200;
            // 
            // cmbCustomerCode
            // 
            this.cmbCustomerCode.FormattingEnabled = true;
            this.cmbCustomerCode.Location = new System.Drawing.Point(0, -50);
            this.cmbCustomerCode.Name = "cmbCustomerCode";
            this.cmbCustomerCode.Size = new System.Drawing.Size(100, 23);
            this.cmbCustomerCode.TabIndex = 999;
            this.cmbCustomerCode.Visible = false;
            this.cmbCustomerCode.SelectedIndexChanged += new System.EventHandler(this.cmbCustomerCode_SelectedIndexChanged);
            // 
            // grpSupplyDefaults
            // 
            this.grpSupplyDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSupplyDefaults.BackColor = System.Drawing.Color.White;
            this.grpSupplyDefaults.Controls.Add(this.dgvSupplyItems);
            this.grpSupplyDefaults.Controls.Add(this.btnDeleteSupplyItem);
            this.grpSupplyDefaults.Controls.Add(this.btnAddSupplyItem);
            this.grpSupplyDefaults.Controls.Add(this.lblSupplyCustomerInfo);
            this.grpSupplyDefaults.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpSupplyDefaults.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpSupplyDefaults.Location = new System.Drawing.Point(8, 286);
            this.grpSupplyDefaults.Name = "grpSupplyDefaults";
            this.grpSupplyDefaults.Size = new System.Drawing.Size(948, 234);
            this.grpSupplyDefaults.TabIndex = 1;
            this.grpSupplyDefaults.TabStop = false;
            this.grpSupplyDefaults.Text = " Customer Supply Order Defaults (Default Quantities & Rates) ";
            // 
            // lblSupplyCustomerInfo
            // 
            this.lblSupplyCustomerInfo.AutoSize = true;
            this.lblSupplyCustomerInfo.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSupplyCustomerInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblSupplyCustomerInfo.Location = new System.Drawing.Point(12, 23);
            this.lblSupplyCustomerInfo.Name = "lblSupplyCustomerInfo";
            this.lblSupplyCustomerInfo.Size = new System.Drawing.Size(374, 13);
            this.lblSupplyCustomerInfo.TabIndex = 0;
            this.lblSupplyCustomerInfo.Text = "Default quantities and rates will automatically pre-fill during sales & orders.";
            // 
            // btnAddSupplyItem
            // 
            this.btnAddSupplyItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSupplyItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnAddSupplyItem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddSupplyItem.FlatAppearance.BorderSize = 0;
            this.btnAddSupplyItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddSupplyItem.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddSupplyItem.ForeColor = System.Drawing.Color.White;
            this.btnAddSupplyItem.Location = new System.Drawing.Point(720, 16);
            this.btnAddSupplyItem.Name = "btnAddSupplyItem";
            this.btnAddSupplyItem.Size = new System.Drawing.Size(105, 26);
            this.btnAddSupplyItem.TabIndex = 1;
            this.btnAddSupplyItem.Text = "+ Add Line";
            this.btnAddSupplyItem.UseVisualStyleBackColor = false;
            this.btnAddSupplyItem.Click += new System.EventHandler(this.btnAddSupplyItem_Click);
            // 
            // btnDeleteSupplyItem
            // 
            this.btnDeleteSupplyItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteSupplyItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDeleteSupplyItem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeleteSupplyItem.FlatAppearance.BorderSize = 0;
            this.btnDeleteSupplyItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteSupplyItem.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteSupplyItem.ForeColor = System.Drawing.Color.White;
            this.btnDeleteSupplyItem.Location = new System.Drawing.Point(835, 16);
            this.btnDeleteSupplyItem.Name = "btnDeleteSupplyItem";
            this.btnDeleteSupplyItem.Size = new System.Drawing.Size(105, 26);
            this.btnDeleteSupplyItem.TabIndex = 2;
            this.btnDeleteSupplyItem.Text = "- Remove Line";
            this.btnDeleteSupplyItem.UseVisualStyleBackColor = false;
            this.btnDeleteSupplyItem.Click += new System.EventHandler(this.btnDeleteSupplyItem_Click);
            // 
            // dgvSupplyItems
            // 
            this.dgvSupplyItems.AllowUserToAddRows = false;
            this.dgvSupplyItems.AllowUserToDeleteRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.dgvSupplyItems.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSupplyItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSupplyItems.BackgroundColor = System.Drawing.Color.White;
            this.dgvSupplyItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSupplyItems.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSupplyItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvSupplyItems.ColumnHeadersHeight = 28;
            this.dgvSupplyItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvSupplyItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnSupplyItemId,
            this.clnSupplyQty,
            this.clnSupplySecQty,
            this.clnSupplyRate,
            this.clnSupplyDiscount,
            this.clnSupplyCarriage,
            this.clnSupplyAddLess});
            this.dgvSupplyItems.EnableHeadersVisualStyles = false;
            this.dgvSupplyItems.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dgvSupplyItems.Location = new System.Drawing.Point(10, 48);
            this.dgvSupplyItems.Name = "dgvSupplyItems";
            this.dgvSupplyItems.RowHeadersVisible = false;
            this.dgvSupplyItems.RowTemplate.Height = 26;
            this.dgvSupplyItems.Size = new System.Drawing.Size(930, 178);
            this.dgvSupplyItems.TabIndex = 3;
            // 
            // clnSupplyItemId
            // 
            this.clnSupplyItemId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clnSupplyItemId.FillWeight = 200F;
            this.clnSupplyItemId.HeaderText = "Supply Item";
            this.clnSupplyItemId.Name = "clnSupplyItemId";
            this.clnSupplyItemId.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clnSupplyItemId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // clnSupplyQty
            // 
            this.clnSupplyQty.FillWeight = 70F;
            this.clnSupplyQty.HeaderText = "Default Qty";
            this.clnSupplyQty.Name = "clnSupplyQty";
            this.clnSupplyQty.Width = 90;
            // 
            // clnSupplySecQty
            // 
            this.clnSupplySecQty.FillWeight = 70F;
            this.clnSupplySecQty.HeaderText = "Pack Qty";
            this.clnSupplySecQty.Name = "clnSupplySecQty";
            this.clnSupplySecQty.Width = 90;
            // 
            // clnSupplyRate
            // 
            this.clnSupplyRate.FillWeight = 80F;
            this.clnSupplyRate.HeaderText = "Custom Rate";
            this.clnSupplyRate.Name = "clnSupplyRate";
            this.clnSupplyRate.Width = 100;
            // 
            // clnSupplyDiscount
            // 
            this.clnSupplyDiscount.FillWeight = 70F;
            this.clnSupplyDiscount.HeaderText = "Discount %";
            this.clnSupplyDiscount.Name = "clnSupplyDiscount";
            this.clnSupplyDiscount.Width = 90;
            // 
            // clnSupplyCarriage
            // 
            this.clnSupplyCarriage.FillWeight = 70F;
            this.clnSupplyCarriage.HeaderText = "Carriage";
            this.clnSupplyCarriage.Name = "clnSupplyCarriage";
            this.clnSupplyCarriage.Width = 90;
            // 
            // clnSupplyAddLess
            // 
            this.clnSupplyAddLess.FillWeight = 70F;
            this.clnSupplyAddLess.HeaderText = "Add / Less";
            this.clnSupplyAddLess.Name = "clnSupplyAddLess";
            this.clnSupplyAddLess.Width = 90;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.lblCreatedBy);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.lblEditBy);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.btnNew);
            this.panel2.Controls.Add(this.btnDelete);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(6, 524);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(952, 54);
            this.panel2.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label4.Location = new System.Drawing.Point(14, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Created By :";
            // 
            // lblCreatedBy
            // 
            this.lblCreatedBy.AutoSize = true;
            this.lblCreatedBy.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCreatedBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblCreatedBy.Location = new System.Drawing.Point(88, 10);
            this.lblCreatedBy.Name = "lblCreatedBy";
            this.lblCreatedBy.Size = new System.Drawing.Size(11, 13);
            this.lblCreatedBy.TabIndex = 1;
            this.lblCreatedBy.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label5.Location = new System.Drawing.Point(14, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Modified By :";
            // 
            // lblEditBy
            // 
            this.lblEditBy.AutoSize = true;
            this.lblEditBy.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEditBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblEditBy.Location = new System.Drawing.Point(88, 28);
            this.lblEditBy.Name = "lblEditBy";
            this.lblEditBy.Size = new System.Drawing.Size(11, 13);
            this.lblEditBy.TabIndex = 3;
            this.lblEditBy.Text = "-";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(525, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(105, 36);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save [F5]";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNew.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.btnNew.Location = new System.Drawing.Point(638, 8);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(95, 36);
            this.btnNew.TabIndex = 5;
            this.btnNew.Text = "New [F3]";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.Location = new System.Drawing.Point(741, 8);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(95, 36);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnClose.Location = new System.Drawing.Point(844, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(95, 36);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Close [Esc]";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmCustomerInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(984, 680);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(960, 650);
            this.Name = "frmCustomerInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Management";
            this.Load += new System.EventHandler(this.frmCustomerInfo_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmCustomerInfo_KeyDown);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tpList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
            this.pnlListHeader.ResumeLayout(false);
            this.pnlListHeader.PerformLayout();
            this.tpDetails.ResumeLayout(false);
            this.grpCustomerInfo.ResumeLayout(false);
            this.grpCustomerInfo.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpSupplyDefaults.ResumeLayout(false);
            this.grpSupplyDefaults.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSupplyItems)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblHeaderSubtitle;
        private System.Windows.Forms.Label lblHeaderShortcuts;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tpList;
        private System.Windows.Forms.TabPage tpDetails;
        private System.Windows.Forms.Panel pnlListHeader;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnRefreshList;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.DataGridView dgvCustomers;
        private System.Windows.Forms.GroupBox grpCustomerInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAccountCode;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPhone1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPhone2;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtFax;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtNic;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtQualification;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtSMSNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIBAN;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkSMSAlert;
        private System.Windows.Forms.CheckBox chkEmailAlert;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblCreatedBy;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblEditBy;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox cmbCustomerCode;
        private ERP.Controls.ProfileImage profileImage1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.Label lblCustomerCategory;
        private System.Windows.Forms.ComboBox cmbCustomerCategory;
        private System.Windows.Forms.CheckBox chkIsSubAccount;
        private System.Windows.Forms.ComboBox cmbParentCustomer;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnCustomerCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.GroupBox grpSupplyDefaults;
        private System.Windows.Forms.Label lblSupplyCustomerInfo;
        private System.Windows.Forms.Button btnAddSupplyItem;
        private System.Windows.Forms.Button btnDeleteSupplyItem;
        private System.Windows.Forms.DataGridView dgvSupplyItems;
        private System.Windows.Forms.DataGridViewComboBoxColumn clnSupplyItemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnSupplyQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnSupplySecQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnSupplyRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnSupplyDiscount;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnSupplyCarriage;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnSupplyAddLess;
    }
}
