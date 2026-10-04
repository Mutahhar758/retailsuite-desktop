using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP.Forms
{
    public partial class frmImeiHistory : Form
    {
        private string _imei;
        private readonly MobileShopApiService _mobileShopApiService;

        public frmImeiHistory(string imei = "")
        {
            InitializeComponent();
            _imei = imei;
            _mobileShopApiService = new MobileShopApiService();
            UserInfo.ApplyFormPermissions(this, AppResource.ImeiStock);
        }

        private async void frmImeiHistory_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_imei))
            {
                txtSearchImei.Text = _imei;
                await LoadHistoryAsync(_imei);
            }
            else
            {
                lblDeviceSummary.Text = "Enter an IMEI or serial number above and click Search.";
                lblStatusBadge.Visible = false;
                txtSearchImei.Focus();
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var search = txtSearchImei.Text.Trim();
            if (string.IsNullOrWhiteSpace(search))
            {
                MessageBox.Show("Please enter an IMEI / Serial number to search.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSearchImei.Focus();
                return;
            }

            _imei = search;
            await LoadHistoryAsync(_imei);
        }

        private void txtSearchImei_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnSearch.PerformClick();
            }
        }

        private async Task LoadHistoryAsync(string imei)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnSearch.Enabled = false;
                lblTitle.Text = $"360° Device Lifecycle - IMEI: {imei}";
                lblDeviceSummary.Text = "Loading details...";
                lblStatusBadge.Visible = false;

                var history = await _mobileShopApiService.GetImeiHistoryAsync(imei);
                if (history == null)
                {
                    MessageBox.Show($"No history found for IMEI '{imei}'.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblDeviceSummary.Text = "No history found for this IMEI.";
                    dgvTimeline.Rows.Clear();
                    dgvCosts.Rows.Clear();
                    return;
                }

                lblDeviceSummary.Text = $"{history.BrandTitle} {history.ModelName} | Total Landed Cost: Rs. {history.TotalCost:N0}";
                lblStatusBadge.Visible = true;
                if (history.IsCurrentlyInStock)
                {
                    lblStatusBadge.Text = "IN STOCK";
                    lblStatusBadge.BackColor = Color.ForestGreen;
                }
                else
                {
                    lblStatusBadge.Text = "SOLD / OUT";
                    lblStatusBadge.BackColor = Color.Crimson;
                }

                // Populate Timeline
                dgvTimeline.Rows.Clear();
                foreach (var ev in history.Timeline)
                {
                    dgvTimeline.Rows.Add(
                        ev.Date.ToString("dd-MMM-yyyy"),
                        ev.EventType,
                        ev.VoucherNo,
                        ev.PartyAccount,
                        ev.Rate.ToString("N0"),
                        ev.PtaStatus,
                        ev.Notes);
                }

                // Populate Cost Additions
                dgvCosts.Rows.Clear();
                foreach (var c in history.CostAdditions)
                {
                    dgvCosts.Rows.Add(
                        c.Date.ToString("dd-MMM-yyyy"),
                        c.CostType,
                        c.Amount.ToString("N0"),
                        c.Description,
                        c.AccountTitle);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading IMEI history: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblDeviceSummary.Text = "Error loading history.";
            }
            finally
            {
                btnSearch.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
