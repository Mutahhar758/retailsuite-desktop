namespace ERP
{
    partial class frmItemDetail
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.picHeaderLogo = new System.Windows.Forms.PictureBox();
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.lblHeaderSubtitle = new System.Windows.Forms.Label();
            this.pnlTopStats = new System.Windows.Forms.Panel();
            this.lblTotalProductsBadge = new System.Windows.Forms.Label();
            this.tbItemMain = new System.Windows.Forms.TabControl();
            this.tbDetail = new System.Windows.Forms.TabPage();
            this.pnlDetailScroll = new System.Windows.Forms.Panel();
            this.grpBasic = new System.Windows.Forms.GroupBox();
            this.lblBarcode = new System.Windows.Forms.Label();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.lblItemKey = new System.Windows.Forms.Label();
            this.txtItemKey = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbItemCatagory = new System.Windows.Forms.ComboBox();
            this.lblItemType = new System.Windows.Forms.Label();
            this.cmbItemType = new System.Windows.Forms.ComboBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.grpPricing = new System.Windows.Forms.GroupBox();
            this.lblPriRate = new System.Windows.Forms.Label();
            this.txtPriRate = new System.Windows.Forms.TextBox();
            this.lblSecRate = new System.Windows.Forms.Label();
            this.txtSecRate = new System.Windows.Forms.TextBox();
            this.lblMinSalePrice = new System.Windows.Forms.Label();
            this.txtMinSalePrice = new System.Windows.Forms.TextBox();
            this.btnCategoryRates = new System.Windows.Forms.Button();
            this.grpUnits = new System.Windows.Forms.GroupBox();
            this.lblPriUnit = new System.Windows.Forms.Label();
            this.cmbPriUnit = new System.Windows.Forms.ComboBox();
            this.lblSecUnit = new System.Windows.Forms.Label();
            this.cmbSecUnit = new System.Windows.Forms.ComboBox();
            this.lblDefUnit = new System.Windows.Forms.Label();
            this.cmbDefUnit = new System.Windows.Forms.ComboBox();
            this.lblQtyInPack = new System.Windows.Forms.Label();
            this.txtQtyInPack = new System.Windows.Forms.TextBox();
            this.btnSubUnits = new System.Windows.Forms.Button();
            this.grpStock = new System.Windows.Forms.GroupBox();
            this.lblOpnStock = new System.Windows.Forms.Label();
            this.txtOpnStock = new System.Windows.Forms.TextBox();
            this.lblOpnRate = new System.Windows.Forms.Label();
            this.txtOpnRate = new System.Windows.Forms.TextBox();
            this.chkLowStockAlert = new System.Windows.Forms.CheckBox();
            this.lblLowStockQty = new System.Windows.Forms.Label();
            this.txtLowStockQty = new System.Windows.Forms.TextBox();
            this.chkAlert = new System.Windows.Forms.CheckBox();
            this.pnlMobileShop = new System.Windows.Forms.GroupBox();
            this.lblBrand = new System.Windows.Forms.Label();
            this.cmbBrand = new System.Windows.Forms.ComboBox();
            this.lblModel = new System.Windows.Forms.Label();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.lblStorage = new System.Windows.Forms.Label();
            this.txtStorage = new System.Windows.Forms.TextBox();
            this.lblRam = new System.Windows.Forms.Label();
            this.txtRam = new System.Windows.Forms.TextBox();
            this.lblColor = new System.Windows.Forms.Label();
            this.txtColor = new System.Windows.Forms.TextBox();
            this.chkRequireImei = new System.Windows.Forms.CheckBox();
            this.pnlRightImage = new System.Windows.Forms.Panel();
            this.profileImage1 = new ERP.Controls.ProfileImage();
            this.grpImageSpecs = new System.Windows.Forms.GroupBox();
            this.lblImageSpecs = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblDetailStatus = new System.Windows.Forms.Label();
            this.tbQuery = new System.Windows.Forms.TabPage();
            this.pnlQueryFilter = new System.Windows.Forms.Panel();
            this.lblQuerySearch = new System.Windows.Forms.Label();
            this.txtQuerySearch = new System.Windows.Forms.TextBox();
            this.lblQueryCategory = new System.Windows.Forms.Label();
            this.cmbQueryCategory = new System.Windows.Forms.ComboBox();
            this.lblQueryType = new System.Windows.Forms.Label();
            this.cmbQueryType = new System.Windows.Forms.ComboBox();
            this.btnQueryFind = new System.Windows.Forms.Button();
            this.btnQueryReset = new System.Windows.Forms.Button();
            this.btnAddNewFromSearch = new System.Windows.Forms.Button();
            this.lblQueryCount = new System.Windows.Forms.Label();
            this.dgvQuery = new System.Windows.Forms.DataGridView();
            this.clnQId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnQBarcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnQCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnQTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnQKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnQItemType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnQPriRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnQSecRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnQPriUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnQSecUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnQMinSalePrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnQOpnStock = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHeaderLogo)).BeginInit();
            this.pnlTopStats.SuspendLayout();
            this.tbItemMain.SuspendLayout();
            this.tbDetail.SuspendLayout();
            this.pnlDetailScroll.SuspendLayout();
            this.grpBasic.SuspendLayout();
            this.grpPricing.SuspendLayout();
            this.grpUnits.SuspendLayout();
            this.grpStock.SuspendLayout();
            this.pnlMobileShop.SuspendLayout();
            this.pnlRightImage.SuspendLayout();
            this.grpImageSpecs.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tbQuery.SuspendLayout();
            this.pnlQueryFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuery)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.picHeaderLogo);
            this.pnlHeader.Controls.Add(this.lblHeaderTitle);
            this.pnlHeader.Controls.Add(this.lblHeaderSubtitle);
            this.pnlHeader.Controls.Add(this.pnlTopStats);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1200, 52);
            this.pnlHeader.TabIndex = 0;
            // 
            // picHeaderLogo
            // 
            this.picHeaderLogo.Image = global::ERP.Properties.Resources.logoPNG;
            this.picHeaderLogo.Location = new System.Drawing.Point(12, 9);
            this.picHeaderLogo.Name = "picHeaderLogo";
            this.picHeaderLogo.Size = new System.Drawing.Size(34, 34);
            this.picHeaderLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picHeaderLogo.TabIndex = 0;
            this.picHeaderLogo.TabStop = false;
            // 
            // lblHeaderTitle
            // 
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblHeaderTitle.Location = new System.Drawing.Point(52, 6);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(193, 25);
            this.lblHeaderTitle.TabIndex = 1;
            this.lblHeaderTitle.Text = "Product Management";
            // 
            // lblHeaderSubtitle
            // 
            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblHeaderSubtitle.Location = new System.Drawing.Point(53, 30);
            this.lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            this.lblHeaderSubtitle.Size = new System.Drawing.Size(325, 13);
            this.lblHeaderSubtitle.TabIndex = 2;
            this.lblHeaderSubtitle.Text = "Manage inventory items, rates, units, stock alerts and images";
            // 
            // pnlTopStats
            // 
            this.pnlTopStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTopStats.Controls.Add(this.lblTotalProductsBadge);
            this.pnlTopStats.Location = new System.Drawing.Point(980, 8);
            this.pnlTopStats.Name = "pnlTopStats";
            this.pnlTopStats.Size = new System.Drawing.Size(208, 36);
            this.pnlTopStats.TabIndex = 3;
            // 
            // lblTotalProductsBadge
            // 
            this.lblTotalProductsBadge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            this.lblTotalProductsBadge.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalProductsBadge.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblTotalProductsBadge.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalProductsBadge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.lblTotalProductsBadge.Location = new System.Drawing.Point(48, 0);
            this.lblTotalProductsBadge.Name = "lblTotalProductsBadge";
            this.lblTotalProductsBadge.Size = new System.Drawing.Size(160, 36);
            this.lblTotalProductsBadge.TabIndex = 0;
            this.lblTotalProductsBadge.Text = "Total Products: 0";
            this.lblTotalProductsBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbItemMain
            // 
            this.tbItemMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbItemMain.Controls.Add(this.tbDetail);
            this.tbItemMain.Controls.Add(this.tbQuery);
            this.tbItemMain.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbItemMain.Location = new System.Drawing.Point(8, 58);
            this.tbItemMain.Name = "tbItemMain";
            this.tbItemMain.SelectedIndex = 0;
            this.tbItemMain.Size = new System.Drawing.Size(1184, 594);
            this.tbItemMain.TabIndex = 1;
            // 
            // tbDetail
            // 
            this.tbDetail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.tbDetail.Controls.Add(this.pnlDetailScroll);
            this.tbDetail.Controls.Add(this.pnlRightImage);
            this.tbDetail.Controls.Add(this.panel1);
            this.tbDetail.Location = new System.Drawing.Point(4, 26);
            this.tbDetail.Name = "tbDetail";
            this.tbDetail.Padding = new System.Windows.Forms.Padding(6);
            this.tbDetail.Size = new System.Drawing.Size(1176, 564);
            this.tbDetail.TabIndex = 0;
            this.tbDetail.Text = "Product Detail";
            // 
            // pnlDetailScroll
            // 
            this.pnlDetailScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDetailScroll.AutoScroll = true;
            this.pnlDetailScroll.BackColor = System.Drawing.Color.White;
            this.pnlDetailScroll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDetailScroll.Controls.Add(this.grpBasic);
            this.pnlDetailScroll.Controls.Add(this.grpPricing);
            this.pnlDetailScroll.Controls.Add(this.grpUnits);
            this.pnlDetailScroll.Controls.Add(this.grpStock);
            this.pnlDetailScroll.Controls.Add(this.pnlMobileShop);
            this.pnlDetailScroll.Location = new System.Drawing.Point(6, 6);
            this.pnlDetailScroll.Name = "pnlDetailScroll";
            this.pnlDetailScroll.Size = new System.Drawing.Size(978, 508);
            this.pnlDetailScroll.TabIndex = 0;
            // 
            // grpBasic
            // 
            this.grpBasic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBasic.Controls.Add(this.lblBarcode);
            this.grpBasic.Controls.Add(this.txtBarcode);
            this.grpBasic.Controls.Add(this.lblItemKey);
            this.grpBasic.Controls.Add(this.txtItemKey);
            this.grpBasic.Controls.Add(this.label11);
            this.grpBasic.Controls.Add(this.cmbItemCatagory);
            this.grpBasic.Controls.Add(this.lblItemType);
            this.grpBasic.Controls.Add(this.cmbItemType);
            this.grpBasic.Controls.Add(this.lblTitle);
            this.grpBasic.Controls.Add(this.txtTitle);
            this.grpBasic.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBasic.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.grpBasic.Location = new System.Drawing.Point(10, 8);
            this.grpBasic.Name = "grpBasic";
            this.grpBasic.Size = new System.Drawing.Size(952, 108);
            this.grpBasic.TabIndex = 0;
            this.grpBasic.TabStop = false;
            this.grpBasic.Text = "Basic Product Information";
            // 
            // lblBarcode
            // 
            this.lblBarcode.AutoSize = true;
            this.lblBarcode.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBarcode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblBarcode.Location = new System.Drawing.Point(10, 26);
            this.lblBarcode.Name = "lblBarcode";
            this.lblBarcode.Size = new System.Drawing.Size(53, 15);
            this.lblBarcode.TabIndex = 0;
            this.lblBarcode.Text = "Barcode:";
            // 
            // txtBarcode
            // 
            this.txtBarcode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBarcode.Location = new System.Drawing.Point(68, 23);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(150, 23);
            this.txtBarcode.TabIndex = 1;
            this.txtBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBarcode_KeyDown);
            // 
            // lblItemKey
            // 
            this.lblItemKey.AutoSize = true;
            this.lblItemKey.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemKey.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblItemKey.Location = new System.Drawing.Point(232, 26);
            this.lblItemKey.Name = "lblItemKey";
            this.lblItemKey.Size = new System.Drawing.Size(56, 15);
            this.lblItemKey.TabIndex = 2;
            this.lblItemKey.Text = "Item Key:";
            // 
            // txtItemKey
            // 
            this.txtItemKey.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtItemKey.Location = new System.Drawing.Point(292, 23);
            this.txtItemKey.Name = "txtItemKey";
            this.txtItemKey.Size = new System.Drawing.Size(80, 23);
            this.txtItemKey.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.label11.Location = new System.Drawing.Point(388, 26);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 15);
            this.label11.TabIndex = 4;
            this.label11.Text = "Category *:";
            // 
            // cmbItemCatagory
            // 
            this.cmbItemCatagory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbItemCatagory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbItemCatagory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItemCatagory.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbItemCatagory.FormattingEnabled = true;
            this.cmbItemCatagory.Location = new System.Drawing.Point(458, 23);
            this.cmbItemCatagory.Name = "cmbItemCatagory";
            this.cmbItemCatagory.Size = new System.Drawing.Size(220, 23);
            this.cmbItemCatagory.TabIndex = 5;
            this.cmbItemCatagory.SelectedIndexChanged += new System.EventHandler(this.cmbItemCatagory_SelectedIndexChanged);
            // 
            // lblItemType
            // 
            this.lblItemType.AutoSize = true;
            this.lblItemType.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblItemType.Location = new System.Drawing.Point(695, 26);
            this.lblItemType.Name = "lblItemType";
            this.lblItemType.Size = new System.Drawing.Size(35, 15);
            this.lblItemType.TabIndex = 6;
            this.lblItemType.Text = "Type:";
            // 
            // cmbItemType
            // 
            this.cmbItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItemType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbItemType.FormattingEnabled = true;
            this.cmbItemType.Location = new System.Drawing.Point(735, 23);
            this.cmbItemType.Name = "cmbItemType";
            this.cmbItemType.Size = new System.Drawing.Size(120, 23);
            this.cmbItemType.TabIndex = 7;
            this.cmbItemType.SelectedIndexChanged += new System.EventHandler(this.cmbItemType_SelectedIndexChanged);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblTitle.Location = new System.Drawing.Point(10, 68);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(95, 15);
            this.lblTitle.TabIndex = 8;
            this.lblTitle.Text = "Product Name *:";
            // 
            // txtTitle
            // 
            this.txtTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTitle.Location = new System.Drawing.Point(112, 64);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(820, 24);
            this.txtTitle.TabIndex = 9;
            this.txtTitle.TextChanged += new System.EventHandler(this.Field_TextChanged);
            // 
            // grpPricing
            // 
            this.grpPricing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPricing.Controls.Add(this.lblPriRate);
            this.grpPricing.Controls.Add(this.txtPriRate);
            this.grpPricing.Controls.Add(this.lblSecRate);
            this.grpPricing.Controls.Add(this.txtSecRate);
            this.grpPricing.Controls.Add(this.lblMinSalePrice);
            this.grpPricing.Controls.Add(this.txtMinSalePrice);
            this.grpPricing.Controls.Add(this.btnCategoryRates);
            this.grpPricing.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPricing.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.grpPricing.Location = new System.Drawing.Point(10, 122);
            this.grpPricing.Name = "grpPricing";
            this.grpPricing.Size = new System.Drawing.Size(952, 70);
            this.grpPricing.TabIndex = 1;
            this.grpPricing.TabStop = false;
            this.grpPricing.Text = "Pricing & Rates";
            // 
            // lblPriRate
            // 
            this.lblPriRate.AutoSize = true;
            this.lblPriRate.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPriRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblPriRate.Location = new System.Drawing.Point(10, 28);
            this.lblPriRate.Name = "lblPriRate";
            this.lblPriRate.Size = new System.Drawing.Size(76, 15);
            this.lblPriRate.TabIndex = 0;
            this.lblPriRate.Text = "Primary Rate:";
            // 
            // txtPriRate
            // 
            this.txtPriRate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPriRate.Location = new System.Drawing.Point(92, 25);
            this.txtPriRate.Name = "txtPriRate";
            this.txtPriRate.Size = new System.Drawing.Size(110, 23);
            this.txtPriRate.TabIndex = 1;
            this.txtPriRate.Text = "0";
            this.txtPriRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPriRate.TextChanged += new System.EventHandler(this.Field_TextChanged);
            this.txtPriRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // lblSecRate
            // 
            this.lblSecRate.AutoSize = true;
            this.lblSecRate.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSecRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblSecRate.Location = new System.Drawing.Point(220, 28);
            this.lblSecRate.Name = "lblSecRate";
            this.lblSecRate.Size = new System.Drawing.Size(90, 15);
            this.lblSecRate.TabIndex = 2;
            this.lblSecRate.Text = "Secondary Rate:";
            // 
            // txtSecRate
            // 
            this.txtSecRate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSecRate.Location = new System.Drawing.Point(314, 25);
            this.txtSecRate.Name = "txtSecRate";
            this.txtSecRate.Size = new System.Drawing.Size(110, 23);
            this.txtSecRate.TabIndex = 3;
            this.txtSecRate.Text = "0";
            this.txtSecRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSecRate.TextChanged += new System.EventHandler(this.Field_TextChanged);
            this.txtSecRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // lblMinSalePrice
            // 
            this.lblMinSalePrice.AutoSize = true;
            this.lblMinSalePrice.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMinSalePrice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblMinSalePrice.Location = new System.Drawing.Point(448, 28);
            this.lblMinSalePrice.Name = "lblMinSalePrice";
            this.lblMinSalePrice.Size = new System.Drawing.Size(83, 15);
            this.lblMinSalePrice.TabIndex = 4;
            this.lblMinSalePrice.Text = "Min Sale Price:";
            // 
            // txtMinSalePrice
            // 
            this.txtMinSalePrice.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMinSalePrice.Location = new System.Drawing.Point(536, 25);
            this.txtMinSalePrice.Name = "txtMinSalePrice";
            this.txtMinSalePrice.Size = new System.Drawing.Size(110, 23);
            this.txtMinSalePrice.TabIndex = 5;
            this.txtMinSalePrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMinSalePrice.TextChanged += new System.EventHandler(this.Field_TextChanged);
            this.txtMinSalePrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // btnCategoryRates
            // 
            this.btnCategoryRates.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnCategoryRates.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCategoryRates.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnCategoryRates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCategoryRates.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCategoryRates.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnCategoryRates.Location = new System.Drawing.Point(670, 23);
            this.btnCategoryRates.Name = "btnCategoryRates";
            this.btnCategoryRates.Size = new System.Drawing.Size(140, 27);
            this.btnCategoryRates.TabIndex = 6;
            this.btnCategoryRates.Text = "Category Rates...";
            this.btnCategoryRates.UseVisualStyleBackColor = false;
            this.btnCategoryRates.Click += new System.EventHandler(this.btnCategoryRates_Click);
            // 
            // grpUnits
            // 
            this.grpUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpUnits.Controls.Add(this.lblPriUnit);
            this.grpUnits.Controls.Add(this.cmbPriUnit);
            this.grpUnits.Controls.Add(this.lblSecUnit);
            this.grpUnits.Controls.Add(this.cmbSecUnit);
            this.grpUnits.Controls.Add(this.lblDefUnit);
            this.grpUnits.Controls.Add(this.cmbDefUnit);
            this.grpUnits.Controls.Add(this.lblQtyInPack);
            this.grpUnits.Controls.Add(this.txtQtyInPack);
            this.grpUnits.Controls.Add(this.btnSubUnits);
            this.grpUnits.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpUnits.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.grpUnits.Location = new System.Drawing.Point(10, 198);
            this.grpUnits.Name = "grpUnits";
            this.grpUnits.Size = new System.Drawing.Size(952, 70);
            this.grpUnits.TabIndex = 2;
            this.grpUnits.TabStop = false;
            this.grpUnits.Text = "Units & Packaging";
            // 
            // lblPriUnit
            // 
            this.lblPriUnit.AutoSize = true;
            this.lblPriUnit.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPriUnit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblPriUnit.Location = new System.Drawing.Point(10, 28);
            this.lblPriUnit.Name = "lblPriUnit";
            this.lblPriUnit.Size = new System.Drawing.Size(76, 15);
            this.lblPriUnit.TabIndex = 0;
            this.lblPriUnit.Text = "Primary Unit:";
            // 
            // cmbPriUnit
            // 
            this.cmbPriUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPriUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPriUnit.FormattingEnabled = true;
            this.cmbPriUnit.Location = new System.Drawing.Point(92, 24);
            this.cmbPriUnit.Name = "cmbPriUnit";
            this.cmbPriUnit.Size = new System.Drawing.Size(110, 23);
            this.cmbPriUnit.TabIndex = 1;
            this.cmbPriUnit.SelectedIndexChanged += new System.EventHandler(this.Field_TextChanged);
            // 
            // lblSecUnit
            // 
            this.lblSecUnit.AutoSize = true;
            this.lblSecUnit.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSecUnit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblSecUnit.Location = new System.Drawing.Point(220, 28);
            this.lblSecUnit.Name = "lblSecUnit";
            this.lblSecUnit.Size = new System.Drawing.Size(56, 15);
            this.lblSecUnit.TabIndex = 2;
            this.lblSecUnit.Text = "Sec Unit:";
            // 
            // cmbSecUnit
            // 
            this.cmbSecUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSecUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSecUnit.FormattingEnabled = true;
            this.cmbSecUnit.Location = new System.Drawing.Point(280, 24);
            this.cmbSecUnit.Name = "cmbSecUnit";
            this.cmbSecUnit.Size = new System.Drawing.Size(110, 23);
            this.cmbSecUnit.TabIndex = 3;
            this.cmbSecUnit.SelectedIndexChanged += new System.EventHandler(this.Field_TextChanged);
            // 
            // lblDefUnit
            // 
            this.lblDefUnit.AutoSize = true;
            this.lblDefUnit.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDefUnit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblDefUnit.Location = new System.Drawing.Point(408, 28);
            this.lblDefUnit.Name = "lblDefUnit";
            this.lblDefUnit.Size = new System.Drawing.Size(73, 15);
            this.lblDefUnit.TabIndex = 4;
            this.lblDefUnit.Text = "Default Unit:";
            // 
            // cmbDefUnit
            // 
            this.cmbDefUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDefUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbDefUnit.FormattingEnabled = true;
            this.cmbDefUnit.Location = new System.Drawing.Point(485, 24);
            this.cmbDefUnit.Name = "cmbDefUnit";
            this.cmbDefUnit.Size = new System.Drawing.Size(110, 23);
            this.cmbDefUnit.TabIndex = 5;
            this.cmbDefUnit.SelectedIndexChanged += new System.EventHandler(this.Field_TextChanged);
            // 
            // lblQtyInPack
            // 
            this.lblQtyInPack.AutoSize = true;
            this.lblQtyInPack.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQtyInPack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblQtyInPack.Location = new System.Drawing.Point(612, 28);
            this.lblQtyInPack.Name = "lblQtyInPack";
            this.lblQtyInPack.Size = new System.Drawing.Size(71, 15);
            this.lblQtyInPack.TabIndex = 6;
            this.lblQtyInPack.Text = "Qty In Pack:";
            // 
            // txtQtyInPack
            // 
            this.txtQtyInPack.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQtyInPack.Location = new System.Drawing.Point(688, 25);
            this.txtQtyInPack.Name = "txtQtyInPack";
            this.txtQtyInPack.Size = new System.Drawing.Size(75, 23);
            this.txtQtyInPack.TabIndex = 7;
            this.txtQtyInPack.Text = "1";
            this.txtQtyInPack.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtQtyInPack.TextChanged += new System.EventHandler(this.Field_TextChanged);
            this.txtQtyInPack.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // btnSubUnits
            // 
            this.btnSubUnits.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnSubUnits.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSubUnits.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnSubUnits.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubUnits.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubUnits.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnSubUnits.Location = new System.Drawing.Point(778, 23);
            this.btnSubUnits.Name = "btnSubUnits";
            this.btnSubUnits.Size = new System.Drawing.Size(100, 27);
            this.btnSubUnits.TabIndex = 8;
            this.btnSubUnits.Text = "Sub-Units...";
            this.btnSubUnits.UseVisualStyleBackColor = false;
            this.btnSubUnits.Click += new System.EventHandler(this.btnSubUnits_Click);
            // 
            // grpStock
            // 
            this.grpStock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpStock.Controls.Add(this.lblOpnStock);
            this.grpStock.Controls.Add(this.txtOpnStock);
            this.grpStock.Controls.Add(this.lblOpnRate);
            this.grpStock.Controls.Add(this.txtOpnRate);
            this.grpStock.Controls.Add(this.chkLowStockAlert);
            this.grpStock.Controls.Add(this.lblLowStockQty);
            this.grpStock.Controls.Add(this.txtLowStockQty);
            this.grpStock.Controls.Add(this.chkAlert);
            this.grpStock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpStock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.grpStock.Location = new System.Drawing.Point(10, 274);
            this.grpStock.Name = "grpStock";
            this.grpStock.Size = new System.Drawing.Size(952, 70);
            this.grpStock.TabIndex = 3;
            this.grpStock.TabStop = false;
            this.grpStock.Text = "Opening Stock & Alerts";
            // 
            // lblOpnStock
            // 
            this.lblOpnStock.AutoSize = true;
            this.lblOpnStock.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOpnStock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblOpnStock.Location = new System.Drawing.Point(10, 28);
            this.lblOpnStock.Name = "lblOpnStock";
            this.lblOpnStock.Size = new System.Drawing.Size(65, 15);
            this.lblOpnStock.TabIndex = 0;
            this.lblOpnStock.Text = "Opn Stock:";
            // 
            // txtOpnStock
            // 
            this.txtOpnStock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOpnStock.Location = new System.Drawing.Point(80, 25);
            this.txtOpnStock.Name = "txtOpnStock";
            this.txtOpnStock.Size = new System.Drawing.Size(95, 23);
            this.txtOpnStock.TabIndex = 1;
            this.txtOpnStock.Text = "0";
            this.txtOpnStock.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtOpnStock.TextChanged += new System.EventHandler(this.Field_TextChanged);
            this.txtOpnStock.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // lblOpnRate
            // 
            this.lblOpnRate.AutoSize = true;
            this.lblOpnRate.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOpnRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblOpnRate.Location = new System.Drawing.Point(192, 28);
            this.lblOpnRate.Name = "lblOpnRate";
            this.lblOpnRate.Size = new System.Drawing.Size(60, 15);
            this.lblOpnRate.TabIndex = 2;
            this.lblOpnRate.Text = "Opn Rate:";
            // 
            // txtOpnRate
            // 
            this.txtOpnRate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOpnRate.Location = new System.Drawing.Point(258, 25);
            this.txtOpnRate.Name = "txtOpnRate";
            this.txtOpnRate.Size = new System.Drawing.Size(95, 23);
            this.txtOpnRate.TabIndex = 3;
            this.txtOpnRate.Text = "0";
            this.txtOpnRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtOpnRate.TextChanged += new System.EventHandler(this.Field_TextChanged);
            this.txtOpnRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // chkLowStockAlert
            // 
            this.chkLowStockAlert.AutoSize = true;
            this.chkLowStockAlert.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkLowStockAlert.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.chkLowStockAlert.Location = new System.Drawing.Point(375, 27);
            this.chkLowStockAlert.Name = "chkLowStockAlert";
            this.chkLowStockAlert.Size = new System.Drawing.Size(107, 19);
            this.chkLowStockAlert.TabIndex = 4;
            this.chkLowStockAlert.Text = "Low Stock Alert";
            this.chkLowStockAlert.UseVisualStyleBackColor = true;
            this.chkLowStockAlert.CheckedChanged += new System.EventHandler(this.Field_TextChanged);
            // 
            // lblLowStockQty
            // 
            this.lblLowStockQty.AutoSize = true;
            this.lblLowStockQty.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLowStockQty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblLowStockQty.Location = new System.Drawing.Point(495, 28);
            this.lblLowStockQty.Name = "lblLowStockQty";
            this.lblLowStockQty.Size = new System.Drawing.Size(86, 15);
            this.lblLowStockQty.TabIndex = 5;
            this.lblLowStockQty.Text = "Low Stock Qty:";
            // 
            // txtLowStockQty
            // 
            this.txtLowStockQty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLowStockQty.Location = new System.Drawing.Point(585, 25);
            this.txtLowStockQty.Name = "txtLowStockQty";
            this.txtLowStockQty.Size = new System.Drawing.Size(80, 23);
            this.txtLowStockQty.TabIndex = 6;
            this.txtLowStockQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLowStockQty.TextChanged += new System.EventHandler(this.Field_TextChanged);
            this.txtLowStockQty.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // chkAlert
            // 
            this.chkAlert.AutoSize = true;
            this.chkAlert.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAlert.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.chkAlert.Location = new System.Drawing.Point(690, 27);
            this.chkAlert.Name = "chkAlert";
            this.chkAlert.Size = new System.Drawing.Size(87, 19);
            this.chkAlert.TabIndex = 7;
            this.chkAlert.Text = "Price Alert";
            this.chkAlert.UseVisualStyleBackColor = true;
            this.chkAlert.CheckedChanged += new System.EventHandler(this.Field_TextChanged);
            // 
            // pnlMobileShop
            // 
            this.pnlMobileShop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMobileShop.Controls.Add(this.lblBrand);
            this.pnlMobileShop.Controls.Add(this.cmbBrand);
            this.pnlMobileShop.Controls.Add(this.lblModel);
            this.pnlMobileShop.Controls.Add(this.txtModel);
            this.pnlMobileShop.Controls.Add(this.lblStorage);
            this.pnlMobileShop.Controls.Add(this.txtStorage);
            this.pnlMobileShop.Controls.Add(this.lblRam);
            this.pnlMobileShop.Controls.Add(this.txtRam);
            this.pnlMobileShop.Controls.Add(this.lblColor);
            this.pnlMobileShop.Controls.Add(this.txtColor);
            this.pnlMobileShop.Controls.Add(this.chkRequireImei);
            this.pnlMobileShop.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlMobileShop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.pnlMobileShop.Location = new System.Drawing.Point(10, 350);
            this.pnlMobileShop.Name = "pnlMobileShop";
            this.pnlMobileShop.Size = new System.Drawing.Size(952, 70);
            this.pnlMobileShop.TabIndex = 4;
            this.pnlMobileShop.TabStop = false;
            this.pnlMobileShop.Text = "Mobile Shop Specifications";
            this.pnlMobileShop.Visible = false;
            // 
            // lblBrand
            // 
            this.lblBrand.AutoSize = true;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBrand.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblBrand.Location = new System.Drawing.Point(10, 28);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(41, 15);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "Brand:";
            // 
            // cmbBrand
            // 
            this.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBrand.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBrand.FormattingEnabled = true;
            this.cmbBrand.Location = new System.Drawing.Point(54, 24);
            this.cmbBrand.Name = "cmbBrand";
            this.cmbBrand.Size = new System.Drawing.Size(120, 23);
            this.cmbBrand.TabIndex = 1;
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblModel.Location = new System.Drawing.Point(185, 28);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(44, 15);
            this.lblModel.TabIndex = 2;
            this.lblModel.Text = "Model:";
            // 
            // txtModel
            // 
            this.txtModel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtModel.Location = new System.Drawing.Point(232, 25);
            this.txtModel.Name = "txtModel";
            this.txtModel.Size = new System.Drawing.Size(110, 23);
            this.txtModel.TabIndex = 3;
            // 
            // lblStorage
            // 
            this.lblStorage.AutoSize = true;
            this.lblStorage.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStorage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblStorage.Location = new System.Drawing.Point(352, 28);
            this.lblStorage.Name = "lblStorage";
            this.lblStorage.Size = new System.Drawing.Size(50, 15);
            this.lblStorage.TabIndex = 4;
            this.lblStorage.Text = "Storage:";
            // 
            // txtStorage
            // 
            this.txtStorage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStorage.Location = new System.Drawing.Point(404, 25);
            this.txtStorage.Name = "txtStorage";
            this.txtStorage.Size = new System.Drawing.Size(75, 23);
            this.txtStorage.TabIndex = 5;
            // 
            // lblRam
            // 
            this.lblRam.AutoSize = true;
            this.lblRam.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblRam.Location = new System.Drawing.Point(490, 28);
            this.lblRam.Name = "lblRam";
            this.lblRam.Size = new System.Drawing.Size(36, 15);
            this.lblRam.TabIndex = 6;
            this.lblRam.Text = "RAM:";
            // 
            // txtRam
            // 
            this.txtRam.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRam.Location = new System.Drawing.Point(530, 25);
            this.txtRam.Name = "txtRam";
            this.txtRam.Size = new System.Drawing.Size(65, 23);
            this.txtRam.TabIndex = 7;
            // 
            // lblColor
            // 
            this.lblColor.AutoSize = true;
            this.lblColor.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblColor.Location = new System.Drawing.Point(605, 28);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(39, 15);
            this.lblColor.TabIndex = 8;
            this.lblColor.Text = "Color:";
            // 
            // txtColor
            // 
            this.txtColor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColor.Location = new System.Drawing.Point(648, 25);
            this.txtColor.Name = "txtColor";
            this.txtColor.Size = new System.Drawing.Size(80, 23);
            this.txtColor.TabIndex = 9;
            // 
            // chkRequireImei
            // 
            this.chkRequireImei.AutoSize = true;
            this.chkRequireImei.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRequireImei.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.chkRequireImei.Location = new System.Drawing.Point(745, 27);
            this.chkRequireImei.Name = "chkRequireImei";
            this.chkRequireImei.Size = new System.Drawing.Size(95, 19);
            this.chkRequireImei.TabIndex = 10;
            this.chkRequireImei.Text = "Require IMEI";
            this.chkRequireImei.UseVisualStyleBackColor = true;
            // 
            // pnlRightImage
            // 
            this.pnlRightImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRightImage.BackColor = System.Drawing.Color.White;
            this.pnlRightImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRightImage.Controls.Add(this.profileImage1);
            this.pnlRightImage.Controls.Add(this.grpImageSpecs);
            this.pnlRightImage.Location = new System.Drawing.Point(990, 6);
            this.pnlRightImage.Name = "pnlRightImage";
            this.pnlRightImage.Size = new System.Drawing.Size(180, 508);
            this.pnlRightImage.TabIndex = 1;
            // 
            // profileImage1
            // 
            this.profileImage1.Location = new System.Drawing.Point(5, 4);
            this.profileImage1.Name = "profileImage1";
            this.profileImage1.Size = new System.Drawing.Size(168, 240);
            this.profileImage1.TabIndex = 0;
            // 
            // grpImageSpecs
            // 
            this.grpImageSpecs.Controls.Add(this.lblImageSpecs);
            this.grpImageSpecs.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpImageSpecs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.grpImageSpecs.Location = new System.Drawing.Point(5, 250);
            this.grpImageSpecs.Name = "grpImageSpecs";
            this.grpImageSpecs.Size = new System.Drawing.Size(168, 150);
            this.grpImageSpecs.TabIndex = 1;
            this.grpImageSpecs.TabStop = false;
            this.grpImageSpecs.Text = "Image Specs";
            // 
            // lblImageSpecs
            // 
            this.lblImageSpecs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblImageSpecs.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImageSpecs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblImageSpecs.Location = new System.Drawing.Point(3, 18);
            this.lblImageSpecs.Name = "lblImageSpecs";
            this.lblImageSpecs.Padding = new System.Windows.Forms.Padding(2);
            this.lblImageSpecs.Size = new System.Drawing.Size(162, 129);
            this.lblImageSpecs.TabIndex = 0;
            this.lblImageSpecs.Text = "• Ratio: 1:1 or 4:5\r\n• Size: 600×600 to\r\n  1000×1000 px\r\n• Max Size: 5 MB\r\n• Format: JPG, PNG\r\n• Clean white back-\r\n  ground displays best";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnNew);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.lblDetailStatus);
            this.panel1.Location = new System.Drawing.Point(6, 520);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1164, 38);
            this.panel1.TabIndex = 2;
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNew.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnNew.Location = new System.Drawing.Point(4, 3);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(88, 30);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "&New [F3]";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(96, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(88, 30);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "&Save [F5]";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.Location = new System.Drawing.Point(188, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(80, 30);
            this.btnDelete.TabIndex = 2;
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
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnClose.Location = new System.Drawing.Point(272, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 30);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblDetailStatus
            // 
            this.lblDetailStatus.AutoSize = true;
            this.lblDetailStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.lblDetailStatus.Location = new System.Drawing.Point(370, 11);
            this.lblDetailStatus.Name = "lblDetailStatus";
            this.lblDetailStatus.Size = new System.Drawing.Size(117, 15);
            this.lblDetailStatus.TabIndex = 4;
            this.lblDetailStatus.Text = "New Product Mode";
            // 
            // tbQuery
            // 
            this.tbQuery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.tbQuery.Controls.Add(this.dgvQuery);
            this.tbQuery.Controls.Add(this.pnlQueryFilter);
            this.tbQuery.Location = new System.Drawing.Point(4, 26);
            this.tbQuery.Name = "tbQuery";
            this.tbQuery.Padding = new System.Windows.Forms.Padding(6);
            this.tbQuery.Size = new System.Drawing.Size(1176, 564);
            this.tbQuery.TabIndex = 1;
            this.tbQuery.Text = "Product Search";
            // 
            // pnlQueryFilter
            // 
            this.pnlQueryFilter.BackColor = System.Drawing.Color.White;
            this.pnlQueryFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlQueryFilter.Controls.Add(this.lblQuerySearch);
            this.pnlQueryFilter.Controls.Add(this.txtQuerySearch);
            this.pnlQueryFilter.Controls.Add(this.lblQueryCategory);
            this.pnlQueryFilter.Controls.Add(this.cmbQueryCategory);
            this.pnlQueryFilter.Controls.Add(this.lblQueryType);
            this.pnlQueryFilter.Controls.Add(this.cmbQueryType);
            this.pnlQueryFilter.Controls.Add(this.btnQueryFind);
            this.pnlQueryFilter.Controls.Add(this.btnQueryReset);
            this.pnlQueryFilter.Controls.Add(this.btnAddNewFromSearch);
            this.pnlQueryFilter.Controls.Add(this.lblQueryCount);
            this.pnlQueryFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlQueryFilter.Location = new System.Drawing.Point(6, 6);
            this.pnlQueryFilter.Name = "pnlQueryFilter";
            this.pnlQueryFilter.Size = new System.Drawing.Size(1164, 42);
            this.pnlQueryFilter.TabIndex = 0;
            // 
            // lblQuerySearch
            // 
            this.lblQuerySearch.AutoSize = true;
            this.lblQuerySearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuerySearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblQuerySearch.Location = new System.Drawing.Point(8, 12);
            this.lblQuerySearch.Name = "lblQuerySearch";
            this.lblQuerySearch.Size = new System.Drawing.Size(48, 15);
            this.lblQuerySearch.TabIndex = 0;
            this.lblQuerySearch.Text = "Search:";
            // 
            // txtQuerySearch
            // 
            this.txtQuerySearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQuerySearch.Location = new System.Drawing.Point(58, 8);
            this.txtQuerySearch.Name = "txtQuerySearch";
            this.txtQuerySearch.Size = new System.Drawing.Size(200, 23);
            this.txtQuerySearch.TabIndex = 1;
            this.txtQuerySearch.TextChanged += new System.EventHandler(this.txtQuerySearch_TextChanged);
            this.txtQuerySearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtQuerySearch_KeyDown);
            // 
            // lblQueryCategory
            // 
            this.lblQueryCategory.AutoSize = true;
            this.lblQueryCategory.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQueryCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblQueryCategory.Location = new System.Drawing.Point(268, 12);
            this.lblQueryCategory.Name = "lblQueryCategory";
            this.lblQueryCategory.Size = new System.Drawing.Size(60, 15);
            this.lblQueryCategory.TabIndex = 2;
            this.lblQueryCategory.Text = "Category:";
            // 
            // cmbQueryCategory
            // 
            this.cmbQueryCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQueryCategory.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbQueryCategory.FormattingEnabled = true;
            this.cmbQueryCategory.Location = new System.Drawing.Point(330, 8);
            this.cmbQueryCategory.Name = "cmbQueryCategory";
            this.cmbQueryCategory.Size = new System.Drawing.Size(160, 23);
            this.cmbQueryCategory.TabIndex = 3;
            this.cmbQueryCategory.SelectedIndexChanged += new System.EventHandler(this.cmbQueryCategory_SelectedIndexChanged);
            // 
            // lblQueryType
            // 
            this.lblQueryType.AutoSize = true;
            this.lblQueryType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQueryType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblQueryType.Location = new System.Drawing.Point(498, 12);
            this.lblQueryType.Name = "lblQueryType";
            this.lblQueryType.Size = new System.Drawing.Size(36, 15);
            this.lblQueryType.TabIndex = 4;
            this.lblQueryType.Text = "Type:";
            // 
            // cmbQueryType
            // 
            this.cmbQueryType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQueryType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbQueryType.FormattingEnabled = true;
            this.cmbQueryType.Location = new System.Drawing.Point(536, 8);
            this.cmbQueryType.Name = "cmbQueryType";
            this.cmbQueryType.Size = new System.Drawing.Size(100, 23);
            this.cmbQueryType.TabIndex = 5;
            this.cmbQueryType.SelectedIndexChanged += new System.EventHandler(this.cmbQueryType_SelectedIndexChanged);
            // 
            // btnQueryFind
            // 
            this.btnQueryFind.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnQueryFind.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQueryFind.FlatAppearance.BorderSize = 0;
            this.btnQueryFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQueryFind.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQueryFind.ForeColor = System.Drawing.Color.White;
            this.btnQueryFind.Location = new System.Drawing.Point(645, 6);
            this.btnQueryFind.Name = "btnQueryFind";
            this.btnQueryFind.Size = new System.Drawing.Size(65, 27);
            this.btnQueryFind.TabIndex = 6;
            this.btnQueryFind.Text = "&Find";
            this.btnQueryFind.UseVisualStyleBackColor = false;
            this.btnQueryFind.Click += new System.EventHandler(this.btnQueryFind_Click);
            // 
            // btnQueryReset
            // 
            this.btnQueryReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnQueryReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQueryReset.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnQueryReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQueryReset.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQueryReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnQueryReset.Location = new System.Drawing.Point(715, 6);
            this.btnQueryReset.Name = "btnQueryReset";
            this.btnQueryReset.Size = new System.Drawing.Size(65, 27);
            this.btnQueryReset.TabIndex = 7;
            this.btnQueryReset.Text = "&Reset";
            this.btnQueryReset.UseVisualStyleBackColor = false;
            this.btnQueryReset.Click += new System.EventHandler(this.btnQueryReset_Click);
            // 
            // btnAddNewFromSearch
            // 
            this.btnAddNewFromSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnAddNewFromSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddNewFromSearch.FlatAppearance.BorderSize = 0;
            this.btnAddNewFromSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddNewFromSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddNewFromSearch.ForeColor = System.Drawing.Color.White;
            this.btnAddNewFromSearch.Location = new System.Drawing.Point(785, 6);
            this.btnAddNewFromSearch.Name = "btnAddNewFromSearch";
            this.btnAddNewFromSearch.Size = new System.Drawing.Size(100, 27);
            this.btnAddNewFromSearch.TabIndex = 8;
            this.btnAddNewFromSearch.Text = "+ Add New";
            this.btnAddNewFromSearch.UseVisualStyleBackColor = false;
            this.btnAddNewFromSearch.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // lblQueryCount
            // 
            this.lblQueryCount.AutoSize = true;
            this.lblQueryCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQueryCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.lblQueryCount.Location = new System.Drawing.Point(900, 12);
            this.lblQueryCount.Name = "lblQueryCount";
            this.lblQueryCount.Size = new System.Drawing.Size(95, 15);
            this.lblQueryCount.TabIndex = 9;
            this.lblQueryCount.Text = "Found: 0 items";
            // 
            // dgvQuery
            // 
            this.dgvQuery.AllowUserToAddRows = false;
            this.dgvQuery.AllowUserToDeleteRows = false;
            this.dgvQuery.AllowUserToResizeRows = false;
            this.dgvQuery.BackgroundColor = System.Drawing.Color.White;
            this.dgvQuery.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgvQuery.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvQuery.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvQuery.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvQuery.ColumnHeadersHeight = 32;
            this.dgvQuery.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvQuery.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnQId,
            this.clnQBarcode,
            this.clnQCategory,
            this.clnQTitle,
            this.clnQKey,
            this.clnQItemType,
            this.clnQPriRate,
            this.clnQSecRate,
            this.clnQPriUnit,
            this.clnQSecUnit,
            this.clnQMinSalePrice,
            this.clnQOpnStock});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvQuery.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvQuery.EnableHeadersVisualStyles = false;
            this.dgvQuery.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dgvQuery.Location = new System.Drawing.Point(6, 48);
            this.dgvQuery.MultiSelect = false;
            this.dgvQuery.Name = "dgvQuery";
            this.dgvQuery.ReadOnly = true;
            this.dgvQuery.RowHeadersWidth = 30;
            this.dgvQuery.RowTemplate.Height = 28;
            this.dgvQuery.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvQuery.Size = new System.Drawing.Size(1164, 510);
            this.dgvQuery.TabIndex = 1;
            this.dgvQuery.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvQuery_CellDoubleClick);
            this.dgvQuery.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvQuery_KeyDown);
            // 
            // clnQId
            // 
            this.clnQId.HeaderText = "Id";
            this.clnQId.Name = "clnQId";
            this.clnQId.ReadOnly = true;
            this.clnQId.Visible = false;
            // 
            // clnQBarcode
            // 
            this.clnQBarcode.HeaderText = "Barcode";
            this.clnQBarcode.MinimumWidth = 100;
            this.clnQBarcode.Name = "clnQBarcode";
            this.clnQBarcode.ReadOnly = true;
            this.clnQBarcode.Width = 110;
            // 
            // clnQCategory
            // 
            this.clnQCategory.HeaderText = "Category";
            this.clnQCategory.MinimumWidth = 110;
            this.clnQCategory.Name = "clnQCategory";
            this.clnQCategory.ReadOnly = true;
            this.clnQCategory.Width = 120;
            // 
            // clnQTitle
            // 
            this.clnQTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clnQTitle.HeaderText = "Product Name / Title";
            this.clnQTitle.MinimumWidth = 180;
            this.clnQTitle.Name = "clnQTitle";
            this.clnQTitle.ReadOnly = true;
            // 
            // clnQKey
            // 
            this.clnQKey.HeaderText = "Key";
            this.clnQKey.MinimumWidth = 50;
            this.clnQKey.Name = "clnQKey";
            this.clnQKey.ReadOnly = true;
            this.clnQKey.Width = 60;
            // 
            // clnQItemType
            // 
            this.clnQItemType.HeaderText = "Type";
            this.clnQItemType.MinimumWidth = 70;
            this.clnQItemType.Name = "clnQItemType";
            this.clnQItemType.ReadOnly = true;
            this.clnQItemType.Width = 75;
            // 
            // clnQPriRate
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clnQPriRate.DefaultCellStyle = dataGridViewCellStyle2;
            this.clnQPriRate.HeaderText = "Pri Rate";
            this.clnQPriRate.MinimumWidth = 80;
            this.clnQPriRate.Name = "clnQPriRate";
            this.clnQPriRate.ReadOnly = true;
            this.clnQPriRate.Width = 85;
            // 
            // clnQSecRate
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clnQSecRate.DefaultCellStyle = dataGridViewCellStyle3;
            this.clnQSecRate.HeaderText = "Sec Rate";
            this.clnQSecRate.MinimumWidth = 80;
            this.clnQSecRate.Name = "clnQSecRate";
            this.clnQSecRate.ReadOnly = true;
            this.clnQSecRate.Width = 85;
            // 
            // clnQPriUnit
            // 
            this.clnQPriUnit.HeaderText = "Pri Unit";
            this.clnQPriUnit.MinimumWidth = 70;
            this.clnQPriUnit.Name = "clnQPriUnit";
            this.clnQPriUnit.ReadOnly = true;
            this.clnQPriUnit.Width = 80;
            // 
            // clnQSecUnit
            // 
            this.clnQSecUnit.HeaderText = "Sec Unit";
            this.clnQSecUnit.MinimumWidth = 70;
            this.clnQSecUnit.Name = "clnQSecUnit";
            this.clnQSecUnit.ReadOnly = true;
            this.clnQSecUnit.Width = 80;
            // 
            // clnQMinSalePrice
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clnQMinSalePrice.DefaultCellStyle = dataGridViewCellStyle4;
            this.clnQMinSalePrice.HeaderText = "Min Price";
            this.clnQMinSalePrice.MinimumWidth = 80;
            this.clnQMinSalePrice.Name = "clnQMinSalePrice";
            this.clnQMinSalePrice.ReadOnly = true;
            this.clnQMinSalePrice.Width = 85;
            // 
            // clnQOpnStock
            // 
            this.clnQOpnStock.HeaderText = "Opn Stock";
            this.clnQOpnStock.MinimumWidth = 80;
            this.clnQOpnStock.Name = "clnQOpnStock";
            this.clnQOpnStock.ReadOnly = true;
            this.clnQOpnStock.Width = 85;
            // 
            // frmItemDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(1200, 660);
            this.Controls.Add(this.tbItemMain);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize = new System.Drawing.Size(1000, 580);
            this.Name = "frmItemDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Product Management";
            this.Load += new System.EventHandler(this.frmItemDetail_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHeaderLogo)).EndInit();
            this.pnlTopStats.ResumeLayout(false);
            this.tbItemMain.ResumeLayout(false);
            this.tbDetail.ResumeLayout(false);
            this.pnlDetailScroll.ResumeLayout(false);
            this.grpBasic.ResumeLayout(false);
            this.grpBasic.PerformLayout();
            this.grpPricing.ResumeLayout(false);
            this.grpPricing.PerformLayout();
            this.grpUnits.ResumeLayout(false);
            this.grpUnits.PerformLayout();
            this.grpStock.ResumeLayout(false);
            this.grpStock.PerformLayout();
            this.pnlMobileShop.ResumeLayout(false);
            this.pnlMobileShop.PerformLayout();
            this.pnlRightImage.ResumeLayout(false);
            this.grpImageSpecs.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tbQuery.ResumeLayout(false);
            this.pnlQueryFilter.ResumeLayout(false);
            this.pnlQueryFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuery)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.PictureBox picHeaderLogo;
        private System.Windows.Forms.Label lblHeaderTitle;
        private System.Windows.Forms.Label lblHeaderSubtitle;
        private System.Windows.Forms.Panel pnlTopStats;
        private System.Windows.Forms.Label lblTotalProductsBadge;
        private System.Windows.Forms.TabControl tbItemMain;
        private System.Windows.Forms.TabPage tbDetail;
        private System.Windows.Forms.TabPage tbQuery;
        private System.Windows.Forms.Panel pnlDetailScroll;
        private System.Windows.Forms.GroupBox grpBasic;
        private System.Windows.Forms.Label lblBarcode;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Label lblItemKey;
        private System.Windows.Forms.TextBox txtItemKey;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbItemCatagory;
        private System.Windows.Forms.Label lblItemType;
        private System.Windows.Forms.ComboBox cmbItemType;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.GroupBox grpPricing;
        private System.Windows.Forms.Label lblPriRate;
        private System.Windows.Forms.TextBox txtPriRate;
        private System.Windows.Forms.Label lblSecRate;
        private System.Windows.Forms.TextBox txtSecRate;
        private System.Windows.Forms.Label lblMinSalePrice;
        private System.Windows.Forms.TextBox txtMinSalePrice;
        private System.Windows.Forms.Button btnCategoryRates;
        private System.Windows.Forms.GroupBox grpUnits;
        private System.Windows.Forms.Label lblPriUnit;
        private System.Windows.Forms.ComboBox cmbPriUnit;
        private System.Windows.Forms.Label lblSecUnit;
        private System.Windows.Forms.ComboBox cmbSecUnit;
        private System.Windows.Forms.Label lblDefUnit;
        private System.Windows.Forms.ComboBox cmbDefUnit;
        private System.Windows.Forms.Label lblQtyInPack;
        private System.Windows.Forms.TextBox txtQtyInPack;
        private System.Windows.Forms.Button btnSubUnits;
        private System.Windows.Forms.GroupBox grpStock;
        private System.Windows.Forms.Label lblOpnStock;
        private System.Windows.Forms.TextBox txtOpnStock;
        private System.Windows.Forms.Label lblOpnRate;
        private System.Windows.Forms.TextBox txtOpnRate;
        private System.Windows.Forms.CheckBox chkLowStockAlert;
        private System.Windows.Forms.Label lblLowStockQty;
        private System.Windows.Forms.TextBox txtLowStockQty;
        private System.Windows.Forms.CheckBox chkAlert;
        private System.Windows.Forms.GroupBox pnlMobileShop;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.ComboBox cmbBrand;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label lblStorage;
        private System.Windows.Forms.TextBox txtStorage;
        private System.Windows.Forms.Label lblRam;
        private System.Windows.Forms.TextBox txtRam;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.TextBox txtColor;
        private System.Windows.Forms.CheckBox chkRequireImei;
        private System.Windows.Forms.Panel pnlRightImage;
        private ERP.Controls.ProfileImage profileImage1;
        private System.Windows.Forms.GroupBox grpImageSpecs;
        private System.Windows.Forms.Label lblImageSpecs;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblDetailStatus;
        private System.Windows.Forms.Panel pnlQueryFilter;
        private System.Windows.Forms.Label lblQuerySearch;
        private System.Windows.Forms.TextBox txtQuerySearch;
        private System.Windows.Forms.Label lblQueryCategory;
        private System.Windows.Forms.ComboBox cmbQueryCategory;
        private System.Windows.Forms.Label lblQueryType;
        private System.Windows.Forms.ComboBox cmbQueryType;
        private System.Windows.Forms.Button btnQueryFind;
        private System.Windows.Forms.Button btnQueryReset;
        private System.Windows.Forms.Button btnAddNewFromSearch;
        private System.Windows.Forms.Label lblQueryCount;
        private System.Windows.Forms.DataGridView dgvQuery;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQId;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQBarcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQItemType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQPriRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQSecRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQPriUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQSecUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQMinSalePrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQOpnStock;
    }
}
