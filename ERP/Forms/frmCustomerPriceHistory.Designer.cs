namespace ERP.Forms
{
    partial class frmCustomerPriceHistory
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
            this.lblItem = new System.Windows.Forms.Label();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.grpLastSale = new System.Windows.Forms.GroupBox();
            this.btnApplyLastRate = new System.Windows.Forms.Button();
            this.lblLastVoucher = new System.Windows.Forms.Label();
            this.lblLastQty = new System.Windows.Forms.Label();
            this.lblLastDate = new System.Windows.Forms.Label();
            this.lblLastRate = new System.Windows.Forms.Label();
            this.lblLastRateTitle = new System.Windows.Forms.Label();
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            this.clnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnVoucher = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnSecRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblRecentTitle = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.grpLastSale.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHeader.Controls.Add(this.lblItem);
            this.pnlHeader.Controls.Add(this.lblCustomer);
            this.pnlHeader.Location = new System.Drawing.Point(12, 12);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(610, 56);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblItem.Location = new System.Drawing.Point(10, 30);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(40, 17);
            this.lblItem.TabIndex = 1;
            this.lblItem.Text = "Item:";
            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblCustomer.Location = new System.Drawing.Point(10, 8);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(71, 17);
            this.lblCustomer.TabIndex = 0;
            this.lblCustomer.Text = "Customer:";
            // 
            // grpLastSale
            // 
            this.grpLastSale.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(247)))), ((int)(((byte)(255)))));
            this.grpLastSale.Controls.Add(this.btnApplyLastRate);
            this.grpLastSale.Controls.Add(this.lblLastVoucher);
            this.grpLastSale.Controls.Add(this.lblLastQty);
            this.grpLastSale.Controls.Add(this.lblLastDate);
            this.grpLastSale.Controls.Add(this.lblLastRate);
            this.grpLastSale.Controls.Add(this.lblLastRateTitle);
            this.grpLastSale.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpLastSale.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.grpLastSale.Location = new System.Drawing.Point(12, 74);
            this.grpLastSale.Name = "grpLastSale";
            this.grpLastSale.Size = new System.Drawing.Size(610, 92);
            this.grpLastSale.TabIndex = 1;
            this.grpLastSale.TabStop = false;
            this.grpLastSale.Text = "Last Sale / Supply Record";
            // 
            // btnApplyLastRate
            // 
            this.btnApplyLastRate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(88)))), ((int)(((byte)(217)))));
            this.btnApplyLastRate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApplyLastRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyLastRate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApplyLastRate.ForeColor = System.Drawing.Color.White;
            this.btnApplyLastRate.Location = new System.Drawing.Point(450, 24);
            this.btnApplyLastRate.Name = "btnApplyLastRate";
            this.btnApplyLastRate.Size = new System.Drawing.Size(145, 48);
            this.btnApplyLastRate.TabIndex = 5;
            this.btnApplyLastRate.Text = "Apply Last Rate";
            this.btnApplyLastRate.UseVisualStyleBackColor = false;
            this.btnApplyLastRate.Click += new System.EventHandler(this.btnApplyLastRate_Click);
            // 
            // lblLastVoucher
            // 
            this.lblLastVoucher.AutoSize = true;
            this.lblLastVoucher.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastVoucher.Location = new System.Drawing.Point(260, 58);
            this.lblLastVoucher.Name = "lblLastVoucher";
            this.lblLastVoucher.Size = new System.Drawing.Size(53, 15);
            this.lblLastVoucher.TabIndex = 4;
            this.lblLastVoucher.Text = "Voucher:";
            // 
            // lblLastQty
            // 
            this.lblLastQty.AutoSize = true;
            this.lblLastQty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastQty.Location = new System.Drawing.Point(14, 58);
            this.lblLastQty.Name = "lblLastQty";
            this.lblLastQty.Size = new System.Drawing.Size(29, 15);
            this.lblLastQty.TabIndex = 3;
            this.lblLastQty.Text = "Qty:";
            // 
            // lblLastDate
            // 
            this.lblLastDate.AutoSize = true;
            this.lblLastDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastDate.Location = new System.Drawing.Point(260, 28);
            this.lblLastDate.Name = "lblLastDate";
            this.lblLastDate.Size = new System.Drawing.Size(34, 15);
            this.lblLastDate.TabIndex = 2;
            this.lblLastDate.Text = "Date:";
            // 
            // lblLastRate
            // 
            this.lblLastRate.AutoSize = true;
            this.lblLastRate.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(88)))), ((int)(((byte)(217)))));
            this.lblLastRate.Location = new System.Drawing.Point(82, 21);
            this.lblLastRate.Name = "lblLastRate";
            this.lblLastRate.Size = new System.Drawing.Size(43, 25);
            this.lblLastRate.TabIndex = 1;
            this.lblLastRate.Text = "Rs. -";
            // 
            // lblLastRateTitle
            // 
            this.lblLastRateTitle.AutoSize = true;
            this.lblLastRateTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastRateTitle.Location = new System.Drawing.Point(14, 26);
            this.lblLastRateTitle.Name = "lblLastRateTitle";
            this.lblLastRateTitle.Size = new System.Drawing.Size(68, 17);
            this.lblLastRateTitle.TabIndex = 0;
            this.lblLastRateTitle.Text = "Last Rate:";
            // 
            // dgvHistory
            // 
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.BackgroundColor = System.Drawing.Color.White;
            this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnDate,
            this.clnVoucher,
            this.clnQty,
            this.clnRate,
            this.clnSecRate,
            this.clnAmount});
            this.dgvHistory.Location = new System.Drawing.Point(12, 196);
            this.dgvHistory.MultiSelect = false;
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.RowHeadersVisible = false;
            this.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.Size = new System.Drawing.Size(610, 160);
            this.dgvHistory.TabIndex = 2;
            this.dgvHistory.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHistory_CellDoubleClick);
            // 
            // clnDate
            // 
            this.clnDate.HeaderText = "Date";
            this.clnDate.Name = "clnDate";
            this.clnDate.ReadOnly = true;
            this.clnDate.Width = 90;
            // 
            // clnVoucher
            // 
            this.clnVoucher.HeaderText = "Voucher #";
            this.clnVoucher.Name = "clnVoucher";
            this.clnVoucher.ReadOnly = true;
            this.clnVoucher.Width = 95;
            // 
            // clnQty
            // 
            this.clnQty.HeaderText = "Qty";
            this.clnQty.Name = "clnQty";
            this.clnQty.ReadOnly = true;
            this.clnQty.Width = 70;
            // 
            // clnRate
            // 
            this.clnRate.HeaderText = "Rate";
            this.clnRate.Name = "clnRate";
            this.clnRate.ReadOnly = true;
            this.clnRate.Width = 95;
            // 
            // clnSecRate
            // 
            this.clnSecRate.HeaderText = "Pack Rate";
            this.clnSecRate.Name = "clnSecRate";
            this.clnSecRate.ReadOnly = true;
            this.clnSecRate.Width = 95;
            // 
            // clnAmount
            // 
            this.clnAmount.HeaderText = "Amount";
            this.clnAmount.Name = "clnAmount";
            this.clnAmount.ReadOnly = true;
            this.clnAmount.Width = 120;
            // 
            // lblRecentTitle
            // 
            this.lblRecentTitle.AutoSize = true;
            this.lblRecentTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecentTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblRecentTitle.Location = new System.Drawing.Point(12, 174);
            this.lblRecentTitle.Name = "lblRecentTitle";
            this.lblRecentTitle.Size = new System.Drawing.Size(262, 17);
            this.lblRecentTitle.TabIndex = 3;
            this.lblRecentTitle.Text = "Recent Transactions (Sales / Sale Supply):";
            // 
            // btnApply
            // 
            this.btnApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnApply.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApply.ForeColor = System.Drawing.Color.White;
            this.btnApply.Location = new System.Drawing.Point(362, 368);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(155, 34);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply Selected Rate";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(527, 368);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(95, 34);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close (Esc)";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmCustomerPriceHistory
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(634, 414);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lblRecentTitle);
            this.Controls.Add(this.dgvHistory);
            this.Controls.Add(this.grpLastSale);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCustomerPriceHistory";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Customer Item Price History";
            this.Load += new System.EventHandler(this.frmCustomerPriceHistory_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.grpLastSale.ResumeLayout(false);
            this.grpLastSale.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.GroupBox grpLastSale;
        private System.Windows.Forms.Label lblLastRateTitle;
        private System.Windows.Forms.Label lblLastRate;
        private System.Windows.Forms.Label lblLastDate;
        private System.Windows.Forms.Label lblLastQty;
        private System.Windows.Forms.Label lblLastVoucher;
        private System.Windows.Forms.Button btnApplyLastRate;
        private System.Windows.Forms.DataGridView dgvHistory;
        private System.Windows.Forms.Label lblRecentTitle;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnVoucher;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnSecRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnAmount;
    }
}
