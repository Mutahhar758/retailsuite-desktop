using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP.Forms
{
    public partial class frmItemCategoryRates : Form
    {
        private readonly string _itemId;
        private readonly string _itemTitle;
        private readonly decimal _defaultPriRate;
        private readonly decimal? _defaultSecRate;
        private readonly InventoryApiService _inventoryApiService;
        private readonly CustomerCategoryApiService _customerCategoryApiService;
        private bool _isSaving;

        public List<ItemCustomerCategoryPriceUpsertRequest> ConfiguredRates { get; private set; }
        public decimal? EditedPriRate { get; private set; }
        public decimal? EditedSecSalePrice { get; private set; }
        public decimal? EditedMinSalePrice { get; private set; }
        private readonly List<ItemCustomerCategoryPriceUpsertRequest> _initialRates;
        private readonly bool _deferSave;
        private TextBox _txtPriRate;
        private TextBox _txtSecSalePrice;
        private TextBox _txtMinSalePrice;

        public frmItemCategoryRates(
            string itemId,
            string itemTitle,
            decimal defaultPriRate,
            decimal? defaultSecRate,
            List<ItemCustomerCategoryPriceUpsertRequest> initialRates = null,
            bool deferSave = false,
            decimal? minSalePrice = null,
            bool showSecondaryRate = true)
        {
            InitializeComponent();
            _itemId = itemId;
            _itemTitle = itemTitle;
            _defaultPriRate = defaultPriRate;
            _defaultSecRate = defaultSecRate;
            _initialRates = initialRates;
            _deferSave = deferSave;

            _inventoryApiService = new InventoryApiService();
            _customerCategoryApiService = new CustomerCategoryApiService();

            lblTitle.Text = string.IsNullOrWhiteSpace(_itemId) || _itemId == "0"
                ? $"Category Rates - {_itemTitle} (New Product)"
                : $"Category Rates - {_itemTitle} ({_itemId})";
            lblDefaults.Text = $"Default Rates: Primary = {_defaultPriRate:N2}" + (_defaultSecRate.HasValue ? $", Secondary = {_defaultSecRate.Value:N2}" : "");

            if (_deferSave)
            {
                InitializeBasePricePanel(defaultPriRate, defaultSecRate, minSalePrice, showSecondaryRate);
            }
        }

        private void InitializeBasePricePanel(decimal priRate, decimal? secRate, decimal? minSalePrice, bool showSecondaryRate)
        {
            var pnlBase = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = System.Drawing.Color.WhiteSmoke,
                Padding = new Padding(12, 6, 12, 6)
            };

            var lblPri = new Label { Text = "Sale Price:", AutoSize = true, Location = new System.Drawing.Point(14, 8) };
            _txtPriRate = new TextBox
            {
                Location = new System.Drawing.Point(14, 26),
                Width = 120,
                Text = priRate.ToString("0.##")
            };
            pnlBase.Controls.Add(lblPri);
            pnlBase.Controls.Add(_txtPriRate);

            int nextX = 150;
            if (showSecondaryRate)
            {
                var lblSec = new Label { Text = "Sec. Sale Price:", AutoSize = true, Location = new System.Drawing.Point(nextX, 8) };
                _txtSecSalePrice = new TextBox
                {
                    Location = new System.Drawing.Point(nextX, 26),
                    Width = 120,
                    Text = secRate.HasValue ? secRate.Value.ToString("0.##") : "0"
                };
                pnlBase.Controls.Add(lblSec);
                pnlBase.Controls.Add(_txtSecSalePrice);
                nextX += 136;
            }

            var lblMin = new Label { Text = "Min Sale Price:", AutoSize = true, Location = new System.Drawing.Point(nextX, 8) };
            _txtMinSalePrice = new TextBox
            {
                Location = new System.Drawing.Point(nextX, 26),
                Width = 120,
                Text = minSalePrice.HasValue ? minSalePrice.Value.ToString("0.##") : string.Empty
            };
            pnlBase.Controls.Add(lblMin);
            pnlBase.Controls.Add(_txtMinSalePrice);

            this.Controls.Add(pnlBase);
            pnlBase.BringToFront();
            dgvRates.BringToFront();
            this.Height += 56;
        }

        private async void frmItemCategoryRates_Load(object sender, EventArgs e)
        {
            await LoadCategoryRatesAsync();
        }

        private async Task LoadCategoryRatesAsync()
        {
            try
            {
                dgvRates.Rows.Clear();

                var categoriesTask = _customerCategoryApiService.GetLookupAsync();
                Task<List<ItemCustomerCategoryPriceDto>> existingRatesTask = null;

                if (!string.IsNullOrWhiteSpace(_itemId) && _itemId != "0")
                {
                    existingRatesTask = _inventoryApiService.GetItemCategoryPricesAsync(_itemId);
                    await Task.WhenAll(categoriesTask, existingRatesTask);
                }
                else
                {
                    await categoriesTask;
                }

                var categories = categoriesTask.Result ?? new List<CustomerCategoryLookupDto>();
                var existingRates = existingRatesTask?.Result ?? new List<ItemCustomerCategoryPriceDto>();

                var ratesDict = existingRates
                    .Where(r => !string.IsNullOrWhiteSpace(r.CustomerCategoryId))
                    .ToDictionary(r => r.CustomerCategoryId, StringComparer.OrdinalIgnoreCase);

                var initialDict = _initialRates != null
                    ? _initialRates.ToDictionary(r => r.CustomerCategoryId, StringComparer.OrdinalIgnoreCase)
                    : null;

                foreach (var cat in categories)
                {
                    string priRateStr = string.Empty;
                    string secRateStr = string.Empty;

                    if (initialDict != null && initialDict.TryGetValue(cat.Code, out var initR))
                    {
                        priRateStr = initR.PriRate > 0 ? initR.PriRate.ToString("0.##") : string.Empty;
                        secRateStr = initR.SecRate.HasValue ? initR.SecRate.Value.ToString("0.##") : string.Empty;
                    }
                    else if (ratesDict.TryGetValue(cat.Code, out var existing))
                    {
                        priRateStr = existing != null ? existing.PriRate.ToString("0.##") : string.Empty;
                        secRateStr = existing != null && existing.SecRate.HasValue ? existing.SecRate.Value.ToString("0.##") : string.Empty;
                    }

                    dgvRates.Rows.Add(cat.Code, cat.Title, priRateStr, secRateStr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading category rates: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving) return;

            _isSaving = true;
            btnSave.Enabled = false;

            try
            {
                var request = new List<ItemCustomerCategoryPriceUpsertRequest>();

                foreach (DataGridViewRow row in dgvRates.Rows)
                {
                    if (row.IsNewRow) continue;

                    string catCode = Convert.ToString(row.Cells[clnCatCode.Index].Value);
                    if (string.IsNullOrWhiteSpace(catCode)) continue;

                    string priRateStr = Convert.ToString(row.Cells[clnPriRate.Index].Value);
                    string secRateStr = Convert.ToString(row.Cells[clnSecRate.Index].Value);

                    if (decimal.TryParse(priRateStr, out decimal priRate) && priRate > 0)
                    {
                        decimal? secRate = null;
                        if (decimal.TryParse(secRateStr, out decimal secVal))
                            secRate = secVal;

                        request.Add(new ItemCustomerCategoryPriceUpsertRequest
                        {
                            CustomerCategoryId = catCode,
                            PriRate = priRate,
                            SecRate = secRate
                        });
                    }
                }

                ConfiguredRates = request;

                if (_deferSave)
                {
                    if (_txtPriRate != null && decimal.TryParse(_txtPriRate.Text, out decimal parsedPri))
                        EditedPriRate = parsedPri;
                    if (_txtSecSalePrice != null && decimal.TryParse(_txtSecSalePrice.Text, out decimal parsedSec))
                        EditedSecSalePrice = parsedSec;
                    if (_txtMinSalePrice != null && decimal.TryParse(_txtMinSalePrice.Text, out decimal parsedMin))
                        EditedMinSalePrice = parsedMin;
                }

                if (!_deferSave && !string.IsNullOrWhiteSpace(_itemId) && _itemId != "0")
                {
                    await _inventoryApiService.SaveItemCategoryPricesAsync(_itemId, request);
                    MessageBox.Show("Category rates saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (_deferSave)
                {
                    // Caller (e.g. Purchase form) will persist ConfiguredRates atomically on save.
                }
                else
                {
                    MessageBox.Show("Category rates configured. Please click Save on Item Detail to save your changes.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving category rates: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isSaving = false;
                btnSave.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
