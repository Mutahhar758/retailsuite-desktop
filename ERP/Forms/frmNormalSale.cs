using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmNormalSale : Form
    {
        private readonly SaleApiService _apiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly NarrationApiService _narrationApiService;
        private readonly ItemCategoryApiService _itemCategoryApiService;
        private readonly UnitApiService _unitApiService;
        private readonly InventoryApiService _inventoryApiService;
        private readonly MobileShopApiService _mobileShopApiService;
        private List<SaleDto> _queryList = new List<SaleDto>();

        private DataTable dtItems = new DataTable();
        private DataTable dtUnits = new DataTable();
        private readonly DataTable dtCategories = new DataTable();
        private readonly DataTable dtAccounts = new DataTable();
        private readonly DataTable dtNarration = new DataTable();

        private int currentRow;
        private bool resetRow = false;
        private bool FLogIn = true;
        private int VoucherIndex = 0;
        private string VoucherNum = null;
        private bool _isSaving;
        private readonly Dictionary<string, decimal> _minSalePriceMap = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, (decimal PriRate, decimal? SecRate)> _categoryPriceMap = new Dictionary<string, (decimal, decimal?)>(StringComparer.OrdinalIgnoreCase);
        private string _originalAccountId = null;

        private class SavedRowRateInfo
        {
            public decimal Rate { get; set; }
            public decimal SecRate { get; set; }
        }

        enum Navigators { Up, Down, Home, End }

        public frmNormalSale()
        {
            InitializeComponent();
            _apiService = new SaleApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _narrationApiService = new NarrationApiService();
            _itemCategoryApiService = new ItemCategoryApiService();
            _unitApiService = new UnitApiService();
            _inventoryApiService = new InventoryApiService();
            _mobileShopApiService = new MobileShopApiService();
            InitializeLookupTables();
            dgvSale.CellFormatting += dgvSale_CellFormattingMinSalePrice;
            dgvSale.Rows.Add();
            SetupGridContextMenu();
            UserInfo.ApplyFormPermissions(this, AppResource.Sales);
        }

        private void SetupGridContextMenu()
        {
            var ctx = new ContextMenuStrip();
            var mnuHistory = new ToolStripMenuItem("Customer Price History (F9)", null, (s, e) => ShowCustomerPriceHistory());
            ctx.Items.Add(mnuHistory);
            dgvSale.ContextMenuStrip = ctx;
        }

        private void SetupF9Hints()
        {
            try
            {
                if (clnRate != null)
                {
                    clnRate.ToolTipText = "Press [F9] to view customer's last sale price & history";
                }

                if (tbDetail != null)
                {
                    var pnlHint = new Panel
                    {
                        Name = "pnlPriceHistoryHint",
                        Location = new System.Drawing.Point(885, 12),
                        Size = new System.Drawing.Size(190, 82),
                        BackColor = System.Drawing.Color.FromArgb(239, 246, 255),
                        BorderStyle = BorderStyle.FixedSingle,
                        Cursor = Cursors.Hand
                    };

                    var lblTitle = new Label
                    {
                        Text = "💡 Price History [F9]",
                        Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
                        ForeColor = System.Drawing.Color.FromArgb(30, 64, 175),
                        Location = new System.Drawing.Point(6, 6),
                        AutoSize = true,
                        Cursor = Cursors.Hand
                    };

                    var lblDesc = new Label
                    {
                        Text = "Press [F9] or Right-Click on any item row to check Customer's last sale price.",
                        Font = new System.Drawing.Font("Segoe UI", 8.25F),
                        ForeColor = System.Drawing.Color.FromArgb(51, 65, 85),
                        Location = new System.Drawing.Point(6, 26),
                        Size = new System.Drawing.Size(175, 50),
                        Cursor = Cursors.Hand
                    };

                    Action openHistory = () => ShowCustomerPriceHistory();
                    pnlHint.Click += (s, e) => openHistory();
                    lblTitle.Click += (s, e) => openHistory();
                    lblDesc.Click += (s, e) => openHistory();

                    pnlHint.Controls.Add(lblTitle);
                    pnlHint.Controls.Add(lblDesc);

                    tbDetail.Controls.Add(pnlHint);
                    pnlHint.BringToFront();
                }
            }
            catch { }
        }

        private void InitializeLookupTables()
        {
            dtItems.Columns.Add("Id", typeof(string));
            dtItems.Columns.Add("Title", typeof(string));
            dtItems.Columns.Add("fkItemcatagory", typeof(string));
            dtItems.Columns.Add("ItemKey", typeof(string));
            dtItems.Columns.Add("Barcode", typeof(string));
            dtItems.Columns.Add("PriRate", typeof(decimal));
            dtItems.Columns.Add("SecRate", typeof(decimal));
            dtItems.Columns.Add("PrimaryUnit", typeof(string));
            dtItems.Columns.Add("SecondaryUnit", typeof(string));
            dtItems.Columns.Add("DefaultUnit", typeof(string));
            dtItems.Columns.Add("RequireImei", typeof(bool));
            dtItems.Columns.Add("AdditionalUnits", typeof(string));

            dtUnits.Columns.Add("Code", typeof(string));
            dtUnits.Columns.Add("Title", typeof(string));
            dtUnits.Columns.Add("ShortTitle", typeof(string));
            dtUnits.Columns.Add("ParentUnitId", typeof(string));
            dtUnits.Columns.Add("ConversionRatio", typeof(decimal));

            dtCategories.Columns.Add("Code", typeof(string));
            dtCategories.Columns.Add("Title", typeof(string));

            dtAccounts.Columns.Add("Account", typeof(string));
            dtAccounts.Columns.Add("Title", typeof(string));
            dtAccounts.Columns.Add("CustomerCategoryId", typeof(string));

            dtNarration.Columns.Add("Code", typeof(string));
            dtNarration.Columns.Add("Title", typeof(string));
        }

        private async System.Threading.Tasks.Task LoadLookupsAsync()
        {
            var accountsTask = _chartOfAccountApiService.GetCustomerAccountsAsync();
            var narrationsTask = _narrationApiService.GetLookupAsync();
            var categoriesTask = _itemCategoryApiService.GetLookupAsync();
            var unitsTask = _unitApiService.GetLookupAsync();
            var itemsTask = _inventoryApiService.GetLookupAsync(null);
            var categoryPricesTask = _inventoryApiService.GetCustomerCategoryPricesAsync();
            await System.Threading.Tasks.Task.WhenAll(accountsTask, narrationsTask, categoriesTask, unitsTask, itemsTask, categoryPricesTask);

            _categoryPriceMap.Clear();
            foreach (var cp in categoryPricesTask.Result)
            {
                if (!string.IsNullOrWhiteSpace(cp.ItemId) && !string.IsNullOrWhiteSpace(cp.CustomerCategoryId))
                {
                    _categoryPriceMap[$"{cp.ItemId}_{cp.CustomerCategoryId}"] = (cp.PriRate, cp.SecRate);
                }
            }

            _minSalePriceMap.Clear();
            foreach (var item in itemsTask.Result)
            {
                if (!string.IsNullOrWhiteSpace(item.Id) && item.MinSalePrice.HasValue && item.MinSalePrice.Value > 0)
                    _minSalePriceMap[item.Id] = item.MinSalePrice.Value;
            }

            dtItems.Rows.Clear();
            foreach (var item in itemsTask.Result)
                dtItems.Rows.Add(item.Id, item.Title, item.ItemCategoryCode, item.ItemKey, item.Barcode, item.PriRate, item.SecRate, item.PrimaryUnit, item.SecondaryUnit, item.DefaultUnit, item.RequireImei == true, item.AdditionalUnits);

            dtUnits = UnitConversionHelper.CreateUnitsTable(unitsTask.Result);

            dtCategories.Rows.Clear();
            foreach (var c in categoriesTask.Result)
                dtCategories.Rows.Add(c.Code, c.Title);

            dtAccounts.Rows.Clear();
            foreach (var a in accountsTask.Result)
                dtAccounts.Rows.Add(a.Account, a.Title, a.CustomerCategoryId);

            dtNarration.Rows.Clear();
            foreach (var n in narrationsTask.Result)
                dtNarration.Rows.Add(n.Code, n.Title);

            FillAccounts();
            FillNarration();
            FillCategories();
            FillItems();
            FillFilterAccounts();
            UnitConversionHelper.UpdateUnitColumnVisibility(dgvSale, clnUnit, clnQty, dtItems, dtUnits);
        }

        private void FillAccounts()
        {
            cmbAccounts.DataSource = dtAccounts.Copy();
            cmbAccounts.DisplayMember = "Title";
            cmbAccounts.ValueMember = "Account";
            cmbAccounts.SelectedIndex = -1;
        }

        private void FillFilterAccounts()
        {
            cmbFilterAccounts.DataSource = dtAccounts.Copy();
            cmbFilterAccounts.DisplayMember = "Title";
            cmbFilterAccounts.ValueMember = "Account";
            cmbFilterAccounts.SelectedIndex = -1;
        }

        private void FillNarration()
        {
            cmbNarration.DataSource = dtNarration.Copy();
            cmbNarration.DisplayMember = "Title";
            cmbNarration.ValueMember = "Code";
            cmbNarration.SelectedIndex = -1;
        }

        private void FillCategories()
        {
        }

        private void FillItems()
        {
            clnItemNo.DataSource = dtItems.Copy();
            clnItemNo.DisplayMember = "Title";
            clnItemNo.ValueMember = "Id";
        }

        private async System.Threading.Tasks.Task FillQueryAsync(string fromDate = "", string toDate = "", string account = "", string voucherNo = "")
        {
            dgvQuery.Rows.Clear();
            _queryList = await _apiService.GetListAsync(fromDate, toDate, account, voucherNo);

            for (int i = 0; i < _queryList.Count; i++)
            {
                var row = _queryList[i];
                dgvQuery.Rows.Add(
                    row.Date.ToString("dd-MMM-yyyy"),
                    "SL-" + row.VoucherNo,
                    row.Account,
                    row.CreatedBy + " | " + row.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                    !string.IsNullOrWhiteSpace(row.LastModifiedBy)
                        ? row.LastModifiedBy + " | " + row.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                        : null);
            }
        }

        internal async System.Threading.Tasks.Task FillSaleAsync(string vno)
        {
            var lines = await _apiService.GetDetailAsync(vno);
            txtVoucherNo.Text = "SL-" + vno;
            VoucherNum = vno;
            dgvSale.Rows.Clear();

            if (lines.Count > 0)
            {
                var first = lines[0];
                _originalAccountId = first.AccountId;
                dtpDate.Value = first.Date;
                if (string.IsNullOrWhiteSpace(first.Narration))
                    cmbNarration.SelectedIndex = -1;
                else
                    cmbNarration.SelectedValue = first.Narration;
                cmbAccounts.SelectedValue = first.AccountId;
                txtDescription.Text = first.Description;
                txtCreatedBy.Text = first.CreatedBy + " | " + first.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt");
                txtEditBy.Text = !string.IsNullOrWhiteSpace(first.LastModifiedBy)
                    ? first.LastModifiedBy + " | " + first.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                    : null;
                txtCashReceipt.Text = first.CashReceipt.ToString("N2");
                txtCashBack.Text = first.CashBack.ToString("N2");

                for (int i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    decimal discountPercent = line.Rate == 0 ? 0 : (line.Discount / line.Rate) * 100;
                    
                    int ind = dgvSale.Rows.Add();
                    var row = dgvSale.Rows[ind];
                    
                    row.Cells[clnSeq.Index].Value = line.Seq.ToString();
                    row.Cells[clnItemKey.Index].Value = line.ItemKey;
                    row.Cells[clnItemNo.Index].Value = null;
                    row.Cells[clnUnit.Index].Value = null;
                    row.Cells[clnRate.Index].Value = line.Rate.ToString("0.##");
                    row.Cells[clnDiscount.Index].Value = line.Discount.ToString("0.##");
                    row.Cells[clnDiscPercent.Index].Value = discountPercent.ToString("0.##");
                    if (clnCarriage != null)
                        row.Cells[clnCarriage.Index].Value = (line.Carriage ?? 0).ToString("0.##");
                    row.Cells[clnAmount.Index].Value = line.Amount.ToString("N2");
                    row.Cells[clnStatus.Index].Value = "0";


                    SetcmbItemSource(line.ItemCategoryCode, ind);
                    dgvSale[clnItemNo.Index, ind].Value = line.ItemId;

                    DataRow itemRow = dtItems.Select("Id = '" + line.ItemId.Replace("'", "''") + "'").FirstOrDefault();
                    decimal displayQty = line.Qty;
                    string displayUnitId = line.Unit;
                    if (itemRow != null)
                    {
                        DataRowView drv = dtItems.DefaultView[dtItems.Rows.IndexOf(itemRow)];
                        SetcmbUnitSource(drv, ind);
                        UnitConversionHelper.ResolveDisplayQtyAndUnit(line.Qty, itemRow, dtUnits, out displayQty, out displayUnitId);
                        UnitConversionHelper.BindRowUnitComboBox(row, clnUnit.Index, itemRow, dtUnits, displayUnitId);
                    }
                    else
                    {
                        SetAllUnitsSource(ind);
                        dgvSale[clnUnit.Index, ind].Value = line.Unit;
                    }

                    row.Cells[clnQty.Index].Value = displayQty.ToString("0.####");
                    dgvSale[clnRate.Index, ind].Value = line.Rate.ToString("0.##");

                    if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                    {
                        dgvSale["clnSecQty", ind].Value = (line.SecQty ?? 0).ToString("0.##");
                        dgvSale["clnSecRate", ind].Value = (line.SecRate ?? 0).ToString("0.##");
                    }

                    if (ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnImei"))
                    {
                        dgvSale["clnImei", ind].Value = line.Imei;
                        dgvSale["clnImei2", ind].Value = line.Imei2;
                        dgvSale["clnPtaStatus", ind].Value = line.PtaStatus;
                        dgvSale["clnWarrantyMonths", ind].Value = line.WarrantyMonths?.ToString();
                        dgvSale["clnBatteryHealth", ind].Value = line.BatteryHealth?.ToString();
                        dgvSale["clnConditionNote", ind].Value = line.ConditionNote;
                    }

                    row.Tag = new SavedRowRateInfo
                    {
                        Rate = line.Rate,
                        SecRate = line.SecRate ?? 0
                    };
                }
            }

            dgvSale.Rows.Add();
            CalcTotAmount();
        }

        internal void FillSale(string vno)
        {
            _ = FillSaleAsync(vno);
        }

        /// <summary>
        /// Highlights the rate cell (with a tooltip) when the selling rate is at or below the
        /// minimum sale price defined on the item.
        /// </summary>
        private void dgvSale_CellFormattingMinSalePrice(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != clnRate.Index || e.RowIndex >= dgvSale.Rows.Count)
                return;

            var row = dgvSale.Rows[e.RowIndex];
            if (row.IsNewRow)
                return;

            string itemId = Convert.ToString(row.Cells[clnItemNo.Index].Value);
            var cell = row.Cells[e.ColumnIndex];
            if (!string.IsNullOrWhiteSpace(itemId)
                && _minSalePriceMap.TryGetValue(itemId, out decimal minPrice)
                && ParseDecimal(cell.Value) <= minPrice)
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 235, 156);
                e.CellStyle.ForeColor = Color.FromArgb(156, 87, 0);
                e.CellStyle.SelectionBackColor = Color.FromArgb(255, 192, 0);
                cell.ToolTipText = "Rate is at the minimum sale price (" + minPrice.ToString("0.##") + ")";
            }
            else
            {
                cell.ToolTipText = clnRate.ToolTipText;
            }
        }
        private static decimal ParseDecimal(object value)
        {
            decimal parsed;
            return decimal.TryParse(Convert.ToString(value), out parsed) ? parsed : 0;
        }

        private void CalcTotAmount()
        {
            decimal totAmount = 0;
            try
            {
                totAmount = (from DataGridViewRow row in dgvSale.Rows
                             where row.Cells[clnAmount.Index].Value != null
                             select ParseDecimal(row.Cells[clnAmount.Index].Value)).Sum();
            }
            catch { }

            txtTotAmount.Text = totAmount.ToString("N2");
        }

        private async System.Threading.Tasks.Task SaveAsync(bool shouldPrint = false)
        {
            dgvSale.EndEdit();
            if (cmbAccounts.SelectedValue == null)
            {
                MessageBox.Show("Please Select Account...!");
                return;
            }

            var lines = new List<SaleLineRequest>();
            foreach (DataGridViewRow row in dgvSale.Rows)
            {
                if (row.Cells[clnItemNo.Index].Value == null)
                    continue;

                decimal secQty = 0;
                decimal secRate = 0;
                string secUnit = null;
                string itemId = Convert.ToString(row.Cells[clnItemNo.Index].Value);
                DataRow itemRow = dtItems.Select("Id = '" + itemId.Replace("'", "''") + "'").FirstOrDefault();
                string imei = ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnImei") ? Convert.ToString(row.Cells["clnImei"].Value) : null;
                bool reqImei = itemRow != null && dtItems.Columns.Contains("RequireImei") && itemRow["RequireImei"] != DBNull.Value && Convert.ToBoolean(itemRow["RequireImei"]);
                if (reqImei && string.IsNullOrWhiteSpace(imei))
                {
                    MessageBox.Show($"Item '{itemRow["Title"]}' requires an IMEI / Serial number.", "IMEI Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                {
                    secQty = ParseDecimal(row.Cells["clnSecQty"].Value);
                    secRate = ParseDecimal(row.Cells["clnSecRate"].Value);
                    
                    if (itemRow != null)
                    {
                        secUnit = Convert.ToString(itemRow["SecondaryUnit"]);
                    }
                }

                decimal enteredQty = ParseDecimal(row.Cells[clnQty.Index].Value);
                string selectedUnitId = Convert.ToString(row.Cells[clnUnit.Index].Value);
                decimal baseQty = UnitConversionHelper.ConvertToBasePrimaryQty(enteredQty, selectedUnitId, itemRow, dtUnits);

                lines.Add(new SaleLineRequest
                {
                    Seq = int.Parse(Convert.ToString(row.Cells[clnSeq.Index].Value)),
                    ItemId = Convert.ToString(row.Cells[clnItemNo.Index].Value),
                    Qty = baseQty,
                    Rate = ParseDecimal(row.Cells[clnRate.Index].Value),
                    Discount = ParseDecimal(row.Cells[clnDiscount.Index].Value),
                    Carriage = clnCarriage != null ? ParseDecimal(row.Cells[clnCarriage.Index].Value) : 0,
                    SecQty = secQty,
                    SecRate = secRate,
                    SecUnit = secUnit,
                    Imei = ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnImei") ? Convert.ToString(row.Cells["clnImei"].Value) : null,
                    Imei2 = ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnImei2") ? Convert.ToString(row.Cells["clnImei2"].Value) : null,
                    PtaStatus = ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnPtaStatus") ? Convert.ToString(row.Cells["clnPtaStatus"].Value) : null,
                    WarrantyMonths = ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnWarrantyMonths") && int.TryParse(Convert.ToString(row.Cells["clnWarrantyMonths"].Value), out int wm) ? (int?)wm : null,
                    BatteryHealth = ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnBatteryHealth") && int.TryParse(Convert.ToString(row.Cells["clnBatteryHealth"].Value), out int bh) ? (int?)bh : null,
                    ConditionNote = ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnConditionNote") ? Convert.ToString(row.Cells["clnConditionNote"].Value) : null
                });
            }

            try
            {
                string voucher = txtVoucherNo.Text != "" ? txtVoucherNo.Text.Substring(3) : null;
                bool isNew = string.IsNullOrWhiteSpace(voucher);

                if (isNew)
                {
                    var createRequest = new SaleCreateRequest
                    {
                        Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                        Account = cmbAccounts.SelectedValue.ToString(),
                        Description = txtDescription.Text,
                        Narration = cmbNarration.SelectedValue?.ToString(),
                        CashReceipt = txtCashReceipt.Value,
                        CashBack = txtCashBack.Value,
                        Lines = lines
                    };
                    voucher = await _apiService.CreateAsync(createRequest);
                }
                else
                {
                    var updateRequest = new SaleUpdateRequest
                    {
                        Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                        Account = cmbAccounts.SelectedValue.ToString(),
                        Description = txtDescription.Text,
                        Narration = cmbNarration.SelectedValue?.ToString(),
                        CashReceipt = txtCashReceipt.Value,
                        CashBack = txtCashBack.Value,
                        Lines = lines
                    };
                    await _apiService.UpdateAsync(voucher, updateRequest);
                }

                await FillSaleAsync(voucher);
                await FillQueryAsync();
                VoucherIndex = _queryList.FindIndex(x => x.VoucherNo == voucher);
                if (VoucherIndex < 0)
                    VoucherIndex = 0;

                if (shouldPrint)
                {
                    Print(true);
                }

                MessageBox.Show("Record Successfully Saved...!");
                btnNew.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving sale: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rdb_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
                FillAccounts();
        }

        private void AllowNewRow()
        {
            if (dgvSale.CurrentRow.Index == dgvSale.Rows[dgvSale.Rows.Count - 1].Index)
            {
                if (dgvSale.CurrentRow.Cells[clnItemNo.Index].Value != null)
                    dgvSale.Rows.Add();
            }
        }

        private void SetupSecondaryQtyColumns()
        {
            clnUnit.Visible = false;

            if (ApiSession.HasSecondaryQty)
            {
                clnQty.HeaderText = "Single Qty";
                clnRate.HeaderText = "Single Rate";

                if (!dgvSale.Columns.Contains("clnSecQty"))
                {
                    var colSecQty = new DataGridViewTextBoxColumn
                    {
                        Name = "clnSecQty",
                        HeaderText = "Pack Qty",
                        Width = 80,
                        DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                    };
                    var colSecRate = new DataGridViewTextBoxColumn
                    {
                        Name = "clnSecRate",
                        HeaderText = "Pack Rate",
                        Width = 80,
                        DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                    };
                    dgvSale.Columns.Add(colSecQty);
                    dgvSale.Columns.Add(colSecRate);
                }
                else
                {
                    dgvSale.Columns["clnSecQty"].Visible = true;
                    dgvSale.Columns["clnSecRate"].Visible = true;
                }

                int dIdx = 0;
                clnSeq.DisplayIndex = dIdx++;
                clnItemKey.DisplayIndex = dIdx++;
                clnItemNo.DisplayIndex = dIdx++;
                clnQty.DisplayIndex = dIdx++;
                if (dgvSale.Columns.Contains("clnSecQty")) dgvSale.Columns["clnSecQty"].DisplayIndex = dIdx++;
                clnRate.DisplayIndex = dIdx++;
                if (dgvSale.Columns.Contains("clnSecRate")) dgvSale.Columns["clnSecRate"].DisplayIndex = dIdx++;
                clnDiscount.DisplayIndex = dIdx++;
                clnDiscPercent.DisplayIndex = dIdx++;
                if (clnCarriage != null && ApiSession.EnableCarriage)
                {
                    clnCarriage.Visible = true;
                    clnCarriage.DisplayIndex = dIdx++;
                }
                else if (clnCarriage != null)
                {
                    clnCarriage.Visible = false;
                }
                clnAmount.DisplayIndex = dIdx++;
                if (clnStatus != null) clnStatus.DisplayIndex = dIdx++;
            }
            else
            {
                clnQty.HeaderText = "Qty";
                clnRate.HeaderText = "Rate";

                if (dgvSale.Columns.Contains("clnSecQty"))
                    dgvSale.Columns["clnSecQty"].Visible = false;
                if (dgvSale.Columns.Contains("clnSecRate"))
                    dgvSale.Columns["clnSecRate"].Visible = false;

                int dIdx = 0;
                clnSeq.DisplayIndex = dIdx++;
                clnItemKey.DisplayIndex = dIdx++;
                clnItemNo.DisplayIndex = dIdx++;
                clnQty.DisplayIndex = dIdx++;
                clnRate.DisplayIndex = dIdx++;
                clnDiscount.DisplayIndex = dIdx++;
                clnDiscPercent.DisplayIndex = dIdx++;
                if (clnCarriage != null && ApiSession.EnableCarriage)
                {
                    clnCarriage.Visible = true;
                    clnCarriage.DisplayIndex = dIdx++;
                }
                else if (clnCarriage != null)
                {
                    clnCarriage.Visible = false;
                }
                clnAmount.DisplayIndex = dIdx++;
                if (clnStatus != null) clnStatus.DisplayIndex = dIdx++;
            }
        }

        private void SetupMobileShopColumns()
        {
            if (ApiSession.HasMobileShopFeature)
            {
                if (!dgvSale.Columns.Contains("clnImei"))
                {
                    var colImei = new DataGridViewTextBoxColumn
                    {
                        Name = "clnImei",
                        HeaderText = "IMEI / Serial",
                        Width = 140
                    };
                    var colImei2 = new DataGridViewTextBoxColumn
                    {
                        Name = "clnImei2",
                        HeaderText = "IMEI 2",
                        Width = 140
                    };
                    var colPta = new DataGridViewComboBoxColumn
                    {
                        Name = "clnPtaStatus",
                        HeaderText = "PTA Status",
                        Width = 110,
                        DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
                    };
                    colPta.Items.AddRange("Official PTA", "Non-PTA", "CPID", "Patched", "JV");

                    var colWarranty = new DataGridViewTextBoxColumn
                    {
                        Name = "clnWarrantyMonths",
                        HeaderText = "Warranty (Mo)",
                        Width = 90
                    };
                    var colBattery = new DataGridViewTextBoxColumn
                    {
                        Name = "clnBatteryHealth",
                        HeaderText = "Battery %",
                        Width = 70
                    };
                    var colCondition = new DataGridViewTextBoxColumn
                    {
                        Name = "clnConditionNote",
                        HeaderText = "Condition / Note",
                        Width = 120
                    };

                    int insertIndex = clnItemNo.Index + 1;
                    dgvSale.Columns.Insert(insertIndex, colImei);
                    dgvSale.Columns.Insert(insertIndex + 1, colImei2);
                    dgvSale.Columns.Insert(insertIndex + 2, colPta);
                    dgvSale.Columns.Insert(insertIndex + 3, colWarranty);
                    dgvSale.Columns.Insert(insertIndex + 4, colBattery);
                    dgvSale.Columns.Insert(insertIndex + 5, colCondition);
                }
                else
                {
                    dgvSale.Columns["clnImei"].Visible = true;
                    dgvSale.Columns["clnImei2"].Visible = true;
                    dgvSale.Columns["clnPtaStatus"].Visible = true;
                    dgvSale.Columns["clnWarrantyMonths"].Visible = true;
                    dgvSale.Columns["clnBatteryHealth"].Visible = true;
                    dgvSale.Columns["clnConditionNote"].Visible = true;
                }
            }
            else
            {
                if (dgvSale.Columns.Contains("clnImei")) dgvSale.Columns["clnImei"].Visible = false;
                if (dgvSale.Columns.Contains("clnImei2")) dgvSale.Columns["clnImei2"].Visible = false;
                if (dgvSale.Columns.Contains("clnPtaStatus")) dgvSale.Columns["clnPtaStatus"].Visible = false;
                if (dgvSale.Columns.Contains("clnWarrantyMonths")) dgvSale.Columns["clnWarrantyMonths"].Visible = false;
                if (dgvSale.Columns.Contains("clnBatteryHealth")) dgvSale.Columns["clnBatteryHealth"].Visible = false;
                if (dgvSale.Columns.Contains("clnConditionNote")) dgvSale.Columns["clnConditionNote"].Visible = false;
            }
        }

        private async void frmPurchase_Load(object sender, EventArgs e)
        {
            try
            {
                if (clnCarriage != null)
                {
                    clnCarriage.Visible = ApiSession.EnableCarriage;
                }
                SetupSecondaryQtyColumns();
                SetupMobileShopColumns();
                ApplyBarcodeFastScanVisibility();
                SetupF9Hints();
                await LoadLookupsAsync();
                await FillQueryAsync();
                if (_queryList.Count > 0 && txtVoucherNo.Text == "")
                    await FillSaleAsync(_queryList[VoucherIndex].VoucherNo);
                FLogIn = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sale data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyBarcodeFastScanVisibility()
        {
            try
            {
                if (groupBox2 == null) return;

                if (!ApiSession.IsBarcodeFastScanEnabled)
                {
                    groupBox2.Visible = false;
                    dgvSale.Location = new System.Drawing.Point(groupBox2.Location.X, groupBox2.Location.Y);
                    dgvSale.Height = panel1.Top - dgvSale.Top - 8;
                }
                else
                {
                    groupBox2.Visible = true;
                    dgvSale.Location = new System.Drawing.Point(groupBox2.Location.X, groupBox2.Bottom + 4);
                    dgvSale.Height = panel1.Top - dgvSale.Top - 8;
                    txtBarcode.Focus();
                }
            }
            catch { }
        }

        private void dgvPurchase_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int seq = 0;
            try
            {
                seq = (int)(from DataGridViewRow row in dgvSale.Rows
                            where row.Cells[clnSeq.Index].Value != null && row.Cells[clnSeq.Index].Value.ToString() != ""
                            select int.Parse(row.Cells[clnSeq.Index].Value.ToString())).Max();
            }
            catch { }

            if (dgvSale.Rows[e.RowIndex].Cells[clnSeq.Index].Value == null)
            {
                dgvSale.Rows[e.RowIndex].Cells[clnSeq.Index].Value = (seq + 1).ToString();
                dgvSale.Rows[e.RowIndex].Cells[clnStatus.Index].Value = "0";
            }
        }

        private async void frmPurchase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && ApiSession.IsBarcodeFastScanEnabled && groupBox2.Visible)
            {
                txtBarcode.Focus();
                txtBarcode.SelectAll();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (!txtBarcode.Focused && e.KeyCode == Keys.Enter && !dgvSale.Focused)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{tab}");
            }
            else if (e.KeyCode == Keys.F1)
            {
                Print(false);
            }
            else if (e.KeyCode == Keys.F6)
            {
                await ExecuteSaveAsync(true);
            }
            else if (e.KeyCode == Keys.F5)
            {
                await ExecuteSaveAsync(false);
            }
        }

        private void dgvPurchase_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvSale.CurrentCellAddress.X == clnItemNo.DisplayIndex || dgvSale.CurrentCell.ColumnIndex == clnItemNo.Index)
            {
                ComboBox cmbItem = e.Control as ComboBox;
                if (cmbItem != null)
                {
                    cmbItem.DropDownStyle = ComboBoxStyle.DropDown;
                    cmbItem.AutoCompleteSource = AutoCompleteSource.ListItems;
                    cmbItem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cmbItem.SelectedIndexChanged -= new EventHandler(cmbItem_SelectedIndexChanged);
                    cmbItem.SelectedIndexChanged += new EventHandler(cmbItem_SelectedIndexChanged);
                }
            }
            else if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQty.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscount.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscPercent.Index ||
                (clnCarriage != null && dgvSale.CurrentCell.ColumnIndex == clnCarriage.Index) ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecQty" ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecRate")
            {
                TextBox tbRate = e.Control as TextBox;
                if (tbRate != null && e.Control.Text != null)
                    tbRate.KeyPress += new KeyPressEventHandler(tbRate_KeyPress);
            }
        }

        private void SetcmbItemSource(string itemCategory, int rowInd)
        {
            DataGridViewComboBoxCell cmb = dgvSale[clnItemNo.Index, rowInd] as DataGridViewComboBoxCell;
            if (cmb != null)
            {
                cmb.DataSource = dtItems.Copy();
                cmb.DisplayMember = "Title";
                cmb.ValueMember = "Id";
            }
        }

        private void SetAllUnitsSource(int rowInd)
        {
        }

        private void tbQty_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void tbRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQty.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscount.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscPercent.Index ||
                (clnCarriage != null && dgvSale.CurrentCell.ColumnIndex == clnCarriage.Index) ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecQty" ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecRate")
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
                {
                    e.Handled = true;
                }
            }
        }

        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((dgvSale.CurrentCellAddress.X == clnItemNo.DisplayIndex || dgvSale.CurrentCell.ColumnIndex == clnItemNo.Index) && (sender as ComboBox).SelectedIndex != -1)
            {
                DataRowView dr = (DataRowView)(sender as ComboBox).SelectedItem;
                if ((sender as ComboBox).SelectedValue == null && dr != null)
                    (sender as ComboBox).SelectedValue = dr[0].ToString();
                if (dr != null)
                {
                    int rowInd = dgvSale.CurrentCellAddress.Y;
                    if (rowInd >= 0 && rowInd < dgvSale.Rows.Count)
                    {
                        if (dr.Row.Table.Columns.Contains("ItemKey"))
                            dgvSale[clnItemKey.Index, rowInd].Value = dr["ItemKey"]?.ToString();
                        if (dr.Row.Table.Columns.Contains("DefaultUnit"))
                        {
                            try
                            {
                                dgvSale[clnUnit.Index, rowInd].Value = dr["DefaultUnit"]?.ToString();
                            }
                            catch { }
                        }
                    }
                    SetcmbUnitSource(dr, dgvSale.CurrentCellAddress.Y);
                    if (txtVoucherNo.Text == "")
                        dgvSale.Rows[dgvSale.CurrentCellAddress.Y].Cells[clnDiscPercent.Index].Value = "0";
                }
            }
        }

        private string GetSelectedCustomerCategoryId()
        {
            if (cmbAccounts.SelectedValue == null) return null;
            string account = cmbAccounts.SelectedValue.ToString();
            var row = dtAccounts.AsEnumerable().FirstOrDefault(r => r["Account"].ToString() == account);
            return row != null && row.Table.Columns.Contains("CustomerCategoryId") && row["CustomerCategoryId"] != DBNull.Value
                ? row["CustomerCategoryId"].ToString()
                : null;
        }

        private (decimal PriRate, decimal? SecRate) ResolveItemRates(string itemId, decimal defaultPriRate, decimal? defaultSecRate)
        {
            string catId = GetSelectedCustomerCategoryId();
            if (!string.IsNullOrWhiteSpace(catId) && _categoryPriceMap.TryGetValue($"{itemId}_{catId}", out var catPrice))
            {
                return (catPrice.PriRate, catPrice.SecRate ?? defaultSecRate);
            }
            return (defaultPriRate, defaultSecRate);
        }

        private void SetcmbUnitSource(DataRowView dr, int rowInd)
        {
            string itemId = dr["Id"].ToString();
            decimal defaultPriRate = ParseDecimal(dr["PriRate"]);
            decimal? defaultSecRate = dr["SecRate"] != DBNull.Value ? (decimal?)ParseDecimal(dr["SecRate"]) : null;

            var (priRate, secRate) = ResolveItemRates(itemId, defaultPriRate, defaultSecRate);

            dgvSale[clnRate.Index, rowInd].Value = priRate.ToString("0.##");
            if (rowInd >= 0 && rowInd < dgvSale.Rows.Count)
            {
                UnitConversionHelper.BindRowUnitComboBox(dgvSale.Rows[rowInd], clnUnit.Index, dr.Row, dtUnits);
            }

            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                if (dgvSale["clnSecQty", rowInd].Value == null || string.IsNullOrWhiteSpace(dgvSale["clnSecQty", rowInd].Value.ToString()))
                    dgvSale["clnSecQty", rowInd].Value = "0";
                dgvSale["clnSecRate", rowInd].Value = (secRate ?? 0).ToString("0.##");
            }
        }

        private void dgvPurchase_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dgvPurchase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{tab}");
            }
            else if (e.KeyCode == Keys.F9)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                ShowCustomerPriceHistory();
            }
        }

        private void ShowCustomerPriceHistory()
        {
            if (cmbAccounts.SelectedValue == null || string.IsNullOrWhiteSpace(cmbAccounts.SelectedValue.ToString()))
            {
                MessageBox.Show("Please select a customer first.", "Customer Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbAccounts.Focus();
                return;
            }

            int rowIndex = dgvSale.CurrentCellAddress.Y >= 0 ? dgvSale.CurrentCellAddress.Y : currentRow;
            if (rowIndex < 0 || rowIndex >= dgvSale.Rows.Count) return;

            var itemIdVal = dgvSale[clnItemNo.Index, rowIndex].Value;
            if (itemIdVal == null || string.IsNullOrWhiteSpace(itemIdVal.ToString()))
            {
                MessageBox.Show("Please select an item in the current row first.", "Item Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string accountId = cmbAccounts.SelectedValue.ToString();
            string customerName = cmbAccounts.Text;
            string itemId = itemIdVal.ToString();
            string itemTitle = dgvSale[clnItemNo.Index, rowIndex].FormattedValue?.ToString() ?? itemId;

            using (var historyForm = new ERP.Forms.frmCustomerPriceHistory(
                accountId,
                customerName,
                itemId,
                itemTitle,
                ApiSession.HasSecondaryQty))
            {
                if (historyForm.ShowDialog(this) == DialogResult.OK)
                {
                    dgvSale[clnRate.Index, rowIndex].Value = historyForm.SelectedRate.ToString("0.##");
                    if (ApiSession.HasSecondaryQty && historyForm.SelectedSecRate.HasValue && dgvSale.Columns.Contains("clnSecRate"))
                    {
                        dgvSale["clnSecRate", rowIndex].Value = historyForm.SelectedSecRate.Value.ToString("0.##");
                    }

                    // Recalculate amount
                    decimal qty = ParseDecimal(dgvSale[clnQty.Index, rowIndex].Value);
                    DataRow itemRow = dtItems.Select("Id = '" + itemId.Replace("'", "''") + "'").FirstOrDefault();
                    string selectedUnitId = Convert.ToString(dgvSale[clnUnit.Index, rowIndex].Value);
                    decimal baseQty = UnitConversionHelper.ConvertToBasePrimaryQty(qty, selectedUnitId, itemRow, dtUnits);
                    decimal rate = historyForm.SelectedRate;
                    decimal discount = ParseDecimal(dgvSale[clnDiscount.Index, rowIndex].Value);
                    decimal carriage = clnCarriage != null ? ParseDecimal(dgvSale[clnCarriage.Index, rowIndex].Value) : 0;
                    decimal secQty = (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty")) ? ParseDecimal(dgvSale["clnSecQty", rowIndex].Value) : 0;
                    decimal secRate = (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecRate")) ? ParseDecimal(dgvSale["clnSecRate", rowIndex].Value) : 0;

                    dgvSale[clnAmount.Index, rowIndex].Value = ((baseQty * rate) + (secQty * secRate) - discount + carriage).ToString("0.##");
                    CalcTotAmount();
                }
            }
        }

        private void dgvPurchase_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == clnItemKey.Index)
            {
                string itemKey = ((string)dgvSale[e.ColumnIndex, e.RowIndex].Value).nullToEmpty().ToUpper();
                DataRow dr = dtItems.Select("ItemKey = '" + itemKey.Replace("'", "''") + "'").FirstOrDefault();
                if (dr != null)
                {
                    DataRowView drv = dtItems.DefaultView[dtItems.Rows.IndexOf(dr)];
                    SetcmbItemSource(dr["fkItemcatagory"].ToString(), dgvSale.CurrentCellAddress.Y);
                    dgvSale[clnItemNo.Index, e.RowIndex].Value = dr["Id"].ToString();
                    SetcmbUnitSource(drv, e.RowIndex);
                }
            }

            AllowNewRow();
            resetRow = true;
            currentRow = e.RowIndex;

            decimal qty = ParseDecimal(dgvSale[clnQty.Index, e.RowIndex].Value);
            decimal rate = ParseDecimal(dgvSale[clnRate.Index, e.RowIndex].Value);
            decimal discount = ParseDecimal(dgvSale[clnDiscount.Index, e.RowIndex].Value);
            decimal discountPercent = ParseDecimal(dgvSale[clnDiscPercent.Index, e.RowIndex].Value);
            decimal carriage = clnCarriage != null ? ParseDecimal(dgvSale[clnCarriage.Index, e.RowIndex].Value) : 0;

            decimal secQty = 0;
            decimal secRate = 0;
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                secQty = ParseDecimal(dgvSale["clnSecQty", e.RowIndex].Value);
                secRate = ParseDecimal(dgvSale["clnSecRate", e.RowIndex].Value);
            }

            dgvSale[clnQty.Index, e.RowIndex].Value = qty.ToString();
            dgvSale[clnRate.Index, e.RowIndex].Value = rate.ToString();
            dgvSale[clnDiscount.Index, e.RowIndex].Value = discount.ToString();
            dgvSale[clnDiscPercent.Index, e.RowIndex].Value = discountPercent.ToString();
            if (clnCarriage != null)
                dgvSale[clnCarriage.Index, e.RowIndex].Value = carriage.ToString();
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                dgvSale["clnSecQty", e.RowIndex].Value = secQty.ToString();
                dgvSale["clnSecRate", e.RowIndex].Value = secRate.ToString();
            }

            string rowItemId = Convert.ToString(dgvSale[clnItemNo.Index, e.RowIndex].Value);
            DataRow rowItem = !string.IsNullOrWhiteSpace(rowItemId)
                ? dtItems.Select("Id = '" + rowItemId.Replace("'", "''") + "'").FirstOrDefault()
                : null;
            string selectedUnitId = Convert.ToString(dgvSale[clnUnit.Index, e.RowIndex].Value);
            decimal baseQty = UnitConversionHelper.ConvertToBasePrimaryQty(qty, selectedUnitId, rowItem, dtUnits);

            decimal lineGross = (baseQty * rate) + (secQty * secRate);
            if (!clnDiscount.Visible)
            {
                discount = lineGross * (discountPercent / 100);
                dgvSale[clnDiscount.Index, e.RowIndex].Value = discount.ToString();
            }
            else
            {
                discountPercent = lineGross == 0 ? 0 : (discount / lineGross) * 100;
                dgvSale[clnDiscPercent.Index, e.RowIndex].Value = discountPercent.ToString();
            }

            dgvSale[clnAmount.Index, e.RowIndex].Value = decimal.Round(lineGross - discount + carriage, 2).ToString();
            CalcTotAmount();

            bool isRateColumn = e.ColumnIndex == clnRate.Index ||
                (dgvSale.Columns.Contains("clnSecRate") && e.ColumnIndex == dgvSale.Columns["clnSecRate"].Index);

            string currentAccountId = cmbAccounts.SelectedItem is DataRowView drvAcc && drvAcc.Row.Table.Columns.Contains("Account")
                ? drvAcc["Account"]?.ToString()
                : cmbAccounts.SelectedValue?.ToString();

            if (isRateColumn && !string.IsNullOrEmpty(_originalAccountId) &&
                string.Equals(currentAccountId?.Trim(), _originalAccountId.Trim(), StringComparison.OrdinalIgnoreCase) &&
                e.RowIndex >= 0 && e.RowIndex < dgvSale.Rows.Count)
            {
                var row = dgvSale.Rows[e.RowIndex];
                if (!row.IsNewRow)
                {
                    var savedRate = row.Tag as SavedRowRateInfo;
                    if (savedRate == null)
                    {
                        savedRate = new SavedRowRateInfo();
                        row.Tag = savedRate;
                    }
                    savedRate.Rate = rate;
                    savedRate.SecRate = secRate;
                }
            }
        }

        private void dgvPurchase_SelectionChanged(object sender, EventArgs e)
        {
            if (resetRow)
            {
                resetRow = false;
                dgvSale.CurrentCell = dgvSale.Rows[currentRow].Cells[dgvSale.CurrentCell.ColumnIndex];
            }
        }

        private async void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            await ExecuteSaveAsync(true);
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            await ExecuteSaveAsync(false);
        }

        private async System.Threading.Tasks.Task ExecuteSaveAsync(bool shouldPrint)
        {
            if (_isSaving)
                return;

            string confirmMsg = shouldPrint ? ("Are you sure?" + Environment.NewLine + "You want to Save & Print this...!") : ("Are you sure?" + Environment.NewLine + "You want to save this...!");
            if (MessageBox.Show(confirmMsg, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _isSaving = true;
                btnSave.Enabled = false;
                if (btnSaveAndPrint != null) btnSaveAndPrint.Enabled = false;
                try
                {
                    await SaveAsync(shouldPrint);
                }
                finally
                {
                    _isSaving = false;
                    btnSave.Enabled = true;
                    if (btnSaveAndPrint != null) btnSaveAndPrint.Enabled = true;
                }
            }
        }

        private async void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                string voucher = txtFilterVoucher.Text;
                if (!string.IsNullOrWhiteSpace(voucher) && voucher.StartsWith("SL-", StringComparison.OrdinalIgnoreCase))
                    voucher = voucher.Substring(3);

                await FillQueryAsync(
                    txtFdate.Text,
                    txtTdate.Text,
                    cmbFilterAccounts.SelectedValue != null ? cmbFilterAccounts.SelectedValue.ToString() : "",
                    voucher);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sale data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void txtFilterVoucher_Validated(object sender, EventArgs e)
        {
            txtFilterVoucher.Text = txtFilterVoucher.Text.ToUpper();
            if (!Regex.IsMatch(txtFilterVoucher.Text, @"SL-\d{5}"))
                txtFilterVoucher.Text = "";
        }

        private async void dgvQuery_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!FLogIn && e.RowIndex > -1)
            {
                VoucherIndex = e.RowIndex;
                string voucherNo = dgvQuery.Rows[e.RowIndex].Cells[clnVoucherNum.Index].Value.ToString();
                tbSaleQuery.SelectedTab = tbDetail;
                await FillSaleAsync(voucherNo.Substring(3));
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            dgvSale.Rows.Clear();
            Validation.Clear(grpInvoiceDetail);
            _originalAccountId = null;
            txtDescription.Text = "";
            txtCreatedBy.Text = "";
            txtEditBy.Text = "";
            txtBarcode.Text = "";
            txtVoucherNo.Text = "";
            VoucherNum = null;
            txtTotAmount.Text = "0";
            txtCashReceipt.Text = "0";
            txtCashBack.Text = "0";
            txtBalance.Text = "0";
            dgvSale.Rows.Add();
            cmbAccounts.SelectedIndex = -1;
            cmbNarration.SelectedIndex = -1;
            dtpDate.Focus();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print(true);
        }

        private void Navigate(Navigators nav)
        {
            if (dgvQuery.CurrentRow != null && tbSaleQuery.SelectedTab == tbDetail)
            {
                if (nav == Navigators.Up && VoucherIndex > 0)
                {
                    VoucherIndex--;
                    _ = FillSaleAsync(_queryList[VoucherIndex].VoucherNo);
                }
                else if (nav == Navigators.Down && _queryList.Count - 1 > VoucherIndex)
                {
                    VoucherIndex++;
                    _ = FillSaleAsync(_queryList[VoucherIndex].VoucherNo);
                }
                else if (nav == Navigators.Home && _queryList.Count > 0)
                {
                    VoucherIndex = 0;
                    _ = FillSaleAsync(_queryList[VoucherIndex].VoucherNo);
                }
                else if (nav == Navigators.End && _queryList.Count > 0)
                {
                    VoucherIndex = _queryList.Count - 1;
                    _ = FillSaleAsync(_queryList[VoucherIndex].VoucherNo);
                }
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            Navigate(Navigators.Home);
        }

        private void btnPri_Click(object sender, EventArgs e)
        {
            Navigate(Navigators.Up);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Navigate(Navigators.Down);
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            Navigate(Navigators.End);
        }

        private void tbSaleQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown)
                Navigate(Navigators.Down);
            else if (e.KeyCode == Keys.PageUp)
                Navigate(Navigators.Up);
            else if (e.KeyCode == Keys.Home)
                Navigate(Navigators.Home);
            else if (e.KeyCode == Keys.End)
                Navigate(Navigators.End);
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtVoucherNo.Text == "")
                return;

            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Delete this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string vno = txtVoucherNo.Text.Substring(3);
                    await _apiService.DeleteAsync(vno);
                    _queryList.RemoveAll(r => r.VoucherNo == vno);

                    DataGridViewRow row = dgvQuery.Rows.Cast<DataGridViewRow>()
                        .FirstOrDefault(r => r.Cells[clnVoucherNum.Index].Value.ToString().Equals(txtVoucherNo.Text));

                    btnNew_Click(null, null);
                    if (row != null)
                        dgvQuery.Rows.Remove(row);

                    CalcTotAmount();
                    MessageBox.Show("Record Successfully Deleted..!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting sale: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void deleteRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Delete this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (dgvSale[clnSeq.Index, dgvSale.CurrentRow.Index].Value != null)
                {
                    try
                    {
                        if (txtVoucherNo.Text != "")
                        {
                            int seq = int.Parse(dgvSale[clnSeq.Index, dgvSale.CurrentRow.Index].Value.ToString());
                            await _apiService.DeleteLineAsync(txtVoucherNo.Text.Substring(3), seq);
                        }

                        dgvSale.Rows.RemoveAt(dgvSale.CurrentRow.Index);
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

        private void dgvSale_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dgvSale.Rows[e.RowIndex].Selected = true;
                dgvSale.CurrentCell = this.dgvSale.Rows[e.RowIndex].Cells[e.ColumnIndex];
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tbDetail_Click(object sender, EventArgs e)
        {
        }

        private void dgvSale_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            Print(false);
        }

        private async void Print2(bool IsDirect)
        {
            if (string.IsNullOrWhiteSpace(txtVoucherNo.Text)) return;
            string vno = txtVoucherNo.Text.StartsWith("SL-", StringComparison.OrdinalIgnoreCase)
                ? txtVoucherNo.Text.Substring(3)
                : txtVoucherNo.Text;

            await ERP.Reporting.SaleReceiptViewer.PrintOrPreviewAsync(vno, IsDirect, ERP.Reporting.Models.SaleReceiptPrintLayout.A5Sheet);
        }

        private async void Print(bool IsDirect)
        {
            if (string.IsNullOrWhiteSpace(txtVoucherNo.Text)) return;
            string vno = txtVoucherNo.Text.StartsWith("SL-", StringComparison.OrdinalIgnoreCase)
                ? txtVoucherNo.Text.Substring(3)
                : txtVoucherNo.Text;

            await ERP.Reporting.SaleReceiptViewer.PrintOrPreviewAsync(vno, IsDirect, ERP.Reporting.Models.SaleReceiptPrintLayout.Thermal80mm);
        }

        private void txtCashReceipt_TextChanged(object sender, EventArgs e)
        {
            txtCashBack.Text = (txtCashReceipt.Value - txtTotAmount.Value <= 0 ? 0 : txtCashReceipt.Value - txtTotAmount.Value).ToString("N2");
            txtBalance.Text = (txtTotAmount.Value - (txtCashReceipt.Value - txtCashBack.Value)).ToString("N2");
        }

        private void txtTotAmount_TextChanged(object sender, EventArgs e)
        {
            txtCashBack.Text = (txtCashReceipt.Value - txtTotAmount.Value <= 0 ? 0 : txtCashReceipt.Value - txtTotAmount.Value).ToString("N2");
            txtBalance.Text = (txtTotAmount.Value - (txtCashReceipt.Value - txtCashBack.Value)).ToString("N2");
        }

        void CopyAsNew()
        {
            if (txtVoucherNo.Text != "")
            {
                txtVoucherNo.Text = "";
                VoucherNum = "";
                _originalAccountId = null;
                txtCreatedBy.Text = "";
                txtEditBy.Text = "";
                dtpDate.Value = DateTime.Now;
                MessageBox.Show("Record copied for new entry. Click Save to create a new record.", "Copy as New", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dtpDate.Focus();
            }
            else
            {
                MessageBox.Show("No record to copy. Please load a record first.", "Copy as New", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnNewCopy_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will copy the current record as a new entry." + Environment.NewLine + "Do you want to continue?", "Copy as New", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CopyAsNew();
            }
        }

        private void cmbAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FLogIn) return;
            UpdateExistingLineRates();
        }

        private void UpdateExistingLineRates()
        {
            string currentAccountId = cmbAccounts.SelectedItem is DataRowView drvAcc && drvAcc.Row.Table.Columns.Contains("Account")
                ? drvAcc["Account"]?.ToString()
                : cmbAccounts.SelectedValue?.ToString();

            bool isSameAsOriginal = !string.IsNullOrEmpty(_originalAccountId) &&
                                    string.Equals(currentAccountId?.Trim(), _originalAccountId.Trim(), StringComparison.OrdinalIgnoreCase);

            for (int i = 0; i < dgvSale.Rows.Count; i++)
            {
                var itemIdObj = dgvSale[clnItemNo.Index, i].Value;
                if (itemIdObj == null || string.IsNullOrWhiteSpace(itemIdObj.ToString()))
                    continue;

                var row = dgvSale.Rows[i];
                if (row.IsNewRow) continue;

                decimal priRate;
                decimal? secRate = null;

                if (isSameAsOriginal && row.Tag is SavedRowRateInfo savedRate)
                {
                    priRate = savedRate.Rate;
                    secRate = savedRate.SecRate;
                }
                else
                {
                    string itemId = itemIdObj.ToString();
                    DataRow itemRow = dtItems.Select("Id = '" + itemId.Replace("'", "''") + "'").FirstOrDefault();
                    if (itemRow == null) continue;

                    decimal defaultPriRate = ParseDecimal(itemRow["PriRate"]);
                    decimal? defaultSecRate = itemRow["SecRate"] != DBNull.Value ? (decimal?)ParseDecimal(itemRow["SecRate"]) : null;

                    var resolved = ResolveItemRates(itemId, defaultPriRate, defaultSecRate);
                    priRate = resolved.PriRate;
                    secRate = resolved.SecRate;
                }

                dgvSale[clnRate.Index, i].Value = priRate.ToString("0.##");

                decimal qty = ParseDecimal(dgvSale[clnQty.Index, i].Value);
                string itemIdForUnit = itemIdObj.ToString();
                DataRow itemRowForUnit = dtItems.Select("Id = '" + itemIdForUnit.Replace("'", "''") + "'").FirstOrDefault();
                string selectedUnitId = Convert.ToString(dgvSale[clnUnit.Index, i].Value);
                decimal baseQty = UnitConversionHelper.ConvertToBasePrimaryQty(qty, selectedUnitId, itemRowForUnit, dtUnits);
                decimal discount = ParseDecimal(dgvSale[clnDiscount.Index, i].Value);
                decimal carriage = clnCarriage != null ? ParseDecimal(dgvSale[clnCarriage.Index, i].Value) : 0m;
                decimal secQty = 0;
                decimal sRate = 0;

                if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecRate") && dgvSale.Columns.Contains("clnSecQty"))
                {
                    sRate = secRate ?? 0;
                    dgvSale["clnSecRate", i].Value = sRate.ToString("0.##");
                    secQty = ParseDecimal(dgvSale["clnSecQty", i].Value);
                }

                decimal lineGross = (baseQty * priRate) + (secQty * sRate);
                dgvSale[clnAmount.Index, i].Value = decimal.Round(lineGross - discount + carriage, 2).ToString();
            }
            CalcTotAmount();
        }

        private async void btnAddNewItem_Click(object sender, EventArgs e)
        {
            frmItemDetail frm = new frmItemDetail();
            if (frm.ShowDialog() == DialogResult.OK)
                await LoadLookupsAsync();
        }

        private bool _isProcessingBarcodeScan = false;

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            if (!_isProcessingBarcodeScan && txtBarcode.Text != "" && (txtBarcode.Text.Contains("\n") || txtBarcode.Text.Contains("\r")))
            {
                ProcessBarcodeScan();
            }
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                if (!_isProcessingBarcodeScan)
                {
                    ProcessBarcodeScan();
                }
            }
        }

        private DataRow FindItemByBarcodeOrCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code) || dtItems == null || dtItems.Rows.Count == 0)
                return null;

            string clean = code.Trim();

            // 1. Exact Barcode match (case-insensitive)
            DataRow match = dtItems.AsEnumerable().FirstOrDefault(r =>
                r["Barcode"] != DBNull.Value &&
                string.Equals(r["Barcode"].ToString().Trim(), clean, StringComparison.OrdinalIgnoreCase));

            // 2. Exact ItemKey match (case-insensitive)
            if (match == null)
            {
                match = dtItems.AsEnumerable().FirstOrDefault(r =>
                    r["ItemKey"] != DBNull.Value &&
                    string.Equals(r["ItemKey"].ToString().Trim(), clean, StringComparison.OrdinalIgnoreCase));
            }

            // 3. Exact Id match (case-insensitive)
            if (match == null)
            {
                match = dtItems.AsEnumerable().FirstOrDefault(r =>
                    r["Id"] != DBNull.Value &&
                    string.Equals(r["Id"].ToString().Trim(), clean, StringComparison.OrdinalIgnoreCase));
            }

            // 4. Exact Title match (case-insensitive)
            if (match == null)
            {
                match = dtItems.AsEnumerable().FirstOrDefault(r =>
                    r["Title"] != DBNull.Value &&
                    string.Equals(r["Title"].ToString().Trim(), clean, StringComparison.OrdinalIgnoreCase));
            }

            // 5. Partial Barcode or Title match if length >= 3
            if (match == null && clean.Length >= 3)
            {
                var partials = dtItems.AsEnumerable().Where(r =>
                    (r["Barcode"] != DBNull.Value && r["Barcode"].ToString().IndexOf(clean, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (r["Title"] != DBNull.Value && r["Title"].ToString().IndexOf(clean, StringComparison.OrdinalIgnoreCase) >= 0)
                ).Take(2).ToList();

                if (partials.Count == 1)
                {
                    match = partials[0];
                }
            }

            return match;
        }

        private void ProcessBarcodeScan()
        {
            if (_isProcessingBarcodeScan) return;
            try
            {
                _isProcessingBarcodeScan = true;

                string raw = txtBarcode.Text.Trim();
                txtBarcode.Clear();
                if (string.IsNullOrWhiteSpace(raw)) return;

                var parsed = BarcodeParser.Parse(raw, dtItems, (id, baseRate) => ResolveItemRates(id, baseRate, null).PriRate);
                if (parsed == null)
                {
                    ScannerToneHelper.PlayError();
                    return;
                }

                DataRow dr = parsed.ItemRow;
                decimal qtyToScan = parsed.Qty;
                decimal itemPriRate = parsed.Rate;

                string itemId = dr["Id"].ToString();
                string itemKey = dr["ItemKey"] != DBNull.Value ? dr["ItemKey"].ToString() : "";
                string itemCategory = dr["fkItemcatagory"] != DBNull.Value ? dr["fkItemcatagory"].ToString() : "";
                string defaultUnit = !string.IsNullOrWhiteSpace(parsed.Unit)
                    ? parsed.Unit
                    : (dr["DefaultUnit"] != DBNull.Value ? dr["DefaultUnit"].ToString() : "");
                decimal itemSecRate = dr["SecRate"] != DBNull.Value ? ParseDecimal(dr["SecRate"]) : 0m;

                DataRowView drv = dtItems.DefaultView[dtItems.Rows.IndexOf(dr)];
                bool isFind = false;
                int ind = -1;

                if (ApiSession.FastScanAutoIncrement)
                {
                    for (int i = 0; i < dgvSale.Rows.Count; i++)
                    {
                        if (Convert.ToString(dgvSale[clnItemNo.Index, i].Value) == itemId)
                        {
                            decimal existingRate = ParseDecimal(dgvSale[clnRate.Index, i].Value);
                            // If same item with same rate, increment quantity
                            if (Math.Abs(existingRate - itemPriRate) < 0.001m)
                            {
                                decimal existingQty = ParseDecimal(dgvSale[clnQty.Index, i].Value);
                                dgvSale[clnQty.Index, i].Value = (existingQty + qtyToScan).ToString("0.##");
                                isFind = true;
                                ind = i;
                                break;
                            }
                        }
                    }
                }

                if (!isFind)
                {
                    int emptyRowIdx = -1;
                    for (int i = 0; i < dgvSale.Rows.Count; i++)
                    {
                        var cellVal = dgvSale[clnItemNo.Index, i].Value;
                        if (cellVal == null || string.IsNullOrWhiteSpace(cellVal.ToString()))
                        {
                            emptyRowIdx = i;
                            break;
                        }
                    }

                    if (emptyRowIdx >= 0)
                    {
                        ind = emptyRowIdx;
                    }
                    else
                    {
                        ind = dgvSale.Rows.Add();
                    }

                    if (dgvSale[clnSeq.Index, ind].Value == null || string.IsNullOrWhiteSpace(dgvSale[clnSeq.Index, ind].Value.ToString()))
                    {
                        int maxSeq = 0;
                        for (int i = 0; i < dgvSale.Rows.Count; i++)
                        {
                            if (i != ind && dgvSale[clnSeq.Index, i].Value != null && int.TryParse(dgvSale[clnSeq.Index, i].Value.ToString(), out int s))
                            {
                                if (s > maxSeq) maxSeq = s;
                            }
                        }
                        dgvSale[clnSeq.Index, ind].Value = (maxSeq + 1).ToString();
                    }

                    dgvSale[clnItemKey.Index, ind].Value = itemKey;
                    SetcmbItemSource(itemCategory, ind);
                    dgvSale[clnItemNo.Index, ind].Value = itemId;
                    SetcmbUnitSource(drv, ind);

                    decimal displayScanQty = qtyToScan;
                    string displayScanUnit = defaultUnit;
                    UnitConversionHelper.ResolveDisplayQtyAndUnit(qtyToScan, dr, dtUnits, out displayScanQty, out displayScanUnit);
                    UnitConversionHelper.BindRowUnitComboBox(dgvSale.Rows[ind], clnUnit.Index, dr, dtUnits, displayScanUnit);

                    dgvSale[clnRate.Index, ind].Value = itemPriRate.ToString("0.##");
                    dgvSale[clnQty.Index, ind].Value = displayScanQty.ToString("0.####");
                    if (dgvSale[clnDiscount.Index, ind].Value == null)
                        dgvSale[clnDiscount.Index, ind].Value = "0";
                    if (dgvSale[clnDiscPercent.Index, ind].Value == null)
                        dgvSale[clnDiscPercent.Index, ind].Value = "0";
                    if (clnCarriage != null && dgvSale[clnCarriage.Index, ind].Value == null)
                        dgvSale[clnCarriage.Index, ind].Value = "0";
                    dgvSale[clnStatus.Index, ind].Value = "0";

                    if (ind == dgvSale.Rows.Count - 1)
                    {
                        dgvSale.Rows.Add();
                    }
                }

                decimal qty = ParseDecimal(dgvSale[clnQty.Index, ind].Value);
                string unitIdOnRow = Convert.ToString(dgvSale[clnUnit.Index, ind].Value);
                decimal baseQtyOnRow = UnitConversionHelper.ConvertToBasePrimaryQty(qty, unitIdOnRow, dr, dtUnits);
                decimal rate = ParseDecimal(dgvSale[clnRate.Index, ind].Value);
                decimal discount = ParseDecimal(dgvSale[clnDiscount.Index, ind].Value);
                decimal discountPercent = ParseDecimal(dgvSale[clnDiscPercent.Index, ind].Value);
                decimal carriage = clnCarriage != null ? ParseDecimal(dgvSale[clnCarriage.Index, ind].Value) : 0m;

                decimal secQty = 0m;
                decimal secRate = 0m;
                if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                {
                    secQty = ParseDecimal(dgvSale["clnSecQty", ind].Value);
                    secRate = ParseDecimal(dgvSale["clnSecRate", ind].Value);
                }

                decimal lineGross = (baseQtyOnRow * rate) + (secQty * secRate);
                if (!clnDiscount.Visible)
                {
                    discount = lineGross * (discountPercent / 100m);
                    dgvSale[clnDiscount.Index, ind].Value = discount.ToString("0.##");
                }
                else
                {
                    discountPercent = lineGross == 0m ? 0m : (discount / lineGross) * 100m;
                    dgvSale[clnDiscPercent.Index, ind].Value = discountPercent.ToString("0.##");
                }

                dgvSale[clnAmount.Index, ind].Value = decimal.Round(lineGross - discount + carriage, 2).ToString("N2");
                CalcTotAmount();

                ScannerToneHelper.PlaySuccess();
            }
            catch (Exception ex)
            {
                ScannerToneHelper.PlayError();
                MessageBox.Show("Error adding item from barcode scan: " + ex.Message, "Fast Scan Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                txtBarcode.Clear();
                txtBarcode.Focus();
                _isProcessingBarcodeScan = false;
            }
        }
    }
}
