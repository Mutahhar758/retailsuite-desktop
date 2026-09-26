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
    /// Modern WinForms viewer for Profit by Customer Report.
    /// Displays customer-level sales, cost of goods sold, gross margin, and profitability metrics.
    /// </summary>
    public class ProfitByCustomerViewer : Form
    {
        private readonly ReportsApiService _reportsApiService;

        // UI Controls - Top Bar
        private Panel pnlTopBar;
        private Label lblFrom;
        private DateTimePicker dtpFrom;
        private Label lblTo;
        private DateTimePicker dtpTo;
        private Button btnPresetMonth;
        private Button btnPresetLastMonth;
        private Button btnGenerate;
        private Button btnExportExcel;
        private Button btnExportCsv;
        private Button btnPrint;
        private Label lblStatus;
        private ProgressBar prgLoading;
        private WebView2 webView;

        // State
        private ProfitByCustomerDto _currentResult;
        private ProfitByCustomerHeader _currentHeader;
        private List<ProfitByCustomerReportItem> _currentItems = new List<ProfitByCustomerReportItem>();
        private string _currentPdfPath;
        private bool _isWebViewReady = false;

        public ProfitByCustomerViewer()
        {
            _reportsApiService = new ReportsApiService();
            InitializeComponentCodeFirst();
        }

        public ProfitByCustomerViewer(DateTime fromDate, DateTime toDate, string customerAccount = null)
            : this()
        {
            if (dtpFrom != null) dtpFrom.Value = fromDate;
            if (dtpTo != null) dtpTo.Value = toDate;
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Profit by Customer";
            this.Size = new Size(1320, 840);
            this.MinimumSize = new Size(1020, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.BackColor = Color.FromArgb(248, 250, 252);

            // 1. Top Control Bar
            pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 76,
                BackColor = Color.White,
                Padding = new Padding(12, 8, 12, 8)
            };
            pnlTopBar.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
            };

            // From Date
            lblFrom = new Label
            {
                Text = "FROM DATE",
                AutoSize = true,
                Location = new Point(16, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpFrom = new DateTimePicker
            {
                Location = new Point(16, 30),
                Width = 110,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
            };

            // To Date
            lblTo = new Label
            {
                Text = "TO DATE",
                AutoSize = true,
                Location = new Point(136, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpTo = new DateTimePicker
            {
                Location = new Point(136, 30),
                Width = 110,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = DateTime.Today
            };

            // Presets
            btnPresetMonth = CreatePresetButton("This Month", 256, 30, () =>
            {
                dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dtpTo.Value = DateTime.Today;
            });

            btnPresetLastMonth = CreatePresetButton("Last Month", 330, 30, () =>
            {
                var prev = DateTime.Today.AddMonths(-1);
                dtpFrom.Value = new DateTime(prev.Year, prev.Month, 1);
                dtpTo.Value = new DateTime(prev.Year, prev.Month, DateTime.DaysInMonth(prev.Year, prev.Month));
            });

            // Action Buttons
            btnGenerate = new Button
            {
                Text = "Generate",
                Location = new Point(410, 28),
                Width = 84,
                Height = 30,
                BackColor = Color.FromArgb(14, 165, 233),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.Click += async (s, e) => await LoadAndRenderReportAsync();

            btnExportExcel = new Button
            {
                Text = "Excel",
                Location = new Point(502, 28),
                Width = 68,
                Height = 30,
                BackColor = Color.FromArgb(22, 163, 74),
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
                Location = new Point(578, 28),
                Width = 60,
                Height = 30,
                BackColor = Color.FromArgb(71, 85, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportCsv.FlatAppearance.BorderSize = 0;
            btnExportCsv.Click += (s, e) => ExportToCsv();

            btnPrint = new Button
            {
                Text = "Print",
                Location = new Point(646, 28),
                Width = 60,
                Height = 30,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Click += (s, e) =>
            {
                if (webView != null && _isWebViewReady)
                    webView.CoreWebView2.ShowPrintUI(Microsoft.Web.WebView2.Core.CoreWebView2PrintDialogKind.Browser);
            };

            // Status label
            lblStatus = new Label
            {
                Text = "Ready",
                AutoSize = true,
                Location = new Point(716, 35),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            pnlTopBar.Controls.AddRange(new Control[]
            {
                lblFrom, dtpFrom,
                lblTo, dtpTo,
                btnPresetMonth, btnPresetLastMonth,
                btnGenerate, btnExportExcel, btnExportCsv, btnPrint,
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
                lblStatus.Text = "Ready. Select period and click 'Generate'.";
                await LoadAndRenderReportAsync();
            };

            this.FormClosing += (s, e) => CleanupTempFile();
        }

        private Button CreatePresetButton(string text, int x, int y, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(text.Length > 8 ? 70 : 54, 26),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(71, 85, 105),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btn.FlatAppearance.BorderSize = 1;
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private async Task InitializeWebViewAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                _isWebViewReady = true;
                webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "WebView2 initialization failed: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
        }

        private async Task LoadAndRenderReportAsync()
        {
            if (!_isWebViewReady) return;

            DateTime fromDate = dtpFrom.Value;
            DateTime toDate = dtpTo.Value;

            SetLoading(true);
            try
            {
                var dto = await Task.Run(() => _reportsApiService.GetProfitByCustomerAsync(fromDate, toDate, null));
                _currentResult = dto;

                _currentHeader = new ProfitByCustomerHeader
                {
                    CompanyName = "RETAIL SUITE",
                    FromDate = dto.FromDate,
                    ToDate = dto.ToDate,
                    CustomerFilter = "All Customers",
                    TotalSales = dto.TotalSales,
                    TotalCost = dto.TotalCost,
                    TotalQtySold = dto.TotalQtySold,
                    TotalCustomers = dto.CustomerCount
                };

                _currentItems = (dto.Lines ?? new List<ProfitByCustomerLineDto>()).Select(l => new ProfitByCustomerReportItem
                {
                    AccountId = l.AccountId,
                    AccountTitle = l.AccountTitle,
                    City = l.City,
                    Phone = l.Phone,
                    InvoiceCount = l.InvoiceCount,
                    TotalQty = l.TotalQty,
                    TotalSales = l.TotalSales,
                    TotalCost = l.TotalCost,
                    Details = l.Details != null
                        ? l.Details.Select(d => new ProfitByCustomerDetailItem
                        {
                            VDate = d.VDate,
                            VNo = d.VNo,
                            VType = d.VType,
                            ItemId = d.ItemId,
                            ItemTitle = d.ItemTitle,
                            Unit = d.Unit,
                            Qty = d.Qty,
                            SaleRate = d.SaleRate,
                            SaleAmount = d.SaleAmount,
                            CostPrice = d.CostPrice,
                            CostAmount = d.CostAmount
                        }).ToList()
                        : new List<ProfitByCustomerDetailItem>()
                }).ToList();

                // Update Status KPIs
                if (_currentHeader.IsProfitable)
                {
                    lblStatus.Text = string.Format("Gross Profit: Rs. {0:#,##0.00} ({1:F1}%) | Sales: Rs. {2:#,##0.00} | Cost: Rs. {3:#,##0.00}",
                        _currentHeader.GrossProfit, _currentHeader.GrossMarginPct, _currentHeader.TotalSales, _currentHeader.TotalCost);
                    lblStatus.ForeColor = Color.FromArgb(16, 185, 129);
                }
                else
                {
                    lblStatus.Text = string.Format("Gross Loss: Rs. ({0:#,##0.00}) ({1:F1}%) | Sales: Rs. {2:#,##0.00}",
                        Math.Abs(_currentHeader.GrossProfit), _currentHeader.GrossMarginPct, _currentHeader.TotalSales);
                    lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
                }

                // Render PDF
                var doc = new ProfitByCustomerDocument(_currentHeader, _currentItems);
                string newPdfPath = await doc.GeneratePdfToTempFileAsync();

                CleanupTempFile();
                _currentPdfPath = newPdfPath;

                if (webView != null && webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.Navigate(_currentPdfPath);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show("Failed to generate report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void SetLoading(bool loading)
        {
            prgLoading.Visible = loading;
            btnGenerate.Enabled = !loading;
            btnExportExcel.Enabled = !loading;
            btnExportCsv.Enabled = !loading;
            btnPrint.Enabled = !loading;

            if (loading)
            {
                lblStatus.Text = "Calculating profit metrics...";
                lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
            }
        }

        private void CleanupTempFile()
        {
            if (!string.IsNullOrEmpty(_currentPdfPath) && File.Exists(_currentPdfPath))
            {
                try
                {
                    File.Delete(_currentPdfPath);
                }
                catch { }
            }
        }

        private void ExportToExcel()
        {
            if (_currentItems == null || _currentItems.Count == 0)
            {
                MessageBox.Show("No records available to export.", "Export Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = string.Format("ProfitByCustomer_{0:yyyyMMdd}_{1:yyyyMMdd}.xlsx", dtpFrom.Value, dtpTo.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Profit By Customer");
                            var h = _currentHeader;

                            // Title & Header info
                            ws.Cell(1, 1).Value = h?.CompanyName ?? "Retail Suite Enterprise";
                            ws.Cell(1, 1).Style.Font.Bold = true;
                            ws.Cell(1, 1).Style.Font.FontSize = 14;

                            ws.Cell(2, 1).Value = "PROFIT BY CUSTOMER REPORT";
                            ws.Cell(2, 1).Style.Font.Bold = true;
                            ws.Cell(2, 1).Style.Font.FontSize = 11;

                            ws.Cell(3, 1).Value = string.Format("Period: {0:dd-MMM-yyyy} to {1:dd-MMM-yyyy} | Filter: {2} | Generated: {3:dd-MMM-yyyy HH:mm}",
                                dtpFrom.Value, dtpTo.Value, h?.CustomerFilter ?? "All Customers", DateTime.Now);
                            ws.Cell(3, 1).Style.Font.Italic = true;
                            ws.Cell(3, 1).Style.Font.FontColor = XLColor.FromArgb(100, 116, 139);

                            // Table Headers (no customer code)
                            int row = 5;
                            string[] headers = { "#", "Customer Title", "City", "Invoices", "Qty Sold", "Sales (Rs.)", "Cost Amount (Rs.)", "Gross Profit (Rs.)", "Margin %" };
                            for (int c = 0; c < headers.Length; c++)
                            {
                                ws.Cell(row, c + 1).Value = headers[c];
                                ws.Cell(row, c + 1).Style.Font.Bold = true;
                                ws.Cell(row, c + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(30, 41, 59);
                                ws.Cell(row, c + 1).Style.Font.FontColor = XLColor.White;
                                ws.Cell(row, c + 1).Style.Alignment.Horizontal = c >= 3 ? XLAlignmentHorizontalValues.Right : XLAlignmentHorizontalValues.Left;
                            }
                            row++;

                            // Table Data
                            int itemIndex = 1;
                            foreach (var item in _currentItems)
                            {
                                ws.Cell(row, 1).Value = itemIndex++;
                                ws.Cell(row, 2).Value = item.AccountTitle;
                                ws.Cell(row, 3).Value = item.City ?? string.Empty;
                                ws.Cell(row, 4).Value = item.InvoiceCount;
                                ws.Cell(row, 5).Value = item.TotalQty;
                                ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0.##";
                                ws.Cell(row, 6).Value = item.TotalSales;
                                ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(row, 7).Value = item.TotalCost;
                                ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(row, 8).Value = item.GrossProfit;
                                ws.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(row, 9).Value = item.GrossMarginPct / 100m;
                                ws.Cell(row, 9).Style.NumberFormat.Format = "0.0%";

                                if (item.GrossProfit < 0)
                                {
                                    ws.Cell(row, 8).Style.Font.FontColor = XLColor.Red;
                                    ws.Cell(row, 9).Style.Font.FontColor = XLColor.Red;
                                }

                                row++;
                            }

                            // Total Summary Row
                            ws.Cell(row, 1).Value = "TOTAL SUMMARY";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Range(row, 1, row, 3).Merge();

                            ws.Cell(row, 4).Value = _currentItems.Sum(x => x.InvoiceCount);
                            ws.Cell(row, 4).Style.Font.Bold = true;

                            ws.Cell(row, 5).Value = h.TotalQtySold;
                            ws.Cell(row, 5).Style.Font.Bold = true;
                            ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0.##";

                            ws.Cell(row, 6).Value = h.TotalSales;
                            ws.Cell(row, 6).Style.Font.Bold = true;
                            ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";

                            ws.Cell(row, 7).Value = h.TotalCost;
                            ws.Cell(row, 7).Style.Font.Bold = true;
                            ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";

                            ws.Cell(row, 8).Value = h.GrossProfit;
                            ws.Cell(row, 8).Style.Font.Bold = true;
                            ws.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";

                            ws.Cell(row, 9).Value = h.GrossMarginPct / 100m;
                            ws.Cell(row, 9).Style.Font.Bold = true;
                            ws.Cell(row, 9).Style.NumberFormat.Format = "0.0%";

                            ws.Range(row, 1, row, 9).Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);
                            ws.Range(row, 1, row, 9).Style.Border.TopBorder = XLBorderStyleValues.Double;
                            ws.Range(row, 1, row, 9).Style.Border.BottomBorder = XLBorderStyleValues.Double;

                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Profit by Customer report exported successfully to Excel.", "Export Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (_currentItems == null || _currentItems.Count == 0)
            {
                MessageBox.Show("No records available to export.", "Export Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.FileName = string.Format("ProfitByCustomer_{0:yyyyMMdd}_{1:yyyyMMdd}.csv", dtpFrom.Value, dtpTo.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(sfd.FileName))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            var exportRows = _currentItems.Select((i, idx) => new
                            {
                                SrNo = idx + 1,
                                CustomerName = i.AccountTitle,
                                City = i.City,
                                Invoices = i.InvoiceCount,
                                QtySold = i.TotalQty,
                                SalesAmount = i.TotalSales,
                                CostAmount = i.TotalCost,
                                GrossProfit = i.GrossProfit,
                                MarginPct = i.GrossMarginPct
                            });

                            csv.WriteRecords(exportRows);
                        }

                        MessageBox.Show("Profit by Customer report exported successfully to CSV.", "Export Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to export CSV file: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
