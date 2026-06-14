using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using ERP.Services.Legacy;

namespace ERP 
{
    class ReportQuery
    {
        private static readonly ReportsApiService _reportsApiService = new ReportsApiService();

        internal  static DataTable AccountStatement(string account ,DateTime Fdate ,DateTime Tdate)
        {
            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetAccountStatementAsync(account, Fdate, Tdate)).GetAwaiter().GetResult();
        }
        internal static DataTable AccountStatementWithDue(string account, DateTime Fdate, DateTime Tdate)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetAccountStatementWithDueAsync(account, Fdate, Tdate)).GetAwaiter().GetResult();
        }
        internal static DataTable StockBalance(DateTime Fdate, DateTime Tdate, string Filter, decimal Qty, string Catagory, string Type)
        {
            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetStockBalanceAsync(Fdate, Tdate, Filter, Qty, Catagory, Type)).GetAwaiter().GetResult();
        }
        internal static DataTable StockLedger( string Item, DateTime Fdate, DateTime Tdate)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetStockLedgerAsync(Item, Fdate, Tdate)).GetAwaiter().GetResult();
        }
        internal static DataTable IncomeSummery(DateTime Fdate, DateTime Tdate)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetIncomeSummaryAsync(Fdate, Tdate)).GetAwaiter().GetResult();
        }
        internal static DataTable TrialBalance(DateTime Fdate, DateTime Tdate)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetTrialBalanceAsync(Fdate, Tdate)).GetAwaiter().GetResult();
        }
        internal static DataTable BalanceSheet( DateTime Tdate)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetBalanceSheetAsync(Tdate)).GetAwaiter().GetResult();
        }
        internal static DataTable Balance(string account, DateTime Tdate)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetBalanceDetailAsync(account, Tdate)).GetAwaiter().GetResult();
        }
        internal static DataSet  SaleBill(string Vno)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetSaleBillAsync(Vno)).GetAwaiter().GetResult();
        }

        internal static DataSet SaleBill2(string Vno)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetSaleBill2Async(Vno)).GetAwaiter().GetResult();
        }
        internal static DataSet PurchaseBill(string Vno)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetPurchaseBillAsync(Vno)).GetAwaiter().GetResult();
        }
        internal static DataSet PurchaseRetBill(string Vno)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetPurchaseRetBillAsync(Vno)).GetAwaiter().GetResult();
        }
        internal static DataSet SaleRetBill(string Vno)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetSaleRetBillAsync(Vno)).GetAwaiter().GetResult();
        }
        internal static DataSet CustomerBill(string account, DateTime Fdate, DateTime Tdate)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetCustomerBillAsync(account, Fdate, Tdate)).GetAwaiter().GetResult();
        }
        internal static DataTable EnvelopeDetail(string[] account)
        {

            return System.Threading.Tasks.Task.Run(() => _reportsApiService.GetEnvelopeAsync(account)).GetAwaiter().GetResult();
        }


    }
}
