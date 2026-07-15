using System;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Dtos.Authentication;
using ERP.Services.Authentication;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmLogIn : Form
    {
        private LoginService _loginService;
        private readonly LicenseService _licenseService;
        private CompanyApiService _companyApiService;
        private PersonalApiService _personalApiService;

        public frmLogIn()
        {
            InitializeComponent();
            _licenseService = new LicenseService();
            _loginService = new LoginService();
            _companyApiService = new CompanyApiService();
            _personalApiService = new PersonalApiService();
            
            // Ensure icon is set
            pictureBox1.Image = global::ERP.Properties.Resources.if_lock_318582;
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
                    _licenseService.UpdateFeaturesInStore(ApiSession.TenantIdentifier, features.HasSupplyFeature, features.HasSecondaryQty);
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
                    CompanyInfo.Cell = company.Cell;
                    CompanyInfo.Cell2 = company.Cell2;
                    CompanyInfo.ContactHead = company.ContactHeader ?? "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Warning: Could not load company info from API: " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                frmMainMenu frm = new frmMainMenu();
                frm.Owner = this;
                UserInfo.LogInDateTime = DateTime.Now;

                Hide();
                frm.ShowDialog();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            _loginService = new LoginService();
            _companyApiService = new CompanyApiService();
            _personalApiService = new PersonalApiService();
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
