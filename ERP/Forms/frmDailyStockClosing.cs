using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP.Forms
{
    public partial class frmDailyStockClosing : Form
    {
        private readonly DailyStockClosingApiService _closingService;
        private readonly ChartOfAccountApiService _accountService;
        private readonly ItemCategoryApiService _categoryService;

        private List<DailyStockClosingItemLineDto> _allItems = new List<DailyStockClosingItemLineDto>();
        private List<ChartOfAccountHeadDto> _customers = new List<ChartOfAccountHeadDto>();
        private List<ChartOfAccountHeadDto> _vendors = new List<ChartOfAccountHeadDto>();
        private List<ChartOfAccountHeadDto> _cashBanks = new List<ChartOfAccountHeadDto>();
        private List<ItemCategoryDto> _categories = new List<ItemCategoryDto>();

        private bool _isExisting = false;
        private string _purchaseVoucherNo = null;
        private string _saleVoucherNo = null;
        private bool _isLoading = false;
        private bool _suppressTotalEvents = false;
        private decimal? _manualSaleTotal = null;
        private decimal? _manualBuyTotal = null;

        public frmDailyStockClosing()
        {
            InitializeComponent();
            _closingService = new DailyStockClosingApiService();
            _accountService = new ChartOfAccountApiService();
            _categoryService = new ItemCategoryApiService();

            EnableDoubleBuffering(dgvStock);
            EnableDoubleBuffering(dgvQuery);

            dtpFromDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpToDate.Value = DateTime.Today;

            this.Shown += frmDailyStockClosing_Shown;
            this.KeyDown += frmDailyStockClosing_KeyDown;

            dtpDate.ValueChanged += dtpDate_ValueChanged;
            cmbCategory.SelectedIndexChanged += (s, e) => RefreshStockGrid();
            txtSearch.TextChanged += (s, e) => RefreshStockGrid();
            chkChangedOnly.CheckedChanged += (s, e) => RefreshStockGrid();

            dgvStock.CellEndEdit += dgvStock_CellEndEdit;
            dgvStock.DataError += (s, e) => { e.ThrowException = false; };

            txtActualSale.Leave += txtActualSale_Leave;
            txtActualBuy.Leave += txtActualBuy_Leave;

            btnNew.Click += (s, e) =>
            {
                if (dtpDate.Value.Date == DateTime.Today)
                {
                    _ = LoadSheetAsync(DateTime.Today);
                }
                else
                {
                    dtpDate.Value = DateTime.Today;
                }
            };
            btnHistory.Click += async (s, e) =>
            {
                tbClosingQuery.SelectedTab = tbQuery;
                await LoadQueryListAsync();
            };
            btnReload.Click += async (s, e) => await LoadSheetAsync(dtpDate.Value);
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;
            btnClose.Click += (s, e) => this.Close();
            btnResetSaleTotal.Click += (s, e) => { _manualSaleTotal = null; UpdateTotalsDisplay(); };
            btnResetBuyTotal.Click += (s, e) => { _manualBuyTotal = null; UpdateTotalsDisplay(); };

            btnFind.Click += async (s, e) => await LoadQueryListAsync();
            dgvQuery.CellDoubleClick += dgvQuery_CellDoubleClick;
            tbClosingQuery.SelectedIndexChanged += async (s, e) =>
            {
                if (tbClosingQuery.SelectedTab == tbQuery)
                {
                    await LoadQueryListAsync();
                }
            };
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

        private async void frmDailyStockClosing_Shown(object sender, EventArgs e)
        {
            await LoadLookupsAsync();
            await LoadSheetAsync(dtpDate.Value);
        }

        private async void frmDailyStockClosing_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                btnSave_Click(sender, EventArgs.Empty);
            }
            else if (e.KeyCode == Keys.F3)
            {
                e.Handled = true;
                dtpDate.Value = DateTime.Today;
                await LoadSheetAsync(DateTime.Today);
            }
            else if (e.KeyCode == Keys.F9)
            {
                e.Handled = true;
                tbClosingQuery.SelectedTab = tbQuery;
                await LoadQueryListAsync();
            }
        }

        private async Task LoadLookupsAsync()
        {
            try
            {
                var custTask = _accountService.GetCustomerAccountsAsync();
                var vendTask = _accountService.GetSupplierAccountsAsync();
                var cashTask = _accountService.GetCashBankAccountsAsync();
                var catTask = _categoryService.GetLookupAsync();

                await Task.WhenAll(custTask, vendTask, cashTask, catTask);

                _customers = custTask.Result ?? new List<ChartOfAccountHeadDto>();
                _vendors = vendTask.Result ?? new List<ChartOfAccountHeadDto>();
                _cashBanks = cashTask.Result ?? new List<ChartOfAccountHeadDto>();
                _categories = catTask.Result ?? new List<ItemCategoryDto>();

                cmbCustomer.DataSource = _customers.Select(c => new { c.Account, Display = $"{c.Title} ({c.Account})" }).ToList();
                cmbCustomer.ValueMember = "Account";
                cmbCustomer.DisplayMember = "Display";

                cmbVendor.DataSource = _vendors.Select(v => new { v.Account, Display = $"{v.Title} ({v.Account})" }).ToList();
                cmbVendor.ValueMember = "Account";
                cmbVendor.DisplayMember = "Display";

                var cashList = new List<object> { new { Account = "", Display = "(Default Cash Account)" } };
                cashList.AddRange(_cashBanks.Select(b => new { b.Account, Display = $"{b.Title} ({b.Account})" }));
                cmbCashAccount.DataSource = cashList;
                cmbCashAccount.ValueMember = "Account";
                cmbCashAccount.DisplayMember = "Display";

                var catList = new List<object> { new { Code = "", Title = "All Categories" } };
                catList.AddRange(_categories.Select(c => new { c.Code, c.Title }));
                cmbCategory.DataSource = catList;
                cmbCategory.ValueMember = "Code";
                cmbCategory.DisplayMember = "Title";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load lookups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            await LoadSheetAsync(dtpDate.Value);
        }

        private async Task LoadSheetAsync(DateTime date)
        {
            if (_isLoading) return;
            _isLoading = true;
            try
            {
                Cursor = Cursors.WaitCursor;
                var sheet = await _closingService.GetSheetAsync(date.ToString("yyyy-MM-dd"));

                _isExisting = sheet.IsExisting;
                _purchaseVoucherNo = sheet.PurchaseVoucherNo;
                _saleVoucherNo = sheet.SaleVoucherNo;

                lblStatus.Text = _isExisting
                    ? $"Existing Closing (PU: {_purchaseVoucherNo ?? "None"} | SL: {_saleVoucherNo ?? "None"})"
                    : "New Daily Closing";
                lblStatus.ForeColor = _isExisting ? Color.FromArgb(37, 99, 235) : Color.FromArgb(22, 163, 74);
                btnSave.Text = _isExisting ? "&Update Closing [F5]" : "&Save Closing [F5]";
                btnDelete.Enabled = _isExisting;

                if (!string.IsNullOrWhiteSpace(sheet.CustomerAccountId))
                    cmbCustomer.SelectedValue = sheet.CustomerAccountId;
                if (!string.IsNullOrWhiteSpace(sheet.VendorAccountId))
                    cmbVendor.SelectedValue = sheet.VendorAccountId;
                if (!string.IsNullOrWhiteSpace(sheet.CashAccountId))
                    cmbCashAccount.SelectedValue = sheet.CashAccountId;
                else
                    cmbCashAccount.SelectedValue = "";

                txtRemarks.Text = sheet.Remarks ?? string.Empty;

                _allItems = sheet.Items ?? new List<DailyStockClosingItemLineDto>();
                foreach (var it in _allItems)
                {
                    it.ManualPurchased = it.PurchasedQty > 0 && it.SoldQty > 0;
                }

                _manualSaleTotal = sheet.EnteredSaleAmount;
                _manualBuyTotal = sheet.EnteredPurchaseAmount;

                RefreshStockGrid();
                UpdateTotalsDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load daily stock closing sheet: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                _isLoading = false;
            }
        }

        private void RefreshStockGrid()
        {
            dgvStock.Rows.Clear();
            var catCode = cmbCategory.SelectedValue as string;
            var query = (txtSearch.Text ?? string.Empty).Trim().ToLowerInvariant();
            var changedOnly = chkChangedOnly.Checked;

            foreach (var item in _allItems)
            {
                if (!string.IsNullOrWhiteSpace(catCode) && !string.Equals(item.CategoryCode, catCode, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (changedOnly && item.PurchasedQty == 0 && item.SoldQty == 0)
                    continue;

                if (!string.IsNullOrEmpty(query))
                {
                    bool matchTitle = (item.Title ?? "").ToLowerInvariant().Contains(query);
                    bool matchKey = (item.ItemKey ?? "").ToLowerInvariant().Contains(query);
                    bool matchBarcode = (item.Barcode ?? "").ToLowerInvariant().Contains(query);
                    if (!matchTitle && !matchKey && !matchBarcode)
                        continue;
                }

                decimal calcBuy = Math.Round(item.PurchasedQty * item.PurchaseRate, 2);
                decimal calcSale = Math.Round(item.SoldQty * item.SaleRate, 2);

                int rowIdx = dgvStock.Rows.Add(
                    item.ItemId,
                    item.Title,
                    item.UnitTitle ?? "",
                    item.OpeningQty.ToString("0.##"),
                    item.ClosingQty.ToString("0.##"),
                    item.PurchasedQty.ToString("0.##"),
                    item.SoldQty.ToString("0.##"),
                    item.PurchaseRate.ToString("0.00"),
                    item.SaleRate.ToString("0.00"),
                    calcBuy.ToString("0.00"),
                    calcSale.ToString("0.00")
                );

                if (item.SoldQty > 0)
                {
                    dgvStock.Rows[rowIdx].Cells["colSoldQty"].Style.ForeColor = Color.FromArgb(22, 163, 74);
                    dgvStock.Rows[rowIdx].Cells["colSoldQty"].Style.Font = new Font(dgvStock.Font, FontStyle.Bold);
                }
                if (item.PurchasedQty > 0)
                {
                    dgvStock.Rows[rowIdx].Cells["colPurchasedQty"].Style.ForeColor = Color.FromArgb(37, 99, 235);
                    dgvStock.Rows[rowIdx].Cells["colPurchasedQty"].Style.Font = new Font(dgvStock.Font, FontStyle.Bold);
                }
            }
        }

        private void dgvStock_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvStock.Rows.Count) return;

            var row = dgvStock.Rows[e.RowIndex];
            var itemId = Convert.ToString(row.Cells["colItemId"].Value);
            var item = _allItems.FirstOrDefault(x => x.ItemId == itemId);
            if (item == null) return;

            var colName = dgvStock.Columns[e.ColumnIndex].Name;
            if (colName == "colClosingQty")
            {
                decimal.TryParse(Convert.ToString(row.Cells["colClosingQty"].Value), out decimal closingQty);
                item.ClosingQty = closingQty;

                if (item.ManualPurchased)
                {
                    item.SoldQty = Math.Max(0m, Math.Round(item.OpeningQty + item.PurchasedQty - item.ClosingQty, 4));
                }
                else
                {
                    var diff = Math.Round(item.ClosingQty - item.OpeningQty, 4);
                    item.PurchasedQty = diff > 0 ? diff : 0m;
                    item.SoldQty = diff < 0 ? Math.Abs(diff) : 0m;
                }
            }
            else if (colName == "colPurchasedQty")
            {
                decimal.TryParse(Convert.ToString(row.Cells["colPurchasedQty"].Value), out decimal purchasedQty);
                item.PurchasedQty = Math.Max(0m, purchasedQty);
                item.ManualPurchased = item.PurchasedQty > 0;
                item.SoldQty = Math.Max(0m, Math.Round(item.OpeningQty + item.PurchasedQty - item.ClosingQty, 4));
            }
            else if (colName == "colPurchaseRate")
            {
                decimal.TryParse(Convert.ToString(row.Cells["colPurchaseRate"].Value), out decimal buyRate);
                item.PurchaseRate = Math.Max(0m, buyRate);
            }
            else if (colName == "colSaleRate")
            {
                decimal.TryParse(Convert.ToString(row.Cells["colSaleRate"].Value), out decimal saleRate);
                item.SaleRate = Math.Max(0m, saleRate);
            }

            row.Cells["colClosingQty"].Value = item.ClosingQty.ToString("0.##");
            row.Cells["colPurchasedQty"].Value = item.PurchasedQty.ToString("0.##");
            row.Cells["colSoldQty"].Value = item.SoldQty.ToString("0.##");
            row.Cells["colPurchaseRate"].Value = item.PurchaseRate.ToString("0.00");
            row.Cells["colSaleRate"].Value = item.SaleRate.ToString("0.00");
            row.Cells["colCalcBuyAmt"].Value = Math.Round(item.PurchasedQty * item.PurchaseRate, 2).ToString("0.00");
            row.Cells["colCalcSaleAmt"].Value = Math.Round(item.SoldQty * item.SaleRate, 2).ToString("0.00");

            UpdateTotalsDisplay();
        }

        private void txtActualSale_Leave(object sender, EventArgs e)
        {
            if (_suppressTotalEvents) return;
            if (decimal.TryParse(txtActualSale.Text, out decimal val) && val >= 0)
                _manualSaleTotal = Math.Round(val, 2);
            else
                _manualSaleTotal = null;
            UpdateTotalsDisplay();
        }

        private void txtActualBuy_Leave(object sender, EventArgs e)
        {
            if (_suppressTotalEvents) return;
            if (decimal.TryParse(txtActualBuy.Text, out decimal val) && val >= 0)
                _manualBuyTotal = Math.Round(val, 2);
            else
                _manualBuyTotal = null;
            UpdateTotalsDisplay();
        }

        private void UpdateTotalsDisplay()
        {
            _suppressTotalEvents = true;
            try
            {
                decimal calcBuy = Math.Round(_allItems.Sum(x => Math.Round(x.PurchasedQty * x.PurchaseRate, 2)), 2);
                decimal calcSale = Math.Round(_allItems.Sum(x => Math.Round(x.SoldQty * x.SaleRate, 2)), 2);

                decimal effectiveBuy = _manualBuyTotal ?? calcBuy;
                decimal effectiveSale = _manualSaleTotal ?? calcSale;

                decimal buyVariance = Math.Round(effectiveBuy - calcBuy, 2);
                decimal saleDiscount = Math.Round(calcSale - effectiveSale, 2);

                txtCalcSaleVal.Text = calcSale.ToString("N2");
                txtActualSale.Text = effectiveSale.ToString("0.00");
                txtSaleVarianceVal.Text = saleDiscount.ToString("N2");

                txtCalcBuyVal.Text = calcBuy.ToString("N2");
                txtActualBuy.Text = effectiveBuy.ToString("0.00");
                txtBuyVarianceVal.Text = buyVariance.ToString("N2");
            }
            finally
            {
                _suppressTotalEvents = false;
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var customerAcc = Convert.ToString(cmbCustomer.SelectedValue);
                var vendorAcc = Convert.ToString(cmbVendor.SelectedValue);
                var cashAcc = Convert.ToString(cmbCashAccount.SelectedValue);

                if (string.IsNullOrWhiteSpace(customerAcc))
                {
                    MessageBox.Show("Please select a Default Walk-In Customer account.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(vendorAcc))
                {
                    MessageBox.Show("Please select a Default Supplier account.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var changedLines = _allItems.Where(x => x.PurchasedQty > 0 || x.SoldQty > 0).ToList();
                if (changedLines.Count == 0 && !_isExisting)
                {
                    MessageBox.Show("No stock count changes detected. Enter closing stock counts or purchased quantities first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal.TryParse(txtActualSale.Text, out decimal enteredSale);
                decimal.TryParse(txtActualBuy.Text, out decimal enteredBuy);

                Cursor = Cursors.WaitCursor;
                btnSave.Enabled = false;

                var res = await _closingService.SaveAsync(new DailyStockClosingSaveRequestDto
                {
                    Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                    CustomerAccountId = customerAcc,
                    VendorAccountId = vendorAcc,
                    CashAccountId = string.IsNullOrWhiteSpace(cashAcc) ? null : cashAcc,
                    ReconciliationMode = "DiscountAddLess",
                    EnteredSaleAmount = enteredSale,
                    EnteredPurchaseAmount = enteredBuy,
                    Remarks = txtRemarks.Text,
                    Lines = changedLines.Select(l => new DailyStockClosingSaveLineRequestDto
                    {
                        ItemId = l.ItemId,
                        OpeningQty = l.OpeningQty,
                        ClosingQty = l.ClosingQty,
                        PurchasedQty = l.PurchasedQty,
                        SoldQty = l.SoldQty,
                        PurchaseRate = l.PurchaseRate,
                        SaleRate = l.SaleRate
                    }).ToList()
                });

                MessageBox.Show(
                    $"Daily Stock Closing saved successfully!\n\nPurchase Voucher: {(string.IsNullOrEmpty(res.PurchaseVoucherNo) ? "None" : "PU-" + res.PurchaseVoucherNo)}\nSale Voucher: {(string.IsNullOrEmpty(res.SaleVoucherNo) ? "None" : "SL-" + res.SaleVoucherNo)}",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                await LoadSheetAsync(dtpDate.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save daily stock closing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async Task LoadQueryListAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var list = await _closingService.GetListAsync(
                    dtpFromDate.Value.ToString("yyyy-MM-dd"),
                    dtpToDate.Value.ToString("yyyy-MM-dd"));

                dgvQuery.Rows.Clear();
                foreach (var item in list)
                {
                    dgvQuery.Rows.Add(
                        item.Date.ToString("yyyy-MM-dd"),
                        string.IsNullOrEmpty(item.PurchaseVoucherNo) ? "None" : "PU-" + item.PurchaseVoucherNo,
                        item.PurchaseAmount.ToString("N2"),
                        string.IsNullOrEmpty(item.SaleVoucherNo) ? "None" : "SL-" + item.SaleVoucherNo,
                        item.SaleAmount.ToString("N2"),
                        item.Remarks ?? "",
                        item.CreatedBy ?? ""
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load closing history: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void dgvQuery_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvQuery.Rows.Count) return;
            var dateStr = Convert.ToString(dgvQuery.Rows[e.RowIndex].Cells["colQDate"].Value);
            if (DateTime.TryParse(dateStr, out DateTime parsedDate))
            {
                tbClosingQuery.SelectedTab = tbDetail;
                if (dtpDate.Value.Date == parsedDate.Date)
                {
                    _ = LoadSheetAsync(parsedDate);
                }
                else
                {
                    dtpDate.Value = parsedDate;
                }
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!_isExisting) return;
            var confirm = MessageBox.Show(
                "Are you sure you want to delete the Daily Stock Closing for this date? This will delete the linked Purchase and Sale vouchers.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                await _closingService.DeleteAsync(dtpDate.Value.ToString("yyyy-MM-dd"));
                MessageBox.Show("Daily Stock Closing deleted.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadSheetAsync(dtpDate.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete daily stock closing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
