namespace ERP
{
    partial class frmItemDetail
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmItemDetail));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cmbItemCatagory = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dgvItemDetail = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTotalItems = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnBarcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnCatagory = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clnItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnPriRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnSecRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnPriUnit = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clnSecUnit = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clnDefUnif = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ClnQtyInPack = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnAlert = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clnLowStockAlert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnOpnStock = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnOpnRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnstatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnIsEdit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItemDetail)).BeginInit();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbItemCatagory
            // 
            this.cmbItemCatagory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbItemCatagory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbItemCatagory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbItemCatagory.FormattingEnabled = true;
            this.cmbItemCatagory.Location = new System.Drawing.Point(125, 104);
            this.cmbItemCatagory.Name = "cmbItemCatagory";
            this.cmbItemCatagory.Size = new System.Drawing.Size(215, 24);
            this.cmbItemCatagory.TabIndex = 159;
            this.cmbItemCatagory.SelectedIndexChanged += new System.EventHandler(this.cmbItemCatagory_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(22, 108);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 16);
            this.label11.TabIndex = 160;
            this.label11.Text = "Item Catagory :";
            // 
            // dgvItemDetail
            // 
            this.dgvItemDetail.AllowUserToAddRows = false;
            this.dgvItemDetail.AllowUserToDeleteRows = false;
            this.dgvItemDetail.AllowUserToOrderColumns = true;
            this.dgvItemDetail.AllowUserToResizeColumns = false;
            this.dgvItemDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvItemDetail.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvItemDetail.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvItemDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItemDetail.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnId,
            this.clnBarcode,
            this.clnCatagory,
            this.clnItem,
            this.clnKey,
            this.clnPriRate,
            this.clnSecRate,
            this.clnPriUnit,
            this.clnSecUnit,
            this.clnDefUnif,
            this.ClnQtyInPack,
            this.clnAlert,
            this.clnLowStockAlert,
            this.clnOpnStock,
            this.clnOpnRate,
            this.clnstatus,
            this.clnIsEdit});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvItemDetail.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvItemDetail.Location = new System.Drawing.Point(0, 140);
            this.dgvItemDetail.MultiSelect = false;
            this.dgvItemDetail.Name = "dgvItemDetail";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvItemDetail.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvItemDetail.RowHeadersWidth = 30;
            this.dgvItemDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvItemDetail.Size = new System.Drawing.Size(1152, 387);
            this.dgvItemDetail.TabIndex = 164;
            this.dgvItemDetail.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItemDetail_CellEndEdit);
            this.dgvItemDetail.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvItemDetail_CellMouseUp);
            this.dgvItemDetail.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItemDetail_CellValueChanged);
            this.dgvItemDetail.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvItemDetail_DataError);
            this.dgvItemDetail.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvItemDetail_EditingControlShowing);
            this.dgvItemDetail.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvItemDetail_RowsAdded);
            this.dgvItemDetail.SelectionChanged += new System.EventHandler(this.dgvItemDetail_SelectionChanged);
            this.dgvItemDetail.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvItemDetail_KeyDown);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Location = new System.Drawing.Point(970, 93);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(170, 41);
            this.panel1.TabIndex = 170;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(86, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(73, 29);
            this.btnClose.TabIndex = 163;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(7, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(73, 29);
            this.btnSave.TabIndex = 162;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtBarcode
            // 
            this.txtBarcode.Location = new System.Drawing.Point(411, 106);
            this.txtBarcode.Multiline = true;
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(213, 22);
            this.txtBarcode.TabIndex = 211;
            this.txtBarcode.TabStop = false;
            this.txtBarcode.TextChanged += new System.EventHandler(this.txtBarcode_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(359, 109);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 16);
            this.label12.TabIndex = 212;
            this.label12.Text = "Barcode : ";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteRecordToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(135, 26);
            // 
            // deleteRecordToolStripMenuItem
            // 
            this.deleteRecordToolStripMenuItem.Name = "deleteRecordToolStripMenuItem";
            this.deleteRecordToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.deleteRecordToolStripMenuItem.Text = "Delete Item";
            this.deleteRecordToolStripMenuItem.Click += new System.EventHandler(this.deleteRecordToolStripMenuItem_Click);
            // 
            // lblTotalItems
            // 
            this.lblTotalItems.AutoSize = true;
            this.lblTotalItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalItems.Location = new System.Drawing.Point(650, 109);
            this.lblTotalItems.Name = "lblTotalItems";
            this.lblTotalItems.Size = new System.Drawing.Size(90, 16);
            this.lblTotalItems.TabIndex = 214;
            this.lblTotalItems.Text = "Total Items : 0";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label17.Location = new System.Drawing.Point(413, 18);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(247, 59);
            this.label17.TabIndex = 218;
            this.label17.Text = "Item Detail";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(339, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(84, 59);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 219;
            this.pictureBox1.TabStop = false;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Code";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 80;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Visible = false;
            this.dataGridViewTextBoxColumn1.Width = 80;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.FillWeight = 50F;
            this.dataGridViewTextBoxColumn2.HeaderText = "Rate";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 120;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.FillWeight = 50F;
            this.dataGridViewTextBoxColumn3.HeaderText = "Rate";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 80;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn4.HeaderText = "Status";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 80;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn4.Visible = false;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewTextBoxColumn5.HeaderText = "Is Edit";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 80;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn5.Visible = false;
            this.dataGridViewTextBoxColumn5.Width = 80;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewTextBoxColumn6.HeaderText = "Status";
            this.dataGridViewTextBoxColumn6.MinimumWidth = 80;
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn6.Visible = false;
            this.dataGridViewTextBoxColumn6.Width = 80;
            // 
            // dataGridViewTextBoxColumn7
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.dataGridViewTextBoxColumn7.DefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridViewTextBoxColumn7.HeaderText = "Is Edit";
            this.dataGridViewTextBoxColumn7.MinimumWidth = 80;
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn7.Visible = false;
            this.dataGridViewTextBoxColumn7.Width = 80;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "Is Edit";
            this.dataGridViewTextBoxColumn8.MinimumWidth = 80;
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            this.dataGridViewTextBoxColumn8.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn8.Visible = false;
            this.dataGridViewTextBoxColumn8.Width = 80;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.HeaderText = "Is Edit";
            this.dataGridViewTextBoxColumn9.MinimumWidth = 70;
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            this.dataGridViewTextBoxColumn9.Visible = false;
            this.dataGridViewTextBoxColumn9.Width = 70;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.HeaderText = "Is Edit";
            this.dataGridViewTextBoxColumn10.MinimumWidth = 100;
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            this.dataGridViewTextBoxColumn10.Visible = false;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.HeaderText = "Opn Rate";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.HeaderText = "Status";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            this.dataGridViewTextBoxColumn12.ReadOnly = true;
            this.dataGridViewTextBoxColumn12.Visible = false;
            // 
            // dataGridViewTextBoxColumn13
            // 
            this.dataGridViewTextBoxColumn13.HeaderText = "Is Edit";
            this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            this.dataGridViewTextBoxColumn13.ReadOnly = true;
            this.dataGridViewTextBoxColumn13.Visible = false;
            // 
            // clnId
            // 
            this.clnId.HeaderText = "Id";
            this.clnId.Name = "clnId";
            this.clnId.ReadOnly = true;
            this.clnId.Visible = false;

            // 
            // clnBarcode
            // 
            this.clnBarcode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clnBarcode.HeaderText = "Barcode";
            this.clnBarcode.MinimumWidth = 100;
            this.clnBarcode.Name = "clnBarcode";
            this.clnBarcode.ReadOnly = true;
            // 
            // clnCatagory
            // 
            this.clnCatagory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clnCatagory.FillWeight = 50F;
            this.clnCatagory.HeaderText = "Catagory";
            this.clnCatagory.MinimumWidth = 90;
            this.clnCatagory.Name = "clnCatagory";
            this.clnCatagory.Width = 90;
            // 
            // clnItem
            // 
            this.clnItem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clnItem.HeaderText = "Item";
            this.clnItem.MinimumWidth = 120;
            this.clnItem.Name = "clnItem";
            this.clnItem.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // clnKey
            // 
            this.clnKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clnKey.HeaderText = "Key";
            this.clnKey.MinimumWidth = 40;
            this.clnKey.Name = "clnKey";
            this.clnKey.Width = 56;
            // 
            // clnPriRate
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clnPriRate.DefaultCellStyle = dataGridViewCellStyle2;
            this.clnPriRate.HeaderText = "Primary Rate";
            this.clnPriRate.MinimumWidth = 60;
            this.clnPriRate.Name = "clnPriRate";
            this.clnPriRate.Width = 60;
            // 
            // clnSecRate
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.clnSecRate.DefaultCellStyle = dataGridViewCellStyle3;
            this.clnSecRate.HeaderText = "Sec Rate";
            this.clnSecRate.MinimumWidth = 60;
            this.clnSecRate.Name = "clnSecRate";
            this.clnSecRate.Width = 60;
            // 
            // clnPriUnit
            // 
            this.clnPriUnit.HeaderText = "Primary Unit";
            this.clnPriUnit.MinimumWidth = 80;
            this.clnPriUnit.Name = "clnPriUnit";
            this.clnPriUnit.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clnPriUnit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.clnPriUnit.Width = 80;
            // 
            // clnSecUnit
            // 
            this.clnSecUnit.HeaderText = "Sec Unit";
            this.clnSecUnit.MinimumWidth = 60;
            this.clnSecUnit.Name = "clnSecUnit";
            this.clnSecUnit.Width = 60;
            // 
            // clnDefUnif
            // 
            this.clnDefUnif.HeaderText = "Default Unit";
            this.clnDefUnif.MinimumWidth = 80;
            this.clnDefUnif.Name = "clnDefUnif";
            this.clnDefUnif.Width = 80;
            // 
            // ClnQtyInPack
            // 
            this.ClnQtyInPack.HeaderText = "Qty In Pack";
            this.ClnQtyInPack.MinimumWidth = 70;
            this.ClnQtyInPack.Name = "ClnQtyInPack";
            this.ClnQtyInPack.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ClnQtyInPack.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ClnQtyInPack.Width = 70;
            // 
            // clnAlert
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.NullValue = "False";
            this.clnAlert.DefaultCellStyle = dataGridViewCellStyle4;
            this.clnAlert.HeaderText = "Alert";
            this.clnAlert.MinimumWidth = 50;
            this.clnAlert.Name = "clnAlert";
            this.clnAlert.Visible = false;
            this.clnAlert.Width = 50;
            // 
            // clnLowStockAlert
            // 
            this.clnLowStockAlert.HeaderText = "Low Stock Alert On";
            this.clnLowStockAlert.MinimumWidth = 70;
            this.clnLowStockAlert.Name = "clnLowStockAlert";
            this.clnLowStockAlert.Visible = false;
            this.clnLowStockAlert.Width = 70;
            // 
            // clnOpnStock
            // 
            this.clnOpnStock.HeaderText = "OpnStock";
            this.clnOpnStock.MinimumWidth = 100;
            this.clnOpnStock.Name = "clnOpnStock";
            // 
            // clnOpnRate
            // 
            this.clnOpnRate.HeaderText = "Opn Rate";
            this.clnOpnRate.Name = "clnOpnRate";
            // 
            // clnstatus
            // 
            this.clnstatus.HeaderText = "Status";
            this.clnstatus.Name = "clnstatus";
            this.clnstatus.ReadOnly = true;
            this.clnstatus.Visible = false;
            // 
            // clnIsEdit
            // 
            this.clnIsEdit.HeaderText = "Is Edit";
            this.clnIsEdit.Name = "clnIsEdit";
            this.clnIsEdit.ReadOnly = true;
            this.clnIsEdit.Visible = false;
            // 
            // frmItemDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1152, 527);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblTotalItems);
            this.Controls.Add(this.txtBarcode);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvItemDetail);
            this.Controls.Add(this.cmbItemCatagory);
            this.Controls.Add(this.label11);
            this.Name = "frmItemDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Item_Detail";
            this.Load += new System.EventHandler(this.frmItemDetail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItemDetail)).EndInit();
            this.panel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbItemCatagory;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridView dgvItemDetail;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteRecordToolStripMenuItem;
        private System.Windows.Forms.Label lblTotalItems;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnId;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnBarcode;
        private System.Windows.Forms.DataGridViewComboBoxColumn clnCatagory;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnPriRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnSecRate;
        private System.Windows.Forms.DataGridViewComboBoxColumn clnPriUnit;
        private System.Windows.Forms.DataGridViewComboBoxColumn clnSecUnit;
        private System.Windows.Forms.DataGridViewComboBoxColumn clnDefUnif;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClnQtyInPack;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clnAlert;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnLowStockAlert;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnOpnStock;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnOpnRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnstatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnIsEdit;
    }
}