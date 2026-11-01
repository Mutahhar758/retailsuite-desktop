using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ERP
{
    class CompanyInfo
    {
        private static string _CompanyName;
        public static string CompanyName
        {
            get { return CompanyInfo._CompanyName; }
            set { CompanyInfo._CompanyName = value; }
        }

        private static string _UrCompanyName;
        public static string UrCompanyName
        {
            get { return CompanyInfo._UrCompanyName; }
            set { CompanyInfo._UrCompanyName = value; }
        }

        private static string _Description;
        public static string Description
        {
            get { return CompanyInfo._Description; }
            set { CompanyInfo._Description = value; }
        }
        
        private static string _Address;
        public static string Address
        {
            get { return CompanyInfo._Address; }
            set { CompanyInfo._Address = value; }
        }

        private static string _Phone;
        public static string Phone
        {
            get { return CompanyInfo._Phone; }
            set { CompanyInfo._Phone = value; }
        }
       
        private static string _Cell;
        public static string Cell
        {
            get { return CompanyInfo._Cell; }
            set { CompanyInfo._Cell = value; }
        }
      
        private static string _Cell2;
        public static string Cell2
        {
            get { return CompanyInfo._Cell2; }
            set { CompanyInfo._Cell2 = value; }
        }

        private static string _ContactHead;
        public static string ContactHead
        {
            get { return CompanyInfo._ContactHead; }
            set { CompanyInfo._ContactHead = value; }
        }

        private static string _Email;
        public static string Email
        {
            get { return CompanyInfo._Email; }
            set { CompanyInfo._Email = value; }
        }

        private static string _Website;
        public static string Website
        {
            get { return CompanyInfo._Website; }
            set { CompanyInfo._Website = value; }
        }

        private static string _Ntn;
        public static string Ntn
        {
            get { return CompanyInfo._Ntn; }
            set { CompanyInfo._Ntn = value; }
        }

        private static string _Strn;
        public static string Strn
        {
            get { return CompanyInfo._Strn; }
            set { CompanyInfo._Strn = value; }
        }

        private static string _LogoMediaId;
        public static string LogoMediaId
        {
            get { return CompanyInfo._LogoMediaId; }
            set { CompanyInfo._LogoMediaId = value; }
        }

        private static string _LogoUrl;
        public static string LogoUrl
        {
            get { return CompanyInfo._LogoUrl; }
            set { CompanyInfo._LogoUrl = value; }
        }

        public static string ThankyouLine
        {
            get { return ConfigInfo.ThankyouLine; }
            set { ConfigInfo.ThankyouLine = value; }
        }
    }
}
