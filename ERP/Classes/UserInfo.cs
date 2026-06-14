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

    }
}
