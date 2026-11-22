using System;
using System.Drawing;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Dtos.Authentication;
using ERP.Services.Authentication;
using ERP.Services.Legacy;
using ERP.Services.Offline;

namespace ERP
{
    public partial class frmLogIn : Form
    {
        private LoginService _loginService;
        private readonly LicenseService _licenseService;
        private CompanyApiService _companyApiService;
        private PersonalApiService _personalApiService;
        private SettingsApiService _settingsApiService;
        private Button btnOfflinePos;
        private Timer _tmrServerCheck;
        private bool _isCheckingServer;

        public frmLogIn()
        {
            InitializeComponent();
            InitializeOfflinePosButton();
            _licenseService = new LicenseService();
            _loginService = new LoginService();
            _companyApiService = new CompanyApiService();
            _personalApiService = new PersonalApiService();
            _settingsApiService = new SettingsApiService();
            
            // Ensure icon is set
            pictureBox1.Image = global::ERP.Properties.Resources.if_lock_318582;

            // Interactive hover styling
            btnLogIn.MouseEnter += (s, e) => btnLogIn.BackColor = System.Drawing.Color.FromArgb(29, 78, 216);
            btnLogIn.MouseLeave += (s, e) => btnLogIn.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            btnAddOrganization.MouseEnter += (s, e) => btnAddOrganization.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            btnAddOrganization.MouseLeave += (s, e) => btnAddOrganization.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
        }

        private void InitializeOfflinePosButton()
        {
            if (btnLogIn == null || btnLogIn.Parent == null) return;

            btnOfflinePos = new Button
            {
                Name = "btnOfflinePos",
                Text = "Offline POS Mode",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(254, 243, 199), // Amber-100
                ForeColor = Color.FromArgb(146, 64, 14),   // Amber-800
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Height = 32,
                Width = Math.Max(btnClose.Right - btnLogIn.Left, btnLogIn.Width),
                Location = new Point(btnLogIn.Left, btnLogIn.Bottom + 10),
                Enabled = false,
                Visible = false
            };
            btnOfflinePos.FlatAppearance.BorderColor = Color.FromArgb(245, 158, 11);
            btnOfflinePos.FlatAppearance.BorderSize = 1;
            btnOfflinePos.MouseEnter += (s, e) => { if (btnOfflinePos.Enabled) btnOfflinePos.BackColor = Color.FromArgb(253, 230, 138); };
            btnOfflinePos.MouseLeave += (s, e) => { if (btnOfflinePos.Enabled) btnOfflinePos.BackColor = Color.FromArgb(254, 243, 199); };
            btnOfflinePos.Click += btnOfflinePos_Click;

            btnLogIn.Parent.Controls.Add(btnOfflinePos);
            RefreshTerminalLabel();

            _tmrServerCheck = new Timer { Interval = 10000 };
            _tmrServerCheck.Tick += async (s, e) => await UpdateOfflineButtonAvailabilityAsync();
            this.FormClosed += (s, e) => { try { _tmrServerCheck?.Stop(); _tmrServerCheck?.Dispose(); } catch { } };
        }

        private string GetTerminalDisplayText()
        {
            return OfflineVoucherGenerator.HasAssignedTerminalCode()
                ? OfflineVoucherGenerator.GetTerminalCode()
                : "(Assigns on first online login)";
        }

        private void RefreshTerminalLabel()
        {
            if (textBox1 != null && !textBox1.IsDisposed)
                textBox1.Text = GetTerminalDisplayText();
        }

        private async System.Threading.Tasks.Task UpdateOfflineButtonAvailabilityAsync()
        {
            if (_isCheckingServer || this.IsDisposed || btnOfflinePos == null) return;
            _isCheckingServer = true;
            try
            {
                bool reachable = await OfflineSyncService.IsApiReachableAsync(3);
                if (this.IsDisposed) return;

                SetOfflineButtonState(!reachable);
            }
            catch
            {
                if (!this.IsDisposed) SetOfflineButtonState(true);
            }
            finally
            {
                _isCheckingServer = false;
            }
        }

