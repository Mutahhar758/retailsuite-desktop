using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP
{
    class AccountHead
    {
        public const int DetailLevel = 5;

        public const string AssetsRootId = "001";
        public const string LiabilitiesRootId = "002";
        public const string IncomeRootId = "003";
        public const string ExpensesRootId = "004";
        public const string CapitalRootId = "005";

        public const string DefaultCustomerHead = "001002001001";
        public const string DefaultVendorHead = "002001001001";
        public const string DefaultCashPrefix = "001002002001";

        private static string _Customer;

        public static string Customer
        {
            get { return _Customer; }
            set { _Customer = value; }
        }
        private static string _Farmer;

        public static string Farmer
        {
            get { return _Farmer; }
            set { _Farmer = value; }
        }
    }
}
