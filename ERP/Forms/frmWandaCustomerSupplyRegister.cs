using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Reporting;
using ERP.Services.Legacy;

namespace ERP
{
    /// <summary>
    /// Dedicated Customer Supply Register for Wanda (Feed & Commodity) Tenants.
    /// Pure dual-unit logic:
    /// - Qty is Weight (Kg), SecQty is Bags, Rate is per Kg, SecRate is Bag Rate
    /// - Formula: (Qty * (Rate - Discount)) + AddLess
    /// - High performance, zero conditional overhead
    /// </summary>
    public partial class frmWandaCustomerSupplyRegister : Form
    {
        private readonly SaleSupplyApiService _apiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly InventoryApiService _inventoryApiService;
        private readonly UnitApiService _unitApiService;

        private List<ChartOfAccountHeadDto> _customers = new List<ChartOfAccountHeadDto>();
        private List<InventoryItemDto> _items = new List<InventoryItemDto>();
        private List<UnitLookupDto> _units = new List<UnitLookupDto>();
        private List<WandaRegisterRowModel> _rowModels = new List<WandaRegisterRowModel>();
        private bool _isLoading = false;
        private bool _isPopulatingGrid = false;

        public frmWandaCustomerSupplyRegister()
        {
            _apiService = new SaleSupplyApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _inventoryApiService = new InventoryApiService();
            _unitApiService = new UnitApiService();

            InitializeComponent();
            WireEvents();

            this.Load += async (s, e) => await InitializeFormAsync();
        }

        public frmWandaCustomerSupplyRegister(string customerId, DateTime fromDate, DateTime toDate)
            : this()
        {
            dtpFromDate.Value = fromDate;
            dtpToDate.Value = toDate;
            this.Tag = customerId;
        }

        private void WireEvents()
        {
            try
            {
                typeof(DataGridView).InvokeMember(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                    null, dgvRecords, new object[] { true });
            }
            catch { }

            btnReload.Click += async (s, e) => await FetchRecordsAsync();
            btnSaveAll.Click += async (s, e) => await SaveAllModifiedRowsAsync();
            btnAddSupplyEntry.Click += (s, e) => OpenAddSupplyEntryDialog();
            btnPrintCustomerBill.Click += (s, e) => OpenCustomerBillViewer();

            cmbCustomer.SelectedIndexChanged += async (s, e) =>
            {
                if (!_isLoading) await FetchRecordsAsync();
            };
            btnSearch.Click += async (s, e) => await FetchRecordsAsync();

            btnPreset1to10.Click += async (s, e) => await ApplyPresetAsync(1, 10);
            btnPreset1to15.Click += async (s, e) => await ApplyPresetAsync(1, 15);
            btnPreset1to20.Click += async (s, e) => await ApplyPresetAsync(1, 20);
            btnPresetMonth.Click += async (s, e) => await ApplyPresetMonthAsync(false);
            btnPresetLastMonth.Click += async (s, e) => await ApplyPresetMonthAsync(true);

            pnlFilters.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, pnlFilters.Height - 1, pnlFilters.Width, pnlFilters.Height - 1);
            };

            dgvRecords.CellValueChanged += OnGridCellValueChanged;
            dgvRecords.CellContentClick += OnGridCellContentClick;