        private void SetOfflineButtonState(bool serverUnreachable)
        {
            if (btnOfflinePos == null || btnOfflinePos.IsDisposed) return;
            btnOfflinePos.Visible = serverUnreachable;
            btnOfflinePos.Enabled = serverUnreachable;
            if (serverUnreachable && this.ClientSize.Height < btnOfflinePos.Bottom + 24)
            {
                this.ClientSize = new Size(this.ClientSize.Width, btnOfflinePos.Bottom + 28);
            }
        }

        private void btnOfflinePos_Click(object sender, EventArgs e)
        {
            StartOfflinePosMode(txtUserId.Text.Trim());
        }

        private void StartOfflinePosMode(string cashierName)
        {
            string tenantId = ApiSession.TenantIdentifier;
            if (string.IsNullOrWhiteSpace(tenantId) && cmbOrganization.SelectedItem is OrganizationLicense lic)
            {
                tenantId = lic.TenantIdentifier;
            }

            if (string.IsNullOrWhiteSpace(cashierName))
            {
                cashierName = PromptForCashierName();
                if (string.IsNullOrWhiteSpace(cashierName))
                    return;
            }

            if (!OfflineCacheService.TryStartOfflinePosSession(tenantId, cashierName, out string err))
            {
                MessageBox.Show(err, "Offline POS Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            frmMainMenu frm = new frmMainMenu();
            frm.Owner = this;
            Hide();
            frm.ShowDialog();
            Close();
        }

        private static string PromptForCashierName()
        {
            using (var dlg = new Form())
            {
                dlg.Text = "Offline POS — Cashier Name";
                dlg.Size = new Size(360, 165);
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.Font = new Font("Segoe UI", 9F);

                var lbl = new Label { Text = "Enter cashier display name for offline receipts:", Location = new Point(14, 14), AutoSize = true };
                var txt = new TextBox { Location = new Point(14, 40), Width = 315, Text = "Cashier" };
                var btnOk = new Button { Text = "Start Offline POS", DialogResult = DialogResult.OK, Location = new Point(135, 80), Width = 115, Height = 30 };
                var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(255, 80), Width = 74, Height = 30 };

                dlg.AcceptButton = btnOk;
                dlg.CancelButton = btnCancel;
                dlg.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });

                return dlg.ShowDialog() == DialogResult.OK ? txt.Text.Trim() : null;
            }
        }

        void INISetting()
        {
            string iniPath = INIFile.GetAppDataFilePath("Settings.ini", true);
            INIFile MyINI = new INIFile(iniPath);
        }

        private void frmLogIn_Load(object sender, EventArgs e)
        {
            INISetting();
            LoadOrganizations();

            // Always show organization controls so user can see them and add more
            lblOrganization.Visible = true;
            cmbOrganization.Visible = true;
            btnAddOrganization.Visible = true;

            if (!_licenseService.HasAnyLicense())
            {
                using (var licenseForm = new Forms.frmLicense())
                {
                    if (licenseForm.ShowDialog() != DialogResult.OK)
                    {
                        Application.Exit();
                        return;
                    }

                    LoadOrganizations();
                }
            }

            UserInfo.UserId = "Log Out";
            _tmrServerCheck?.Start();
            _ = UpdateOfflineButtonAvailabilityAsync();
        }

