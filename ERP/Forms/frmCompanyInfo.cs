using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP.Forms
{
    public partial class frmCompanyInfo : Form
    {
        private readonly CompanyApiService _companyApiService = new CompanyApiService();
        private string _logoMediaId;
        private string _pendingUploadFilePath;

        public frmCompanyInfo()
        {
            InitializeComponent();
        }

        private async void frmCompanyInfo_Load(object sender, EventArgs e)
        {
            await LoadCompanyInfoAsync();
        }

        private async Task LoadCompanyInfoAsync()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                lblStatus.Text = "Loading company details...";

                var company = await _companyApiService.GetCurrentAsync();
                PopulateFields(company);
                await LoadLogoPreviewAsync(company.LogoUrl);

                lblStatus.Text = company.LastModifiedOn.HasValue
                    ? "Last updated on " + company.LastModifiedOn.Value.ToLocalTime().ToString("dd-MMM-yyyy hh:mm tt")
                    : "Ready";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading company details.";
                MessageBox.Show("Failed to load company information: " + ex.Message, "Company Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void PopulateFields(CompanyInfoDto company)
        {
            txtCompanyName.Text = company?.CompanyName ?? CompanyInfo.CompanyName ?? "";
            txtUrCompanyName.Text = company?.UrCompanyName ?? "";
            txtDescr.Text = company?.Descr ?? "";
            txtContactHeader.Text = company?.ContactHeader ?? "";
            txtAddress.Text = company?.Address ?? "";
            txtPhone.Text = company?.Phone ?? "";
            txtCell.Text = company?.Cell ?? "";
            txtCell2.Text = company?.Cell2 ?? "";
            txtEmail.Text = company?.Email ?? "";
            txtWebsite.Text = company?.Website ?? "";
            txtNtn.Text = company?.Ntn ?? "";
            txtStrn.Text = company?.Strn ?? "";

            _logoMediaId = company?.LogoMediaId ?? "";
            _pendingUploadFilePath = null;
            btnRemoveLogo.Enabled = !string.IsNullOrWhiteSpace(_logoMediaId) || !string.IsNullOrWhiteSpace(company?.LogoUrl);
        }

        private async Task LoadLogoPreviewAsync(string logoUrl)
        {
            if (string.IsNullOrWhiteSpace(logoUrl))
            {
                SetLogoPreviewImage(null);
                return;
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var bytes = await httpClient.GetByteArrayAsync(logoUrl);
                    if (bytes != null && bytes.Length > 0)
                    {
                        using (var ms = new MemoryStream(bytes))
                        {
                            var img = Image.FromStream(ms);
                            SetLogoPreviewImage(new Bitmap(img));
                        }
                    }
                }
            }
            catch
            {
                // Ignore preview load error if media URL is unreachable
            }
        }

        private void SetLogoPreviewImage(Image newImage)
        {
            var old = picLogoPreview.Image;
            picLogoPreview.Image = newImage;
            old?.Dispose();
            lblNoLogo.Visible = newImage == null;
        }

        private void btnBrowseLogo_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Company Logo";
                ofd.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.webp;*.bmp)|*.png;*.jpg;*.jpeg;*.webp;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var fileInfo = new FileInfo(ofd.FileName);
                    if (fileInfo.Length > 5 * 1024 * 1024)
                    {
                        MessageBox.Show("Logo image must be smaller than 5 MB.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                        {
                            var img = Image.FromStream(fs);
                            SetLogoPreviewImage(new Bitmap(img));
                        }

                        _pendingUploadFilePath = ofd.FileName;
                        btnRemoveLogo.Enabled = true;
                        lblStatus.Text = "Selected new logo: " + Path.GetFileName(ofd.FileName) + " (click Save to upload)";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Invalid image file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnRemoveLogo_Click(object sender, EventArgs e)
        {
            _logoMediaId = "";
            _pendingUploadFilePath = null;
            SetLogoPreviewImage(null);
            btnRemoveLogo.Enabled = false;
            lblStatus.Text = "Logo removed (click Save to apply)";
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!UserInfo.IsOwner && !UserInfo.HasPermission(AppAction.Update, AppResource.Settings))
            {
                MessageBox.Show("You do not have permission to update company settings.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnSave.Enabled = false;
                btnBrowseLogo.Enabled = false;
                btnRemoveLogo.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;

                if (!string.IsNullOrWhiteSpace(_pendingUploadFilePath) && File.Exists(_pendingUploadFilePath))
                {
                    lblStatus.Text = "Uploading company logo...";
                    var presigned = await _companyApiService.GetLogoUploadUrlAsync(Path.GetFileName(_pendingUploadFilePath));
                    if (presigned != null && !string.IsNullOrWhiteSpace(presigned.UploadUrl))
                    {
                        await _companyApiService.UploadFileAsync(presigned.UploadUrl, _pendingUploadFilePath);
                        _logoMediaId = presigned.FileId;
                        _pendingUploadFilePath = null;
                    }
                }

                lblStatus.Text = "Saving company information...";
                var updateRequest = new CompanyUpdateRequestDto
                {
                    UrCompanyName = txtUrCompanyName.Text.Trim(),
                    Descr = txtDescr.Text.Trim(),
                    ContactHeader = txtContactHeader.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Cell = txtCell.Text.Trim(),
                    Cell2 = txtCell2.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Website = txtWebsite.Text.Trim(),
                    Ntn = txtNtn.Text.Trim(),
                    Strn = txtStrn.Text.Trim(),
                    LogoMediaId = _logoMediaId ?? ""
                };

                var updated = await _companyApiService.UpdateCurrentAsync(updateRequest);

                // Update in-memory CompanyInfo static properties
                CompanyInfo.CompanyName = updated.CompanyName;
                CompanyInfo.UrCompanyName = updated.UrCompanyName;
                CompanyInfo.Description = updated.Descr;
                CompanyInfo.ContactHead = updated.ContactHeader ?? "";
                CompanyInfo.Address = updated.Address;
                CompanyInfo.Phone = updated.Phone;
                CompanyInfo.Cell = updated.Cell;
                CompanyInfo.Cell2 = updated.Cell2;
                CompanyInfo.Email = updated.Email;
                CompanyInfo.Website = updated.Website;
                CompanyInfo.Ntn = updated.Ntn;
                CompanyInfo.Strn = updated.Strn;
                CompanyInfo.LogoMediaId = updated.LogoMediaId;
                CompanyInfo.LogoUrl = updated.LogoUrl;

                PopulateFields(updated);

                if (this.MdiParent is frmMainMenu mainMenu)
                {
                    await mainMenu.RefreshCompanyBrandingAsync();
                }

                lblStatus.Text = "Saved successfully at " + DateTime.Now.ToString("hh:mm:ss tt");
                MessageBox.Show("Company information saved successfully!", "Company Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Save failed: " + ex.Message;
                MessageBox.Show("Error saving company information: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
                btnBrowseLogo.Enabled = true;
                btnRemoveLogo.Enabled = !string.IsNullOrWhiteSpace(_logoMediaId) || picLogoPreview.Image != null;
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
