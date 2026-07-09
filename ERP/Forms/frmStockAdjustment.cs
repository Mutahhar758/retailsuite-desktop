using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmStockAdjustment : Form
    {
        private readonly StockAdjustmentApiService _apiService;
        private readonly NarrationApiService _narrationApiService;
        private readonly ItemCategoryApiService _itemCategoryApiService;
        private readonly UnitApiService _unitApiService;
        private readonly InventoryApiService _inventoryApiService;
        private List<StockAdjustmentDto> _queryList = new List<StockAdjustmentDto>();

        private readonly DataTable dtItems = new DataTable();
        private readonly DataTable dtUnits = new DataTable();
        private readonly DataTable dtCategories = new DataTable();
        private readonly DataTable dtNarration = new DataTable();

        private int currentRow;
        private bool resetRow = false;
        private static bool FLogIn = true;
        private string VoucherNum = null;
        private bool _isSaving;

        enum Navigators { Up, Down, Home, End }

        public frmStockAdjustment()
        {
            InitializeComponent();
            _apiService = new StockAdjustmentApiService();
            _narrationApiService = new NarrationApiService();
            _itemCategoryApiService = new ItemCategoryApiService();
            _unitApiService = new UnitApiService();
            _inventoryApiService = new InventoryApiService();
            InitializeLookupTables();
            dgvSale.Rows.Add();
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

            dtUnits.Columns.Add("Code", typeof(string));
            dtUnits.Columns.Add("Title", typeof(string));

            dtCategories.Columns.Add("Code", typeof(string));
            dtCategories.Columns.Add("Title", typeof(string));

            dtNarration.Columns.Add("Code", typeof(string));
            dtNarration.Columns.Add("Title", typeof(string));
        }

        private async System.Threading.Tasks.Task LoadLookupsAsync()
        {
            var narrationsTask = _narrationApiService.GetActiveNarrationsAsync();
            var categoriesTask = _itemCategoryApiService.GetActiveAsync();
            var unitsTask = _unitApiService.GetActiveAsync();
            var itemsTask = _inventoryApiService.GetItemsAsync(null);
            await System.Threading.Tasks.Task.WhenAll(narrationsTask, categoriesTask, unitsTask, itemsTask);

            dtItems.Rows.Clear();
            foreach (var item in itemsTask.Result)
                dtItems.Rows.Add(item.Id, item.Title, item.ItemCategoryCode, item.ItemKey, item.Barcode, item.PriRate, item.SecRate, item.PrimaryUnit, item.SecondaryUnit, item.DefaultUnit);

            dtUnits.Rows.Clear();
            foreach (var u in unitsTask.Result)
                dtUnits.Rows.Add(u.Code, u.Title);

            dtCategories.Rows.Clear();
            foreach (var c in categoriesTask.Result)
                dtCategories.Rows.Add(c.Code, c.Title);

            dtNarration.Rows.Clear();
            foreach (var n in narrationsTask.Result)
                dtNarration.Rows.Add(n.Code, n.Title);

            FillNarration();
            FillCategories();
            FillFilterCategories();
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
            clnCatagory.DataSource = dtCategories.Copy();
            clnCatagory.DisplayMember = "Title";
            clnCatagory.ValueMember = "Code";
        }

        private void FillFilterCategories()
        {
            cmbFilterAccounts.DataSource = dtCategories.Copy();
            cmbFilterAccounts.DisplayMember = "Title";
            cmbFilterAccounts.ValueMember = "Code";
            cmbFilterAccounts.SelectedIndex = -1;
        }

        private static decimal ParseDecimal(object value)
        {
            decimal parsed;
            return decimal.TryParse(Convert.ToString(value), out parsed) ? parsed : 0;
        }

        void CalcTotAmount()
        {
            decimal totAmount = 0;
            try
            {
                totAmount = (from DataGridViewRow row in dgvSale.Rows
                             where row.Cells[clnAmount.Index].Value != null
                             select ParseDecimal(row.Cells[clnAmount.Index].Value)).Sum();
            }
            catch
            {
            }

            txtTotAmount.Text = totAmount.ToString("N2");
            if (txtVoucherNo.Text == "")
                txtCashReceipt.Text = totAmount.ToString("N2");
        }

        private async System.Threading.Tasks.Task SaveAsync()
        {
            var lines = new List<StockAdjustmentLineApiRequest>();
            foreach (DataGridViewRow row in dgvSale.Rows)
            {
                if (row.Cells[clnItemNo.Index].Value == null)
                    continue;

                decimal secQtyIn = 0;
                decimal secQtyOut = 0;
                decimal secRate = 0;
                string secUnit = null;
                if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQtyIn"))
                {
                    secQtyIn = ParseDecimal(row.Cells["clnSecQtyIn"].Value);
                    secQtyOut = ParseDecimal(row.Cells["clnSecQtyOut"].Value);
                    secRate = ParseDecimal(row.Cells["clnSecRate"].Value);
                    
                    string itemId = Convert.ToString(row.Cells[clnItemNo.Index].Value);
                    DataRow itemRow = dtItems.Select("Id = '" + itemId.Replace("'", "''") + "'").FirstOrDefault();
                    if (itemRow != null)
                    {
                        secUnit = Convert.ToString(itemRow["SecondaryUnit"]);
                    }
                }

                lines.Add(new StockAdjustmentLineApiRequest
                {
                    Seq = int.Parse(Convert.ToString(row.Cells[clnSeq.Index].Value)),
                    ItemCategoryCode = Convert.ToString(row.Cells[clnCatagory.Index].Value),
                    ItemId = Convert.ToString(row.Cells[clnItemNo.Index].Value),
                    Unit = ApiSession.HasSecondaryQty ? null : Convert.ToString(row.Cells[clnUnit.Index].Value),
                    QtyIn = ParseDecimal(row.Cells[clnQtyIn.Index].Value),
                    QtyOut = ParseDecimal(row.Cells[clnQtyOut.Index].Value),
                    Rate = ParseDecimal(row.Cells[clnRate.Index].Value),
                    SecQtyIn = secQtyIn,
                    SecQtyOut = secQtyOut,
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

                _isSaving = true;
                if (isNew)
                {
                    var request = new StockAdjustmentCreateApiRequest
                    {
                        Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                        Description = txtDescription.Text,
                        Narration = cmbNarration.SelectedValue?.ToString(),
                        Lines = lines
                    };
                    voucher = await _apiService.CreateAsync(request);
                }
                else
                {
                    var request = new StockAdjustmentUpdateApiRequest
                    {
                        Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                        Description = txtDescription.Text,
                        Narration = cmbNarration.SelectedValue?.ToString(),
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
                MessageBox.Show("Error saving stock adjustment: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isSaving = false;
            }
        }

        private async System.Threading.Tasks.Task FillQueryAsync(string fromDate = "", string toDate = "", string itemCategoryCode = "", string voucherNo = "")
        {
            dgvQuery.Rows.Clear();
            _queryList = await _apiService.GetListAsync(fromDate, toDate, itemCategoryCode, voucherNo);

            for (int i = 0; i < _queryList.Count; i++)
            {
                var row = _queryList[i];
                dgvQuery.Rows.Add(
                    row.Date.ToString("dd-MMM-yyyy"),
                    "SA-" + row.VoucherNo,
                    row.Description,
                    row.CreatedBy + " | " + row.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                    !string.IsNullOrWhiteSpace(row.LastModifiedBy)
                        ? row.LastModifiedBy + " | " + row.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                        : null);
            }
        }

        internal async System.Threading.Tasks.Task FillSaleAsync(string vno)
        {
            var lines = await _apiService.GetDetailAsync(vno);
            txtVoucherNo.Text = "SA-" + vno;
            VoucherNum = vno;
            dgvSale.Rows.Clear();

            if (lines.Count > 0)
            {
                var first = lines[0];
                dtpDate.Value = first.Date;
                if (string.IsNullOrWhiteSpace(first.Narration))
                    cmbNarration.SelectedIndex = -1;
                else
                    cmbNarration.SelectedValue = first.Narration;
                txtDescription.Text = first.Description;
                txtCreatedBy.Text = first.CreatedBy + " | " + first.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt");
                txtEditBy.Text = !string.IsNullOrWhiteSpace(first.LastModifiedBy)
                    ? first.LastModifiedBy + " | " + first.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                    : null;

                for (int i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    
                    int ind = dgvSale.Rows.Add();
                    var row = dgvSale.Rows[ind];
                    
                    row.Cells[clnSeq.Index].Value = line.Seq.ToString();
                    row.Cells[clnItemKey.Index].Value = line.ItemKey;
                    row.Cells[clnCatagory.Index].Value = line.ItemCategoryCode;
                    row.Cells[clnItemNo.Index].Value = null;
                    row.Cells[clnUnit.Index].Value = null;
                    row.Cells[clnQtyIn.Index].Value = line.QtyIn.ToString("0.##");
                    row.Cells[clnQtyOut.Index].Value = line.QtyOut.ToString("0.##");
                    row.Cells[clnRate.Index].Value = line.Rate.ToString("0.##");
                    row.Cells[clnAmount.Index].Value = line.Amount.ToString("N2");
                    row.Cells[clnStatus.Index].Value = "0";


                    SetcmbItemSource(line.ItemCategoryCode, ind);
                    dgvSale[clnItemNo.Index, ind].Value = line.ItemId;

                    DataRow itemRow = dtItems.Select("Id = '" + line.ItemId.Replace("'", "''") + "'").FirstOrDefault();
                    if (itemRow != null)
                    {
                        DataRowView drv = dtItems.DefaultView[dtItems.Rows.IndexOf(itemRow)];
                        SetcmbUnitSource(drv, ind);
                    }
                    else
                    {
                        SetAllUnitsSource(ind);
                    }

                    dgvSale[clnUnit.Index, ind].Value = line.Unit;
                    dgvSale[clnRate.Index, ind].Value = line.Rate.ToString("0.##");

                    if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQtyIn"))
                    {
                        dgvSale["clnSecQtyIn", ind].Value = (line.SecQtyIn ?? 0).ToString("0.##");
                        dgvSale["clnSecQtyOut", ind].Value = (line.SecQtyOut ?? 0).ToString("0.##");
                        dgvSale["clnSecRate", ind].Value = (line.SecRate ?? 0).ToString("0.##");
                    }
                }
            }

            txtCashReceipt.Text = "0";
            txtCashBack.Text = "0";
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
                if (dgvSale.CurrentRow.Cells[clnItemNo.Index].Value != null)
                    dgvSale.Rows.Add();
            }
        }

        private void SetupSecondaryQtyColumns()
        {
            if (ApiSession.HasSecondaryQty)
            {
                clnUnit.Visible = false;
                clnQtyIn.HeaderText = "Single Qty In";
                clnQtyOut.HeaderText = "Single Qty Out";
                clnRate.HeaderText = "Single Rate";

                if (!dgvSale.Columns.Contains("clnSecQtyIn"))
                {
                    var colSecQtyIn = new DataGridViewTextBoxColumn
                    {
                        Name = "clnSecQtyIn",
                        HeaderText = "Pack Qty In",
                        Width = 80
                    };
                    var colSecQtyOut = new DataGridViewTextBoxColumn
                    {
                        Name = "clnSecQtyOut",
                        HeaderText = "Pack Qty Out",
                        Width = 80
                    };
                    var colSecRate = new DataGridViewTextBoxColumn
                    {
                        Name = "clnSecRate",
                        HeaderText = "Pack Rate",
                        Width = 80
                    };
                    int insertIndex = clnRate.Index;
                    dgvSale.Columns.Insert(insertIndex, colSecQtyIn);
                    dgvSale.Columns.Insert(insertIndex + 1, colSecQtyOut);
                    dgvSale.Columns.Insert(insertIndex + 2, colSecRate);
                }
            }
        }

        private async void frmPurchase_Load(object sender, EventArgs e)
        {
            try
            {
                SetupSecondaryQtyColumns();
                await LoadLookupsAsync();
                await FillQueryAsync();
                btnNew_Click(null, null);
                FLogIn = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading stock adjustment data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            }
        }

        private async void frmPurchase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !dgvSale.Focused)
            {
                SendKeys.Send("{tab}");
            }
            else if (e.KeyCode == Keys.F5)
            {
                await SaveAsync();
            }
        }

        private void dgvPurchase_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvSale.CurrentCellAddress.X == clnItemNo.DisplayIndex)
            {
                ComboBox cmbItem = e.Control as ComboBox;
                if (cmbItem != null)
                {
                    cmbItem.DropDownStyle = ComboBoxStyle.DropDown;
                    cmbItem.AutoCompleteSource = AutoCompleteSource.ListItems;
                    cmbItem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cmbItem.SelectedIndexChanged += new EventHandler(cmbItem_SelectedIndexChanged);
                }
            }
            else if (dgvSale.CurrentCellAddress.X == clnCatagory.DisplayIndex)
            {
                ComboBox cmbCatagory = e.Control as ComboBox;
                if (cmbCatagory != null)
                {
                    cmbCatagory.DropDownStyle = ComboBoxStyle.DropDown;
                    cmbCatagory.AutoCompleteSource = AutoCompleteSource.ListItems;
                    cmbCatagory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cmbCatagory.SelectedIndexChanged += new EventHandler(cmbCatagory_SelectedIndexChanged);
                }
            }
            else if (dgvSale.CurrentCellAddress.X == clnUnit.DisplayIndex)
            {
                ComboBox cmbunit = e.Control as ComboBox;
                if (cmbunit != null)
                {
                    cmbunit.SelectedIndexChanged += new EventHandler(cmbunit_SelectedIndexChanged);
                }
            }
            else if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQtyIn.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQtyOut.Index ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecQtyIn" ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecQtyOut" ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecRate")
            {
                TextBox tbRate = e.Control as TextBox;
                if (tbRate != null && e.Control.Text != null)
                {
                    tbRate.KeyPress += new KeyPressEventHandler(tbRate_KeyPress);
                }
            }
        }

        void cmbunit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvSale.CurrentCellAddress.X == clnUnit.DisplayIndex)
            {
                if ((sender as ComboBox).SelectedIndex != -1)
                {
                    if (dgvSale[clnItemNo.Index, dgvSale.CurrentCellAddress.Y].Value == null)
                        return;

                    string itemId = dgvSale[clnItemNo.Index, dgvSale.CurrentCellAddress.Y].Value.ToString();
                    DataRow dr = dtItems.Select("Id = '" + itemId.Replace("'", "''") + "'").FirstOrDefault();
                    if (dr != null)
                    {
                        dgvSale[clnRate.Index, dgvSale.CurrentCellAddress.Y].Value =
                            (sender as ComboBox).SelectedValue.ToString() == dr["SecondaryUnit"].ToString()
                                ? dr["SecRate"].ToString()
                                : dr["PriRate"].ToString();
                    }
                }
            }
        }

        void cmbCatagory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvSale.CurrentCellAddress.X == clnCatagory.DisplayIndex)
            {
                if ((sender as ComboBox).SelectedIndex != -1)
                {
                    DataRowView dr = (DataRowView)(sender as ComboBox).SelectedItem;
                    if ((sender as ComboBox).SelectedValue == null)
                        (sender as ComboBox).SelectedValue = dr[0].ToString();
                    SetcmbItemSource(dr[0].ToString(), dgvSale.CurrentCellAddress.Y);
                }
            }
        }

        private void SetcmbItemSource(string itemCategory, int rowInd)
        {
            DataTable dt = dtItems.Clone();
            DataRow[] rows = dtItems.Select("fkItemcatagory = '" + itemCategory.Replace("'", "''") + "'");
            if (rows.Length > 0)
            {
                for (int i = 0; i < rows.Length; i++)
                    dt.ImportRow(rows[i]);
            }

            DataGridViewComboBoxCell cmb = dgvSale[clnItemNo.Index, rowInd] as DataGridViewComboBoxCell;
            cmb.DataSource = dt;
            cmb.DisplayMember = "Title";
            cmb.ValueMember = "Id";
        }

        private void SetAllUnitsSource(int rowInd)
        {
            DataGridViewComboBoxCell cmb = dgvSale[clnUnit.Index, rowInd] as DataGridViewComboBoxCell;
            cmb.DataSource = dtUnits.Copy();
            cmb.DisplayMember = "Title";
            cmb.ValueMember = "Code";
        }

        void tbQty_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        void tbRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQtyIn.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQtyOut.Index ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecQtyIn" ||
                dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecQtyOut" ||
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

        void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvSale.CurrentCellAddress.X == clnItemNo.DisplayIndex)
            {
                if ((sender as ComboBox).SelectedIndex != -1)
                {
                    DataRowView dr = (DataRowView)(sender as ComboBox).SelectedItem;
                    if ((sender as ComboBox).SelectedValue == null)
                        (sender as ComboBox).SelectedValue = dr[0].ToString();
                    if (dr != null)
                    {
                        SetcmbUnitSource(dr, dgvSale.CurrentCellAddress.Y);
                    }
                }
            }
        }

        private void SetcmbUnitSource(DataRowView dr, int rowInd)
        {
            DataTable dt = dtUnits.Clone();
            string primaryUnit = Convert.ToString(dr["PrimaryUnit"]);
            string secondaryUnit = Convert.ToString(dr["SecondaryUnit"]);
            DataRow[] rows = dtUnits.Select("Code in ('" + primaryUnit.Replace("'", "''") + "','" + secondaryUnit.Replace("'", "''") + "')");
            if (rows.Length > 0)
            {
                for (int i = 0; i < rows.Length; i++)
                    dt.ImportRow(rows[i]);
            }

            DataGridViewComboBoxCell cmb = dgvSale[clnUnit.Index, rowInd] as DataGridViewComboBoxCell;
            cmb.DataSource = dt;
            cmb.DisplayMember = "Title";
            cmb.ValueMember = "Code";
            cmb.Value = dr["DefaultUnit"];
            if (ApiSession.HasSecondaryQty)
            {
                dgvSale[clnRate.Index, rowInd].Value = dr["PriRate"].ToString();
            }
            else
            {
                dgvSale[clnRate.Index, rowInd].Value = dr["DefaultUnit"].ToString() == dr["SecondaryUnit"].ToString() ? dr["SecRate"].ToString() : dr["PriRate"].ToString();
            }

            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQtyIn"))
            {
                dgvSale["clnSecQtyIn", rowInd].Value = "0";
                dgvSale["clnSecQtyOut", rowInd].Value = "0";
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
                    dgvSale[clnCatagory.Index, e.RowIndex].Value = dr["fkItemcatagory"].ToString();
                    SetcmbItemSource(dr["fkItemcatagory"].ToString(), dgvSale.CurrentCellAddress.Y);
                    dgvSale[clnItemNo.Index, e.RowIndex].Value = dr["Id"].ToString();
                    SetcmbUnitSource(drv, e.RowIndex);
                    dgvSale[clnUnit.Index, e.RowIndex].Value = dr["DefaultUnit"].ToString();
                }
            }

            AllowNewRow();
            resetRow = true;
            currentRow = e.RowIndex;

            decimal qtyIn = ParseDecimal(dgvSale[clnQtyIn.Index, e.RowIndex].Value);
            decimal qtyOut = ParseDecimal(dgvSale[clnQtyOut.Index, e.RowIndex].Value);
            decimal rate = ParseDecimal(dgvSale[clnRate.Index, e.RowIndex].Value);

            decimal secQtyIn = 0;
            decimal secQtyOut = 0;
            decimal secRate = 0;
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQtyIn"))
            {
                secQtyIn = ParseDecimal(dgvSale["clnSecQtyIn", e.RowIndex].Value);
                secQtyOut = ParseDecimal(dgvSale["clnSecQtyOut", e.RowIndex].Value);
                secRate = ParseDecimal(dgvSale["clnSecRate", e.RowIndex].Value);
            }

            dgvSale[clnQtyIn.Index, e.RowIndex].Value = qtyIn.ToString();
            dgvSale[clnQtyOut.Index, e.RowIndex].Value = qtyOut.ToString();
            dgvSale[clnRate.Index, e.RowIndex].Value = rate.ToString();
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQtyIn"))
            {
                dgvSale["clnSecQtyIn", e.RowIndex].Value = secQtyIn.ToString();
                dgvSale["clnSecQtyOut", e.RowIndex].Value = secQtyOut.ToString();
                dgvSale["clnSecRate", e.RowIndex].Value = secRate.ToString();
            }

            dgvSale[clnAmount.Index, e.RowIndex].Value = decimal.Round(((qtyIn - qtyOut) * rate) + ((secQtyIn - secQtyOut) * secRate), 2).ToString();
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
                if (!string.IsNullOrWhiteSpace(voucher) && voucher.StartsWith("SA-", StringComparison.OrdinalIgnoreCase))
                    voucher = voucher.Substring(3);

                await FillQueryAsync(
                    txtFdate.Text,
                    txtTdate.Text,
                    cmbFilterAccounts.SelectedValue != null ? cmbFilterAccounts.SelectedValue.ToString() : "",
                    voucher);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading stock adjustment data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (!Regex.IsMatch(txtFilterVoucher.Text, @"SA-\d{5}"))
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
            Validation.Clear(this);
            txtTotAmount.Text = "0";
            txtCashReceipt.Text = "0";
            txtCashBack.Text = "0";
            txtBalance.Text = "0";
            txtVoucherNo.Text = "";
            VoucherNum = null;
            txtCreatedBy.Text = "";
            txtEditBy.Text = "";
            txtDescription.Text = "";
            dgvSale.Rows.Clear();
            dgvSale.Rows.Add();
            cmbNarration.SelectedIndex = dtNarration.Rows.Count > 0 ? 0 : -1;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void Print()
        {
            if (txtVoucherNo.Text != "")
            {
                DataSet ds = ReportQuery.SaleBill(txtVoucherNo.Text.Substring(3));
                if (ds.Tables[1].Rows.Count > 0)
                {
                    Reports.SaleReceipt rpt = new Reports.SaleReceipt();

                    rpt.SetDataSource(ds.Tables[0]);
                    rpt.SetParameterValue("@companyname", CompanyInfo.CompanyName);
                    rpt.SetParameterValue("@Account", (string)ds.Tables[1].Rows[0]["Title"]);
                    rpt.SetParameterValue("@VoucherNo", txtVoucherNo.Text);
                    rpt.SetParameterValue("@Date", (DateTime)ds.Tables[1].Rows[0]["vdate"]);
                    rpt.SetParameterValue("@User", UserInfo.UserName);
                    rpt.SetParameterValue("@ServerDate", DateTime.Now);
                    rpt.SetParameterValue("@CashPaid", (Decimal)ds.Tables[1].Rows[0]["CashReceipt"]);
                    rpt.SetParameterValue("@CashBack", (Decimal)ds.Tables[1].Rows[0]["CashBack"]);
                    rpt.PrintOptions.PrinterName = ConfigInfo.ThermalPrinterName;
                    rpt.PrintToPrinter(1, true, 0, 0);
                }
            }
        }

        void Navigate(Navigators nav)
        {
            if (dgvQuery.CurrentRow != null && tbSaleQuery.SelectedTab == tbDetail)
            {
                int rowIndex = dgvQuery.CurrentRow.Index;
                if (nav == Navigators.Down && dgvQuery.Rows.Count - 1 > rowIndex)
                {
                    dgvQuery.CurrentCell = dgvQuery.Rows[rowIndex + 1].Cells[clnVoucherNum.Index];
                    string voucherNo = dgvQuery.CurrentCell.Value.ToString();
                    FillSale(voucherNo.Substring(3));
                }
                else if (nav == Navigators.Up && 0 < rowIndex)
                {
                    dgvQuery.CurrentCell = dgvQuery.Rows[rowIndex - 1].Cells[clnVoucherNum.Index];
                    string voucherNo = dgvQuery.CurrentCell.Value.ToString();
                    FillSale(voucherNo.Substring(3));
                }
                else if (nav == Navigators.Home)
                {
                    dgvQuery.CurrentCell = dgvQuery.Rows[0].Cells[clnVoucherNum.Index];
                    string voucherNo = dgvQuery.CurrentCell.Value.ToString();
                    FillSale(voucherNo.Substring(3));
                }
                else if (nav == Navigators.End)
                {
                    dgvQuery.CurrentCell = dgvQuery.Rows[dgvQuery.Rows.Count - 1].Cells[clnVoucherNum.Index];
                    string voucherNo = dgvQuery.CurrentCell.Value.ToString();
                    FillSale(voucherNo.Substring(3));
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
                            dgvQuery.Rows.Remove(row);

                        CalcTotAmount();
                        MessageBox.Show("Record Successfully Deleted..!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting stock adjustment: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void tbDetail_Click(object sender, EventArgs e)
        {
        }

        private void dgvSale_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (txtVoucherNo.Text != "")
            {
                DataSet ds = ReportQuery.SaleBill(txtVoucherNo.Text.Substring(3));
                if (ds.Tables[1].Rows.Count > 0)
                {
                    frmReportView frm = new frmReportView();
                    Reports.SaleBill2 rpt = new Reports.SaleBill2();

                    rpt.SetDataSource(ds.Tables[0]);
                    rpt.SetParameterValue("@HeaderLocation", Application.StartupPath + "\\rptHeader.jpg");
                    rpt.SetParameterValue("@Account", (string)ds.Tables[1].Rows[0]["Title"]);
                    rpt.SetParameterValue("@VoucherNo", txtVoucherNo.Text);
                    rpt.SetParameterValue("@VDate", (DateTime)ds.Tables[1].Rows[0]["vdate"]);
                    rpt.SetParameterValue("@Amount", ds.Tables[1].Rows[0]["Amount"]);
                    rpt.SetParameterValue("@Discount", ds.Tables[1].Rows[0]["Discount"]);
                    rpt.SetParameterValue("@NetAmount", ds.Tables[1].Rows[0]["NetAmount"]);
                    frm.rptViewer.ReportSource = rpt;
                    frm.ShowDialog();
                }
            }
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
    }
}
