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
using ERP.Reporting.Models;
using ERP.Services.Legacy;

namespace ERP.Forms
{
    /// <summary>
    /// Modern, touch-screen first Point of Sale (POS) interface for RetailSuite Desktop.
    /// Features large touch product tiles, category strip, rapid barcode scanning,
    /// touch-enabled cart steppers, multi-order tabs, and direct thermal receipt printing.
    /// </summary>
    public class frmTouchPOS : Form
    {
        // Data & Services
        private readonly SaleApiService _saleApiService;
        private readonly InventoryApiService _inventoryApiService;
        private readonly ItemCategoryApiService _categoryApiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;

        private List<InventoryItemDto> _allItems = new List<InventoryItemDto>();
        private List<InventoryItemDto> _filteredItems = new List<InventoryItemDto>();
        private List<ItemCategoryDto> _categories = new List<ItemCategoryDto>();
        private List<ChartOfAccountHeadDto> _customers = new List<ChartOfAccountHeadDto>();

        // Multi-order Hold Support
        private readonly Dictionary<int, List<TouchCartItem>> _orders = new Dictionary<int, List<TouchCartItem>>();
        private readonly Dictionary<int, decimal> _orderDiscounts = new Dictionary<int, decimal>();
        private int _activeOrderIndex = 1;

        private string _activeCategoryCode = "ALL";
        private bool _isLoading = false;

        // UI Controls
        private Panel pnlTopBar;
        private Label lblAppTitle;
        private Label lblOperator;
        private Label lblClock;
        private Timer tmrClock;
        private Button btnNewOrder;
        private Button btnClose;

        private Panel pnlLeftCatalog;
        private Panel pnlCatalogHeader;
        private Label lblBarcode;
        private TextBox txtBarcode;
        private Label lblSearch;
        private TextBox txtSearch;
        private Label lblCustomer;
        private ComboBox cmbCustomer;

        private FlowLayoutPanel pnlCategories;
        private FlowLayoutPanel pnlProducts;

        private Panel pnlRightCart;
        private Panel pnlCartTabs;
        private Button btnOrder1;
        private Button btnOrder2;
        private Button btnOrder3;

        private DataGridView dgvCart;
        private Panel pnlCartSummary;
        private Label lblSummaryCounts;
        private Label lblSubtotal;
        private Panel pnlDiscountRow;
        private Label lblDiscountTitle;
        private Button btnDisc0;
        private Button btnDisc5;
        private Button btnDisc10;
        private TextBox txtDiscountFlat;
        private Panel pnlNetPayableBanner;
        private Label lblNetPayableTitle;
        private Label lblNetPayableValue;

        private Panel pnlCartActions;
        private Button btnClearCart;
        private Button btnPay;

        public class TouchCartItem
        {
            public string ItemId { get; set; }
            public string Title { get; set; }
            public string ItemCode { get; set; }
            public string Barcode { get; set; }
            public string Unit { get; set; }
            public decimal Qty { get; set; }
            public decimal Rate { get; set; }
            public decimal Discount { get; set; }
            public decimal LineTotal => Math.Max(0, Math.Round(Qty * Rate - Discount, 2));
        }

        public frmTouchPOS()
        {
            _saleApiService = new SaleApiService();
            _inventoryApiService = new InventoryApiService();
            _categoryApiService = new ItemCategoryApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();

            _orders[1] = new List<TouchCartItem>();
            _orders[2] = new List<TouchCartItem>();
            _orders[3] = new List<TouchCartItem>();
            _orderDiscounts[1] = 0;
            _orderDiscounts[2] = 0;
            _orderDiscounts[3] = 0;

            InitializeCodeFirstUI();
        }

        private void InitializeCodeFirstUI()
        {
            this.Text = "RetailSuite - Touch Screen POS";
            this.Size = new Size(1280, 800);
            this.MinimumSize = new Size(1024, 700);
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 250, 252); // Slate-50
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.KeyPreview = true;
            this.KeyDown += FrmTouchPOS_KeyDown;

