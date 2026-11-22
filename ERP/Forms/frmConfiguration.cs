using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;
using ERP.Services.Offline;

namespace ERP.Forms
{
    public partial class frmConfiguration : Form
    {
        private readonly SettingsApiService _settingsService = new SettingsApiService();
        private CheckBox chkAllowPurchaseSalePriceUpdate;
        private TextBox txtTerminalCode;
        private Label lblOfflineCacheInfo;

        public frmConfiguration()
        {
            InitializeComponent();
            InitializePurchaseSalePriceCheckBox();
            InitializeOfflineTerminalControls();
        }

        private void InitializeOfflineTerminalControls()
        {
            if (chkAllowPurchaseSalePriceUpdate == null || chkAllowPurchaseSalePriceUpdate.Parent == null) return;
            var parent = chkAllowPurchaseSalePriceUpdate.Parent;
            int topY = chkAllowPurchaseSalePriceUpdate.Bottom + 12;

            var lblTerm = new Label
            {
                Text = "Offline POS Terminal Code (2–6 chars, e.g. T01, POS1):",
                AutoSize = true,
                Font = new Font("Segoe UI", 8.75F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(chkAllowPurchaseSalePriceUpdate.Left, topY + 3)
            };

            txtTerminalCode = new TextBox
            {
                Name = "txtTerminalCode",
                Width = 88,
                MaxLength = 6,
                CharacterCasing = CharacterCasing.Upper,
                Text = OfflineVoucherGenerator.GetTerminalCode(),
                Location = new Point(lblTerm.Left + 310, topY)
            };

            lblOfflineCacheInfo = new Label
            {
                Text = "",
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(chkAllowPurchaseSalePriceUpdate.Left, topY + 28)
            };

            parent.Controls.Add(lblTerm);
            parent.Controls.Add(txtTerminalCode);
            parent.Controls.Add(lblOfflineCacheInfo);

            if (parent.Height < lblOfflineCacheInfo.Bottom + 12)
            {
                int extra = (lblOfflineCacheInfo.Bottom + 16) - parent.Height;
                parent.Height += extra;
                this.Height += extra;
            }
        }

        private void InitializePurchaseSalePriceCheckBox()
        {
            if (chkEnableSecondaryQty == null || chkEnableSecondaryQty.Parent == null) return;

            chkAllowPurchaseSalePriceUpdate = new CheckBox
            {
                Name = "chkAllowPurchaseSalePriceUpdate",
                Text = "Allow Updating Sale Prices && Category Rates from Purchase Form",
                AutoSize = true,
                Font = chkEnableSecondaryQty.Font,
                ForeColor = chkEnableSecondaryQty.ForeColor,
                Location = new Point(chkEnableSecondaryQty.Left, chkEnableSecondaryQty.Bottom + 8)
            };
            chkEnableSecondaryQty.Parent.Controls.Add(chkAllowPurchaseSalePriceUpdate);
        }

        private async void frmConfiguration_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. Load installed printers
                foreach (string strPrinter in PrinterSettings.InstalledPrinters)
                {
                    cmbPrinter.Items.Add(strPrinter);
                    cmbA4Printer.Items.Add(strPrinter);
                }

                // Default Bill Format options
                cmbDefaultFormat.Items.Clear();
                cmbDefaultFormat.Items.AddRange(new object[] { "80mm Thermal Receipt", "A5 Sheet (Slip)", "A4 Sheet (Commercial Invoice)" });
                cmbDefaultFormat.SelectedIndex = ConfigInfo.PreferredSaleReceiptLayout == ERP.Reporting.Models.SaleReceiptPrintLayout.Thermal80mm
                    ? 0
                    : (ConfigInfo.PreferredSaleReceiptLayout == ERP.Reporting.Models.SaleReceiptPrintLayout.A5Sheet ? 1 : 2);

                // Thermal Printer selection
                if (!string.IsNullOrWhiteSpace(ConfigInfo.ThermalPrinterName) && cmbPrinter.Items.Contains(ConfigInfo.ThermalPrinterName))
                {
                    cmbPrinter.SelectedItem = ConfigInfo.ThermalPrinterName;
                }
                else if (cmbPrinter.Items.Count > 0)
                {
                    cmbPrinter.SelectedIndex = 0;
                }

                // A4 Printer selection
                if (!string.IsNullOrWhiteSpace(ConfigInfo.A4PrinterName) && cmbA4Printer.Items.Contains(ConfigInfo.A4PrinterName))
                {
                    cmbA4Printer.SelectedItem = ConfigInfo.A4PrinterName;
                }
                else if (cmbA4Printer.Items.Count > 0)
                {
                    try
                    {
                        string defaultSysPrinter = new PrinterSettings().PrinterName;
                        if (!string.IsNullOrWhiteSpace(defaultSysPrinter) && cmbA4Printer.Items.Contains(defaultSysPrinter))
                        {
                            cmbA4Printer.SelectedItem = defaultSysPrinter;
                        }
                        else
                        {
                            cmbA4Printer.SelectedIndex = 0;
                        }
                    }
                    catch
                    {
                        cmbA4Printer.SelectedIndex = 0;
                    }
                }

                // 2. Load Offline Terminal Info
                if (txtTerminalCode != null)
                {
                    txtTerminalCode.Text = OfflineVoucherGenerator.GetTerminalCode();
                }
                if (lblOfflineCacheInfo != null)
                {
                    var cached = OfflineCacheService.LoadCatalog();
                    string syncedText = cached != null
                        ? $"Offline catalog cached: {cached.Items?.Count ?? 0} items (last synced {cached.SyncedAtUtc.ToLocalTime():dd-MMM-yyyy HH:mm})"
                        : "Offline catalog not yet cached (log in online once to sync).";
                    lblOfflineCacheInfo.Text = syncedText;
                }

                // 3. Load Tenant Printer, QR Payment & Inventory settings from API (online only)
                if (ApiSession.IsOfflineMode)
                {
                    lblStatus.Text = "Offline Mode — local printer & terminal settings only";
                }
                else
                {
                    lblStatus.Text = "Loading settings...";
                    await LoadTenantPrinterSettingsAsync();
                    await LoadQrSettingsAsync();
                    await LoadInventorySettingsAsync();
                    await LoadBarcodeSettingsAsync();
                    lblStatus.Text = "Ready";
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async Task LoadTenantPrinterSettingsAsync()
        {
            try
            {
                string tenantFormat = await _settingsService.GetSettingValueAsync("Bill.DefaultFormat");
                if (!string.IsNullOrWhiteSpace(tenantFormat))
                {
                    ConfigInfo.DefaultBillFormat = tenantFormat;
                    cmbDefaultFormat.SelectedIndex = tenantFormat.IndexOf("thermal", StringComparison.OrdinalIgnoreCase) >= 0
                        ? 0
                        : (tenantFormat.IndexOf("a5", StringComparison.OrdinalIgnoreCase) >= 0 ? 1 : 2);
                }

                string tenantThermal = await _settingsService.GetSettingValueAsync("Printer.ThermalPrinter");
                if (!string.IsNullOrWhiteSpace(tenantThermal))
                {
                    ConfigInfo.ThermalPrinterName = tenantThermal;
                    if (cmbPrinter.Items.Contains(tenantThermal))
                    {
                        cmbPrinter.SelectedItem = tenantThermal;
                    }
                }

                string tenantA4 = await _settingsService.GetSettingValueAsync("Printer.A4Printer");
                if (!string.IsNullOrWhiteSpace(tenantA4))
                {
                    ConfigInfo.A4PrinterName = tenantA4;
                    if (cmbA4Printer.Items.Contains(tenantA4))
                    {
                        cmbA4Printer.SelectedItem = tenantA4;
                    }
                }
            }
            catch { }
        }

        private async Task LoadInventorySettingsAsync()
        {
            try
            {
                string secQtyStr = await _settingsService.GetSettingValueAsync("Inventory.EnableSecondaryQty");
                if (!string.IsNullOrWhiteSpace(secQtyStr))
                {
                    chkEnableSecondaryQty.Checked = string.Equals(secQtyStr, "true", StringComparison.OrdinalIgnoreCase);
                }

                if (chkAllowPurchaseSalePriceUpdate != null)
                {
                    string purchasePriceStr = await _settingsService.GetSettingValueAsync("Purchase.AllowSalePriceUpdate");
                    chkAllowPurchaseSalePriceUpdate.Checked = string.Equals(purchasePriceStr, "true", StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    chkEnableSecondaryQty.Checked = ApiSession.HasSecondaryQty;
                }
            }
            catch
            {
                chkEnableSecondaryQty.Checked = ApiSession.HasSecondaryQty;
            }
        }

        private async Task LoadBarcodeSettingsAsync()
        {
            try
            {
                string enabledStr = await _settingsService.GetSettingValueAsync("Barcode.FastScanEnabled");
                string incStr = await _settingsService.GetSettingValueAsync("Barcode.FastScanIncrement");
                string soundStr = await _settingsService.GetSettingValueAsync("Barcode.FastScanSound");

                chkEnableBarcodeFastScan.Checked = string.Equals(enabledStr, "true", StringComparison.OrdinalIgnoreCase);
                chkFastScanAutoIncrement.Checked = string.IsNullOrWhiteSpace(incStr) || !string.Equals(incStr, "false", StringComparison.OrdinalIgnoreCase);
                chkFastScanSound.Checked = string.IsNullOrWhiteSpace(soundStr) || !string.Equals(soundStr, "false", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                chkEnableBarcodeFastScan.Checked = ConfigInfo.IsBarcodeFastScanEnabled;
                chkFastScanAutoIncrement.Checked = ConfigInfo.FastScanAutoIncrement;
                chkFastScanSound.Checked = ConfigInfo.FastScanSound;
            }
        }

        private void txtScannerTest_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string scanned = txtScannerTest.Text.Trim();
                if (!string.IsNullOrEmpty(scanned))
                {
                    if (chkFastScanSound.Checked)
                    {
                        ScannerToneHelper.PlaySuccess();
                    }
                    lblTestResult.ForeColor = Color.Green;
                    lblTestResult.Text = "Scanned: " + scanned + " (Tone played!)";
                    txtScannerTest.Clear();
                }
            }
        }

        private async Task LoadQrSettingsAsync()
        {
            try
            {
                string enabledStr = await _settingsService.GetSettingValueAsync("Bill.QrPayment.Enabled");
                string bankName = await _settingsService.GetSettingValueAsync("Bill.QrPayment.BankName");
                string accountTitle = await _settingsService.GetSettingValueAsync("Bill.QrPayment.AccountTitle");
                string accountNumber = await _settingsService.GetSettingValueAsync("Bill.QrPayment.AccountNumber");

                chkQrEnabled.Checked = string.Equals(enabledStr, "true", StringComparison.OrdinalIgnoreCase);
                txtBankName.Text = bankName ?? string.Empty;
                txtAccountTitle.Text = accountTitle ?? string.Empty;
                txtAccountNumber.Text = accountNumber ?? string.Empty;

                ToggleQrFields(chkQrEnabled.Checked);
                UpdateQrPreview();
            }
            catch
            {
                chkQrEnabled.Checked = false;
                ToggleQrFields(false);
            }
        }

        private void ToggleQrFields(bool enabled)
        {
            txtBankName.Enabled = enabled;
            txtAccountTitle.Enabled = enabled;
            txtAccountNumber.Enabled = enabled;
        }

        private void chkQrEnabled_CheckedChanged(object sender, EventArgs e)
        {
            ToggleQrFields(chkQrEnabled.Checked);
            UpdateQrPreview();
        }

        private void OnQrFieldChanged(object sender, EventArgs e)
        {
            UpdateQrPreview();
        }

        private void UpdateQrPreview()
        {
            try
            {
                if (!chkQrEnabled.Checked || string.IsNullOrWhiteSpace(txtAccountNumber.Text))
                {
                    if (picQrPreview.Image != null)
                    {
                        var oldImg = picQrPreview.Image;
                        picQrPreview.Image = null;
                        oldImg.Dispose();
                    }
                    return;
                }

                var qrInfo = new QrPaymentInfo
                {
                    IsEnabled = true,
                    BankName = txtBankName.Text.Trim(),
                    AccountTitle = txtAccountTitle.Text.Trim(),
                    AccountNumber = txtAccountNumber.Text.Trim()
                };

                // Build EMVCo payload with sample test amount PKR 1,000 for preview
                string payload = qrInfo.BuildEmvCoPayload(1000m);
                byte[] pngBytes = QrCodeHelper.GeneratePng(payload, 5);

                if (pngBytes != null && pngBytes.Length > 0)
                {
                    using (var ms = new MemoryStream(pngBytes))
                    {
                        var newImg = Image.FromStream(ms);
                        var oldImg = picQrPreview.Image;
                        picQrPreview.Image = new Bitmap(newImg);
                        oldImg?.Dispose();
                    }
                }

                string iban = QrPaymentInfo.NormalizeToIban(txtAccountNumber.Text.Trim(), txtBankName.Text.Trim());
                if (!string.IsNullOrEmpty(iban))
                {
                    lblPreviewAmount.Text = "Raast IBAN: " + iban;
                }
            }
            catch
            {
                // Ignored in preview
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = false;
                lblStatus.Text = "Saving configuration...";

                // 1. Save Thermal Printer, A4 Printer, and Default Bill Format to INI and ConfigInfo
                string selectedThermal = cmbPrinter.SelectedItem != null ? cmbPrinter.SelectedItem.ToString() : "";
                if (!string.IsNullOrWhiteSpace(selectedThermal))
                {
                    INIFile.WriteValue("PrinterSetting", "ThermalPrinter", selectedThermal);
                    ConfigInfo.ThermalPrinterName = selectedThermal;
                }

                string selectedA4 = cmbA4Printer.SelectedItem != null ? cmbA4Printer.SelectedItem.ToString() : "";
                if (!string.IsNullOrWhiteSpace(selectedA4))
                {
                    INIFile.WriteValue("PrinterSetting", "A4Printer", selectedA4);
                    ConfigInfo.A4PrinterName = selectedA4;
                }

                string selectedFormat = cmbDefaultFormat.SelectedIndex == 0
                    ? "80mm Thermal"
                    : (cmbDefaultFormat.SelectedIndex == 1 ? "A5 Sheet" : "A4 Sheet");
                INIFile.WriteValue("PrinterSetting", "DefaultBillFormat", selectedFormat);
                ConfigInfo.DefaultBillFormat = selectedFormat;

                // 2. Save Settings to API
                var settingsToSave = new List<SettingItemDto>
                {
                    new SettingItemDto
                    {
                        Key = "Printer.ThermalPrinter",
                        Value = selectedThermal,
                        Description = "Thermal Receipt Printer Name",
                        Category = "Printer"
                    },
                    new SettingItemDto
                    {
                        Key = "Printer.A4Printer",
                        Value = selectedA4,
                        Description = "A4 Standard Invoice Printer Name",
                        Category = "Printer"
                    },
                    new SettingItemDto
                    {
                        Key = "Bill.DefaultFormat",
                        Value = selectedFormat,
                        Description = "Default Bill Print Format (80mm Thermal, A5 Sheet, or A4 Sheet)",
                        Category = "Bill"
                    },
                    new SettingItemDto
                    {
                        Key = "Bill.QrPayment.Enabled",
                        Value = chkQrEnabled.Checked ? "true" : "false",
                        Description = "Enable QR Payment on bills",
                        Category = "Bill.QrPayment"
                    },
                    new SettingItemDto
                    {
                        Key = "Bill.QrPayment.BankName",
                        Value = txtBankName.Text.Trim(),
                        Description = "Bank or wallet name for QR payment label",
                        Category = "Bill.QrPayment"
                    },
                    new SettingItemDto
                    {
                        Key = "Bill.QrPayment.AccountTitle",
                        Value = txtAccountTitle.Text.Trim(),
                        Description = "Merchant / account title for QR payment",
                        Category = "Bill.QrPayment"
                    },
                    new SettingItemDto
                    {
                        Key = "Bill.QrPayment.AccountNumber",
                        Value = txtAccountNumber.Text.Trim(),
                        Description = "IBAN or RAAST Alias for QR payment",
                        Category = "Bill.QrPayment"
                    },
                    new SettingItemDto
                    {
                        Key = "Inventory.EnableSecondaryQty",
                        Value = chkEnableSecondaryQty.Checked ? "true" : "false",
                        Description = "Enable secondary quantity (pack/single) on transaction forms",
                        Category = "Inventory"
                    },
                    new SettingItemDto
                    {
                        Key = "Purchase.AllowSalePriceUpdate",
                        Value = (chkAllowPurchaseSalePriceUpdate != null && chkAllowPurchaseSalePriceUpdate.Checked) ? "true" : "false",
                        Description = "Allow updating item sale prices and customer category rates from Purchase forms",
                        Category = "Transaction"
                    },
                    new SettingItemDto
                    {
                        Key = "Barcode.FastScanEnabled",
                        Value = chkEnableBarcodeFastScan.Checked ? "true" : "false",
                        Description = "Enable Barcode Fast Scanning on Transaction Grids",
                        Category = "Barcode"
                    },
                    new SettingItemDto
                    {
                        Key = "Barcode.FastScanIncrement",
                        Value = chkFastScanAutoIncrement.Checked ? "true" : "false",
                        Description = "Auto-Increment Quantity When Scanning Duplicate Items",
                        Category = "Barcode"
                    },
                    new SettingItemDto
                    {
                        Key = "Barcode.FastScanSound",
                        Value = chkFastScanSound.Checked ? "true" : "false",
                        Description = "Play Audio Tone on Successful Barcode Scan",
                        Category = "Barcode"
                    }
                };

                // Always persist Offline Terminal Code (server-validated when online to guarantee uniqueness)
                if (txtTerminalCode != null && !string.IsNullOrWhiteSpace(txtTerminalCode.Text))
                {
                    if (!ApiSession.IsOfflineMode && !string.IsNullOrWhiteSpace(ApiSession.AccessToken))
                    {
                        string registered = await OfflineVoucherGenerator.RegisterTerminalWithServerAsync(txtTerminalCode.Text);
                        txtTerminalCode.Text = registered;
                    }
                    else
                    {
                        OfflineVoucherGenerator.SaveTerminalCode(txtTerminalCode.Text);
                        txtTerminalCode.Text = OfflineVoucherGenerator.GetTerminalCode();
                    }
                }

                if (ApiSession.IsOfflineMode)
                {
                    OfflineCacheService.SaveCompanyAndSettingsSnapshot();
                    lblStatus.Text = "Saved locally (Offline Mode) at " + DateTime.Now.ToString("HH:mm:ss");
                    MessageBox.Show("Local printer & offline terminal settings saved!", "Configuration (Offline)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                bool apiSuccess = await _settingsService.BatchUpsertAsync(settingsToSave);

                // Update active session flag immediately
                ApiSession.HasSecondaryQty = chkEnableSecondaryQty.Checked;
                if (chkAllowPurchaseSalePriceUpdate != null)
                    ApiSession.AllowSalePriceUpdateInPurchase = chkAllowPurchaseSalePriceUpdate.Checked;
                ConfigInfo.IsBarcodeFastScanEnabled = chkEnableBarcodeFastScan.Checked;
                ConfigInfo.FastScanAutoIncrement = chkFastScanAutoIncrement.Checked;
                ConfigInfo.FastScanSound = chkFastScanSound.Checked;

                // Clear cached QrPaymentInfo so new bills immediately use updated settings
                QrPaymentInfo.ClearCache();

                lblStatus.Text = apiSuccess ? "Saved successfully at " + DateTime.Now.ToString("HH:mm:ss") : "Saved locally (API sync issue)";

                MessageBox.Show(
                    apiSuccess
                        ? "Configuration and settings saved successfully!"
                        : "Printer setting saved, but failed to sync settings with the server. Please check your network connection.",
                    "Configuration",
                    MessageBoxButtons.OK,
                    apiSuccess ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Save failed: " + ex.Message;
                MessageBox.Show("Error saving configuration: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
