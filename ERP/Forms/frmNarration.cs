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
    public partial class frmNarration : Form
    {
        bool FLogin = true;
        private readonly NarrationApiService _narrationApiService;
        private readonly HashSet<string> _existingCodes = new HashSet<string>();
        private bool _isSaving;

        public frmNarration()
        {
            InitializeComponent();
            _narrationApiService = new NarrationApiService();
            UserInfo.ApplyFormPermissions(this, AppResource.Narrations);
        }

        async System.Threading.Tasks.Task FillFormAsync()
        {
            var narrations = await _narrationApiService.GetActiveNarrationsAsync();
            dgvNarration.Rows.Clear();
            _existingCodes.Clear();
            for (int i = 0; i < narrations.Count; i++)
            {
                var code = narrations[i].Code ?? string.Empty;
                dgvNarration.Rows.Add(code, narrations[i].Title, narrations[i].Status.ToString(), "0");
                _existingCodes.Add(code);
            }
        }
        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            dgvNarration.Rows[dgvNarration.Rows.Count - 2].Cells[clnStatus.Index].Value = "0";
            dgvNarration.Rows[dgvNarration.Rows.Count - 2].Cells[clnIsEdit.Index].Value = "1";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvNarration_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!FLogin)
            {
                dgvNarration.CurrentRow.Cells[clnIsEdit.Index].Value = "1";
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
                    foreach (DataGridViewRow row in dgvNarration.Rows)
                    {
                        if (row.Cells[clnNarration.Index].Value == null || Convert.ToString(row.Cells[clnIsEdit.Index].Value) != "1")
                            continue;

                        var code = row.Cells[clnCode.Index].Value != null ? row.Cells[clnCode.Index].Value.ToString() : string.Empty;
                        var title = row.Cells[clnNarration.Index].Value.ToString();

                        if (string.IsNullOrWhiteSpace(title))
                            continue;

                        if (!string.IsNullOrWhiteSpace(code) && _existingCodes.Contains(code))
                            await _narrationApiService.UpdateAsync(code, title);
                        else
                            await _narrationApiService.CreateAsync(title);
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

        private void dgvNarration_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dgvNarration.Rows[e.RowIndex].Selected = true;
                dgvNarration.CurrentCell = this.dgvNarration.Rows[e.RowIndex].Cells[e.ColumnIndex];
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private async void deleteRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Delete this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (dgvNarration[clnCode.Index, dgvNarration.CurrentRow.Index].Value != null)
                {
                    await _narrationApiService.DeleteAsync(dgvNarration.CurrentRow.Cells[clnCode.Index].Value.ToString());
                    dgvNarration.Rows.RemoveAt(dgvNarration.CurrentRow.Index);
                    MessageBox.Show("Record Successfully Deleted..!");
                }
            }
        }
    }
}
