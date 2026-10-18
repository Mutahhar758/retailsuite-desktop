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
    public partial class frmWandaSaleSupply : Form
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

        public frmWandaSaleSupply()
        {
            InitializeComponent();
            _apiService = new SaleSupplyApiService();
            _supplyOrderApiService = new SupplyOrderApiService();
            _customerApiService = new CustomerApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _narrationApiService = new NarrationApiService();
            _unitApiService = new UnitApiService();
            _inventoryApiService = new InventoryApiService();
            InitializeLookupTables();
            dgvSale.Rows.Add();
            dgvSale.RowsRemoved += (s, e) => CalcTotals();
            UserInfo.ApplyFormPermissions(this, AppResource.SaleSupplies);
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
            dtItems.Columns.Add("QtyInPack", typeof(decimal));

            dtUnits.Columns.Add("Code", typeof(string));
            dtUnits.Columns.Add("Title", typeof(string));

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
                dtItems.Rows.Add(item.Id, item.Title, item.PriRate, item.SecRate, item.PrimaryUnit, item.SecondaryUnit, item.DefaultUnit, item.QtyInPack);

            dtUnits.Rows.Clear();
            foreach (var u in unitsTask.Result)
                dtUnits.Rows.Add(u.Code, u.Title);

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

                    totQty += ParseDecimal(row.Cells[clnQty.Index].Value);
                    if (dgvSale.Columns.Contains("clnSecQty"))
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
            if (totSecQty > 0)
            {
                txtTotQty.Text = totQty.ToString("N2") + " Kg (" + totSecQty.ToString("N0") + " Bags)";
            }
            else
            {
                txtTotQty.Text = totQty.ToString("N2") + " Kg";
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
            dgvSale.EndEdit();

            if (cmbItem.SelectedIndex == -1 || cmbItem.SelectedValue == null || string.IsNullOrWhiteSpace(cmbItem.SelectedValue.ToString()))
            {
                MessageBox.Show("Please Select Item...!");
                return;
            }

            var lines = new List<SaleSupplyLineApiRequest>();
            foreach (DataGridViewRow row in dgvSale.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string custId = Convert.ToString(row.Cells[clnCustomer.Index].Value);
                if (string.IsNullOrWhiteSpace(custId))
                    continue;

                decimal qty = ParseDecimal(row.Cells[clnQty.Index].Value);
                decimal discount = ParseDecimal(row.Cells[clnDiscount.Index].Value);
                decimal carriage = ParseDecimal(row.Cells[clnCarriage.Index].Value);
                decimal addLess = ParseDecimal(row.Cells[clnAddLess.Index].Value);

                decimal secQty = 0;
                decimal secRate = 0;
                string secUnit = null;
                decimal? qtyInPack = null;
                decimal? packing = null;
                if (dgvSale.Columns.Contains("clnSecQty"))
                {
                    secQty = ParseDecimal(row.Cells["clnSecQty"].Value);
                    secRate = ParseDecimal(row.Cells["clnSecRate"].Value);
                    if (dgvSale.Columns.Contains("clnPackQty"))
                    {
                        decimal pQty = ParseDecimal(row.Cells["clnPackQty"].Value);
                        if (pQty > 0) qtyInPack = pQty;
                    }
                    if (dgvSale.Columns.Contains("clnPacking"))
                    {
                        decimal pck = ParseDecimal(row.Cells["clnPacking"].Value);
                        if (pck > 0) packing = pck;
                    }
                    if (packing == null || packing <= 0)
                        packing = qtyInPack ?? GetSelectedItemDefaultPacking();
                    if (qtyInPack == null || qtyInPack <= 0)
                        qtyInPack = packing;
                    
                    if (cmbItem.SelectedValue != null)
                    {
                        string itemId = Convert.ToString(cmbItem.SelectedValue);
                        DataRow itemRow = dtItems.Select("Id = '" + itemId.Replace("'", "''") + "'").FirstOrDefault();
                        if (itemRow != null)
                        {
                            secUnit = Convert.ToString(itemRow["SecondaryUnit"]);
                        }
                    }
                    if (string.IsNullOrWhiteSpace(secUnit))
                        secUnit = "Bags";
                }

                if (qty <= 0 && secQty <= 0 && addLess == 0)
                    continue;

                decimal enteredRate = ParseDecimal(row.Cells[clnRate.Index].Value);
                decimal finalRate = (row.Cells[clnRate.Index].Tag is decimal tagRate && tagRate > 0 && Math.Round(tagRate, 2) == Math.Round(enteredRate, 2))
                    ? tagRate
                    : ((packing > 0 && secRate > 0) ? Math.Round(secRate / packing.Value, 8) : enteredRate);

                int seq = int.TryParse(Convert.ToString(row.Cells[clnSeq.Index].Value), out int parsedSeq) && parsedSeq > 0
                    ? parsedSeq
                    : (lines.Count + 1);

                lines.Add(new SaleSupplyLineApiRequest
                {
                    Seq = seq,
                    CustomerId = custId,
                    Qty = qty,
                    Rate = finalRate,
                    Discount = discount,
                    Carriage = carriage,
                    AddLess = addLess,
                    SecQty = secQty,
                    SecRate = secRate,
                    SecUnit = secUnit,
                    QtyInPack = qtyInPack,
                    Packing = packing
                });
            }

            if (lines.Count == 0)
            {
                MessageBox.Show("Please add at least one row...!");
                return;
            }

            try
            {
                string rawVoucher = txtVoucherNo.Text?.Trim();
                string voucher = null;
                if (!string.IsNullOrWhiteSpace(rawVoucher))
                {
                    voucher = rawVoucher.StartsWith("SP-", StringComparison.OrdinalIgnoreCase)
                        ? rawVoucher.Substring(3)
                        : rawVoucher;
                }
                bool isNew = string.IsNullOrWhiteSpace(voucher);

                int? supplyOrderId = null;
                if (cmbSupplyOrder.SelectedIndex >= 0 && cmbSupplyOrder.SelectedValue != null)
                {
                    if (int.TryParse(cmbSupplyOrder.SelectedValue.ToString(), out int parsedSoId) && parsedSoId > 0)
                        supplyOrderId = parsedSoId;
                }

                string narration = (cmbNarration.SelectedIndex >= 0 && cmbNarration.SelectedValue != null && !string.IsNullOrWhiteSpace(cmbNarration.SelectedValue.ToString()))
                    ? cmbNarration.SelectedValue.ToString()
                    : null;

                if (isNew)
                {
                    var request = new SaleSupplyCreateApiRequest
                    {
                        Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                        ItemId = cmbItem.SelectedValue.ToString(),
                        Description = txtDescription.Text,
                        Narration = narration,
                        SupplyOrderMasterId = supplyOrderId,
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
                        Narration = narration,
                        SupplyOrderMasterId = supplyOrderId,
                        Lines = lines
                    };
                    await _apiService.UpdateAsync(voucher, request);
                }

                await FillSaleAsync(voucher);
                await FillQueryAsync();

                MessageBox.Show("Record Successfully Saved...!");
                btnNew.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving sale supply: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task FillQueryAsync(string fromDate = "", string toDate = "", string itemId = "", string voucherNo = "")
        {
            dgvQuery.Rows.Clear();
            _queryList = await _apiService.GetListAsync(fromDate, toDate, itemId, voucherNo);

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

                for (int i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    decimal discountPercent = line.Rate == 0 ? 0 : (line.Discount / line.Rate) * 100;
                    
                    int ind = dgvSale.Rows.Add();
                    var row = dgvSale.Rows[ind];
                    
                    row.Cells[clnSeq.Index].Value = line.Seq.ToString();
                    row.Cells[clnCustomer.Index].Value = line.CustomerId;
                    row.Cells[clnUnit.Index].Value = line.Unit;
                    row.Cells[clnQty.Index].Value = line.Qty.ToString("0.##");
                    row.Cells[clnRate.Index].Value = line.Rate.ToString("0.##");
                    row.Cells[clnRate.Index].Tag = line.Rate;
                    row.Cells[clnDiscount.Index].Value = line.Discount.ToString("0.##");
                    row.Cells[clnDiscPercent.Index].Value = discountPercent.ToString("0.##");
                    row.Cells[clnCarriage.Index].Value = (line.Carriage ?? 0).ToString("0.##");
                    row.Cells[clnAddLess.Index].Value = line.AddLess.ToString("0.##");
                    row.Cells[clnAmount.Index].Value = line.Amount.ToString("N2");
                    row.Cells[clnStatus.Index].Value = "0";

                    if (dgvSale.Columns.Contains("clnSecQty"))
                    {
                        row.Cells["clnSecQty"].Value = (line.SecQty ?? 0).ToString("0.##");
                        row.Cells["clnSecRate"].Value = (line.SecRate ?? 0).ToString("0.##");
                        decimal packingVal = (line.Packing.HasValue && line.Packing.Value > 0)
                            ? line.Packing.Value
                            : ((line.QtyInPack.HasValue && line.QtyInPack.Value > 0) ? line.QtyInPack.Value : 50);
                        if (dgvSale.Columns.Contains("clnPacking"))
                        {
                            row.Cells["clnPacking"].Value = packingVal.ToString("0.##");
                        }
                        if (dgvSale.Columns.Contains("clnPackQty"))
                        {
                            decimal packVal = (line.QtyInPack.HasValue && line.QtyInPack.Value > 0)
                                ? line.QtyInPack.Value
                                : (line.SecQty.HasValue && line.SecQty.Value > 0 ? Math.Round(line.Qty / line.SecQty.Value, 2) : packingVal);
                            if (packVal == 0) packVal = packingVal;
                            row.Cells["clnPackQty"].Value = packVal.ToString("0.##");
                        }
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
            clnQty.HeaderText = "Weight (Kg)";
            clnRate.HeaderText = "Rate (Rs/Kg)";

            if (!dgvSale.Columns.Contains("clnSecQty"))
            {
                var colSecQty = new DataGridViewTextBoxColumn
                {
                    Name = "clnSecQty",
                    HeaderText = "Bags",
                    Width = 75,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                };
                var colPackQty = new DataGridViewTextBoxColumn
                {
                    Name = "clnPackQty",
                    HeaderText = "Pack Qty",
                    Width = 75,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                };
                var colPacking = new DataGridViewTextBoxColumn
                {
                    Name = "clnPacking",
                    HeaderText = "Packing",
                    Width = 75,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                };
                var colSecRate = new DataGridViewTextBoxColumn
                {
                    Name = "clnSecRate",
                    HeaderText = "Bag Rate",
                    Width = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                };

                dgvSale.Columns.Add(colSecQty);
                dgvSale.Columns.Add(colPackQty);
                dgvSale.Columns.Add(colPacking);
                dgvSale.Columns.Add(colSecRate);
            }

            int dIdx = 0;
            clnSeq.DisplayIndex = dIdx++;
            clnCustomer.DisplayIndex = dIdx++;
            clnUnit.DisplayIndex = dIdx++;
            clnQty.DisplayIndex = dIdx++;
            if (dgvSale.Columns.Contains("clnSecQty")) dgvSale.Columns["clnSecQty"].DisplayIndex = dIdx++;
            if (dgvSale.Columns.Contains("clnPackQty")) dgvSale.Columns["clnPackQty"].DisplayIndex = dIdx++;
            if (dgvSale.Columns.Contains("clnPacking")) dgvSale.Columns["clnPacking"].DisplayIndex = dIdx++;
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

                if (cmbItem.SelectedValue != null)
                {
                    DataRowView dr = (DataRowView)cmbItem.SelectedItem;
                    if (dr != null && dr.Row.Table.Columns.Contains("PrimaryUnit") && dr["PrimaryUnit"] != DBNull.Value)
                    {
                        dgvSale.Rows[e.RowIndex].Cells[clnUnit.Index].Value = dr["PrimaryUnit"].ToString();
                    }
                    SetRateForRow(e.RowIndex);
                }
                else
                {
                    decimal defaultPack = GetSelectedItemDefaultPacking();
                    if (dgvSale.Columns.Contains("clnPacking"))
                        dgvSale.Rows[e.RowIndex].Cells["clnPacking"].Value = defaultPack.ToString("0.##");
                    if (dgvSale.Columns.Contains("clnPackQty"))
                        dgvSale.Rows[e.RowIndex].Cells["clnPackQty"].Value = defaultPack.ToString("0.##");
                    if (dgvSale.Columns.Contains("clnSecQty"))
                        dgvSale.Rows[e.RowIndex].Cells["clnSecQty"].Value = "0";
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
        }

        private void dgvPurchase_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvSale.CurrentCellAddress.X == clnCustomer.DisplayIndex)
            {
                ComboBox cmbCustomer = e.Control as ComboBox;
                if (cmbCustomer != null)
                {
                    cmbCustomer.DropDownStyle = ComboBoxStyle.DropDown;
                    cmbCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;
                    cmbCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
                }
            }
            else if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQty.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscount.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscPercent.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnCarriage.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnAddLess.Index ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecQty" ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnPackQty" ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnPacking" ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecRate")
            {
                TextBox tbRate = e.Control as TextBox;
                if (tbRate != null && e.Control.Text != null)
                {
                    tbRate.KeyPress += new KeyPressEventHandler(tbRate_KeyPress);
                }
            }
        }

        private async void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvSale.CurrentCellAddress.X == clnCustomer.DisplayIndex && dgvSale.CurrentCellAddress.Y >= 0)
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
                        try
                        {
                            var customItems = await _customerApiService.GetSupplyItemsAsync(custId, itemId);
                            var customSetting = customItems?.FirstOrDefault();
                            if (customSetting != null)
                            {
                                dgvSale[clnQty.Index, rowIndex].Value = (customSetting.Qty > 0 ? customSetting.Qty : 1).ToString("0.##");
                                if (dgvSale.Columns.Contains("clnSecQty"))
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

        private decimal GetSelectedItemDefaultPacking()
        {
            if (cmbItem.SelectedValue != null && dtItems != null)
            {
                string filter = string.Format("Id = '{0}'", cmbItem.SelectedValue.ToString());
                DataRow dr = dtItems.Select(filter).FirstOrDefault();
                if (dr != null && dr.Table.Columns.Contains("QtyInPack") && dr["QtyInPack"] != DBNull.Value)
                {
                    decimal p = Convert.ToDecimal(dr["QtyInPack"]);
                    if (p > 0) return p;
                }
            }
            return 50;
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
                    decimal priRate = dr["PriRate"] != DBNull.Value ? Convert.ToDecimal(dr["PriRate"]) : 0;
                    decimal secRate = dr["SecRate"] != DBNull.Value ? Convert.ToDecimal(dr["SecRate"]) : 0;

                    string custId = dgvSale.Rows[rowIndex].Cells[clnCustomer.Index].Value?.ToString();
                    string catId = null;
                    if (!string.IsNullOrWhiteSpace(custId))
                    {
                        var custRow = dtCustomers.AsEnumerable().FirstOrDefault(r => r["Account"].ToString() == custId);
                        catId = custRow != null && custRow.Table.Columns.Contains("CustomerCategoryId") && custRow["CustomerCategoryId"] != DBNull.Value
                            ? custRow["CustomerCategoryId"].ToString()
                            : null;
                    }

                    if (!string.IsNullOrWhiteSpace(catId) && _categoryPriceMap.TryGetValue($"{itemId}_{catId}", out var catPrice))
                    {
                        priRate = catPrice.PriRate;
                        if (catPrice.SecRate.HasValue)
                            secRate = catPrice.SecRate.Value;
                    }

                    decimal defaultPacking = 0;
                    if (dr.Table.Columns.Contains("QtyInPack") && dr["QtyInPack"] != DBNull.Value)
                        defaultPacking = Convert.ToDecimal(dr["QtyInPack"]);
                    if (defaultPacking == 0) defaultPacking = 50;

                    if (secRate == 0 && defaultPacking > 0)
                        secRate = Math.Round(priRate * defaultPacking, 2);

                    decimal kgRate = (defaultPacking > 0 && secRate > 0) ? Math.Round(secRate / defaultPacking, 8) : priRate;

                    dgvSale.Rows[rowIndex].Cells[clnRate.Index].Value = kgRate.ToString("0.##");
                    dgvSale.Rows[rowIndex].Cells[clnRate.Index].Tag = kgRate;

                    if (dgvSale.Columns.Contains("clnSecQty"))
                    {
                        if (dgvSale.Rows[rowIndex].Cells["clnSecQty"].Value == null || string.IsNullOrWhiteSpace(dgvSale.Rows[rowIndex].Cells["clnSecQty"].Value.ToString()))
                        {
                            dgvSale.Rows[rowIndex].Cells["clnSecQty"].Value = "0";
                        }
                        dgvSale.Rows[rowIndex].Cells["clnSecRate"].Value = secRate.ToString("0.####");
                        if (dgvSale.Columns.Contains("clnPacking"))
                        {
                            dgvSale.Rows[rowIndex].Cells["clnPacking"].Value = defaultPacking.ToString("0.##");
                        }
                        if (dgvSale.Columns.Contains("clnPackQty"))
                        {
                            decimal kQty = ParseDecimal(dgvSale.Rows[rowIndex].Cells[clnQty.Index].Value);
                            decimal bQty = ParseDecimal(dgvSale.Rows[rowIndex].Cells["clnSecQty"].Value);
                            decimal pQty = (bQty > 0 && kQty > 0) ? Math.Round(kQty / bQty, 2) : defaultPacking;
                            dgvSale.Rows[rowIndex].Cells["clnPackQty"].Value = pQty.ToString("0.##");
                        }
                    }
                }
            }
        }

        private void RecalculateRow(int rowIndex, int editedColumnIndex = -1)
        {
            if (rowIndex < 0 || rowIndex >= dgvSale.Rows.Count)
                return;

            DataGridViewRow row = dgvSale.Rows[rowIndex];
            if (row.IsNewRow)
                return;

            string colName = editedColumnIndex >= 0 && editedColumnIndex < dgvSale.Columns.Count 
                ? dgvSale.Columns[editedColumnIndex].Name 
                : string.Empty;

            decimal kgQty = ParseDecimal(row.Cells[clnQty.Index].Value);
            decimal bagQty = dgvSale.Columns.Contains("clnSecQty") ? ParseDecimal(row.Cells["clnSecQty"].Value) : 0;
            decimal packing = dgvSale.Columns.Contains("clnPacking") ? ParseDecimal(row.Cells["clnPacking"].Value) : 0;
            if (packing == 0)
            {
                packing = GetSelectedItemDefaultPacking();
                if (dgvSale.Columns.Contains("clnPacking"))
                    row.Cells["clnPacking"].Value = packing.ToString("0.##");
            }
            decimal packQty = dgvSale.Columns.Contains("clnPackQty") ? ParseDecimal(row.Cells["clnPackQty"].Value) : 0;
            if (packQty == 0 && packing > 0)
            {
                packQty = packing;
                if (dgvSale.Columns.Contains("clnPackQty"))
                    row.Cells["clnPackQty"].Value = packQty.ToString("0.##");
            }
            decimal bagRate = dgvSale.Columns.Contains("clnSecRate") ? ParseDecimal(row.Cells["clnSecRate"].Value) : 0;
            decimal kgRate = row.Cells[clnRate.Index].Tag is decimal tagKgRate && tagKgRate > 0
                ? tagKgRate
                : ((packing > 0 && bagRate > 0) ? Math.Round(bagRate / packing, 8) : ParseDecimal(row.Cells[clnRate.Index].Value));
            decimal discount = ParseDecimal(row.Cells[clnDiscount.Index].Value);
            decimal discountPercent = ParseDecimal(row.Cells[clnDiscPercent.Index].Value);
            decimal carriage = ParseDecimal(row.Cells[clnCarriage.Index].Value);
            decimal addLess = ParseDecimal(row.Cells[clnAddLess.Index].Value);

            // 1. Quantity relationships
            if (editedColumnIndex == clnQty.Index)
            {
                if (bagQty > 0)
                    packQty = Math.Round(kgQty / bagQty, 2);
                else if (packQty > 0)
                    bagQty = Math.Round(kgQty / packQty, 2);
            }
            else if (colName == "clnSecQty")
            {
                if (packQty > 0)
                    kgQty = Math.Round(bagQty * packQty, 2);
                else if (kgQty > 0)
                    packQty = Math.Round(kgQty / bagQty, 2);
            }
            else if (colName == "clnPackQty")
            {
                if (bagQty > 0)
                    kgQty = Math.Round(bagQty * packQty, 2);
                else if (kgQty > 0)
                    bagQty = Math.Round(kgQty / packQty, 2);
            }

            // 2. Rate relationships
            if (colName == "clnPacking")
            {
                if (packing > 0)
                {
                    if (bagRate > 0)
                        kgRate = Math.Round(bagRate / packing, 8);
                    else if (kgRate > 0)
                        bagRate = Math.Round(kgRate * packing, 2);

                    packQty = packing;
                    if (bagQty > 0)
                        kgQty = Math.Round(bagQty * packQty, 2);
                    else if (kgQty > 0)
                        bagQty = Math.Round(kgQty / packQty, 2);
                }
            }
            else if (editedColumnIndex == clnRate.Index)
            {
                decimal enteredRate = ParseDecimal(row.Cells[clnRate.Index].Value);
                if (row.Cells[clnRate.Index].Tag is decimal tagRate && tagRate > 0 && Math.Round(enteredRate, 2) == Math.Round(tagRate, 2))
                {
                    kgRate = tagRate;
                }
                else
                {
                    kgRate = enteredRate;
                    if (packing > 0)
                        bagRate = Math.Round(kgRate * packing, 2);
                }
            }
            else if (colName == "clnSecRate")
            {
                if (packing > 0)
                    kgRate = Math.Round(bagRate / packing, 8);
            }
            else if (packing > 0 && bagRate > 0)
            {
                kgRate = (row.Cells[clnRate.Index].Tag is decimal tagRate && tagRate > 0 && Math.Round(tagRate * packing, 2) == bagRate)
                    ? tagRate
                    : Math.Round(bagRate / packing, 8);
            }

            decimal lineGross = kgQty * kgRate;
            if (editedColumnIndex == clnDiscPercent.Index || (!clnDiscount.Visible && clnDiscPercent.Visible))
            {
                discount = decimal.Round(lineGross * (discountPercent / 100), 2);
            }
            else
            {
                discountPercent = lineGross == 0 ? 0 : decimal.Round((discount / lineGross) * 100, 2);
            }

            row.Cells[clnQty.Index].Value = kgQty.ToString("0.##");
            if (dgvSale.Columns.Contains("clnSecQty")) row.Cells["clnSecQty"].Value = bagQty.ToString("0.##");
            if (dgvSale.Columns.Contains("clnPackQty")) row.Cells["clnPackQty"].Value = packQty.ToString("0.##");
            if (dgvSale.Columns.Contains("clnPacking")) row.Cells["clnPacking"].Value = packing.ToString("0.##");
            row.Cells[clnRate.Index].Value = kgRate.ToString("0.##");
            row.Cells[clnRate.Index].Tag = kgRate;
            if (dgvSale.Columns.Contains("clnSecRate")) row.Cells["clnSecRate"].Value = bagRate.ToString("0.##");
            row.Cells[clnDiscount.Index].Value = discount.ToString("0.##");
            row.Cells[clnDiscPercent.Index].Value = discountPercent.ToString("0.##");
            row.Cells[clnCarriage.Index].Value = carriage.ToString("0.##");
            row.Cells[clnAddLess.Index].Value = addLess.ToString("0.##");

            decimal lineAmount = (lineGross - discount) + carriage + addLess;
            row.Cells[clnAmount.Index].Value = decimal.Round(lineAmount, 2).ToString("N2");
        }


        void tbRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQty.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscount.Index ||
                 dgvSale.CurrentCell.ColumnIndex == clnDiscPercent.Index ||
                 dgvSale.CurrentCell.ColumnIndex == clnCarriage.Index ||
                 dgvSale.CurrentCell.ColumnIndex == clnAddLess.Index ||
                 dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecQty" ||
                 dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnPackQty" ||
                 dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnPacking" ||
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

                if (dgvSale.CurrentCell != null && dgvSale.CurrentCell.ColumnIndex == clnCustomer.Index)
                {
                    int rowIndex = dgvSale.CurrentCell.RowIndex;
                    dgvSale.CurrentCell = dgvSale.Rows[rowIndex].Cells[clnQty.Index];
                }
                else if (dgvSale.CurrentCell != null && dgvSale.CurrentCell.ColumnIndex == clnQty.Index)
                {
                    int rowIndex = dgvSale.CurrentCell.RowIndex;
                    if (dgvSale.Columns.Contains("clnSecQty"))
                    {
                        dgvSale.CurrentCell = dgvSale.Rows[rowIndex].Cells[dgvSale.Columns["clnSecQty"].Index];
                    }
                    else
                    {
                        bool rateFilled = dgvSale.Rows[rowIndex].Cells[clnRate.Index].Value != null &&
                                         !string.IsNullOrEmpty(dgvSale.Rows[rowIndex].Cells[clnRate.Index].Value.ToString());
                        if (rateFilled && dgvSale.Rows.Count > (rowIndex + 1))
                        {
                            dgvSale.CurrentCell = dgvSale.Rows[rowIndex + 1].Cells[clnCustomer.Index];
                        }
                        else
                        {
                            SendKeys.Send("{tab}");
                        }
                    }
                }
                else if (dgvSale.CurrentCell != null && dgvSale.Columns.Contains("clnSecQty") && dgvSale.CurrentCell.ColumnIndex == dgvSale.Columns["clnSecQty"].Index)
                {
                    int rowIndex = dgvSale.CurrentCell.RowIndex;
                    bool rateFilled = dgvSale.Rows[rowIndex].Cells[clnRate.Index].Value != null &&
                                     !string.IsNullOrEmpty(dgvSale.Rows[rowIndex].Cells[clnRate.Index].Value.ToString());
                    if (rateFilled && dgvSale.Rows.Count > (rowIndex + 1))
                    {
                        dgvSale.CurrentCell = dgvSale.Rows[rowIndex + 1].Cells[clnCustomer.Index];
                    }
                    else
                    {
                        SendKeys.Send("{tab}");
                    }
                }
                else
                {
                    SendKeys.Send("{tab}");
                }
            }
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
            dgvSale.Rows.Add();
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

            foreach (DataGridViewRow row in dgvSale.Rows)
                    {
                        if (row.IsNewRow) continue;

                        if (!string.IsNullOrEmpty(primaryUnit))
                            row.Cells[clnUnit.Index].Value = primaryUnit;

                        if (isSameAsOriginal && row.Tag is SavedRowRateInfo savedRate)
                        {
                            row.Cells[clnRate.Index].Value = savedRate.Rate.ToString("0.##");
                            row.Cells[clnRate.Index].Tag = savedRate.Rate;
                            if (dgvSale.Columns.Contains("clnSecQty"))
                            {
                                if (row.Cells["clnSecQty"].Value == null || string.IsNullOrWhiteSpace(row.Cells["clnSecQty"].Value.ToString()))
                                {
                                    row.Cells["clnSecQty"].Value = "0";
                                }
                                row.Cells["clnSecRate"].Value = savedRate.SecRate.ToString("0.####");
                            }
                        }
                        else
                        {
                            SetRateForRow(row.Index);
                        }

                        // Only apply the prefill default qty when the row does not already
                        // have a qty value. This prevents overwriting manually-entered or
                        // previously-saved quantities when an existing voucher is loaded
                        // and cmbItem.SelectedValue is set, which re-triggers this event.
                        // Note: a user-entered zero is still a valid qty, so we only check
                        // for null/whitespace — NOT for non-zero.
                        bool hasExistingQty = row.Cells[clnQty.Index].Value != null &&
                                              !string.IsNullOrWhiteSpace(row.Cells[clnQty.Index].Value.ToString());

                        if (!hasExistingQty)
                        {
                            string custId = row.Cells[clnCustomer.Index].Value?.ToString();
                            if (!string.IsNullOrEmpty(custId) && itemQtyMap.TryGetValue(custId, out var customSetting))
                            {
                                row.Cells[clnQty.Index].Value = (customSetting.Qty > 0 ? customSetting.Qty : 1).ToString("0.##");
                                if (dgvSale.Columns.Contains("clnSecQty"))
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

                            dgvSale.Rows[idx].Cells[clnQty.Index].Value = qty.ToString("0.##");
                            if (dgvSale.Columns.Contains("clnSecQty"))
                            {
                                dgvSale.Rows[idx].Cells["clnSecQty"].Value = secQty.ToString("0.##");
                            }

                            if (cmbItem.SelectedValue != null)
                            {
                                DataRowView itemDr = cmbItem.SelectedItem as DataRowView;
                                if (itemDr != null && itemDr.Row.Table.Columns.Contains("PrimaryUnit") && itemDr["PrimaryUnit"] != DBNull.Value)
                                {
                                    dgvSale.Rows[idx].Cells[clnUnit.Index].Value = itemDr["PrimaryUnit"].ToString();
                                }
                                SetRateForRow(idx);
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
    }
}
