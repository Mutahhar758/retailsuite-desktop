using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmBankInfo : Form
    {
        private readonly BankAccountApiService _bankAccountApiService;
        private List<BankAccountDto> _bankAccounts;
        private string _currentAccountCode;
        private bool _isSaving;
        private bool _isDeleting;
        private bool _isNewMode;

        public frmBankInfo()
        {
            InitializeComponent();
            _bankAccountApiService = new BankAccountApiService();
            _bankAccounts = new List<BankAccountDto>();

            InitPopularBanks();
            cmbCurrency.SelectedItem = "PKR";
            UserInfo.ApplyFormPermissions(this, AppResource.BankAccounts);
        }

        private void InitPopularBanks()
        {
            var popularBanks = new AutoCompleteStringCollection();
            popularBanks.AddRange(new string[]
            {
                "Meezan Bank",
                "Habib Bank Limited (HBL)",
                "United Bank Limited (UBL)",
                "MCB Bank",
                "Bank Alfalah",
                "Allied Bank Limited (ABL)",
                "Bank Al Habib",
                "Standard Chartered Bank",
                "Faysal Bank",
                "Askari Bank",
                "Bank of Punjab (BOP)",
                "Dubai Islamic Bank",
                "Soneri Bank",
                "JS Bank",
                "National Bank of Pakistan (NBP)",
                "Summit Bank",
                "Al Baraka Bank"
            });
            txtBankName.AutoCompleteCustomSource = popularBanks;
        }

        private async void FrmBankInfo_Load(object sender, EventArgs e)
        {
            try
            {
                await FillBankAccountsAsync();
                if (_bankAccounts != null && _bankAccounts.Count > 0)
                {
                    _currentAccountCode = _bankAccounts[0].Account;
                    FillDetail(_currentAccountCode);
                    SetNewMode(false);
                }
                else
                {
                    SetNewMode(true);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  State Management
        // ══════════════════════════════════════════════════════════════════

        private void RdoAccountType_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccountTypeVisibility();
        }

        private void UpdateAccountTypeVisibility()
        {
            bool isCash = rdoCash.Checked;
            lblBankName.Visible = !isCash;
            txtBankName.Visible = !isCash;
            lblAccountNumber.Visible = !isCash;
            txtAccountNumber.Visible = !isCash;
            lblIBAN.Visible = !isCash;
            txtIBAN.Visible = !isCash;
            lblBranchName.Visible = !isCash;
            txtBranchName.Visible = !isCash;
            lblBranchCode.Visible = !isCash;
            txtBranchCode.Visible = !isCash;
            lblSwiftCode.Visible = !isCash;
            txtSwiftCode.Visible = !isCash;
            btnReconcile.Enabled = !isCash && !string.IsNullOrEmpty(_currentAccountCode);

            lblAccountTitle.Text = isCash ? "Cash Title * :" : "Account Title * :";
        }

        private void SetNewMode(bool isNew)
        {
            _isNewMode = isNew;
            rdoBank.Enabled = isNew;
            rdoCash.Enabled = isNew;
            if (isNew)
            {
                btnNew.Text = "Cancel";
                _currentAccountCode = null;
                ClearDetail();
                if (rdoCash.Checked)
                    txtAccountTitle.Focus();
                else
                    txtBankName.Focus();
                btnDelete.Enabled = false;
                btnStatement.Enabled = false;
                btnReconcile.Enabled = false;
            }
            else
            {
                btnNew.Text = "&New";
                bool hasSelection = !string.IsNullOrEmpty(_currentAccountCode);
                btnDelete.Enabled = hasSelection;
                btnStatement.Enabled = hasSelection;
                btnReconcile.Enabled = hasSelection && !rdoCash.Checked;
            }
        }

        private void ClearDetail()
        {
            rdoBank.Checked = true;
            UpdateAccountTypeVisibility();
            txtBankName.Text = string.Empty;
            txtAccountTitle.Text = string.Empty;
            txtAccountNumber.Text = string.Empty;
            txtIBAN.Text = string.Empty;
            txtBranchName.Text = string.Empty;
            txtBranchCode.Text = string.Empty;
            txtSwiftCode.Text = string.Empty;
            txtOpeningBalance.Text = "0.00";
            cmbCurrency.SelectedItem = "PKR";
            chkActive.Checked = true;
            txtNotes.Text = string.Empty;
            lblCreatedBy.Text = "-";
            lblEditBy.Text = "-";
        }

        // ══════════════════════════════════════════════════════════════════
        //  Data Helpers
        // ══════════════════════════════════════════════════════════════════

        private async Task FillBankAccountsAsync()
        {
            _bankAccounts = await _bankAccountApiService.GetAsync();
            PopulateGrid(_bankAccounts);
        }

        private void PopulateGrid(List<BankAccountDto> list)
        {
            dgvBankAccounts.Rows.Clear();
            if (list == null) return;

            foreach (var b in list)
            {
                bool isCash = string.Equals(b.AccountType, "Cash", StringComparison.OrdinalIgnoreCase);
                dgvBankAccounts.Rows.Add(
                    b.Account,
                    isCash ? "Cash" : "Bank",
                    isCash ? "Cash Drawer" : (b.BankName ?? "-"),
                    b.AccountTitle,
                    isCash ? "-" : (b.AccountNumber ?? "-"),
                    isCash ? "-" : (b.BranchName ?? "-"),
                    b.CurrentBalance.ToString("N2"),
                    b.Currency ?? "PKR",
                    b.Active ? "Yes" : "No"
                );
            }
        }

        private void FillDetail(string accountCode)
        {
            if (string.IsNullOrWhiteSpace(accountCode)) return;

            var b = _bankAccounts?.FirstOrDefault(x =>
                string.Equals(x.Account, accountCode, StringComparison.OrdinalIgnoreCase));

            if (b == null)
            {
                ClearDetail();
                return;
            }

            _currentAccountCode = b.Account;
            bool isCash = string.Equals(b.AccountType, "Cash", StringComparison.OrdinalIgnoreCase);
            if (isCash)
                rdoCash.Checked = true;
            else
                rdoBank.Checked = true;
            UpdateAccountTypeVisibility();

            txtBankName.Text = b.BankName ?? string.Empty;
            txtAccountTitle.Text = b.AccountTitle ?? string.Empty;
            txtAccountNumber.Text = b.AccountNumber ?? string.Empty;
            txtIBAN.Text = b.Iban ?? string.Empty;
            txtBranchName.Text = b.BranchName ?? string.Empty;
            txtBranchCode.Text = b.BranchCode ?? string.Empty;
            txtSwiftCode.Text = b.SwiftCode ?? string.Empty;
            txtOpeningBalance.Text = b.OpeningBalance.ToString("F2");

            if (!string.IsNullOrEmpty(b.Currency) && cmbCurrency.Items.Contains(b.Currency))
                cmbCurrency.SelectedItem = b.Currency;
            else
                cmbCurrency.SelectedItem = "PKR";

            chkActive.Checked = b.Active;
            txtNotes.Text = b.Notes ?? string.Empty;

            lblCreatedBy.Text = FormatAudit(b.CreatedBy, b.CreatedOn);
            lblEditBy.Text = FormatAudit(b.LastModifiedBy, b.LastModifiedOn);
        }

        private static string FormatAudit(string by, DateTime? on) =>
            string.IsNullOrWhiteSpace(by) || !on.HasValue
                ? "-"
                : by + "  |  " + on.Value.ToString("dd-MMM-yyyy hh:mm tt");

        // ══════════════════════════════════════════════════════════════════
        //  List Tab Events
        // ══════════════════════════════════════════════════════════════════

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string q = txtSearch.Text.Trim().ToLower();
            var filtered = string.IsNullOrWhiteSpace(q)
                ? _bankAccounts
                : _bankAccounts?.Where(b =>
                    Contains(b.AccountType, q) ||
                    Contains(b.BankName, q) ||
                    Contains(b.AccountTitle, q) ||
                    Contains(b.AccountNumber, q) ||
                    Contains(b.Iban, q) ||
                    Contains(b.BranchName, q)).ToList();

            PopulateGrid(filtered);
        }

        private static bool Contains(string source, string query) =>
            !string.IsNullOrEmpty(source) && source.ToLower().Contains(query);

        private void DgvBankAccounts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string code = dgvBankAccounts.Rows[e.RowIndex].Cells["colAccount"].Value?.ToString();
            if (string.IsNullOrEmpty(code)) return;

            _currentAccountCode = code;
            SetNewMode(false);
            FillDetail(code);
            tabControl.SelectedTab = tpDetails;
        }

        private void BtnAddNew_Click(object sender, EventArgs e)
        {
            SetNewMode(true);
            tabControl.SelectedTab = tpDetails;
        }

        private async void BtnRefreshList_Click(object sender, EventArgs e)
        {
            try
            {
                await FillBankAccountsAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  Detail Tab Button Events
        // ══════════════════════════════════════════════════════════════════

        private void BtnNew_Click(object sender, EventArgs e) => SetNewMode(!_isNewMode);

        private void BtnClose_Click(object sender, EventArgs e) => this.Close();

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving) return;

            bool isCash = rdoCash.Checked;
            if (!isCash && string.IsNullOrWhiteSpace(txtBankName.Text))
            {
                MessageBox.Show("Bank Name is required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBankName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAccountTitle.Text))
            {
                MessageBox.Show((isCash ? "Cash Account Title" : "Account Title") + " is required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAccountTitle.Focus();
                return;
            }

            if (MessageBox.Show($"Save {(isCash ? "cash" : "bank")} account details?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _isSaving = true;
            btnSave.Enabled = false;

            try
            {
                decimal openingBalance = txtOpeningBalance.Value;

                if (_isNewMode)
                {
                    var createReq = new BankAccountCreateApiRequest
                    {
                        AccountType = isCash ? "Cash" : "Bank",
                        BankName = isCash ? "Cash" : txtBankName.Text.Trim(),
                        AccountTitle = txtAccountTitle.Text.Trim(),
                        AccountNumber = isCash || string.IsNullOrWhiteSpace(txtAccountNumber.Text) ? null : txtAccountNumber.Text.Trim(),
                        Iban = isCash || string.IsNullOrWhiteSpace(txtIBAN.Text) ? null : txtIBAN.Text.Trim(),
                        BranchName = isCash || string.IsNullOrWhiteSpace(txtBranchName.Text) ? null : txtBranchName.Text.Trim(),
                        BranchCode = isCash || string.IsNullOrWhiteSpace(txtBranchCode.Text) ? null : txtBranchCode.Text.Trim(),
                        SwiftCode = isCash || string.IsNullOrWhiteSpace(txtSwiftCode.Text) ? null : txtSwiftCode.Text.Trim(),
                        Currency = cmbCurrency.SelectedItem?.ToString() ?? "PKR",
                        Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim(),
                        Active = chkActive.Checked,
                        OpeningBalance = openingBalance
                    };

                    string newCode = await _bankAccountApiService.CreateAsync(createReq);
                    MessageBox.Show($"{(isCash ? "Cash" : "Bank")} account '{createReq.AccountTitle}' created successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    await FillBankAccountsAsync();
                    _currentAccountCode = newCode;
                    SetNewMode(false);
                    FillDetail(newCode);
                }
                else
                {
                    var updateReq = new BankAccountUpdateApiRequest
                    {
                        AccountType = isCash ? "Cash" : "Bank",
                        BankName = isCash ? "Cash" : txtBankName.Text.Trim(),
                        AccountTitle = txtAccountTitle.Text.Trim(),
                        AccountNumber = isCash || string.IsNullOrWhiteSpace(txtAccountNumber.Text) ? null : txtAccountNumber.Text.Trim(),
                        Iban = isCash || string.IsNullOrWhiteSpace(txtIBAN.Text) ? null : txtIBAN.Text.Trim(),
                        BranchName = isCash || string.IsNullOrWhiteSpace(txtBranchName.Text) ? null : txtBranchName.Text.Trim(),
                        BranchCode = isCash || string.IsNullOrWhiteSpace(txtBranchCode.Text) ? null : txtBranchCode.Text.Trim(),
                        SwiftCode = isCash || string.IsNullOrWhiteSpace(txtSwiftCode.Text) ? null : txtSwiftCode.Text.Trim(),
                        Currency = cmbCurrency.SelectedItem?.ToString() ?? "PKR",
                        Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim(),
                        Active = chkActive.Checked,
                        OpeningBalance = openingBalance
                    };

                    await _bankAccountApiService.UpdateAsync(_currentAccountCode, updateReq);
                    MessageBox.Show($"{(isCash ? "Cash" : "Bank")} account '{updateReq.AccountTitle}' updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    await FillBankAccountsAsync();
                    FillDetail(_currentAccountCode);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                _isSaving = false;
                btnSave.Enabled = true;
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_isDeleting || string.IsNullOrWhiteSpace(_currentAccountCode)) return;

            if (MessageBox.Show("Are you sure you want to delete this bank account?\n\n(Accounts with ledger transactions cannot be deleted; you can deactivate them instead.)",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _isDeleting = true;
            btnDelete.Enabled = false;

            try
            {
                await _bankAccountApiService.DeleteAsync(_currentAccountCode);
                MessageBox.Show("Bank account deleted successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                await FillBankAccountsAsync();
                SetNewMode(true);
                tabControl.SelectedTab = tpList;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                _isDeleting = false;
                btnDelete.Enabled = true;
            }
        }

        private void BtnStatement_Click(object sender, EventArgs e)
        {
            try
            {
                var frm = new frmReportParameters();
                frm.Reportname = "Account Statement";
                if (this.MdiParent != null) frm.MdiParent = this.MdiParent;
                frm.Show();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void BtnReconcile_Click(object sender, EventArgs e)
        {
            try
            {
                var frm = new frmBankReconcilation();
                if (this.MdiParent != null) frm.MdiParent = this.MdiParent;
                frm.Show();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void ShowError(Exception ex)
        {
            string msg = ex.Message;
            if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                msg += "\n" + ex.InnerException.Message;

            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
