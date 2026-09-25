using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
    /// Modern, code-first WinForms Stock Balance & Inventory Valuation Viewer hosting WebView2
    /// with QuestPDF landscape rendering and ClosedXML / CsvHelper export functionality.
    /// </summary>
    public class StockBalanceViewer : Form
    {
        // UI Controls
        private Panel pnlTopBar;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblFrom;
        private DateTimePicker dtpFrom;
        private Label lblTo;
        private DateTimePicker dtpTo;
        private Label lblFilter;
        private ComboBox cmbFilter;
        private Button btnRefresh;
        private Button btnExportExcel;
        private Button btnExportCsv;
        private Button btnPrint;
        private Label lblStatus;
        private ProgressBar prgLoading;
        private WebView2 webView;

        // State
        private StockBalanceHeader _currentHeader;
        private List<StockBalanceReportItem> _currentItems;
        private string _currentPdfPath;
        private bool _isWebViewReady = false;
        private readonly ItemCategoryApiService _categoryService;
        private CheckBox chkShowStockValue;

        public StockBalanceViewer()
        {
            _categoryService = new ItemCategoryApiService();
            InitializeComponentCodeFirst();
        }

        public StockBalanceViewer(DateTime fromDate, DateTime toDate)
            : this()
        {
            if (dtpFrom != null) dtpFrom.Value = fromDate;
            if (dtpTo != null) dtpTo.Value = toDate;
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Stock Balance";
            this.Size = new Size(1220, 840);
            this.MinimumSize = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.BackColor = Color.FromArgb(248, 250, 252);

            // 1. Top Control Bar Panel
            pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = Color.White,
                Padding = new Padding(16, 8, 16, 8)
            };
            pnlTopBar.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
            };

            // Category Filter Dropdown
            lblCategory = new Label
            {
                Text = "CATEGORY",
                AutoSize = true,
                Location = new Point(16, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            cmbCategory = new ComboBox
            {
                Location = new Point(16, 32),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems,
                Font = new Font("Segoe UI", 9F)
            };

            // From Date Control
            lblFrom = new Label
            {
                Text = "FROM DATE",
                AutoSize = true,
                Location = new Point(226, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpFrom = new DateTimePicker
            {
                Location = new Point(226, 32),
                Width = 110,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = DateTime.Today.AddDays(-30)
            };

            // To Date Control
            lblTo = new Label
            {
                Text = "TO DATE",
                AutoSize = true,
                Location = new Point(346, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpTo = new DateTimePicker
            {
                Location = new Point(346, 32),
                Width = 110,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = DateTime.Today
            };

            // Stock Balance Filter (All, > 0, < 0, = 0)
            lblFilter = new Label
            {
                Text = "STOCK FILTER",
                AutoSize = true,
                Location = new Point(466, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            cmbFilter = new ComboBox
            {
                Location = new Point(466, 32),
                Width = 135,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbFilter.Items.AddRange(new object[] { "All Stock", "Positive (> 0)", "Negative (< 0)", "Zero (= 0)" });
            cmbFilter.SelectedIndex = 0;

            // Generate Button
            btnRefresh = new Button
            {
                Text = "Generate",
                Location = new Point(611, 30),
                Size = new Size(84, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) => await LoadAndRenderReportAsync();

            // Export to Excel Button
            btnExportExcel = new Button
            {
                Text = "Excel",
                Location = new Point(703, 30),
                Size = new Size(68, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnExportExcel.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnExportExcel.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
            btnExportExcel.Click += async (s, e) => await ExportToExcelAsync();

            // Export to CSV Button
            btnExportCsv = new Button
            {
                Text = "CSV",
                Location = new Point(777, 30),
                Size = new Size(68, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnExportCsv.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnExportCsv.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
            btnExportCsv.Click += async (s, e) => await ExportToCsvAsync();

            // Print Button
            btnPrint = new Button
            {
                Text = "Print",
                Location = new Point(851, 30),
                Size = new Size(68, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnPrint.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnPrint.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
            btnPrint.Click += (s, e) =>
            {
                if (webView != null && webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.ShowPrintUI(Microsoft.Web.WebView2.Core.CoreWebView2PrintDialogKind.Browser);
                }
            };

            // Status Label & Progress Indicator
            lblStatus = new Label
            {
                Text = "Ready",
                Location = new Point(611, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            prgLoading = new ProgressBar
            {
                Location = new Point(703, 13),
                Size = new Size(110, 12),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            // Show Stock Value Checkbox
            chkShowStockValue = new CheckBox
            {
                Text = "Show Stock Value",
                Location = new Point(925, 35),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(30, 41, 59),
                Cursor = Cursors.Hand,
                Checked = true
            };

            // Add controls to top bar
            pnlTopBar.Controls.Add(lblCategory);
            pnlTopBar.Controls.Add(cmbCategory);
            pnlTopBar.Controls.Add(lblFrom);
            pnlTopBar.Controls.Add(dtpFrom);
            pnlTopBar.Controls.Add(lblTo);
            pnlTopBar.Controls.Add(dtpTo);
            pnlTopBar.Controls.Add(lblFilter);
            pnlTopBar.Controls.Add(cmbFilter);
            pnlTopBar.Controls.Add(btnRefresh);
            pnlTopBar.Controls.Add(btnExportExcel);
            pnlTopBar.Controls.Add(btnExportCsv);
            pnlTopBar.Controls.Add(btnPrint);
            pnlTopBar.Controls.Add(chkShowStockValue);
            pnlTopBar.Controls.Add(lblStatus);
            pnlTopBar.Controls.Add(prgLoading);

            // 2. Main WebView2 Control
            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(webView);
            this.Controls.Add(pnlTopBar);

            this.Load += async (s, e) =>
            {
                await InitializeWebView2Async();
                await LoadCategoriesAsync();
                SetLoadingState(false, "Ready. Select category & filters, then click 'Filter' to view stock balance.");
            };
        }

        private async Task InitializeWebView2Async()
        {
            try
            {
                SetLoadingState(true, "Initializing document viewer runtime...");
                string userDataFolder = Path.Combine(Path.GetTempPath(), "RetailSuite_WebView2");
                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);

                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                webView.CoreWebView2.Settings.AreDevToolsEnabled = false;

                _isWebViewReady = true;
                SetLoadingState(false, "Viewer ready");
            }
            catch (Exception ex)
            {
                SetLoadingState(false, "WebView2 runtime unavailable");
                MessageBox.Show(
                    "WebView2 runtime failed to initialize: " + ex.Message +
                    "\n\nPlease ensure Microsoft Edge WebView2 Runtime is installed on this machine.",
                    "Viewer Runtime Notice",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await _categoryService.GetLookupAsync();
                var dt = new DataTable();
                dt.Columns.Add("Code", typeof(string));
                dt.Columns.Add("Title", typeof(string));

                dt.Rows.Add("", "All Categories");
                if (categories != null)
                {
                    foreach (var cat in categories)
                    {
                        dt.Rows.Add(cat.Code, cat.Title);
                    }
                }

                cmbCategory.DisplayMember = "Title";
                cmbCategory.ValueMember = "Code";
                cmbCategory.DataSource = dt;
                cmbCategory.SelectedIndex = 0;
            }
            catch
            {
                var dt = new DataTable();
                dt.Columns.Add("Code", typeof(string));
                dt.Columns.Add("Title", typeof(string));
                dt.Rows.Add("", "All Categories");

                cmbCategory.DisplayMember = "Title";
                cmbCategory.ValueMember = "Code";
                cmbCategory.DataSource = dt;
                cmbCategory.SelectedIndex = 0;
            }
        }

        public async Task LoadAndRenderReportAsync()
        {
            if (!_isWebViewReady) return;

            SetLoadingState(true, "Calculating inventory valuation & generating PDF...");

            try
            {
                string categoryCode = cmbCategory.SelectedValue != null ? cmbCategory.SelectedValue.ToString() : string.Empty;
                string categoryTitle = cmbCategory.Text ?? "All Categories";
                DateTime from = dtpFrom.Value.Date;
                DateTime to = dtpTo.Value.Date.AddDays(1).AddSeconds(-1);
                string filterSelection = cmbFilter.SelectedItem?.ToString() ?? "All Items";
                string queryFilter = "All";
                if (filterSelection == "Closing Qty > 0") queryFilter = "> 0";
                else if (filterSelection == "Closing Qty < 0") queryFilter = "< 0";
                else if (filterSelection == "Closing Qty = 0") queryFilter = "= 0";

                var reportData = await Task.Run(() =>
                {
                    DataTable dt = ReportQuery.StockBalance(from, to, queryFilter, 0m, categoryCode, null);
                    var items = (dt != null && dt.Rows.Count > 0) ? StockBalanceDataService.FromDataTable(dt) : new List<StockBalanceReportItem>();

                    // Apply client-side quantity filter safeguard
                    if (queryFilter == "> 0") items = items.Where(x => x.ClosingQty > 0).ToList();
                    else if (queryFilter == "< 0") items = items.Where(x => x.ClosingQty < 0).ToList();
                    else if (queryFilter == "= 0") items = items.Where(x => x.ClosingQty == 0).ToList();

                    // Re-index
                    for (int i = 0; i < items.Count; i++) items[i].Index = i + 1;

                    var header = new StockBalanceHeader
                    {
                        CompanyName = CompanyInfo.CompanyName,
                        CategoryName = categoryTitle,
                        Filter = filterSelection,
                        FromDate = from,
                        ToDate = to,
                        TotalItems = items.Count,
                        TotalOpeningQty = items.Sum(x => x.OpeningQty),
                        TotalQtyIn = items.Sum(x => x.QtyIn),
                        TotalQtyOut = items.Sum(x => x.QtyOut),
                        TotalClosingQty = items.Sum(x => x.ClosingQty),
                        TotalStockValue = items.Sum(x => x.TotalValue)
                    };

                    return new StockBalanceDataResult(header, items);
                });

                _currentHeader = reportData.Header;
                _currentItems = reportData.Items;

                _currentPdfPath = await StockBalanceDocument.GeneratePdfToTempFileAsync(_currentHeader, _currentItems, chkShowStockValue.Checked);

                if (File.Exists(_currentPdfPath) && webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.Navigate(new Uri(_currentPdfPath).AbsoluteUri);
                }

                SetLoadingState(false, string.Format("Report ready ({0} SKUs, Value: {1:C0})", _currentItems.Count, _currentHeader.TotalStockValue));
            }
            catch (Exception ex)
            {
                SetLoadingState(false, "Error loading report");
                MessageBox.Show(
                    "Failed to generate stock balance report: " + ex.Message,
                    "Report Generation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task ExportToExcelAsync()
        {
            if (_currentItems == null || _currentItems.Count == 0)
            {
                MessageBox.Show("There is no stock balance data available to export.", "Export Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = string.Format("StockBalance_{0:yyyyMMdd_HHmmss}.xlsx", DateTime.Now);

                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                SetLoadingState(true, "Exporting to Excel...");

                try
                {
                    string filePath = sfd.FileName;
                    var header = _currentHeader;
                    var items = _currentItems;

                    await Task.Run(() =>
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Stock Balance");

                            // Report Header Banner
                            ws.Cell("A1").Value = header?.CompanyName ?? "RetailSuite Enterprise";
                            ws.Cell("A1").Style.Font.Bold = true;
                            ws.Cell("A1").Style.Font.FontSize = 15;
                            ws.Cell("A1").Style.Font.FontColor = XLColor.FromHtml("#0F172A");

                            ws.Cell("A2").Value = "STOCK BALANCE & INVENTORY VALUATION AUDIT";
                            ws.Cell("A2").Style.Font.Bold = true;
                            ws.Cell("A2").Style.Font.FontSize = 10;
                            ws.Cell("A2").Style.Font.FontColor = XLColor.FromHtml("#475569");

                            ws.Cell("A4").Value = "Category:";
                            ws.Cell("A4").Style.Font.Bold = true;
                            ws.Cell("B4").Value = header?.CategoryName ?? "All Categories";

                            ws.Cell("D4").Value = "Period:";
                            ws.Cell("D4").Style.Font.Bold = true;
                            ws.Cell("E4").Value = string.Format("{0:yyyy-MM-dd} to {1:yyyy-MM-dd}", header?.FromDate, header?.ToDate);

                            ws.Cell("D5").Value = "Filter:";
                            ws.Cell("D5").Style.Font.Bold = true;
                            ws.Cell("E5").Value = header?.Filter ?? "All Stock";

                            // Table Column Headers
                            int startRow = 7;
                            string[] headers = { "#", "Item / Product Name", "Unit", "Opening Qty", "Inward (+)", "Outward (-)", "Closing Qty", "Rate", "Total Value" };
                            for (int c = 0; c < headers.Length; c++)
                            {
                                var cell = ws.Cell(startRow, c + 1);
                                cell.Value = headers[c];
                                cell.Style.Font.Bold = true;
                                cell.Style.Font.FontColor = XLColor.FromHtml("#334155");
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
                                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                if (c == 0 || c == 2) cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                if (c >= 3) cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            }

                            // Table Data Rows
                            int rowIdx = startRow + 1;
                            foreach (var itm in items)
                            {
                                ws.Cell(rowIdx, 1).Value = itm.Index;
                                ws.Cell(rowIdx, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                ws.Cell(rowIdx, 2).Value = itm.ItemName ?? "";

                                ws.Cell(rowIdx, 3).Value = itm.Unit ?? "";
                                ws.Cell(rowIdx, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                ws.Cell(rowIdx, 4).Value = itm.OpeningQty;
                                ws.Cell(rowIdx, 4).Style.NumberFormat.Format = "#,##0.##";

                                ws.Cell(rowIdx, 5).Value = itm.QtyIn;
                                ws.Cell(rowIdx, 5).Style.NumberFormat.Format = "#,##0.##";

                                ws.Cell(rowIdx, 6).Value = itm.QtyOut;
                                ws.Cell(rowIdx, 6).Style.NumberFormat.Format = "#,##0.##";

                                ws.Cell(rowIdx, 7).Value = itm.ClosingQty;
                                ws.Cell(rowIdx, 7).Style.NumberFormat.Format = "#,##0.##";

                                ws.Cell(rowIdx, 8).Value = itm.Rate;
                                ws.Cell(rowIdx, 8).Style.NumberFormat.Format = "#,##0.00";

                                ws.Cell(rowIdx, 9).Value = itm.TotalValue;
                                ws.Cell(rowIdx, 9).Style.NumberFormat.Format = "#,##0.00";

                                if (rowIdx % 2 == 1)
                                {
                                    ws.Range(rowIdx, 1, rowIdx, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                                }

                                rowIdx++;
                            }

                            // Summary Total Row
                            ws.Cell(rowIdx, 1).Value = "TOTAL SUMMARY";
                            ws.Cell(rowIdx, 1).Style.Font.Bold = true;
                            ws.Range(rowIdx, 1, rowIdx, 3).Merge();

                            ws.Cell(rowIdx, 4).Value = items.Sum(x => x.OpeningQty);
                            ws.Cell(rowIdx, 4).Style.Font.Bold = true;
                            ws.Cell(rowIdx, 4).Style.NumberFormat.Format = "#,##0.##";

                            ws.Cell(rowIdx, 5).Value = items.Sum(x => x.QtyIn);
                            ws.Cell(rowIdx, 5).Style.Font.Bold = true;
                            ws.Cell(rowIdx, 5).Style.NumberFormat.Format = "#,##0.##";

                            ws.Cell(rowIdx, 6).Value = items.Sum(x => x.QtyOut);
                            ws.Cell(rowIdx, 6).Style.Font.Bold = true;
                            ws.Cell(rowIdx, 6).Style.NumberFormat.Format = "#,##0.##";

                            ws.Cell(rowIdx, 7).Value = items.Sum(x => x.ClosingQty);
                            ws.Cell(rowIdx, 7).Style.Font.Bold = true;
                            ws.Cell(rowIdx, 7).Style.NumberFormat.Format = "#,##0.##";

                            ws.Cell(rowIdx, 8).Value = "-";
                            ws.Cell(rowIdx, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            ws.Cell(rowIdx, 9).Value = items.Sum(x => x.TotalValue);
                            ws.Cell(rowIdx, 9).Style.Font.Bold = true;
                            ws.Cell(rowIdx, 9).Style.NumberFormat.Format = "#,##0.00";

                            var totalRange = ws.Range(rowIdx, 1, rowIdx, 9);
                            totalRange.Style.Border.TopBorder = XLBorderStyleValues.Medium;
                            totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Double;
                            totalRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");

                            ws.Columns().AdjustToContents();
                            ws.Column(2).Width = Math.Max(ws.Column(2).Width, 35);

                            workbook.SaveAs(filePath);
                        }
                    });

                    SetLoadingState(false, "Excel export complete");
                    MessageBox.Show(
                        "Stock Balance exported to Excel successfully!\n\nFile: " + filePath,
                        "Export Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    SetLoadingState(false, "Excel export failed");
                    MessageBox.Show("Failed to export to Excel: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task ExportToCsvAsync()
        {
            if (_currentItems == null || _currentItems.Count == 0)
            {
                MessageBox.Show("There is no stock balance data available to export.", "Export Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Comma-Separated Values (*.csv)|*.csv";
                sfd.FileName = string.Format("StockBalance_{0:yyyyMMdd_HHmmss}.csv", DateTime.Now);

                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                SetLoadingState(true, "Exporting to CSV...");

                try
                {
                    string filePath = sfd.FileName;
                    var items = _currentItems;

                    await Task.Run(() =>
                    {
                        using (var writer = new StreamWriter(filePath))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.WriteRecords(items);
                        }
                    });

                    SetLoadingState(false, "CSV export complete");
                    MessageBox.Show(
                        "Stock Balance exported to CSV successfully!\n\nFile: " + filePath,
                        "Export Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    SetLoadingState(false, "CSV export failed");
                    MessageBox.Show("Failed to export to CSV: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SetLoadingState(bool isLoading, string statusText)
        {
            lblStatus.Text = statusText;
            prgLoading.Visible = isLoading;
            btnRefresh.Enabled = !isLoading;

            bool hasData = (_currentItems != null && _currentItems.Count > 0);
            btnExportExcel.Enabled = !isLoading && hasData;
            btnExportCsv.Enabled = !isLoading && hasData;
            btnPrint.Enabled = !isLoading && _isWebViewReady && hasData;
            cmbCategory.Enabled = !isLoading;
            dtpFrom.Enabled = !isLoading;
            dtpTo.Enabled = !isLoading;
            cmbFilter.Enabled = !isLoading;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            try
            {
                if (!string.IsNullOrEmpty(_currentPdfPath) && File.Exists(_currentPdfPath))
                {
                    // Delay delete or let OS clean temp
                }
            }
            catch { }
        }
    }
}
