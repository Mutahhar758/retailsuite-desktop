using System.Drawing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;


namespace ERP
{
    public partial class frmNormalSaleSupply : Form
    {
        private readonly SaleSupplyApiService _apiService;
        private readonly SupplyOrderApiService _supplyOrderApiService;
        private readonly CustomerApiService _customerApiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly NarrationApiService _narrationApiService;
        private readonly UnitApiService _unitApiService;
        private readonly InventoryApiService _inventoryApiService;
        private List<SaleSupplyDto> _queryList = new List<SaleSupplyDto>();

        private DataTable dtItems = new DataTable();
        private DataTable dtUnits = new DataTable();
        private DataTable dtCustomers = new DataTable();
        private DataTable dtNarration = new DataTable();
        private DataTable dtSupplyOrders = new DataTable();

        private int currentRow;
        private bool resetRow = false;
        private bool FLogIn = true;
        private string VoucherNum = null;
        private bool _isSaving;
        private readonly Dictionary<string, (decimal PriRate, decimal? SecRate)> _categoryPriceMap = new Dictionary<string, (decimal, decimal?)>(StringComparer.OrdinalIgnoreCase);
        private string _originalItemId = null;

        private class SavedRowRateInfo
        {
            public decimal Rate { get; set; }
            public decimal SecRate { get; set; }
        }

        enum Navigators { Up, Down, Home, End };

        public frmNormalSaleSupply()
        {
            InitializeComponent();
            ApplyModernStyling();
            _apiService = new SaleSupplyApiService();
            _supplyOrderApiService = new SupplyOrderApiService();
            _customerApiService = new CustomerApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _narrationApiService = new NarrationApiService();
            _unitApiService = new UnitApiService();
            _inventoryApiService = new InventoryApiService();
            InitializeLookupTables();
            int initialRow = dgvSale.Rows.Add();
            clnAmount.ReadOnly = false;
            ClearRowValues(initialRow);
            dgvSale.RowsRemoved += (s, e) => CalcTotals();
            dgvSale.Enter += dgvSale_Enter;
            UserInfo.ApplyFormPermissions(this, AppResource.SaleSupplies);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F3)
            {
                btnNew.PerformClick();
                return true;
            }
            if (keyData == Keys.F5)
            {
                btnSave.PerformClick();
                return true;
            }
            if (keyData == Keys.Escape)
            {
                btnClose.PerformClick();
                return true;
            }
            if ((keyData == Keys.Enter || keyData == Keys.Tab) && dgvSale.ContainsFocus)
            {
                if (HandleGridEnterNavigation())
                    return true;
            }

