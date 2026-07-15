using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP
{
    class UserInfo
    {
        private static string _UserId;

        public static string UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        private static string _UserName;

        public static string UserName
        {
            get { return UserInfo._UserName; }
            set { UserInfo._UserName = value; }
        }

        private static string _Email;

        public static string Email
        {
            get { return UserInfo._Email; }
            set { UserInfo._Email = value; }
        }

        private static DateTime _LogInDateTime;

        public static DateTime LogInDateTime
        {
            get { return UserInfo._LogInDateTime; }
            set { UserInfo._LogInDateTime = value; }
        }

        private static bool _IsOwner;

        public static bool IsOwner
        {
            get { return UserInfo._IsOwner; }
            set { UserInfo._IsOwner = value; }
        }

        private static List<string> _Permissions = new List<string>();

        public static List<string> Permissions
        {
            get { return UserInfo._Permissions; }
            set { UserInfo._Permissions = value; }
        }

        public static bool HasPermission(string action, string resource)
        {
            if (IsOwner) return true;
            string permissionName = $"Permissions.{resource}.{action}";
            return _Permissions != null && _Permissions.Contains(permissionName);
        }

        public static void ApplyFormPermissions(System.Windows.Forms.Form form, string resourceName)
        {
            if (IsOwner) return;

            bool canCreate = HasPermission(AppAction.Create, resourceName);
            bool canUpdate = HasPermission(AppAction.Update, resourceName);
            bool canDelete = HasPermission(AppAction.Delete, resourceName);

            DisableOrHideButtonsRecursive(form.Controls, canCreate || canUpdate, canDelete);
        }

        private static void DisableOrHideButtonsRecursive(System.Windows.Forms.Control.ControlCollection controls, bool canSave, bool canDelete)
        {
            foreach (System.Windows.Forms.Control control in controls)
            {
                if (control is System.Windows.Forms.Button button)
                {
                    string name = button.Name.ToLower();
                    string text = button.Text.ToLower();

                    if (name.Contains("save") || text.Contains("save"))
                    {
                        button.Visible = canSave;
                    }
                    else if (name.Contains("delete") || text.Contains("delete") || name.Contains("del") || text.Contains("del"))
                    {
                        button.Visible = canDelete;
                    }
                }

                if (control.HasChildren)
                {
                    DisableOrHideButtonsRecursive(control.Controls, canSave, canDelete);
                }
            }
        }
    }

    public static class AppAction
    {
        public const string View = "View";
        public const string Search = "Search";
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string Export = "Export";
        public const string Print = "Print";
    }

    public static class AppResource
    {
        public const string Users = "Users";
        public const string Roles = "Roles";
        public const string Dashboard = "Dashboard";
        public const string Reports = "Reports";
        public const string PrinterSettings = "PrinterSettings";
        public const string ChartOfAccounts = "ChartOfAccounts";
        public const string DetailAccounts = "DetailAccounts";
        public const string Customers = "Customers";
        public const string Vendors = "Vendors";
        public const string InventoryItems = "InventoryItems";
        public const string ItemCategories = "ItemCategories";
        public const string Units = "Units";
        public const string Narrations = "Narrations";
        public const string HRInfo = "HRInfo";
        public const string SupplyOrders = "SupplyOrders";
        public const string OpeningBalances = "OpeningBalances";
        public const string PaymentVouchers = "PaymentVouchers";
        public const string ReceiptVouchers = "ReceiptVouchers";
        public const string JournalVouchers = "JournalVouchers";
        public const string Purchases = "Purchases";
        public const string Sales = "Sales";
        public const string POSSales = "POSSales";
        public const string SaleSupplies = "SaleSupplies";
        public const string PurchaseReturns = "PurchaseReturns";
        public const string SaleReturns = "SaleReturns";
        public const string StockAdjustments = "StockAdjustments";
        public const string BankReconciliations = "BankReconciliations";
        public const string Payrolls = "Payrolls";
    }
}
