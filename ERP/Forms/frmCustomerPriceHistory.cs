using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP.Forms
{
    public partial class frmCustomerPriceHistory : Form
    {
        private readonly SaleApiService _apiService;
        private readonly string _accountId;
        private readonly string _customerName;
        private readonly string _itemId;
        private readonly string _itemTitle;
        private readonly bool _hasSecondaryQty;

        public decimal SelectedRate { get; private set; }
        public decimal? SelectedSecRate { get; private set; }

        public frmCustomerPriceHistory(
            string accountId,
            string customerName,
            string itemId,
            string itemTitle,
            bool hasSecondaryQty = false)
        {
            InitializeComponent();
            _apiService = new SaleApiService();
            _accountId = accountId;
            _customerName = customerName;
            _itemId = itemId;
            _itemTitle = itemTitle;
            _hasSecondaryQty = hasSecondaryQty;
        }

        private async void frmCustomerPriceHistory_Load(object sender, EventArgs e)
        {
            lblCustomer.Text = $"Customer: {_customerName ?? _accountId ?? "(Not Selected)"}";
            lblItem.Text = $"Item: {_itemTitle ?? _itemId ?? "(Not Selected)"}";

            clnSecRate.Visible = _hasSecondaryQty;

            if (string.IsNullOrWhiteSpace(_accountId) || string.IsNullOrWhiteSpace(_itemId))
            {
                MessageBox.Show(
                    "Customer and Item must both be selected to view price history.",
                    "Information Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Close();
                return;
            }

            await LoadHistoryAsync();
        }

        private async Task LoadHistoryAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnApply.Enabled = false;
                btnApplyLastRate.Enabled = false;

                var history = await _apiService.GetCustomerItemHistoryAsync(_accountId, _itemId);

                if (history == null || !history.HasHistory)
                {
                    lblLastRate.Text = "No previous sales";
                    lblLastDate.Text = "Date: -";
                    lblLastQty.Text = "Qty: -";
                    lblLastVoucher.Text = "Voucher: -";
                    dgvHistory.Rows.Clear();
                    MessageBox.Show(
                        "No prior sale or supply history found for this customer and item.",
                        "No History",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Populate last sale highlight
                var lastRate = history.LastRate ?? 0;
                lblLastRate.Text = $"Rs. {lastRate.ToString("N2")}";
                if (_hasSecondaryQty && history.LastSecRate.HasValue)
                {
                    lblLastRate.Text += $" (Pack: Rs. {history.LastSecRate.Value.ToString("N2")})";
                }

                var dtStr = history.LastSaleDate.HasValue
                    ? history.LastSaleDate.Value.ToString("dd-MMM-yyyy")
                    : "-";
                lblLastDate.Text = $"Date: {dtStr}";
                lblLastQty.Text = $"Qty: {history.LastQty ?? 0}";

                var vPrefix = !string.IsNullOrWhiteSpace(history.LastVType) ? $"{history.LastVType}-" : "";
                lblLastVoucher.Text = $"Voucher: {vPrefix}{history.LastVoucherNo}";

                btnApplyLastRate.Enabled = true;

                // Populate recent sales grid
                dgvHistory.Rows.Clear();
                foreach (var rec in history.RecentSales)
                {
                    var prefix = !string.IsNullOrWhiteSpace(rec.VType) ? $"{rec.VType}-" : "";
                    var fullVoucher = $"{prefix}{rec.VoucherNo}";

                    int idx = dgvHistory.Rows.Add(
                        rec.Date.ToString("dd-MMM-yyyy"),
                        fullVoucher,
                        rec.Qty.ToString("0.##"),
                        rec.Rate.ToString("N2"),
                        rec.SecRate.HasValue ? rec.SecRate.Value.ToString("N2") : "-",
                        rec.Amount.ToString("N2")
                    );

                    dgvHistory.Rows[idx].Tag = rec;
                }

                if (dgvHistory.Rows.Count > 0)
                {
                    dgvHistory.Rows[0].Selected = true;
                    btnApply.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load price history: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnApplyLastRate_Click(object sender, EventArgs e)
        {
            if (dgvHistory.Rows.Count > 0 && dgvHistory.Rows[0].Tag is CustomerItemSaleRecord firstRec)
            {
                ApplyRecord(firstRec);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (dgvHistory.SelectedRows.Count > 0 && dgvHistory.SelectedRows[0].Tag is CustomerItemSaleRecord rec)
            {
                ApplyRecord(rec);
            }
            else
            {
                MessageBox.Show("Please select a transaction row to apply its rate.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvHistory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvHistory.Rows[e.RowIndex].Tag is CustomerItemSaleRecord rec)
            {
                ApplyRecord(rec);
            }
        }

        private void ApplyRecord(CustomerItemSaleRecord rec)
        {
            SelectedRate = rec.Rate;
            SelectedSecRate = rec.SecRate;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