            // 1. Top Navigation Bar
            pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.FromArgb(15, 23, 42), // Slate-900
                Padding = new Padding(16, 8, 16, 8)
            };

            lblAppTitle = new Label
            {
                Text = "🛒 RetailSuite Touch POS",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(16, 14)
            };

            lblOperator = new Label
            {
                Text = "Operator: " + (UserInfo.UserName ?? "Admin"),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184), // Slate-400
                AutoSize = true,
                Location = new Point(270, 18)
            };

            lblClock = new Label
            {
                Text = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt"),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248), // Sky-400
                AutoSize = true,
                Location = new Point(480, 18)
            };

            tmrClock = new Timer { Interval = 1000 };
            tmrClock.Tick += (s, e) => lblClock.Text = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");
            tmrClock.Start();

            btnNewOrder = new Button
            {
                Text = "➕ New Bill (F1)",
                Size = new Size(130, 38),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(this.ClientSize.Width - 250, 9),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(30, 41, 59), // Slate-800
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnNewOrder.FlatAppearance.BorderColor = Color.FromArgb(51, 65, 85);
            btnNewOrder.Click += (s, e) => ResetActiveOrder();

            btnClose = new Button
            {
                Text = "✕ Close (Esc)",
                Size = new Size(100, 38),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(this.ClientSize.Width - 110, 9),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 38, 38), // Red-600
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => ConfirmCloseForm();

            pnlTopBar.Controls.Add(lblAppTitle);
            pnlTopBar.Controls.Add(lblOperator);
            pnlTopBar.Controls.Add(lblClock);
            pnlTopBar.Controls.Add(btnNewOrder);
            pnlTopBar.Controls.Add(btnClose);

            // 2. Right Cart Workspace (35% width, docked Right)
            pnlRightCart = new Panel
            {
                Dock = DockStyle.Right,
                Width = 460,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12)
            };

            // Order Tabs Strip
            pnlCartTabs = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.FromArgb(241, 245, 249)
            };

            btnOrder1 = CreateTabButton("Bill 1", 1, 0);
            btnOrder2 = CreateTabButton("Bill 2 (Hold)", 2, 140);
            btnOrder3 = CreateTabButton("Bill 3 (Hold)", 3, 280);
            pnlCartTabs.Controls.Add(btnOrder1);
            pnlCartTabs.Controls.Add(btnOrder2);
            pnlCartTabs.Controls.Add(btnOrder3);

            // Cart DataGridView
            dgvCart = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false
            };
            dgvCart.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            dgvCart.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(51, 65, 85);
            dgvCart.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvCart.ColumnHeadersHeight = 36;
            dgvCart.RowTemplate.Height = 44;
            dgvCart.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            dgvCart.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 249, 255);
            dgvCart.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);

            // Cart Columns
            var colTitle = new DataGridViewTextBoxColumn
            {
                HeaderText = "Item",
                DataPropertyName = "Title",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };
            var colRate = new DataGridViewTextBoxColumn
            {
                HeaderText = "Rate",
                DataPropertyName = "Rate",
                Width = 65,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
            };
            var colQty = new DataGridViewTextBoxColumn
            {
                HeaderText = "Qty",
                DataPropertyName = "Qty",
                Width = 55,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10F, FontStyle.Bold) }
            };
            var colTotal = new DataGridViewTextBoxColumn
            {
                HeaderText = "Total",
                DataPropertyName = "LineTotal",
                Width = 75,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Format = "N0" }
            };

            var colDec = new DataGridViewButtonColumn
            {
                HeaderText = "-",
                Text = "➖",
                UseColumnTextForButtonValue = true,
                Width = 38,
                FlatStyle = FlatStyle.Flat
            };
            var colInc = new DataGridViewButtonColumn
            {
                HeaderText = "+",
                Text = "➕",
                UseColumnTextForButtonValue = true,
                Width = 38,
                FlatStyle = FlatStyle.Flat
            };
            var colDel = new DataGridViewButtonColumn
            {
                HeaderText = "✕",
                Text = "✕",
                UseColumnTextForButtonValue = true,
                Width = 38,
                FlatStyle = FlatStyle.Flat
            };

            dgvCart.Columns.AddRange(colTitle, colRate, colQty, colTotal, colDec, colInc, colDel);
            dgvCart.CellContentClick += DgvCart_CellContentClick;

            // Cart Summary Panel
            pnlCartSummary = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 185,
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12, 8, 12, 8)
            };

            lblSummaryCounts = new Label
            {
                Text = "Items: 0   |   Total Qty: 0",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(12, 8),
                Size = new Size(200, 20)
            };

            lblSubtotal = new Label
            {
                Text = "Subtotal: Rs. 0",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(220, 6),
                Size = new Size(200, 22),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Discount Row
            pnlDiscountRow = new Panel
            {
                Location = new Point(12, 34),
                Size = new Size(410, 36)
            };

            lblDiscountTitle = new Label
            {
                Text = "Discount:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(0, 8),
                Size = new Size(65, 20)
            };

            btnDisc0 = CreateDiscButton("0%", 0, 70);
            btnDisc5 = CreateDiscButton("5%", 5, 125);
            btnDisc10 = CreateDiscButton("10%", 10, 180);

            txtDiscountFlat = new TextBox
            {
                Location = new Point(240, 5),
                Size = new Size(165, 25),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Right,
                Text = "0"
            };
            txtDiscountFlat.TextChanged += (s, e) => RecalculateTotals();

            pnlDiscountRow.Controls.Add(lblDiscountTitle);
            pnlDiscountRow.Controls.Add(btnDisc0);
            pnlDiscountRow.Controls.Add(btnDisc5);
            pnlDiscountRow.Controls.Add(btnDisc10);
            pnlDiscountRow.Controls.Add(txtDiscountFlat);

            // Giant Net Payable Banner
            pnlNetPayableBanner = new Panel
            {
                Location = new Point(12, 76),
                Size = new Size(410, 50),
                BackColor = Color.FromArgb(15, 23, 42), // Slate-900
                Padding = new Padding(10, 4, 10, 4)
            };

            lblNetPayableTitle = new Label
            {
                Text = "NET PAYABLE",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(10, 14),
                AutoSize = true
            };

            lblNetPayableValue = new Label
            {
                Text = "Rs. 0",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 211, 153), // Emerald-400
                Location = new Point(150, 4),
                Size = new Size(250, 40),
                TextAlign = ContentAlignment.MiddleRight
            };

            pnlNetPayableBanner.Controls.Add(lblNetPayableTitle);
            pnlNetPayableBanner.Controls.Add(lblNetPayableValue);

            // Cart Bottom Action Buttons
            pnlCartActions = new Panel
            {
                Location = new Point(12, 132),
                Size = new Size(410, 48)
            };

            btnClearCart = new Button
            {
                Text = "Clear",
                Size = new Size(95, 46),
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(220, 38, 38),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClearCart.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnClearCart.Click += (s, e) => ClearCurrentCart();

            btnPay = new Button
            {
                Text = "PAY / TENDER (F6)",
                Size = new Size(305, 46),
                Location = new Point(105, 0),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(22, 163, 74), // Emerald-600
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPay.FlatAppearance.BorderSize = 0;
            btnPay.Click += async (s, e) => await OpenPaymentDialogAsync();

            pnlCartActions.Controls.Add(btnClearCart);
            pnlCartActions.Controls.Add(btnPay);

            pnlCartSummary.Controls.Add(lblSummaryCounts);
            pnlCartSummary.Controls.Add(lblSubtotal);
            pnlCartSummary.Controls.Add(pnlDiscountRow);
            pnlCartSummary.Controls.Add(pnlNetPayableBanner);
            pnlCartSummary.Controls.Add(pnlCartActions);

            pnlRightCart.Controls.Add(dgvCart);
            pnlRightCart.Controls.Add(pnlCartTabs);
            pnlRightCart.Controls.Add(pnlCartSummary);

            // 3. Left Catalog Workspace (65% width, docked Fill)
            pnlLeftCatalog = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(12)
            };

            // Catalog Header (Barcode Scan, Search, Customer)
            pnlCatalogHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12, 8, 12, 8)
            };

            lblBarcode = new Label
            {
                Text = "SCAN BARCODE (F2):",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(12, 8),
                AutoSize = true
            };

            txtBarcode = new TextBox
            {
                Location = new Point(12, 30),
                Size = new Size(220, 28),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(240, 249, 255)
            };
            txtBarcode.KeyDown += TxtBarcode_KeyDown;

            lblSearch = new Label
            {
                Text = "SEARCH ITEM:",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(245, 8),
                AutoSize = true
            };

            txtSearch = new TextBox
            {
                Location = new Point(245, 30),
                Size = new Size(220, 28),
                Font = new Font("Segoe UI", 11F, FontStyle.Regular)
            };
            txtSearch.TextChanged += (s, e) => FilterProducts();

            lblCustomer = new Label
            {
                Text = "CUSTOMER / ACCOUNT:",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(480, 8),
                AutoSize = true
            };

            cmbCustomer = new ComboBox
            {
                Location = new Point(480, 30),
                Size = new Size(250, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular)
            };

            pnlCatalogHeader.Controls.Add(lblBarcode);
            pnlCatalogHeader.Controls.Add(txtBarcode);
            pnlCatalogHeader.Controls.Add(lblSearch);
            pnlCatalogHeader.Controls.Add(txtSearch);
            pnlCatalogHeader.Controls.Add(lblCustomer);
            pnlCatalogHeader.Controls.Add(cmbCustomer);

            // Category Touch Horizontal Strip
            pnlCategories = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 54,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(6, 6, 6, 6)
            };

            // Product Cards FlowLayoutPanel
            pnlProducts = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(8)
            };

            pnlLeftCatalog.Controls.Add(pnlProducts);
            pnlLeftCatalog.Controls.Add(pnlCategories);
            pnlLeftCatalog.Controls.Add(pnlCatalogHeader);

            // Add top bar and split workspaces to Form
            this.Controls.Add(pnlLeftCatalog);
            this.Controls.Add(pnlRightCart);
            this.Controls.Add(pnlTopBar);

            this.Load += async (s, e) => await LoadPosDataAsync();
        }

        private Button CreateTabButton(string text, int orderIndex, int xPos)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(xPos, 2),
                Size = new Size(130, 38),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = orderIndex == _activeOrderIndex ? Color.FromArgb(15, 23, 42) : Color.White,
                ForeColor = orderIndex == _activeOrderIndex ? Color.White : Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btn.Click += (s, e) => SwitchOrderTab(orderIndex);
            return btn;
        }

        private Button CreateDiscButton(string text, decimal percent, int xPos)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(xPos, 4),
                Size = new Size(48, 28),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btn.Click += (s, e) =>
            {
                decimal sub = GetActiveCart().Sum(i => i.LineTotal);
                decimal disc = Math.Round((sub * percent) / 100m, 2);
                txtDiscountFlat.Text = disc.ToString("0.##", CultureInfo.InvariantCulture);
            };
            return btn;
        }

        private async Task LoadPosDataAsync()
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                // 1. Fetch Customers
                _customers = await _chartOfAccountApiService.GetCustomerAccountsAsync();
                cmbCustomer.DataSource = _customers;
                cmbCustomer.DisplayMember = "Title";
                cmbCustomer.ValueMember = "Account";

                // Default to standard Cash Customer "001002001001" if present
                var defaultCust = _customers.FirstOrDefault(c => c.Account == "001002001001" || c.Account == AccountHead.Customer);
                if (defaultCust != null)
                {
                    cmbCustomer.SelectedValue = defaultCust.Account;
                }
                else if (_customers.Count > 0)
                {
                    cmbCustomer.SelectedIndex = 0;
                }

                // 2. Fetch Categories
                _categories = await _categoryApiService.GetLookupAsync();
                RenderCategoryButtons();

                // 3. Fetch Items
                _allItems = await _inventoryApiService.GetLookupAsync(null);
                _filteredItems = new List<InventoryItemDto>(_allItems);
                RenderProductTiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load POS catalog: " + ex.Message, "POS Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                _isLoading = false;
                txtBarcode.Focus();
            }
        }

        private void RenderCategoryButtons()
        {
            pnlCategories.SuspendLayout();
            pnlCategories.Controls.Clear();

            // "All Categories" chip
            Button btnAll = CreateCategoryChip("All Items", "ALL", _activeCategoryCode == "ALL");
            pnlCategories.Controls.Add(btnAll);

            foreach (var cat in _categories)
            {
                var catCode = cat.Code;
                Button btnCat = CreateCategoryChip(cat.Title, catCode, _activeCategoryCode == catCode);
                pnlCategories.Controls.Add(btnCat);
            }
            pnlCategories.ResumeLayout();
        }

        private Button CreateCategoryChip(string title, string code, bool isActive)
        {
            var btn = new Button
            {
                Text = title,
                AutoSize = true,
                MinimumSize = new Size(95, 38),
                Height = 38,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = isActive ? Color.FromArgb(15, 23, 42) : Color.White,
                ForeColor = isActive ? Color.White : Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 8, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = isActive ? Color.FromArgb(15, 23, 42) : Color.FromArgb(203, 213, 225);
            btn.Click += (s, e) =>
            {
                _activeCategoryCode = code;
                RenderCategoryButtons();
                FilterProducts();
            };
            return btn;
        }

        private void FilterProducts()
        {
            string query = (txtSearch.Text ?? "").Trim().ToLower();

            _filteredItems = _allItems.Where(item =>
            {
                bool matchesCategory = (_activeCategoryCode == "ALL") ||
                                       string.Equals(item.ItemCategoryCode, _activeCategoryCode, StringComparison.OrdinalIgnoreCase);

                if (!matchesCategory) return false;

                if (string.IsNullOrWhiteSpace(query)) return true;

                bool matchesTitle = (item.Title ?? "").ToLower().Contains(query);
                bool matchesBarcode = (item.Barcode ?? "").ToLower().Contains(query);
                bool matchesKey = (item.ItemKey ?? "").ToLower().Contains(query);

                return matchesTitle || matchesBarcode || matchesKey;
            }).Take(120).ToList();

            RenderProductTiles();
        }

        private void RenderProductTiles()
        {
            pnlProducts.SuspendLayout();
            pnlProducts.Controls.Clear();

            foreach (var item in _filteredItems)
            {
                var currentItem = item;
                Panel card = new Panel
                {
                    Size = new Size(140, 115),
                    Margin = new Padding(6),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Hand
                };

                Label lblTitle = new Label
                {
                    Text = currentItem.Title ?? "Item",
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    Location = new Point(6, 6),
                    Size = new Size(126, 42),
                    TextAlign = ContentAlignment.TopLeft
                };

                Label lblUnit = new Label
                {
                    Text = currentItem.DefaultUnitTitle ?? currentItem.PrimaryUnit ?? "Pcs",
                    Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Location = new Point(6, 52),
                    Size = new Size(126, 16)
                };

                Label lblPrice = new Label
                {
                    Text = "Rs. " + currentItem.PriRate.ToString("N0", CultureInfo.InvariantCulture),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(2, 132, 199), // Sky-600
                    Location = new Point(6, 74),
                    Size = new Size(126, 32),
                    TextAlign = ContentAlignment.BottomRight
                };

                card.Controls.Add(lblTitle);
                card.Controls.Add(lblUnit);
                card.Controls.Add(lblPrice);

                Action clickAction = () => AddOrIncrementItem(currentItem);

                card.Click += (s, e) => clickAction();
                lblTitle.Click += (s, e) => clickAction();
                lblUnit.Click += (s, e) => clickAction();
                lblPrice.Click += (s, e) => clickAction();

                pnlProducts.Controls.Add(card);
            }

            pnlProducts.ResumeLayout();
        }

        private List<TouchCartItem> GetActiveCart()
        {
            if (!_orders.ContainsKey(_activeOrderIndex))
            {
                _orders[_activeOrderIndex] = new List<TouchCartItem>();
            }
            return _orders[_activeOrderIndex];
        }

        private void AddOrIncrementItem(InventoryItemDto item)
        {
            var cart = GetActiveCart();
            var existing = cart.FirstOrDefault(c => c.ItemId == item.Id);

            if (existing != null)
            {
                existing.Qty += 1;
            }
            else
            {
                cart.Add(new TouchCartItem
                {
                    ItemId = item.Id,
                    Title = item.Title,
                    ItemCode = item.ItemKey,
                    Barcode = item.Barcode,
                    Unit = item.DefaultUnitTitle ?? item.PrimaryUnit ?? "Pcs",
                    Qty = 1,
                    Rate = item.PriRate,
                    Discount = 0
                });
            }

            ScannerToneHelper.PlaySuccess();
            RefreshCartGrid();
            txtBarcode.Focus();
        }

        private void RefreshCartGrid()
        {
            var cart = GetActiveCart();
            dgvCart.DataSource = null;
            dgvCart.DataSource = cart.ToList();

            RecalculateTotals();
        }

        private void RecalculateTotals()
        {
            var cart = GetActiveCart();
            int itemCount = cart.Count;
            decimal totalQty = cart.Sum(i => i.Qty);
            decimal subtotal = cart.Sum(i => i.LineTotal);

            lblSummaryCounts.Text = $"Items: {itemCount}   |   Total Qty: {totalQty:0.##}";
            lblSubtotal.Text = "Subtotal: Rs. " + subtotal.ToString("N0", CultureInfo.InvariantCulture);

            decimal flatDiscount = 0;
            decimal.TryParse(txtDiscountFlat.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out flatDiscount);

            decimal net = Math.Max(0, subtotal - flatDiscount);
            lblNetPayableValue.Text = "Rs. " + net.ToString("N0", CultureInfo.InvariantCulture);
        }

        private void DgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvCart.Rows.Count) return;

            var cart = GetActiveCart();
            if (e.RowIndex >= cart.Count) return;

            var item = cart[e.RowIndex];

            // colDec: 4, colInc: 5, colDel: 6
            if (e.ColumnIndex == 4) // Minus
            {
                if (item.Qty > 1)
                {
                    item.Qty -= 1;
                }
                else
                {
                    cart.RemoveAt(e.RowIndex);
                }
                RefreshCartGrid();
            }
            else if (e.ColumnIndex == 5) // Plus
            {
                item.Qty += 1;
                RefreshCartGrid();
            }
            else if (e.ColumnIndex == 6) // Delete
            {
                cart.RemoveAt(e.RowIndex);
                RefreshCartGrid();
            }
        }

        private void SwitchOrderTab(int orderIndex)
        {
            _activeOrderIndex = orderIndex;

            btnOrder1.BackColor = orderIndex == 1 ? Color.FromArgb(15, 23, 42) : Color.White;
            btnOrder1.ForeColor = orderIndex == 1 ? Color.White : Color.FromArgb(71, 85, 105);

            btnOrder2.BackColor = orderIndex == 2 ? Color.FromArgb(15, 23, 42) : Color.White;
            btnOrder2.ForeColor = orderIndex == 2 ? Color.White : Color.FromArgb(71, 85, 105);

            btnOrder3.BackColor = orderIndex == 3 ? Color.FromArgb(15, 23, 42) : Color.White;
            btnOrder3.ForeColor = orderIndex == 3 ? Color.White : Color.FromArgb(71, 85, 105);

            RefreshCartGrid();
            txtBarcode.Focus();
        }

        private void ResetActiveOrder()
        {
            var cart = GetActiveCart();
            if (cart.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to clear the current bill?", "New Bill", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
            }
            ClearCurrentCart();
        }

        private void ClearCurrentCart()
        {
            var cart = GetActiveCart();
            cart.Clear();
            txtDiscountFlat.Text = "0";
            RefreshCartGrid();
            txtBarcode.Focus();
        }

        private void TxtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                string code = (txtBarcode.Text ?? "").Trim();
                txtBarcode.Text = "";

                if (string.IsNullOrWhiteSpace(code)) return;

                var matched = _allItems.FirstOrDefault(i =>
                    string.Equals(i.Barcode, code, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(i.ItemKey, code, StringComparison.OrdinalIgnoreCase));

                if (matched != null)
                {
                    AddOrIncrementItem(matched);
                }
                else
                {
                    ScannerToneHelper.PlayError();
                    MessageBox.Show($"Item barcode '{code}' not found.", "Barcode Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                txtBarcode.Focus();
            }
        }

        private async Task OpenPaymentDialogAsync()
        {
            var cart = GetActiveCart();
            if (cart.Count == 0)
            {
                MessageBox.Show("Cart is empty. Please add at least one item.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBarcode.Focus();
                return;
            }

            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer account.", "Customer Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCustomer.Focus();
                return;
            }

            decimal subtotal = cart.Sum(i => i.LineTotal);
            decimal flatDiscount = 0;
            decimal.TryParse(txtDiscountFlat.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out flatDiscount);
            decimal netPayable = Math.Max(0, subtotal - flatDiscount);

            using (var dlg = new frmTouchPosPayment(netPayable))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    await ExecuteSaveSaleAsync(dlg.CashReceived, dlg.CashBack, dlg.ShouldPrintReceipt);
                }
            }
        }

        private async Task ExecuteSaveSaleAsync(decimal cashReceived, decimal cashBack, bool printReceipt)
        {
            var cart = GetActiveCart();
            if (cart.Count == 0) return;

            string accountId = cmbCustomer.SelectedValue?.ToString() ?? "001002001001";

            var lines = new List<SaleLineRequest>();
            int seq = 1;
            foreach (var item in cart)
            {
                lines.Add(new SaleLineRequest
                {
                    Seq = seq++,
                    ItemId = item.ItemId,
                    Qty = item.Qty,
                    Rate = item.Rate,
                    Discount = item.Discount
                });
            }

            var request = new SaleCreateRequest
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Account = accountId,
                Description = "Touch Screen POS Sale",
                CashReceipt = cashReceived,
                CashBack = cashBack,
                Lines = lines
            };

            btnPay.Enabled = false;
            try
            {
                string voucherNo = await _saleApiService.CreateAsync(request);

                if (!string.IsNullOrWhiteSpace(voucherNo))
                {
                    if (printReceipt)
                    {
                        try
                        {
                            await SaleReceiptViewer.PrintOrPreviewAsync(voucherNo, isDirect: true, preferredLayout: SaleReceiptPrintLayout.Thermal80mm);
                        }
                        catch (Exception printEx)
                        {
                            MessageBox.Show("Sale saved (#" + voucherNo + "), but receipt printing failed: " + printEx.Message, "Print Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                    ScannerToneHelper.PlaySuccess();
                    MessageBox.Show($"Sale Voucher #{voucherNo} successfully completed!", "Sale Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearCurrentCart();
                }
                else
                {
                    MessageBox.Show("Server did not return a voucher number.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save POS sale: " + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnPay.Enabled = true;
                txtBarcode.Focus();
            }
        }

        private void FrmTouchPOS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                e.Handled = true;
                ResetActiveOrder();
            }
            else if (e.KeyCode == Keys.F2)
            {
                e.Handled = true;
                txtBarcode.Focus();
                txtBarcode.SelectAll();
            }
            else if (e.KeyCode == Keys.F6)
            {
                e.Handled = true;
                _ = OpenPaymentDialogAsync();
            }
            else if (e.KeyCode == Keys.F7)
            {
                e.Handled = true;
                int nextTab = (_activeOrderIndex % 3) + 1;
                SwitchOrderTab(nextTab);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                ConfirmCloseForm();
            }
        }

        private void ConfirmCloseForm()
        {
            var cart = GetActiveCart();
            if (cart.Count > 0)
            {
                if (MessageBox.Show("Cart has items in progress. Are you sure you want to exit Touch POS?", "Exit POS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
            }
            this.Close();
        }
    }
}
