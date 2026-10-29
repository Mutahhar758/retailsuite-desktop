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
    public partial class frmNormalPurchase : Form
    {
        private readonly PurchaseApiService _apiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly NarrationApiService _narrationApiService;
        private readonly ItemCategoryApiService _itemCategoryApiService;
        private readonly UnitApiService _unitApiService;
        private readonly InventoryApiService _inventoryApiService;
        private readonly MobileShopApiService _mobileShopApiService;
        private TextBox txtSellerCnic;
        private TextBox txtSellerContact;
        private Label lblSellerCnic;
        private Label lblSellerContact;
        private List<PurchaseDto> _queryList = new List<PurchaseDto>();

        private readonly DataTable dtItems = new DataTable();
        private DataTable dtUnits = new DataTable();
        private readonly DataTable dtCategories = new DataTable();
        private readonly DataTable dtAccounts = new DataTable();
        private readonly DataTable dtNarration = new DataTable();

        private int currentRow;
        private bool resetRow = false;
        private bool FLogIn = true;
        private int VoucherIndex = 0;

        enum Navigators { Up, Down, Home, End }

        public frmNormalPurchase()
        {
            InitializeComponent();
            _apiService = new PurchaseApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _narrationApiService = new NarrationApiService();
            _itemCategoryApiService = new ItemCategoryApiService();
            _unitApiService = new UnitApiService();
            _inventoryApiService = new InventoryApiService();
            _mobileShopApiService = new MobileShopApiService();
            InitializeLookupTables();
            dgvSale.Rows.Add();
            EnableDoubleBuffering(dgvSale);
            dgvSale.Enter += dgvSale_Enter;
            UserInfo.ApplyFormPermissions(this, AppResource.Purchases);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == Keys.Enter || keyData == Keys.Tab) && dgvSale.ContainsFocus)
            {
                if (HandleGridEnterNavigation())
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
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

        private void dgvSale_Enter(object sender, EventArgs e)
        {
            if (IsHandleCreated && dgvSale.Rows.Count > 0 && clnItemNo != null)
            {
                BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (dgvSale.Rows.Count > 0 && clnItemNo != null)
                        {
                            int row = dgvSale.CurrentCell != null ? dgvSale.CurrentCell.RowIndex : 0;
                            if (row >= 0 && row < dgvSale.Rows.Count)
                            {
                                if (dgvSale.CurrentCell == null || dgvSale.CurrentCell.ColumnIndex == clnItemKey.Index)
                                {
                                    dgvSale.CurrentCell = dgvSale[clnItemNo.Index, row];
                                }
                            }
                        }
                    }
                    catch { }
                }));
            }
        }

        private void cmbAccounts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                if (ApiSession.IsBarcodeFastScanEnabled && groupBox2.Visible && txtBarcode.Enabled)
                {
                    txtBarcode.Focus();
                    txtBarcode.SelectAll();
                }
                else
                {
                    dgvSale.Focus();
                    if (dgvSale.Rows.Count > 0 && clnItemNo != null)
                    {
                        int targetRow = dgvSale.CurrentCell != null ? dgvSale.CurrentCell.RowIndex : 0;
                        if (targetRow < 0 || targetRow >= dgvSale.Rows.Count) targetRow = 0;
                        dgvSale.CurrentCell = dgvSale[clnItemNo.Index, targetRow];
                    }
                }
            }
        }

        private void txtCashReceipt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                btnSave.Focus();
            }
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
            var itemsTask = _inventoryApiService.GetLookupAsync(null, true);
            await System.Threading.Tasks.Task.WhenAll(accountsTask, narrationsTask, categoriesTask, unitsTask, itemsTask);

            dtItems.Rows.Clear();
            foreach (var item in itemsTask.Result)
                dtItems.Rows.Add(item.Id, item.Title, item.ItemCategoryCode, item.ItemKey, item.Barcode, item.LastPurchaseRate ?? item.PriRate, item.LastPurchaseSecRate ?? item.SecRate, item.PrimaryUnit, item.SecondaryUnit, item.DefaultUnit, item.RequireImei == true, item.AdditionalUnits);

            dtUnits = UnitConversionHelper.CreateUnitsTable(unitsTask.Result);

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
            UnitConversionHelper.UpdateUnitColumnVisibility(dgvSale, clnUnit, clnQty, dtItems, dtUnits);
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
                    MessageBox.Show("Error loading purchases: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            dgvQuery.Rows.Clear();
            var page = await _apiService.GetPagedListAsync(fromDate, toDate, account, voucherNo, pageNumber, _qPageSize);
            _queryList = page.Data;
            _pager.SetPagination(page.CurrentPage, page.TotalPages, page.TotalCount, page.PageSize);
            _qPageSize = page.PageSize;

            for (int i = 0; i < _queryList.Count; i++)
            {
                var r = _queryList[i];
                dgvQuery.Rows.Add(
                    r.Date.ToString("dd-MMM-yyyy"),
                    "PU-" + r.VoucherNo,
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
            txtVoucherNo.Text = "PU-" + vno;
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
                txtCashReceipt.Text = (first.CashPaid ?? 0).ToString("N2");
                txtCashBack.Text = (first.CashBack ?? 0).ToString("N2");

                for (int i = 0; i < lines.Count; i++)
                {
                    var l = lines[i];
                    int ind = dgvSale.Rows.Add();
                    var row = dgvSale.Rows[ind];
                    row.Cells[clnSeq.Index].Value = l.Seq.ToString();
                    row.Cells[clnItemKey.Index].Value = l.ItemKey;
                    row.Cells[clnItemNo.Index].Value = null;
                    row.Cells[clnUnit.Index].Value = null;
                    row.Cells[clnRate.Index].Value = l.Rate.ToString("0.##");
                    row.Cells[clnAddless.Index].Value = l.AddLess.ToString("0.##");
                    row.Cells[clnAmount.Index].Value = l.Amount.ToString("N2");
                    row.Cells[clnStatus.Index].Value = "0";


                    SetcmbItemSource(l.ItemCategoryCode, ind);
                    dgvSale[clnItemNo.Index, ind].Value = l.ItemId;

                    DataRow itemRow = dtItems.Select("Id = '" + l.ItemId.Replace("'", "''") + "'").FirstOrDefault();
                    decimal displayQty = l.Qty;
                    string displayUnitId = l.Unit;
                    if (itemRow != null)
                    {
                        DataRowView drv = dtItems.DefaultView[dtItems.Rows.IndexOf(itemRow)];
                        SetcmbUnitSource(drv, ind);
                        UnitConversionHelper.ResolveDisplayQtyAndUnit(l.Qty, itemRow, dtUnits, out displayQty, out displayUnitId);
                        UnitConversionHelper.BindRowUnitComboBox(row, clnUnit.Index, itemRow, dtUnits, displayUnitId);
                    }
                    else
                    {
                        SetAllUnitsSource(ind);
                        dgvSale[clnUnit.Index, ind].Value = l.Unit;
                    }

                    row.Cells[clnQty.Index].Value = displayQty.ToString("0.####");
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
                        dgvSale["clnPtaStatus", ind].Value = l.PtaStatus;
                        dgvSale["clnBatteryHealth", ind].Value = l.BatteryHealth?.ToString();
                        dgvSale["clnConditionNote", ind].Value = l.ConditionNote;
                    }
                }

                if (ApiSession.HasMobileShopFeature && txtSellerCnic != null)
                {
                    var master = _queryList.FirstOrDefault(q => q.VoucherNo == vno);
                    txtSellerCnic.Text = master?.SellerCnic ?? "";
                    txtSellerContact.Text = master?.SellerContact ?? "";
                }
            }

            dgvSale.Rows.Add();
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
            txtTotAmount.Text = totAmount.ToString("N2");
        }

        private static decimal ParseDecimal(object value)
        {
            decimal parsed;
            return decimal.TryParse(Convert.ToString(value), out parsed) ? parsed : 0;
        }

        private void AllowNewRow()
        {
            if (dgvSale.CurrentRow != null && dgvSale.CurrentRow.Index == dgvSale.Rows[dgvSale.Rows.Count - 1].Index)
            {
                if (dgvSale.CurrentRow.Cells[clnItemNo.Index].Value != null && !string.IsNullOrWhiteSpace(dgvSale.CurrentRow.Cells[clnItemNo.Index].Value.ToString()))
                {
                    int newIdx = dgvSale.Rows.Add();
                    ClearRowValues(newIdx);
                }
            }
        }

        private void ClearRowValues(int rowIdx)
        {
            if (rowIdx < 0 || rowIdx >= dgvSale.Rows.Count) return;
            var row = dgvSale.Rows[rowIdx];
            row.Cells[clnItemNo.Index].Value = null;
            row.Cells[clnItemKey.Index].Value = null;
            if (clnUnit != null) row.Cells[clnUnit.Index].Value = null;
            row.Cells[clnQty.Index].Value = null;
            row.Cells[clnRate.Index].Value = null;
            row.Cells[clnAddless.Index].Value = null;
            row.Cells[clnAmount.Index].Value = null;
            if (dgvSale.Columns.Contains("clnSecQty")) row.Cells["clnSecQty"].Value = null;
            if (dgvSale.Columns.Contains("clnSecRate")) row.Cells["clnSecRate"].Value = null;
            row.Tag = null;
        }

        private void RecalculateRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvSale.Rows.Count) return;

            decimal qty = ParseDecimal(dgvSale[clnQty.Index, rowIndex].Value);
            decimal rate = ParseDecimal(dgvSale[clnRate.Index, rowIndex].Value);
            decimal addLess = ParseDecimal(dgvSale[clnAddless.Index, rowIndex].Value);

            decimal secQty = 0;
            decimal secRate = 0;
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                secQty = ParseDecimal(dgvSale["clnSecQty", rowIndex].Value);
                secRate = ParseDecimal(dgvSale["clnSecRate", rowIndex].Value);
            }

            dgvSale[clnQty.Index, rowIndex].Value = qty.ToString();
            dgvSale[clnRate.Index, rowIndex].Value = rate.ToString();
            dgvSale[clnAddless.Index, rowIndex].Value = addLess.ToString();
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                dgvSale["clnSecQty", rowIndex].Value = secQty.ToString();
                dgvSale["clnSecRate", rowIndex].Value = secRate.ToString();
            }

            string rowItemId = Convert.ToString(dgvSale[clnItemNo.Index, rowIndex].Value);
            DataRow rowItem = !string.IsNullOrWhiteSpace(rowItemId)
                ? dtItems.Select("Id = '" + rowItemId.Replace("'", "''") + "'").FirstOrDefault()
                : null;
            string selectedUnitId = Convert.ToString(dgvSale[clnUnit.Index, rowIndex].Value);
            decimal baseQty = UnitConversionHelper.ConvertToBasePrimaryQty(qty, selectedUnitId, rowItem, dtUnits);

            dgvSale[clnAmount.Index, rowIndex].Value = decimal.Round((baseQty * rate) + addLess + (secQty * secRate), 2).ToString();
            CalcTotAmount();
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
                clnAddless.DisplayIndex = dIdx++;
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
                clnAddless.DisplayIndex = dIdx++;
                clnAmount.DisplayIndex = dIdx++;
                if (clnStatus != null) clnStatus.DisplayIndex = dIdx++;
            }
        }

        private void SetupMobileShop()
        {
            if (ApiSession.HasMobileShopFeature)
            {
                if (txtSellerCnic == null)
                {
                    lblSellerCnic = new Label { Text = "Seller CNIC:", AutoSize = true, Location = new Point(550, 72) };
                    txtSellerCnic = new TextBox { Location = new Point(630, 70), Width = 130 };
                    lblSellerContact = new Label { Text = "Seller Mobile:", AutoSize = true, Location = new Point(550, 96) };
                    txtSellerContact = new TextBox { Location = new Point(630, 94), Width = 130 };
                    grpInvoiceDetail.Controls.Add(lblSellerCnic);
                    grpInvoiceDetail.Controls.Add(txtSellerCnic);
                    grpInvoiceDetail.Controls.Add(lblSellerContact);
                    grpInvoiceDetail.Controls.Add(txtSellerContact);
                }

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
                    dgvSale.Columns.Insert(insertIndex + 3, colBattery);
                    dgvSale.Columns.Insert(insertIndex + 4, colCondition);
                }
                else
                {
                    dgvSale.Columns["clnImei"].Visible = true;
                    dgvSale.Columns["clnImei2"].Visible = true;
                    dgvSale.Columns["clnPtaStatus"].Visible = true;
                    dgvSale.Columns["clnBatteryHealth"].Visible = true;
                    dgvSale.Columns["clnConditionNote"].Visible = true;
                }
            }
            else
            {
                if (txtSellerCnic != null)
                {
                    lblSellerCnic.Visible = false;
                    txtSellerCnic.Visible = false;
                    lblSellerContact.Visible = false;
                    txtSellerContact.Visible = false;
                }
                if (dgvSale.Columns.Contains("clnImei")) dgvSale.Columns["clnImei"].Visible = false;
                if (dgvSale.Columns.Contains("clnImei2")) dgvSale.Columns["clnImei2"].Visible = false;
                if (dgvSale.Columns.Contains("clnPtaStatus")) dgvSale.Columns["clnPtaStatus"].Visible = false;
                if (dgvSale.Columns.Contains("clnBatteryHealth")) dgvSale.Columns["clnBatteryHealth"].Visible = false;
                if (dgvSale.Columns.Contains("clnConditionNote")) dgvSale.Columns["clnConditionNote"].Visible = false;
            }
        }

        private async void frmPurchase_Load(object sender, EventArgs e)
        {
            try
            {
                SetupSecondaryQtyColumns();
                SetupMobileShop();
                ApplyBarcodeFastScanVisibility();
                await LoadLookupsAsync();
                await FillQueryAsync();
                if (_queryList.Count > 0 && txtVoucherNo.Text == "")
                    await FillPurchaseAsync(_queryList[VoucherIndex].VoucherNo);
                FLogIn = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyBarcodeFastScanVisibility()
        {
            try
            {
                if (groupBox2 == null) return;
                int bottomBoundary = Math.Min(panel1.Top, groupBox3.Top);

                if (!ApiSession.IsBarcodeFastScanEnabled)
                {
                    groupBox2.Visible = false;
                    dgvSale.Location = new System.Drawing.Point(groupBox2.Location.X, groupBox2.Location.Y);
                    dgvSale.Height = bottomBoundary - dgvSale.Top - 8;
                }
                else
                {
                    groupBox2.Visible = true;
                    dgvSale.Location = new System.Drawing.Point(groupBox2.Location.X, groupBox2.Bottom + 6);
                    dgvSale.Height = bottomBoundary - dgvSale.Top - 8;
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
            else if (e.KeyCode == Keys.F1)
            {
                btnPreview.PerformClick();
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnNew.PerformClick();
            }
            else if (e.KeyCode == Keys.F5)
            {
                btnSave.PerformClick();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                btnClose.PerformClick();
            }
            else if (!txtBarcode.Focused && e.KeyCode == Keys.Enter && !dgvSale.Focused)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{tab}");
            }
        }

        private void dgvPurchase_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox cmb)
            {
                cmb.SelectedIndexChanged -= new EventHandler(cmbItem_SelectedIndexChanged);
                cmb.SelectionChangeCommitted -= new EventHandler(cmbItem_SelectionChangeCommitted);
                cmb.SelectionChangeCommitted -= new EventHandler(cmbUnit_SelectionChangeCommitted);

                if (dgvSale.CurrentCell != null && dgvSale.CurrentCell.ColumnIndex == clnItemNo.Index)
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

                    cmb.SelectedIndexChanged += new EventHandler(cmbItem_SelectedIndexChanged);
                    cmb.SelectionChangeCommitted += new EventHandler(cmbItem_SelectionChangeCommitted);
                }
                else if (dgvSale.CurrentCell != null && dgvSale.CurrentCell.ColumnIndex == clnUnit.Index)
                {
                    cmb.DropDownStyle = ComboBoxStyle.DropDownList;
                    var cellVal = dgvSale.CurrentCell.Value;
                    if (cellVal != null && !string.IsNullOrWhiteSpace(cellVal.ToString()))
                    {
                        try { cmb.SelectedValue = cellVal.ToString(); } catch { }
                    }
                    cmb.SelectionChangeCommitted += new EventHandler(cmbUnit_SelectionChangeCommitted);
                }
            }
            else if (dgvSale.CurrentCell != null && (
                dgvSale.CurrentCell.ColumnIndex == clnRate.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQty.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnAddless.Index ||
                (dgvSale.Columns.Contains("clnSecQty") && dgvSale.CurrentCell.ColumnIndex == dgvSale.Columns["clnSecQty"].Index) ||
                (dgvSale.Columns.Contains("clnSecRate") && dgvSale.CurrentCell.ColumnIndex == dgvSale.Columns["clnSecRate"].Index)))
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null && e.Control.Text != null)
                {
                    if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index || (dgvSale.Columns.Contains("clnSecRate") && dgvSale.CurrentCell.ColumnIndex == dgvSale.Columns["clnSecRate"].Index))
                    {
                        tb.KeyPress -= new KeyPressEventHandler(tbRate_KeyPress);
                        tb.KeyPress += new KeyPressEventHandler(tbRate_KeyPress);
                    }
                    else if (dgvSale.CurrentCell.ColumnIndex == clnAddless.Index)
                    {
                        tb.KeyPress -= new KeyPressEventHandler(tbAddless_KeyPress);
                        tb.KeyPress += new KeyPressEventHandler(tbAddless_KeyPress);
                    }
                    else
                    {
                        tb.KeyPress -= new KeyPressEventHandler(tbQty_KeyPress);
                        tb.KeyPress += new KeyPressEventHandler(tbQty_KeyPress);
                    }
                }
            }
        }

        private void tbAddless_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvSale.CurrentCell.ColumnIndex == clnAddless.Index)
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
                    && !((sender as TextBox).Text.Count(a => a == '.') == 0 && e.KeyChar == '.')
                    && !(((sender as TextBox).Text.Count(a => a == '-') == 0 && e.KeyChar == '-') && (sender as TextBox).SelectionStart == 0))
                {
                    e.Handled = true;
                }
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
            if (dgvSale.CurrentCell == null || dgvSale.CurrentCell.ColumnIndex != clnItemNo.Index)
                return;

            if (sender is ComboBox cmb && cmb.SelectedIndex != -1)
            {
                DataRowView dr = cmb.SelectedItem as DataRowView;
                if (dr == null || !dr.Row.Table.Columns.Contains("Id"))
                    return;

                if (string.IsNullOrWhiteSpace(cmb.Text) && (dgvSale.CurrentCell.Value == null || string.IsNullOrWhiteSpace(dgvSale.CurrentCell.Value.ToString())))
                    return;

                if (cmb.SelectedValue == null)
                    cmb.SelectedValue = dr["Id"].ToString();

                int rowInd = dgvSale.CurrentCellAddress.Y;
                if (rowInd >= 0 && rowInd < dgvSale.Rows.Count)
                {
                    dgvSale[clnItemNo.Index, rowInd].Value = dr["Id"].ToString();
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

        private void cmbItem_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (dgvSale.CurrentCell != null && dgvSale.CurrentCell.ColumnIndex != clnItemNo.Index)
                return;

            if (sender is ComboBox cmb && cmb.SelectedItem is DataRowView drv)
            {
                if (!drv.Row.Table.Columns.Contains("Id"))
                    return;

                int rowInd = dgvSale.CurrentCellAddress.Y;
                if (rowInd >= 0 && rowInd < dgvSale.Rows.Count)
                {
                    string id = drv["Id"].ToString();
                    dgvSale[clnItemNo.Index, rowInd].Value = id;
                    if (drv.Row.Table.Columns.Contains("ItemKey"))
                        dgvSale[clnItemKey.Index, rowInd].Value = drv["ItemKey"]?.ToString();
                    SetcmbUnitSource(drv, rowInd);
                }
            }
        }

        private void cmbUnit_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (dgvSale.CurrentCell != null && dgvSale.CurrentCell.ColumnIndex != clnUnit.Index)
                return;

            if (sender is ComboBox cmb)
            {
                int rowInd = dgvSale.CurrentCellAddress.Y;
                if (rowInd >= 0 && rowInd < dgvSale.Rows.Count)
                {
                    string unitCode = cmb.SelectedValue?.ToString();
                    if (string.IsNullOrEmpty(unitCode) && cmb.SelectedItem is DataRowView drv && drv.Row.Table.Columns.Contains("Code"))
                    {
                        unitCode = drv["Code"]?.ToString();
                    }

                    if (!string.IsNullOrEmpty(unitCode))
                    {
                        dgvSale[clnUnit.Index, rowInd].Value = unitCode;
                        RecalculateRow(rowInd);
                    }
                }
            }
        }

        private void SetcmbUnitSource(DataRowView dr, int rowInd)
        {
            if (dr == null || !dr.Row.Table.Columns.Contains("Id") || rowInd < 0 || rowInd >= dgvSale.Rows.Count)
                return;

            dgvSale[clnRate.Index, rowInd].Value = dr["PriRate"].ToString();
            UnitConversionHelper.BindRowUnitComboBox(dgvSale.Rows[rowInd], clnUnit.Index, dr.Row, dtUnits);

            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                if (dgvSale["clnSecQty", rowInd].Value == null || string.IsNullOrWhiteSpace(dgvSale["clnSecQty", rowInd].Value.ToString()))
                    dgvSale["clnSecQty", rowInd].Value = "0";
                dgvSale["clnSecRate", rowInd].Value = dr["SecRate"]?.ToString() ?? "0";
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
            currentRow = e.RowIndex;
            RecalculateRow(e.RowIndex);
        }

        private void dgvPurchase_SelectionChanged(object sender, EventArgs e)
        {
            if (resetRow)
            {
                resetRow = false;
                dgvSale.CurrentCell = dgvSale.Rows[currentRow].Cells[dgvSale.CurrentCell.ColumnIndex];
            }
        }

        private bool HandleGridEnterNavigation()
        {
            if (dgvSale.CurrentCell == null) return false;

            int rowIndex = dgvSale.CurrentCell.RowIndex;
            int colIndex = dgvSale.CurrentCell.ColumnIndex;

            // 0. If on Item Key column
            if (colIndex == clnItemKey.Index)
            {
                dgvSale.EndEdit();
                var currentKeyVal = dgvSale[clnItemKey.Index, rowIndex].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(currentKeyVal))
                {
                    DataRow dr = dtItems.Select("ItemKey = '" + currentKeyVal.Replace("'", "''") + "'").FirstOrDefault();
                    if (dr != null)
                    {
                        DataRowView drv = dtItems.DefaultView[dtItems.Rows.IndexOf(dr)];
                        SetcmbItemSource(dr["fkItemcatagory"].ToString(), rowIndex);
                        dgvSale[clnItemNo.Index, rowIndex].Value = dr["Id"].ToString();
                        SetcmbUnitSource(drv, rowIndex);
                    }
                }

                dgvSale.CurrentCell = dgvSale[clnItemNo.Index, rowIndex];
                dgvSale.BeginEdit(true);
                return true;
            }

            // 1. If on Item Description column (clnItemNo)
            if (colIndex == clnItemNo.Index)
            {
                if (dgvSale.EditingControl is ComboBox cmb)
                {
                    if (cmb.SelectedItem is DataRowView drv)
                    {
                        string id = drv["Id"].ToString();
                        dgvSale[clnItemNo.Index, rowIndex].Value = id;
                        if (drv.Row.Table.Columns.Contains("ItemKey"))
                            dgvSale[clnItemKey.Index, rowIndex].Value = drv["ItemKey"]?.ToString();
                        SetcmbUnitSource(drv, rowIndex);
                    }
                    else if (!string.IsNullOrWhiteSpace(cmb.Text))
                    {
                        string text = cmb.Text.Trim();
                        var match = dtItems.AsEnumerable().FirstOrDefault(r =>
                            string.Equals(r["Title"]?.ToString(), text, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(r["ItemKey"]?.ToString(), text, StringComparison.OrdinalIgnoreCase));
                        if (match == null)
                        {
                            match = dtItems.AsEnumerable().FirstOrDefault(r =>
                                r["Title"]?.ToString().StartsWith(text, StringComparison.OrdinalIgnoreCase) == true);
                        }
                        if (match != null)
                        {
                            string id = match["Id"].ToString();
                            dgvSale[clnItemNo.Index, rowIndex].Value = id;
                            var drvMatch = dtItems.DefaultView[dtItems.Rows.IndexOf(match)];
                            if (match.Table.Columns.Contains("ItemKey"))
                                dgvSale[clnItemKey.Index, rowIndex].Value = match["ItemKey"]?.ToString();
                            SetcmbUnitSource(drvMatch, rowIndex);
                        }
                    }
                }

                dgvSale.EndEdit();

                var itemVal = dgvSale[clnItemNo.Index, rowIndex].Value;
                bool isItemEmpty = itemVal == null || string.IsNullOrWhiteSpace(itemVal.ToString());
                bool isLastRow = (rowIndex == dgvSale.Rows.Count - 1);

                // If on last row without selecting item -> Exit grid to txtCashReceipt!
                if (isLastRow && isItemEmpty)
                {
                    dgvSale.EndEdit();
                    CalcTotAmount();
                    txtCashReceipt.Text = txtTotAmount.Text;
                    txtCashReceipt.Focus();
                    txtCashReceipt.SelectAll();
                    return true;
                }

                // If an item is selected
                if (!isItemEmpty)
                {
                    if (dgvSale[clnQty.Index, rowIndex].Value == null || string.IsNullOrWhiteSpace(dgvSale[clnQty.Index, rowIndex].Value.ToString()) || dgvSale[clnQty.Index, rowIndex].Value.ToString() == "0")
                    {
                        dgvSale[clnQty.Index, rowIndex].Value = "1";
                    }
                    RecalculateRow(rowIndex);

                    // Check if unit is visible and HAS MORE THAN ONE UNIT
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
            if (colIndex == clnUnit.Index)
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
                RecalculateRow(rowIndex);
                dgvSale.CurrentCell = dgvSale[clnQty.Index, rowIndex];
                dgvSale.BeginEdit(true);
                return true;
            }

            // 3. If on Qty column
            if (colIndex == clnQty.Index)
            {
                dgvSale.EndEdit();
                RecalculateRow(rowIndex);

                bool hasSecQty = ApiSession.HasSecondaryQty &&
                                 dgvSale.Columns.Contains("clnSecQty") &&
                                 dgvSale.Columns["clnSecQty"].Visible;

                if (hasSecQty)
                {
                    dgvSale.CurrentCell = dgvSale.Rows[rowIndex].Cells[dgvSale.Columns["clnSecQty"].Index];
                    dgvSale.BeginEdit(true);
                    return true;
                }

                // Move to next row's Item No
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
                dgvSale.CurrentCell = dgvSale[clnItemNo.Index, nextRowIdx];
                dgvSale.BeginEdit(true);
                return true;
            }

            // 4. If on secondary qty or rate or addless
            bool isSecQtyCol = dgvSale.Columns.Contains("clnSecQty") && colIndex == dgvSale.Columns["clnSecQty"].Index;
            if (isSecQtyCol || colIndex == clnRate.Index || colIndex == clnAddless.Index)
            {
                dgvSale.EndEdit();
                RecalculateRow(rowIndex);

                // Move to next row's Item No
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
                dgvSale.CurrentCell = dgvSale[clnItemNo.Index, nextRowIdx];
                dgvSale.BeginEdit(true);
                return true;
            }

            // Fallback
            dgvSale.EndEdit();
            RecalculateRow(rowIndex);
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
            dgvSale.CurrentCell = dgvSale[clnItemNo.Index, targetIdx];
            dgvSale.BeginEdit(true);
            return true;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to save this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            if (cmbAccounts.SelectedValue == null)
            {
                MessageBox.Show("Please Select Account...!");
                return;
            }

            var lines = new List<PurchaseLineRequest>();
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

                lines.Add(new PurchaseLineRequest
                {
                    Seq = int.Parse(Convert.ToString(row.Cells[clnSeq.Index].Value)),
                    ItemId = Convert.ToString(row.Cells[clnItemNo.Index].Value),
                    Qty = baseQty,
                    Rate = ParseDecimal(row.Cells[clnRate.Index].Value),
                    AddLess = ParseDecimal(row.Cells[clnAddless.Index].Value),
                    SecQty = secQty,
                    SecRate = secRate,
                    SecUnit = secUnit,
                    Imei = ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnImei") ? Convert.ToString(row.Cells["clnImei"].Value) : null,
                    Imei2 = ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnImei2") ? Convert.ToString(row.Cells["clnImei2"].Value) : null,
                    PtaStatus = ApiSession.HasMobileShopFeature && dgvSale.Columns.Contains("clnPtaStatus") ? Convert.ToString(row.Cells["clnPtaStatus"].Value) : null,
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
                    var createRequest = new PurchaseCreateRequest
                    {
                        Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                        Account = cmbAccounts.SelectedValue.ToString(),
                        Description = txtDescription.Text,
                        Narration = cmbNarration.SelectedValue?.ToString(),
                        CashPaid = txtCashReceipt.Value,
                        CashBack = txtCashBack.Value,
                        SellerCnic = ApiSession.HasMobileShopFeature && txtSellerCnic != null ? txtSellerCnic.Text.Trim() : null,
                        SellerContact = ApiSession.HasMobileShopFeature && txtSellerContact != null ? txtSellerContact.Text.Trim() : null,
                        Lines = lines
                    };
                    voucher = await _apiService.CreateAsync(createRequest);
                }
                else
                {
                    var updateRequest = new PurchaseUpdateRequest
                    {
                        Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                        Account = cmbAccounts.SelectedValue.ToString(),
                        Description = txtDescription.Text,
                        Narration = cmbNarration.SelectedValue?.ToString(),
                        CashPaid = txtCashReceipt.Value,
                        CashBack = txtCashBack.Value,
                        SellerCnic = ApiSession.HasMobileShopFeature && txtSellerCnic != null ? txtSellerCnic.Text.Trim() : null,
                        SellerContact = ApiSession.HasMobileShopFeature && txtSellerContact != null ? txtSellerContact.Text.Trim() : null,
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
                MessageBox.Show("Error saving purchase: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                string voucher = txtFilterVoucher.Text;
                if (!string.IsNullOrWhiteSpace(voucher) && voucher.StartsWith("PU-", StringComparison.OrdinalIgnoreCase))
                    voucher = voucher.Substring(3);

                await FillQueryAsync(
                    txtFdate.Text,
                    txtTdate.Text,
                    cmbFilterAccounts.SelectedValue != null ? cmbFilterAccounts.SelectedValue.ToString() : "",
                    voucher);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (!Regex.IsMatch(txtFilterVoucher.Text, @"PU-\d{5}"))
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
            txtVoucherNo.Text = "";
            lblTotAmount.Text = "0";
            txtTotAmount.Text = "0";
            txtCashReceipt.Text = "0";
            txtCashBack.Text = "0";
            txtBalance.Text = "0";
            dgvSale.Rows.Add();
            if (dgvSale.Rows.Count > 0 && clnItemNo != null)
            {
                try
                {
                    dgvSale.CurrentCell = dgvSale[clnItemNo.Index, 0];
                }
                catch { }
            }
            cmbAccounts.SelectedIndex = -1;
            cmbNarration.SelectedIndex = -1;
            cmbAccounts.Focus();
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

        private void txtCashBack_TextChanged(object sender, EventArgs e)
        {
            txtBalance.Text = (txtTotAmount.Value - (txtCashReceipt.Value - txtCashBack.Value)).ToString("N2");
        }

        void CopyAsNew()
        {
            if (txtVoucherNo.Text != "")
            {
                txtVoucherNo.Text = "";
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
                    MessageBox.Show("Error deleting purchase: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    decimal displayScanQty = qtyToScan;
                    string displayScanUnit = defaultUnit;
                    UnitConversionHelper.ResolveDisplayQtyAndUnit(qtyToScan, dr, dtUnits, out displayScanQty, out displayScanUnit);
                    UnitConversionHelper.BindRowUnitComboBox(dgvSale.Rows[ind], clnUnit.Index, dr, dtUnits, displayScanUnit);

                    dgvSale[clnRate.Index, ind].Value = itemPriRate.ToString("0.##");
                    dgvSale[clnQty.Index, ind].Value = displayScanQty.ToString("0.####");
                    if (dgvSale[clnAddless.Index, ind].Value == null)
                        dgvSale[clnAddless.Index, ind].Value = "0";
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
                decimal addLess = ParseDecimal(dgvSale[clnAddless.Index, ind].Value);
                decimal secQty = 0m;
                decimal secRate = 0m;
                if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                {
                    secQty = ParseDecimal(dgvSale["clnSecQty", ind].Value);
                    secRate = ParseDecimal(dgvSale["clnSecRate", ind].Value);
                    dgvSale["clnSecQty", ind].Value = secQty.ToString("0.##");
                    dgvSale["clnSecRate", ind].Value = secRate.ToString("0.##");
                }

                dgvSale[clnQty.Index, ind].Value = qty.ToString("0.####");
                dgvSale[clnRate.Index, ind].Value = rate.ToString("0.##");
                dgvSale[clnAddless.Index, ind].Value = addLess.ToString("0.##");
                dgvSale[clnAmount.Index, ind].Value = decimal.Round((baseQtyOnRow * rate) + addLess + (secQty * secRate), 2).ToString("N2");
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
                DataSet ds = ReportQuery.PurchaseBill(txtVoucherNo.Text.Substring(3));
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
