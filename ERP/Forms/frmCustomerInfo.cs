using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmCustomerInfo : Form
    {
        // ── Data / state fields (UI controls live in Designer.cs) ─────────
        bool Flogin = true;
        private readonly CustomerApiService _customerApiService;
        private readonly InventoryApiService _inventoryApiService;
        private readonly CustomerCategoryApiService _customerCategoryApiService;
        private List<CustomerDto> _customers;
        private bool _isSaving;
        private bool _isDeleting;
        private bool _isNewMode = false;

        public frmCustomerInfo()
        {
            InitializeComponent();
            _customerApiService = new CustomerApiService();
            chkIsSubAccount.CheckedChanged += (s, e) =>
            {
                cmbParentCustomer.Enabled = chkIsSubAccount.Checked;
                if (!chkIsSubAccount.Checked)
                    cmbParentCustomer.SelectedIndex = -1;
            };
            _inventoryApiService = new InventoryApiService();
            _customerCategoryApiService = new CustomerCategoryApiService();
            _customers = new List<CustomerDto>();
            UserInfo.ApplyFormPermissions(this, AppResource.Customers);
            SetupEntryFlow();
        }

        // ══════════════════════════════════════════════════════════════════
        //  State
        // ══════════════════════════════════════════════════════════════════

        private void SetNewMode(bool isNew)
        {
            _isNewMode = isNew;
            if (isNew)
            {
                btnNew.Text = "Cancel";
                txtAccountCode.Text = "(Auto-Generated)";
                ClearDetail();
                txtTitle.Focus();
                btnDelete.Enabled = false;
            }
            else
            {
                btnNew.Text = "&New";
                btnDelete.Enabled = (cmbCustomerCode.SelectedValue != null);
                string code = cmbCustomerCode.SelectedValue?.ToString();
                if (!string.IsNullOrEmpty(code))
                    FillDetail(code);
            }
        }

        private void ClearDetail()
        {
            txtTitle.Text = txtEmail.Text = txtFax.Text = txtNic.Text = string.Empty;
            txtAddress.Text = txtQualification.Text = txtPhone1.Text = txtPhone2.Text = string.Empty;
            txtSMSNumber.Text = txtIBAN.Text = string.Empty;
            lblCreatedBy.Text = lblEditBy.Text = "-";
            chkActive.Checked = true;
            chkSMSAlert.Checked = chkEmailAlert.Checked = false;
            if (cmbCustomerCategory != null && cmbCustomerCategory.DataSource != null)
                cmbCustomerCategory.SelectedValue = string.Empty;
            chkIsSubAccount.Checked = false;
            cmbParentCustomer.Enabled = false;
            PopulateParentCustomersCombo(string.Empty);
            cmbParentCustomer.SelectedIndex = -1;
            profileImage1?.ClearImage();
            dgvSupplyItems?.Rows.Clear();
            if (lblSupplyCustomerInfo != null)
                lblSupplyCustomerInfo.Text = "Customer: (New Customer)";
        }

        // ══════════════════════════════════════════════════════════════════
        //  Data helpers
        // ══════════════════════════════════════════════════════════════════

        private ERP.Controls.PaginationControl _pager;
        private List<CustomerDto> _pagedCustomers;
        private System.Windows.Forms.Timer _searchTimer;

        private void EnsurePager()
        {
            if (_pager != null) return;
            _pager = new ERP.Controls.PaginationControl();
            _pager.PageChanged += async (s, e) =>
            {
                await LoadCustomerPageAsync(e.PageNumber, e.PageSize);
            };
            _pager.Attach(tpList, dgvCustomers);
        }

        private async System.Threading.Tasks.Task LoadCustomerPageAsync(int pageNumber, int pageSize)
        {
            try
            {
                string search = txtSearch?.Text?.Trim();
                var result = await _customerApiService.GetPagedListAsync(pageNumber, pageSize, search: search);
                _pagedCustomers = result.Data;
                PopulateGrid(_pagedCustomers);
                _pager?.SetPagination(result.CurrentPage, result.TotalPages, result.TotalCount, result.PageSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task FillCustomersAsync()
        {
            _customers = await _customerApiService.GetAsync();

            cmbCustomerCode.DataSource    = null;
            cmbCustomerCode.DataSource    = _customers;
            cmbCustomerCode.DisplayMember = "Title";
            cmbCustomerCode.ValueMember   = "Account";

            EnsurePager();
            await LoadCustomerPageAsync(1, _pager?.PageSize ?? 20);
        }

        private void PopulateGrid(List<CustomerDto> list)
        {
            dgvCustomers.Rows.Clear();
            if (list == null) return;
            foreach (var c in list)
            {
                string titleDisplay = c.IsSubAccount && !string.IsNullOrWhiteSpace(c.ParentCustomerTitle)
                    ? $"{c.Title} (Sub of: {c.ParentCustomerTitle})"
                    : c.Title;
                dgvCustomers.Rows.Add(c.Account, titleDisplay, c.Phone1,
                                      c.Email, c.Address, c.CustomerCategoryTitle ?? "-", c.Active ? "Yes" : "No");
            }
        }

        private async void FillDetail(string code)
        {
            if (Flogin || string.IsNullOrWhiteSpace(code)) return;

            var c = _customers?.FirstOrDefault(x =>
                string.Equals(x.Account, code, StringComparison.OrdinalIgnoreCase))
                ?? _pagedCustomers?.FirstOrDefault(x =>
                string.Equals(x.Account, code, StringComparison.OrdinalIgnoreCase));

            if (c == null) { ClearDetail(); txtAccountCode.Text = string.Empty; return; }

            txtAccountCode.Text   = code;
            txtTitle.Text         = c.Title         ?? string.Empty;
            txtEmail.Text         = c.Email         ?? string.Empty;
            txtFax.Text           = c.Fax           ?? string.Empty;
            txtNic.Text           = c.Cnic          ?? string.Empty;
            txtAddress.Text       = c.Address       ?? string.Empty;
            txtQualification.Text = c.Qualification ?? string.Empty;
            txtPhone1.Text        = c.Phone1        ?? string.Empty;
            txtPhone2.Text        = c.Phone2        ?? string.Empty;
            txtSMSNumber.Text     = c.SmsNumber     ?? string.Empty;
            txtIBAN.Text          = c.Iban          ?? string.Empty;
            chkSMSAlert.Checked   = c.SmsAlert;
            chkEmailAlert.Checked = c.EmailAlert;
            chkActive.Checked     = c.Active;
            if (cmbCustomerCategory != null && cmbCustomerCategory.DataSource != null)
                cmbCustomerCategory.SelectedValue = c.CustomerCategoryId ?? string.Empty;
            PopulateParentCustomersCombo(code);
            chkIsSubAccount.Checked = c.IsSubAccount;
            cmbParentCustomer.Enabled = c.IsSubAccount;
            if (!string.IsNullOrEmpty(c.ParentCustomerAccountId))
                cmbParentCustomer.SelectedValue = c.ParentCustomerAccountId;
            else
                cmbParentCustomer.SelectedIndex = -1;

            lblCreatedBy.Text = FormatAudit(c.CreatedBy,       c.CreatedOn);
            lblEditBy.Text    = FormatAudit(c.LastModifiedBy,  c.LastModifiedOn);

            if (profileImage1 != null)
            {
                profileImage1.MediaId = c.MediaId;
                profileImage1.MediaUrl = c.MediaUrl;
                await profileImage1.LoadImageAsync(c.MediaUrl);
            }

            dgvSupplyItems?.Rows.Clear();
            if (lblSupplyCustomerInfo != null)
                lblSupplyCustomerInfo.Text = $"Customer: {c.Title} ({code})";

            var supplyList = c.SupplyItems;
            if (supplyList == null || supplyList.Count == 0)
            {
                try
                {
                    supplyList = await _customerApiService.GetSupplyItemsAsync(code);
                }
                catch
                {
                    supplyList = new List<CustomerSupplyItemDto>();
                }
            }

            if (supplyList != null && dgvSupplyItems != null)
            {
                foreach (var item in supplyList)
                {
                    int rowIdx = dgvSupplyItems.Rows.Add();
                    var r = dgvSupplyItems.Rows[rowIdx];
                    r.Cells[clnSupplyItemId.Index].Value = item.ItemId;
                    r.Cells[clnSupplyQty.Index].Value = (item.Qty > 0 ? item.Qty : 1).ToString("0.##");
                    if (clnSupplySecQty.Visible)
                        r.Cells[clnSupplySecQty.Index].Value = (item.SecQty ?? 0).ToString("0.##");
                    r.Cells[clnSupplyRate.Index].Value = item.Rate.HasValue ? item.Rate.Value.ToString("0.##") : string.Empty;
                    r.Cells[clnSupplyDiscount.Index].Value = item.Discount.HasValue ? item.Discount.Value.ToString("0.##") : string.Empty;
                    r.Cells[clnSupplyCarriage.Index].Value = item.Carriage.HasValue ? item.Carriage.Value.ToString("0.##") : string.Empty;
                    r.Cells[clnSupplyAddLess.Index].Value = item.AddLess.HasValue ? item.AddLess.Value.ToString("0.##") : string.Empty;
                }
            }
        }

        private static string FormatAudit(string by, DateTime? on) =>
            string.IsNullOrWhiteSpace(by) || !on.HasValue
                ? "-"
                : by + "  |  " + on.Value.ToString("dd-MMM-yyyy hh:mm tt");

        private CustomerUpsertApiRequest BuildRequest()
        {
            var supplyItems = new List<CustomerSupplyItemDto>();
            if (dgvSupplyItems != null)
            {
                foreach (DataGridViewRow r in dgvSupplyItems.Rows)
                {
                    if (r.IsNewRow) continue;
                    var itemIdObj = r.Cells[clnSupplyItemId.Index].Value;
                    if (itemIdObj == null || string.IsNullOrWhiteSpace(itemIdObj.ToString())) continue;

                    decimal.TryParse(Convert.ToString(r.Cells[clnSupplyQty.Index].Value), out decimal qty);
                    decimal? secQty = null;
                    if (decimal.TryParse(Convert.ToString(r.Cells[clnSupplySecQty.Index].Value), out decimal sqVal))
                        secQty = sqVal;

                    decimal? rate = decimal.TryParse(Convert.ToString(r.Cells[clnSupplyRate.Index].Value), out decimal rVal) ? (decimal?)rVal : null;
                    decimal? discount = decimal.TryParse(Convert.ToString(r.Cells[clnSupplyDiscount.Index].Value), out decimal dVal) ? (decimal?)dVal : null;
                    decimal? carriage = decimal.TryParse(Convert.ToString(r.Cells[clnSupplyCarriage.Index].Value), out decimal cVal) ? (decimal?)cVal : null;
                    decimal? addLess = decimal.TryParse(Convert.ToString(r.Cells[clnSupplyAddLess.Index].Value), out decimal aVal) ? (decimal?)aVal : null;

                    supplyItems.Add(new CustomerSupplyItemDto
                    {
                        ItemId = itemIdObj.ToString(),
                        Qty = qty > 0 ? qty : 1,
                        SecQty = secQty,
                        Rate = rate,
                        Discount = discount,
                        Carriage = carriage,
                        AddLess = addLess
                    });
                }
            }

            return new CustomerUpsertApiRequest
            {
                Title         = txtTitle.Text,
                Email         = txtEmail.Text,
                Fax           = txtFax.Text,
                Cnic          = txtNic.Text,
                Address       = txtAddress.Text,
                Qualification = txtQualification.Text,
                Phone1        = txtPhone1.Text,
                Phone2        = txtPhone2.Text,
                SmsNumber     = txtSMSNumber.Text,
                Iban          = txtIBAN.Text,
                SmsAlert      = chkSMSAlert.Checked,
                EmailAlert    = chkEmailAlert.Checked,
                Active        = chkActive.Checked,
                MediaId       = profileImage1?.MediaId,
                CustomerCategoryId = cmbCustomerCategory != null && cmbCustomerCategory.SelectedValue != null && !string.IsNullOrWhiteSpace(cmbCustomerCategory.SelectedValue.ToString())
                    ? cmbCustomerCategory.SelectedValue.ToString()
                    : null,
                ParentCustomerAccountId = chkIsSubAccount.Checked && cmbParentCustomer.SelectedValue != null && !string.IsNullOrWhiteSpace(cmbParentCustomer.SelectedValue.ToString())
                    ? cmbParentCustomer.SelectedValue.ToString()
                    : null,
                SupplyItems   = supplyItems
            };
        }

        // ══════════════════════════════════════════════════════════════════
        //  List-tab events
        // ══════════════════════════════════════════════════════════════════

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (_searchTimer == null)
            {
                _searchTimer = new System.Windows.Forms.Timer { Interval = 350 };
                _searchTimer.Tick += async (s, ev) =>
                {
                    _searchTimer.Stop();
                    await LoadCustomerPageAsync(1, _pager?.PageSize ?? 20);
                };
            }
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private static bool Contains(string s, string q) =>
            !string.IsNullOrEmpty(s) && s.ToLower().Contains(q);

        private void DgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvCustomers.Rows.Count) return;
            string code = (dgvCustomers.Columns.Contains("Account")
                ? dgvCustomers.Rows[e.RowIndex].Cells["Account"].Value
                : dgvCustomers.Rows[e.RowIndex].Cells[0].Value)?.ToString();
            if (string.IsNullOrEmpty(code)) return;

            _isNewMode = false;
            btnNew.Text = "&New";
            cmbCustomerCode.SelectedValue = code;
            txtAccountCode.Text = code;
            FillDetail(code);
            tabControl.SelectedTab = tpDetails;
            btnDelete.Enabled = true;
        }

        private void DgvCustomers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dgvCustomers.CurrentRow != null)
            {
                e.Handled = true;
                DgvCustomers_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, dgvCustomers.CurrentRow.Index));
            }
        }

        private void BtnAddNew_Click(object sender, EventArgs e)
        {
            SetNewMode(true);
            tabControl.SelectedTab = tpDetails;
        }

        private async void BtnRefreshList_Click(object sender, EventArgs e)
        {
            try   { await FillCustomersAsync(); }
            catch (Exception ex) { ShowError(ex); }
        }

        // ══════════════════════════════════════════════════════════════════
        //  Detail-tab button events
        // ══════════════════════════════════════════════════════════════════

        private void btnNew_Click(object sender, EventArgs e) => SetNewMode(!_isNewMode);

        private void btnCancel_Click(object sender, EventArgs e) => this.Close();

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving) return;
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Customer Name is required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Save changes?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            _isSaving = true;
            btnSave.Enabled = false;
            try
            {
                if (_isNewMode)
                {
                    string newCode = await _customerApiService.CreateAsync(BuildRequest());
                    MessageBox.Show("Saved successfully!\nCode: " + newCode, "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await FillCustomersAsync();
                    _isNewMode = false;
                    btnNew.Text = "&New";
                    cmbCustomerCode.SelectedValue = newCode;
                    FillDetail(newCode);
                    btnDelete.Enabled = true;
                }
                else
                {
                    string code = cmbCustomerCode.SelectedValue?.ToString();
                    if (string.IsNullOrEmpty(code)) return;
                    await _customerApiService.UpsertAsync(code, BuildRequest());
                    MessageBox.Show("Saved successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await FillCustomersAsync();
                    cmbCustomerCode.SelectedValue = code;
                    FillDetail(code);
                }
            }
            catch (Exception ex) { ShowError(ex); }
            finally { _isSaving = false; btnSave.Enabled = true; }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_isDeleting || _isNewMode) return;
            string code = cmbCustomerCode.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("No customer selected.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(
                    $"Delete customer '{txtTitle.Text}' ({code})?\nThis cannot be undone.",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes) return;

            _isDeleting = true;
            btnDelete.Enabled = false;
            try
            {
                await _customerApiService.DeleteAsync(code);
                MessageBox.Show("Deleted successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await FillCustomersAsync();
                ClearDetail();
                txtAccountCode.Text = string.Empty;
                tabControl.SelectedTab = tpList;
            }
            catch (Exception ex) { ShowError(ex); }
            finally { _isDeleting = false; btnDelete.Enabled = false; }
        }

        // ══════════════════════════════════════════════════════════════════
        //  Form events
        // ══════════════════════════════════════════════════════════════════

        private async void frmCustomerInfo_Load(object sender, EventArgs e)
        {
            try
            {
                profileImage1.SearchButton.Click += btnCustomerSearch_Click;
                profileImage1.CancelButton.Click += btnCustomerCancel_Click;
                dgvSupplyItems.DataError += dgvSupplyItems_DataError;

                var items = await _inventoryApiService.GetLookupAsync(null);
                _supplyItems = items ?? new List<InventoryItemDto>();
                clnSupplyItemId.DataSource = items;
                clnSupplyItemId.DisplayMember = "Title";
                clnSupplyItemId.ValueMember = "Id";
                clnSupplySecQty.Visible = ApiSession.HasSecondaryQty;
                clnSupplyCarriage.Visible = ApiSession.EnableCarriage;

                try
                {
                    var categories = await _customerCategoryApiService.GetLookupAsync();
                    var catList = new List<CustomerCategoryLookupDto>
                    {
                        new CustomerCategoryLookupDto { Code = string.Empty, Title = "-- None --" }
                    };
                    if (categories != null)
                        catList.AddRange(categories);

                    cmbCustomerCategory.DataSource = catList;
                    cmbCustomerCategory.DisplayMember = "Title";
                    cmbCustomerCategory.ValueMember = "Code";
                }
                catch { }

                await FillCustomersAsync();
                Flogin = false;
                string code = cmbCustomerCode.SelectedValue?.ToString();
                if (!string.IsNullOrEmpty(code))
                {
                    txtAccountCode.Text = code;
                    FillDetail(code);
                }
                btnDelete.Enabled = !string.IsNullOrEmpty(code);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private List<InventoryItemDto> _supplyItems = new List<InventoryItemDto>();

        // ══════════════════════════════════════════════════════════════════
        //  Data-entry flow (same as Sale / Purchase): Enter = next field,
        //  grid cells advance on Enter, empty last row -> Save (row kept)
        // ══════════════════════════════════════════════════════════════════

        private Control[] _enterOrder;

        private void SetupEntryFlow()
        {
            _enterOrder = new Control[]
            {
                txtTitle, txtEmail, txtPhone1, txtPhone2, txtFax, txtNic, txtAddress,
                txtQualification, txtSMSNumber, txtIBAN, chkSMSAlert, chkEmailAlert,
                cmbCustomerCategory, chkIsSubAccount, cmbParentCustomer, chkActive, dgvSupplyItems
            };

            // Tab order = Enter order (details tab)
            int idx = 1;
            foreach (var c in _enterOrder)
            {
                if (c == chkSMSAlert || c == chkEmailAlert) continue; // children of groupBox1
                if (c == btnSave) continue;                           // child of panel2
                if (c == dgvSupplyItems)
                {
                    grpSupplyDefaults.TabStop = false; grpSupplyDefaults.TabIndex = idx++;
                }
                c.TabIndex = idx++;
                c.TabStop = true;
                if (c == txtIBAN) groupBox1.TabIndex = idx++;
            }
            chkSMSAlert.TabIndex = 0;
            chkEmailAlert.TabIndex = 1;
            panel2.TabIndex = idx++;
            btnSave.TabIndex = 0; btnNew.TabIndex = 1; btnDelete.TabIndex = 2; btnClose.TabIndex = 3;

            // Visible focus indication
            foreach (var tb in new TextBox[] { txtTitle, txtEmail, txtPhone1, txtPhone2, txtFax, txtNic,
                                               txtAddress, txtQualification, txtSMSNumber, txtIBAN })
            {
                var box = tb;
                box.Enter += (s, e) => box.BackColor = Color.FromArgb(254, 243, 199);
                box.Leave += (s, e) => box.BackColor = Color.White;
            }

            foreach (var cmb in new ComboBox[] { cmbCustomerCategory, cmbParentCustomer })
            {
                var c = cmb;
                c.DrawMode = DrawMode.OwnerDrawFixed;
                c.ItemHeight = 19;
                c.Enter += (s, e) => c.Invalidate();
                c.Leave += (s, e) => c.Invalidate();
                c.DrawItem += (s, e) =>
                {
                    bool open = c.DroppedDown;
                    bool hot = (e.State & DrawItemState.Selected) == DrawItemState.Selected && open;
                    Color bg = hot ? Color.FromArgb(37, 99, 235)
                             : (c.Focused && !open) ? Color.FromArgb(254, 243, 199) : Color.White;
                    Color fg = hot ? Color.White : Color.FromArgb(15, 23, 42);
                    using (var b = new SolidBrush(bg)) e.Graphics.FillRectangle(b, e.Bounds);
                    if (e.Index >= 0 && e.Index < c.Items.Count)
                        TextRenderer.DrawText(e.Graphics, c.GetItemText(c.Items[e.Index]), c.Font,
                            new Rectangle(e.Bounds.X + 2, e.Bounds.Y, e.Bounds.Width - 4, e.Bounds.Height), fg,
                            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                    if (c.Focused && !open)
                        using (var p = new Pen(Color.FromArgb(217, 119, 6), 1.5f))
                            e.Graphics.DrawRectangle(p, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                };
            }

            foreach (var chk in new CheckBox[] { chkActive, chkIsSubAccount, chkSMSAlert, chkEmailAlert })
            {
                var k = chk;
                k.Enter += (s, e) => k.BackColor = Color.FromArgb(254, 243, 199);
                k.Leave += (s, e) => k.BackColor = Color.Transparent;
            }

            foreach (var btn in new Button[] { btnNew, btnSave, btnDelete, btnClose, btnAddSupplyItem, btnDeleteSupplyItem })
            {
                var b = btn;
                b.Enter += (s, e) => b.Invalidate();
                b.Leave += (s, e) => b.Invalidate();
                b.Paint += (s, e) =>
                {
                    if (!b.Focused) return;
                    using (var p = new Pen(Color.FromArgb(59, 130, 246), 2))
                        e.Graphics.DrawRectangle(p, 1, 1, b.Width - 3, b.Height - 3);
                };
            }

            dgvSupplyItems.EditingControlShowing += dgvSupplyItems_EditingControlShowing;
            dgvCustomers.KeyDown += DgvCustomers_KeyDown;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            if (keyData == key && (key == Keys.Enter || key == Keys.Tab) && tabControl.SelectedTab == tpDetails)
            {
                if (dgvSupplyItems.ContainsFocus)
                {
                    if (HandleSupplyGridNavigation()) return true;
                }
                else if (key == Keys.Enter && ActiveControl is Button == false)
                {
                    int i = Array.IndexOf(_enterOrder, ActiveControl);
                    if (i >= 0)
                    {
                        for (int n = i + 1; n < _enterOrder.Length; n++)
                        {
                            var next = _enterOrder[n];
                            if (!next.Enabled || !next.Visible) continue;
                            if (next == dgvSupplyItems) { FocusSupplyGrid(); return true; }
                            next.Focus();
                            if (next is TextBox t) t.SelectAll();
                            return true;
                        }
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void dgvSupplyItems_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvSupplyItems.CurrentCell == null || dgvSupplyItems.CurrentCell.ColumnIndex != clnSupplyItemId.Index) return;
            if (e.Control is ComboBox cb)
            {
                cb.DropDownStyle = ComboBoxStyle.DropDown;
                cb.AutoCompleteSource = AutoCompleteSource.ListItems;
                cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                var v = dgvSupplyItems.CurrentCell.Value;
                if (v == null || string.IsNullOrWhiteSpace(v.ToString()))
                {
                    try { cb.SelectedIndex = -1; cb.Text = ""; } catch { }
                }
                else
                {
                    try { cb.SelectedValue = v.ToString(); } catch { }
                }
            }
        }

        private bool HandleSupplyGridNavigation()
        {
            var cell = dgvSupplyItems.CurrentCell;
            if (cell == null) return false;
            int row = cell.RowIndex, col = cell.ColumnIndex;

            if (col == clnSupplyItemId.Index)
            {
                if (dgvSupplyItems.EditingControl is ComboBox cb)
                {
                    InventoryItemDto match = null;
                    if (cb.SelectedValue != null && cb.SelectedIndex != -1)
                        match = _supplyItems.FirstOrDefault(i => i.Id == cb.SelectedValue.ToString());
                    else if (!string.IsNullOrWhiteSpace(cb.Text))
                    {
                        string t = cb.Text.Trim();
                        match = _supplyItems.FirstOrDefault(i => string.Equals(i.Title, t, StringComparison.OrdinalIgnoreCase)
                                                              || string.Equals(i.Id, t, StringComparison.OrdinalIgnoreCase))
                             ?? _supplyItems.FirstOrDefault(i => i.Title != null && i.Title.StartsWith(t, StringComparison.OrdinalIgnoreCase));
                    }
                    if (match != null) dgvSupplyItems[clnSupplyItemId.Index, row].Value = match.Id;
                }
                dgvSupplyItems.EndEdit();

                var val = dgvSupplyItems[clnSupplyItemId.Index, row].Value;
                bool empty = val == null || string.IsNullOrWhiteSpace(val.ToString());
                if (empty && row == dgvSupplyItems.Rows.Count - 1)
                {
                    btnSave.Focus();   // last row kept as-is, focus leaves grid
                    return true;
                }
                if (empty) { MoveSupplyTo(row + 1, clnSupplyItemId.Index); return true; }
            }
            else
            {
                dgvSupplyItems.EndEdit();
            }

            // next visible, editable column in display order
            var cols = dgvSupplyItems.Columns.Cast<DataGridViewColumn>()
                       .Where(c => c.Visible && !c.ReadOnly).OrderBy(c => c.DisplayIndex).ToList();
            int pos = cols.FindIndex(c => c.Index == col);
            if (pos >= 0 && pos < cols.Count - 1)
            {
                MoveSupplyTo(row, cols[pos + 1].Index);
                return true;
            }

            // end of row -> next row's item cell
            if (row == dgvSupplyItems.Rows.Count - 1) btnAddSupplyItem_Click(null, null);
            MoveSupplyTo(row + 1, clnSupplyItemId.Index);
            return true;
        }

        private void FocusSupplyGrid()
        {
            if (dgvSupplyItems.Rows.Count == 0) btnAddSupplyItem_Click(null, null);
            dgvSupplyItems.Focus();
            MoveSupplyTo(0, clnSupplyItemId.Index);
        }

        private void MoveSupplyTo(int row, int col)
        {
            if (row < 0 || row >= dgvSupplyItems.Rows.Count) return;
            dgvSupplyItems.CurrentCell = dgvSupplyItems[col, row];
            dgvSupplyItems.BeginEdit(true);
        }

        private void dgvSupplyItems_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void btnAddSupplyItem_Click(object sender, EventArgs e)
        {
            int idx = dgvSupplyItems.Rows.Add();
            dgvSupplyItems.Rows[idx].Cells[clnSupplyQty.Index].Value = "1";
            if (clnSupplySecQty.Visible)
                dgvSupplyItems.Rows[idx].Cells[clnSupplySecQty.Index].Value = "0";
        }

        private void btnDeleteSupplyItem_Click(object sender, EventArgs e)
        {
            if (dgvSupplyItems.CurrentRow != null && !dgvSupplyItems.CurrentRow.IsNewRow)
            {
                dgvSupplyItems.Rows.Remove(dgvSupplyItems.CurrentRow);
            }
        }

        private void frmCustomerInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5 && btnSave.Enabled) { e.Handled = true; btnSave.PerformClick(); }
            else if (e.KeyCode == Keys.F3 && btnNew.Enabled) { e.Handled = true; btnNew.PerformClick(); }
            else if (e.KeyCode == Keys.Escape && btnClose.Enabled) { e.Handled = true; btnClose.PerformClick(); }
        }

        private void cmbCustomerCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Flogin)
            {
                string code = cmbCustomerCode.SelectedValue?.ToString();
                txtAccountCode.Text = code ?? string.Empty;
                FillDetail(code);
            }
        }

        private async void btnCustomerSearch_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                        var presigned = await _customerApiService.GetPresignedUploadUrlAsync(fileName);
                        if (presigned != null)
                        {
                            await _customerApiService.UploadFileAsync(presigned.UploadUrl, openFileDialog.FileName);
                            profileImage1.MediaId = presigned.FileId;
                            profileImage1.MediaUrl = openFileDialog.FileName;
                            profileImage1.PictureBox.Image = System.Drawing.Image.FromFile(openFileDialog.FileName);
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

        private void btnCustomerCancel_Click(object sender, EventArgs e)
        {
            profileImage1.ClearImage();
        }

        private void PopulateParentCustomersCombo(string currentAccount)
        {
            var validParents = _customers?
                .Where(x => !string.Equals(x.Account, currentAccount, StringComparison.OrdinalIgnoreCase) && !x.IsSubAccount)
                .ToList() ?? new List<CustomerDto>();

            var list = new List<CustomerDto> { new CustomerDto { Account = string.Empty, Title = "-- Select Main Customer --" } };
            list.AddRange(validParents);

            cmbParentCustomer.DataSource = null;
            cmbParentCustomer.DisplayMember = "Title";
            cmbParentCustomer.ValueMember = "Account";
            cmbParentCustomer.DataSource = list;
        }

        // ── Helper ────────────────────────────────────────────────────────
        private static void ShowError(Exception ex) =>
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

