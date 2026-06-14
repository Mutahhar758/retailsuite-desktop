using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmUnits : Form
    {
        bool FLogin = true;
        private readonly UnitApiService _unitApiService;
        private bool _isSaving;

        public frmUnits()
        {
            InitializeComponent();
            _unitApiService = new UnitApiService();
        }

        async System.Threading.Tasks.Task FillFormAsync()
        {
            var units = await _unitApiService.GetActiveAsync();
            dgvUnits.Rows.Clear();
            for (int i = 0; i < units.Count; i++)
            {
                dgvUnits.Rows.Add(units[i].Code, units[i].Title, "0", "0");
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            int code = 0;
            try
            {
                code = (int)(from DataGridViewRow row in dgvUnits.Rows
                             where row.Cells[clnCode.Index].Value != null && row.Cells[clnCode.Index].Value.ToString() != ""
                             select int.Parse(row.Cells[clnCode.Index].Value.ToString())).Max();
                dgvUnits.Rows[dgvUnits.Rows.Count - 2].Cells[clnCode.Index].Value = (code + 1).ToString("D3");
                dgvUnits.Rows[dgvUnits.Rows.Count - 2].Cells[clnStatus.Index].Value = "0";
                dgvUnits.Rows[dgvUnits.Rows.Count - 2].Cells[clnIsEdit.Index].Value = "1";

            }
            catch (Exception)
            {
                dgvUnits.Rows[dgvUnits.Rows.Count - 2].Cells[clnCode.Index].Value = (code + 1).ToString("D3");
                dgvUnits.Rows[dgvUnits.Rows.Count - 2].Cells[clnStatus.Index].Value = "0";
                dgvUnits.Rows[dgvUnits.Rows.Count - 2].Cells[clnIsEdit.Index].Value = "1";
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
                dgvUnits.CurrentRow.Cells[clnIsEdit.Index].Value = "1";
            }
        }

        private async void frmNarration_Load(object sender, EventArgs e)
        {
            try
            {
                await FillFormAsync();
                FLogin = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving)
                return;

            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Save this...!", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _isSaving = true;
            btnSave.Enabled = false;
            try
            {
                foreach (DataGridViewRow row in dgvUnits.Rows)
                {
                    if (row.Cells[clnCode.Index].Value != null && row.Cells[clnUnits.Index].Value != null && Convert.ToString(row.Cells[clnIsEdit.Index].Value) == "1")
                    {
                        await _unitApiService.UpsertAsync(
                            row.Cells[clnCode.Index].Value.ToString(),
                            new UnitUpsertApiRequest
                            {
                                Title = row.Cells[clnUnits.Index].Value.ToString(),
                                Active = Convert.ToString(row.Cells[clnStatus.Index].Value) == "0"
                            });
                    }
                }
                MessageBox.Show("Record Successfully Saved..!");
                await FillFormAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isSaving = false;
                btnSave.Enabled = true;
            }
        }
    }
}
