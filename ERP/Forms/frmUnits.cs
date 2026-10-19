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
            UserInfo.ApplyFormPermissions(this, AppResource.Units);
        }

        async System.Threading.Tasks.Task FillFormAsync()
        {
            FLogin = true;
            var units = await _unitApiService.GetActiveAsync();

            var dtParentUnits = new DataTable();
            dtParentUnits.Columns.Add("Code", typeof(string));
            dtParentUnits.Columns.Add("Title", typeof(string));
            dtParentUnits.Rows.Add("", "(Base Unit)");
            for (int i = 0; i < units.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(units[i].ParentUnitId))
                {
                    var label = !string.IsNullOrWhiteSpace(units[i].ShortTitle)
                        ? units[i].Title + " (" + units[i].ShortTitle + ")"
                        : units[i].Title;
                    dtParentUnits.Rows.Add(units[i].Code, label);
                }
            }

            clnParentUnit.DataSource = dtParentUnits;
            clnParentUnit.ValueMember = "Code";
            clnParentUnit.DisplayMember = "Title";

            dgvUnits.Rows.Clear();
            for (int i = 0; i < units.Count; i++)
            {
                string parentUnitId = units[i].ParentUnitId ?? "";
                if (!string.IsNullOrEmpty(parentUnitId) && dtParentUnits.Select("Code = '" + parentUnitId.Replace("'", "''") + "'").Length == 0)
                {
                    dtParentUnits.Rows.Add(parentUnitId, units[i].ParentUnitTitle ?? parentUnitId);
                }

                string ratioText = units[i].ConversionRatio.HasValue && units[i].ConversionRatio.Value > 1m
                    ? units[i].ConversionRatio.Value.ToString("0.####")
                    : "";

                dgvUnits.Rows.Add(
                    units[i].Code,
                    units[i].Title,
                    units[i].ShortTitle ?? "",
                    parentUnitId,
                    ratioText,
                    "0",
                    "0");
            }
            FLogin = false;
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
                dgvUnits.Rows[dgvUnits.Rows.Count - 2].Cells[clnParentUnit.Index].Value = "";
                dgvUnits.Rows[dgvUnits.Rows.Count - 2].Cells[clnStatus.Index].Value = "0";
                dgvUnits.Rows[dgvUnits.Rows.Count - 2].Cells[clnIsEdit.Index].Value = "1";

            }
            catch (Exception)
            {
                dgvUnits.Rows[dgvUnits.Rows.Count - 2].Cells[clnCode.Index].Value = (code + 1).ToString("D3");
                dgvUnits.Rows[dgvUnits.Rows.Count - 2].Cells[clnParentUnit.Index].Value = "";
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
            if (!FLogin && dgvUnits.CurrentRow != null)
            {
                dgvUnits.CurrentRow.Cells[clnIsEdit.Index].Value = "1";
            }
        }

        private async void frmNarration_Load(object sender, EventArgs e)
        {
            try
            {
                dgvUnits.DataError += (s, err) => { err.ThrowException = false; };
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

            foreach (DataGridViewRow row in dgvUnits.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells[clnCode.Index].Value != null && row.Cells[clnUnits.Index].Value != null && Convert.ToString(row.Cells[clnIsEdit.Index].Value) == "1")
                {
                    string code = Convert.ToString(row.Cells[clnCode.Index].Value).Trim();
                    string title = Convert.ToString(row.Cells[clnUnits.Index].Value).Trim();
                    string parentId = Convert.ToString(row.Cells[clnParentUnit.Index].Value).Trim();
                    string ratioStr = Convert.ToString(row.Cells[clnConversionRatio.Index].Value).Trim();

                    if (!string.IsNullOrEmpty(parentId))
                    {
                        if (parentId == code)
                        {
                            MessageBox.Show("Unit '" + title + "' cannot be its own Parent Unit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        decimal ratio;
                        if (!decimal.TryParse(ratioStr, out ratio) || ratio <= 1m)
                        {
                            MessageBox.Show("Conversion Ratio for sub-unit '" + title + "' must be greater than 1 (e.g. 1000 for 1000 g = 1 kg).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
            }

            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Save this...!", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _isSaving = true;
            btnSave.Enabled = false;
            try
            {
                foreach (DataGridViewRow row in dgvUnits.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (row.Cells[clnCode.Index].Value != null && row.Cells[clnUnits.Index].Value != null && Convert.ToString(row.Cells[clnIsEdit.Index].Value) == "1")
                    {
                        string shortTitle = Convert.ToString(row.Cells[clnShortTitle.Index].Value).Trim();
                        string parentId = Convert.ToString(row.Cells[clnParentUnit.Index].Value).Trim();
                        string ratioStr = Convert.ToString(row.Cells[clnConversionRatio.Index].Value).Trim();
                        decimal parsedRatio;
                        decimal? conversionRatio = !string.IsNullOrEmpty(parentId) && decimal.TryParse(ratioStr, out parsedRatio) && parsedRatio > 1m
                            ? parsedRatio
                            : (decimal?)null;

                        await _unitApiService.UpsertAsync(
                            row.Cells[clnCode.Index].Value.ToString(),
                            new UnitUpsertApiRequest
                            {
                                Title = row.Cells[clnUnits.Index].Value.ToString(),
                                ShortTitle = string.IsNullOrEmpty(shortTitle) ? null : shortTitle,
                                ParentUnitId = string.IsNullOrEmpty(parentId) ? null : parentId,
                                ConversionRatio = conversionRatio,
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
