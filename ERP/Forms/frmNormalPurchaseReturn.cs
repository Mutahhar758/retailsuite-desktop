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
    public partial class frmNormalPurchaseReturn : Form
    {
        private readonly PurchaseReturnApiService _apiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly NarrationApiService _narrationApiService;
        private readonly ItemCategoryApiService _itemCategoryApiService;
        private readonly UnitApiService _unitApiService;
        private readonly InventoryApiService _inventoryApiService;
        private List<PurchaseReturnDto> _queryList = new List<PurchaseReturnDto>();

        private readonly DataTable dtItems = new DataTable();
        private readonly DataTable dtUnits = new DataTable();
        private readonly DataTable dtCategories = new DataTable();
        private readonly DataTable dtAccounts = new DataTable();
        private readonly DataTable dtNarration = new DataTable();

        private int currentRow;
        private bool resetRow = false;
        private bool FLogIn = true;
        private int VoucherIndex = 0;
        private bool _isSaving;

        enum Navigators { Up, Down, Home, End }

        public frmNormalPurchaseReturn()
        {
            InitializeComponent();
            _apiService = new PurchaseReturnApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _narrationApiService = new NarrationApiService();
            _itemCategoryApiService = new ItemCategoryApiService();
            _unitApiService = new UnitApiService();
            _inventoryApiService = new InventoryApiService();
            InitializeLookupTables();
            dgvSale.Rows.Add();
            UserInfo.ApplyFormPermissions(this, AppResource.PurchaseReturns);
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

            dtNarration.Columns.Add("Code", typeof(string));
            dtNarration.Columns.Add("Title", typeof(string));
        }

        private async System.Threading.Tasks.Task LoadLookupsAsync()
        {
            var accountsTask = _chartOfAccountApiService.GetSupplierAccountsAsync();
            var narrationsTask = _narrationApiService.GetLookupAsync();
            var categoriesTask = _itemCategoryApiService.GetLookupAsync();
            var unitsTask = _unitApiService.GetLookupAsync();
            var itemsTask = _inventoryApiService.GetLookupAsync(null);
            await System.Threading.Tasks.Task.WhenAll(accountsTask, narrationsTask, categoriesTask, unitsTask, itemsTask);

            dtItems.Rows.Clear();
            foreach (var item in itemsTask.Result)
                dtItems.Rows.Add(item.Id, item.Title, item.ItemCategoryCode, item.ItemKey, item.Barcode, item.PriRate, item.SecRate, item.PrimaryUnit, item.SecondaryUnit, item.DefaultUnit, item.RequireImei == true, item.AdditionalUnits);

            dtUnits.Rows.Clear();
            foreach (var u in unitsTask.Result)
                dtUnits.Rows.Add(u.Code, u.Title, u.ShortTitle, u.ParentUnitId, u.ConversionRatio.HasValue ? (object)u.ConversionRatio.Value : DBNull.Value);

            dtCategories.Rows.Clear();
            foreach (var c in categoriesTask.Result)
                dtCategories.Rows.Add(c.Code, c.Title);

            dtAccounts.Rows.Clear();
            foreach (var a in accountsTask.Result)
                dtAccounts.Rows.Add(a.Account, a.Title);

            dtNarration.Rows.Clear();
            foreach (var n in narrationsTask.Result)
                dtNarration.Rows.Add(n.Code, n.Title);

            FillAccounts();
            FillNarration();
            FillCategories();
            FillItems();
            FillFilterAccounts();
        }

        private void FillAccounts()
        {
            cmbAccounts.DataSource = dtAccounts.Copy();
            cmbAccounts.DisplayMember = "Title";
            cmbAccounts.ValueMember = "Account";
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

        private ERP.Controls.PaginationControl _pager;
        private string _qFrom = "", _qTo = "", _qAccount = "", _qVoucherNo = "";
        private int _qPageSize = 20;

        private void EnsurePager()
        {
            if (_pager != null) return;
            _pager = new ERP.Controls.PaginationControl();
            _pager.PageChanged += async (s, e) =>
            {
                try
                {
                    await FillQueryAsync(_qFrom, _qTo, _qAccount, _qVoucherNo, e.PageNumber, e.PageSize);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading purchase returns: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            _pager.Attach(tbQuery, dgvQuery);
        }

        private async System.Threading.Tasks.Task FillQueryAsync(
            string fromDate = "", string toDate = "",
            string account = "", string voucherNo = "",
            int pageNumber = 1, int pageSize = 0)
        {
            EnsurePager();
            _qFrom = fromDate; _qTo = toDate; _qAccount = account; _qVoucherNo = voucherNo;
            if (pageSize > 0) _qPageSize = pageSize;

            var page = await _apiService.GetPagedListAsync(fromDate, toDate, account, voucherNo, pageNumber, _qPageSize);
            _queryList = page.Data;
            _pager.SetPagination(page.CurrentPage, page.TotalPages, page.TotalCount, page.PageSize);
            _qPageSize = page.PageSize;

            dgvQuery.Rows.Clear();
            for (int i = 0; i < _queryList.Count; i++)
            {
                var r = _queryList[i];
                dgvQuery.Rows.Add(
                    r.Date.ToString("dd-MMM-yyyy"),
                    "PR-" + r.VoucherNo,
                    r.Account,
                    r.CreatedBy + " | " + r.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                    !string.IsNullOrWhiteSpace(r.LastModifiedBy)
                        ? r.LastModifiedBy + " | " + r.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                        : null);
            }
        }

        internal async System.Threading.Tasks.Task FillPurchaseAsync(string vno)
        {
            var lines = await _apiService.GetDetailAsync(vno);
            txtVoucherNo.Text = "PR-" + vno;
            dgvSale.Rows.Clear();

            if (lines.Count > 0)
            {
                var first = lines[0];
                dtpDate.Value = first.Date;
                cmbAccounts.SelectedValue = first.AccountId;
                if (string.IsNullOrWhiteSpace(first.Narration))
                    cmbNarration.SelectedIndex = -1;
                else
                    cmbNarration.SelectedValue = first.Narration;
                txtDescription.Text = first.Description;
                txtCreatedBy.Text = first.CreatedBy + " | " + first.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt");
                txtEditBy.Text = !string.IsNullOrWhiteSpace(first.LastModifiedBy)
                    ? first.LastModifiedBy + " | " + first.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null;

                for (int i = 0; i < lines.Count; i++)
                {
                    var l = lines[i];
                    int ind = dgvSale.Rows.Add();
                    var row = dgvSale.Rows[ind];
                    row.Cells[clnSeq.Index].Value = l.Seq.ToString();
                    row.Cells[clnItemKey.Index].Value = l.ItemKey;
                    row.Cells[clnItemNo.Index].Value = null;
                    row.Cells[clnUnit.Index].Value = null;
                    row.Cells[clnQty.Index].Value = l.Qty.ToString("0.##");
                    row.Cells[clnRate.Index].Value = l.Rate.ToString("0.##");
                    row.Cells[clnAmount.Index].Value = l.Amount.ToString("N2");
                    row.Cells[clnStatus.Index].Value = "0";


                    SetcmbItemSource(l.ItemCategoryCode, ind);
                    dgvSale[clnItemNo.Index, ind].Value = l.ItemId;

                    DataRow itemRow = dtItems.Select("Id = '" + l.ItemId.Replace("'", "''") + "'").FirstOrDefault();
                    decimal displayQty = l.Qty;
                    string displayUnit = l.Unit;
                    if (itemRow != null)
                    {
                        DataRowView drv = dtItems.DefaultView[dtItems.Rows.IndexOf(itemRow)];
                        var resolved = UnitConversionHelper.ResolveDisplayQtyAndUnit(l.Qty, itemRow, dtUnits);
                        displayQty = resolved.DisplayQty;
                        displayUnit = resolved.UnitCode;
                        UnitConversionHelper.BindRowUnitComboBox(dgvSale, clnUnit.Index, ind, itemRow, dtUnits, displayUnit);
                        SetcmbUnitSource(drv, ind);
                    }
                    else
                    {
                        SetAllUnitsSource(ind);
                    }

                    dgvSale[clnQty.Index, ind].Value = displayQty.ToString("0.###");
                    dgvSale[clnUnit.Index, ind].Value = displayUnit;
                    dgvSale[clnRate.Index, ind].Value = l.Rate.ToString("0.##");

                    if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                    {
                        dgvSale["clnSecQty", ind].Value = (l.SecQty ?? 0).ToString("0.##");
                        dgvSale["clnSecRate", ind].Value = (l.SecRate ?? 0).ToString("0.##");
                    }

                    if (ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnImei"))
                    {
                        dgvSale["clnImei", ind].Value = l.Imei;
                        dgvSale["clnImei2", ind].Value = l.Imei2;
                    }
                }
            }

            dgvSale.Rows.Add();
            UnitConversionHelper.UpdateUnitColumnVisibility(dgvSale, clnUnit.Index, clnItemNo.Index, dtItems, dtUnits);
            CalcTotAmount();
        }

        internal void FillPurchase(string vno)
        {
            _ = FillPurchaseAsync(vno);
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

            lblTotAmount.Text = totAmount.ToString();
        }

        private static decimal ParseDecimal(object value)
        {
            decimal parsed;
            return decimal.TryParse(Convert.ToString(value), out parsed) ? parsed : 0;
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

                    int insertIndex = clnItemNo.Index + 1;
                    dgvSale.Columns.Insert(insertIndex, colImei);
                    dgvSale.Columns.Insert(insertIndex + 1, colImei2);
                }
                else
                {
                    dgvSale.Columns["clnImei"].Visible = true;
                    dgvSale.Columns["clnImei2"].Visible = true;
                }
            }
            else
            {
                if (dgvSale.Columns.Contains("clnImei")) dgvSale.Columns["clnImei"].Visible = false;
                if (dgvSale.Columns.Contains("clnImei2")) dgvSale.Columns["clnImei2"].Visible = false;
            }
        }

        private async void frmPurchase_Load(object sender, EventArgs e)
        {
            try
            {
                SetupSecondaryQtyColumns();
                SetupMobileShopColumns();
                ApplyBarcodeFastScanVisibility();
                await LoadLookupsAsync();
                await FillQueryAsync();
                if (_queryList.Count > 0 && txtVoucherNo.Text == "")
                    await FillPurchaseAsync(_queryList[VoucherIndex].VoucherNo);
                FLogIn = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase return data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void frmPurchase_KeyDown(object sender, KeyEventArgs e)
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
            else if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index || dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecRate")
            {
                TextBox tbRate = e.Control as TextBox;
                if (tbRate != null && e.Control.Text != null)
                    tbRate.KeyPress += new KeyPressEventHandler(tbRate_KeyPress);
            }
            else if (dgvSale.CurrentCell.ColumnIndex == clnQty.Index || dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecQty")
            {
                TextBox tbQty = e.Control as TextBox;
                if (tbQty != null && e.Control.Text != null)
                    tbQty.KeyPress += new KeyPressEventHandler(tbQty_KeyPress);
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
            if (dgvSale.CurrentCell.ColumnIndex == clnQty.Index || dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecQty")
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

        private void tbRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index || dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecRate")
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
                }
            }
        }

        private void SetcmbUnitSource(DataRowView dr, int rowInd)
        {
            dgvSale[clnRate.Index, rowInd].Value = dr["PriRate"].ToString();
            if (dr != null && dr.Row != null)
            {
                string currentUnit = Convert.ToString(dgvSale[clnUnit.Index, rowInd].Value);
                UnitConversionHelper.BindRowUnitComboBox(dgvSale, clnUnit.Index, rowInd, dr.Row, dtUnits, currentUnit);
                UnitConversionHelper.UpdateUnitColumnVisibility(dgvSale, clnUnit.Index, clnItemNo.Index, dtItems, dtUnits);
            }

            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                if (dgvSale["clnSecQty", rowInd].Value == null || string.IsNullOrWhiteSpace(dgvSale["clnSecQty", rowInd].Value.ToString()))
                    dgvSale["clnSecQty", rowInd].Value = "0";
                dgvSale["clnSecRate", rowInd].Value = dr["SecRate"].ToString();
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
                    dgvSale[clnUnit.Index, e.RowIndex].Value = dr["DefaultUnit"].ToString();
                    SetcmbUnitSource(drv, e.RowIndex);
                }
            }

            AllowNewRow();
            resetRow = true;
            currentRow = e.RowIndex;

            decimal qty = ParseDecimal(dgvSale[clnQty.Index, e.RowIndex].Value);
            decimal rate = ParseDecimal(dgvSale[clnRate.Index, e.RowIndex].Value);

            string rowItemId = Convert.ToString(dgvSale[clnItemNo.Index, e.RowIndex].Value);
            DataRow rowItem = !string.IsNullOrWhiteSpace(rowItemId)
                ? dtItems.Select("Id = '" + rowItemId.Replace("'", "''") + "'").FirstOrDefault()
                : null;
            string selectedUnit = Convert.ToString(dgvSale[clnUnit.Index, e.RowIndex].Value);
            decimal baseQty = UnitConversionHelper.ConvertToBasePrimaryQty(qty, selectedUnit, rowItem, dtUnits);

            decimal secQty = 0;
            decimal secRate = 0;
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                secQty = ParseDecimal(dgvSale["clnSecQty", e.RowIndex].Value);
                secRate = ParseDecimal(dgvSale["clnSecRate", e.RowIndex].Value);
            }

            dgvSale[clnQty.Index, e.RowIndex].Value = qty.ToString();
            dgvSale[clnRate.Index, e.RowIndex].Value = rate.ToString();
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                dgvSale["clnSecQty", e.RowIndex].Value = secQty.ToString();
                dgvSale["clnSecRate", e.RowIndex].Value = secRate.ToString();
            }

            dgvSale[clnAmount.Index, e.RowIndex].Value = decimal.Round((baseQty * rate) + (secQty * secRate), 2).ToString();
            CalcTotAmount();
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

            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to save this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _isSaving = true;
            btnSave.Enabled = false;
            try
            {
                if (cmbAccounts.SelectedValue == null)
                {
                    MessageBox.Show("Please Select Account...!");
                    return;
                }

                var lines = new List<PurchaseReturnLineRequest>();
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
                    string imei2 = ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnImei2") ? Convert.ToString(row.Cells["clnImei2"].Value) : null;

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

                    decimal inputQty = ParseDecimal(row.Cells[clnQty.Index].Value);
                    string selectedUnit = Convert.ToString(row.Cells[clnUnit.Index].Value);
                    decimal baseQty = UnitConversionHelper.ConvertToBasePrimaryQty(inputQty, selectedUnit, itemRow, dtUnits);

                    lines.Add(new PurchaseReturnLineRequest
                    {
                        Seq = int.Parse(Convert.ToString(row.Cells[clnSeq.Index].Value)),
                        ItemId = Convert.ToString(row.Cells[clnItemNo.Index].Value),
                        Qty = baseQty,
                        Rate = ParseDecimal(row.Cells[clnRate.Index].Value),
                        SecQty = secQty,
                        SecRate = secRate,
                        SecUnit = secUnit,
                        Imei = imei,
                        Imei2 = imei2
                    });
                }

                try
                {
                    string voucher = txtVoucherNo.Text != "" ? txtVoucherNo.Text.Substring(3) : null;
                    bool isNew = string.IsNullOrWhiteSpace(voucher);

                    if (isNew)
                    {
                        var createRequest = new PurchaseReturnCreateRequest
                        {
                            Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                            Account = cmbAccounts.SelectedValue.ToString(),
                            Description = txtDescription.Text,
                            Narration = cmbNarration.SelectedValue?.ToString(),
                            Lines = lines
                        };
                        voucher = await _apiService.CreateAsync(createRequest);
                    }
                    else
                    {
                        var updateRequest = new PurchaseReturnUpdateRequest
                        {
                            Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                            Account = cmbAccounts.SelectedValue.ToString(),
                            Description = txtDescription.Text,
                            Narration = cmbNarration.SelectedValue?.ToString(),
                            Lines = lines
                        };
                        await _apiService.UpdateAsync(voucher, updateRequest);
                    }

                    await FillPurchaseAsync(voucher);
                    await FillQueryAsync();
                    VoucherIndex = _queryList.FindIndex(x => x.VoucherNo == voucher);
                    if (VoucherIndex < 0) VoucherIndex = 0;

                    MessageBox.Show("Record Successfully Saved...!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving purchase return: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                _isSaving = false;
                btnSave.Enabled = true;
            }
        }

        private bool _isQueryLoading = false;
        private async void btnFind_Click(object sender, EventArgs e)
        {
            if (_isQueryLoading) return;
            _isQueryLoading = true;
            btnFind.Enabled = false;
            try
            {
                string voucher = txtFilterVoucher.Text;
                if (!string.IsNullOrWhiteSpace(voucher) && voucher.StartsWith("PR-", StringComparison.OrdinalIgnoreCase))
                    voucher = voucher.Substring(3);

                await FillQueryAsync(
                    txtFdate.Text,
                    txtTdate.Text,
                    cmbFilterAccounts.SelectedValue != null ? cmbFilterAccounts.SelectedValue.ToString() : "",
                    voucher,
                    1,
                    _qPageSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase return data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isQueryLoading = false;
                btnFind.Enabled = true;
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
            if (!Regex.IsMatch(txtFilterVoucher.Text, @"PR-\d{5}"))
                txtFilterVoucher.Text = "";
        }

        private async void dgvQuery_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!FLogIn && e.RowIndex != -1)
            {
                VoucherIndex = e.RowIndex;
                string voucherNo = dgvQuery.Rows[e.RowIndex].Cells[clnVoucherNum.Index].Value.ToString();
                tbSaleQuery.SelectedTab = tbDetail;
                await FillPurchaseAsync(voucherNo.Substring(3));
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            dgvSale.Rows.Clear();
            Validation.Clear(grpInvoiceDetail);
            txtDescription.Text = "";
            txtCreatedBy.Text = "";
            txtEditBy.Text = "";
            txtBarcode.Text = "";
            lblTotAmount.Text = "0";
            dgvSale.Rows.Add();
            dtpDate.Focus();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
        }

        private void Navigate(Navigators nav)
        {
            if (dgvQuery.CurrentRow != null && tbSaleQuery.SelectedTab == tbDetail)
            {
                if (nav == Navigators.Up && VoucherIndex > 0)
                {
                    VoucherIndex--;
                    _ = FillPurchaseAsync(_queryList[VoucherIndex].VoucherNo);
                }
                else if (nav == Navigators.Down && _queryList.Count - 1 > VoucherIndex)
                {
                    VoucherIndex++;
                    _ = FillPurchaseAsync(_queryList[VoucherIndex].VoucherNo);
                }
                else if (nav == Navigators.Home && _queryList.Count > 0)
                {
                    VoucherIndex = _queryList.Count - 1;
                    _ = FillPurchaseAsync(_queryList[VoucherIndex].VoucherNo);
                }
                else if (nav == Navigators.End && _queryList.Count > 0)
                {
                    VoucherIndex = 0;
                    _ = FillPurchaseAsync(_queryList[VoucherIndex].VoucherNo);
                }
            }
        }

        private void btnHome_Click(object sender, EventArgs e) => Navigate(Navigators.Home);
        private void btnPri_Click(object sender, EventArgs e) => Navigate(Navigators.Up);
        private void btnNext_Click(object sender, EventArgs e) => Navigate(Navigators.Down);
        private void btnEnd_Click(object sender, EventArgs e) => Navigate(Navigators.End);

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
                    MessageBox.Show("Error deleting purchase return: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                var parsed = BarcodeParser.Parse(raw, dtItems);
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

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(defaultUnit))
                            dgvSale[clnUnit.Index, ind].Value = defaultUnit;
                    }
                    catch { }

                    dgvSale[clnRate.Index, ind].Value = itemPriRate.ToString("0.##");
                    dgvSale[clnQty.Index, ind].Value = qtyToScan.ToString("0.##");
                    dgvSale[clnStatus.Index, ind].Value = "0";

                    if (ind == dgvSale.Rows.Count - 1)
                    {
                        dgvSale.Rows.Add();
                    }
                }

                decimal qty = ParseDecimal(dgvSale[clnQty.Index, ind].Value);
                decimal rate = ParseDecimal(dgvSale[clnRate.Index, ind].Value);

                string selectedUnit = Convert.ToString(dgvSale[clnUnit.Index, ind].Value);
                decimal baseQty = UnitConversionHelper.ConvertToBasePrimaryQty(qty, selectedUnit, dr, dtUnits);

                decimal secQty = 0m;
                decimal secRate = 0m;
                if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                {
                    secQty = ParseDecimal(dgvSale["clnSecQty", ind].Value);
                    secRate = ParseDecimal(dgvSale["clnSecRate", ind].Value);
                    dgvSale["clnSecQty", ind].Value = secQty.ToString("0.##");
                    dgvSale["clnSecRate", ind].Value = secRate.ToString("0.##");
                }

                dgvSale[clnQty.Index, ind].Value = qty.ToString("0.##");
                dgvSale[clnRate.Index, ind].Value = rate.ToString("0.##");
                dgvSale[clnAmount.Index, ind].Value = decimal.Round((baseQty * rate) + (secQty * secRate), 2).ToString("N2");
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

        private void btnPreview_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (txtVoucherNo.Text != "")
            {
                DataSet ds = ReportQuery.PurchaseRetBill(txtVoucherNo.Text.Substring(3));
                if (ds.Tables[1].Rows.Count > 0)
                {
                    frmReportView frm = new frmReportView();
                    Reports.rpt_PurchaseBill rpt = new Reports.rpt_PurchaseBill();

                    rpt.SetDataSource(ds.Tables[0]);
                    rpt.SetParameterValue("@HeaderLocation", Application.StartupPath + "\\rptHeader.jpg");
                    rpt.SetParameterValue("@Account", (string)ds.Tables[1].Rows[0]["Title"]);
                    rpt.SetParameterValue("@VoucherNo", txtVoucherNo.Text);
                    rpt.SetParameterValue("@VDate", (DateTime)ds.Tables[1].Rows[0]["vdate"]);
                    rpt.SetParameterValue("@Amount", ds.Tables[1].Rows[0]["Amount"]);
                    rpt.SetParameterValue("@Discount", ds.Tables[1].Rows[0]["Discount"]);
                    rpt.SetParameterValue("@NetAmount", ds.Tables[1].Rows[0]["NetAmount"]);
                    Cursor.Current = Cursors.Default;
                    frm.rptViewer.ReportSource = rpt;
                    frm.ShowDialog();
                }
            }
        }
    }
}

