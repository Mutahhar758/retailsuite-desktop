namespace ERP
{
    partial class frmReportParameters
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
            this.grpDateRange = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dtpTDate = new System.Windows.Forms.DateTimePicker();
            this.dtpFDate = new System.Windows.Forms.DateTimePicker();
            this.grpAsOn = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpAsOn = new System.Windows.Forms.DateTimePicker();
            this.grpAccounts = new System.Windows.Forms.GroupBox();
            this.cmbAccount = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lblReport = new System.Windows.Forms.Label();
            this.grpItem = new System.Windows.Forms.GroupBox();
            this.cmbItem = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.grpFilter = new System.Windows.Forms.GroupBox();
            this.cmbFilter = new System.Windows.Forms.ComboBox();
            this.txtQty = new ERP.DecimalTextbox(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.pnlControl = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.grpItemCatagory = new System.Windows.Forms.GroupBox();
            this.cmbItemCatagory = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grpType = new System.Windows.Forms.GroupBox();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.grpSelectedAccounts = new System.Windows.Forms.GroupBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.chklstAccounts = new System.Windows.Forms.CheckedListBox();
            this.grpPrintOption = new System.Windows.Forms.GroupBox();
            this.rdoDirectPrint = new System.Windows.Forms.RadioButton();
            this.rdoViewReport = new System.Windows.Forms.RadioButton();
            this.grpDateRange.SuspendLayout();
            this.grpAsOn.SuspendLayout();
            this.grpAccounts.SuspendLayout();
            this.grpItem.SuspendLayout();
            this.grpFilter.SuspendLayout();
            this.pnlControl.SuspendLayout();
            this.grpItemCatagory.SuspendLayout();
            this.grpType.SuspendLayout();
            this.grpSelectedAccounts.SuspendLayout();
            this.grpPrintOption.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbSupplyOrder
            // 
            this.cmbSupplyOrder = new System.Windows.Forms.ComboBox();
            // 
            // grpDateRange
            // 
            this.grpDateRange.Controls.Add(this.label1);
            this.grpDateRange.Controls.Add(this.label7);
            this.grpDateRange.Controls.Add(this.dtpTDate);
            this.grpDateRange.Controls.Add(this.dtpFDate);
            this.grpDateRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDateRange.Location = new System.Drawing.Point(12, 60);
            this.grpDateRange.Name = "grpDateRange";
            this.grpDateRange.Size = new System.Drawing.Size(391, 57);
            this.grpDateRange.TabIndex = 0;
            this.grpDateRange.TabStop = false;
            this.grpDateRange.Text = "Date Range ";
            this.grpDateRange.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(200, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 16);
            this.label1.TabIndex = 196;
            this.label1.Text = "To :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(6, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 16);
            this.label7.TabIndex = 195;
            this.label7.Text = "From :";
            // 
            // dtpTDate
            // 
            this.dtpTDate.CustomFormat = "dd-MMM-yyyy";
            this.dtpTDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTDate.Location = new System.Drawing.Point(238, 23);
            this.dtpTDate.Name = "dtpTDate";
            this.dtpTDate.Size = new System.Drawing.Size(131, 22);
            this.dtpTDate.TabIndex = 194;
            // 
            // dtpFDate
            // 
            this.dtpFDate.CustomFormat = "dd-MMM-yyyy";
            this.dtpFDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFDate.Location = new System.Drawing.Point(57, 23);
            this.dtpFDate.Name = "dtpFDate";
            this.dtpFDate.Size = new System.Drawing.Size(131, 22);
            this.dtpFDate.TabIndex = 193;
            // 
            // grpAsOn
            // 
            this.grpAsOn.Controls.Add(this.label2);
            this.grpAsOn.Controls.Add(this.dtpAsOn);
            this.grpAsOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpAsOn.Location = new System.Drawing.Point(12, 123);
            this.grpAsOn.Name = "grpAsOn";
            this.grpAsOn.Size = new System.Drawing.Size(208, 57);
            this.grpAsOn.TabIndex = 1;
            this.grpAsOn.TabStop = false;
            this.grpAsOn.Text = "As On ";
            this.grpAsOn.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 16);
            this.label2.TabIndex = 198;
            this.label2.Text = "Date :";
            // 
            // dtpAsOn
            // 
            this.dtpAsOn.CustomFormat = "dd-MMM-yyyy";
            this.dtpAsOn.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpAsOn.Location = new System.Drawing.Point(57, 21);
            this.dtpAsOn.Name = "dtpAsOn";
            this.dtpAsOn.Size = new System.Drawing.Size(131, 22);
            this.dtpAsOn.TabIndex = 197;
            // 
            // grpAccounts
            // 
            this.grpAccounts.Controls.Add(this.cmbAccount);
            this.grpAccounts.Controls.Add(this.label10);
            this.grpAccounts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpAccounts.Location = new System.Drawing.Point(12, 186);
            this.grpAccounts.Name = "grpAccounts";
            this.grpAccounts.Size = new System.Drawing.Size(391, 57);
            this.grpAccounts.TabIndex = 1;
            this.grpAccounts.TabStop = false;
            this.grpAccounts.Text = "Account ";
            this.grpAccounts.Visible = false;
            // 
            // cmbAccount
            // 
            this.cmbAccount.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbAccount.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAccount.FormattingEnabled = true;
            this.cmbAccount.Location = new System.Drawing.Point(79, 21);
            this.cmbAccount.Name = "cmbAccount";
            this.cmbAccount.Size = new System.Drawing.Size(290, 24);
            this.cmbAccount.TabIndex = 198;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(11, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 16);
            this.label10.TabIndex = 199;
            this.label10.Text = "Account :";
            // 
            // lblReport
            // 
            this.lblReport.BackColor = System.Drawing.Color.Gainsboro;
            this.lblReport.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReport.Location = new System.Drawing.Point(0, 0);
            this.lblReport.Name = "lblReport";
            this.lblReport.Size = new System.Drawing.Size(631, 46);
            this.lblReport.TabIndex = 196;
            this.lblReport.Text = "Report";
            this.lblReport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpItem
            // 
            this.grpItem.Controls.Add(this.cmbItem);
            this.grpItem.Controls.Add(this.label4);
            this.grpItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpItem.Location = new System.Drawing.Point(12, 249);
            this.grpItem.Name = "grpItem";
            this.grpItem.Size = new System.Drawing.Size(301, 55);
            this.grpItem.TabIndex = 200;
            this.grpItem.TabStop = false;
            this.grpItem.Text = "Item ";
            this.grpItem.Visible = false;
            // 
            // cmbItem
            // 
            this.cmbItem.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbItem.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Location = new System.Drawing.Point(79, 21);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(211, 24);
            this.cmbItem.TabIndex = 200;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(11, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 16);
            this.label4.TabIndex = 201;
            this.label4.Text = "Item :";
            // 
            // grpFilter
            // 
            this.grpFilter.Controls.Add(this.cmbFilter);
            this.grpFilter.Controls.Add(this.txtQty);
            this.grpFilter.Controls.Add(this.label3);
            this.grpFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpFilter.Location = new System.Drawing.Point(12, 307);
            this.grpFilter.Name = "grpFilter";
            this.grpFilter.Size = new System.Drawing.Size(263, 44);
            this.grpFilter.TabIndex = 201;
            this.grpFilter.TabStop = false;
            this.grpFilter.Text = "Filter";
            this.grpFilter.Visible = false;
            // 
            // cmbFilter
            // 
            this.cmbFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilter.FormattingEnabled = true;
            this.cmbFilter.Items.AddRange(new object[] {
            "All",
            "Equal",
            "Greater",
            "Less"});
            this.cmbFilter.Location = new System.Drawing.Point(54, 13);
            this.cmbFilter.Name = "cmbFilter";
            this.cmbFilter.Size = new System.Drawing.Size(91, 24);
            this.cmbFilter.TabIndex = 203;
            this.cmbFilter.SelectedIndexChanged += new System.EventHandler(this.cmbFilter_SelectedIndexChanged);
            // 
            // txtQty
            // 
            this.txtQty.Location = new System.Drawing.Point(151, 14);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(100, 22);
            this.txtQty.TabIndex = 202;
            this.txtQty.Text = "0";
            this.txtQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 16);
            this.label3.TabIndex = 201;
            this.label3.Text = "Qty : ";
            // 
            // pnlControl
            // 
            this.pnlControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlControl.Controls.Add(this.btnClose);
            this.pnlControl.Controls.Add(this.btnPrint);
            this.pnlControl.Location = new System.Drawing.Point(409, 63);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Size = new System.Drawing.Size(165, 42);
            this.pnlControl.TabIndex = 202;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(85, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(73, 29);
            this.btnClose.TabIndex = 206;
            this.btnClose.Tag = "";
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Location = new System.Drawing.Point(6, 6);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(73, 29);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Tag = "";
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // grpItemCatagory
            // 
            this.grpItemCatagory.Controls.Add(this.cmbItemCatagory);
            this.grpItemCatagory.Controls.Add(this.label5);
            this.grpItemCatagory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpItemCatagory.Location = new System.Drawing.Point(239, 125);
            this.grpItemCatagory.Name = "grpItemCatagory";
            this.grpItemCatagory.Size = new System.Drawing.Size(301, 55);
            this.grpItemCatagory.TabIndex = 202;
            this.grpItemCatagory.TabStop = false;
            this.grpItemCatagory.Text = "Catagory";
            this.grpItemCatagory.Visible = false;
            // 
            // cmbItemCatagory
            // 
            this.cmbItemCatagory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbItemCatagory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbItemCatagory.FormattingEnabled = true;
            this.cmbItemCatagory.Location = new System.Drawing.Point(87, 21);
            this.cmbItemCatagory.Name = "cmbItemCatagory";
            this.cmbItemCatagory.Size = new System.Drawing.Size(208, 24);
            this.cmbItemCatagory.TabIndex = 200;
            
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(11, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 16);
            this.label5.TabIndex = 201;
            this.label5.Text = "Catagory :";
            // 
            // grpType
            // 
            this.grpType.Controls.Add(this.cmbType);
            this.grpType.Controls.Add(this.label6);
            this.grpType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpType.Location = new System.Drawing.Point(326, 249);
            this.grpType.Name = "grpType";
            this.grpType.Size = new System.Drawing.Size(301, 55);
            this.grpType.TabIndex = 203;
            this.grpType.TabStop = false;
            this.grpType.Text = "Type";
            this.grpType.Visible = false;
            // 
            // cmbType
            // 
            this.cmbType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(87, 21);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(208, 24);
            this.cmbType.TabIndex = 200;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(11, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 16);
            this.label6.TabIndex = 201;
            this.label6.Text = "Type :";
            // 
            // grpSelectedAccounts
            // 
            this.grpSelectedAccounts.BackColor = System.Drawing.SystemColors.Control;
            this.grpSelectedAccounts.Controls.Add(this.cmbSupplyOrder);
            this.grpSelectedAccounts.Controls.Add(this.chkSelectAll);
            this.grpSelectedAccounts.Controls.Add(this.chklstAccounts);
            this.grpSelectedAccounts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpSelectedAccounts.Location = new System.Drawing.Point(12, 357);
            this.grpSelectedAccounts.Name = "grpSelectedAccounts";
            this.grpSelectedAccounts.Size = new System.Drawing.Size(258, 170);
            this.grpSelectedAccounts.TabIndex = 204;
            this.grpSelectedAccounts.TabStop = false;
            this.grpSelectedAccounts.Text = "Select Accounts ";
            this.grpSelectedAccounts.Visible = false;
            // 
            // cmbSupplyOrder
            // 
            this.cmbSupplyOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSupplyOrder.FormattingEnabled = true;
            this.cmbSupplyOrder.Location = new System.Drawing.Point(5, 20);
            this.cmbSupplyOrder.Name = "cmbSupplyOrder";
            this.cmbSupplyOrder.Size = new System.Drawing.Size(247, 24);
            this.cmbSupplyOrder.TabIndex = 205;
            this.cmbSupplyOrder.SelectedIndexChanged += new System.EventHandler(this.cmbSupplyOrder_SelectedIndexChanged);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(5, 50);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(83, 20);
            this.chkSelectAll.TabIndex = 204;
            this.chkSelectAll.Text = "Select All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // chklstAccounts
            // 
            this.chklstAccounts.BackColor = System.Drawing.SystemColors.Control;
            this.chklstAccounts.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.chklstAccounts.FormattingEnabled = true;
            this.chklstAccounts.Location = new System.Drawing.Point(3, 75);
            this.chklstAccounts.Name = "chklstAccounts";
            this.chklstAccounts.Size = new System.Drawing.Size(252, 89);
            this.chklstAccounts.TabIndex = 203;
            // 
            // grpPrintOption
            // 
            this.grpPrintOption.Controls.Add(this.rdoDirectPrint);
            this.grpPrintOption.Controls.Add(this.rdoViewReport);
            this.grpPrintOption.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPrintOption.Location = new System.Drawing.Point(282, 357);
            this.grpPrintOption.Name = "grpPrintOption";
            this.grpPrintOption.Size = new System.Drawing.Size(258, 55);
            this.grpPrintOption.TabIndex = 205;
            this.grpPrintOption.TabStop = false;
            this.grpPrintOption.Text = "Print Option";
            this.grpPrintOption.Visible = false;
            // 
            // rdoDirectPrint
            // 
            this.rdoDirectPrint.AutoSize = true;
            this.rdoDirectPrint.Location = new System.Drawing.Point(142, 22);
            this.rdoDirectPrint.Name = "rdoDirectPrint";
            this.rdoDirectPrint.Size = new System.Drawing.Size(90, 20);
            this.rdoDirectPrint.TabIndex = 1;
            this.rdoDirectPrint.Text = "Direct Print";
            this.rdoDirectPrint.UseVisualStyleBackColor = true;
            // 
            // rdoViewReport
            // 
            this.rdoViewReport.AutoSize = true;
            this.rdoViewReport.Checked = true;
            this.rdoViewReport.Location = new System.Drawing.Point(20, 22);
            this.rdoViewReport.Name = "rdoViewReport";
            this.rdoViewReport.Size = new System.Drawing.Size(99, 20);
            this.rdoViewReport.TabIndex = 0;
            this.rdoViewReport.TabStop = true;
            this.rdoViewReport.Text = "View Report";
            this.rdoViewReport.UseVisualStyleBackColor = true;
            this.rdoViewReport.CheckedChanged += new System.EventHandler(this.rdoViewReport_CheckedChanged);
            // 
            // frmReportParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 485);
            this.Controls.Add(this.grpPrintOption);
            this.Controls.Add(this.grpSelectedAccounts);
            this.Controls.Add(this.grpType);
            this.Controls.Add(this.grpItemCatagory);
            this.Controls.Add(this.pnlControl);
            this.Controls.Add(this.grpFilter);
            this.Controls.Add(this.grpItem);
            this.Controls.Add(this.lblReport);
            this.Controls.Add(this.grpAsOn);
            this.Controls.Add(this.grpAccounts);
            this.Controls.Add(this.grpDateRange);
            this.Name = "frmReportParameters";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmReportParameters";
            this.Load += new System.EventHandler(this.frmReportParameters_Load);
            this.grpDateRange.ResumeLayout(false);
            this.grpDateRange.PerformLayout();
            this.grpAsOn.ResumeLayout(false);
            this.grpAsOn.PerformLayout();
            this.grpAccounts.ResumeLayout(false);
            this.grpAccounts.PerformLayout();
            this.grpItem.ResumeLayout(false);
            this.grpItem.PerformLayout();
            this.grpFilter.ResumeLayout(false);
            this.grpFilter.PerformLayout();
            this.pnlControl.ResumeLayout(false);
            this.grpItemCatagory.ResumeLayout(false);
            this.grpItemCatagory.PerformLayout();
            this.grpType.ResumeLayout(false);
            this.grpType.PerformLayout();
            this.grpSelectedAccounts.ResumeLayout(false);
            this.grpSelectedAccounts.PerformLayout();
            this.grpPrintOption.ResumeLayout(false);
            this.grpPrintOption.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDateRange;
        private System.Windows.Forms.GroupBox grpAsOn;
        private System.Windows.Forms.GroupBox grpAccounts;
        private System.Windows.Forms.DateTimePicker dtpTDate;
        private System.Windows.Forms.DateTimePicker dtpFDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpAsOn;
        private System.Windows.Forms.Label lblReport;
        private System.Windows.Forms.ComboBox cmbAccount;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox grpItem;
        private System.Windows.Forms.ComboBox cmbItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grpFilter;
        private DecimalTextbox txtQty;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbFilter;
        private System.Windows.Forms.Panel pnlControl;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.GroupBox grpItemCatagory;
        private System.Windows.Forms.ComboBox cmbItemCatagory;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox grpType;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox grpSelectedAccounts;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.CheckedListBox chklstAccounts;
        private System.Windows.Forms.ComboBox cmbSupplyOrder;
        private System.Windows.Forms.GroupBox grpPrintOption;
        private System.Windows.Forms.RadioButton rdoDirectPrint;
        private System.Windows.Forms.RadioButton rdoViewReport;
    }
}