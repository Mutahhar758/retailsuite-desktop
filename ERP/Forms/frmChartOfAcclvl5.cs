using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmChartOfAccLvl5 : Form
    {
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private List<ChartOfAccountDto> _chartAccounts;
        private Dictionary<string, ChartOfAccountDto> _chartAccountMap;
        private List<ChartOfAccountHeadDto> _heads;

        public frmChartOfAccLvl5()
        {
            InitializeComponent();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _chartAccounts = new List<ChartOfAccountDto>();
            _chartAccountMap = new Dictionary<string, ChartOfAccountDto>();
            _heads = new List<ChartOfAccountHeadDto>();
            UserInfo.ApplyFormPermissions(this, AppResource.DetailAccounts);
        }

        bool FlogIn = true;
        internal string AccountHead = "";

        private async void frmChartOfAccount_Load(object sender, EventArgs e)
        {
            try
            {
                _heads = await _chartOfAccountApiService.GetHeadsAsync(4);
                cmbAccountHead.DataSource = _heads;
                cmbAccountHead.DisplayMember = "Title";
                cmbAccountHead.ValueMember = "Account";
                cmbAccountHead.SelectedIndex = -1;

                _chartAccounts = await _chartOfAccountApiService.GetActiveAccountsAsync();
                _chartAccountMap = _chartAccounts
                    .Where(x => !string.IsNullOrWhiteSpace(x.Account))
                    .ToDictionary(x => x.Account, x => x);

                FlogIn = false;

                if (!string.IsNullOrWhiteSpace(AccountHead))
                {
                    cmbAccountHead.SelectedValue = AccountHead;
                    Filldgv();
                    dgvAccount.Select();
                }
                else
                    Filldgv();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Filldgv()
        {
            if (FlogIn) return;

            var selected = cmbAccountHead.SelectedItem as ChartOfAccountHeadDto;

            lblParentHeader.Text = selected?.Title ?? string.Empty;
            dgvAccount.Rows.Clear();

            if (selected == null)
            {
                lbldgvCreated.Text = "-";
                lbldgvEdit.Text = "-";
                return;
            }

            string q = txtSearch.Text.Trim().ToLower();

            var filtered = _chartAccounts
                .Where(x => string.Equals(x.ParentId, selected.Account, StringComparison.OrdinalIgnoreCase))
                .Where(x => string.IsNullOrWhiteSpace(q) || 
                            (!string.IsNullOrEmpty(x.Account) && x.Account.ToLower().Contains(q)) || 
                            (!string.IsNullOrEmpty(x.Title) && x.Title.ToLower().Contains(q)))
                .OrderBy(x => x.Account)
                .ToList();

            for (int i = 0; i < filtered.Count; i++)
            {
                dgvAccount.Rows.Add(
                    filtered[i].Account,
                    filtered[i].Title,
                    filtered[i].ParentId,
                    filtered[i].AccType,
                    filtered[i].AccLevel.ToString(),
                    string.IsNullOrWhiteSpace(filtered[i].CreatedBy)
                        ? ""
                        : filtered[i].CreatedBy + " | " + filtered[i].CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                    string.IsNullOrWhiteSpace(filtered[i].LastModifiedBy) || !filtered[i].LastModifiedOn.HasValue
                        ? ""
                        : filtered[i].LastModifiedBy + " | " + filtered[i].LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                    "0");
            }

            AllowRowAdd();

            if (filtered.Count == 0)
            {
                lbldgvCreated.Text = "-";
                lbldgvEdit.Text = "-";
            }
        }

        private void btnOpenning_Click(object sender, EventArgs e)
        {
            var selected = cmbAccountHead.SelectedItem as ChartOfAccountHeadDto;
            frmOpeningBal frm = new frmOpeningBal();
            if (selected != null)
                frm.ParentAcc = selected.Account;
            frm.ShowDialog();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Save this...!", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                bool saved = false;
                var selected = cmbAccountHead.SelectedItem as ChartOfAccountHeadDto;

                foreach (DataGridViewRow item in dgvAccount.Rows)
                {
                    if (item.Cells[clnTitle.Index].Value == null ||
                        item.Cells[clnIsEdit.Index].Value?.ToString() != "1")
                        continue;

                    var title = item.Cells[clnTitle.Index].Value.ToString();
                    if (string.IsNullOrWhiteSpace(title)) continue;

                    var account = item.Cells[clnAccount.Index].Value?.ToString();

                    if (!string.IsNullOrWhiteSpace(account) && _chartAccountMap.ContainsKey(account))
                        await _chartOfAccountApiService.UpdateAsync(account, title);
                    else if (selected != null)
                        await _chartOfAccountApiService.CreateAsync(selected.Account, title);

                    saved = true;
                }

                if (saved)
                {
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("Record Successfully Saved..!");

                    _chartAccounts = await _chartOfAccountApiService.GetActiveAccountsAsync();
                    _chartAccountMap = _chartAccounts
                        .Where(x => !string.IsNullOrWhiteSpace(x.Account))
                        .ToDictionary(x => x.Account, x => x);

                    Filldgv();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvAccount_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAccount.CurrentRow != null)
            {
                lbldgvCreated.Text = dgvAccount.CurrentRow.Cells[clnCreated.Index].Value?.ToString() ?? "-";
                lbldgvEdit.Text = dgvAccount.CurrentRow.Cells[clnEdit.Index].Value?.ToString() ?? "-";
            }
            else
            {
                lbldgvCreated.Text = "-";
                lbldgvEdit.Text = "-";
            }
        }

        private void dgvAccount_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!FlogIn && e.RowIndex >= 0 && e.RowIndex < dgvAccount.Rows.Count)
                dgvAccount.Rows[e.RowIndex].Cells[clnIsEdit.Index].Value = "1";
        }

        private void dgvAccount_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == dgvAccount.Rows.Count - 1 &&
                dgvAccount[clnTitle.Index, e.RowIndex].Value != null)
                AllowRowAdd();
        }

        private void AllowRowAdd()
        {
            var selected = cmbAccountHead.SelectedItem as ChartOfAccountHeadDto;
            if (selected == null) return;

            int ind = dgvAccount.Rows.Add("", null, selected.Account, "Detail", "5", "", "", "1");
            dgvAccount.CurrentCell = dgvAccount.Rows[ind].Cells[clnTitle.Index];
        }

        private void cmbAccountHead_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filldgv();
        }

        private void frmChartOfAccLvl5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !dgvAccount.Focused)
                SendKeys.Send("{tab}");
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            Filldgv();
        }
    }
}
