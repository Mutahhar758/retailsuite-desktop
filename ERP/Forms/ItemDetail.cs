using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmItemDetail : Form
    {
        private DataTable dtUnits = new DataTable();
        private DataTable dtCatagory = new DataTable();
        private DataTable dtItemDetail = new DataTable();
        private readonly InventoryApiService _inventoryApiService;
        private readonly ItemCategoryApiService _itemCategoryApiService;
        private readonly UnitApiService _unitApiService;
        private readonly BrandApiService _brandApiService;

        private string _currentEditingItemId = null;
        private string _currentMediaId = string.Empty;
        private string _currentMediaUrl = string.Empty;
        private string _currentAdditionalUnits = null;
        private List<ItemCustomerCategoryPriceUpsertRequest> _pendingCategoryRates = null;

        private bool FLogin = true;
        private bool _isPopulating = false;

        public frmItemDetail()
        {
            InitializeComponent();
            _inventoryApiService = new InventoryApiService();
            _itemCategoryApiService = new ItemCategoryApiService();
            _unitApiService = new UnitApiService();
            _brandApiService = new BrandApiService();
            UserInfo.ApplyFormPermissions(this, AppResource.InventoryItems);
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
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetupButtonFocusHighlights()
        {
            var buttons = new Button[] { btnSave, btnNew, btnDelete, btnClose, btnQueryFind, btnQueryReset, btnAddNewFromSearch, btnCategoryRates, btnSubUnits };
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
                        using (var pen = new Pen(Color.FromArgb(37, 99, 235), 2))
                        {
                            e.Graphics.DrawRectangle(pen, 1, 1, rect.Width - 2, rect.Height - 2);
                        }
                        if (btn == btnSave)
                        {
                            using (var penInner = new Pen(Color.FromArgb(180, 255, 255, 255), 1))
                            {
                                e.Graphics.DrawRectangle(penInner, 2, 2, rect.Width - 4, rect.Height - 4);
                            }
                        }
                    }
                };
            }
        }

        private void ApplyModernStyling()
        {
            SetupButtonFocusHighlights();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            // Query Grid styling
            dgvQuery.EnableHeadersVisualStyles = false;
            dgvQuery.BackgroundColor = Color.White;
            dgvQuery.BorderStyle = BorderStyle.FixedSingle;
            dgvQuery.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvQuery.GridColor = Color.FromArgb(226, 232, 240);
            dgvQuery.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            dgvQuery.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            dgvQuery.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(241, 245, 249);
            dgvQuery.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
            dgvQuery.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvQuery.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvQuery.ColumnHeadersHeight = 32;

            dgvQuery.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvQuery.DefaultCellStyle.BackColor = Color.White;
            dgvQuery.DefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            dgvQuery.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvQuery.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvQuery.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvQuery.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            dgvQuery.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvQuery.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvQuery.RowTemplate.Height = 28;

            // 1px bottom border on header
            pnlHeader.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
                }
            };

            // Button hover animations
            btnSave.MouseEnter += (s, e) => { if (btnSave.Text.Contains("*")) btnSave.BackColor = Color.FromArgb(194, 65, 12); else btnSave.BackColor = Color.FromArgb(21, 128, 61); };
            btnSave.MouseLeave += (s, e) => { if (btnSave.Text.Contains("*")) btnSave.BackColor = Color.FromArgb(234, 88, 12); else btnSave.BackColor = Color.FromArgb(22, 163, 74); };
            btnNew.MouseEnter += (s, e) => btnNew.BackColor = Color.FromArgb(226, 232, 240);
            btnNew.MouseLeave += (s, e) => btnNew.BackColor = Color.FromArgb(241, 245, 249);
            btnDelete.MouseEnter += (s, e) => btnDelete.BackColor = Color.FromArgb(254, 202, 202);
            btnDelete.MouseLeave += (s, e) => btnDelete.BackColor = Color.FromArgb(254, 242, 242);
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(226, 232, 240);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.FromArgb(241, 245, 249);
            btnQueryFind.MouseEnter += (s, e) => btnQueryFind.BackColor = Color.FromArgb(29, 78, 216);
            btnQueryFind.MouseLeave += (s, e) => btnQueryFind.BackColor = Color.FromArgb(37, 99, 235);
            btnQueryReset.MouseEnter += (s, e) => btnQueryReset.BackColor = Color.FromArgb(226, 232, 240);
            btnQueryReset.MouseLeave += (s, e) => btnQueryReset.BackColor = Color.FromArgb(241, 245, 249);
            btnAddNewFromSearch.MouseEnter += (s, e) => btnAddNewFromSearch.BackColor = Color.FromArgb(21, 128, 61);
            btnAddNewFromSearch.MouseLeave += (s, e) => btnAddNewFromSearch.BackColor = Color.FromArgb(22, 163, 74);
        }

        private async System.Threading.Tasks.Task fillItemCatagory()
        {
            var categories = await _itemCategoryApiService.GetLookupAsync();

            dtCatagory = new DataTable();
            dtCatagory.Columns.Add("Code", typeof(string));
            dtCatagory.Columns.Add("Title", typeof(string));

            for (int i = 0; i < categories.Count; i++)
            {
                dtCatagory.Rows.Add(categories[i].Code, categories[i].Title);
            }

            cmbItemCatagory.DataSource = dtCatagory.Copy();
            cmbItemCatagory.DisplayMember = "Title";
            cmbItemCatagory.ValueMember = "Code";
        }

        private async System.Threading.Tasks.Task LoadUnitsAsync()
        {
            var units = await _unitApiService.GetLookupAsync();
            dtUnits = UnitConversionHelper.CreateUnitsTable(units);
        }

        private void FillUnits()
        {
            cmbPriUnit.DataSource = dtUnits.Copy();
            cmbPriUnit.DisplayMember = "Title";
            cmbPriUnit.ValueMember = "Code";

            cmbSecUnit.DataSource = dtUnits.Copy();
            cmbSecUnit.DisplayMember = "Title";
            cmbSecUnit.ValueMember = "Code";

            cmbDefUnit.DataSource = dtUnits.Copy();
            cmbDefUnit.DisplayMember = "Title";
            cmbDefUnit.ValueMember = "Code";
        }

        private async System.Threading.Tasks.Task LoadItemDetailAsync()
        {
            var items = await _inventoryApiService.GetItemsAsync(null);

            dtItemDetail = new DataTable();
            dtItemDetail.Columns.Add("Id", typeof(string));
            dtItemDetail.Columns.Add("Barcode", typeof(string));
            dtItemDetail.Columns.Add("fkitemcatagory", typeof(string));
            dtItemDetail.Columns.Add("Title", typeof(string));
            dtItemDetail.Columns.Add("ItemKey", typeof(string));
            dtItemDetail.Columns.Add("PriRate", typeof(decimal));
            dtItemDetail.Columns.Add("SecRate", typeof(decimal));
            dtItemDetail.Columns.Add("PrimaryUnit", typeof(string));
            dtItemDetail.Columns.Add("SecondaryUnit", typeof(string));
            dtItemDetail.Columns.Add("DefaultUnit", typeof(string));
            dtItemDetail.Columns.Add("QtyInPack", typeof(decimal));
            dtItemDetail.Columns.Add("Alert", typeof(string));
            dtItemDetail.Columns.Add("LowStockAlert", typeof(bool));
            dtItemDetail.Columns.Add("LowStockQty", typeof(decimal));
            dtItemDetail.Columns.Add("MinSalePrice", typeof(decimal));
            dtItemDetail.Columns.Add("OpnStock", typeof(decimal));
            dtItemDetail.Columns.Add("OpnRate", typeof(decimal));
            dtItemDetail.Columns.Add("status", typeof(string));
            dtItemDetail.Columns.Add("MediaId", typeof(string));
            dtItemDetail.Columns.Add("MediaUrl", typeof(string));
            dtItemDetail.Columns.Add("ItemType", typeof(int));
            dtItemDetail.Columns.Add("RequireImei", typeof(bool));
            dtItemDetail.Columns.Add("BrandId", typeof(string));
            dtItemDetail.Columns.Add("ModelName", typeof(string));
            dtItemDetail.Columns.Add("Storage", typeof(string));
            dtItemDetail.Columns.Add("Ram", typeof(string));
            dtItemDetail.Columns.Add("Color", typeof(string));
            dtItemDetail.Columns.Add("AdditionalUnits", typeof(string));

            for (int i = 0; i < items.Count; i++)
            {
                dtItemDetail.Rows.Add(
                    items[i].Id,
                    items[i].Barcode,
                    items[i].ItemCategoryCode,
                    items[i].Title,
                    items[i].ItemKey,
                    items[i].PriRate,
                    items[i].SecRate,
                    items[i].PrimaryUnit,
                    items[i].SecondaryUnit,
                    items[i].DefaultUnit,
                    items[i].QtyInPack.HasValue ? (object)items[i].QtyInPack.Value : DBNull.Value,
                    items[i].Alert ? "1" : "0",
                    items[i].LowStockAlert.HasValue ? (object)items[i].LowStockAlert.Value : DBNull.Value,
                    items[i].LowStockQty.HasValue ? (object)items[i].LowStockQty.Value : DBNull.Value,
                    items[i].MinSalePrice.HasValue ? (object)items[i].MinSalePrice.Value : DBNull.Value,
                    items[i].OpnStock.HasValue ? (object)items[i].OpnStock.Value : DBNull.Value,
                    items[i].OpnRate.HasValue ? (object)items[i].OpnRate.Value : DBNull.Value,
                    "0",
                    items[i].MediaId,
                    items[i].MediaUrl,
                    items[i].ItemType == "Service" ? 1 : 0,
                    items[i].RequireImei == true,
                    items[i].BrandId,
                    items[i].ModelName,
                    items[i].Storage,
                    items[i].Ram,
                    items[i].Color,
                    items[i].AdditionalUnits);
            }
        }

        private async void frmItemDetail_Load(object sender, EventArgs e)
        {
            ApplyModernStyling();

            profileImage1.Title = "Product Image";
            profileImage1.SizeMode = PictureBoxSizeMode.Zoom;
            profileImage1.PictureBox.BackColor = Color.White;
            profileImage1.DefaultImage = ERP.Controls.ProfileImage.CreateProductPlaceholder(profileImage1.PictureBox.Width, profileImage1.PictureBox.Height);
            profileImage1.ClearImage();

            profileImage1.SearchButton.Click += btnProductSearch_Click;
            profileImage1.CancelButton.Click += btnProductCancel_Click;

            DataTable dtItemTypes = new DataTable();
            dtItemTypes.Columns.Add("Value", typeof(int));
            dtItemTypes.Columns.Add("Name", typeof(string));
            dtItemTypes.Rows.Add(0, "Product");
            dtItemTypes.Rows.Add(1, "Service");

            cmbItemType.DataSource = dtItemTypes;
            cmbItemType.DisplayMember = "Name";
            cmbItemType.ValueMember = "Value";

            await LoadUnitsAsync();
            await fillItemCatagory();
            await LoadItemDetailAsync();
            FillUnits();

            if (ApiSession.HasMobileShopFeature)
            {
                pnlMobileShop.Visible = true;
                var brands = await _brandApiService.GetLookupAsync();
                var dtBrand = new DataTable();
                dtBrand.Columns.Add("Id", typeof(string));
                dtBrand.Columns.Add("Title", typeof(string));
                foreach (var b in brands)
                    dtBrand.Rows.Add(b.Id, b.Title);

                cmbBrand.DataSource = dtBrand;
                cmbBrand.DisplayMember = "Title";
                cmbBrand.ValueMember = "Id";
            }
            else
            {
                pnlMobileShop.Visible = false;
            }

            InitQueryFilterControls();
            FillQueryGrid();

            if (dtItemDetail.Rows.Count > 0)
            {
                await LoadProductIntoDetailAsync(dtItemDetail.Rows[0]["Id"]?.ToString());
            }
            else
            {
                ClearDetailFields();
            }

            FLogin = false;
            if (!UserInfo.HasPermission(AppAction.Update, AppResource.OpeningBalances))
            {
                txtOpnStock.ReadOnly = true;
                txtOpnRate.ReadOnly = true;
            }
        }

        public async System.Threading.Tasks.Task LoadProductIntoDetailAsync(string itemId)
        {
            if (string.IsNullOrEmpty(itemId) || dtItemDetail == null) return;

            DataRow row = dtItemDetail.Select($"Id = '{itemId.Replace("'", "''")}'").FirstOrDefault();
            if (row == null) return;

            _isPopulating = true;
            try
            {
                _currentEditingItemId = itemId;
                txtBarcode.Text = row["Barcode"]?.ToString() ?? "";
                txtItemKey.Text = row["ItemKey"]?.ToString() ?? "";
                txtTitle.Text = row["Title"]?.ToString() ?? "";

                if (row["fkitemcatagory"] != DBNull.Value)
                    cmbItemCatagory.SelectedValue = row["fkitemcatagory"].ToString();

                int itemType = row["ItemType"] != DBNull.Value ? Convert.ToInt32(row["ItemType"]) : 0;
                cmbItemType.SelectedValue = itemType;

                txtPriRate.Text = row["PriRate"] != DBNull.Value ? Convert.ToDecimal(row["PriRate"]).ToString("0.##") : "0";
                txtSecRate.Text = row["SecRate"] != DBNull.Value ? Convert.ToDecimal(row["SecRate"]).ToString("0.##") : "0";
                txtMinSalePrice.Text = row["MinSalePrice"] != DBNull.Value ? Convert.ToDecimal(row["MinSalePrice"]).ToString("0.##") : "";

                if (row["PrimaryUnit"] != DBNull.Value)
                    cmbPriUnit.SelectedValue = row["PrimaryUnit"].ToString();
                if (row["SecondaryUnit"] != DBNull.Value)
                    cmbSecUnit.SelectedValue = row["SecondaryUnit"].ToString();
                if (row["DefaultUnit"] != DBNull.Value)
                    cmbDefUnit.SelectedValue = row["DefaultUnit"].ToString();

                txtQtyInPack.Text = row["QtyInPack"] != DBNull.Value ? Convert.ToDecimal(row["QtyInPack"]).ToString("0.##") : "1";

                txtOpnStock.Text = row["OpnStock"] != DBNull.Value ? Convert.ToDecimal(row["OpnStock"]).ToString("0.##") : "0";
                txtOpnRate.Text = row["OpnRate"] != DBNull.Value ? Convert.ToDecimal(row["OpnRate"]).ToString("0.##") : "0";

                chkLowStockAlert.Checked = row["LowStockAlert"] != DBNull.Value && Convert.ToBoolean(row["LowStockAlert"]);
                txtLowStockQty.Text = row["LowStockQty"] != DBNull.Value ? Convert.ToDecimal(row["LowStockQty"]).ToString("0.##") : "";
                chkAlert.Checked = row["Alert"]?.ToString() == "1" || string.Equals(row["Alert"]?.ToString(), "true", StringComparison.OrdinalIgnoreCase);

                if (ApiSession.HasMobileShopFeature)
                {
                    if (row.Table.Columns.Contains("RequireImei") && row["RequireImei"] != DBNull.Value)
                        chkRequireImei.Checked = Convert.ToBoolean(row["RequireImei"]);
                    if (row.Table.Columns.Contains("BrandId") && row["BrandId"] != DBNull.Value)
                        cmbBrand.SelectedValue = row["BrandId"].ToString();
                    if (row.Table.Columns.Contains("ModelName"))
                        txtModel.Text = row["ModelName"]?.ToString() ?? "";
                    if (row.Table.Columns.Contains("Storage"))
                        txtStorage.Text = row["Storage"]?.ToString() ?? "";
                    if (row.Table.Columns.Contains("Ram"))
                        txtRam.Text = row["Ram"]?.ToString() ?? "";
                    if (row.Table.Columns.Contains("Color"))
                        txtColor.Text = row["Color"]?.ToString() ?? "";
                }

                _currentMediaId = row["MediaId"]?.ToString() ?? string.Empty;
                _currentMediaUrl = row["MediaUrl"]?.ToString() ?? string.Empty;
                _currentAdditionalUnits = row["AdditionalUnits"]?.ToString();
                _pendingCategoryRates = null;

                profileImage1.MediaId = _currentMediaId;
                profileImage1.MediaUrl = _currentMediaUrl;
                if (!string.IsNullOrEmpty(_currentMediaUrl))
                    await profileImage1.LoadImageAsync(_currentMediaUrl);
                else
                    profileImage1.ClearImage();

                lblDetailStatus.Text = $"Editing: {txtTitle.Text} (ID: {itemId})";
                lblDetailStatus.ForeColor = Color.FromArgb(37, 99, 235);
                btnDelete.Enabled = true;
                UpdateSaveButtonState(false);
            }
            finally
            {
                _isPopulating = false;
            }
        }

        private void ClearDetailFields()
        {
            _isPopulating = true;
            try
            {
                _currentEditingItemId = null;
                _currentMediaId = string.Empty;
                _currentMediaUrl = string.Empty;
                _currentAdditionalUnits = null;
                _pendingCategoryRates = null;

                txtBarcode.Text = "";
                txtItemKey.Text = "";
                txtTitle.Text = "";

                if (cmbItemCatagory.Items.Count > 0 && cmbItemCatagory.SelectedIndex < 0)
                    cmbItemCatagory.SelectedIndex = 0;

                cmbItemType.SelectedIndex = 0;

                txtPriRate.Text = "0";
                txtSecRate.Text = "0";
                txtMinSalePrice.Text = "";

                if (cmbPriUnit.Items.Count > 0 && cmbPriUnit.SelectedIndex < 0)
                    cmbPriUnit.SelectedIndex = 0;
                if (cmbSecUnit.Items.Count > 0 && cmbSecUnit.SelectedIndex < 0)
                    cmbSecUnit.SelectedIndex = 0;
                if (cmbDefUnit.Items.Count > 0 && cmbDefUnit.SelectedIndex < 0)
                    cmbDefUnit.SelectedIndex = 0;

                txtQtyInPack.Text = "1";
                txtOpnStock.Text = "0";
                txtOpnRate.Text = "0";

                chkLowStockAlert.Checked = false;
                txtLowStockQty.Text = "";
                chkAlert.Checked = false;

                if (ApiSession.HasMobileShopFeature)
                {
                    chkRequireImei.Checked = false;
                    if (cmbBrand.Items.Count > 0) cmbBrand.SelectedIndex = -1;
                    txtModel.Text = "";
                    txtStorage.Text = "";
                    txtRam.Text = "";
                    txtColor.Text = "";
                }

                profileImage1.ClearImage();
                lblDetailStatus.Text = "New Product Mode";
                lblDetailStatus.ForeColor = Color.FromArgb(22, 163, 74);
                btnDelete.Enabled = false;
                UpdateSaveButtonState(false);
                txtBarcode.Focus();
            }
            finally
            {
                _isPopulating = false;
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbItemCatagory.SelectedValue == null || string.IsNullOrWhiteSpace(cmbItemCatagory.SelectedValue.ToString()))
            {
                MessageBox.Show("Please select a Category!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbItemCatagory.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter Product Name / Title!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            bool isService = cmbItemType.SelectedIndex == 1;

            var request = new InventoryItemUpsertApiRequest
            {
                Id = _currentEditingItemId,
                ItemCategoryCode = Convert.ToString(cmbItemCatagory.SelectedValue),
                Barcode = txtBarcode.Text?.Trim() ?? "",
                Title = txtTitle.Text?.Trim(),
                ItemKey = txtItemKey.Text?.Trim(),
                PriRate = ParseDecimal(txtPriRate.Text),
                SecRate = ParseDecimal(txtSecRate.Text),
                PrimaryUnit = isService ? string.Empty : Convert.ToString(cmbPriUnit.SelectedValue),
                SecondaryUnit = isService ? string.Empty : Convert.ToString(cmbSecUnit.SelectedValue),
                DefaultUnit = isService ? string.Empty : Convert.ToString(cmbDefUnit.SelectedValue),
                QtyInPack = isService ? null : ParseNullableDecimal(txtQtyInPack.Text),
                Alert = chkAlert.Checked,
                LowStockAlert = isService ? (bool?)null : chkLowStockAlert.Checked,
                LowStockQty = isService ? (decimal?)null : ParseNullableDecimal(txtLowStockQty.Text),
                MinSalePrice = ParseNullableDecimal(txtMinSalePrice.Text),
                OpnStock = isService ? 0 : ParseNullableDecimal(txtOpnStock.Text),
                OpnRate = isService ? 0 : ParseNullableDecimal(txtOpnRate.Text),
                ItemType = isService ? "Service" : "Product",
                MediaId = _currentMediaId ?? string.Empty,
                RequireImei = ApiSession.HasMobileShopFeature && chkRequireImei.Checked,
                BrandId = ApiSession.HasMobileShopFeature && cmbBrand.SelectedValue != null ? cmbBrand.SelectedValue.ToString() : null,
                ModelName = ApiSession.HasMobileShopFeature ? txtModel.Text : null,
                Storage = ApiSession.HasMobileShopFeature ? txtStorage.Text : null,
                Ram = ApiSession.HasMobileShopFeature ? txtRam.Text : null,
                Color = ApiSession.HasMobileShopFeature ? txtColor.Text : null,
                AdditionalUnits = isService ? null : _currentAdditionalUnits
            };

            try
            {
                if (string.IsNullOrEmpty(_currentEditingItemId) || _currentEditingItemId == "0")
                {
                    var newId = await _inventoryApiService.CreateItemAsync(request);
                    if (!string.IsNullOrWhiteSpace(newId))
                    {
                        _currentEditingItemId = newId;
                        if (_pendingCategoryRates != null && _pendingCategoryRates.Count > 0)
                        {
                            try { await _inventoryApiService.SaveItemCategoryPricesAsync(newId, _pendingCategoryRates); } catch { }
                        }
                    }
                }
                else
                {
                    await _inventoryApiService.UpdateItemAsync(_currentEditingItemId, request);
                }

                MessageBox.Show("Record Successfully Saved..!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadItemDetailAsync();
                FillQueryGrid();
                lblDetailStatus.Text = $"Saved: {txtTitle.Text} (ID: {_currentEditingItemId})";
                lblDetailStatus.ForeColor = Color.FromArgb(22, 163, 74);
                btnDelete.Enabled = true;
                UpdateSaveButtonState(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentEditingItemId) || _currentEditingItemId == "0") return;

            if (MessageBox.Show($"Are you sure you want to delete '{txtTitle.Text}'?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    await _inventoryApiService.DeleteItemAsync(_currentEditingItemId);
                    MessageBox.Show("Record Deleted Successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadItemDetailAsync();
                    FillQueryGrid();
                    ClearDetailFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to delete product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            tbItemMain.SelectedTab = tbDetail;
            ClearDetailFields();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitQueryFilterControls()
        {
            DataTable dtQCat = new DataTable();
            dtQCat.Columns.Add("Code", typeof(string));
            dtQCat.Columns.Add("Title", typeof(string));
            dtQCat.Rows.Add("ALL", "[All Categories]");
            if (dtCatagory != null)
            {
                foreach (DataRow r in dtCatagory.Rows)
                {
                    dtQCat.Rows.Add(r["Code"], r["Title"]);
                }
            }
            cmbQueryCategory.DataSource = dtQCat;
            cmbQueryCategory.DisplayMember = "Title";
            cmbQueryCategory.ValueMember = "Code";
            cmbQueryCategory.SelectedIndex = 0;

            cmbQueryType.Items.Clear();
            cmbQueryType.Items.Add("[All Types]");
            cmbQueryType.Items.Add("Product");
            cmbQueryType.Items.Add("Service");
            cmbQueryType.SelectedIndex = 0;
        }

        private void FillQueryGrid()
        {
            if (dtItemDetail == null) return;

            string searchText = txtQuerySearch?.Text?.Trim()?.ToLower() ?? "";
            string catFilter = cmbQueryCategory?.SelectedValue?.ToString() ?? "";
            int typeFilter = cmbQueryType?.SelectedIndex ?? 0;

            dgvQuery.Rows.Clear();

            var catMap = new Dictionary<string, string>();
            if (dtCatagory != null)
            {
                foreach (DataRow dr in dtCatagory.Rows)
                {
                    string code = dr["Code"]?.ToString();
                    string title = dr["Title"]?.ToString();
                    if (!string.IsNullOrEmpty(code))
                        catMap[code] = title;
                }
            }

            var unitMap = new Dictionary<string, string>();
            if (dtUnits != null)
            {
                foreach (DataRow dr in dtUnits.Rows)
                {
                    string code = dr["Code"]?.ToString();
                    string title = dr["Title"]?.ToString();
                    if (!string.IsNullOrEmpty(code))
                        unitMap[code] = title;
                }
            }

            int count = 0;
            foreach (DataRow row in dtItemDetail.Rows)
            {
                string id = row["Id"]?.ToString();
                string barcode = row["Barcode"]?.ToString() ?? "";
                string catCode = row["fkitemcatagory"]?.ToString() ?? "";
                string title = row["Title"]?.ToString() ?? "";
                string key = row["ItemKey"]?.ToString() ?? "";
                int itemType = row["ItemType"] != DBNull.Value ? Convert.ToInt32(row["ItemType"]) : 0;
                decimal priRate = row["PriRate"] != DBNull.Value ? Convert.ToDecimal(row["PriRate"]) : 0;
                decimal secRate = row["SecRate"] != DBNull.Value ? Convert.ToDecimal(row["SecRate"]) : 0;
                string priUnit = row["PrimaryUnit"]?.ToString() ?? "";
                string secUnit = row["SecondaryUnit"]?.ToString() ?? "";
                decimal? minPrice = row["MinSalePrice"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["MinSalePrice"]) : null;
                decimal? opnStock = row["OpnStock"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["OpnStock"]) : null;

                if (!string.IsNullOrEmpty(catFilter) && catFilter != "ALL" && catCode != catFilter)
                    continue;

                if (typeFilter == 1 && itemType != 0)
                    continue;
                if (typeFilter == 2 && itemType != 1)
                    continue;

                if (!string.IsNullOrEmpty(searchText))
                {
                    bool match = title.ToLower().Contains(searchText) ||
                                 barcode.ToLower().Contains(searchText) ||
                                 key.ToLower().Contains(searchText);
                    if (!match) continue;
                }

                string catName = catMap.TryGetValue(catCode, out var cName) ? cName : catCode;
                string priUnitName = unitMap.TryGetValue(priUnit, out var puName) ? puName : priUnit;
                string secUnitName = unitMap.TryGetValue(secUnit, out var suName) ? suName : secUnit;
                string typeName = itemType == 1 ? "Service" : "Product";

                int r = dgvQuery.Rows.Add(
                    id,
                    barcode,
                    catName,
                    title,
                    key,
                    typeName,
                    priRate.ToString("N2"),
                    secRate.ToString("N2"),
                    priUnitName,
                    secUnitName,
                    minPrice.HasValue ? minPrice.Value.ToString("N2") : "",
                    opnStock.HasValue ? opnStock.Value.ToString("N2") : ""
                );
                dgvQuery.Rows[r].Tag = id;
                count++;
            }

            if (lblQueryCount != null)
                lblQueryCount.Text = $"Found: {count} products";
            if (lblTotalProductsBadge != null)
                lblTotalProductsBadge.Text = $"Total Products: {dtItemDetail.Rows.Count}";
        }

        private async void dgvQuery_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvQuery.Rows.Count) return;

            string id = dgvQuery.Rows[e.RowIndex].Cells[clnQId.Index].Value?.ToString();
            if (string.IsNullOrEmpty(id)) return;

            await LoadProductIntoDetailAsync(id);
            tbItemMain.SelectedTab = tbDetail;
            txtTitle.Focus();
            txtTitle.SelectAll();
        }

        private void dgvQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dgvQuery.CurrentRow != null)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                dgvQuery_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, dgvQuery.CurrentRow.Index));
            }
        }

        private void txtQuerySearch_TextChanged(object sender, EventArgs e)
        {
            FillQueryGrid();
        }

        private void txtQuerySearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (dgvQuery.Rows.Count > 0)
                {
                    dgvQuery.Focus();
                }
            }
        }

        private void cmbQueryCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!FLogin) FillQueryGrid();
        }

        private void cmbQueryType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!FLogin) FillQueryGrid();
        }

        private void btnQueryFind_Click(object sender, EventArgs e)
        {
            FillQueryGrid();
        }

        private void btnQueryReset_Click(object sender, EventArgs e)
        {
            txtQuerySearch.Text = "";
            if (cmbQueryCategory.Items.Count > 0) cmbQueryCategory.SelectedIndex = 0;
            if (cmbQueryType.Items.Count > 0) cmbQueryType.SelectedIndex = 0;
            FillQueryGrid();
        }

        private async void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                string scanned = txtBarcode.Text.Trim();

                var matchedRow = dtItemDetail.AsEnumerable().FirstOrDefault(r => string.Equals(r["Barcode"]?.ToString(), scanned, StringComparison.OrdinalIgnoreCase));
                if (matchedRow != null)
                {
                    string id = matchedRow["Id"]?.ToString();
                    await LoadProductIntoDetailAsync(id);
                }
                else
                {
                    txtTitle.Focus();
                    txtTitle.SelectAll();
                }
            }
        }

        private void cmbItemCatagory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isPopulating)
                Field_TextChanged(sender, e);
        }

        private void cmbItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isService = cmbItemType.SelectedIndex == 1;
            cmbPriUnit.Enabled = !isService;
            cmbSecUnit.Enabled = !isService;
            cmbDefUnit.Enabled = !isService;
            txtQtyInPack.Enabled = !isService;
            txtOpnStock.Enabled = !isService;
            txtOpnRate.Enabled = !isService;
            chkLowStockAlert.Enabled = !isService;
            txtLowStockQty.Enabled = !isService;

            if (!_isPopulating)
                Field_TextChanged(sender, e);
        }

        private void btnCategoryRates_Click(object sender, EventArgs e)
        {
            var initialRates = _pendingCategoryRates;
            using (var frm = new ERP.Forms.frmItemCategoryRates(_currentEditingItemId, txtTitle.Text, ParseDecimal(txtPriRate.Text), ParseDecimal(txtSecRate.Text), initialRates))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    if (string.IsNullOrEmpty(_currentEditingItemId) || _currentEditingItemId == "0")
                    {
                        _pendingCategoryRates = frm.ConfiguredRates;
                    }
                    UpdateSaveButtonState(true);
                }
            }
        }

        private void btnSubUnits_Click(object sender, EventArgs e)
        {
            if (cmbPriUnit.SelectedValue == null)
            {
                MessageBox.Show("Please select a Primary Unit first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (var frm = new frmItemSubUnits(txtTitle.Text?.Trim(), Convert.ToString(cmbPriUnit.SelectedValue), dtUnits, _currentAdditionalUnits))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _currentAdditionalUnits = frm.ResultAdditionalUnitsJson;
                    UpdateSaveButtonState(true);
                }
            }
        }

        private async void btnProductSearch_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                        var presigned = await _inventoryApiService.GetPresignedUploadUrlAsync(fileName);
                        if (presigned != null)
                        {
                            await _inventoryApiService.UploadFileAsync(presigned.UploadUrl, openFileDialog.FileName);
                            _currentMediaId = presigned.FileId;
                            _currentMediaUrl = openFileDialog.FileName;
                            profileImage1.MediaId = presigned.FileId;
                            profileImage1.MediaUrl = openFileDialog.FileName;
                            profileImage1.PictureBox.Image = Image.FromFile(openFileDialog.FileName);
                            UpdateSaveButtonState(true);
                            MessageBox.Show("Image uploaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to upload image: {ex.Message}", "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnProductCancel_Click(object sender, EventArgs e)
        {
            _currentMediaId = string.Empty;
            _currentMediaUrl = string.Empty;
            profileImage1.ClearImage();
            UpdateSaveButtonState(true);
        }

        private void Field_TextChanged(object sender, EventArgs e)
        {
            if (!_isPopulating && !FLogin)
            {
                UpdateSaveButtonState(true);
            }
        }

        private void Numeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void UpdateSaveButtonState(bool hasPendingChanges)
        {
            if (hasPendingChanges)
            {
                btnSave.Text = "* Save [F5]";
                btnSave.BackColor = Color.FromArgb(234, 88, 12);
                btnSave.ForeColor = Color.White;
                btnSave.Font = new Font(btnSave.Font, FontStyle.Bold);
            }
            else
            {
                btnSave.Text = "&Save [F5]";
                btnSave.BackColor = Color.FromArgb(22, 163, 74);
                btnSave.ForeColor = Color.White;
                btnSave.Font = new Font(btnSave.Font, FontStyle.Bold);
            }
        }

        private static decimal ParseDecimal(object value)
        {
            decimal parsed;
            return decimal.TryParse(Convert.ToString(value), out parsed) ? parsed : 0;
        }

        private static decimal? ParseNullableDecimal(object value)
        {
            decimal parsed;
            return decimal.TryParse(Convert.ToString(value), out parsed) ? (decimal?)parsed : null;
        }
    }
}
