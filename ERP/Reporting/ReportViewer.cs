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
    /// Modes supported by the modern ReportViewer.
    /// </summary>
    public enum ReportViewerMode
    {
        AccountStatement,
        AccountStatementWithDue
    }

    /// <summary>
    /// Modern, code-first WinForms Report Viewer hosting WebView2 with QuestPDF rendering
    /// and ClosedXML / CsvHelper export functionality. Supports multiple report modes.
    /// </summary>
    public class ReportViewer : Form
    {
        // Mode
        private readonly ReportViewerMode _mode;

        // UI Controls
        private Panel pnlTopBar;
        private Label lblAccount;
        private ComboBox cmbAccount;
        private Label lblFrom;
        private DateTimePicker dtpFrom;
        private Label lblTo;
        private DateTimePicker dtpTo;
        private Label lblDateBasis;
        private ComboBox cmbDateBasis;
        private Button btnRefresh;
        private Button btnExportExcel;
        private Button btnExportCsv;
        private Button btnPrint;
        private Label lblStatus;
        private ProgressBar prgLoading;
        private WebView2 webView;

        // State - Account Statement
        private AccountStatementHeader _currentHeader;
        private List<AccountStatementReportItem> _currentItems;

        // State - Account Statement With Due
        private AccountStatementWithDueHeader _currentDueHeader;
        private List<AccountStatementWithDueReportItem> _currentDueItems;

        private string _currentPdfPath;
        private bool _isWebViewReady = false;
        private readonly ChartOfAccountApiService _chartOfAccountService;

        public ReportViewer()
            : this(ReportViewerMode.AccountStatement)
        {
        }

        public ReportViewer(ReportViewerMode mode)
        {
            _mode = mode;
            _chartOfAccountService = new ChartOfAccountApiService();
            InitializeComponentCodeFirst();
        }

        public ReportViewer(string initialAccountCode, string initialAccountTitle, DateTime fromDate, DateTime toDate, ReportViewerMode mode = ReportViewerMode.AccountStatement)
            : this(mode)
        {
            if (dtpFrom != null) dtpFrom.Value = fromDate;
            if (dtpTo != null) dtpTo.Value = toDate;
        }

        /// <summary>
        /// Code-first UI setup with zero reliance on WinForms visual designer files.
        /// Executive, corporate design system matching enterprise financial systems.
        /// </summary>
        private void InitializeComponentCodeFirst()
        {
            this.Text = _mode == ReportViewerMode.AccountStatementWithDue
                ? "Account Statement With Due"
                : "Account Statement";

            this.Size = new Size(1180, 820);
            this.MinimumSize = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.BackColor = Color.FromArgb(248, 250, 252);

            // 1. Top Control Bar Panel (Executive White Bar with Hairline Slate Border)
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

            // Account Selection Label & Dropdown (Displays Pure Title, Value is Code)
            lblAccount = new Label
            {
                Text = "ACCOUNT",
                AutoSize = true,
                Location = new Point(16, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            cmbAccount = new ComboBox
            {
                Location = new Point(16, 32),
                Width = 260,
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
                Location = new Point(286, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpFrom = new DateTimePicker
            {
                Location = new Point(286, 32),
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
                Location = new Point(406, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            dtpTo = new DateTimePicker
            {
                Location = new Point(406, 32),
                Width = 110,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = DateTime.Today
            };

            // Date Basis Filter Control (Voucher Date vs Clearing Date)
            lblDateBasis = new Label
            {
                Text = "DATE BASIS",
                AutoSize = true,
                Location = new Point(526, 12),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            cmbDateBasis = new ComboBox
            {
                Location = new Point(526, 32),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbDateBasis.Items.AddRange(new object[] { "Voucher Date", "Clearing Date" });
            cmbDateBasis.SelectedIndex = 0;

            // Refresh / Generate Button (Executive Royal Blue Accent)
            btnRefresh = new Button
            {
                Text = "Generate",
                Location = new Point(656, 30),
                Size = new Size(84, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) => await LoadAndRenderReportAsync();

            // Export to Excel Button (Executive Outline Button)
            btnExportExcel = new Button
            {
                Text = "Excel",
                Location = new Point(748, 30),
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

            // Export to CSV Button (Executive Outline Button)
            btnExportCsv = new Button
            {
                Text = "CSV",
                Location = new Point(822, 30),
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

            // Print Button (WebView2 Native Print UI)
            btnPrint = new Button
            {
                Text = "Print",
                Location = new Point(896, 30),
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

            // Progress & Status Indicator
            lblStatus = new Label
            {
                Text = "Ready",
                Location = new Point(656, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            prgLoading = new ProgressBar
            {
                Location = new Point(748, 13),
                Size = new Size(110, 12),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            // Add top controls to header panel
            pnlTopBar.Controls.Add(lblAccount);
            pnlTopBar.Controls.Add(cmbAccount);
            pnlTopBar.Controls.Add(lblFrom);
            pnlTopBar.Controls.Add(dtpFrom);
            pnlTopBar.Controls.Add(lblTo);
            pnlTopBar.Controls.Add(dtpTo);
            pnlTopBar.Controls.Add(lblDateBasis);
            pnlTopBar.Controls.Add(cmbDateBasis);
            pnlTopBar.Controls.Add(btnRefresh);
            pnlTopBar.Controls.Add(btnExportExcel);
            pnlTopBar.Controls.Add(btnExportCsv);
            pnlTopBar.Controls.Add(btnPrint);
            pnlTopBar.Controls.Add(lblStatus);
            pnlTopBar.Controls.Add(prgLoading);

            // 2. Main Document Viewer Control (WebView2)
            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            // Compose root layout
            this.Controls.Add(webView);
            this.Controls.Add(pnlTopBar);

            // Form Load Event: Asynchronously initialize WebView2 and populate account heads
            this.Load += async (s, e) =>
            {
                await InitializeWebView2Async();
                await LoadAccountsAsync();
                SetLoadingState(false, "Ready. Select account & date range, then click 'Filter' to view statement.");
            };
        }

        /// <summary>
        /// Asynchronously initializes Microsoft Edge WebView2 runtime.
        /// </summary>
        private async Task InitializeWebView2Async()
        {
            try
            {
                SetLoadingState(true, "Initializing document viewer runtime...");
                string userDataFolder = Path.Combine(Path.GetTempPath(), "RetailSuite_WebView2");
                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);

                // Configure viewer preferences (clean, distraction-free document presentation)
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

        /// <summary>
        /// Asynchronously fetches live Chart of Account heads via ChartOfAccountApiService.
        /// </summary>
        private async Task LoadAccountsAsync()
        {
            try
            {
                var accounts = await _chartOfAccountService.GetDetailAccountsAsync();
                var dt = new DataTable();
                dt.Columns.Add("Code", typeof(string));
                dt.Columns.Add("Title", typeof(string));

                if (accounts != null && accounts.Count > 0)
                {
                    foreach (var acc in accounts)
                    {
                        dt.Rows.Add(acc.Account, acc.Title);
                    }
                }

                cmbAccount.DisplayMember = "Title";
                cmbAccount.ValueMember = "Code";
                cmbAccount.DataSource = dt;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading accounts: " + ex.Message;
            }
        }

        /// <summary>
        /// Loads report data from database, compiles the QuestPDF document,
        /// and navigates WebView2 to the resulting PDF file.
        /// </summary>
        public async Task LoadAndRenderReportAsync()
        {
            if (!_isWebViewReady) return;

            SetLoadingState(true, "Fetching statement data & generating PDF...");

            try
            {
                string accountCode = cmbAccount.SelectedValue != null ? cmbAccount.SelectedValue.ToString() : string.Empty;
                string accountTitle = cmbAccount.Text ?? "General Ledger Account";
                DateTime from = dtpFrom.Value.Date;
                DateTime to = dtpTo.Value.Date.AddDays(1).AddSeconds(-1);
                string selectedDateBasis = cmbDateBasis?.SelectedItem?.ToString() ?? "Voucher Date";
                string dateBasisQueryParam = selectedDateBasis == "Clearing Date" ? "ClearingDate" : "VoucherDate";

                int entryCount = 0;

                if (_mode == ReportViewerMode.AccountStatementWithDue)
                {
                    // Fetch Account Statement With Due asynchronously directly from DB
                    var reportData = await Task.Run(() =>
                    {
                        DataTable dt = ReportQuery.AccountStatementWithDue(accountCode, from, to, dateBasisQueryParam);
                        var items = (dt != null && dt.Rows.Count > 0)
                            ? AccountStatementWithDueDataService.FromDataTable(dt, 0m)
                            : new List<AccountStatementWithDueReportItem>();

                        var header = new AccountStatementWithDueHeader
                        {
                            CompanyName = CompanyInfo.CompanyName,
                            AccountTitle = accountTitle,
                            AccountCode = accountCode,
                            FromDate = from,
                            ToDate = to,
                            DateBasis = selectedDateBasis,
                            OpeningBalance = 0m,
                            TotalDebit = items.Sum(x => x.Debit),
                            TotalCredit = items.Sum(x => x.Credit),
                            ClosingBalance = items.Count > 0 ? items.Last().Balance : 0m
                        };
                        return new AccountStatementWithDueDataResult(header, items);
                    });

                    _currentDueHeader = reportData.Header;
                    _currentDueItems = reportData.Items;
                    entryCount = _currentDueItems.Count;

                    // Render PDF document to temporary path asynchronously
                    _currentPdfPath = await AccountStatementWithDueDocument.GeneratePdfToTempFileAsync(_currentDueHeader, _currentDueItems);
                }
                else
                {
                    // Standard Account Statement directly from DB
                    var reportData = await Task.Run(() =>
                    {
                        DataTable dt = ReportQuery.AccountStatement(accountCode, from, to, dateBasisQueryParam);
                        var items = (dt != null && dt.Rows.Count > 0)
                            ? AccountStatementDataService.FromDataTable(dt, 0m)
                            : new List<AccountStatementReportItem>();

                        var header = new AccountStatementHeader
                        {
                            CompanyName = CompanyInfo.CompanyName,
                            AccountTitle = accountTitle,
                            AccountCode = accountCode,
                            FromDate = from,
                            ToDate = to,
                            DateBasis = selectedDateBasis,
                            OpeningBalance = 0m,
                            TotalDebit = items.Sum(x => x.Debit),
                            TotalCredit = items.Sum(x => x.Credit),
                            ClosingBalance = items.Count > 0 ? items.Last().Balance : 0m
                        };
                        return new AccountStatementDataResult(header, items);
                    });

                    _currentHeader = reportData.Header;
                    _currentItems = reportData.Items;
                    entryCount = _currentItems.Count;

                    // Render PDF document to temporary path asynchronously
                    _currentPdfPath = await AccountStatementDocument.GeneratePdfToTempFileAsync(_currentHeader, _currentItems);
                }

                // Load generated PDF into WebView2 control
                if (File.Exists(_currentPdfPath) && webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.Navigate(new Uri(_currentPdfPath).AbsoluteUri);
                }

                SetLoadingState(false, string.Format("Report ready ({0} entries)", entryCount));
            }
            catch (Exception ex)
            {
                SetLoadingState(false, "Error loading report");
                MessageBox.Show(
                    "Failed to generate statement: " + ex.Message,
                    "Report Generation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Exports the active report to an Excel (.xlsx) file using ClosedXML.
        /// Executive, corporate presentation layout.
        /// </summary>
        private async Task ExportToExcelAsync()
        {
            bool hasData = _mode == ReportViewerMode.AccountStatementWithDue
                ? (_currentDueItems != null && _currentDueItems.Count > 0)
                : (_currentItems != null && _currentItems.Count > 0);

            if (!hasData)
            {
                MessageBox.Show("There is no statement data available to export.", "Export Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                string baseCode = _mode == ReportViewerMode.AccountStatementWithDue
                    ? (string.IsNullOrWhiteSpace(_currentDueHeader?.AccountCode) ? "Report" : _currentDueHeader.AccountCode.Replace("-", ""))
                    : (string.IsNullOrWhiteSpace(_currentHeader?.AccountCode) ? "Report" : _currentHeader.AccountCode.Replace("-", ""));

                string reportPrefix = _mode == ReportViewerMode.AccountStatementWithDue ? "AccountStatementWithDue" : "AccountStatement";
                sfd.FileName = string.Format("{0}_{1}_{2:yyyyMMdd}.xlsx", reportPrefix, baseCode, DateTime.Now);

                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                SetLoadingState(true, "Exporting to Excel...");

                try
                {
                    string filePath = sfd.FileName;

                    if (_mode == ReportViewerMode.AccountStatementWithDue)
                    {
                        var header = _currentDueHeader;
                        var items = _currentDueItems;

                        await Task.Run(() =>
                        {
                            using (var workbook = new XLWorkbook())
                            {
                                var ws = workbook.Worksheets.Add("Account Statement With Due");

                                // Report Header Banner
                                ws.Cell("A1").Value = header?.CompanyName ?? "RetailSuite Enterprise";
                                ws.Cell("A1").Style.Font.Bold = true;
                                ws.Cell("A1").Style.Font.FontSize = 15;
                                ws.Cell("A1").Style.Font.FontColor = XLColor.FromHtml("#0F172A");

                                ws.Cell("A2").Value = "STATEMENT OF ACCOUNT (WITH DUE DAYS) / CREDIT CONTROL";
                                ws.Cell("A2").Style.Font.Bold = true;
                                ws.Cell("A2").Style.Font.FontSize = 10;
                                ws.Cell("A2").Style.Font.FontColor = XLColor.FromHtml("#475569");

                                ws.Cell("A4").Value = "Account:";
                                ws.Cell("A4").Style.Font.Bold = true;
                                ws.Cell("B4").Value = header?.AccountTitle ?? "N/A";

                                ws.Cell("D4").Value = "Period:";
                                ws.Cell("D4").Style.Font.Bold = true;
                                ws.Cell("E4").Value = string.Format("{0:yyyy-MM-dd} to {1:yyyy-MM-dd}", header?.FromDate, header?.ToDate);

                                ws.Cell("D5").Value = "Basis:";
                                ws.Cell("D5").Style.Font.Bold = true;
                                ws.Cell("E5").Value = header?.DateBasis ?? "Voucher Date";

                                // Table Column Headers
                                int startRow = 7;
                                string[] headers = { "Date", "Voucher #", "Particulars / Narration", "Due Days", "Due Date", "Debit", "Credit", "Balance" };
                                for (int c = 0; c < headers.Length; c++)
                                {
                                    var cell = ws.Cell(startRow, c + 1);
                                    cell.Value = headers[c];
                                    cell.Style.Font.Bold = true;
                                    cell.Style.Font.FontColor = XLColor.FromHtml("#334155");
                                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
                                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    if (c == 3 || c == 4) cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    if (c >= 5) cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                }

                                // Table Data Rows
                                int rowIdx = startRow + 1;
                                foreach (var itm in items)
                                {
                                    ws.Cell(rowIdx, 1).Value = itm.Date;
                                    ws.Cell(rowIdx, 1).Style.DateFormat.Format = "yyyy-MM-dd";

                                    ws.Cell(rowIdx, 2).Value = itm.VoucherNo ?? "";

                                    ws.Cell(rowIdx, 3).Value = itm.Particular ?? "";

                                    if (itm.DueDays.HasValue && itm.DueDays.Value > 0)
                                    {
                                        ws.Cell(rowIdx, 4).Value = itm.DueDays.Value;
                                        ws.Cell(rowIdx, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    }
                                    else
                                    {
                                        ws.Cell(rowIdx, 4).Value = "-";
                                        ws.Cell(rowIdx, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    }

                                    if (itm.DueDate.HasValue)
                                    {
                                        ws.Cell(rowIdx, 5).Value = itm.DueDate.Value;
                                        ws.Cell(rowIdx, 5).Style.DateFormat.Format = "yyyy-MM-dd";
                                        ws.Cell(rowIdx, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    }
                                    else
                                    {
                                        ws.Cell(rowIdx, 5).Value = "-";
                                        ws.Cell(rowIdx, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    }

                                    ws.Cell(rowIdx, 6).Value = itm.Debit;
                                    ws.Cell(rowIdx, 6).Style.NumberFormat.Format = "#,##0.00";

                                    ws.Cell(rowIdx, 7).Value = itm.Credit;
                                    ws.Cell(rowIdx, 7).Style.NumberFormat.Format = "#,##0.00";

                                    ws.Cell(rowIdx, 8).Value = itm.Balance;
                                    ws.Cell(rowIdx, 8).Style.NumberFormat.Format = "#,##0.00";

                                    if (rowIdx % 2 == 1)
                                    {
                                        ws.Range(rowIdx, 1, rowIdx, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                                    }

                                    rowIdx++;
                                }

                                // Summary Total Row (Accounting Standard)
                                ws.Cell(rowIdx, 1).Value = "TOTALS & NET MOVEMENT";
                                ws.Cell(rowIdx, 1).Style.Font.Bold = true;
                                ws.Range(rowIdx, 1, rowIdx, 5).Merge();

                                ws.Cell(rowIdx, 6).Value = items.Sum(x => x.Debit);
                                ws.Cell(rowIdx, 6).Style.Font.Bold = true;
                                ws.Cell(rowIdx, 6).Style.NumberFormat.Format = "#,##0.00";

                                ws.Cell(rowIdx, 7).Value = items.Sum(x => x.Credit);
                                ws.Cell(rowIdx, 7).Style.Font.Bold = true;
                                ws.Cell(rowIdx, 7).Style.NumberFormat.Format = "#,##0.00";

                                ws.Cell(rowIdx, 8).Value = items.Count > 0 ? items.Last().Balance : (header?.ClosingBalance ?? 0m);
                                ws.Cell(rowIdx, 8).Style.Font.Bold = true;
                                ws.Cell(rowIdx, 8).Style.NumberFormat.Format = "#,##0.00";

                                var totalRange = ws.Range(rowIdx, 1, rowIdx, 8);
                                totalRange.Style.Border.TopBorder = XLBorderStyleValues.Medium;
                                totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Double;
                                totalRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");

                                // Auto-fit columns
                                ws.Columns().AdjustToContents();
                                ws.Column(3).Width = Math.Max(ws.Column(3).Width, 35);

                                workbook.SaveAs(filePath);
                            }
                        });
                    }
                    else
                    {
                        var header = _currentHeader;
                        var items = _currentItems;

                        await Task.Run(() =>
                        {
                            using (var workbook = new XLWorkbook())
                            {
                                var ws = workbook.Worksheets.Add("Account Statement");

                                // Report Header Banner
                                ws.Cell("A1").Value = header?.CompanyName ?? "RetailSuite Enterprise";
                                ws.Cell("A1").Style.Font.Bold = true;
                                ws.Cell("A1").Style.Font.FontSize = 15;
                                ws.Cell("A1").Style.Font.FontColor = XLColor.FromHtml("#0F172A");

                                ws.Cell("A2").Value = "STATEMENT OF ACCOUNT / GENERAL LEDGER";
                                ws.Cell("A2").Style.Font.Bold = true;
                                ws.Cell("A2").Style.Font.FontSize = 10;
                                ws.Cell("A2").Style.Font.FontColor = XLColor.FromHtml("#475569");

                                ws.Cell("A4").Value = "Account:";
                                ws.Cell("A4").Style.Font.Bold = true;
                                ws.Cell("B4").Value = header?.AccountTitle ?? "N/A";

                                ws.Cell("D4").Value = "Period:";
                                ws.Cell("D4").Style.Font.Bold = true;
                                ws.Cell("E4").Value = string.Format("{0:yyyy-MM-dd} to {1:yyyy-MM-dd}", header?.FromDate, header?.ToDate);

                                ws.Cell("D5").Value = "Basis:";
                                ws.Cell("D5").Style.Font.Bold = true;
                                ws.Cell("E5").Value = header?.DateBasis ?? "Voucher Date";

                                // Table Column Headers
                                int startRow = 7;
                                string[] headers = { "Date", "Voucher #", "Particulars / Narration", "Debit", "Credit", "Balance" };
                                for (int c = 0; c < headers.Length; c++)
                                {
                                    var cell = ws.Cell(startRow, c + 1);
                                    cell.Value = headers[c];
                                    cell.Style.Font.Bold = true;
                                    cell.Style.Font.FontColor = XLColor.FromHtml("#334155");
                                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
                                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    if (c >= 3) cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                }

                                // Table Data Rows
                                int rowIdx = startRow + 1;
                                foreach (var itm in items)
                                {
                                    ws.Cell(rowIdx, 1).Value = itm.Date;
                                    ws.Cell(rowIdx, 1).Style.DateFormat.Format = "yyyy-MM-dd";

                                    ws.Cell(rowIdx, 2).Value = itm.VoucherNo ?? "";

                                    ws.Cell(rowIdx, 3).Value = itm.Particular ?? "";

                                    ws.Cell(rowIdx, 4).Value = itm.Debit;
                                    ws.Cell(rowIdx, 4).Style.NumberFormat.Format = "#,##0.00";

                                    ws.Cell(rowIdx, 5).Value = itm.Credit;
                                    ws.Cell(rowIdx, 5).Style.NumberFormat.Format = "#,##0.00";

                                    ws.Cell(rowIdx, 6).Value = itm.Balance;
                                    ws.Cell(rowIdx, 6).Style.NumberFormat.Format = "#,##0.00";

                                    if (rowIdx % 2 == 1)
                                    {
                                        ws.Range(rowIdx, 1, rowIdx, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                                    }

                                    rowIdx++;
                                }

                                // Summary Total Row (Accounting Standard)
                                ws.Cell(rowIdx, 1).Value = "TOTALS & NET MOVEMENT";
                                ws.Cell(rowIdx, 1).Style.Font.Bold = true;
                                ws.Range(rowIdx, 1, rowIdx, 3).Merge();

                                ws.Cell(rowIdx, 4).Value = items.Sum(x => x.Debit);
                                ws.Cell(rowIdx, 4).Style.Font.Bold = true;
                                ws.Cell(rowIdx, 4).Style.NumberFormat.Format = "#,##0.00";

                                ws.Cell(rowIdx, 5).Value = items.Sum(x => x.Credit);
                                ws.Cell(rowIdx, 5).Style.Font.Bold = true;
                                ws.Cell(rowIdx, 5).Style.NumberFormat.Format = "#,##0.00";

                                ws.Cell(rowIdx, 6).Value = items.Count > 0 ? items.Last().Balance : (header?.ClosingBalance ?? 0m);
                                ws.Cell(rowIdx, 6).Style.Font.Bold = true;
                                ws.Cell(rowIdx, 6).Style.NumberFormat.Format = "#,##0.00";

                                var totalRange = ws.Range(rowIdx, 1, rowIdx, 6);
                                totalRange.Style.Border.TopBorder = XLBorderStyleValues.Medium;
                                totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Double;
                                totalRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");

                                // Auto-fit columns
                                ws.Columns().AdjustToContents();
                                ws.Column(3).Width = Math.Max(ws.Column(3).Width, 35); // Keep particulars column roomy

                                workbook.SaveAs(filePath);
                            }
                        });
                    }

                    SetLoadingState(false, "Excel export complete");
                    MessageBox.Show(
                        "Statement exported to Excel successfully!\n\nFile: " + filePath,
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

        /// <summary>
        /// Exports the active report items to CSV using CsvHelper.
        /// </summary>
        private async Task ExportToCsvAsync()
        {
            bool hasData = _mode == ReportViewerMode.AccountStatementWithDue
                ? (_currentDueItems != null && _currentDueItems.Count > 0)
                : (_currentItems != null && _currentItems.Count > 0);

            if (!hasData)
            {
                MessageBox.Show("There is no statement data available to export.", "Export Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Comma-Separated Values (*.csv)|*.csv";
                string baseCode = _mode == ReportViewerMode.AccountStatementWithDue
                    ? (string.IsNullOrWhiteSpace(_currentDueHeader?.AccountCode) ? "Report" : _currentDueHeader.AccountCode.Replace("-", ""))
                    : (string.IsNullOrWhiteSpace(_currentHeader?.AccountCode) ? "Report" : _currentHeader.AccountCode.Replace("-", ""));

                string reportPrefix = _mode == ReportViewerMode.AccountStatementWithDue ? "AccountStatementWithDue" : "AccountStatement";
                sfd.FileName = string.Format("{0}_{1}_{2:yyyyMMdd}.csv", reportPrefix, baseCode, DateTime.Now);

                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                SetLoadingState(true, "Exporting to CSV...");

                try
                {
                    string filePath = sfd.FileName;

                    if (_mode == ReportViewerMode.AccountStatementWithDue)
                    {
                        var items = _currentDueItems;
                        await Task.Run(() =>
                        {
                            using (var writer = new StreamWriter(filePath))
                            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                            {
                                csv.WriteRecords(items);
                            }
                        });
                    }
                    else
                    {
                        var items = _currentItems;
                        await Task.Run(() =>
                        {
                            using (var writer = new StreamWriter(filePath))
                            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                            {
                                csv.WriteRecords(items);
                            }
                        });
                    }

                    SetLoadingState(false, "CSV export complete");
                    MessageBox.Show(
                        "Statement exported to CSV successfully!\n\nFile: " + filePath,
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

            bool hasData = _mode == ReportViewerMode.AccountStatementWithDue
                ? (_currentDueItems != null && _currentDueItems.Count > 0)
                : (_currentItems != null && _currentItems.Count > 0);

            btnExportExcel.Enabled = !isLoading && hasData;
            btnExportCsv.Enabled = !isLoading && hasData;
            btnPrint.Enabled = !isLoading && _isWebViewReady && hasData;
            cmbAccount.Enabled = !isLoading;
            dtpFrom.Enabled = !isLoading;
            dtpTo.Enabled = !isLoading;
            cmbDateBasis.Enabled = !isLoading;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            try
            {
                // Clean up temp PDF file when form closes if desired
                if (!string.IsNullOrEmpty(_currentPdfPath) && File.Exists(_currentPdfPath))
                {
                    // Delay delete or let OS clean temp
                }
            }
            catch { }
        }
    }
}
