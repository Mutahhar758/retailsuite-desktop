using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmReceipt : Form
    {
        enum GL
        {
            VDate, Vtime, VoucherNo, Vtype, Vseq, DRAccount, Amount,
            CRAccount, Narration, Remarks, CheckNum, CheckDate, CheckStatus,
            Clear, status, CreatedBy, CreatedTime, EditBy, EditTime
        };

        private readonly ReceiptApiService _apiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly NarrationApiService _narrationApiService;
        private int currentRow;
        private bool resetRow = false;
        bool FLogIn = true;
        private string VoucherNum = null;
        private List<ReceiptDto> _queryList = new List<ReceiptDto>();
        enum Navigators { Up, Down, Home, End };
        int VoucherIndex = 0;
        private bool _isSaving;

        DataTable dtAccount = new DataTable();
        DataTable dtNarration = new DataTable();
        DataTable dtCashBanks = new DataTable();

        public frmReceipt()
        {
            InitializeComponent();
            _apiService = new ReceiptApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _narrationApiService = new NarrationApiService();
            dgvExpenses.Rows.Add();
            UserInfo.ApplyFormPermissions(this, AppResource.ReceiptVouchers);
        }

        async System.Threading.Tasks.Task ShowBalAsync(string title, string account)
        {
            var bal = await _apiService.GetAccountBalanceAsync(account);
            lblBalance.Text = title + " = " + (bal >= 0 ? bal.ToString("N2") + " Dr" : (-bal).ToString("N2") + " Cr");
        }

        #region Fill Controls

        async System.Threading.Tasks.Task LoadLookupsAsync()
        {
            var accountsTask = _chartOfAccountApiService.GetDetailAccountsAsync();
            var cashBanksTask = _chartOfAccountApiService.GetCashBankAccountsAsync();
            var narrationsTask = _narrationApiService.GetActiveNarrationsAsync();
            await System.Threading.Tasks.Task.WhenAll(accountsTask, cashBanksTask, narrationsTask);

            dtAccount = new DataTable();
            dtAccount.Columns.Add("Account", typeof(string));
            dtAccount.Columns.Add("Title", typeof(string));
            foreach (var a in accountsTask.Result)
                dtAccount.Rows.Add(a.Account, a.Title);

            dtCashBanks = new DataTable();
            dtCashBanks.Columns.Add("Account", typeof(string));
            dtCashBanks.Columns.Add("Title", typeof(string));
            foreach (var cb in cashBanksTask.Result)
                dtCashBanks.Rows.Add(cb.Account, cb.Title);

            dtNarration = new DataTable();
            dtNarration.Columns.Add("Code", typeof(string));
            dtNarration.Columns.Add("Title", typeof(string));
            foreach (var n in narrationsTask.Result)
                dtNarration.Rows.Add(n.Code, n.Title);

            FillAccount();
            FillCashBanks();
            FillNarration();
            FillFilterAccount();
            FillFilterCashBanks();
            FillFilterNarration();
        }

        async System.Threading.Tasks.Task FillQueryAsync(
            string fromDate = "", string toDate = "",
            string cashBank = "", string account = "", string narration = "")
        {
            dgvQuery.Rows.Clear();
            _queryList = await _apiService.GetListAsync(fromDate, toDate, cashBank, account, narration);
            for (int i = 0; i < _queryList.Count; i++)
            {
                var r = _queryList[i];
                dgvQuery.Rows.Add(
                    r.Date,
                    "RV-" + r.VoucherNo,
                    r.Amount.ToString(),
                    r.Narration,
                    r.CreatedBy + " | " + r.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                    !string.IsNullOrWhiteSpace(r.LastModifiedBy)
                        ? r.LastModifiedBy + " | " + r.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                        : null);
            }
        }

        internal async System.Threading.Tasks.Task FillReceiptAsync(string vno)
        {
            string cashBankFilter = cmbFilterAccounts.SelectedValue != null
                ? cmbFilterAccounts.SelectedValue.ToString() : "";
            var lines = await _apiService.GetDetailAsync(vno, cashBankFilter);
            txtVoucherNo.Text = "RV-" + vno;
            VoucherNum = vno;
            dgvExpenses.Rows.Clear();
            if (lines.Count > 0)
            {
                var first = lines[0];
                dtpDate.Value = first.Date;
                cmbCashBanks.SelectedValue = first.CashBankAccountId;
                if (string.IsNullOrWhiteSpace(first.Narration))
                    cmbNarration.SelectedIndex = -1;
                else
                    cmbNarration.SelectedValue = first.Narration;
                txtCreatedBy.Text = first.CreatedBy + " | " + first.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt");
                txtEditBy.Text = !string.IsNullOrWhiteSpace(first.LastModifiedBy)
                    ? first.LastModifiedBy + " | " + first.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null;
                for (int i = 0; i < lines.Count; i++)
                {
                    var l = lines[i];
                    dgvExpenses.Rows.Add(
                        l.Seq.ToString(), l.AccountId,
                        Validation.NullIfEmpty(l.CheckNum),
                        l.CheckDate.HasValue ? (object)l.CheckDate.Value : null,
                        l.Amount.ToString("N0"), l.Remarks,
                        l.CreatedBy + " | " + l.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                        !string.IsNullOrWhiteSpace(l.LastModifiedBy)
                            ? l.LastModifiedBy + " | " + l.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null);
                }
                dgvExpenses.Rows.Add();
                CalcTotAmount();
            }
        }

        void FillAccount() { clnAccount.DataSource = dtAccount.Copy(); clnAccount.DisplayMember = "Title"; clnAccount.ValueMember = "Account"; }
        void FillCashBanks() { cmbCashBanks.DataSource = dtCashBanks.Copy(); cmbCashBanks.DisplayMember = "Title"; cmbCashBanks.ValueMember = "Account"; }
        void FillNarration() { cmbNarration.DataSource = dtNarration.Copy(); cmbNarration.DisplayMember = "Title"; cmbNarration.ValueMember = "Code"; }
        void FillFilterAccount() { cmbFilterAccounts.DataSource = dtAccount.Copy(); cmbFilterAccounts.DisplayMember = "Title"; cmbFilterAccounts.ValueMember = "Account"; cmbFilterAccounts.SelectedIndex = -1; }
        void FillFilterCashBanks() { cmbFilterCashBanks.DataSource = dtCashBanks.Copy(); cmbFilterCashBanks.DisplayMember = "Title"; cmbFilterCashBanks.ValueMember = "Account"; cmbFilterCashBanks.SelectedIndex = -1; }
        void FillFilterNarration() { cmbFilterNarration.DataSource = dtNarration.Copy(); cmbFilterNarration.DisplayMember = "Title"; cmbFilterNarration.ValueMember = "Code"; cmbFilterNarration.SelectedIndex = -1; }

        #endregion

        void checkValidation(int rowindex, int colIndex)
        {
            if (colIndex == clnCheque.Index)
            {
                if (dgvExpenses[colIndex, rowindex].Value != null && dgvExpenses[colIndex, rowindex].Value.ToString().Length > 0)
                {
                    dgvExpenses[clnChequeDate.Index, rowindex].ReadOnly = false;
                    dgvExpenses[clnChequeDate.Index, rowindex].Style.BackColor = Color.Empty;
                    dgvExpenses[clnChequeDate.Index, rowindex].Style.SelectionBackColor = Color.Empty;
                    if (dgvExpenses[clnChequeDate.Index, rowindex].Value == null)
                        dgvExpenses[clnChequeDate.Index, rowindex].Value = DateTime.Now;
                }
                else
                {
                    dgvExpenses[clnChequeDate.Index, rowindex].ReadOnly = true;
                    dgvExpenses[clnChequeDate.Index, rowindex].Style.BackColor = Color.LightGray;
                    dgvExpenses[clnChequeDate.Index, rowindex].Style.SelectionBackColor = Color.Gray;
                    dgvExpenses[clnChequeDate.Index, rowindex].Value = null;
                }
            }
        }

        void AllowNewRow()
        {
            if (dgvExpenses.CurrentRow.Index == dgvExpenses.Rows[dgvExpenses.Rows.Count - 1].Index)
                if (dgvExpenses.CurrentRow.Cells[clnAccount.Index].Value != null && dgvExpenses.CurrentRow.Cells[clnAmount.Index].Value != null)
                    dgvExpenses.Rows.Add();
        }

        private async void frmReceipt_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadLookupsAsync();
                await FillQueryAsync();
                if (_queryList.Count > 0 && txtVoucherNo.Text == "")
                    await FillReceiptAsync(_queryList[VoucherIndex].VoucherNo);
                FLogIn = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading receipt data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; SendKeys.Send("{tab}"); } }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        { AllowNewRow(); resetRow = true; currentRow = e.RowIndex; }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        { if (resetRow) { resetRow = false; dgvExpenses.CurrentCell = dgvExpenses.Rows[currentRow].Cells[dgvExpenses.CurrentCell.ColumnIndex]; } }

        void CalcTotAmount()
        {
            decimal Tot = 0;
            try { Tot = (from DataGridViewRow row in dgvExpenses.Rows where row.Cells[clnAmount.Index].Value != null select decimal.Parse(row.Cells[clnAmount.Index].Value.ToString())).Sum(); } catch { }
            lblTotAmount.Text = Tot.ToString();
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvExpenses.CurrentCellAddress.X == clnAccount.DisplayIndex)
            {
                ComboBox cb = e.Control as ComboBox;
                if (cb != null) { cb.DropDownStyle = ComboBoxStyle.DropDown; cb.AutoCompleteSource = AutoCompleteSource.ListItems; cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend; cb.SelectedIndexChanged += new EventHandler(cb_SelectedIndexChanged); }
            }
            if (dgvExpenses.CurrentCell.ColumnIndex == clnAmount.Index)
            { TextBox tb = e.Control as TextBox; if (tb != null && e.Control.Text != null) tb.KeyPress += new KeyPressEventHandler(tb_KeyPress); }
        }

        void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvExpenses.CurrentCell.ColumnIndex == clnAccount.Index)
                if ((sender as ComboBox).SelectedIndex != -1)
                {
                    DataRowView dr = (DataRowView)(sender as ComboBox).SelectedItem;
                    _ = ShowBalAsync(dr[1].ToString(), dr[0].ToString());
                    if ((sender as ComboBox).SelectedValue == null) (sender as ComboBox).SelectedValue = dr[0].ToString();
                }
        }

        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvExpenses.CurrentCell.ColumnIndex == clnAmount.Index)
            {
                if ((sender as TextBox).SelectedText.Length > 0) { int selind = (sender as TextBox).SelectionStart; (sender as TextBox).Text = (sender as TextBox).Text.Replace((sender as TextBox).SelectedText, ""); (sender as TextBox).SelectionStart = selind; (sender as TextBox).SelectionLength = 0; }
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !((sender as TextBox).Text.Count(a => a == '.') == 0 && e.KeyChar == '.')) e.Handled = true;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        { if (!FLogIn) checkValidation(e.RowIndex, e.ColumnIndex); CalcTotAmount(); }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            checkValidation(e.RowIndex, clnCheque.Index);
            int seq = 0;
            try { seq = (int)(from DataGridViewRow row in dgvExpenses.Rows where row.Cells[clnSeq.Index].Value != null && row.Cells[clnSeq.Index].Value.ToString() != "" select int.Parse(row.Cells[clnSeq.Index].Value.ToString())).Max(); } catch { }
            if (dgvExpenses.Rows[e.RowIndex].Cells[clnSeq.Index].Value == null) dgvExpenses.Rows[e.RowIndex].Cells[clnSeq.Index].Value = (seq + 1).ToString();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        { e.ThrowException = false; }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving)
                return;

            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to save this...!", "Confirmation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string voucher = txtVoucherNo.Text == "" ? null : txtVoucherNo.Text.Split('-')[1];

                if (cmbCashBanks.SelectedValue != null)
                {
                    var lines = new System.Collections.Generic.List<ReceiptLineRequest>();
                    foreach (DataGridViewRow row in dgvExpenses.Rows)
                    {
                        if (row.Cells[clnAccount.Index].Value == null || row.Cells[clnAmount.Index].Value == null)
                            continue;
                        var checkDateVal = row.Cells[clnChequeDate.Index].Value;
                        lines.Add(new ReceiptLineRequest
                        {
                            Seq       = int.Parse(row.Cells[clnSeq.Index].Value.ToString()),
                            Account   = row.Cells[clnAccount.Index].Value.ToString(),
                            Amount    = decimal.Parse(row.Cells[clnAmount.Index].Value.ToString()),
                            CheckNum  = (string)row.Cells[clnCheque.Index].Value,
                            CheckDate = checkDateVal != null ? ((DateTime)checkDateVal).ToString("yyyy-MM-dd") : null,
                            Remarks   = (string)row.Cells[clnRemarks.Index].Value
                        });
                    }

                    _isSaving = true;
                    btnSave.Enabled = false;
                    try
                    {
                        bool isNew = string.IsNullOrWhiteSpace(voucher);
                        if (isNew)
                        {
                            var createRequest = new ReceiptCreateRequest
                            {
                                Date            = dtpDate.Value.ToString("yyyy-MM-dd"),
                                CashBankAccount = cmbCashBanks.SelectedValue.ToString(),
                                Narration       = cmbNarration.SelectedValue?.ToString(),
                                Lines           = lines
                            };
                            voucher = await _apiService.CreateAsync(createRequest);
                        }
                        else
                        {
                            var updateRequest = new ReceiptUpdateRequest
                            {
                                Date            = dtpDate.Value.ToString("yyyy-MM-dd"),
                                CashBankAccount = cmbCashBanks.SelectedValue.ToString(),
                                Narration       = cmbNarration.SelectedValue?.ToString(),
                                Lines           = lines
                            };
                            await _apiService.UpdateAsync(voucher, updateRequest);
                        }

                        await FillReceiptAsync(voucher);

                        if (isNew)
                        {
                            var refreshed = await _apiService.GetListAsync();
                            var entry = refreshed.Find(r => r.VoucherNo == voucher);
                            if (entry != null)
                            {
                                _queryList.Insert(0, entry);
                                dgvQuery.Rows.Insert(0,
                                    entry.Date,
                                    "RV-" + entry.VoucherNo,
                                    entry.Amount.ToString(),
                                    entry.Narration,
                                    entry.CreatedBy + " | " + entry.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                                    null);
                            }
                        }

                        MessageBox.Show("Record Successfully Saved..!");
                        btnNew.Focus();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving receipt: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        _isSaving = false;
                        btnSave.Enabled = true;
                    }
                }
            }
        }

        private async void dgvQuery_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        { if (!FLogIn && e.RowIndex != -1) { string voucherNo = dgvQuery.Rows[e.RowIndex].Cells[clnVoucherNum.Index].Value.ToString(); tabDetailQuery.SelectedTab = tabpgDetail; await FillReceiptAsync(voucherNo.Substring(3)); } }

        private async void btnFind_Click(object sender, EventArgs e)
        {
            try { await FillQueryAsync(txtFdate.Text, txtTdate.Text, cmbFilterCashBanks.SelectedValue != null ? cmbFilterCashBanks.SelectedValue.ToString() : "", cmbFilterAccounts.SelectedValue != null ? cmbFilterAccounts.SelectedValue.ToString() : "", cmbFilterNarration.SelectedValue != null ? cmbFilterNarration.SelectedValue.ToString() : ""); }
            catch (Exception ex) { MessageBox.Show("Error loading receipts: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void txtFdate_Validated(object sender, EventArgs e)
        {
            try { string text = (sender as TextBox).Text.Replace(" ", "-").Replace("/", "-"); DateTime date = DateTime.ParseExact(text, Validation.dateformats, CultureInfo.InvariantCulture, DateTimeStyles.None); (sender as TextBox).Text = date.ToString("dd-MMM-yyyy"); }
            catch (Exception) { (sender as TextBox).Text = ""; }
        }

        private async void dgvQuery_KeyDown(object sender, KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter && dgvQuery.CurrentRow != null) { string voucherNo = dgvQuery.CurrentRow.Cells[clnVoucherNum.Index].Value.ToString(); tabDetailQuery.SelectedTab = tabpgDetail; await FillReceiptAsync(voucherNo.Substring(3)); e.Handled = true; } }

        void Navigate(Navigators Nav)
        {
            if (dgvQuery.CurrentRow != null && tabDetailQuery.SelectedTab == tabpgDetail)
            {
                if (Nav == Navigators.Up && VoucherIndex > 0) { VoucherIndex--; _ = FillReceiptAsync(_queryList[VoucherIndex].VoucherNo); }
                else if (Nav == Navigators.Down && _queryList.Count - 1 > VoucherIndex) { VoucherIndex++; _ = FillReceiptAsync(_queryList[VoucherIndex].VoucherNo); }
                else if (Nav == Navigators.End) { VoucherIndex = 0; _ = FillReceiptAsync(_queryList[VoucherIndex].VoucherNo); }
                else if (Nav == Navigators.Home) { VoucherIndex = _queryList.Count - 1; _ = FillReceiptAsync(_queryList[VoucherIndex].VoucherNo); }
            }
        }

        private void tabDetailQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown) Navigate(Navigators.Down);
            else if (e.KeyCode == Keys.PageUp) Navigate(Navigators.Up);
            else if (e.KeyCode == Keys.Home) Navigate(Navigators.Home);
            else if (e.KeyCode == Keys.End) Navigate(Navigators.End);
        }

        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }

        private void btnNew_Click(object sender, EventArgs e)
        { dgvExpenses.Rows.Clear(); Validation.Clear(grpInvoiceDetail); dgvExpenses.Rows.Add(); dtpDate.Focus(); }

        #region Navigation
        private void btnHome_Click(object sender, EventArgs e) => Navigate(Navigators.Home);
        private void btnPri_Click(object sender, EventArgs e)  => Navigate(Navigators.Down);
        private void btnNext_Click(object sender, EventArgs e) => Navigate(Navigators.Up);
        private void btnEnd_Click(object sender, EventArgs e)  => Navigate(Navigators.End);
        #endregion

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtVoucherNo.Text == "") return;
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Delete this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string vno = txtVoucherNo.Text.Substring(3);
                    await _apiService.DeleteAsync(vno);
                    DataGridViewRow row = dgvQuery.Rows.Cast<DataGridViewRow>().First(r => r.Cells[clnVoucherNum.Index].Value.ToString().Equals(txtVoucherNo.Text));
                    _queryList.RemoveAll(r => r.VoucherNo == vno);
                    btnNew_Click(null, null);
                    dgvQuery.Rows.Remove(row);
                    CalcTotAmount();
                    MessageBox.Show("Record Successfully Deleted..!");
                }
                catch (Exception ex) { MessageBox.Show("Error deleting receipt: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private async void deleteRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Delete this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (dgvExpenses[clnSeq.Index, dgvExpenses.CurrentRow.Index].Value != null)
                {
                    try
                    {
                        if (txtVoucherNo.Text != "") { int seq = int.Parse(dgvExpenses[clnSeq.Index, dgvExpenses.CurrentRow.Index].Value.ToString()); await _apiService.DeleteLineAsync(txtVoucherNo.Text.Substring(3), seq); }
                        dgvExpenses.Rows.RemoveAt(dgvExpenses.CurrentRow.Index);
                        CalcTotAmount();
                        MessageBox.Show("Record Successfully Deleted..!");
                    }
                    catch (Exception ex) { MessageBox.Show("Error deleting line: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void dgvExpenses_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            { dgvExpenses.Rows[e.RowIndex].Selected = true; dgvExpenses.CurrentCell = dgvExpenses.Rows[e.RowIndex].Cells[e.ColumnIndex]; contextMenuStrip1.Show(Cursor.Position); }
        }

        private void frmReceipt_KeyDown(object sender, KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter && !dgvExpenses.Focused) SendKeys.Send("{tab}"); }

        private void dgvExpenses_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dgvExpenses[clnAccount.Index, e.RowIndex].Value != null)
                _ = ShowBalAsync((dgvExpenses.Rows[e.RowIndex].Cells[clnAccount.Index] as DataGridViewComboBoxCell).FormattedValue.ToString(), (dgvExpenses.Rows[e.RowIndex].Cells[clnAccount.Index] as DataGridViewComboBoxCell).Value.ToString());
        }

        private async void addNewAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChartOfAccLvl5 frm = new frmChartOfAccLvl5();
            frm.AccountHead = AccountHead.Customer;
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                await LoadLookupsAsync();
            }
        }
    }
}