        private async void btnLogIn_Click(object sender, EventArgs e)
        {
            string username = txtUserId.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter your username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserId.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            progressBar1.Visible = true;
            btnLogIn.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                LoginResultDto result = await _loginService.LoginAsync(username, password);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ApiSession.AccessToken = result.TokenResponse.Token;
                ApiSession.RefreshToken = result.TokenResponse.RefreshToken;
                ApiSession.RefreshTokenExpiryTime = result.TokenResponse.RefreshTokenExpiryTime;

                try
                {
                    var features = await _licenseService.GetFeaturesAsync();
                    ApiSession.HasSupplyFeature = features.HasSupplyFeature;
                    ApiSession.HasSecondaryQty = features.HasSecondaryQty;
                    ApiSession.HasVariablePackFeature = features.HasVariablePackFeature;
                    ApiSession.HasMobileShopFeature = features.HasMobileShopFeature;
                    _licenseService.UpdateFeaturesInStore(ApiSession.TenantIdentifier, features.HasSupplyFeature, features.HasSecondaryQty, features.HasVariablePackFeature, features.HasMobileShopFeature);
                }
                catch { }

                try
                {
                    var profile = await _personalApiService.GetProfileAsync();
                    UserInfo.UserId = string.IsNullOrWhiteSpace(profile.UserName) ? username : profile.UserName;
                    UserInfo.UserName = string.IsNullOrWhiteSpace(profile.UserName) ? username : profile.UserName;
                    UserInfo.Email = profile.Email;
                    UserInfo.IsOwner = profile.IsOwner;
                    ApiSession.UserEmail = profile.Email;

                    try
                    {
                        UserInfo.Permissions = await _personalApiService.GetPermissionsAsync();
                    }
                    catch (System.Exception exPerm)
                    {
                        UserInfo.Permissions = new System.Collections.Generic.List<string>();
                        MessageBox.Show("Warning: Could not load user permissions: " + exPerm.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    UserInfo.UserId = username;
                    UserInfo.UserName = username;
                    UserInfo.Email = string.Empty;
                    UserInfo.IsOwner = false;
                    UserInfo.Permissions = new System.Collections.Generic.List<string>();
                    ApiSession.UserEmail = string.Empty;
                    MessageBox.Show("Warning: Could not load personal info from API: " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                try
                {
                    var company = await _companyApiService.GetCurrentAsync();
                    CompanyInfo.CompanyName = company.CompanyName;
                    CompanyInfo.UrCompanyName = company.UrCompanyName;
                    CompanyInfo.Description = company.Descr;
                    CompanyInfo.Address = company.Address;
                    CompanyInfo.Phone = company.Phone;
                    CompanyInfo.Cell = company.Cell;
                    CompanyInfo.Cell2 = company.Cell2;
                    CompanyInfo.ContactHead = company.ContactHeader ?? "";
                    CompanyInfo.Email = company.Email;
                    CompanyInfo.Website = company.Website;
                    CompanyInfo.Ntn = company.Ntn;
                    CompanyInfo.Strn = company.Strn;
                    CompanyInfo.LogoMediaId = company.LogoMediaId;
                    CompanyInfo.LogoUrl = company.LogoUrl;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Warning: Could not load company info from API: " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                try
                {
                    ConfigInfo.Reset();

                    var thankYou = await _settingsApiService.GetSettingValueAsync("Bill.ThankYouMessage");
                    if (!string.IsNullOrWhiteSpace(thankYou))
                    {
                        ConfigInfo.ThankyouLine = thankYou;
                    }

                    var secQtySetting = await _settingsApiService.GetSettingValueAsync("Inventory.EnableSecondaryQty");
                    if (!string.IsNullOrWhiteSpace(secQtySetting))
                    {
                        ApiSession.HasSecondaryQty = string.Equals(secQtySetting, "true", StringComparison.OrdinalIgnoreCase);
                    }

                    var supplySetting = await _settingsApiService.GetSettingValueAsync("Supply.EnableSupply");
                    if (!string.IsNullOrWhiteSpace(supplySetting))
                    {
                        ApiSession.HasSupplyFeature = string.Equals(supplySetting, "true", StringComparison.OrdinalIgnoreCase);
                    }

                    var carriageSetting = await _settingsApiService.GetSettingValueAsync("Transaction.EnableCarriage");
                    if (!string.IsNullOrWhiteSpace(carriageSetting))
                    {
                        ApiSession.EnableCarriage = string.Equals(carriageSetting, "true", StringComparison.OrdinalIgnoreCase);
                    }

                    var purchaseSalePriceSetting = await _settingsApiService.GetSettingValueAsync("Purchase.AllowSalePriceUpdate");
                    ApiSession.AllowSalePriceUpdateInPurchase = string.Equals(purchaseSalePriceSetting, "true", StringComparison.OrdinalIgnoreCase);

                    var fastScanSetting = await _settingsApiService.GetSettingValueAsync("Barcode.FastScanEnabled");
                    ApiSession.IsBarcodeFastScanEnabled = string.Equals(fastScanSetting, "true", StringComparison.OrdinalIgnoreCase);

                    var incSetting = await _settingsApiService.GetSettingValueAsync("Barcode.FastScanIncrement");
                    if (!string.IsNullOrWhiteSpace(incSetting))
                    {
                        ApiSession.FastScanAutoIncrement = !string.Equals(incSetting, "false", StringComparison.OrdinalIgnoreCase);
                    }

                    var soundSetting = await _settingsApiService.GetSettingValueAsync("Barcode.FastScanSound");
                    if (!string.IsNullOrWhiteSpace(soundSetting))
                    {
                        ApiSession.FastScanSound = !string.Equals(soundSetting, "false", StringComparison.OrdinalIgnoreCase);
                    }

                    var defaultFormat = await _settingsApiService.GetSettingValueAsync("Bill.DefaultFormat");
                    if (!string.IsNullOrWhiteSpace(defaultFormat))
                    {
                        ConfigInfo.DefaultBillFormat = defaultFormat;
                    }

                    var thermalPrinter = await _settingsApiService.GetSettingValueAsync("Printer.ThermalPrinter");
                    if (!string.IsNullOrWhiteSpace(thermalPrinter))
                    {
                        ConfigInfo.ThermalPrinterName = thermalPrinter;
                    }

                    var a4Printer = await _settingsApiService.GetSettingValueAsync("Printer.A4Printer");
                    if (!string.IsNullOrWhiteSpace(a4Printer))
                    {
                        ConfigInfo.A4PrinterName = a4Printer;
                    }
                }
                catch
                {
                }

                ApiSession.NeedsOnlineReauth = false;
                try
                {
                    ApiSession.TerminalCode = await OfflineVoucherGenerator.RegisterTerminalWithServerAsync();
                }
                catch
                {
                    ApiSession.TerminalCode = OfflineVoucherGenerator.GetTerminalCode();
                }
                ApiSession.SetOfflineMode(false);
                OfflineCacheService.SaveCompanyAndSettingsSnapshot();
                _ = OfflineCacheService.RefreshCatalogAsync();

                frmMainMenu frm = new frmMainMenu();
                frm.Owner = this;
                UserInfo.LogInDateTime = DateTime.Now;

                Hide();
                frm.ShowDialog();
                Close();
            }
            catch (Exception ex)
            {
                if (OfflineSyncService.IsNetworkException(ex))
                {
                    SetOfflineButtonState(true);
                    var ans = MessageBox.Show(
                        "Cannot reach the RetailSuite server (" + ex.Message + ").\n\nWould you like to continue in Offline POS Mode (Touch POS & Sale Billing only)?",
                        "Server Unreachable",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (ans == DialogResult.Yes)
                    {
                        StartOfflinePosMode(txtUserId.Text.Trim());
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                progressBar1.Visible = false;
                btnLogIn.Enabled = true;
                txtPassword.Clear();
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnAddOrganization_Click(object sender, EventArgs e)
        {
            using (var licenseForm = new Forms.frmLicense())
            {
                if (licenseForm.ShowDialog() == DialogResult.OK)
                {
                    LoadOrganizations();
                    
                    // Show dropdown if multiple now
                    if (cmbOrganization.Items.Count > 1)
                    {
                        lblOrganization.Visible = true;
                        cmbOrganization.Visible = true;
                        btnAddOrganization.Visible = true;
                    }
                }
            }
        }

        private void cmbOrganization_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbOrganization.SelectedItem is OrganizationLicense license)
            {
                UpdateServices(license);
            }
        }

        private void LoadOrganizations()
        {
            var licenses = _licenseService.GetAllLicenses();
            cmbOrganization.DataSource = null;
            cmbOrganization.DisplayMember = "Name";
            cmbOrganization.ValueMember = "TenantId";
            cmbOrganization.DataSource = licenses;

            if (licenses.Count > 0)
            {
                cmbOrganization.SelectedIndex = 0;
                UpdateServices(licenses[0]);
            }
        }

        private void UpdateServices(OrganizationLicense license)
        {
            ApiSession.TenantIdentifier = license.TenantIdentifier;
            ApiSession.HasSupplyFeature = license.HasSupplyFeature;
            ApiSession.HasSecondaryQty = license.HasSecondaryQty;
            ApiSession.HasVariablePackFeature = license.HasVariablePackFeature;
            ApiSession.HasMobileShopFeature = license.HasMobileShopFeature;
            _loginService = new LoginService();
            _companyApiService = new CompanyApiService();
            _personalApiService = new PersonalApiService();
            ConfigInfo.Reset();
            RefreshTerminalLabel();
        }

        private void frmLogIn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{tab}");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
