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
            // ── Instantiate every control ─────────────────────────────────
            this.tabControl       = new System.Windows.Forms.TabControl();
            this.tpList           = new System.Windows.Forms.TabPage();
            this.tpDetails        = new System.Windows.Forms.TabPage();
            // List tab
            this.pnlListHeader    = new System.Windows.Forms.Panel();
            this.lblSearch        = new System.Windows.Forms.Label();
            this.txtSearch        = new System.Windows.Forms.TextBox();
            this.btnRefreshList   = new System.Windows.Forms.Button();
            this.btnAddNew        = new System.Windows.Forms.Button();
            this.dgvCustomers     = new System.Windows.Forms.DataGridView();
            // Details tab – banner
            this.pnlDetailBanner  = new System.Windows.Forms.Panel();
            this.lblBannerTitle   = new System.Windows.Forms.Label();
            // Details tab – fields
            this.label2           = new System.Windows.Forms.Label();   // Account Code
            this.txtAccountCode   = new System.Windows.Forms.TextBox();
            this.chkActive        = new System.Windows.Forms.CheckBox();
            this.lblCustomerName  = new System.Windows.Forms.Label();
            this.txtTitle         = new System.Windows.Forms.TextBox();
            this.label21          = new System.Windows.Forms.Label();   // Email
            this.txtEmail         = new System.Windows.Forms.TextBox();
            this.label8           = new System.Windows.Forms.Label();   // Phone 1
            this.txtPhone1        = new System.Windows.Forms.TextBox();
            this.label7           = new System.Windows.Forms.Label();   // Phone 2
            this.txtPhone2        = new System.Windows.Forms.TextBox();
            this.label20          = new System.Windows.Forms.Label();   // Fax
            this.txtFax           = new System.Windows.Forms.TextBox();
            this.label15          = new System.Windows.Forms.Label();   // CNIC
            this.txtNic           = new System.Windows.Forms.TextBox();
            this.label12          = new System.Windows.Forms.Label();   // Address
            this.txtAddress       = new System.Windows.Forms.TextBox();
            this.label13          = new System.Windows.Forms.Label();   // Qualification
            this.txtQualification = new System.Windows.Forms.TextBox();
            this.label11          = new System.Windows.Forms.Label();   // SMS Number
            this.txtSMSNumber     = new System.Windows.Forms.TextBox();
            this.label1           = new System.Windows.Forms.Label();   // IBAN
            this.txtIBAN          = new System.Windows.Forms.TextBox();
            this.groupBox1        = new System.Windows.Forms.GroupBox();
            this.chkSMSAlert      = new System.Windows.Forms.CheckBox();
            this.chkEmailAlert    = new System.Windows.Forms.CheckBox();
            this.label4           = new System.Windows.Forms.Label();   // Created By
            this.lblCreatedBy     = new System.Windows.Forms.Label();
            this.label5           = new System.Windows.Forms.Label();   // Edit By
            this.lblEditBy        = new System.Windows.Forms.Label();
            this.panel2           = new System.Windows.Forms.Panel();
            this.btnNew           = new System.Windows.Forms.Button();
            this.btnSave          = new System.Windows.Forms.Button();
            this.btnDelete        = new System.Windows.Forms.Button();
            this.btnClose         = new System.Windows.Forms.Button();
            this.cmbCustomerCode  = new System.Windows.Forms.ComboBox(); // hidden

            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tpList.SuspendLayout();
            this.tpDetails.SuspendLayout();
            this.pnlListHeader.SuspendLayout();
            this.pnlDetailBanner.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();

            // ── tabControl ───────────────────────────────────────────────────
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(760, 560);
            this.tabControl.TabIndex = 0;
            this.tabControl.Controls.Add(this.tpList);
            this.tabControl.Controls.Add(this.tpDetails);

            // ── tpList ───────────────────────────────────────────────────────
            this.tpList.BackColor = System.Drawing.Color.FromArgb(250, 251, 253);
            this.tpList.Location = new System.Drawing.Point(4, 26);
            this.tpList.Name = "tpList";
            this.tpList.Size = new System.Drawing.Size(752, 530);
            this.tpList.TabIndex = 0;
            this.tpList.Text = "Customer Directory";
            // Fill control FIRST, Top-docked panel SECOND (WinForms dock Z-order rule)
            this.tpList.Controls.Add(this.dgvCustomers);
            this.tpList.Controls.Add(this.pnlListHeader);

            // ── pnlListHeader ────────────────────────────────────────────────
            this.pnlListHeader.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.pnlListHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlListHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlListHeader.Name = "pnlListHeader";
            this.pnlListHeader.Size = new System.Drawing.Size(752, 52);
            this.pnlListHeader.TabIndex = 1;
            this.pnlListHeader.Controls.Add(this.lblSearch);
            this.pnlListHeader.Controls.Add(this.txtSearch);
            this.pnlListHeader.Controls.Add(this.btnRefreshList);
            this.pnlListHeader.Controls.Add(this.btnAddNew);

            // lblSearch
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblSearch.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblSearch.Location = new System.Drawing.Point(12, 17);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search :";

            // txtSearch
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtSearch.Location = new System.Drawing.Point(72, 14);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(250, 22);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);

            // btnRefreshList
            this.btnRefreshList.BackColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.btnRefreshList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshList.FlatAppearance.BorderSize = 0;
            this.btnRefreshList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnRefreshList.ForeColor = System.Drawing.Color.White;
            this.btnRefreshList.Location = new System.Drawing.Point(335, 11);
            this.btnRefreshList.Name = "btnRefreshList";
            this.btnRefreshList.Size = new System.Drawing.Size(90, 30);
            this.btnRefreshList.TabIndex = 2;
            this.btnRefreshList.Text = "Refresh";
            this.btnRefreshList.UseVisualStyleBackColor = false;
            this.btnRefreshList.Click += new System.EventHandler(this.BtnRefreshList_Click);

            // btnAddNew
            this.btnAddNew.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnAddNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddNew.FlatAppearance.BorderSize = 0;
            this.btnAddNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnAddNew.ForeColor = System.Drawing.Color.White;
            this.btnAddNew.Location = new System.Drawing.Point(432, 11);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(120, 30);
            this.btnAddNew.TabIndex = 3;
            this.btnAddNew.Text = "+ New Customer";
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.BtnAddNew_Click);

            // ── dgvCustomers ─────────────────────────────────────────────────
            // Layout grid: col widths sum ≈ 752px (tab page width)
            this.dgvCustomers.AllowUserToAddRows = false;
            this.dgvCustomers.AllowUserToDeleteRows = false;
            this.dgvCustomers.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(239, 246, 255);
            this.dgvCustomers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvCustomers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCustomers.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.dgvCustomers.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvCustomers.ColumnHeadersHeight = 30;
            this.dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCustomers.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.dgvCustomers.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(96, 165, 250);
            this.dgvCustomers.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgvCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomers.EnableHeadersVisualStyles = false;
            this.dgvCustomers.GridColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.dgvCustomers.Location = new System.Drawing.Point(0, 0);
            this.dgvCustomers.MultiSelect = false;
            this.dgvCustomers.Name = "dgvCustomers";
            this.dgvCustomers.ReadOnly = true;
            this.dgvCustomers.RowHeadersVisible = false;
            this.dgvCustomers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomers.Size = new System.Drawing.Size(752, 530);
            this.dgvCustomers.TabIndex = 0;
            this.dgvCustomers.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvCustomers_CellDoubleClick);
            this.dgvCustomers.Columns.Add("Account", "Account Code");
            this.dgvCustomers.Columns.Add("Title",   "Customer Name");
            this.dgvCustomers.Columns.Add("Phone1",  "Phone # 1");
            this.dgvCustomers.Columns.Add("Email",   "Email");
            this.dgvCustomers.Columns.Add("Address", "Address");
            this.dgvCustomers.Columns.Add("Active",  "Active");
            this.dgvCustomers.Columns["Account"].Width = 95;
            this.dgvCustomers.Columns["Title"].Width   = 195;
            this.dgvCustomers.Columns["Phone1"].Width  = 105;
            this.dgvCustomers.Columns["Email"].Width   = 160;
            this.dgvCustomers.Columns["Address"].Width = 175;
            this.dgvCustomers.Columns["Active"].Width  = 55;

            // ── tpDetails ────────────────────────────────────────────────────
            this.tpDetails.BackColor = System.Drawing.Color.FromArgb(250, 251, 253);
            this.tpDetails.Location = new System.Drawing.Point(4, 26);
            this.tpDetails.Name = "tpDetails";
            this.tpDetails.Size = new System.Drawing.Size(752, 530);
            this.tpDetails.TabIndex = 1;
            this.tpDetails.Text = "Customer Details";
            // Absolute-positioned controls first, then DockStyle.Top banner last
            this.tpDetails.Controls.Add(this.cmbCustomerCode);
            this.tpDetails.Controls.Add(this.label2);
            this.tpDetails.Controls.Add(this.txtAccountCode);
            this.tpDetails.Controls.Add(this.chkActive);
            this.tpDetails.Controls.Add(this.lblCustomerName);
            this.tpDetails.Controls.Add(this.txtTitle);
            this.tpDetails.Controls.Add(this.label21);
            this.tpDetails.Controls.Add(this.txtEmail);
            this.tpDetails.Controls.Add(this.label8);
            this.tpDetails.Controls.Add(this.txtPhone1);
            this.tpDetails.Controls.Add(this.label7);
            this.tpDetails.Controls.Add(this.txtPhone2);
            this.tpDetails.Controls.Add(this.label20);
            this.tpDetails.Controls.Add(this.txtFax);
            this.tpDetails.Controls.Add(this.label15);
            this.tpDetails.Controls.Add(this.txtNic);
            this.tpDetails.Controls.Add(this.label12);
            this.tpDetails.Controls.Add(this.txtAddress);
            this.tpDetails.Controls.Add(this.label13);
            this.tpDetails.Controls.Add(this.txtQualification);
            this.tpDetails.Controls.Add(this.label11);
            this.tpDetails.Controls.Add(this.txtSMSNumber);
            this.tpDetails.Controls.Add(this.label1);
            this.tpDetails.Controls.Add(this.txtIBAN);
            this.tpDetails.Controls.Add(this.groupBox1);
            this.tpDetails.Controls.Add(this.label4);
            this.tpDetails.Controls.Add(this.lblCreatedBy);
            this.tpDetails.Controls.Add(this.label5);
            this.tpDetails.Controls.Add(this.lblEditBy);
            this.tpDetails.Controls.Add(this.panel2);
            this.tpDetails.Controls.Add(this.pnlDetailBanner); // DockStyle.Top — added LAST

            // ── pnlDetailBanner (blue header) ────────────────────────────────
            this.pnlDetailBanner.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.pnlDetailBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDetailBanner.Location = new System.Drawing.Point(0, 0);
            this.pnlDetailBanner.Name = "pnlDetailBanner";
            this.pnlDetailBanner.Size = new System.Drawing.Size(752, 44);
            this.pnlDetailBanner.TabIndex = 0;
            this.pnlDetailBanner.Controls.Add(this.lblBannerTitle);

            this.lblBannerTitle.AutoSize = true;
            this.lblBannerTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblBannerTitle.ForeColor = System.Drawing.Color.White;
            this.lblBannerTitle.Location = new System.Drawing.Point(14, 12);
            this.lblBannerTitle.Name = "lblBannerTitle";
            this.lblBannerTitle.TabIndex = 0;
            this.lblBannerTitle.Text = "Customer Information";

            // ════════════════════════════════════════════════════════════════
            //  Detail form controls
            //  Layout grid (all right-edges align at x=550):
            //    Left  label : x=12,  w=128  (right-aligned text)
            //    Left  input : x=148, w=162  → right=310  (narrow)
            //                         w=402  → right=550  (wide)
            //    Right label : x=322, w=62   (right-aligned text)
            //    Right input : x=388, w=162  → right=550
            //  Row Y starts at 56 (below 44 px banner), step = 30 px
            // ════════════════════════════════════════════════════════════════

            // ── Row 1  y=56 : Account Code | Active ─────────────────────────
            this.label2.AutoSize = false;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.label2.Location = new System.Drawing.Point(12, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 20);
            this.label2.TabIndex = 100;
            this.label2.Text = "Account Code :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtAccountCode.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.txtAccountCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAccountCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtAccountCode.Location = new System.Drawing.Point(148, 56);
            this.txtAccountCode.Name = "txtAccountCode";
            this.txtAccountCode.ReadOnly = true;
            this.txtAccountCode.Size = new System.Drawing.Size(162, 22);
            this.txtAccountCode.TabIndex = 101;
            this.txtAccountCode.TabStop = false;

            this.chkActive.AutoSize = false;
            this.chkActive.Checked = true;
            this.chkActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
            this.chkActive.ForeColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.chkActive.Location = new System.Drawing.Point(324, 58);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(80, 20);
            this.chkActive.TabIndex = 102;
            this.chkActive.Text = "Active";

            // ── Row 2  y=86 : Customer Name (wide) ──────────────────────────
            this.lblCustomerName.AutoSize = false;
            this.lblCustomerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.lblCustomerName.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblCustomerName.Location = new System.Drawing.Point(12, 87);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(128, 20);
            this.lblCustomerName.TabIndex = 103;
            this.lblCustomerName.Text = "Customer Name :";
            this.lblCustomerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtTitle.BackColor = System.Drawing.Color.White;
            this.txtTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtTitle.Location = new System.Drawing.Point(148, 86);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(402, 22);
            this.txtTitle.TabIndex = 1;

            // ── Row 3  y=116 : Email (wide) ──────────────────────────────────
            this.label21.AutoSize = false;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.label21.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.label21.Location = new System.Drawing.Point(12, 117);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(128, 20);
            this.label21.TabIndex = 110;
            this.label21.Text = "Email :";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtEmail.BackColor = System.Drawing.Color.White;
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtEmail.Location = new System.Drawing.Point(148, 116);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(402, 22);
            this.txtEmail.TabIndex = 2;

            // ── Row 4  y=146 : Phone 1 | Phone 2 ────────────────────────────
            this.label8.AutoSize = false;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.label8.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.label8.Location = new System.Drawing.Point(12, 147);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(128, 20);
            this.label8.TabIndex = 111;
            this.label8.Text = "Phone # 1 :";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtPhone1.BackColor = System.Drawing.Color.White;
            this.txtPhone1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPhone1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtPhone1.Location = new System.Drawing.Point(148, 146);
            this.txtPhone1.Name = "txtPhone1";
            this.txtPhone1.Size = new System.Drawing.Size(162, 22);
            this.txtPhone1.TabIndex = 3;

            this.label7.AutoSize = false;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.label7.Location = new System.Drawing.Point(322, 147);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 20);
            this.label7.TabIndex = 112;
            this.label7.Text = "Phone # 2 :";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtPhone2.BackColor = System.Drawing.Color.White;
            this.txtPhone2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPhone2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtPhone2.Location = new System.Drawing.Point(388, 146);
            this.txtPhone2.Name = "txtPhone2";
            this.txtPhone2.Size = new System.Drawing.Size(162, 22);
            this.txtPhone2.TabIndex = 4;

            // ── Row 5  y=176 : Fax | CNIC ───────────────────────────────────
            this.label20.AutoSize = false;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.label20.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.label20.Location = new System.Drawing.Point(12, 177);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(128, 20);
            this.label20.TabIndex = 113;
            this.label20.Text = "Fax :";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtFax.BackColor = System.Drawing.Color.White;
            this.txtFax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFax.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtFax.Location = new System.Drawing.Point(148, 176);
            this.txtFax.Name = "txtFax";
            this.txtFax.Size = new System.Drawing.Size(162, 22);
            this.txtFax.TabIndex = 5;

            this.label15.AutoSize = false;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.label15.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.label15.Location = new System.Drawing.Point(322, 177);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(62, 20);
            this.label15.TabIndex = 114;
            this.label15.Text = "CNIC :";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtNic.BackColor = System.Drawing.Color.White;
            this.txtNic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNic.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtNic.Location = new System.Drawing.Point(388, 176);
            this.txtNic.Name = "txtNic";
            this.txtNic.Size = new System.Drawing.Size(162, 22);
            this.txtNic.TabIndex = 6;

            // ── Row 6  y=206 : Address (wide) ───────────────────────────────
            this.label12.AutoSize = false;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.label12.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.label12.Location = new System.Drawing.Point(12, 207);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(128, 20);
            this.label12.TabIndex = 115;
            this.label12.Text = "Address :";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtAddress.BackColor = System.Drawing.Color.White;
            this.txtAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtAddress.Location = new System.Drawing.Point(148, 206);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(402, 22);
            this.txtAddress.TabIndex = 7;

            // ── Row 7  y=236 : Qualification (wide) ─────────────────────────
            this.label13.AutoSize = false;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.label13.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.label13.Location = new System.Drawing.Point(12, 237);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(128, 20);
            this.label13.TabIndex = 116;
            this.label13.Text = "Qualification :";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtQualification.BackColor = System.Drawing.Color.White;
            this.txtQualification.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtQualification.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtQualification.Location = new System.Drawing.Point(148, 236);
            this.txtQualification.Name = "txtQualification";
            this.txtQualification.Size = new System.Drawing.Size(402, 22);
            this.txtQualification.TabIndex = 8;

            // ── Row 8  y=266 : SMS Number | IBAN ────────────────────────────
            this.label11.AutoSize = false;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.label11.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.label11.Location = new System.Drawing.Point(12, 267);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(128, 20);
            this.label11.TabIndex = 117;
            this.label11.Text = "SMS Number :";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtSMSNumber.BackColor = System.Drawing.Color.White;
            this.txtSMSNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSMSNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtSMSNumber.Location = new System.Drawing.Point(148, 266);
            this.txtSMSNumber.Name = "txtSMSNumber";
            this.txtSMSNumber.Size = new System.Drawing.Size(162, 22);
            this.txtSMSNumber.TabIndex = 9;

            this.label1.AutoSize = false;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.label1.Location = new System.Drawing.Point(322, 267);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 20);
            this.label1.TabIndex = 118;
            this.label1.Text = "IBAN :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtIBAN.BackColor = System.Drawing.Color.White;
            this.txtIBAN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIBAN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.txtIBAN.Location = new System.Drawing.Point(388, 266);
            this.txtIBAN.Name = "txtIBAN";
            this.txtIBAN.Size = new System.Drawing.Size(162, 22);
            this.txtIBAN.TabIndex = 10;

            // ── Alert groupbox  y=300 ────────────────────────────────────────
            this.groupBox1.Controls.Add(this.chkSMSAlert);
            this.groupBox1.Controls.Add(this.chkEmailAlert);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.groupBox1.Location = new System.Drawing.Point(12, 300);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 44);
            this.groupBox1.TabIndex = 120;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Alerts";

            this.chkSMSAlert.AutoSize = true;
            this.chkSMSAlert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.chkSMSAlert.Location = new System.Drawing.Point(14, 18);
            this.chkSMSAlert.Name = "chkSMSAlert";
            this.chkSMSAlert.TabIndex = 11;
            this.chkSMSAlert.Text = "SMS";

            this.chkEmailAlert.AutoSize = true;
            this.chkEmailAlert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.chkEmailAlert.Location = new System.Drawing.Point(88, 18);
            this.chkEmailAlert.Name = "chkEmailAlert";
            this.chkEmailAlert.TabIndex = 12;
            this.chkEmailAlert.Text = "Email";

            // ── Audit info  y=354 / y=376 ────────────────────────────────────
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.label4.Location = new System.Drawing.Point(12, 354);
            this.label4.Name = "label4";
            this.label4.TabIndex = 130;
            this.label4.Text = "Created By :";

            this.lblCreatedBy.AutoSize = true;
            this.lblCreatedBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblCreatedBy.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblCreatedBy.Location = new System.Drawing.Point(148, 354);
            this.lblCreatedBy.Name = "lblCreatedBy";
            this.lblCreatedBy.TabIndex = 131;
            this.lblCreatedBy.Text = "-";

            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.label5.Location = new System.Drawing.Point(12, 376);
            this.label5.Name = "label5";
            this.label5.TabIndex = 132;
            this.label5.Text = "Edit By :";

            this.lblEditBy.AutoSize = true;
            this.lblEditBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblEditBy.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblEditBy.Location = new System.Drawing.Point(148, 376);
            this.lblEditBy.Name = "lblEditBy";
            this.lblEditBy.TabIndex = 133;
            this.lblEditBy.Text = "-";

            // ── Button panel  y=412 ──────────────────────────────────────────
            this.panel2.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.panel2.Controls.Add(this.btnNew);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.btnDelete);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Location = new System.Drawing.Point(12, 412);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(384, 38);
            this.panel2.TabIndex = 140;

            this.btnNew.BackColor = System.Drawing.Color.FromArgb(59, 130, 246);
            this.btnNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnNew.ForeColor = System.Drawing.Color.White;
            this.btnNew.Location = new System.Drawing.Point(3, 4);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(88, 30);
            this.btnNew.TabIndex = 13;
            this.btnNew.Text = "&New";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            this.btnSave.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(97, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(88, 30);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(191, 4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(88, 30);
            this.btnDelete.TabIndex = 15;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);

            this.btnClose.BackColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(285, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 30);
            this.btnClose.TabIndex = 16;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnCancel_Click);

            // ── cmbCustomerCode (hidden – used internally for selection) ─────
            this.cmbCustomerCode.FormattingEnabled = true;
            this.cmbCustomerCode.Location = new System.Drawing.Point(0, -50);
            this.cmbCustomerCode.Name = "cmbCustomerCode";
            this.cmbCustomerCode.Size = new System.Drawing.Size(200, 21);
            this.cmbCustomerCode.TabIndex = 999;
            this.cmbCustomerCode.Tag = "Lock";
            this.cmbCustomerCode.Visible = false;
            this.cmbCustomerCode.SelectedIndexChanged += new System.EventHandler(this.cmbCustomerCode_SelectedIndexChanged);

            // ── Form ─────────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 560);
            this.Controls.Add(this.tabControl);
            this.KeyPreview = true;
            this.Name = "frmCustomerInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Customer Info";
            this.Load += new System.EventHandler(this.frmCustomerInfo_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmCustomerInfo_KeyDown);

            // ── ResumeLayout ─────────────────────────────────────────────────
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.pnlDetailBanner.ResumeLayout(false);
            this.pnlDetailBanner.PerformLayout();
            this.pnlListHeader.ResumeLayout(false);
            this.pnlListHeader.PerformLayout();
            this.tpList.ResumeLayout(false);
            this.tpDetails.ResumeLayout(false);
            this.tpDetails.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tpList;
        private System.Windows.Forms.TabPage tpDetails;
        private System.Windows.Forms.Panel pnlListHeader;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnRefreshList;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.DataGridView dgvCustomers;
        private System.Windows.Forms.Panel pnlDetailBanner;
        private System.Windows.Forms.Label lblBannerTitle;
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
    }
}