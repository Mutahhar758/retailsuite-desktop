using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ERP.Classes
{
    public class ParsedBarcodeItem
    {
        public DataRow ItemRow { get; set; }
        public decimal Qty { get; set; } = 1m;
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public bool IsVariableWeight { get; set; }
        public string Unit { get; set; }
        public string RawCode { get; set; }
    }

    /// <summary>
    /// Unified Barcode Parser for RetailSuite Desktop ERP.
    /// Supports:
    /// 1. Smart Code-128: "VW-{ItemCode}-Q{qty}-R{rate}" or "-P{amount}"
    /// 2. Delimited: "{code}*{qty}*{rate}"
    /// 3. Mart Weighing Machine Barcodes (EAN-13 Scale Barcodes with prefixes 20-28)
    /// 4. Multiplier syntax: "5*890123" or "890123*5"
    /// 5. Standard item barcode / SKU / ID lookup.
    /// </summary>
    public static class BarcodeParser
    {
        private static readonly string[] DefaultScalePrefixes = new[] { "20", "21", "22", "23", "24", "25", "28" };

        public static ParsedBarcodeItem Parse(string rawInput, DataTable dtItems, Func<string, decimal, decimal> rateResolver = null)
        {
            if (string.IsNullOrWhiteSpace(rawInput) || dtItems == null || dtItems.Rows.Count == 0)
                return null;

            string raw = rawInput.Trim();

            // 1. RetailSuite Variable Code-128: "VW-{ItemCode}-Q{qty}-R{rate}" or "-P{amount}"
            if (raw.StartsWith("VW-", StringComparison.OrdinalIgnoreCase) || raw.StartsWith("VW:", StringComparison.OrdinalIgnoreCase))
            {
                string withoutPrefix = raw.Substring(3).Trim();
                var match = Regex.Match(withoutPrefix, @"^(.+?)[-:]Q([\d.]+)(?:[-:]([RP])([\d.]+))?$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string codePart = match.Groups[1].Value.Trim();
                    if (decimal.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedQty) && parsedQty > 0)
                    {
                        string priceType = match.Groups[3].Value.ToUpperInvariant();
                        decimal? priceVal = null;
                        if (match.Groups[4].Success && decimal.TryParse(match.Groups[4].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal pv))
                        {
                            priceVal = pv;
                        }

                        DataRow dr = FindItemByCode(codePart, dtItems);
                        if (dr != null)
                        {
                            decimal baseRate = dr["PriRate"] != DBNull.Value ? Convert.ToDecimal(dr["PriRate"]) : 0m;
                            if (rateResolver != null && dr.Table.Columns.Contains("Id") && dr["Id"] != DBNull.Value)
                                baseRate = rateResolver(dr["Id"].ToString(), baseRate);

                            decimal effectiveRate = baseRate;
                            decimal effectiveAmount;

                            if (priceType == "P" && priceVal.HasValue)
                            {
                                effectiveAmount = decimal.Round(priceVal.Value, 2);
                                effectiveRate = decimal.Round(effectiveAmount / parsedQty, 2);
                            }
                            else if (priceType == "R" && priceVal.HasValue)
                            {
                                effectiveRate = decimal.Round(priceVal.Value, 2);
                                effectiveAmount = decimal.Round(parsedQty * effectiveRate, 2);
                            }
                            else
                            {
                                effectiveAmount = decimal.Round(parsedQty * effectiveRate, 2);
                            }

                            string unit = dr.Table.Columns.Contains("DefaultUnit") && dr["DefaultUnit"] != DBNull.Value
                                ? dr["DefaultUnit"].ToString()
                                : (dr.Table.Columns.Contains("PrimaryUnit") && dr["PrimaryUnit"] != DBNull.Value ? dr["PrimaryUnit"].ToString() : "kg");

                            return new ParsedBarcodeItem
                            {
                                ItemRow = dr,
                                Qty = parsedQty,
                                Rate = effectiveRate,
                                Amount = effectiveAmount,
                                IsVariableWeight = true,
                                Unit = unit,
                                RawCode = raw
                            };
                        }
                    }
                }
            }

            // 2. Delimited Syntax: {ItemCode}*{qty} or {ItemCode}*{qty}*{rate} (No VW, Q, R needed)
            if (raw.Contains("*"))
            {
                string[] starParts = raw.Split('*');
                if (starParts.Length == 3 || starParts.Length == 2)
                {
                    string codePart = starParts[0].Trim();
                    if (decimal.TryParse(starParts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal qVal) && qVal > 0)
                    {
                        DataRow dr = FindItemByCode(codePart, dtItems);
                        if (dr != null)
                        {
                            decimal rate = dr["PriRate"] != DBNull.Value ? Convert.ToDecimal(dr["PriRate"]) : 0m;
                            if (starParts.Length == 3 && decimal.TryParse(starParts[2].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rVal) && rVal >= 0)
                            {
                                rate = decimal.Round(rVal, 2);
                            }
                            else if (rateResolver != null && dr.Table.Columns.Contains("Id") && dr["Id"] != DBNull.Value)
                            {
                                rate = rateResolver(dr["Id"].ToString(), rate);
                            }

                            string unit = dr.Table.Columns.Contains("DefaultUnit") && dr["DefaultUnit"] != DBNull.Value
                                ? dr["DefaultUnit"].ToString()
                                : (dr.Table.Columns.Contains("PrimaryUnit") && dr["PrimaryUnit"] != DBNull.Value ? dr["PrimaryUnit"].ToString() : "kg");

                            return new ParsedBarcodeItem
                            {
                                ItemRow = dr,
                                Qty = qVal,
                                Rate = rate,
                                Amount = decimal.Round(qVal * rate, 2),
                                IsVariableWeight = true,
                                Unit = unit,
                                RawCode = raw
                            };
                        }
                    }
                }
            }

            // 3. Mart Weighing Machine Barcodes (EAN-13 Scale Barcodes: 2X IIIII WWWWW C)
            string cleanDigits = Regex.Replace(raw, @"\D", "");
            if (cleanDigits.Length == 13)
            {
                string prefix = cleanDigits.Substring(0, 2);
                if (Array.IndexOf(DefaultScalePrefixes, prefix) >= 0)
                {
                    string pluString = cleanDigits.Substring(2, 5);
                    string valueString = cleanDigits.Substring(7, 5);

                    DataRow dr = FindItemByPlu(pluString, dtItems);
                    if (dr != null && int.TryParse(valueString, out int rawValue))
                    {
                        bool isWeightScale = prefix == "21" || prefix == "28" || prefix == "23";
                        decimal baseRate = dr["PriRate"] != DBNull.Value ? Convert.ToDecimal(dr["PriRate"]) : 0m;
                        if (rateResolver != null && dr.Table.Columns.Contains("Id") && dr["Id"] != DBNull.Value)
                            baseRate = rateResolver(dr["Id"].ToString(), baseRate);

                        string unit = dr.Table.Columns.Contains("DefaultUnit") && dr["DefaultUnit"] != DBNull.Value
                            ? dr["DefaultUnit"].ToString()
                            : (dr.Table.Columns.Contains("PrimaryUnit") && dr["PrimaryUnit"] != DBNull.Value ? dr["PrimaryUnit"].ToString() : "kg");

                        if (isWeightScale)
                        {
                            decimal weightKg = decimal.Round(rawValue / 1000m, 3);
                            decimal amount = decimal.Round(weightKg * baseRate, 2);

                            return new ParsedBarcodeItem
                            {
                                ItemRow = dr,
                                Qty = weightKg,
                                Rate = baseRate,
                                Amount = amount,
                                IsVariableWeight = true,
                                Unit = unit,
                                RawCode = raw
                            };
                        }
                        else
                        {
                            decimal totalPrice = rawValue;
                            decimal calculatedQty = baseRate > 0 ? decimal.Round(totalPrice / baseRate, 3) : 1m;
                            decimal rate = baseRate > 0 ? baseRate : totalPrice;

                            return new ParsedBarcodeItem
                            {
                                ItemRow = dr,
                                Qty = calculatedQty,
                                Rate = rate,
                                Amount = totalPrice,
                                IsVariableWeight = true,
                                Unit = unit,
                                RawCode = raw
                            };
                        }
                    }
                }
            }

            // 4. Multiplier syntax: "5*890123" or "890123*5"
            decimal scanQty = 1m;
            string cleanCode = raw;

            if (raw.Contains("*"))
            {
                string[] parts = raw.Split('*');
                if (parts.Length == 2)
                {
                    string p0 = parts[0].Trim();
                    string p1 = parts[1].Trim();
                    bool num0Parsed = decimal.TryParse(p0, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal num0);
                    bool num1Parsed = decimal.TryParse(p1, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal num1);

                    if (FindItemByCode(p1, dtItems) != null && num0Parsed && num0 > 0)
                    {
                        scanQty = num0;
                        cleanCode = p1;
                    }
                    else if (FindItemByCode(p0, dtItems) != null && num1Parsed && num1 > 0)
                    {
                        scanQty = num1;
                        cleanCode = p0;
                    }
                    else if (num0Parsed && num0 > 0 && num0 < 1000 && !num1Parsed)
                    {
                        scanQty = num0;
                        cleanCode = p1;
                    }
                    else if (num1Parsed && num1 > 0 && num1 < 1000 && !num0Parsed)
                    {
                        scanQty = num1;
                        cleanCode = p0;
                    }
                }
            }

            // 5. Standard item lookup
            DataRow stdItem = FindItemByCode(cleanCode, dtItems);
            if (stdItem != null)
            {
                decimal rate = stdItem["PriRate"] != DBNull.Value ? Convert.ToDecimal(stdItem["PriRate"]) : 0m;
                if (rateResolver != null && stdItem.Table.Columns.Contains("Id") && stdItem["Id"] != DBNull.Value)
                    rate = rateResolver(stdItem["Id"].ToString(), rate);
                string unit = stdItem.Table.Columns.Contains("DefaultUnit") && stdItem["DefaultUnit"] != DBNull.Value
                    ? stdItem["DefaultUnit"].ToString()
                    : (stdItem.Table.Columns.Contains("PrimaryUnit") && stdItem["PrimaryUnit"] != DBNull.Value ? stdItem["PrimaryUnit"].ToString() : "");

                return new ParsedBarcodeItem
                {
                    ItemRow = stdItem,
                    Qty = scanQty,
                    Rate = rate,
                    Amount = decimal.Round(scanQty * rate, 2),
                    IsVariableWeight = false,
                    Unit = unit,
                    RawCode = raw
                };
            }

            return null;
        }

        public static DataRow FindItemByCode(string code, DataTable dtItems)
        {
            if (string.IsNullOrWhiteSpace(code) || dtItems == null) return null;
            string target = code.Trim().ToLowerInvariant();

            // 1. Exact Barcode
            foreach (DataRow r in dtItems.Rows)
            {
                if (r["Barcode"] != DBNull.Value && r["Barcode"].ToString().Trim().ToLowerInvariant() == target)
                    return r;
            }

            // 2. Exact ItemKey or Id
            foreach (DataRow r in dtItems.Rows)
            {
                if (r["Id"] != DBNull.Value && r["Id"].ToString().Trim().ToLowerInvariant() == target)
                    return r;
                if (dtItems.Columns.Contains("ItemKey") && r["ItemKey"] != DBNull.Value && r["ItemKey"].ToString().Trim().ToLowerInvariant() == target)
                    return r;
            }

            // 3. Exact Title
            foreach (DataRow r in dtItems.Rows)
            {
                if (r["Title"] != DBNull.Value && r["Title"].ToString().Trim().ToLowerInvariant() == target)
                    return r;
            }

            // 4. Fallback single partial match if length >= 3
            if (target.Length >= 3)
            {
                DataRow singleMatch = null;
                int matchCount = 0;
                foreach (DataRow r in dtItems.Rows)
                {
                    string bc = r["Barcode"] != DBNull.Value ? r["Barcode"].ToString().ToLowerInvariant() : "";
                    string tt = r["Title"] != DBNull.Value ? r["Title"].ToString().ToLowerInvariant() : "";
                    if (bc.Contains(target) || tt.Contains(target))
                    {
                        matchCount++;
                        singleMatch = r;
                        if (matchCount > 1) break;
                    }
                }
                if (matchCount == 1) return singleMatch;
            }

            return null;
        }

        private static DataRow FindItemByPlu(string plu, DataTable dtItems)
        {
            string cleanPlu = plu.Trim();
            int.TryParse(cleanPlu, out int numPlu);
            string numericString = numPlu.ToString();

            foreach (DataRow r in dtItems.Rows)
            {
                string bc = r["Barcode"] != DBNull.Value ? r["Barcode"].ToString().Trim() : "";
                string id = r["Id"] != DBNull.Value ? r["Id"].ToString().Trim() : "";
                string key = dtItems.Columns.Contains("ItemKey") && r["ItemKey"] != DBNull.Value ? r["ItemKey"].ToString().Trim() : "";

                if (string.Equals(bc, cleanPlu, StringComparison.OrdinalIgnoreCase)) return r;
                if (id == cleanPlu || id == numericString) return r;
                if (key == cleanPlu || key == numericString) return r;
            }

            return null;
        }
    }
}
