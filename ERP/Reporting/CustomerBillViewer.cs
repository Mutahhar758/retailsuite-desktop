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
    /// Modern, code-first WinForms Customer Bill / Statement Viewer hosting WebView2
    /// with QuestPDF rendering, ClosedXML / CsvHelper exports, and direct silent bulk printing.
    /// </summary>
    public class CustomerBillViewer : Form
    {
        // Services
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly SupplyOrderApiService _supplyOrderApiService;

        // UI Controls - Mode Switch
        private Panel pnlHeader;
        private Label lblTitle;
        private Button btnModeSingle;
        private Button btnModeBulk;

        // UI Controls - Single Mode Panel
        private Panel pnlSingleControls;
        private Label lblCustomer;
        private ComboBox cmbCustomer;
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
        private TextBox txtCustomerSearch;
        private CheckBox chkSelectAll;
        private Label lblSupplyOrder;
        private ComboBox cmbSupplyOrder;
        private CheckedListBox chklstCustomers;
        private Label lblSelectedCount;
        private Label lblPrinter;
        private ComboBox cmbPrinter;
        private Label lblBulkDates;
        private DateTimePicker dtpBulkFromDate;
        private DateTimePicker dtpBulkToDate;
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
        private List<ChartOfAccountHeadDto> _allCustomers = new List<ChartOfAccountHeadDto>();
        private List<ChartOfAccountHeadDto> _filteredCustomers = new List<ChartOfAccountHeadDto>();
        private CustomerBillDataResult _currentResult;
        private string _currentPdfPath;
        private bool _isWebViewReady = false;
        private bool _isBulkMode = false;
        private CancellationTokenSource _bulkCts;

        public CustomerBillViewer()
        {
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _supplyOrderApiService = new SupplyOrderApiService();

            InitializeComponentCodeFirst();
            this.Load += async (s, e) => await InitializeDataAsync();
        }

        public CustomerBillViewer(string initialCustomerCode, DateTime fromDate, DateTime toDate)
            : this()
        {
            dtpFromDate.Value = fromDate;
            dtpToDate.Value = toDate;
            dtpBulkFromDate.Value = fromDate;
            dtpBulkToDate.Value = toDate;
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Customer Bill";
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
                Text = "CUSTOMER BILL & STATEMENT",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                Location = new Point(16, 15),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblTitle);

            btnModeSingle = new Button
            {
                Text = "Single Customer Preview",
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

            lblCustomer = new Label
            {
                Text = "SELECT CUSTOMER",
                AutoSize = true,
                Location = new Point(16, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            cmbCustomer = new ComboBox
            {
                Location = new Point(16, 28),
                Width = 230,
                DropDownStyle = ComboBoxStyle.DropDownList,
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
                Location = new Point(501, 10),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            cmbDateBasis = new ComboBox
            {
                Location = new Point(501, 28),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbDateBasis.Items.AddRange(new object[] { "VoucherDate", "EntryDate" });
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
            cmbLayout.SelectedIndex = 0;
            cmbLayout.SelectedIndexChanged += async (s, e) =>
            {
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
            btnPrintThermalSingle.Click += (s, e) => TriggerDirectThermalPrint();

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
                lblCustomer, cmbCustomer,
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

            txtCustomerSearch = new TextBox
            {
                Location = new Point(12, curY),
                Width = 370,
                Font = new Font("Segoe UI", 9F)
            };
            txtCustomerSearch.TextChanged += (s, e) => FilterCustomerChecklist();
            pnlBulkSidebar.Controls.Add(txtCustomerSearch);
            curY += 30;

            // Supply Order Profile Dropdown Filter
            lblSupplyOrder = new Label
            {
                Text = "FILTER BY SUPPLY ORDER PROFILE:",
                Location = new Point(12, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlBulkSidebar.Controls.Add(lblSupplyOrder);
            curY += 18;

            cmbSupplyOrder = new ComboBox
            {
                Location = new Point(12, curY),
                Width = 370,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbSupplyOrder.SelectedIndexChanged += async (s, e) => await OnSupplyOrderChangedAsync();
            pnlBulkSidebar.Controls.Add(cmbSupplyOrder);
            curY += 32;

            // Select All & Count
            chkSelectAll = new CheckBox
            {
                Text = "Select All Customers",
                Location = new Point(12, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59)
            };
            chkSelectAll.CheckedChanged += (s, e) =>
            {
                for (int i = 0; i < chklstCustomers.Items.Count; i++)
                {
                    chklstCustomers.SetItemChecked(i, chkSelectAll.Checked);
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

            // Customer Checklist
            chklstCustomers = new CheckedListBox
            {
                Location = new Point(12, curY),
                Width = 370,
                Height = 220,
                CheckOnClick = true,
                Font = new Font("Segoe UI", 8.5F)
            };
            chklstCustomers.ItemCheck += (s, e) =>
            {
                this.BeginInvoke(new Action(UpdateSelectedCount));
            };
            pnlBulkSidebar.Controls.Add(chklstCustomers);
            curY += 226;

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
            cmbBulkFormat.SelectedIndex = 1; // Default to 80mm thermal receipt for bulk printing
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
                Text = "Select a customer or switch to Bulk Print mode to begin.",
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

                if (!string.IsNullOrWhiteSpace(ConfigInfo.ThermalPrinterName) && cmbPrinter.Items.Contains(ConfigInfo.ThermalPrinterName))
                {
                    cmbPrinter.SelectedItem = ConfigInfo.ThermalPrinterName;
                }
                else if (cmbPrinter.Items.Count > 0)
                {
                    cmbPrinter.SelectedIndex = 0;
                }

                // Populate Customers
                var customers = await _chartOfAccountApiService.GetCustomerAccountsAsync();
                _allCustomers = customers ?? new List<ChartOfAccountHeadDto>();
                _filteredCustomers = new List<ChartOfAccountHeadDto>(_allCustomers);

                // Populate Single Mode ComboBox
                cmbCustomer.DataSource = null;
                cmbCustomer.DisplayMember = "Title";
                cmbCustomer.ValueMember = "Account";
                cmbCustomer.DataSource = new List<ChartOfAccountHeadDto>(_allCustomers);
                if (cmbCustomer.Items.Count > 0) cmbCustomer.SelectedIndex = 0;

                // Populate Bulk Mode CheckList
                PopulateCustomerChecklist();

                // Populate Supply Orders
                var supplyOrders = await _supplyOrderApiService.GetAsync();
                var dtOrders = new DataTable();
                dtOrders.Columns.Add("Id", typeof(string));
                dtOrders.Columns.Add("Title", typeof(string));
                dtOrders.Rows.Add("", "--- Select Profile to Filter ---");

                if (supplyOrders != null)
                {
                    foreach (var so in supplyOrders)
                        dtOrders.Rows.Add(so.Id.ToString(), so.Title);
                }
                cmbSupplyOrder.DataSource = dtOrders;
                cmbSupplyOrder.DisplayMember = "Title";
                cmbSupplyOrder.ValueMember = "Id";
                cmbSupplyOrder.SelectedIndex = 0;

                // Initialize WebView2
                await EnsureWebViewInitializedAsync();

                // Ready state: Do not auto-generate bill on form load; wait for user to click Generate Bill
                lblStatus.Text = "Ready. Select customer & date range, then click 'Generate Bill'.";
                lblStatus.ForeColor = Color.FromArgb(71, 85, 105);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error initializing customer data: " + ex.Message;
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

        private void PopulateCustomerChecklist()
        {
            chklstCustomers.Items.Clear();
            foreach (var cust in _filteredCustomers)
            {
                chklstCustomers.Items.Add(cust);
            }
            chklstCustomers.DisplayMember = "Title";
            UpdateSelectedCount();
        }

        private void FilterCustomerChecklist()
        {
            string query = txtCustomerSearch.Text.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(query))
            {
                _filteredCustomers = new List<ChartOfAccountHeadDto>(_allCustomers);
            }
            else
            {
                _filteredCustomers = _allCustomers
                    .Where(x => (x.Title ?? "").ToLowerInvariant().Contains(query) || (x.Account ?? "").ToLowerInvariant().Contains(query))
                    .ToList();
            }
            PopulateCustomerChecklist();
        }

        private void UpdateSelectedCount()
        {
            int count = chklstCustomers.CheckedItems.Count;
            lblSelectedCount.Text = string.Format("Selected: {0} of {1}", count, chklstCustomers.Items.Count);
        }

        private async Task OnSupplyOrderChangedAsync()
        {
            if (cmbSupplyOrder.SelectedIndex <= 0 || cmbSupplyOrder.SelectedValue == null) return;

            string idStr = cmbSupplyOrder.SelectedValue.ToString();
            int id = 0;
            if (!int.TryParse(idStr, out id)) return;

            try
            {
                for (int i = 0; i < chklstCustomers.Items.Count; i++)
                    chklstCustomers.SetItemChecked(i, false);
                chkSelectAll.Checked = false;

                var order = await _supplyOrderApiService.GetByIdAsync(id);
                if (order != null && order.Details != null)
                {
                    var customerIds = order.Details.Select(d => d.CustomerId).ToList();
                    for (int i = 0; i < chklstCustomers.Items.Count; i++)
                    {
                        var cust = chklstCustomers.Items[i] as ChartOfAccountHeadDto;
                        if (cust != null && customerIds.Contains(cust.Account))
                        {
                            chklstCustomers.SetItemChecked(i, true);
                        }
                    }
                }
                UpdateSelectedCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading supply order profile: " + ex.Message, "Supply Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task EnsureWebViewInitializedAsync()
        {
            if (!_isWebViewReady)
            {
                string userDataFolder = Path.Combine(Path.GetTempPath(), "RetailSuite", "WebView2_CustomerBill");
                if (!Directory.Exists(userDataFolder)) Directory.CreateDirectory(userDataFolder);

                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);
                _isWebViewReady = true;
            }
        }

        // ==========================================
        // SINGLE BILL LOGIC
        // ==========================================
        private async Task LoadAndRenderSingleBillAsync()
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.", "Customer Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string customerCode = cmbCustomer.SelectedValue.ToString();
            string customerTitle = cmbCustomer.Text;
            DateTime fromDate = dtpFromDate.Value.Date;
            DateTime toDate = dtpToDate.Value.Date;
            string dateBasis = cmbDateBasis.SelectedItem != null ? cmbDateBasis.SelectedItem.ToString() : "VoucherDate";

            SetLoading(true, string.Format("Generating bill for {0}...", customerTitle));

            try
            {
                await EnsureWebViewInitializedAsync();

                DataSet ds = await Task.Run(() => ReportQuery.CustomerBill(customerCode, fromDate, toDate, dateBasis));
                CustomerBillDataResult result = CustomerBillDataService.ConvertDataSet(ds, customerCode, customerTitle, fromDate, toDate, dateBasis);

                _currentResult = result;
                var layout = (cmbLayout != null && cmbLayout.SelectedIndex == 1) ? CustomerBillPrintLayout.Thermal80mm : CustomerBillPrintLayout.A4Sheet;
                var doc = new CustomerBillDocument(result.Summary, result.Lines, layout);
                _currentPdfPath = await doc.GeneratePdfToTempFileAsync();

                webView.CoreWebView2.Navigate(_currentPdfPath);
                webView.Visible = true;
                string layoutLabel = layout == CustomerBillPrintLayout.Thermal80mm ? "80mm Thermal Receipt" : "A4 Invoice";
                lblStatus.Text = string.Format("Customer Bill ({0}): {1} ({2:dd-MMM-yyyy} - {3:dd-MMM-yyyy}) • Net Due: {4:#,##0.00}",
                    layoutLabel, result.Summary.CustomerName, result.Summary.FromDate, result.Summary.ToDate, result.Summary.NetBalance);
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

        private void TriggerPrintPreview()
        {
            if (webView != null && _isWebViewReady)
            {
                webView.CoreWebView2.ShowPrintUI(Microsoft.Web.WebView2.Core.CoreWebView2PrintDialogKind.Browser);
            }
        }

        private void TriggerDirectThermalPrint()
        {
            if (_currentResult == null)
            {
                MessageBox.Show("Please preview or generate the customer bill first.", "Thermal Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                var doc = new CustomerBillDocument(_currentResult.Summary, _currentResult.Lines, CustomerBillPrintLayout.Thermal80mm);
                CustomerBillDocument.PrintDirectToPrinter(doc, printer);
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

            string printer = cmbPrinter.SelectedItem != null ? cmbPrinter.SelectedItem.ToString() : ConfigInfo.ThermalPrinterName;
            var layout = (cmbLayout != null && cmbLayout.SelectedIndex == 1) ? CustomerBillPrintLayout.Thermal80mm : CustomerBillPrintLayout.A4Sheet;
            try
            {
                var doc = new CustomerBillDocument(_currentResult.Summary, _currentResult.Lines, layout);
                CustomerBillDocument.PrintDirectToPrinter(doc, printer);
                MessageBox.Show(string.Format("Customer bill sent silently to '{0}' successfully!", printer), "Print Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            var checkedItems = chklstCustomers.CheckedItems.Cast<ChartOfAccountHeadDto>().ToList();
            if (checkedItems.Count == 0)
            {
                MessageBox.Show("Please check at least one customer from the list for bulk printing.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string printer = cmbPrinter.SelectedItem != null ? cmbPrinter.SelectedItem.ToString() : ConfigInfo.ThermalPrinterName;
            if (string.IsNullOrWhiteSpace(printer))
            {
                MessageBox.Show("Please select a destination printer.", "Printer Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime fromDate = dtpBulkFromDate.Value.Date;
            DateTime toDate = dtpBulkToDate.Value.Date;
            string dateBasis = "VoucherDate";
            var layout = (cmbBulkFormat != null && cmbBulkFormat.SelectedIndex == 1) ? CustomerBillPrintLayout.Thermal80mm : CustomerBillPrintLayout.A4Sheet;
            string formatName = layout == CustomerBillPrintLayout.Thermal80mm ? "80mm Thermal Receipt" : "A4 Commercial Invoice";

            var confirm = MessageBox.Show(
                string.Format("Are you sure you want to silently print {0} customer bill(s) in {1} format directly to '{2}'?", checkedItems.Count, formatName, printer),
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

            int printedCount = 0;
            int errorCount = 0;

            try
            {
                for (int i = 0; i < checkedItems.Count; i++)
                {
                    if (_bulkCts.IsCancellationRequested) break;

                    var customer = checkedItems[i];
                    lblBulkStatus.Text = string.Format("Printing {0} of {1}: {2}...", i + 1, checkedItems.Count, customer.Title);
                    prgBulkProgress.Value = i + 1;

                    await Task.Run(() =>
                    {
                        try
                        {
                            DataSet ds = ReportQuery.CustomerBill(customer.Account, fromDate, toDate, dateBasis);
                            var result = CustomerBillDataService.ConvertDataSet(ds, customer.Account, customer.Title, fromDate, toDate, dateBasis);

                            // Only print if there are line items or a non-zero balance
                            if (result.Lines.Count > 0 || Math.Abs(result.Summary.NetBalance) > 0.01m)
                            {
                                var doc = new CustomerBillDocument(result.Summary, result.Lines, layout);
                                CustomerBillDocument.PrintDirectToPrinter(doc, printer);
                                printedCount++;
                            }
                        }
                        catch
                        {
                            errorCount++;
                        }
                    });
                }

                if (_bulkCts.IsCancellationRequested)
                {
                    lblBulkStatus.Text = string.Format("Cancelled. Printed {0} bill(s).", printedCount);
                    MessageBox.Show(string.Format("Bulk printing cancelled by user. Printed {0} bill(s).", printedCount), "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    lblBulkStatus.Text = string.Format("Complete! {0} bill(s) printed directly.", printedCount);
                    MessageBox.Show(string.Format("{0} customer bill(s) printed directly to '{1}' without interaction!", printedCount, printer), "Bulk Print Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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
            var checkedItems = chklstCustomers.CheckedItems.Cast<ChartOfAccountHeadDto>().ToList();
            if (checkedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one customer to preview batch.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DateTime fromDate = dtpBulkFromDate.Value.Date;
            DateTime toDate = dtpBulkToDate.Value.Date;
            string dateBasis = "VoucherDate";
            var layout = (cmbBulkFormat != null && cmbBulkFormat.SelectedIndex == 1) ? CustomerBillPrintLayout.Thermal80mm : CustomerBillPrintLayout.A4Sheet;

            SetLoading(true, string.Format("Compiling batch preview for {0} customers...", checkedItems.Count));

            try
            {
                await EnsureWebViewInitializedAsync();

                var batch = new List<CustomerBillDataResult>();

                await Task.Run(() =>
                {
                    foreach (var cust in checkedItems)
                    {
                        try
                        {
                            DataSet ds = ReportQuery.CustomerBill(cust.Account, fromDate, toDate, dateBasis);
                            var res = CustomerBillDataService.ConvertDataSet(ds, cust.Account, cust.Title, fromDate, toDate, dateBasis);
                            batch.Add(res);
                        }
                        catch
                        {
                            // Skip customer if database query fails
                        }
                    }
                });

                var batchDoc = new CustomerBillBatchDocument(batch, layout);
                _currentPdfPath = await batchDoc.GeneratePdfToTempFileAsync();

                webView.CoreWebView2.Navigate(_currentPdfPath);
                webView.Visible = true;
                lblStatus.Text = string.Format("Combined Batch Preview: {0} customer bills compiled.", batch.Count);
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
        // EXPORT LOGIC
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
                sfd.FileName = string.Format("CustomerBill_{0}_{1:yyyyMMdd}.xlsx", _currentResult.Summary.CustomerCode ?? "Bill", DateTime.Today);
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var wb = new XLWorkbook())
                        {
                            var ws = wb.Worksheets.Add("Customer Bill");

                            // Header
                            ws.Cell("A1").Value = _currentResult.Summary.CompanyName;
                            ws.Cell("A1").Style.Font.Bold = true;
                            ws.Cell("A1").Style.Font.FontSize = 14;

                            ws.Cell("A2").Value = "CUSTOMER BILL / INVOICE STATEMENT";
                            ws.Cell("A2").Style.Font.Bold = true;
                            ws.Cell("A2").Style.Font.FontSize = 11;

                            ws.Cell("A3").Value = string.Format("Billing Period: {0:dd-MMM-yyyy} to {1:dd-MMM-yyyy}", _currentResult.Summary.FromDate, _currentResult.Summary.ToDate);
                            ws.Cell("A4").Value = string.Format("Customer: {0}", _currentResult.Summary.CustomerName);

                            int row = 6;
                            // Table Headers
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

                            ws.Cell(row, 4).Value = "Less Payments Received:";
                            ws.Cell(row, 9).Value = -_currentResult.Summary.PaymentsReceived;
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
                sfd.FileName = string.Format("CustomerBill_{0}_{1:yyyyMMdd}.csv", _currentResult.Summary.CustomerCode ?? "Bill", DateTime.Today);
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(sfd.FileName))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.WriteRecords(_currentResult.Lines);
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
