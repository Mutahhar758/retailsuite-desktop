using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmSaleSupply : Form
    {
        private readonly SaleSupplyApiService _apiService;
        private readonly SupplyOrderApiService _supplyOrderApiService;
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

        enum Navigators { Up, Down, Home, End };

        public frmSaleSupply()
        {
            InitializeComponent();
            _apiService = new SaleSupplyApiService();
            _supplyOrderApiService = new SupplyOrderApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _narrationApiService = new NarrationApiService();
            _unitApiService = new UnitApiService();
            _inventoryApiService = new InventoryApiService();
            InitializeLookupTables();
            dgvSale.Rows.Add();
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

            dtUnits.Columns.Add("Code", typeof(string));
            dtUnits.Columns.Add("Title", typeof(string));

            dtCustomers.Columns.Add("Account", typeof(string));
            dtCustomers.Columns.Add("Title", typeof(string));

            dtNarration.Columns.Add("Code", typeof(string));
            dtNarration.Columns.Add("Title", typeof(string));

            dtSupplyOrders.Columns.Add("Id", typeof(int));
            dtSupplyOrders.Columns.Add("Title", typeof(string));
        }

        private async System.Threading.Tasks.Task LoadLookupsAsync()
        {
            var customersTask = _chartOfAccountApiService.GetCustomerAccountsAsync();
            var narrationsTask = _narrationApiService.GetActiveNarrationsAsync();
            var unitsTask = _unitApiService.GetActiveAsync();
            var itemsTask = _inventoryApiService.GetItemsAsync(null);
            var supplyOrdersTask = _supplyOrderApiService.GetAsync();
            await System.Threading.Tasks.Task.WhenAll(customersTask, narrationsTask, unitsTask, itemsTask, supplyOrdersTask);

            dtItems.Rows.Clear();
            foreach (var item in itemsTask.Result)
                dtItems.Rows.Add(item.Id, item.Title, item.PriRate, item.SecRate, item.PrimaryUnit, item.SecondaryUnit, item.DefaultUnit);

            dtUnits.Rows.Clear();
            foreach (var u in unitsTask.Result)
                dtUnits.Rows.Add(u.Code, u.Title);

            dtCustomers.Rows.Clear();
            foreach (var c in customersTask.Result)
                dtCustomers.Rows.Add(c.Account, c.Title);

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

        void CalcTotAmount()
        {
            decimal TotAmount = 0;
            try
            {
                TotAmount = (from DataGridViewRow row in dgvSale.Rows
                             where row.Cells[clnAmount.Index].Value != null
                             select ParseDecimal(row.Cells[clnAmount.Index].Value)).Sum();
            }
            catch
            {
            }
            txtTotAmount.Text = TotAmount.ToString("N2");
        }

        private async System.Threading.Tasks.Task SaveAsync()
        {
            if (cmbItem.SelectedValue == null)
            {
                MessageBox.Show("Please Select Item...!");
                return;
            }

            var lines = new List<SaleSupplyLineApiRequest>();
            foreach (DataGridViewRow row in dgvSale.Rows)
            {
                if (row.Cells[clnCustomer.Index].Value == null)
                    continue;

                lines.Add(new SaleSupplyLineApiRequest
                {
                    Seq = int.Parse(Convert.ToString(row.Cells[clnSeq.Index].Value)),
                    CustomerId = Convert.ToString(row.Cells[clnCustomer.Index].Value),
                    Unit = Convert.ToString(row.Cells[clnUnit.Index].Value),
                    Qty = ParseDecimal(row.Cells[clnQty.Index].Value),
                    Rate = ParseDecimal(row.Cells[clnRate.Index].Value),
                    Discount = ParseDecimal(row.Cells[clnDiscount.Index].Value),
                    AddLess = ParseDecimal(row.Cells[clnAddLess.Index].Value)
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

            if (lines.Count > 0)
            {
                var first = lines[0];
                dtpDate.Value = first.Date;
                if (string.IsNullOrWhiteSpace(first.Narration))
                    cmbNarration.SelectedIndex = -1;
                else
                    cmbNarration.SelectedValue = first.Narration;
                cmbItem.SelectedValue = first.ItemId;
                txtDescription.Text = first.Description;
                txtCreatedBy.Text = first.CreatedBy + " | " + first.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt");
                txtEditBy.Text = !string.IsNullOrWhiteSpace(first.LastModifiedBy)
                    ? first.LastModifiedBy + " | " + first.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                    : null;

                for (int i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    decimal discountPercent = line.Rate == 0 ? 0 : (line.Discount / line.Rate) * 100;
                    dgvSale.Rows.Add(
                        line.Seq.ToString(),
                        line.CustomerId,
                        line.Unit,
                        line.Qty.ToString("0.##"),
                        line.Rate.ToString("0.##"),
                        line.Discount.ToString("0.##"),
                        discountPercent.ToString("0.##"),
                        line.AddLess.ToString("0.##"),
                        line.Amount.ToString("N2"),
                        "0");
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

        private async void frmPurchase_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadLookupsAsync();
                await FillQueryAsync();
                FLogIn = false;
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
                    if (dr != null && dr["PrimaryUnit"] != null)
                    {
                        dgvSale.Rows[e.RowIndex].Cells[clnUnit.Index].Value = dr["PrimaryUnit"].ToString();
                        SetRateForRow(e.RowIndex);
                    }
                }
            }
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
            else if (dgvSale.CurrentCellAddress.X == clnUnit.DisplayIndex)
            {
                ComboBox cmbunit = e.Control as ComboBox;
                if (cmbunit != null)
                {
                    cmbunit.SelectedIndexChanged += new EventHandler(cmbunit_SelectedIndexChanged);
                }
            }
            else if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQty.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscount.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscPercent.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnAddLess.Index)
            {
                TextBox tbRate = e.Control as TextBox;
                if (tbRate != null && e.Control.Text != null)
                {
                    tbRate.KeyPress += new KeyPressEventHandler(tbRate_KeyPress);
                }
            }
        }

        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvSale.CurrentCellAddress.X == clnCustomer.DisplayIndex)
            {
                if ((sender as ComboBox).SelectedIndex != -1)
                {
                    dgvSale[clnCustomer.Index, dgvSale.CurrentCellAddress.Y].Value = (sender as ComboBox).SelectedValue;
                }
            }
        }

        void cmbunit_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (dgvSale.CurrentCellAddress.X == clnUnit.DisplayIndex)
            {
                if ((sender as ComboBox).SelectedIndex != -1)
                {
                    SetRateForRow(dgvSale.CurrentCellAddress.Y);
                }

            }
        }

        void SetRateForRow(int rowIndex)
        {
            if (cmbItem.SelectedValue != null && rowIndex >= 0 && rowIndex < dgvSale.Rows.Count)
            {
                if (dgvSale.Rows[rowIndex].Cells[clnUnit.Index].Value != null)
                {
                    string Filter = string.Format("Id = '{0}'", cmbItem.SelectedValue.ToString());
                    DataRow dr = dtItems.Select(Filter).FirstOrDefault();
                    if (dr != null)
                    {
                        string unitValue = dgvSale.Rows[rowIndex].Cells[clnUnit.Index].Value.ToString();
                        string rate = unitValue == dr["SecondaryUnit"].ToString() ? dr["SecRate"].ToString() : dr["PriRate"].ToString();
                        dgvSale.Rows[rowIndex].Cells[clnRate.Index].Value = rate;
                    }
                }
            }
        }

        void tbRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvSale.CurrentCell.ColumnIndex == clnRate.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnQty.Index ||
                dgvSale.CurrentCell.ColumnIndex == clnDiscount.Index ||
                 dgvSale.CurrentCell.ColumnIndex == clnDiscPercent.Index ||
                 dgvSale.CurrentCell.ColumnIndex == clnAddLess.Index)
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
            DataTable dt = dtUnits.Copy();
            dt.Rows.Clear();
            if (cmbItem.SelectedValue != null)
            {
                DataRowView dr = (DataRowView)cmbItem.SelectedItem;
                dt = dtUnits.Select("Code in ('" + dr["PrimaryUnit"].ToString() + "','" + dr["SecondaryUnit"].ToString() + "')").Count() == 0 ? dt :
                    dtUnits.Select("Code in ('" + dr["PrimaryUnit"].ToString() + "','" + dr["SecondaryUnit"].ToString() + "')").CopyToDataTable();
            }
            clnUnit.DataSource = dt;
            clnUnit.DisplayMember = "Title";
            clnUnit.ValueMember = "Code";
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
                    bool unitFilled = dgvSale.Rows[rowIndex].Cells[clnUnit.Index].Value != null &&
                                     !string.IsNullOrEmpty(dgvSale.Rows[rowIndex].Cells[clnUnit.Index].Value.ToString());

                    if (unitFilled)
                    {
                        dgvSale.CurrentCell = dgvSale.Rows[rowIndex].Cells[clnQty.Index];
                    }
                    else
                    {
                        SendKeys.Send("{tab}");
                    }
                }
                else if (dgvSale.CurrentCell != null && dgvSale.CurrentCell.ColumnIndex == clnQty.Index)
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

            decimal Qty = ParseDecimal(dgvSale[clnQty.Index, e.RowIndex].Value);
            decimal Rate = ParseDecimal(dgvSale[clnRate.Index, e.RowIndex].Value);
            decimal Discount = ParseDecimal(dgvSale[clnDiscount.Index, e.RowIndex].Value);
            decimal DiscountPercent = ParseDecimal(dgvSale[clnDiscPercent.Index, e.RowIndex].Value);
            decimal AddLess = ParseDecimal(dgvSale[clnAddLess.Index, e.RowIndex].Value);

            dgvSale[clnQty.Index, e.RowIndex].Value = Qty.ToString();
            dgvSale[clnRate.Index, e.RowIndex].Value = Rate.ToString();
            dgvSale[clnDiscount.Index, e.RowIndex].Value = Discount.ToString();
            dgvSale[clnDiscPercent.Index, e.RowIndex].Value = DiscountPercent.ToString();
            dgvSale[clnAddLess.Index, e.RowIndex].Value = AddLess.ToString();

            if (!clnDiscount.Visible)
                dgvSale[clnDiscount.Index, e.RowIndex].Value = (Rate * (DiscountPercent / 100)).ToString();
            else
                dgvSale[clnDiscPercent.Index, e.RowIndex].Value = Rate == 0 ? "0" : ((Discount / Rate) * 100).ToString();

            Rate = Rate - (clnDiscPercent.Visible ? Rate * (DiscountPercent / 100) : Discount);
            dgvSale[clnAmount.Index, e.RowIndex].Value = decimal.Round((Qty * Rate) + AddLess, 2).ToString();

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
            Validation.Clear(this);
            txtTotAmount.Text = "0";
            txtVoucherNo.Text = "";
            VoucherNum = null;
            txtCreatedBy.Text = "";
            txtEditBy.Text = "";
            txtDescription.Text = "";
            dgvSale.Rows.Clear();
            dgvSale.Rows.Add();
            dtpDate.Focus();
            cmbItem.SelectedIndex = -1;
            cmbNarration.SelectedIndex = dtNarration.Rows.Count > 0 ? 0 : -1;
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

        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetclnUnitSource();

            if (cmbItem.SelectedValue != null)
            {
                DataRowView dr = cmbItem.SelectedItem as DataRowView;
                if (dr != null && dr["PrimaryUnit"] != null)
                {
                    string primaryUnit = dr["PrimaryUnit"].ToString();

                    foreach (DataGridViewRow row in dgvSale.Rows)
                    {
                        row.Cells[clnUnit.Index].Value = primaryUnit;
                        SetRateForRow(row.Index);
                    }
                }
            }
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
                    var supplyOrder = await _supplyOrderApiService.GetByIdAsync(supplyOrderId);

                    if (supplyOrder != null && supplyOrder.Details != null)
                    {
                        for (int i = 0; i < supplyOrder.Details.Count; i++)
                        {
                            int idx = dgvSale.Rows.Add();
                            dgvSale.Rows[idx].Cells[clnSeq.Index].Value = (idx + 1).ToString();
                            dgvSale.Rows[idx].Cells[clnCustomer.Index].Value = supplyOrder.Details[i].CustomerId;

                            if (cmbItem.SelectedValue != null)
                            {
                                DataRowView itemDr = cmbItem.SelectedItem as DataRowView;
                                if (itemDr != null && itemDr["PrimaryUnit"] != null)
                                {
                                    dgvSale.Rows[idx].Cells[clnUnit.Index].Value = itemDr["PrimaryUnit"].ToString();
                                    SetRateForRow(idx);
                                }
                            }
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
    }
}
