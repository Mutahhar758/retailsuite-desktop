using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmItemCatagory : Form
    {
        bool FLogin = true;
        private readonly ItemCategoryApiService _itemCategoryApiService;
        private readonly HashSet<string> _existingCodes = new HashSet<string>();
        private bool _isSaving;

        public frmItemCatagory()
        {
            InitializeComponent();
            _itemCategoryApiService = new ItemCategoryApiService();
        }

        async System.Threading.Tasks.Task FillFormAsync()
        {
            var itemCategories = await _itemCategoryApiService.GetActiveAsync();
            dgvItemCatagory.Rows.Clear();
            _existingCodes.Clear();

            for (int i = 0; i < itemCategories.Count; i++)
            {
                var code = itemCategories[i].Code ?? string.Empty;
                dgvItemCatagory.Rows.Add(
                    code,
                    itemCategories[i].Title,
                    itemCategories[i].Active,
                    "0",
                    "0",
                    itemCategories[i].MediaId ?? string.Empty,
                    itemCategories[i].MediaUrl ?? string.Empty);

                _existingCodes.Add(code);
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            int code = 0;
            try
            {
                code = (int)(from DataGridViewRow row in dgvItemCatagory.Rows
                             where row.Cells[clnCode.Index].Value != null && row.Cells[clnCode.Index].Value.ToString() != ""
                             select int.Parse(row.Cells[clnCode.Index].Value.ToString())).Max();
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells[clnCode.Index].Value = (code + 1).ToString("D3");
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells[clnStatus.Index].Value = "0";
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells[clnIsEdit.Index].Value = "1";
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells["clnMediaId"].Value = string.Empty;
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells["clnMediaUrl"].Value = string.Empty;
            }
            catch (Exception)
            {
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells[clnCode.Index].Value = (code + 1).ToString("D3");
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells[clnStatus.Index].Value = "0";
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells[clnIsEdit.Index].Value = "1";
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells["clnMediaId"].Value = string.Empty;
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells["clnMediaUrl"].Value = string.Empty;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvNarration_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!FLogin)
            {
                dgvItemCatagory.CurrentRow.Cells[clnIsEdit.Index].Value = "1";
            }
        }

        private async void frmNarration_Load(object sender, EventArgs e)
        {
            profileImage1.SearchButton.Click += btnCategorySearch_Click;
            profileImage1.CancelButton.Click += btnCategoryCancel_Click;
            dgvItemCatagory.SelectionChanged += dgvItemCatagory_SelectionChanged;

            await FillFormAsync();
            FLogin = false;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving)
                return;

            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Save this...!", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _isSaving = true;
                btnSave.Enabled = false;
                try
                {
                    foreach (DataGridViewRow row in dgvItemCatagory.Rows)
                    {
                        if (row.Cells[clnItemCatagory.Index].Value == null || Convert.ToString(row.Cells[clnIsEdit.Index].Value) != "1")
                            continue;

                        var code = row.Cells[clnCode.Index].Value != null ? row.Cells[clnCode.Index].Value.ToString() : string.Empty;
                        var title = row.Cells[clnItemCatagory.Index].Value.ToString();
                        var active = Validation.ToBool(row.Cells[clnActive.Index].Value);
                        var mediaId = row.Cells["clnMediaId"].Value != null ? row.Cells["clnMediaId"].Value.ToString() : string.Empty;

                        if (string.IsNullOrWhiteSpace(title))
                            continue;

                        if (!string.IsNullOrWhiteSpace(code) && _existingCodes.Contains(code))
                            await _itemCategoryApiService.UpdateAsync(code, title, active, mediaId);
                        else
                            await _itemCategoryApiService.CreateAsync(title, active, mediaId);
                    }

                    MessageBox.Show("Record Successfully Saved..!");
                    await FillFormAsync();
                }
                finally
                {
                    _isSaving = false;
                    btnSave.Enabled = true;
                }
            }
        }

        private void dgvItemCatagory_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private async void dgvItemCatagory_SelectionChanged(object sender, EventArgs e)
        {
            await LoadSelectedRowImageAsync();
        }

        private async System.Threading.Tasks.Task LoadSelectedRowImageAsync()
        {
            if (profileImage1 == null || dgvItemCatagory.CurrentRow == null)
                return;

            var row = dgvItemCatagory.CurrentRow;
            if (row.Cells["clnMediaId"] != null && row.Cells["clnMediaId"].Value != null)
            {
                var mediaId = Convert.ToString(row.Cells["clnMediaId"].Value);
                var mediaUrl = Convert.ToString(row.Cells["clnMediaUrl"].Value);

                profileImage1.MediaId = mediaId;
                profileImage1.MediaUrl = mediaUrl;
                await profileImage1.LoadImageAsync(mediaUrl);
            }
            else
            {
                profileImage1.ClearImage();
            }
        }

        private async void btnCategorySearch_Click(object sender, EventArgs e)
        {
            if (dgvItemCatagory.CurrentRow == null)
            {
                MessageBox.Show("Please select a category row first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                        var presigned = await _itemCategoryApiService.GetPresignedUploadUrlAsync(fileName);
                        if (presigned != null)
                        {
                            await _itemCategoryApiService.UploadFileAsync(presigned.UploadUrl, openFileDialog.FileName);
                            
                            var row = dgvItemCatagory.CurrentRow;
                            row.Cells["clnMediaId"].Value = presigned.FileId;
                            row.Cells["clnMediaUrl"].Value = openFileDialog.FileName;
                            row.Cells["clnIsEdit"].Value = "1";
                            
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

        private void btnCategoryCancel_Click(object sender, EventArgs e)
        {
            if (dgvItemCatagory.CurrentRow != null)
            {
                var row = dgvItemCatagory.CurrentRow;
                row.Cells["clnMediaId"].Value = string.Empty;
                row.Cells["clnMediaUrl"].Value = string.Empty;
                row.Cells["clnIsEdit"].Value = "1";
            }
            profileImage1.ClearImage();
        }
    }
}
