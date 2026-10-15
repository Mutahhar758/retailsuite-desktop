namespace ERP.Forms
{
    partial class frmBOM
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
            System.Windows.Forms.DataGridViewCellStyle dgvStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dgvStyle2 = new System.Windows.Forms.DataGridViewCellStyle();

            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlLeftHeader = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dgvBomList = new System.Windows.Forms.DataGridView();
            this.clnListId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListFinished = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnListQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpMaster = new System.Windows.Forms.GroupBox();
            this.lblBomCode = new System.Windows.Forms.Label();
            this.txtBomCode = new System.Windows.Forms.TextBox();
            this.lblBomName = new System.Windows.Forms.Label();
            this.txtBomName = new System.Windows.Forms.TextBox();
            this.lblFinishedItem = new System.Windows.Forms.Label();
            this.cmbFinishedItem = new System.Windows.Forms.ComboBox();
            this.lblOutputQty = new System.Windows.Forms.Label();
            this.txtOutputQty = new System.Windows.Forms.TextBox();
            this.lblLaborCost = new System.Windows.Forms.Label();
            this.txtLaborCost = new System.Windows.Forms.TextBox();
            this.lblOverheadCost = new System.Windows.Forms.Label();
            this.txtOverheadCost = new System.Windows.Forms.TextBox();
            this.lblTotalCostTitle = new System.Windows.Forms.Label();
            this.lblTotalEstimatedCost = new System.Windows.Forms.Label();
            this.chkIsActive = new System.Windows.Forms.CheckBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.grpDetails = new System.Windows.Forms.GroupBox();
            this.dgvRawMaterials = new System.Windows.Forms.DataGridView();
            this.clnRawItem = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clnQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnStdCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnScrapPct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnLineCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clnDetailNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlDetailButtons = new System.Windows.Forms.Panel();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.btnDeleteRow = new System.Windows.Forms.Button();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();

            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlLeftHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBomList)).BeginInit();
            this.grpMaster.SuspendLayout();
            this.grpDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRawMaterials)).BeginInit();
            this.pnlDetailButtons.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();

            // pnlHeader
            this.pnlHeader.BackColor = System.Drawing.Color.SteelBlue;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1100, 48);
            this.pnlHeader.TabIndex = 0;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(296, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Bill of Materials / Product Recipe";

            // splitContainer1
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 48);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.dgvBomList);
            this.splitContainer1.Panel1.Controls.Add(this.pnlLeftHeader);
            this.splitContainer1.Panel2.Controls.Add(this.grpDetails);
            this.splitContainer1.Panel2.Controls.Add(this.grpMaster);
            this.splitContainer1.Panel2.Controls.Add(this.pnlActions);
            this.splitContainer1.Size = new System.Drawing.Size(1100, 620);
            this.splitContainer1.SplitterDistance = 340;
            this.splitContainer1.TabIndex = 1;

            // pnlLeftHeader
            this.pnlLeftHeader.Controls.Add(this.lblSearch);
            this.pnlLeftHeader.Controls.Add(this.txtSearch);
            this.pnlLeftHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLeftHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlLeftHeader.Name = "pnlLeftHeader";
            this.pnlLeftHeader.Size = new System.Drawing.Size(340, 45);
            this.pnlLeftHeader.TabIndex = 0;

            // lblSearch
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(10, 14);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(45, 13);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search:";

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(60, 11);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(265, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);

            // dgvBomList
            this.dgvBomList.AllowUserToAddRows = false;
            this.dgvBomList.AllowUserToDeleteRows = false;
            this.dgvBomList.AutoGenerateColumns = false;
            this.dgvBomList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBomList.BackgroundColor = System.Drawing.Color.White;
            this.dgvBomList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBomList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnListId,
            this.clnListCode,
            this.clnListName,
            this.clnListFinished,
            this.clnListQty});
            this.dgvBomList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBomList.Location = new System.Drawing.Point(0, 45);
            this.dgvBomList.MultiSelect = false;
            this.dgvBomList.Name = "dgvBomList";
            this.dgvBomList.ReadOnly = true;
            this.dgvBomList.RowHeadersVisible = false;
            this.dgvBomList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBomList.Size = new System.Drawing.Size(340, 575);
            this.dgvBomList.TabIndex = 1;
            this.dgvBomList.SelectionChanged += new System.EventHandler(this.dgvBomList_SelectionChanged);

            // Columns for dgvBomList
            this.clnListId.DataPropertyName = "Id";
            this.clnListId.HeaderText = "ID";
            this.clnListId.Name = "clnListId";
            this.clnListId.ReadOnly = true;
            this.clnListId.Visible = false;

            this.clnListCode.DataPropertyName = "BomCode";
            this.clnListCode.HeaderText = "Code";
            this.clnListCode.Name = "clnListCode";
            this.clnListCode.ReadOnly = true;
            this.clnListCode.FillWeight = 50F;

            this.clnListName.DataPropertyName = "BomName";
            this.clnListName.HeaderText = "BOM Name";
            this.clnListName.Name = "clnListName";
            this.clnListName.ReadOnly = true;
            this.clnListName.FillWeight = 80F;

            this.clnListFinished.DataPropertyName = "FinishedItemName";
            this.clnListFinished.HeaderText = "Finished Item";
            this.clnListFinished.Name = "clnListFinished";
            this.clnListFinished.ReadOnly = true;
            this.clnListFinished.FillWeight = 100F;

            this.clnListQty.DataPropertyName = "OutputQuantity";
            this.clnListQty.HeaderText = "Yield Qty";
            this.clnListQty.Name = "clnListQty";
            this.clnListQty.ReadOnly = true;
            this.clnListQty.FillWeight = 40F;

            // grpMaster
            this.grpMaster.Controls.Add(this.lblBomCode);
            this.grpMaster.Controls.Add(this.txtBomCode);
            this.grpMaster.Controls.Add(this.lblBomName);
            this.grpMaster.Controls.Add(this.txtBomName);
            this.grpMaster.Controls.Add(this.lblFinishedItem);
            this.grpMaster.Controls.Add(this.cmbFinishedItem);
            this.grpMaster.Controls.Add(this.lblOutputQty);
            this.grpMaster.Controls.Add(this.txtOutputQty);
            this.grpMaster.Controls.Add(this.lblLaborCost);
            this.grpMaster.Controls.Add(this.txtLaborCost);
            this.grpMaster.Controls.Add(this.lblOverheadCost);
            this.grpMaster.Controls.Add(this.txtOverheadCost);
            this.grpMaster.Controls.Add(this.lblTotalCostTitle);
            this.grpMaster.Controls.Add(this.lblTotalEstimatedCost);
            this.grpMaster.Controls.Add(this.chkIsActive);
            this.grpMaster.Controls.Add(this.lblNotes);
            this.grpMaster.Controls.Add(this.txtNotes);
            this.grpMaster.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpMaster.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.grpMaster.Location = new System.Drawing.Point(0, 0);
            this.grpMaster.Name = "grpMaster";
            this.grpMaster.Size = new System.Drawing.Size(756, 175);
            this.grpMaster.TabIndex = 0;
            this.grpMaster.TabStop = false;
            this.grpMaster.Text = "Recipe Information";

            // lblBomCode
            this.lblBomCode.AutoSize = true;
            this.lblBomCode.Location = new System.Drawing.Point(15, 25);
            this.lblBomCode.Name = "lblBomCode";
            this.lblBomCode.Size = new System.Drawing.Size(65, 15);
            this.lblBomCode.Text = "BOM Code:";

            // txtBomCode
            this.txtBomCode.Location = new System.Drawing.Point(90, 22);
            this.txtBomCode.Name = "txtBomCode";
            this.txtBomCode.Size = new System.Drawing.Size(120, 23);

            // lblBomName
            this.lblBomName.AutoSize = true;
            this.lblBomName.Location = new System.Drawing.Point(230, 25);
            this.lblBomName.Name = "lblBomName";
            this.lblBomName.Size = new System.Drawing.Size(69, 15);
            this.lblBomName.Text = "BOM Name:";

            // txtBomName
            this.txtBomName.Location = new System.Drawing.Point(305, 22);
            this.txtBomName.Name = "txtBomName";
            this.txtBomName.Size = new System.Drawing.Size(260, 23);

            // chkIsActive
            this.chkIsActive.AutoSize = true;
            this.chkIsActive.Checked = true;
            this.chkIsActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIsActive.Location = new System.Drawing.Point(590, 24);
            this.chkIsActive.Name = "chkIsActive";
            this.chkIsActive.Size = new System.Drawing.Size(59, 19);
            this.chkIsActive.Text = "Active";

            // lblFinishedItem
            this.lblFinishedItem.AutoSize = true;
            this.lblFinishedItem.Location = new System.Drawing.Point(15, 58);
            this.lblFinishedItem.Name = "lblFinishedItem";
            this.lblFinishedItem.Size = new System.Drawing.Size(74, 15);
            this.lblFinishedItem.Text = "Output Item:";

            // cmbFinishedItem
            this.cmbFinishedItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFinishedItem.FormattingEnabled = true;
            this.cmbFinishedItem.Location = new System.Drawing.Point(90, 55);
            this.cmbFinishedItem.Name = "cmbFinishedItem";
            this.cmbFinishedItem.Size = new System.Drawing.Size(320, 23);

            // lblOutputQty
            this.lblOutputQty.AutoSize = true;
            this.lblOutputQty.Location = new System.Drawing.Point(430, 58);
            this.lblOutputQty.Name = "lblOutputQty";
            this.lblOutputQty.Size = new System.Drawing.Size(68, 15);
            this.lblOutputQty.Text = "Output Qty:";

            // txtOutputQty
            this.txtOutputQty.Location = new System.Drawing.Point(505, 55);
            this.txtOutputQty.Name = "txtOutputQty";
            this.txtOutputQty.Size = new System.Drawing.Size(80, 23);
            this.txtOutputQty.Text = "1";
            this.txtOutputQty.TextChanged += new System.EventHandler(this.CalculateCost);

            // lblLaborCost
            this.lblLaborCost.AutoSize = true;
            this.lblLaborCost.Location = new System.Drawing.Point(15, 93);
            this.lblLaborCost.Name = "lblLaborCost";
            this.lblLaborCost.Size = new System.Drawing.Size(66, 15);
            this.lblLaborCost.Text = "Labor Cost:";

            // txtLaborCost
            this.txtLaborCost.Location = new System.Drawing.Point(90, 90);
            this.txtLaborCost.Name = "txtLaborCost";
            this.txtLaborCost.Size = new System.Drawing.Size(100, 23);
            this.txtLaborCost.Text = "0";
            this.txtLaborCost.TextChanged += new System.EventHandler(this.CalculateCost);

            // lblOverheadCost
            this.lblOverheadCost.AutoSize = true;
            this.lblOverheadCost.Location = new System.Drawing.Point(210, 93);
            this.lblOverheadCost.Name = "lblOverheadCost";
            this.lblOverheadCost.Size = new System.Drawing.Size(88, 15);
            this.lblOverheadCost.Text = "Overhead Cost:";

            // txtOverheadCost
            this.txtOverheadCost.Location = new System.Drawing.Point(305, 90);
            this.txtOverheadCost.Name = "txtOverheadCost";
            this.txtOverheadCost.Size = new System.Drawing.Size(100, 23);
            this.txtOverheadCost.Text = "0";
            this.txtOverheadCost.TextChanged += new System.EventHandler(this.CalculateCost);

            // lblTotalCostTitle
            this.lblTotalCostTitle.AutoSize = true;
            this.lblTotalCostTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalCostTitle.Location = new System.Drawing.Point(430, 93);
            this.lblTotalCostTitle.Name = "lblTotalCostTitle";
            this.lblTotalCostTitle.Size = new System.Drawing.Size(64, 15);
            this.lblTotalCostTitle.Text = "Est. Total:";

            // lblTotalEstimatedCost
            this.lblTotalEstimatedCost.AutoSize = true;
            this.lblTotalEstimatedCost.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalEstimatedCost.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblTotalEstimatedCost.Location = new System.Drawing.Point(505, 91);
            this.lblTotalEstimatedCost.Name = "lblTotalEstimatedCost";
            this.lblTotalEstimatedCost.Size = new System.Drawing.Size(37, 19);
            this.lblTotalEstimatedCost.Text = "0.00";

            // lblNotes
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(15, 128);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(41, 15);
            this.lblNotes.Text = "Notes:";

            // txtNotes
            this.txtNotes.Location = new System.Drawing.Point(90, 125);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(600, 23);

            // grpDetails
            this.grpDetails.Controls.Add(this.dgvRawMaterials);
            this.grpDetails.Controls.Add(this.pnlDetailButtons);
            this.grpDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDetails.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grpDetails.Location = new System.Drawing.Point(0, 175);
            this.grpDetails.Name = "grpDetails";
            this.grpDetails.Size = new System.Drawing.Size(756, 395);
            this.grpDetails.TabIndex = 1;
            this.grpDetails.TabStop = false;
            this.grpDetails.Text = "Raw Materials & Ingredients";

            // pnlDetailButtons
            this.pnlDetailButtons.Controls.Add(this.btnAddRow);
            this.pnlDetailButtons.Controls.Add(this.btnDeleteRow);
            this.pnlDetailButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDetailButtons.Location = new System.Drawing.Point(3, 19);
            this.pnlDetailButtons.Name = "pnlDetailButtons";
            this.pnlDetailButtons.Size = new System.Drawing.Size(750, 34);
            this.pnlDetailButtons.TabIndex = 0;

            // btnAddRow
            this.btnAddRow.BackColor = System.Drawing.Color.ForestGreen;
            this.btnAddRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddRow.ForeColor = System.Drawing.Color.White;
            this.btnAddRow.Location = new System.Drawing.Point(6, 4);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(85, 26);
            this.btnAddRow.TabIndex = 0;
            this.btnAddRow.Text = "+ Add Line";
            this.btnAddRow.UseVisualStyleBackColor = false;
            this.btnAddRow.Click += new System.EventHandler(this.btnAddRow_Click);

            // btnDeleteRow
            this.btnDeleteRow.BackColor = System.Drawing.Color.Firebrick;
            this.btnDeleteRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteRow.ForeColor = System.Drawing.Color.White;
            this.btnDeleteRow.Location = new System.Drawing.Point(97, 4);
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(85, 26);
            this.btnDeleteRow.TabIndex = 1;
            this.btnDeleteRow.Text = "- Remove";
            this.btnDeleteRow.UseVisualStyleBackColor = false;
            this.btnDeleteRow.Click += new System.EventHandler(this.btnDeleteRow_Click);

            // dgvRawMaterials
            this.dgvRawMaterials.AllowUserToAddRows = false;
            this.dgvRawMaterials.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRawMaterials.BackgroundColor = System.Drawing.Color.White;
            this.dgvRawMaterials.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRawMaterials.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnRawItem,
            this.clnQty,
            this.clnStdCost,
            this.clnScrapPct,
            this.clnLineCost,
            this.clnDetailNotes});
            this.dgvRawMaterials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRawMaterials.Location = new System.Drawing.Point(3, 53);
            this.dgvRawMaterials.Name = "dgvRawMaterials";
            this.dgvRawMaterials.RowHeadersVisible = false;
            this.dgvRawMaterials.Size = new System.Drawing.Size(750, 339);
            this.dgvRawMaterials.TabIndex = 1;
            this.dgvRawMaterials.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRawMaterials_CellValueChanged);

            // Columns
            this.clnRawItem.HeaderText = "Raw Material / Component";
            this.clnRawItem.Name = "clnRawItem";
            this.clnRawItem.FillWeight = 140F;

            this.clnQty.HeaderText = "Quantity";
            this.clnQty.Name = "clnQty";
            this.clnQty.FillWeight = 50F;
            this.clnQty.MinimumWidth = 70;

            this.clnStdCost.HeaderText = "Unit Cost";
            this.clnStdCost.Name = "clnStdCost";
            this.clnStdCost.FillWeight = 50F;
            this.clnStdCost.MinimumWidth = 70;

            this.clnScrapPct.HeaderText = "Scrap %";
            this.clnScrapPct.Name = "clnScrapPct";
            this.clnScrapPct.FillWeight = 40F;
            this.clnScrapPct.MinimumWidth = 60;

            this.clnLineCost.HeaderText = "Est. Line Cost";
            this.clnLineCost.Name = "clnLineCost";
            this.clnLineCost.ReadOnly = true;
            this.clnLineCost.FillWeight = 60F;

            this.clnDetailNotes.HeaderText = "Notes";
            this.clnDetailNotes.Name = "clnDetailNotes";
            this.clnDetailNotes.FillWeight = 80F;

            // pnlActions
            this.pnlActions.Controls.Add(this.btnNew);
            this.pnlActions.Controls.Add(this.btnSave);
            this.pnlActions.Controls.Add(this.btnDelete);
            this.pnlActions.Controls.Add(this.btnClose);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActions.Location = new System.Drawing.Point(0, 570);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(756, 50);
            this.pnlActions.TabIndex = 2;

            // btnNew
            this.btnNew.BackColor = System.Drawing.Color.LightGray;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNew.Location = new System.Drawing.Point(15, 10);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(80, 32);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            // btnSave
            this.btnSave.BackColor = System.Drawing.Color.SteelBlue;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(105, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 32);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // btnDelete
            this.btnDelete.BackColor = System.Drawing.Color.Crimson;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(205, 10);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(85, 32);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            // btnClose
            this.btnClose.BackColor = System.Drawing.Color.Gray;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(298, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 32);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // frmBOM Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 668);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlHeader);
            this.Name = "frmBOM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bill of Materials (BOM)";
            this.Load += new System.EventHandler(this.frmBOM_Load);

            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnlLeftHeader.ResumeLayout(false);
            this.pnlLeftHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBomList)).EndInit();
            this.grpMaster.ResumeLayout(false);
            this.grpMaster.PerformLayout();
            this.grpDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRawMaterials)).EndInit();
            this.pnlDetailButtons.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlLeftHeader;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridView dgvBomList;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListId;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListFinished;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnListQty;
        private System.Windows.Forms.GroupBox grpMaster;
        private System.Windows.Forms.Label lblBomCode;
        private System.Windows.Forms.TextBox txtBomCode;
        private System.Windows.Forms.Label lblBomName;
        private System.Windows.Forms.TextBox txtBomName;
        private System.Windows.Forms.Label lblFinishedItem;
        private System.Windows.Forms.ComboBox cmbFinishedItem;
        private System.Windows.Forms.Label lblOutputQty;
        private System.Windows.Forms.TextBox txtOutputQty;
        private System.Windows.Forms.Label lblLaborCost;
        private System.Windows.Forms.TextBox txtLaborCost;
        private System.Windows.Forms.Label lblOverheadCost;
        private System.Windows.Forms.TextBox txtOverheadCost;
        private System.Windows.Forms.Label lblTotalCostTitle;
        private System.Windows.Forms.Label lblTotalEstimatedCost;
        private System.Windows.Forms.CheckBox chkIsActive;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.GroupBox grpDetails;
        private System.Windows.Forms.Panel pnlDetailButtons;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.Button btnDeleteRow;
        private System.Windows.Forms.DataGridView dgvRawMaterials;
        private System.Windows.Forms.DataGridViewComboBoxColumn clnRawItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnStdCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnScrapPct;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnLineCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnDetailNotes;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClose;
    }
}
