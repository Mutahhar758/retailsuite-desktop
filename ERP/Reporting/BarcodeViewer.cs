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
        private Label lblTotalAmount;
        private TextBox txtTotalAmount;
        private bool _isSyncingAmounts = false;
        private Label lblLayout;
        private ComboBox cmbLayout;
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

            // Copies & Price Row
            lblCopies = new Label
            {
                Text = "COPIES / QUANTITY:",
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
                Value = 10,
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
                Text = "Variable Weight / Quantity (e.g. Meat / Produce)",
                Location = new Point(14, curY),
                Width = 330,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };
            pnlSidebar.Controls.Add(chkVariableWeight);
            curY += 26;

            // Variable Weight Controls: Qty & Total Amount
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

            lblTotalAmount = new Label
            {
                Text = "TOTAL AMOUNT (RS):",
                Location = new Point(180, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Visible = false
            };
            pnlSidebar.Controls.Add(lblTotalAmount);
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

            txtTotalAmount = new TextBox
            {
                Location = new Point(180, curY),
                Width = 164,
                Font = new Font("Segoe UI", 9F),
                Visible = false
            };
            pnlSidebar.Controls.Add(txtTotalAmount);
            curY += 34;

            chkVariableWeight.CheckedChanged += (s, e) =>
            {
                bool isVar = chkVariableWeight.Checked;
                lblWeightQty.Visible = isVar;
                nudWeightQty.Visible = isVar;
                lblUnitName.Visible = isVar;
                lblTotalAmount.Visible = isVar;
                txtTotalAmount.Visible = isVar;
                lblPrice.Text = isVar ? "UNIT RATE (RS):" : "PRINT PRICE (RS):";
                SyncVariableWeightTotal();
            };

            nudWeightQty.ValueChanged += (s, e) => SyncVariableWeightTotal();
            txtPrice.TextChanged += (s, e) =>
            {
                if (chkVariableWeight.Checked)
                    SyncVariableWeightTotal();
            };
            txtTotalAmount.TextChanged += (s, e) =>
            {
                if (chkVariableWeight.Checked && !_isSyncingAmounts && nudWeightQty.Value > 0)
                {
                    if (decimal.TryParse(txtTotalAmount.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tot))
                    {
                        try
                        {
                            _isSyncingAmounts = true;
                            decimal computedRate = Math.Round(tot / nudWeightQty.Value);
                            txtPrice.Text = computedRate.ToString("0");
                        }
                        finally
                        {
                            _isSyncingAmounts = false;
                        }
                    }
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

                // Load Units for Title Resolution
                try
                {
                    var units = await _unitApiService.GetLookupAsync();
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

        private void SyncVariableWeightTotal()
        {
            if (_isSyncingAmounts) return;
            try
            {
                _isSyncingAmounts = true;
                decimal rate = 0m;
                decimal.TryParse(txtPrice.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out rate);
                rate = Math.Round(rate);
                decimal qty = nudWeightQty.Value;
                decimal total = Math.Round(qty * rate);
                txtTotalAmount.Text = total.ToString("0");
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

        private void OnItemSelected()
        {
            var item = cmbItem.SelectedItem as InventoryItemDto;
            if (item != null)
            {
                txtPrice.Text = Math.Round(item.PriRate).ToString("0");
                string u = GetUnitTitle(item);
                if (lblUnitName != null) lblUnitName.Text = u;
                if (chkVariableWeight != null && chkVariableWeight.Checked)
                {
                    SyncVariableWeightTotal();
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

            decimal price = item.PriRate;
            decimal.TryParse(txtPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out price);
            int copies = (int)nudCopies.Value;
            var layout = GetSelectedLayout();

            string barcode;
            bool isVariable = chkVariableWeight.Checked;
            decimal? varQty = null;
            string varUnit = null;
            decimal? varTotal = null;

            if (isVariable)
            {
                decimal qty = nudWeightQty.Value;
                decimal rate = Math.Round(price);
                decimal total = Math.Round(qty * rate);
                if (decimal.TryParse(txtTotalAmount.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedTot))
                {
                    total = Math.Round(parsedTot);
                }

                string rawBase = !string.IsNullOrWhiteSpace(item.Barcode) ? item.Barcode : (!string.IsNullOrWhiteSpace(item.ItemKey) ? item.ItemKey : item.Id);
                string baseCode = rawBase?.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
                decimal defaultRate = Math.Round(item.PriRate);
                if (rate == defaultRate)
                {
                    barcode = string.Format(CultureInfo.InvariantCulture, "{0}*{1:0.###}", baseCode, qty);
                }
                else
                {
                    barcode = string.Format(CultureInfo.InvariantCulture, "{0}*{1:0.###}*{2:0}", baseCode, qty, rate);
                }

                varQty = qty;
                varUnit = GetUnitTitle(item);
                varTotal = total;
                price = rate;
            }
            else
            {
                barcode = (!string.IsNullOrWhiteSpace(item.Barcode) ? item.Barcode : ("200" + (item.Id ?? "101").PadLeft(7, '0'))).Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
            }

            var labelItem = new BarcodeLabelItem
            {
                ItemId = item.Id,
                Title = item.Title,
                Barcode = barcode,
                Rate = price,
                Copies = copies,
                IsVariableWeight = isVariable,
                Qty = varQty,
                Unit = varUnit,
                TotalAmount = varTotal
            };
            _currentItem = labelItem;

            prgLoading.Visible = true;
            lblStatus.Text = string.Format("Generating {0} barcode label(s) for '{1}'...", copies, item.Title);

            try
            {
                var doc = new BarcodeDocument(new List<BarcodeLabelItem> { labelItem }, layout);
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
                    var doc = new BarcodeDocument(new List<BarcodeLabelItem> { _currentItem }, layout);
                    BarcodeDocument.PrintDirectToPrinter(doc, printer);
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
