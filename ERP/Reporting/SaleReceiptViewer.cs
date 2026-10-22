using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Reporting.Documents;
using ERP.Reporting.Models;
using Microsoft.Web.WebView2.WinForms;

namespace ERP.Reporting
{
    /// <summary>
    /// Modern, code-first WinForms receipt viewer hosting WebView2 with QuestPDF rendering.
    /// Provides instant switching between 80mm Continuous Thermal Receipt, A5, and A4 invoice formats.
    /// </summary>
    public class SaleReceiptViewer : Form
    {
        private readonly SaleReceiptDataResult _dataResult;
        private SaleReceiptPrintLayout _layout;
        private string _currentPdfPath;
        private bool _isWebViewReady = false;

        // UI Controls
        private Panel pnlTopBar;
        private Label lblTitle;
        private Label lblLayout;
        private ComboBox cmbLayout;
        private Button btnDirectPrint;
        private Button btnPrintDialog;
        private Button btnSavePdf;
        private Label lblStatus;
        private ProgressBar prgLoading;
        private WebView2 webView;

        public SaleReceiptViewer(SaleReceiptDataResult dataResult, SaleReceiptPrintLayout initialLayout = SaleReceiptPrintLayout.Thermal80mm)
        {
            _dataResult = dataResult;
            _layout = initialLayout;
            InitializeComponentCodeFirst();
        }

