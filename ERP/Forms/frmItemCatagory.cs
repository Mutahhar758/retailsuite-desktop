using System;
using System.Collections.Generic;
using System.Data;
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
                    "0");

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
            }
            catch (Exception)
            {
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells[clnCode.Index].Value = (code + 1).ToString("D3");
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells[clnStatus.Index].Value = "0";
                dgvItemCatagory.Rows[dgvItemCatagory.Rows.Count - 2].Cells[clnIsEdit.Index].Value = "1";
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

                        if (string.IsNullOrWhiteSpace(title))
                            continue;

                        if (!string.IsNullOrWhiteSpace(code) && _existingCodes.Contains(code))
                            await _itemCategoryApiService.UpdateAsync(code, title, active);
                        else
                            await _itemCategoryApiService.CreateAsync(title, active);
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
    }
}
