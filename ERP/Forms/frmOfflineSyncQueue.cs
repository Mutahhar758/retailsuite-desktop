using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Reporting;
using ERP.Services.Authentication;
using ERP.Services.Legacy;
using ERP.Services.Offline;

namespace ERP.Forms
{
    /// <summary>
    /// Master-detail review & confirmation screen for Offline POS / Sale Billing vouchers.
    /// Allows an authenticated user to inspect every queued bill's line items and totals,
    /// then explicitly Sync Selected, Ignore/Discard Selected, or Reprint Receipt.
    /// </summary>
    public class frmOfflineSyncQueue : Form
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel pnlToolbar;
        private ComboBox cmbFilter;
        private Button btnSelectAll;
        private Button btnSyncSelected;
        private Button btnIgnoreSelected;
        private Button btnReprint;
        private Button btnRefreshCache;
        private Button btnClose;

        private SplitContainer splitMain;
        private DataGridView dgvVouchers;
        private Panel pnlDetailHeader;
        private Label lblDetailTitle;
        private Label lblDetailMeta;
        private DataGridView dgvLines;

        private Panel pnlFooter;
        private Label lblFooterStatus;

        private List<OfflineQueuedSale> _currentList = new List<OfflineQueuedSale>();
        private bool _isBusy = false;

        public bool SyncedOrChanged { get; private set; }

        public frmOfflineSyncQueue()
        {
            InitializeComponentCodeFirst();
            this.Load += (s, e) => LoadGridData();
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Offline Sales — Sync Review & Confirmation";
            this.Size = new Size(1120, 700);
            this.MinimumSize = new Size(920, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.BackColor = Color.FromArgb(248, 250, 252);

            // 1. Header
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Color.FromArgb(15, 23, 42),
                Padding = new Padding(16, 10, 16, 10)
            };

            lblTitle = new Label
            {
                Text = "Offline Sales Sync Confirmation",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 8)
            };

            lblSubtitle = new Label
            {
                Text = "Review offline bills below. Select vouchers to Sync to ERP or Ignore/Discard.",
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(16, 36)
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);