            if (keyData == Keys.Enter && !dgvSale.ContainsFocus)
            {
                if (dtpDate.Focused)
                {
                    cmbItem.Focus();
                    return true;
                }
                if (cmbItem.Focused || cmbItem.ContainsFocus)
                {
                    cmbSupplyOrder.Focus();
                    return true;
                }
                if (cmbSupplyOrder.Focused || cmbSupplyOrder.ContainsFocus)
                {
                    FocusGridFirstRow();
                    return true;
                }
                if (cmbNarration.Focused || cmbNarration.ContainsFocus || txtDescription.Focused)
                {
                    FocusGridFirstRow();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetupButtonFocusHighlights()
        {
            var buttons = new Button[] { btnSave, btnNew, btnCopyAsNew, btnScanSheet, btnDelete, btnClose, btnScanSheetTop };
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
                        rect.Width -= 1;
                        rect.Height -= 1;
                        using (var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(37, 99, 235), 2))
                        {
                            e.Graphics.DrawRectangle(pen, rect);
                        }
                        if (btn == btnSave)
                        {
                            using (var whitePen = new System.Drawing.Pen(System.Drawing.Color.White, 1))
                            {
                                e.Graphics.DrawRectangle(whitePen, rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);
                            }
                        }
                    }
                };
            }
        }

        private void FocusGridFirstRow()
        {
            try
            {
                if (dgvSale.Rows.Count == 0)
                {
                    int r = dgvSale.Rows.Add();
                    ClearRowValues(r);
                }
                dgvSale.Focus();
                int row = dgvSale.CurrentCell != null ? dgvSale.CurrentCell.RowIndex : 0;
                if (row < 0 || row >= dgvSale.Rows.Count) row = 0;
                dgvSale.CurrentCell = dgvSale[clnCustomer.Index, row];
                dgvSale.BeginEdit(true);
            }
            catch { }
        }

        private void dgvSale_Enter(object sender, EventArgs e)
        {
            if (IsHandleCreated && dgvSale.Rows.Count > 0 && clnCustomer != null)
            {
                BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (dgvSale.Rows.Count > 0 && clnCustomer != null)
                        {
                            int row = dgvSale.CurrentCell != null ? dgvSale.CurrentCell.RowIndex : 0;
                            if (row >= 0 && row < dgvSale.Rows.Count)
                            {
                                if (dgvSale.CurrentCell == null || dgvSale.CurrentCell.ColumnIndex == clnSeq.Index)
                                {
                                    dgvSale.CurrentCell = dgvSale[clnCustomer.Index, row];
                                }
                            }
                        }
                    }
                    catch { }
                }));
            }
        }

        private void ClearRowValues(int rowIdx)
        {
            if (rowIdx < 0 || rowIdx >= dgvSale.Rows.Count) return;
            var row = dgvSale.Rows[rowIdx];
            row.Cells[clnCustomer.Index].Value = null;
            if (clnUnit != null) row.Cells[clnUnit.Index].Value = null;
            row.Cells[clnQty.Index].Value = null;
            row.Cells[clnRate.Index].Value = null;
            row.Cells[clnDiscount.Index].Value = null;
            row.Cells[clnDiscPercent.Index].Value = null;
            if (clnCarriage != null) row.Cells[clnCarriage.Index].Value = null;
            if (clnAddLess != null) row.Cells[clnAddLess.Index].Value = null;
            row.Cells[clnAmount.Index].Value = null;
            if (dgvSale.Columns.Contains("clnSecQty")) row.Cells["clnSecQty"].Value = null;
            if (dgvSale.Columns.Contains("clnSecRate")) row.Cells["clnSecRate"].Value = null;
            row.Tag = null;
        }

        private void ApplyModernStyling()
        {
            SetupButtonFocusHighlights();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            // Sale Grid
            dgvSale.EnableHeadersVisualStyles = false;
            clnQty.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            clnRate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            clnDiscount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            clnDiscPercent.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            if (clnCarriage != null) clnCarriage.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            clnAddLess.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            clnAmount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSale.BackgroundColor = System.Drawing.Color.White;
            dgvSale.BorderStyle = BorderStyle.FixedSingle;
            dgvSale.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSale.GridColor = System.Drawing.Color.FromArgb(226, 232, 240);
            dgvSale.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            dgvSale.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dgvSale.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            dgvSale.ColumnHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dgvSale.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dgvSale.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSale.ColumnHeadersHeight = 32;

            dgvSale.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            dgvSale.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvSale.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            dgvSale.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            dgvSale.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            dgvSale.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            dgvSale.AlternatingRowsDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            dgvSale.AlternatingRowsDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            dgvSale.AlternatingRowsDefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            dgvSale.RowTemplate.Height = 28;

            // Query Grid
            dgvQuery.EnableHeadersVisualStyles = false;
            dgvQuery.BackgroundColor = System.Drawing.Color.White;
            dgvQuery.BorderStyle = BorderStyle.FixedSingle;
            dgvQuery.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvQuery.GridColor = System.Drawing.Color.FromArgb(226, 232, 240);
            dgvQuery.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            dgvQuery.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dgvQuery.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            dgvQuery.ColumnHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dgvQuery.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dgvQuery.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvQuery.ColumnHeadersHeight = 32;

            dgvQuery.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            dgvQuery.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvQuery.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            dgvQuery.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            dgvQuery.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            dgvQuery.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            dgvQuery.AlternatingRowsDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            dgvQuery.AlternatingRowsDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            dgvQuery.AlternatingRowsDefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            dgvQuery.RowTemplate.Height = 28;

            // 1px border at bottom of header
            pnlHeader.Paint += (s, e) =>
            {
                using (var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
                }
            };

            // Button hover animations
            btnSave.MouseEnter += (s, e) => btnSave.BackColor = System.Drawing.Color.FromArgb(21, 128, 61);
            btnSave.MouseLeave += (s, e) => btnSave.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            btnNew.MouseEnter += (s, e) => btnNew.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            btnNew.MouseLeave += (s, e) => btnNew.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            btnCopyAsNew.MouseEnter += (s, e) => btnCopyAsNew.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            btnCopyAsNew.MouseLeave += (s, e) => btnCopyAsNew.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            btnScanSheet.MouseEnter += (s, e) => btnScanSheet.BackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            btnScanSheet.MouseLeave += (s, e) => btnScanSheet.BackColor = System.Drawing.Color.FromArgb(239, 246, 255);
            btnScanSheetTop.MouseEnter += (s, e) => btnScanSheetTop.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            btnScanSheetTop.MouseLeave += (s, e) => btnScanSheetTop.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            btnDelete.MouseEnter += (s, e) => btnDelete.BackColor = System.Drawing.Color.FromArgb(254, 202, 202);
            btnDelete.MouseLeave += (s, e) => btnDelete.BackColor = System.Drawing.Color.FromArgb(254, 242, 242);
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            btnFind.MouseEnter += (s, e) => btnFind.BackColor = System.Drawing.Color.FromArgb(29, 78, 216);
            btnFind.MouseLeave += (s, e) => btnFind.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
        }


        private void InitializeLookupTables()
        {
            dtItems.Columns.Add("Id", typeof(string));
            dtItems.Columns.Add("Title", typeof(string));
            dtItems.Columns.Add("PriRate", typeof(decimal));
            dtItems.Columns.Add("SecRate", typeof(decimal));
            dtItems.Columns.Add("PrimaryUnit", typeof(string));
            dtItems.Columns.Add("SecondaryUnit", typeof(string));
            dtItems.Columns.Add("DefaultUnit", typeof(string));
            dtItems.Columns.Add("AdditionalUnits", typeof(string));

            dtUnits.Columns.Add("Code", typeof(string));
            dtUnits.Columns.Add("Title", typeof(string));
            dtUnits.Columns.Add("ShortTitle", typeof(string));
            dtUnits.Columns.Add("ParentUnitId", typeof(string));
            dtUnits.Columns.Add("ConversionRatio", typeof(decimal));

            dtCustomers.Columns.Add("Account", typeof(string));
            dtCustomers.Columns.Add("Title", typeof(string));
            dtCustomers.Columns.Add("CustomerCategoryId", typeof(string));

            dtNarration.Columns.Add("Code", typeof(string));
            dtNarration.Columns.Add("Title", typeof(string));

            dtSupplyOrders.Columns.Add("Id", typeof(int));
            dtSupplyOrders.Columns.Add("Title", typeof(string));
        }

        private async System.Threading.Tasks.Task LoadLookupsAsync()
        {
            var customersTask = _chartOfAccountApiService.GetCustomerAccountsAsync();
            var narrationsTask = _narrationApiService.GetLookupAsync();
            var unitsTask = _unitApiService.GetLookupAsync();
            var itemsTask = _inventoryApiService.GetLookupAsync(null);
            var supplyOrdersTask = _supplyOrderApiService.GetAsync();
            var categoryPricesTask = _inventoryApiService.GetCustomerCategoryPricesAsync();
            await System.Threading.Tasks.Task.WhenAll(customersTask, narrationsTask, unitsTask, itemsTask, supplyOrdersTask, categoryPricesTask);

            _categoryPriceMap.Clear();
            foreach (var cp in categoryPricesTask.Result)
            {
                if (!string.IsNullOrWhiteSpace(cp.ItemId) && !string.IsNullOrWhiteSpace(cp.CustomerCategoryId))
                {
                    _categoryPriceMap[$"{cp.ItemId}_{cp.CustomerCategoryId}"] = (cp.PriRate, cp.SecRate);
                }
            }

            dtItems.Rows.Clear();
            foreach (var item in itemsTask.Result)
                dtItems.Rows.Add(item.Id, item.Title, item.PriRate, item.SecRate, item.PrimaryUnit, item.SecondaryUnit, item.DefaultUnit, item.AdditionalUnits);

            dtUnits = UnitConversionHelper.CreateUnitsTable(unitsTask.Result);

            dtCustomers.Rows.Clear();
            foreach (var c in customersTask.Result)
                dtCustomers.Rows.Add(c.Account, c.Title, c.CustomerCategoryId);

            dtNarration.Rows.Clear();
            foreach (var n in narrationsTask.Result)
                dtNarration.Rows.Add(n.Code, n.Title);

            dtSupplyOrders.Rows.Clear();
            foreach (var so in supplyOrdersTask.Result)
                dtSupplyOrders.Rows.Add(so.Id, so.Title);

            FillItems();
            FillCustomers();
            FillNarration();
            FillFilterItems();
            FillSupplyOrders();
        }

        private DataRow GetSelectedHeaderItemRow()
        {
            if (cmbItem.SelectedItem is DataRowView drv)
                return drv.Row;
            if (cmbItem.SelectedValue != null)
            {
                string itemId = Convert.ToString(cmbItem.SelectedValue);
                return dtItems.Select("Id = '" + itemId.Replace("'", "''") + "'").FirstOrDefault();
            }
            return null;
        }

        private void FillItems()
        {
            cmbItem.DataSource = dtItems.Copy();
            cmbItem.DisplayMember = "Title";
            cmbItem.ValueMember = "Id";
            cmbItem.SelectedIndex = -1;
        }

        private void FillCustomers()
        {
            clnCustomer.DataSource = dtCustomers.Copy();
            clnCustomer.DisplayMember = "Title";
            clnCustomer.ValueMember = "Account";
        }

        private void FillNarration()
        {
            cmbNarration.DataSource = dtNarration.Copy();
            cmbNarration.DisplayMember = "Title";
            cmbNarration.ValueMember = "Code";
            cmbNarration.SelectedIndex = -1;
        }

        private void FillFilterItems()
        {
            cmbFilterAccounts.DataSource = dtItems.Copy();
            cmbFilterAccounts.DisplayMember = "Title";
            cmbFilterAccounts.ValueMember = "Id";
            cmbFilterAccounts.SelectedIndex = -1;
        }

        private void FillSupplyOrders()
        {
            cmbSupplyOrder.DataSource = dtSupplyOrders.Copy();
            cmbSupplyOrder.DisplayMember = "Title";
            cmbSupplyOrder.ValueMember = "Id";
            cmbSupplyOrder.SelectedIndex = -1;
        }

        private static decimal ParseDecimal(object value)
        {
            decimal parsed;
            return decimal.TryParse(Convert.ToString(value), out parsed) ? parsed : 0;
        }

        void CalcTotals()
        {
            int count = 0;
            decimal totQty = 0;
            decimal totSecQty = 0;
            decimal totAddLess = 0;
            decimal totAmount = 0;
            DataRow headerItemRow = GetSelectedHeaderItemRow();

            try
            {
                foreach (DataGridViewRow row in dgvSale.Rows)
                {
                    if (row.IsNewRow) continue;

                    bool hasCustomer = row.Cells[clnCustomer.Index].Value != null &&
                                       !string.IsNullOrWhiteSpace(row.Cells[clnCustomer.Index].Value.ToString());
                    if (hasCustomer)
                    {
                        count++;
                    }

                    decimal enteredQty = ParseDecimal(row.Cells[clnQty.Index].Value);
                    string selectedUnitId = Convert.ToString(row.Cells[clnUnit.Index].Value);
                    decimal baseQty = UnitConversionHelper.ConvertToBasePrimaryQty(enteredQty, selectedUnitId, headerItemRow, dtUnits);
                    totQty += baseQty;
                    if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                    {
                        totSecQty += ParseDecimal(row.Cells["clnSecQty"].Value);
                    }
                    totAddLess += ParseDecimal(row.Cells[clnAddLess.Index].Value);
                    totAmount += ParseDecimal(row.Cells[clnAmount.Index].Value);
                }
            }
            catch
            {
            }

            txtTotCount.Text = count.ToString();
            if (ApiSession.HasSecondaryQty && totSecQty > 0)
            {
                txtTotQty.Text = totQty.ToString("N2") + " (" + totSecQty.ToString("N2") + ")";
            }
            else
            {
                txtTotQty.Text = totQty.ToString("N2");
            }

            if (totAddLess > 0)
            {
                txtTotAddLess.Text = "+" + totAddLess.ToString("N2");
                txtTotAddLess.ForeColor = System.Drawing.Color.DarkGreen;
            }
            else if (totAddLess < 0)
            {
                txtTotAddLess.Text = totAddLess.ToString("N2");
                txtTotAddLess.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                txtTotAddLess.Text = "0.00";
                txtTotAddLess.ForeColor = System.Drawing.SystemColors.WindowText;
            }

            txtTotAmount.Text = totAmount.ToString("N2");
        }

        void CalcTotAmount()
        {
            CalcTotals();
        }

        private async System.Threading.Tasks.Task SaveAsync()
        {
            if (cmbItem.SelectedValue == null)
            {
                MessageBox.Show("Please Select Item...!");
                return;
            }

            DataRow headerItemRow = GetSelectedHeaderItemRow();
            var lines = new List<SaleSupplyLineApiRequest>();
            foreach (DataGridViewRow row in dgvSale.Rows)
            {
                if (row.Cells[clnCustomer.Index].Value == null)
                    continue;

                decimal secQty = 0;
                decimal secRate = 0;
                string secUnit = null;
                if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                {
                    secQty = ParseDecimal(row.Cells["clnSecQty"].Value);
                    secRate = ParseDecimal(row.Cells["clnSecRate"].Value);
                    
                    if (headerItemRow != null)
                    {
                        secUnit = Convert.ToString(headerItemRow["SecondaryUnit"]);
                    }
                }

                decimal enteredQty = ParseDecimal(row.Cells[clnQty.Index].Value);
                string selectedUnitId = Convert.ToString(row.Cells[clnUnit.Index].Value);
                decimal baseQty = UnitConversionHelper.ConvertToBasePrimaryQty(enteredQty, selectedUnitId, headerItemRow, dtUnits);

                lines.Add(new SaleSupplyLineApiRequest
                {
                    Seq = int.Parse(Convert.ToString(row.Cells[clnSeq.Index].Value)),
                    CustomerId = Convert.ToString(row.Cells[clnCustomer.Index].Value),
                    Qty = baseQty,
                    Rate = ParseDecimal(row.Cells[clnRate.Index].Value),
                    Discount = ParseDecimal(row.Cells[clnDiscount.Index].Value),
                    Carriage = ParseDecimal(row.Cells[clnCarriage.Index].Value),
                    AddLess = ParseDecimal(row.Cells[clnAddLess.Index].Value),
                    SecQty = secQty,
                    SecRate = secRate,
                    SecUnit = secUnit
                });
            }

            if (lines.Count == 0)
            {
                MessageBox.Show("Please add at least one row...!");
                return;
            }

            try
            {
                string voucher = txtVoucherNo.Text != "" ? txtVoucherNo.Text.Substring(3) : null;
                bool isNew = string.IsNullOrWhiteSpace(voucher);

                if (isNew)
                {
                    var request = new SaleSupplyCreateApiRequest
                    {
                        Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                        ItemId = cmbItem.SelectedValue.ToString(),
                        Description = txtDescription.Text,
                        Narration = cmbNarration.SelectedValue?.ToString(),
                        SupplyOrderMasterId = cmbSupplyOrder.SelectedValue != null ? (int?)Convert.ToInt32(cmbSupplyOrder.SelectedValue) : null,
                        Lines = lines
                    };
                    voucher = await _apiService.CreateAsync(request);
                }
                else
                {
                    var request = new SaleSupplyUpdateApiRequest
                    {
                        Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                        ItemId = cmbItem.SelectedValue.ToString(),
                        Description = txtDescription.Text,
                        Narration = cmbNarration.SelectedValue?.ToString(),
                        SupplyOrderMasterId = cmbSupplyOrder.SelectedValue != null ? (int?)Convert.ToInt32(cmbSupplyOrder.SelectedValue) : null,
                        Lines = lines
                    };
                    await _apiService.UpdateAsync(voucher, request);
                }

                await FillSaleAsync(voucher);
                await FillQueryAsync(_qFrom, _qTo, _qItemId, _qVoucherNo, 1);

                MessageBox.Show("Record Successfully Saved...!");
                btnNew.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving sale supply: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private ERP.Controls.PaginationControl _pager;
        private string _qFrom = "", _qTo = "", _qItemId = "", _qVoucherNo = "";
        private int _qPageSize = 20;

        private void EnsurePager()
        {
            if (_pager != null) return;
            _pager = new ERP.Controls.PaginationControl();
            _pager.PageChanged += async (s, e) =>
            {
                try
                {
                    await FillQueryAsync(_qFrom, _qTo, _qItemId, _qVoucherNo, e.PageNumber, e.PageSize);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading sale supplies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            tbQuery.Controls.Add(_pager);
        }

        private async System.Threading.Tasks.Task FillQueryAsync(
            string fromDate = "", string toDate = "",
            string itemId = "", string voucherNo = "",
            int pageNumber = 1, int pageSize = 0)
        {
            EnsurePager();
            _qFrom = fromDate; _qTo = toDate; _qItemId = itemId; _qVoucherNo = voucherNo;
            if (pageSize > 0) _qPageSize = pageSize;

            dgvQuery.Rows.Clear();
            var page = await _apiService.GetPagedListAsync(fromDate, toDate, itemId, voucherNo, pageNumber, _qPageSize);
            _queryList = page.Data;
            _pager.SetPagination(page.CurrentPage, page.TotalPages, page.TotalCount, page.PageSize);
            _qPageSize = page.PageSize;

            for (int i = 0; i < _queryList.Count; i++)
            {
                var row = _queryList[i];
                dgvQuery.Rows.Add(
                    row.Date.ToString("dd-MMM-yyyy"),
                    "SP-" + row.VoucherNo,
                    row.Item,
                    row.SupplyOrderTitle,
                    row.CreatedBy + " | " + row.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                    !string.IsNullOrWhiteSpace(row.LastModifiedBy)
                        ? row.LastModifiedBy + " | " + row.LastModifiedOn.Value.ToString("dd-Mmm-yyyy hh:mm:ss tt")
                        : null);
            }
        }

        internal async System.Threading.Tasks.Task FillSaleAsync(string vno)
        {
            var lines = await _apiService.GetDetailAsync(vno);
            txtVoucherNo.Text = "SP-" + vno;
            VoucherNum = vno;
            dgvSale.Rows.Clear();
            txtSearchCustomer.Text = ""; // clear any active customer filter when loading a voucher

            if (lines.Count > 0)
            {
                var first = lines[0];
                _originalItemId = first.ItemId;
                dtpDate.Value = first.Date;
                if (string.IsNullOrWhiteSpace(first.Narration))
                    cmbNarration.SelectedIndex = -1;
                else
                    cmbNarration.SelectedValue = first.Narration;
                cmbItem.SelectedValue = first.ItemId;
                txtDescription.Text = first.Description;

                // Restore Supply Order Profile
                if (first.SupplyOrderMasterId.HasValue)
                    cmbSupplyOrder.SelectedValue = first.SupplyOrderMasterId.Value;
                else
                    cmbSupplyOrder.SelectedIndex = -1;

                txtCreatedBy.Text = first.CreatedBy + " | " + first.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt");
                txtEditBy.Text = !string.IsNullOrWhiteSpace(first.LastModifiedBy)
                    ? first.LastModifiedBy + " | " + first.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                    : null;

                DataRow itemRow = dtItems.Select("Id = '" + first.ItemId.Replace("'", "''") + "'").FirstOrDefault();

                for (int i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    decimal discountPercent = line.Rate == 0 ? 0 : (line.Discount / line.Rate) * 100;
                    
                    int ind = dgvSale.Rows.Add();
                    var row = dgvSale.Rows[ind];
                    
                    decimal displayQty = line.Qty;
                    string displayUnitId = line.Unit;
                    if (itemRow != null)
                    {
                        UnitConversionHelper.ResolveDisplayQtyAndUnit(line.Qty, itemRow, dtUnits, out displayQty, out displayUnitId);
                        UnitConversionHelper.BindRowUnitComboBox(row, clnUnit.Index, itemRow, dtUnits, displayUnitId);
                    }
                    else
                    {
                        row.Cells[clnUnit.Index].Value = line.Unit;
                    }

                    row.Cells[clnSeq.Index].Value = line.Seq.ToString();
                    row.Cells[clnCustomer.Index].Value = line.CustomerId;
                    row.Cells[clnQty.Index].Value = displayQty.ToString("0.####");
                    row.Cells[clnRate.Index].Value = line.Rate.ToString("0.##");
                    row.Cells[clnDiscount.Index].Value = line.Discount.ToString("0.##");
                    row.Cells[clnDiscPercent.Index].Value = discountPercent.ToString("0.##");
                    row.Cells[clnCarriage.Index].Value = (line.Carriage ?? 0).ToString("0.##");
                    row.Cells[clnAddLess.Index].Value = line.AddLess.ToString("0.##");
                    row.Cells[clnAmount.Index].Value = line.Amount.ToString("N2");
                    row.Cells[clnStatus.Index].Value = "0";

                    if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                    {
                        row.Cells["clnSecQty"].Value = (line.SecQty ?? 0).ToString("0.##");
                        row.Cells["clnSecRate"].Value = (line.SecRate ?? 0).ToString("0.##");
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

        void AllowNewRow()
        {
            if (dgvSale.CurrentRow.Index == dgvSale.Rows[dgvSale.Rows.Count - 1].Index)
            {
                if (dgvSale.CurrentRow.Cells[clnCustomer.Index].Value != null)
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
                        Width = 85,
                        DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                    };
                    int insertIndex = clnDiscount.Index;
                    dgvSale.Columns.Insert(insertIndex, colSecQty);
                    dgvSale.Columns.Insert(insertIndex + 1, colSecRate);
                }
            }
            else
            {
                clnQty.HeaderText = "Quantity";
                clnRate.HeaderText = "Rate";

                if (dgvSale.Columns.Contains("clnSecQty"))
                    dgvSale.Columns.Remove("clnSecQty");
                if (dgvSale.Columns.Contains("clnSecRate"))
                    dgvSale.Columns.Remove("clnSecRate");
            }

            int dIdx = 0;
            clnSeq.DisplayIndex = dIdx++;
            clnCustomer.DisplayIndex = dIdx++;
            clnUnit.DisplayIndex = dIdx++;
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
            clnAddLess.DisplayIndex = dIdx++;
            clnAmount.DisplayIndex = dIdx++;
            if (clnStatus != null) clnStatus.DisplayIndex = dIdx++;
        }

        private async void frmPurchase_Load(object sender, EventArgs e)
        {
            try
            {
                if (clnCarriage != null)
                {
                    clnCarriage.Visible = ApiSession.EnableCarriage;
                }
                var clnProfile = new System.Windows.Forms.DataGridViewTextBoxColumn();
                clnProfile.Name = "clnProfile";
                clnProfile.HeaderText = "Profile";
                clnProfile.MinimumWidth = 150;
                clnProfile.Width = 150;
                clnProfile.ReadOnly = true;
                dgvQuery.Columns.Insert(3, clnProfile);

                SetupSecondaryQtyColumns();
                await LoadLookupsAsync();
                await FillQueryAsync();
                FLogIn = false;
                CalcTotals();
                BeginInvoke(new Action(() => dtpDate.Focus()));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sale supply data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            catch
            {
            }
            if (dgvSale.Rows[e.RowIndex].Cells[clnSeq.Index].Value == null)
            {
                dgvSale.Rows[e.RowIndex].Cells[clnSeq.Index].Value = (seq + 1).ToString();
                dgvSale.Rows[e.RowIndex].Cells[clnStatus.Index].Value = "0";
                ClearRowValues(e.RowIndex);

                DataRow itemRow = GetSelectedHeaderItemRow();
                if (itemRow != null)
                {
                    UnitConversionHelper.BindRowUnitComboBox(dgvSale.Rows[e.RowIndex], clnUnit.Index, itemRow, dtUnits);
                    SetRateForRow(e.RowIndex);
                }
            }
            CalcTotals();
        }

        private async void frmPurchase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !dgvSale.Focused)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{tab}");
            }
            else if (e.KeyCode == Keys.F5)
            {
                await SaveAsync();
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnNew.PerformClick();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                btnClose.PerformClick();
            }
        }

        private void dgvPurchase_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox cmb)
            {
                cmb.SelectedIndexChanged -= CmbCustomer_SelectedIndexChanged;
                cmb.SelectionChangeCommitted -= CmbCustomer_SelectionChangeCommitted;

                if (dgvSale.CurrentCell != null && dgvSale.CurrentCell.ColumnIndex == clnCustomer.Index)
                {
                    cmb.DropDownStyle = ComboBoxStyle.DropDown;
                    cmb.AutoCompleteSource = AutoCompleteSource.ListItems;
                    cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                    var cellVal = dgvSale.CurrentCell.Value;
                    if (cellVal == null || string.IsNullOrWhiteSpace(cellVal.ToString()))
                    {
                        try
                        {
                            cmb.SelectedIndex = -1;
                            cmb.Text = "";
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            cmb.SelectedValue = cellVal.ToString();
                        }
                        catch { }
                    }

                    cmb.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
                    cmb.SelectionChangeCommitted += CmbCustomer_SelectionChangeCommitted;
                }
                else if (clnUnit != null && dgvSale.CurrentCell != null && dgvSale.CurrentCell.ColumnIndex == clnUnit.Index)
                {
                    cmb.DropDownStyle = ComboBoxStyle.DropDownList;
                    var cellVal = dgvSale.CurrentCell.Value;
                    if (cellVal != null && !string.IsNullOrWhiteSpace(cellVal.ToString()))
                    {
                        try { cmb.SelectedValue = cellVal.ToString(); } catch { }
                    }
                }
            }
            else if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQty.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscount.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscPercent.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnCarriage.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnAddLess.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnAmount.Index ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecQty" ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecRate")
            {
                TextBox tbRate = e.Control as TextBox;
                if (tbRate != null && e.Control.Text != null)
                {
                    tbRate.KeyPress -= new KeyPressEventHandler(tbRate_KeyPress);
                    tbRate.KeyPress += new KeyPressEventHandler(tbRate_KeyPress);
                }
            }
        }

        private void CmbCustomer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            CmbCustomer_SelectedIndexChanged(sender, e);
        }

        private async void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvSale.CurrentCell != null && dgvSale.CurrentCell.ColumnIndex == clnCustomer.Index && dgvSale.CurrentCell.RowIndex >= 0)
            {
                var cb = sender as ComboBox;
                if (cb != null && cb.SelectedValue != null)
                {
                    string custId = cb.SelectedValue.ToString();
                    int rowIndex = dgvSale.CurrentCellAddress.Y;
                    dgvSale[clnCustomer.Index, rowIndex].Value = custId;

                    if (cmbItem.SelectedValue != null)
                    {
                        string itemId = cmbItem.SelectedValue.ToString();
                        DataRow headerItemRow = GetSelectedHeaderItemRow();
                        try
                        {
                            var customItems = await _customerApiService.GetSupplyItemsAsync(custId, itemId);
                            var customSetting = customItems?.FirstOrDefault();
                            if (customSetting != null)
                            {
                                decimal rawDefaultQty = customSetting.Qty > 0 ? customSetting.Qty : 1;
                                decimal dispQty = rawDefaultQty;
                                string dispUnitId = headerItemRow != null ? Convert.ToString(headerItemRow["PrimaryUnit"]) : "";
                                if (headerItemRow != null)
                                {
                                    UnitConversionHelper.ResolveDisplayQtyAndUnit(rawDefaultQty, headerItemRow, dtUnits, out dispQty, out dispUnitId);
                                    UnitConversionHelper.BindRowUnitComboBox(dgvSale.Rows[rowIndex], clnUnit.Index, headerItemRow, dtUnits, dispUnitId);
                                }
                                dgvSale[clnQty.Index, rowIndex].Value = dispQty.ToString("0.####");
                                if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                                {
                                    dgvSale["clnSecQty", rowIndex].Value = (customSetting.SecQty ?? 0).ToString("0.##");
                                }
                                if (customSetting.Discount.HasValue)
                                    dgvSale[clnDiscount.Index, rowIndex].Value = customSetting.Discount.Value.ToString("0.##");
                                if (customSetting.Carriage.HasValue)
                                    dgvSale[clnCarriage.Index, rowIndex].Value = customSetting.Carriage.Value.ToString("0.##");
                                if (customSetting.AddLess.HasValue)
                                    dgvSale[clnAddLess.Index, rowIndex].Value = customSetting.AddLess.Value.ToString("0.##");
                            }
                        }
                        catch
                        {
                        }

                        SetRateForRow(rowIndex);

                        // Apply rate override AFTER SetRateForRow so it wins over the item default
                        try
                        {
                            var customItems = await _customerApiService.GetSupplyItemsAsync(custId, cmbItem.SelectedValue.ToString());
                            var customSetting = customItems?.FirstOrDefault();
                            if (customSetting?.Rate.HasValue == true)
                                dgvSale[clnRate.Index, rowIndex].Value = customSetting.Rate.Value.ToString("0.##");
                        }
                        catch { }

                        RecalculateRow(rowIndex);
                        CalcTotAmount();
                    }
                }
            }
        }

        void SetRateForRow(int rowIndex)
        {
            if (cmbItem.SelectedValue != null && rowIndex >= 0 && rowIndex < dgvSale.Rows.Count)
            {
                string itemId = cmbItem.SelectedValue.ToString();
                string Filter = string.Format("Id = '{0}'", itemId);
                DataRow dr = dtItems.Select(Filter).FirstOrDefault();
                if (dr != null)
                {
                    decimal defaultPriRate = ParseDecimal(dr["PriRate"]);
                    decimal? defaultSecRate = dr["SecRate"] != DBNull.Value ? (decimal?)ParseDecimal(dr["SecRate"]) : null;

                    string custId = dgvSale.Rows[rowIndex].Cells[clnCustomer.Index].Value?.ToString();
                    string catId = null;
                    if (!string.IsNullOrWhiteSpace(custId))
                    {
                        var custRow = dtCustomers.AsEnumerable().FirstOrDefault(r => r["Account"].ToString() == custId);
                        catId = custRow != null && custRow.Table.Columns.Contains("CustomerCategoryId") && custRow["CustomerCategoryId"] != DBNull.Value
                            ? custRow["CustomerCategoryId"].ToString()
                            : null;
                    }

                    decimal priRate = defaultPriRate;
                    decimal? secRate = defaultSecRate;
                    if (!string.IsNullOrWhiteSpace(catId) && _categoryPriceMap.TryGetValue($"{itemId}_{catId}", out var catPrice))
                    {
                        priRate = catPrice.PriRate;
                        secRate = catPrice.SecRate ?? defaultSecRate;
                    }

                    dgvSale.Rows[rowIndex].Cells[clnRate.Index].Value = priRate.ToString("0.##");

                    if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                    {
                        if (dgvSale.Rows[rowIndex].Cells["clnSecQty"].Value == null || string.IsNullOrWhiteSpace(dgvSale.Rows[rowIndex].Cells["clnSecQty"].Value.ToString()))
                        {
                            dgvSale.Rows[rowIndex].Cells["clnSecQty"].Value = "0";
                        }
                        dgvSale.Rows[rowIndex].Cells["clnSecRate"].Value = (secRate ?? 0).ToString("0.##");
                    }
                }
            }
        }

        private void RecalculateRow(int rowIndex, int editedColumnIndex = -1)
        {
            if (rowIndex < 0 || rowIndex >= dgvSale.Rows.Count)
                return;

            DataGridViewRow row = dgvSale.Rows[rowIndex];
            if (editedColumnIndex == clnAmount.Index)
            {
                RecalculateRowFromAmount(rowIndex);
                return;
            }
            if (row.IsNewRow)
                return;

            decimal Qty = ParseDecimal(row.Cells[clnQty.Index].Value);
            DataRow headerItemRow = GetSelectedHeaderItemRow();
            string selectedUnitId = Convert.ToString(row.Cells[clnUnit.Index].Value);
            decimal baseQty = UnitConversionHelper.ConvertToBasePrimaryQty(Qty, selectedUnitId, headerItemRow, dtUnits);

            decimal Rate = ParseDecimal(row.Cells[clnRate.Index].Value);
            decimal Discount = ParseDecimal(row.Cells[clnDiscount.Index].Value);
            decimal DiscountPercent = ParseDecimal(row.Cells[clnDiscPercent.Index].Value);
            decimal Carriage = ParseDecimal(row.Cells[clnCarriage.Index].Value);
            decimal AddLess = ParseDecimal(row.Cells[clnAddLess.Index].Value);

            decimal secQty = 0;
            decimal secRate = 0;
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                secQty = ParseDecimal(row.Cells["clnSecQty"].Value);
                secRate = ParseDecimal(row.Cells["clnSecRate"].Value);
            }

            decimal lineGross = (baseQty * Rate) + (secQty * secRate);
            if (editedColumnIndex == clnDiscPercent.Index || (!clnDiscount.Visible && clnDiscPercent.Visible))
            {
                Discount = decimal.Round(lineGross * (DiscountPercent / 100), 2);
                row.Cells[clnDiscount.Index].Value = Discount.ToString("0.##");
            }
            else
            {
                DiscountPercent = lineGross == 0 ? 0 : decimal.Round((Discount / lineGross) * 100, 2);
                row.Cells[clnDiscPercent.Index].Value = DiscountPercent.ToString("0.##");
            }

            row.Cells[clnQty.Index].Value = Qty.ToString();
            row.Cells[clnRate.Index].Value = Rate.ToString();
            row.Cells[clnDiscount.Index].Value = Discount.ToString();
            row.Cells[clnDiscPercent.Index].Value = DiscountPercent.ToString();
            row.Cells[clnCarriage.Index].Value = Carriage.ToString();
            row.Cells[clnAddLess.Index].Value = AddLess.ToString();
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                row.Cells["clnSecQty"].Value = secQty.ToString();
                row.Cells["clnSecRate"].Value = secRate.ToString();
            }

            decimal lineAmount = (lineGross - Discount) + Carriage + AddLess;
            row.Cells[clnAmount.Index].Value = decimal.Round(lineAmount, 2).ToString();
        }

        private void RecalculateRowFromAmount(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvSale.Rows.Count)
                return;

            DataGridViewRow row = dgvSale.Rows[rowIndex];
            if (row.IsNewRow)
                return;

            decimal amount = ParseDecimal(row.Cells[clnAmount.Index].Value);
            decimal Rate = ParseDecimal(row.Cells[clnRate.Index].Value);
            decimal Discount = ParseDecimal(row.Cells[clnDiscount.Index].Value);
            decimal DiscountPercent = ParseDecimal(row.Cells[clnDiscPercent.Index].Value);
            decimal Carriage = ParseDecimal(row.Cells[clnCarriage.Index].Value);
            decimal AddLess = ParseDecimal(row.Cells[clnAddLess.Index].Value);

            // Reset secondary qty to zero on manual amount change
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                row.Cells["clnSecQty"].Value = "0";
            }

            decimal netGross = amount - Carriage - AddLess;
            decimal lineGross = 0m;
            if (!clnDiscount.Visible && clnDiscPercent.Visible && DiscountPercent > 0m && DiscountPercent < 100m)
            {
                decimal factor = 1m - (DiscountPercent / 100m);
                lineGross = factor > 0m ? netGross / factor : 0m;
                Discount = decimal.Round(lineGross * (DiscountPercent / 100m), 2);
                row.Cells[clnDiscount.Index].Value = Discount.ToString("0.##");
            }
            else
            {
                lineGross = netGross + Discount;
                if (lineGross != 0m)
                {
                    DiscountPercent = decimal.Round((Discount / lineGross) * 100m, 2);
                    row.Cells[clnDiscPercent.Index].Value = DiscountPercent.ToString("0.##");
                }
            }

            if (Rate > 0m)
            {
                decimal baseQty = Math.Max(0m, lineGross / Rate);

                DataRow headerItemRow = GetSelectedHeaderItemRow();
                string selectedUnitId = Convert.ToString(row.Cells[clnUnit.Index].Value);
                decimal unitFactor = 1m;
                if (headerItemRow != null && !string.IsNullOrWhiteSpace(selectedUnitId))
                {
                    var selectableUnits = UnitConversionHelper.GetSelectableUnitsTable(headerItemRow, dtUnits);
                    var matchedUnit = selectableUnits.Select("Code = '" + selectedUnitId.Replace("'", "''") + "'").FirstOrDefault();
                    if (matchedUnit != null && matchedUnit.Table.Columns.Contains("Factor"))
                    {
                        decimal.TryParse(Convert.ToString(matchedUnit["Factor"]), out unitFactor);
                        if (unitFactor <= 0m) unitFactor = 1m;
                    }
                }

                decimal displayQty = Math.Round(baseQty * unitFactor, 4, MidpointRounding.AwayFromZero);
                row.Cells[clnQty.Index].Value = displayQty.ToString("0.####");
            }

            row.Cells[clnAmount.Index].Value = decimal.Round(amount, 2).ToString();
        }


        void tbRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQty.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscount.Index ||
                 dgvSale.CurrentCell.ColumnIndex == clnDiscPercent.Index ||
                 dgvSale.CurrentCell.ColumnIndex == clnCarriage.Index ||
                 dgvSale.CurrentCell.ColumnIndex == clnAddLess.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnAmount.Index ||
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

                bool isAddLessColumn = dgvSale.CurrentCell.ColumnIndex == clnAddLess.Index;

                if (!char.IsControl(e.KeyChar)
           && !char.IsDigit(e.KeyChar)
           && !((sender as TextBox).Text.Count(a => a == '.') == 0 && e.KeyChar == '.')
           && !(isAddLessColumn && e.KeyChar == '-' && (sender as TextBox).SelectionStart == 0 && !(sender as TextBox).Text.Contains('-')))
                {
                    e.Handled = true;
                }
            }
        }

        private void SetclnUnitSource()
        {
            DataRow headerItemRow = GetSelectedHeaderItemRow();
            var selectableUnits = UnitConversionHelper.GetSelectableUnitsTable(headerItemRow, dtUnits);
            bool hasMulti = selectableUnits.Rows.Count > 1;
            clnUnit.Visible = hasMulti;
            if (hasMulti)
            {
                clnUnit.HeaderText = "Unit";
                clnUnit.Width = 65;
                clnUnit.ReadOnly = false;
                try
                {
                    clnUnit.DisplayIndex = clnQty.DisplayIndex + 1;
                }
                catch { }
            }
            for (int i = 0; i < dgvSale.Rows.Count; i++)
            {
                string currentUnit = Convert.ToString(dgvSale.Rows[i].Cells[clnUnit.Index].Value);
                UnitConversionHelper.BindRowUnitComboBox(dgvSale.Rows[i], clnUnit.Index, headerItemRow, dtUnits, currentUnit);
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
                e.Handled = true;
                HandleGridEnterNavigation();
            }
        }

        private bool HandleGridEnterNavigation()
        {
            if (dgvSale.CurrentCell == null) return false;

            int rowIndex = dgvSale.CurrentCell.RowIndex;
            int colIndex = dgvSale.CurrentCell.ColumnIndex;

            // 1. If on Customer column
            if (colIndex == clnCustomer.Index)
            {
                if (dgvSale.EditingControl is ComboBox cmb)
                {
                    if (cmb.SelectedItem is DataRowView drv && drv.Row.Table.Columns.Contains("Account"))
                    {
                        string id = drv["Account"].ToString();
                        dgvSale[clnCustomer.Index, rowIndex].Value = id;
                    }
                    else if (!string.IsNullOrWhiteSpace(cmb.Text))
                    {
                        string text = cmb.Text.Trim();
                        var match = dtCustomers.AsEnumerable().FirstOrDefault(r =>
                            string.Equals(r["Title"]?.ToString(), text, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(r["Account"]?.ToString(), text, StringComparison.OrdinalIgnoreCase));
                        if (match == null)
                        {
                            match = dtCustomers.AsEnumerable().FirstOrDefault(r =>
                                r["Title"]?.ToString().StartsWith(text, StringComparison.OrdinalIgnoreCase) == true);
                        }
                        if (match != null)
                        {
                            string id = match["Account"].ToString();
                            dgvSale[clnCustomer.Index, rowIndex].Value = id;
                        }
                    }
                }

                dgvSale.EndEdit();

                var custVal = dgvSale[clnCustomer.Index, rowIndex].Value;
                bool isCustEmpty = custVal == null || string.IsNullOrWhiteSpace(custVal.ToString());
                bool isLastRow = (rowIndex == dgvSale.Rows.Count - 1);

                // If on last row without selecting customer -> Exit grid to btnSave!
                if (isLastRow && isCustEmpty)
                {
                    dgvSale.EndEdit();
                    CalcTotals();
                    btnSave.Focus();
                    return true;
                }

                // If customer is selected
                if (!isCustEmpty)
                {
                    SetRateForRow(rowIndex);
                    string custId = custVal.ToString();
                    // SetCarriageDiscForCustomer

                    if (dgvSale[clnQty.Index, rowIndex].Value == null ||
                        string.IsNullOrWhiteSpace(dgvSale[clnQty.Index, rowIndex].Value.ToString()) ||
                        dgvSale[clnQty.Index, rowIndex].Value.ToString() == "0")
                    {
                        dgvSale[clnQty.Index, rowIndex].Value = "1";
                    }
                    CalcTotals();

                    // Check if unit is visible and HAS MORE THAN ONE UNIT (not ReadOnly)
                    bool hasMultipleUnits = false;
                    if (clnUnit != null && clnUnit.Visible && rowIndex >= 0 && rowIndex < dgvSale.Rows.Count)
                    {
                        var unitCell = dgvSale.Rows[rowIndex].Cells[clnUnit.Index] as DataGridViewComboBoxCell;
                        if (unitCell != null && !unitCell.ReadOnly)
                        {
                            if (unitCell.DataSource is DataTable dtUnitChoices)
                            {
                                hasMultipleUnits = dtUnitChoices.Rows.Count > 1;
                            }
                            else if (unitCell.Items != null)
                            {
                                hasMultipleUnits = unitCell.Items.Count > 1;
                            }
                        }
                    }

                    if (hasMultipleUnits)
                    {
                        dgvSale.CurrentCell = dgvSale[clnUnit.Index, rowIndex];
                    }
                    else
                    {
                        dgvSale.CurrentCell = dgvSale[clnQty.Index, rowIndex];
                    }
                    dgvSale.BeginEdit(true);
                    return true;
                }

                return true;
            }

            // 2. If on Unit column
            if (clnUnit != null && colIndex == clnUnit.Index)
            {
                if (dgvSale.EditingControl is ComboBox cmbUnit)
                {
                    string unitCode = cmbUnit.SelectedValue?.ToString();
                    if (string.IsNullOrEmpty(unitCode) && cmbUnit.SelectedItem is DataRowView drv && drv.Row.Table.Columns.Contains("Code"))
                    {
                        unitCode = drv["Code"]?.ToString();
                    }
                    if (!string.IsNullOrEmpty(unitCode))
                    {
                        dgvSale[clnUnit.Index, rowIndex].Value = unitCode;
                    }
                }

                dgvSale.EndEdit();
                CalcTotals();
                dgvSale.CurrentCell = dgvSale[clnQty.Index, rowIndex];
                dgvSale.BeginEdit(true);
                return true;
            }

            // 3. If on Qty column
            if (colIndex == clnQty.Index)
            {
                dgvSale.EndEdit();
                CalcTotals();

                bool hasSecQty = ApiSession.HasSecondaryQty &&
                                 dgvSale.Columns.Contains("clnSecQty") &&
                                 dgvSale.Columns["clnSecQty"].Visible;

                if (hasSecQty)
                {
                    dgvSale.CurrentCell = dgvSale.Rows[rowIndex].Cells[dgvSale.Columns["clnSecQty"].Index];
                    dgvSale.BeginEdit(true);
                    return true;
                }

                // Move to next row's Customer
                int nextRowIdx;
                if (rowIndex == dgvSale.Rows.Count - 1)
                {
                    nextRowIdx = dgvSale.Rows.Add();
                    ClearRowValues(nextRowIdx);
                }
                else
                {
                    nextRowIdx = rowIndex + 1;
                }
                dgvSale.CurrentCell = dgvSale[clnCustomer.Index, nextRowIdx];
                dgvSale.BeginEdit(true);
                return true;
            }

            // 4. If on Qty Pack (clnSecQty) column or rate/disc columns
            bool isSecQtyCol = dgvSale.Columns.Contains("clnSecQty") && colIndex == dgvSale.Columns["clnSecQty"].Index;
            if (isSecQtyCol || colIndex == clnRate.Index || colIndex == clnDiscPercent.Index || colIndex == clnDiscount.Index)
            {
                dgvSale.EndEdit();
                CalcTotals();

                // Move to next row's Customer
                int nextRowIdx;
                if (rowIndex == dgvSale.Rows.Count - 1)
                {
                    nextRowIdx = dgvSale.Rows.Add();
                    ClearRowValues(nextRowIdx);
                }
                else
                {
                    nextRowIdx = rowIndex + 1;
                }
                dgvSale.CurrentCell = dgvSale[clnCustomer.Index, nextRowIdx];
                dgvSale.BeginEdit(true);
                return true;
            }

            // 5. If on Amount column
            if (colIndex == clnAmount.Index)
            {
                dgvSale.EndEdit();
                RecalculateRowFromAmount(rowIndex);
                CalcTotals();

                // Move to next row's Customer
                int nextRowIdx;
                if (rowIndex == dgvSale.Rows.Count - 1)
                {
                    nextRowIdx = dgvSale.Rows.Add();
                    ClearRowValues(nextRowIdx);
                }
                else
                {
                    nextRowIdx = rowIndex + 1;
                }
                dgvSale.CurrentCell = dgvSale[clnCustomer.Index, nextRowIdx];
                dgvSale.BeginEdit(true);
                return true;
            }

            // Fallback
            dgvSale.EndEdit();
            CalcTotals();
            int targetIdx;
            if (rowIndex < dgvSale.Rows.Count - 1)
            {
                targetIdx = rowIndex + 1;
            }
            else
            {
                targetIdx = dgvSale.Rows.Add();
                ClearRowValues(targetIdx);
            }
            dgvSale.CurrentCell = dgvSale[clnCustomer.Index, targetIdx];
            dgvSale.BeginEdit(true);
            return true;
        }

        private void dgvPurchase_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AllowNewRow();
            resetRow = true;
            currentRow = e.RowIndex;

            RecalculateRow(e.RowIndex, e.ColumnIndex);
            CalcTotAmount();

            bool isRateColumn = e.ColumnIndex == clnRate.Index ||
                (dgvSale.Columns.Contains("clnSecRate") && e.ColumnIndex == dgvSale.Columns["clnSecRate"].Index);

            string currentSelectedId = cmbItem.SelectedItem is DataRowView drv && drv.Row.Table.Columns.Contains("Id")
                ? drv["Id"]?.ToString()
                : cmbItem.SelectedValue?.ToString();

            if (isRateColumn && !string.IsNullOrEmpty(_originalItemId) &&
                string.Equals(currentSelectedId?.Trim(), _originalItemId?.Trim(), StringComparison.OrdinalIgnoreCase) &&
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
                    savedRate.Rate = ParseDecimal(row.Cells[clnRate.Index].Value);
                    if (dgvSale.Columns.Contains("clnSecRate"))
                    {
                        savedRate.SecRate = ParseDecimal(row.Cells["clnSecRate"].Value);
                    }
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

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving)
                return;

            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to save this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _isSaving = true;
                btnSave.Enabled = false;
                try
                {
                    await SaveAsync();
                }
                finally
                {
                    _isSaving = false;
                    btnSave.Enabled = true;
                }
            }
        }

        private async void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                string voucher = txtFilterVoucher.Text;
                if (!string.IsNullOrWhiteSpace(voucher) && voucher.StartsWith("SP-", StringComparison.OrdinalIgnoreCase))
                    voucher = voucher.Substring(3);

                await FillQueryAsync(
                    txtFdate.Text,
                    txtTdate.Text,
                    cmbFilterAccounts.SelectedValue != null ? cmbFilterAccounts.SelectedValue.ToString() : "",
                    voucher);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sale supply data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (!Regex.IsMatch(txtFilterVoucher.Text, @"SP-\d{5}"))
                txtFilterVoucher.Text = "";
        }

        private async void dgvQuery_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!FLogIn && e.RowIndex > -1)
            {
                string voucherNo = dgvQuery.Rows[e.RowIndex].Cells[clnVoucherNum.Index].Value.ToString();
                tbSaleQuery.SelectedTab = tbDetail;
                await FillSaleAsync(voucherNo.Substring(3));
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtVoucherNo.Text = "";
            VoucherNum = null;
            _originalItemId = null;
            dtpDate.Value = DateTime.Now;
            txtCreatedBy.Text = "";
            txtEditBy.Text = "";
            txtDescription.Text = "";
            dgvSale.Rows.Clear();
            int newRow = dgvSale.Rows.Add();
            ClearRowValues(newRow);
            dtpDate.Focus();
            cmbItem.SelectedIndex = -1;
            cmbNarration.SelectedIndex = dtNarration.Rows.Count > 0 ? 0 : -1;
            cmbSupplyOrder.SelectedIndex = -1;
            txtSearchCustomer.Text = "";
            CalcTotAmount();
        }

        void Navigate(Navigators Nav)
        {
            if (dgvQuery.CurrentRow != null && tbSaleQuery.SelectedTab == tbDetail)
            {
                int rowIndex = dgvQuery.CurrentRow.Index;
                if (Nav == Navigators.Down && dgvQuery.Rows.Count - 1 > rowIndex)
                {
                    dgvQuery.CurrentCell = dgvQuery.Rows[rowIndex + 1].Cells[clnVoucherNum.Index];
                    string voucherNo = dgvQuery.CurrentCell.Value.ToString();
                    FillSale(voucherNo.Substring(3));
                }
                else if (Nav == Navigators.Up && 0 < rowIndex)
                {
                    dgvQuery.CurrentCell = dgvQuery.Rows[rowIndex - 1].Cells[clnVoucherNum.Index];
                    string voucherNo = dgvQuery.CurrentCell.Value.ToString();
                    FillSale(voucherNo.Substring(3));
                }
                else if (Nav == Navigators.Home)
                {
                    dgvQuery.CurrentCell = dgvQuery.Rows[0].Cells[clnVoucherNum.Index];
                    string voucherNo = dgvQuery.CurrentCell.Value.ToString();
                    FillSale(voucherNo.Substring(3));
                }
                else if (Nav == Navigators.End)
                {
                    dgvQuery.CurrentCell = dgvQuery.Rows[dgvQuery.Rows.Count - 1].Cells[clnVoucherNum.Index];
                    string voucherNo = dgvQuery.CurrentCell.Value.ToString();
                    FillSale(voucherNo.Substring(3));
                }
            }
        }

        #region Navigation
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
        #endregion

        private void tbSaleQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown)
            {
                Navigate(Navigators.Down);
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                Navigate(Navigators.Up);
            }
            else if (e.KeyCode == Keys.Home)
            {
                Navigate(Navigators.Home);
            }
            else if (e.KeyCode == Keys.End)
            {
                Navigate(Navigators.End);
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
                        await _apiService.DeleteAsync(txtVoucherNo.Text.Substring(3));

                        DataGridViewRow row = dgvQuery.Rows.Cast<DataGridViewRow>()
                            .FirstOrDefault(r => r.Cells[clnVoucherNum.Index].Value.ToString().Equals(txtVoucherNo.Text));

                        btnNew_Click(null, null);
                        if (row != null)
                        {
                            dgvQuery.Rows.Remove(row);
                        }

                        CalcTotAmount();
                        MessageBox.Show("Record Successfully Deleted..!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting sale supply: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
            this.Close();
        }

        private void txtSearchCustomer_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearchCustomer.Text.Trim();

            foreach (DataGridViewRow row in dgvSale.Rows)
            {
                if (row.IsNewRow) continue;

                if (string.IsNullOrEmpty(search))
                {
                    row.Visible = true;
                    continue;
                }

                // Resolve display text for the customer ComboBox column
                string customerDisplay = string.Empty;
                object cellValue = row.Cells[clnCustomer.Index].Value;
                if (cellValue != null && !string.IsNullOrEmpty(cellValue.ToString()))
                {
                    // Look up the title from dtCustomers using the stored account id
                    DataRow[] matches = dtCustomers.Select("Account = '" + cellValue.ToString().Replace("'", "''") + "'");
                    if (matches.Length > 0)
                        customerDisplay = matches[0]["Title"].ToString();
                    else
                        customerDisplay = cellValue.ToString();
                }

                row.Visible = customerDisplay.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        private void tbDetail_Click(object sender, EventArgs e)
        {
        }

        private void dgvSale_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        void CopyAsNew()
        {
            if (txtVoucherNo.Text != "")
            {
                txtVoucherNo.Text = "";
                VoucherNum = "";
                _originalItemId = null;
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

        private async void btnAddNewItem_Click(object sender, EventArgs e)
        {
            frmItemDetail frm = new frmItemDetail();
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await LoadLookupsAsync();
            }
        }

        private async void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetclnUnitSource();

            DataRowView dr = cmbItem.SelectedItem as DataRowView;
            string itemId = dr != null && dr.Row.Table.Columns.Contains("Id")
                ? dr["Id"]?.ToString()
                : cmbItem.SelectedValue?.ToString();

            if (string.IsNullOrWhiteSpace(itemId))
                return;

            string primaryUnit = (dr != null && dr.Row.Table.Columns.Contains("PrimaryUnit") && dr["PrimaryUnit"] != DBNull.Value)
                ? dr["PrimaryUnit"].ToString()
                : string.Empty;

            string originalId = _originalItemId?.Trim();
            bool isSameAsOriginal = !string.IsNullOrEmpty(originalId) &&
                                    string.Equals(itemId.Trim(), originalId, StringComparison.OrdinalIgnoreCase);

            Dictionary<string, CustomerSupplyItemDto> itemQtyMap = new Dictionary<string, CustomerSupplyItemDto>();
            if (!isSameAsOriginal)
            {
                try
                {
                    var customSupplyItems = await _customerApiService.GetSupplyItemsAsync(null, itemId);

                    DataRowView nowDr = cmbItem.SelectedItem as DataRowView;
                    string nowId = nowDr != null && nowDr.Row.Table.Columns.Contains("Id")
                        ? nowDr["Id"]?.ToString()
                        : cmbItem.SelectedValue?.ToString();

                    if (!string.Equals(nowId?.Trim(), itemId.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return; // User already selected another item while async call was fetching
                    }

                    if (customSupplyItems != null)
                    {
                        itemQtyMap = customSupplyItems
                            .Where(x => !string.IsNullOrEmpty(x.CustomerAccountId))
                            .GroupBy(x => x.CustomerAccountId)
                            .ToDictionary(g => g.Key, g => g.First());
                    }
                }
                catch
                {
                }
            }

            DataRow headerItemRow = GetSelectedHeaderItemRow();

            foreach (DataGridViewRow row in dgvSale.Rows)
                    {
                        if (row.IsNewRow) continue;

                        if (!isSameAsOriginal)
                        {
                            UnitConversionHelper.BindRowUnitComboBox(row, clnUnit.Index, headerItemRow, dtUnits, primaryUnit);
                        }
                        else
                        {
                            string existingUnit = Convert.ToString(row.Cells[clnUnit.Index].Value);
                            UnitConversionHelper.BindRowUnitComboBox(row, clnUnit.Index, headerItemRow, dtUnits, !string.IsNullOrWhiteSpace(existingUnit) ? existingUnit : primaryUnit);
                        }

                        if (isSameAsOriginal && row.Tag is SavedRowRateInfo savedRate)
                        {
                            row.Cells[clnRate.Index].Value = savedRate.Rate.ToString("0.##");
                            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                            {
                                if (row.Cells["clnSecQty"].Value == null || string.IsNullOrWhiteSpace(row.Cells["clnSecQty"].Value.ToString()))
                                {
                                    row.Cells["clnSecQty"].Value = "0";
                                }
                                row.Cells["clnSecRate"].Value = savedRate.SecRate.ToString("0.##");
                            }
                        }
                        else
                        {
                            SetRateForRow(row.Index);
                        }

                        bool hasExistingQty = row.Cells[clnQty.Index].Value != null &&
                                              !string.IsNullOrWhiteSpace(row.Cells[clnQty.Index].Value.ToString());

                        if (!hasExistingQty)
                        {
                            string custId = row.Cells[clnCustomer.Index].Value?.ToString();
                            if (!string.IsNullOrEmpty(custId) && itemQtyMap.TryGetValue(custId, out var customSetting))
                            {
                                decimal rawDefaultQty = customSetting.Qty > 0 ? customSetting.Qty : 1;
                                decimal dispQty = rawDefaultQty;
                                string dispUnitId = primaryUnit;
                                if (headerItemRow != null)
                                {
                                    UnitConversionHelper.ResolveDisplayQtyAndUnit(rawDefaultQty, headerItemRow, dtUnits, out dispQty, out dispUnitId);
                                    UnitConversionHelper.BindRowUnitComboBox(row, clnUnit.Index, headerItemRow, dtUnits, dispUnitId);
                                }
                                row.Cells[clnQty.Index].Value = dispQty.ToString("0.####");
                                if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                                {
                                    bool hasExistingSecQty = row.Cells["clnSecQty"].Value != null &&
                                                             !string.IsNullOrWhiteSpace(row.Cells["clnSecQty"].Value.ToString());
                                    if (!hasExistingSecQty)
                                        row.Cells["clnSecQty"].Value = (customSetting.SecQty ?? 0).ToString("0.##");
                                }
                                if (customSetting.Discount.HasValue)
                                    row.Cells[clnDiscount.Index].Value = customSetting.Discount.Value.ToString("0.##");
                                if (customSetting.Carriage.HasValue)
                                    row.Cells[clnCarriage.Index].Value = customSetting.Carriage.Value.ToString("0.##");
                                if (customSetting.AddLess.HasValue)
                                    row.Cells[clnAddLess.Index].Value = customSetting.AddLess.Value.ToString("0.##");
                            }
                        }

                        // SetRateForRow sets item default; override with custom rate afterwards if defined
                        if (!isSameAsOriginal &&
                            !string.IsNullOrEmpty(row.Cells[clnCustomer.Index].Value?.ToString()) &&
                            itemQtyMap.TryGetValue(row.Cells[clnCustomer.Index].Value.ToString(), out var rateSetting) &&
                            rateSetting.Rate.HasValue)
                        {
                            row.Cells[clnRate.Index].Value = rateSetting.Rate.Value.ToString("0.##");
                        }

                        RecalculateRow(row.Index);
                    }

                    CalcTotAmount();
        }

        private async void cmbSupplyOrder_Leave(object sender, EventArgs e)
        {
            if (cmbSupplyOrder.SelectedIndex != -1 && cmbSupplyOrder.SelectedValue != null)
            {
                bool isNewVoucher = string.IsNullOrEmpty(txtVoucherNo.Text);
                bool hasNoRecords = dgvSale.Rows.Count <= 1;

                if (!isNewVoucher || !hasNoRecords)
                {
                    return;
                }

                try
                {
                    dgvSale.Rows.Clear();
                    int supplyOrderId = Convert.ToInt32(cmbSupplyOrder.SelectedValue);
                    string currentItemId = cmbItem.SelectedValue != null ? cmbItem.SelectedValue.ToString() : null;

                    var supplyOrderTask = _supplyOrderApiService.GetByIdAsync(supplyOrderId);
                    var supplyItemsTask = !string.IsNullOrWhiteSpace(currentItemId) 
                        ? _customerApiService.GetSupplyItemsAsync(null, currentItemId) 
                        : Task.FromResult(new List<CustomerSupplyItemDto>());

                    await Task.WhenAll(supplyOrderTask, supplyItemsTask);

                    var supplyOrder = supplyOrderTask.Result;
                    var customSupplyItems = supplyItemsTask.Result;
                    var itemQtyMap = (customSupplyItems ?? new List<CustomerSupplyItemDto>())
                        .Where(x => !string.IsNullOrEmpty(x.CustomerAccountId))
                        .GroupBy(x => x.CustomerAccountId)
                        .ToDictionary(g => g.Key, g => g.First());

                    if (supplyOrder != null && supplyOrder.Details != null)
                    {
                        DataRow headerItemRow = GetSelectedHeaderItemRow();
                        for (int i = 0; i < supplyOrder.Details.Count; i++)
                        {
                            int idx = dgvSale.Rows.Add();
                            dgvSale.Rows[idx].Cells[clnSeq.Index].Value = (idx + 1).ToString();
                            string custId = supplyOrder.Details[i].CustomerId;
                            dgvSale.Rows[idx].Cells[clnCustomer.Index].Value = custId;

                            decimal qty = 1;
                            decimal secQty = 0;
                            if (!string.IsNullOrEmpty(custId) && itemQtyMap.TryGetValue(custId, out var customSetting))
                            {
                                qty = customSetting.Qty > 0 ? customSetting.Qty : 1;
                                secQty = customSetting.SecQty ?? 0;
                            }

                            decimal dispQty = qty;
                            string dispUnit = headerItemRow != null ? Convert.ToString(headerItemRow["PrimaryUnit"]) : "";
                            if (headerItemRow != null)
                            {
                                UnitConversionHelper.ResolveDisplayQtyAndUnit(qty, headerItemRow, dtUnits, out dispQty, out dispUnit);
                                UnitConversionHelper.BindRowUnitComboBox(dgvSale.Rows[idx], clnUnit.Index, headerItemRow, dtUnits, dispUnit);
                                SetRateForRow(idx);
                            }

                            dgvSale.Rows[idx].Cells[clnQty.Index].Value = dispQty.ToString("0.####");
                            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                            {
                                dgvSale.Rows[idx].Cells["clnSecQty"].Value = secQty.ToString("0.##");
                            }

                            // Apply rate / discount / addLess overrides after SetRateForRow
                            if (!string.IsNullOrEmpty(custId) && itemQtyMap.TryGetValue(custId, out var overrideSetting))
                            {
                                if (overrideSetting.Rate.HasValue)
                                    dgvSale.Rows[idx].Cells[clnRate.Index].Value = overrideSetting.Rate.Value.ToString("0.##");
                                if (overrideSetting.Discount.HasValue)
                                    dgvSale.Rows[idx].Cells[clnDiscount.Index].Value = overrideSetting.Discount.Value.ToString("0.##");
                                if (overrideSetting.Carriage.HasValue)
                                    dgvSale.Rows[idx].Cells[clnCarriage.Index].Value = overrideSetting.Carriage.Value.ToString("0.##");
                                if (overrideSetting.AddLess.HasValue)
                                    dgvSale.Rows[idx].Cells[clnAddLess.Index].Value = overrideSetting.AddLess.Value.ToString("0.##");
                            }

                            RecalculateRow(idx);
                        }
                    }

                    dgvSale.Rows.Add();
                    CalcTotAmount();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading supply order details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void lblSearchCustomer_Click(object sender, EventArgs e)
        {

        }

        private async void btnScanSheet_Click(object sender, EventArgs e)
        {
            DateTime voucherDate = dtpDate.Value;
            using (var dlg = new ERP.Forms.frmScanSupplySheet(voucherDate, dtCustomers))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK && dlg.ImportedLines.Count > 0)
                {
                    if (dgvSale.Rows.Count == 1)
                    {
                        var firstRow = dgvSale.Rows[0];
                        var custVal = firstRow.Cells[clnCustomer.Index].Value?.ToString();
                        if (string.IsNullOrWhiteSpace(custVal))
                        {
                            dgvSale.Rows.Clear();
                        }
                    }

                    var headerItemRow = dtItems.AsEnumerable().FirstOrDefault(r => r.Field<string>("Id") == cmbItem.SelectedValue?.ToString());

                    foreach (var line in dlg.ImportedLines)
                    {
                        int newRowIdx = dgvSale.Rows.Add();
                        var row = dgvSale.Rows[newRowIdx];
                        row.Cells[clnSeq.Index].Value = (newRowIdx + 1).ToString();
                        row.Cells[clnStatus.Index].Value = "0";

                        if (!string.IsNullOrWhiteSpace(line.MatchedCustomerId))
                        {
                            row.Cells[clnCustomer.Index].Value = line.MatchedCustomerId;
                        }

                        if (headerItemRow != null)
                        {
                            UnitConversionHelper.BindRowUnitComboBox(row, clnUnit.Index, headerItemRow, dtUnits, headerItemRow.Field<string>("DefaultUnit") ?? headerItemRow.Field<string>("PrimaryUnit"));
                        }

                        row.Cells[clnQty.Index].Value = line.Qty > 0 ? line.Qty.ToString("0.####") : "1";
                        row.Cells[clnRate.Index].Value = line.Rate.ToString("0.##");
                        row.Cells[clnDiscount.Index].Value = line.Discount.ToString("0.##");
                        RecalculateRow(newRowIdx);
                    }

                    dgvSale.Rows.Add();
                    CalcTotals();

                    if (!string.IsNullOrWhiteSpace(VoucherNum) && VoucherNum != "NEW" && dlg.ScannedFilePaths.Count > 0)
                    {
                        try
                        {
                            foreach (var path in dlg.ScannedFilePaths)
                            {
                                var mediaId = await _apiService.UploadAttachmentDirectAsync(path);
                                await _apiService.AddAttachmentAsync(VoucherNum, mediaId, System.IO.Path.GetFileName(path));
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, "Lines imported, but failed to upload attachments: " + ex.Message, "Attachment Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                    MessageBox.Show(this, $"Successfully imported {dlg.ImportedLines.Count} line(s) into voucher.", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
