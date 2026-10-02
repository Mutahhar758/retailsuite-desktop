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
            if (string.IsNullOrWhiteSpace(_imei))
            {
                _imei = PromptImei();
                if (string.IsNullOrWhiteSpace(_imei))
                {
                    this.Close();
                    return;
                }
            }

            lblTitle.Text = $"360° Device Lifecycle - IMEI: {_imei}";
            await LoadHistoryAsync();
        }

        private string PromptImei()
        {
            using (var prompt = new Form())
            {
                prompt.Width = 380;
                prompt.Height = 160;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "Search IMEI Timeline";
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.MaximizeBox = false;
                prompt.MinimizeBox = false;

                var textLabel = new Label() { Left = 20, Top = 18, Text = "Enter IMEI / Serial Number:", AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
                var textBox = new TextBox() { Left = 20, Top = 45, Width = 320, Font = new Font("Segoe UI", 10f) };
                var confirmation = new Button() { Text = "Search", Left = 175, Width = 80, Top = 80, DialogResult = DialogResult.OK, BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                var cancel = new Button() { Text = "Cancel", Left = 265, Width = 75, Top = 80, DialogResult = DialogResult.Cancel };
                confirmation.Click += (s, ev) => { prompt.Close(); };
                cancel.Click += (s, ev) => { prompt.Close(); };
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(cancel);
                prompt.AcceptButton = confirmation;
                prompt.CancelButton = cancel;

                return prompt.ShowDialog(this) == DialogResult.OK ? textBox.Text.Trim() : "";
            }
        }

        private async Task LoadHistoryAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var history = await _mobileShopApiService.GetImeiHistoryAsync(_imei);
                if (history == null)
                {
                    MessageBox.Show("No history found for this IMEI.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                lblDeviceSummary.Text = $"{history.BrandTitle} {history.ModelName} | Total Landed Cost: Rs. {history.TotalCost:N0}";
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
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
