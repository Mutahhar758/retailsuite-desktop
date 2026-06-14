using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP
{
    static class stringValidation
    {
        internal static string EmptyToNull(this string str)
        {
            string Retvalue = str == "" ? null : str;
            return Retvalue;
        }
        internal static string nullToEmpty(this string str)
        {
            string Retvalue = str == null ? "" : str;
            return Retvalue;
        }
    }
}
