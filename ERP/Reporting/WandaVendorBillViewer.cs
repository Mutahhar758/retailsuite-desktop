using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
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
    /// Dedicated Vendor Bill / Statement Viewer for Wanda Feed & Commodities businesses.
    /// Full dual-unit support (Bags, Weight, Bag Rate, Carriage, Payments Made).
    /// </summary>
    public class WandaVendorBillViewer : Form
    {
        // Services
        private readonly ChartOfAccountApiService _chartOfAccountApiService;

        // UI Controls - Mode Switch
        private Panel pnlHeader;
        private Label lblTitle;
        private Button btnModeSingle;
        private Button btnModeBulk;

        // UI Controls - Single Mode Panel
        private Panel pnlSingleControls;
        private Label lblVendor;
        private ComboBox cmbVendor;
        private Label lblFromDate;
        private DateTimePicker dtpFromDate;
        private Label lblToDate;
        private DateTimePicker dtpToDate;
        private Label lblDateBasis;
        private ComboBox cmbDateBasis;
        private Label lblLayout;
        private ComboBox cmbLayout;
        private Button btnGenerateSingle;
        private Button btnExportExcel;
        private Button btnExportCsv;
        private Button btnPrintPreview;
        private Button btnPrintThermalSingle;
        private Button btnPrintDirectSingle;

        // UI Controls - Bulk Mode Left Panel
        private Panel pnlBulkSidebar;
        private Label lblBulkSearch;
        private TextBox txtVendorSearch;
        private CheckBox chkSelectAll;
        private CheckedListBox chklstVendors;
        private Label lblSelectedCount;
        private Label lblPrinter;
        private ComboBox cmbPrinter;
        private Label lblBulkDates;
        private DateTimePicker dtpBulkFromDate;
        private DateTimePicker dtpBulkToDate;
        private Label lblBulkDateBasis;
        private ComboBox cmbBulkDateBasis;
        private Label lblBulkFormat;
        private ComboBox cmbBulkFormat;
        private Button btnBulkPrintDirect;
        private Button btnBulkPreviewBatch;
        private Button btnCancelBulk;
        private ProgressBar prgBulkProgress;
        private Label lblBulkStatus;

        // UI Controls - Viewer Canvas
        private Panel pnlCanvas;
        private ProgressBar prgLoading;
        private Label lblStatus;
        private WebView2 webView;

        // State
        private List<ChartOfAccountHeadDto> _allVendors = new List<ChartOfAccountHeadDto>();
        private List<ChartOfAccountHeadDto> _filteredVendors = new List<ChartOfAccountHeadDto>();
        private VendorBillDataResult _currentResult;
        private string _currentPdfPath;
        private bool _isWebViewReady = false;
        private bool _isBulkMode = false;
        private CancellationTokenSource _bulkCts;
        private string _initialVendorCode;

        public WandaVendorBillViewer()
        {
            _chartOfAccountApiService = new ChartOfAccountApiService();

            InitializeComponentCodeFirst();
            this.Load += async (s, e) => await InitializeDataAsync();
        }

        public WandaVendorBillViewer(string initialVendorCode, DateTime fromDate, DateTime toDate)
            : this()
        {
            _initialVendorCode = initialVendorCode;
            dtpFromDate.Value = fromDate;
            dtpToDate.Value = toDate;
            dtpBulkFromDate.Value = fromDate;
            dtpBulkToDate.Value = toDate;
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Vendor Bill & Statement (Wanda)";
            this.Size = new Size(1220, 850);
            this.MinimumSize = new Size(1020, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.BackColor = Color.FromArgb(248, 250, 252);

            // ==========================================
            // 1. TOP HEADER & MODE SWITCHER
            // ==========================================
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 54,
                BackColor = Color.FromArgb(15, 23, 42),
                Padding = new Padding(16, 8, 16, 8)
            };

            lblTitle = new Label
            {
                Text = "VENDOR BILL & STATEMENT",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                Location = new Point(16, 15),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblTitle);

            btnModeSingle = new Button
            {
                Text = "Single Vendor Preview",
                Location = new Point(330, 12),
                Width = 165,
                Height = 30,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnModeSingle.FlatAppearance.BorderSize = 0;
            btnModeSingle.Click += (s, e) => SwitchMode(false);
            pnlHeader.Controls.Add(btnModeSingle);

            btnModeBulk = new Button
            {
                Text = "Bulk Print Direct",
                Location = new Point(500, 12),
                Width = 155,
                Height = 30,
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnModeBulk.FlatAppearance.BorderSize = 0;
            btnModeBulk.Click += (s, e) => SwitchMode(true);
            pnlHeader.Controls.Add(btnModeBulk);

            this.Controls.Add(pnlHeader);

            // ==========================================
            // 2. SINGLE MODE CONTROL BAR
            // ==========================================
            pnlSingleControls = new Panel
            {
                Dock = DockStyle.Top,
                Height = 68,
                BackColor = Color.White,
                Padding = new Padding(16, 8, 16, 8)
            };
            pnlSingleControls.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, pnlSingleControls.Height - 1, pnlSingleControls.Width, pnlSingleControls.Height - 1);
            };

            lblVendor = new Label
            {
                Text = "SELECT CUSTOMER",
                AutoSize = true,
                Location = new Point(16, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            cmbVendor = new ComboBox
            {
                Location = new Point(16, 28),
                Width = 230,
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems,
                Font = new Font("Segoe UI", 9F)
            };

            lblFromDate = new Label
            {
                Text = "FROM DATE",
                AutoSize = true,
                Location = new Point(255, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            dtpFromDate = new DateTimePicker
            {
                Location = new Point(255, 28),
                Width = 115,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
            };

            lblToDate = new Label
            {
                Text = "TO DATE",
                AutoSize = true,
                Location = new Point(378, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            dtpToDate = new DateTimePicker
            {
                Location = new Point(378, 28),
                Width = 115,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = DateTime.Today
            };

            lblDateBasis = new Label
            {
                Text = "DATE BASIS",
                AutoSize = true,
                Location = new Point(490, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            cmbDateBasis = new ComboBox
            {
                Location = new Point(490, 28),
                Width = 115,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbDateBasis.Items.AddRange(new object[] { "Clearing Date", "Voucher Date" });
            cmbDateBasis.SelectedIndex = 0;

            lblLayout = new Label
            {
                Text = "FORMAT",
                AutoSize = true,
                Location = new Point(610, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            cmbLayout = new ComboBox
            {
                Location = new Point(610, 28),
                Width = 106,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbLayout.Items.AddRange(new object[] { "A4 Sheet", "80mm Thermal" });
            cmbLayout.SelectedIndex = ConfigInfo.IsThermalDefault ? 1 : 0;
            cmbLayout.SelectedIndexChanged += async (s, e) =>
            {
                bool isThermal = cmbLayout.SelectedIndex == 1;
                if (webView != null && _isWebViewReady && webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.Settings.HiddenPdfToolbarItems = isThermal
                        ? Microsoft.Web.WebView2.Core.CoreWebView2PdfToolbarItems.Print
                        : Microsoft.Web.WebView2.Core.CoreWebView2PdfToolbarItems.None;
                }
                string preferredPrinter = isThermal ? ConfigInfo.ThermalPrinterName : ConfigInfo.A4PrinterName;
                if (!string.IsNullOrWhiteSpace(preferredPrinter) && cmbPrinter != null && cmbPrinter.Items.Contains(preferredPrinter))
                {
                    cmbPrinter.SelectedItem = preferredPrinter;
                }
                if (_currentResult != null)
                {
                    await LoadAndRenderSingleBillAsync();
                }
            };

            btnGenerateSingle = new Button
            {
                Text = "Preview",
                Location = new Point(724, 26),
                Width = 68,
                Height = 28,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGenerateSingle.FlatAppearance.BorderSize = 0;
            btnGenerateSingle.Click += async (s, e) => await LoadAndRenderSingleBillAsync();

            btnExportExcel = new Button
            {
                Text = "Excel",
                Location = new Point(796, 26),
                Width = 52,
                Height = 28,
                BackColor = Color.FromArgb(16, 149, 193),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportExcel.FlatAppearance.BorderSize = 0;
            btnExportExcel.Click += (s, e) => ExportSingleToExcel();

            btnExportCsv = new Button
            {
                Text = "CSV",
                Location = new Point(852, 26),
                Width = 46,
                Height = 28,
                BackColor = Color.FromArgb(71, 85, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportCsv.FlatAppearance.BorderSize = 0;
            btnExportCsv.Click += (s, e) => ExportSingleToCsv();

            btnPrintPreview = new Button
            {
                Text = "Print",
                Location = new Point(902, 26),
                Width = 52,
                Height = 28,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrintPreview.FlatAppearance.BorderSize = 0;
            btnPrintPreview.Click += (s, e) => TriggerPrintPreview();

            btnPrintThermalSingle = new Button
            {
                Text = "🖨 Thermal",
                Location = new Point(958, 26),
                Width = 96,
                Height = 28,
                BackColor = Color.FromArgb(217, 119, 6),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrintThermalSingle.FlatAppearance.BorderSize = 0;
            btnPrintThermalSingle.Click += async (s, e) => await TriggerDirectThermalPrintAsync();

            btnPrintDirectSingle = new Button
            {
                Text = "🖨 Direct A4",
                Location = new Point(1058, 26),
                Width = 88,
                Height = 28,
                BackColor = Color.FromArgb(5, 150, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrintDirectSingle.FlatAppearance.BorderSize = 0;
            btnPrintDirectSingle.Click += (s, e) => TriggerDirectSinglePrint();

            pnlSingleControls.Controls.AddRange(new Control[] {
                lblVendor, cmbVendor,
                lblFromDate, dtpFromDate,
                lblToDate, dtpToDate,
                lblDateBasis, cmbDateBasis,
                lblLayout, cmbLayout,
                btnGenerateSingle, btnExportExcel, btnExportCsv, btnPrintPreview, btnPrintThermalSingle, btnPrintDirectSingle
            });

            this.Controls.Add(pnlSingleControls);

            // ==========================================
            // 3. BULK MODE SIDEBAR (DOCK LEFT)
            // ==========================================
            pnlBulkSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 400,
                BackColor = Color.White,
                Padding = new Padding(12),
                Visible = false
            };
            pnlBulkSidebar.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), pnlBulkSidebar.Width - 1, 0, pnlBulkSidebar.Width - 1, pnlBulkSidebar.Height);
            };

            int curY = 12;

            // Search Box
            lblBulkSearch = new Label
            {
                Text = "SEARCH / FILTER CUSTOMERS:",
                Location = new Point(12, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlBulkSidebar.Controls.Add(lblBulkSearch);
            curY += 18;

            txtVendorSearch = new TextBox
            {
                Location = new Point(12, curY),
                Width = 370,
                Font = new Font("Segoe UI", 9F)
            };
            txtVendorSearch.TextChanged += (s, e) => FilterVendorChecklist();
            pnlBulkSidebar.Controls.Add(txtVendorSearch);
            curY += 32;

            // Select All & Count
            chkSelectAll = new CheckBox
            {
                Text = "Select All Vendors",
                Location = new Point(12, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59)
            };
            chkSelectAll.CheckedChanged += (s, e) =>
            {
                for (int i = 0; i < chklstVendors.Items.Count; i++)
                {
                    chklstVendors.SetItemChecked(i, chkSelectAll.Checked);
                }
                UpdateSelectedCount();
            };
            pnlBulkSidebar.Controls.Add(chkSelectAll);

            lblSelectedCount = new Label
            {
                Text = "Selected: 0",
                Location = new Point(250, curY + 2),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235)
            };
            pnlBulkSidebar.Controls.Add(lblSelectedCount);
            curY += 26;

            // Vendor Checklist
            chklstVendors = new CheckedListBox
            {
                Location = new Point(12, curY),
                Width = 370,
                Height = 260,
                CheckOnClick = true,
                Font = new Font("Segoe UI", 8.5F)
            };
            chklstVendors.ItemCheck += (s, e) =>
            {
                this.BeginInvoke(new Action(UpdateSelectedCount));
            };
            pnlBulkSidebar.Controls.Add(chklstVendors);
            curY += 268;

            // Printer Selector
            lblPrinter = new Label
            {
                Text = "DESTINATION PRINTER (DIRECT SILENT):",
                Location = new Point(12, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlBulkSidebar.Controls.Add(lblPrinter);
            curY += 18;

            cmbPrinter = new ComboBox
            {
                Location = new Point(12, curY),
                Width = 370,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            pnlBulkSidebar.Controls.Add(cmbPrinter);
            curY += 32;

            // Bulk Date Range
            lblBulkDates = new Label
            {
                Text = "DATE RANGE:",
                Location = new Point(12, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlBulkSidebar.Controls.Add(lblBulkDates);
            curY += 18;

            dtpBulkFromDate = new DateTimePicker
            {
                Location = new Point(12, curY),
                Width = 180,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
            };
            pnlBulkSidebar.Controls.Add(dtpBulkFromDate);

            dtpBulkToDate = new DateTimePicker
            {
                Location = new Point(202, curY),
                Width = 180,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9F),
                Value = DateTime.Today
            };
            pnlBulkSidebar.Controls.Add(dtpBulkToDate);
            curY += 36;

            // Bulk Date Basis
            lblBulkDateBasis = new Label
            {
                Text = "DATE BASIS:",
                Location = new Point(12, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlBulkSidebar.Controls.Add(lblBulkDateBasis);
            curY += 18;

            cmbBulkDateBasis = new ComboBox
            {
                Location = new Point(12, curY),
                Width = 370,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbBulkDateBasis.Items.AddRange(new object[] { "Clearing Date (Recommended)", "Voucher Date" });
            cmbBulkDateBasis.SelectedIndex = 0;
            pnlBulkSidebar.Controls.Add(cmbBulkDateBasis);
            curY += 34;

            lblBulkFormat = new Label
            {
                Text = "PRINT FORMAT:",
                Location = new Point(12, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlBulkSidebar.Controls.Add(lblBulkFormat);
            curY += 18;

            cmbBulkFormat = new ComboBox
            {
                Location = new Point(12, curY),
                Width = 370,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbBulkFormat.Items.AddRange(new object[] { "A4 Commercial Invoice (Full Page)", "80mm Thermal Receipt (POS Roll)" });
            cmbBulkFormat.SelectedIndex = ConfigInfo.IsThermalDefault ? 1 : 0;
            cmbBulkFormat.SelectedIndexChanged += (s, e) =>
            {
                bool isThermal = cmbBulkFormat.SelectedIndex == 1;
                string preferredPrinter = isThermal ? ConfigInfo.ThermalPrinterName : ConfigInfo.A4PrinterName;
                if (!string.IsNullOrWhiteSpace(preferredPrinter) && cmbPrinter != null && cmbPrinter.Items.Contains(preferredPrinter))
                {
                    cmbPrinter.SelectedItem = preferredPrinter;
                }
            };
            pnlBulkSidebar.Controls.Add(cmbBulkFormat);
            curY += 34;

            // Action Buttons
            btnBulkPrintDirect = new Button
            {
                Text = "🖨 Bulk Print (Direct to Printer)",
                Location = new Point(12, curY),
                Width = 230,
                Height = 36,
                BackColor = Color.FromArgb(5, 150, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBulkPrintDirect.FlatAppearance.BorderSize = 0;
            btnBulkPrintDirect.Click += async (s, e) => await ExecuteDirectBulkPrintAsync();
            pnlBulkSidebar.Controls.Add(btnBulkPrintDirect);

            btnBulkPreviewBatch = new Button
            {
                Text = "Preview Batch",
                Location = new Point(248, curY),
                Width = 134,
                Height = 36,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBulkPreviewBatch.FlatAppearance.BorderSize = 0;
            btnBulkPreviewBatch.Click += async (s, e) => await PreviewCombinedBatchAsync();
            pnlBulkSidebar.Controls.Add(btnBulkPreviewBatch);
            curY += 42;

            btnCancelBulk = new Button
            {
                Text = "Cancel Printing",
                Location = new Point(12, curY),
                Width = 370,
                Height = 28,
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Visible = false,
                Cursor = Cursors.Hand
            };
            btnCancelBulk.FlatAppearance.BorderSize = 0;
            btnCancelBulk.Click += (s, e) => _bulkCts?.Cancel();
            pnlBulkSidebar.Controls.Add(btnCancelBulk);
            curY += 32;

            // Progress Bar & Status
            prgBulkProgress = new ProgressBar
            {
                Location = new Point(12, curY),
                Width = 370,
                Height = 12,
                Visible = false
            };
            pnlBulkSidebar.Controls.Add(prgBulkProgress);
            curY += 16;

            lblBulkStatus = new Label
            {
                Text = "Ready",
                Location = new Point(12, curY),
                Width = 370,
                Height = 36,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
            pnlBulkSidebar.Controls.Add(lblBulkStatus);

            this.Controls.Add(pnlBulkSidebar);

            // ==========================================
            // 4. MAIN CANVAS & WEBVIEW2
            // ==========================================
            pnlCanvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(12)
            };

            lblStatus = new Label
            {
                Text = "Select a vendor or switch to Bulk Print mode to begin.",
                AutoSize = true,
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlCanvas.Controls.Add(lblStatus);

            prgLoading = new ProgressBar
            {
                Location = new Point(20, 50),
                Width = 260,
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
                // Populate Printers
                cmbPrinter.Items.Clear();
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    cmbPrinter.Items.Add(printer);
                }

                bool isThermal = cmbLayout != null && cmbLayout.SelectedIndex == 1;
                string preferredPrinter = isThermal ? ConfigInfo.ThermalPrinterName : ConfigInfo.A4PrinterName;

                if (!string.IsNullOrWhiteSpace(preferredPrinter) && cmbPrinter.Items.Contains(preferredPrinter))
                {
                    cmbPrinter.SelectedItem = preferredPrinter;
                }
                else if (!string.IsNullOrWhiteSpace(ConfigInfo.ThermalPrinterName) && cmbPrinter.Items.Contains(ConfigInfo.ThermalPrinterName))
                {
                    cmbPrinter.SelectedItem = ConfigInfo.ThermalPrinterName;
                }
                else if (cmbPrinter.Items.Count > 0)
                {
                    cmbPrinter.SelectedIndex = 0;
                }

                // Populate Vendors
                var vendors = await _chartOfAccountApiService.GetSupplierAccountsAsync();
                _allVendors = vendors ?? new List<ChartOfAccountHeadDto>();
                _filteredVendors = new List<ChartOfAccountHeadDto>(_allVendors);

                // Populate Single Mode ComboBox
                cmbVendor.DataSource = null;
                cmbVendor.DisplayMember = "Title";
                cmbVendor.ValueMember = "Account";
                cmbVendor.DataSource = new List<ChartOfAccountHeadDto>(_allVendors);
                if (cmbVendor.Items.Count > 0) cmbVendor.SelectedIndex = 0;

                // Populate Bulk Mode CheckList
                PopulateVendorChecklist();

                // Initialize WebView2
                await EnsureWebViewInitializedAsync();

                lblStatus.Text = "Ready. Select vendor & date range, then click 'Preview'.";
                lblStatus.ForeColor = Color.FromArgb(71, 85, 105);

                if (!string.IsNullOrWhiteSpace(_initialVendorCode))
                {
                    cmbVendor.SelectedValue = _initialVendorCode;
                    await LoadAndRenderSingleBillAsync();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error initializing vendor data: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void SwitchMode(bool isBulk)
        {
            _isBulkMode = isBulk;
            if (isBulk)
            {
                btnModeBulk.BackColor = Color.FromArgb(37, 99, 235);
                btnModeSingle.BackColor = Color.FromArgb(51, 65, 85);
                pnlSingleControls.Visible = false;
                pnlBulkSidebar.Visible = true;
                pnlBulkSidebar.BringToFront();
                pnlCanvas.BringToFront();
                UpdateSelectedCount();
            }
            else
            {
                btnModeSingle.BackColor = Color.FromArgb(37, 99, 235);
                btnModeBulk.BackColor = Color.FromArgb(51, 65, 85);
                pnlBulkSidebar.Visible = false;
                pnlSingleControls.Visible = true;
                pnlSingleControls.BringToFront();
                pnlCanvas.BringToFront();
            }
        }

        private void PopulateVendorChecklist()
        {
            chklstVendors.Items.Clear();
            foreach (var cust in _filteredVendors)
            {
                chklstVendors.Items.Add(cust);
            }
            chklstVendors.DisplayMember = "Title";
            UpdateSelectedCount();
        }

        private void FilterVendorChecklist()
        {
            string query = txtVendorSearch.Text.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(query))
            {
                _filteredVendors = new List<ChartOfAccountHeadDto>(_allVendors);
            }
            else
            {
                _filteredVendors = _allVendors
                    .Where(x => (x.Title ?? "").ToLowerInvariant().Contains(query) || (x.Account ?? "").ToLowerInvariant().Contains(query))
                    .ToList();
            }
            PopulateVendorChecklist();
        }

        private void UpdateSelectedCount()
        {
            int count = chklstVendors.CheckedItems.Count;
            lblSelectedCount.Text = string.Format("Selected: {0} of {1}", count, chklstVendors.Items.Count);
        }

        private async Task EnsureWebViewInitializedAsync()
        {
            if (!_isWebViewReady)
            {
                string userDataFolder = Path.Combine(Path.GetTempPath(), "RetailSuite", "WebView2_VendorBill");
                if (!Directory.Exists(userDataFolder)) Directory.CreateDirectory(userDataFolder);

                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);
                _isWebViewReady = true;
            }
        }

        // ==========================================
        // SINGLE BILL LOGIC (PURE SINGLE-UNIT)
        // ==========================================
        private async Task LoadAndRenderSingleBillAsync()
        {
            if (cmbVendor.SelectedValue == null)
            {
                MessageBox.Show("Please select a vendor.", "Vendor Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string vendorCode = cmbVendor.SelectedValue.ToString();
            string vendorTitle = cmbVendor.Text;
            DateTime fromDate = dtpFromDate.Value.Date;
            DateTime toDate = dtpToDate.Value.Date;
            string dateBasis = (cmbDateBasis.SelectedItem != null && cmbDateBasis.SelectedItem.ToString().StartsWith("Clearing", StringComparison.OrdinalIgnoreCase))
                ? "ClearingDate"
                : "VoucherDate";

            SetLoading(true, string.Format("Generating bill for {0}...", vendorTitle));

            try
            {
                await EnsureWebViewInitializedAsync();

                DataSet ds = await Task.Run(() => ReportQuery.WandaVendorBill(vendorCode, fromDate, toDate, dateBasis));
                VendorBillDataResult result = VendorBillDataService.ConvertWandaDataSet(ds, vendorCode, vendorTitle, fromDate, toDate, dateBasis);

                _currentResult = result;
                var layout = (cmbLayout != null && cmbLayout.SelectedIndex == 1) ? VendorBillPrintLayout.Thermal80mm : VendorBillPrintLayout.A4Sheet;
                bool isThermal = layout == VendorBillPrintLayout.Thermal80mm;
                if (webView != null && _isWebViewReady && webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.Settings.HiddenPdfToolbarItems = isThermal
                        ? Microsoft.Web.WebView2.Core.CoreWebView2PdfToolbarItems.Print
                        : Microsoft.Web.WebView2.Core.CoreWebView2PdfToolbarItems.None;
                }

                var doc = new VendorBillDocument(result.Summary, result.Lines, layout);
                _currentPdfPath = await doc.GeneratePdfToTempFileAsync();

                webView.CoreWebView2.Navigate(_currentPdfPath);
                webView.Visible = true;
                string layoutLabel = layout == VendorBillPrintLayout.Thermal80mm ? "80mm Thermal Receipt" : "A4 Invoice";
                lblStatus.Text = string.Format("Vendor Bill ({0}): {1} ({2:dd-MMM-yyyy} - {3:dd-MMM-yyyy}) • Net Due: {4:#,##0.00}",
                    layoutLabel, result.Summary.VendorName, result.Summary.FromDate, result.Summary.ToDate, result.Summary.NetBalance);
                lblStatus.ForeColor = Color.FromArgb(30, 41, 59);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error rendering bill: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void TriggerPrintPreview()
        {
            if (webView != null && _isWebViewReady)
            {
                bool isThermal = cmbLayout != null && cmbLayout.SelectedIndex == 1;
                if (isThermal)
                {
                    await TriggerDirectThermalPrintAsync();
                    return;
                }

                // For A4 Sheet format, open standard browser print dialog
                webView.CoreWebView2.ShowPrintUI(Microsoft.Web.WebView2.Core.CoreWebView2PrintDialogKind.Browser);
            }
        }

        private async Task TriggerDirectThermalPrintAsync()
        {
            if (_currentResult == null)
            {
                MessageBox.Show("Please preview or generate the vendor bill first.", "Thermal Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string printer = !string.IsNullOrWhiteSpace(ConfigInfo.ThermalPrinterName) 
                ? ConfigInfo.ThermalPrinterName 
                : (cmbPrinter.SelectedItem != null ? cmbPrinter.SelectedItem.ToString() : null);

            if (string.IsNullOrWhiteSpace(printer))
            {
                MessageBox.Show("Thermal printer name is not configured in settings. Please configure your thermal printer.", "Printer Not Configured", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Native vector printing via WebView2
                // Directly applies the verified settings (Scale 90%, 80x3276mm continuous roll, margins 0)
                // without showing any print dialog or requiring manual selection.
                if (webView != null && _isWebViewReady && webView.CoreWebView2 != null)
                {
                    var printSettings = webView.CoreWebView2.Environment.CreatePrintSettings();
                    printSettings.PrinterName = printer;
                    printSettings.ScaleFactor = 0.9;
                    printSettings.PageWidth = 3.1496; // 80mm
                    printSettings.PageHeight = 128.976; // 3276mm continuous roll
                    printSettings.MarginTop = 0;
                    printSettings.MarginBottom = 0;
                    printSettings.MarginLeft = 0;
                    printSettings.MarginRight = 0;
                    printSettings.ShouldPrintBackgrounds = true;
                    printSettings.ShouldPrintHeaderAndFooter = false;

                    var status = await webView.CoreWebView2.PrintAsync(printSettings);
                    if (status == Microsoft.Web.WebView2.Core.CoreWebView2PrintStatus.Succeeded)
                    {
                        MessageBox.Show(string.Format("80mm Thermal Receipt sent to '{0}' successfully!", printer), "Print Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                // Fallback to GDI+ spooler if WebView2 is not ready
                var doc = new VendorBillDocument(_currentResult.Summary, _currentResult.Lines, VendorBillPrintLayout.Thermal80mm);
                VendorBillDocument.PrintDirectToPrinter(doc, printer);
                MessageBox.Show(string.Format("80mm Thermal Receipt sent silently to '{0}' successfully!", printer), "Print Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thermal print error: " + ex.Message, "Print Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TriggerDirectSinglePrint()
        {
            if (_currentResult == null)
            {
                MessageBox.Show("Please generate the bill before printing.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool isThermal = cmbLayout != null && cmbLayout.SelectedIndex == 1;
            string defaultPrinter = isThermal ? ConfigInfo.ThermalPrinterName : ConfigInfo.A4PrinterName;
            string printer = cmbPrinter.SelectedItem != null ? cmbPrinter.SelectedItem.ToString() : defaultPrinter;

            if (string.IsNullOrWhiteSpace(printer))
            {
                MessageBox.Show((isThermal ? "Thermal" : "A4") + " printer name is not configured in settings. Please select a destination printer.", "Printer Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var layout = isThermal ? VendorBillPrintLayout.Thermal80mm : VendorBillPrintLayout.A4Sheet;
            try
            {
                var doc = new VendorBillDocument(_currentResult.Summary, _currentResult.Lines, layout);
                VendorBillDocument.PrintDirectToPrinter(doc, printer);
                string formatTitle = isThermal ? "80mm Thermal Receipt" : "A4 Vendor bill";
                MessageBox.Show(string.Format("{0} sent silently to '{1}' successfully!", formatTitle, printer), "Print Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print error: " + ex.Message, "Print Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // BULK DIRECT SILENT PRINT LOGIC
        // ==========================================
        private async Task ExecuteDirectBulkPrintAsync()
        {
            var checkedItems = chklstVendors.CheckedItems.Cast<ChartOfAccountHeadDto>().ToList();
            if (checkedItems.Count == 0)
            {
                MessageBox.Show("Please check at least one vendor from the list for bulk printing.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var layout = (cmbBulkFormat != null && cmbBulkFormat.SelectedIndex == 1) ? VendorBillPrintLayout.Thermal80mm : VendorBillPrintLayout.A4Sheet;
            string defaultPrinter = layout == VendorBillPrintLayout.Thermal80mm ? ConfigInfo.ThermalPrinterName : ConfigInfo.A4PrinterName;
            string printer = cmbPrinter.SelectedItem != null ? cmbPrinter.SelectedItem.ToString() : defaultPrinter;
            if (string.IsNullOrWhiteSpace(printer))
            {
                MessageBox.Show("Please select a destination printer.", "Printer Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime fromDate = dtpBulkFromDate.Value.Date;
            DateTime toDate = dtpBulkToDate.Value.Date;
            string dateBasis = (cmbBulkDateBasis != null && cmbBulkDateBasis.SelectedItem != null && cmbBulkDateBasis.SelectedItem.ToString().StartsWith("Clearing", StringComparison.OrdinalIgnoreCase))
                ? "ClearingDate"
                : "VoucherDate";
            string formatName = layout == VendorBillPrintLayout.Thermal80mm ? "80mm Thermal Receipt" : "A4 Commercial Invoice";

            var confirm = MessageBox.Show(
                string.Format("Are you sure you want to silently print {0} vendor bill(s) in {1} format directly to '{2}'?", checkedItems.Count, formatName, printer),
                "Confirm Silent Bulk Print",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            _bulkCts = new CancellationTokenSource();
            btnBulkPrintDirect.Enabled = false;
            btnBulkPreviewBatch.Enabled = false;
            btnCancelBulk.Visible = true;
            prgBulkProgress.Visible = true;
            prgBulkProgress.Minimum = 0;
            prgBulkProgress.Maximum = checkedItems.Count;
            prgBulkProgress.Value = 0;

            try
            {
                await EnsureWebViewInitializedAsync();

                var batch = new List<VendorBillDataResult>();

                for (int i = 0; i < checkedItems.Count; i++)
                {
                    if (_bulkCts.IsCancellationRequested) break;

                    var vendor = checkedItems[i];
                    lblBulkStatus.Text = string.Format("Compiling {0} of {1}: {2}...", i + 1, checkedItems.Count, vendor.Title);
                    prgBulkProgress.Value = i + 1;

                    await Task.Run(() =>
                    {
                        try
                        {
                            DataSet ds = ReportQuery.WandaVendorBill(vendor.Account, fromDate, toDate, dateBasis);
                            var res = VendorBillDataService.ConvertWandaDataSet(ds, vendor.Account, vendor.Title, fromDate, toDate, dateBasis);

                            if (res.Lines.Count > 0 || Math.Abs(res.Summary.NetBalance) > 0.01m)
                            {
                                batch.Add(res);
                            }
                        }
                        catch
                        {
                        }
                    });
                }

                if (_bulkCts.IsCancellationRequested)
                {
                    lblBulkStatus.Text = "Cancelled by user.";
                    MessageBox.Show("Bulk printing was cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (batch.Count == 0)
                {
                    lblBulkStatus.Text = "No active transactions found.";
                    MessageBox.Show("No active vendor bills to print.", "Nothing to Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int printedCount = 0;
                for (int i = 0; i < batch.Count; i++)
                {
                    if (_bulkCts.IsCancellationRequested) break;

                    var bill = batch[i];
                    lblBulkStatus.Text = string.Format("Printing {0} of {1}: {2}...", i + 1, batch.Count, bill.Summary.VendorName);
                    prgBulkProgress.Value = i + 1;

                    var singleDoc = new VendorBillDocument(bill.Summary, bill.Lines, layout);

                    if (webView != null && _isWebViewReady && webView.CoreWebView2 != null)
                    {
                        string singlePdfPath = await singleDoc.GeneratePdfToTempFileAsync();

                        var tcs = new TaskCompletionSource<bool>();
                        EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs> navHandler = null;
                        navHandler = (s, e) =>
                        {
                            webView.NavigationCompleted -= navHandler;
                            tcs.TrySetResult(e.IsSuccess);
                        };
                        webView.NavigationCompleted += navHandler;
                        webView.CoreWebView2.Navigate(singlePdfPath);
                        await tcs.Task;

                        // Brief pause to ensure the PDF viewer has fully rendered
                        await Task.Delay(250);

                        var printSettings = webView.CoreWebView2.Environment.CreatePrintSettings();
                        printSettings.PrinterName = printer;
                        printSettings.ShouldPrintBackgrounds = true;
                        printSettings.ShouldPrintHeaderAndFooter = false;
                        printSettings.MarginTop = 0;
                        printSettings.MarginBottom = 0;
                        printSettings.MarginLeft = 0;
                        printSettings.MarginRight = 0;

                        if (layout == VendorBillPrintLayout.Thermal80mm)
                        {
                            printSettings.ScaleFactor = 0.9;
                            printSettings.PageWidth = 3.1496;
                            printSettings.PageHeight = 128.976;
                        }

                        var status = await webView.CoreWebView2.PrintAsync(printSettings);
                        if (status == Microsoft.Web.WebView2.Core.CoreWebView2PrintStatus.Succeeded)
                        {
                            printedCount++;
                        }
                    }
                    else
                    {
                        VendorBillDocument.PrintDirectToPrinter(singleDoc, printer);
                        printedCount++;
                    }

                    // Crucial hardware buffer pacing: allow 600ms between bills so the thermal cutter can cycle and printer FIFO buffer flushes
                    await Task.Delay(600);
                }

                if (_bulkCts.IsCancellationRequested)
                {
                    lblBulkStatus.Text = string.Format("Cancelled. Printed {0} of {1} bill(s).", printedCount, batch.Count);
                    MessageBox.Show(string.Format("Bulk printing cancelled by user. Printed {0} bill(s).", printedCount), "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    lblBulkStatus.Text = string.Format("Complete! {0} bill(s) printed cleanly.", printedCount);
                    MessageBox.Show(string.Format("{0} vendor bill(s) printed cleanly to '{1}'!", printedCount, printer), "Bulk Print Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                lblBulkStatus.Text = "Error during bulk print: " + ex.Message;
                MessageBox.Show("Bulk print error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBulkPrintDirect.Enabled = true;
                btnBulkPreviewBatch.Enabled = true;
                btnCancelBulk.Visible = false;
                prgBulkProgress.Visible = false;
                _bulkCts?.Dispose();
                _bulkCts = null;
            }
        }

        private async Task PreviewCombinedBatchAsync()
        {
            var checkedItems = chklstVendors.CheckedItems.Cast<ChartOfAccountHeadDto>().ToList();
            if (checkedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one vendor to preview batch.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DateTime fromDate = dtpBulkFromDate.Value.Date;
            DateTime toDate = dtpBulkToDate.Value.Date;
            string dateBasis = (cmbBulkDateBasis != null && cmbBulkDateBasis.SelectedItem != null && cmbBulkDateBasis.SelectedItem.ToString().StartsWith("Clearing", StringComparison.OrdinalIgnoreCase))
                ? "ClearingDate"
                : "VoucherDate";
            var layout = (cmbBulkFormat != null && cmbBulkFormat.SelectedIndex == 1) ? VendorBillPrintLayout.Thermal80mm : VendorBillPrintLayout.A4Sheet;

            SetLoading(true, string.Format("Compiling batch preview for {0} vendors...", checkedItems.Count));

            try
            {
                await EnsureWebViewInitializedAsync();

                var batch = new List<VendorBillDataResult>();

                await Task.Run(() =>
                {
                    foreach (var cust in checkedItems)
                    {
                        try
                        {
                            DataSet ds = ReportQuery.WandaVendorBill(cust.Account, fromDate, toDate, dateBasis);
                            var res = VendorBillDataService.ConvertWandaDataSet(ds, cust.Account, cust.Title, fromDate, toDate, dateBasis);
                            batch.Add(res);
                        }
                        catch
                        {
                            // Skip vendor if database query fails
                        }
                    }
                });

                var batchDoc = new VendorBillBatchDocument(batch, layout);
                _currentPdfPath = await batchDoc.GeneratePdfToTempFileAsync();

                if (webView != null && _isWebViewReady && webView.CoreWebView2 != null)
                {
                    bool isThermalBatch = layout == VendorBillPrintLayout.Thermal80mm;
                    webView.CoreWebView2.Settings.HiddenPdfToolbarItems = isThermalBatch
                        ? Microsoft.Web.WebView2.Core.CoreWebView2PdfToolbarItems.Print
                        : Microsoft.Web.WebView2.Core.CoreWebView2PdfToolbarItems.None;
                }

                webView.CoreWebView2.Navigate(_currentPdfPath);
                webView.Visible = true;
                lblStatus.Text = string.Format("Combined Batch Preview: {0} vendor bills compiled.", batch.Count);
                lblStatus.ForeColor = Color.FromArgb(30, 41, 59);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error compiling batch preview: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                SetLoading(false);
            }
        }

        // ==========================================
        // EXPORT LOGIC (STANDARD 9 COLUMNS)
        // ==========================================
        private void ExportSingleToExcel()
        {
            if (_currentResult == null)
            {
                MessageBox.Show("Please generate the bill before exporting.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = string.Format("VendorBill_{0}_{1:yyyyMMdd}.xlsx", _currentResult.Summary.VendorCode ?? "Bill", DateTime.Today);
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var wb = new XLWorkbook())
                        {
                            var ws = wb.Worksheets.Add("Vendor Bill");

                            // Header
                            ws.Cell("A1").Value = _currentResult.Summary.CompanyName;
                            ws.Cell("A1").Style.Font.Bold = true;
                            ws.Cell("A1").Style.Font.FontSize = 14;

                            ws.Cell("A2").Value = "VENDOR BILL / INVOICE STATEMENT";
                            ws.Cell("A2").Style.Font.Bold = true;
                            ws.Cell("A2").Style.Font.FontSize = 11;

                            ws.Cell("A3").Value = string.Format("Billing Period: {0:dd-MMM-yyyy} to {1:dd-MMM-yyyy}", _currentResult.Summary.FromDate, _currentResult.Summary.ToDate);
                            ws.Cell("A4").Value = string.Format("Vendor: {0}", _currentResult.Summary.VendorName);

                            int row = 6;
                            // Table Headers (Standard 9 columns)
                            string[] headers = { "#", "Date", "Voucher #", "Item Description", "Unit", "Qty", "Rate", "Add / Less", "Amount" };
                            for (int c = 0; c < headers.Length; c++)
                            {
                                ws.Cell(row, c + 1).Value = headers[c];
                                ws.Cell(row, c + 1).Style.Font.Bold = true;
                                ws.Cell(row, c + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
                                ws.Cell(row, c + 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            }

                            row++;
                            for (int i = 0; i < _currentResult.Lines.Count; i++)
                            {
                                var l = _currentResult.Lines[i];
                                ws.Cell(row, 1).Value = i + 1;
                                ws.Cell(row, 2).Value = l.FormattedDate;
                                ws.Cell(row, 3).Value = l.VNo;
                                ws.Cell(row, 4).Value = l.Item;
                                ws.Cell(row, 5).Value = l.Unit;
                                ws.Cell(row, 6).Value = l.Qty;
                                ws.Cell(row, 7).Value = l.Rate;
                                ws.Cell(row, 8).Value = l.AddLess;
                                ws.Cell(row, 9).Value = l.Amount;

                                ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.##";
                                ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00";
                                row++;
                            }

                            // Subtotal
                            ws.Cell(row, 4).Value = "Current Bill Total:";
                            ws.Cell(row, 4).Style.Font.Bold = true;
                            ws.Cell(row, 9).Value = _currentResult.Summary.CurrentBillTotal;
                            ws.Cell(row, 9).Style.Font.Bold = true;
                            ws.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00";
                            row += 2;

                            // Summary Reconciliation
                            ws.Cell(row, 4).Value = "Previous Balance (B/F):";
                            ws.Cell(row, 9).Value = _currentResult.Summary.PreviousBalance;
                            ws.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00";
                            row++;

                            ws.Cell(row, 4).Value = "Current Period Bill:";
                            ws.Cell(row, 9).Value = _currentResult.Summary.CurrentBillTotal;
                            ws.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00";
                            row++;

                            ws.Cell(row, 4).Value = "Gross Total Payable:";
                            ws.Cell(row, 4).Style.Font.Bold = true;
                            ws.Cell(row, 9).Value = _currentResult.Summary.GrossTotal;
                            ws.Cell(row, 9).Style.Font.Bold = true;
                            ws.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00";
                            row++;

                            ws.Cell(row, 4).Value = "Less Payments Made:";
                            ws.Cell(row, 9).Value = -_currentResult.Summary.PaymentsMade;
                            ws.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00";
                            row++;

                            ws.Cell(row, 4).Value = "NET BALANCE DUE:";
                            ws.Cell(row, 4).Style.Font.Bold = true;
                            ws.Cell(row, 9).Value = _currentResult.Summary.NetBalance;
                            ws.Cell(row, 9).Style.Font.Bold = true;
                            ws.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(row, 9).Style.Border.BottomBorder = XLBorderStyleValues.Double;

                            ws.Columns().AdjustToContents();
                            wb.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Excel bill exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Export error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportSingleToCsv()
        {
            if (_currentResult == null)
            {
                MessageBox.Show("Please generate the bill before exporting.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.FileName = string.Format("VendorBill_{0}_{1:yyyyMMdd}.csv", _currentResult.Summary.VendorCode ?? "Bill", DateTime.Today);
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(sfd.FileName))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            var exportRows = _currentResult.Lines.Select((l, i) => new
                            {
                                SerialNo = i + 1,
                                Date = l.FormattedDate,
                                VoucherNo = l.VNo,
                                Item = l.Item,
                                Unit = l.Unit,
                                Qty = l.Qty,
                                Rate = l.Rate,
                                AddLess = l.AddLess,
                                Amount = l.Amount
                            });
                            csv.WriteRecords(exportRows);
                        }
                        MessageBox.Show("CSV bill exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Export error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SetLoading(bool isLoading, string statusText = "")
        {
            prgLoading.Visible = isLoading;
            if (!string.IsNullOrEmpty(statusText))
            {
                lblStatus.Text = statusText;
            }
            btnGenerateSingle.Enabled = !isLoading;
            btnExportExcel.Enabled = !isLoading;
            btnExportCsv.Enabled = !isLoading;
            btnPrintPreview.Enabled = !isLoading;
            btnPrintDirectSingle.Enabled = !isLoading;
        }
    }
}
