using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP
{
    class AccountHead
    {
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
