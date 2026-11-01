namespace ERP.Forms
{
    partial class frmCompanyInfo
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.grpIdentity = new System.Windows.Forms.GroupBox();
            this.lblCompanyName = new System.Windows.Forms.Label();
            this.txtCompanyName = new System.Windows.Forms.TextBox();
            this.lblBopBadge = new System.Windows.Forms.Label();
            this.lblUrCompanyName = new System.Windows.Forms.Label();
            this.txtUrCompanyName = new System.Windows.Forms.TextBox();
            this.lblDescr = new System.Windows.Forms.Label();
            this.txtDescr = new System.Windows.Forms.TextBox();
            this.lblContactHeader = new System.Windows.Forms.Label();
            this.txtContactHeader = new System.Windows.Forms.TextBox();
            this.grpLogo = new System.Windows.Forms.GroupBox();
            this.picLogoPreview = new System.Windows.Forms.PictureBox();
            this.lblNoLogo = new System.Windows.Forms.Label();
            this.btnBrowseLogo = new System.Windows.Forms.Button();
            this.btnRemoveLogo = new System.Windows.Forms.Button();
            this.lblLogoHint = new System.Windows.Forms.Label();
            this.grpContact = new System.Windows.Forms.GroupBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblCell = new System.Windows.Forms.Label();
            this.txtCell = new System.Windows.Forms.TextBox();
            this.lblCell2 = new System.Windows.Forms.Label();
            this.txtCell2 = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblWebsite = new System.Windows.Forms.Label();
            this.txtWebsite = new System.Windows.Forms.TextBox();
            this.grpTax = new System.Windows.Forms.GroupBox();
            this.lblNtn = new System.Windows.Forms.Label();
            this.txtNtn = new System.Windows.Forms.TextBox();
            this.lblStrn = new System.Windows.Forms.Label();
            this.txtStrn = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grpIdentity.SuspendLayout();
            this.grpLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogoPreview)).BeginInit();
            this.grpContact.SuspendLayout();
            this.grpTax.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpIdentity
            // 
            this.grpIdentity.BackColor = System.Drawing.Color.White;
            this.grpIdentity.Controls.Add(this.lblBopBadge);
            this.grpIdentity.Controls.Add(this.txtCompanyName);
            this.grpIdentity.Controls.Add(this.lblCompanyName);
            this.grpIdentity.Controls.Add(this.txtUrCompanyName);
            this.grpIdentity.Controls.Add(this.lblUrCompanyName);
            this.grpIdentity.Controls.Add(this.txtDescr);
            this.grpIdentity.Controls.Add(this.lblDescr);
            this.grpIdentity.Controls.Add(this.txtContactHeader);
            this.grpIdentity.Controls.Add(this.lblContactHeader);
            this.grpIdentity.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpIdentity.Location = new System.Drawing.Point(14, 12);
            this.grpIdentity.Name = "grpIdentity";
            this.grpIdentity.Size = new System.Drawing.Size(480, 232);
            this.grpIdentity.TabIndex = 0;
            this.grpIdentity.TabStop = false;
            this.grpIdentity.Text = "Company Identity";
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.AutoSize = true;
            this.lblCompanyName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblCompanyName.Location = new System.Drawing.Point(16, 26);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Size = new System.Drawing.Size(96, 15);
            this.lblCompanyName.TabIndex = 0;
            this.lblCompanyName.Text = "Company Name :";
            // 
            // lblBopBadge
            // 
            this.lblBopBadge.AutoSize = true;
            this.lblBopBadge.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.lblBopBadge.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.lblBopBadge.Location = new System.Drawing.Point(290, 27);
            this.lblBopBadge.Name = "lblBopBadge";
            this.lblBopBadge.Size = new System.Drawing.Size(172, 13);
            this.lblBopBadge.TabIndex = 1;
            this.lblBopBadge.Text = "Managed by Back Office (BOP)";
            // 
            // txtCompanyName
            // 
            this.txtCompanyName.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.txtCompanyName.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.txtCompanyName.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.txtCompanyName.Location = new System.Drawing.Point(18, 45);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.ReadOnly = true;
            this.txtCompanyName.Size = new System.Drawing.Size(444, 24);
            this.txtCompanyName.TabIndex = 2;
            this.txtCompanyName.TabStop = false;
            // 
            // lblUrCompanyName
            // 
            this.lblUrCompanyName.AutoSize = true;
            this.lblUrCompanyName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblUrCompanyName.Location = new System.Drawing.Point(16, 78);
            this.lblUrCompanyName.Name = "lblUrCompanyName";
            this.lblUrCompanyName.Size = new System.Drawing.Size(188, 15);
            this.lblUrCompanyName.TabIndex = 3;
            this.lblUrCompanyName.Text = "Urdu Company Name / Subtitle :";
            // 
            // txtUrCompanyName
            // 
            this.txtUrCompanyName.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.txtUrCompanyName.Location = new System.Drawing.Point(18, 97);
            this.txtUrCompanyName.MaxLength = 200;
            this.txtUrCompanyName.Name = "txtUrCompanyName";
            this.txtUrCompanyName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtUrCompanyName.Size = new System.Drawing.Size(444, 24);
            this.txtUrCompanyName.TabIndex = 4;
            // 
            // lblDescr
            // 
            this.lblDescr.AutoSize = true;
            this.lblDescr.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblDescr.Location = new System.Drawing.Point(16, 130);
            this.lblDescr.Name = "lblDescr";
            this.lblDescr.Size = new System.Drawing.Size(174, 15);
            this.lblDescr.TabIndex = 5;
            this.lblDescr.Text = "Tagline / Business Description :";
            // 
            // txtDescr
            // 
            this.txtDescr.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.txtDescr.Location = new System.Drawing.Point(18, 149);
            this.txtDescr.MaxLength = 300;
            this.txtDescr.Name = "txtDescr";
            this.txtDescr.Size = new System.Drawing.Size(444, 24);
            this.txtDescr.TabIndex = 6;
            // 
            // lblContactHeader
            // 
            this.lblContactHeader.AutoSize = true;
            this.lblContactHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblContactHeader.Location = new System.Drawing.Point(16, 180);
            this.lblContactHeader.Name = "lblContactHeader";
            this.lblContactHeader.Size = new System.Drawing.Size(156, 15);
            this.lblContactHeader.TabIndex = 7;
            this.lblContactHeader.Text = "Contact Header / Attention :";
            // 
            // txtContactHeader
            // 
            this.txtContactHeader.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.txtContactHeader.Location = new System.Drawing.Point(18, 199);
            this.txtContactHeader.MaxLength = 200;
            this.txtContactHeader.Name = "txtContactHeader";
            this.txtContactHeader.Size = new System.Drawing.Size(444, 24);
            this.txtContactHeader.TabIndex = 8;
            // 
            // grpLogo
            // 
            this.grpLogo.BackColor = System.Drawing.Color.White;
            this.grpLogo.Controls.Add(this.lblNoLogo);
            this.grpLogo.Controls.Add(this.picLogoPreview);
            this.grpLogo.Controls.Add(this.btnBrowseLogo);
            this.grpLogo.Controls.Add(this.btnRemoveLogo);
            this.grpLogo.Controls.Add(this.lblLogoHint);
            this.grpLogo.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpLogo.Location = new System.Drawing.Point(504, 12);
            this.grpLogo.Name = "grpLogo";
            this.grpLogo.Size = new System.Drawing.Size(250, 232);
            this.grpLogo.TabIndex = 1;
            this.grpLogo.TabStop = false;
            this.grpLogo.Text = "Company Logo";
            // 
            // picLogoPreview
            // 
            this.picLogoPreview.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.picLogoPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picLogoPreview.Location = new System.Drawing.Point(18, 28);
            this.picLogoPreview.Name = "picLogoPreview";
            this.picLogoPreview.Size = new System.Drawing.Size(214, 130);
            this.picLogoPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogoPreview.TabIndex = 0;
            this.picLogoPreview.TabStop = false;
            // 
            // lblNoLogo
            // 
            this.lblNoLogo.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.lblNoLogo.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular);
            this.lblNoLogo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblNoLogo.Location = new System.Drawing.Point(25, 78);
            this.lblNoLogo.Name = "lblNoLogo";
            this.lblNoLogo.Size = new System.Drawing.Size(200, 30);
            this.lblNoLogo.TabIndex = 4;
            this.lblNoLogo.Text = "No Logo Uploaded";
            this.lblNoLogo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnBrowseLogo
            // 
            this.btnBrowseLogo.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnBrowseLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowseLogo.FlatAppearance.BorderSize = 0;
            this.btnBrowseLogo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseLogo.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnBrowseLogo.ForeColor = System.Drawing.Color.White;
            this.btnBrowseLogo.Location = new System.Drawing.Point(18, 166);
            this.btnBrowseLogo.Name = "btnBrowseLogo";
            this.btnBrowseLogo.Size = new System.Drawing.Size(118, 28);
            this.btnBrowseLogo.TabIndex = 1;
            this.btnBrowseLogo.Text = "Upload Logo...";
            this.btnBrowseLogo.UseVisualStyleBackColor = false;
            this.btnBrowseLogo.Click += new System.EventHandler(this.btnBrowseLogo_Click);
            // 
            // btnRemoveLogo
            // 
            this.btnRemoveLogo.BackColor = System.Drawing.Color.FromArgb(254, 226, 226);
            this.btnRemoveLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemoveLogo.Enabled = false;
            this.btnRemoveLogo.FlatAppearance.BorderSize = 0;
            this.btnRemoveLogo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveLogo.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular);
            this.btnRemoveLogo.ForeColor = System.Drawing.Color.FromArgb(185, 28, 28);
            this.btnRemoveLogo.Location = new System.Drawing.Point(142, 166);
            this.btnRemoveLogo.Name = "btnRemoveLogo";
            this.btnRemoveLogo.Size = new System.Drawing.Size(90, 28);
            this.btnRemoveLogo.TabIndex = 2;
            this.btnRemoveLogo.Text = "Remove";
            this.btnRemoveLogo.UseVisualStyleBackColor = false;
            this.btnRemoveLogo.Click += new System.EventHandler(this.btnRemoveLogo_Click);
            // 
            // lblLogoHint
            // 
            this.lblLogoHint.Font = new System.Drawing.Font("Segoe UI", 7.75F, System.Drawing.FontStyle.Regular);
            this.lblLogoHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblLogoHint.Location = new System.Drawing.Point(16, 198);
            this.lblLogoHint.Name = "lblLogoHint";
            this.lblLogoHint.Size = new System.Drawing.Size(220, 28);
            this.lblLogoHint.TabIndex = 3;
            this.lblLogoHint.Text = "PNG, JPG, or WEBP up to 5 MB.";
            this.lblLogoHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpContact
            // 
            this.grpContact.BackColor = System.Drawing.Color.White;
            this.grpContact.Controls.Add(this.txtAddress);
            this.grpContact.Controls.Add(this.lblAddress);
            this.grpContact.Controls.Add(this.txtPhone);
            this.grpContact.Controls.Add(this.lblPhone);
            this.grpContact.Controls.Add(this.txtCell);
            this.grpContact.Controls.Add(this.lblCell);
            this.grpContact.Controls.Add(this.txtCell2);
            this.grpContact.Controls.Add(this.lblCell2);
            this.grpContact.Controls.Add(this.txtEmail);
            this.grpContact.Controls.Add(this.lblEmail);
            this.grpContact.Controls.Add(this.txtWebsite);
            this.grpContact.Controls.Add(this.lblWebsite);
            this.grpContact.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpContact.Location = new System.Drawing.Point(14, 252);
            this.grpContact.Name = "grpContact";
            this.grpContact.Size = new System.Drawing.Size(740, 200);
            this.grpContact.TabIndex = 2;
            this.grpContact.TabStop = false;
            this.grpContact.Text = "Address & Contact Information";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblAddress.Location = new System.Drawing.Point(16, 26);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(108, 15);
            this.lblAddress.TabIndex = 0;
            this.lblAddress.Text = "Company Address :";
            // 
            // txtAddress
            // 
            this.txtAddress.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.txtAddress.Location = new System.Drawing.Point(18, 45);
            this.txtAddress.MaxLength = 500;
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAddress.Size = new System.Drawing.Size(704, 44);
            this.txtAddress.TabIndex = 1;
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblPhone.Location = new System.Drawing.Point(16, 96);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(156, 15);
            this.lblPhone.TabIndex = 2;
            this.lblPhone.Text = "Company Phone (Landline) :";
            // 
            // txtPhone
            // 
            this.txtPhone.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.txtPhone.Location = new System.Drawing.Point(18, 114);
            this.txtPhone.MaxLength = 100;
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(222, 24);
            this.txtPhone.TabIndex = 3;
            // 
            // lblCell
            // 
            this.lblCell.AutoSize = true;
            this.lblCell.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblCell.Location = new System.Drawing.Point(256, 96);
            this.lblCell.Name = "lblCell";
            this.lblCell.Size = new System.Drawing.Size(90, 15);
            this.lblCell.TabIndex = 4;
            this.lblCell.Text = "Mobile / Cell 1 :";
            // 
            // txtCell
            // 
            this.txtCell.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.txtCell.Location = new System.Drawing.Point(258, 114);
            this.txtCell.MaxLength = 100;
            this.txtCell.Name = "txtCell";
            this.txtCell.Size = new System.Drawing.Size(222, 24);
            this.txtCell.TabIndex = 5;
            // 
            // lblCell2
            // 
            this.lblCell2.AutoSize = true;
            this.lblCell2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblCell2.Location = new System.Drawing.Point(496, 96);
            this.lblCell2.Name = "lblCell2";
            this.lblCell2.Size = new System.Drawing.Size(173, 15);
            this.lblCell2.TabIndex = 6;
            this.lblCell2.Text = "Secondary Mobile / WhatsApp :";
            // 
            // txtCell2
            // 
            this.txtCell2.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.txtCell2.Location = new System.Drawing.Point(498, 114);
            this.txtCell2.MaxLength = 100;
            this.txtCell2.Name = "txtCell2";
            this.txtCell2.Size = new System.Drawing.Size(224, 24);
            this.txtCell2.TabIndex = 7;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblEmail.Location = new System.Drawing.Point(16, 146);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(87, 15);
            this.lblEmail.TabIndex = 8;
            this.lblEmail.Text = "Email Address :";
            // 
            // txtEmail
            // 
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.txtEmail.Location = new System.Drawing.Point(18, 164);
            this.txtEmail.MaxLength = 150;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(340, 24);
            this.txtEmail.TabIndex = 9;
            // 
            // lblWebsite
            // 
            this.lblWebsite.AutoSize = true;
            this.lblWebsite.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblWebsite.Location = new System.Drawing.Point(376, 146);
            this.lblWebsite.Name = "lblWebsite";
            this.lblWebsite.Size = new System.Drawing.Size(56, 15);
            this.lblWebsite.TabIndex = 10;
            this.lblWebsite.Text = "Website :";
            // 
            // txtWebsite
            // 
            this.txtWebsite.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.txtWebsite.Location = new System.Drawing.Point(378, 164);
            this.txtWebsite.MaxLength = 200;
            this.txtWebsite.Name = "txtWebsite";
            this.txtWebsite.Size = new System.Drawing.Size(344, 24);
            this.txtWebsite.TabIndex = 11;
            // 
            // grpTax
            // 
            this.grpTax.BackColor = System.Drawing.Color.White;
            this.grpTax.Controls.Add(this.txtNtn);
            this.grpTax.Controls.Add(this.lblNtn);
            this.grpTax.Controls.Add(this.txtStrn);
            this.grpTax.Controls.Add(this.lblStrn);
            this.grpTax.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpTax.Location = new System.Drawing.Point(14, 460);
            this.grpTax.Name = "grpTax";
            this.grpTax.Size = new System.Drawing.Size(740, 82);
            this.grpTax.TabIndex = 3;
            this.grpTax.TabStop = false;
            this.grpTax.Text = "Tax & Registration Numbers";
            // 
            // lblNtn
            // 
            this.lblNtn.AutoSize = true;
            this.lblNtn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblNtn.Location = new System.Drawing.Point(16, 26);
            this.lblNtn.Name = "lblNtn";
            this.lblNtn.Size = new System.Drawing.Size(166, 15);
            this.lblNtn.TabIndex = 0;
            this.lblNtn.Text = "NTN (National Tax Number) :";
            // 
            // txtNtn
            // 
            this.txtNtn.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.txtNtn.Location = new System.Drawing.Point(18, 44);
            this.txtNtn.MaxLength = 100;
            this.txtNtn.Name = "txtNtn";
            this.txtNtn.Size = new System.Drawing.Size(340, 24);
            this.txtNtn.TabIndex = 1;
            // 
            // lblStrn
            // 
            this.lblStrn.AutoSize = true;
            this.lblStrn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblStrn.Location = new System.Drawing.Point(376, 26);
            this.lblStrn.Name = "lblStrn";
            this.lblStrn.Size = new System.Drawing.Size(185, 15);
            this.lblStrn.TabIndex = 2;
            this.lblStrn.Text = "STRN / GST Registration Number :";
            // 
            // txtStrn
            // 
            this.txtStrn.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.txtStrn.Location = new System.Drawing.Point(378, 44);
            this.txtStrn.MaxLength = 100;
            this.txtStrn.Name = "txtStrn";
            this.txtStrn.Size = new System.Drawing.Size(344, 24);
            this.txtStrn.TabIndex = 3;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(542, 554);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 32);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "&Save Changes";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.btnClose.Location = new System.Drawing.Point(670, 554);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(84, 32);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular);
            this.lblStatus.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblStatus.Location = new System.Drawing.Point(14, 560);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(510, 20);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmCompanyInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.ClientSize = new System.Drawing.Size(768, 598);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpTax);
            this.Controls.Add(this.grpContact);
            this.Controls.Add(this.grpLogo);
            this.Controls.Add(this.grpIdentity);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCompanyInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Company Information";
            this.Load += new System.EventHandler(this.frmCompanyInfo_Load);
            this.grpIdentity.ResumeLayout(false);
            this.grpIdentity.PerformLayout();
            this.grpLogo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLogoPreview)).EndInit();
            this.grpContact.ResumeLayout(false);
            this.grpContact.PerformLayout();
            this.grpTax.ResumeLayout(false);
            this.grpTax.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpIdentity;
        private System.Windows.Forms.Label lblCompanyName;
        private System.Windows.Forms.TextBox txtCompanyName;
        private System.Windows.Forms.Label lblBopBadge;
        private System.Windows.Forms.Label lblUrCompanyName;
        private System.Windows.Forms.TextBox txtUrCompanyName;
        private System.Windows.Forms.Label lblDescr;
        private System.Windows.Forms.TextBox txtDescr;
        private System.Windows.Forms.Label lblContactHeader;
        private System.Windows.Forms.TextBox txtContactHeader;
        private System.Windows.Forms.GroupBox grpLogo;
        private System.Windows.Forms.PictureBox picLogoPreview;
        private System.Windows.Forms.Label lblNoLogo;
        private System.Windows.Forms.Button btnBrowseLogo;
        private System.Windows.Forms.Button btnRemoveLogo;
        private System.Windows.Forms.Label lblLogoHint;
        private System.Windows.Forms.GroupBox grpContact;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblCell;
        private System.Windows.Forms.TextBox txtCell;
        private System.Windows.Forms.Label lblCell2;
        private System.Windows.Forms.TextBox txtCell2;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblWebsite;
        private System.Windows.Forms.TextBox txtWebsite;
        private System.Windows.Forms.GroupBox grpTax;
        private System.Windows.Forms.Label lblNtn;
        private System.Windows.Forms.TextBox txtNtn;
        private System.Windows.Forms.Label lblStrn;
        private System.Windows.Forms.TextBox txtStrn;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblStatus;
    }
}
