using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
namespace ERP
{   
    class clsConnection
    {
        public static void Con()
        {
            
            //string connection = @"Data Source=server.bizgripsolutions.com;Initial Catalog=BG_HRPrinting;User ID=mutahhar;Password=mutahhar11;MultipleActiveResultSets=true;WSID=" + UserInfo.UserId + " | " + "";
            //string connection = @"Data Source=server.bizgripsolutions.com;Initial Catalog=BG_ShoaibBookStall;User ID=mutahhar;Password=mutahhar11;MultipleActiveResultSets=true;WSID=" + UserInfo.UserId + " | " + "";
            string connection = @"Data Source=server.bizgripsolutions.com;Initial Catalog=BG_Test_MR;User ID=mutahhar;Password=mutahhar11;MultipleActiveResultSets=true;WSID=" + UserInfo.UserId + " | " + "";
            //string connection = @"Data Source=25.21.174.87;Initial Catalog=POS_SP;User ID=mutahhar;Password=mutahhar11;MultipleActiveResultSets=true;WSID=" + UserInfo.UserId + " | " + "";
            //string connection = @"Data Source=.;Initial Catalog=POS_SP;User ID=mutahhar;Password=mutahhar11;MultipleActiveResultSets=true;WSID=" + UserInfo.UserId + " | " + "";
            //string connection = @"Data Source=.;Initial Catalog=POS_SP;Integrated security = true;MultipleActiveResultSets=true;WSID=" + UserInfo.UserId + " | " + "";

            //if (con == null)
            //    con = new SqlConnection(connection);
            //else
            //{
            //    con.Close(); 
            //    SqlConnection.ClearPool(con);
            //    con.ConnectionString = connection;
            //}
            //con.StatisticsEnabled = true;
            //con.Open(); 
        }
        public static SqlConnection con;
    }
}
 