using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP.Forms
{
    public partial class frmScanSupplySheet : Form
    {
        private readonly SaleSupplyApiService _apiService;
        private readonly DataTable _dtCustomers;
        private readonly List<string> _filePaths = new List<string>();
        private Image _currentImage = null;

        public List<ScannedSaleSupplyLineDto> ImportedLines { get; private set; } = new List<ScannedSaleSupplyLineDto>();
        public List<string> ScannedFilePaths { get; private set; } = new List<string>();

        public frmScanSupplySheet(DateTime initialDate, DataTable dtCustomers)
        {
            InitializeComponent();
            _apiService = new SaleSupplyApiService();
            _dtCustomers = dtCustomers != null ? dtCustomers.Copy() : new DataTable();
            dtpTargetDate.Value = initialDate;
            InitializeDataGrid();
        }

        private void InitializeDataGrid()
        {
            dgvLines.AutoGenerateColumns = false;
            dgvLines.Columns.Clear();

            // 1. Seq
            var colSeq = new DataGridViewTextBoxColumn
            {
                Name = "Seq",
                HeaderText = "#",
                Width = 40,
                ReadOnly = true
            };
            dgvLines.Columns.Add(colSeq);

            // 2. Raw Customer (from slip)
            var colRaw = new DataGridViewTextBoxColumn
            {
                Name = "RawCustomer",
                HeaderText = "Customer on Slip",
                Width = 140,
                ReadOnly = true
            };
            dgvLines.Columns.Add(colRaw);

            // 3. Matched Customer ComboBox
            var colCustomer = new DataGridViewComboBoxColumn
            {
                Name = "CustomerId",
                HeaderText = "Mapped Account",
                DataSource = _dtCustomers,
                DisplayMember = "Title",
                ValueMember = "Account",
                Width = 180,
                FlatStyle = FlatStyle.Flat
            };
            dgvLines.Columns.Add(colCustomer);

            // 4. Qty
            var colQty = new DataGridViewTextBoxColumn
            {
                Name = "Qty",
                HeaderText = "Qty",
                Width = 60
            };
            dgvLines.Columns.Add(colQty);

            // 5. Rate
            var colRate = new DataGridViewTextBoxColumn
            {
                Name = "Rate",
                HeaderText = "Rate",
                Width = 70
            };
            dgvLines.Columns.Add(colRate);

            // 6. Disc
            var colDisc = new DataGridViewTextBoxColumn
            {
                Name = "Discount",
                HeaderText = "Disc",
                Width = 60
            };
            dgvLines.Columns.Add(colDisc);

            // 7. Amount
            var colAmount = new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "Amount",
                Width = 80,
                ReadOnly = true
            };
            dgvLines.Columns.Add(colAmount);

            // 8. Delete Button
            var colDel = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "",
                Text = "X",
                UseColumnTextForButtonValue = true,
                Width = 35
            };
            dgvLines.Columns.Add(colDel);
        }

        private void chkUseTargetDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpTargetDate.Enabled = chkUseTargetDate.Checked;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Paper Sheet Image(s) or PDF";
                ofd.Filter = "Image & PDF Files (*.jpg;*.jpeg;*.png;*.pdf)|*.jpg;*.jpeg;*.png;*.pdf|All Files (*.*)|*.*";
                ofd.Multiselect = true;

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    foreach (var file in ofd.FileNames)
                    {
                        if (!_filePaths.Any(p => string.Equals(p, file, StringComparison.OrdinalIgnoreCase)))
                        {
                            _filePaths.Add(file);
                        }
                    }

                    RefreshPageDropdown();
                    if (_filePaths.Count > 0 && cboPages.SelectedIndex < 0)
                    {
                        cboPages.SelectedIndex = 0;
                    }

                    lblStatus.Text = $"{_filePaths.Count} sheet(s) selected.";
                    lblSummary.Text = $"{_filePaths.Count} sheet(s) ready for AI analysis.";
                }
            }
        }

        private void RefreshPageDropdown()
        {
            var prevIndex = cboPages.SelectedIndex;
            cboPages.Items.Clear();
            for (int i = 0; i < _filePaths.Count; i++)
            {
                cboPages.Items.Add($"Page {i + 1}: {Path.GetFileName(_filePaths[i])}");
            }

            if (cboPages.Items.Count > 0)
            {
                cboPages.SelectedIndex = (prevIndex >= 0 && prevIndex < cboPages.Items.Count) ? prevIndex : 0;
            }
            else
            {
                cboPages.SelectedIndex = -1;
                ClearPreview();
            }
        }

        private void cboPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPages.SelectedIndex >= 0 && cboPages.SelectedIndex < _filePaths.Count)
            {
                LoadImagePreview(_filePaths[cboPages.SelectedIndex]);
            }
            else
            {
                ClearPreview();
            }
        }

        private void LoadImagePreview(string filePath)
        {
            try
            {
                ClearPreview();
                if (File.Exists(filePath))
                {
                    var ext = Path.GetExtension(filePath).ToLowerInvariant();
                    if (ext != ".pdf")
                    {
                        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            _currentImage = Image.FromStream(stream);
                            picPreview.Image = (Image)_currentImage.Clone();
                        }
                    }
                    else
                    {
                        picPreview.Image = null;
                    }
                }
            }
            catch (Exception ex)
            {
                picPreview.Image = null;
                lblStatus.Text = "Preview error: " + ex.Message;
            }
        }

        private void ClearPreview()
        {
            if (picPreview.Image != null)
            {
                picPreview.Image.Dispose();
                picPreview.Image = null;
            }
            if (_currentImage != null)
            {
                _currentImage.Dispose();
                _currentImage = null;
            }
        }

        private void btnRemovePage_Click(object sender, EventArgs e)
        {
            if (cboPages.SelectedIndex >= 0 && cboPages.SelectedIndex < _filePaths.Count)
            {
                _filePaths.RemoveAt(cboPages.SelectedIndex);
                RefreshPageDropdown();
            }
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            if (_filePaths.Count == 0)
            {
                MessageBox.Show(this, "Please select at least one sheet image or PDF to scan.", "No Files", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnScan.Enabled = false;
            btnBrowse.Enabled = false;
            btnAddPage.Enabled = false;
            btnRemovePage.Enabled = false;
            btnImport.Enabled = false;
            Cursor = Cursors.WaitCursor;
            lblStatus.ForeColor = Color.Navy;
            lblStatus.Text = $"Analyzing {_filePaths.Count} page(s) with Gemini Vision AI...";

            try
            {
                string targetDate = chkUseTargetDate.Checked ? dtpTargetDate.Value.ToString("yyyy-MM-dd") : null;
                var result = await _apiService.ScanSheetAsync(_filePaths, targetDate);

                PopulateGrid(result.Lines);
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = $"Extracted {result.Lines?.Count ?? 0} lines. (Remaining quota: {result.RemainingScansThisMonth}/{result.MonthlyLimit})";
                lblSummary.Text = $"Extracted {result.Lines?.Count ?? 0} lines from {_filePaths.Count} page(s).";
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Firebrick;
                lblStatus.Text = "Scan failed: " + ex.Message;
                MessageBox.Show(this, "Failed to scan paper sheets via AI: " + ex.Message, "Scan Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnScan.Enabled = true;
                btnBrowse.Enabled = true;
                btnAddPage.Enabled = true;
                btnRemovePage.Enabled = true;
                btnImport.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void PopulateGrid(List<ScannedSaleSupplyLineDto> lines)
        {
            dgvLines.Rows.Clear();
            if (lines == null || lines.Count == 0) return;

            foreach (var line in lines)
            {
                int rowIndex = dgvLines.Rows.Add();
                var row = dgvLines.Rows[rowIndex];

                row.Cells["Seq"].Value = line.Seq;
                row.Cells["RawCustomer"].Value = line.RawCustomerName;

                // Match with account if present
                if (!string.IsNullOrWhiteSpace(line.MatchedCustomerId))
                {
                    row.Cells["CustomerId"].Value = line.MatchedCustomerId;
                }

                row.Cells["Qty"].Value = line.Qty > 0 ? line.Qty : 1;
                row.Cells["Rate"].Value = line.Rate;
                row.Cells["Discount"].Value = line.Discount;

                decimal amount = (line.Qty * line.Rate) - line.Discount + line.AddLess + line.Carriage;
                row.Cells["Amount"].Value = Math.Max(0, amount);
            }

            UpdateTotalSummary();
        }

        private void dgvLines_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvLines.Rows[e.RowIndex];
            string colName = dgvLines.Columns[e.ColumnIndex].Name;

            if (colName == "Qty" || colName == "Rate" || colName == "Discount")
            {
                decimal qty = Convert.ToDecimal(row.Cells["Qty"].Value ?? 0);
                decimal rate = Convert.ToDecimal(row.Cells["Rate"].Value ?? 0);
                decimal disc = Convert.ToDecimal(row.Cells["Discount"].Value ?? 0);
                decimal amount = Math.Max(0, (qty * rate) - disc);
                row.Cells["Amount"].Value = amount;
                UpdateTotalSummary();
            }
        }

        private void dgvLines_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvLines.Columns[e.ColumnIndex].Name == "Delete")
            {
                dgvLines.Rows.RemoveAt(e.RowIndex);
                UpdateTotalSummary();
            }
        }

        private void UpdateTotalSummary()
        {
            int total = dgvLines.Rows.Count;
            int matched = 0;
            decimal totalAmount = 0;

            foreach (DataGridViewRow row in dgvLines.Rows)
            {
                if (row.Cells["CustomerId"].Value != null && !string.IsNullOrWhiteSpace(row.Cells["CustomerId"].Value.ToString()))
                {
                    matched++;
                }
                totalAmount += Convert.ToDecimal(row.Cells["Amount"].Value ?? 0);
            }

            lblSummary.Text = $"Total Lines: {total} | Matched: {matched}/{total} | Total Amount: Rs. {totalAmount:N2}";
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (dgvLines.Rows.Count == 0)
            {
                MessageBox.Show(this, "No lines to import.", "Empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int unassigned = 0;
            foreach (DataGridViewRow row in dgvLines.Rows)
            {
                if (row.Cells["CustomerId"].Value == null || string.IsNullOrWhiteSpace(row.Cells["CustomerId"].Value.ToString()))
                {
                    unassigned++;
                }
            }

            if (unassigned > 0)
            {
                var dr = MessageBox.Show(this, $"{unassigned} line(s) are not assigned to a customer account. Do you want to import anyway? (You must assign accounts before saving the voucher)", "Unmatched Customers", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes) return;
            }

            ImportedLines.Clear();
            int seq = 1;
            foreach (DataGridViewRow row in dgvLines.Rows)
            {
                var line = new ScannedSaleSupplyLineDto
                {
                    Seq = seq++,
                    RawCustomerName = Convert.ToString(row.Cells["RawCustomer"].Value),
                    MatchedCustomerId = Convert.ToString(row.Cells["CustomerId"].Value),
                    Qty = Convert.ToDecimal(row.Cells["Qty"].Value ?? 1),
                    Rate = Convert.ToDecimal(row.Cells["Rate"].Value ?? 0),
                    Discount = Convert.ToDecimal(row.Cells["Discount"].Value ?? 0),
                    Amount = Convert.ToDecimal(row.Cells["Amount"].Value ?? 0),
                    IsCustomerMatched = row.Cells["CustomerId"].Value != null && !string.IsNullOrWhiteSpace(row.Cells["CustomerId"].Value.ToString())
                };
                ImportedLines.Add(line);
            }

            ScannedFilePaths = new List<string>(_filePaths);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearPreview();
            base.OnFormClosed(e);
        }
    }
}
