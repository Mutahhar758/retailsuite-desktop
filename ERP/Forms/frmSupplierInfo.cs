using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmSupplierInfo : Form
    {
        // ── Data / state fields ──────────────────────────────────────────
        bool Flogin = true;
        private readonly VendorApiService _vendorApiService;
        private List<VendorDto> _vendors;
        private bool _isSaving;
        private bool _isDeleting;
        private bool _isNewMode = false;

        public frmSupplierInfo()
        {
            InitializeComponent();
            _vendorApiService = new VendorApiService();
            _vendors = new List<VendorDto>();
            UserInfo.ApplyFormPermissions(this, AppResource.Vendors);
        }

        // ══════════════════════════════════════════════════════════════════
        //  State
        // ══════════════════════════════════════════════════════════════════

        private void SetNewMode(bool isNew)
        {
            _isNewMode = isNew;
            if (isNew)
            {
                btnNew.Text = "Cancel";
                txtAccountCode.Text = "(Auto-Generated)";
                ClearDetail();
                txtTitle.Focus();
                btnDelete.Enabled = false;
            }
            else
            {
                btnNew.Text = "&New";
                btnDelete.Enabled = (cmbCustomerCode.SelectedValue != null);
                string code = cmbCustomerCode.SelectedValue?.ToString();
                if (!string.IsNullOrEmpty(code))
                    FillDetail(code);
            }
        }

        private void ClearDetail()
        {
            txtTitle.Text = txtEmail.Text = txtFax.Text = txtNic.Text = string.Empty;
            txtAddress.Text = txtQualification.Text = txtPhone1.Text = txtPhone2.Text = string.Empty;
            txtSMSNumber.Text = txtIBAN.Text = string.Empty;
            lblCreatedBy.Text = lblEditBy.Text = "-";
            chkActive.Checked = true;
            chkSMSAlert.Checked = chkEmailAlert.Checked = chkShowInSales.Checked = false;
            profileImage1?.ClearImage();
        }

        // ══════════════════════════════════════════════════════════════════
        //  Data helpers
        // ══════════════════════════════════════════════════════════════════

        private async System.Threading.Tasks.Task FillVendorsAsync()
        {
            _vendors = await _vendorApiService.GetAsync();

            cmbCustomerCode.DataSource    = null;
            cmbCustomerCode.DataSource    = _vendors;
            cmbCustomerCode.DisplayMember = "Title";
            cmbCustomerCode.ValueMember   = "Account";

            PopulateGrid(_vendors);
        }

        private void PopulateGrid(List<VendorDto> list)
        {
            dgvVendors.Rows.Clear();
            if (list == null) return;
            foreach (var v in list)
                dgvVendors.Rows.Add(v.Account, v.Title, v.Phone1,
                                    v.Email, v.Address, v.Active ? "Yes" : "No");
        }

        private async void FillDetail(string code)
        {
            if (Flogin || string.IsNullOrWhiteSpace(code)) return;

            var v = _vendors?.FirstOrDefault(x =>
                string.Equals(x.Account, code, StringComparison.OrdinalIgnoreCase));

            if (v == null) { ClearDetail(); txtAccountCode.Text = string.Empty; return; }

            txtAccountCode.Text    = code;
            txtTitle.Text          = v.Title         ?? string.Empty;
            txtEmail.Text          = v.Email         ?? string.Empty;
            txtFax.Text            = v.Fax           ?? string.Empty;
            txtNic.Text            = v.Cnic          ?? string.Empty;
            txtAddress.Text        = v.Address       ?? string.Empty;
            txtQualification.Text  = v.Qualification ?? string.Empty;
            txtPhone1.Text         = v.Phone1        ?? string.Empty;
            txtPhone2.Text         = v.Phone2        ?? string.Empty;
            txtSMSNumber.Text      = v.SmsNumber     ?? string.Empty;
            txtIBAN.Text           = v.Iban          ?? string.Empty;
            chkSMSAlert.Checked    = v.SmsAlert;
            chkEmailAlert.Checked  = v.EmailAlert;
            chkActive.Checked      = v.Active;
            chkShowInSales.Checked = v.ShowInSales;

            lblCreatedBy.Text = FormatAudit(v.CreatedBy,      v.CreatedOn);
            lblEditBy.Text    = FormatAudit(v.LastModifiedBy, v.LastModifiedOn);

            if (profileImage1 != null)
            {
                profileImage1.MediaId = v.MediaId;
                profileImage1.MediaUrl = v.MediaUrl;
                await profileImage1.LoadImageAsync(v.MediaUrl);
            }
        }

        private static string FormatAudit(string by, DateTime? on) =>
            string.IsNullOrWhiteSpace(by) || !on.HasValue
                ? "-"
                : by + "  |  " + on.Value.ToString("dd-MMM-yyyy hh:mm tt");

        private VendorUpsertApiRequest BuildRequest() => new VendorUpsertApiRequest
        {
            Title         = txtTitle.Text,
            Email         = txtEmail.Text,
            Fax           = txtFax.Text,
            Cnic          = txtNic.Text,
            Address       = txtAddress.Text,
            Qualification = txtQualification.Text,
            Phone1        = txtPhone1.Text,
            Phone2        = txtPhone2.Text,
            SmsNumber     = txtSMSNumber.Text,
            Iban          = txtIBAN.Text,
            SmsAlert      = chkSMSAlert.Checked,
            EmailAlert    = chkEmailAlert.Checked,
            Active        = chkActive.Checked,
            ShowInSales   = chkShowInSales.Checked,
            MediaId       = profileImage1?.MediaId
        };

        // ══════════════════════════════════════════════════════════════════
        //  List-tab events
        // ══════════════════════════════════════════════════════════════════

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string q = txtSearch.Text.Trim().ToLower();
            var list = string.IsNullOrWhiteSpace(q)
                ? _vendors
                : _vendors?.Where(v =>
                    Contains(v.Title,   q) || Contains(v.Account, q) ||
                    Contains(v.Phone1,  q) || Contains(v.Email,   q) ||
                    Contains(v.Address, q)).ToList();
            PopulateGrid(list);
        }

        private static bool Contains(string s, string q) =>
            !string.IsNullOrEmpty(s) && s.ToLower().Contains(q);

        private void DgvVendors_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string code = dgvVendors.Rows[e.RowIndex].Cells["Account"].Value?.ToString();
            if (string.IsNullOrEmpty(code)) return;

            _isNewMode = false;
            btnNew.Text = "&New";
            cmbCustomerCode.SelectedValue = code;
            txtAccountCode.Text = code;
            FillDetail(code);
            tabControl.SelectedTab = tpDetails;
            btnDelete.Enabled = true;
        }

        private void BtnAddNew_Click(object sender, EventArgs e)
        {
            SetNewMode(true);
            tabControl.SelectedTab = tpDetails;
        }

        private async void BtnRefreshList_Click(object sender, EventArgs e)
        {
            try   { await FillVendorsAsync(); }
            catch (Exception ex) { ShowError(ex); }
        }

        // ══════════════════════════════════════════════════════════════════
        //  Detail-tab button events
        // ══════════════════════════════════════════════════════════════════

        private void btnNew_Click(object sender, EventArgs e) => SetNewMode(!_isNewMode);

        private void btnCancel_Click(object sender, EventArgs e) => this.Close();

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving) return;
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Vendor Name is required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Save changes?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            _isSaving = true;
            btnSave.Enabled = false;
            try
            {
                if (_isNewMode)
                {
                    string newCode = await _vendorApiService.CreateAsync(BuildRequest());
                    MessageBox.Show("Saved successfully!\nCode: " + newCode, "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await FillVendorsAsync();
                    _isNewMode = false;
                    btnNew.Text = "&New";
                    cmbCustomerCode.SelectedValue = newCode;
                    FillDetail(newCode);
                    btnDelete.Enabled = true;
                }
                else
                {
                    string code = cmbCustomerCode.SelectedValue?.ToString();
                    if (string.IsNullOrEmpty(code)) return;
                    await _vendorApiService.UpsertAsync(code, BuildRequest());
                    MessageBox.Show("Saved successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await FillVendorsAsync();
                    cmbCustomerCode.SelectedValue = code;
                    FillDetail(code);
                }
            }
            catch (Exception ex) { ShowError(ex); }
            finally { _isSaving = false; btnSave.Enabled = true; }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_isDeleting || _isNewMode) return;
            string code = cmbCustomerCode.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("No vendor selected.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(
                    $"Delete vendor '{txtTitle.Text}' ({code})?\nThis cannot be undone.",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes) return;

            _isDeleting = true;
            btnDelete.Enabled = false;
            try
            {
                await _vendorApiService.DeleteAsync(code);
                MessageBox.Show("Deleted successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await FillVendorsAsync();
                ClearDetail();
                txtAccountCode.Text = string.Empty;
                tabControl.SelectedTab = tpList;
            }
            catch (Exception ex) { ShowError(ex); }
            finally { _isDeleting = false; btnDelete.Enabled = false; }
        }

        // ══════════════════════════════════════════════════════════════════
        //  Form events
        // ══════════════════════════════════════════════════════════════════

        private async void frmCustomerInfo_Load(object sender, EventArgs e)
        {
            try
            {
                profileImage1.SearchButton.Click += btnVendorSearch_Click;
                profileImage1.CancelButton.Click += btnVendorCancel_Click;
                await FillVendorsAsync();
                Flogin = false;
                string code = cmbCustomerCode.SelectedValue?.ToString();
                if (!string.IsNullOrEmpty(code))
                {
                    txtAccountCode.Text = code;
                    FillDetail(code);
                }
                btnDelete.Enabled = !string.IsNullOrEmpty(code);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void frmCustomerInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{tab}");
        }

        private void cmbCustomerCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Flogin)
            {
                string code = cmbCustomerCode.SelectedValue?.ToString();
                txtAccountCode.Text = code ?? string.Empty;
                FillDetail(code);
            }
        }

        private async void btnVendorSearch_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                        var presigned = await _vendorApiService.GetPresignedUploadUrlAsync(fileName);
                        if (presigned != null)
                        {
                            await _vendorApiService.UploadFileAsync(presigned.UploadUrl, openFileDialog.FileName);
                            profileImage1.MediaId = presigned.FileId;
                            profileImage1.MediaUrl = openFileDialog.FileName;
                            profileImage1.PictureBox.Image = System.Drawing.Image.FromFile(openFileDialog.FileName);
                            MessageBox.Show("Image uploaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to upload image: {ex.Message}", "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnVendorCancel_Click(object sender, EventArgs e)
        {
            profileImage1.ClearImage();
        }

        private static void ShowError(Exception ex) =>
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
