namespace ERP.Forms
{
    partial class frmConfiguration
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPrinter = new System.Windows.Forms.TabPage();
            this.lblPrinterInfo = new System.Windows.Forms.Label();
            this.cmbPrinter = new System.Windows.Forms.ComboBox();
            this.lblNarration = new System.Windows.Forms.Label();
            this.tabQrPayment = new System.Windows.Forms.TabPage();
            this.lblPreviewAmount = new System.Windows.Forms.Label();
            this.lblQrHint = new System.Windows.Forms.Label();
            this.picQrPreview = new System.Windows.Forms.PictureBox();
            this.lblPreviewTitle = new System.Windows.Forms.Label();
            this.txtAccountNumber = new System.Windows.Forms.TextBox();
            this.lblAccountNumber = new System.Windows.Forms.Label();
            this.txtAccountTitle = new System.Windows.Forms.TextBox();
            this.lblAccountTitle = new System.Windows.Forms.Label();
            this.txtBankName = new System.Windows.Forms.TextBox();
            this.lblBankName = new System.Windows.Forms.Label();
            this.chkQrEnabled = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPrinter.SuspendLayout();
            this.tabQrPayment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQrPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPrinter);
            this.tabControl1.Controls.Add(this.tabQrPayment);
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(484, 335);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPrinter
            // 
            this.tabPrinter.BackColor = System.Drawing.SystemColors.Window;
            this.tabPrinter.Controls.Add(this.lblPrinterInfo);
            this.tabPrinter.Controls.Add(this.cmbPrinter);
            this.tabPrinter.Controls.Add(this.lblNarration);
            this.tabPrinter.Location = new System.Drawing.Point(4, 24);
            this.tabPrinter.Name = "tabPrinter";
            this.tabPrinter.Padding = new System.Windows.Forms.Padding(3);
            this.tabPrinter.Size = new System.Drawing.Size(476, 307);
            this.tabPrinter.TabIndex = 0;
            this.tabPrinter.Text = "Thermal Printer";
            // 
            // lblPrinterInfo
            // 
            this.lblPrinterInfo.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrinterInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPrinterInfo.Location = new System.Drawing.Point(20, 75);
            this.lblPrinterInfo.Name = "lblPrinterInfo";
            this.lblPrinterInfo.Size = new System.Drawing.Size(425, 40);
            this.lblPrinterInfo.TabIndex = 221;
            this.lblPrinterInfo.Text = "Select the thermal receipt printer installed on this computer. Used for printing " +
    "standard and feed mill customer slips and thermal receipts.";
            // 
            // cmbPrinter
            // 
            this.cmbPrinter.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbPrinter.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrinter.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPrinter.FormattingEnabled = true;
            this.cmbPrinter.Location = new System.Drawing.Point(23, 42);
            this.cmbPrinter.Name = "cmbPrinter";
            this.cmbPrinter.Size = new System.Drawing.Size(350, 25);
            this.cmbPrinter.TabIndex = 218;
            // 
            // lblNarration
            // 
            this.lblNarration.AutoSize = true;
            this.lblNarration.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNarration.Location = new System.Drawing.Point(20, 19);
            this.lblNarration.Name = "lblNarration";
            this.lblNarration.Size = new System.Drawing.Size(155, 17);
            this.lblNarration.TabIndex = 219;
            this.lblNarration.Text = "Thermal Receipt Printer :";
            // 
            // tabQrPayment
            // 
            this.tabQrPayment.BackColor = System.Drawing.SystemColors.Window;
            this.tabQrPayment.Controls.Add(this.lblPreviewAmount);
            this.tabQrPayment.Controls.Add(this.lblQrHint);
            this.tabQrPayment.Controls.Add(this.picQrPreview);
            this.tabQrPayment.Controls.Add(this.lblPreviewTitle);
            this.tabQrPayment.Controls.Add(this.txtAccountNumber);
            this.tabQrPayment.Controls.Add(this.lblAccountNumber);
            this.tabQrPayment.Controls.Add(this.txtAccountTitle);
            this.tabQrPayment.Controls.Add(this.lblAccountTitle);
            this.tabQrPayment.Controls.Add(this.txtBankName);
            this.tabQrPayment.Controls.Add(this.lblBankName);
            this.tabQrPayment.Controls.Add(this.chkQrEnabled);
            this.tabQrPayment.Location = new System.Drawing.Point(4, 24);
            this.tabQrPayment.Name = "tabQrPayment";
            this.tabQrPayment.Padding = new System.Windows.Forms.Padding(3);
            this.tabQrPayment.Size = new System.Drawing.Size(476, 307);
            this.tabQrPayment.TabIndex = 1;
            this.tabQrPayment.Text = "QR Code Payment";
            // 
            // lblPreviewAmount
            // 
            this.lblPreviewAmount.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPreviewAmount.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPreviewAmount.Location = new System.Drawing.Point(270, 240);
            this.lblPreviewAmount.Name = "lblPreviewAmount";
            this.lblPreviewAmount.Size = new System.Drawing.Size(185, 45);
            this.lblPreviewAmount.TabIndex = 10;
            this.lblPreviewAmount.Text = "Note: On printed bills, the customer\'s actual net balance will be embedded automatically.";
            this.lblPreviewAmount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblQrHint
            // 
            this.lblQrHint.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQrHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblQrHint.Location = new System.Drawing.Point(18, 222);
            this.lblQrHint.Name = "lblQrHint";
            this.lblQrHint.Size = new System.Drawing.Size(232, 45);
            this.lblQrHint.TabIndex = 9;
            this.lblQrHint.Text = "Customers scan with Meezan, HBL, Alfalah, UBL, JazzCash, Easypaisa or any Raast/1Link bank app.";
            // 
            // picQrPreview
            // 
            this.picQrPreview.BackColor = System.Drawing.Color.White;
            this.picQrPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picQrPreview.Location = new System.Drawing.Point(275, 58);
            this.picQrPreview.Name = "picQrPreview";
            this.picQrPreview.Size = new System.Drawing.Size(175, 175);
            this.picQrPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picQrPreview.TabIndex = 8;
            this.picQrPreview.TabStop = false;
            // 
            // lblPreviewTitle
            // 
            this.lblPreviewTitle.AutoSize = true;
            this.lblPreviewTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPreviewTitle.Location = new System.Drawing.Point(272, 38);
            this.lblPreviewTitle.Name = "lblPreviewTitle";
            this.lblPreviewTitle.Size = new System.Drawing.Size(95, 15);
            this.lblPreviewTitle.TabIndex = 7;
            this.lblPreviewTitle.Text = "Live QR Preview :";
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccountNumber.Location = new System.Drawing.Point(20, 186);
            this.txtAccountNumber.MaxLength = 34;
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.Size = new System.Drawing.Size(230, 23);
            this.txtAccountNumber.TabIndex = 6;
            this.txtAccountNumber.TextChanged += new System.EventHandler(this.OnQrFieldChanged);
            // 
            // lblAccountNumber
            // 
            this.lblAccountNumber.AutoSize = true;
            this.lblAccountNumber.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccountNumber.Location = new System.Drawing.Point(17, 168);
            this.lblAccountNumber.Name = "lblAccountNumber";
            this.lblAccountNumber.Size = new System.Drawing.Size(155, 15);
            this.lblAccountNumber.TabIndex = 5;
            this.lblAccountNumber.Text = "IBAN / Account / Raast ID :";
            // 
            // txtAccountTitle
            // 
            this.txtAccountTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccountTitle.Location = new System.Drawing.Point(20, 133);
            this.txtAccountTitle.MaxLength = 25;
            this.txtAccountTitle.Name = "txtAccountTitle";
            this.txtAccountTitle.Size = new System.Drawing.Size(230, 23);
            this.txtAccountTitle.TabIndex = 4;
            this.txtAccountTitle.TextChanged += new System.EventHandler(this.OnQrFieldChanged);
            // 
            // lblAccountTitle
            // 
            this.lblAccountTitle.AutoSize = true;
            this.lblAccountTitle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccountTitle.Location = new System.Drawing.Point(17, 115);
            this.lblAccountTitle.Name = "lblAccountTitle";
            this.lblAccountTitle.Size = new System.Drawing.Size(125, 15);
            this.lblAccountTitle.TabIndex = 3;
            this.lblAccountTitle.Text = "Account / Store Title :";
            // 
            // txtBankName
            // 
            this.txtBankName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBankName.Location = new System.Drawing.Point(20, 80);
            this.txtBankName.MaxLength = 50;
            this.txtBankName.Name = "txtBankName";
            this.txtBankName.Size = new System.Drawing.Size(230, 23);
            this.txtBankName.TabIndex = 2;
            this.txtBankName.TextChanged += new System.EventHandler(this.OnQrFieldChanged);
            // 
            // lblBankName
            // 
            this.lblBankName.AutoSize = true;
            this.lblBankName.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBankName.Location = new System.Drawing.Point(17, 62);
            this.lblBankName.Name = "lblBankName";
            this.lblBankName.Size = new System.Drawing.Size(122, 15);
            this.lblBankName.TabIndex = 1;
            this.lblBankName.Text = "Bank / Wallet Name :";
            // 
            // chkQrEnabled
            // 
            this.chkQrEnabled.AutoSize = true;
            this.chkQrEnabled.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkQrEnabled.Location = new System.Drawing.Point(20, 20);
            this.chkQrEnabled.Name = "chkQrEnabled";
            this.chkQrEnabled.Size = new System.Drawing.Size(262, 19);
            this.chkQrEnabled.TabIndex = 0;
            this.chkQrEnabled.Text = "Enable QR Code Payment on Customer Bills";
            this.chkQrEnabled.UseVisualStyleBackColor = true;
            this.chkQrEnabled.CheckedChanged += new System.EventHandler(this.chkQrEnabled_CheckedChanged);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(312, 355);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(95, 30);
            this.btnSave.TabIndex = 220;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(413, 355);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 30);
            this.btnClose.TabIndex = 221;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblStatus.Location = new System.Drawing.Point(12, 360);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(280, 20);
            this.lblStatus.TabIndex = 222;
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(508, 396);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfiguration";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "System Configuration";
            this.Load += new System.EventHandler(this.frmConfiguration_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPrinter.ResumeLayout(false);
            this.tabPrinter.PerformLayout();
            this.tabQrPayment.ResumeLayout(false);
            this.tabQrPayment.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQrPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPrinter;
        private System.Windows.Forms.TabPage tabQrPayment;
        private System.Windows.Forms.ComboBox cmbPrinter;
        private System.Windows.Forms.Label lblNarration;
        private System.Windows.Forms.Label lblPrinterInfo;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkQrEnabled;
        private System.Windows.Forms.TextBox txtBankName;
        private System.Windows.Forms.Label lblBankName;
        private System.Windows.Forms.TextBox txtAccountTitle;
        private System.Windows.Forms.Label lblAccountTitle;
        private System.Windows.Forms.TextBox txtAccountNumber;
        private System.Windows.Forms.Label lblAccountNumber;
        private System.Windows.Forms.Label lblPreviewTitle;
        private System.Windows.Forms.PictureBox picQrPreview;
        private System.Windows.Forms.Label lblQrHint;
        private System.Windows.Forms.Label lblPreviewAmount;
    }
}