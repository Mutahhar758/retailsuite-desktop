using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
namespace ERP
{
    class Query
    {
        internal static DataTable getData(string sql)
        {
            DataTable dt = new DataTable();
            // SqlConnection con = conOpen();            
            SqlDataAdapter adap = new SqlDataAdapter(sql, clsConnection.con);
            //SqlCommandBuilder comBuild = new SqlCommandBuilder();
            adap.Fill(dt);
            adap.Dispose();
            return dt;
        }
        
    }
}
