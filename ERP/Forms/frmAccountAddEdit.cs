using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP
{
    public partial class frmAccountAddEdit : Form
    {
        private readonly ChartOfAccountApiService _chartOfAccountApiService;
        private bool _isSaving;

        public frmAccountAddEdit()
        {
            InitializeComponent();
            _chartOfAccountApiService = new ChartOfAccountApiService();
        }
       public string parentId = "";
       public string Account = "";
       public string Title = "";
       public string level = ""; 
       public string Type = "";
       public string CreatedBy = "";
       public string EditBy = "";
        private void frmAccountAddEdit_Load(object sender, EventArgs e)
        {
            lblParent.Text = parentId;
            lblAccount.Text = Account;
            txtTitle.Text = Title;
            lblLevel.Text = level; 
            lblType.Text  = Type;
            lblCreated.Text = CreatedBy;
            lblEdit.Text = EditBy ;

        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving)
                return;

            if (MessageBox.Show("Are you sure?" + Environment.NewLine + "You want to Save this...!", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    MessageBox.Show("Title is required.");
                    return;
                }

                _isSaving = true;
                btnSave.Enabled = false;
                try
                {
                    if (string.IsNullOrWhiteSpace(Account))
                    {
                        Account = await _chartOfAccountApiService.CreateAsync(lblParent.Text, txtTitle.Text.Trim());
                        lblAccount.Text = Account;
                    }
                    else
                    {
                        await _chartOfAccountApiService.UpdateAsync(Account, txtTitle.Text.Trim());
                    }

                    MessageBox.Show("Record Successfully Saved..!");
                    this.DialogResult = DialogResult.Yes;
                    this.Close();
                }
                finally
                {
                    _isSaving = false;
                    btnSave.Enabled = true;
                }
            }
        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
            if (txtTitle.Text.Length > 0) btnSave.Enabled = true;
            else btnSave.Enabled = false;  
        }

        private void frmAccountAddEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();  
        }

      
    }
}
