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
        private readonly UnitApiService _unitApiService;

        private DataTable _dtUnits = new DataTable();
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
        private DataGridViewTextBoxColumn colTitle;
        private DataGridViewButtonColumn colUnit;
        private DataGridViewTextBoxColumn colRate;
        private DataGridViewTextBoxColumn colQty;
        private DataGridViewTextBoxColumn colTotal;
        private DataGridViewButtonColumn colDec;
        private DataGridViewButtonColumn colInc;
        private DataGridViewButtonColumn colDel;

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

        public class SelectableUnitOption
        {
            public string UnitId { get; set; }
            public string Title { get; set; }
            public decimal Factor { get; set; }
        }

        public class TouchCartItem
        {
            public string ItemId { get; set; }
            public string Title { get; set; }
            public string ItemCode { get; set; }
            public string Barcode { get; set; }
            public decimal BaseRate { get; set; }
            public decimal Discount { get; set; }

            public List<SelectableUnitOption> SelectableUnits { get; set; } = new List<SelectableUnitOption>();
            public string SelectedUnitId { get; set; }
            public string SelectedUnitTitle { get; set; }
            public decimal SelectedUnitFactor { get; set; } = 1m;

            // Effective rate per selected display unit
            public decimal Rate
            {
                get
                {
                    if (SelectedUnitFactor > 1m)
                    {
                        return Math.Round(BaseRate / SelectedUnitFactor, 2);
                    }
                    return BaseRate;
                }
                set
                {
                    if (SelectedUnitFactor <= 1m)
                    {
                        BaseRate = value;
                    }
                }
            }

            public string DisplayUnitLabel => (SelectableUnits != null && SelectableUnits.Count > 1)
                ? $"{SelectedUnitTitle} ▾"
                : (SelectedUnitTitle ?? "Pcs");

            // DisplayQty is the directly editable quantity (e.g. 2 Boxes or 10 Pcs)
            public decimal DisplayQty { get; set; } = 1m;

            // Qty is the converted base primary quantity for backend inventory ledger deduction
            public decimal Qty => SelectedUnitFactor > 1m
                ? Math.Round(DisplayQty / SelectedUnitFactor, 4, MidpointRounding.AwayFromZero)
                : DisplayQty;

            public decimal LineTotal => Math.Max(0, Math.Round(DisplayQty * Rate - Discount, 2));
        }

        public frmTouchPOS()
        {
            _saleApiService = new SaleApiService();
            _inventoryApiService = new InventoryApiService();
            _categoryApiService = new ItemCategoryApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _unitApiService = new UnitApiService();

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
                AutoGenerateColumns = false,
                EditMode = DataGridViewEditMode.EditOnEnter
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
            colTitle = new DataGridViewTextBoxColumn
            {
                HeaderText = "Item",
                DataPropertyName = "Title",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };
            colUnit = new DataGridViewButtonColumn
            {
                HeaderText = "Unit",
                DataPropertyName = "DisplayUnitLabel",
                Width = 62,
                FlatStyle = FlatStyle.Flat
            };
            colRate = new DataGridViewTextBoxColumn
            {
                HeaderText = "Rate",
                DataPropertyName = "Rate",
                Width = 58,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" }
            };
            colQty = new DataGridViewTextBoxColumn
            {
                HeaderText = "Qty",
                DataPropertyName = "DisplayQty",
                Width = 60,
                ReadOnly = false, // Directly editable text field!
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    BackColor = Color.FromArgb(240, 249, 255), // Highlighted so cashier knows it's editable
                    SelectionBackColor = Color.FromArgb(186, 230, 253),
                    SelectionForeColor = Color.FromArgb(15, 23, 42)
                }
            };
            colTotal = new DataGridViewTextBoxColumn
            {
                HeaderText = "Total",
                DataPropertyName = "LineTotal",
                Width = 72,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Format = "N0" }
            };

            colDec = new DataGridViewButtonColumn
            {
                HeaderText = "-",
                Text = "➖",
                UseColumnTextForButtonValue = true,
                Width = 36,
                FlatStyle = FlatStyle.Flat
            };
            colInc = new DataGridViewButtonColumn
            {
                HeaderText = "+",
                Text = "➕",
                UseColumnTextForButtonValue = true,
                Width = 36,
                FlatStyle = FlatStyle.Flat
            };
            colDel = new DataGridViewButtonColumn
            {
                HeaderText = "✕",
                Text = "✕",
                UseColumnTextForButtonValue = true,
                Width = 36,
                FlatStyle = FlatStyle.Flat
            };

            dgvCart.Columns.AddRange(colTitle, colUnit, colRate, colQty, colTotal, colDec, colInc, colDel);
            dgvCart.CellContentClick += DgvCart_CellContentClick;
            dgvCart.CellEndEdit += DgvCart_CellEndEdit;
            dgvCart.CellFormatting += DgvCart_CellFormatting;
            dgvCart.DataError += (s, e) => { e.Cancel = false; };

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

                // 2. Fetch Units (for sub-unit conversion)
                try
                {
                    var unitsList = await _unitApiService.GetLookupAsync();
                    _dtUnits = UnitConversionHelper.CreateUnitsTable(unitsList);
                }
                catch (Exception unitEx)
                {
                    System.Diagnostics.Debug.WriteLine("Units lookup load error: " + unitEx.Message);
                }

                // 3. Fetch Categories
                _categories = await _categoryApiService.GetLookupAsync();
                RenderCategoryButtons();

                // 4. Fetch Items
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
                if (!string.IsNullOrWhiteSpace(cat.MediaUrl))
                {
                    _ = LoadCategoryIconAsync(btnCat, cat.MediaUrl);
                }
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

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Image> _imageCache =
            new System.Collections.Concurrent.ConcurrentDictionary<string, Image>(StringComparer.OrdinalIgnoreCase);

        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = delegate { return true; }
        });

        private async Task LoadImageIntoPictureBoxAsync(PictureBox pic, string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            if (_imageCache.TryGetValue(url, out Image cached))
            {
                if (!pic.IsDisposed) pic.Image = cached;
                return;
            }

            try
            {
                byte[] bytes;
                if (url.Contains(":\\") || url.StartsWith("file://") || System.IO.File.Exists(url))
                {
                    bytes = System.IO.File.ReadAllBytes(url);
                }
                else
                {
                    bytes = await _httpClient.GetByteArrayAsync(url);
                }

                if (bytes != null && bytes.Length > 0)
                {
                    using (var ms = new System.IO.MemoryStream(bytes))
                    {
                        var original = Image.FromStream(ms);
                        var bmp = new Bitmap(original);
                        _imageCache[url] = bmp;
                        if (!pic.IsDisposed)
                        {
                            pic.BeginInvoke(new Action(() =>
                            {
                                if (!pic.IsDisposed) pic.Image = bmp;
                            }));
                        }
                    }
                }
            }
            catch
            {
                // Silently leave placeholder if image fails to load
            }
        }

        private async Task LoadCategoryIconAsync(Button btn, string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            if (_imageCache.TryGetValue(url, out Image cached))
            {
                if (!btn.IsDisposed)
                {
                    btn.Image = new Bitmap(cached, new Size(24, 24));
                    btn.ImageAlign = ContentAlignment.MiddleLeft;
                    btn.TextImageRelation = TextImageRelation.ImageBeforeText;
                }
                return;
            }

            try
            {
                byte[] bytes;
                if (url.Contains(":\\") || url.StartsWith("file://") || System.IO.File.Exists(url))
                {
                    bytes = System.IO.File.ReadAllBytes(url);
                }
                else
                {
                    bytes = await _httpClient.GetByteArrayAsync(url);
                }

                if (bytes != null && bytes.Length > 0)
                {
                    using (var ms = new System.IO.MemoryStream(bytes))
                    {
                        var original = Image.FromStream(ms);
                        var bmp = new Bitmap(original);
                        _imageCache[url] = bmp;
                        if (!btn.IsDisposed)
                        {
                            btn.BeginInvoke(new Action(() =>
                            {
                                if (!btn.IsDisposed)
                                {
                                    btn.Image = new Bitmap(bmp, new Size(24, 24));
                                    btn.ImageAlign = ContentAlignment.MiddleLeft;
                                    btn.TextImageRelation = TextImageRelation.ImageBeforeText;
                                }
                            }));
                        }
                    }
                }
            }
            catch { }
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
                    Size = new Size(146, 185),
                    Margin = new Padding(6),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Hand
                };

                // Product Image
                PictureBox pic = new PictureBox
                {
                    Location = new Point(4, 4),
                    Size = new Size(136, 95),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.FromArgb(248, 250, 252),
                    Cursor = Cursors.Hand
                };

                // Set placeholder first
                try
                {
                    pic.Image = ERP.Controls.ProfileImage.CreateProductPlaceholder(136, 95);
                }
                catch { }

                if (!string.IsNullOrWhiteSpace(currentItem.MediaUrl))
                {
                    _ = LoadImageIntoPictureBoxAsync(pic, currentItem.MediaUrl);
                }

                Label lblTitle = new Label
                {
                    Text = currentItem.Title ?? "Item",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    Location = new Point(5, 102),
                    Size = new Size(134, 34),
                    TextAlign = ContentAlignment.TopLeft,
                    AutoEllipsis = true,
                    Cursor = Cursors.Hand
                };

                Label lblUnit = new Label
                {
                    Text = currentItem.DefaultUnitTitle ?? currentItem.PrimaryUnit ?? "Pcs",
                    Font = new Font("Segoe UI", 7.5F, FontStyle.Regular),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Location = new Point(5, 138),
                    Size = new Size(134, 16),
                    Cursor = Cursors.Hand
                };

                Label lblPrice = new Label
                {
                    Text = "Rs. " + currentItem.PriRate.ToString("N0", CultureInfo.InvariantCulture),
                    Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(2, 132, 199), // Sky-600
                    Location = new Point(5, 154),
                    Size = new Size(134, 25),
                    TextAlign = ContentAlignment.BottomRight,
                    Cursor = Cursors.Hand
                };

                card.Controls.Add(pic);
                card.Controls.Add(lblTitle);
                card.Controls.Add(lblUnit);
                card.Controls.Add(lblPrice);

                Action clickAction = () => AddOrIncrementItem(currentItem);

                card.Click += (s, e) => clickAction();
                pic.Click += (s, e) => clickAction();
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

        private List<SelectableUnitOption> GetSelectableUnitsForItem(InventoryItemDto item)
        {
            var list = new List<SelectableUnitOption>();
            string primaryUnit = !string.IsNullOrWhiteSpace(item.PrimaryUnit) ? item.PrimaryUnit : (item.DefaultUnit ?? "Pcs");
            string primaryTitle = !string.IsNullOrWhiteSpace(item.DefaultUnitTitle) ? item.DefaultUnitTitle : primaryUnit;

            if (_dtUnits != null && _dtUnits.Rows.Count > 0)
            {
                var priRows = _dtUnits.Select("Code = '" + primaryUnit.Replace("'", "''") + "'");
                if (priRows.Length > 0)
                {
                    string st = _dtUnits.Columns.Contains("ShortTitle") ? Convert.ToString(priRows[0]["ShortTitle"]).Trim() : "";
                    string t = Convert.ToString(priRows[0]["Title"]).Trim();
                    primaryTitle = !string.IsNullOrEmpty(st) ? st : t;
                }
            }

            list.Add(new SelectableUnitOption
            {
                UnitId = primaryUnit,
                Title = primaryTitle,
                Factor = 1m
            });

            var subUnits = UnitConversionHelper.GetEffectiveSubUnits(primaryUnit, item.AdditionalUnits, _dtUnits);
            if (subUnits != null)
            {
                foreach (var sub in subUnits)
                {
                    if (sub.Factor > 1m && !list.Any(u => u.UnitId == sub.UnitId))
                    {
                        list.Add(new SelectableUnitOption
                        {
                            UnitId = sub.UnitId,
                            Title = !string.IsNullOrWhiteSpace(sub.ShortTitle) ? sub.ShortTitle : sub.UnitTitle,
                            Factor = sub.Factor
                        });
                    }
                }
            }

            return list;
        }

        private void AddOrIncrementItem(InventoryItemDto item)
        {
            var cart = GetActiveCart();
            var existing = cart.FirstOrDefault(c => c.ItemId == item.Id);

            if (existing != null)
            {
                existing.DisplayQty += 1;
            }
            else
            {
                var selectableUnits = GetSelectableUnitsForItem(item);
                var defaultOpt = selectableUnits.FirstOrDefault() ?? new SelectableUnitOption
                {
                    UnitId = item.PrimaryUnit ?? "Pcs",
                    Title = item.DefaultUnitTitle ?? item.PrimaryUnit ?? "Pcs",
                    Factor = 1m
                };

                cart.Add(new TouchCartItem
                {
                    ItemId = item.Id,
                    Title = item.Title,
                    ItemCode = item.ItemKey,
                    Barcode = item.Barcode,
                    BaseRate = item.PriRate,
                    Rate = item.PriRate,
                    Discount = 0,
                    SelectableUnits = selectableUnits,
                    SelectedUnitId = defaultOpt.UnitId,
                    SelectedUnitTitle = defaultOpt.Title,
                    SelectedUnitFactor = defaultOpt.Factor,
                    DisplayQty = 1m
                });
            }

            ScannerToneHelper.PlaySuccess();
            RefreshCartGrid();
            txtBarcode.Focus();
        }

        private void RefreshCartGrid()
        {
            if (this.IsDisposed) return;

            Action action = () =>
            {
                if (this.IsDisposed || dgvCart.IsDisposed) return;

                try
                {
                    dgvCart.EndEdit();
                }
                catch { }

                var cart = GetActiveCart();
                int prevRow = -1;
                int prevCol = -1;
                try
                {
                    if (dgvCart.CurrentCell != null)
                    {
                        prevRow = dgvCart.CurrentCell.RowIndex;
                        prevCol = dgvCart.CurrentCell.ColumnIndex;
                    }
                }
                catch { }

                try
                {
                    dgvCart.DataSource = null;
                    dgvCart.DataSource = cart.ToList();

                    if (prevRow >= 0 && prevRow < dgvCart.Rows.Count && prevCol >= 0 && prevCol < dgvCart.Columns.Count)
                    {
                        dgvCart.CurrentCell = dgvCart.Rows[prevRow].Cells[prevCol];
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Cart grid refresh error: " + ex.Message);
                }

                RecalculateTotals();
            };

            if (this.IsHandleCreated)
            {
                this.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        private void RecalculateTotals()
        {
            var cart = GetActiveCart();
            int itemCount = cart.Count;
            decimal totalQty = cart.Sum(i => i.DisplayQty);
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

            // Unit column: Cycle through available sub-units
            if (dgvCart.Columns[e.ColumnIndex] == colUnit)
            {
                if (item.SelectableUnits != null && item.SelectableUnits.Count > 1)
                {
                    int currentIndex = item.SelectableUnits.FindIndex(u => u.UnitId == item.SelectedUnitId);
                    int nextIndex = (currentIndex + 1) % item.SelectableUnits.Count;
                    var nextUnit = item.SelectableUnits[nextIndex];

                    // Convert current base quantity to new unit display quantity
                    decimal currentBaseQty = item.Qty;
                    item.SelectedUnitId = nextUnit.UnitId;
                    item.SelectedUnitTitle = nextUnit.Title;
                    item.SelectedUnitFactor = nextUnit.Factor;
                    item.DisplayQty = Math.Max(0.0001m, Math.Round(currentBaseQty * nextUnit.Factor, 4));

                    RefreshCartGrid();
                }
                return;
            }

            if (dgvCart.Columns[e.ColumnIndex] == colDec) // Minus
            {
                if (item.DisplayQty > 1)
                {
                    item.DisplayQty -= 1;
                }
                else
                {
                    cart.RemoveAt(e.RowIndex);
                }
                RefreshCartGrid();
            }
            else if (dgvCart.Columns[e.ColumnIndex] == colInc) // Plus
            {
                item.DisplayQty += 1;
                RefreshCartGrid();
            }
            else if (dgvCart.Columns[e.ColumnIndex] == colDel) // Delete
            {
                cart.RemoveAt(e.RowIndex);
                RefreshCartGrid();
            }
        }

        private void DgvCart_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvCart.Rows.Count) return;

            var cart = GetActiveCart();
            if (e.RowIndex >= cart.Count) return;

            var item = cart[e.RowIndex];

            if (dgvCart.Columns[e.ColumnIndex] == colQty)
            {
                var cellVal = dgvCart.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (cellVal != null && decimal.TryParse(cellVal.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal enteredQty))
                {
                    if (enteredQty > 0)
                    {
                        item.DisplayQty = enteredQty;
                    }
                    else
                    {
                        cart.RemoveAt(e.RowIndex);
                    }
                }
                RefreshCartGrid();
            }
        }

        private void DgvCart_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvCart.Rows.Count) return;

            var cart = GetActiveCart();
            if (e.RowIndex >= cart.Count) return;

            var item = cart[e.RowIndex];

            if (dgvCart.Columns[e.ColumnIndex] == colUnit)
            {
                if (item.SelectableUnits != null && item.SelectableUnits.Count > 1)
                {
                    e.CellStyle.BackColor = Color.FromArgb(238, 242, 255); // Indigo-50
                    e.CellStyle.ForeColor = Color.FromArgb(79, 70, 229);   // Indigo-600
                    e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                }
            }
            else if (dgvCart.Columns[e.ColumnIndex] == colDel)
            {
                e.CellStyle.ForeColor = Color.FromArgb(220, 38, 38);
                e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
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
                    Qty = item.Qty, // Base primary quantity for ledger deduction
                    Rate = item.BaseRate, // Primary unit rate
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
