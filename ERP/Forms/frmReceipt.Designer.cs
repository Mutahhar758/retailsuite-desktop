namespace ERP
{
    partial class frmReceipt
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReceipt));
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.calendarColumn1 = new CalendarColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabDetailQuery = new System.Windows.Forms.TabControl();
            this.tabpgDetail = new System.Windows.Forms.TabPage();
            this.dgvExpenses = new System.Windows.Forms.DataGridView();
            this.clnSeq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnAccount = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clnCheque = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnChequeDate = new CalendarColumn();
            this.clnAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnRemarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnExpCreatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnExpEditBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblBalance = new System.Windows.Forms.Label();
            this.lblTotAmount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnEnd = new System.Windows.Forms.Button();
            this.btnPri = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.grpInvoiceDetail = new System.Windows.Forms.GroupBox();
            this.cmbNarration = new System.Windows.Forms.ComboBox();
            this.lblNarration = new System.Windows.Forms.Label();
            this.cmbCashBanks = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtEditBy = new System.Windows.Forms.TextBox();
            this.txtCreatedBy = new System.Windows.Forms.TextBox();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.txtVoucherNo = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbFilterAccounts = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtTdate = new System.Windows.Forms.TextBox();
            this.txtFdate = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbFilterNarration = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbFilterCashBanks = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.dgvQuery = new System.Windows.Forms.DataGridView();
            this.clnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnVoucherNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClnCreatedby = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnEditby = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label17 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.addNewAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabDetailQuery.SuspendLayout();
            this.tabpgDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).BeginInit();
            this.panel1.SuspendLayout();
            this.grpInvoiceDetail.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuery)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Cheque";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 100;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // calendarColumn1
            // 
            this.calendarColumn1.HeaderText = "ChequeDate";
            this.calendarColumn1.Name = "calendarColumn1";
            this.calendarColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.calendarColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Amount";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 120;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 120;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Narration";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 180;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 180;
            // 
            // tabDetailQuery
            // 
            this.tabDetailQuery.Controls.Add(this.tabpgDetail);
            this.tabDetailQuery.Controls.Add(this.tabPage4);
            this.tabDetailQuery.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabDetailQuery.Location = new System.Drawing.Point(0, 63);
            this.tabDetailQuery.Name = "tabDetailQuery";
            this.tabDetailQuery.SelectedIndex = 0;
            this.tabDetailQuery.Size = new System.Drawing.Size(934, 504);
            this.tabDetailQuery.TabIndex = 0;
            this.tabDetailQuery.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tabDetailQuery_KeyDown);
            // 
            // tabpgDetail
            // 
            this.tabpgDetail.Controls.Add(this.dgvExpenses);
            this.tabpgDetail.Controls.Add(this.lblBalance);
            this.tabpgDetail.Controls.Add(this.lblTotAmount);
            this.tabpgDetail.Controls.Add(this.label1);
            this.tabpgDetail.Controls.Add(this.panel1);
            this.tabpgDetail.Controls.Add(this.grpInvoiceDetail);
            this.tabpgDetail.Location = new System.Drawing.Point(4, 22);
            this.tabpgDetail.Name = "tabpgDetail";
            this.tabpgDetail.Padding = new System.Windows.Forms.Padding(3);
            this.tabpgDetail.Size = new System.Drawing.Size(926, 478);
            this.tabpgDetail.TabIndex = 0;
            this.tabpgDetail.Text = "Detail";
            this.tabpgDetail.UseVisualStyleBackColor = true;
            // 
            // dgvExpenses
            // 
            this.dgvExpenses.AllowUserToAddRows = false;
            this.dgvExpenses.AllowUserToDeleteRows = false;
            this.dgvExpenses.AllowUserToOrderColumns = true;
            this.dgvExpenses.AllowUserToResizeColumns = false;
            this.dgvExpenses.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvExpenses.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvExpenses.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExpenses.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnSeq,
            this.clnAccount,
            this.clnCheque,
            this.clnChequeDate,
            this.clnAmount,
            this.clnRemarks,
            this.clnExpCreatedBy,
            this.clnExpEditBy});
            this.dgvExpenses.Location = new System.Drawing.Point(6, 126);
            this.dgvExpenses.MultiSelect = false;
            this.dgvExpenses.Name = "dgvExpenses";
            this.dgvExpenses.RowHeadersWidth = 30;
            this.dgvExpenses.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvExpenses.Size = new System.Drawing.Size(890, 241);
            this.dgvExpenses.TabIndex = 1;
            this.dgvExpenses.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dgvExpenses.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvExpenses_CellMouseUp);
            this.dgvExpenses.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dgvExpenses.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView1_EditingControlShowing);
            this.dgvExpenses.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvExpenses_RowEnter);
            this.dgvExpenses.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            this.dgvExpenses.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            this.dgvExpenses.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            // 
            // clnSeq
            // 
            this.clnSeq.HeaderText = "Seq";
            this.clnSeq.Name = "clnSeq";
            this.clnSeq.ReadOnly = true;
            this.clnSeq.Visible = false;
            // 
            // clnAccount
            // 
            this.clnAccount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clnAccount.DefaultCellStyle = dataGridViewCellStyle9;
            this.clnAccount.FillWeight = 50F;
            this.clnAccount.HeaderText = "Account";
            this.clnAccount.MinimumWidth = 120;
            this.clnAccount.Name = "clnAccount";
            this.clnAccount.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clnAccount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // clnCheque
            // 
            this.clnCheque.HeaderText = "Cheque";
            this.clnCheque.MinimumWidth = 100;
            this.clnCheque.Name = "clnCheque";
            // 
            // clnChequeDate
            // 
            this.clnChequeDate.HeaderText = "ChequeDate";
            this.clnChequeDate.Name = "clnChequeDate";
            this.clnChequeDate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clnChequeDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // clnAmount
            // 
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clnAmount.DefaultCellStyle = dataGridViewCellStyle10;
            this.clnAmount.HeaderText = "Amount";
            this.clnAmount.MinimumWidth = 120;
            this.clnAmount.Name = "clnAmount";
            this.clnAmount.Width = 120;
            // 
            // clnRemarks
            // 
            this.clnRemarks.HeaderText = "Remarks";
            this.clnRemarks.MinimumWidth = 180;
            this.clnRemarks.Name = "clnRemarks";
            this.clnRemarks.Width = 180;
            // 
            // clnExpCreatedBy
            // 
            this.clnExpCreatedBy.HeaderText = "Created By";
            this.clnExpCreatedBy.Name = "clnExpCreatedBy";
            this.clnExpCreatedBy.ReadOnly = true;
            this.clnExpCreatedBy.Visible = false;
            // 
            // clnExpEditBy
            // 
            this.clnExpEditBy.HeaderText = "Edit By";
            this.clnExpEditBy.Name = "clnExpEditBy";
            this.clnExpEditBy.ReadOnly = true;
            this.clnExpEditBy.Visible = false;
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBalance.Location = new System.Drawing.Point(78, 405);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(69, 19);
            this.lblBalance.TabIndex = 177;
            this.lblBalance.Text = "Balance :";
            // 
            // lblTotAmount
            // 
            this.lblTotAmount.AutoSize = true;
            this.lblTotAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotAmount.Location = new System.Drawing.Point(614, 407);
            this.lblTotAmount.Name = "lblTotAmount";
            this.lblTotAmount.Size = new System.Drawing.Size(15, 16);
            this.lblTotAmount.TabIndex = 176;
            this.lblTotAmount.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 405);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 19);
            this.label1.TabIndex = 175;
            this.label1.Text = "Balance :";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnNext);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnEnd);
            this.panel1.Controls.Add(this.btnPri);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnHome);
            this.panel1.Controls.Add(this.btnNew);
            this.panel1.Location = new System.Drawing.Point(263, 405);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(324, 68);
            this.panel1.TabIndex = 2;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(165, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(73, 29);
            this.btnDelete.TabIndex = 166;
            this.btnDelete.TabStop = false;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.Location = new System.Drawing.Point(164, 35);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(36, 29);
            this.btnNext.TabIndex = 164;
            this.btnNext.TabStop = false;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(245, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(73, 29);
            this.btnClose.TabIndex = 163;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnEnd
            // 
            this.btnEnd.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnEnd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnd.Location = new System.Drawing.Point(206, 35);
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Size = new System.Drawing.Size(36, 29);
            this.btnEnd.TabIndex = 163;
            this.btnEnd.TabStop = false;
            this.btnEnd.Text = ">>";
            this.btnEnd.UseVisualStyleBackColor = false;
            this.btnEnd.Click += new System.EventHandler(this.btnEnd_Click);
            // 
            // btnPri
            // 
            this.btnPri.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnPri.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPri.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPri.Location = new System.Drawing.Point(122, 35);
            this.btnPri.Name = "btnPri";
            this.btnPri.Size = new System.Drawing.Size(36, 29);
            this.btnPri.TabIndex = 162;
            this.btnPri.TabStop = false;
            this.btnPri.Text = "<";
            this.btnPri.UseVisualStyleBackColor = false;
            this.btnPri.Click += new System.EventHandler(this.btnPri_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(85, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(73, 29);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHome.Location = new System.Drawing.Point(80, 35);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(36, 29);
            this.btnHome.TabIndex = 161;
            this.btnHome.TabStop = false;
            this.btnHome.Text = "<<";
            this.btnHome.UseVisualStyleBackColor = false;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Location = new System.Drawing.Point(5, 3);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(73, 29);
            this.btnNew.TabIndex = 161;
            this.btnNew.Text = "&New";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // grpInvoiceDetail
            // 
            this.grpInvoiceDetail.Controls.Add(this.cmbNarration);
            this.grpInvoiceDetail.Controls.Add(this.lblNarration);
            this.grpInvoiceDetail.Controls.Add(this.cmbCashBanks);
            this.grpInvoiceDetail.Controls.Add(this.label11);
            this.grpInvoiceDetail.Controls.Add(this.label13);
            this.grpInvoiceDetail.Controls.Add(this.txtEditBy);
            this.grpInvoiceDetail.Controls.Add(this.txtCreatedBy);
            this.grpInvoiceDetail.Controls.Add(this.dtpDate);
            this.grpInvoiceDetail.Controls.Add(this.txtVoucherNo);
            this.grpInvoiceDetail.Controls.Add(this.label9);
            this.grpInvoiceDetail.Controls.Add(this.label8);
            this.grpInvoiceDetail.Controls.Add(this.label7);
            this.grpInvoiceDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpInvoiceDetail.Location = new System.Drawing.Point(5, 25);
            this.grpInvoiceDetail.Name = "grpInvoiceDetail";
            this.grpInvoiceDetail.Size = new System.Drawing.Size(732, 95);
            this.grpInvoiceDetail.TabIndex = 0;
            this.grpInvoiceDetail.TabStop = false;
            this.grpInvoiceDetail.Text = "Invoice Detail ";
            // 
            // cmbNarration
            // 
            this.cmbNarration.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbNarration.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbNarration.FormattingEnabled = true;
            this.cmbNarration.Location = new System.Drawing.Point(407, 61);
            this.cmbNarration.Name = "cmbNarration";
            this.cmbNarration.Size = new System.Drawing.Size(317, 24);
            this.cmbNarration.TabIndex = 2;
            // 
            // lblNarration
            // 
            this.lblNarration.AutoSize = true;
            this.lblNarration.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNarration.Location = new System.Drawing.Point(289, 64);
            this.lblNarration.Name = "lblNarration";
            this.lblNarration.Size = new System.Drawing.Size(69, 16);
            this.lblNarration.TabIndex = 174;
            this.lblNarration.Text = "Narration :";
            // 
            // cmbCashBanks
            // 
            this.cmbCashBanks.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbCashBanks.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbCashBanks.FormattingEnabled = true;
            this.cmbCashBanks.Location = new System.Drawing.Point(407, 36);
            this.cmbCashBanks.Name = "cmbCashBanks";
            this.cmbCashBanks.Size = new System.Drawing.Size(317, 24);
            this.cmbCashBanks.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(289, 39);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(113, 16);
            this.label11.TabIndex = 158;
            this.label11.Text = "Cash And Banks :";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(289, 14);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(43, 16);
            this.label13.TabIndex = 172;
            this.label13.Text = "Date :";
            // 
            // txtEditBy
            // 
            this.txtEditBy.Location = new System.Drawing.Point(92, 63);
            this.txtEditBy.Name = "txtEditBy";
            this.txtEditBy.ReadOnly = true;
            this.txtEditBy.Size = new System.Drawing.Size(183, 22);
            this.txtEditBy.TabIndex = 161;
            this.txtEditBy.TabStop = false;
            // 
            // txtCreatedBy
            // 
            this.txtCreatedBy.Location = new System.Drawing.Point(92, 39);
            this.txtCreatedBy.Name = "txtCreatedBy";
            this.txtCreatedBy.ReadOnly = true;
            this.txtCreatedBy.Size = new System.Drawing.Size(183, 22);
            this.txtCreatedBy.TabIndex = 160;
            this.txtCreatedBy.TabStop = false;
            // 
            // dtpDate
            // 
            this.dtpDate.CustomFormat = "dd-MMM-yyyy";
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(407, 12);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(125, 22);
            this.dtpDate.TabIndex = 0;
            // 
            // txtVoucherNo
            // 
            this.txtVoucherNo.Location = new System.Drawing.Point(92, 15);
            this.txtVoucherNo.Name = "txtVoucherNo";
            this.txtVoucherNo.ReadOnly = true;
            this.txtVoucherNo.Size = new System.Drawing.Size(123, 22);
            this.txtVoucherNo.TabIndex = 157;
            this.txtVoucherNo.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(10, 66);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 16);
            this.label9.TabIndex = 159;
            this.label9.Text = "Edit By :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(10, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 16);
            this.label8.TabIndex = 158;
            this.label8.Text = "Created By :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(10, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 16);
            this.label7.TabIndex = 157;
            this.label7.Text = "Voucher # :";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox1);
            this.tabPage4.Controls.Add(this.dgvQuery);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(926, 478);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Query";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cmbFilterAccounts);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtTdate);
            this.groupBox1.Controls.Add(this.txtFdate);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbFilterNarration);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbFilterCashBanks);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnFind);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(6, 327);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(900, 111);
            this.groupBox1.TabIndex = 188;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(17, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 16);
            this.label4.TabIndex = 198;
            this.label4.Text = "Date :";
            // 
            // cmbFilterAccounts
            // 
            this.cmbFilterAccounts.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbFilterAccounts.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbFilterAccounts.FormattingEnabled = true;
            this.cmbFilterAccounts.Location = new System.Drawing.Point(537, 21);
            this.cmbFilterAccounts.Name = "cmbFilterAccounts";
            this.cmbFilterAccounts.Size = new System.Drawing.Size(335, 24);
            this.cmbFilterAccounts.TabIndex = 196;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(455, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 16);
            this.label10.TabIndex = 197;
            this.label10.Text = "Account :";
            // 
            // txtTdate
            // 
            this.txtTdate.Location = new System.Drawing.Point(339, 21);
            this.txtTdate.Name = "txtTdate";
            this.txtTdate.Size = new System.Drawing.Size(103, 22);
            this.txtTdate.TabIndex = 195;
            this.txtTdate.Validated += new System.EventHandler(this.txtFdate_Validated);
            // 
            // txtFdate
            // 
            this.txtFdate.Location = new System.Drawing.Point(179, 21);
            this.txtFdate.Name = "txtFdate";
            this.txtFdate.Size = new System.Drawing.Size(108, 22);
            this.txtFdate.TabIndex = 194;
            this.txtFdate.Validated += new System.EventHandler(this.txtFdate_Validated);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(293, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 16);
            this.label6.TabIndex = 193;
            this.label6.Text = "To :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(127, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 16);
            this.label5.TabIndex = 192;
            this.label5.Text = "From :";
            // 
            // cmbFilterNarration
            // 
            this.cmbFilterNarration.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbFilterNarration.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbFilterNarration.FormattingEnabled = true;
            this.cmbFilterNarration.Location = new System.Drawing.Point(537, 48);
            this.cmbFilterNarration.Name = "cmbFilterNarration";
            this.cmbFilterNarration.Size = new System.Drawing.Size(335, 24);
            this.cmbFilterNarration.TabIndex = 190;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(455, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 16);
            this.label2.TabIndex = 191;
            this.label2.Text = "Narration :";
            // 
            // cmbFilterCashBanks
            // 
            this.cmbFilterCashBanks.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbFilterCashBanks.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbFilterCashBanks.FormattingEnabled = true;
            this.cmbFilterCashBanks.Location = new System.Drawing.Point(135, 47);
            this.cmbFilterCashBanks.Name = "cmbFilterCashBanks";
            this.cmbFilterCashBanks.Size = new System.Drawing.Size(307, 24);
            this.cmbFilterCashBanks.TabIndex = 188;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(17, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 16);
            this.label3.TabIndex = 189;
            this.label3.Text = "Cash And Banks :";
            // 
            // btnFind
            // 
            this.btnFind.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFind.Location = new System.Drawing.Point(799, 76);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(73, 29);
            this.btnFind.TabIndex = 165;
            this.btnFind.Text = "&Find";
            this.btnFind.UseVisualStyleBackColor = false;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // dgvQuery
            // 
            this.dgvQuery.AllowUserToAddRows = false;
            this.dgvQuery.AllowUserToDeleteRows = false;
            this.dgvQuery.AllowUserToOrderColumns = true;
            this.dgvQuery.AllowUserToResizeColumns = false;
            this.dgvQuery.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvQuery.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dgvQuery.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvQuery.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnDate,
            this.clnVoucherNum,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn9,
            this.ClnCreatedby,
            this.clnEditby});
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvQuery.DefaultCellStyle = dataGridViewCellStyle13;
            this.dgvQuery.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvQuery.Location = new System.Drawing.Point(3, 3);
            this.dgvQuery.MultiSelect = false;
            this.dgvQuery.Name = "dgvQuery";
            this.dgvQuery.ReadOnly = true;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvQuery.RowHeadersDefaultCellStyle = dataGridViewCellStyle14;
            this.dgvQuery.RowHeadersWidth = 30;
            this.dgvQuery.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvQuery.Size = new System.Drawing.Size(920, 318);
            this.dgvQuery.TabIndex = 164;
            this.dgvQuery.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvQuery_CellDoubleClick);
            this.dgvQuery.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvQuery_KeyDown);
            // 
            // clnDate
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.Format = "dd-MMM-yyyy";
            dataGridViewCellStyle12.NullValue = null;
            this.clnDate.DefaultCellStyle = dataGridViewCellStyle12;
            this.clnDate.FillWeight = 50F;
            this.clnDate.HeaderText = "Date";
            this.clnDate.MinimumWidth = 110;
            this.clnDate.Name = "clnDate";
            this.clnDate.ReadOnly = true;
            this.clnDate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clnDate.Width = 110;
            // 
            // clnVoucherNum
            // 
            this.clnVoucherNum.HeaderText = "Voucher #";
            this.clnVoucherNum.Name = "clnVoucherNum";
            this.clnVoucherNum.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "Amount";
            this.dataGridViewTextBoxColumn8.MinimumWidth = 120;
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            this.dataGridViewTextBoxColumn8.Width = 120;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn9.HeaderText = "Narration";
            this.dataGridViewTextBoxColumn9.MinimumWidth = 200;
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // ClnCreatedby
            // 
            this.ClnCreatedby.HeaderText = "Created By";
            this.ClnCreatedby.MinimumWidth = 180;
            this.ClnCreatedby.Name = "ClnCreatedby";
            this.ClnCreatedby.ReadOnly = true;
            this.ClnCreatedby.Width = 180;
            // 
            // clnEditby
            // 
            this.clnEditby.HeaderText = "Edit By";
            this.clnEditby.MinimumWidth = 180;
            this.clnEditby.Name = "clnEditby";
            this.clnEditby.ReadOnly = true;
            this.clnEditby.Width = 180;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteRecordToolStripMenuItem,
            this.addNewAccountToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(172, 70);
            // 
            // deleteRecordToolStripMenuItem
            // 
            this.deleteRecordToolStripMenuItem.Name = "deleteRecordToolStripMenuItem";
            this.deleteRecordToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.deleteRecordToolStripMenuItem.Text = "Delete Record";
            this.deleteRecordToolStripMenuItem.Click += new System.EventHandler(this.deleteRecordToolStripMenuItem_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label17.Location = new System.Drawing.Point(422, 9);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(185, 59);
            this.label17.TabIndex = 218;
            this.label17.Text = "RECEIPT";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(348, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(84, 59);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 219;
            this.pictureBox1.TabStop = false;
            // 
            // addNewAccountToolStripMenuItem
            // 
            this.addNewAccountToolStripMenuItem.Name = "addNewAccountToolStripMenuItem";
            this.addNewAccountToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.addNewAccountToolStripMenuItem.Text = "Add New Account";
            this.addNewAccountToolStripMenuItem.Click += new System.EventHandler(this.addNewAccountToolStripMenuItem_Click);
            // 
            // frmReceipt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 567);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tabDetailQuery);
            this.KeyPreview = true;
            this.Name = "frmReceipt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Receipt";
            this.Load += new System.EventHandler(this.frmReceipt_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmReceipt_KeyDown);
            this.tabDetailQuery.ResumeLayout(false);
            this.tabpgDetail.ResumeLayout(false);
            this.tabpgDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).EndInit();
            this.panel1.ResumeLayout(false);
            this.grpInvoiceDetail.ResumeLayout(false);
            this.grpInvoiceDetail.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuery)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private CalendarColumn calendarColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.TabControl tabDetailQuery;
        private System.Windows.Forms.TabPage tabpgDetail;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.DataGridView dgvExpenses;
        private System.Windows.Forms.GroupBox grpInvoiceDetail;
        private System.Windows.Forms.ComboBox cmbCashBanks;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtEditBy;
        private System.Windows.Forms.TextBox txtCreatedBy;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.TextBox txtVoucherNo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbNarration;
        private System.Windows.Forms.Label lblNarration;
        private System.Windows.Forms.Label lblTotAmount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.DataGridView dgvQuery;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbFilterAccounts;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtTdate;
        private System.Windows.Forms.TextBox txtFdate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbFilterNarration;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbFilterCashBanks;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnEnd;
        private System.Windows.Forms.Button btnPri;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteRecordToolStripMenuItem;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnSeq;
        private System.Windows.Forms.DataGridViewComboBoxColumn clnAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnCheque;
        private CalendarColumn clnChequeDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnRemarks;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnExpCreatedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnExpEditBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnVoucherNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClnCreatedby;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnEditby;
        private System.Windows.Forms.ToolStripMenuItem addNewAccountToolStripMenuItem;
    }
}