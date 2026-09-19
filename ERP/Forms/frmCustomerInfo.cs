using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmCustomerInfo : Form
    {
        // ── Data / state fields (UI controls live in Designer.cs) ─────────
        bool Flogin = true;
        private readonly CustomerApiService _customerApiService;
        private readonly InventoryApiService _inventoryApiService;
        private List<CustomerDto> _customers;
        private bool _isSaving;
        private bool _isDeleting;
        private bool _isNewMode = false;

        public frmCustomerInfo()
        {
            InitializeComponent();
            _customerApiService = new CustomerApiService();
            _inventoryApiService = new InventoryApiService();
            _customers = new List<CustomerDto>();
            UserInfo.ApplyFormPermissions(this, AppResource.Customers);
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
            chkSMSAlert.Checked = chkEmailAlert.Checked = false;
            profileImage1?.ClearImage();
            dgvSupplyItems?.Rows.Clear();
            if (lblSupplyCustomerInfo != null)
                lblSupplyCustomerInfo.Text = "Customer: (New Customer)";
        }

        // ══════════════════════════════════════════════════════════════════
        //  Data helpers
        // ══════════════════════════════════════════════════════════════════

        private async System.Threading.Tasks.Task FillCustomersAsync()
        {
            _customers = await _customerApiService.GetAsync();

            cmbCustomerCode.DataSource    = null;
            cmbCustomerCode.DataSource    = _customers;
            cmbCustomerCode.DisplayMember = "Title";
            cmbCustomerCode.ValueMember   = "Account";

            PopulateGrid(_customers);
        }

        private void PopulateGrid(List<CustomerDto> list)
        {
            dgvCustomers.Rows.Clear();
            if (list == null) return;
            foreach (var c in list)
                dgvCustomers.Rows.Add(c.Account, c.Title, c.Phone1,
                                      c.Email, c.Address, c.Active ? "Yes" : "No");
        }

        private async void FillDetail(string code)
        {
            if (Flogin || string.IsNullOrWhiteSpace(code)) return;

            var c = _customers?.FirstOrDefault(x =>
                string.Equals(x.Account, code, StringComparison.OrdinalIgnoreCase));

            if (c == null) { ClearDetail(); txtAccountCode.Text = string.Empty; return; }

            txtAccountCode.Text   = code;
            txtTitle.Text         = c.Title         ?? string.Empty;
            txtEmail.Text         = c.Email         ?? string.Empty;
            txtFax.Text           = c.Fax           ?? string.Empty;
            txtNic.Text           = c.Cnic          ?? string.Empty;
            txtAddress.Text       = c.Address       ?? string.Empty;
            txtQualification.Text = c.Qualification ?? string.Empty;
            txtPhone1.Text        = c.Phone1        ?? string.Empty;
            txtPhone2.Text        = c.Phone2        ?? string.Empty;
            txtSMSNumber.Text     = c.SmsNumber     ?? string.Empty;
            txtIBAN.Text          = c.Iban          ?? string.Empty;
            chkSMSAlert.Checked   = c.SmsAlert;
            chkEmailAlert.Checked = c.EmailAlert;
            chkActive.Checked     = c.Active;

            lblCreatedBy.Text = FormatAudit(c.CreatedBy,       c.CreatedOn);
            lblEditBy.Text    = FormatAudit(c.LastModifiedBy,  c.LastModifiedOn);

            if (profileImage1 != null)
            {
                profileImage1.MediaId = c.MediaId;
                profileImage1.MediaUrl = c.MediaUrl;
                await profileImage1.LoadImageAsync(c.MediaUrl);
            }

            dgvSupplyItems?.Rows.Clear();
            if (lblSupplyCustomerInfo != null)
                lblSupplyCustomerInfo.Text = $"Customer: {c.Title} ({code})";

            var supplyList = c.SupplyItems;
            if (supplyList == null || supplyList.Count == 0)
            {
                try
                {
                    supplyList = await _customerApiService.GetSupplyItemsAsync(code);
                }
                catch
                {
                    supplyList = new List<CustomerSupplyItemDto>();
                }
            }

            if (supplyList != null && dgvSupplyItems != null)
            {
                foreach (var item in supplyList)
                {
                    int rowIdx = dgvSupplyItems.Rows.Add();
                    var r = dgvSupplyItems.Rows[rowIdx];
                    r.Cells[clnSupplyItemId.Index].Value = item.ItemId;
                    r.Cells[clnSupplyQty.Index].Value = (item.Qty > 0 ? item.Qty : 1).ToString("0.##");
                    if (clnSupplySecQty.Visible)
                        r.Cells[clnSupplySecQty.Index].Value = (item.SecQty ?? 0).ToString("0.##");
                    r.Cells[clnSupplyRate.Index].Value = item.Rate.HasValue ? item.Rate.Value.ToString("0.##") : string.Empty;
                    r.Cells[clnSupplyDiscount.Index].Value = item.Discount.HasValue ? item.Discount.Value.ToString("0.##") : string.Empty;
                    r.Cells[clnSupplyAddLess.Index].Value = item.AddLess.HasValue ? item.AddLess.Value.ToString("0.##") : string.Empty;
                }
            }
        }

        private static string FormatAudit(string by, DateTime? on) =>
            string.IsNullOrWhiteSpace(by) || !on.HasValue
                ? "-"
                : by + "  |  " + on.Value.ToString("dd-MMM-yyyy hh:mm tt");

        private CustomerUpsertApiRequest BuildRequest()
        {
            var supplyItems = new List<CustomerSupplyItemDto>();
            if (dgvSupplyItems != null)
            {
                foreach (DataGridViewRow r in dgvSupplyItems.Rows)
                {
                    if (r.IsNewRow) continue;
                    var itemIdObj = r.Cells[clnSupplyItemId.Index].Value;
                    if (itemIdObj == null || string.IsNullOrWhiteSpace(itemIdObj.ToString())) continue;

                    decimal.TryParse(Convert.ToString(r.Cells[clnSupplyQty.Index].Value), out decimal qty);
                    decimal? secQty = null;
                    if (decimal.TryParse(Convert.ToString(r.Cells[clnSupplySecQty.Index].Value), out decimal sqVal))
                        secQty = sqVal;

                    decimal? rate = decimal.TryParse(Convert.ToString(r.Cells[clnSupplyRate.Index].Value), out decimal rVal) ? (decimal?)rVal : null;
                    decimal? discount = decimal.TryParse(Convert.ToString(r.Cells[clnSupplyDiscount.Index].Value), out decimal dVal) ? (decimal?)dVal : null;
                    decimal? addLess = decimal.TryParse(Convert.ToString(r.Cells[clnSupplyAddLess.Index].Value), out decimal aVal) ? (decimal?)aVal : null;

                    supplyItems.Add(new CustomerSupplyItemDto
                    {
                        ItemId = itemIdObj.ToString(),
                        Qty = qty > 0 ? qty : 1,
                        SecQty = secQty,
                        Rate = rate,
                        Discount = discount,
                        AddLess = addLess
                    });
                }
            }

            return new CustomerUpsertApiRequest
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
                MediaId       = profileImage1?.MediaId,
                SupplyItems   = supplyItems
            };
        }

        // ══════════════════════════════════════════════════════════════════
        //  List-tab events
        // ══════════════════════════════════════════════════════════════════

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string q = txtSearch.Text.Trim().ToLower();
            var list = string.IsNullOrWhiteSpace(q)
                ? _customers
                : _customers?.Where(c =>
                    Contains(c.Title,   q) || Contains(c.Account, q) ||
                    Contains(c.Phone1,  q) || Contains(c.Email,   q) ||
                    Contains(c.Address, q)).ToList();
            PopulateGrid(list);
        }

        private static bool Contains(string s, string q) =>
            !string.IsNullOrEmpty(s) && s.ToLower().Contains(q);

        private void DgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string code = dgvCustomers.Rows[e.RowIndex].Cells["Account"].Value?.ToString();
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
            try   { await FillCustomersAsync(); }
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
                MessageBox.Show("Customer Name is required.", "Validation",
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
                    string newCode = await _customerApiService.CreateAsync(BuildRequest());
                    MessageBox.Show("Saved successfully!\nCode: " + newCode, "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await FillCustomersAsync();
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
                    await _customerApiService.UpsertAsync(code, BuildRequest());
                    MessageBox.Show("Saved successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await FillCustomersAsync();
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
                MessageBox.Show("No customer selected.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(
                    $"Delete customer '{txtTitle.Text}' ({code})?\nThis cannot be undone.",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes) return;

            _isDeleting = true;
            btnDelete.Enabled = false;
            try
            {
                await _customerApiService.DeleteAsync(code);
                MessageBox.Show("Deleted successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await FillCustomersAsync();
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
                profileImage1.SearchButton.Click += btnCustomerSearch_Click;
                profileImage1.CancelButton.Click += btnCustomerCancel_Click;
                dgvSupplyItems.DataError += dgvSupplyItems_DataError;

                var items = await _inventoryApiService.GetLookupAsync(null);
                clnSupplyItemId.DataSource = items;
                clnSupplyItemId.DisplayMember = "Title";
                clnSupplyItemId.ValueMember = "Id";
                clnSupplySecQty.Visible = ApiSession.HasSecondaryQty;

                await FillCustomersAsync();
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

        private void dgvSupplyItems_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void btnAddSupplyItem_Click(object sender, EventArgs e)
        {
            int idx = dgvSupplyItems.Rows.Add();
            dgvSupplyItems.Rows[idx].Cells[clnSupplyQty.Index].Value = "1";
            if (clnSupplySecQty.Visible)
                dgvSupplyItems.Rows[idx].Cells[clnSupplySecQty.Index].Value = "0";
        }

        private void btnDeleteSupplyItem_Click(object sender, EventArgs e)
        {
            if (dgvSupplyItems.CurrentRow != null && !dgvSupplyItems.CurrentRow.IsNewRow)
            {
                dgvSupplyItems.Rows.Remove(dgvSupplyItems.CurrentRow);
            }
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

        private async void btnCustomerSearch_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                        var presigned = await _customerApiService.GetPresignedUploadUrlAsync(fileName);
                        if (presigned != null)
                        {
                            await _customerApiService.UploadFileAsync(presigned.UploadUrl, openFileDialog.FileName);
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

        private void btnCustomerCancel_Click(object sender, EventArgs e)
        {
            profileImage1.ClearImage();
        }

        // ── Helper ────────────────────────────────────────────────────────
        private static void ShowError(Exception ex) =>
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
