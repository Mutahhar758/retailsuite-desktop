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
    public partial class frmBOM : Form
    {
        private readonly BomApiService _bomApiService;
        private readonly InventoryApiService _inventoryApiService;
        private List<BomDto> _allBoms = new List<BomDto>();
        private List<InventoryItemDto> _items = new List<InventoryItemDto>();
        private int _selectedBomId = 0;
        private bool _isLoading = false;

        public frmBOM()
        {
            InitializeComponent();
            dgvBomList.AutoGenerateColumns = false;
            _bomApiService = new BomApiService();
            _inventoryApiService = new InventoryApiService();
            UserInfo.ApplyFormPermissions(this, AppResource.BillOfMaterials);
        }

        private async void frmBOM_Load(object sender, EventArgs e)
        {
            await LoadItemsAsync();
            await LoadBomsAsync();
        }

        private async Task LoadItemsAsync()
        {
            try
            {
                _items = await _inventoryApiService.GetLookupAsync("") ?? new List<InventoryItemDto>();

                // Finished item combo
                cmbFinishedItem.DisplayMember = "Title";
                cmbFinishedItem.ValueMember = "Id";
                cmbFinishedItem.DataSource = new List<InventoryItemDto>(_items);

                // Raw item grid column combo
                clnRawItem.DisplayMember = "Title";
                clnRawItem.ValueMember = "Id";
                clnRawItem.DataSource = new List<InventoryItemDto>(_items);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load inventory items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadBomsAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _allBoms = await _bomApiService.GetListAsync() ?? new List<BomDto>();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load BOMs: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ApplyFilter()
        {
            string search = (txtSearch.Text ?? "").Trim().ToLowerInvariant();
            var filtered = _allBoms.Where(b =>
                string.IsNullOrEmpty(search) ||
                (b.BomCode ?? "").ToLowerInvariant().Contains(search) ||
                (b.BomName ?? "").ToLowerInvariant().Contains(search) ||
                (b.FinishedItemName ?? "").ToLowerInvariant().Contains(search)
            ).ToList();

            dgvBomList.DataSource = filtered;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void dgvBomList_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            if (dgvBomList.CurrentRow != null && dgvBomList.CurrentRow.DataBoundItem is BomDto bom)
            {
                PopulateBom(bom);
            }
        }

        private void PopulateBom(BomDto bom)
        {
            _isLoading = true;
            _selectedBomId = bom.Id;
            txtBomCode.Text = bom.BomCode;
            txtBomName.Text = bom.BomName;
            cmbFinishedItem.SelectedValue = bom.FinishedItemId;
            txtOutputQty.Text = bom.OutputQuantity.ToString("0.##");
            txtLaborCost.Text = bom.EstimatedLaborCost.ToString("0.##");
            txtOverheadCost.Text = bom.EstimatedOverheadCost.ToString("0.##");
            chkIsActive.Checked = bom.IsActive;
            txtNotes.Text = bom.Notes;

            dgvRawMaterials.Rows.Clear();
            if (bom.Items != null)
            {
                foreach (var line in bom.Items)
                {
                    int rowIndex = dgvRawMaterials.Rows.Add();
                    var row = dgvRawMaterials.Rows[rowIndex];
                    row.Cells[clnRawItem.Index].Value = line.RawItemId;
                    row.Cells[clnQty.Index].Value = line.Quantity.ToString("0.##");
                    row.Cells[clnStdCost.Index].Value = line.StandardCost.ToString("0.##");
                    row.Cells[clnScrapPct.Index].Value = line.ScrapPercentage.ToString("0.##");
                    row.Cells[clnLineCost.Index].Value = line.LineCost.ToString("0.##");
                    row.Cells[clnDetailNotes.Index].Value = line.Notes;
                }
            }

            _isLoading = false;
            RecalculateTotals();
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            int rowIndex = dgvRawMaterials.Rows.Add();
            var row = dgvRawMaterials.Rows[rowIndex];
            if (_items.Count > 0)
                row.Cells[clnRawItem.Index].Value = _items[0].Id;
            row.Cells[clnQty.Index].Value = "1";
            row.Cells[clnStdCost.Index].Value = "0";
            row.Cells[clnScrapPct.Index].Value = "0";
            row.Cells[clnLineCost.Index].Value = "0.00";
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
                    var curCost = ParseDecimal(row.Cells[clnStdCost.Index].Value);
                    if (curCost == 0)
                        row.Cells[clnStdCost.Index].Value = item.PriRate.ToString("0.##");
                }
            }

            // Calculate line cost
            decimal qty = ParseDecimal(row.Cells[clnQty.Index].Value);
            decimal cost = ParseDecimal(row.Cells[clnStdCost.Index].Value);
            decimal scrap = ParseDecimal(row.Cells[clnScrapPct.Index].Value);
            decimal lineCost = (qty * cost) * (1m + (scrap / 100m));
            row.Cells[clnLineCost.Index].Value = lineCost.ToString("0.##");

            RecalculateTotals();
        }

        private void CalculateCost(object sender, EventArgs e)
        {
            if (!_isLoading) RecalculateTotals();
        }

        private void RecalculateTotals()
        {
            decimal rawMaterialsTotal = 0;
            foreach (DataGridViewRow row in dgvRawMaterials.Rows)
            {
                if (row.IsNewRow) continue;
                rawMaterialsTotal += ParseDecimal(row.Cells[clnLineCost.Index].Value);
            }

            decimal labor = ParseDecimal(txtLaborCost.Text);
            decimal overhead = ParseDecimal(txtOverheadCost.Text);
            decimal total = rawMaterialsTotal + labor + overhead;

            lblTotalEstimatedCost.Text = total.ToString("N2");
        }

        private decimal ParseDecimal(object val)
        {
            if (val == null) return 0;
            decimal d;
            if (decimal.TryParse(val.ToString(), out d)) return d;
            return 0;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            _isLoading = true;
            _selectedBomId = 0;
            txtBomCode.Text = "BOM-" + DateTime.Now.ToString("yyyyMMdd-HHmm");
            txtBomName.Text = "";
            if (_items.Count > 0) cmbFinishedItem.SelectedIndex = 0;
            txtOutputQty.Text = "1";
            txtLaborCost.Text = "0";
            txtOverheadCost.Text = "0";
            chkIsActive.Checked = true;
            txtNotes.Text = "";
            dgvRawMaterials.Rows.Clear();
            _isLoading = false;
            RecalculateTotals();
            txtBomName.Focus();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            string bomName = (txtBomName.Text ?? "").Trim();
            if (string.IsNullOrEmpty(bomName))
            {
                MessageBox.Show("Please enter a Recipe / BOM Name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBomName.Focus();
                return;
            }

            string finishedItemId = cmbFinishedItem.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(finishedItemId))
            {
                MessageBox.Show("Please select an Output / Finished Product.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbFinishedItem.Focus();
                return;
            }

            decimal outputQty = ParseDecimal(txtOutputQty.Text);
            if (outputQty <= 0)
            {
                MessageBox.Show("Output Quantity must be greater than 0.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOutputQty.Focus();
                return;
            }

            if (dgvRawMaterials.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one raw material / ingredient line.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var finishedItem = _items.FirstOrDefault(i => i.Id == finishedItemId);
            var request = new BomCreateUpdateRequest
            {
                BomCode = txtBomCode.Text.Trim(),
                BomName = bomName,
                FinishedItemId = finishedItemId,
                OutputQuantity = outputQty,
                UnitId = finishedItem?.DefaultUnit ?? finishedItem?.PrimaryUnit,
                EstimatedLaborCost = ParseDecimal(txtLaborCost.Text),
                EstimatedOverheadCost = ParseDecimal(txtOverheadCost.Text),
                IsActive = chkIsActive.Checked,
                Notes = txtNotes.Text.Trim(),
                Items = new List<BomDetailCreateUpdateRequest>()
            };

            int seq = 1;
            foreach (DataGridViewRow row in dgvRawMaterials.Rows)
            {
                if (row.IsNewRow) continue;
                string rawId = row.Cells[clnRawItem.Index].Value?.ToString();
                if (string.IsNullOrEmpty(rawId)) continue;

                var rawItem = _items.FirstOrDefault(i => i.Id == rawId);
                string unitId = rawItem?.DefaultUnit ?? rawItem?.PrimaryUnit;

                request.Items.Add(new BomDetailCreateUpdateRequest
                {
                    Seq = seq++,
                    RawItemId = rawId,
                    Quantity = ParseDecimal(row.Cells[clnQty.Index].Value),
                    UnitId = unitId,
                    ScrapPercentage = ParseDecimal(row.Cells[clnScrapPct.Index].Value),
                    Notes = row.Cells[clnDetailNotes.Index].Value?.ToString()
                });
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                if (_selectedBomId == 0)
                {
                    int newId = await _bomApiService.CreateAsync(request);
                    MessageBox.Show("Recipe / BOM created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _selectedBomId = newId;
                }
                else
                {
                    await _bomApiService.UpdateAsync(_selectedBomId, request);
                    MessageBox.Show("Recipe / BOM updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await LoadBomsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save BOM: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedBomId <= 0)
            {
                MessageBox.Show("Please select a BOM to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this Bill of Materials?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    await _bomApiService.DeleteAsync(_selectedBomId);
                    MessageBox.Show("BOM deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnNew_Click(sender, e);
                    await LoadBomsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to delete BOM: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