        private void InitializeComponentCodeFirst()
        {
            this.Text = "Sale Receipt - " + (_dataResult?.Summary?.VoucherNo ?? "Voucher");
            this.Size = new Size(950, 750);
            this.MinimumSize = new Size(800, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.BackColor = Color.FromArgb(248, 250, 252);

            // 1. Top Action Toolbar
            pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.White,
                Padding = new Padding(12, 10, 12, 10)
            };
            pnlTopBar.Paint += (s, pe) =>
            {
                pe.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
            };

            lblTitle = new Label
            {
                Text = _dataResult?.Summary?.VoucherNo ?? "Sale Receipt",
                Location = new Point(14, 16),
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };

            lblLayout = new Label
            {
                Text = "Format:",
                Location = new Point(170, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            cmbLayout = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(220, 14),
                Size = new Size(160, 28),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular)
            };
            cmbLayout.Items.AddRange(new object[] { "80mm Thermal Slip", "A5 Sheet Invoice", "A4 Sheet Invoice" });
            cmbLayout.SelectedIndex = _layout == SaleReceiptPrintLayout.Thermal80mm ? 0 : (_layout == SaleReceiptPrintLayout.A5Sheet ? 1 : 2);
            cmbLayout.SelectedIndexChanged += async (s, e) =>
            {
                if (cmbLayout.SelectedIndex == 0) _layout = SaleReceiptPrintLayout.Thermal80mm;
                else if (cmbLayout.SelectedIndex == 1) _layout = SaleReceiptPrintLayout.A5Sheet;
                else _layout = SaleReceiptPrintLayout.A4Sheet;

                await RenderPdfAsync();
            };

            btnDirectPrint = new Button
            {
                Text = "Quick Print",
                Location = new Point(395, 12),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDirectPrint.FlatAppearance.BorderSize = 0;
            btnDirectPrint.Click += async (s, e) =>
            {
                try
                {
                    btnDirectPrint.Enabled = false;
                    lblStatus.Text = "Sending directly to printer...";
                    var doc = new SaleReceiptDocument(_dataResult.Summary, _dataResult.Lines, _layout);
                    string printer = (_layout == SaleReceiptPrintLayout.Thermal80mm && !string.IsNullOrWhiteSpace(ConfigInfo.ThermalPrinterName))
                        ? ConfigInfo.ThermalPrinterName
                        : null;
                    await Task.Run(() => SaleReceiptDocument.PrintDirectToPrinter(doc, printer));
                    lblStatus.Text = "Printed successfully.";
                    MessageBox.Show("Receipt sent directly to printer.", "Print Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Print error.";
                    MessageBox.Show("Direct print failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnDirectPrint.Enabled = true;
                }
            };

            btnPrintDialog = new Button
            {
                Text = "Print...",
                Location = new Point(505, 12),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnPrintDialog.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnPrintDialog.Click += (s, e) =>
            {
                if (webView != null && webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.ShowPrintUI(Microsoft.Web.WebView2.Core.CoreWebView2PrintDialogKind.Browser);
                }
            };

            btnSavePdf = new Button
            {
                Text = "Save PDF",
                Location = new Point(595, 12),
                Size = new Size(85, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnSavePdf.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnSavePdf.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(_currentPdfPath) || !File.Exists(_currentPdfPath)) return;
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "PDF Document (*.pdf)|*.pdf";
                    sfd.FileName = string.Format("Receipt_{0}.pdf", _dataResult?.Summary?.VoucherNo?.Replace("-", "_") ?? "Sale");
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        File.Copy(_currentPdfPath, sfd.FileName, true);
                        MessageBox.Show("PDF saved successfully to:\n" + sfd.FileName, "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };

            lblStatus = new Label
            {
                Text = "Ready",
                Location = new Point(695, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            prgLoading = new ProgressBar
            {
                Location = new Point(810, 20),
                Size = new Size(90, 12),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            pnlTopBar.Controls.Add(lblTitle);
            pnlTopBar.Controls.Add(lblLayout);
            pnlTopBar.Controls.Add(cmbLayout);
            pnlTopBar.Controls.Add(btnDirectPrint);
            pnlTopBar.Controls.Add(btnPrintDialog);
            pnlTopBar.Controls.Add(btnSavePdf);
            pnlTopBar.Controls.Add(lblStatus);
            pnlTopBar.Controls.Add(prgLoading);

            // 2. WebView2 Viewer Control
            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(webView);
            this.Controls.Add(pnlTopBar);

            this.Load += async (s, e) =>
            {
                await InitializeWebView2Async();
                await RenderPdfAsync();
            };
        }

        private async Task InitializeWebView2Async()
        {
            try
            {
                prgLoading.Visible = true;
                lblStatus.Text = "Initializing viewer...";
                string userDataFolder = Path.Combine(Path.GetTempPath(), "RetailSuite_WebView2");
                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);
                _isWebViewReady = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Viewer init failed.";
                System.Diagnostics.Debug.WriteLine("WebView2 Init Error: " + ex.Message);
            }
            finally
            {
                prgLoading.Visible = false;
            }
        }

        private async Task RenderPdfAsync()
        {
            try
            {
                prgLoading.Visible = true;
                lblStatus.Text = "Rendering QuestPDF receipt...";

                var doc = new SaleReceiptDocument(_dataResult.Summary, _dataResult.Lines, _layout);
                _currentPdfPath = await doc.GeneratePdfToTempFileAsync();

                if (_isWebViewReady && webView.CoreWebView2 != null && File.Exists(_currentPdfPath))
                {
                    webView.CoreWebView2.Navigate(new Uri(_currentPdfPath).AbsoluteUri);
                }
                else if (File.Exists(_currentPdfPath))
                {
                    // Fallback to opening system default viewer if WebView2 failed to initialize
                    System.Diagnostics.Process.Start(_currentPdfPath);
                }

                lblStatus.Text = "Receipt ready.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Render error.";
                MessageBox.Show("Failed to render QuestPDF receipt: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                prgLoading.Visible = false;
            }
        }

        /// <summary>
        /// Central helper to print or preview any sale receipt across Desktop using QuestPDF.
        /// </summary>
        public static async Task PrintOrPreviewAsync(string voucherNo, bool isDirect, SaleReceiptPrintLayout? preferredLayout = null, bool isDuplicate = false)
        {
            if (string.IsNullOrWhiteSpace(voucherNo)) return;
            string cleanVoucher = voucherNo.StartsWith("SL-", StringComparison.OrdinalIgnoreCase) ? voucherNo.Substring(3) : voucherNo;

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                DataSet ds = await Task.Run(() => ReportQuery.SaleBill2(cleanVoucher));
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("No transaction details found for sale voucher #" + voucherNo, "Receipt Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dataResult = SaleReceiptDataResult.FromDataSet(ds, voucherNo, isDuplicate);
                var layout = preferredLayout ?? (ConfigInfo.IsThermalDefault ? SaleReceiptPrintLayout.Thermal80mm : SaleReceiptPrintLayout.A5Sheet);

                if (isDirect)
                {
                    var doc = new SaleReceiptDocument(dataResult.Summary, dataResult.Lines, layout);
                    string printer = (layout == SaleReceiptPrintLayout.Thermal80mm && !string.IsNullOrWhiteSpace(ConfigInfo.ThermalPrinterName))
                        ? ConfigInfo.ThermalPrinterName
                        : null;

                    await Task.Run(() => SaleReceiptDocument.PrintDirectToPrinter(doc, printer));
                }
                else
                {
                    var viewer = new SaleReceiptViewer(dataResult, layout);
                    viewer.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating QuestPDF sale receipt: " + ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
