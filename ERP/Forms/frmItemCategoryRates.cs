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

        public frmItemCategoryRates(string itemId, string itemTitle, decimal defaultPriRate, decimal? defaultSecRate)
        {
            InitializeComponent();
            _itemId = itemId;
            _itemTitle = itemTitle;
            _defaultPriRate = defaultPriRate;
            _defaultSecRate = defaultSecRate;

            _inventoryApiService = new InventoryApiService();
            _customerCategoryApiService = new CustomerCategoryApiService();

            lblTitle.Text = $"Category Rates - {_itemTitle} ({_itemId})";
            lblDefaults.Text = $"Default Rates: Primary = {_defaultPriRate:N2}" + (_defaultSecRate.HasValue ? $", Secondary = {_defaultSecRate.Value:N2}" : "");
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
                var existingRatesTask = _inventoryApiService.GetItemCategoryPricesAsync(_itemId);

                await Task.WhenAll(categoriesTask, existingRatesTask);

                var categories = categoriesTask.Result ?? new List<CustomerCategoryLookupDto>();
                var existingRates = existingRatesTask.Result ?? new List<ItemCustomerCategoryPriceDto>();

                var ratesDict = existingRates
                    .Where(r => !string.IsNullOrWhiteSpace(r.CustomerCategoryId))
                    .ToDictionary(r => r.CustomerCategoryId, StringComparer.OrdinalIgnoreCase);

                foreach (var cat in categories)
                {
                    ratesDict.TryGetValue(cat.Code, out var existing);

                    string priRateStr = existing != null ? existing.PriRate.ToString("0.##") : string.Empty;
                    string secRateStr = existing != null && existing.SecRate.HasValue ? existing.SecRate.Value.ToString("0.##") : string.Empty;

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

                await _inventoryApiService.SaveItemCategoryPricesAsync(_itemId, request);
                MessageBox.Show("Category rates saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
