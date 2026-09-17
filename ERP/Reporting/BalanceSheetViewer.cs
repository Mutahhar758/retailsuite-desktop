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
    /// Modern, code-first WinForms Balance Sheet (Statement of Financial Position) Viewer hosting WebView2
    /// with QuestPDF financial rendering and ClosedXML / CsvHelper export functionality.
    /// </summary>
    public class BalanceSheetViewer : Form
    {
        // UI Controls
        private Panel pnlTopBar;
        private Label lblAsOn;
        private DateTimePicker dtpAsOn;
        private Button btnGenerate;
        private Button btnExportExcel;
        private Button btnExportCsv;
        private Button btnPrint;
        private Label lblStatus;
        private ProgressBar prgLoading;
        private WebView2 webView;

        // State
        private BalanceSheetDataResult _currentResult;
        private string _currentPdfPath;
        private bool _isWebViewReady = false;

        public BalanceSheetViewer()
        {
            InitializeComponentCodeFirst();
        }

        public BalanceSheetViewer(DateTime asOnDate)
            : this()
        {
            if (dtpAsOn != null) dtpAsOn.Value = asOnDate;
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Balance Sheet";
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

            // As On Date Control
            lblAsOn = new Label
            {
                Text = "AS OF DATE",
                AutoSize = true,
                Location = new Point(16, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpAsOn = new DateTimePicker
            {
                Location = new Point(16, 32),
                Width = 125,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = DateTime.Today
            };

            // Generate Button
            btnGenerate = new Button
            {
                Text = "Generate",
                Location = new Point(155, 30),
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
                Location = new Point(245, 30),
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
                Location = new Point(315, 30),
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
                Location = new Point(375, 30),
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
                Location = new Point(448, 35),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129)
            };

            pnlTopBar.Controls.AddRange(new Control[]
            {
                lblAsOn, dtpAsOn,
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
                lblStatus.Text = "Ready. Select 'As On' date and click 'Generate'.";
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

            DateTime asOn = dtpAsOn.Value;

            SetLoading(true);
            try
            {
                DataTable dt = await Task.Run(() => ReportQuery.BalanceSheet(asOn));
                var result = BalanceSheetDataService.ConvertDataTable(dt, asOn);

                _currentResult = result;

                // Update Status
                var h = _currentResult.Header;
                if (h.IsBalanced)
                {
                    lblStatus.Text = string.Format("✓ BALANCED | Assets: Rs. {0:#,##0.00} | Liab + Eq: Rs. {1:#,##0.00}", h.TotalAssets, h.TotalLiabilitiesAndEquity);
                    lblStatus.ForeColor = Color.FromArgb(16, 185, 129);
                }
                else
                {
                    lblStatus.Text = string.Format("⚠ OUT OF BALANCE: Variance Rs. {0:#,##0.00}", h.Variance);
                    lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
                }

                // Render PDF
                var doc = new BalanceSheetDocument(h, _currentResult.AssetItems, _currentResult.LiabilityItems, _currentResult.EquityItems);
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
                MessageBox.Show("Error loading Balance Sheet report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("No balance sheet records available to export.", "Export Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = string.Format("BalanceSheet_{0:yyyyMMdd}.xlsx", dtpAsOn.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Balance Sheet");
                            var h = _currentResult.Header;

                            // Brand Header
                            ws.Cell(1, 1).Value = h?.CompanyName ?? "Retail Suite Enterprise";
                            ws.Cell(1, 1).Style.Font.Bold = true;
                            ws.Cell(1, 1).Style.Font.FontSize = 14;

                            ws.Cell(2, 1).Value = "BALANCE SHEET (STATEMENT OF FINANCIAL POSITION)";
                            ws.Cell(2, 1).Style.Font.Bold = true;
                            ws.Cell(2, 1).Style.Font.FontSize = 11;

                            ws.Cell(3, 1).Value = string.Format("As of: {0:dd-MMM-yyyy} | Generated: {1:dd-MMM-yyyy HH:mm}", dtpAsOn.Value, DateTime.Now);
                            ws.Cell(3, 1).Style.Font.Italic = true;
                            ws.Cell(3, 1).Style.Font.FontColor = XLColor.FromArgb(100, 116, 139);

                            int row = 5;

                            // SECTION 1: ASSETS
                            ws.Cell(row, 1).Value = "1. ASSETS";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(224, 231, 255);
                            row++;

                            foreach (var item in _currentResult.AssetItems)
                            {
                                ws.Cell(row, 1).Value = item.Title;
                                ws.Cell(row, 2).Value = item.Amount;
                                ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                                row++;
                            }

                            // Total Assets
                            ws.Cell(row, 1).Value = "TOTAL ASSETS (A)";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Cell(row, 2).Value = h.TotalAssets;
                            ws.Cell(row, 2).Style.Font.Bold = true;
                            ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                            ws.Range(row, 1, row, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range(row, 1, row, 2).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);
                            row += 2;

                            // SECTION 2: LIABILITIES
                            ws.Cell(row, 1).Value = "2. LIABILITIES";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(254, 226, 226);
                            row++;

                            foreach (var item in _currentResult.LiabilityItems)
                            {
                                ws.Cell(row, 1).Value = item.Title;
                                ws.Cell(row, 2).Value = item.Amount;
                                ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                                row++;
                            }

                            // Total Liabilities
                            ws.Cell(row, 1).Value = "TOTAL LIABILITIES (B)";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Cell(row, 2).Value = h.TotalLiabilities;
                            ws.Cell(row, 2).Style.Font.Bold = true;
                            ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                            ws.Range(row, 1, row, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range(row, 1, row, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);
                            row += 2;

                            // SECTION 3: EQUITY
                            ws.Cell(row, 1).Value = "3. CAPITAL & OWNER'S EQUITY";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 252, 231);
                            row++;

                            foreach (var item in _currentResult.EquityItems)
                            {
                                ws.Cell(row, 1).Value = item.Title;
                                ws.Cell(row, 2).Value = item.Amount;
                                ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                                row++;
                            }

                            // Total Equity
                            ws.Cell(row, 1).Value = "TOTAL CAPITAL & EQUITY (C)";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Cell(row, 2).Value = h.TotalEquity;
                            ws.Cell(row, 2).Style.Font.Bold = true;
                            ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                            ws.Range(row, 1, row, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range(row, 1, row, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);
                            row += 2;

                            // GRAND RECONCILIATION ROW
                            ws.Cell(row, 1).Value = "TOTAL LIABILITIES & EQUITY  [B + C]";
                            ws.Cell(row, 1).Style.Font.Bold = true;
                            ws.Cell(row, 1).Style.Font.FontSize = 11;
                            ws.Cell(row, 2).Value = h.TotalLiabilitiesAndEquity;
                            ws.Cell(row, 2).Style.Font.Bold = true;
                            ws.Cell(row, 2).Style.Font.FontSize = 11;
                            ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                            ws.Range(row, 1, row, 2).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                            ws.Range(row, 1, row, 2).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);

                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Balance Sheet exported successfully to Excel.", "Export Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("No balance sheet records available to export.", "Export Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.FileName = string.Format("BalanceSheet_{0:yyyyMMdd}.csv", dtpAsOn.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(sfd.FileName))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            var exportRows = _currentResult.AllItems.Select(x => new
                            {
                                Section = x.Level1,
                                Category = x.Level2,
                                Title = x.Title,
                                Amount = x.Amount,
                                RawBalance = x.RawBalance
                            }).ToList();

                            csv.WriteRecords(exportRows);
                        }

                        MessageBox.Show("Balance Sheet exported successfully to CSV.", "Export Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
