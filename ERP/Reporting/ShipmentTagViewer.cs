using System;
using System.Collections.Generic;
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
    /// Modern WinForms viewer for generating and printing shipping carton/parcel label tags.
    /// Supports multi-carton batches ("Carton 1 of N"), carrier tracking, and direct silent printing.
    /// </summary>
    public class ShipmentTagViewer : Form
    {
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly CustomerApiService _customerApiService;

        // UI Controls - Sidebar
        private Panel pnlSidebar;
        private Label lblCustomer;
        private ComboBox cmbCustomer;
        private Label lblName;
        private TextBox txtName;
        private Label lblAddress;
        private TextBox txtAddress;
        private Label lblCity;
        private TextBox txtCity;
        private Label lblPhone;
        private TextBox txtPhone;
        private Label lblCourier;
        private ComboBox cmbCourier;
        private Label lblCartons;
        private NumericUpDown nudCartons;
        private Label lblWeight;
        private TextBox txtWeight;
        private Label lblTracking;
        private TextBox txtTracking;
        private Label lblPrinter;
        private ComboBox cmbPrinter;
        private Button btnPreview;
        private Button btnPrintDirect;
        private Button btnPrintDialog;

        // UI Controls - Canvas
        private Panel pnlCanvas;
        private Label lblStatus;
        private ProgressBar prgLoading;
        private WebView2 webView;

        // State
        private List<ChartOfAccountHeadDto> _allCustomers = new List<ChartOfAccountHeadDto>();
        private List<CustomerDto> _customerDetails = new List<CustomerDto>();
        private List<ShipmentTagItem> _currentTags = new List<ShipmentTagItem>();
        private string _currentPdfPath;
        private bool _isWebViewReady = false;

        public ShipmentTagViewer()
        {
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _customerApiService = new CustomerApiService();

            InitializeComponentCodeFirst();
            this.Load += async (s, e) => await InitializeDataAsync();
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Shipment Label Tag";
            this.Size = new Size(1160, 800);
            this.MinimumSize = new Size(950, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.BackColor = Color.FromArgb(248, 250, 252);

            // 1. Sidebar (Dock Left)
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 370,
                BackColor = Color.White,
                Padding = new Padding(14),
                AutoScroll = true
            };
            pnlSidebar.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height);
            };

            int curY = 12;

            var lblTitle = new Label
            {
                Text = "SHIPMENT LABEL CONFIGURATION",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };
            pnlSidebar.Controls.Add(lblTitle);
            curY += 26;

            // Customer Selector
            lblCustomer = new Label
            {
                Text = "SELECT CUSTOMER / RECIPIENT:",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblCustomer);
            curY += 18;

            cmbCustomer = new ComboBox
            {
                Location = new Point(14, curY),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbCustomer.SelectedIndexChanged += (s, e) => OnCustomerSelected();
            pnlSidebar.Controls.Add(cmbCustomer);
            curY += 32;

            // Consignee Name
            lblName = new Label
            {
                Text = "CONSIGNEE NAME / CONTACT PERSON:",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblName);
            curY += 18;

            txtName = new TextBox
            {
                Location = new Point(14, curY),
                Width = 330,
                Font = new Font("Segoe UI", 9F)
            };
            pnlSidebar.Controls.Add(txtName);
            curY += 30;

            // Address
            lblAddress = new Label
            {
                Text = "DELIVERY ADDRESS:",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblAddress);
            curY += 18;

            txtAddress = new TextBox
            {
                Location = new Point(14, curY),
                Width = 330,
                Multiline = true,
                Height = 44,
                Font = new Font("Segoe UI", 9F)
            };
            pnlSidebar.Controls.Add(txtAddress);
            curY += 50;

            // City & Phone Row
            lblCity = new Label
            {
                Text = "DESTINATION CITY:",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblCity);

            lblPhone = new Label
            {
                Text = "PHONE / CELL:",
                Location = new Point(180, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblPhone);
            curY += 18;

            txtCity = new TextBox
            {
                Location = new Point(14, curY),
                Width = 155,
                Font = new Font("Segoe UI", 9F)
            };
            pnlSidebar.Controls.Add(txtCity);

            txtPhone = new TextBox
            {
                Location = new Point(180, curY),
                Width = 164,
                Font = new Font("Segoe UI", 9F)
            };
            pnlSidebar.Controls.Add(txtPhone);
            curY += 32;

            // Courier Service
            lblCourier = new Label
            {
                Text = "COURIER / CARRIER SERVICE:",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblCourier);
            curY += 18;

            cmbCourier = new ComboBox
            {
                Location = new Point(14, curY),
                Width = 330,
                Font = new Font("Segoe UI", 9F)
            };
            cmbCourier.Items.AddRange(new object[] { "TCS Express Cargo", "Leopards Courier", "M&P Express", "Own Delivery Van", "By Hand Dispatch" });
            cmbCourier.SelectedIndex = 0;
            pnlSidebar.Controls.Add(cmbCourier);
            curY += 32;

            // Cartons & Weight Row
            lblCartons = new Label
            {
                Text = "TOTAL CARTONS:",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblCartons);

            lblWeight = new Label
            {
                Text = "EST. WEIGHT (KG):",
                Location = new Point(180, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblWeight);
            curY += 18;

            nudCartons = new NumericUpDown
            {
                Location = new Point(14, curY),
                Width = 155,
                Minimum = 1,
                Maximum = 500,
                Value = 1,
                Font = new Font("Segoe UI", 9F)
            };
            pnlSidebar.Controls.Add(nudCartons);

            txtWeight = new TextBox
            {
                Location = new Point(180, curY),
                Width = 164,
                Text = "10.0",
                Font = new Font("Segoe UI", 9F)
            };
            pnlSidebar.Controls.Add(txtWeight);
            curY += 32;

            // Tracking #
            lblTracking = new Label
            {
                Text = "TRACKING / CONSIGNMENT #:",
                Location = new Point(14, curY),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlSidebar.Controls.Add(lblTracking);
            curY += 18;

            txtTracking = new TextBox
            {
                Location = new Point(14, curY),
                Width = 330,
                Font = new Font("Segoe UI", 9F),
                Text = "TRK-" + DateTime.Today.ToString("yyyyMM") + "-001"
            };
            pnlSidebar.Controls.Add(txtTracking);
            curY += 32;

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
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            pnlSidebar.Controls.Add(cmbPrinter);
            curY += 36;

            // Buttons
            btnPreview = new Button
            {
                Text = "Preview Tags",
                Location = new Point(14, curY),
                Width = 160,
                Height = 34,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPreview.FlatAppearance.BorderSize = 0;
            btnPreview.Click += async (s, e) => await LoadAndRenderTagsAsync();
            pnlSidebar.Controls.Add(btnPreview);

            btnPrintDirect = new Button
            {
                Text = "🖨 Print Direct",
                Location = new Point(184, curY),
                Width = 160,
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
                Width = 330,
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
                Text = "Configure shipment details on the left and click 'Preview Tags'.",
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
                foreach (string p in PrinterSettings.InstalledPrinters) cmbPrinter.Items.Add(p);

                if (!string.IsNullOrWhiteSpace(ConfigInfo.ThermalPrinterName) && cmbPrinter.Items.Contains(ConfigInfo.ThermalPrinterName))
                    cmbPrinter.SelectedItem = ConfigInfo.ThermalPrinterName;
                else if (cmbPrinter.Items.Count > 0)
                    cmbPrinter.SelectedIndex = 0;

                // Load Customers
                var accounts = await _chartOfAccountApiService.GetCustomerAccountsAsync();
                _allCustomers = accounts ?? new List<ChartOfAccountHeadDto>();

                try
                {
                    var details = await _customerApiService.GetAsync();
                    _customerDetails = details ?? new List<CustomerDto>();
                }
                catch
                {
                    _customerDetails = new List<CustomerDto>();
                }

                cmbCustomer.DisplayMember = "Title";
                cmbCustomer.ValueMember = "Account";
                cmbCustomer.DataSource = _allCustomers;

                // Initialize WebView2
                string userDataFolder = Path.Combine(Path.GetTempPath(), "RetailSuite", "WebView2_ShipmentTag");
                if (!Directory.Exists(userDataFolder)) Directory.CreateDirectory(userDataFolder);
                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);
                _isWebViewReady = true;

                if (cmbCustomer.Items.Count > 0)
                {
                    cmbCustomer.SelectedIndex = 0;
                    OnCustomerSelected();
                    await LoadAndRenderTagsAsync();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error initializing customer data: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void OnCustomerSelected()
        {
            var cust = cmbCustomer.SelectedItem as ChartOfAccountHeadDto;
            if (cust != null)
            {
                txtName.Text = cust.Title;

                var detail = _customerDetails.FirstOrDefault(c => string.Equals(c.Account, cust.Account, StringComparison.OrdinalIgnoreCase));
                if (detail != null)
                {
                    txtAddress.Text = detail.Address ?? string.Empty;
                    txtPhone.Text = !string.IsNullOrWhiteSpace(detail.Phone1) ? detail.Phone1 : (!string.IsNullOrWhiteSpace(detail.Phone2) ? detail.Phone2 : (detail.SmsNumber ?? string.Empty));
                    txtCity.Text = string.Empty;
                }
                else
                {
                    txtAddress.Text = "Commercial Area";
                    txtPhone.Text = "";
                    txtCity.Text = "Lahore";
                }
            }
        }

        private async Task LoadAndRenderTagsAsync()
        {
            string consigneeName = txtName.Text.Trim();
            if (string.IsNullOrEmpty(consigneeName))
            {
                MessageBox.Show("Please enter consignee name.", "Name Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int totalCartons = (int)nudCartons.Value;
            decimal weight = 0;
            decimal.TryParse(txtWeight.Text, out weight);

            string shipperName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise";
            string shipperAddress = !string.IsNullOrWhiteSpace(CompanyInfo.Address) ? CompanyInfo.Address : "Plot 42-B, Commercial Area, Lahore";
            string shipperPhone = !string.IsNullOrWhiteSpace(CompanyInfo.ContactHead) ? CompanyInfo.ContactHead : CompanyInfo.Cell;

            var baseTag = new ShipmentTagItem
            {
                TrackingNo = txtTracking.Text.Trim(),
                DispatchDate = DateTime.Today,
                ShipperName = shipperName,
                ShipperAddress = shipperAddress,
                ShipperPhone = shipperPhone,
                ConsigneeName = consigneeName,
                ConsigneeAddress = txtAddress.Text.Trim(),
                ConsigneeCity = txtCity.Text.Trim(),
                ConsigneePhone = txtPhone.Text.Trim(),
                CourierService = cmbCourier.Text,
                WeightKg = weight
            };

            var tags = ShipmentTagDataService.GenerateCartonTags(baseTag, totalCartons);
            _currentTags = tags;

            prgLoading.Visible = true;
            lblStatus.Text = string.Format("Generating {0} shipment tag(s) for '{1}'...", totalCartons, consigneeName);

            try
            {
                var doc = new ShipmentTagDocument(tags);
                _currentPdfPath = await doc.GeneratePdfToTempFileAsync();

                webView.CoreWebView2.Navigate(_currentPdfPath);
                webView.Visible = true;
                lblStatus.Text = string.Format("Previewing {0} Shipment Label(s) for '{1}' • Carrier: {2}",
                    totalCartons, consigneeName, baseTag.CourierService);
                lblStatus.ForeColor = Color.FromArgb(30, 41, 59);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error rendering shipment tags: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                prgLoading.Visible = false;
            }
        }

        private async Task PrintDirectAsync()
        {
            if (_currentTags.Count == 0)
            {
                await LoadAndRenderTagsAsync();
                if (_currentTags.Count == 0) return;
            }

            string printer = cmbPrinter.SelectedItem != null ? cmbPrinter.SelectedItem.ToString() : ConfigInfo.ThermalPrinterName;
            var confirm = MessageBox.Show(
                string.Format("Are you sure you want to silently send {0} shipment label tag(s) directly to '{1}'?", _currentTags.Count, printer),
                "Confirm Direct Print",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                prgLoading.Visible = true;
                lblStatus.Text = "Printing shipment tags silently...";

                await Task.Run(() =>
                {
                    var doc = new ShipmentTagDocument(_currentTags);
                    ShipmentTagDocument.PrintDirectToPrinter(doc, printer);
                });

                MessageBox.Show(string.Format("{0} shipment label tag(s) sent directly to '{1}' successfully!", _currentTags.Count, printer), "Print Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblStatus.Text = string.Format("Complete: {0} tags sent to {1}.", _currentTags.Count, printer);
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
