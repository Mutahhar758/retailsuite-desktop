namespace ERP.Forms
{
    partial class frmHRInfo
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbDetail = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbPayable = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbExpenseAccount = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.ntxtOvertime = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.ntxtLeaveCharges = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbSalaryType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ntxtSalary = new System.Windows.Forms.NumericUpDown();
            this.profileImage1 = new ERP.Controls.ProfileImage();
            this.grpInvoiceDetail = new System.Windows.Forms.GroupBox();
            this.cmbDesignation = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmdMaritialStatus = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpDOB = new System.Windows.Forms.DateTimePicker();
            this.dtpJoiningDate = new System.Windows.Forms.DateTimePicker();
            this.dtpAppointmentDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFatherName = new System.Windows.Forms.TextBox();
            this.label48 = new System.Windows.Forms.Label();
            this.txtCNIC = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.cmbGender = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtHRID = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbSearch = new System.Windows.Forms.TabPage();
            this.dgvQuery = new System.Windows.Forms.DataGridView();
            this.clnHRID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnDesignation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnJoiningDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnSalaryType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnSalary = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tbDetail.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtOvertime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtLeaveCharges)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtSalary)).BeginInit();
            this.grpInvoiceDetail.SuspendLayout();
            this.tbSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuery)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tbDetail);
            this.tabControl1.Controls.Add(this.tbSearch);
            this.tabControl1.Location = new System.Drawing.Point(12, 48);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(995, 379);
            this.tabControl1.TabIndex = 2;
            // 
            // tbDetail
            // 
            this.tbDetail.Controls.Add(this.panel1);
            this.tbDetail.Controls.Add(this.groupBox1);
            this.tbDetail.Controls.Add(this.profileImage1);
            this.tbDetail.Controls.Add(this.grpInvoiceDetail);
            this.tbDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDetail.Location = new System.Drawing.Point(4, 22);
            this.tbDetail.Name = "tbDetail";
            this.tbDetail.Padding = new System.Windows.Forms.Padding(3);
            this.tbDetail.Size = new System.Drawing.Size(987, 353);
            this.tbDetail.TabIndex = 0;
            this.tbDetail.Text = "Detail";
            this.tbDetail.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnNew);
            this.panel1.Location = new System.Drawing.Point(6, 293);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(326, 39);
            this.panel1.TabIndex = 2;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(165, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(73, 29);
            this.btnDelete.TabIndex = 165;
            this.btnDelete.TabStop = false;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(244, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(73, 29);
            this.btnClose.TabIndex = 163;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(86, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(73, 29);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Location = new System.Drawing.Point(7, 3);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(73, 29);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "&New";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbPayable);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.cmbExpenseAccount);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.ntxtOvertime);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.ntxtLeaveCharges);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cmbSalaryType);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.ntxtSalary);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(6, 130);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(812, 80);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Salary Detail ";
            // 
            // cmbPayable
            // 
            this.cmbPayable.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbPayable.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbPayable.FormattingEnabled = true;
            this.cmbPayable.Items.AddRange(new object[] {
            "Monthly",
            "Weekly",
            "Daily"});
            this.cmbPayable.Location = new System.Drawing.Point(482, 48);
            this.cmbPayable.Name = "cmbPayable";
            this.cmbPayable.Size = new System.Drawing.Size(218, 24);
            this.cmbPayable.TabIndex = 301;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label12.Location = new System.Drawing.Point(359, 54);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(116, 16);
            this.label12.TabIndex = 302;
            this.label12.Text = "Payable Account :";
            // 
            // cmbExpenseAccount
            // 
            this.cmbExpenseAccount.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbExpenseAccount.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbExpenseAccount.FormattingEnabled = true;
            this.cmbExpenseAccount.Items.AddRange(new object[] {
            "Monthly",
            "Weekly",
            "Daily"});
            this.cmbExpenseAccount.Location = new System.Drawing.Point(126, 48);
            this.cmbExpenseAccount.Name = "cmbExpenseAccount";
            this.cmbExpenseAccount.Size = new System.Drawing.Size(218, 24);
            this.cmbExpenseAccount.TabIndex = 299;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label11.Location = new System.Drawing.Point(8, 54);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(118, 16);
            this.label11.TabIndex = 300;
            this.label11.Text = "Expense Account :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label10.Location = new System.Drawing.Point(639, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 16);
            this.label10.TabIndex = 298;
            this.label10.Text = "Overtime :";
            // 
            // ntxtOvertime
            // 
            this.ntxtOvertime.Location = new System.Drawing.Point(714, 17);
            this.ntxtOvertime.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.ntxtOvertime.Name = "ntxtOvertime";
            this.ntxtOvertime.Size = new System.Drawing.Size(95, 22);
            this.ntxtOvertime.TabIndex = 3;
            this.ntxtOvertime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label9.Location = new System.Drawing.Point(417, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(106, 16);
            this.label9.TabIndex = 296;
            this.label9.Text = "Leave Charges :";
            // 
            // ntxtLeaveCharges
            // 
            this.ntxtLeaveCharges.Location = new System.Drawing.Point(529, 18);
            this.ntxtLeaveCharges.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.ntxtLeaveCharges.Name = "ntxtLeaveCharges";
            this.ntxtLeaveCharges.Size = new System.Drawing.Size(102, 22);
            this.ntxtLeaveCharges.TabIndex = 2;
            this.ntxtLeaveCharges.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label8.Location = new System.Drawing.Point(228, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 16);
            this.label8.TabIndex = 294;
            this.label8.Text = "Salary :";
            // 
            // cmbSalaryType
            // 
            this.cmbSalaryType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSalaryType.FormattingEnabled = true;
            this.cmbSalaryType.Items.AddRange(new object[] {
            "Monthly",
            "Weekly",
            "Daily"});
            this.cmbSalaryType.Location = new System.Drawing.Point(100, 20);
            this.cmbSalaryType.Name = "cmbSalaryType";
            this.cmbSalaryType.Size = new System.Drawing.Size(123, 24);
            this.cmbSalaryType.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label6.Location = new System.Drawing.Point(8, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 16);
            this.label6.TabIndex = 293;
            this.label6.Text = "Salary Type :";
            // 
            // ntxtSalary
            // 
            this.ntxtSalary.Location = new System.Drawing.Point(291, 20);
            this.ntxtSalary.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.ntxtSalary.Name = "ntxtSalary";
            this.ntxtSalary.Size = new System.Drawing.Size(120, 22);
            this.ntxtSalary.TabIndex = 1;
            this.ntxtSalary.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // profileImage1
            // 
            this.profileImage1.Image = null;
            this.profileImage1.Location = new System.Drawing.Point(821, 11);
            this.profileImage1.Name = "profileImage1";
            this.profileImage1.Size = new System.Drawing.Size(160, 240);
            this.profileImage1.TabIndex = 295;
            // 
            // grpInvoiceDetail
            // 
            this.grpInvoiceDetail.Controls.Add(this.cmbDesignation);
            this.grpInvoiceDetail.Controls.Add(this.label4);
            this.grpInvoiceDetail.Controls.Add(this.cmdMaritialStatus);
            this.grpInvoiceDetail.Controls.Add(this.label3);
            this.grpInvoiceDetail.Controls.Add(this.dtpDOB);
            this.grpInvoiceDetail.Controls.Add(this.dtpJoiningDate);
            this.grpInvoiceDetail.Controls.Add(this.dtpAppointmentDate);
            this.grpInvoiceDetail.Controls.Add(this.label2);
            this.grpInvoiceDetail.Controls.Add(this.txtFatherName);
            this.grpInvoiceDetail.Controls.Add(this.label48);
            this.grpInvoiceDetail.Controls.Add(this.txtCNIC);
            this.grpInvoiceDetail.Controls.Add(this.label25);
            this.grpInvoiceDetail.Controls.Add(this.cmbGender);
            this.grpInvoiceDetail.Controls.Add(this.label5);
            this.grpInvoiceDetail.Controls.Add(this.label41);
            this.grpInvoiceDetail.Controls.Add(this.label44);
            this.grpInvoiceDetail.Controls.Add(this.label1);
            this.grpInvoiceDetail.Controls.Add(this.txtName);
            this.grpInvoiceDetail.Controls.Add(this.txtHRID);
            this.grpInvoiceDetail.Controls.Add(this.label7);
            this.grpInvoiceDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpInvoiceDetail.Location = new System.Drawing.Point(6, 14);
            this.grpInvoiceDetail.Name = "grpInvoiceDetail";
            this.grpInvoiceDetail.Size = new System.Drawing.Size(812, 110);
            this.grpInvoiceDetail.TabIndex = 0;
            this.grpInvoiceDetail.TabStop = false;
            this.grpInvoiceDetail.Text = "Basic Detail ";
            // 
            // cmbDesignation
            // 
            this.cmbDesignation.FormattingEnabled = true;
            this.cmbDesignation.Items.AddRange(new object[] {
            "Labour",
            "Accounts",
            "Security"});
            this.cmbDesignation.Location = new System.Drawing.Point(610, 74);
            this.cmbDesignation.Name = "cmbDesignation";
            this.cmbDesignation.Size = new System.Drawing.Size(194, 24);
            this.cmbDesignation.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label4.Location = new System.Drawing.Point(515, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 302;
            this.label4.Text = "Designation";
            // 
            // cmdMaritialStatus
            // 
            this.cmdMaritialStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmdMaritialStatus.FormattingEnabled = true;
            this.cmdMaritialStatus.Items.AddRange(new object[] {
            "Single",
            "Married",
            "Widow"});
            this.cmdMaritialStatus.Location = new System.Drawing.Point(459, 45);
            this.cmdMaritialStatus.Name = "cmdMaritialStatus";
            this.cmdMaritialStatus.Size = new System.Drawing.Size(105, 24);
            this.cmdMaritialStatus.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label3.Location = new System.Drawing.Point(352, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 16);
            this.label3.TabIndex = 300;
            this.label3.Text = "Maritial Status :";
            // 
            // dtpDOB
            // 
            this.dtpDOB.CustomFormat = "dd-MMM-yyyy";
            this.dtpDOB.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDOB.Location = new System.Drawing.Point(229, 47);
            this.dtpDOB.Name = "dtpDOB";
            this.dtpDOB.Size = new System.Drawing.Size(115, 22);
            this.dtpDOB.TabIndex = 4;
            // 
            // dtpJoiningDate
            // 
            this.dtpJoiningDate.CustomFormat = "dd-MMM-yyyy";
            this.dtpJoiningDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpJoiningDate.Location = new System.Drawing.Point(377, 76);
            this.dtpJoiningDate.Name = "dtpJoiningDate";
            this.dtpJoiningDate.Size = new System.Drawing.Size(128, 22);
            this.dtpJoiningDate.TabIndex = 8;
            // 
            // dtpAppointmentDate
            // 
            this.dtpAppointmentDate.CustomFormat = "dd-MMM-yyyy";
            this.dtpAppointmentDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpAppointmentDate.Location = new System.Drawing.Point(140, 76);
            this.dtpAppointmentDate.Name = "dtpAppointmentDate";
            this.dtpAppointmentDate.Size = new System.Drawing.Size(128, 22);
            this.dtpAppointmentDate.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(484, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 16);
            this.label2.TabIndex = 295;
            this.label2.Text = "Father Name :";
            // 
            // txtFatherName
            // 
            this.txtFatherName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFatherName.Location = new System.Drawing.Point(586, 19);
            this.txtFatherName.Name = "txtFatherName";
            this.txtFatherName.Size = new System.Drawing.Size(217, 22);
            this.txtFatherName.TabIndex = 2;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.BackColor = System.Drawing.Color.Transparent;
            this.label48.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label48.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label48.Location = new System.Drawing.Point(574, 50);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(48, 16);
            this.label48.TabIndex = 251;
            this.label48.Text = "C.N.I.C";
            // 
            // txtCNIC
            // 
            this.txtCNIC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCNIC.Location = new System.Drawing.Point(637, 47);
            this.txtCNIC.Name = "txtCNIC";
            this.txtCNIC.Size = new System.Drawing.Size(166, 22);
            this.txtCNIC.TabIndex = 6;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.BackColor = System.Drawing.Color.Transparent;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label25.Location = new System.Drawing.Point(167, 50);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(43, 16);
            this.label25.TabIndex = 292;
            this.label25.Text = "DOB :";
            // 
            // cmbGender
            // 
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.FormattingEnabled = true;
            this.cmbGender.Items.AddRange(new object[] {
            "Male",
            "Female"});
            this.cmbGender.Location = new System.Drawing.Point(66, 47);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(89, 24);
            this.cmbGender.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label5.Location = new System.Drawing.Point(6, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 16);
            this.label5.TabIndex = 291;
            this.label5.Text = "Gender";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.BackColor = System.Drawing.Color.Transparent;
            this.label41.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label41.Location = new System.Drawing.Point(13, 79);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(121, 16);
            this.label41.TabIndex = 289;
            this.label41.Text = "Appointment Date :";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.BackColor = System.Drawing.Color.Transparent;
            this.label44.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label44.Location = new System.Drawing.Point(282, 79);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(89, 16);
            this.label44.TabIndex = 288;
            this.label44.Text = "Joining Date :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(172, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 16);
            this.label1.TabIndex = 243;
            this.label1.Text = "Name :";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(229, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(247, 22);
            this.txtName.TabIndex = 1;
            // 
            // txtHRID
            // 
            this.txtHRID.Location = new System.Drawing.Point(66, 19);
            this.txtHRID.Name = "txtHRID";
            this.txtHRID.ReadOnly = true;
            this.txtHRID.Size = new System.Drawing.Size(89, 22);
            this.txtHRID.TabIndex = 0;
            this.txtHRID.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(10, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 16);
            this.label7.TabIndex = 157;
            this.label7.Text = "HR ID :";
            // 
            // tbSearch
            // 
            this.tbSearch.Controls.Add(this.dgvQuery);
            this.tbSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSearch.Location = new System.Drawing.Point(4, 22);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Padding = new System.Windows.Forms.Padding(3);
            this.tbSearch.Size = new System.Drawing.Size(987, 353);
            this.tbSearch.TabIndex = 1;
            this.tbSearch.Text = "Search";
            this.tbSearch.UseVisualStyleBackColor = true;
            // 
            // dgvQuery
            // 
            this.dgvQuery.AllowUserToAddRows = false;
            this.dgvQuery.AllowUserToDeleteRows = false;
            this.dgvQuery.AllowUserToOrderColumns = true;
            this.dgvQuery.AllowUserToResizeColumns = false;
            this.dgvQuery.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvQuery.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvQuery.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvQuery.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnHRID,
            this.clnName,
            this.clnDesignation,
            this.clnJoiningDate,
            this.clnSalaryType,
            this.clnSalary});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvQuery.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvQuery.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvQuery.Location = new System.Drawing.Point(3, 3);
            this.dgvQuery.MultiSelect = false;
            this.dgvQuery.Name = "dgvQuery";
            this.dgvQuery.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvQuery.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvQuery.RowHeadersWidth = 30;
            this.dgvQuery.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvQuery.Size = new System.Drawing.Size(981, 318);
            this.dgvQuery.TabIndex = 165;
            this.dgvQuery.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvQuery_CellDoubleClick);
            // 
            // clnHRID
            // 
            this.clnHRID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clnHRID.HeaderText = "HR ID";
            this.clnHRID.Name = "clnHRID";
            this.clnHRID.ReadOnly = true;
            this.clnHRID.Width = 69;
            // 
            // clnName
            // 
            this.clnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clnName.HeaderText = "Name";
            this.clnName.Name = "clnName";
            this.clnName.ReadOnly = true;
            // 
            // clnDesignation
            // 
            this.clnDesignation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clnDesignation.HeaderText = "Designation";
            this.clnDesignation.Name = "clnDesignation";
            this.clnDesignation.ReadOnly = true;
            this.clnDesignation.Width = 105;
            // 
            // clnJoiningDate
            // 
            this.clnJoiningDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clnJoiningDate.HeaderText = "JoiningDate";
            this.clnJoiningDate.Name = "clnJoiningDate";
            this.clnJoiningDate.ReadOnly = true;
            this.clnJoiningDate.Width = 105;
            // 
            // clnSalaryType
            // 
            this.clnSalaryType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clnSalaryType.HeaderText = "Salary Type";
            this.clnSalaryType.Name = "clnSalaryType";
            this.clnSalaryType.ReadOnly = true;
            this.clnSalaryType.Width = 98;
            // 
            // clnSalary
            // 
            this.clnSalary.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clnSalary.HeaderText = "Salary";
            this.clnSalary.Name = "clnSalary";
            this.clnSalary.ReadOnly = true;
            this.clnSalary.Width = 72;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn1.HeaderText = "HR ID";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn3.HeaderText = "Designation";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn4.HeaderText = "JoiningDate";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn5.HeaderText = "Salary Type";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn6.HeaderText = "Salary";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // frmHRInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1019, 467);
            this.Controls.Add(this.tabControl1);
            this.KeyPreview = true;
            this.Name = "frmHRInfo";
            this.Text = "frmHRInfo";
            this.Load += new System.EventHandler(this.frmHRInfo_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmHRInfo_KeyDown);
            this.tabControl1.ResumeLayout(false);
            this.tbDetail.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtOvertime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtLeaveCharges)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ntxtSalary)).EndInit();
            this.grpInvoiceDetail.ResumeLayout(false);
            this.grpInvoiceDetail.PerformLayout();
            this.tbSearch.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuery)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbDetail;
        private System.Windows.Forms.TabPage tbSearch;
        private System.Windows.Forms.GroupBox grpInvoiceDetail;
        private System.Windows.Forms.TextBox txtHRID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.ComboBox cmbGender;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.TextBox txtCNIC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFatherName;
        private System.Windows.Forms.DateTimePicker dtpDOB;
        private System.Windows.Forms.DateTimePicker dtpJoiningDate;
        private System.Windows.Forms.DateTimePicker dtpAppointmentDate;
        private Controls.ProfileImage profileImage1;
        private System.Windows.Forms.ComboBox cmdMaritialStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbDesignation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown ntxtLeaveCharges;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbSalaryType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown ntxtSalary;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown ntxtOvertime;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.DataGridView dgvQuery;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnHRID;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnDesignation;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnJoiningDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnSalaryType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnSalary;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.ComboBox cmbExpenseAccount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbPayable;
        private System.Windows.Forms.Label label12;
    }
}