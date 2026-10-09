using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP.Forms
{
    public partial class frmConfiguration : Form
    {
        private readonly SettingsApiService _settingsService = new SettingsApiService();

        public frmConfiguration()
        {
            InitializeComponent();
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
                cmbDefaultFormat.Items.AddRange(new object[] { "80mm Thermal Receipt", "A4 Sheet (Commercial Invoice)" });
                if (ConfigInfo.IsThermalDefault)
                {
                    cmbDefaultFormat.SelectedIndex = 0;
                }
                else
                {
                    cmbDefaultFormat.SelectedIndex = 1;
                }

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

                // 2. Load Tenant Printer, QR Payment & Inventory settings from API
                lblStatus.Text = "Loading settings...";
                await LoadTenantPrinterSettingsAsync();
                await LoadQrSettingsAsync();
                await LoadInventorySettingsAsync();
                lblStatus.Text = "Ready";
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
                    bool isThermal = tenantFormat.IndexOf("thermal", StringComparison.OrdinalIgnoreCase) >= 0;
                    cmbDefaultFormat.SelectedIndex = isThermal ? 0 : 1;
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

                string selectedFormat = (cmbDefaultFormat.SelectedIndex == 0) ? "80mm Thermal" : "A4 Sheet";
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
                        Description = "Default Bill Print Format (80mm Thermal or A4 Sheet)",
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
                    }
                };

                bool apiSuccess = await _settingsService.BatchUpsertAsync(settingsToSave);

                // Update active session flag immediately
                ApiSession.HasSecondaryQty = chkEnableSecondaryQty.Checked;

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
