using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP.Forms
{
    public partial class frmCustomerCategory : Form
    {
        private readonly CustomerCategoryApiService _customerCategoryApiService;
        private readonly HashSet<string> _existingCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private bool _fLogin = true;
        private bool _isSaving;

        public frmCustomerCategory()
        {
            InitializeComponent();
            _customerCategoryApiService = new CustomerCategoryApiService();
            UserInfo.ApplyFormPermissions(this, AppResource.CustomerCategories);
        }

        private async void frmCustomerCategory_Load(object sender, EventArgs e)
        {
            await LoadCategoriesAsync();
            _fLogin = false;
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                _fLogin = true;
                dgvCategories.Rows.Clear();
                _existingCodes.Clear();

                var categories = await _customerCategoryApiService.GetActiveAsync();
                foreach (var c in categories)
                {
                    _existingCodes.Add(c.Code);
                    dgvCategories.Rows.Add(c.Code, c.Title, c.Description ?? string.Empty, c.Active, "0");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _fLogin = false;
            }
        }

        private void dgvCategories_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            int maxCode = 0;
            try
            {
                maxCode = (from DataGridViewRow row in dgvCategories.Rows
                           where row.Cells[clnCode.Index].Value != null && !string.IsNullOrWhiteSpace(row.Cells[clnCode.Index].Value.ToString())
                           select int.Parse(row.Cells[clnCode.Index].Value.ToString())).Max();
            }
            catch { }

            int prevIdx = dgvCategories.Rows.Count - 2;
            if (prevIdx >= 0)
            {
                dgvCategories.Rows[prevIdx].Cells[clnCode.Index].Value = (maxCode + 1).ToString("D3");
                dgvCategories.Rows[prevIdx].Cells[clnActive.Index].Value = true;
                dgvCategories.Rows[prevIdx].Cells[clnIsEdit.Index].Value = "1";
            }
        }

        private void dgvCategories_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_fLogin && e.RowIndex >= 0 && dgvCategories.Rows[e.RowIndex] != null)
            {
                dgvCategories.Rows[e.RowIndex].Cells[clnIsEdit.Index].Value = "1";
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving) return;

            if (MessageBox.Show("Are you sure you want to save customer categories?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _isSaving = true;
            btnSave.Enabled = false;

            try
            {
                foreach (DataGridViewRow row in dgvCategories.Rows)
                {
                    if (row.IsNewRow) continue;

                    string code = Convert.ToString(row.Cells[clnCode.Index].Value);
                    string title = Convert.ToString(row.Cells[clnTitle.Index].Value);
                    string description = Convert.ToString(row.Cells[clnDescription.Index].Value);
                    bool active = row.Cells[clnActive.Index].Value != null && Convert.ToBoolean(row.Cells[clnActive.Index].Value);
                    string isEdit = Convert.ToString(row.Cells[clnIsEdit.Index].Value);

                    if (string.IsNullOrWhiteSpace(title) || isEdit != "1")
                        continue;

                    if (!string.IsNullOrWhiteSpace(code) && _existingCodes.Contains(code))
                    {
                        await _customerCategoryApiService.UpdateAsync(code, title.Trim(), description, active);
                    }
                    else
                    {
                        var newCode = await _customerCategoryApiService.CreateAsync(code, title.Trim(), description, active);
                        if (!string.IsNullOrWhiteSpace(newCode))
                            _existingCodes.Add(newCode);
                    }

                    row.Cells[clnIsEdit.Index].Value = "0";
                }

                MessageBox.Show("Customer categories saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadCategoriesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving customer categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isSaving = false;
                btnSave.Enabled = true;
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow == null || dgvCategories.CurrentRow.IsNewRow) return;

            string code = Convert.ToString(dgvCategories.CurrentRow.Cells[clnCode.Index].Value);
            string title = Convert.ToString(dgvCategories.CurrentRow.Cells[clnTitle.Index].Value);

            if (string.IsNullOrWhiteSpace(code)) return;

            if (MessageBox.Show($"Are you sure you want to delete customer category '{title}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                await _customerCategoryApiService.DeleteAsync(code);
                dgvCategories.Rows.Remove(dgvCategories.CurrentRow);
                MessageBox.Show("Customer category deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting customer category: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadCategoriesAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
