using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmChartOfAcc : Form
    {
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private List<ChartOfAccountDto> _chartAccounts;
        private Dictionary<string, ChartOfAccountDto> _chartAccountMap;

        public frmChartOfAcc()
        {
            InitializeComponent();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _chartAccounts = new List<ChartOfAccountDto>();
            _chartAccountMap = new Dictionary<string, ChartOfAccountDto>();
        }

        bool FlogIn = true;

        private static string NormalizeParentId(string parentId)
        {
            return string.IsNullOrWhiteSpace(parentId) ? "0" : parentId;
        }

        private async System.Threading.Tasks.Task FillAsync()
        {
            tvChartOfAcc.Nodes.Clear();
            _chartAccounts = await _chartOfAccountApiService.GetActiveAccountsAsync();
            _chartAccountMap = _chartAccounts
                .Where(x => !string.IsNullOrWhiteSpace(x.Account))
                .ToDictionary(x => x.Account, x => x);

            tvChartOfAcc.Nodes.Add("0", "Chart Of Account", null);

            for (int i = 0; i < _chartAccounts.Count; i++)
            {
                string parentId = NormalizeParentId(_chartAccounts[i].ParentId);
                var parentNode = tvChartOfAcc.Nodes.Find(parentId, true).FirstOrDefault();
                if (parentNode == null)
                    continue;

                parentNode.Nodes.Add(_chartAccounts[i].Account, _chartAccounts[i].Title);
            }

            tvChartOfAcc.CollapseAll();
            tvChartOfAcc.Nodes[0].Expand();
            tvChartOfAcc.SelectedNode = tvChartOfAcc.Nodes[0];
        }

        private async void frmChartOfAccount_Load(object sender, EventArgs e)
        {
            try
            {
                await FillAsync();
                FlogIn = false;
                Filldgv();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void FillDetail()
        {
            try
            {
                if (tvChartOfAcc.SelectedNode == null || tvChartOfAcc.SelectedNode.Level == 0)
                    throw new Exception();

                lblParent.Text = tvChartOfAcc.SelectedNode.Parent != null ? tvChartOfAcc.SelectedNode.Parent.Text : "-";
                lblAccount.Text = tvChartOfAcc.SelectedNode.Name;
                lblTitle.Text = tvChartOfAcc.SelectedNode.Text;
                lblLevel.Text = tvChartOfAcc.SelectedNode.Level.ToString();

                ChartOfAccountDto chartOfAccount;
                if (_chartAccountMap.TryGetValue(tvChartOfAcc.SelectedNode.Name, out chartOfAccount))
                {
                    lbltype.Text = chartOfAccount.AccType;
                    lblCreated.Text = string.IsNullOrWhiteSpace(chartOfAccount.CreatedBy)
                        ? "-"
                        : chartOfAccount.CreatedBy + " | " + chartOfAccount.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    lblEdit.Text = string.IsNullOrWhiteSpace(chartOfAccount.LastModifiedBy) || !chartOfAccount.LastModifiedOn.HasValue
                        ? "-"
                        : chartOfAccount.LastModifiedBy + " | " + chartOfAccount.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt");
                }
                else
                {
                    lbltype.Text = "-";
                    lblCreated.Text = "-";
                    lblEdit.Text = "-";
                }
            }
            catch (Exception)
            {
                lblParent.Text = "-";
                lblAccount.Text = "-";
                lblTitle.Text = "-";
                lblLevel.Text = "-";
                lbltype.Text = "-";
                lblCreated.Text = "-";
                lblEdit.Text = "-";
            }
        }

        void Filldgv()
        {
            if (!FlogIn && tvChartOfAcc.SelectedNode != null)
            {
                lblParentHeader.Text = tvChartOfAcc.SelectedNode.Text;

                var filtered = _chartAccounts
                    .Where(x => string.Equals(NormalizeParentId(x.ParentId), tvChartOfAcc.SelectedNode.Name, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.Account)
                    .ToList();

                dgvAccount.Rows.Clear();
                for (int i = 0; i < filtered.Count; i++)
                {
                    dgvAccount.Rows.Add(
                        filtered[i].Account,
                        filtered[i].Title,
                        NormalizeParentId(filtered[i].ParentId),
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
        }

        private void tvChartOfAcc_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tvChartOfAcc.SelectedNode.ContextMenuStrip = contextMenuStrip1;
            if (e.Node.Level == 5)
                tvChartOfAcc.SelectedNode.ContextMenuStrip.Items[0].Text = "&Add Into " + e.Node.Parent.Text;
            else
                tvChartOfAcc.SelectedNode.ContextMenuStrip.Items[0].Text = "&Add";
            Filldgv();
            if (!FlogIn)
            {
                FillDetail();
            }
        }

        private async void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvChartOfAcc.SelectedNode != tvChartOfAcc.Nodes[0])
                {
                    if (tvChartOfAcc.SelectedNode.Level == 5)
                        tvChartOfAcc.SelectedNode = tvChartOfAcc.SelectedNode.Parent;

                    frmAccountAddEdit frm = new frmAccountAddEdit();
                    frm.parentId = tvChartOfAcc.SelectedNode.Name;
                    frm.Account = string.Empty;
                    frm.Type = tvChartOfAcc.SelectedNode.Level == 4 ? "Detail" : "Parent";
                    frm.level = (tvChartOfAcc.SelectedNode.Level + 1).ToString();
                    frm.ShowDialog();
                    tvChartOfAcc.CollapseAll();
                    if (frm.DialogResult == DialogResult.Yes)
                    {
                        await FillAsync();
                        tvChartOfAcc.SelectedNode = tvChartOfAcc.Nodes.Find(frm.Account, true).FirstOrDefault();
                    }
                    else
                    {
                        tvChartOfAcc.SelectedNode = tvChartOfAcc.Nodes.Find(frm.parentId, true).FirstOrDefault();
                    }

                    if (tvChartOfAcc.SelectedNode != null)
                        tvChartOfAcc.SelectedNode.Expand();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void changeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvChartOfAcc.SelectedNode != tvChartOfAcc.Nodes[0] && tvChartOfAcc.SelectedNode != tvChartOfAcc.Nodes[0].Nodes[tvChartOfAcc.SelectedNode.Index])
                {
                    frmAccountAddEdit frm = new frmAccountAddEdit();
                    frm.parentId = tvChartOfAcc.SelectedNode.Parent.Name;
                    frm.Account = tvChartOfAcc.SelectedNode.Name;
                    frm.Title = tvChartOfAcc.SelectedNode.Text;
                    frm.Type = tvChartOfAcc.SelectedNode.Level == 5 ? "Detail" : "Parent";
                    frm.level = tvChartOfAcc.SelectedNode.Level.ToString();

                    ChartOfAccountDto chartOfAccount;
                    if (_chartAccountMap.TryGetValue(tvChartOfAcc.SelectedNode.Name, out chartOfAccount))
                    {
                        frm.CreatedBy = string.IsNullOrWhiteSpace(chartOfAccount.CreatedBy)
                            ? "-"
                            : chartOfAccount.CreatedBy + " | " + chartOfAccount.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt");
                        frm.EditBy = string.IsNullOrWhiteSpace(chartOfAccount.LastModifiedBy) || !chartOfAccount.LastModifiedOn.HasValue
                            ? "-"
                            : chartOfAccount.LastModifiedBy + " | " + chartOfAccount.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    }

                    frm.ShowDialog();
                    if (frm.DialogResult == DialogResult.Yes)
                    {
                        await FillAsync();
                    }

                    tvChartOfAcc.CollapseAll();
                    tvChartOfAcc.SelectedNode = tvChartOfAcc.Nodes.Find(frm.Account, true).FirstOrDefault();
                    if (tvChartOfAcc.SelectedNode != null)
                        tvChartOfAcc.SelectedNode.Expand();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tvChartOfAcc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert)
                addToolStripMenuItem_Click(null, null);
            else if (e.KeyCode == Keys.Enter)
                changeToolStripMenuItem_Click(null, null);
        }

        private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvChartOfAcc.SelectedNode != null && tvChartOfAcc.SelectedNode != tvChartOfAcc.Nodes[0] && tvChartOfAcc.SelectedNode.Level != 1)
                {
                    if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Delete this...!", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        await _chartOfAccountApiService.DeleteAsync(tvChartOfAcc.SelectedNode.Name);
                        MessageBox.Show("Record Successfully Deleted..!");
                        await FillAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenning_Click(object sender, EventArgs e)
        {
            frmOpeningBal frm = new frmOpeningBal();
            frm.ShowDialog();
        }

        private void tvChartOfAcc_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) tvChartOfAcc.SelectedNode = e.Node;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Save this...!", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bool saved = false;
                    string selectedAccount = tvChartOfAcc.SelectedNode != null ? tvChartOfAcc.SelectedNode.Name : string.Empty;

                    foreach (DataGridViewRow item in dgvAccount.Rows)
                    {
                        if (item.Cells[clnTitle.Index].Value == null || item.Cells[clnIsEdit.Index].Value == null || item.Cells[clnIsEdit.Index].Value.ToString() != "1")
                            continue;

                        var title = item.Cells[clnTitle.Index].Value.ToString();
                        if (string.IsNullOrWhiteSpace(title))
                            continue;

                        var account = item.Cells[clnAccount.Index].Value != null ? item.Cells[clnAccount.Index].Value.ToString() : string.Empty;

                        if (!string.IsNullOrWhiteSpace(account) && _chartAccountMap.ContainsKey(account))
                        {
                            await _chartOfAccountApiService.UpdateAsync(account, title);
                        }
                        else
                        {
                            var parentId = item.Cells[clnParent.Index].Value != null ? item.Cells[clnParent.Index].Value.ToString() : string.Empty;
                            await _chartOfAccountApiService.CreateAsync(parentId, title);
                        }

                        saved = true;
                    }

                    if (saved)
                    {
                        MessageBox.Show("Record Successfully Saved..!");
                        await FillAsync();
                        if (!string.IsNullOrWhiteSpace(selectedAccount))
                        {
                            var selectedNode = tvChartOfAcc.Nodes.Find(selectedAccount, true).FirstOrDefault();
                            if (selectedNode != null)
                                tvChartOfAcc.SelectedNode = selectedNode;
                        }
                    }
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
                lbldgvCreated.Text = dgvAccount.CurrentRow.Cells[clnCreated.Index].Value != null ? dgvAccount.CurrentRow.Cells[clnCreated.Index].Value.ToString() : "-";
                lbldgvEdit.Text = dgvAccount.CurrentRow.Cells[clnEdit.Index].Value != null ? dgvAccount.CurrentRow.Cells[clnEdit.Index].Value.ToString() : "-";
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
            {
                dgvAccount.Rows[e.RowIndex].Cells[clnIsEdit.Index].Value = "1";
            }
        }

        private void dgvAccount_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == dgvAccount.Rows.Count - 1)
            {
                if (dgvAccount[clnTitle.Index, e.RowIndex].Value != null)
                {
                    AllowRowAdd();
                }
            }
        }

        private void AllowRowAdd()
        {
            if (tvChartOfAcc.SelectedNode != null && tvChartOfAcc.SelectedNode.Level != 0 && tvChartOfAcc.SelectedNode.Level != 5)
            {
                dgvAccount.Rows.Add("", null, tvChartOfAcc.SelectedNode.Name,
                    tvChartOfAcc.SelectedNode.Level == 4 ? "Detail" : "Parent",
                    (tvChartOfAcc.SelectedNode.Level + 1).ToString(), "", "", "1");
            }
        }
    }
}
