using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmOpeningBal : Form
    {
        private readonly OpeningBalanceApiService _openingBalanceApiService;
        private bool _isSaving;

        public frmOpeningBal()
        {
            InitializeComponent();
            _openingBalanceApiService = new OpeningBalanceApiService();
        }

        internal string ParentAcc = null;

        private async System.Threading.Tasks.Task FillFormAsync()
        {
            var data = await _openingBalanceApiService.GetAsync(ParentAcc);
            dgvOpenning.Rows.Clear();
            for (int i = 0; i < data.Count; i++)
            {
                dgvOpenning.Rows.Add(
                    data[i].ParentCode,
                    data[i].Code,
                    data[i].Title,
                    data[i].Bal > 0 ? data[i].Bal.ToString("N2") : null,
                    data[i].Bal < 0 ? (-data[i].Bal).ToString("N2") : null);
            }
        }

        void CalcTotals()
        {
            decimal totDr = 0, totCr = 0;
            try
            {
                totDr = (from DataGridViewRow row in dgvOpenning.Rows
                         where row.Cells[clnDr.Index].Value != null
                         select decimal.Parse(row.Cells[clnDr.Index].Value.ToString())).Sum();
            }
            catch { }

            try
            {
                totCr = (from DataGridViewRow row in dgvOpenning.Rows
                         where row.Cells[clnCr.Index].Value != null
                         select decimal.Parse(row.Cells[clnCr.Index].Value.ToString())).Sum();
            }
            catch { }

            lblTotDr.Text = totDr.ToString();
            lblTotCr.Text = totCr.ToString();
        }

        private async void frmOpeningBal_Load(object sender, EventArgs e)
        {
            try
            {
                await FillFormAsync();
                CalcTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnOk_Click(object sender, EventArgs e)
        {
            if (_isSaving)
                return;

            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to save this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _isSaving = true;
            btnOk.Enabled = false;
            try
            {
                foreach (DataGridViewRow item in dgvOpenning.Rows)
                {
                    var account = item.Cells[clnAccount.Index].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(account))
                        continue;

                    decimal? dr = null, cr = null;

                    if (item.Cells[clnDr.Index].Value != null &&
                        decimal.TryParse(item.Cells[clnDr.Index].Value.ToString(), out var drVal))
                        dr = drVal;

                    if (item.Cells[clnCr.Index].Value != null &&
                        decimal.TryParse(item.Cells[clnCr.Index].Value.ToString(), out var crVal))
                        cr = crVal;

                    await _openingBalanceApiService.UpsertAsync(account, dr, cr);
                }

                MessageBox.Show("Record Successfully Saved..!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isSaving = false;
                btnOk.Enabled = true;
            }
        }

        private void dgvOpenning_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CalcTotals();
        }
    }
}
