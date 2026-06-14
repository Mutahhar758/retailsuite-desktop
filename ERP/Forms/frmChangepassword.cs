using System;
using System.Windows.Forms;
using ERP.Services.Authentication;

namespace ERP.Forms
{
    public partial class frmChangepassword : Form
    {
        private readonly LoginService _loginService;

        public frmChangepassword()
        {
            InitializeComponent();
            _loginService = new LoginService();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmChangepassword_Load(object sender, EventArgs e)
        {
            txtUserId.Text = UserInfo.UserId;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to change your password...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            if (string.IsNullOrWhiteSpace(txtOldPassword.Text))
            {
                MessageBox.Show("Please enter old password.");
                txtOldPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                MessageBox.Show("Please enter new password.");
                txtNewPassword.Focus();
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Password not match");
                txtConfirmPassword.Focus();
                return;
            }

            btnSave.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                string message = await _loginService.ChangePasswordAsync(txtOldPassword.Text, txtNewPassword.Text, false);
                MessageBox.Show(message);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private void frmChangepassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{tab}");
            }
        }
    }
}
