namespace ERP.Forms
{
    partial class frmImeiStock
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cmbPtaFilter = new System.Windows.Forms.ComboBox();
            this.lblPtaFilter = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvStock = new System.Windows.Forms.DataGridView();
            this.clnImei = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnImei2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnItemTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnBrand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnStorage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnPta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnBattery = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnCondition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnPurchaseCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnAddedCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnTotalCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnSellingPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnPurchaseVoucher = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnPurchaseDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblTotalCount = new System.Windows.Forms.Label();
            this.btnAddCost = new System.Windows.Forms.Button();
            this.btnViewHistory = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStock)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.SteelBlue;
            this.pnlTop.Controls.Add(this.btnRefresh);
            this.pnlTop.Controls.Add(this.cmbPtaFilter);
            this.pnlTop.Controls.Add(this.lblPtaFilter);
            this.pnlTop.Controls.Add(this.txtSearch);
            this.pnlTop.Controls.Add(this.lblSearch);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1080, 52);
            this.pnlTop.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(990, 11);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(78, 30);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cmbPtaFilter
            // 
            this.cmbPtaFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPtaFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPtaFilter.FormattingEnabled = true;
            this.cmbPtaFilter.Items.AddRange(new object[] {
            "All",
            "Official PTA",
            "Non-PTA",
            "CPID",
            "Patched",
            "JV"});
            this.cmbPtaFilter.Location = new System.Drawing.Point(860, 15);
            this.cmbPtaFilter.Name = "cmbPtaFilter";
            this.cmbPtaFilter.Size = new System.Drawing.Size(120, 23);
            this.cmbPtaFilter.TabIndex = 4;
            this.cmbPtaFilter.SelectedIndexChanged += new System.EventHandler(this.cmbPtaFilter_SelectedIndexChanged);
            // 
            // lblPtaFilter
            // 
            this.lblPtaFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPtaFilter.AutoSize = true;
            this.lblPtaFilter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPtaFilter.ForeColor = System.Drawing.Color.White;
            this.lblPtaFilter.Location = new System.Drawing.Point(825, 18);
            this.lblPtaFilter.Name = "lblPtaFilter";
            this.lblPtaFilter.Size = new System.Drawing.Size(32, 15);
            this.lblPtaFilter.TabIndex = 3;
            this.lblPtaFilter.Text = "PTA:";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(620, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(190, 23);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // lblSearch
            // 
            this.lblSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearch.ForeColor = System.Drawing.Color.White;
            this.lblSearch.Location = new System.Drawing.Point(535, 18);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(81, 15);
            this.lblSearch.TabIndex = 1;
            this.lblSearch.Text = "Search IMEI:";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(270, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "IMEI Stock Ledger & Refurbishment";
            // 
            // dgvStock
            // 
            this.dgvStock.AllowUserToAddRows = false;
            this.dgvStock.AllowUserToDeleteRows = false;
            this.dgvStock.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvStock.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvStock.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStock.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnImei,
            this.clnImei2,
            this.clnItemTitle,
            this.clnBrand,
            this.clnModel,
            this.clnStorage,
            this.clnColor,
            this.clnPta,
            this.clnBattery,
            this.clnCondition,
            this.clnPurchaseCost,
            this.clnAddedCost,
            this.clnTotalCost,
            this.clnSellingPrice,
            this.clnPurchaseVoucher,
            this.clnPurchaseDate});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvStock.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvStock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStock.Location = new System.Drawing.Point(0, 52);
            this.dgvStock.MultiSelect = false;
            this.dgvStock.Name = "dgvStock";
            this.dgvStock.ReadOnly = true;
            this.dgvStock.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStock.Size = new System.Drawing.Size(1080, 488);
            this.dgvStock.TabIndex = 1;
            // 
            // clnImei
            // 
            this.clnImei.HeaderText = "IMEI 1";
            this.clnImei.Name = "clnImei";
            this.clnImei.ReadOnly = true;
            this.clnImei.Width = 135;
            // 
            // clnImei2
            // 
            this.clnImei2.HeaderText = "IMEI 2";
            this.clnImei2.Name = "clnImei2";
            this.clnImei2.ReadOnly = true;
            this.clnImei2.Width = 135;
            // 
            // clnItemTitle
            // 
            this.clnItemTitle.HeaderText = "Device Title";
            this.clnItemTitle.Name = "clnItemTitle";
            this.clnItemTitle.ReadOnly = true;
            this.clnItemTitle.Width = 140;
            // 
            // clnBrand
            // 
            this.clnBrand.HeaderText = "Brand";
            this.clnBrand.Name = "clnBrand";
            this.clnBrand.ReadOnly = true;
            this.clnBrand.Width = 85;
            // 
            // clnModel
            // 
            this.clnModel.HeaderText = "Model";
            this.clnModel.Name = "clnModel";
            this.clnModel.ReadOnly = true;
            this.clnModel.Width = 90;
            // 
            // clnStorage
            // 
            this.clnStorage.HeaderText = "Storage";
            this.clnStorage.Name = "clnStorage";
            this.clnStorage.ReadOnly = true;
            this.clnStorage.Width = 65;
            // 
            // clnColor
            // 
            this.clnColor.HeaderText = "Color";
            this.clnColor.Name = "clnColor";
            this.clnColor.ReadOnly = true;
            this.clnColor.Width = 75;
            // 
            // clnPta
            // 
            this.clnPta.HeaderText = "PTA Status";
            this.clnPta.Name = "clnPta";
            this.clnPta.ReadOnly = true;
            this.clnPta.Width = 90;
            // 
            // clnBattery
            // 
            this.clnBattery.HeaderText = "Battery";
            this.clnBattery.Name = "clnBattery";
            this.clnBattery.ReadOnly = true;
            this.clnBattery.Width = 60;
            // 
            // clnCondition
            // 
            this.clnCondition.HeaderText = "Condition";
            this.clnCondition.Name = "clnCondition";
            this.clnCondition.ReadOnly = true;
            this.clnCondition.Width = 100;
            // 
            // clnPurchaseCost
            // 
            this.clnPurchaseCost.HeaderText = "Purchase Cost";
            this.clnPurchaseCost.Name = "clnPurchaseCost";
            this.clnPurchaseCost.ReadOnly = true;
            this.clnPurchaseCost.Width = 95;
            // 
            // clnAddedCost
            // 
            this.clnAddedCost.HeaderText = "Refurb/PTA";
            this.clnAddedCost.Name = "clnAddedCost";
            this.clnAddedCost.ReadOnly = true;
            this.clnAddedCost.Width = 85;
            // 
            // clnTotalCost
            // 
            this.clnTotalCost.HeaderText = "Total Cost";
            this.clnTotalCost.Name = "clnTotalCost";
            this.clnTotalCost.ReadOnly = true;
            this.clnTotalCost.Width = 90;
            // 
            // clnSellingPrice
            // 
            this.clnSellingPrice.HeaderText = "Sale Price";
            this.clnSellingPrice.Name = "clnSellingPrice";
            this.clnSellingPrice.ReadOnly = true;
            this.clnSellingPrice.Width = 85;
            // 
            // clnPurchaseVoucher
            // 
            this.clnPurchaseVoucher.HeaderText = "Voucher";
            this.clnPurchaseVoucher.Name = "clnPurchaseVoucher";
            this.clnPurchaseVoucher.ReadOnly = true;
            this.clnPurchaseVoucher.Width = 80;
            // 
            // clnPurchaseDate
            // 
            this.clnPurchaseDate.HeaderText = "Date";
            this.clnPurchaseDate.Name = "clnPurchaseDate";
            this.clnPurchaseDate.ReadOnly = true;
            this.clnPurchaseDate.Width = 80;
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.SystemColors.Control;
            this.pnlBottom.Controls.Add(this.lblTotalCount);
            this.pnlBottom.Controls.Add(this.btnAddCost);
            this.pnlBottom.Controls.Add(this.btnViewHistory);
            this.pnlBottom.Controls.Add(this.btnClose);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 540);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1080, 50);
            this.pnlBottom.TabIndex = 2;
            // 
            // lblTotalCount
            // 
            this.lblTotalCount.AutoSize = true;
            this.lblTotalCount.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCount.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblTotalCount.Location = new System.Drawing.Point(12, 16);
            this.lblTotalCount.Name = "lblTotalCount";
            this.lblTotalCount.Size = new System.Drawing.Size(126, 17);
            this.lblTotalCount.TabIndex = 0;
            this.lblTotalCount.Text = "Total Phones: 0";
            // 
            // btnAddCost
            // 
            this.btnAddCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddCost.BackColor = System.Drawing.Color.DarkOrange;
            this.btnAddCost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddCost.ForeColor = System.Drawing.Color.White;
            this.btnAddCost.Location = new System.Drawing.Point(620, 10);
            this.btnAddCost.Name = "btnAddCost";
            this.btnAddCost.Size = new System.Drawing.Size(200, 32);
            this.btnAddCost.TabIndex = 1;
            this.btnAddCost.Text = "+ Add Landed Cost / Refurb";
            this.btnAddCost.UseVisualStyleBackColor = false;
            this.btnAddCost.Click += new System.EventHandler(this.btnAddCost_Click);
            // 
            // btnViewHistory
            // 
            this.btnViewHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewHistory.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnViewHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewHistory.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnViewHistory.ForeColor = System.Drawing.Color.White;
            this.btnViewHistory.Location = new System.Drawing.Point(828, 10);
            this.btnViewHistory.Name = "btnViewHistory";
            this.btnViewHistory.Size = new System.Drawing.Size(160, 32);
            this.btnViewHistory.TabIndex = 2;
            this.btnViewHistory.Text = "360° Timeline History";
            this.btnViewHistory.UseVisualStyleBackColor = false;
            this.btnViewHistory.Click += new System.EventHandler(this.btnViewHistory_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Gray;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(996, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 32);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmImeiStock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1080, 590);
            this.Controls.Add(this.dgvStock);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmImeiStock";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IMEI Stock Ledger";
            this.Load += new System.EventHandler(this.frmImeiStock_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStock)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.ComboBox cmbPtaFilter;
        private System.Windows.Forms.Label lblPtaFilter;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.DataGridView dgvStock;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblTotalCount;
        private System.Windows.Forms.Button btnAddCost;
        private System.Windows.Forms.Button btnViewHistory;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnImei;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnImei2;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnItemTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnBrand;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnModel;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnStorage;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnColor;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnPta;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnBattery;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnCondition;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnPurchaseCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnAddedCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnTotalCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnSellingPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnPurchaseVoucher;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnPurchaseDate;
    }
}
