using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmJournalVoucher : Form
    {
        private readonly JournalVoucherApiService _apiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly NarrationApiService _narrationApiService;
        private int currentRow;
        private bool resetRow = false;
        bool FLogIn = true;
        private string VoucherNum = null;
        private List<JournalVoucherDto> _queryList = new List<JournalVoucherDto>();
        enum Navigators { Up, Down, Home, End };
        int VoucherIndex = 0;
        private bool _isSaving;

        DataTable dtAccount = new DataTable();
        DataTable dtNarration = new DataTable();

        public frmJournalVoucher()
        {
            InitializeComponent();
            _apiService = new JournalVoucherApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _narrationApiService = new NarrationApiService();
            UserInfo.ApplyFormPermissions(this, AppResource.JournalVouchers);
            dgvJournal.Rows.Add();
            EnableDoubleBuffering(dgvJournal);
            EnableDoubleBuffering(dgvQuery);
            dgvJournal.Enter += dgvJournal_Enter;
            this.Shown += frmJournalVoucher_Shown;
            SetupButtonFocusHighlights();
        }

        private void SetupButtonFocusHighlights()
        {
            var buttons = new Button[] { btnSave, btnNew, btnCopyAsNew, btnDelete, btnClose };
            foreach (var btn in buttons)
            {
                if (btn == null) continue;
                btn.Enter += (s, e) => btn.Invalidate();
                btn.Leave += (s, e) => btn.Invalidate();
                btn.Paint += (s, e) =>
                {
                    if (btn.Focused)
                    {
                        var rect = btn.ClientRectangle;
                        using (var pen = new Pen(Color.FromArgb(59, 130, 246), 2))
                        {
                            e.Graphics.DrawRectangle(pen, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3);
                        }
                    }
                };
            }
        }

        private void frmJournalVoucher_Shown(object sender, EventArgs e)
        {
            try
            {
                dtpDate.Focus();
            }
            catch { }
        }

        private void FocusGridFirstRow()
        {
            try
            {
                if (tabDetailQuery.SelectedTab != tabpgDetail)
                    tabDetailQuery.SelectedTab = tabpgDetail;

                if (dgvJournal.Rows.Count == 0)
                {
                    int r = dgvJournal.Rows.Add();
                    ClearRowValues(r);
                }

                int targetRow = dgvJournal.CurrentCell != null ? dgvJournal.CurrentCell.RowIndex : 0;
                if (targetRow < 0 || targetRow >= dgvJournal.Rows.Count) targetRow = 0;

                dgvJournal.Focus();
                dgvJournal.CurrentCell = dgvJournal[clnDrAccount.Index, targetRow];

                BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (dgvJournal.Rows.Count > 0 && clnDrAccount != null)
                        {
                            int r = dgvJournal.CurrentCell != null ? dgvJournal.CurrentCell.RowIndex : 0;
                            if (r < 0 || r >= dgvJournal.Rows.Count) r = 0;
                            dgvJournal.CurrentCell = dgvJournal[clnDrAccount.Index, r];
                            dgvJournal.BeginEdit(true);
                            if (dgvJournal.EditingControl is ComboBox cb)
                            {
                                cb.Focus();
                            }
                        }
                    }
                    catch { }
                }));
            }
            catch { }
        }

        private void dtpDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                FocusGridFirstRow();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                btnClose.PerformClick();
                return true;
            }

            if ((keyData == Keys.Enter || keyData == Keys.Tab) && !dgvJournal.ContainsFocus)
            {
                if (dtpDate.Focused || dtpDate.ContainsFocus || (ActiveControl != null && (ActiveControl == dtpDate || ActiveControl.Parent == dtpDate)))
                {
                    FocusGridFirstRow();
                    return true;
                }

                if (cmbNarration.Focused || cmbNarration.ContainsFocus || ActiveControl == cmbNarration)
                {
                    FocusGridFirstRow();
                    return true;
                }
            }

            if ((keyData == Keys.Enter || keyData == Keys.Tab) && dgvJournal.ContainsFocus)
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

        private void dgvJournal_Enter(object sender, EventArgs e)
        {
            if (IsHandleCreated && dgvJournal.Rows.Count > 0 && clnDrAccount != null)
            {
                BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (dgvJournal.Rows.Count > 0 && clnDrAccount != null)
                        {
                            int row = dgvJournal.CurrentCell != null ? dgvJournal.CurrentCell.RowIndex : 0;
                            if (row < 0 || row >= dgvJournal.Rows.Count) row = 0;

                            if (dgvJournal.CurrentCell == null || dgvJournal.CurrentCell.ColumnIndex == clnSeq.Index)
                            {
                                dgvJournal.CurrentCell = dgvJournal[clnDrAccount.Index, row];
                            }

                            if (!dgvJournal.IsCurrentCellInEditMode)
                            {
                                dgvJournal.BeginEdit(true);
                            }

                            if (dgvJournal.EditingControl is ComboBox cb)
                            {
                                cb.Focus();
                            }
                        }
                    }
                    catch { }
                }));
            }
        }

        async System.Threading.Tasks.Task ShowDrBalAsync(string title, string account)
        {
            var bal = await _apiService.GetAccountBalanceAsync(account);
            lblDrBalance.Text = title + " = " + (bal >= 0 ? bal.ToString("N2") + " Dr" : (-bal).ToString("N2") + " Cr");
        }

        async System.Threading.Tasks.Task ShowCrBalAsync(string title, string account)
        {
            var bal = await _apiService.GetAccountBalanceAsync(account);
            lblCrBalance.Text = title + " = " + (bal >= 0 ? bal.ToString("N2") + " Dr" : (-bal).ToString("N2") + " Cr");
        }

        #region Fill Controls

        async System.Threading.Tasks.Task LoadLookupsAsync()
        {
            var accountsTask = _chartOfAccountApiService.GetDetailAccountsAsync();
            var narrationsTask = _narrationApiService.GetActiveNarrationsAsync();
            await System.Threading.Tasks.Task.WhenAll(accountsTask, narrationsTask);

            dtAccount = new DataTable();
            dtAccount.Columns.Add("Account", typeof(string));
            dtAccount.Columns.Add("Title", typeof(string));
            foreach (var a in accountsTask.Result)
                dtAccount.Rows.Add(a.Account, a.Title);

            dtNarration = new DataTable();
            dtNarration.Columns.Add("Code", typeof(string));
            dtNarration.Columns.Add("Title", typeof(string));
            foreach (var n in narrationsTask.Result)
                dtNarration.Rows.Add(n.Code, n.Title);

            FillAccount();
            FillNarration();
            FillFilterAccount();
            FillFilterNarration();
        }

        private ERP.Controls.PaginationControl _pager;
        private string _qFrom = "", _qTo = "", _qAccount = "", _qNarration = "";
        private int _qPageSize = 20;

        private void EnsurePager()
        {
            if (_pager != null) return;
            _pager = new ERP.Controls.PaginationControl();
            _pager.PageChanged += async (s, e) =>
            {
                try
                {
                    await FillQueryAsync(_qFrom, _qTo, _qAccount, _qNarration, e.PageNumber, e.PageSize);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading journal vouchers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            _pager.Attach(tabPage4, dgvQuery);
        }

        async System.Threading.Tasks.Task FillQueryAsync(
            string fromDate = "", string toDate = "",
            string account = "", string narration = "",
            int pageNumber = 1, int pageSize = 0)
        {
            EnsurePager();
            _qFrom = fromDate; _qTo = toDate; _qAccount = account; _qNarration = narration;
            if (pageSize > 0) _qPageSize = pageSize;

            var page = await _apiService.GetPagedListAsync(fromDate, toDate, account, narration, "", pageNumber, _qPageSize);
            _queryList = page.Data;
            _pager.SetPagination(page.CurrentPage, page.TotalPages, page.TotalCount, page.PageSize);
            _qPageSize = page.PageSize;
            VoucherIndex = 0;
            dgvQuery.Rows.Clear();
            for (int i = 0; i < _queryList.Count; i++)
            {
                var r = _queryList[i];
                dgvQuery.Rows.Add(
                    r.Date,
                    "JV-" + r.VoucherNo,
                    r.Amount.ToString(),
                    r.Narration,
                    r.CreatedBy + " | " + r.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                    !string.IsNullOrWhiteSpace(r.LastModifiedBy)
                        ? r.LastModifiedBy + " | " + r.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                        : null);
            }
        }

        internal async System.Threading.Tasks.Task FillJournalAsync(string vno)
        {
            string accountFilter = cmbFilterAccounts.SelectedValue != null
                ? cmbFilterAccounts.SelectedValue.ToString() : "";
            var lines = await _apiService.GetDetailAsync(vno, accountFilter);

            txtVoucherNo.Text = "JV-" + vno;
            VoucherNum = vno;
            dgvJournal.Rows.Clear();

            if (lines.Count > 0)
            {
                var first = lines[0];
                dtpDate.Value = first.Date;
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
                    dgvJournal.Rows.Add(
                        l.Seq.ToString(),
                        l.DrAccountId,
                        l.Amount.ToString("N0"),
                        l.CrAccountId,
                        l.Remarks,
                        l.CreatedBy + " | " + l.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                        !string.IsNullOrWhiteSpace(l.LastModifiedBy)
                            ? l.LastModifiedBy + " | " + l.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null);
                }

                dgvJournal.Rows.Add();
                CalcTotAmount();
            }
        }

        internal void FillJournal(string vno)
        {
            _ = FillJournalAsync(vno);
        }

        void FillAccount()
        {
            clnDrAccount.DataSource = dtAccount.Copy();
            clnDrAccount.DisplayMember = "Title";
            clnDrAccount.ValueMember = "Account";
            clnCrAccount.DataSource = dtAccount.Copy();
            clnCrAccount.DisplayMember = "Title";
            clnCrAccount.ValueMember = "Account";
        }

        void FillNarration() { cmbNarration.DataSource = dtNarration.Copy(); cmbNarration.DisplayMember = "Title"; cmbNarration.ValueMember = "Code"; }
        void FillFilterAccount() { cmbFilterAccounts.DataSource = dtAccount.Copy(); cmbFilterAccounts.DisplayMember = "Title"; cmbFilterAccounts.ValueMember = "Account"; cmbFilterAccounts.SelectedIndex = -1; }
        void FillFilterNarration() { cmbFilterNarration.DataSource = dtNarration.Copy(); cmbFilterNarration.DisplayMember = "Title"; cmbFilterNarration.ValueMember = "Code"; cmbFilterNarration.SelectedIndex = -1; }

        #endregion

        void AllowNewRow()
        {
            if (dgvJournal.CurrentRow != null && dgvJournal.CurrentRow.Index == dgvJournal.Rows[dgvJournal.Rows.Count - 1].Index)
            {
                if (dgvJournal.CurrentRow.Cells[clnDrAccount.Index].Value != null &&
                    dgvJournal.CurrentRow.Cells[clnCrAccount.Index].Value != null &&
                    dgvJournal.CurrentRow.Cells[clnAmount.Index].Value != null)
                {
                    int next = dgvJournal.Rows.Add();
                    ClearRowValues(next);
                }
            }
        }

        private void ClearRowValues(int rowIdx)
        {
            if (rowIdx < 0 || rowIdx >= dgvJournal.Rows.Count) return;
            var row = dgvJournal.Rows[rowIdx];
            row.Cells[clnDrAccount.Index].Value = null;
            row.Cells[clnCrAccount.Index].Value = null;
            row.Cells[clnAmount.Index].Value = null;
            row.Cells[clnRemarks.Index].Value = null;
            int seq = 0;
            try { seq = (int)(from DataGridViewRow r in dgvJournal.Rows where r.Cells[clnSeq.Index].Value != null && r.Cells[clnSeq.Index].Value.ToString() != "" select int.Parse(r.Cells[clnSeq.Index].Value.ToString())).Max(); } catch { }
            row.Cells[clnSeq.Index].Value = (seq + 1).ToString();
        }

        private async void frmReceipt_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadLookupsAsync();
                await FillQueryAsync();
                if (_queryList.Count > 0 && txtVoucherNo.Text == "")
                    await FillJournalAsync(_queryList[VoucherIndex].VoucherNo);
                FLogIn = false;
                BeginInvoke(new Action(() =>
                {
                    try { dtpDate.Focus(); } catch { }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading journal voucher data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                dgvJournal.CurrentCell = dgvJournal.Rows[currentRow].Cells[dgvJournal.CurrentCell.ColumnIndex];
            }
        }

        void CalcTotAmount()
        {
            decimal Tot = 0;
            try
            {
                Tot = (from DataGridViewRow row in dgvJournal.Rows
                       where row.Cells[clnAmount.Index].Value != null && !string.IsNullOrWhiteSpace(row.Cells[clnAmount.Index].Value.ToString())
                       select decimal.Parse(row.Cells[clnAmount.Index].Value.ToString())).Sum();
            }
            catch { }
            lblTotAmount.Text = Tot.ToString("N2");
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvJournal.CurrentCell != null && (dgvJournal.CurrentCellAddress.X == clnDrAccount.DisplayIndex || dgvJournal.CurrentCell.ColumnIndex == clnDrAccount.Index))
            {
                ComboBox cb = e.Control as ComboBox;
                if (cb != null)
                {
                    cb.DropDownStyle = ComboBoxStyle.DropDown;
                    cb.AutoCompleteSource = AutoCompleteSource.ListItems;
                    cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cb.SelectedIndexChanged -= new EventHandler(cbDr_SelectedIndexChanged);
                    cb.SelectionChangeCommitted -= new EventHandler(cbDr_SelectionChangeCommitted);

                    var cellVal = dgvJournal.CurrentCell.Value;
                    if (cellVal == null || string.IsNullOrWhiteSpace(cellVal.ToString()))
                    {
                        try { cb.SelectedIndex = -1; cb.Text = ""; } catch { }
                    }
                    else
                    {
                        try { cb.SelectedValue = cellVal.ToString(); } catch { }
                    }

                    cb.SelectedIndexChanged += new EventHandler(cbDr_SelectedIndexChanged);
                    cb.SelectionChangeCommitted += new EventHandler(cbDr_SelectionChangeCommitted);
                }
            }
            else if (dgvJournal.CurrentCell != null && (dgvJournal.CurrentCellAddress.X == clnCrAccount.DisplayIndex || dgvJournal.CurrentCell.ColumnIndex == clnCrAccount.Index))
            {
                ComboBox cb = e.Control as ComboBox;
                if (cb != null)
                {
                    cb.DropDownStyle = ComboBoxStyle.DropDown;
                    cb.AutoCompleteSource = AutoCompleteSource.ListItems;
                    cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cb.SelectedIndexChanged -= new EventHandler(cbCr_SelectedIndexChanged);
                    cb.SelectionChangeCommitted -= new EventHandler(cbCr_SelectionChangeCommitted);

                    var cellVal = dgvJournal.CurrentCell.Value;
                    if (cellVal == null || string.IsNullOrWhiteSpace(cellVal.ToString()))
                    {
                        try { cb.SelectedIndex = -1; cb.Text = ""; } catch { }
                    }
                    else
                    {
                        try { cb.SelectedValue = cellVal.ToString(); } catch { }
                    }

                    cb.SelectedIndexChanged += new EventHandler(cbCr_SelectedIndexChanged);
                    cb.SelectionChangeCommitted += new EventHandler(cbCr_SelectionChangeCommitted);
                }
            }
            else if (dgvJournal.CurrentCell != null && dgvJournal.CurrentCell.ColumnIndex == clnAmount.Index)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null && e.Control.Text != null)
                {
                    tb.KeyPress -= new KeyPressEventHandler(tb_KeyPress);
                    tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);
                }
            }
        }

        void cbDr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvJournal.CurrentCell != null && dgvJournal.CurrentCell.ColumnIndex == clnDrAccount.Index)
            {
                if (sender is ComboBox cb && cb.SelectedIndex != -1)
                {
                    DataRowView dr = cb.SelectedItem as DataRowView;
                    if (dr != null)
                    {
                        if (cb.SelectedValue == null) cb.SelectedValue = dr["Account"].ToString();
                        int rowInd = dgvJournal.CurrentCellAddress.Y;
                        if (rowInd >= 0 && rowInd < dgvJournal.Rows.Count)
                        {
                            dgvJournal[clnDrAccount.Index, rowInd].Value = dr["Account"].ToString();
                        }
                        _ = ShowDrBalAsync(dr["Title"].ToString(), dr["Account"].ToString());
                    }
                }
            }
        }

        void cbDr_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (dgvJournal.CurrentCell != null && dgvJournal.CurrentCell.ColumnIndex == clnDrAccount.Index)
            {
                if (sender is ComboBox cb && cb.SelectedItem is DataRowView drv)
                {
                    int rowInd = dgvJournal.CurrentCellAddress.Y;
                    if (rowInd >= 0 && rowInd < dgvJournal.Rows.Count)
                    {
                        string acct = drv["Account"].ToString();
                        dgvJournal[clnDrAccount.Index, rowInd].Value = acct;
                        _ = ShowDrBalAsync(drv["Title"].ToString(), acct);
                    }
                }
            }
        }

        void cbCr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvJournal.CurrentCell != null && dgvJournal.CurrentCell.ColumnIndex == clnCrAccount.Index)
            {
                if (sender is ComboBox cb && cb.SelectedIndex != -1)
                {
                    DataRowView dr = cb.SelectedItem as DataRowView;
                    if (dr != null)
                    {
                        if (cb.SelectedValue == null) cb.SelectedValue = dr["Account"].ToString();
                        int rowInd = dgvJournal.CurrentCellAddress.Y;
                        if (rowInd >= 0 && rowInd < dgvJournal.Rows.Count)
                        {
                            dgvJournal[clnCrAccount.Index, rowInd].Value = dr["Account"].ToString();
                        }
                        _ = ShowCrBalAsync(dr["Title"].ToString(), dr["Account"].ToString());
                    }
                }
            }
        }

        void cbCr_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (dgvJournal.CurrentCell != null && dgvJournal.CurrentCell.ColumnIndex == clnCrAccount.Index)
            {
                if (sender is ComboBox cb && cb.SelectedItem is DataRowView drv)
                {
                    int rowInd = dgvJournal.CurrentCellAddress.Y;
                    if (rowInd >= 0 && rowInd < dgvJournal.Rows.Count)
                    {
                        string acct = drv["Account"].ToString();
                        dgvJournal[clnCrAccount.Index, rowInd].Value = acct;
                        _ = ShowCrBalAsync(drv["Title"].ToString(), acct);
                    }
                }
            }
        }

        private bool HandleGridEnterNavigation()
        {
            if (dgvJournal.CurrentCell == null) return false;

            int rowIndex = dgvJournal.CurrentCell.RowIndex;
            int colIndex = dgvJournal.CurrentCell.ColumnIndex;

            // 1. Debit Account (clnDrAccount)
            if (colIndex == clnDrAccount.Index)
            {
                if (dgvJournal.EditingControl is ComboBox cb)
                {
                    if (cb.SelectedItem is DataRowView drv)
                    {
                        string acct = drv["Account"].ToString();
                        dgvJournal[clnDrAccount.Index, rowIndex].Value = acct;
                        _ = ShowDrBalAsync(drv["Title"].ToString(), acct);
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
                            dgvJournal[clnDrAccount.Index, rowIndex].Value = acct;
                            _ = ShowDrBalAsync(match["Title"].ToString(), acct);
                        }
                    }
                }

                dgvJournal.EndEdit();

                var acctVal = dgvJournal[clnDrAccount.Index, rowIndex].Value;
                bool isAcctEmpty = acctVal == null || string.IsNullOrWhiteSpace(acctVal.ToString());
                bool isLastRow = (rowIndex == dgvJournal.Rows.Count - 1);

                // If on last row without selecting debit account -> Exit grid to Save button!
                if (isLastRow && isAcctEmpty)
                {
                    dgvJournal.EndEdit();
                    CalcTotAmount();
                    btnSave.Focus();
                    return true;
                }

                // If debit account selected -> move to Amount
                if (!isAcctEmpty)
                {
                    dgvJournal.CurrentCell = dgvJournal[clnAmount.Index, rowIndex];
                    dgvJournal.BeginEdit(true);
                    return true;
                }

                return true;
            }

            // 2. Amount (clnAmount)
            if (colIndex == clnAmount.Index)
            {
                dgvJournal.EndEdit();
                CalcTotAmount();
                dgvJournal.CurrentCell = dgvJournal[clnCrAccount.Index, rowIndex];
                dgvJournal.BeginEdit(true);
                if (dgvJournal.EditingControl is ComboBox cb)
                {
                    cb.Focus();
                }
                return true;
            }

            // 3. Credit Account (clnCrAccount)
            if (colIndex == clnCrAccount.Index)
            {
                if (dgvJournal.EditingControl is ComboBox cb)
                {
                    if (cb.SelectedItem is DataRowView drv)
                    {
                        string acct = drv["Account"].ToString();
                        dgvJournal[clnCrAccount.Index, rowIndex].Value = acct;
                        _ = ShowCrBalAsync(drv["Title"].ToString(), acct);
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
                            dgvJournal[clnCrAccount.Index, rowIndex].Value = acct;
                            _ = ShowCrBalAsync(match["Title"].ToString(), acct);
                        }
                    }
                }

                dgvJournal.EndEdit();
                dgvJournal.CurrentCell = dgvJournal[clnRemarks.Index, rowIndex];
                dgvJournal.BeginEdit(true);
                return true;
            }

            // 4. Remarks (clnRemarks)
            if (colIndex == clnRemarks.Index)
            {
                dgvJournal.EndEdit();
                CalcTotAmount();

                int nextRowIdx;
                if (rowIndex == dgvJournal.Rows.Count - 1)
                {
                    nextRowIdx = dgvJournal.Rows.Add();
                    ClearRowValues(nextRowIdx);
                }
                else
                {
                    nextRowIdx = rowIndex + 1;
                }
                dgvJournal.CurrentCell = dgvJournal[clnDrAccount.Index, nextRowIdx];
                dgvJournal.BeginEdit(true);
                if (dgvJournal.EditingControl is ComboBox cb)
                {
                    cb.Focus();
                }
                return true;
            }

            // Fallback
            dgvJournal.EndEdit();
            CalcTotAmount();
            int targetIdx = rowIndex < dgvJournal.Rows.Count - 1 ? rowIndex + 1 : dgvJournal.Rows.Add();
            if (targetIdx == dgvJournal.Rows.Count - 1) ClearRowValues(targetIdx);
            dgvJournal.CurrentCell = dgvJournal[clnDrAccount.Index, targetIdx];
            dgvJournal.BeginEdit(true);
            if (dgvJournal.EditingControl is ComboBox cbFallback)
            {
                cbFallback.Focus();
            }
            return true;
        }

        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvJournal.CurrentCell.ColumnIndex == clnAmount.Index)
            {
                if ((sender as TextBox).SelectedText.Length > 0)
                {
                    int selind = (sender as TextBox).SelectionStart;
                    (sender as TextBox).Text = (sender as TextBox).Text.Replace((sender as TextBox).SelectedText, "");
                    (sender as TextBox).SelectionStart = selind;
                    (sender as TextBox).SelectionLength = 0;
                }
                if (!char.IsControl(e.KeyChar)
                    && !char.IsDigit(e.KeyChar)
                    && !((sender as TextBox).Text.Count(a => a == '.') == 0 && e.KeyChar == '.'))
                    e.Handled = true;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CalcTotAmount();
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int seq = 0;
            try
            {
                seq = (int)(from DataGridViewRow row in dgvJournal.Rows
                            where row.Cells[clnSeq.Index].Value != null && row.Cells[clnSeq.Index].Value.ToString() != ""
                            select int.Parse(row.Cells[clnSeq.Index].Value.ToString())).Max();
            }
            catch { }
            if (dgvJournal.Rows[e.RowIndex].Cells[clnSeq.Index].Value == null)
                dgvJournal.Rows[e.RowIndex].Cells[clnSeq.Index].Value = (seq + 1).ToString();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving)
                return;

            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to save this...!", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _isSaving = true;
                btnSave.Enabled = false;
                try
                {
                    string voucher = string.IsNullOrWhiteSpace(txtVoucherNo.Text)
                        ? null
                        : (txtVoucherNo.Text.StartsWith("JV-", StringComparison.OrdinalIgnoreCase)
                            ? txtVoucherNo.Text.Substring(3)
                            : txtVoucherNo.Text);

                    var lines = new List<JournalVoucherLineRequest>();
                    foreach (DataGridViewRow row in dgvJournal.Rows)
                    {
                        if (row.Cells[clnDrAccount.Index].Value == null ||
                            row.Cells[clnCrAccount.Index].Value == null ||
                            row.Cells[clnAmount.Index].Value == null)
                            continue;

                        lines.Add(new JournalVoucherLineRequest
                        {
                            Seq = int.Parse(row.Cells[clnSeq.Index].Value.ToString()),
                            DrAccount = row.Cells[clnDrAccount.Index].Value.ToString(),
                            CrAccount = row.Cells[clnCrAccount.Index].Value.ToString(),
                            Amount = decimal.Parse(row.Cells[clnAmount.Index].Value.ToString()),
                            Remarks = (string)row.Cells[clnRemarks.Index].Value
                        });
                    }

                    bool isNew = string.IsNullOrWhiteSpace(voucher);
                    if (isNew)
                    {
                        var createRequest = new JournalVoucherCreateRequest
                        {
                            Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                            Narration = cmbNarration.SelectedValue?.ToString(),
                            Lines = lines
                        };
                        voucher = await _apiService.CreateAsync(createRequest);
                    }
                    else
                    {
                        var updateRequest = new JournalVoucherUpdateRequest
                        {
                            Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                            Narration = cmbNarration.SelectedValue?.ToString(),
                            Lines = lines
                        };
                        await _apiService.UpdateAsync(voucher, updateRequest);
                    }

                    await FillJournalAsync(voucher);

                    if (isNew)
                    {
                        await FillQueryAsync(_qFrom, _qTo, _qAccount, _qNarration, 1);
                    }

                    MessageBox.Show("Record Successfully Saved..!");
                    btnNew.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving journal voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _isSaving = false;
                    btnSave.Enabled = true;
                }
            }
        }

        private async void dgvQuery_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!FLogIn && e.RowIndex != -1)
            {
                string voucherNo = dgvQuery.Rows[e.RowIndex].Cells[clnVoucherNum.Index].Value.ToString();
                tabDetailQuery.SelectedTab = tabpgDetail;
                await FillJournalAsync(voucherNo.Substring(3));
            }
        }

        private bool _isQueryLoading = false;
        private async void btnFind_Click(object sender, EventArgs e)
        {
            if (_isQueryLoading) return;
            _isQueryLoading = true;
            btnFind.Enabled = false;
            try
            {
                await FillQueryAsync(
                    txtFdate.Text,
                    txtTdate.Text,
                    cmbFilterAccounts.SelectedValue != null ? cmbFilterAccounts.SelectedValue.ToString() : "",
                    cmbFilterNarration.SelectedValue != null ? cmbFilterNarration.SelectedValue.ToString() : "",
                    1,
                    _qPageSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading journal vouchers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isQueryLoading = false;
                btnFind.Enabled = true;
            }
        }

        private void txtFdate_Validated(object sender, EventArgs e)
        {
            try
            {
                string text = (sender as TextBox).Text.Replace(" ", "-").Replace("/", "-");
                DateTime date = DateTime.ParseExact(text, Validation.dateformats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                (sender as TextBox).Text = date.ToString("dd-MMM-yyyy");
            }
            catch (Exception)
            {
                (sender as TextBox).Text = "";
            }
        }

        private async void dgvQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dgvQuery.CurrentRow != null)
            {
                string voucherNo = dgvQuery.CurrentRow.Cells[clnVoucherNum.Index].Value.ToString();
                tabDetailQuery.SelectedTab = tabpgDetail;
                await FillJournalAsync(voucherNo.Substring(3));
                e.Handled = true;
            }
        }

        void Navigate(Navigators Nav)
        {
            if (dgvQuery.CurrentRow != null && tabDetailQuery.SelectedTab == tabpgDetail)
            {
                if (Nav == Navigators.Up && VoucherIndex > 0) { VoucherIndex--; _ = FillJournalAsync(_queryList[VoucherIndex].VoucherNo); }
                else if (Nav == Navigators.Down && _queryList.Count - 1 > VoucherIndex) { VoucherIndex++; _ = FillJournalAsync(_queryList[VoucherIndex].VoucherNo); }
                else if (Nav == Navigators.End) { VoucherIndex = 0; _ = FillJournalAsync(_queryList[VoucherIndex].VoucherNo); }
                else if (Nav == Navigators.Home) { VoucherIndex = _queryList.Count - 1; _ = FillJournalAsync(_queryList[VoucherIndex].VoucherNo); }
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

        void CopyAsNew()
        {
            if (txtVoucherNo.Text != "")
            {
                txtVoucherNo.Text = "";
                VoucherNum = null;
                txtCreatedBy.Text = "";
                txtEditBy.Text = "";
                dtpDate.Value = DateTime.Now;
                dtpDate.Focus();
            }
            else
            {
                MessageBox.Show("No record to copy. Please load a record first.", "Copy as New", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCopyAsNew_Click(object sender, EventArgs e) => CopyAsNew();

        private void btnNew_Click(object sender, EventArgs e)
        {
            dgvJournal.Rows.Clear();
            Validation.Clear(grpInvoiceDetail);
            lblTotAmount.Text = "0.00";
            lblDrBalance.Text = "0.00";
            lblCrBalance.Text = "0.00";
            txtVoucherNo.Text = "";
            VoucherNum = null;
            int r = dgvJournal.Rows.Add();
            ClearRowValues(r);
            cmbNarration.SelectedIndex = -1;
            dtpDate.Value = DateTime.Now;
            dtpDate.Focus();
        }

        #region Navigation
        private void btnHome_Click(object sender, EventArgs e) => Navigate(Navigators.Home);
        private void btnPri_Click(object sender, EventArgs e) => Navigate(Navigators.Up);
        private void btnNext_Click(object sender, EventArgs e) => Navigate(Navigators.Down);
        private void btnEnd_Click(object sender, EventArgs e) => Navigate(Navigators.End);
        #endregion

        private async void deleteRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Delete this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (dgvJournal[clnSeq.Index, dgvJournal.CurrentRow.Index].Value != null)
                {
                    try
                    {
                        if (txtVoucherNo.Text != "")
                        {
                            int seq = int.Parse(dgvJournal[clnSeq.Index, dgvJournal.CurrentRow.Index].Value.ToString());
                            await _apiService.DeleteLineAsync(txtVoucherNo.Text.Substring(3), seq);
                        }
                        dgvJournal.Rows.RemoveAt(dgvJournal.CurrentRow.Index);
                        CalcTotAmount();
                        MessageBox.Show("Record Successfully Deleted..!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting line: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void dgvJournal_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dgvJournal.Rows[e.RowIndex].Selected = true;
                dgvJournal.CurrentCell = dgvJournal.Rows[e.RowIndex].Cells[e.ColumnIndex];
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtVoucherNo.Text != "")
            {
                if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Delete this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        string vno = txtVoucherNo.Text.Substring(3);
                        await _apiService.DeleteAsync(vno);

                        DataGridViewRow row = dgvQuery.Rows.Cast<DataGridViewRow>()
                            .First(r => r.Cells[clnVoucherNum.Index].Value.ToString().Equals(txtVoucherNo.Text));

                        _queryList.RemoveAll(r => r.VoucherNo == vno);
                        btnNew_Click(null, null);
                        dgvQuery.Rows.Remove(row);
                        CalcTotAmount();
                        MessageBox.Show("Record Successfully Deleted..!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting journal voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void frmJournalVoucher_KeyDown(object sender, KeyEventArgs e)
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
        }

        private void dgvJournal_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dgvJournal[clnDrAccount.Index, e.RowIndex].Value != null)
                _ = ShowDrBalAsync(
                    (dgvJournal.Rows[e.RowIndex].Cells[clnDrAccount.Index] as DataGridViewComboBoxCell).FormattedValue.ToString(),
                    (dgvJournal.Rows[e.RowIndex].Cells[clnDrAccount.Index] as DataGridViewComboBoxCell).Value.ToString());

            if (e.RowIndex > -1 && dgvJournal[clnCrAccount.Index, e.RowIndex].Value != null)
                _ = ShowCrBalAsync(
                    (dgvJournal.Rows[e.RowIndex].Cells[clnCrAccount.Index] as DataGridViewComboBoxCell).FormattedValue.ToString(),
                    (dgvJournal.Rows[e.RowIndex].Cells[clnCrAccount.Index] as DataGridViewComboBoxCell).Value.ToString());
        }
    }
}
