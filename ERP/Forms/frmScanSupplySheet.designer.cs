namespace ERP.Forms
{
    partial class frmScanSupplySheet
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.chkUseTargetDate = new System.Windows.Forms.CheckBox();
            this.dtpTargetDate = new System.Windows.Forms.DateTimePicker();
            this.lblTargetDateHint = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.pnlPageNav = new System.Windows.Forms.Panel();
            this.btnRemovePage = new System.Windows.Forms.Button();
            this.btnAddPage = new System.Windows.Forms.Button();
            this.cboPages = new System.Windows.Forms.ComboBox();
            this.lblPages = new System.Windows.Forms.Label();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.dgvLines = new System.Windows.Forms.DataGridView();
            this.lblLinesTitle = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.pnlPageNav.SuspendLayout();
            this.pnlRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLines)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.AliceBlue;
            this.pnlTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTop.Controls.Add(this.chkUseTargetDate);
            this.pnlTop.Controls.Add(this.dtpTargetDate);
            this.pnlTop.Controls.Add(this.lblTargetDateHint);
            this.pnlTop.Controls.Add(this.btnBrowse);
            this.pnlTop.Controls.Add(this.btnScan);
            this.pnlTop.Controls.Add(this.lblStatus);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1080, 72);
            this.pnlTop.TabIndex = 0;
            // 
            // chkUseTargetDate
            // 
            this.chkUseTargetDate.AutoSize = true;
            this.chkUseTargetDate.Checked = true;
            this.chkUseTargetDate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseTargetDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseTargetDate.ForeColor = System.Drawing.Color.Navy;
            this.chkUseTargetDate.Location = new System.Drawing.Point(12, 12);
            this.chkUseTargetDate.Name = "chkUseTargetDate";
            this.chkUseTargetDate.Size = new System.Drawing.Size(155, 19);
            this.chkUseTargetDate.TabIndex = 0;
            this.chkUseTargetDate.Text = "Target Extraction Date:";
            this.chkUseTargetDate.UseVisualStyleBackColor = true;
            this.chkUseTargetDate.CheckedChanged += new System.EventHandler(this.chkUseTargetDate_CheckedChanged);
            // 
            // dtpTargetDate
            // 
            this.dtpTargetDate.CustomFormat = "yyyy-MM-dd";
            this.dtpTargetDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTargetDate.Location = new System.Drawing.Point(173, 10);
            this.dtpTargetDate.Name = "dtpTargetDate";
            this.dtpTargetDate.Size = new System.Drawing.Size(110, 22);
            this.dtpTargetDate.TabIndex = 1;
            // 
            // lblTargetDateHint
            // 
            this.lblTargetDateHint.AutoSize = true;
            this.lblTargetDateHint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTargetDateHint.ForeColor = System.Drawing.Color.DimGray;
            this.lblTargetDateHint.Location = new System.Drawing.Point(12, 38);
            this.lblTargetDateHint.Name = "lblTargetDateHint";
            this.lblTargetDateHint.Size = new System.Drawing.Size(515, 13);
            this.lblTargetDateHint.TabIndex = 2;
            this.lblTargetDateHint.Text = "Filter specific date row if sheet is a multi-date pivot/matrix register (customers as columns, dates as rows).";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.Location = new System.Drawing.Point(750, 12);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(140, 32);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "Select Sheet(s)...";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnScan
            // 
            this.btnScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScan.BackColor = System.Drawing.Color.MidnightBlue;
            this.btnScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScan.ForeColor = System.Drawing.Color.White;
            this.btnScan.Location = new System.Drawing.Point(898, 12);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(168, 32);
            this.btnScan.TabIndex = 4;
            this.btnScan.Text = "Scan with AI Vision";
            this.btnScan.UseVisualStyleBackColor = false;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Green;
            this.lblStatus.Location = new System.Drawing.Point(540, 48);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(526, 18);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 72);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.pnlLeft);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pnlRight);
            this.splitContainer.Size = new System.Drawing.Size(1080, 508);
            this.splitContainer.SplitterDistance = 420;
            this.splitContainer.TabIndex = 1;
            // 
            // pnlLeft
            // 
            this.pnlLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLeft.Controls.Add(this.picPreview);
            this.pnlLeft.Controls.Add(this.pnlPageNav);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(420, 508);
            this.pnlLeft.TabIndex = 0;
            // 
            // picPreview
            // 
            this.picPreview.BackColor = System.Drawing.Color.White;
            this.picPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPreview.Location = new System.Drawing.Point(0, 38);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(418, 468);
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPreview.TabIndex = 0;
            this.picPreview.TabStop = false;
            // 
            // pnlPageNav
            // 
            this.pnlPageNav.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pnlPageNav.Controls.Add(this.btnRemovePage);
            this.pnlPageNav.Controls.Add(this.btnAddPage);
            this.pnlPageNav.Controls.Add(this.cboPages);
            this.pnlPageNav.Controls.Add(this.lblPages);
            this.pnlPageNav.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPageNav.Location = new System.Drawing.Point(0, 0);
            this.pnlPageNav.Name = "pnlPageNav";
            this.pnlPageNav.Size = new System.Drawing.Size(418, 38);
            this.pnlPageNav.TabIndex = 1;
            // 
            // btnRemovePage
            // 
            this.btnRemovePage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemovePage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemovePage.ForeColor = System.Drawing.Color.Firebrick;
            this.btnRemovePage.Location = new System.Drawing.Point(348, 6);
            this.btnRemovePage.Name = "btnRemovePage";
            this.btnRemovePage.Size = new System.Drawing.Size(64, 25);
            this.btnRemovePage.TabIndex = 3;
            this.btnRemovePage.Text = "Remove";
            this.btnRemovePage.UseVisualStyleBackColor = true;
            this.btnRemovePage.Click += new System.EventHandler(this.btnRemovePage_Click);
            // 
            // btnAddPage
            // 
            this.btnAddPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddPage.Location = new System.Drawing.Point(268, 6);
            this.btnAddPage.Name = "btnAddPage";
            this.btnAddPage.Size = new System.Drawing.Size(74, 25);
            this.btnAddPage.TabIndex = 2;
            this.btnAddPage.Text = "+ Add Page";
            this.btnAddPage.UseVisualStyleBackColor = true;
            this.btnAddPage.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // cboPages
            // 
            this.cboPages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPages.FormattingEnabled = true;
            this.cboPages.Location = new System.Drawing.Point(62, 7);
            this.cboPages.Name = "cboPages";
            this.cboPages.Size = new System.Drawing.Size(200, 22);
            this.cboPages.TabIndex = 1;
            this.cboPages.SelectedIndexChanged += new System.EventHandler(this.cboPages_SelectedIndexChanged);
            // 
            // lblPages
            // 
            this.lblPages.AutoSize = true;
            this.lblPages.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPages.Location = new System.Drawing.Point(8, 10);
            this.lblPages.Name = "lblPages";
            this.lblPages.Size = new System.Drawing.Size(46, 15);
            this.lblPages.TabIndex = 0;
            this.lblPages.Text = "Sheets:";
            // 
            // pnlRight
            // 
            this.pnlRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRight.Controls.Add(this.dgvLines);
            this.pnlRight.Controls.Add(this.lblLinesTitle);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(0, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(656, 508);
            this.pnlRight.TabIndex = 0;
            // 
            // dgvLines
            // 
            this.dgvLines.AllowUserToAddRows = false;
            this.dgvLines.AllowUserToDeleteRows = false;
            this.dgvLines.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLines.BackgroundColor = System.Drawing.Color.White;
            this.dgvLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLines.Location = new System.Drawing.Point(0, 32);
            this.dgvLines.Name = "dgvLines";
            this.dgvLines.RowHeadersVisible = false;
            this.dgvLines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvLines.Size = new System.Drawing.Size(654, 474);
            this.dgvLines.TabIndex = 0;
            this.dgvLines.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLines_CellValueChanged);
            this.dgvLines.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLines_CellContentClick);
            // 
            // lblLinesTitle
            // 
            this.lblLinesTitle.BackColor = System.Drawing.Color.GhostWhite;
            this.lblLinesTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLinesTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLinesTitle.ForeColor = System.Drawing.Color.Navy;
            this.lblLinesTitle.Location = new System.Drawing.Point(0, 0);
            this.lblLinesTitle.Name = "lblLinesTitle";
            this.lblLinesTitle.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblLinesTitle.Size = new System.Drawing.Size(654, 32);
            this.lblLinesTitle.TabIndex = 1;
            this.lblLinesTitle.Text = "Extracted Customer Deliveries (Review && Adjust):";
            this.lblLinesTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.SystemColors.Control;
            this.pnlBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBottom.Controls.Add(this.lblSummary);
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Controls.Add(this.btnImport);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 580);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1080, 48);
            this.pnlBottom.TabIndex = 2;
            // 
            // lblSummary
            // 
            this.lblSummary.AutoSize = true;
            this.lblSummary.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSummary.ForeColor = System.Drawing.Color.Black;
            this.lblSummary.Location = new System.Drawing.Point(12, 16);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(183, 15);
            this.lblSummary.TabIndex = 0;
            this.lblSummary.Text = "Select sheet images and click Scan.";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(868, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(86, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.BackColor = System.Drawing.Color.MidnightBlue;
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImport.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImport.ForeColor = System.Drawing.Color.White;
            this.btnImport.Location = new System.Drawing.Point(960, 8);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(106, 30);
            this.btnImport.TabIndex = 2;
            this.btnImport.Text = "Import Lines";
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // frmScanSupplySheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1080, 628);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlBottom);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.Name = "frmScanSupplySheet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AI Handwritten Sheet & Register Scanner";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.pnlPageNav.ResumeLayout(false);
            this.pnlPageNav.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLines)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.CheckBox chkUseTargetDate;
        private System.Windows.Forms.DateTimePicker dtpTargetDate;
        private System.Windows.Forms.Label lblTargetDateHint;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Panel pnlPageNav;
        private System.Windows.Forms.Label lblPages;
        private System.Windows.Forms.ComboBox cboPages;
        private System.Windows.Forms.Button btnAddPage;
        private System.Windows.Forms.Button btnRemovePage;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Label lblLinesTitle;
        private System.Windows.Forms.DataGridView dgvLines;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnImport;
    }
}
