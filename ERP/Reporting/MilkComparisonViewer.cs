using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using CsvHelper;
using ERP.Classes;
using ERP.Reporting.Documents;
using ERP.Reporting.Models;
using ERP.Services.Legacy;
using Microsoft.Web.WebView2.WinForms;

namespace ERP.Reporting
{
    /// <summary>
    /// Modern WinForms viewer for Milk Purchase vs Supply & Sales Comparison.
    /// Provides parity with retailsuite-web-retail's MilkComparisonReport.
    /// </summary>
    public class MilkComparisonViewer : Form
    {
        private readonly InventoryApiService _inventoryApiService;

        // UI Controls - Top Bar
        private Panel pnlTopBar;
        private Label lblItem;
        private ComboBox cmbItem;
        private Label lblFrom;
        private DateTimePicker dtpFrom;
        private Label lblTo;
        private DateTimePicker dtpTo;

        // Quick Presets
        private Button btnPreset1to10;
        private Button btnPreset1to15;
        private Button btnPreset1to20;
        private Button btnPresetMonth;
        private Button btnPresetLastMonth;

        private Button btnGenerate;
        private Button btnExportExcel;
        private Button btnExportCsv;
        private Button btnPrintPreview;
        private Button btnPrintDirect;
        private Label lblStatus;
        private ProgressBar prgLoading;
        private WebView2 webView;

        // State
        private List<InventoryItemDto> _allItems = new List<InventoryItemDto>();
        private MilkComparisonHeader _currentHeader;
        private List<MilkComparisonLineItem> _currentLines = new List<MilkComparisonLineItem>();
        private MilkComparisonSummary _currentSummary;
        private string _currentPdfPath;
        private bool _isWebViewReady = false;
        private bool _isPopulatingItems = false;

        public MilkComparisonViewer()
        {
            _inventoryApiService = new InventoryApiService();
            InitializeComponentCodeFirst();
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Milk Comparison";
            this.Size = new Size(1280, 800);
            this.MinimumSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.BackColor = Color.FromArgb(248, 250, 252);

            // 1. Top Control Bar
            pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 84,
                BackColor = Color.White,
                Padding = new Padding(12, 8, 12, 8)
            };
            pnlTopBar.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
            };

