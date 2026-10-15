using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP.Forms
{
    public partial class frmManufacturing : Form
    {
        private readonly ManufacturingApiService _manufacturingApiService;
        private readonly BomApiService _bomApiService;
        private readonly InventoryApiService _inventoryApiService;

        private List<ProductionMasterDto> _allVouchers = new List<ProductionMasterDto>();
        private List<BomDto> _allBoms = new List<BomDto>();
        private List<InventoryItemDto> _items = new List<InventoryItemDto>();

        private string _selectedVoucherNo = string.Empty;
        private string _currentStatus = "Planned";
        private bool _isLoading = false;

        public frmManufacturing()
        {
            InitializeComponent();
            dgvProductionList.AutoGenerateColumns = false;
            _manufacturingApiService = new ManufacturingApiService();
            _bomApiService = new BomApiService();
            _inventoryApiService = new InventoryApiService();
            UserInfo.ApplyFormPermissions(this, AppResource.Manufacturing);
        }

        private async void frmManufacturing_Load(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Today.AddDays(-30);
            dtpTo.Value = DateTime.Today;

            await LoadLookupsAsync();
            await LoadVouchersAsync();
            btnNew_Click(sender, e);
        }

        private async Task LoadLookupsAsync()
        {
            try
            {
                _items = await _inventoryApiService.GetLookupAsync("") ?? new List<InventoryItemDto>();

                cmbFinishedItem.DisplayMember = "Title";
                cmbFinishedItem.ValueMember = "Id";
                cmbFinishedItem.DataSource = new List<InventoryItemDto>(_items);

                clnRawItem.DisplayMember = "Title";
                clnRawItem.ValueMember = "Id";
                clnRawItem.DataSource = new List<InventoryItemDto>(_items);

                _allBoms = await _bomApiService.GetListAsync() ?? new List<BomDto>();
                var bomListForCombo = new List<BomDto> { new BomDto { Id = 0, BomName = "-- Select Recipe / BOM --" } };
                bomListForCombo.AddRange(_allBoms);

                cmbBom.DisplayMember = "BomName";
                cmbBom.ValueMember = "Id";
                cmbBom.DataSource = bomListForCombo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load lookups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadVouchersAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                string status = cmbStatusFilter.SelectedItem?.ToString() ?? "All";
                if (status == "All") status = "";

                _allVouchers = await _manufacturingApiService.GetListAsync(
                    fromDate: dtpFrom.Value.ToString("yyyy-MM-dd"),
                    toDate: dtpTo.Value.ToString("yyyy-MM-dd"),
                    status: status
                ) ?? new List<ProductionMasterDto>();

                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load vouchers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ApplyFilter()
        {
            string search = (txtSearch.Text ?? "").Trim().ToLowerInvariant();
            var filtered = _allVouchers.Where(v =>
                string.IsNullOrEmpty(search) ||
                (v.VoucherNo ?? "").ToLowerInvariant().Contains(search) ||
                (v.FinishedItemName ?? "").ToLowerInvariant().Contains(search) ||
                (v.BatchNo ?? "").ToLowerInvariant().Contains(search)
            ).ToList();

            dgvProductionList.DataSource = filtered;
        }

        private async void btnFilter_Click(object sender, EventArgs e)
        {
            await LoadVouchersAsync();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private async void dgvProductionList_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            if (dgvProductionList.CurrentRow != null && dgvProductionList.CurrentRow.DataBoundItem is ProductionMasterDto item)
            {
                await LoadVoucherDetailAsync(item.VoucherNo);
            }
        }

        private async Task LoadVoucherDetailAsync(string voucherNo)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var voucher = await _manufacturingApiService.GetByVoucherNoAsync(voucherNo);
                if (voucher != null)
                {
                    PopulateVoucher(voucher);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load voucher detail: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void PopulateVoucher(ProductionMasterDto voucher)
        {
            _isLoading = true;
            _selectedVoucherNo = voucher.VoucherNo;
            txtVoucherNo.Text = voucher.VoucherNo;
            dtpDate.Value = voucher.Date;

            if (voucher.BomId.HasValue && voucher.BomId.Value > 0)
                cmbBom.SelectedValue = voucher.BomId.Value;
            else
                cmbBom.SelectedIndex = 0;

            cmbFinishedItem.SelectedValue = voucher.FinishedItemId;
            txtBatchNo.Text = voucher.BatchNo;
            txtPlannedQty.Text = voucher.PlannedQuantity.ToString("0.##");
            txtProducedQty.Text = voucher.ProducedQuantity.ToString("0.##");
            txtRejectedQty.Text = voucher.RejectedQuantity.ToString("0.##");

            _currentStatus = voucher.Status ?? "Planned";
            lblStatusDisplay.Text = _currentStatus;
            UpdateStatusBadgeColor(_currentStatus);

            txtStage.Text = voucher.CurrentStage;
            txtRemarks.Text = voucher.Remarks;

            txtLaborCost.Text = voucher.ActualLaborCost.ToString("0.##");
            txtOverheadCost.Text = voucher.ActualOverheadCost.ToString("0.##");

            dgvRawMaterials.Rows.Clear();
            if (voucher.Items != null)
            {
                foreach (var line in voucher.Items)
                {
                    int rowIndex = dgvRawMaterials.Rows.Add();
                    var row = dgvRawMaterials.Rows[rowIndex];
                    row.Cells[clnRawItem.Index].Value = line.RawItemId;
                    row.Cells[clnPlannedQty.Index].Value = line.PlannedQuantity.ToString("0.##");
                    row.Cells[clnActualQty.Index].Value = line.ActualQuantity.ToString("0.##");
                    row.Cells[clnWastageQty.Index].Value = line.WastageQuantity.ToString("0.##");
                    row.Cells[clnUnitCost.Index].Value = line.UnitCost.ToString("0.##");
                    row.Cells[clnTotalCost.Index].Value = line.TotalCost.ToString("0.##");
                    row.Cells[clnDetailNotes.Index].Value = line.Notes;
                }
            }

            _isLoading = false;
            RecalculateTotals();
            UpdateActionButtonsState();
        }

        private void UpdateStatusBadgeColor(string status)
        {
            switch (status)
            {
                case "Completed":
                    lblStatusDisplay.ForeColor = Color.DarkGreen;
                    break;
                case "InProgress":
                    lblStatusDisplay.ForeColor = Color.DarkOrange;
                    break;
                case "Cancelled":
                    lblStatusDisplay.ForeColor = Color.Crimson;
                    break;
                default:
                    lblStatusDisplay.ForeColor = Color.DarkBlue;
                    break;
            }
        }

        private void UpdateActionButtonsState()
        {
            bool isCompletedOrCancelled = _currentStatus == "Completed" || _currentStatus == "Cancelled";
            btnSave.Enabled = !isCompletedOrCancelled;
            btnIssueMaterials.Enabled = _currentStatus == "Planned" && !string.IsNullOrEmpty(_selectedVoucherNo);
            btnComplete.Enabled = !isCompletedOrCancelled && !string.IsNullOrEmpty(_selectedVoucherNo);
            btnCancel.Enabled = !isCompletedOrCancelled && !string.IsNullOrEmpty(_selectedVoucherNo);
            btnDelete.Enabled = !isCompletedOrCancelled && !string.IsNullOrEmpty(_selectedVoucherNo);
            dgvRawMaterials.ReadOnly = isCompletedOrCancelled;
            btnAddRow.Enabled = !isCompletedOrCancelled;
            btnDeleteRow.Enabled = !isCompletedOrCancelled;
        }

        private void cmbBom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            if (cmbBom.SelectedItem is BomDto bom && bom.Id > 0)
            {
                _isLoading = true;
                cmbFinishedItem.SelectedValue = bom.FinishedItemId;
                decimal plannedQty = ParseDecimal(txtPlannedQty.Text);
                if (plannedQty <= 0) plannedQty = bom.OutputQuantity > 0 ? bom.OutputQuantity : 1;
                txtPlannedQty.Text = plannedQty.ToString("0.##");

                decimal multiplier = bom.OutputQuantity > 0 ? (plannedQty / bom.OutputQuantity) : 1;
                txtLaborCost.Text = (bom.EstimatedLaborCost * multiplier).ToString("0.##");
                txtOverheadCost.Text = (bom.EstimatedOverheadCost * multiplier).ToString("0.##");

                dgvRawMaterials.Rows.Clear();
                if (bom.Items != null)
                {
                    foreach (var line in bom.Items)
                    {
                        int rowIndex = dgvRawMaterials.Rows.Add();
                        var row = dgvRawMaterials.Rows[rowIndex];
                        decimal requiredQty = line.Quantity * multiplier;
                        row.Cells[clnRawItem.Index].Value = line.RawItemId;
                        row.Cells[clnPlannedQty.Index].Value = requiredQty.ToString("0.##");
                        row.Cells[clnActualQty.Index].Value = requiredQty.ToString("0.##");
                        row.Cells[clnWastageQty.Index].Value = "0";
                        row.Cells[clnUnitCost.Index].Value = line.StandardCost.ToString("0.##");
                        row.Cells[clnTotalCost.Index].Value = (requiredQty * line.StandardCost).ToString("0.##");
                        row.Cells[clnDetailNotes.Index].Value = line.Notes;
                    }
                }

                _isLoading = false;
                RecalculateTotals();
            }
        }

        private void txtPlannedQty_TextChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            if (cmbBom.SelectedItem is BomDto bom && bom.Id > 0 && dgvRawMaterials.Rows.Count > 0)
            {
                decimal plannedQty = ParseDecimal(txtPlannedQty.Text);
                if (plannedQty > 0 && bom.OutputQuantity > 0)
                {
                    decimal multiplier = plannedQty / bom.OutputQuantity;
                    _isLoading = true;
                    txtLaborCost.Text = (bom.EstimatedLaborCost * multiplier).ToString("0.##");
                    txtOverheadCost.Text = (bom.EstimatedOverheadCost * multiplier).ToString("0.##");

                    for (int i = 0; i < dgvRawMaterials.Rows.Count && i < (bom.Items?.Count ?? 0); i++)
                    {
                        var row = dgvRawMaterials.Rows[i];
                        var line = bom.Items[i];
                        decimal requiredQty = line.Quantity * multiplier;
                        row.Cells[clnPlannedQty.Index].Value = requiredQty.ToString("0.##");
                        row.Cells[clnActualQty.Index].Value = requiredQty.ToString("0.##");
                        decimal cost = ParseDecimal(row.Cells[clnUnitCost.Index].Value);
                        row.Cells[clnTotalCost.Index].Value = (requiredQty * cost).ToString("0.##");
                    }
                    _isLoading = false;
                    RecalculateTotals();
                }
            }
        }

        private void dgvRawMaterials_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_isLoading || e.RowIndex < 0) return;
            var row = dgvRawMaterials.Rows[e.RowIndex];

            // If item changed, default its standard rate if 0
            if (e.ColumnIndex == clnRawItem.Index)
            {
                var itemId = row.Cells[clnRawItem.Index].Value?.ToString();
                var item = _items.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    var curCost = ParseDecimal(row.Cells[clnUnitCost.Index].Value);
                    if (curCost == 0)
                        row.Cells[clnUnitCost.Index].Value = item.PriRate.ToString("0.##");
                }
            }

            decimal actualQty = ParseDecimal(row.Cells[clnActualQty.Index].Value);
            decimal cost = ParseDecimal(row.Cells[clnUnitCost.Index].Value);
            decimal lineCost = actualQty * cost;
            row.Cells[clnTotalCost.Index].Value = lineCost.ToString("0.##");

            RecalculateTotals();
        }

        private void CalculateUnitCost(object sender, EventArgs e)
        {
            if (!_isLoading) RecalculateTotals();
        }

        private void RecalculateTotals()
        {
            decimal rawMaterialsTotal = 0;
            foreach (DataGridViewRow row in dgvRawMaterials.Rows)
            {
                if (row.IsNewRow) continue;
                rawMaterialsTotal += ParseDecimal(row.Cells[clnTotalCost.Index].Value);
            }

            decimal labor = ParseDecimal(txtLaborCost.Text);
            decimal overhead = ParseDecimal(txtOverheadCost.Text);
            decimal totalCost = rawMaterialsTotal + labor + overhead;

            txtRawCost.Text = rawMaterialsTotal.ToString("N2");
            txtTotalCost.Text = totalCost.ToString("N2");

            decimal producedQty = ParseDecimal(txtProducedQty.Text);
            decimal plannedQty = ParseDecimal(txtPlannedQty.Text);
            decimal qtyForUnitCost = producedQty > 0 ? producedQty : plannedQty;

            decimal unitCost = qtyForUnitCost > 0 ? (totalCost / qtyForUnitCost) : 0;
            txtUnitCost.Text = unitCost.ToString("N2");
        }

        private decimal ParseDecimal(object val)
        {
            if (val == null) return 0;
            decimal d;
            if (decimal.TryParse(val.ToString(), out d)) return d;
            return 0;
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            int rowIndex = dgvRawMaterials.Rows.Add();
            var row = dgvRawMaterials.Rows[rowIndex];
            if (_items.Count > 0)
                row.Cells[clnRawItem.Index].Value = _items[0].Id;
            row.Cells[clnPlannedQty.Index].Value = "1";
            row.Cells[clnActualQty.Index].Value = "1";
            row.Cells[clnWastageQty.Index].Value = "0";
            row.Cells[clnUnitCost.Index].Value = "0";
            row.Cells[clnTotalCost.Index].Value = "0.00";
            row.Cells[clnDetailNotes.Index].Value = "";
            RecalculateTotals();
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            if (dgvRawMaterials.CurrentRow != null && !dgvRawMaterials.CurrentRow.IsNewRow)
            {
                dgvRawMaterials.Rows.Remove(dgvRawMaterials.CurrentRow);
                RecalculateTotals();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            _isLoading = true;
            _selectedVoucherNo = string.Empty;
            txtVoucherNo.Text = "";
            dtpDate.Value = DateTime.Today;
            if (cmbBom.Items.Count > 0) cmbBom.SelectedIndex = 0;
            if (_items.Count > 0) cmbFinishedItem.SelectedIndex = 0;
            txtBatchNo.Text = "BATCH-" + DateTime.Now.ToString("yyyyMMdd");
            txtPlannedQty.Text = "1";
            txtProducedQty.Text = "0";
            txtRejectedQty.Text = "0";
            _currentStatus = "Planned";
            lblStatusDisplay.Text = "Planned";
            UpdateStatusBadgeColor("Planned");
            txtStage.Text = "Planning";
            txtRemarks.Text = "";
            txtLaborCost.Text = "0";
            txtOverheadCost.Text = "0";
            dgvRawMaterials.Rows.Clear();
            _isLoading = false;
            RecalculateTotals();
            UpdateActionButtonsState();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            string finishedItemId = cmbFinishedItem.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(finishedItemId))
            {
                MessageBox.Show("Please select an Output / Finished Product.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbFinishedItem.Focus();
                return;
            }

            decimal plannedQty = ParseDecimal(txtPlannedQty.Text);
            if (plannedQty <= 0)
            {
                MessageBox.Show("Planned Quantity must be greater than 0.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPlannedQty.Focus();
                return;
            }

            if (dgvRawMaterials.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one raw material line.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? bomId = null;
            if (cmbBom.SelectedItem is BomDto bom && bom.Id > 0) bomId = bom.Id;

            var finishedItem = _items.FirstOrDefault(i => i.Id == finishedItemId);
            var request = new ProductionCreateUpdateRequest
            {
                VoucherNo = _selectedVoucherNo,
                Date = dtpDate.Value,
                BomId = bomId,
                FinishedItemId = finishedItemId,
                PlannedQuantity = plannedQty,
                ProducedQuantity = ParseDecimal(txtProducedQty.Text),
                RejectedQuantity = ParseDecimal(txtRejectedQty.Text),
                UnitId = finishedItem?.DefaultUnit ?? finishedItem?.PrimaryUnit,
                Status = _currentStatus,
                CurrentStage = txtStage.Text.Trim(),
                ActualLaborCost = ParseDecimal(txtLaborCost.Text),
                ActualOverheadCost = ParseDecimal(txtOverheadCost.Text),
                BatchNo = txtBatchNo.Text.Trim(),
                Remarks = txtRemarks.Text.Trim(),
                Items = new List<ProductionDetailCreateUpdateRequest>()
            };

            int seq = 1;
            foreach (DataGridViewRow row in dgvRawMaterials.Rows)
            {
                if (row.IsNewRow) continue;
                string rawId = row.Cells[clnRawItem.Index].Value?.ToString();
                if (string.IsNullOrEmpty(rawId)) continue;

                var rawItem = _items.FirstOrDefault(i => i.Id == rawId);
                string unitId = rawItem?.DefaultUnit ?? rawItem?.PrimaryUnit;

                request.Items.Add(new ProductionDetailCreateUpdateRequest
                {
                    Seq = seq++,
                    RawItemId = rawId,
                    PlannedQuantity = ParseDecimal(row.Cells[clnPlannedQty.Index].Value),
                    ActualQuantity = ParseDecimal(row.Cells[clnActualQty.Index].Value),
                    UnitId = unitId,
                    UnitCost = ParseDecimal(row.Cells[clnUnitCost.Index].Value),
                    TotalCost = ParseDecimal(row.Cells[clnTotalCost.Index].Value),
                    WastageQuantity = ParseDecimal(row.Cells[clnWastageQty.Index].Value),
                    Notes = row.Cells[clnDetailNotes.Index].Value?.ToString()
                });
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                if (string.IsNullOrEmpty(_selectedVoucherNo))
                {
                    string newVoucherNo = await _manufacturingApiService.CreateAsync(request);
                    MessageBox.Show("Production plan saved successfully! Voucher: " + newVoucherNo, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _selectedVoucherNo = newVoucherNo;
                    txtVoucherNo.Text = newVoucherNo;
                }
                else
                {
                    await _manufacturingApiService.UpdateAsync(_selectedVoucherNo, request);
                    MessageBox.Show("Production plan updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await LoadVouchersAsync();
                UpdateActionButtonsState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save production voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void btnIssueMaterials_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedVoucherNo))
            {
                MessageBox.Show("Please save the production plan before issuing materials.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Issue raw materials to production floor (WIP)? This marks status as InProgress and registers the materials issued.", "Confirm Issue", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    var req = new IssueMaterialsApiRequest
                    {
                        IssueDate = DateTime.Now,
                        Stage = "Assembly / In-Progress",
                        Materials = new List<MaterialIssueItemDto>()
                    };

                    foreach (DataGridViewRow row in dgvRawMaterials.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string rawId = row.Cells[clnRawItem.Index].Value?.ToString();
                        if (string.IsNullOrEmpty(rawId)) continue;

                        req.Materials.Add(new MaterialIssueItemDto
                        {
                            RawItemId = rawId,
                            ActualQuantity = ParseDecimal(row.Cells[clnActualQty.Index].Value),
                            WastageQuantity = ParseDecimal(row.Cells[clnWastageQty.Index].Value)
                        });
                    }

                    await _manufacturingApiService.IssueMaterialsAsync(_selectedVoucherNo, req);
                    MessageBox.Show("Materials issued to WIP floor successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    await LoadVouchersAsync();
                    await LoadVoucherDetailAsync(_selectedVoucherNo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to issue materials: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private async void btnComplete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedVoucherNo))
            {
                MessageBox.Show("Please save the production plan first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal producedQty = ParseDecimal(txtProducedQty.Text);
            if (producedQty <= 0)
            {
                decimal planned = ParseDecimal(txtPlannedQty.Text);
                txtProducedQty.Text = planned.ToString("0.##");
                producedQty = planned;
            }

            string confirmMsg = $"Complete this production batch?\n\nProduced Qty: {producedQty}\n\nThis will consume the raw materials from stock, add {producedQty} finished product to inventory with calculated unit cost, and balance accounting!";
            if (MessageBox.Show(confirmMsg, "Confirm Completion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    var req = new CompleteProductionApiRequest
                    {
                        CompletionDate = DateTime.Now,
                        ProducedQuantity = producedQty,
                        RejectedQuantity = ParseDecimal(txtRejectedQty.Text),
                        ActualLaborCost = ParseDecimal(txtLaborCost.Text),
                        ActualOverheadCost = ParseDecimal(txtOverheadCost.Text)
                    };

                    await _manufacturingApiService.CompleteProductionAsync(_selectedVoucherNo, req);
                    MessageBox.Show("Production batch completed successfully! Stock and GL ledgers updated.", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    await LoadVouchersAsync();
                    await LoadVoucherDetailAsync(_selectedVoucherNo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to complete production: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private async void btnCancel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedVoucherNo)) return;

            if (MessageBox.Show("Are you sure you want to cancel this production batch?", "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    await _manufacturingApiService.CancelProductionAsync(_selectedVoucherNo);
                    MessageBox.Show("Production batch cancelled.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadVouchersAsync();
                    await LoadVoucherDetailAsync(_selectedVoucherNo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to cancel production: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedVoucherNo)) return;

            if (MessageBox.Show("Are you sure you want to delete this production voucher?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    await _manufacturingApiService.DeleteAsync(_selectedVoucherNo);
                    MessageBox.Show("Production voucher deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnNew_Click(sender, e);
                    await LoadVouchersAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to delete voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
