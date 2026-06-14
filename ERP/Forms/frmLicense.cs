using System;
using System.Windows.Forms;
using ERP.Services.Authentication;

namespace ERP.Forms
{
    public partial class frmLicense : Form
    {
        private readonly LicenseService _licenseService;

        public frmLicense()
        {
            InitializeComponent();
            _licenseService = new LicenseService();
        }

        private async void btnActivate_Click(object sender, EventArgs e)
        {
            string licenseKey = txtLicenseKey.Text.Trim();

            if (string.IsNullOrEmpty(licenseKey))
            {
                MessageBox.Show("Please enter a license key.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnActivate.Enabled = false;
            Cursor = Cursors.WaitCursor;

            try
            {
                var result = await _licenseService.VerifyLicenseKeyAsync(licenseKey);
                if (result.Success)
                {
                    MessageBox.Show("Application activated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(result.Message, "Activation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnActivate.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