            // Product / Item Selector
            lblItem = new Label
            {
                Text = "MILK PRODUCT / ITEM",
                AutoSize = true,
                Location = new Point(14, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            cmbItem = new ComboBox
            {
                Location = new Point(14, 30),
                Width = 230,
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems,
                Font = new Font("Segoe UI", 9F)
            };

            // From Date
            lblFrom = new Label
            {
                Text = "FROM DATE",
                AutoSize = true,
                Location = new Point(254, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpFrom = new DateTimePicker
            {
                Location = new Point(254, 30),
                Width = 105,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 8.5F),
                Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
            };

            // To Date
            lblTo = new Label
            {
                Text = "TO DATE",
                AutoSize = true,
                Location = new Point(366, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpTo = new DateTimePicker
            {
                Location = new Point(366, 30),
                Width = 105,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 8.5F),
                Value = DateTime.Today
            };

            // Presets row
            int presetY = 56;
            btnPreset1to10 = CreatePresetButton("1-10", 254, presetY, () => ApplyPreset(1, 10));
            btnPreset1to15 = CreatePresetButton("1-15", 298, presetY, () => ApplyPreset(1, 15));
            btnPreset1to20 = CreatePresetButton("1-20", 342, presetY, () => ApplyPreset(1, 20));
            btnPresetMonth = CreatePresetButton("This Mo", 386, presetY, () =>
            {
                dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dtpTo.Value = DateTime.Today;
            });
            btnPresetLastMonth = CreatePresetButton("Last Mo", 442, presetY, () =>
            {
                var prev = DateTime.Today.AddMonths(-1);
                dtpFrom.Value = new DateTime(prev.Year, prev.Month, 1);
                dtpTo.Value = new DateTime(prev.Year, prev.Month, DateTime.DaysInMonth(prev.Year, prev.Month));
            });

            // Action Buttons
            btnGenerate = new Button
            {
                Text = "Generate",
                Location = new Point(510, 28),
                Width = 85,
                Height = 30,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.Click += async (s, e) => await LoadAndRenderReportAsync();

            btnExportExcel = new Button
            {
                Text = "Excel",
                Location = new Point(602, 28),
                Width = 65,
                Height = 30,
                BackColor = Color.FromArgb(16, 149, 193),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportExcel.FlatAppearance.BorderSize = 0;
            btnExportExcel.Click += (s, e) => ExportToExcel();

            btnExportCsv = new Button
            {
                Text = "CSV",
                Location = new Point(673, 28),
                Width = 55,
                Height = 30,
                BackColor = Color.FromArgb(71, 85, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportCsv.FlatAppearance.BorderSize = 0;
            btnExportCsv.Click += (s, e) => ExportToCsv();

            btnPrintPreview = new Button
            {
                Text = "Print",
                Location = new Point(734, 28),
                Width = 65,
                Height = 30,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrintPreview.FlatAppearance.BorderSize = 0;
            btnPrintPreview.Click += (s, e) =>
            {
                if (webView != null && _isWebViewReady)
                    webView.CoreWebView2.ShowPrintUI(Microsoft.Web.WebView2.Core.CoreWebView2PrintDialogKind.Browser);
            };

            btnPrintDirect = new Button
            {
                Text = "🖨 Direct",
                Location = new Point(805, 28),
                Width = 80,
                Height = 30,
                BackColor = Color.FromArgb(5, 150, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrintDirect.FlatAppearance.BorderSize = 0;
            btnPrintDirect.Click += (s, e) => PrintDirect();

            // Status label
            lblStatus = new Label
            {
                Text = "Ready",
                AutoSize = true,
                Location = new Point(895, 35),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            pnlTopBar.Controls.AddRange(new Control[]
            {
                lblItem, cmbItem,
                lblFrom, dtpFrom,
                lblTo, dtpTo,
                btnPreset1to10, btnPreset1to15, btnPreset1to20, btnPresetMonth, btnPresetLastMonth,
                btnGenerate, btnExportExcel, btnExportCsv, btnPrintPreview, btnPrintDirect,
                lblStatus
            });

            // 2. Loading Progress Bar
            prgLoading = new ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 3,
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            // 3. WebView2 PDF Viewer
            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(webView);
            this.Controls.Add(prgLoading);
            this.Controls.Add(pnlTopBar);

            this.Load += async (s, e) =>
            {
                await InitializeWebViewAsync();
                await PopulateItemsAsync();
                if (lblStatus != null) lblStatus.Text = "Ready. Select item & date range, then click 'Generate'.";
            };

            this.FormClosing += (s, e) => CleanupTempFile();
        }

        private Button CreatePresetButton(string text, int x, int y, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(text.Length > 5 ? 52 : 40, 20),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(71, 85, 105),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btn.FlatAppearance.BorderSize = 1;
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private void ApplyPreset(int startDay, int endDay)
        {
            dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, startDay);
            int maxDays = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            dtpTo.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, Math.Min(endDay, maxDays));
        }

        private async Task InitializeWebViewAsync()
        {
            try
            {
                string userDataFolder = Path.Combine(Path.GetTempPath(), "RetailSuite", "WebView2_MilkComparison");
                if (!Directory.Exists(userDataFolder)) Directory.CreateDirectory(userDataFolder);
                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);
                _isWebViewReady = true;
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                webView.CoreWebView2.Settings.IsZoomControlEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("WebView2 runtime required: " + ex.Message, "WebView2 Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task PopulateItemsAsync()
        {
            try
            {
                _isPopulatingItems = true;
                var items = await _inventoryApiService.GetLookupAsync(null);
                _allItems = items ?? new List<InventoryItemDto>();

                var dt = new DataTable();
                dt.Columns.Add("Id", typeof(string));
                dt.Columns.Add("Title", typeof(string));

                if (_allItems.Count > 0)
                {
                    foreach (var itm in _allItems) dt.Rows.Add(itm.Id, itm.Title);
                }

                cmbItem.DisplayMember = "Title";
                cmbItem.ValueMember = "Id";
                cmbItem.DataSource = dt;

                // Auto-select item containing "Milk" / "Dodh" / "دودھ" / "Doodh"
                int targetIndex = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string t = dt.Rows[i]["Title"].ToString().ToLower();
                    if (t.Contains("milk") || t.Contains("dodh") || t.Contains("دودھ") || t.Contains("doodh"))
                    {
                        targetIndex = i;
                        break;
                    }
                }
                if (cmbItem.Items.Count > targetIndex) cmbItem.SelectedIndex = targetIndex;
            }
            catch
            {
                // Ignore fallback
            }
            finally
            {
                _isPopulatingItems = false;
            }
        }

        private async Task LoadAndRenderReportAsync()
        {
            if (!_isWebViewReady) return;

            string itemId = cmbItem.SelectedValue != null ? cmbItem.SelectedValue.ToString() : null;
            string itemTitle = cmbItem.Text;

            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date;

            prgLoading.Visible = true;
            lblStatus.Text = "Loading milk purchase & supply data...";

            try
            {
                var data = await MilkComparisonDataService.GetComparisonDataAsync(fromDate, toDate, itemId, itemTitle);
                _currentHeader = data.Header;
                _currentLines = data.Lines;
                _currentSummary = data.Summary;

                var doc = new MilkComparisonDocument(_currentHeader, _currentLines, _currentSummary);
                string newPdfPath = await doc.GeneratePdfToTempFileAsync();

                CleanupTempFile();
                _currentPdfPath = newPdfPath;

                webView.CoreWebView2.Navigate(_currentPdfPath);

                lblStatus.Text = string.Format("{0} Days • Purch: {1:N0} Ltr • Disp: {2:N0} Ltr • Net: {3}{4:N0} Ltr",
                    _currentLines.Count, _currentSummary.TotalPurchaseQty, _currentSummary.TotalDispatchedQty,
                    _currentSummary.TotalNetDiffQty > 0 ? "+" : "", _currentSummary.TotalNetDiffQty);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                prgLoading.Visible = false;
            }
        }

        private void PrintDirect()
        {
            if (_currentLines == null || _currentLines.Count == 0)
            {
                MessageBox.Show("No comparison data available to print.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string printer = !string.IsNullOrWhiteSpace(ConfigInfo.ThermalPrinterName) ? ConfigInfo.ThermalPrinterName : null;
            try
            {
                var doc = new MilkComparisonDocument(_currentHeader, _currentLines, _currentSummary);
                doc.PrintDirectToPrinter(printer);
                MessageBox.Show("Report sent directly to printer.", "Print Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Direct print failed: " + ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToExcel()
        {
            if (_currentLines == null || _currentLines.Count == 0)
            {
                MessageBox.Show("No data to export.", "Excel Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = string.Format("MilkComparison_{0:yyyyMMdd}_{1:yyyyMMdd}.xlsx", dtpFrom.Value, dtpTo.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Milk Comparison");

                            // Title Block
                            ws.Cell("A1").Value = _currentHeader.CompanyName.ToUpper();
                            ws.Cell("A1").Style.Font.Bold = true;
                            ws.Cell("A1").Style.Font.FontSize = 14;

                            ws.Cell("A2").Value = "MILK PURCHASE VS SUPPLY & DISPATCH COMPARISON REPORT";
                            ws.Cell("A2").Style.Font.Bold = true;

                            ws.Cell("A3").Value = string.Format("Item: {0} ({1}) | Period: {2:dd-MMM-yyyy} to {3:dd-MMM-yyyy}",
                                _currentHeader.ItemTitle, _currentHeader.UnitTitle, _currentHeader.FromDate, _currentHeader.ToDate);

                            // Headers
                            int row = 5;
                            string[] headers = new[]
                            {
                                "Date", "Day", "Purchase Qty", "Purch Rate", "Purchase Amount",
                                "Supply Qty", "Sale Rate", "Supply Amount",
                                "Daily Diff Qty", "Net Diff Qty", "Diff Amount", "Status"
                            };

                            for (int c = 0; c < headers.Length; c++)
                            {
                                var cell = ws.Cell(row, c + 1);
                                cell.Value = headers[c];
                                cell.Style.Font.Bold = true;
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A8A");
                                cell.Style.Font.FontColor = XLColor.White;
                                cell.Style.Alignment.Horizontal = c >= 2 && c <= 10 ? XLAlignmentHorizontalValues.Right : XLAlignmentHorizontalValues.Left;
                            }

                            // Data Rows
                            row = 6;
                            foreach (var item in _currentLines)
                            {
                                ws.Cell(row, 1).Value = item.Date.ToString("yyyy-MM-dd");
                                ws.Cell(row, 2).Value = item.DayName;
                                ws.Cell(row, 3).Value = item.PurchaseQty;
                                ws.Cell(row, 4).Value = item.PurchaseAvgRate;
                                ws.Cell(row, 5).Value = item.PurchaseAmount;
                                ws.Cell(row, 6).Value = item.TotalDispatchedQty;
                                ws.Cell(row, 7).Value = item.SupplyAvgRate;
                                ws.Cell(row, 8).Value = item.SupplyAmount;
                                ws.Cell(row, 9).Value = item.DiffQty;
                                ws.Cell(row, 10).Value = item.NetDiffQty;
                                ws.Cell(row, 11).Value = item.DiffAmount;
                                ws.Cell(row, 12).Value = item.Status;

                                ws.Range(row, 3, row, 11).Style.NumberFormat.Format = "#,##0.00";
                                row++;
                            }

                            // Totals Row
                            ws.Cell(row, 1).Value = "GRAND TOTAL";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Cell(row, 3).Value = _currentSummary.TotalPurchaseQty;
                            ws.Cell(row, 4).Value = _currentSummary.AvgPurchaseRate;
                            ws.Cell(row, 5).Value = _currentSummary.TotalPurchaseAmount;
                            ws.Cell(row, 6).Value = _currentSummary.TotalDispatchedQty;
                            ws.Cell(row, 7).Value = _currentSummary.AvgSupplyRate;
                            ws.Cell(row, 8).Value = _currentSummary.TotalSupplyAmount + _currentSummary.TotalRegularSaleAmount;
                            ws.Cell(row, 9).Value = _currentSummary.TotalDiffQty;
                            ws.Cell(row, 10).Value = _currentSummary.TotalNetDiffQty;
                            ws.Cell(row, 11).Value = _currentSummary.TotalDiffAmount;

                            var totalRange = ws.Range(row, 1, row, 12);
                            totalRange.Style.Font.Bold = true;
                            totalRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Double;

                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Excel exported successfully.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Excel export failed: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportToCsv()
        {
            if (_currentLines == null || _currentLines.Count == 0)
            {
                MessageBox.Show("No data to export.", "CSV Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.FileName = string.Format("MilkComparison_{0:yyyyMMdd}_{1:yyyyMMdd}.csv", dtpFrom.Value, dtpTo.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(sfd.FileName))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.WriteRecords(_currentLines);
                        }
                        MessageBox.Show("CSV exported successfully.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("CSV export failed: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void CleanupTempFile()
        {
            if (!string.IsNullOrEmpty(_currentPdfPath) && File.Exists(_currentPdfPath))
            {
                try { File.Delete(_currentPdfPath); }
                catch { }
                _currentPdfPath = null;
            }
        }
    }
}
