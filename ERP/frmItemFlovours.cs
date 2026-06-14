using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERP
{
    public partial class frmItemDetail : Form
    {
        bool FLogin = true;
        public frmItemDetail()
        {
            InitializeComponent();
        }
        void AllowNewRow()
        {
            if (dgvItemDetail.CurrentRow.Index == dgvItemDetail.Rows[dgvItemDetail.Rows.Count - 1].Index)
            {
                if (dgvItemDetail.CurrentRow.Cells[clnItem.Index].Value != null &&
                    dgvItemDetail.CurrentRow.Cells[clnRate.Index].Value != null)
                    dgvItemDetail.Rows.Add();
            }
        }
        void fillItemCatagory()
        {
            cmbItemCatagory.DataSource = Query.ItemCatagoryIndex();
            cmbItemCatagory.DisplayMember = "Title";
            cmbItemCatagory.ValueMember = "Code"; 
        }
        DataTable dtItemDetail = Query.ItemDetail();
        void Fillform(string catagory)
        {
            DataTable dtfilter = dtItemDetail.Copy();
            dtfilter.Rows.Clear();
            dtfilter = dtItemDetail.Select("fkitemcatagory = '" + catagory + "'").Count() > 0 ? dtItemDetail.Select("fkitemcatagory = '" + catagory + "'").CopyToDataTable() : dtfilter;
            dgvItemDetail.Rows.Clear();
            for (int i = 0; i < dtfilter.Rows.Count; i++)
            {
                dgvItemDetail.Rows.Add(dtfilter.Rows[i]["Title"].ToString(),
                    dtfilter.Rows[i]["Rate"].ToString(),
                    dtfilter.Rows[i]["status"].ToString(), 
                    "0");
            }
        }
        private void frmItemDetail_Load(object sender, EventArgs e)
        {
            fillItemCatagory();
            dgvItemDetail.Rows.Add();
            Fillform((string)cmbItemCatagory.SelectedValue); 
            FLogin = false;
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
            if (dgvItemDetail.CurrentCell.ColumnIndex == clnRate.Index)
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
            if (dgvItemDetail.CurrentCell.ColumnIndex == clnRate.Index)
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
            if (!FLogin && dgvItemDetail.CurrentRow != null)
            {
                dgvItemDetail.CurrentRow.Cells[clnIsEdit.Index].Value = "1";
            }
        }
        private void dgvItemDetail_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dgvItemDetail.Rows[e.RowIndex].Cells[clnIsEdit.Index].Value = "1";
            dgvItemDetail.Rows[e.RowIndex].Cells[clnstatus.Index].Value = "0";
  
        }
        #endregion
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Save this...!", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dgvItemDetail.Rows)
                {
                    if (cmbItemCatagory.SelectedValue  != null &&
                        row.Cells[clnItem.Index].Value != null && 
                        row.Cells[clnRate.Index].Value != null && 
                        row.Cells[clnIsEdit.Index].Value  == "1")
                        DML.ItemAdd_Edit(cmbItemCatagory.SelectedValue.ToString()  , "" /* No Code */,
                            row.Cells[clnItem.Index].Value.ToString(),
                            row.Cells[clnRate.Index].Value.ToString(),
                            row.Cells[clnstatus.Index].Value.ToString());
                }
                MessageBox.Show("Record Successfully Saved..!");
            }
        }

        private void cmbItemCatagory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( !FLogin && cmbItemCatagory.SelectedValue != null)
            Fillform((string)cmbItemCatagory.SelectedValue);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

       
        
    }
}
