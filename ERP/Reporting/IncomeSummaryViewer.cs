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
using Microsoft.Web.WebView2.WinForms;

namespace ERP.Reporting
{
    /// <summary>
    /// Modern, code-first WinForms Income Summary (Profit & Loss) Viewer hosting WebView2
    /// with QuestPDF financial rendering and ClosedXML / CsvHelper export functionality.
    /// </summary>
    public class IncomeSummaryViewer : Form
    {
        // UI Controls
        private Panel pnlTopBar;
        private Label lblFrom;
        private DateTimePicker dtpFrom;
        private Label lblTo;
        private DateTimePicker dtpTo;
        private Button btnGenerate;
        private Button btnExportExcel;
        private Button btnExportCsv;
        private Button btnPrint;
        private Label lblStatus;
        private ProgressBar prgLoading;
        private WebView2 webView;

        // State
        private IncomeSummaryDataResult _currentResult;
        private string _currentPdfPath;
        private bool _isWebViewReady = false;

        public IncomeSummaryViewer()
        {
            InitializeComponentCodeFirst();
        }

        public IncomeSummaryViewer(DateTime fromDate, DateTime toDate)
            : this()
        {
            if (dtpFrom != null) dtpFrom.Value = fromDate;
            if (dtpTo != null) dtpTo.Value = toDate;
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Income Summary";
            this.Size = new Size(1160, 820);
            this.MinimumSize = new Size(980, 650);
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

            // From Date Control
            lblFrom = new Label
            {
                Text = "FROM DATE",
                AutoSize = true,
                Location = new Point(16, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpFrom = new DateTimePicker
            {
                Location = new Point(16, 32),
                Width = 115,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
            };

            // To Date Control
            lblTo = new Label
            {
                Text = "TO DATE",
                AutoSize = true,
                Location = new Point(146, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpTo = new DateTimePicker
            {
                Location = new Point(146, 32),
                Width = 115,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = DateTime.Today
            };

            // Generate Button
            btnGenerate = new Button
            {
                Text = "Generate",
                Location = new Point(278, 30),
                Width = 84,
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
                Location = new Point(368, 30),
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
                Location = new Point(438, 30),
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
                Location = new Point(498, 30),
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
                Location = new Point(572, 35),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129)
            };

            pnlTopBar.Controls.AddRange(new Control[]
            {
                lblFrom, dtpFrom,
                lblTo, dtpTo,
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
                lblStatus.Text = "Ready. Select date range and click 'Generate'.";
                lblStatus.ForeColor = Color.FromArgb(71, 85, 105);
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

        private async Task LoadAndRenderReportAsync()
        {
            if (!_isWebViewReady) return;

            DateTime fromDate = dtpFrom.Value;
            DateTime toDate = dtpTo.Value;

            SetLoading(true);
            try
            {
                DataTable dt = await Task.Run(() => ReportQuery.IncomeSummery(fromDate, toDate));
                var result = IncomeSummaryDataService.ConvertDataTable(dt, fromDate, toDate);

                _currentResult = result;

                // Update Status
                var h = _currentResult.Header;
                if (h.IsProfitable)
                {
                    lblStatus.Text = string.Format("Net Profit: Rs. {0:#,##0.00} ({1:F1}%) | Gross Margin: {2:F1}%", h.NetIncome, h.NetMarginPct, h.GrossMarginPct);
                    lblStatus.ForeColor = Color.FromArgb(16, 185, 129);
                }
                else
                {
                    lblStatus.Text = string.Format("Net Deficit: Rs. ({0:#,##0.00}) | Gross Margin: {1:F1}%", Math.Abs(h.NetIncome), h.GrossMarginPct);
                    lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
                }

                // Render PDF
                var doc = new IncomeSummaryDocument(h, _currentResult.SalesItems, _currentResult.CogsItems, _currentResult.ExpenseItems);
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
                MessageBox.Show("Error loading Income Summary report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoading(false);
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
            if (_currentResult == null)
            {
                MessageBox.Show("No income statement records available to export.", "Export Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = string.Format("IncomeStatement_{0:yyyyMMdd}_{1:yyyyMMdd}.xlsx", dtpFrom.Value, dtpTo.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Income Statement");
                            var h = _currentResult.Header;

                            // Brand Header
                            ws.Cell(1, 1).Value = h?.CompanyName ?? "Retail Suite Enterprise";
                            ws.Cell(1, 1).Style.Font.Bold = true;
                            ws.Cell(1, 1).Style.Font.FontSize = 14;

                            ws.Cell(2, 1).Value = "INCOME STATEMENT / PROFIT & LOSS SUMMARY";
                            ws.Cell(2, 1).Style.Font.Bold = true;
                            ws.Cell(2, 1).Style.Font.FontSize = 11;

                            ws.Cell(3, 1).Value = string.Format("Period: {0:dd-MMM-yyyy} to {1:dd-MMM-yyyy} | Generated: {2:dd-MMM-yyyy HH:mm}", dtpFrom.Value, dtpTo.Value, DateTime.Now);
                            ws.Cell(3, 1).Style.Font.Italic = true;
                            ws.Cell(3, 1).Style.Font.FontColor = XLColor.FromArgb(100, 116, 139);

                            int row = 5;

                            // SECTION 1: REVENUE
                            ws.Cell(row, 1).Value = "1. REVENUE / OPERATING TURNOVER";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(224, 231, 255);
                            row++;

                            foreach (var item in _currentResult.SalesItems)
                            {
                                ws.Cell(row, 1).Value = item.Title;
                                ws.Cell(row, 2).Value = item.Amount;
                                ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                                row++;
                            }

                            // Subtotal Sales
                            ws.Cell(row, 1).Value = "TOTAL REVENUE (A)";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Cell(row, 2).Value = h.TotalSales;
                            ws.Cell(row, 2).Style.Font.Bold = true;
                            ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                            ws.Range(row, 1, row, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range(row, 1, row, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);
                            row += 2;

                            // SECTION 2: COGS
                            ws.Cell(row, 1).Value = "2. COST OF GOODS SOLD (COGS)";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);
                            row++;

                            foreach (var item in _currentResult.CogsItems)
                            {
                                ws.Cell(row, 1).Value = item.Title;
                                ws.Cell(row, 2).Value = item.Amount;
                                ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                                row++;
                            }

                            // Subtotal COGS
                            ws.Cell(row, 1).Value = "TOTAL COST OF GOODS SOLD (B)";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Cell(row, 2).Value = h.TotalCogs;
                            ws.Cell(row, 2).Style.Font.Bold = true;
                            ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                            ws.Range(row, 1, row, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range(row, 1, row, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);
                            row += 2;

                            // GROSS PROFIT ROW
                            ws.Cell(row, 1).Value = string.Format("GROSS PROFIT / (LOSS) [A - B]  (Margin: {0:F1}%)", h.GrossMarginPct);
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Cell(row, 2).Value = h.GrossProfit;
                            ws.Cell(row, 2).Style.Font.Bold = true;
                            ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(238, 242, 255);
                            ws.Range(row, 1, row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            row += 2;

                            // SECTION 3: EXPENSES
                            ws.Cell(row, 1).Value = "3. OPERATING EXPENSES";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(254, 226, 226);
                            row++;

                            foreach (var item in _currentResult.ExpenseItems)
                            {
                                ws.Cell(row, 1).Value = item.Title;
                                ws.Cell(row, 2).Value = item.Amount;
                                ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                                row++;
                            }

                            // Subtotal Expenses
                            ws.Cell(row, 1).Value = "TOTAL OPERATING EXPENSES (C)";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Cell(row, 2).Value = h.TotalExpenses;
                            ws.Cell(row, 2).Style.Font.Bold = true;
                            ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                            ws.Range(row, 1, row, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range(row, 1, row, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);
                            row += 2;

                            // NET PROFIT ROW
                            ws.Cell(row, 1).Value = string.Format("NET PROFIT / (LOSS) FOR THE PERIOD  (Net Margin: {0:F1}%)", h.NetMarginPct);
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Cell(row, 1).Style.Font.FontSize = 11;
                            ws.Cell(row, 2).Value = h.NetIncome;
                            ws.Cell(row, 2).Style.Font.Bold = true;
                            ws.Cell(row, 2).Style.Font.FontSize = 11;
                            ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                            ws.Range(row, 1, row, 2).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                            ws.Range(row, 1, row, 2).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);

                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Income Statement exported successfully to Excel.", "Export Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (_currentResult == null || _currentResult.AllItems.Count == 0)
            {
                MessageBox.Show("No income statement records available to export.", "Export Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.FileName = string.Format("IncomeStatement_{0:yyyyMMdd}_{1:yyyyMMdd}.csv", dtpFrom.Value, dtpTo.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(sfd.FileName))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            var exportRows = _currentResult.AllItems.Select(x => new
                            {
                                Category = x.Category,
                                Title = x.Title,
                                Amount = x.Amount,
                                Debit = x.Debit,
                                Credit = x.Credit
                            }).ToList();

                            csv.WriteRecords(exportRows);
                        }

                        MessageBox.Show("Income Statement exported successfully to CSV.", "Export Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
