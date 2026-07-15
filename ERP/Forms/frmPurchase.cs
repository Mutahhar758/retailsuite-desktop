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
    public partial class frmPurchase : Form
    {
        private readonly PurchaseApiService _apiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly NarrationApiService _narrationApiService;
        private readonly ItemCategoryApiService _itemCategoryApiService;
        private readonly UnitApiService _unitApiService;
        private readonly InventoryApiService _inventoryApiService;
        private List<PurchaseDto> _queryList = new List<PurchaseDto>();

        private readonly DataTable dtItems = new DataTable();
        private readonly DataTable dtUnits = new DataTable();
        private readonly DataTable dtCategories = new DataTable();
        private readonly DataTable dtAccounts = new DataTable();
        private readonly DataTable dtNarration = new DataTable();

        private int currentRow;
        private bool resetRow = false;
        private bool FLogIn = true;
        private int VoucherIndex = 0;

        enum Navigators { Up, Down, Home, End }

        public frmPurchase()
        {
            InitializeComponent();
            _apiService = new PurchaseApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _narrationApiService = new NarrationApiService();
            _itemCategoryApiService = new ItemCategoryApiService();
            _unitApiService = new UnitApiService();
            _inventoryApiService = new InventoryApiService();
            InitializeLookupTables();
            dgvSale.Rows.Add();
            UserInfo.ApplyFormPermissions(this, AppResource.Purchases);
        }

        private void InitializeLookupTables()
        {
            dtItems.Columns.Add("Id", typeof(string));
            dtItems.Columns.Add("Title", typeof(string));
            dtItems.Columns.Add("fkItemcatagory", typeof(string));
            dtItems.Columns.Add("ItemKey", typeof(string));
            dtItems.Columns.Add("PriRate", typeof(decimal));
            dtItems.Columns.Add("SecRate", typeof(decimal));
            dtItems.Columns.Add("PrimaryUnit", typeof(string));
            dtItems.Columns.Add("SecondaryUnit", typeof(string));
            dtItems.Columns.Add("DefaultUnit", typeof(string));

            dtUnits.Columns.Add("Code", typeof(string));
            dtUnits.Columns.Add("Title", typeof(string));

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
                dtItems.Rows.Add(item.Id, item.Title, item.ItemCategoryCode, item.ItemKey, item.PriRate, item.SecRate, item.PrimaryUnit, item.SecondaryUnit, item.DefaultUnit);

            dtUnits.Rows.Clear();
            foreach (var u in unitsTask.Result)
                dtUnits.Rows.Add(u.Code, u.Title);

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
            clnCatagory.DataSource = dtCategories.Copy();
            clnCatagory.DisplayMember = "Title";
            clnCatagory.ValueMember = "Code";
        }

        private async System.Threading.Tasks.Task FillQueryAsync(string fromDate = "", string toDate = "", string account = "", string voucherNo = "")
        {
            dgvQuery.Rows.Clear();
            _queryList = await _apiService.GetListAsync(fromDate, toDate, account, voucherNo);

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

                for (int i = 0; i < lines.Count; i++)
                {
                    var l = lines[i];
                    int ind = dgvSale.Rows.Add();
                    var row = dgvSale.Rows[ind];
                    row.Cells[clnSeq.Index].Value = l.Seq.ToString();
                    row.Cells[clnItemKey.Index].Value = l.ItemKey;
                    row.Cells[clnCatagory.Index].Value = l.ItemCategoryCode;
                    row.Cells[clnItemNo.Index].Value = null;
                    row.Cells[clnUnit.Index].Value = null;
                    row.Cells[clnQty.Index].Value = l.Qty.ToString("0.##");
                    row.Cells[clnRate.Index].Value = l.Rate.ToString("0.##");
                    row.Cells[clnAddless.Index].Value = l.AddLess.ToString("0.##");
                    row.Cells[clnAmount.Index].Value = l.Amount.ToString("N2");
                    row.Cells[clnStatus.Index].Value = "0";


                    SetcmbItemSource(l.ItemCategoryCode, ind);
                    dgvSale[clnItemNo.Index, ind].Value = l.ItemId;

                    DataRow itemRow = dtItems.Select("Id = '" + l.ItemId.Replace("'", "''") + "'").FirstOrDefault();
                    if (itemRow != null)
                    {
                        DataRowView drv = dtItems.DefaultView[dtItems.Rows.IndexOf(itemRow)];
                        SetcmbUnitSource(drv, ind);
                    }
                    else
                    {
                        SetAllUnitsSource(ind);
                    }

                    dgvSale[clnUnit.Index, ind].Value = l.Unit;
                    dgvSale[clnRate.Index, ind].Value = l.Rate.ToString("0.##");

                    if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                    {
                        dgvSale["clnSecQty", ind].Value = (l.SecQty ?? 0).ToString("0.##");
                        dgvSale["clnSecRate", ind].Value = (l.SecRate ?? 0).ToString("0.##");
                    }
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
            if (ApiSession.HasSecondaryQty)
            {
                clnUnit.Visible = false;
                clnQty.HeaderText = "Single Qty";
                clnRate.HeaderText = "Single Rate";

                if (!dgvSale.Columns.Contains("clnSecQty"))
                {
                    var colSecQty = new DataGridViewTextBoxColumn
                    {
                        Name = "clnSecQty",
                        HeaderText = "Pack Qty",
                        Width = 80
                    };
                    var colSecRate = new DataGridViewTextBoxColumn
                    {
                        Name = "clnSecRate",
                        HeaderText = "Pack Rate",
                        Width = 80
                    };
                    int insertIndex = clnAddless.Index;
                    dgvSale.Columns.Insert(insertIndex, colSecQty);
                    dgvSale.Columns.Insert(insertIndex + 1, colSecRate);
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
                if (_queryList.Count > 0 && txtVoucherNo.Text == "")
                    await FillPurchaseAsync(_queryList[VoucherIndex].VoucherNo);
                FLogIn = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            catch { }

            if (dgvSale.Rows[e.RowIndex].Cells[clnSeq.Index].Value == null)
            {
                dgvSale.Rows[e.RowIndex].Cells[clnSeq.Index].Value = (seq + 1).ToString();
                dgvSale.Rows[e.RowIndex].Cells[clnStatus.Index].Value = "0";
            }
        }

        private void frmPurchase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !dgvSale.Focused)
                SendKeys.Send("{tab}");
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
                    cmbunit.SelectedIndexChanged += new EventHandler(cmbunit_SelectedIndexChanged);
            }
            else if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index || dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecRate")
            {
                TextBox tbRate = e.Control as TextBox;
                if (tbRate != null && e.Control.Text != null)
                    tbRate.KeyPress += new KeyPressEventHandler(tbRate_KeyPress);
            }
            else if (dgvSale.CurrentCell.ColumnIndex == clnAddless.Index)
            {
                TextBox tbAddless = e.Control as TextBox;
                if (tbAddless != null && e.Control.Text != null)
                    tbAddless.KeyPress += new KeyPressEventHandler(tbAddless_KeyPress);
            }
            else if (dgvSale.CurrentCell.ColumnIndex == clnQty.Index || dgvSale.Columns[dgvSale.CurrentCell.ColumnIndex].Name == "clnSecQty")
            {
                TextBox tbQty = e.Control as TextBox;
                if (tbQty != null && e.Control.Text != null)
                    tbQty.KeyPress += new KeyPressEventHandler(tbQty_KeyPress);
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

        private void cmbunit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvSale.CurrentCellAddress.X == clnUnit.DisplayIndex && (sender as ComboBox).SelectedIndex != -1)
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

        private void cmbCatagory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvSale.CurrentCellAddress.X == clnCatagory.DisplayIndex && (sender as ComboBox).SelectedIndex != -1)
            {
                DataRowView dr = (DataRowView)(sender as ComboBox).SelectedItem;
                if ((sender as ComboBox).SelectedValue == null)
                    (sender as ComboBox).SelectedValue = dr[0].ToString();
                SetcmbItemSource(dr[0].ToString(), dgvSale.CurrentCellAddress.Y);
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
            if (dgvSale.CurrentCellAddress.X == clnItemNo.DisplayIndex && (sender as ComboBox).SelectedIndex != -1)
            {
                DataRowView dr = (DataRowView)(sender as ComboBox).SelectedItem;
                if ((sender as ComboBox).SelectedValue == null)
                    (sender as ComboBox).SelectedValue = dr[0].ToString();
                if (dr != null)
                    SetcmbUnitSource(dr, dgvSale.CurrentCellAddress.Y);
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
                dgvSale[clnRate.Index, rowInd].Value = dr["DefaultUnit"].ToString() == dr["SecondaryUnit"].ToString()
                    ? dr["SecRate"].ToString()
                    : dr["PriRate"].ToString();
            }

            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
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

            decimal qty = ParseDecimal(dgvSale[clnQty.Index, e.RowIndex].Value);
            decimal rate = ParseDecimal(dgvSale[clnRate.Index, e.RowIndex].Value);
            decimal addLess = ParseDecimal(dgvSale[clnAddless.Index, e.RowIndex].Value);

            decimal secQty = 0;
            decimal secRate = 0;
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                secQty = ParseDecimal(dgvSale["clnSecQty", e.RowIndex].Value);
                secRate = ParseDecimal(dgvSale["clnSecRate", e.RowIndex].Value);
            }

            dgvSale[clnQty.Index, e.RowIndex].Value = qty.ToString();
            dgvSale[clnRate.Index, e.RowIndex].Value = rate.ToString();
            dgvSale[clnAddless.Index, e.RowIndex].Value = addLess.ToString();
            if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
            {
                dgvSale["clnSecQty", e.RowIndex].Value = secQty.ToString();
                dgvSale["clnSecRate", e.RowIndex].Value = secRate.ToString();
            }

            dgvSale[clnAmount.Index, e.RowIndex].Value = decimal.Round((qty * rate) + addLess + (secQty * secRate), 2).ToString();
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
                if (ApiSession.HasSecondaryQty && dgvSale.Columns.Contains("clnSecQty"))
                {
                    secQty = ParseDecimal(row.Cells["clnSecQty"].Value);
                    secRate = ParseDecimal(row.Cells["clnSecRate"].Value);
                    
                    string itemId = Convert.ToString(row.Cells[clnItemNo.Index].Value);
                    DataRow itemRow = dtItems.Select("Id = '" + itemId.Replace("'", "''") + "'").FirstOrDefault();
                    if (itemRow != null)
                    {
                        secUnit = Convert.ToString(itemRow["SecondaryUnit"]);
                    }
                }

                lines.Add(new PurchaseLineRequest
                {
                    Seq = int.Parse(Convert.ToString(row.Cells[clnSeq.Index].Value)),
                    ItemId = Convert.ToString(row.Cells[clnItemNo.Index].Value),
                    Unit = ApiSession.HasSecondaryQty ? null : Convert.ToString(row.Cells[clnUnit.Index].Value),
                    Qty = ParseDecimal(row.Cells[clnQty.Index].Value),
                    Rate = ParseDecimal(row.Cells[clnRate.Index].Value),
                    AddLess = ParseDecimal(row.Cells[clnAddless.Index].Value),
                    SecQty = secQty,
                    SecRate = secRate,
                    SecUnit = secUnit
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

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
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
