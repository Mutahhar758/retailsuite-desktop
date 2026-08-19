using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmSupplyOrder : Form
    {
        private readonly SupplyOrderApiService _supplyOrderApiService;
        private readonly CustomerApiService _customerApiService;
        private List<CustomerDto> _customers = new List<CustomerDto>();
        private string _id = "0";
        private bool _loadingList;
        private bool _suppressSelectionChanged;
        private bool _isNewMode;

        public frmSupplyOrder()
        {
            InitializeComponent();
            _supplyOrderApiService = new SupplyOrderApiService();
            _customerApiService = new CustomerApiService();
            UserInfo.ApplyFormPermissions(this, AppResource.SupplyOrders);
        }

        private async void frmSupplyOrder_Load(object sender, EventArgs e)
        {
            try
            {
                await FillCustomersAsync();
                await LoadListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async System.Threading.Tasks.Task FillCustomersAsync()
        {
            _customers = await _customerApiService.GetAsync() ?? new List<CustomerDto>();
            clnCustomer.DataSource = _customers;
            clnCustomer.DisplayMember = "Title";
            clnCustomer.ValueMember = "Account";
        }

        private async System.Threading.Tasks.Task LoadListAsync()
        {
            _loadingList = true;
            var orders = await _supplyOrderApiService.GetAsync();
            dgvList.DataSource = orders;
            if (dgvList.Columns.Contains("Details")) dgvList.Columns["Details"].Visible = false;

            if (dgvList.Rows.Count > 0)
            {
                if (dgvList.CurrentRow == null)
                {
                    dgvList.ClearSelection();
                    dgvList.Rows[0].Selected = true;
                    dgvList.CurrentCell = dgvList.Rows[0].Cells[0];
                }

                await LoadCurrentSelectionAsync();
            }
            else
            {
                btnNew_Click(null, null);
            }

            _loadingList = false;
        }

        private void txtSearchCustomer_TextChanged(object sender, EventArgs e)
        {
            ApplyCustomerFilter();
        }

        private void ApplyCustomerFilter()
        {
            string q = (txtSearchCustomer.Text ?? "").Trim();
            dgvCustomers.CurrentCell = null;

            foreach (DataGridViewRow row in dgvCustomers.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (string.IsNullOrEmpty(q))
                {
                    row.Visible = true;
                    continue;
                }

                var customerId = row.Cells[clnCustomer.Index].Value?.ToString();
                var sortOrder = row.Cells[clnSortOrder.Index].Value?.ToString();

                var customer = _customers.FirstOrDefault(c => c.Account == customerId);
                var customerTitle = customer?.Title ?? "";

                bool isMatch = customerTitle.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                            || (customerId != null && customerId.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                            || (sortOrder != null && sortOrder.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0);

                row.Visible = isMatch;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            _suppressSelectionChanged = true;
            _isNewMode = true;
            try
            {
                if (dgvList.Rows.Count > 0)
                {
                    dgvList.ClearSelection();
                    dgvList.CurrentCell = null;
                }

                _id = "0";
                txtTitle.Text = "";
                txtSearchCustomer.Text = "";
                dgvCustomers.Rows.Clear();
                txtTitle.Focus();
            }
            finally
            {
                _suppressSelectionChanged = false;
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (txtTitle.Text.Trim() == "")
            {
                MessageBox.Show("Please enter a title.");
                return;
            }

            try
            {
                var request = new SupplyOrderUpsertApiRequest
                {
                    Title = txtTitle.Text.Trim(),
                    Details = BuildDetailRequests()
                };

                if (_id == "0")
                {
                    var createdId = await _supplyOrderApiService.CreateAsync(request);
                    _id = createdId.ToString();
                }
                else
                {
                    await _supplyOrderApiService.UpdateAsync(Convert.ToInt32(_id), request);
                }

                MessageBox.Show("Saved successfully.");
                await LoadListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_id == "0")
                return;

            if (MessageBox.Show("Are you sure you want to delete this order?", "Confirm Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                await _supplyOrderApiService.DeleteAsync(Convert.ToInt32(_id));
                MessageBox.Show("Deleted successfully.");
                await LoadListAsync();
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void dgvList_SelectionChanged(object sender, EventArgs e)
        {
            if (_loadingList || _suppressSelectionChanged || dgvList.CurrentRow == null)
                return;

            await LoadCurrentSelectionAsync();
        }

        private async System.Threading.Tasks.Task LoadCurrentSelectionAsync()
        {
            if (dgvList.CurrentRow == null)
                return;

            if (dgvList.CurrentRow.Cells["Id"].Value == null)
                return;

            _id = dgvList.CurrentRow.Cells["Id"].Value.ToString();
            _isNewMode = false;
            txtTitle.Text = dgvList.CurrentRow.Cells["Title"].Value != null
                ? dgvList.CurrentRow.Cells["Title"].Value.ToString()
                : string.Empty;

            txtSearchCustomer.Text = "";
            await LoadDetailsAsync(_id);
        }

        private async System.Threading.Tasks.Task LoadDetailsAsync(string id)
        {
            dgvCustomers.Rows.Clear();
            var order = await _supplyOrderApiService.GetByIdAsync(Convert.ToInt32(id));
            if (_isNewMode || _id != id)
                return;
            if (order == null || order.Details == null)
                return;

            foreach (var detail in order.Details)
            {
                int idx = dgvCustomers.Rows.Add();
                dgvCustomers.Rows[idx].Cells[clnCustomer.Index].Value = detail.CustomerId;
                dgvCustomers.Rows[idx].Cells[clnSortOrder.Index].Value = detail.SortOrder;
            }
        }

        private List<SupplyOrderDetailUpsertApiRequest> BuildDetailRequests()
        {
            var details = new List<SupplyOrderDetailUpsertApiRequest>();

            foreach (DataGridViewRow row in dgvCustomers.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Cells[clnCustomer.Index].Value == null)
                    continue;

                var customerId = row.Cells[clnCustomer.Index].Value.ToString();
                if (string.IsNullOrWhiteSpace(customerId))
                    continue;

                int sortOrder = 0;
                if (row.Cells[clnSortOrder.Index].Value != null)
                {
                    int.TryParse(row.Cells[clnSortOrder.Index].Value.ToString(), out sortOrder);
                }

                details.Add(new SupplyOrderDetailUpsertApiRequest
                {
                    CustomerId = customerId,
                    SortOrder = sortOrder
                });
            }

            return details;
        }
    }
}