            // 2. Toolbar
            pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.White,
                Padding = new Padding(12, 9, 12, 9)
            };
            pnlToolbar.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, pnlToolbar.Height - 1, pnlToolbar.Width, pnlToolbar.Height - 1);
            };

            var lblFilter = new Label
            {
                Text = "Show:",
                AutoSize = true,
                Location = new Point(14, 17),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85)
            };

            cmbFilter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(62, 13),
                Width = 145,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };
            cmbFilter.Items.AddRange(new object[] { "Pending Review", "Synced Archive", "Ignored Archive" });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => LoadGridData();

            btnSelectAll = CreateToolbarButton("Select All", 218, 86, Color.FromArgb(241, 245, 249), Color.FromArgb(15, 23, 42));
            btnSelectAll.Click += (s, e) => ToggleSelectAll();

            btnSyncSelected = CreateToolbarButton("Sync Selected", 314, 128, Color.FromArgb(37, 99, 235), Color.White);
            btnSyncSelected.Click += async (s, e) => await SyncSelectedAsync();

            btnIgnoreSelected = CreateToolbarButton("Ignore Selected", 452, 130, Color.FromArgb(220, 38, 38), Color.White);
            btnIgnoreSelected.Click += (s, e) => IgnoreSelected();

            btnReprint = CreateToolbarButton("Reprint Receipt", 592, 126, Color.FromArgb(241, 245, 249), Color.FromArgb(15, 23, 42));
            btnReprint.Click += async (s, e) => await ReprintSelectedAsync();

            btnRefreshCache = CreateToolbarButton("Refresh Catalog", 728, 126, Color.FromArgb(241, 245, 249), Color.FromArgb(15, 23, 42));
            btnRefreshCache.Click += async (s, e) => await RefreshCatalogAsync();

            btnClose = CreateToolbarButton("Decide Later", 864, 108, Color.FromArgb(226, 232, 240), Color.FromArgb(15, 23, 42));
            btnClose.Click += (s, e) => this.Close();

            pnlToolbar.Controls.AddRange(new Control[]
            {
                lblFilter, cmbFilter, btnSelectAll, btnSyncSelected, btnIgnoreSelected, btnReprint, btnRefreshCache, btnClose
            });

            // 3. Footer
            pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 32,
                BackColor = Color.FromArgb(241, 245, 249)
            };
            pnlFooter.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(203, 213, 225), 1), 0, 0, pnlFooter.Width, 0);
            };
            lblFooterStatus = new Label
            {
                Text = "Ready",
                AutoSize = true,
                Location = new Point(14, 8),
                ForeColor = Color.FromArgb(71, 85, 105),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular)
            };
            pnlFooter.Controls.Add(lblFooterStatus);

            // 4. Master-Detail Split Container
            splitMain = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300,
                SplitterWidth = 6,
                BackColor = Color.FromArgb(226, 232, 240)
            };

            // Master Grid
            dgvVouchers = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };
            dgvVouchers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            dgvVouchers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvVouchers.EnableHeadersVisualStyles = false;
            dgvVouchers.RowTemplate.Height = 30;

            dgvVouchers.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colSelect", HeaderText = "", Width = 36, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            dgvVouchers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colVoucherNo", HeaderText = "Voucher No", ReadOnly = true, FillWeight = 110 });
            dgvVouchers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDateTime", HeaderText = "Date / Time", ReadOnly = true, FillWeight = 130 });
            dgvVouchers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCashier", HeaderText = "Cashier", ReadOnly = true, FillWeight = 95 });
            dgvVouchers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCustomer", HeaderText = "Customer", ReadOnly = true, FillWeight = 150 });
            dgvVouchers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSource", HeaderText = "Screen", ReadOnly = true, FillWeight = 85 });
            dgvVouchers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colItems", HeaderText = "Lines", ReadOnly = true, FillWeight = 55 });
            dgvVouchers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNetAmount", HeaderText = "Net Amount", ReadOnly = true, FillWeight = 95 });
            dgvVouchers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCashReceipt", HeaderText = "Paid", ReadOnly = true, FillWeight = 90 });
            dgvVouchers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Status", ReadOnly = true, FillWeight = 80 });
            dgvVouchers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colError", HeaderText = "Notes / Error", ReadOnly = true, FillWeight = 170 });

            dgvVouchers.SelectionChanged += (s, e) => RenderSelectedBillDetail();
            dgvVouchers.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvVouchers.IsCurrentCellDirty)
                    dgvVouchers.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            dgvVouchers.CellValueChanged += (s, e) => UpdateActionButtonsState();

            splitMain.Panel1.Controls.Add(dgvVouchers);

            // Detail Pane
            pnlDetailHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 46,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(12, 6, 12, 6)
            };
            lblDetailTitle = new Label
            {
                Text = "Bill Line Items",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(12, 6)
            };
            lblDetailMeta = new Label
            {
                Text = "Select a bill above to inspect items and totals before syncing or ignoring.",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(12, 24)
            };
            pnlDetailHeader.Controls.Add(lblDetailTitle);
            pnlDetailHeader.Controls.Add(lblDetailMeta);

            dgvLines = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };
            dgvLines.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            dgvLines.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            dgvLines.EnableHeadersVisualStyles = false;
            dgvLines.RowTemplate.Height = 26;

            dgvLines.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSeq", HeaderText = "#", FillWeight = 40 });
            dgvLines.Columns.Add(new DataGridViewTextBoxColumn { Name = "colItem", HeaderText = "Item", FillWeight = 240 });
            dgvLines.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUnit", HeaderText = "Unit", FillWeight = 80 });
            dgvLines.Columns.Add(new DataGridViewTextBoxColumn { Name = "colQty", HeaderText = "Qty", FillWeight = 75 });
            dgvLines.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRate", HeaderText = "Rate", FillWeight = 90 });
            dgvLines.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDisc", HeaderText = "Discount", FillWeight = 85 });
            dgvLines.Columns.Add(new DataGridViewTextBoxColumn { Name = "colAmt", HeaderText = "Amount", FillWeight = 100 });

            splitMain.Panel2.Controls.Add(dgvLines);
            splitMain.Panel2.Controls.Add(pnlDetailHeader);

            this.Controls.Add(splitMain);
            this.Controls.Add(pnlToolbar);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlFooter);
        }

        private Button CreateToolbarButton(string text, int x, int width, Color backColor, Color foreColor)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, 10),
                Size = new Size(width, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = foreColor,
                Font = new Font("Segoe UI", 8.75F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private bool IsPendingView => cmbFilter.SelectedIndex == 0;

        private void LoadGridData()
        {
            dgvVouchers.Rows.Clear();
            dgvLines.Rows.Clear();

            if (cmbFilter.SelectedIndex == 0)
                _currentList = OfflineStore.GetPendingSales();
            else if (cmbFilter.SelectedIndex == 1)
                _currentList = OfflineStore.GetSyncedSales();
            else
                _currentList = OfflineStore.GetIgnoredSales();

            dgvVouchers.Columns["colSelect"].Visible = IsPendingView;

            foreach (var sale in _currentList)
            {
                int rowIdx = dgvVouchers.Rows.Add(
                    IsPendingView, // pre-check pending bills for convenient review
                    "SL-" + sale.VoucherNo,
                    sale.CreatedAtLocal.ToString("dd-MMM-yyyy HH:mm"),
                    sale.CashierName ?? "",
                    sale.CustomerName ?? "",
                    sale.SourceForm ?? "POS",
                    sale.ReceiptLines?.Count ?? sale.Request?.Lines?.Count ?? 0,
                    sale.NetAmount.ToString("N2"),
                    sale.CashReceipt.ToString("N2"),
                    sale.Status.ToString(),
                    string.IsNullOrWhiteSpace(sale.LastError)
                        ? (string.IsNullOrWhiteSpace(sale.ReviewedBy) ? "" : "By: " + sale.ReviewedBy)
                        : sale.LastError
                );
                dgvVouchers.Rows[rowIdx].Tag = sale;
            }

            if (dgvVouchers.Rows.Count > 0)
            {
                dgvVouchers.Rows[0].Selected = true;
                RenderSelectedBillDetail();
            }
            else
            {
                lblDetailTitle.Text = "Bill Line Items";
                lblDetailMeta.Text = "No bills in this view.";
            }

            UpdateActionButtonsState();
            UpdateFooterSummary();
        }

        private void RenderSelectedBillDetail()
        {
            dgvLines.Rows.Clear();
            var sale = GetFocusedSale();
            if (sale == null)
            {
                lblDetailTitle.Text = "Bill Line Items";
                lblDetailMeta.Text = "Select a bill above to inspect items and totals.";
                return;
            }

            lblDetailTitle.Text = $"Bill Details — SL-{sale.VoucherNo} ({sale.CustomerName})";
            lblDetailMeta.Text = $"Terminal: {sale.TerminalCode}  |  Cashier: {sale.CashierName}  |  Gross: {sale.GrossTotal:N2}  |  Disc: {sale.TotalDiscount:N2}  |  Net: {sale.NetAmount:N2}  |  Paid: {sale.CashReceipt:N2}  |  Change: {sale.CashBack:N2}";

            if (sale.ReceiptLines != null)
            {
                foreach (var line in sale.ReceiptLines)
                {
                    dgvLines.Rows.Add(
                        line.Seq,
                        line.Item,
                        line.Unit,
                        line.Qty.ToString("#,##0.##"),
                        line.Rate.ToString("N2"),
                        line.Discount.ToString("N2"),
                        line.Amount.ToString("N2")
                    );
                }
            }
        }

        private OfflineQueuedSale GetFocusedSale()
        {
            if (dgvVouchers.SelectedRows.Count == 0) return null;
            return dgvVouchers.SelectedRows[0].Tag as OfflineQueuedSale;
        }

        private List<OfflineQueuedSale> GetCheckedSales()
        {
            var list = new List<OfflineQueuedSale>();
            foreach (DataGridViewRow row in dgvVouchers.Rows)
            {
                bool isChecked = row.Cells["colSelect"].Value != null && Convert.ToBoolean(row.Cells["colSelect"].Value);
                if (isChecked && row.Tag is OfflineQueuedSale sale)
                    list.Add(sale);
            }
            return list;
        }

        private void ToggleSelectAll()
        {
            if (!IsPendingView || dgvVouchers.Rows.Count == 0) return;
            bool anyUnchecked = dgvVouchers.Rows.Cast<DataGridViewRow>()
                .Any(r => !(r.Cells["colSelect"].Value != null && Convert.ToBoolean(r.Cells["colSelect"].Value)));

            foreach (DataGridViewRow row in dgvVouchers.Rows)
                row.Cells["colSelect"].Value = anyUnchecked;

            UpdateActionButtonsState();
        }

        private void UpdateActionButtonsState()
        {
            int checkedCount = IsPendingView ? GetCheckedSales().Count : 0;
            btnSelectAll.Enabled = !_isBusy && IsPendingView && dgvVouchers.Rows.Count > 0;
            btnSyncSelected.Enabled = !_isBusy && IsPendingView && checkedCount > 0;
            btnIgnoreSelected.Enabled = !_isBusy && IsPendingView && checkedCount > 0;
            btnReprint.Enabled = !_isBusy && GetFocusedSale() != null;
            btnRefreshCache.Enabled = !_isBusy;
        }

        private void UpdateFooterSummary()
        {
            int pendingCount = OfflineStore.GetPendingCount();
            string cacheText = ApiSession.LastOfflineCacheSyncUtc.HasValue
                ? ApiSession.LastOfflineCacheSyncUtc.Value.ToLocalTime().ToString("dd-MMM HH:mm")
                : "Not synced";
            lblFooterStatus.Text = $"Terminal: {OfflineVoucherGenerator.GetTerminalCode()}  |  Pending Bills: {pendingCount}  |  Last Catalog Cache: {cacheText}";
        }

        private async Task<bool> EnsureOnlineAuthenticatedAsync()
        {
            bool reachable = await OfflineSyncService.IsApiReachableAsync(5);
            if (!reachable)
            {
                MessageBox.Show("Server is still unreachable. Please check your internet connection and try again.", "Still Offline", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(ApiSession.AccessToken) && !ApiSession.NeedsOnlineReauth)
                return true;

            // Prompt for real online credentials (never stored on disk)
            using (var dlg = new OnlineAuthPromptDialog())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return false;
            }

            return !string.IsNullOrWhiteSpace(ApiSession.AccessToken);
        }

        private async Task SyncSelectedAsync()
        {
            var selected = GetCheckedSales();
            if (selected.Count == 0) return;

            decimal totalNet = selected.Sum(s => s.NetAmount);
            var confirm = MessageBox.Show(
                $"Confirm syncing {selected.Count} offline bill(s) totaling {totalNet:N2} to the server?\n\nVoucher numbers will be preserved as printed on receipts.",
                "Confirm Sync to Server",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            _isBusy = true;
            UpdateActionButtonsState();
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (!await EnsureOnlineAuthenticatedAsync())
                    return;

                lblFooterStatus.Text = $"Syncing {selected.Count} bill(s)...";
                var batch = await OfflineSyncService.SyncSelectedSalesAsync(selected.Select(s => s.VoucherNo));
                SyncedOrChanged = true;

                // If we were in Offline Mode and successfully authenticated + synced, switch back to Online Mode
                if (ApiSession.IsOfflineMode && !string.IsNullOrWhiteSpace(ApiSession.AccessToken) && !batch.StoppedDueToNetwork)
                {
                    ApiSession.NeedsOnlineReauth = false;
                    ApiSession.SetOfflineMode(false);
                }

                string msg = $"Synced: {batch.SucceededCount}\nFailed: {batch.FailedCount}";
                if (batch.Messages.Count > 0)
                    msg += "\n\nDetails:\n" + string.Join("\n", batch.Messages.Take(8));

                MessageBox.Show(msg, "Sync Result", MessageBoxButtons.OK,
                    batch.FailedCount > 0 || batch.StoppedDueToNetwork ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                LoadGridData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sync failed: " + ex.Message, "Sync Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isBusy = false;
                Cursor.Current = Cursors.Default;
                UpdateActionButtonsState();
                UpdateFooterSummary();
            }
        }

        private void IgnoreSelected()
        {
            var selected = GetCheckedSales();
            if (selected.Count == 0) return;

            decimal totalNet = selected.Sum(s => s.NetAmount);
            var confirm = MessageBox.Show(
                $"Are you sure you want to IGNORE / DISCARD {selected.Count} offline bill(s) totaling {totalNet:N2}?\n\nIgnored bills will NOT be posted to the ERP ledger (they are moved to the Ignored Archive locally).",
                "Confirm Ignore Offline Bills",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            int ignored = OfflineSyncService.IgnoreSelectedSales(selected.Select(s => s.VoucherNo), UserInfo.UserName);
            SyncedOrChanged = true;
            MessageBox.Show($"{ignored} offline bill(s) moved to Ignored Archive.", "Ignored", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadGridData();
        }

        private async Task ReprintSelectedAsync()
        {
            var sale = GetFocusedSale();
            if (sale == null) return;

            await SaleReceiptViewer.PrintOrPreviewOfflineAsync(sale.ToReceiptDataResult(isDuplicate: true), isDirect: false);
        }

        private async Task RefreshCatalogAsync()
        {
            _isBusy = true;
            UpdateActionButtonsState();
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                if (!await EnsureOnlineAuthenticatedAsync())
                    return;

                lblFooterStatus.Text = "Refreshing offline catalog cache...";
                await OfflineCacheService.RefreshCatalogAsync();
                MessageBox.Show("Offline product & customer catalog refreshed successfully.", "Cache Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to refresh catalog: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isBusy = false;
                Cursor.Current = Cursors.Default;
                UpdateActionButtonsState();
                UpdateFooterSummary();
            }
        }

        /// <summary>
        /// Compact modal dialog to authenticate online right before syncing bills created in unauthenticated Offline POS Mode.
        /// Credentials are sent directly to POST /api/auth/login and never persisted on disk.
        /// </summary>
        private class OnlineAuthPromptDialog : Form
        {
            private TextBox txtUser;
            private TextBox txtPass;
            private Button btnOk;
            private Button btnCancel;
            private Label lblMsg;

            public OnlineAuthPromptDialog()
            {
                this.Text = "Online Sign-In Required to Sync";
                this.Size = new Size(400, 230);
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Font = new Font("Segoe UI", 9F);

                lblMsg = new Label
                {
                    Text = "Sign in with your RetailSuite account to verify permissions and sync offline bills:",
                    Location = new Point(16, 14),
                    Size = new Size(355, 36),
                    ForeColor = Color.FromArgb(51, 65, 85)
                };

                var lblUser = new Label { Text = "Username:", Location = new Point(16, 62), AutoSize = true };
                txtUser = new TextBox { Location = new Point(105, 59), Width = 260 };

                var lblPass = new Label { Text = "Password:", Location = new Point(16, 98), AutoSize = true };
                txtPass = new TextBox { Location = new Point(105, 95), Width = 260, UseSystemPasswordChar = true };

                btnOk = new Button
                {
                    Text = "Sign In & Continue",
                    Location = new Point(145, 142),
                    Size = new Size(130, 32),
                    BackColor = Color.FromArgb(37, 99, 235),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                btnOk.FlatAppearance.BorderSize = 0;
                btnOk.Click += async (s, e) => await DoLoginAsync();

                btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(285, 142),
                    Size = new Size(80, 32),
                    DialogResult = DialogResult.Cancel
                };

                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;
                this.Controls.AddRange(new Control[] { lblMsg, lblUser, txtUser, lblPass, txtPass, btnOk, btnCancel });
            }

            private async Task DoLoginAsync()
            {
                string username = txtUser.Text.Trim();
                string password = txtPass.Text;
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter both username and password.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                btnOk.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    var loginService = new LoginService();
                    var result = await loginService.LoginAsync(username, password);
                    if (!result.Success)
                    {
                        MessageBox.Show(result.Message, "Sign-In Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ApiSession.AccessToken = result.TokenResponse.Token;
                    ApiSession.RefreshToken = result.TokenResponse.RefreshToken;
                    ApiSession.RefreshTokenExpiryTime = result.TokenResponse.RefreshTokenExpiryTime;
                    ApiSession.NeedsOnlineReauth = false;

                    try
                    {
                        var personalService = new PersonalApiService();
                        var profile = await personalService.GetProfileAsync();
                        UserInfo.UserId = string.IsNullOrWhiteSpace(profile.UserName) ? username : profile.UserName;
                        UserInfo.UserName = UserInfo.UserId;
                        UserInfo.Email = profile.Email;
                        UserInfo.IsOwner = profile.IsOwner;
                        ApiSession.UserEmail = profile.Email;
                        UserInfo.Permissions = await personalService.GetPermissionsAsync();
                    }
                    catch { }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Online sign-in failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnOk.Enabled = true;
                    Cursor.Current = Cursors.Default;
                }
            }
        }
    }
}
