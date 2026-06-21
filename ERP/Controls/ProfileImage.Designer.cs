namespace ERP.Controls
{
    partial class ProfileImage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileImage));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.picHRPicture = new System.Windows.Forms.PictureBox();
            this.btnCam = new System.Windows.Forms.Button();
            this.btnHRSearch = new System.Windows.Forms.Button();
            this.btnHRCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHRPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.picHRPicture);
            this.groupBox1.Controls.Add(this.btnCam);
            this.groupBox1.Controls.Add(this.btnHRSearch);
            this.groupBox1.Controls.Add(this.btnHRCancel);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(151, 230);
            this.groupBox1.TabIndex = 295;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Profile Photo";
            // 
            // picHRPicture
            // 
            this.picHRPicture.Image = ((System.Drawing.Image)(resources.GetObject("picHRPicture.Image")));
            this.picHRPicture.Location = new System.Drawing.Point(10, 21);
            this.picHRPicture.Name = "picHRPicture";
            this.picHRPicture.Size = new System.Drawing.Size(133, 123);
            this.picHRPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picHRPicture.TabIndex = 80;
            this.picHRPicture.TabStop = false;
            // 
            // btnCam
            // 
            this.btnCam.Image = ((System.Drawing.Image)(resources.GetObject("btnCam.Image")));
            this.btnCam.Location = new System.Drawing.Point(55, 150);
            this.btnCam.Name = "btnCam";
            this.btnCam.Size = new System.Drawing.Size(42, 38);
            this.btnCam.TabIndex = 457;
            this.btnCam.Tag = "Off";
            this.btnCam.UseVisualStyleBackColor = true;
            // 
            // btnHRSearch
            // 
            this.btnHRSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnHRSearch.Image")));
            this.btnHRSearch.Location = new System.Drawing.Point(10, 150);
            this.btnHRSearch.Name = "btnHRSearch";
            this.btnHRSearch.Size = new System.Drawing.Size(42, 38);
            this.btnHRSearch.TabIndex = 444;
            this.btnHRSearch.UseVisualStyleBackColor = true;
            // 
            // btnHRCancel
            // 
            this.btnHRCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnHRCancel.Image")));
            this.btnHRCancel.Location = new System.Drawing.Point(101, 150);
            this.btnHRCancel.Name = "btnHRCancel";
            this.btnHRCancel.Size = new System.Drawing.Size(42, 38);
            this.btnHRCancel.TabIndex = 446;
            this.btnHRCancel.UseVisualStyleBackColor = true;
            // 
            // ProfileImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ProfileImage";
            this.Size = new System.Drawing.Size(160, 240);
            this.Load += new System.EventHandler(this.ProfileImage_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picHRPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox picHRPicture;
        private System.Windows.Forms.Button btnCam;
        private System.Windows.Forms.Button btnHRSearch;
        private System.Windows.Forms.Button btnHRCancel;
    }
}
