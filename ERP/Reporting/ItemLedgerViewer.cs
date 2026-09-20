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
    /// Modern, code-first WinForms Item Ledger / Stock Ledger Viewer hosting WebView2
    /// with QuestPDF financial rendering and ClosedXML / CsvHelper export functionality.
    /// </summary>
    public class ItemLedgerViewer : Form
    {
        // UI Controls
        private Panel pnlTopBar;
        private Label lblItem;
        private ComboBox cmbItem;
        private Label lblFrom;
        private DateTimePicker dtpFrom;
        private Label lblTo;
        private DateTimePicker dtpTo;
        private Label lblFilter;
        private ComboBox cmbFilter;
        private Label lblSearch;
        private TextBox txtSearch;
        private CheckBox chkShowCost;
        private Button btnGenerate;
        private Button btnExportExcel;
        private Button btnExportCsv;
        private Button btnPrint;
        private Label lblStatus;
        private ProgressBar prgLoading;
        private WebView2 webView;

        // API Service
        private readonly InventoryApiService _inventoryApiService = new InventoryApiService();

        // State
        private ItemLedgerHeader _currentHeader;
        private List<ItemLedgerReportItem> _allItems = new List<ItemLedgerReportItem>();
        private List<ItemLedgerReportItem> _filteredItems = new List<ItemLedgerReportItem>();
        private string _currentPdfPath;
        private bool _isWebViewReady = false;
        private bool _isPopulatingItems = false;

        public ItemLedgerViewer()
        {
            InitializeComponentCodeFirst();
        }

        public ItemLedgerViewer(string selectedItemId, DateTime fromDate, DateTime toDate)
            : this()
        {
            if (dtpFrom != null) dtpFrom.Value = fromDate;
            if (dtpTo != null) dtpTo.Value = toDate;
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Item Ledger";
            this.Size = new Size(1240, 820);
            this.MinimumSize = new Size(1020, 650);
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

            // Item Dropdown
            lblItem = new Label
            {
                Text = "PRODUCT / ITEM",
                AutoSize = true,
                Location = new Point(16, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            cmbItem = new ComboBox
            {
                Location = new Point(16, 32),
                Width = 230,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };


            // From Date Control
            lblFrom = new Label
            {
                Text = "FROM DATE",
                AutoSize = true,
                Location = new Point(256, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpFrom = new DateTimePicker
            {
                Location = new Point(256, 32),
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
                Location = new Point(376, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpTo = new DateTimePicker
            {
                Location = new Point(376, 32),
                Width = 110,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = DateTime.Today
            };

            // Movement Filter Dropdown
            lblFilter = new Label
            {
                Text = "MOVEMENT FILTER",
                AutoSize = true,
                Location = new Point(496, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            cmbFilter = new ComboBox
            {
                Location = new Point(496, 32),
                Width = 130,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbFilter.Items.AddRange(new object[] { "All Movements", "Inward Only (+)", "Outward Only (-)" });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => ApplyClientFilter();

            // Search Box
            lblSearch = new Label
            {
                Text = "SEARCH PARTICULAR",
                AutoSize = true,
                Location = new Point(636, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            txtSearch = new TextBox
            {
                Location = new Point(636, 32),
                Width = 135,
                Font = new Font("Segoe UI", 9F)
            };
            txtSearch.TextChanged += (s, e) => ApplyClientFilter();

            // Show Cost Checkbox
            chkShowCost = new CheckBox
            {
                Text = "Cost Price",
                Location = new Point(781, 33),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Cursor = Cursors.Hand
            };
            chkShowCost.CheckedChanged += async (s, e) =>
            {
                if (cmbItem.SelectedValue != null && !string.IsNullOrWhiteSpace(cmbItem.SelectedValue.ToString()))
                {
                    await LoadAndRenderReportAsync();
                }
            };

            // Generate Button
            btnGenerate = new Button
            {
                Text = "Generate",
                Location = new Point(875, 30),
                Width = 80,
                Height = 28,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.Click += async (s, e) => await LoadAndRenderReportAsync();

            // Excel Export Button
            btnExportExcel = new Button
            {
                Text = "Excel",
                Location = new Point(960, 30),
                Width = 65,
                Height = 28,
                BackColor = Color.FromArgb(16, 149, 193),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportExcel.FlatAppearance.BorderSize = 0;
            btnExportExcel.Click += (s, e) => ExportToExcel();

            // CSV Export Button
            btnExportCsv = new Button
            {
                Text = "CSV",
                Location = new Point(1030, 30),
                Width = 55,
                Height = 28,
                BackColor = Color.FromArgb(71, 85, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportCsv.FlatAppearance.BorderSize = 0;
            btnExportCsv.Click += (s, e) => ExportToCsv();

            // Print Button
            btnPrint = new Button
            {
                Text = "Print",
                Location = new Point(1090, 30),
                Width = 60,
                Height = 28,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Click += (s, e) => TriggerPrint();

            // Status Label
            lblStatus = new Label
            {
                Location = new Point(1155, 35),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129)
            };

            pnlTopBar.Controls.AddRange(new Control[]
            {
                lblItem, cmbItem,
                lblFrom, dtpFrom,
                lblTo, dtpTo,
                lblFilter, cmbFilter,
                lblSearch, txtSearch,
                chkShowCost,
                btnGenerate,
                btnExportExcel, btnExportCsv, btnPrint,
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
                if (lblStatus != null) lblStatus.Text = "Ready. Select item & date range, then click 'Generate Report'.";
            };

            this.FormClosing += (s, e) =>
            {
                CleanupTempFile();
            };
        }

        private async Task InitializeWebViewAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                _isWebViewReady = true;
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                webView.CoreWebView2.Settings.IsZoomControlEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "WebView2 runtime is required to view modern reports.\nDetails: " + ex.Message,
                    "WebView2 Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private async Task PopulateItemsAsync()
        {
            try
            {
                _isPopulatingItems = true;
                var items = await _inventoryApiService.GetLookupAsync(null);

                var dt = new DataTable();
                dt.Columns.Add("ID", typeof(string));
                dt.Columns.Add("Title", typeof(string));

                if (items != null && items.Count > 0)
                {
                    foreach (var itm in items)
                    {
                        dt.Rows.Add(itm.Id, itm.Title);
                    }
                }

                cmbItem.DataSource = dt;
                cmbItem.DisplayMember = "Title";
                cmbItem.ValueMember = "ID";

                if (cmbItem.Items.Count > 0)
                {
                    cmbItem.SelectedIndex = 0;
                }
            }
            catch
            {
                // Leave empty on error
            }
            finally
            {
                _isPopulatingItems = false;
            }
        }

        private async Task LoadAndRenderReportAsync()
        {
            if (!_isWebViewReady) return;

            string selectedItemId = cmbItem.SelectedValue != null ? cmbItem.SelectedValue.ToString() : string.Empty;
            string selectedItemTitle = cmbItem.Text ?? "Inventory Item";
            DateTime fromDate = dtpFrom.Value;
            DateTime toDate = dtpTo.Value;
            bool showCostPrice = chkShowCost != null && chkShowCost.Checked;

            SetLoading(true);
            try
            {
                ItemLedgerDataResult result = null;

                // 1. Fetch live data
                DataTable dt = await Task.Run(() => ReportQuery.StockLedger(selectedItemId, fromDate, toDate, showCostPrice));
                if (dt != null && dt.Rows.Count > 0)
                {
                    result = ItemLedgerDataService.ConvertDataTable(dt, selectedItemId, selectedItemTitle, fromDate, toDate, "All", showCostPrice);
                }
                else
                {
                    var emptyHeader = new ItemLedgerHeader
                    {
                        CompanyName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise",
                        ItemId = selectedItemId ?? string.Empty,
                        ItemTitle = !string.IsNullOrWhiteSpace(selectedItemTitle) ? selectedItemTitle : "Inventory Item",
                        FromDate = fromDate,
                        ToDate = toDate,
                        OpeningBalance = 0,
                        TotalIn = 0,
                        TotalOut = 0,
                        ClosingBalance = 0,
                        ShowCostPrice = showCostPrice,
                        GeneratedAt = DateTime.Now
                    };
                    result = new ItemLedgerDataResult(emptyHeader, new List<ItemLedgerReportItem>());
                }

                _currentHeader = result.Header;
                _allItems = result.Items ?? new List<ItemLedgerReportItem>();

                ApplyClientFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Item Ledger report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void ApplyClientFilter()
        {
            string filterType = "All";
            if (cmbFilter.SelectedIndex == 1) filterType = "Inward";
            else if (cmbFilter.SelectedIndex == 2) filterType = "Outward";

            string search = txtSearch.Text != null ? txtSearch.Text.Trim() : string.Empty;

            var items = _allItems.AsEnumerable();

            if (filterType == "Inward")
            {
                items = items.Where(x => x.VoucherNo == "-" || x.QtyIn > 0);
            }
            else if (filterType == "Outward")
            {
                items = items.Where(x => x.VoucherNo == "-" || x.QtyOut > 0);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                items = items.Where(x =>
                    (x.Particular ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (x.VoucherNo ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            _filteredItems = items.ToList();

            UpdateStatusBadge();
            _ = RenderFilteredDocumentAsync();
        }

        private async Task RenderFilteredDocumentAsync()
        {
            if (!_isWebViewReady) return;

            try
            {
                decimal totalIn = _filteredItems.Where(x => x.VoucherNo != "-").Sum(x => x.QtyIn);
                decimal totalOut = _filteredItems.Where(x => x.VoucherNo != "-").Sum(x => x.QtyOut);
                decimal closingBal = _currentHeader != null ? _currentHeader.ClosingBalance : 0m;

                var header = new ItemLedgerHeader
                {
                    CompanyName = _currentHeader != null ? _currentHeader.CompanyName : "Retail Suite Enterprise",
                    ItemId = _currentHeader != null ? _currentHeader.ItemId : string.Empty,
                    ItemTitle = _currentHeader != null ? _currentHeader.ItemTitle : "Inventory Item",
                    FromDate = dtpFrom.Value,
                    ToDate = dtpTo.Value,
                    OpeningBalance = _currentHeader != null ? _currentHeader.OpeningBalance : 0m,
                    TotalIn = totalIn,
                    TotalOut = totalOut,
                    ClosingBalance = closingBal,
                    ShowCostPrice = _currentHeader != null && _currentHeader.ShowCostPrice,
                    GeneratedAt = DateTime.Now
                };

                var doc = new ItemLedgerDocument(header, _filteredItems);
                string newPdfPath = await doc.GeneratePdfToTempFileAsync();

                CleanupTempFile();
                _currentPdfPath = newPdfPath;

                if (webView != null && webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.Navigate(new Uri(_currentPdfPath).AbsoluteUri);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PDF Render Error: " + ex.Message);
            }
        }

        private void UpdateStatusBadge()
        {
            if (_currentHeader != null)
            {
                lblStatus.Text = string.Format("Open: {0:#,##0.00} | In: +{1:#,##0.00} | Out: -{2:#,##0.00} | Close: {3:#,##0.00}",
                    _currentHeader.OpeningBalance,
                    _currentHeader.TotalIn,
                    _currentHeader.TotalOut,
                    _currentHeader.ClosingBalance);
                lblStatus.ForeColor = Color.FromArgb(37, 99, 235);
            }
            else
            {
                lblStatus.Text = "No ledger records";
                lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
            }
        }

        private void SetLoading(bool isLoading)
        {
            prgLoading.Visible = isLoading;
            btnGenerate.Enabled = !isLoading;
            btnExportExcel.Enabled = !isLoading;
            btnExportCsv.Enabled = !isLoading;
            btnPrint.Enabled = !isLoading;
        }

        private void ExportToExcel()
        {
            if (_filteredItems == null || _filteredItems.Count == 0)
            {
                MessageBox.Show("No item ledger records available to export.", "Export Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                string safeItem = (_currentHeader?.ItemTitle ?? "ItemLedger").Replace(" ", "_").Replace("/", "-");
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = string.Format("{0}_{1:yyyyMMdd}_{2:yyyyMMdd}.xlsx", safeItem, dtpFrom.Value, dtpTo.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Item Ledger");

                            // Brand Header
                            ws.Cell(1, 1).Value = _currentHeader?.CompanyName ?? "Retail Suite Enterprise";
                            ws.Cell(1, 1).Style.Font.Bold = true;
                            ws.Cell(1, 1).Style.Font.FontSize = 14;

                            ws.Cell(2, 1).Value = string.Format("Item Ledger: {0} ({1})", _currentHeader?.ItemTitle ?? "Item", _currentHeader?.ItemId ?? "");
                            ws.Cell(2, 1).Style.Font.Bold = true;
                            ws.Cell(2, 1).Style.Font.FontSize = 11;

                            ws.Cell(3, 1).Value = string.Format("Period: {0:dd-MMM-yyyy} to {1:dd-MMM-yyyy} | Generated: {2:dd-MMM-yyyy HH:mm}", dtpFrom.Value, dtpTo.Value, DateTime.Now);
                            ws.Cell(3, 1).Style.Font.Italic = true;
                            ws.Cell(3, 1).Style.Font.FontColor = XLColor.FromArgb(100, 116, 139);

                            bool showCostPrice = _currentHeader?.ShowCostPrice == true;

                            // Columns Table Header
                            int row = 5;
                            string[] headers = showCostPrice
                                ? new string[] { "Date", "Voucher #", "Particular / Narrative", "Rate", "Cost Price", "Inward (+)", "Outward (-)", "Balance Qty" }
                                : new string[] { "Date", "Voucher #", "Particular / Narrative", "Rate", "Inward (+)", "Outward (-)", "Balance Qty" };

                            for (int col = 1; col <= headers.Length; col++)
                            {
                                var cell = ws.Cell(row, col);
                                cell.Value = headers[col - 1];
                                cell.Style.Font.Bold = true;
                                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);
                                cell.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                                cell.Style.Border.BottomBorderColor = XLColor.FromArgb(51, 65, 85);
                            }

                            // Rows
                            row = 6;
                            foreach (var item in _filteredItems)
                            {
                                ws.Cell(row, 1).Value = item.Date.ToString("dd-MMM-yyyy");
                                ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                ws.Cell(row, 2).Value = item.VoucherNo;
                                ws.Cell(row, 2).Style.Font.Bold = true;

                                ws.Cell(row, 3).Value = item.Particular;

                                if (item.Rate.HasValue && item.Rate.Value > 0)
                                {
                                    ws.Cell(row, 4).Value = item.Rate.Value;
                                    ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
                                }
                                else
                                {
                                    ws.Cell(row, 4).Value = "-";
                                    ws.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                }

                                int inwardCol = showCostPrice ? 6 : 5;
                                int outwardCol = showCostPrice ? 7 : 6;
                                int balCol = showCostPrice ? 8 : 7;

                                if (showCostPrice)
                                {
                                    if (item.CostPrice.HasValue && item.CostPrice.Value > 0)
                                    {
                                        ws.Cell(row, 5).Value = item.CostPrice.Value;
                                        ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
                                    }
                                    else
                                    {
                                        ws.Cell(row, 5).Value = "-";
                                        ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    }
                                }

                                if (item.QtyIn > 0)
                                {
                                    ws.Cell(row, inwardCol).Value = item.QtyIn;
                                    ws.Cell(row, inwardCol).Style.NumberFormat.Format = "#,##0.00";
                                }
                                else
                                {
                                    ws.Cell(row, inwardCol).Value = "-";
                                    ws.Cell(row, inwardCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                }

                                if (item.QtyOut > 0)
                                {
                                    ws.Cell(row, outwardCol).Value = item.QtyOut;
                                    ws.Cell(row, outwardCol).Style.NumberFormat.Format = "#,##0.00";
                                }
                                else
                                {
                                    ws.Cell(row, outwardCol).Value = "-";
                                    ws.Cell(row, outwardCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                }

                                ws.Cell(row, balCol).Value = item.Balance;
                                ws.Cell(row, balCol).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(row, balCol).Style.Font.Bold = true;

                                if (row % 2 == 1)
                                {
                                    ws.Range(row, 1, row, balCol).Style.Fill.BackgroundColor = XLColor.FromArgb(248, 250, 252);
                                }

                                row++;
                            }

                            // Summary Row
                            int lastCol = showCostPrice ? 8 : 7;
                            var summaryRange = ws.Range(row, 1, row, lastCol);
                            summaryRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            summaryRange.Style.Border.BottomBorder = XLBorderStyleValues.Double;
                            summaryRange.Style.Font.Bold = true;
                            summaryRange.Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);

                            ws.Cell(row, 1).Value = "TOTALS";
                            if (showCostPrice)
                            {
                                ws.Cell(row, 6).FormulaA1 = string.Format("SUM(F6:F{0})", row - 1);
                                ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(row, 7).FormulaA1 = string.Format("SUM(G6:G{0})", row - 1);
                                ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(row, 8).Value = _currentHeader?.ClosingBalance ?? 0m;
                                ws.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";
                            }
                            else
                            {
                                ws.Cell(row, 5).FormulaA1 = string.Format("SUM(E6:E{0})", row - 1);
                                ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(row, 6).FormulaA1 = string.Format("SUM(F6:F{0})", row - 1);
                                ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(row, 7).Value = _currentHeader?.ClosingBalance ?? 0m;
                                ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
                            }

                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Item Ledger exported successfully to Excel.", "Export Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to export Excel file: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportToCsv()
        {
            if (_filteredItems == null || _filteredItems.Count == 0)
            {
                MessageBox.Show("No item ledger records available to export.", "Export Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                string safeItem = (_currentHeader?.ItemTitle ?? "ItemLedger").Replace(" ", "_").Replace("/", "-");
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.FileName = string.Format("{0}_{1:yyyyMMdd}_{2:yyyyMMdd}.csv", safeItem, dtpFrom.Value, dtpTo.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(sfd.FileName))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            bool showCostPrice = _currentHeader?.ShowCostPrice == true;
                            if (showCostPrice)
                            {
                                var exportRows = _filteredItems.Select(x => new
                                {
                                    Date = x.FormattedDate,
                                    VoucherNo = x.VoucherNo,
                                    Particular = x.Particular,
                                    Rate = x.Rate,
                                    CostPrice = x.CostPrice,
                                    Inward = x.QtyIn,
                                    Outward = x.QtyOut,
                                    Balance = x.Balance
                                }).ToList();
                                csv.WriteRecords(exportRows);
                            }
                            else
                            {
                                var exportRows = _filteredItems.Select(x => new
                                {
                                    Date = x.FormattedDate,
                                    VoucherNo = x.VoucherNo,
                                    Particular = x.Particular,
                                    Rate = x.Rate,
                                    Inward = x.QtyIn,
                                    Outward = x.QtyOut,
                                    Balance = x.Balance
                                }).ToList();
                                csv.WriteRecords(exportRows);
                            }
                        }

                        MessageBox.Show("Item Ledger exported successfully to CSV.", "Export Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to export CSV file: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void TriggerPrint()
        {
            if (!_isWebViewReady || webView == null || webView.CoreWebView2 == null)
            {
                MessageBox.Show("Preview is not ready for printing.", "Print Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                webView.CoreWebView2.ExecuteScriptAsync("window.print();");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening print dialog: " + ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CleanupTempFile()
        {
            if (!string.IsNullOrWhiteSpace(_currentPdfPath) && File.Exists(_currentPdfPath))
            {
                try
                {
                    File.Delete(_currentPdfPath);
                }
                catch
                {
                    // Non-blocking cleanup
                }
            }
        }
    }
}
