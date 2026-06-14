using System;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Dtos.Authentication;
using ERP.Services.Authentication;

namespace ERP
{
    public partial class frmAuthentication : Form
    {
        private readonly LoginService _loginService;

        public frmAuthentication()
        {
            InitializeComponent();
            _loginService = new LoginService();
        }

        private void frmLogIn_Load(object sender, EventArgs e)
        {
            txtUserId.Text = string.IsNullOrWhiteSpace(UserInfo.UserName) ? string.Empty : UserInfo.UserName;
            txtUserId.ReadOnly = !string.IsNullOrWhiteSpace(UserInfo.UserName);
        }

        private async void btnLogIn_Click(object sender, EventArgs e)
        {
            string username = txtUserId.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter your username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            btnLogIn.Enabled = false;

            try
            {
                LoginResultDto result = await _loginService.LoginAsync(username, password);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Authentication failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogIn.Enabled = true;
                txtPassword.Clear();
                Cursor.Current = Cursors.Default;
            }
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
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
