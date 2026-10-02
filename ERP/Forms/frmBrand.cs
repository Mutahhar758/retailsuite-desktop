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
    public partial class frmBrand : Form
    {
        private readonly BrandApiService _brandApiService;
        private bool _fLogin = true;
        private bool _isSaving;

        public frmBrand()
        {
            InitializeComponent();
            _brandApiService = new BrandApiService();
            UserInfo.ApplyFormPermissions(this, AppResource.Brands);
        }

        private async void frmBrand_Load(object sender, EventArgs e)
        {
            await LoadBrandsAsync();
            _fLogin = false;
        }

        private async Task LoadBrandsAsync()
        {
            try
            {
                dgvBrands.Rows.Clear();
                var brands = await _brandApiService.GetListAsync();
                foreach (var b in brands)
                {
                    int rIdx = dgvBrands.Rows.Add(b.Id, b.Title, b.Active, "0");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading brands: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvBrands_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            int maxId = 0;
            try
            {
                maxId = (from DataGridViewRow row in dgvBrands.Rows
                         where row.Cells[clnId.Index].Value != null && !string.IsNullOrWhiteSpace(row.Cells[clnId.Index].Value.ToString())
                         select int.Parse(row.Cells[clnId.Index].Value.ToString())).Max();
            }
            catch { }

            int prevIdx = dgvBrands.Rows.Count - 2;
            if (prevIdx >= 0)
            {
                dgvBrands.Rows[prevIdx].Cells[clnId.Index].Value = (maxId + 1).ToString("D3");
                dgvBrands.Rows[prevIdx].Cells[clnActive.Index].Value = true;
                dgvBrands.Rows[prevIdx].Cells[clnIsEdit.Index].Value = "1";
            }
        }

        private void dgvBrands_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_fLogin && e.RowIndex >= 0 && dgvBrands.Rows[e.RowIndex] != null)
            {
                dgvBrands.Rows[e.RowIndex].Cells[clnIsEdit.Index].Value = "1";
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving) return;

            if (MessageBox.Show("Are you sure you want to save brands?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _isSaving = true;
            btnSave.Enabled = false;

            try
            {
                foreach (DataGridViewRow row in dgvBrands.Rows)
                {
                    if (row.IsNewRow) continue;

                    string id = Convert.ToString(row.Cells[clnId.Index].Value);
                    string title = Convert.ToString(row.Cells[clnTitle.Index].Value);
                    bool active = row.Cells[clnActive.Index].Value != null && Convert.ToBoolean(row.Cells[clnActive.Index].Value);
                    string isEdit = Convert.ToString(row.Cells[clnIsEdit.Index].Value);

                    if (string.IsNullOrWhiteSpace(title) || isEdit != "1")
                        continue;

                    var request = new BrandUpsertRequest
                    {
                        Id = id,
                        Title = title.Trim(),
                        Active = active
                    };

                    await _brandApiService.UpdateAsync(id, request);
                    row.Cells[clnIsEdit.Index].Value = "0";
                }

                MessageBox.Show("Brands saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadBrandsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving brands: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isSaving = false;
                btnSave.Enabled = true;
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBrands.CurrentRow == null || dgvBrands.CurrentRow.IsNewRow) return;

            string id = Convert.ToString(dgvBrands.CurrentRow.Cells[clnId.Index].Value);
            string title = Convert.ToString(dgvBrands.CurrentRow.Cells[clnTitle.Index].Value);

            if (string.IsNullOrWhiteSpace(id)) return;

            if (MessageBox.Show($"Are you sure you want to delete brand '{title}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                await _brandApiService.DeleteAsync(id);
                dgvBrands.Rows.Remove(dgvBrands.CurrentRow);
                MessageBox.Show("Brand deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting brand: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadBrandsAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
