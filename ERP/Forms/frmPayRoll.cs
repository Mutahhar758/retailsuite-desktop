using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmPayRoll : Form
    {
        private readonly PayrollApiService _payrollService;
        private List<PayrollVoucherDto> _queryList = new List<PayrollVoucherDto>();
        private List<PayrollEmployeeDto> _employees = new List<PayrollEmployeeDto>();

        public frmPayRoll()
        {
            InitializeComponent();
            _payrollService = new PayrollApiService();
            dgvPayroll.Rows.Add();
            UserInfo.ApplyFormPermissions(this, AppResource.Payrolls);
        }

        private int currentRow;
        private bool resetRow = false;
        bool FLogIn = true;

        void CalculateNetSalary(int RowInd)
        {
            decimal Salary = 0, NoOfLeaves = 0, LeaveCharges = 0, Overtime = 0, OvertimeCharges = 0, Bonus = 0, NetSalary = 0;
            if (!decimal.TryParse(Convert.ToString(dgvPayroll.Rows[RowInd].Cells[clnSalary.Index].Value), NumberStyles.Any, CultureInfo.CurrentCulture, out Salary))
                dgvPayroll.Rows[RowInd].Cells[clnSalary.Index].Value = Salary;
            if (!decimal.TryParse(Convert.ToString(dgvPayroll.Rows[RowInd].Cells[clnTotLeaves.Index].Value), NumberStyles.Any, CultureInfo.CurrentCulture, out NoOfLeaves))
                dgvPayroll.Rows[RowInd].Cells[clnTotLeaves.Index].Value = NoOfLeaves;
            if (!decimal.TryParse(Convert.ToString(dgvPayroll.Rows[RowInd].Cells[clnLeaveChg.Index].Value), NumberStyles.Any, CultureInfo.CurrentCulture, out LeaveCharges))
                dgvPayroll.Rows[RowInd].Cells[clnLeaveChg.Index].Value = LeaveCharges;
            if (!decimal.TryParse(Convert.ToString(dgvPayroll.Rows[RowInd].Cells[clnOvertime.Index].Value), NumberStyles.Any, CultureInfo.CurrentCulture, out Overtime))
                dgvPayroll.Rows[RowInd].Cells[clnOvertime.Index].Value = Overtime;
            if (!decimal.TryParse(Convert.ToString(dgvPayroll.Rows[RowInd].Cells[clnOvertimeCharges.Index].Value), NumberStyles.Any, CultureInfo.CurrentCulture, out OvertimeCharges))
                dgvPayroll.Rows[RowInd].Cells[clnOvertimeCharges.Index].Value = OvertimeCharges;
            if (!decimal.TryParse(Convert.ToString(dgvPayroll.Rows[RowInd].Cells[clnBonus.Index].Value), NumberStyles.Any, CultureInfo.CurrentCulture, out Bonus))
                dgvPayroll.Rows[RowInd].Cells[clnBonus.Index].Value = Bonus;
            NetSalary = Salary - (NoOfLeaves * LeaveCharges) + (Overtime * OvertimeCharges) + Bonus;
            dgvPayroll.Rows[RowInd].Cells[clnNetSalary.Index].Value = NetSalary.ToString("N0");
        }

        #region Fill Controls
        async System.Threading.Tasks.Task LoadLookupsAsync()
        {
            var lookups = await _payrollService.GetLookupsAsync();
            _employees = lookups.Employees ?? new List<PayrollEmployeeDto>();

            var employeeData = _employees
                .OrderBy(x => x.Name)
                .Select(x => new { ID = x.Id, Title = x.Name })
                .ToList();

            clnHRID.DataSource = employeeData;
            clnHRID.DisplayMember = "Title";
            clnHRID.ValueMember = "ID";

            clnExpense.DataSource = (lookups.ExpenseAccounts ?? new List<PayrollLookupItemDto>())
                .OrderBy(x => x.Title)
                .ToList();
            clnExpense.DisplayMember = "Title";
            clnExpense.ValueMember = "Code";

            clnPayableAccount.DataSource = (lookups.PayableAccounts ?? new List<PayrollLookupItemDto>())
                .OrderBy(x => x.Title)
                .ToList();
            clnPayableAccount.DisplayMember = "Title";
            clnPayableAccount.ValueMember = "Code";

            var existingItems = cmbSalaryType.Items.Cast<object>()
                .Select(x => Convert.ToString(x))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var salaryTypes = _employees
                .Where(x => !string.IsNullOrWhiteSpace(x.SalaryType))
                .Select(x => x.SalaryType)
                .Concat(existingItems)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            cmbSalaryType.Items.Clear();
            foreach (var salaryType in salaryTypes)
                cmbSalaryType.Items.Add(salaryType);
        }

        async System.Threading.Tasks.Task FillQueryAsync(string fromDate = "", string toDate = "")
        {
            dgvQuery.Rows.Clear();
            _queryList = await _payrollService.GetListAsync(fromDate, toDate);

            for (int i = 0; i < _queryList.Count; i++)
            {
                var row = _queryList[i];
                dgvQuery.Rows.Add(row.Date,
                    "PL-" + row.VoucherNo,
                    row.Amount.ToString("N0"),
                    row.SalaryType,
                    row.CreatedBy + " | " + row.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                    !string.IsNullOrWhiteSpace(row.LastModifiedBy) && row.LastModifiedOn.HasValue
                        ? row.LastModifiedBy + " | " + row.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                        : null);
            }
        }

        void FillQuery(string[] filter)
        {
            string fromDate = string.Empty;
            string toDate = string.Empty;

            if (filter != null)
            {
                foreach (var item in filter)
                {
                    if (string.IsNullOrWhiteSpace(item))
                        continue;

                    if (item.Contains("Vdate >="))
                        fromDate = item.Split('\'').Length > 1 ? item.Split('\'')[1] : string.Empty;
                    if (item.Contains("Vdate <="))
                        toDate = item.Split('\'').Length > 1 ? item.Split('\'')[1] : string.Empty;
                }
            }

            FillQueryAsync(fromDate, toDate);
        }

        internal async System.Threading.Tasks.Task FillPayrollAsync(string vno)
        {
            var lines = await _payrollService.GetDetailAsync(vno);
            txtVoucherNo.Text = "PL-" + vno;
            dgvPayroll.Rows.Clear();

            if (lines.Count > 0)
            {
                dtpDate.Value = lines[0].Date;
                cmbSalaryType.Text = lines[0].SalaryType;
                txtDescription.Text = lines[0].Description;
                txtCreatedBy.Text = lines[0].CreatedBy + " | " + lines[0].CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt");
                txtEditBy.Text = !string.IsNullOrWhiteSpace(lines[0].LastModifiedBy) && lines[0].LastModifiedOn.HasValue
                    ? lines[0].LastModifiedBy + " | " + lines[0].LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                    : null;

                for (int i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    dgvPayroll.Rows.Add(line.Seq.ToString(),
                        line.HrId,
                        line.PayableAccount,
                        line.ExpenseAccount,
                        line.Salary.ToString("N0"),
                        line.NoOfLeaves.ToString("N0"),
                        line.LeaveCharges.ToString("N0"),
                        line.Overtime.ToString("N0"),
                        line.OvertimeCharges.ToString("N0"),
                        line.Bonus.ToString("N0"),
                        line.NetSalary.ToString("N0"),
                        line.Remarks,
                        line.CreatedBy + " | " + line.CreatedOn.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                        !string.IsNullOrWhiteSpace(line.LastModifiedBy) && line.LastModifiedOn.HasValue
                            ? line.LastModifiedBy + " | " + line.LastModifiedOn.Value.ToString("dd-MMM-yyyy hh:mm:ss tt")
                            : null);
                }
            }

            CalcTotAmount();
        }

        internal void FillPayroll(string vno)
        {
            FillPayrollAsync(vno);
        }

        #endregion

        private async void frmReceipt_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadLookupsAsync();
                await FillQueryAsync();

                if (_queryList.Count > 0 && txtVoucherNo.Text == "")
                {
                    string voucherNo = _queryList[VoucherIndex].VoucherNo;
                    await FillPayrollAsync(voucherNo);
                }
                FLogIn = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payroll data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{tab}");
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            resetRow = true;
            currentRow = e.RowIndex;
            CalculateNetSalary(e.RowIndex);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (resetRow)
            {
                resetRow = false;
                dgvPayroll.CurrentCell = dgvPayroll.Rows[currentRow].Cells[dgvPayroll.CurrentCell.ColumnIndex];
            }
        }

        void CalcTotAmount()
        {
            decimal Tot = 0;
            try
            {
                Tot = (from DataGridViewRow row in dgvPayroll.Rows
                       where row.Cells[clnNetSalary.Index].Value != null
                       select ParseDecimal(row.Cells[clnNetSalary.Index].Value)).Sum();
            }
            catch
            {
            }
            lblTotAmount.Text = Tot.ToString();
        }

        private decimal ParseDecimal(object value)
        {
            decimal parsed;
            return decimal.TryParse(Convert.ToString(value), NumberStyles.Any, CultureInfo.CurrentCulture, out parsed) ? parsed : 0;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvPayroll.CurrentCell.ColumnIndex == clnSalary.Index ||
                dgvPayroll.CurrentCell.ColumnIndex == clnTotLeaves.Index ||
                dgvPayroll.CurrentCell.ColumnIndex == clnLeaveChg.Index ||
                dgvPayroll.CurrentCell.ColumnIndex == clnOvertime.Index ||
                dgvPayroll.CurrentCell.ColumnIndex == clnOvertimeCharges.Index ||
                dgvPayroll.CurrentCell.ColumnIndex == clnBonus.Index)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null && e.Control.Text != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);
                }
            }
        }

        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvPayroll.CurrentCell.ColumnIndex == clnSalary.Index ||
                dgvPayroll.CurrentCell.ColumnIndex == clnTotLeaves.Index ||
                dgvPayroll.CurrentCell.ColumnIndex == clnLeaveChg.Index ||
                dgvPayroll.CurrentCell.ColumnIndex == clnOvertime.Index ||
                dgvPayroll.CurrentCell.ColumnIndex == clnOvertimeCharges.Index ||
                dgvPayroll.CurrentCell.ColumnIndex == clnBonus.Index)
            {
                if ((sender as TextBox).SelectedText.Length > 0)
                {
                    int selind = (sender as TextBox).SelectionStart;
                    (sender as TextBox).Text = (sender as TextBox).Text.Replace((sender as TextBox).SelectedText, "");
                    (sender as TextBox).SelectionStart = selind;
                    (sender as TextBox).SelectionLength = 0;
                }
                if (!char.IsControl(e.KeyChar)
           && !char.IsDigit(e.KeyChar)
           && !((sender as TextBox).Text.Count(a => a == '.') == 0 && e.KeyChar == '.'))
                {
                    e.Handled = true;
                }
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CalcTotAmount();
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int seq = 0;
            try
            {
                seq = (int)(from DataGridViewRow row in dgvPayroll.Rows
                            where row.Cells[clnSeq.Index].Value != null && row.Cells[clnSeq.Index].Value.ToString() != ""
                            select int.Parse(row.Cells[clnSeq.Index].Value.ToString())).Max();
            }
            catch
            {
            }
            if (dgvPayroll.Rows[e.RowIndex].Cells[clnSeq.Index].Value == null)
                dgvPayroll.Rows[e.RowIndex].Cells[clnSeq.Index].Value = (seq + 1).ToString();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private List<PayrollLineApiRequest> BuildLines()
        {
            var lines = new List<PayrollLineApiRequest>();

            foreach (DataGridViewRow row in dgvPayroll.Rows)
            {
                if (row.Cells[clnHRID.Index].Value == null ||
                    row.Cells[clnExpense.Index].Value == null ||
                    row.Cells[clnNetSalary.Index].Value == null)
                    continue;

                long seq = 0;
                long.TryParse(Convert.ToString(row.Cells[clnSeq.Index].Value), out seq);
                if (seq == 0)
                    continue;

                lines.Add(new PayrollLineApiRequest
                {
                    Seq = seq,
                    HrId = Convert.ToString(row.Cells[clnHRID.Index].Value),
                    PayableAccount = Convert.ToString(row.Cells[clnPayableAccount.Index].Value),
                    ExpenseAccount = Convert.ToString(row.Cells[clnExpense.Index].Value),
                    Salary = ParseDecimal(row.Cells[clnSalary.Index].Value),
                    NoOfLeaves = ParseDecimal(row.Cells[clnTotLeaves.Index].Value),
                    LeaveCharges = ParseDecimal(row.Cells[clnLeaveChg.Index].Value),
                    Overtime = ParseDecimal(row.Cells[clnOvertime.Index].Value),
                    OvertimeCharges = ParseDecimal(row.Cells[clnOvertimeCharges.Index].Value),
                    Bonus = ParseDecimal(row.Cells[clnBonus.Index].Value),
                    NetSalary = ParseDecimal(row.Cells[clnNetSalary.Index].Value),
                    Remarks = Convert.ToString(row.Cells[clnRemarks.Index].Value)
                });
            }

            return lines;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to save this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            if (!UserInfo.HasPermission(AppAction.Update, AppResource.Payrolls) && txtVoucherNo.Text != "")
            {
                frmAuthentication frm = new frmAuthentication();
                if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
            }

            if (string.IsNullOrWhiteSpace(cmbSalaryType.Text))
                return;

            var lines = BuildLines();
            if (lines.Count == 0)
            {
                MessageBox.Show("Please enter at least one payroll line.");
                return;
            }

            try
            {
                var request = new PayrollUpsertApiRequest
                {
                    Date = dtpDate.Value.ToString("yyyy-MM-dd"),
                    SalaryType = cmbSalaryType.Text,
                    Description = txtDescription.Text,
                    Lines = lines
                };

                string voucher = txtVoucherNo.Text == "" ? null : txtVoucherNo.Text.Split('-')[1];
                if (string.IsNullOrWhiteSpace(voucher))
                    voucher = await _payrollService.CreateAsync(request);
                else
                    await _payrollService.UpdateAsync(voucher, request);

                await FillQueryAsync();
                await FillPayrollAsync(voucher);

                MessageBox.Show("Record Successfully Saved..!");
                btnNew.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving payroll: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void dgvQuery_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!FLogIn && e.RowIndex != -1)
            {
                string voucherNo = dgvQuery.Rows[e.RowIndex].Cells[clnVoucherNum.Index].Value.ToString();
                tabDetailQuery.SelectedTab = tabpgDetail;
                await FillPayrollAsync(voucherNo.Substring(3));
                VoucherIndex = e.RowIndex;
            }
        }

        private async void btnFind_Click(object sender, EventArgs e)
        {
            string FDate = txtFdate.Text, TDate = txtTdate.Text;
            await FillQueryAsync(FDate, TDate);
        }

        private void txtFdate_Validated(object sender, EventArgs e)
        {
            try
            {
                string text = (sender as TextBox).Text.Replace(" ", "-").Replace("/", "-");
                DateTime date = DateTime.ParseExact(text, Validation.dateformats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                (sender as TextBox).Text = date.ToString("dd-MMM-yyyy");
            }
            catch (Exception)
            {
                (sender as TextBox).Text = "";
            }
        }

        private async void dgvQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvQuery.CurrentRow != null)
                {
                    string voucherNo = dgvQuery.CurrentRow.Cells[clnVoucherNum.Index].Value.ToString();
                    tabDetailQuery.SelectedTab = tabpgDetail;
                    await FillPayrollAsync(voucherNo.Substring(3));
                    VoucherIndex = dgvQuery.CurrentRow.Index;
                    e.Handled = true;
                }
            }
        }

        enum Navigators { Up, Down, Home, End };
        int VoucherIndex = 0;

        async System.Threading.Tasks.Task NavigateAsync(Navigators Nav)
        {
            if (dgvQuery.CurrentRow != null && tabDetailQuery.SelectedTab == tabpgDetail && _queryList.Count > 0)
            {
                if (Nav == Navigators.Up && 0 < VoucherIndex)
                {
                    VoucherIndex--;
                    await FillPayrollAsync(_queryList[VoucherIndex].VoucherNo);
                }
                else if (Nav == Navigators.Down && _queryList.Count - 1 > VoucherIndex)
                {
                    VoucherIndex++;
                    await FillPayrollAsync(_queryList[VoucherIndex].VoucherNo);
                }
                else if (Nav == Navigators.End)
                {
                    VoucherIndex = 0;
                    await FillPayrollAsync(_queryList[VoucherIndex].VoucherNo);
                }
                else if (Nav == Navigators.Home)
                {
                    VoucherIndex = _queryList.Count - 1;
                    await FillPayrollAsync(_queryList[VoucherIndex].VoucherNo);
                }
            }
        }

        private async void tabDetailQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown)
            {
                await NavigateAsync(Navigators.Down);
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                await NavigateAsync(Navigators.Up);
            }
            else if (e.KeyCode == Keys.Home)
            {
                await NavigateAsync(Navigators.Home);
            }
            else if (e.KeyCode == Keys.End)
            {
                await NavigateAsync(Navigators.End);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            dgvPayroll.Rows.Clear();
            Validation.Clear(grpInvoiceDetail);
            dgvPayroll.Rows.Add();
            dtpDate.Focus();
        }

        #region Navigation
        private async void btnHome_Click(object sender, EventArgs e)
        {
            await NavigateAsync(Navigators.Home);
        }

        private async void btnPri_Click(object sender, EventArgs e)
        {
            await NavigateAsync(Navigators.Up);
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            await NavigateAsync(Navigators.Down);
        }

        private async void btnEnd_Click(object sender, EventArgs e)
        {
            await NavigateAsync(Navigators.End);
        }
        #endregion

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtVoucherNo.Text != "")
            {
                if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Delete this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        string voucherNo = txtVoucherNo.Text.Substring(3);
                        await _payrollService.DeleteAsync(voucherNo);

                        DataGridViewRow row = dgvQuery.Rows.Cast<DataGridViewRow>()
                            .Where(r => r.Cells[clnVoucherNum.Index].Value.ToString().Equals(txtVoucherNo.Text))
                            .FirstOrDefault();

                        btnNew_Click(null, null);
                        if (row != null) dgvQuery.Rows.Remove(row);
                        await FillQueryAsync();
                        CalcTotAmount();
                        MessageBox.Show("Record Successfully Deleted..!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting payroll: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void deleteRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Delete this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (dgvPayroll[clnSeq.Index, dgvPayroll.CurrentRow.Index].Value != null)
                {
                    try
                    {
                        if (txtVoucherNo.Text != "")
                        {
                            var seq = Convert.ToInt64(dgvPayroll[clnSeq.Index, dgvPayroll.CurrentRow.Index].Value);
                            await _payrollService.DeleteLineAsync(txtVoucherNo.Text.Substring(3), seq);
                        }

                        dgvPayroll.Rows.RemoveAt(dgvPayroll.CurrentRow.Index);
                        CalcTotAmount();
                        MessageBox.Show("Record Successfully Deleted..!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting payroll line: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void dgvExpenses_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dgvPayroll.Rows[e.RowIndex].Selected = true;
                dgvPayroll.CurrentCell = this.dgvPayroll.Rows[e.RowIndex].Cells[e.ColumnIndex];
                contextMenuStrip1.Show(Cursor.Position);
            }

        }

        private void frmPayments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !dgvPayroll.Focused)
            {
                SendKeys.Send("{tab}");
            }
        }

        private async void addNewAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChartOfAccLvl5 frm = new frmChartOfAccLvl5();
            frm.AccountHead = AccountHead.Farmer;
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                await LoadLookupsAsync();
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (txtVoucherNo.Text == "")
            {
                var selectedSalaryType = cmbSalaryType.Text;
                var records = _employees
                    .Where(x => string.Equals(x.SalaryType, selectedSalaryType, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.Id)
                    .ToList();

                dgvPayroll.Rows.Clear();
                for (int i = 0; i < records.Count; i++)
                {
                    var emp = records[i];
                    int ind = dgvPayroll.Rows.Add((i + 1).ToString(),
                        emp.Id,
                        emp.PayableAccount,
                        emp.ExpenseAccount,
                        emp.Salary.ToString("N0"),
                        0,
                        emp.LeaveCharges.ToString("N0"),
                        0,
                        emp.Overtime.ToString("N0"),
                        0);
                    CalculateNetSalary(ind);
                }
            }
        }

        private async void btnPrint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtVoucherNo.Text))
                return;

            try
            {
                // Extract voucher number (remove prefix if present)
                string voucherNo = txtVoucherNo.Text.Length > 3 
                    ? txtVoucherNo.Text.Substring(3) 
                    : txtVoucherNo.Text;

                // Fetch payroll detail from API instead of direct SQL
                var payrollDetails = await _payrollService.GetDetailAsync(voucherNo);

                if (payrollDetails == null || payrollDetails.Count == 0)
                {
                    MessageBox.Show("No payroll record found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Create DataTable from API response
                DataTable dt = BuildPayrollDataTable(payrollDetails);

                // Initialize and configure report
                Reports.rpt_Payroll rpt = new Reports.rpt_Payroll();
                rpt.SetDataSource(dt);
                rpt.SetParameterValue("@CompanyName", CompanyInfo.CompanyName);
                rpt.SetParameterValue("@VoucherNo", txtVoucherNo.Text);
                rpt.SetParameterValue("@Vdate", payrollDetails[0].Date);

                // Display report
                frmReportView frmrpt = new frmReportView();
                frmrpt.rptViewer.ReportSource = rpt;
                frmrpt.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable BuildPayrollDataTable(List<PayrollDetailDto> payrollDetails)
        {
            DataTable dt = new DataTable();

            // Define columns matching the original query
            dt.Columns.Add("vdate", typeof(DateTime));
            dt.Columns.Add("hrname", typeof(string));
            dt.Columns.Add("Salary", typeof(decimal));
            dt.Columns.Add("NoOfLeave", typeof(decimal));
            dt.Columns.Add("LeaveCharges", typeof(decimal));
            dt.Columns.Add("Overtime", typeof(decimal));
            dt.Columns.Add("OvertimeCharges", typeof(decimal));
            dt.Columns.Add("Bonus", typeof(decimal));
            dt.Columns.Add("NetSalary", typeof(decimal));
            dt.Columns.Add("netpayable", typeof(decimal)); // Note: This would need to be fetched separately if account balance is required

            // Populate rows from API response
            foreach (var detail in payrollDetails)
            {
                dt.Rows.Add(
                    detail.Date, // DateOnly will be implicitly converted to DateTime
                    detail.HrName ?? string.Empty,
                    detail.Salary,
                    detail.NoOfLeaves,
                    detail.LeaveCharges,
                    detail.Overtime,
                    detail.OvertimeCharges,
                    detail.Bonus,
                    detail.NetSalary,
                    0m // TODO: Implement account balance lookup if needed
                );
            }

            return dt;
        }
    }
}