            this.Resize += (s, e) => LayoutHeaderButtons();
            LayoutHeaderButtons();
        }

        private void LayoutHeaderButtons()
        {
            if (pnlHeader == null) return;
            int rightMargin = 16;
            int spacing = 8;
            int curX = pnlHeader.ClientSize.Width - rightMargin;

            Button[] rightToLeft = { btnPrintCustomerBill, btnAddSupplyEntry, btnSaveAll, btnReload };
            foreach (var btn in rightToLeft)
            {
                if (btn == null) continue;
                curX -= btn.Width;
                btn.Location = new Point(curX, (pnlHeader.Height - btn.Height) / 2);
                curX -= spacing;
            }
        }

        private async Task InitializeFormAsync()
        {
            try
            {
                _isLoading = true;
                lblStatus.Text = "Loading Wanda customers, items and defaults...";
                prgProgress.Visible = true;

                var now = DateTime.Today;
                if (this.Tag == null)
                {
                    dtpFromDate.Value = new DateTime(now.Year, now.Month, 1);
                    dtpToDate.Value = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
                }

                var customerTask = _chartOfAccountApiService.GetCustomerAccountsAsync();
                var itemTask = _inventoryApiService.GetLookupAsync(null);
                var unitTask = _unitApiService.GetLookupAsync();

                await Task.WhenAll(customerTask, itemTask, unitTask);

                _customers = await customerTask;
                _items = await itemTask;
                _units = await unitTask;

                cmbCustomer.DisplayMember = "Title";
                cmbCustomer.ValueMember = "Account";
                cmbCustomer.DataSource = new List<ChartOfAccountHeadDto>(_customers);

                var filterItems = new List<InventoryItemDto>
                {
                    new InventoryItemDto { Id = "", Title = "-- All Products --" }
                };
                filterItems.AddRange(_items);
                cmbItem.DisplayMember = "Title";
                cmbItem.ValueMember = "Id";
                cmbItem.DataSource = filterItems;
                cmbItem.SelectedIndex = 0;

                string passedCus = this.Tag as string;
                if (!string.IsNullOrEmpty(passedCus))
                {
                    cmbCustomer.SelectedValue = passedCus;
                }
                else if (_customers.Count > 0)
                {
                    cmbCustomer.SelectedIndex = 0;
                }

                _isLoading = false;
                prgProgress.Visible = false;

                if (cmbCustomer.SelectedValue != null)
                {
                    await FetchRecordsAsync();
                }
                else
                {
                    lblStatus.Text = "Ready. Select a Wanda customer to begin.";
                }
            }
            catch (Exception ex)
            {
                _isLoading = false;
                prgProgress.Visible = false;
                lblStatus.Text = "Error loading form lookups: " + ex.Message;
                MessageBox.Show("Failed to load customer register lookups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ApplyPresetAsync(int startDay, int endDay)
        {
            var now = DateTime.Today;
            dtpFromDate.Value = new DateTime(now.Year, now.Month, startDay);
            dtpToDate.Value = new DateTime(now.Year, now.Month, Math.Min(endDay, DateTime.DaysInMonth(now.Year, now.Month)));
            await FetchRecordsAsync();
        }

        private async Task ApplyPresetMonthAsync(bool lastMonth)
        {
            var target = lastMonth ? DateTime.Today.AddMonths(-1) : DateTime.Today;
            dtpFromDate.Value = new DateTime(target.Year, target.Month, 1);
            dtpToDate.Value = new DateTime(target.Year, target.Month, DateTime.DaysInMonth(target.Year, target.Month));
            await FetchRecordsAsync();
        }

        private async Task FetchRecordsAsync()
        {
            if (cmbCustomer.SelectedValue == null) return;
            string customerId = cmbCustomer.SelectedValue.ToString();
            if (string.IsNullOrWhiteSpace(customerId)) return;

            try
            {
                _isLoading = true;
                lblStatus.Text = "Fetching supply records for " + cmbCustomer.Text + "...";
                prgProgress.Visible = true;
                btnSearch.Enabled = false;
                btnReload.Enabled = false;

                string fromDate = dtpFromDate.Value.ToString("yyyy-MM-dd");
                string toDate = dtpToDate.Value.ToString("yyyy-MM-dd");
                string itemId = cmbItem.SelectedValue != null ? cmbItem.SelectedValue.ToString() : "";

                var lines = await _apiService.GetCustomerLinesAsync(customerId, fromDate, toDate, itemId);
                lines.Sort((a, b) => a.Date.CompareTo(b.Date));

                _rowModels = lines.Select(l => new WandaRegisterRowModel(l)).ToList();

                PopulateGridFromModels();
                lblStatus.Text = $"Loaded {_rowModels.Count} supply records for {cmbCustomer.Text}.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error fetching records: " + ex.Message;
                MessageBox.Show("Failed to load supply records: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isLoading = false;
                prgProgress.Visible = false;
                btnSearch.Enabled = true;
                btnReload.Enabled = true;
            }
        }

        private void PopulateGridFromModels()
        {
            _isPopulatingGrid = true;
            dgvRecords.Rows.Clear();

            foreach (var m in _rowModels)
            {
                int rowIndex = dgvRecords.Rows.Add();
                var row = dgvRecords.Rows[rowIndex];

                row.Cells["colDate"].Value = m.Date.ToString("dd-MMM-yyyy");
                row.Cells["colVoucher"].Value = "SP-" + m.VoucherNo;
                row.Cells["colItem"].Value = !string.IsNullOrWhiteSpace(m.ItemTitle) ? m.ItemTitle : m.ItemId;
                row.Cells["colQty"].Value = m.Qty.ToString("N2");
                row.Cells["colSecQty"].Value = m.SecQty.ToString("N2");
                row.Cells["colPackQty"].Value = m.PackQty.ToString("N2");
                row.Cells["colRate"].Value = m.Rate.ToString("N2");
                row.Cells["colSecRate"].Value = m.SecRate.ToString("N2");
                row.Cells["colDiscount"].Value = m.Discount.ToString("N2");
                row.Cells["colAddLess"].Value = m.AddLess.ToString("N2");
                row.Cells["colAmount"].Value = m.Amount.ToString("N2");

                row.Tag = m;
                ApplyRowDirtyStyle(row, m.IsDirty);
            }

            _isPopulatingGrid = false;
            RecalculateKpis();
        }

        private void OnGridCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_isPopulatingGrid || _isLoading) return;
            if (e.RowIndex < 0 || e.RowIndex >= dgvRecords.Rows.Count) return;

            var row = dgvRecords.Rows[e.RowIndex];
            var model = row.Tag as WandaRegisterRowModel;
            if (model == null) return;

            string colName = dgvRecords.Columns[e.ColumnIndex].Name;

            if (colName == "colQty")
            {
                model.Qty = ParseDecimal(row.Cells["colQty"].Value);
                if (model.SecQty > 0)
                    model.PackQty = Math.Round(model.Qty / model.SecQty, 2);
                else if (model.PackQty > 0)
                    model.SecQty = Math.Round(model.Qty / model.PackQty, 2);
                model.IsDirty = true;
            }
            else if (colName == "colSecQty")
            {
                model.SecQty = ParseDecimal(row.Cells["colSecQty"].Value);
                if (model.PackQty > 0)
                    model.Qty = Math.Round(model.SecQty * model.PackQty, 2);
                else if (model.Qty > 0)
                    model.PackQty = Math.Round(model.Qty / model.SecQty, 2);
                model.IsDirty = true;
            }
            else if (colName == "colPackQty")
            {
                model.PackQty = ParseDecimal(row.Cells["colPackQty"].Value);
                model.Packing = model.PackQty;
                if (model.SecQty > 0)
                    model.Qty = Math.Round(model.SecQty * model.PackQty, 2);
                else if (model.Qty > 0)
                    model.SecQty = Math.Round(model.Qty / model.PackQty, 2);

                if (model.Packing > 0)
                {
                    if (model.SecRate > 0)
                        model.Rate = Math.Round(model.SecRate / model.Packing, 4);
                    else if (model.Rate > 0)
                        model.SecRate = Math.Round(model.Rate * model.Packing, 4);
                }
                model.IsDirty = true;
            }
            else if (colName == "colRate")
            {
                model.Rate = ParseDecimal(row.Cells["colRate"].Value);
                if (model.Packing > 0)
                    model.SecRate = Math.Round(model.Rate * model.Packing, 4);
                model.IsDirty = true;
            }
            else if (colName == "colSecRate")
            {
                model.SecRate = ParseDecimal(row.Cells["colSecRate"].Value);
                if (model.Packing > 0)
                    model.Rate = Math.Round(model.SecRate / model.Packing, 4);
                model.IsDirty = true;
            }
            else if (colName == "colDiscount")
            {
                model.Discount = ParseDecimal(row.Cells["colDiscount"].Value);
                model.IsDirty = true;
            }
            else if (colName == "colAddLess")
            {
                model.AddLess = ParseDecimal(row.Cells["colAddLess"].Value);
                model.IsDirty = true;
            }

            // Wanda formula: (Qty * (Rate - Discount)) + AddLess
            model.Amount = Math.Round((model.Qty * (model.Rate - model.Discount)) + model.AddLess, 2);

            _isPopulatingGrid = true;
            row.Cells["colQty"].Value = model.Qty.ToString("N2");
            row.Cells["colSecQty"].Value = model.SecQty.ToString("N2");
            row.Cells["colPackQty"].Value = model.PackQty.ToString("N2");
            row.Cells["colRate"].Value = model.Rate.ToString("N2");
            row.Cells["colSecRate"].Value = model.SecRate.ToString("N2");
            row.Cells["colDiscount"].Value = model.Discount.ToString("N2");
            row.Cells["colAddLess"].Value = model.AddLess.ToString("N2");
            row.Cells["colAmount"].Value = model.Amount.ToString("N2");
            _isPopulatingGrid = false;

            ApplyRowDirtyStyle(row, model.IsDirty);
            RecalculateKpis();
        }

        private void ApplyRowDirtyStyle(DataGridViewRow row, bool isDirty)
        {
            if (isDirty)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(254, 243, 199);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(146, 64, 14);
                row.Cells["colSave"].Style.BackColor = Color.FromArgb(217, 119, 6);
                row.Cells["colSave"].Style.ForeColor = Color.White;
            }
            else
            {
                row.DefaultCellStyle.BackColor = row.Index % 2 == 0 ? Color.White : Color.FromArgb(248, 250, 252);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
                row.Cells["colSave"].Style.BackColor = Color.FromArgb(241, 245, 249);
                row.Cells["colSave"].Style.ForeColor = Color.FromArgb(148, 163, 184);
            }
        }

        private void RecalculateKpis()
        {
            int totalRecords = _rowModels.Count;
            decimal totalQty = _rowModels.Sum(m => m.Qty);
            decimal totalBags = _rowModels.Sum(m => m.SecQty);
            decimal totalAmount = _rowModels.Sum(m => m.Amount);
            int dirtyCount = _rowModels.Count(m => m.IsDirty);

            lblKpiRecordsVal.Text = $"{totalRecords} Records";
            lblKpiQtyVal.Text = totalQty.ToString("N2");
            lblKpiBagsVal.Text = totalBags.ToString("N2");
            lblKpiAmountVal.Text = $"Rs. {totalAmount:N2}";

            if (dirtyCount > 0)
            {
                lblKpiPendingVal.Text = $"{dirtyCount} Unsaved Changes";
                lblKpiPendingVal.ForeColor = Color.FromArgb(217, 119, 6);
                btnSaveAll.Enabled = true;
                btnSaveAll.Text = $"Save All Changes ({dirtyCount})";
            }
            else
            {
                lblKpiPendingVal.Text = "0 Pending";
                lblKpiPendingVal.ForeColor = Color.FromArgb(100, 116, 139);
                btnSaveAll.Enabled = false;
                btnSaveAll.Text = "Save All Changes (0)";
            }
        }

        private async void OnGridCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvRecords.Rows.Count) return;

            var row = dgvRecords.Rows[e.RowIndex];
            var model = row.Tag as WandaRegisterRowModel;
            if (model == null) return;

            string colName = dgvRecords.Columns[e.ColumnIndex].Name;

            if (colName == "colSave")
            {
                if (!model.IsDirty)
                {
                    MessageBox.Show("No changes to save for this row.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                await SaveSingleRowAsync(row, model);
            }
            else if (colName == "colDelete")
            {
                var dr = MessageBox.Show(
                    $"Are you sure you want to delete this line from SP-{model.VoucherNo}?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    await DeleteRowAsync(row, model);
                }
            }
        }

        private async Task SaveSingleRowAsync(DataGridViewRow row, WandaRegisterRowModel model)
        {
            try
            {
                lblStatus.Text = $"Saving record for SP-{model.VoucherNo}...";
                prgProgress.Visible = true;

                await _apiService.UpdateLineAsync(model.VoucherNo, model.Seq, new SaleSupplyLineApiRequest
                {
                    Seq = model.Seq,
                    CustomerId = model.CustomerId,
                    Qty = model.Qty,
                    Rate = model.Rate,
                    Discount = model.Discount,
                    AddLess = model.AddLess,
                    SecQty = model.SecQty,
                    SecRate = model.SecRate,
                    SecUnit = "Bags",
                    QtyInPack = model.PackQty,
                    Packing = model.Packing
                });

                model.IsDirty = false;
                ApplyRowDirtyStyle(row, false);
                RecalculateKpis();

                lblStatus.Text = $"Successfully updated line for voucher SP-{model.VoucherNo}.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error saving line: " + ex.Message;
                MessageBox.Show("Failed to save supply record: " + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                prgProgress.Visible = false;
            }
        }

        private async Task SaveAllModifiedRowsAsync()
        {
            var dirtyModels = _rowModels.Where(m => m.IsDirty).ToList();
            if (dirtyModels.Count == 0)
            {
                MessageBox.Show("There are no modified records to save.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                lblStatus.Text = $"Batch saving {dirtyModels.Count} modified records...";
                prgProgress.Visible = true;
                btnSaveAll.Enabled = false;

                var requests = dirtyModels.Select(m => new SaleSupplyCustomerLineUpdateRequest
                {
                    VoucherNo = m.VoucherNo,
                    Seq = m.Seq,
                    Line = new SaleSupplyLineApiRequest
                    {
                        Seq = m.Seq,
                        CustomerId = m.CustomerId,
                        Qty = m.Qty,
                        Rate = m.Rate,
                        Discount = m.Discount,
                        AddLess = m.AddLess,
                        SecQty = m.SecQty,
                        SecRate = m.SecRate,
                        SecUnit = "Bags",
                        QtyInPack = m.PackQty,
                        Packing = m.Packing
                    }
                }).ToList();

                await _apiService.UpdateCustomerLinesAsync(requests);

                foreach (var m in dirtyModels)
                {
                    m.IsDirty = false;
                }

                foreach (DataGridViewRow row in dgvRecords.Rows)
                {
                    ApplyRowDirtyStyle(row, false);
                }

                RecalculateKpis();
                lblStatus.Text = $"Successfully updated {dirtyModels.Count} Wanda supply records.";
                MessageBox.Show($"Successfully saved {dirtyModels.Count} supply record updates.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error batch saving records: " + ex.Message;
                MessageBox.Show("Failed to batch save supply records: " + ex.Message, "Batch Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                prgProgress.Visible = false;
                btnSaveAll.Enabled = _rowModels.Any(m => m.IsDirty);
            }
        }

        private async Task DeleteRowAsync(DataGridViewRow row, WandaRegisterRowModel model)
        {
            try
            {
                lblStatus.Text = $"Deleting supply line from voucher SP-{model.VoucherNo}...";
                prgProgress.Visible = true;

                await _apiService.DeleteLineAsync(model.VoucherNo, model.Seq);

                _rowModels.Remove(model);
                dgvRecords.Rows.Remove(row);
                RecalculateKpis();

                lblStatus.Text = $"Record from voucher SP-{model.VoucherNo} deleted.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error deleting record: " + ex.Message;
                MessageBox.Show("Failed to delete record: " + ex.Message, "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                prgProgress.Visible = false;
            }
        }

        private void OpenAddSupplyEntryDialog()
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string customerId = cmbCustomer.SelectedValue.ToString();
            string customerTitle = cmbCustomer.Text;

            using (var dlg = new frmAddWandaSupplyEntryDialog(customerId, customerTitle, _items, _units, dtpFromDate.Value))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _ = FetchRecordsAsync();
                }
            }
        }

        private void OpenCustomerBillViewer()
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer first to print bill.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string customerId = cmbCustomer.SelectedValue.ToString();
            DateTime fromD = dtpFromDate.Value;
            DateTime toD = dtpToDate.Value;

            try
            {
                var viewer = new WandaCustomerBillViewer(customerId, fromD, toD);
                if (this.MdiParent != null)
                {
                    viewer.MdiParent = this.MdiParent;
                }
                viewer.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open Customer Bill Viewer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal ParseDecimal(object val)
        {
            if (val == null) return 0;
            string s = val.ToString().Replace(",", "").Trim();
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal r) ? r : 0;
        }

        private class WandaRegisterRowModel
        {
            public string VoucherNo { get; set; }
            public int Seq { get; set; }
            public DateTime Date { get; set; }
            public string CustomerId { get; set; }
            public string ItemId { get; set; }
            public string ItemTitle { get; set; }
            public decimal Qty { get; set; }
            public decimal SecQty { get; set; }
            public decimal Rate { get; set; }
            public decimal SecRate { get; set; }
            public decimal PackQty { get; set; }
            public decimal Packing { get; set; }
            public decimal Discount { get; set; }
            public decimal AddLess { get; set; }
            public decimal Amount { get; set; }
            public bool IsDirty { get; set; }

            public WandaRegisterRowModel(SaleSupplyLineDto dto)
            {
                VoucherNo = dto.VoucherNo;
                Seq = dto.Seq;
                Date = dto.Date;
                CustomerId = dto.CustomerId;
                ItemId = dto.ItemId;
                ItemTitle = dto.ItemTitle ?? dto.ItemId;
                Qty = dto.Qty;
                SecQty = dto.SecQty ?? 0;
                Rate = dto.Rate;
                SecRate = dto.SecRate ?? 0;
                PackQty = dto.QtyInPack ?? (dto.SecQty.HasValue && dto.SecQty.Value > 0 ? Math.Round(dto.Qty / dto.SecQty.Value, 2) : 50);
                Packing = dto.Packing ?? PackQty;
                Discount = dto.Discount;
                AddLess = dto.AddLess;
                Amount = dto.Amount;
                IsDirty = false;
            }
        }

        private class frmAddWandaSupplyEntryDialog : Form
        {
            private readonly SaleSupplyApiService _apiService = new SaleSupplyApiService();
            private readonly string _customerId;
            private DateTimePicker dtpDate;
            private ComboBox cmbItem;
            private TextBox txtQty;
            private TextBox txtSecQty;
            private TextBox txtRate;
            private TextBox txtSecRate;
            private TextBox txtDiscount;
            private TextBox txtAddLess;
            private Label lblAmountPreview;
            private Button btnSave;
            private Button btnCancel;

            public frmAddWandaSupplyEntryDialog(string customerId, string customerTitle, List<InventoryItemDto> items, List<UnitLookupDto> units, DateTime defaultDate)
            {
                _customerId = customerId;
                this.Text = "Add Wanda Supply Entry (Feed/Commodity) - " + customerTitle;
                this.Size = new Size(450, 400);
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.BackColor = Color.White;

                int y = 20;

                AddLabel("Date:", 16, y);
                dtpDate = new DateTimePicker { Location = new Point(130, y), Width = 280, Format = DateTimePickerFormat.Custom, CustomFormat = "dd-MMM-yyyy", Value = defaultDate };
                this.Controls.Add(dtpDate);
                y += 34;

                AddLabel("Product:", 16, y);
                cmbItem = new ComboBox { Location = new Point(130, y), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbItem.DisplayMember = "Title";
                cmbItem.ValueMember = "Id";
                cmbItem.DataSource = new List<InventoryItemDto>(items);
                cmbItem.SelectedIndexChanged += (s, e) =>
                {
                    if (cmbItem.SelectedItem is InventoryItemDto sel)
                    {
                        txtRate.Text = sel.PriRate.ToString("N2");
                    }
                };
                this.Controls.Add(cmbItem);
                y += 34;

                AddLabel("Weight (Kg):", 16, y);
                txtQty = new TextBox { Location = new Point(130, y), Width = 120, Text = "1.00" };
                txtQty.TextChanged += (s, e) => UpdateAmt();
                this.Controls.Add(txtQty);

                AddLabel("Bags (Qty):", 260, y);
                txtSecQty = new TextBox { Location = new Point(330, y), Width = 80, Text = "0.00" };
                txtSecQty.TextChanged += (s, e) => UpdateAmt();
                this.Controls.Add(txtSecQty);
                y += 34;

                AddLabel("Rate (/Kg):", 16, y);
                txtRate = new TextBox { Location = new Point(130, y), Width = 120, Text = "0.00" };
                txtRate.TextChanged += (s, e) => UpdateAmt();
                this.Controls.Add(txtRate);

                AddLabel("Bag Rate:", 260, y);
                txtSecRate = new TextBox { Location = new Point(330, y), Width = 80, Text = "0.00" };
                txtSecRate.TextChanged += (s, e) => UpdateAmt();
                this.Controls.Add(txtSecRate);
                y += 34;

                AddLabel("Discount:", 16, y);
                txtDiscount = new TextBox { Location = new Point(130, y), Width = 120, Text = "0.00" };
                txtDiscount.TextChanged += (s, e) => UpdateAmt();
                this.Controls.Add(txtDiscount);

                AddLabel("Add/Less:", 260, y);
                txtAddLess = new TextBox { Location = new Point(330, y), Width = 80, Text = "0.00" };
                txtAddLess.TextChanged += (s, e) => UpdateAmt();
                this.Controls.Add(txtAddLess);
                y += 38;

                var lblAmtTitle = new Label { Text = "Calculated Amount:", Location = new Point(16, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
                this.Controls.Add(lblAmtTitle);
                lblAmountPreview = new Label { Text = "Rs. 0.00", Location = new Point(140, y), AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(16, 185, 129) };
                this.Controls.Add(lblAmountPreview);
                y += 44;

                btnSave = new Button
                {
                    Text = "Save Entry",
                    Location = new Point(200, y),
                    Width = 105,
                    Height = 32,
                    BackColor = Color.FromArgb(16, 185, 129),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnSave.FlatAppearance.BorderSize = 0;
                btnSave.Click += async (s, e) => await OnSubmitAsync();
                this.Controls.Add(btnSave);

                btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(315, y),
                    Width = 95,
                    Height = 32,
                    BackColor = Color.FromArgb(241, 245, 249),
                    ForeColor = Color.FromArgb(51, 65, 85),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5F),
                    DialogResult = DialogResult.Cancel,
                    Cursor = Cursors.Hand
                };
                btnCancel.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
                this.Controls.Add(btnCancel);

                UpdateAmt();
            }

            private void AddLabel(string text, int x, int y)
            {
                var lbl = new Label
                {
                    Text = text,
                    Location = new Point(x, y + 3),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(71, 85, 105)
                };
                this.Controls.Add(lbl);
            }

            private decimal Parse(TextBox tb)
            {
                if (tb == null) return 0;
                string s = tb.Text.Replace(",", "").Trim();
                return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal r) ? r : 0;
            }

            private void UpdateAmt()
            {
                decimal qty = Parse(txtQty);
                decimal rate = Parse(txtRate);
                decimal disc = Parse(txtDiscount);
                decimal addLess = Parse(txtAddLess);
                // Wanda formula: (Weight * (Rate - Discount)) + AddLess
                decimal amt = Math.Round((qty * (rate - disc)) + addLess, 2);

                if (lblAmountPreview != null)
                {
                    lblAmountPreview.Text = $"Rs. {amt:N2}";
                }
            }

            private async Task OnSubmitAsync()
            {
                if (cmbItem.SelectedValue == null)
                {
                    MessageBox.Show("Please select a Wanda item.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal qty = Parse(txtQty);
                if (qty <= 0)
                {
                    MessageBox.Show("Weight (Kg) must be greater than 0.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    btnSave.Enabled = false;
                    btnSave.Text = "Saving...";

                    string dateStr = dtpDate.Value.ToString("yyyy-MM-dd");
                    string itemId = cmbItem.SelectedValue.ToString();
                    decimal rate = Parse(txtRate);
                    decimal disc = Parse(txtDiscount);
                    decimal addLess = Parse(txtAddLess);
                    decimal secQty = Parse(txtSecQty);
                    decimal secRate = Parse(txtSecRate);

                    decimal defaultPack = 50;
                    if (cmbItem.SelectedItem is InventoryItemDto itemObj && itemObj.QtyInPack.HasValue && itemObj.QtyInPack.Value > 0)
                        defaultPack = itemObj.QtyInPack.Value;
                    else if (secQty > 0)
                        defaultPack = Math.Round(qty / secQty, 2);

                    var existingVouchers = await _apiService.GetListAsync(dateStr, dateStr, itemId);

                    if (existingVouchers != null && existingVouchers.Count > 0)
                    {
                        var targetVoucher = existingVouchers[0];
                        var details = await _apiService.GetDetailAsync(targetVoucher.VoucherNo);
                        int nextSeq = details.Count > 0 ? details.Max(d => d.Seq) + 1 : 1;

                        var updatedLines = details.Select(d => new SaleSupplyLineApiRequest
                        {
                            Seq = d.Seq,
                            CustomerId = d.CustomerId,
                            Qty = d.Qty,
                            Rate = d.Rate,
                            Discount = d.Discount,
                            AddLess = d.AddLess,
                            SecQty = d.SecQty,
                            SecRate = d.SecRate,
                            SecUnit = "Bags",
                            QtyInPack = d.QtyInPack,
                            Packing = d.Packing
                        }).ToList();

                        updatedLines.Add(new SaleSupplyLineApiRequest
                        {
                            Seq = nextSeq,
                            CustomerId = _customerId,
                            Qty = qty,
                            Rate = rate,
                            Discount = disc,
                            AddLess = addLess,
                            SecQty = secQty,
                            SecRate = secRate,
                            SecUnit = "Bags",
                            QtyInPack = defaultPack,
                            Packing = defaultPack
                        });

                        await _apiService.UpdateAsync(targetVoucher.VoucherNo, new SaleSupplyUpdateApiRequest
                        {
                            Date = dateStr,
                            ItemId = itemId,
                            Lines = updatedLines
                        });

                        MessageBox.Show($"Added Wanda supply line to existing voucher SP-{targetVoucher.VoucherNo}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var newVoucherNo = await _apiService.CreateAsync(new SaleSupplyCreateApiRequest
                        {
                            Date = dateStr,
                            ItemId = itemId,
                            Lines = new List<SaleSupplyLineApiRequest>
                            {
                                new SaleSupplyLineApiRequest
                                {
                                    Seq = 1,
                                    CustomerId = _customerId,
                                    Qty = qty,
                                    Rate = rate,
                                    Discount = disc,
                                    AddLess = addLess,
                                    SecQty = secQty,
                                    SecRate = secRate,
                                    SecUnit = "Bags",
                                    QtyInPack = defaultPack,
                                    Packing = defaultPack
                                }
                            }
                        });

                        MessageBox.Show($"Created new Wanda supply voucher SP-{newVoucherNo}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to add Wanda supply entry: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnSave.Enabled = true;
                    btnSave.Text = "Save Entry";
                }
            }
        }
    }
}
