namespace ERP.Forms
{
    partial class frmImeiHistory
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
            this.lblStatusBadge = new System.Windows.Forms.Label();
            this.lblDeviceSummary = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tbHistory = new System.Windows.Forms.TabControl();
            this.tabTimeline = new System.Windows.Forms.TabPage();
            this.dgvTimeline = new System.Windows.Forms.DataGridView();
            this.clnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnEvent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnVoucher = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnParty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnPta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabCostAdditions = new System.Windows.Forms.TabPage();
            this.dgvCosts = new System.Windows.Forms.DataGridView();
            this.clnCostDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnCostType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnCostAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnCostDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnCostAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            this.tbHistory.SuspendLayout();
            this.tabTimeline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimeline)).BeginInit();
            this.tabCostAdditions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCosts)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.MidnightBlue;
            this.pnlTop.Controls.Add(this.lblStatusBadge);
            this.pnlTop.Controls.Add(this.lblDeviceSummary);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(920, 60);
            this.pnlTop.TabIndex = 0;
            // 
            // lblStatusBadge
            // 
            this.lblStatusBadge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatusBadge.BackColor = System.Drawing.Color.ForestGreen;
            this.lblStatusBadge.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusBadge.ForeColor = System.Drawing.Color.White;
            this.lblStatusBadge.Location = new System.Drawing.Point(785, 14);
            this.lblStatusBadge.Name = "lblStatusBadge";
            this.lblStatusBadge.Size = new System.Drawing.Size(120, 30);
            this.lblStatusBadge.TabIndex = 2;
            this.lblStatusBadge.Text = "IN STOCK";
            this.lblStatusBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDeviceSummary
            // 
            this.lblDeviceSummary.AutoSize = true;
            this.lblDeviceSummary.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeviceSummary.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.lblDeviceSummary.Location = new System.Drawing.Point(13, 34);
            this.lblDeviceSummary.Name = "lblDeviceSummary";
            this.lblDeviceSummary.Size = new System.Drawing.Size(130, 17);
            this.lblDeviceSummary.TabIndex = 1;
            this.lblDeviceSummary.Text = "Loading details...";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "360° Device Lifecycle Timeline";
            // 
            // tbHistory
            // 
            this.tbHistory.Controls.Add(this.tabTimeline);
            this.tbHistory.Controls.Add(this.tabCostAdditions);
            this.tbHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbHistory.Location = new System.Drawing.Point(0, 60);
            this.tbHistory.Name = "tbHistory";
            this.tbHistory.SelectedIndex = 0;
            this.tbHistory.Size = new System.Drawing.Size(920, 420);
            this.tbHistory.TabIndex = 1;
            // 
            // tabTimeline
            // 
            this.tabTimeline.Controls.Add(this.dgvTimeline);
            this.tabTimeline.Location = new System.Drawing.Point(4, 24);
            this.tabTimeline.Name = "tabTimeline";
            this.tabTimeline.Padding = new System.Windows.Forms.Padding(3);
            this.tabTimeline.Size = new System.Drawing.Size(912, 392);
            this.tabTimeline.TabIndex = 0;
            this.tabTimeline.Text = "Full Event Timeline";
            this.tabTimeline.UseVisualStyleBackColor = true;
            // 
            // dgvTimeline
            // 
            this.dgvTimeline.AllowUserToAddRows = false;
            this.dgvTimeline.AllowUserToDeleteRows = false;
            this.dgvTimeline.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTimeline.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTimeline.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTimeline.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnDate,
            this.clnEvent,
            this.clnVoucher,
            this.clnParty,
            this.clnRate,
            this.clnPta,
            this.clnNotes});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTimeline.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTimeline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTimeline.Location = new System.Drawing.Point(3, 3);
            this.dgvTimeline.MultiSelect = false;
            this.dgvTimeline.Name = "dgvTimeline";
            this.dgvTimeline.ReadOnly = true;
            this.dgvTimeline.Size = new System.Drawing.Size(906, 386);
            this.dgvTimeline.TabIndex = 0;
            // 
            // clnDate
            // 
            this.clnDate.HeaderText = "Date";
            this.clnDate.Name = "clnDate";
            this.clnDate.ReadOnly = true;
            this.clnDate.Width = 110;
            // 
            // clnEvent
            // 
            this.clnEvent.HeaderText = "Event Type";
            this.clnEvent.Name = "clnEvent";
            this.clnEvent.ReadOnly = true;
            this.clnEvent.Width = 150;
            // 
            // clnVoucher
            // 
            this.clnVoucher.HeaderText = "Voucher #";
            this.clnVoucher.Name = "clnVoucher";
            this.clnVoucher.ReadOnly = true;
            this.clnVoucher.Width = 100;
            // 
            // clnParty
            // 
            this.clnParty.HeaderText = "Party / Customer";
            this.clnParty.Name = "clnParty";
            this.clnParty.ReadOnly = true;
            this.clnParty.Width = 160;
            // 
            // clnRate
            // 
            this.clnRate.HeaderText = "Amount (Rs)";
            this.clnRate.Name = "clnRate";
            this.clnRate.ReadOnly = true;
            this.clnRate.Width = 100;
            // 
            // clnPta
            // 
            this.clnPta.HeaderText = "PTA Status";
            this.clnPta.Name = "clnPta";
            this.clnPta.ReadOnly = true;
            this.clnPta.Width = 100;
            // 
            // clnNotes
            // 
            this.clnNotes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clnNotes.HeaderText = "Notes / Description";
            this.clnNotes.Name = "clnNotes";
            this.clnNotes.ReadOnly = true;
            // 
            // tabCostAdditions
            // 
            this.tabCostAdditions.Controls.Add(this.dgvCosts);
            this.tabCostAdditions.Location = new System.Drawing.Point(4, 24);
            this.tabCostAdditions.Name = "tabCostAdditions";
            this.tabCostAdditions.Padding = new System.Windows.Forms.Padding(3);
            this.tabCostAdditions.Size = new System.Drawing.Size(912, 392);
            this.tabCostAdditions.TabIndex = 1;
            this.tabCostAdditions.Text = "Refurbishment & Landed Costs";
            this.tabCostAdditions.UseVisualStyleBackColor = true;
            // 
            // dgvCosts
            // 
            this.dgvCosts.AllowUserToAddRows = false;
            this.dgvCosts.AllowUserToDeleteRows = false;
            this.dgvCosts.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCosts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCosts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCosts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnCostDate,
            this.clnCostType,
            this.clnCostAmount,
            this.clnCostDesc,
            this.clnCostAccount});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCosts.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvCosts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCosts.Location = new System.Drawing.Point(3, 3);
            this.dgvCosts.MultiSelect = false;
            this.dgvCosts.Name = "dgvCosts";
            this.dgvCosts.ReadOnly = true;
            this.dgvCosts.Size = new System.Drawing.Size(906, 386);
            this.dgvCosts.TabIndex = 0;
            // 
            // clnCostDate
            // 
            this.clnCostDate.HeaderText = "Date";
            this.clnCostDate.Name = "clnCostDate";
            this.clnCostDate.ReadOnly = true;
            this.clnCostDate.Width = 110;
            // 
            // clnCostType
            // 
            this.clnCostType.HeaderText = "Cost Type";
            this.clnCostType.Name = "clnCostType";
            this.clnCostType.ReadOnly = true;
            this.clnCostType.Width = 160;
            // 
            // clnCostAmount
            // 
            this.clnCostAmount.HeaderText = "Amount (Rs)";
            this.clnCostAmount.Name = "clnCostAmount";
            this.clnCostAmount.ReadOnly = true;
            this.clnCostAmount.Width = 110;
            // 
            // clnCostDesc
            // 
            this.clnCostDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clnCostDesc.HeaderText = "Description";
            this.clnCostDesc.Name = "clnCostDesc";
            this.clnCostDesc.ReadOnly = true;
            // 
            // clnCostAccount
            // 
            this.clnCostAccount.HeaderText = "Paid From";
            this.clnCostAccount.Name = "clnCostAccount";
            this.clnCostAccount.ReadOnly = true;
            this.clnCostAccount.Width = 140;
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.SystemColors.Control;
            this.pnlBottom.Controls.Add(this.btnClose);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 480);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(920, 45);
            this.pnlBottom.TabIndex = 2;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Gray;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(833, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 29);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmImeiHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 525);
            this.Controls.Add(this.tbHistory);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmImeiHistory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "IMEI Lifecycle History";
            this.Load += new System.EventHandler(this.frmImeiHistory_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.tbHistory.ResumeLayout(false);
            this.tabTimeline.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimeline)).EndInit();
            this.tabCostAdditions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCosts)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDeviceSummary;
        private System.Windows.Forms.Label lblStatusBadge;
        private System.Windows.Forms.TabControl tbHistory;
        private System.Windows.Forms.TabPage tabTimeline;
        private System.Windows.Forms.DataGridView dgvTimeline;
        private System.Windows.Forms.TabPage tabCostAdditions;
        private System.Windows.Forms.DataGridView dgvCosts;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnEvent;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnVoucher;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnParty;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnPta;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnNotes;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnCostDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnCostType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnCostAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnCostDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnCostAccount;
    }
}
