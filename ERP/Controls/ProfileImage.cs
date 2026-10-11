using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP.Controls
{
    public partial class ProfileImage : UserControl
    {
        private Image _defaultImage;

        public ProfileImage()
        {
            InitializeComponent();
        }

        private void ProfileImage_Load(object sender, EventArgs e)
        {
            if (_defaultImage == null)
                _defaultImage = picHRPicture.Image;

            try
            {
                var tt = new ToolTip();
                tt.SetToolTip(btnHRSearch, "Browse / Upload Image");
                tt.SetToolTip(btnHRCancel, "Remove Image");
                tt.SetToolTip(btnCam, "Capture Camera Photo");
            }
            catch { }
        }

        public Image DefaultImage
        {
            get => _defaultImage;
            set
            {
                _defaultImage = value;
                if (picHRPicture.Image == null || picHRPicture.Image == _defaultImage)
                {
                    picHRPicture.Image = value;
                }
            }
        }

        public PictureBox PictureBox => picHRPicture;

        public Image Image
        {
            get => picHRPicture.Image;
            set => picHRPicture.Image = value;
        }

        public string Title
        {
            get => groupBox1.Text;
            set => groupBox1.Text = value;
        }

        public PictureBoxSizeMode SizeMode
        {
            get => picHRPicture.SizeMode;
            set => picHRPicture.SizeMode = value;
        }

        public Button SearchButton => btnHRSearch;
        public Button CancelButton => btnHRCancel;
        public Button CamButton => btnCam;

        public string MediaId { get; set; }
        public string MediaUrl { get; set; }

        public static Image CreateProductPlaceholder(int width = 133, int height = 123)
        {
            if (width <= 0) width = 133;
            if (height <= 0) height = 123;
            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                using (var bgBrush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                {
                    g.FillRectangle(bgBrush, 0, 0, width, height);
                }

                using (var borderPen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    g.DrawRectangle(borderPen, 0, 0, width - 1, height - 1);
                }

                int cx = width / 2;
                int cy = height / 2 - 8;

                using (var iconPen = new Pen(Color.FromArgb(148, 163, 184), 2f))
                {
                    var bagRect = new Rectangle(cx - 20, cy - 8, 40, 34);
                    g.DrawRectangle(iconPen, bagRect);
                    g.DrawArc(iconPen, cx - 11, cy - 19, 22, 22, 180, 180);
                }

                using (var font = new Font("Segoe UI", 8f, FontStyle.Regular))
                using (var textBrush = new SolidBrush(Color.FromArgb(148, 163, 184)))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString("No Product Image", font, textBrush, new RectangleF(0, cy + 30, width, 18), sf);
                }
            }
            return bmp;
        }

        public void ClearImage()
        {
            picHRPicture.Image = _defaultImage;
            MediaId = null;
            MediaUrl = null;
        }

        public async Task LoadImageAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                ClearImage();
                return;
            }

            try
            {
                if (url.Contains(":\\") || url.StartsWith("file://") || File.Exists(url))
                {
                    var fileBytes = File.ReadAllBytes(url);
                    using (var ms = new MemoryStream(fileBytes))
                    {
                        picHRPicture.Image = Image.FromStream(ms);
                    }
                    MediaUrl = url;
                    return;
                }

                var handler = new System.Net.Http.HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = delegate { return true; };
                using (var client = new System.Net.Http.HttpClient(handler))
                {
                    var bytes = await client.GetByteArrayAsync(url);
                    using (var ms = new MemoryStream(bytes))
                    {
                        picHRPicture.Image = Image.FromStream(ms);
                    }
                }
                MediaUrl = url;
            }
            catch
            {
                ClearImage();
            }
        }
    }
}
