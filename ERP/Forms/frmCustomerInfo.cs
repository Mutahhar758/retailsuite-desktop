using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmCustomerInfo : Form
    {
        // ── Data / state fields (UI controls live in Designer.cs) ─────────
        bool Flogin = true;
        private readonly CustomerApiService _customerApiService;
        private List<CustomerDto> _customers;
        private bool _isSaving;
        private bool _isDeleting;
        private bool _isNewMode = false;

        public frmCustomerInfo()
        {
            InitializeComponent();
            _customerApiService = new CustomerApiService();
            _customers = new List<CustomerDto>();
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

        private void FillDetail(string code)
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
        }

        private static string FormatAudit(string by, DateTime? on) =>
            string.IsNullOrWhiteSpace(by) || !on.HasValue
                ? "-"
                : by + "  |  " + on.Value.ToString("dd-MMM-yyyy hh:mm tt");

        private CustomerUpsertApiRequest BuildRequest() => new CustomerUpsertApiRequest
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
            Active        = chkActive.Checked
        };

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

        // ── Helper ────────────────────────────────────────────────────────
        private static void ShowError(Exception ex) =>
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
