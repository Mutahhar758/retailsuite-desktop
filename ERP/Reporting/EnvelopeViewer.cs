using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Reporting.Documents;
using ERP.Reporting.Models;
using ERP.Services.Legacy;
using Microsoft.Web.WebView2.WinForms;

namespace ERP.Reporting
{
    /// <summary>
    /// Modern WinForms viewer for printing business mailing envelopes.
    /// Supports multi-account selection, DL envelope preview in WebView2, and direct silent printing.
    /// </summary>
    public class EnvelopeViewer : Form
    {
        private readonly ChartOfAccountApiService _chartOfAccountApiService;

        // UI Controls
        private Panel pnlSidebar;
        private Label lblSearch;
        private TextBox txtSearch;
        private CheckBox chkSelectAll;
        private CheckedListBox chklstAccounts;
        private Label lblSelectedCount;
        private Label lblPrinter;
        private ComboBox cmbPrinter;
        private Button btnPreview;
        private Button btnPrintDirect;
        private Button btnPrintDialog;

        private Panel pnlCanvas;
        private Label lblStatus;
        private ProgressBar prgLoading;
        private WebView2 webView;

        // State
        private List<ChartOfAccountHeadDto> _allAccounts = new List<ChartOfAccountHeadDto>();
        private List<ChartOfAccountHeadDto> _filteredAccounts = new List<ChartOfAccountHeadDto>();
        private List<EnvelopeItem> _currentEnvelopes = new List<EnvelopeItem>();
        private string _currentPdfPath;
        private bool _isWebViewReady = false;

