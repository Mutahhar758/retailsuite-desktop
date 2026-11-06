using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Reporting.Documents;
using ERP.Reporting.Models;
using ERP.Services.Legacy;
using Microsoft.Web.WebView2.WinForms;

namespace ERP.Reporting
{
    /// <summary>
    /// Modern WinForms viewer for generating and printing product barcode price stickers.
    /// Supports thermal roll sticker printing and standard A4 multi-label sheets.
    /// </summary>
    public class BarcodeViewer : Form
    {
        private readonly InventoryApiService _inventoryApiService;
        private readonly ItemCategoryApiService _itemCategoryApiService;
        private readonly UnitApiService _unitApiService;
        private readonly Dictionary<string, string> _unitsMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // UI Controls - Sidebar
        private Panel pnlSidebar;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblItem;
        private ComboBox cmbItem;
        private Label lblCopies;
        private NumericUpDown nudCopies;
        private Label lblPrice;
        private TextBox txtPrice;
        private CheckBox chkVariableWeight;
        private Label lblWeightQty;
        private NumericUpDown nudWeightQty;
        private Label lblUnitName;
        private Label lblCalcAmount;
        private TextBox txtCalcAmount;
        private Label lblCalcRate;
        private TextBox txtCalcRate;
        private Label lblCalcHelp;
        private bool _isSyncingAmounts = false;
        private CheckBox chkIncludeCompanySubHeader;
        private CheckBox chkIncludePrice;
        private CheckBox chkIncludeRate;
        private Label lblLayout;
        private ComboBox cmbLayout;
        private Label lblSpread;
        private NumericUpDown nudSpread;
        private Label lblSpreadHelp;
        private Label lblPrinter;
        private ComboBox cmbPrinter;
        private Button btnPreview;
        private Button btnPrintDirect;
        private Button btnPrintDialog;

        // UI Controls - Canvas
        private Panel pnlCanvas;
        private Label lblStatus;
        private ProgressBar prgLoading;
        private WebView2 webView;

        // State
        private List<InventoryItemDto> _allItems = new List<InventoryItemDto>();
        private List<ItemCategoryDto> _allCategories = new List<ItemCategoryDto>();
        private List<UnitLookupDto> _allUnits = new List<UnitLookupDto>();
        private BarcodeLabelItem _currentItem;
        private string _currentPdfPath;
        private bool _isWebViewReady = false;

