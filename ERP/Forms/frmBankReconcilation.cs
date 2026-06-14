using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmBankReconcilation : Form
    {
        private readonly BankReconciliationApiService _apiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        bool AllowValidate = false;

        public frmBankReconcilation()
        {
            InitializeComponent();
            _apiService = new BankReconciliationApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
        }

        async System.Threading.Tasks.Task LoadLookupsAsync()
        {
            var banks = await _chartOfAccountApiService.GetCashBankAccountsAsync();
            cmbBanks.DataSource = banks.OrderBy(x => x.Title).ToList();
            cmbBanks.DisplayMember = "Title";
            cmbBanks.ValueMember = "Account";
            cmbBanks.SelectedIndex = banks.Count > 0 ? 0 : -1;
        }

        async System.Threading.Tasks.Task FillAsync()
        {
            if (cmbBanks.SelectedValue == null)
                return;

            var snapshot = await _apiService.GetSnapshotAsync((string)cmbBanks.SelectedValue, dtpFdate.Value, dtpTdate.Value);

            AllowValidate = false;
            dgvBankRec.Rows.Clear();

            var lines = snapshot.Lines ?? new List<BankReconciliationLineDto>();
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                dgvBankRec.Rows.Add(line.Clear,
                    line.VoucherNo,
                    line.Date,
                    line.CheckDate.HasValue ? (object)line.CheckDate.Value : null,
                    line.CheckNum,
                    line.ReconcileDate.HasValue ? (object)line.ReconcileDate.Value : null,
                    line.Title,
                    line.Dr.ToString("N2"),
                    line.Cr.ToString("N2"));
            }

            txtReconcileBal.Text = snapshot.ReconcileBalance.ToString("N2");
            txtStatementBal.Text = snapshot.StatementBalance.ToString("N2");

            decimal totDr = dgvBankRec.Rows.Cast<DataGridViewRow>().Sum(rr => GetDecimal(rr.Cells[clnDr.Index].Value));
            decimal totCr = dgvBankRec.Rows.Cast<DataGridViewRow>().Sum(rr => GetDecimal(rr.Cells[clnCredit.Index].Value));
            lblTotDr.Text = totDr.ToString("N2");
            lblTotCr.Text = totCr.ToString("N2");

            AllowValidate = true;
            CalcDiff();
        }

        static decimal GetDecimal(object value)
        {
            decimal parsed;
            return decimal.TryParse(Convert.ToString(value), out parsed) ? parsed : 0;
        }

        private async void frmBankReconcilation_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadLookupsAsync();
                if (cmbBanks.SelectedValue != null)
                    await FillAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bank reconciliation data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                await FillAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bank reconciliation data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvBankRec_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvBankRec.CurrentCell.ColumnIndex == clnClear.Index)
            {
                CheckBox chkClear = e.Control as CheckBox;
                if (chkClear != null)
                {
                    chkClear.CheckedChanged += new EventHandler(chkClear_CheckedChanged);
                }
            }
        }

        void chkClear_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CalcDiff()
        {
            decimal Totclear = 0, TotBal = 0;
            Totclear = (Decimal)(from DataGridViewRow rr in dgvBankRec.Rows
                                 where Convert.ToBoolean(rr.Cells[clnClear.Index].Value.ToString()) == true
                                 select GetDecimal(rr.Cells[clnDr.Index].Value) - GetDecimal(rr.Cells[clnCredit.Index].Value)).Sum();

            TotBal = (Decimal)(from DataGridViewRow rr in dgvBankRec.Rows
                               select GetDecimal(rr.Cells[clnDr.Index].Value) - GetDecimal(rr.Cells[clnCredit.Index].Value)).Sum();
            txtClear.Text = Totclear.ToString("N2");
            txtRealBal.Text = TotBal.ToString("N2");
            txtDiff.Text = (TotBal - Totclear).ToString("N2");
        }

        private void dgvBankRec_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (AllowValidate && e.RowIndex >= 0 && e.ColumnIndex == clnClear.Index)
            {
                CalcDiff();
                if ((bool)dgvBankRec[clnClear.Index, e.RowIndex].Value == true)
                {
                    dgvBankRec[clnRecDate.Index, e.RowIndex].Value = dtpTdate.Value;
                }
                else
                {
                    dgvBankRec[clnRecDate.Index, e.RowIndex].Value = null;
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Save this...!", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                var lines = new List<BankReconciliationLineSaveRequestDto>();
                foreach (DataGridViewRow item in dgvBankRec.Rows)
                {
                    if (item.Cells[clnVNo.Index].Value == null)
                        continue;

                    var clear = item.Cells[clnClear.Index].Value != null && (bool)item.Cells[clnClear.Index].Value;
                    var recDate = item.Cells[clnRecDate.Index].Value == null
                        ? null
                        : Convert.ToDateTime(item.Cells[clnRecDate.Index].Value).ToString("yyyy-MM-dd");

                    lines.Add(new BankReconciliationLineSaveRequestDto
                    {
                        VoucherNo = item.Cells[clnVNo.Index].Value.ToString(),
                        Clear = clear,
                        ReconcileDate = recDate
                    });
                }

                await _apiService.SaveAsync(lines);
                MessageBox.Show("Record Successfully Saved..!");
                await FillAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving bank reconciliation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
