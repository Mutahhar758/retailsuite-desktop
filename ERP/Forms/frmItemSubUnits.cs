using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ERP
{
    public class frmItemSubUnits : Form
    {
        private readonly string _primaryUnitId;
        private readonly string _primaryUnitLabel;
        private readonly DataTable _dtUnits;
        private CheckBox _chkOverride;
        private Label _lblInheritedInfo;
        private DataGridView _dgvSubUnits;
        private DataGridViewComboBoxColumn _clnSubUnit;
        private DataGridViewTextBoxColumn _clnFactor;
        private Button _btnOk;
        private Button _btnCancel;

        public string ResultAdditionalUnitsJson { get; private set; }

        public frmItemSubUnits(string itemTitle, string primaryUnitId, DataTable dtUnits, string currentAdditionalUnitsJson)
        {
            _primaryUnitId = primaryUnitId ?? "";
            _dtUnits = dtUnits;
            ResultAdditionalUnitsJson = currentAdditionalUnitsJson;

            _primaryUnitLabel = _primaryUnitId;
            if (_dtUnits != null && !string.IsNullOrEmpty(_primaryUnitId))
            {
                var rows = _dtUnits.Select("Code = '" + _primaryUnitId.Replace("'", "''") + "'");
                if (rows.Length > 0)
                {
                    string st = _dtUnits.Columns.Contains("ShortTitle") ? Convert.ToString(rows[0]["ShortTitle"]).Trim() : "";
                    string t = Convert.ToString(rows[0]["Title"]).Trim();
                    _primaryUnitLabel = !string.IsNullOrEmpty(st) ? st : t;
                }
            }

            InitializeLayout(itemTitle);
            LoadData(currentAdditionalUnitsJson);
        }

        private void InitializeLayout(string itemTitle)
        {
            Text = "Sub-Units Configuration" + (!string.IsNullOrWhiteSpace(itemTitle) ? " - " + itemTitle : "");
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(480, 320);
            BackColor = SystemColors.ButtonHighlight;

            var inheritedSubs = UnitConversionHelper.GetEffectiveSubUnits(_primaryUnitId, null, _dtUnits);
            string inheritedSummary = inheritedSubs.Count > 0
                ? "Default from Primary Unit (" + _primaryUnitLabel + "): " + string.Join(", ", inheritedSubs.Select(s => s.Factor.ToString("0.####") + " " + (s.ShortTitle ?? s.UnitTitle) + " = 1 " + _primaryUnitLabel))
                : "Default from Primary Unit (" + _primaryUnitLabel + "): No sub-units configured on unit setup.";

            _lblInheritedInfo = new Label
            {
                Location = new Point(14, 14),
                Size = new Size(452, 34),
                Text = inheritedSummary,
                ForeColor = Color.FromArgb(30, 64, 175),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular)
            };

            _chkOverride = new CheckBox
            {
                Location = new Point(14, 52),
                Size = new Size(440, 24),
                Text = "Override default sub-unit ratio / unit for this item",
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold)
            };
            _chkOverride.CheckedChanged += (s, e) =>
            {
                _dgvSubUnits.Enabled = _chkOverride.Checked;
            };

            _dgvSubUnits = new DataGridView
            {
                Location = new Point(14, 82),
                Size = new Size(452, 175),
                AllowUserToDeleteRows = true,
                AllowUserToAddRows = true,
                RowHeadersWidth = 28,
                BackgroundColor = SystemColors.ControlLightLight,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            _dgvSubUnits.DataError += (s, e) => { e.ThrowException = false; };

            var dtSelectableUnits = new DataTable();
            dtSelectableUnits.Columns.Add("Code", typeof(string));
            dtSelectableUnits.Columns.Add("Title", typeof(string));
            if (_dtUnits != null)
            {
                for (int i = 0; i < _dtUnits.Rows.Count; i++)
                {
                    string code = Convert.ToString(_dtUnits.Rows[i]["Code"]);
                    if (code == _primaryUnitId) continue;
                    string title = Convert.ToString(_dtUnits.Rows[i]["Title"]);
                    dtSelectableUnits.Rows.Add(code, title);
                }
            }

            _clnSubUnit = new DataGridViewComboBoxColumn
            {
                HeaderText = "Sub-Unit",
                Name = "clnSubUnit",
                DataSource = dtSelectableUnits,
                ValueMember = "Code",
                DisplayMember = "Title",
                Width = 220,
                FlatStyle = FlatStyle.Flat
            };

            _clnFactor = new DataGridViewTextBoxColumn
            {
                HeaderText = "Ratio (> 1 per 1 " + _primaryUnitLabel + ")",
                Name = "clnFactor",
                Width = 180
            };

            _dgvSubUnits.Columns.AddRange(new DataGridViewColumn[] { _clnSubUnit, _clnFactor });

            _btnOk = new Button
            {
                Text = "&OK",
                Location = new Point(296, 272),
                Size = new Size(80, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = SystemColors.ControlLight
            };
            _btnOk.Click += BtnOk_Click;

            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(386, 272),
                Size = new Size(80, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = SystemColors.ControlLight,
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(_lblInheritedInfo);
            Controls.Add(_chkOverride);
            Controls.Add(_dgvSubUnits);
            Controls.Add(_btnOk);
            Controls.Add(_btnCancel);
            AcceptButton = _btnOk;
            CancelButton = _btnCancel;
        }

        private void LoadData(string currentAdditionalUnitsJson)
        {
            bool hasOverride = !string.IsNullOrWhiteSpace(currentAdditionalUnitsJson);
            _chkOverride.Checked = hasOverride;
            _dgvSubUnits.Enabled = hasOverride;

            var listToDisplay = UnitConversionHelper.GetEffectiveSubUnits(_primaryUnitId, currentAdditionalUnitsJson, _dtUnits);
            _dgvSubUnits.Rows.Clear();
            for (int i = 0; i < listToDisplay.Count; i++)
            {
                var item = listToDisplay[i];
                _dgvSubUnits.Rows.Add(item.UnitId, item.Factor.ToString("0.####"));
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (!_chkOverride.Checked)
            {
                ResultAdditionalUnitsJson = null;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            var list = new List<AdditionalUnitConfigDto>();
            foreach (DataGridViewRow row in _dgvSubUnits.Rows)
            {
                if (row.IsNewRow) continue;
                string unitId = Convert.ToString(row.Cells[0].Value).Trim();
                string factorStr = Convert.ToString(row.Cells[1].Value).Trim();
                if (string.IsNullOrEmpty(unitId) && string.IsNullOrEmpty(factorStr))
                    continue;

                if (string.IsNullOrEmpty(unitId))
                {
                    MessageBox.Show("Please select a Sub-Unit.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal factor;
                if (!decimal.TryParse(factorStr, out factor) || factor <= 1m)
                {
                    MessageBox.Show("Ratio must be greater than 1 (e.g. 1000 for 1000 g = 1 kg).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string unitTitle = unitId;
                string shortTitle = unitId;
                if (_dtUnits != null)
                {
                    var uRows = _dtUnits.Select("Code = '" + unitId.Replace("'", "''") + "'");
                    if (uRows.Length > 0)
                    {
                        unitTitle = Convert.ToString(uRows[0]["Title"]);
                        shortTitle = _dtUnits.Columns.Contains("ShortTitle") && !string.IsNullOrWhiteSpace(Convert.ToString(uRows[0]["ShortTitle"]))
                            ? Convert.ToString(uRows[0]["ShortTitle"])
                            : unitTitle;
                    }
                }

                list.Add(new AdditionalUnitConfigDto
                {
                    UnitId = unitId,
                    UnitTitle = unitTitle,
                    ShortTitle = shortTitle,
                    Factor = factor
                });
            }

            ResultAdditionalUnitsJson = JsonConvert.SerializeObject(list);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
