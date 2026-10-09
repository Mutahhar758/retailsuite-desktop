using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP.Forms
{
    public partial class frmRepairJob : Form
    {
        private readonly MobileShopApiService _mobileShopApiService;
        private readonly BrandApiService _brandApiService;
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private readonly InventoryApiService _inventoryApiService;
        private readonly HRInfoApiService _hrInfoApiService;

        private List<InventoryItemDto> _cachedItems = new List<InventoryItemDto>();
        private List<ChartOfAccountHeadDto> _customerAccounts = new List<ChartOfAccountHeadDto>();
        private List<ChartOfAccountHeadDto> _cashAccounts = new List<ChartOfAccountHeadDto>();
        private List<BrandDto> _brands = new List<BrandDto>();
        private List<HRInfoDto> _technicians = new List<HRInfoDto>();

        private string _loadedJobNo = "";
        private string _saleVoucherNo = "";

        public frmRepairJob()
        {
            InitializeComponent();
            _mobileShopApiService = new MobileShopApiService();
            _brandApiService = new BrandApiService();
            _chartOfAccountApiService = new ChartOfAccountApiService();
            _inventoryApiService = new InventoryApiService();
            _hrInfoApiService = new HRInfoApiService();
        }

        private async void frmRepairJob_Load(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                cmbStatus.SelectedItem = "Received";
                cmbListStatus.SelectedItem = "All";
                dtpListFrom.Value = DateTime.Today.AddDays(-30);
                dtpListTo.Value = DateTime.Today;

                await LoadDropdownsAsync();
                await RefreshJobListAsync();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing repair job module: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task LoadDropdownsAsync()
        {
            try
            {
                try
                {
                    _brands = await _brandApiService.GetListAsync();
                    if (_brands == null || _brands.Count == 0)
                    {
                        var lookup = await _brandApiService.GetLookupAsync();
                        _brands = lookup.Select(b => new BrandDto { Id = b.Id, Title = b.Title, Active = true }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading brands: " + ex.Message);
                    try
                    {
                        var lookup = await _brandApiService.GetLookupAsync();
                        _brands = lookup.Select(b => new BrandDto { Id = b.Id, Title = b.Title, Active = true }).ToList();
                    }
                    catch
                    {
                        _brands = new List<BrandDto>();
                    }
                }

                cmbBrand.DisplayMember = "Title";
                cmbBrand.ValueMember = "Id";
                cmbBrand.DataSource = _brands?.ToList() ?? new List<BrandDto>();

                try
                {
                    _customerAccounts = await _chartOfAccountApiService.GetCustomerAccountsAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading customers: " + ex.Message);
                    _customerAccounts = new List<ChartOfAccountHeadDto>();
                }
                cmbCustomer.DisplayMember = "Title";
                cmbCustomer.ValueMember = "Id";
                cmbCustomer.DataSource = _customerAccounts?.ToList() ?? new List<ChartOfAccountHeadDto>();

                try
                {
                    _cashAccounts = await _chartOfAccountApiService.GetCashBankAccountsAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading cash accounts: " + ex.Message);
                    _cashAccounts = new List<ChartOfAccountHeadDto>();
                }
                cmbAdvanceAccount.DisplayMember = "Title";
                cmbAdvanceAccount.ValueMember = "Id";
                cmbAdvanceAccount.DataSource = _cashAccounts?.ToList() ?? new List<ChartOfAccountHeadDto>();

                try
                {
                    _cachedItems = await _inventoryApiService.GetLookupAsync(null);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading items: " + ex.Message);
                    _cachedItems = new List<InventoryItemDto>();
                }

                try
                {
                    _technicians = await _hrInfoApiService.GetAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading technicians: " + ex.Message);
                    _technicians = new List<HRInfoDto>();
                }
                cmbTechnician.DisplayMember = "Name";
                cmbTechnician.ValueMember = "Id";
                var techList = new List<HRInfoDto> { new HRInfoDto { Id = "", Name = "-- None --" } };
                if (_technicians != null) techList.AddRange(_technicians);
                cmbTechnician.DataSource = techList;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading dropdowns: " + ex.Message);
            }
        }

        private void ResetForm()
        {
            _loadedJobNo = "";
            _saleVoucherNo = "";
            txtJobNo.Text = "";
            dtpJobDate.Value = DateTime.Today;
            cmbStatus.SelectedItem = "Received";
            if (cmbCustomer.Items.Count > 0) cmbCustomer.SelectedIndex = 0;
            txtCustomerPhone.Text = "";
            if (cmbBrand.Items.Count > 0) cmbBrand.SelectedIndex = 0;
            txtModel.Text = "";
            txtColor.Text = "";
            txtImei.Text = "";
            txtPasscode.Text = "";
            txtProblem.Text = "";
            txtPhysicalCondition.Text = "";
            txtAccessories.Text = "";
            if (cmbTechnician.Items.Count > 0) cmbTechnician.SelectedIndex = 0;
            txtTechnicianNotes.Text = "";

            txtEstimatedCost.Text = "0.00";
            txtAdvanceReceived.Text = "0.00";
            txtPartsTotal.Text = "0.00";
            txtServicesTotal.Text = "0.00";
            txtNetTotal.Text = "0.00";
            txtBalanceDue.Text = "0.00";
            lblSaleVoucher.Text = "";
            btnBillJob.Enabled = false;

            dgvParts.Rows.Clear();
            dgvServices.Rows.Clear();
            tcMain.SelectedTab = tpJobCard;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedItem == null && string.IsNullOrWhiteSpace(cmbCustomer.Text))
            {
                MessageBox.Show("Please select or enter customer name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCustomer.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtProblem.Text))
            {
                MessageBox.Show("Please enter the problem description / fault reported.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProblem.Focus();
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                var customerAccountId = cmbCustomer.SelectedValue != null ? cmbCustomer.SelectedValue.ToString() : "";
                var brandId = cmbBrand.SelectedValue != null ? cmbBrand.SelectedValue.ToString() : "";
                var techId = cmbTechnician.SelectedValue != null ? cmbTechnician.SelectedValue.ToString() : "";
                if (techId == "-- None --" || techId == "0") techId = "";

                decimal.TryParse(txtEstimatedCost.Text, out var estCost);
                decimal.TryParse(txtAdvanceReceived.Text, out var advRec);
                var advAccount = cmbAdvanceAccount.SelectedValue != null ? cmbAdvanceAccount.SelectedValue.ToString() : "";

                // Collect parts
                var parts = new List<RepairJobPartRequest>();
                foreach (DataGridViewRow row in dgvParts.Rows)
                {
                    if (row.IsNewRow) continue;
                    var itemId = row.Cells[0].Value?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(itemId)) continue;
                    decimal.TryParse(row.Cells[2].Value?.ToString(), out var qty);
                    decimal.TryParse(row.Cells[3].Value?.ToString(), out var price);
                    if (qty <= 0) qty = 1;

                    parts.Add(new RepairJobPartRequest
                    {
                        ItemId = itemId,
                        Qty = qty,
                        UnitPrice = price
                    });
                }

                // Collect services
                var services = new List<RepairJobServiceItemRequest>();
                foreach (DataGridViewRow row in dgvServices.Rows)
                {
                    if (row.IsNewRow) continue;
                    var desc = row.Cells[0].Value?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(desc)) continue;
                    decimal.TryParse(row.Cells[1].Value?.ToString(), out var amt);

                    services.Add(new RepairJobServiceItemRequest
                    {
                        ItemId = "",
                        Description = desc,
                        Amount = amt
                    });
                }

                if (string.IsNullOrWhiteSpace(_loadedJobNo))
                {
                    // Create
                    var createReq = new RepairJobCreateRequest
                    {
                        JobDate = dtpJobDate.Value.ToString("yyyy-MM-dd"),
                        CustomerAccountId = customerAccountId,
                        CustomerPhone = txtCustomerPhone.Text.Trim(),
                        BrandId = brandId,
                        ModelName = txtModel.Text.Trim(),
                        Color = txtColor.Text.Trim(),
                        Imei = txtImei.Text.Trim(),
                        Passcode = txtPasscode.Text.Trim(),
                        ProblemDescription = txtProblem.Text.Trim(),
                        PhysicalCondition = txtPhysicalCondition.Text.Trim(),
                        AccessoriesReceived = txtAccessories.Text.Trim(),
                        TechnicianNotes = txtTechnicianNotes.Text.Trim(),
                        TechnicianId = techId,
                        EstimatedCost = estCost,
                        AdvanceReceived = advRec,
                        AdvanceAccountId = advAccount,
                        Parts = parts,
                        Services = services
                    };

                    var newJobNo = await _mobileShopApiService.CreateRepairJobAsync(createReq);
                    _loadedJobNo = newJobNo;
                    txtJobNo.Text = newJobNo;
                    MessageBox.Show($"Repair Job #{newJobNo} saved successfully!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Update
                    var updateReq = new RepairJobUpdateRequest
                    {
                        CustomerAccountId = customerAccountId,
                        CustomerPhone = txtCustomerPhone.Text.Trim(),
                        BrandId = brandId,
                        ModelName = txtModel.Text.Trim(),
                        Color = txtColor.Text.Trim(),
                        Imei = txtImei.Text.Trim(),
                        Passcode = txtPasscode.Text.Trim(),
                        ProblemDescription = txtProblem.Text.Trim(),
                        PhysicalCondition = txtPhysicalCondition.Text.Trim(),
                        AccessoriesReceived = txtAccessories.Text.Trim(),
                        TechnicianNotes = txtTechnicianNotes.Text.Trim(),
                        TechnicianId = techId,
                        EstimatedCost = estCost,
                        Parts = parts,
                        Services = services
                    };

                    await _mobileShopApiService.UpdateRepairJobAsync(_loadedJobNo, updateReq);

                    // Update status if needed
                    var status = cmbStatus.SelectedItem?.ToString() ?? "Received";
                    await _mobileShopApiService.UpdateRepairJobStatusAsync(_loadedJobNo, new RepairJobStatusUpdateRequest
                    {
                        Status = status,
                        Notes = txtTechnicianNotes.Text.Trim()
                    });

                    MessageBox.Show($"Repair Job #{_loadedJobNo} updated successfully!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await RefreshJobListAsync();
                await LoadJobDetailAsync(_loadedJobNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving repair job: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task LoadJobDetailAsync(string jobNo)
        {
            if (string.IsNullOrWhiteSpace(jobNo)) return;
            Cursor = Cursors.WaitCursor;
            try
            {
                var job = await _mobileShopApiService.GetRepairJobDetailAsync(jobNo);
                if (job == null) return;

                _loadedJobNo = job.JobNo;
                txtJobNo.Text = job.JobNo;
                dtpJobDate.Value = job.JobDate;
                cmbStatus.SelectedItem = job.Status;

                if (!string.IsNullOrWhiteSpace(job.CustomerAccountId))
                    cmbCustomer.SelectedValue = job.CustomerAccountId;
                else
                    cmbCustomer.Text = job.CustomerAccountTitle;

                txtCustomerPhone.Text = job.CustomerPhone;

                if (!string.IsNullOrWhiteSpace(job.BrandId))
                    cmbBrand.SelectedValue = job.BrandId;
                txtModel.Text = job.ModelName;
                txtColor.Text = job.Color;
                txtImei.Text = job.Imei;
                txtPasscode.Text = job.Passcode;
                txtProblem.Text = job.ProblemDescription;
                txtPhysicalCondition.Text = job.PhysicalCondition;
                txtAccessories.Text = job.AccessoriesReceived;

                if (!string.IsNullOrWhiteSpace(job.TechnicianId))
                    cmbTechnician.SelectedValue = job.TechnicianId;
                else if (cmbTechnician.Items.Count > 0)
                    cmbTechnician.SelectedIndex = 0;

                txtTechnicianNotes.Text = job.TechnicianNotes;
                txtEstimatedCost.Text = job.EstimatedCost.ToString("F2");
                txtAdvanceReceived.Text = job.AdvanceReceived.ToString("F2");

                _saleVoucherNo = job.SaleVoucherNo ?? "";
                if (!string.IsNullOrWhiteSpace(_saleVoucherNo))
                {
                    lblSaleVoucher.Text = "Billed in Sale: " + _saleVoucherNo;
                    btnBillJob.Enabled = false;
                }
                else
                {
                    lblSaleVoucher.Text = "";
                    btnBillJob.Enabled = (job.Status == "Repaired" || job.Status == "Delivered");
                }

                // Populate parts
                dgvParts.Rows.Clear();
                foreach (var p in job.Parts)
                {
                    dgvParts.Rows.Add(p.ItemId, p.ItemTitle, p.Qty.ToString("0.##"), p.UnitPrice.ToString("F2"), p.TotalPrice.ToString("F2"));
                }

                // Populate services
                dgvServices.Rows.Clear();
                foreach (var s in job.Services)
                {
                    dgvServices.Rows.Add(s.Description, s.Amount.ToString("F2"));
                }

                CalculateTotals(null, null);
                tcMain.SelectedTab = tpJobCard;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading job details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void btnBillJob_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_loadedJobNo))
            {
                MessageBox.Show("Please save the job card before billing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(_saleVoucherNo))
            {
                MessageBox.Show($"This job is already billed in Sale Invoice #{_saleVoucherNo}.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dialog = MessageBox.Show($"Create Sale Invoice for Repair Job #{_loadedJobNo}?\n\nNet Total: Rs. {txtNetTotal.Text}\nAdvance Deducted: Rs. {txtAdvanceReceived.Text}\nBalance Due: Rs. {txtBalanceDue.Text}",
                "Confirm Billing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog != DialogResult.Yes) return;

            Cursor = Cursors.WaitCursor;
            try
            {
                var voucherNo = await _mobileShopApiService.BillRepairJobAsync(_loadedJobNo);
                _saleVoucherNo = voucherNo;
                lblSaleVoucher.Text = "Billed in Sale: " + voucherNo;
                btnBillJob.Enabled = false;
                cmbStatus.SelectedItem = "Delivered";

                MessageBox.Show($"Repair Job successfully converted to Sale Invoice: {voucherNo}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await RefreshJobListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating bill: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtJobNo.Text))
            {
                MessageBox.Show("Please save the job before printing claim slip.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var doc = new PrintDocument();
                doc.PrintPage += PrintClaimSlipPage;

                using (var preview = new PrintPreviewDialog())
                {
                    preview.Document = doc;
                    preview.StartPosition = FormStartPosition.CenterParent;
                    preview.Width = 600;
                    preview.Height = 700;
                    preview.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error preparing print document: " + ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintClaimSlipPage(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            float y = 20;
            float left = 20;
            float width = 280; // 80mm roll width approx 280-300px
            var fontTitle = new Font("Segoe UI", 12, FontStyle.Bold);
            var fontBold = new Font("Segoe UI", 9, FontStyle.Bold);
            var fontRegular = new Font("Segoe UI", 9, FontStyle.Regular);
            var fontSmall = new Font("Segoe UI", 8, FontStyle.Regular);
            var fontSmallBold = new Font("Segoe UI", 8, FontStyle.Bold);
            var sfCenter = new StringFormat { Alignment = StringAlignment.Center };
            var sfRight = new StringFormat { Alignment = StringAlignment.Far };

            // Header
            var compName = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "MOBILE CARE & REPAIRS";
            g.DrawString(compName, fontTitle, Brushes.Black, new RectangleF(left, y, width, 25), sfCenter);
            y += 24;

            if (!string.IsNullOrWhiteSpace(CompanyInfo.Address))
            {
                g.DrawString(CompanyInfo.Address, fontSmall, Brushes.Black, new RectangleF(left, y, width, 18), sfCenter);
                y += 18;
            }

            if (!string.IsNullOrWhiteSpace(CompanyInfo.ContactHead))
            {
                g.DrawString("Contact: " + CompanyInfo.ContactHead, fontSmall, Brushes.Black, new RectangleF(left, y, width, 18), sfCenter);
                y += 18;
            }

            g.DrawString("--------------------------------------------------", fontRegular, Brushes.Black, left, y);
            y += 15;
            g.DrawString("REPAIR JOB CARD / CLAIM SLIP", fontBold, Brushes.Black, new RectangleF(left, y, width, 20), sfCenter);
            y += 20;
            g.DrawString("--------------------------------------------------", fontRegular, Brushes.Black, left, y);
            y += 15;

            // Details
            g.DrawString("Job No: " + txtJobNo.Text, fontBold, Brushes.Black, left, y);
            g.DrawString("Date: " + dtpJobDate.Value.ToString("dd/MM/yyyy"), fontRegular, Brushes.Black, left + 150, y);
            y += 20;

            g.DrawString("Customer: " + cmbCustomer.Text, fontRegular, Brushes.Black, left, y);
            y += 18;
            g.DrawString("Phone: " + txtCustomerPhone.Text, fontRegular, Brushes.Black, left, y);
            y += 20;

            g.DrawString("--------------------------------------------------", fontRegular, Brushes.Black, left, y);
            y += 15;
            g.DrawString("Device: " + cmbBrand.Text + " " + txtModel.Text + " (" + txtColor.Text + ")", fontBold, Brushes.Black, left, y);
            y += 18;
            if (!string.IsNullOrWhiteSpace(txtImei.Text))
            {
                g.DrawString("IMEI/Serial: " + txtImei.Text, fontRegular, Brushes.Black, left, y);
                y += 18;
            }
            if (!string.IsNullOrWhiteSpace(txtPasscode.Text))
            {
                g.DrawString("Passcode/Pattern: " + txtPasscode.Text, fontRegular, Brushes.Black, left, y);
                y += 18;
            }
            if (!string.IsNullOrWhiteSpace(txtAccessories.Text))
            {
                g.DrawString("Accessories: " + txtAccessories.Text, fontRegular, Brushes.Black, left, y);
                y += 18;
            }
            if (!string.IsNullOrWhiteSpace(txtPhysicalCondition.Text))
            {
                g.DrawString("Condition: " + txtPhysicalCondition.Text, fontRegular, Brushes.Black, left, y);
                y += 18;
            }
            g.DrawString("Problem: " + txtProblem.Text, fontRegular, Brushes.Black, left, y);
            y += 25;

            g.DrawString("--------------------------------------------------", fontRegular, Brushes.Black, left, y);
            y += 15;

            // Financials
            g.DrawString("Estimated Cost:", fontRegular, Brushes.Black, left, y);
            g.DrawString("Rs. " + txtEstimatedCost.Text, fontBold, Brushes.Black, new RectangleF(left, y, width, 18), sfRight);
            y += 18;

            g.DrawString("Advance Paid:", fontRegular, Brushes.Black, left, y);
            g.DrawString("Rs. " + txtAdvanceReceived.Text, fontBold, Brushes.Black, new RectangleF(left, y, width, 18), sfRight);
            y += 18;

            g.DrawString("Balance Due:", fontBold, Brushes.Black, left, y);
            g.DrawString("Rs. " + txtBalanceDue.Text, fontBold, Brushes.Black, new RectangleF(left, y, width, 18), sfRight);
            y += 25;

            g.DrawString("--------------------------------------------------", fontRegular, Brushes.Black, left, y);
            y += 15;

            // Notice
            g.DrawString("TERMS & CONDITIONS:", fontSmallBold, Brushes.Black, left, y);
            y += 15;
            g.DrawString("1. Please present this original slip for receiving device.\n2. Shop is not responsible for uncollected devices after 30 days.\n3. Data backup is customer's own responsibility.", fontSmall, Brushes.Black, new RectangleF(left, y, width, 55));
            y += 60;

            g.DrawString("Cust. Sign: ____________   Shop Sign: ____________", fontSmall, Brushes.Black, left, y);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            var status = cmbStatus.SelectedItem?.ToString() ?? "";
            btnBillJob.Enabled = (string.IsNullOrWhiteSpace(_saleVoucherNo) && (status == "Repaired" || status == "Delivered"));
        }

        private void dgvParts_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvParts.Rows[e.RowIndex];

            // If item code entered, lookup title and price
            if (e.ColumnIndex == 0)
            {
                var itemId = row.Cells[0].Value?.ToString() ?? "";
                var found = _cachedItems.FirstOrDefault(i => string.Equals(i.Id, itemId, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(i.Barcode, itemId, StringComparison.OrdinalIgnoreCase));
                if (found != null)
                {
                    row.Cells[0].Value = found.Id;
                    row.Cells[1].Value = found.Title;
                    if (row.Cells[2].Value == null || string.IsNullOrWhiteSpace(row.Cells[2].Value.ToString()))
                        row.Cells[2].Value = "1";
                    row.Cells[3].Value = found.PriRate.ToString("F2");
                }
            }

            decimal.TryParse(row.Cells[2].Value?.ToString(), out var qty);
            decimal.TryParse(row.Cells[3].Value?.ToString(), out var price);
            var total = qty * price;
            row.Cells[4].Value = total.ToString("F2");

            CalculateTotals(sender, e);
        }

        private void dgvParts_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CalculateTotals(sender, e);
        }

        private void dgvServices_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CalculateTotals(sender, e);
        }

        private void dgvServices_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CalculateTotals(sender, e);
        }

        private void CalculateTotals(object sender, EventArgs e)
        {
            decimal partsTotal = 0;
            foreach (DataGridViewRow row in dgvParts.Rows)
            {
                if (row.IsNewRow) continue;
                decimal.TryParse(row.Cells[4].Value?.ToString(), out var rowTotal);
                partsTotal += rowTotal;
            }

            decimal servicesTotal = 0;
            foreach (DataGridViewRow row in dgvServices.Rows)
            {
                if (row.IsNewRow) continue;
                decimal.TryParse(row.Cells[1].Value?.ToString(), out var amt);
                servicesTotal += amt;
            }

            var netTotal = partsTotal + servicesTotal;
            decimal.TryParse(txtAdvanceReceived.Text, out var advance);
            var balanceDue = netTotal - advance;

            txtPartsTotal.Text = partsTotal.ToString("F2");
            txtServicesTotal.Text = servicesTotal.ToString("F2");
            txtNetTotal.Text = netTotal.ToString("F2");
            txtBalanceDue.Text = balanceDue.ToString("F2");
        }

        private async Task RefreshJobListAsync()
        {
            try
            {
                var status = cmbListStatus.SelectedItem?.ToString() ?? "All";
                if (status == "All") status = "";

                var jobs = await _mobileShopApiService.GetRepairJobsAsync(
                    fromDate: dtpListFrom.Value.ToString("yyyy-MM-dd"),
                    toDate: dtpListTo.Value.ToString("yyyy-MM-dd"),
                    status: status,
                    search: txtListSearch.Text.Trim()
                );

                dgvJobList.Rows.Clear();
                foreach (var j in jobs)
                {
                    dgvJobList.Rows.Add(
                        j.JobNo,
                        j.JobDate.ToString("dd/MM/yyyy"),
                        j.Status,
                        j.CustomerAccountTitle,
                        j.CustomerPhone,
                        $"{j.BrandTitle} {j.ModelName}".Trim(),
                        j.Imei,
                        j.NetTotal.ToString("F2"),
                        j.AdvanceReceived.ToString("F2"),
                        j.BalanceDue.ToString("F2")
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error refreshing job list: " + ex.Message);
            }
        }

        private async void btnListRefresh_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            await RefreshJobListAsync();
            Cursor = Cursors.Default;
        }

        private async void txtListSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Cursor = Cursors.WaitCursor;
                await RefreshJobListAsync();
                Cursor = Cursors.Default;
            }
        }

        private async void dgvJobList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var jobNo = dgvJobList.Rows[e.RowIndex].Cells[0].Value?.ToString();
            if (!string.IsNullOrWhiteSpace(jobNo))
            {
                await LoadJobDetailAsync(jobNo);
            }
        }

        private void frmRepairJob_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                btnNew_Click(sender, e);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnSave_Click(sender, e);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F5)
            {
                btnPrint_Click(sender, e);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                btnClose_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}
