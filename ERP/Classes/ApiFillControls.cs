using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using ERP.Services.Legacy;

namespace ERP.Classes
{
    internal static class ApiFillControls
    {
        public static async Task FillcmbCatagoryAsync(ComboBox cmb)
        {
            var service = new ItemCategoryApiService();
            var categories = await service.GetLookupAsync();

            var dt = ToDataTable(categories);
            cmb.DataSource = dt;
            cmb.DisplayMember = "Title";
            cmb.ValueMember = "Code";
            cmb.SelectedIndex = -1;
        }

        public static async Task FillclnCatagoryAsync(DataGridViewComboBoxColumn cln)
        {
            var service = new ItemCategoryApiService();
            var categories = await service.GetLookupAsync();

            cln.DataSource = ToDataTable(categories);
            cln.DisplayMember = "Title";
            cln.ValueMember = "Code";
        }

        public static async Task FillcmbUnitAsync(ComboBox cmb)
        {
            var service = new UnitApiService();
            var units = await service.GetLookupAsync();

            var dt = ToUnitDataTable(units);
            cmb.DataSource = dt;
            cmb.DisplayMember = "Title";
            cmb.ValueMember = "Code";
            cmb.SelectedIndex = -1;
        }

        public static async Task FillclnUnitAsync(DataGridViewComboBoxColumn cln)
        {
            var service = new UnitApiService();
            var units = await service.GetLookupAsync();

            cln.DataSource = ToUnitDataTable(units);
            cln.DisplayMember = "Title";
            cln.ValueMember = "Code";
        }

        private static DataTable ToDataTable(List<ItemCategoryDto> categories)
        {
            var dt = new DataTable();
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Title", typeof(string));

            foreach (var category in categories)
                dt.Rows.Add(category.Code, category.Title);

            return dt;
        }

        private static DataTable ToUnitDataTable(List<UnitLookupDto> units)
        {
            var dt = new DataTable();
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Title", typeof(string));

            foreach (var unit in units)
                dt.Rows.Add(unit.Code, unit.Title);

            return dt;
        }
    }
}
