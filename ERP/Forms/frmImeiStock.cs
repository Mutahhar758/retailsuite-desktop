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
    public partial class frmImeiStock : Form
    {
        private readonly MobileShopApiService _mobileShopApiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private List<ImeiStockDto> _allStock = new List<ImeiStockDto>();

        public frmImeiStock()
        {
            InitializeComponent();
            _mobileShopApiService = new MobileShopApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            UserInfo.ApplyFormPermissions(this, AppResource.ImeiStock);
        }

        private async void frmImeiStock_Load(object sender, EventArgs e)
        {
            cmbPtaFilter.SelectedIndex = 0;
            await LoadStockAsync();
        }

        private async Task LoadStockAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _allStock = await _mobileShopApiService.GetAvailableImeisAsync();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading IMEI stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ApplyFilter()
        {
            string search = (txtSearch.Text ?? string.Empty).Trim().ToLowerInvariant();
            string pta = cmbPtaFilter.SelectedItem?.ToString() ?? "All";

            var filtered = _allStock.Where(x =>
            {
                if (pta != "All" && !string.Equals(x.PtaStatus, pta, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (!string.IsNullOrWhiteSpace(search))
                {
                    bool match = (x.Imei != null && x.Imei.ToLowerInvariant().Contains(search))
                              || (x.Imei2 != null && x.Imei2.ToLowerInvariant().Contains(search))
                              || (x.ItemTitle != null && x.ItemTitle.ToLowerInvariant().Contains(search))
                              || (x.BrandTitle != null && x.BrandTitle.ToLowerInvariant().Contains(search))
                              || (x.ModelName != null && x.ModelName.ToLowerInvariant().Contains(search));
                    if (!match) return false;
                }

                return true;
            }).ToList();

            dgvStock.Rows.Clear();
            foreach (var item in filtered)
            {
                dgvStock.Rows.Add(
                    item.Imei,
                    item.Imei2,
                    item.ItemTitle,
                    item.BrandTitle,
                    item.ModelName,
                    item.Storage,
                    item.Color,
                    item.PtaStatus,
                    item.BatteryHealth.HasValue ? item.BatteryHealth.Value + "%" : "-",
                    item.ConditionNote,
                    item.PurchaseCost.ToString("N0"),
                    item.AddedCost.ToString("N0"),
                    item.TotalCost.ToString("N0"),
                    item.SellingPrice.ToString("N0"),
                    item.PurchaseVoucherNo,
                    item.PurchaseDate.ToString("dd-MMM-yyyy"));
            }

            lblTotalCount.Text = $"Total Phones in Stock: {filtered.Count}";
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void cmbPtaFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadStockAsync();
        }

        private async void btnAddCost_Click(object sender, EventArgs e)
        {
            if (dgvStock.CurrentRow == null)
            {
                MessageBox.Show("Please select an IMEI from the list first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string imei = Convert.ToString(dgvStock.CurrentRow.Cells[clnImei.Index].Value);
            string device = Convert.ToString(dgvStock.CurrentRow.Cells[clnItemTitle.Index].Value);

            using (var form = new Form())
            {
                form.Text = $"Add Landed Cost / Refurbishment - IMEI: {imei}";
                form.Size = new Size(420, 320);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var lblInfo = new Label { Text = $"Device: {device}", Location = new Point(20, 15), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
                
                var lblType = new Label { Text = "Cost Type:", Location = new Point(20, 50), AutoSize = true };
                var cmbType = new ComboBox { Location = new Point(130, 48), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbType.Items.AddRange(new object[] { "PTA Approval Tax", "LCD / Display Replacement", "Battery Replacement", "Back Glass / Body Repair", "Labour / Servicing", "Other Refurbishment" });
                cmbType.SelectedIndex = 0;

                var lblAmount = new Label { Text = "Amount (Rs):", Location = new Point(20, 90), AutoSize = true };
                var numAmount = new NumericUpDown { Location = new Point(130, 88), Width = 240, Maximum = 10000000, DecimalPlaces = 0, ThousandsSeparator = true };

                var lblDesc = new Label { Text = "Description:", Location = new Point(20, 130), AutoSize = true };
                var txtDesc = new TextBox { Location = new Point(130, 128), Width = 240 };

                var lblAccount = new Label { Text = "Paid From:", Location = new Point(20, 170), AutoSize = true };
                var cmbAccount = new ComboBox { Location = new Point(130, 168), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };

                try
                {
                    var accounts = await _chartOfAccountApiService.GetCashBankAccountsAsync();
                    cmbAccount.DisplayMember = "Title";
                    cmbAccount.ValueMember = "Account";
                    cmbAccount.DataSource = accounts;
                }
                catch { }

                var btnSubmit = new Button { Text = "Save Cost", DialogResult = DialogResult.OK, Location = new Point(190, 220), Width = 90, Height = 32, BackColor = Color.ForestGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(290, 220), Width = 80, Height = 32 };

                form.Controls.AddRange(new Control[] { lblInfo, lblType, cmbType, lblAmount, numAmount, lblDesc, txtDesc, lblAccount, cmbAccount, btnSubmit, btnCancel });
                form.AcceptButton = btnSubmit;
                form.CancelButton = btnCancel;

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    if (numAmount.Value <= 0)
                    {
                        MessageBox.Show("Please enter a valid cost amount.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        var req = new ImeiCostAdditionRequest
                        {
                            Imei = imei,
                            CostType = cmbType.SelectedItem.ToString(),
                            Amount = numAmount.Value,
                            Description = txtDesc.Text.Trim(),
                            PaidFromAccountId = cmbAccount.SelectedValue?.ToString()
                        };

                        await _mobileShopApiService.AddImeiCostAsync(req);
                        MessageBox.Show("Landed cost added successfully! The total cost for this device has been updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadStockAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error adding cost: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            if (dgvStock.CurrentRow == null)
            {
                MessageBox.Show("Please select an IMEI from the list first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string imei = Convert.ToString(dgvStock.CurrentRow.Cells[clnImei.Index].Value);
            var historyForm = new frmImeiHistory(imei);
            historyForm.ShowDialog(this);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
