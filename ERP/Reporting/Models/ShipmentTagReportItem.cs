using System;
using System.Collections.Generic;
using ERP.Classes;

namespace ERP.Reporting.Models
{
    /// <summary>
    /// Data model representing a parcel/box shipment dispatch tag.
    /// </summary>
    public class ShipmentTagItem
    {
        public string TrackingNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime DispatchDate { get; set; } = DateTime.Today;

        public string ShipperName { get; set; }
        public string ShipperAddress { get; set; }
        public string ShipperPhone { get; set; }

        public string ConsigneeAccount { get; set; }
        public string ConsigneeName { get; set; }
        public string ConsigneeCompany { get; set; }
        public string ConsigneeAddress { get; set; }
        public string ConsigneeCity { get; set; }
        public string ConsigneePhone { get; set; }

        public int PackageNumber { get; set; } = 1;
        public int TotalPackages { get; set; } = 1;
        public decimal WeightKg { get; set; }
        public string CourierService { get; set; } = "Express Cargo";
        public string Remarks { get; set; } = "HANDLE WITH CARE • FRAGILE • KEEP DRY";

        public string PackageTag => string.Format("CARTON {0} OF {1}", PackageNumber, TotalPackages);
    }

    public static class ShipmentTagDataService
    {
        public static List<ShipmentTagItem> GenerateCartonTags(ShipmentTagItem baseTag, int totalCartons)
        {
            var list = new List<ShipmentTagItem>();
            int count = Math.Max(1, totalCartons);
            for (int i = 1; i <= count; i++)
            {
                list.Add(new ShipmentTagItem
                {
                    TrackingNo = baseTag.TrackingNo,
                    InvoiceNo = baseTag.InvoiceNo,
                    DispatchDate = baseTag.DispatchDate,
                    ShipperName = baseTag.ShipperName,
                    ShipperAddress = baseTag.ShipperAddress,
                    ShipperPhone = baseTag.ShipperPhone,
                    ConsigneeAccount = baseTag.ConsigneeAccount,
                    ConsigneeName = baseTag.ConsigneeName,
                    ConsigneeCompany = baseTag.ConsigneeCompany,
                    ConsigneeAddress = baseTag.ConsigneeAddress,
                    ConsigneeCity = baseTag.ConsigneeCity,
                    ConsigneePhone = baseTag.ConsigneePhone,
                    PackageNumber = i,
                    TotalPackages = count,
                    WeightKg = baseTag.WeightKg,
                    CourierService = baseTag.CourierService,
                    Remarks = baseTag.Remarks
                });
            }
            return list;
        }
    }
}
