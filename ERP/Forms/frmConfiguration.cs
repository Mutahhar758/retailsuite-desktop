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
                }

                if (!string.IsNullOrWhiteSpace(ConfigInfo.ThermalPrinterName) && cmbPrinter.Items.Contains(ConfigInfo.ThermalPrinterName))
                {
                    cmbPrinter.SelectedItem = ConfigInfo.ThermalPrinterName;
                }
                else if (cmbPrinter.Items.Count > 0)
                {
                    cmbPrinter.SelectedIndex = 0;
                }

                // 2. Load QR Payment settings from API
                lblStatus.Text = "Loading settings...";
                await LoadQrSettingsAsync();
                lblStatus.Text = "Ready";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading settings: " + ex.Message;
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

                // 1. Save Thermal Printer to INI
                if (cmbPrinter.SelectedItem != null)
                {
                    string selectedPrinter = cmbPrinter.SelectedItem.ToString();
                    INIFile.WriteValue("PrinterSetting", "ThermalPrinter", selectedPrinter);
                    ConfigInfo.ThermalPrinterName = selectedPrinter;
                }

                // 2. Save QR Payment settings to API
                var settingsToSave = new List<SettingItemDto>
                {
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
                    }
                };

                bool apiSuccess = await _settingsService.BatchUpsertAsync(settingsToSave);

                // Clear cached QrPaymentInfo so new bills immediately use updated settings
                QrPaymentInfo.ClearCache();

                lblStatus.Text = apiSuccess ? "Saved successfully at " + DateTime.Now.ToString("HH:mm:ss") : "Saved locally (API sync issue)";

                MessageBox.Show(
                    apiSuccess
                        ? "Configuration and QR payment settings saved successfully!"
                        : "Printer setting saved, but failed to sync QR settings with the server. Please check your network connection.",
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