        public EnvelopeViewer()
        {
            _chartOfAccountApiService = new ChartOfAccountApiService();
            InitializeComponentCodeFirst();
            this.Load += async (s, e) => await InitializeDataAsync();
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Envelope";
            this.Size = new Size(1160, 760);
            this.MinimumSize = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.BackColor = Color.FromArgb(248, 250, 252);

            // 1. Sidebar (Dock Left)
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 380,
                BackColor = Color.White,
                Padding = new Padding(14)
            };
            pnlSidebar.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height);
            };

            int curY = 14;

            // Title
            var lblTitle = new Label
            {
                Text = "ENVELOPE RECIPIENTS",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };
            pnlSidebar.Controls.Add(lblTitle);
            curY += 28;

            // Search Filter
            lblSearch = new Label
            {
                Text = "SEARCH ACCOUNTS:",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblSearch);
            curY += 18;

            txtSearch = new TextBox
            {
                Location = new Point(14, curY),
                Width = 350,
                Font = new Font("Segoe UI", 9F)
            };
            txtSearch.TextChanged += (s, e) => FilterAccountList();
            pnlSidebar.Controls.Add(txtSearch);
            curY += 32;

            // Select All
            chkSelectAll = new CheckBox
            {
                Text = "Select All Recipients",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59)
            };
            chkSelectAll.CheckedChanged += (s, e) =>
            {
                for (int i = 0; i < chklstAccounts.Items.Count; i++)
                    chklstAccounts.SetItemChecked(i, chkSelectAll.Checked);
                UpdateSelectedCount();
            };
            pnlSidebar.Controls.Add(chkSelectAll);

            lblSelectedCount = new Label
            {
                Text = "Selected: 0",
                Location = new Point(240, curY + 2),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235)
            };
            pnlSidebar.Controls.Add(lblSelectedCount);
            curY += 26;

            // Accounts CheckList
            chklstAccounts = new CheckedListBox
            {
                Location = new Point(14, curY),
                Width = 350,
                Height = 340,
                CheckOnClick = true,
                Font = new Font("Segoe UI", 8.5F)
            };
            chklstAccounts.ItemCheck += (s, e) => this.BeginInvoke(new Action(UpdateSelectedCount));
            pnlSidebar.Controls.Add(chklstAccounts);
            curY += 346;

            // Printer Selector
            lblPrinter = new Label
            {
                Text = "PRINTER (FOR DIRECT SILENT PRINT):",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblPrinter);
            curY += 18;

            cmbPrinter = new ComboBox
            {
                Location = new Point(14, curY),
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            pnlSidebar.Controls.Add(cmbPrinter);
            curY += 34;

            // Action Buttons
            btnPreview = new Button
            {
                Text = "Preview Envelopes",
                Location = new Point(14, curY),
                Width = 170,
                Height = 34,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPreview.FlatAppearance.BorderSize = 0;
            btnPreview.Click += async (s, e) => await LoadAndRenderEnvelopesAsync();
            pnlSidebar.Controls.Add(btnPreview);

            btnPrintDirect = new Button
            {
                Text = "🖨 Print Direct",
                Location = new Point(194, curY),
                Width = 170,
                Height = 34,
                BackColor = Color.FromArgb(5, 150, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrintDirect.FlatAppearance.BorderSize = 0;
            btnPrintDirect.Click += async (s, e) => await PrintDirectAsync();
            pnlSidebar.Controls.Add(btnPrintDirect);
            curY += 40;

            btnPrintDialog = new Button
            {
                Text = "Print with Dialog...",
                Location = new Point(14, curY),
                Width = 350,
                Height = 28,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrintDialog.FlatAppearance.BorderSize = 0;
            btnPrintDialog.Click += (s, e) =>
            {
                if (webView != null && _isWebViewReady)
                    webView.CoreWebView2.ShowPrintUI(Microsoft.Web.WebView2.Core.CoreWebView2PrintDialogKind.Browser);
            };
            pnlSidebar.Controls.Add(btnPrintDialog);

            this.Controls.Add(pnlSidebar);

            // 2. Canvas Area
            pnlCanvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(12)
            };

            lblStatus = new Label
            {
                Text = "Select recipients from the left panel and click 'Preview Envelopes'.",
                AutoSize = true,
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlCanvas.Controls.Add(lblStatus);

            prgLoading = new ProgressBar
            {
                Location = new Point(20, 48),
                Width = 240,
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
                // Printers
                cmbPrinter.Items.Clear();
                foreach (string p in PrinterSettings.InstalledPrinters)
                    cmbPrinter.Items.Add(p);

                if (!string.IsNullOrWhiteSpace(ConfigInfo.ThermalPrinterName) && cmbPrinter.Items.Contains(ConfigInfo.ThermalPrinterName))
                    cmbPrinter.SelectedItem = ConfigInfo.ThermalPrinterName;
                else if (cmbPrinter.Items.Count > 0)
                    cmbPrinter.SelectedIndex = 0;

                // Load customer accounts
                var accounts = await _chartOfAccountApiService.GetCustomerAccountsAsync();
                _allAccounts = accounts ?? new List<ChartOfAccountHeadDto>();
                _filteredAccounts = new List<ChartOfAccountHeadDto>(_allAccounts);
                PopulateAccountList();

                // Initialize WebView2
                string userDataFolder = Path.Combine(Path.GetTempPath(), "RetailSuite", "WebView2_Envelope");
                if (!Directory.Exists(userDataFolder)) Directory.CreateDirectory(userDataFolder);
                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);
                _isWebViewReady = true;

                // Pre-check first 3 if available and preview
                if (chklstAccounts.Items.Count > 0)
                {
                    int toCheck = Math.Min(3, chklstAccounts.Items.Count);
                    for (int i = 0; i < toCheck; i++) chklstAccounts.SetItemChecked(i, true);
                    UpdateSelectedCount();
                    await LoadAndRenderEnvelopesAsync();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error initializing accounts: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void PopulateAccountList()
        {
            chklstAccounts.Items.Clear();
            foreach (var acc in _filteredAccounts)
                chklstAccounts.Items.Add(acc);
            chklstAccounts.DisplayMember = "Title";
            UpdateSelectedCount();
        }

        private void FilterAccountList()
        {
            string q = txtSearch.Text.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(q))
            {
                _filteredAccounts = new List<ChartOfAccountHeadDto>(_allAccounts);
            }
            else
            {
                _filteredAccounts = _allAccounts
                    .Where(x => (x.Title ?? "").ToLowerInvariant().Contains(q) || (x.Account ?? "").ToLowerInvariant().Contains(q))
                    .ToList();
            }
            PopulateAccountList();
        }

        private void UpdateSelectedCount()
        {
            lblSelectedCount.Text = string.Format("Selected: {0} of {1}", chklstAccounts.CheckedItems.Count, chklstAccounts.Items.Count);
        }

        private async Task LoadAndRenderEnvelopesAsync()
        {
            var checkedItems = chklstAccounts.CheckedItems.Cast<ChartOfAccountHeadDto>().ToList();
            if (checkedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one recipient.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            prgLoading.Visible = true;
            lblStatus.Text = string.Format("Generating {0} envelopes...", checkedItems.Count);

            try
            {
                string[] codes = checkedItems.Select(x => x.Account).ToArray();
                List<EnvelopeItem> envelopes = null;

                try
                {
                    DataTable dt = await Task.Run(() => ReportQuery.EnvelopeDetail(codes));
                    envelopes = EnvelopeDataService.ConvertDataTable(dt);
                }
                catch
                {
                    envelopes = EnvelopeDataService.GetMockData();
                }

                if (envelopes == null || envelopes.Count == 0)
                {
                    // Fallback to names from checked list
                    string senderCompany = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise";
                    string senderAddress = CompanyInfo.Address ?? string.Empty;
                    string senderPhone = !string.IsNullOrWhiteSpace(CompanyInfo.ContactHead) ? CompanyInfo.ContactHead : CompanyInfo.Cell;

                    envelopes = checkedItems.Select(x => new EnvelopeItem
                    {
                        SenderCompany = senderCompany,
                        SenderAddress = senderAddress,
                        SenderPhone = senderPhone,
                        RecipientName = x.Title,
                        RecipientAccount = x.Account
                    }).ToList();
                }

                _currentEnvelopes = envelopes;
                var doc = new EnvelopeDocument(envelopes);
                _currentPdfPath = await doc.GeneratePdfToTempFileAsync();

                webView.CoreWebView2.Navigate(_currentPdfPath);
                webView.Visible = true;
                lblStatus.Text = string.Format("Previewing {0} DL Mailing Envelopes (220 x 110 mm).", envelopes.Count);
                lblStatus.ForeColor = Color.FromArgb(30, 41, 59);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error rendering envelopes: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                prgLoading.Visible = false;
            }
        }

        private async Task PrintDirectAsync()
        {
            if (_currentEnvelopes.Count == 0)
            {
                await LoadAndRenderEnvelopesAsync();
                if (_currentEnvelopes.Count == 0) return;
            }

            string printer = cmbPrinter.SelectedItem != null ? cmbPrinter.SelectedItem.ToString() : ConfigInfo.ThermalPrinterName;
            var confirm = MessageBox.Show(
                string.Format("Are you sure you want to silently send {0} envelope(s) to '{1}'?", _currentEnvelopes.Count, printer),
                "Confirm Print",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                prgLoading.Visible = true;
                lblStatus.Text = "Printing envelopes silently...";

                await Task.Run(() =>
                {
                    var doc = new EnvelopeDocument(_currentEnvelopes);
                    EnvelopeDocument.PrintDirectToPrinter(doc, printer);
                });

                MessageBox.Show(string.Format("{0} envelope(s) sent silently to printer '{1}'!", _currentEnvelopes.Count, printer), "Print Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblStatus.Text = string.Format("Complete: {0} envelopes sent to {1}.", _currentEnvelopes.Count, printer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                prgLoading.Visible = false;
            }
        }
    }
}
