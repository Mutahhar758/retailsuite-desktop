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
    /// Modern WinForms viewer for Customer Balance & Recovery Reconciliation.
    /// Provides parity with retailsuite-web-retail's CustomerBalanceRecoveryReport.
    /// </summary>
    public class CustomerBalanceRecoveryViewer : Form
    {
        private readonly ChartOfAccountApiService _chartOfAccountApiService;

        // UI Controls - Top Bar
        private Panel pnlTopBar;
        private Label lblCustomer;
        private ComboBox cmbCustomer;
        private Label lblFrom;
        private DateTimePicker dtpFrom;
        private Label lblTo;
        private DateTimePicker dtpTo;
        private Label lblBasis;
        private ComboBox cmbDateBasis;
        private Label lblFilter;
        private ComboBox cmbFilter;

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
        private List<ChartOfAccountHeadDto> _allCustomers = new List<ChartOfAccountHeadDto>();
        private CustomerBalanceRecoveryHeader _currentHeader;
        private List<CustomerBalanceRecoveryLineItem> _currentLines = new List<CustomerBalanceRecoveryLineItem>();
        private CustomerBalanceRecoverySummary _currentSummary;
        private string _currentPdfPath;
        private bool _isWebViewReady = false;
        private bool _isPopulatingCustomers = false;

        public CustomerBalanceRecoveryViewer()
        {
            _chartOfAccountApiService = new ChartOfAccountApiService();
            InitializeComponentCodeFirst();
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Customer Balance & Recovery";
            this.Size = new Size(1320, 820);
            this.MinimumSize = new Size(1020, 620);
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

            // Customer Selector
            lblCustomer = new Label
            {
                Text = "CUSTOMER ACCOUNT",
                AutoSize = true,
                Location = new Point(12, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            cmbCustomer = new ComboBox
            {
                Location = new Point(12, 30),
                Width = 210,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };

            // From Date
            lblFrom = new Label
            {
                Text = "FROM DATE",
                AutoSize = true,
                Location = new Point(230, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpFrom = new DateTimePicker
            {
                Location = new Point(230, 30),
                Width = 100,
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
                Location = new Point(336, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpTo = new DateTimePicker
            {
                Location = new Point(336, 30),
                Width = 100,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 8.5F),
                Value = DateTime.Today
            };

            // Presets row
            int presetY = 56;
            btnPreset1to10 = CreatePresetButton("1-10", 230, presetY, () => ApplyPreset(1, 10));
            btnPreset1to15 = CreatePresetButton("1-15", 272, presetY, () => ApplyPreset(1, 15));
            btnPreset1to20 = CreatePresetButton("1-20", 314, presetY, () => ApplyPreset(1, 20));
            btnPresetMonth = CreatePresetButton("This Mo", 356, presetY, () =>
            {
                dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dtpTo.Value = DateTime.Today;
            });
            btnPresetLastMonth = CreatePresetButton("Last Mo", 410, presetY, () =>
            {
                var prev = DateTime.Today.AddMonths(-1);
                dtpFrom.Value = new DateTime(prev.Year, prev.Month, 1);
                dtpTo.Value = new DateTime(prev.Year, prev.Month, DateTime.DaysInMonth(prev.Year, prev.Month));
            });

            // Date Basis
            lblBasis = new Label
            {
                Text = "DATE BASIS",
                AutoSize = true,
                Location = new Point(444, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            cmbDateBasis = new ComboBox
            {
                Location = new Point(444, 30),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8.5F)
            };
            cmbDateBasis.Items.AddRange(new object[] { "Clearing Date", "Voucher Date" });
            cmbDateBasis.SelectedIndex = 0;

            // Status Filter
            lblFilter = new Label
            {
                Text = "STATUS FILTER",
                AutoSize = true,
                Location = new Point(570, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            cmbFilter = new ComboBox
            {
                Location = new Point(570, 30),
                Width = 125,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8.5F)
            };
            cmbFilter.Items.AddRange(new object[] { "All Balances", "Outstanding Only", "Cleared Only", "Unpaid Only" });
            cmbFilter.SelectedIndex = 0;

            // Action Buttons
            btnGenerate = new Button
            {
                Text = "Generate",
                Location = new Point(705, 28),
                Width = 78,
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
                Location = new Point(788, 28),
                Width = 60,
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
                Location = new Point(852, 28),
                Width = 52,
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
                Location = new Point(908, 28),
                Width = 60,
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
                Location = new Point(972, 28),
                Width = 74,
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
                Location = new Point(1054, 35),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            pnlTopBar.Controls.AddRange(new Control[]
            {
                lblCustomer, cmbCustomer,
                lblFrom, dtpFrom,
                lblTo, dtpTo,
                btnPreset1to10, btnPreset1to15, btnPreset1to20, btnPresetMonth, btnPresetLastMonth,
                lblBasis, cmbDateBasis,
                lblFilter, cmbFilter,
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
                await PopulateCustomersAsync();
                if (lblStatus != null) lblStatus.Text = "Ready. Select customer & date range, then click 'Generate'.";
            };

            this.FormClosing += (s, e) => CleanupTempFile();
        }

        private Button CreatePresetButton(string text, int x, int y, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(text.Length > 5 ? 50 : 38, 20),
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
                string userDataFolder = Path.Combine(Path.GetTempPath(), "RetailSuite", "WebView2_CustomerRecovery");
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

        private async Task PopulateCustomersAsync()
        {
            try
            {
                _isPopulatingCustomers = true;
                var customers = await _chartOfAccountApiService.GetCustomerAccountsAsync();
                _allCustomers = customers ?? new List<ChartOfAccountHeadDto>();

                var dt = new DataTable();
                dt.Columns.Add("Account", typeof(string));
                dt.Columns.Add("Title", typeof(string));

                dt.Rows.Add("", "--- All Customers ---");

                if (_allCustomers.Count > 0)
                {
                    foreach (var c in _allCustomers) dt.Rows.Add(c.Account, c.Title);
                }

                cmbCustomer.DisplayMember = "Title";
                cmbCustomer.ValueMember = "Account";
                cmbCustomer.DataSource = dt;
                cmbCustomer.SelectedIndex = 0;
            }
            catch
            {
                // Ignore fallback
            }
            finally
            {
                _isPopulatingCustomers = false;
            }
        }

        private async Task LoadAndRenderReportAsync()
        {
            if (!_isWebViewReady) return;

            string custAccount = cmbCustomer.SelectedValue != null ? cmbCustomer.SelectedValue.ToString() : null;
            string custTitle = cmbCustomer.Text;

            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date;

            string basis = cmbDateBasis.SelectedIndex == 1 ? "VoucherDate" : "ClearingDate";
            string filter = "All";
            if (cmbFilter.SelectedIndex == 1) filter = "OutstandingOnly";
            else if (cmbFilter.SelectedIndex == 2) filter = "ClearedOnly";
            else if (cmbFilter.SelectedIndex == 3) filter = "UnpaidOnly";

            prgLoading.Visible = true;
            lblStatus.Text = "Loading customer recovery data...";

            try
            {
                var data = await CustomerBalanceRecoveryDataService.GetRecoveryDataAsync(
                    fromDate, toDate, custAccount, custTitle, basis, filter);

                _currentHeader = data.Header;
                _currentLines = data.Lines;
                _currentSummary = data.Summary;

                var doc = new CustomerBalanceRecoveryDocument(_currentHeader, _currentLines, _currentSummary);
                string newPdfPath = await doc.GeneratePdfToTempFileAsync();

                CleanupTempFile();
                _currentPdfPath = newPdfPath;

                webView.CoreWebView2.Navigate(_currentPdfPath);

                lblStatus.Text = string.Format("{0} Customers • Due: Rs. {1:N0} • Rec: Rs. {2:N0} ({3:N1}%) • Cls: Rs. {4:N0}",
                    _currentLines.Count, _currentSummary.TotalDue, _currentSummary.TotalRecovery,
                    _currentSummary.OverallRecoveryRate, _currentSummary.TotalClosingBalance);
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
                MessageBox.Show("No recovery data available to print.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string printer = !string.IsNullOrWhiteSpace(ConfigInfo.ThermalPrinterName) ? ConfigInfo.ThermalPrinterName : null;
            try
            {
                var doc = new CustomerBalanceRecoveryDocument(_currentHeader, _currentLines, _currentSummary);
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
                sfd.FileName = string.Format("CustomerRecovery_{0:yyyyMMdd}_{1:yyyyMMdd}.xlsx", dtpFrom.Value, dtpTo.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Customer Recovery");

                            // Title
                            ws.Cell("A1").Value = _currentHeader.CompanyName.ToUpper();
                            ws.Cell("A1").Style.Font.Bold = true;
                            ws.Cell("A1").Style.Font.FontSize = 14;

                            ws.Cell("A2").Value = "CUSTOMER BALANCE & RECOVERY RECONCILIATION REPORT";
                            ws.Cell("A2").Style.Font.Bold = true;

                            ws.Cell("A3").Value = string.Format("Period: {0:dd-MMM-yyyy} to {1:dd-MMM-yyyy} | Basis: {2} | Filter: {3}",
                                _currentHeader.FromDate, _currentHeader.ToDate, _currentHeader.DateBasis, _currentHeader.BalanceFilter);

                            // Headers
                            int row = 5;
                            string[] headers = new[]
                            {
                                "#", "Account ID", "Customer Title", "Phone", "Address",
                                "Prev Balance", "Current Billing", "Total Due",
                                "Recovery", "Closing Balance", "Recovery %", "Status"
                            };

                            for (int c = 0; c < headers.Length; c++)
                            {
                                var cell = ws.Cell(row, c + 1);
                                cell.Value = headers[c];
                                cell.Style.Font.Bold = true;
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A8A");
                                cell.Style.Font.FontColor = XLColor.White;
                                cell.Style.Alignment.Horizontal = c >= 5 && c <= 10 ? XLAlignmentHorizontalValues.Right : XLAlignmentHorizontalValues.Left;
                            }

                            // Data Rows
                            row = 6;
                            for (int i = 0; i < _currentLines.Count; i++)
                            {
                                var item = _currentLines[i];
                                ws.Cell(row, 1).Value = i + 1;
                                ws.Cell(row, 2).Value = item.CustomerAccountId;
                                ws.Cell(row, 3).Value = item.CustomerTitle;
                                ws.Cell(row, 4).Value = item.Phone;
                                ws.Cell(row, 5).Value = item.Address;
                                ws.Cell(row, 6).Value = item.PreviousBalance;
                                ws.Cell(row, 7).Value = item.CurrentBilling;
                                ws.Cell(row, 8).Value = item.TotalDue;
                                ws.Cell(row, 9).Value = item.RecoveryAmount;
                                ws.Cell(row, 10).Value = item.ClosingBalance;
                                ws.Cell(row, 11).Value = item.RecoveryPercentage / 100m;
                                ws.Cell(row, 12).Value = item.Status;

                                ws.Range(row, 6, row, 10).Style.NumberFormat.Format = "#,##0";
                                ws.Cell(row, 11).Style.NumberFormat.Format = "0.0%";
                                row++;
                            }

                            // Grand Totals
                            ws.Cell(row, 1).Value = "";
                            ws.Cell(row, 2).Value = "";
                            ws.Cell(row, 3).Value = "GRAND TOTAL";
                            ws.Cell(row, 3).Style.Font.Bold = true;
                            ws.Cell(row, 6).Value = _currentSummary.TotalPreviousBalance;
                            ws.Cell(row, 7).Value = _currentSummary.TotalCurrentBilling;
                            ws.Cell(row, 8).Value = _currentSummary.TotalDue;
                            ws.Cell(row, 9).Value = _currentSummary.TotalRecovery;
                            ws.Cell(row, 10).Value = _currentSummary.TotalClosingBalance;
                            ws.Cell(row, 11).Value = _currentSummary.OverallRecoveryRate / 100m;

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
                sfd.FileName = string.Format("CustomerRecovery_{0:yyyyMMdd}_{1:yyyyMMdd}.csv", dtpFrom.Value, dtpTo.Value);

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
