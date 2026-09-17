using System;
using System.Collections.Generic;
using System.Data;
using ERP.Classes;

namespace ERP.Reporting.Models
{
    /// <summary>
    /// Represents an individual envelope address record with sender and recipient details.
    /// </summary>
    public class EnvelopeItem
    {
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string SenderPhone { get; set; }

        public string RecipientAccount { get; set; }
        public string RecipientName { get; set; }
        public string RecipientCompany { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientPhone { get; set; }
    }

    /// <summary>
    /// Service to transform Envelope DataTables or customer lists into Envelope items.
    /// </summary>
    public static class EnvelopeDataService
    {
        public static List<EnvelopeItem> ConvertDataTable(DataTable dt)
        {
            var list = new List<EnvelopeItem>();
            string senderCompany = !string.IsNullOrWhiteSpace(CompanyInfo.CompanyName) ? CompanyInfo.CompanyName : "Retail Suite Enterprise";
            string senderAddress = CompanyInfo.Address ?? string.Empty;
            string senderPhone = !string.IsNullOrWhiteSpace(CompanyInfo.ContactHead) ? CompanyInfo.ContactHead : CompanyInfo.Cell;

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string custName = row.Table.Columns.Contains("CustomerName") && row["CustomerName"] != DBNull.Value ? row["CustomerName"].ToString().Trim() : string.Empty;
                    string address = row.Table.Columns.Contains("Address") && row["Address"] != DBNull.Value ? row["Address"].ToString().Trim() : string.Empty;
                    string cell = row.Table.Columns.Contains("Cell") && row["Cell"] != DBNull.Value ? row["Cell"].ToString().Trim() : string.Empty;
                    string compName = row.Table.Columns.Contains("CompanyName") && row["CompanyName"] != DBNull.Value ? row["CompanyName"].ToString().Trim() : string.Empty;

                    list.Add(new EnvelopeItem
                    {
                        SenderCompany = senderCompany,
                        SenderAddress = senderAddress,
                        SenderPhone = senderPhone,
                        RecipientName = custName,
                        RecipientCompany = compName,
                        RecipientAddress = address,
                        RecipientPhone = cell
                    });
                }
            }

            return list;
        }
    }
}
