namespace ERP
{
    partial class frmOpeningBal
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvOpenning = new System.Windows.Forms.DataGridView();
            this.clnParent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnDr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnCr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblTotDr = new System.Windows.Forms.Label();
            this.lblTotCr = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOpenning)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvOpenning
            // 
            this.dgvOpenning.AllowUserToAddRows = false;
            this.dgvOpenning.AllowUserToDeleteRows = false;
            this.dgvOpenning.AllowUserToOrderColumns = true;
            this.dgvOpenning.AllowUserToResizeColumns = false;
            this.dgvOpenning.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvOpenning.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvOpenning.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOpenning.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnParent,
            this.clnAccount,
            this.clnTitle,
            this.clnDr,
            this.clnCr});
            this.dgvOpenning.Location = new System.Drawing.Point(12, 46);
            this.dgvOpenning.MultiSelect = false;
            this.dgvOpenning.Name = "dgvOpenning";
            this.dgvOpenning.RowHeadersWidth = 30;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvOpenning.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvOpenning.RowTemplate.Height = 25;
            this.dgvOpenning.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvOpenning.Size = new System.Drawing.Size(827, 346);
            this.dgvOpenning.TabIndex = 211;
            this.dgvOpenning.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOpenning_CellValueChanged);
            // 
            // clnParent
            // 
            this.clnParent.HeaderText = "Parent";
            this.clnParent.Name = "clnParent";
            this.clnParent.ReadOnly = true;
            this.clnParent.Visible = false;
            // 
            // clnAccount
            // 
            this.clnAccount.HeaderText = "Account";
            this.clnAccount.MinimumWidth = 130;
            this.clnAccount.Name = "clnAccount";
            this.clnAccount.ReadOnly = true;
            this.clnAccount.Width = 130;
            // 
            // clnTitle
            // 
            this.clnTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clnTitle.HeaderText = "Title";
            this.clnTitle.MinimumWidth = 180;
            this.clnTitle.Name = "clnTitle";
            this.clnTitle.ReadOnly = true;
            // 
            // clnDr
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clnDr.DefaultCellStyle = dataGridViewCellStyle2;
            this.clnDr.HeaderText = "Debit";
            this.clnDr.MinimumWidth = 150;
            this.clnDr.Name = "clnDr";
            this.clnDr.Width = 150;
            // 
            // clnCr
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clnCr.DefaultCellStyle = dataGridViewCellStyle3;
            this.clnCr.HeaderText = "Credit";
            this.clnCr.MinimumWidth = 150;
            this.clnCr.Name = "clnCr";
            this.clnCr.Width = 150;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Parent";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Visible = false;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Account";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 130;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 130;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.HeaderText = "Title";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 180;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn4.HeaderText = "Debit";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 150;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 150;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewTextBoxColumn5.HeaderText = "Credit";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 150;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 150;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Location = new System.Drawing.Point(668, 440);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(171, 40);
            this.panel1.TabIndex = 212;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(86, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 29);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Location = new System.Drawing.Point(6, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(73, 29);
            this.btnOk.TabIndex = 161;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lblTotDr
            // 
            this.lblTotDr.AutoSize = true;
            this.lblTotDr.Location = new System.Drawing.Point(571, 399);
            this.lblTotDr.Name = "lblTotDr";
            this.lblTotDr.Size = new System.Drawing.Size(10, 13);
            this.lblTotDr.TabIndex = 213;
            this.lblTotDr.Text = "-";
            // 
            // lblTotCr
            // 
            this.lblTotCr.AutoSize = true;
            this.lblTotCr.Location = new System.Drawing.Point(723, 399);
            this.lblTotCr.Name = "lblTotCr";
            this.lblTotCr.Size = new System.Drawing.Size(10, 13);
            this.lblTotCr.TabIndex = 214;
            this.lblTotCr.Text = "-";
            // 
            // frmOpeningBal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 492);
            this.Controls.Add(this.lblTotCr);
            this.Controls.Add(this.lblTotDr);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvOpenning);
            this.Name = "frmOpeningBal";
            this.Text = "frmOpeningBal";
            this.Load += new System.EventHandler(this.frmOpeningBal_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOpenning)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvOpenning;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnParent;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnDr;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnCr;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lblTotDr;
        private System.Windows.Forms.Label lblTotCr;
    }
}