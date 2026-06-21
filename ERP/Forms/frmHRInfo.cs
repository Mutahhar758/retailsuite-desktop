using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP.Forms
{
    public partial class frmHRInfo : Form
    {
        private readonly HRInfoApiService _hrInfoService;
        private readonly ChartOfAccountApiService _chartOfAccountService;
        private List<HRInfoDto> _employees;

        public frmHRInfo()
        {
            InitializeComponent();
            _hrInfoService = new HRInfoApiService();
            _chartOfAccountService = new ChartOfAccountApiService();
            _employees = new List<HRInfoDto>();
        }

        async void FillQuery()
        {
            try
            {
                _employees = await _hrInfoService.GetAsync();
                dgvQuery.Rows.Clear();
                foreach (var emp in _employees)
                {
                    dgvQuery.Rows.Add(emp.Id,
                        emp.Name,
                        emp.Designation,
                        emp.JoiningDate.ToString("yyyy-MM-dd"),
                        emp.SalaryType,
                        emp.Salary.ToString("N2")
                        );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading HR records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        async void FillDetail(string ID)
        {
            try
            {
                var emp = await _hrInfoService.GetByIdAsync(ID);
                Validation.Clear(tbDetail);
                if (emp != null)
                {
                    txtHRID.Text = emp.Id;
                    txtName.Text = emp.Name;
                    txtFatherName.Text = emp.FatherName ?? string.Empty;
                    cmbGender.Text = emp.Gender;
                    dtpDOB.Value = emp.Dob;
                    cmdMaritialStatus.Text = emp.MaritialStatus ?? string.Empty;
                    txtCNIC.Text = emp.Cnic ?? string.Empty;
                    dtpAppointmentDate.Value = emp.AppointmentDate;
                    dtpJoiningDate.Value = emp.JoiningDate;
                    cmbDesignation.Text = emp.Designation ?? string.Empty;
                    cmbSalaryType.Text = emp.SalaryType;
                    ntxtSalary.Value = emp.Salary;
                    ntxtLeaveCharges.Value = emp.LeaveCharges;
                    ntxtOvertime.Value = emp.Overtime;
                    if (emp.ExpenseAccount != null)
                        cmbExpenseAccount.SelectedValue = emp.ExpenseAccount;
                    if (emp.PayableAccount != null)
                        cmbPayable.SelectedValue = emp.PayableAccount;

                    profileImage1.MediaId = emp.MediaId;
                    profileImage1.MediaUrl = emp.MediaUrl;
                    await profileImage1.LoadImageAsync(emp.MediaUrl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading HR detail: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadAccountLookupsAsync()
        {
            var expenseAccounts = await _chartOfAccountService.GetAccountsByPrefixAsync("004", 5);
            var payableAccounts = await _chartOfAccountService.GetAccountsByPrefixAsync("002", 5);

            BindAccountCombo(cmbExpenseAccount, expenseAccounts.OrderBy(x => x.Title).ToList());
            BindAccountCombo(cmbPayable, payableAccounts.OrderBy(x => x.Title).ToList());
        }

        private static void BindAccountCombo(ComboBox combo, List<ChartOfAccountHeadDto> accounts)
        {
            combo.DataSource = accounts;
            combo.DisplayMember = "Title";
            combo.ValueMember = "Account";
            combo.SelectedIndex = -1;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void frmHRInfo_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadAccountLookupsAsync();
                profileImage1.SearchButton.Click += btnHRSearch_Click;
                profileImage1.CancelButton.Click += btnHRCancel_Click;
                FillQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading lookup data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnHRSearch_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                        var presigned = await _hrInfoService.GetPresignedUploadUrlAsync(fileName);
                        if (presigned != null)
                        {
                            await _hrInfoService.UploadFileAsync(presigned.UploadUrl, openFileDialog.FileName);
                            profileImage1.MediaId = presigned.FileId;
                            profileImage1.MediaUrl = openFileDialog.FileName;
                            profileImage1.PictureBox.Image = Image.FromFile(openFileDialog.FileName);
                            MessageBox.Show("Image uploaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to upload image: {ex.Message}", "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnHRCancel_Click(object sender, EventArgs e)
        {
            profileImage1.ClearImage();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to save this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (UserInfo.UserId != "Admin")
                {
                    frmAuthentication frm = new frmAuthentication();
                    if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }
                }

                try
                {
                    var request = new HRInfoUpsertApiRequest
                    {
                        Name = txtName.Text,
                        FatherName = txtFatherName.Text,
                        Gender = cmbGender.Text,
                        Dob = dtpDOB.Value.ToString("yyyy-MM-dd"),
                        MaritialStatus = cmdMaritialStatus.Text,
                        Cnic = txtCNIC.Text,
                        AppointmentDate = dtpAppointmentDate.Value.ToString("yyyy-MM-dd"),
                        JoiningDate = dtpJoiningDate.Value.ToString("yyyy-MM-dd"),
                        Designation = cmbDesignation.Text,
                        SalaryType = cmbSalaryType.Text,
                        Salary = ntxtSalary.Value,
                        LeaveCharges = ntxtLeaveCharges.Value,
                        Overtime = ntxtOvertime.Value,
                        ExpenseAccount = cmbExpenseAccount.SelectedValue?.ToString() ?? string.Empty,
                        PayableAccount = cmbPayable.SelectedValue?.ToString() ?? string.Empty,
                        MediaId = profileImage1.MediaId
                    };

                    if (string.IsNullOrEmpty(txtHRID.Text))
                    {
                        // Create new
                        await _hrInfoService.CreateAsync(request);
                    }
                    else
                    {
                        // Update existing
                        await _hrInfoService.UpdateAsync(txtHRID.Text, request);
                    }

                    MessageBox.Show("Record Successfully Saved...!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FillQuery();
                    Validation.Clear(tbDetail);
                    profileImage1.ClearImage();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvQuery_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedId = dgvQuery.Rows[e.RowIndex].Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(selectedId))
                {
                    FillDetail(selectedId);
                    tabControl1.SelectedTab = tbDetail;
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Validation.Clear(tbDetail);
            txtHRID.Clear();
            profileImage1.ClearImage();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtHRID.Text != "")
            {
                if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Delete this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (UserInfo.UserId != "Admin")
                    {
                        frmAuthentication frm = new frmAuthentication();
                        if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        {
                            return;
                        }
                    }

                    try
                    {
                        await _hrInfoService.DeleteAsync(txtHRID.Text);
                        Validation.Clear(tbDetail);
                        profileImage1.ClearImage();
                        MessageBox.Show("Record Successfully Deleted...!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FillQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void frmHRInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && tabControl1.SelectedTab == tbDetail)
            {
                SendKeys.Send("{tab}");
            }
        }
    }
}
