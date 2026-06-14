namespace ERP
{
    partial class frmBankReconcilation
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvBankRec = new System.Windows.Forms.DataGridView();
            this.clnClear = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clnVNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnVDate = new CalendarColumn();
            this.clnChequeDate = new CalendarColumn();
            this.clnCheckno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnRecDate = new CalendarColumn();
            this.clnAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnDr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnCredit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpStatementInfo = new System.Windows.Forms.GroupBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtReconcileBal = new System.Windows.Forms.TextBox();
            this.txtStatementBal = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbBanks = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpTdate = new System.Windows.Forms.DateTimePicker();
            this.dtpFdate = new System.Windows.Forms.DateTimePicker();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.calendarColumn1 = new CalendarColumn();
            this.calendarColumn2 = new CalendarColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.calendarColumn3 = new CalendarColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTotDr = new System.Windows.Forms.Label();
            this.lblTotCr = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtRealBal = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDiff = new System.Windows.Forms.TextBox();
            this.txtClear = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankRec)).BeginInit();
            this.grpStatementInfo.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvBankRec
            // 
            this.dgvBankRec.AllowUserToAddRows = false;
            this.dgvBankRec.AllowUserToDeleteRows = false;
            this.dgvBankRec.AllowUserToOrderColumns = true;
            this.dgvBankRec.AllowUserToResizeColumns = false;
            this.dgvBankRec.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBankRec.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBankRec.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBankRec.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBankRec.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnClear,
            this.clnVNo,
            this.clnVDate,
            this.clnChequeDate,
            this.clnCheckno,
            this.clnRecDate,
            this.clnAccount,
            this.clnDr,
            this.clnCredit});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvBankRec.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvBankRec.Location = new System.Drawing.Point(3, 168);
            this.dgvBankRec.MultiSelect = false;
            this.dgvBankRec.Name = "dgvBankRec";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBankRec.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvBankRec.RowHeadersWidth = 30;
            this.dgvBankRec.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvBankRec.Size = new System.Drawing.Size(944, 202);
            this.dgvBankRec.TabIndex = 163;
            this.dgvBankRec.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBankRec_CellValueChanged);
            this.dgvBankRec.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvBankRec_EditingControlShowing);
            // 
            // clnClear
            // 
            this.clnClear.HeaderText = "Clear";
            this.clnClear.MinimumWidth = 50;
            this.clnClear.Name = "clnClear";
            this.clnClear.Width = 50;
            // 
            // clnVNo
            // 
            this.clnVNo.HeaderText = "Voucher No";
            this.clnVNo.MinimumWidth = 100;
            this.clnVNo.Name = "clnVNo";
            this.clnVNo.ReadOnly = true;
            this.clnVNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clnVNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clnVDate
            // 
            this.clnVDate.HeaderText = "V., Date";
            this.clnVDate.MinimumWidth = 90;
            this.clnVDate.Name = "clnVDate";
            this.clnVDate.ReadOnly = true;
            this.clnVDate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clnVDate.Width = 90;
            // 
            // clnChequeDate
            // 
            this.clnChequeDate.HeaderText = "Cheque Date";
            this.clnChequeDate.Name = "clnChequeDate";
            this.clnChequeDate.ReadOnly = true;
            // 
            // clnCheckno
            // 
            this.clnCheckno.HeaderText = "Cheque No";
            this.clnCheckno.Name = "clnCheckno";
            this.clnCheckno.ReadOnly = true;
            // 
            // clnRecDate
            // 
            this.clnRecDate.HeaderText = "Reconcile Date";
            this.clnRecDate.MinimumWidth = 90;
            this.clnRecDate.Name = "clnRecDate";
            this.clnRecDate.ReadOnly = true;
            this.clnRecDate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clnRecDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.clnRecDate.Width = 90;
            // 
            // clnAccount
            // 
            this.clnAccount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clnAccount.HeaderText = "Account";
            this.clnAccount.MinimumWidth = 180;
            this.clnAccount.Name = "clnAccount";
            this.clnAccount.ReadOnly = true;
            // 
            // clnDr
            // 
            this.clnDr.HeaderText = "Debit";
            this.clnDr.MinimumWidth = 100;
            this.clnDr.Name = "clnDr";
            this.clnDr.ReadOnly = true;
            // 
            // clnCredit
            // 
            this.clnCredit.HeaderText = "Credit";
            this.clnCredit.MinimumWidth = 100;
            this.clnCredit.Name = "clnCredit";
            this.clnCredit.ReadOnly = true;
            // 
            // grpStatementInfo
            // 
            this.grpStatementInfo.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.grpStatementInfo.Controls.Add(this.btnSearch);
            this.grpStatementInfo.Controls.Add(this.txtReconcileBal);
            this.grpStatementInfo.Controls.Add(this.txtStatementBal);
            this.grpStatementInfo.Controls.Add(this.label9);
            this.grpStatementInfo.Controls.Add(this.label8);
            this.grpStatementInfo.Controls.Add(this.cmbBanks);
            this.grpStatementInfo.Controls.Add(this.label11);
            this.grpStatementInfo.Controls.Add(this.label3);
            this.grpStatementInfo.Controls.Add(this.label2);
            this.grpStatementInfo.Controls.Add(this.dtpTdate);
            this.grpStatementInfo.Controls.Add(this.dtpFdate);
            this.grpStatementInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpStatementInfo.Location = new System.Drawing.Point(3, 22);
            this.grpStatementInfo.Name = "grpStatementInfo";
            this.grpStatementInfo.Size = new System.Drawing.Size(809, 87);
            this.grpStatementInfo.TabIndex = 164;
            this.grpStatementInfo.TabStop = false;
            this.grpStatementInfo.Text = "Statement Information";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Location = new System.Drawing.Point(723, 50);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(73, 29);
            this.btnSearch.TabIndex = 167;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtReconcileBal
            // 
            this.txtReconcileBal.Location = new System.Drawing.Point(570, 29);
            this.txtReconcileBal.Name = "txtReconcileBal";
            this.txtReconcileBal.ReadOnly = true;
            this.txtReconcileBal.Size = new System.Drawing.Size(138, 22);
            this.txtReconcileBal.TabIndex = 166;
            // 
            // txtStatementBal
            // 
            this.txtStatementBal.Location = new System.Drawing.Point(570, 56);
            this.txtStatementBal.Name = "txtStatementBal";
            this.txtStatementBal.ReadOnly = true;
            this.txtStatementBal.Size = new System.Drawing.Size(138, 22);
            this.txtStatementBal.TabIndex = 165;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(427, 57);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(127, 16);
            this.label9.TabIndex = 163;
            this.label9.Text = "Statement Balance :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(426, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(128, 16);
            this.label8.TabIndex = 162;
            this.label8.Text = "Reconcile Balance :";
            // 
            // cmbBanks
            // 
            this.cmbBanks.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbBanks.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbBanks.FormattingEnabled = true;
            this.cmbBanks.Location = new System.Drawing.Point(112, 55);
            this.cmbBanks.Name = "cmbBanks";
            this.cmbBanks.Size = new System.Drawing.Size(308, 24);
            this.cmbBanks.TabIndex = 159;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(15, 58);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(96, 16);
            this.label11.TabIndex = 160;
            this.label11.Text = "Bank Account :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(248, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 16);
            this.label3.TabIndex = 75;
            this.label3.Text = "To :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(31, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 16);
            this.label2.TabIndex = 74;
            this.label2.Text = "Date From :";
            // 
            // dtpTdate
            // 
            this.dtpTdate.CustomFormat = "dd-MMM-yyyy";
            this.dtpTdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTdate.Location = new System.Drawing.Point(294, 27);
            this.dtpTdate.Name = "dtpTdate";
            this.dtpTdate.Size = new System.Drawing.Size(126, 22);
            this.dtpTdate.TabIndex = 73;
            // 
            // dtpFdate
            // 
            this.dtpFdate.CustomFormat = "dd-MMM-yyyy";
            this.dtpFdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFdate.Location = new System.Drawing.Point(114, 27);
            this.dtpFdate.Name = "dtpFdate";
            this.dtpFdate.Size = new System.Drawing.Size(120, 22);
            this.dtpFdate.TabIndex = 72;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Voucher No";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 100;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // calendarColumn1
            // 
            this.calendarColumn1.HeaderText = "V., Date";
            this.calendarColumn1.MinimumWidth = 90;
            this.calendarColumn1.Name = "calendarColumn1";
            this.calendarColumn1.ReadOnly = true;
            this.calendarColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.calendarColumn1.Width = 90;
            // 
            // calendarColumn2
            // 
            this.calendarColumn2.HeaderText = "Cheque Date";
            this.calendarColumn2.Name = "calendarColumn2";
            this.calendarColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Cheque No";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // calendarColumn3
            // 
            this.calendarColumn3.HeaderText = "Reconcile Date";
            this.calendarColumn3.MinimumWidth = 90;
            this.calendarColumn3.Name = "calendarColumn3";
            this.calendarColumn3.ReadOnly = true;
            this.calendarColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.calendarColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.calendarColumn3.Width = 90;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.HeaderText = "Account";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 180;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Debit";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 100;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Credit";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 100;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // lblTotDr
            // 
            this.lblTotDr.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblTotDr.AutoSize = true;
            this.lblTotDr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotDr.Location = new System.Drawing.Point(754, 377);
            this.lblTotDr.Name = "lblTotDr";
            this.lblTotDr.Size = new System.Drawing.Size(15, 16);
            this.lblTotDr.TabIndex = 165;
            this.lblTotDr.Text = "0";
            // 
            // lblTotCr
            // 
            this.lblTotCr.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblTotCr.AutoSize = true;
            this.lblTotCr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotCr.Location = new System.Drawing.Point(874, 377);
            this.lblTotCr.Name = "lblTotCr";
            this.lblTotCr.Size = new System.Drawing.Size(15, 16);
            this.lblTotCr.TabIndex = 167;
            this.lblTotCr.Text = "0";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox1.Controls.Add(this.txtRealBal);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtDiff);
            this.groupBox1.Controls.Add(this.txtClear);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 115);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(718, 46);
            this.groupBox1.TabIndex = 168;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Difference ";
            // 
            // txtRealBal
            // 
            this.txtRealBal.Location = new System.Drawing.Point(338, 17);
            this.txtRealBal.Name = "txtRealBal";
            this.txtRealBal.ReadOnly = true;
            this.txtRealBal.Size = new System.Drawing.Size(138, 22);
            this.txtRealBal.TabIndex = 168;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(224, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 16);
            this.label5.TabIndex = 167;
            this.label5.Text = "Real Balance :";
            // 
            // txtDiff
            // 
            this.txtDiff.Location = new System.Drawing.Point(570, 17);
            this.txtDiff.Name = "txtDiff";
            this.txtDiff.ReadOnly = true;
            this.txtDiff.Size = new System.Drawing.Size(138, 22);
            this.txtDiff.TabIndex = 166;
            // 
            // txtClear
            // 
            this.txtClear.Location = new System.Drawing.Point(80, 17);
            this.txtClear.Name = "txtClear";
            this.txtClear.ReadOnly = true;
            this.txtClear.Size = new System.Drawing.Size(138, 22);
            this.txtClear.TabIndex = 165;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 16);
            this.label1.TabIndex = 163;
            this.label1.Text = "Clear  :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(489, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 16);
            this.label4.TabIndex = 162;
            this.label4.Text = "Difference :";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(4, 377);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(73, 29);
            this.btnSave.TabIndex = 169;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(83, 377);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(73, 29);
            this.btnClose.TabIndex = 170;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmBankReconcilation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(950, 418);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblTotCr);
            this.Controls.Add(this.lblTotDr);
            this.Controls.Add(this.grpStatementInfo);
            this.Controls.Add(this.dgvBankRec);
            this.Name = "frmBankReconcilation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bank Reconcilation";
            this.Load += new System.EventHandler(this.frmBankReconcilation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankRec)).EndInit();
            this.grpStatementInfo.ResumeLayout(false);
            this.grpStatementInfo.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvBankRec;
        private System.Windows.Forms.GroupBox grpStatementInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        protected System.Windows.Forms.DateTimePicker dtpTdate;
        protected System.Windows.Forms.DateTimePicker dtpFdate;
        private System.Windows.Forms.ComboBox cmbBanks;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtReconcileBal;
        private System.Windows.Forms.TextBox txtStatementBal;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clnClear;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnVNo;
        private CalendarColumn clnVDate;
        private CalendarColumn clnChequeDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnCheckno;
        private CalendarColumn clnRecDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnDr;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnCredit;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private CalendarColumn calendarColumn1;
        private CalendarColumn calendarColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private CalendarColumn calendarColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblTotDr;
        private System.Windows.Forms.Label lblTotCr;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtDiff;
        private System.Windows.Forms.TextBox txtClear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtRealBal;
        private System.Windows.Forms.Label label5;
    }
}