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
            dgvJournal.Rows.Add();
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

        async System.Threading.Tasks.Task FillQueryAsync(
            string fromDate = "", string toDate = "",
            string account = "", string narration = "")
        {
            dgvQuery.Rows.Clear();
            _queryList = await _apiService.GetListAsync(fromDate, toDate, account, narration);
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
            if (dgvJournal.CurrentRow.Index == dgvJournal.Rows[dgvJournal.Rows.Count - 1].Index)
            {
                if (dgvJournal.CurrentRow.Cells[clnDrAccount.Index].Value != null &&
                    dgvJournal.CurrentRow.Cells[clnCrAccount.Index].Value != null &&
                    dgvJournal.CurrentRow.Cells[clnAmount.Index].Value != null)
                    dgvJournal.Rows.Add();
            }
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
                SendKeys.Send("{tab}");
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AllowNewRow();
            resetRow = true;
            currentRow = e.RowIndex;
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
                       where row.Cells[clnAmount.Index].Value != null
                       select decimal.Parse(row.Cells[clnAmount.Index].Value.ToString())).Sum();
            }
            catch { }
            lblTotAmount.Text = Tot.ToString();
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvJournal.CurrentCellAddress.X == clnDrAccount.DisplayIndex)
            {
                ComboBox cb = e.Control as ComboBox;
                if (cb != null)
                {
                    cb.DropDownStyle = ComboBoxStyle.DropDown;
                    cb.AutoCompleteSource = AutoCompleteSource.ListItems;
                    cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cb.SelectedIndexChanged += new EventHandler(cbDr_SelectedIndexChanged);
                }
            }
            if (dgvJournal.CurrentCellAddress.X == clnCrAccount.DisplayIndex)
            {
                ComboBox cb = e.Control as ComboBox;
                if (cb != null)
                {
                    cb.DropDownStyle = ComboBoxStyle.DropDown;
                    cb.AutoCompleteSource = AutoCompleteSource.ListItems;
                    cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cb.SelectedIndexChanged += new EventHandler(cbCr_SelectedIndexChanged);
                }
            }
            if (dgvJournal.CurrentCell.ColumnIndex == clnAmount.Index)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null && e.Control.Text != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);
                }
            }
        }

        void cbDr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvJournal.CurrentCell.ColumnIndex == clnDrAccount.Index)
                if ((sender as ComboBox).SelectedIndex != -1)
                {
                    DataRowView dr = (DataRowView)(sender as ComboBox).SelectedItem;
                    _ = ShowDrBalAsync(dr[1].ToString(), dr[0].ToString());
                    if ((sender as ComboBox).SelectedValue == null)
                        (sender as ComboBox).SelectedValue = dr[0].ToString();
                }
        }

        void cbCr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvJournal.CurrentCell.ColumnIndex == clnCrAccount.Index)
                if ((sender as ComboBox).SelectedIndex != -1)
                {
                    DataRowView dr = (DataRowView)(sender as ComboBox).SelectedItem;
                    _ = ShowCrBalAsync(dr[1].ToString(), dr[0].ToString());
                    if ((sender as ComboBox).SelectedValue == null)
                        (sender as ComboBox).SelectedValue = dr[0].ToString();
                }
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
                    string voucher = txtVoucherNo.Text == "" ? null : txtVoucherNo.Text.Split('-')[1];

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
                        var refreshed = await _apiService.GetListAsync();
                        var entry = refreshed.Find(r => r.VoucherNo == voucher);
                        if (entry != null)
                        {
                            _queryList.Insert(0, entry);
                            dgvQuery.Rows.Insert(0,
                                entry.Date,
                                "JV-" + entry.VoucherNo,
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

        private async void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                await FillQueryAsync(
                    txtFdate.Text,
                    txtTdate.Text,
                    cmbFilterAccounts.SelectedValue != null ? cmbFilterAccounts.SelectedValue.ToString() : "",
                    cmbFilterNarration.SelectedValue != null ? cmbFilterNarration.SelectedValue.ToString() : "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading journal vouchers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            dgvJournal.Rows.Clear();
            Validation.Clear(grpInvoiceDetail);
            dgvJournal.Rows.Add();
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
            if (e.KeyCode == Keys.Enter && !dgvJournal.Focused)
            {
                SendKeys.Send("{tab}");
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
