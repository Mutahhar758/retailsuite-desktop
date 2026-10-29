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
    public partial class frmPayments : Form
    {
        enum GL
        {
            VDate,
            Vtime,
            VoucherNo,
            Vtype,
            Vseq,
            DRAccount,
            Amount,
            CRAccount,
            Narration,
            Remarks,
            CheckNum,
            CheckDate,
            CheckStatus,
            Clear,
            status,
            CreatedBy,
            CreatedTime,
            EditBy,
            EditTime
        };

        private readonly PaymentApiService _apiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly NarrationApiService _narrationApiService;
        private int currentRow;
        private bool resetRow = false;
        bool FLogIn = true;
        private string VoucherNum = null;
        private List<PaymentDto> _queryList = new List<PaymentDto>();
        enum Navigators { Up, Down, Home, End };
        int VoucherIndex = 0;
        private bool _isSaving;

        DataTable dtAccount = new DataTable();
        DataTable dtNarration = new DataTable();
        DataTable dtCashBanks = new DataTable();

        public frmPayments()
        {
            InitializeComponent();
            _apiService = new PaymentApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _narrationApiService = new NarrationApiService();
            UserInfo.ApplyFormPermissions(this, AppResource.PaymentVouchers);
            dgvExpenses.Rows.Add();
            EnableDoubleBuffering(dgvExpenses);
            EnableDoubleBuffering(dgvQuery);
            dgvExpenses.Enter += dgvExpenses_Enter;
            this.Shown += frmPayments_Shown;
            dtpDate.KeyDown += dtpDate_KeyDown;
            cmbCashBanks.KeyDown += cmbCashBanks_KeyDown;
        }

        private void frmPayments_Shown(object sender, EventArgs e)
        {
            try
            {
                dtpDate.Focus();
            }
            catch { }
        }

        private void dtpDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                cmbCashBanks.Focus();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == Keys.Enter || keyData == Keys.Tab) && dgvExpenses.ContainsFocus)
            {
                if (HandleGridEnterNavigation())
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static void EnableDoubleBuffering(Control control)
        {
            try
            {
                typeof(Control).InvokeMember(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                    null,
                    control,
                    new object[] { true });
            }
            catch { }
        }

        private void dgvExpenses_Enter(object sender, EventArgs e)
        {
            if (IsHandleCreated && dgvExpenses.Rows.Count > 0 && clnAccount != null)
            {
                BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (dgvExpenses.Rows.Count > 0 && clnAccount != null)
                        {
                            int row = dgvExpenses.CurrentCell != null ? dgvExpenses.CurrentCell.RowIndex : 0;
                            if (row >= 0 && row < dgvExpenses.Rows.Count)
                            {
                                dgvExpenses.CurrentCell = dgvExpenses[clnAccount.Index, row];
                            }
                        }
                    }
                    catch { }
                }));
            }
        }

        private void cmbCashBanks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                dgvExpenses.Focus();
                if (dgvExpenses.Rows.Count > 0 && clnAccount != null)
                {
                    int targetRow = dgvExpenses.CurrentCell != null ? dgvExpenses.CurrentCell.RowIndex : 0;
                    if (targetRow < 0 || targetRow >= dgvExpenses.Rows.Count) targetRow = 0;
                    dgvExpenses.CurrentCell = dgvExpenses[clnAccount.Index, targetRow];
                    dgvExpenses.BeginEdit(true);
                }
            }
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

        private ERP.Controls.PaginationControl _pager;
        private string _qFrom = "", _qTo = "", _qCashBank = "", _qAccount = "", _qNarration = "";
        private int _qPageSize = 20;

        private void EnsurePager()
        {
            if (_pager != null) return;
            _pager = new ERP.Controls.PaginationControl();
            _pager.PageChanged += async (s, e) =>
            {
                try
                {
                    await FillQueryAsync(_qFrom, _qTo, _qCashBank, _qAccount, _qNarration, e.PageNumber, e.PageSize);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading payments: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            _pager.Attach(tabPage4, dgvQuery);
        }

        async System.Threading.Tasks.Task FillQueryAsync(
            string fromDate = "", string toDate = "",
            string cashBank = "", string account = "", string narration = "",
            int pageNumber = 1, int pageSize = 0)
        {
            EnsurePager();
            _qFrom = fromDate; _qTo = toDate; _qCashBank = cashBank; _qAccount = account; _qNarration = narration;
            if (pageSize > 0) _qPageSize = pageSize;

            dgvQuery.Rows.Clear();
            var page = await _apiService.GetPagedListAsync(fromDate, toDate, cashBank, account, narration, "", pageNumber, _qPageSize);
            _queryList = page.Data;
            _pager.SetPagination(page.CurrentPage, page.TotalPages, page.TotalCount, page.PageSize);
            _qPageSize = page.PageSize;
            VoucherIndex = 0;
            for (int i = 0; i < _queryList.Count; i++)
            {
                var r = _queryList[i];
                dgvQuery.Rows.Add(
                    r.Date,
                    "PV-" + r.VoucherNo,
                    r.Amount.ToString(),
                    r.Narration,
                    r.CreatedBy + " | " + r.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                    !string.IsNullOrWhiteSpace(r.LastModifiedBy)
                        ? r.LastModifiedBy + " | " + r.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                        : null);
            }
        }

        internal async System.Threading.Tasks.Task FillPaymentAsync(string vno)
        {
            string cashBankFilter = cmbFilterAccounts.SelectedValue != null
                ? cmbFilterAccounts.SelectedValue.ToString() : "";
            var lines = await _apiService.GetDetailAsync(vno, cashBankFilter);
            txtVoucherNo.Text = "PV-" + vno;
            VoucherNum = vno;
            dgvExpenses.Rows.Clear();
            if (lines.Count > 0)
            {
                var first = lines[0];
                dtpDate.Value = first.Date;
                if (first.ClearingDate.HasValue)
                {
                    dtpClearingDate.Value = first.ClearingDate.Value;
                    dtpClearingDate.Checked = true;
                }
                else
                {
                    dtpClearingDate.Checked = false;
                }
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
                        l.Seq.ToString(),
                        l.AccountId,
                        l.Amount.ToString("N0"),
                        l.Remarks,
                        Validation.NullIfEmpty(l.CheckNum),
                        l.CheckDate.HasValue ? (object)l.CheckDate.Value : null,
                        l.CreatedBy + " | " + l.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                        !string.IsNullOrWhiteSpace(l.LastModifiedBy)
                            ? l.LastModifiedBy + " | " + l.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null);
                }
                dgvExpenses.Rows.Add();
                CalcTotAmount();
            }
        }

        internal void FillPayment(string vno)
        {
            _ = FillPaymentAsync(vno);
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
            if (dgvExpenses.CurrentRow != null && dgvExpenses.CurrentRow.Index == dgvExpenses.Rows[dgvExpenses.Rows.Count - 1].Index)
            {
                if (dgvExpenses.CurrentRow.Cells[clnAccount.Index].Value != null && dgvExpenses.CurrentRow.Cells[clnAmount.Index].Value != null)
                {
                    int newIdx = dgvExpenses.Rows.Add();
                    ClearRowValues(newIdx);
                }
            }
        }

        private void ClearRowValues(int rowIdx)
        {
            if (rowIdx < 0 || rowIdx >= dgvExpenses.Rows.Count) return;
            var row = dgvExpenses.Rows[rowIdx];
            row.Cells[clnAccount.Index].Value = null;
            row.Cells[clnCheque.Index].Value = null;
            row.Cells[clnChequeDate.Index].Value = null;
            row.Cells[clnAmount.Index].Value = null;
            row.Cells[clnRemarks.Index].Value = null;
            checkValidation(rowIdx, clnCheque.Index);
            int seq = 0;
            try { seq = (int)(from DataGridViewRow r in dgvExpenses.Rows where r.Cells[clnSeq.Index].Value != null && r.Cells[clnSeq.Index].Value.ToString() != "" select int.Parse(r.Cells[clnSeq.Index].Value.ToString())).Max(); } catch { }
            row.Cells[clnSeq.Index].Value = (seq + 1).ToString();
        }

        private async void frmReceipt_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadLookupsAsync();
                await FillQueryAsync();
                if (_queryList.Count > 0 && txtVoucherNo.Text == "")
                    await FillPaymentAsync(_queryList[VoucherIndex].VoucherNo);
                FLogIn = false;
                BeginInvoke(new Action(() =>
                {
                    try { dtpDate.Focus(); } catch { }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payment data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                HandleGridEnterNavigation();
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AllowNewRow();
            currentRow = e.RowIndex;
            CalcTotAmount();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (resetRow)
            {
                resetRow = false;
                dgvExpenses.CurrentCell = dgvExpenses.Rows[currentRow].Cells[dgvExpenses.CurrentCell.ColumnIndex];
            }
        }

        void CalcTotAmount()
        {
            decimal Tot = 0;
            try
            {
                Tot = (from DataGridViewRow row in dgvExpenses.Rows
                       where row.Cells[clnAmount.Index].Value != null && !string.IsNullOrWhiteSpace(row.Cells[clnAmount.Index].Value.ToString())
                       select decimal.Parse(row.Cells[clnAmount.Index].Value.ToString())).Sum();
            }
            catch { }
            lblTotAmount.Text = Tot.ToString("N2");
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvExpenses.CurrentCell != null && (dgvExpenses.CurrentCellAddress.X == clnAccount.DisplayIndex || dgvExpenses.CurrentCell.ColumnIndex == clnAccount.Index))
            {
                ComboBox cb = e.Control as ComboBox;
                if (cb != null)
                {
                    cb.DropDownStyle = ComboBoxStyle.DropDown;
                    cb.AutoCompleteSource = AutoCompleteSource.ListItems;
                    cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cb.SelectedIndexChanged -= new EventHandler(cb_SelectedIndexChanged);
                    cb.SelectionChangeCommitted -= new EventHandler(cb_SelectionChangeCommitted);

                    var cellVal = dgvExpenses.CurrentCell.Value;
                    if (cellVal == null || string.IsNullOrWhiteSpace(cellVal.ToString()))
                    {
                        try { cb.SelectedIndex = -1; cb.Text = ""; } catch { }
                    }
                    else
                    {
                        try { cb.SelectedValue = cellVal.ToString(); } catch { }
                    }

                    cb.SelectedIndexChanged += new EventHandler(cb_SelectedIndexChanged);
                    cb.SelectionChangeCommitted += new EventHandler(cb_SelectionChangeCommitted);
                }
            }
            else if (dgvExpenses.CurrentCell != null && dgvExpenses.CurrentCell.ColumnIndex == clnAmount.Index)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null && e.Control.Text != null)
                {
                    tb.KeyPress -= new KeyPressEventHandler(tb_KeyPress);
                    tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);
                }
            }
        }

        void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvExpenses.CurrentCell != null && dgvExpenses.CurrentCell.ColumnIndex == clnAccount.Index)
            {
                if (sender is ComboBox cb && cb.SelectedIndex != -1)
                {
                    DataRowView dr = cb.SelectedItem as DataRowView;
                    if (dr != null)
                    {
                        if (cb.SelectedValue == null) cb.SelectedValue = dr["Account"].ToString();
                        int rowInd = dgvExpenses.CurrentCellAddress.Y;
                        if (rowInd >= 0 && rowInd < dgvExpenses.Rows.Count)
                        {
                            dgvExpenses[clnAccount.Index, rowInd].Value = dr["Account"].ToString();
                        }
                        _ = ShowBalAsync(dr["Title"].ToString(), dr["Account"].ToString());
                    }
                }
            }
        }

        void cb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (dgvExpenses.CurrentCell != null && dgvExpenses.CurrentCell.ColumnIndex == clnAccount.Index)
            {
                if (sender is ComboBox cb && cb.SelectedItem is DataRowView drv)
                {
                    int rowInd = dgvExpenses.CurrentCellAddress.Y;
                    if (rowInd >= 0 && rowInd < dgvExpenses.Rows.Count)
                    {
                        string acct = drv["Account"].ToString();
                        dgvExpenses[clnAccount.Index, rowInd].Value = acct;
                        _ = ShowBalAsync(drv["Title"].ToString(), acct);
                    }
                }
            }
        }

        private bool HandleGridEnterNavigation()
        {
            if (dgvExpenses.CurrentCell == null) return false;

            int rowIndex = dgvExpenses.CurrentCell.RowIndex;
            int colIndex = dgvExpenses.CurrentCell.ColumnIndex;

            // 1. If on Account column (clnAccount)
            if (colIndex == clnAccount.Index)
            {
                if (dgvExpenses.EditingControl is ComboBox cb)
                {
                    if (cb.SelectedItem is DataRowView drv)
                    {
                        string acct = drv["Account"].ToString();
                        dgvExpenses[clnAccount.Index, rowIndex].Value = acct;
                        _ = ShowBalAsync(drv["Title"].ToString(), acct);
                    }
                    else if (!string.IsNullOrWhiteSpace(cb.Text))
                    {
                        string text = cb.Text.Trim();
                        var match = dtAccount.AsEnumerable().FirstOrDefault(r =>
                            string.Equals(r["Title"]?.ToString(), text, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(r["Account"]?.ToString(), text, StringComparison.OrdinalIgnoreCase));
                        if (match == null)
                        {
                            match = dtAccount.AsEnumerable().FirstOrDefault(r =>
                                r["Title"]?.ToString().StartsWith(text, StringComparison.OrdinalIgnoreCase) == true);
                        }
                        if (match != null)
                        {
                            string acct = match["Account"].ToString();
                            dgvExpenses[clnAccount.Index, rowIndex].Value = acct;
                            _ = ShowBalAsync(match["Title"].ToString(), acct);
                        }
                    }
                }

                dgvExpenses.EndEdit();

                var acctVal = dgvExpenses[clnAccount.Index, rowIndex].Value;
                bool isAcctEmpty = acctVal == null || string.IsNullOrWhiteSpace(acctVal.ToString());
                bool isLastRow = (rowIndex == dgvExpenses.Rows.Count - 1);

                // If on last row without selecting account -> Exit grid to Save button!
                if (isLastRow && isAcctEmpty)
                {
                    dgvExpenses.EndEdit();
                    CalcTotAmount();
                    btnSave.Focus();
                    return true;
                }

                // If account selected -> move to Amount directly!
                if (!isAcctEmpty)
                {
                    dgvExpenses.CurrentCell = dgvExpenses[clnAmount.Index, rowIndex];
                    dgvExpenses.BeginEdit(true);
                    return true;
                }

                return true;
            }

            // 2. If on Cheque column
            if (colIndex == clnCheque.Index)
            {
                dgvExpenses.EndEdit();
                checkValidation(rowIndex, clnCheque.Index);
                if (!dgvExpenses[clnChequeDate.Index, rowIndex].ReadOnly)
                {
                    dgvExpenses.CurrentCell = dgvExpenses[clnChequeDate.Index, rowIndex];
                }
                else
                {
                    dgvExpenses.CurrentCell = dgvExpenses[clnAmount.Index, rowIndex];
                }
                dgvExpenses.BeginEdit(true);
                return true;
            }

            // 3. If on Cheque Date column
            if (colIndex == clnChequeDate.Index)
            {
                dgvExpenses.EndEdit();
                dgvExpenses.CurrentCell = dgvExpenses[clnAmount.Index, rowIndex];
                dgvExpenses.BeginEdit(true);
                return true;
            }

            // 4. If on Amount column
            if (colIndex == clnAmount.Index)
            {
                dgvExpenses.EndEdit();
                CalcTotAmount();
                dgvExpenses.CurrentCell = dgvExpenses[clnRemarks.Index, rowIndex];
                dgvExpenses.BeginEdit(true);
                return true;
            }

            // 5. If on Remarks column
            if (colIndex == clnRemarks.Index)
            {
                dgvExpenses.EndEdit();
                CalcTotAmount();

                int nextRowIdx;
                if (rowIndex == dgvExpenses.Rows.Count - 1)
                {
                    nextRowIdx = dgvExpenses.Rows.Add();
                    ClearRowValues(nextRowIdx);
                }
                else
                {
                    nextRowIdx = rowIndex + 1;
                }
                dgvExpenses.CurrentCell = dgvExpenses[clnAccount.Index, nextRowIdx];
                dgvExpenses.BeginEdit(true);
                return true;
            }

            // Fallback
            dgvExpenses.EndEdit();
            CalcTotAmount();
            int targetIdx = rowIndex < dgvExpenses.Rows.Count - 1 ? rowIndex + 1 : dgvExpenses.Rows.Add();
            if (targetIdx == dgvExpenses.Rows.Count - 1) ClearRowValues(targetIdx);
            dgvExpenses.CurrentCell = dgvExpenses[clnAccount.Index, targetIdx];
            dgvExpenses.BeginEdit(true);
            return true;
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
                    var lines = new List<PaymentLineRequest>();
                    foreach (DataGridViewRow row in dgvExpenses.Rows)
                    {
                        if (row.Cells[clnAccount.Index].Value == null || row.Cells[clnAmount.Index].Value == null)
                            continue;
                        var checkDateVal = row.Cells[clnChequeDate.Index].Value;
                        lines.Add(new PaymentLineRequest
                        {
                            Seq = int.Parse(row.Cells[clnSeq.Index].Value.ToString()),
                            Account = row.Cells[clnAccount.Index].Value.ToString(),
                            Amount = decimal.Parse(row.Cells[clnAmount.Index].Value.ToString()),
                            CheckNum = (string)row.Cells[clnCheque.Index].Value,
                            CheckDate = checkDateVal != null ? ((DateTime)checkDateVal).ToString("yyyy-MM-dd") : null,
                            Remarks = (string)row.Cells[clnRemarks.Index].Value
                        });
                    }

                    _isSaving = true;
                    btnSave.Enabled = false;
                    try
                    {
                        bool isNew = string.IsNullOrWhiteSpace(voucher);
                        if (isNew)
                        {
                            var createRequest = new PaymentCreateRequest
                            {
                                Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                                ClearingDate = dtpClearingDate.Checked ? dtpClearingDate.Value.ToString("yyyy-MM-dd") : null,
                                CashBankAccount = cmbCashBanks.SelectedValue.ToString(),
                                Narration = cmbNarration.SelectedValue?.ToString(),
                                Lines = lines
                            };
                            voucher = await _apiService.CreateAsync(createRequest);
                        }
                        else
                        {
                            var updateRequest = new PaymentUpdateRequest
                            {
                                Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                                ClearingDate = dtpClearingDate.Checked ? dtpClearingDate.Value.ToString("yyyy-MM-dd") : null,
                                CashBankAccount = cmbCashBanks.SelectedValue.ToString(),
                                Narration = cmbNarration.SelectedValue?.ToString(),
                                Lines = lines
                            };
                            await _apiService.UpdateAsync(voucher, updateRequest);
                        }

                        await FillPaymentAsync(voucher);

                        if (isNew)
                        {
                            var refreshed = await _apiService.GetListAsync();
                            var entry = refreshed.Find(r => r.VoucherNo == voucher);
                            if (entry != null)
                            {
                                _queryList.Insert(0, entry);
                                dgvQuery.Rows.Insert(0,
                                    entry.Date,
                                    "PV-" + entry.VoucherNo,
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
                        MessageBox.Show("Error saving payment: " + ex.Message, "Error",
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
        { if (!FLogIn && e.RowIndex != -1) { string voucherNo = dgvQuery.Rows[e.RowIndex].Cells[clnVoucherNum.Index].Value.ToString(); tabDetailQuery.SelectedTab = tabpgDetail; await FillPaymentAsync(voucherNo.Substring(3)); } }

        private async void btnFind_Click(object sender, EventArgs e)
        {
            try { await FillQueryAsync(txtFdate.Text, txtTdate.Text, cmbFilterCashBanks.SelectedValue != null ? cmbFilterCashBanks.SelectedValue.ToString() : "", cmbFilterAccounts.SelectedValue != null ? cmbFilterAccounts.SelectedValue.ToString() : "", cmbFilterNarration.SelectedValue != null ? cmbFilterNarration.SelectedValue.ToString() : "", 1, _qPageSize); }
            catch (Exception ex) { MessageBox.Show("Error loading payments: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void txtFdate_Validated(object sender, EventArgs e)
        {
            try { string text = (sender as TextBox).Text.Replace(" ", "-").Replace("/", "-"); DateTime date = DateTime.ParseExact(text, Validation.dateformats, CultureInfo.InvariantCulture, DateTimeStyles.None); (sender as TextBox).Text = date.ToString("dd-MMM-yyyy"); }
            catch (Exception) { (sender as TextBox).Text = ""; }
        }

        private async void dgvQuery_KeyDown(object sender, KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter && dgvQuery.CurrentRow != null) { string voucherNo = dgvQuery.CurrentRow.Cells[clnVoucherNum.Index].Value.ToString(); tabDetailQuery.SelectedTab = tabpgDetail; await FillPaymentAsync(voucherNo.Substring(3)); e.Handled = true; } }

        void Navigate(Navigators Nav)
        {
            if (dgvQuery.CurrentRow != null && tabDetailQuery.SelectedTab == tabpgDetail)
            {
                if (Nav == Navigators.Up && VoucherIndex > 0) { VoucherIndex--; _ = FillPaymentAsync(_queryList[VoucherIndex].VoucherNo); }
                else if (Nav == Navigators.Down && _queryList.Count - 1 > VoucherIndex) { VoucherIndex++; _ = FillPaymentAsync(_queryList[VoucherIndex].VoucherNo); }
                else if (Nav == Navigators.End) { VoucherIndex = 0; _ = FillPaymentAsync(_queryList[VoucherIndex].VoucherNo); }
                else if (Nav == Navigators.Home) { VoucherIndex = _queryList.Count - 1; _ = FillPaymentAsync(_queryList[VoucherIndex].VoucherNo); }
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
        {
            dgvExpenses.Rows.Clear();
            Validation.Clear(grpInvoiceDetail);
            dtpClearingDate.Checked = false;
            lblTotAmount.Text = "0";
            lblBalance.Text = "0.00";
            txtVoucherNo.Text = "";
            VoucherNum = null;
            dgvExpenses.Rows.Add();
            if (dgvExpenses.Rows.Count > 0 && clnAccount != null)
            {
                try
                {
                    dgvExpenses.CurrentCell = dgvExpenses[clnAccount.Index, 0];
                }
                catch { }
            }
            cmbCashBanks.SelectedIndex = -1;
            cmbNarration.SelectedIndex = -1;
            dtpDate.Focus();
        }

        #region Navigation
        private void btnHome_Click(object sender, EventArgs e) => Navigate(Navigators.Home);
        private void btnPri_Click(object sender, EventArgs e) => Navigate(Navigators.Down);
        private void btnNext_Click(object sender, EventArgs e) => Navigate(Navigators.Up);
        private void btnEnd_Click(object sender, EventArgs e) => Navigate(Navigators.End);
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
                catch (Exception ex) { MessageBox.Show("Error deleting payment: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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

        private void frmPayments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                btnNew.PerformClick();
            }
            else if (e.KeyCode == Keys.F5)
            {
                btnSave.PerformClick();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                btnClose.PerformClick();
            }
            else if (e.KeyCode == Keys.Enter && !dgvExpenses.Focused && !cmbCashBanks.Focused && !dtpDate.Focused)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{tab}");
            }
        }

        private void dgvExpenses_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dgvExpenses[clnAccount.Index, e.RowIndex].Value != null)
                _ = ShowBalAsync((dgvExpenses.Rows[e.RowIndex].Cells[clnAccount.Index] as DataGridViewComboBoxCell).FormattedValue.ToString(), (dgvExpenses.Rows[e.RowIndex].Cells[clnAccount.Index] as DataGridViewComboBoxCell).Value.ToString());
        }

        private async void addNewAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChartOfAccLvl5 frm = new frmChartOfAccLvl5();
            frm.AccountHead = AccountHead.Farmer;
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                await LoadLookupsAsync();
            }
        }
    }
}
