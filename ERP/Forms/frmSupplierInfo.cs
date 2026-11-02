using System;
using System.Collections.Generic;
using System.Drawing;
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
            SetupEntryFlow();
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

        private ERP.Controls.PaginationControl _pager;
        private List<VendorDto> _pagedVendors;
        private System.Windows.Forms.Timer _searchTimer;

        private void EnsurePager()
        {
            if (_pager != null) return;
            _pager = new ERP.Controls.PaginationControl();
            _pager.PageChanged += async (s, e) =>
            {
                await LoadVendorPageAsync(e.PageNumber, e.PageSize);
            };
            _pager.Attach(tpList, dgvVendors);
        }

        private async System.Threading.Tasks.Task LoadVendorPageAsync(int pageNumber, int pageSize)
        {
            try
            {
                string search = txtSearch?.Text?.Trim();
                var result = await _vendorApiService.GetPagedListAsync(pageNumber, pageSize, search: search);
                _pagedVendors = result.Data;
                PopulateGrid(_pagedVendors);
                _pager?.SetPagination(result.CurrentPage, result.TotalPages, result.TotalCount, result.PageSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load vendors: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task FillVendorsAsync()
        {
            _vendors = await _vendorApiService.GetAsync();

            cmbCustomerCode.DataSource    = null;
            cmbCustomerCode.DataSource    = _vendors;
            cmbCustomerCode.DisplayMember = "Title";
            cmbCustomerCode.ValueMember   = "Account";

            EnsurePager();
            await LoadVendorPageAsync(1, _pager?.PageSize ?? 20);
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
                string.Equals(x.Account, code, StringComparison.OrdinalIgnoreCase))
                ?? _pagedVendors?.FirstOrDefault(x =>
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
            if (_searchTimer == null)
            {
                _searchTimer = new System.Windows.Forms.Timer { Interval = 350 };
                _searchTimer.Tick += async (s, ev) =>
                {
                    _searchTimer.Stop();
                    await LoadVendorPageAsync(1, _pager?.PageSize ?? 20);
                };
            }
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private static bool Contains(string s, string q) =>
            !string.IsNullOrEmpty(s) && s.ToLower().Contains(q);

        private void DgvVendors_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvVendors.Rows.Count) return;
            string code = (dgvVendors.Columns.Contains("Account")
                ? dgvVendors.Rows[e.RowIndex].Cells["Account"].Value
                : dgvVendors.Rows[e.RowIndex].Cells[0].Value)?.ToString();
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

        private async void frmSupplierInfo_Load(object sender, EventArgs e)
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

        // ══════════════════════════════════════════════════════════════════
        //  Data-entry flow: Enter = next field,
        //  F5 = Save, F3 = New, Esc = Close, focus highlights
        // ══════════════════════════════════════════════════════════════════

        private Control[] _enterOrder;

        private void SetupEntryFlow()
        {
            _enterOrder = new Control[]
            {
                txtTitle, txtEmail, txtPhone1, txtPhone2, txtFax, txtNic, txtAddress,
                txtQualification, txtSMSNumber, txtIBAN, chkSMSAlert, chkEmailAlert,
                chkActive, chkShowInSales, btnSave
            };

            // Tab order = Enter order (details tab)
            int idx = 1;
            foreach (var c in _enterOrder)
            {
                if (c == chkSMSAlert || c == chkEmailAlert) continue; // children of groupBox1
                if (c == btnSave) continue;                           // child of panel2
                c.TabIndex = idx++;
                c.TabStop = true;
                if (c == txtIBAN) groupBox1.TabIndex = idx++;
            }
            chkSMSAlert.TabIndex = 0;
            chkEmailAlert.TabIndex = 1;
            panel2.TabIndex = idx++;
            btnSave.TabIndex = 0; btnNew.TabIndex = 1; btnDelete.TabIndex = 2; btnClose.TabIndex = 3;

            // Visible focus indication
            foreach (var tb in new TextBox[] { txtTitle, txtEmail, txtPhone1, txtPhone2, txtFax, txtNic,
                                               txtAddress, txtQualification, txtSMSNumber, txtIBAN })
            {
                var box = tb;
                box.Enter += (s, e) => box.BackColor = Color.FromArgb(254, 243, 199);
                box.Leave += (s, e) => box.BackColor = Color.White;
            }

            foreach (var chk in new CheckBox[] { chkActive, chkShowInSales, chkSMSAlert, chkEmailAlert })
            {
                var k = chk;
                k.Enter += (s, e) => k.BackColor = Color.FromArgb(254, 243, 199);
                k.Leave += (s, e) => k.BackColor = Color.Transparent;
            }

            foreach (var btn in new Button[] { btnNew, btnSave, btnDelete, btnClose })
            {
                var b = btn;
                b.Enter += (s, e) => b.Invalidate();
                b.Leave += (s, e) => b.Invalidate();
                b.Paint += (s, e) =>
                {
                    if (!b.Focused) return;
                    using (var p = new Pen(Color.FromArgb(59, 130, 246), 2))
                        e.Graphics.DrawRectangle(p, 1, 1, b.Width - 3, b.Height - 3);
                };
            }

            dgvVendors.KeyDown += DgvVendors_KeyDown;
        }

        private void DgvVendors_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dgvVendors.CurrentRow != null)
            {
                e.Handled = true;
                DgvVendors_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, dgvVendors.CurrentRow.Index));
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            if (keyData == key && (key == Keys.Enter || key == Keys.Tab) && tabControl.SelectedTab == tpDetails)
            {
                if (key == Keys.Enter && ActiveControl is Button == false)
                {
                    int i = Array.IndexOf(_enterOrder, ActiveControl);
                    if (i >= 0)
                    {
                        for (int n = i + 1; n < _enterOrder.Length; n++)
                        {
                            var next = _enterOrder[n];
                            if (!next.Enabled || !next.Visible) continue;
                            next.Focus();
                            if (next is TextBox t) t.SelectAll();
                            return true;
                        }
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void frmCustomerInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5 && btnSave.Enabled) { e.Handled = true; btnSave.PerformClick(); }
            else if (e.KeyCode == Keys.F3 && btnNew.Enabled) { e.Handled = true; btnNew.PerformClick(); }
            else if (e.KeyCode == Keys.Escape && btnClose.Enabled) { e.Handled = true; btnClose.PerformClick(); }
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

