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
            _defaultImage = picHRPicture.Image;
        }

        public Image DefaultImage => _defaultImage;

        public PictureBox PictureBox => picHRPicture;

        public Image Image
        {
            get => picHRPicture.Image;
            set => picHRPicture.Image = value;
        }

        public Button SearchButton => btnHRSearch;
        public Button CancelButton => btnHRCancel;
        public Button CamButton => btnCam;

        public string MediaId { get; set; }
        public string MediaUrl { get; set; }

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
