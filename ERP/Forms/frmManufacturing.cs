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

            EnableDoubleBuffering(dgvRawMaterials);
            EnableDoubleBuffering(dgvProductionList);
            EnableDoubleBuffering(grpMaster);
            EnableDoubleBuffering(grpSearchFilter);

            dgvRawMaterials.Enter += dgvRawMaterials_Enter;
            dgvRawMaterials.EditingControlShowing += dgvRawMaterials_EditingControlShowing;
            dgvRawMaterials.KeyDown += dgvRawMaterials_KeyDown;
            dgvRawMaterials.DataError += dgvRawMaterials_DataError;
            dgvProductionList.CellDoubleClick += dgvProductionList_CellDoubleClick;
            dgvProductionList.KeyDown += dgvProductionList_KeyDown;
            this.Shown += frmManufacturing_Shown;
            SetupButtonFocusHighlights();
            SetupDropdownFocusHighlights();
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

        private void SetupButtonFocusHighlights()
        {
            var buttons = new Button[] { btnSave, btnNew, btnIssueMaterials, btnComplete, btnCancel, btnDelete, btnClose, btnFilter, btnAddRow, btnDeleteRow };
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
                        using (var pen = new Pen(Color.FromArgb(59, 130, 246), 2))
                        {
                            e.Graphics.DrawRectangle(pen, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3);
                        }
                    }
                };
            }
        }

        private void SetupDropdownFocusHighlights()
        {
            var combos = new (ComboBox Combo, Control Parent)[]
            {
                (cmbBom, grpMaster),
                (cmbFinishedItem, grpMaster),
                (cmbStatusFilter, grpSearchFilter)
            };

            foreach (var item in combos)
            {
                var cmb = item.Combo;
                var parent = item.Parent;
                if (cmb == null) continue;

                cmb.DrawMode = DrawMode.OwnerDrawFixed;
                cmb.ItemHeight = 19;

                cmb.Enter += (s, e) =>
                {
                    cmb.Invalidate();
                    if (parent != null) parent.Invalidate();
                };

                cmb.Leave += (s, e) =>
                {
                    cmb.Invalidate();
                    if (parent != null) parent.Invalidate();
                };

                cmb.DrawItem += (s, e) =>
                {
                    bool isDropDownOpen = cmb.DroppedDown;
                    bool isHovered = isDropDownOpen && ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
                    bool isFocused = cmb.Focused;

                    Color bg;
                    Color fg;

                    if (isHovered)
                    {
                        bg = Color.FromArgb(37, 99, 235); // Blue-600 in list
                        fg = Color.White;
                    }
                    else if (isFocused && !isDropDownOpen)
                    {
                        bg = Color.FromArgb(254, 243, 199); // Warm Amber-100 highlight when focused
                        fg = Color.FromArgb(15, 23, 42);    // Slate-900
                    }
                    else
                    {
                        bg = Color.White;
                        fg = Color.FromArgb(15, 23, 42);
                    }

                    using (var brush = new SolidBrush(bg))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }

                    if (e.Index >= 0 && e.Index < cmb.Items.Count)
                    {
                        string text = cmb.GetItemText(cmb.Items[e.Index]);
                        using (var brush = new SolidBrush(fg))
                        {
                            using (var sf = new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Near,
                                FormatFlags = StringFormatFlags.NoWrap,
                                Trimming = StringTrimming.EllipsisCharacter
                            })
                            {
                                var textRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, Math.Max(0, e.Bounds.Width - 8), e.Bounds.Height);
                                e.Graphics.DrawString(text, e.Font ?? cmb.Font, brush, textRect, sf);
                            }
                        }
                    }

                    if (isFocused && !isDropDownOpen)
                    {
                        using (var pen = new Pen(Color.FromArgb(217, 119, 6), 1.5f)) // Amber-600 border inside
                        {
                            var r = e.Bounds;
                            r.Width -= 1;
                            r.Height -= 1;
                            e.Graphics.DrawRectangle(pen, r);
                        }
                    }
                };
            }

            var headerControls = new Control[] { dtpDate, cmbBom, cmbFinishedItem, txtBatchNo, txtPlannedQty };
            foreach (var ctrl in headerControls)
            {
                if (ctrl == null) continue;
                ctrl.Enter += (s, e) => { if (grpMaster != null) grpMaster.Invalidate(); };
                ctrl.Leave += (s, e) => { if (grpMaster != null) grpMaster.Invalidate(); };
            }

            if (grpMaster != null)
            {
                grpMaster.Paint += (s, e) =>
                {
                    DrawControlFocusHalo(e.Graphics, cmbBom);
                    DrawControlFocusHalo(e.Graphics, cmbFinishedItem);
                    DrawControlFocusHalo(e.Graphics, dtpDate);
                    DrawControlFocusHalo(e.Graphics, txtPlannedQty);
                    DrawControlFocusHalo(e.Graphics, txtBatchNo);
                };
            }

            if (grpSearchFilter != null)
            {
                grpSearchFilter.Paint += (s, e) =>
                {
                    DrawControlFocusHalo(e.Graphics, cmbStatusFilter);
                };
            }
        }

        private void DrawControlFocusHalo(Graphics g, Control ctrl)
        {
            if (ctrl != null && (ctrl.Focused || ctrl.ContainsFocus))
            {
                var r = ctrl.Bounds;
                r.Inflate(2, 2);
                using (var pen = new Pen(Color.FromArgb(37, 99, 235), 2)) // Vibrant Blue-600 halo
                {
                    g.DrawRectangle(pen, r);
                }
            }
        }

        private void frmManufacturing_Shown(object sender, EventArgs e)
        {
            try
            {
                dtpDate.Focus();
            }
            catch { }
        }

        private void FocusGridFirstRow()
        {
            try
            {
                if (tabDetailQuery.SelectedTab != tabpgDetail)
                    tabDetailQuery.SelectedTab = tabpgDetail;

                if (dgvRawMaterials.Rows.Count == 0)
                {
                    btnAddRow_Click(null, null);
                }

                int targetRow = dgvRawMaterials.CurrentCell != null ? dgvRawMaterials.CurrentCell.RowIndex : 0;
                if (targetRow < 0 || targetRow >= dgvRawMaterials.Rows.Count) targetRow = 0;

                dgvRawMaterials.Focus();
                dgvRawMaterials.CurrentCell = dgvRawMaterials[clnRawItem.Index, targetRow];

                BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (dgvRawMaterials.Rows.Count > 0 && clnRawItem != null)
                        {
                            int r = dgvRawMaterials.CurrentCell != null ? dgvRawMaterials.CurrentCell.RowIndex : 0;
                            if (r < 0 || r >= dgvRawMaterials.Rows.Count) r = 0;
                            dgvRawMaterials.CurrentCell = dgvRawMaterials[clnRawItem.Index, r];
                            dgvRawMaterials.BeginEdit(true);
                            if (dgvRawMaterials.EditingControl is ComboBox cb)
                            {
                                cb.Focus();
                            }
                        }
                    }
                    catch { }
                }));
            }
            catch { }
        }

        private void dgvRawMaterials_Enter(object sender, EventArgs e)
        {
            if (IsHandleCreated && dgvRawMaterials.Rows.Count > 0 && clnRawItem != null)
            {
                BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (dgvRawMaterials.Rows.Count > 0 && clnRawItem != null)
                        {
                            int row = dgvRawMaterials.CurrentCell != null ? dgvRawMaterials.CurrentCell.RowIndex : 0;
                            if (row < 0 || row >= dgvRawMaterials.Rows.Count) row = 0;

                            if (dgvRawMaterials.CurrentCell == null)
                            {
                                dgvRawMaterials.CurrentCell = dgvRawMaterials[clnRawItem.Index, row];
                            }

                            if (!dgvRawMaterials.IsCurrentCellInEditMode)
                            {
                                dgvRawMaterials.BeginEdit(true);
                            }

                            if (dgvRawMaterials.EditingControl is ComboBox cb)
                            {
                                cb.Focus();
                            }
                        }
                    }
                    catch { }
                }));
            }
        }

        private void dgvRawMaterials_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvRawMaterials.CurrentCell != null && dgvRawMaterials.CurrentCell.ColumnIndex == clnRawItem.Index)
            {
                ComboBox cb = e.Control as ComboBox;
                if (cb != null)
                {
                    cb.DropDownStyle = ComboBoxStyle.DropDown;
                    cb.AutoCompleteSource = AutoCompleteSource.ListItems;
                    cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                    var cellVal = dgvRawMaterials.CurrentCell.Value;
                    if (cellVal == null || string.IsNullOrWhiteSpace(cellVal.ToString()))
                    {
                        try { cb.SelectedIndex = -1; cb.Text = ""; } catch { }
                    }
                    else
                    {
                        try { cb.SelectedValue = cellVal.ToString(); } catch { }
                    }

                    cb.SelectionChangeCommitted -= cb_SelectionChangeCommitted;
                    cb.SelectionChangeCommitted += cb_SelectionChangeCommitted;
                }
            }
        }

        private void dgvRawMaterials_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dgvRawMaterials_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                HandleGridEnterNavigation();
            }
        }

        private void cb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (dgvRawMaterials.CurrentCell != null && dgvRawMaterials.CurrentCell.ColumnIndex == clnRawItem.Index)
            {
                if (sender is ComboBox cb && cb.SelectedValue != null && cb.SelectedIndex != -1)
                {
                    string itemId = cb.SelectedValue.ToString();
                    dgvRawMaterials.CurrentCell.Value = itemId;
                    UpdateItemDefaults(dgvRawMaterials.CurrentCell.RowIndex, itemId);
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                btnClose.PerformClick();
                return true;
            }

            if (keyData == Keys.Enter && !dgvRawMaterials.ContainsFocus)
            {
                if (dtpDate.Focused || dtpDate.ContainsFocus || (ActiveControl != null && (ActiveControl == dtpDate || ActiveControl.Parent == dtpDate)))
                {
                    cmbBom.Focus();
                    return true;
                }
                if (cmbBom.Focused || cmbBom.ContainsFocus || ActiveControl == cmbBom)
                {
                    cmbFinishedItem.Focus();
                    return true;
                }
                if (cmbFinishedItem.Focused || cmbFinishedItem.ContainsFocus || ActiveControl == cmbFinishedItem)
                {
                    txtPlannedQty.Focus();
                    txtPlannedQty.SelectAll();
                    return true;
                }
                if (txtPlannedQty.Focused || ActiveControl == txtPlannedQty)
                {
                    txtBatchNo.Focus();
                    txtBatchNo.SelectAll();
                    return true;
                }
                if (txtBatchNo.Focused || ActiveControl == txtBatchNo)
                {
                    FocusGridFirstRow();
                    return true;
                }
            }

            Keys key = keyData & Keys.KeyCode;
            Keys modifiers = keyData & Keys.Modifiers;
            if ((key == Keys.Enter || (key == Keys.Tab && modifiers == Keys.None)) && dgvRawMaterials.ContainsFocus)
            {
                if (HandleGridEnterNavigation())
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool HandleGridEnterNavigation()
        {
            if (dgvRawMaterials.CurrentCell == null) return false;

            int rowIndex = dgvRawMaterials.CurrentCell.RowIndex;
            int colIndex = dgvRawMaterials.CurrentCell.ColumnIndex;

            // 1. Raw Item (clnRawItem)
            if (colIndex == clnRawItem.Index)
            {
                if (dgvRawMaterials.EditingControl is ComboBox cb)
                {
                    if (cb.SelectedValue != null && cb.SelectedIndex != -1)
                    {
                        string selId = cb.SelectedValue.ToString();
                        dgvRawMaterials[clnRawItem.Index, rowIndex].Value = selId;
                        UpdateItemDefaults(rowIndex, selId);
                    }
                    else if (!string.IsNullOrWhiteSpace(cb.Text))
                    {
                        string text = cb.Text.Trim();
                        var match = _items.FirstOrDefault(r =>
                            string.Equals(r.Title, text, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(r.Id, text, StringComparison.OrdinalIgnoreCase));
                        if (match == null)
                        {
                            match = _items.FirstOrDefault(r =>
                                r.Title != null && r.Title.StartsWith(text, StringComparison.OrdinalIgnoreCase));
                        }
                        if (match != null)
                        {
                            dgvRawMaterials[clnRawItem.Index, rowIndex].Value = match.Id;
                            UpdateItemDefaults(rowIndex, match.Id);
                        }
                    }
                }

                dgvRawMaterials.EndEdit();

                var itemVal = dgvRawMaterials[clnRawItem.Index, rowIndex].Value;
                bool isItemEmpty = itemVal == null || string.IsNullOrWhiteSpace(itemVal.ToString());
                bool isLastRow = (rowIndex == dgvRawMaterials.Rows.Count - 1);

                if (isLastRow && isItemEmpty)
                {
                    dgvRawMaterials.EndEdit();
                    RecalculateTotals();
                    btnSave.Focus();
                    return true;
                }

                if (!isItemEmpty)
                {
                    dgvRawMaterials.CurrentCell = dgvRawMaterials[clnPlannedQty.Index, rowIndex];
                    dgvRawMaterials.BeginEdit(true);
                    return true;
                }

                if (rowIndex < dgvRawMaterials.Rows.Count - 1)
                {
                    dgvRawMaterials.CurrentCell = dgvRawMaterials[clnRawItem.Index, rowIndex + 1];
                    dgvRawMaterials.BeginEdit(true);
                    return true;
                }

                btnSave.Focus();
                return true;
            }

            // 2. Planned Qty (clnPlannedQty)
            if (colIndex == clnPlannedQty.Index)
            {
                dgvRawMaterials.EndEdit();
                dgvRawMaterials.CurrentCell = dgvRawMaterials[clnActualQty.Index, rowIndex];
                dgvRawMaterials.BeginEdit(true);
                return true;
            }

            // 3. Actual Qty (clnActualQty)
            if (colIndex == clnActualQty.Index)
            {
                dgvRawMaterials.EndEdit();
                RecalculateTotals();
                dgvRawMaterials.CurrentCell = dgvRawMaterials[clnWastageQty.Index, rowIndex];
                dgvRawMaterials.BeginEdit(true);
                return true;
            }

            // 4. Wastage Qty (clnWastageQty)
            if (colIndex == clnWastageQty.Index)
            {
                dgvRawMaterials.EndEdit();
                dgvRawMaterials.CurrentCell = dgvRawMaterials[clnUnitCost.Index, rowIndex];
                dgvRawMaterials.BeginEdit(true);
                return true;
            }

            // 5. Unit Cost (clnUnitCost)
            if (colIndex == clnUnitCost.Index)
            {
                dgvRawMaterials.EndEdit();
                RecalculateTotals();
                // Skip read-only clnTotalCost and go to clnDetailNotes
                dgvRawMaterials.CurrentCell = dgvRawMaterials[clnDetailNotes.Index, rowIndex];
                dgvRawMaterials.BeginEdit(true);
                return true;
            }

            // 6. Notes / Batch Details (clnDetailNotes)
            if (colIndex == clnDetailNotes.Index)
            {
                dgvRawMaterials.EndEdit();
                RecalculateTotals();

                // If on the last row and item is null/empty -> exit to Save button
                var itemVal = dgvRawMaterials[clnRawItem.Index, rowIndex].Value;
                bool isItemEmpty = itemVal == null || string.IsNullOrWhiteSpace(itemVal.ToString());
                if (rowIndex == dgvRawMaterials.Rows.Count - 1 && isItemEmpty)
                {
                    RecalculateTotals();
                    btnSave.Focus();
                    return true;
                }

                int nextRowIdx;
                if (rowIndex == dgvRawMaterials.Rows.Count - 1)
                {
                    btnAddRow_Click(null, null);
                    nextRowIdx = dgvRawMaterials.Rows.Count - 1;
                }
                else
                {
                    nextRowIdx = rowIndex + 1;
                }

                dgvRawMaterials.CurrentCell = dgvRawMaterials[clnRawItem.Index, nextRowIdx];
                dgvRawMaterials.BeginEdit(true);
                if (dgvRawMaterials.EditingControl is ComboBox cb)
                {
                    cb.Focus();
                }
                return true;
            }

            // Fallback
            dgvRawMaterials.EndEdit();
            RecalculateTotals();
            int targetIdx;
            if (rowIndex < dgvRawMaterials.Rows.Count - 1)
            {
                targetIdx = rowIndex + 1;
            }
            else
            {
                btnAddRow_Click(null, null);
                targetIdx = dgvRawMaterials.Rows.Count - 1;
            }
            dgvRawMaterials.CurrentCell = dgvRawMaterials[clnRawItem.Index, targetIdx];
            dgvRawMaterials.BeginEdit(true);
            if (dgvRawMaterials.EditingControl is ComboBox cbFallback)
            {
                cbFallback.Focus();
            }
            return true;
        }

        private async void frmManufacturing_Load(object sender, EventArgs e)
        {
            _isLoading = true;
            dtpFrom.Value = DateTime.Today.AddDays(-30);
            dtpTo.Value = DateTime.Today;

            await LoadLookupsAsync();
            await LoadVouchersAsync();
            _isLoading = false;
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

        private ERP.Controls.PaginationControl _pager;
        private string _qFrom = "", _qTo = "", _qStatus = "", _qSearch = "";
        private int _qPageSize = 20;

        private void EnsurePager()
        {
            if (_pager != null) return;
            _pager = new ERP.Controls.PaginationControl();
            _pager.PageChanged += async (s, e) =>
            {
                try
                {
                    await LoadVouchersAsync(e.PageNumber, e.PageSize);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading vouchers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            _pager.Attach(tabpgQuery, dgvProductionList);
        }

        private async Task LoadVouchersAsync(int pageNumber = 1, int pageSize = 0)
        {
            try
            {
                EnsurePager();
                Cursor = Cursors.WaitCursor;
                string status = cmbStatusFilter.SelectedItem?.ToString() ?? "All";
                if (status == "All") status = "";

                _qFrom = dtpFrom.Value.ToString("yyyy-MM-dd");
                _qTo = dtpTo.Value.ToString("yyyy-MM-dd");
                _qStatus = status;
                _qSearch = (txtSearch.Text ?? "").Trim();
                if (pageSize > 0) _qPageSize = pageSize;

                var page = await _manufacturingApiService.GetPagedListAsync(
                    fromDate: _qFrom,
                    toDate: _qTo,
                    status: _qStatus,
                    voucherNo: _qSearch,
                    pageNumber: pageNumber,
                    pageSize: _qPageSize
                );

                _allVouchers = page.Data;
                _pager.SetPagination(page.CurrentPage, page.TotalPages, page.TotalCount, page.PageSize);
                _qPageSize = page.PageSize;

                dgvProductionList.DataSource = _allVouchers;
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
            await LoadVouchersAsync(1, _qPageSize);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private async void dgvProductionList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvProductionList.Rows[e.RowIndex].DataBoundItem is ProductionMasterDto item)
            {
                tabDetailQuery.SelectedTab = tabpgDetail;
                await LoadVoucherDetailAsync(item.VoucherNo);
            }
        }

        private async void dgvProductionList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dgvProductionList.CurrentRow != null && dgvProductionList.CurrentRow.DataBoundItem is ProductionMasterDto item)
            {
                e.Handled = true;
                tabDetailQuery.SelectedTab = tabpgDetail;
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
                    lblStatusDisplay.ForeColor = Color.FromArgb(21, 128, 61);
                    break;
                case "InProgress":
                    lblStatusDisplay.ForeColor = Color.FromArgb(217, 119, 6);
                    break;
                case "Cancelled":
                    lblStatusDisplay.ForeColor = Color.FromArgb(220, 38, 38);
                    break;
                default:
                    lblStatusDisplay.ForeColor = Color.FromArgb(30, 64, 175);
                    break;
            }
        }

        private void UpdateActionButtonsState()
        {
            bool isCompletedOrCancelled = !string.IsNullOrEmpty(_selectedVoucherNo) && (_currentStatus == "Completed" || _currentStatus == "Cancelled");
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

        private void UpdateItemDefaults(int rowIndex, string itemId)
        {
            if (rowIndex < 0 || rowIndex >= dgvRawMaterials.Rows.Count) return;
            var row = dgvRawMaterials.Rows[rowIndex];
            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                var curCost = ParseDecimal(row.Cells[clnUnitCost.Index].Value);
                if (curCost == 0)
                    row.Cells[clnUnitCost.Index].Value = item.PriRate.ToString("0.##");

                decimal actualQty = ParseDecimal(row.Cells[clnActualQty.Index].Value);
                if (actualQty <= 0)
                {
                    actualQty = 1;
                    row.Cells[clnActualQty.Index].Value = "1";
                }
                decimal plannedQty = ParseDecimal(row.Cells[clnPlannedQty.Index].Value);
                if (plannedQty <= 0)
                {
                    row.Cells[clnPlannedQty.Index].Value = "1";
                }
                decimal cost = ParseDecimal(row.Cells[clnUnitCost.Index].Value);
                row.Cells[clnTotalCost.Index].Value = (actualQty * cost).ToString("0.##");
                RecalculateTotals();
            }
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            int rowIndex = dgvRawMaterials.Rows.Add();
            var row = dgvRawMaterials.Rows[rowIndex];
            row.Cells[clnRawItem.Index].Value = null;
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
            dgvRawMaterials.ReadOnly = false;
            dgvRawMaterials.Rows.Clear();
            btnAddRow_Click(null, null);
            _isLoading = false;
            RecalculateTotals();
            UpdateActionButtonsState();
            dtpDate.Focus();
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
                btnNew.Focus();
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

        private void frmManufacturing_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
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
        }
    }
}
