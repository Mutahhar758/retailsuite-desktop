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
    /// Modern, code-first WinForms Account Balance (Balance Detail) Viewer hosting WebView2
    /// with QuestPDF financial rendering and ClosedXML / CsvHelper export functionality.
    /// </summary>
    public class AccountBalanceViewer : Form
    {
        // UI Controls
        private Panel pnlTopBar;
        private Label lblAccountHead;
        private ComboBox cmbAccountHead;
        private Label lblAsOn;
        private DateTimePicker dtpAsOn;
        private Label lblFilter;
        private ComboBox cmbFilter;
        private Label lblSearch;
        private TextBox txtSearch;
        private Button btnGenerate;
        private Button btnExportExcel;
        private Button btnExportCsv;
        private Button btnPrint;
        private Label lblStatus;
        private ProgressBar prgLoading;
        private WebView2 webView;

        // API Service
        private readonly ChartOfAccountApiService _chartOfAccountApiService = new ChartOfAccountApiService();

        // State
        private AccountBalanceHeader _currentHeader;
        private List<AccountBalanceReportItem> _allItems = new List<AccountBalanceReportItem>();
        private List<AccountBalanceReportItem> _filteredItems = new List<AccountBalanceReportItem>();
        private string _currentPdfPath;
        private bool _isWebViewReady = false;
        private bool _isPopulatingHeads = false;

        public AccountBalanceViewer()
        {
            InitializeComponentCodeFirst();
        }

        public AccountBalanceViewer(string selectedAccountHeadId, DateTime asOnDate)
            : this()
        {
            if (dtpAsOn != null) dtpAsOn.Value = asOnDate;
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Account Balance";
            this.Size = new Size(1220, 820);
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

            // Account Head Dropdown
            lblAccountHead = new Label
            {
                Text = "ACCOUNT HEAD (LVL 4)",
                AutoSize = true,
                Location = new Point(16, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            cmbAccountHead = new ComboBox
            {
                Location = new Point(16, 32),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbAccountHead.SelectedIndexChanged += (s, e) =>
            {
                if (!_isPopulatingHeads)
                {
                    _ = LoadAndRenderReportAsync();
                }
            };

            // As On Date Control
            lblAsOn = new Label
            {
                Text = "AS ON DATE",
                AutoSize = true,
                Location = new Point(246, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpAsOn = new DateTimePicker
            {
                Location = new Point(246, 32),
                Width = 115,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = DateTime.Today
            };

            // Balance Filter Dropdown
            lblFilter = new Label
            {
                Text = "BALANCE FILTER",
                AutoSize = true,
                Location = new Point(371, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            cmbFilter = new ComboBox
            {
                Location = new Point(371, 32),
                Width = 135,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbFilter.Items.AddRange(new object[] { "All Balances", "Debit Only (> 0)", "Credit Only (< 0)" });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => ApplyClientFilter();

            // Search Box
            lblSearch = new Label
            {
                Text = "SEARCH ACCOUNT",
                AutoSize = true,
                Location = new Point(516, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            txtSearch = new TextBox
            {
                Location = new Point(516, 32),
                Width = 140,
                Font = new Font("Segoe UI", 9F)
            };
            txtSearch.TextChanged += (s, e) => ApplyClientFilter();

            // Generate Button
            btnGenerate = new Button
            {
                Text = "Generate",
                Location = new Point(668, 30),
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
                Location = new Point(758, 30),
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
                Location = new Point(828, 30),
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
                Location = new Point(888, 30),
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
                Location = new Point(958, 35),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129)
            };

            pnlTopBar.Controls.AddRange(new Control[]
            {
                lblAccountHead, cmbAccountHead,
                lblAsOn, dtpAsOn,
                lblFilter, cmbFilter,
                lblSearch, txtSearch,
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
                await PopulateAccountHeadsAsync();
                lblStatus.Text = "Ready. Select an account head, then click 'Generate'.";
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

        private async Task PopulateAccountHeadsAsync()
        {
            try
            {
                _isPopulatingHeads = true;
                var heads = await _chartOfAccountApiService.GetHeadsAsync(4);

                var dt = new DataTable();
                dt.Columns.Add("Account", typeof(string));
                dt.Columns.Add("Title", typeof(string));

                if (heads != null && heads.Count > 0)
                {
                    foreach (var h in heads)
                    {
                        dt.Rows.Add(h.Account, h.Title);
                    }
                }

                cmbAccountHead.DataSource = dt;
                cmbAccountHead.DisplayMember = "Title";
                cmbAccountHead.ValueMember = "Account";

                if (cmbAccountHead.Items.Count > 0)
                {
                    cmbAccountHead.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading account heads: " + ex.Message;
            }
            finally
            {
                _isPopulatingHeads = false;
            }
        }

        private async Task LoadAndRenderReportAsync()
        {
            if (!_isWebViewReady) return;

            string selectedHeadId = cmbAccountHead.SelectedValue != null ? cmbAccountHead.SelectedValue.ToString() : string.Empty;
            string selectedHeadTitle = cmbAccountHead.Text ?? "Account Head";
            DateTime asOn = dtpAsOn.Value;

            SetLoading(true);
            try
            {
                DataTable dt = await Task.Run(() => ReportQuery.Balance(selectedHeadId, asOn));
                var result = AccountBalanceDataService.ConvertDataTable(dt, selectedHeadId, selectedHeadTitle, asOn);

                _currentHeader = result.Header;
                _allItems = result.Items ?? new List<AccountBalanceReportItem>();

                ApplyClientFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Account Balance report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void ApplyClientFilter()
        {
            string filterType = "All";
            if (cmbFilter.SelectedIndex == 1) filterType = "Debit";
            else if (cmbFilter.SelectedIndex == 2) filterType = "Credit";

            string search = txtSearch.Text != null ? txtSearch.Text.Trim() : string.Empty;

            var items = _allItems.AsEnumerable();

            if (filterType == "Debit")
            {
                items = items.Where(x => x.Debit > 0);
            }
            else if (filterType == "Credit")
            {
                items = items.Where(x => x.Credit > 0);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                items = items.Where(x => (x.AccountTitle ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            _filteredItems = items.ToList();

            // Re-index
            for (int i = 0; i < _filteredItems.Count; i++)
            {
                _filteredItems[i].Index = i + 1;
            }

            UpdateStatusBadge();
            _ = RenderFilteredDocumentAsync();
        }

        private async Task RenderFilteredDocumentAsync()
        {
            if (!_isWebViewReady) return;

            try
            {
                var header = new AccountBalanceHeader
                {
                    CompanyName = _currentHeader != null ? _currentHeader.CompanyName : "Retail Suite Enterprise",
                    AccountHeadTitle = _currentHeader != null ? _currentHeader.AccountHeadTitle : "Account Head",
                    AccountHeadId = _currentHeader != null ? _currentHeader.AccountHeadId : string.Empty,
                    AsOnDate = dtpAsOn.Value,
                    TotalAccounts = _filteredItems.Count,
                    TotalDebit = _filteredItems.Sum(x => x.Debit),
                    TotalCredit = _filteredItems.Sum(x => x.Credit),
                    GeneratedAt = DateTime.Now
                };

                var doc = new AccountBalanceDocument(header, _filteredItems);
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
            if (_filteredItems != null && _filteredItems.Count > 0)
            {
                decimal dr = _filteredItems.Sum(x => x.Debit);
                decimal cr = _filteredItems.Sum(x => x.Credit);
                decimal net = dr - cr;
                string netSign = net >= 0 ? "Dr" : "Cr";

                lblStatus.Text = string.Format("Accounts: {0} | Net: Rs. {1:#,##0.00} {2}", _filteredItems.Count, Math.Abs(net), netSign);
                lblStatus.ForeColor = Color.FromArgb(37, 99, 235);
            }
            else
            {
                lblStatus.Text = "No accounts found";
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
                MessageBox.Show("No account balance records available to export.", "Export Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                string safeHead = (_currentHeader?.AccountHeadTitle ?? "AccountBalance").Replace(" ", "_").Replace("/", "-");
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = string.Format("{0}_{1:yyyyMMdd}.xlsx", safeHead, dtpAsOn.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Account Balances");

                            // Brand Header
                            ws.Cell(1, 1).Value = _currentHeader?.CompanyName ?? "Retail Suite Enterprise";
                            ws.Cell(1, 1).Style.Font.Bold = true;
                            ws.Cell(1, 1).Style.Font.FontSize = 14;

                            ws.Cell(2, 1).Value = string.Format("Account Balance Schedule: {0}", _currentHeader?.AccountHeadTitle ?? "All Heads");
                            ws.Cell(2, 1).Style.Font.Bold = true;
                            ws.Cell(2, 1).Style.Font.FontSize = 11;

                            ws.Cell(3, 1).Value = string.Format("As On: {0:dd-MMM-yyyy} | Generated: {1:dd-MMM-yyyy HH:mm}", dtpAsOn.Value, DateTime.Now);
                            ws.Cell(3, 1).Style.Font.Italic = true;
                            ws.Cell(3, 1).Style.Font.FontColor = XLColor.FromArgb(100, 116, 139);

                            // Columns Table Header
                            int row = 5;
                            string[] headers = { "#", "Account Title / Subsidiary", "Debit (Dr)", "Credit (Cr)", "Net Balance", "Type" };
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
                                ws.Cell(row, 1).Value = item.Index;
                                ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                ws.Cell(row, 2).Value = item.AccountTitle;
                                ws.Cell(row, 2).Style.Font.Bold = true;

                                ws.Cell(row, 3).Value = item.Debit;
                                ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";

                                ws.Cell(row, 4).Value = item.Credit;
                                ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";

                                ws.Cell(row, 5).Value = item.Balance;
                                ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";

                                ws.Cell(row, 6).Value = item.Nature;
                                ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                ws.Cell(row, 6).Style.Font.Bold = true;

                                if (row % 2 == 1)
                                {
                                    ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(248, 250, 252);
                                }

                                row++;
                            }

                            // Summary Row
                            var summaryRange = ws.Range(row, 1, row, 6);
                            summaryRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            summaryRange.Style.Border.BottomBorder = XLBorderStyleValues.Double;
                            summaryRange.Style.Font.Bold = true;
                            summaryRange.Style.Fill.BackgroundColor = XLColor.FromArgb(241, 245, 249);

                            ws.Cell(row, 1).Value = "TOTALS";
                            ws.Cell(row, 3).FormulaA1 = string.Format("SUM(C6:C{0})", row - 1);
                            ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(row, 4).FormulaA1 = string.Format("SUM(D6:D{0})", row - 1);
                            ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(row, 5).FormulaA1 = string.Format("C{0}-D{0}", row);
                            ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";

                            decimal netBal = _filteredItems.Sum(x => x.Debit) - _filteredItems.Sum(x => x.Credit);
                            ws.Cell(row, 6).Value = netBal >= 0 ? "Dr" : "Cr";
                            ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Account Balance exported successfully to Excel.", "Export Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("No account balance records available to export.", "Export Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                string safeHead = (_currentHeader?.AccountHeadTitle ?? "AccountBalance").Replace(" ", "_").Replace("/", "-");
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.FileName = string.Format("{0}_{1:yyyyMMdd}.csv", safeHead, dtpAsOn.Value);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(sfd.FileName))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            var exportRows = _filteredItems.Select(x => new
                            {
                                Index = x.Index,
                                AccountTitle = x.AccountTitle,
                                Debit = x.Debit,
                                Credit = x.Credit,
                                Balance = x.Balance,
                                Nature = x.Nature
                            }).ToList();

                            csv.WriteRecords(exportRows);
                        }

                        MessageBox.Show("Account Balance exported successfully to CSV.", "Export Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
