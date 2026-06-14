using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ERP.Classes;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmItemDetail : Form
    {
        DataTable dtUnits = new DataTable();
        DataTable dtCatagory = new DataTable();
        DataTable dtItemDetail = new DataTable();
        private readonly InventoryApiService _inventoryApiService;
        private readonly ItemCategoryApiService _itemCategoryApiService;
        private readonly UnitApiService _unitApiService;
        bool FLogin = true;

        public frmItemDetail()
        {
            InitializeComponent();
            _inventoryApiService = new InventoryApiService();
            _itemCategoryApiService = new ItemCategoryApiService();
            _unitApiService = new UnitApiService();
        }
        void AllowNewRow()
        {
            if (dgvItemDetail.CurrentRow.Index == dgvItemDetail.Rows[dgvItemDetail.Rows.Count - 1].Index)
            {
                if (dgvItemDetail.CurrentRow.Cells[clnItem.Index].Value != null &&
                    dgvItemDetail.CurrentRow.Cells[clnPriRate.Index].Value != null &&
                    dgvItemDetail.CurrentRow.Cells[clnSecRate.Index].Value != null)
                    dgvItemDetail.Rows.Add();
            }
        }
        async System.Threading.Tasks.Task fillItemCatagory()
        {
            var categories = await _itemCategoryApiService.GetActiveAsync();

            dtCatagory = new DataTable();
            dtCatagory.Columns.Add("Code", typeof(string));
            dtCatagory.Columns.Add("Title", typeof(string));

            for (int i = 0; i < categories.Count; i++)
            {
                dtCatagory.Rows.Add(categories[i].Code, categories[i].Title);
            }

            cmbItemCatagory.DataSource = dtCatagory;
            cmbItemCatagory.DisplayMember = "Title";
            cmbItemCatagory.ValueMember = "Code";

            clnCatagory.DataSource = dtCatagory.Copy();
            clnCatagory.DisplayMember = "Title";
            clnCatagory.ValueMember = "Code";
        }
        async System.Threading.Tasks.Task LoadUnitsAsync()
        {
            var units = await _unitApiService.GetActiveAsync();

            dtUnits = new DataTable();
            dtUnits.Columns.Add("Code", typeof(string));
            dtUnits.Columns.Add("Title", typeof(string));

            for (int i = 0; i < units.Count; i++)
            {
                dtUnits.Rows.Add(units[i].Code, units[i].Title);
            }
        }
        void FillUnits()
        {
            clnPriUnit.DataSource = dtUnits.Copy();
            clnPriUnit.DisplayMember = "Title";
            clnPriUnit.ValueMember = "Code";
            clnSecUnit.DataSource = dtUnits.Copy();
            clnSecUnit.DisplayMember = "Title";
            clnSecUnit.ValueMember = "Code";
        }
        async System.Threading.Tasks.Task LoadItemDetailAsync()
        {
            var items = await _inventoryApiService.GetItemsAsync(null);

            dtItemDetail = new DataTable();
            dtItemDetail.Columns.Add("Id", typeof(string));
            dtItemDetail.Columns.Add("Barcode", typeof(string));
            dtItemDetail.Columns.Add("fkitemcatagory", typeof(string));
            dtItemDetail.Columns.Add("Title", typeof(string));
            dtItemDetail.Columns.Add("ItemKey", typeof(string));
            dtItemDetail.Columns.Add("PriRate", typeof(decimal));
            dtItemDetail.Columns.Add("SecRate", typeof(decimal));
            dtItemDetail.Columns.Add("PrimaryUnit", typeof(string));
            dtItemDetail.Columns.Add("SecondaryUnit", typeof(string));
            dtItemDetail.Columns.Add("DefaultUnit", typeof(string));
            dtItemDetail.Columns.Add("QtyInPack", typeof(decimal));
            dtItemDetail.Columns.Add("Alert", typeof(string));
            dtItemDetail.Columns.Add("LowStockAlert", typeof(decimal));
            dtItemDetail.Columns.Add("OpnStock", typeof(decimal));
            dtItemDetail.Columns.Add("OpnRate", typeof(decimal));
            dtItemDetail.Columns.Add("status", typeof(string));

            for (int i = 0; i < items.Count; i++)
            {
                dtItemDetail.Rows.Add(
                    items[i].Id,
                    items[i].Barcode,
                    items[i].ItemCategoryCode,
                    items[i].Title,
                    items[i].ItemKey,
                    items[i].PriRate,
                    items[i].SecRate,
                    items[i].PrimaryUnit,
                    items[i].SecondaryUnit,
                    items[i].DefaultUnit,
                    items[i].QtyInPack.HasValue ? (object)items[i].QtyInPack.Value : DBNull.Value,
                    items[i].Alert ? "1" : "0",
                    items[i].LowStockAlert.HasValue ? (object)items[i].LowStockAlert.Value : DBNull.Value,
                    items[i].OpnStock.HasValue ? (object)items[i].OpnStock.Value : DBNull.Value,
                    items[i].OpnRate.HasValue ? (object)items[i].OpnRate.Value : DBNull.Value,
                    "0");
            }
        }
        void Fillform(string catagory)
        {
            DataTable dtfilter = dtItemDetail.Copy();
            dtfilter.Rows.Clear();
            dtfilter = dtItemDetail.Select("fkitemcatagory = '" + catagory + "'").Count() > 0 ? dtItemDetail.Select("fkitemcatagory = '" + catagory + "'").CopyToDataTable() : dtfilter;
            dgvItemDetail.Rows.Clear();
            for (int i = 0; i < dtfilter.Rows.Count; i++)
            {
                dgvItemDetail.Rows.Add(dtfilter.Rows[i]["Id"].ToString(),
                    dtfilter.Rows[i]["Barcode"].ToString(),
                    dtfilter.Rows[i]["fkitemcatagory"].ToString(),
                    dtfilter.Rows[i]["Title"].ToString(),
                    dtfilter.Rows[i]["ItemKey"].ToString(),
                    dtfilter.Rows[i]["PriRate"].ToString(),
                    dtfilter.Rows[i]["SecRate"].ToString(),
                    dtfilter.Rows[i]["PrimaryUnit"].ToString().EmptyToNull(),
                    dtfilter.Rows[i]["SecondaryUnit"].ToString().EmptyToNull(),
                    dtfilter.Rows[i]["DefaultUnit"].ToString().EmptyToNull(),
                    dtfilter.Rows[i]["QtyInPack"].ToString().EmptyToNull(),
                    Validation.ToBool(dtfilter.Rows[i]["Alert"].ToString()),
                    dtfilter.Rows[i]["LowStockAlert"].ToString().EmptyToNull(),
                    dtfilter.Rows[i]["OpnStock"].ToString().EmptyToNull(),
                    dtfilter.Rows[i]["OpnRate"].ToString().EmptyToNull(),
                    dtfilter.Rows[i]["status"].ToString(),
                    "0");
            }
            dgvItemDetail.Rows.Add();
            CalcTotals();
        }
        private async void frmItemDetail_Load(object sender, EventArgs e)
        {
            await LoadUnitsAsync();
            await fillItemCatagory();
            await LoadItemDetailAsync();
            FillUnits();
            dgvItemDetail.Rows.Add();
            Fillform((string)cmbItemCatagory.SelectedValue);
            FLogin = false;
            if (UserInfo.UserId != "Admin" )
            {
                clnOpnStock.ReadOnly = true;
            }
        }
      
        #region dgvItemDetail Events
        private int currentRow;
        private bool resetRow = false;
        private void dgvItemDetail_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AllowNewRow();
            resetRow = true;
            currentRow = e.RowIndex;
            
        }
        private void dgvItemDetail_SelectionChanged(object sender, EventArgs e)
        {
            if (resetRow)
            {
                resetRow = false;     
                dgvItemDetail.CurrentCell = dgvItemDetail.Rows[currentRow].Cells[dgvItemDetail.CurrentCell.ColumnIndex];
            }
        }
        private void dgvItemDetail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{tab}");
            }
        }
        private void dgvItemDetail_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvItemDetail.CurrentCell.ColumnIndex == clnPriRate.Index || dgvItemDetail.CurrentCell.ColumnIndex == clnSecRate.Index)
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
            if (dgvItemDetail.CurrentCell.ColumnIndex == clnPriRate.Index || dgvItemDetail.CurrentCell.ColumnIndex == clnSecRate.Index)
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
        private void dgvItemDetail_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (!FLogin && dgvItemDetail.CurrentRow != null)
                {
                    dgvItemDetail.CurrentRow.Cells[clnIsEdit.Index].Value = "1";
                }
                DataGridViewComboBoxCell cmb = (dgvItemDetail[clnDefUnif.Index, e.RowIndex] as DataGridViewComboBoxCell);
                string Filter = " Code in (";
                Filter += "'" + Validation.IsNull((string)dgvItemDetail[clnPriUnit.Index, e.RowIndex].Value, "") + "'";
                Filter += ",'" + Validation.IsNull((string)dgvItemDetail[clnSecUnit.Index, e.RowIndex].Value, "") + "')";
                DataTable dt = dtUnits.Copy();
                dt.Rows.Clear(); 
                dt = dtUnits.Select(Filter).Count() == 0 ? null : dtUnits.Select(Filter).CopyToDataTable();
                cmb.DataSource = dt;
                clnDefUnif.DisplayMember = "Title";
                clnDefUnif.ValueMember = "Code";
                if (dgvItemDetail[clnPriUnit.Index, e.RowIndex].Value == null)
                    dgvItemDetail[clnPriUnit.Index, e.RowIndex].Value = dgvItemDetail[clnSecUnit.Index, e.RowIndex].Value;
                if (dgvItemDetail[clnSecUnit.Index, e.RowIndex].Value == null)
                    dgvItemDetail[clnSecUnit.Index, e.RowIndex].Value = dgvItemDetail[clnPriUnit.Index, e.RowIndex].Value;              
                if(cmb.Value == null)
                cmb.Value = dgvItemDetail[clnSecUnit.Index, e.RowIndex].Value;

            }
        }
        private void dgvItemDetail_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dgvItemDetail.Rows[e.RowIndex].Cells[clnCatagory.Index].Value = cmbItemCatagory.SelectedValue;
            dgvItemDetail.Rows[e.RowIndex].Cells[clnIsEdit.Index].Value = "1";
            dgvItemDetail.Rows[e.RowIndex].Cells[clnstatus.Index].Value = "0";
            if (dgvItemDetail.Rows[e.RowIndex].Cells[clnId.Index].Value == null)
             dgvItemDetail.Rows[e.RowIndex].Cells[clnId.Index].Value = "0";           
            
        }
        #endregion
        

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Save this...!", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dgvItemDetail.Rows)
                {
                    if (row.Cells[clnCatagory.Index].Value != null &&
                        row.Cells[clnItem.Index].Value != null &&
                        row.Cells[clnPriRate.Index].Value != null &&
                        row.Cells[clnSecRate.Index].Value != null &&
                        row.Cells[clnPriUnit.Index].Value != null && 
                        row.Cells[clnIsEdit.Index].Value.ToString()   == "1")
                                                {
                            var request = new InventoryItemUpsertApiRequest
                            {
                                Id = Convert.ToString(row.Cells[clnId.Index].Value),
                                ItemCategoryCode = Convert.ToString(row.Cells[clnCatagory.Index].Value),
                                Barcode = Convert.ToString(row.Cells[clnBarcode.Index].Value),
                                Title = Convert.ToString(row.Cells[clnItem.Index].Value),
                                ItemKey = Convert.ToString(row.Cells[clnKey.Index].Value),
                                PriRate = ParseDecimal(row.Cells[clnPriRate.Index].Value),
                                SecRate = ParseDecimal(row.Cells[clnSecRate.Index].Value),
                                PrimaryUnit = Convert.ToString(row.Cells[clnPriUnit.Index].Value),
                                SecondaryUnit = Convert.ToString(row.Cells[clnSecUnit.Index].Value),
                                DefaultUnit = Convert.ToString(row.Cells[clnDefUnif.Index].Value),
                                QtyInPack = ParseNullableDecimal(row.Cells[ClnQtyInPack.Index].Value),
                                Alert = Validation.ToBool(row.Cells[clnAlert.Index].Value),
                                LowStockAlert = ParseNullableDecimal(row.Cells[clnLowStockAlert.Index].Value),
                                OpnStock = ParseNullableDecimal(row.Cells[clnOpnStock.Index].Value),
                                OpnRate = ParseNullableDecimal(row.Cells[clnOpnRate.Index].Value)
                            };

                            var id = request.Id ?? string.Empty;
                            if (id == string.Empty || id == "0")
                                await _inventoryApiService.CreateItemAsync(request);
                            else
                                await _inventoryApiService.UpdateItemAsync(id, request);
                        }
                }
                MessageBox.Show("Record Successfully Saved..!");
                await LoadItemDetailAsync();
                Fillform((string)cmbItemCatagory.SelectedValue);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private async void cmbItemCatagory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( !FLogin && cmbItemCatagory.SelectedValue != null)
                Fillform((string)cmbItemCatagory.SelectedValue);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

      

        private void dgvItemDetail_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            if (txtBarcode.Text != "" && txtBarcode.Text.Substring(txtBarcode.Text.Length - 1) == "\n")
            { 
                bool IsFind = false;
                for (int i = 0; i < dgvItemDetail.Rows .Count; i++)
                {
                    if ((string )dgvItemDetail[clnBarcode.Index,i].Value == txtBarcode.Text)
                    {
                        dgvItemDetail.CurrentCell = dgvItemDetail[clnBarcode.Index, i];
                        IsFind = true;   
                        break;                        
                    }
                }
                if (!IsFind)
                {
                    DataRow dr = dtItemDetail.Select("Barcode = '" + txtBarcode.Text + "'").FirstOrDefault(); 
                    if (dr != null)
                    {
                        dr = dtCatagory.Select("Code = '" + dr["fkitemcatagory"].ToString() + "'").FirstOrDefault();
                        MessageBox.Show("Barcode is exist in '" + dr["Title"].ToString() + "' catagory");
                        IsFind = true;
                    }
                }
                if (!IsFind)
                {
                    object[] Param = new object[]{"0",txtBarcode.Text,cmbItemCatagory.SelectedValue,
                        "",null,"0","0","003","003","003","1",true,"0","0","0"};  
                    dgvItemDetail.Rows.Insert (0,Param);
                    dgvItemDetail.CurrentCell = dgvItemDetail[clnBarcode.Index, 0];
                }
                txtBarcode.Text = "";
                
                txtBarcode.Focus();
               
            }
            
        }

        private async void deleteRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Delete this...!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var id = Convert.ToString(dgvItemDetail[clnId.Index, dgvItemDetail.CurrentRow.Index].Value);
                if (id != "0")
                {
                    await _inventoryApiService.DeleteItemAsync(id);
                    await LoadItemDetailAsync();
                    Fillform((string)cmbItemCatagory.SelectedValue);
                }
                else
                {
                    dgvItemDetail.Rows.RemoveAt(dgvItemDetail.CurrentRow.Index);
                    CalcTotals();
                }

                MessageBox.Show("Record Successfully Deleted..!");
            }
        }

        private decimal ParseDecimal(object value)
        {
            decimal ret;
            return decimal.TryParse(Convert.ToString(value), out ret) ? ret : 0m;
        }

        private decimal? ParseNullableDecimal(object value)
        {
            var text = Convert.ToString(value);
            if (string.IsNullOrWhiteSpace(text))
                return null;

            decimal ret;
            return decimal.TryParse(text, out ret) ? (decimal?)ret : null;
        }
        void CalcTotals()
        {
            int TotItems = 0;
            try
            {
                TotItems = (from DataGridViewRow row in dgvItemDetail.Rows
                            where (string)row.Cells[clnId.Index].Value != "0"
                            select row).Count();

            }
            catch
            {

            }
            lblTotalItems.Text = "Total Items : " + (TotItems).ToString("N0");           
        }

        private void dgvItemDetail_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dgvItemDetail.Rows[e.RowIndex].Selected = true;
                dgvItemDetail.CurrentCell = this.dgvItemDetail.Rows[e.RowIndex].Cells[e.ColumnIndex];
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

      

       
        
    }
}