        public BarcodeViewer()
        {
            _inventoryApiService = new InventoryApiService();
            _itemCategoryApiService = new ItemCategoryApiService();
            _unitApiService = new UnitApiService();

            InitializeComponentCodeFirst();
            this.Load += async (s, e) => await InitializeDataAsync();
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Barcode";
            this.Size = new Size(1160, 760);
            this.MinimumSize = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.BackColor = Color.FromArgb(248, 250, 252);

            // 1. Sidebar (Dock Left)
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 360,
                BackColor = Color.White,
                Padding = new Padding(14),
                AutoScroll = true
            };
            pnlSidebar.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height);
            };

            int curY = 14;

            var lblTitle = new Label
            {
                Text = "BARCODE LABEL CONFIGURATION",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };
            pnlSidebar.Controls.Add(lblTitle);
            curY += 28;

            // Category Filter
            lblCategory = new Label
            {
                Text = "CATEGORY FILTER:",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblCategory);
            curY += 18;

            cmbCategory = new ComboBox
            {
                Location = new Point(14, curY),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbCategory.SelectedIndexChanged += (s, e) => FilterItemsByCategory();
            pnlSidebar.Controls.Add(cmbCategory);
            curY += 34;

            // Item Dropdown
            lblItem = new Label
            {
                Text = "PRODUCT / ITEM:",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblItem);
            curY += 18;

            cmbItem = new ComboBox
            {
                Location = new Point(14, curY),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbItem.SelectedIndexChanged += (s, e) => OnItemSelected();
            pnlSidebar.Controls.Add(cmbItem);
            curY += 34;

            // Copies & Standard Price Row
            lblCopies = new Label
            {
                Text = "STICKER COPIES:",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblCopies);

            lblPrice = new Label
            {
                Text = "PRINT PRICE (RS):",
                Location = new Point(180, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblPrice);
            curY += 18;

            nudCopies = new NumericUpDown
            {
                Location = new Point(14, curY),
                Width = 150,
                Minimum = 1,
                Maximum = 5000,
                Value = 3,
                Font = new Font("Segoe UI", 9F)
            };
            pnlSidebar.Controls.Add(nudCopies);

            txtPrice = new TextBox
            {
                Location = new Point(180, curY),
                Width = 164,
                Font = new Font("Segoe UI", 9F)
            };
            pnlSidebar.Controls.Add(txtPrice);
            curY += 34;

            // Variable Weight Checkbox
            chkVariableWeight = new CheckBox
            {
                Text = "Variable Weight / Quantity (Put Qty in Barcode)",
                Location = new Point(14, curY),
                Width = 330,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };
            pnlSidebar.Controls.Add(chkVariableWeight);
            curY += 26;

            // Variable Weight Controls: Qty & Amount to calculate Qty
            lblWeightQty = new Label
            {
                Text = "QTY (PRIMARY UNIT):",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Visible = false
            };
            pnlSidebar.Controls.Add(lblWeightQty);

            lblCalcAmount = new Label
            {
                Text = "CALC FROM AMOUNT (RS):",
                Location = new Point(180, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Visible = false
            };
            pnlSidebar.Controls.Add(lblCalcAmount);
            curY += 18;

            nudWeightQty = new NumericUpDown
            {
                Location = new Point(14, curY),
                Width = 110,
                DecimalPlaces = 3,
                Minimum = 0.001m,
                Maximum = 9999m,
                Value = 1.000m,
                Increment = 0.1m,
                Font = new Font("Segoe UI", 9F),
                Visible = false
            };
            pnlSidebar.Controls.Add(nudWeightQty);

            lblUnitName = new Label
            {
                Text = "kg",
                Location = new Point(128, curY + 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(71, 85, 105),
                Visible = false
            };
            pnlSidebar.Controls.Add(lblUnitName);

            txtCalcAmount = new TextBox
            {
                Location = new Point(180, curY),
                Width = 164,
                Font = new Font("Segoe UI", 9F),
                Visible = false
            };
            pnlSidebar.Controls.Add(txtCalcAmount);
            curY += 30;

            lblCalcRate = new Label
            {
                Text = "ITEM RATE FOR CALC (RS):",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Visible = false
            };
            pnlSidebar.Controls.Add(lblCalcRate);
            curY += 18;

            txtCalcRate = new TextBox
            {
                Location = new Point(14, curY),
                Width = 330,
                Font = new Font("Segoe UI", 9F),
                Visible = false
            };
            pnlSidebar.Controls.Add(txtCalcRate);
            curY += 28;

            lblCalcHelp = new Label
            {
                Text = "* Enter amount to auto-calculate qty. Neither amount nor price will be printed.",
                Location = new Point(14, curY),
                Width = 330,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Visible = false
            };
            pnlSidebar.Controls.Add(lblCalcHelp);
            curY += 26;

            chkVariableWeight.CheckedChanged += (s, e) =>
            {
                bool isVar = chkVariableWeight.Checked;
                lblWeightQty.Visible = isVar;
                nudWeightQty.Visible = isVar;
                lblUnitName.Visible = isVar;
                lblCalcAmount.Visible = isVar;
                txtCalcAmount.Visible = isVar;
                lblCalcRate.Visible = isVar;
                txtCalcRate.Visible = isVar;
                lblCalcHelp.Visible = isVar;

                lblPrice.Visible = !isVar;
                txtPrice.Visible = !isVar;
                nudCopies.Width = isVar ? 330 : 150;

                if (isVar)
                {
                    if (!string.IsNullOrWhiteSpace(txtPrice.Text))
                    {
                        txtCalcRate.Text = txtPrice.Text;
                    }
                    SyncQtyFromRateAndAmount();
                }
            };

            nudWeightQty.ValueChanged += (s, e) =>
            {
                if (_isSyncingAmounts || !chkVariableWeight.Checked) return;
                try
                {
                    _isSyncingAmounts = true;
                    if (decimal.TryParse(txtCalcRate.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rate) && rate > 0)
                    {
                        txtCalcAmount.Text = Math.Round(nudWeightQty.Value * rate).ToString("0");
                    }
                }
                finally
                {
                    _isSyncingAmounts = false;
                }
            };

            txtCalcAmount.TextChanged += (s, e) =>
            {
                if (_isSyncingAmounts || !chkVariableWeight.Checked) return;
                try
                {
                    _isSyncingAmounts = true;
                    if (decimal.TryParse(txtCalcAmount.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amt) &&
                        decimal.TryParse(txtCalcRate.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rate) && rate > 0)
                    {
                        decimal calculatedQty = Math.Max(0.001m, Math.Round(amt / rate, 3));
                        if (calculatedQty <= nudWeightQty.Maximum)
                        {
                            nudWeightQty.Value = calculatedQty;
                        }
                    }
                }
                finally
                {
                    _isSyncingAmounts = false;
                }
            };

            txtCalcRate.TextChanged += (s, e) =>
            {
                if (_isSyncingAmounts || !chkVariableWeight.Checked) return;
                try
                {
                    _isSyncingAmounts = true;
                    if (decimal.TryParse(txtCalcAmount.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amt) &&
                        decimal.TryParse(txtCalcRate.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rate) && rate > 0)
                    {
                        decimal calculatedQty = Math.Max(0.001m, Math.Round(amt / rate, 3));
                        if (calculatedQty <= nudWeightQty.Maximum)
                        {
                            nudWeightQty.Value = calculatedQty;
                        }
                    }
                }
                finally
                {
                    _isSyncingAmounts = false;
                }
            };

            // Layout Format
            lblLayout = new Label
            {
                Text = "PRINT FORMAT:",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblLayout);
            curY += 18;

            cmbLayout = new ComboBox
            {
                Location = new Point(14, curY),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbLayout.Items.AddRange(new object[]
            {
                "4\" Thermal Roll (3 Across - 3-Up)",
                "Thermal Sticker Roll (50 x 30 mm)",
                "A4 Sticker Sheet (24 labels per sheet)"
            });
            cmbLayout.SelectedIndex = 0;
            cmbLayout.SelectedIndexChanged += (s, e) =>
            {
                var lyt = GetSelectedLayout();
                bool is3Up = (lyt == BarcodePrintLayout.Roll4Inch3Up);
                if (lblSpread != null) lblSpread.Visible = is3Up;
                if (nudSpread != null) nudSpread.Visible = is3Up;
                if (lblSpreadHelp != null) lblSpreadHelp.Visible = is3Up;

                string preferredPrinter = (lyt == BarcodePrintLayout.Roll4Inch3Up || lyt == BarcodePrintLayout.ThermalRoll)
                    ? ConfigInfo.ThermalPrinterName
                    : ConfigInfo.A4PrinterName;
                if (!string.IsNullOrWhiteSpace(preferredPrinter) && cmbPrinter != null && cmbPrinter.Items.Contains(preferredPrinter))
                {
                    cmbPrinter.SelectedItem = preferredPrinter;
                }
            };
            pnlSidebar.Controls.Add(cmbLayout);
            curY += 34;

            // 3-Up Outer Margin Offset / Spread Control
            lblSpread = new Label
            {
                Text = "3-UP SIDE STICKERS MARGIN / SPREAD (MM):",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblSpread);
            curY += 18;

            nudSpread = new NumericUpDown
            {
                Location = new Point(14, curY),
                Width = 100,
                DecimalPlaces = 1,
                Minimum = 0.0m,
                Maximum = 6.0m,
                Increment = 0.5m,
                Value = 2.0m,
                Font = new Font("Segoe UI", 9F)
            };
            try
            {
                string savedSpread = INIFile.ReadValue("PrinterSetting", "Barcode3UpSideSpread");
                if (decimal.TryParse(savedSpread, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal sVal))
                {
                    nudSpread.Value = Math.Max(nudSpread.Minimum, Math.Min(nudSpread.Maximum, sVal));
                }
            }
            catch { }
            nudSpread.ValueChanged += (s, e) =>
            {
                try
                {
                    INIFile.WriteValue("PrinterSetting", "Barcode3UpSideSpread", nudSpread.Value.ToString("0.0", CultureInfo.InvariantCulture));
                }
                catch { }
            };
            pnlSidebar.Controls.Add(nudSpread);

            lblSpreadHelp = new Label
            {
                Text = "Decreases outer margins (Default: 2.0 mm)",
                Location = new Point(120, curY + 2),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblSpreadHelp);
            curY += 30;

            chkIncludeCompanySubHeader = new CheckBox
            {
                Text = "Show Tagline / Address on Sticker",
                Location = new Point(14, curY),
                Width = 330,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(71, 85, 105),
                Checked = true
            };
            pnlSidebar.Controls.Add(chkIncludeCompanySubHeader);
            curY += 26;

            chkIncludePrice = new CheckBox
            {
                Text = "Show Price / Amount on Sticker",
                Location = new Point(14, curY),
                Width = 330,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(71, 85, 105),
                Checked = true
            };
            pnlSidebar.Controls.Add(chkIncludePrice);
            curY += 24;

            chkIncludeRate = new CheckBox
            {
                Text = "Show Unit Rate on Sticker (e.g. Rs/kg)",
                Location = new Point(14, curY),
                Width = 330,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(71, 85, 105),
                Checked = false
            };
            pnlSidebar.Controls.Add(chkIncludeRate);
            curY += 26;

            // Destination Printer
            lblPrinter = new Label
            {
                Text = "PRINTER (FOR DIRECT SILENT PRINT):",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblPrinter);
            curY += 18;

            cmbPrinter = new ComboBox
            {
                Location = new Point(14, curY),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            pnlSidebar.Controls.Add(cmbPrinter);
            curY += 36;

            // Action Buttons
            btnPreview = new Button
            {
                Text = "Preview Labels",
                Location = new Point(14, curY),
                Width = 160,
                Height = 34,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPreview.FlatAppearance.BorderSize = 0;
            btnPreview.Click += async (s, e) => await LoadAndRenderBarcodeAsync();
            pnlSidebar.Controls.Add(btnPreview);

            btnPrintDirect = new Button
            {
                Text = "🖨 Print Direct",
                Location = new Point(184, curY),
                Width = 160,
                Height = 34,
                BackColor = Color.FromArgb(5, 150, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrintDirect.FlatAppearance.BorderSize = 0;
            btnPrintDirect.Click += async (s, e) => await PrintDirectAsync();
            pnlSidebar.Controls.Add(btnPrintDirect);
            curY += 40;

            btnPrintDialog = new Button
            {
                Text = "Print with Dialog...",
                Location = new Point(14, curY),
                Width = 330,
                Height = 28,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrintDialog.FlatAppearance.BorderSize = 0;
            btnPrintDialog.Click += (s, e) =>
            {
                if (webView != null && _isWebViewReady)
                    webView.CoreWebView2.ShowPrintUI(Microsoft.Web.WebView2.Core.CoreWebView2PrintDialogKind.Browser);
            };
            pnlSidebar.Controls.Add(btnPrintDialog);

            this.Controls.Add(pnlSidebar);

            // 2. Canvas Area
            pnlCanvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(12)
            };

            lblStatus = new Label
            {
                Text = "Select a product item and click 'Preview Labels'.",
                AutoSize = true,
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlCanvas.Controls.Add(lblStatus);

            prgLoading = new ProgressBar
            {
                Location = new Point(20, 48),
                Width = 240,
                Height = 12,
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };
            pnlCanvas.Controls.Add(prgLoading);

            webView = new WebView2
            {
                Dock = DockStyle.Fill,
                Visible = false
            };
            pnlCanvas.Controls.Add(webView);

            this.Controls.Add(pnlCanvas);
            pnlCanvas.BringToFront();
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                // Printers
                cmbPrinter.Items.Clear();
                foreach (string p in PrinterSettings.InstalledPrinters) cmbPrinter.Items.Add(p);

                if (!string.IsNullOrWhiteSpace(ConfigInfo.ThermalPrinterName) && cmbPrinter.Items.Contains(ConfigInfo.ThermalPrinterName))
                    cmbPrinter.SelectedItem = ConfigInfo.ThermalPrinterName;
                else if (cmbPrinter.Items.Count > 0)
                    cmbPrinter.SelectedIndex = 0;

                // Load Categories
                var categories = await _itemCategoryApiService.GetLookupAsync();
                _allCategories = categories ?? new List<ItemCategoryDto>();
                cmbCategory.Items.Clear();
                cmbCategory.Items.Add("--- All Categories ---");
                foreach (var c in _allCategories) cmbCategory.Items.Add(c.Title);
                cmbCategory.SelectedIndex = 0;

                // Load Units for Title Resolution & Sub-Unit Structure
                try
                {
                    var units = await _unitApiService.GetLookupAsync();
                    _allUnits = units ?? new List<UnitLookupDto>();
                    if (units != null)
                    {
                        foreach (var u in units)
                        {
                            if (!string.IsNullOrWhiteSpace(u.Code) && !string.IsNullOrWhiteSpace(u.Title))
                            {
                                _unitsMap[u.Code] = u.Title;
                            }
                        }
                    }
                }
                catch { }

                // Load Items
                var items = await _inventoryApiService.GetLookupAsync(null);
                _allItems = items ?? new List<InventoryItemDto>();
                FilterItemsByCategory();

                // Initialize WebView2
                string userDataFolder = Path.Combine(Path.GetTempPath(), "RetailSuite", "WebView2_Barcode");
                if (!Directory.Exists(userDataFolder)) Directory.CreateDirectory(userDataFolder);
                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);
                _isWebViewReady = true;

                if (cmbItem.Items.Count > 0)
                {
                    cmbItem.SelectedIndex = 0;
                }
                lblStatus.Text = "Ready. Select item & click 'Generate Preview'.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error initializing barcode data: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void FilterItemsByCategory()
        {
            cmbItem.DataSource = null;
            var list = _allItems;
            if (cmbCategory.SelectedIndex > 0)
            {
                string catName = cmbCategory.SelectedItem.ToString();
                var catObj = _allCategories.FirstOrDefault(c => c.Title == catName);
                if (catObj != null)
                {
                    list = _allItems.Where(i => string.Equals(i.ItemCategoryCode, catObj.Code, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }

            cmbItem.DisplayMember = "Title";
            cmbItem.ValueMember = "Id";
            cmbItem.DataSource = list;
            if (list.Count > 0) cmbItem.SelectedIndex = 0;
        }

        private void SyncQtyFromRateAndAmount()
        {
            if (_isSyncingAmounts) return;
            try
            {
                _isSyncingAmounts = true;
                if (decimal.TryParse(txtCalcAmount.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amt) &&
                    decimal.TryParse(txtCalcRate.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rate) && rate > 0)
                {
                    decimal calculatedQty = Math.Max(0.001m, Math.Round(amt / rate, 3));
                    if (calculatedQty <= nudWeightQty.Maximum)
                    {
                        nudWeightQty.Value = calculatedQty;
                    }
                }
                else if (decimal.TryParse(txtCalcRate.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal r) && r > 0)
                {
                    txtCalcAmount.Text = Math.Round(nudWeightQty.Value * r).ToString("0");
                }
            }
            finally
            {
                _isSyncingAmounts = false;
            }
        }

        private string GetUnitTitle(InventoryItemDto item)
        {
            if (item == null) return "kg";

            // 1. If item has populated unit title directly from API
            if (!string.IsNullOrWhiteSpace(item.PrimaryUnitTitle)) return item.PrimaryUnitTitle;
            if (!string.IsNullOrWhiteSpace(item.DefaultUnitTitle)) return item.DefaultUnitTitle;

            // 2. Lookup by code in unit dictionary
            string code = !string.IsNullOrWhiteSpace(item.PrimaryUnit) ? item.PrimaryUnit : item.DefaultUnit;
            if (!string.IsNullOrWhiteSpace(code))
            {
                if (_unitsMap.TryGetValue(code, out string title) && !string.IsNullOrWhiteSpace(title))
                {
                    return title;
                }
                if (!decimal.TryParse(code, out _))
                {
                    return code;
                }
            }

            return "kg";
        }

        private (decimal DisplayQty, string DisplayUnit) ResolveSubUnit(InventoryItemDto item, decimal baseQty)
        {
            if (item == null || baseQty <= 0m || baseQty % 1m == 0m)
            {
                return (baseQty, GetUnitTitle(item));
            }

            string primaryUnitCode = !string.IsNullOrWhiteSpace(item.PrimaryUnit) ? item.PrimaryUnit : item.DefaultUnit;
            if (string.IsNullOrWhiteSpace(primaryUnitCode))
            {
                return (baseQty, GetUnitTitle(item));
            }

            // 1. Check item-level additional units override if available
            if (!string.IsNullOrWhiteSpace(item.AdditionalUnits))
            {
                var explicitSubs = UnitConversionHelper.ParseAdditionalUnits(item.AdditionalUnits);
                if (explicitSubs != null && explicitSubs.Count > 0)
                {
                    foreach (var sub in explicitSubs)
                    {
                        if (sub.Factor > 1m)
                        {
                            decimal cand = baseQty * sub.Factor;
                            if (Math.Abs(cand - Math.Round(cand)) < 0.0001m)
                            {
                                string label = !string.IsNullOrWhiteSpace(sub.ShortTitle) ? sub.ShortTitle.Trim() : (!string.IsNullOrWhiteSpace(sub.UnitTitle) ? sub.UnitTitle.Trim() : sub.UnitId);
                                return (Math.Round(cand, 3), label);
                            }
                        }
                    }
                }
            }

            // 2. Query unit hierarchy from defined unit structure where ParentUnitId == primaryUnitCode
            if (_allUnits != null && _allUnits.Count > 0)
            {
                var childUnits = _allUnits.Where(u =>
                    (string.Equals(u.ParentUnitId, primaryUnitCode, StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(u.ParentUnitTitle, primaryUnitCode, StringComparison.OrdinalIgnoreCase)) &&
                    u.ConversionRatio.HasValue && u.ConversionRatio.Value > 1m)
                    .OrderBy(u => u.ConversionRatio.Value)
                    .ToList();

                foreach (var child in childUnits)
                {
                    decimal factor = child.ConversionRatio.Value;
                    decimal cand = baseQty * factor;
                    if (Math.Abs(cand - Math.Round(cand)) < 0.0001m)
                    {
                        string label = !string.IsNullOrWhiteSpace(child.ShortTitle) ? child.ShortTitle.Trim() : (!string.IsNullOrWhiteSpace(child.Title) ? child.Title.Trim() : child.Code);
                        return (Math.Round(cand, 3), label);
                    }
                }
            }

            return (baseQty, GetUnitTitle(item));
        }

        private string GetCompanySubHeader()
        {
            if (chkIncludeCompanySubHeader != null && !chkIncludeCompanySubHeader.Checked)
                return null;

            if (!string.IsNullOrWhiteSpace(CompanyInfo.Description))
                return CompanyInfo.Description.Trim();
            if (!string.IsNullOrWhiteSpace(CompanyInfo.Address))
                return CompanyInfo.Address.Trim();
            return null;
        }

        private void OnItemSelected()
        {
            var item = cmbItem.SelectedItem as InventoryItemDto;
            if (item != null)
            {
                string rStr = Math.Round(item.PriRate).ToString("0");
                if (txtPrice != null) txtPrice.Text = rStr;
                if (txtCalcRate != null) txtCalcRate.Text = rStr;
                string u = GetUnitTitle(item);
                if (lblUnitName != null) lblUnitName.Text = u;
                if (chkVariableWeight != null && chkVariableWeight.Checked)
                {
                    SyncQtyFromRateAndAmount();
                }
            }
        }

        private BarcodePrintLayout GetSelectedLayout()
        {
            if (cmbLayout.SelectedIndex == 1) return BarcodePrintLayout.ThermalRoll;
            if (cmbLayout.SelectedIndex == 2) return BarcodePrintLayout.SheetA4;
            return BarcodePrintLayout.Roll4Inch3Up;
        }

        private string GetLayoutDisplayName(BarcodePrintLayout layout)
        {
            switch (layout)
            {
                case BarcodePrintLayout.Roll4Inch3Up: return "4\" Thermal Roll (3 Across)";
                case BarcodePrintLayout.ThermalRoll: return "Thermal Roll (50x30mm)";
                case BarcodePrintLayout.SheetA4: return "A4 Sheet (24 labels)";
                default: return "4\" Thermal Roll (3 Across)";
            }
        }

        private async Task LoadAndRenderBarcodeAsync()
        {
            var item = cmbItem.SelectedItem as InventoryItemDto;
            if (item == null)
            {
                MessageBox.Show("Please select an item.", "Item Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int copies = (int)nudCopies.Value;
            var layout = GetSelectedLayout();

            string barcode;
            bool isVariable = chkVariableWeight.Checked;
            decimal? varQty = null;
            string varUnit = null;
            decimal rate = 0m;
            decimal totalAmount = 0m;

            if (isVariable)
            {
                decimal qty = nudWeightQty.Value;
                decimal.TryParse(txtCalcRate.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out rate);
                rate = Math.Round(rate);
                totalAmount = Math.Round(qty * rate);

                string rawBase = !string.IsNullOrWhiteSpace(item.Barcode) ? item.Barcode : (!string.IsNullOrWhiteSpace(item.ItemKey) ? item.ItemKey : item.Id);
                string baseCode = rawBase?.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim() ?? "000000";

                // Format as compact all-numeric Code 128C format:
                // 12-digit format: 2 (prefix) + 6-digit Item ID + 5-digit Qty (x1000) (Qty Only, NO AMOUNT IN BARCODE NUMBER)
                string idStr = !string.IsNullOrWhiteSpace(item.Id) ? item.Id.Trim() : baseCode;
                if (long.TryParse(idStr, out long numId) && numId <= 999999)
                {
                    string idPart = numId.ToString("D6");
                    int qtyGrams = Math.Min(99999, Math.Max(1, (int)Math.Round(qty * 1000m)));
                    barcode = string.Format(CultureInfo.InvariantCulture, "2{0}{1:D5}", idPart, qtyGrams);
                }
                else
                {
                    barcode = string.Format(CultureInfo.InvariantCulture, "{0}*{1:0.###}", baseCode, qty);
                }

                varQty = qty;
                varUnit = GetUnitTitle(item);
            }
            else
            {
                decimal.TryParse(txtPrice.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out rate);
                barcode = (!string.IsNullOrWhiteSpace(item.Barcode) ? item.Barcode : ("200" + (item.Id ?? "101").PadLeft(7, '0'))).Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
            }

            var (subQty, subUnit) = isVariable ? ResolveSubUnit(item, varQty ?? 1m) : (1m, GetUnitTitle(item));
            string companySub = GetCompanySubHeader();

            var labelItem = new BarcodeLabelItem
            {
                ItemId = item.Id,
                Title = item.Title,
                Barcode = barcode,
                Rate = rate,
                Copies = copies,
                IsVariableWeight = isVariable,
                Qty = varQty,
                Unit = varUnit,
                DisplayQty = isVariable ? subQty : (decimal?)null,
                DisplayUnit = isVariable ? subUnit : null,
                TotalAmount = isVariable ? totalAmount : (decimal?)null,
                CompanySubHeader = companySub
            };
            _currentItem = labelItem;

            prgLoading.Visible = true;
            lblStatus.Text = string.Format("Generating {0} barcode label(s) for '{1}'...", copies, item.Title);

            try
            {
                float spread = nudSpread != null ? (float)nudSpread.Value : 2.0f;
                bool incPrice = chkIncludePrice == null || chkIncludePrice.Checked;
                bool incRate = chkIncludeRate != null && chkIncludeRate.Checked;
                bool incSub = chkIncludeCompanySubHeader == null || chkIncludeCompanySubHeader.Checked;
                var doc = new BarcodeDocument(new List<BarcodeLabelItem> { labelItem }, layout, spread, companySub,
                    includeCompanyName: true,
                    includeCompanySubHeader: incSub,
                    includeTitle: true,
                    includePrice: incPrice,
                    includeRate: incRate);
                _currentPdfPath = await doc.GeneratePdfToTempFileAsync();

                webView.CoreWebView2.Navigate(_currentPdfPath);
                webView.Visible = true;
                lblStatus.Text = string.Format("Previewing {0} label(s) for '{1}' (Barcode: {2}) • Format: {3}",
                    copies, item.Title, barcode, GetLayoutDisplayName(layout));
                lblStatus.ForeColor = Color.FromArgb(30, 41, 59);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error rendering barcode: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                prgLoading.Visible = false;
            }
        }

        private async Task PrintDirectAsync()
        {
            if (_currentItem == null)
            {
                await LoadAndRenderBarcodeAsync();
                if (_currentItem == null) return;
            }

            var layout = GetSelectedLayout();
            string defaultPrinter = (layout == BarcodePrintLayout.Roll4Inch3Up || layout == BarcodePrintLayout.ThermalRoll)
                ? ConfigInfo.ThermalPrinterName
                : ConfigInfo.A4PrinterName;
            string printer = cmbPrinter.SelectedItem != null ? cmbPrinter.SelectedItem.ToString() : defaultPrinter;

            var confirm = MessageBox.Show(
                string.Format("Are you sure you want to silently send {0} barcode label(s) directly to '{1}'?", _currentItem.Copies, printer),
                "Confirm Silent Print",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                prgLoading.Visible = true;
                lblStatus.Text = "Printing barcode labels silently...";

                await Task.Run(() =>
                {
                    float spread = nudSpread != null ? (float)nudSpread.Value : 2.0f;
                    string companySub = GetCompanySubHeader();
                    bool incPrice = chkIncludePrice == null || chkIncludePrice.Checked;
                    bool incRate = chkIncludeRate != null && chkIncludeRate.Checked;
                    bool incSub = chkIncludeCompanySubHeader == null || chkIncludeCompanySubHeader.Checked;
                    var doc = new BarcodeDocument(new List<BarcodeLabelItem> { _currentItem }, layout, spread, companySub,
                        includeCompanyName: true,
                        includeCompanySubHeader: incSub,
                        includeTitle: true,
                        includePrice: incPrice,
                        includeRate: incRate);
                    BarcodeDocument.PrintDirectToPrinter(doc, printer, layout);
                });

                MessageBox.Show(string.Format("{0} barcode label(s) sent directly to '{1}' successfully!", _currentItem.Copies, printer), "Print Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblStatus.Text = string.Format("Complete: {0} labels sent to {1}.", _currentItem.Copies, printer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                prgLoading.Visible = false;
            }
        }
    }
}
