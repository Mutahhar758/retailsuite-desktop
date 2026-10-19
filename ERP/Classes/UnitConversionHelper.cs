using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ERP.Services.Legacy;
using Newtonsoft.Json;

namespace ERP
{
    internal class AdditionalUnitConfigDto
    {
        [JsonProperty("unitId")]
        public string UnitId { get; set; }

        [JsonProperty("unitTitle")]
        public string UnitTitle { get; set; }

        [JsonProperty("shortTitle")]
        public string ShortTitle { get; set; }

        [JsonProperty("factor")]
        public decimal Factor { get; set; }
    }

    internal static class UnitConversionHelper
    {
        public static DataTable CreateUnitsTable(IList<UnitLookupDto> units)
        {
            var dt = new DataTable();
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("ShortTitle", typeof(string));
            dt.Columns.Add("ParentUnitId", typeof(string));
            dt.Columns.Add("ConversionRatio", typeof(decimal));

            if (units != null)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    var u = units[i];
                    var displayTitle = !string.IsNullOrWhiteSpace(u.ShortTitle) ? u.ShortTitle.Trim() : (u.Title ?? "");
                    dt.Rows.Add(
                        u.Code ?? "",
                        displayTitle,
                        u.ShortTitle ?? "",
                        u.ParentUnitId ?? "",
                        u.ConversionRatio.HasValue ? u.ConversionRatio.Value : 0m);
                }
            }

            return dt;
        }

        public static List<AdditionalUnitConfigDto> ParseAdditionalUnits(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                var parsed = JsonConvert.DeserializeObject<List<AdditionalUnitConfigDto>>(json);
                if (parsed == null)
                    return new List<AdditionalUnitConfigDto>();

                return parsed
                    .Where(u => u != null && !string.IsNullOrWhiteSpace(u.UnitId) && u.Factor > 1m)
                    .ToList();
            }
            catch
            {
                return null;
            }
        }

        public static List<AdditionalUnitConfigDto> GetEffectiveSubUnits(string primaryUnitId, string additionalUnitsJson, DataTable dtUnits)
        {
            if (string.IsNullOrWhiteSpace(primaryUnitId))
                return new List<AdditionalUnitConfigDto>();

            // Explicit item override (including "[]" to disable sub-units)
            if (additionalUnitsJson != null && additionalUnitsJson.Trim() != "")
            {
                var explicitList = ParseAdditionalUnits(additionalUnitsJson);
                if (explicitList != null)
                {
                    foreach (var item in explicitList)
                    {
                        if (dtUnits != null && string.IsNullOrWhiteSpace(item.ShortTitle))
                        {
                            var unitRows = dtUnits.Select("Code = '" + item.UnitId.Replace("'", "''") + "'");
                            if (unitRows.Length > 0)
                            {
                                var st = Convert.ToString(unitRows[0]["ShortTitle"]);
                                var title = Convert.ToString(unitRows[0]["Title"]);
                                item.ShortTitle = !string.IsNullOrWhiteSpace(st) ? st : title;
                                if (string.IsNullOrWhiteSpace(item.UnitTitle))
                                    item.UnitTitle = title;
                            }
                        }
                    }
                    return explicitList;
                }
            }

            var result = new List<AdditionalUnitConfigDto>();
            if (dtUnits == null || !dtUnits.Columns.Contains("ParentUnitId"))
                return result;

            var childRows = dtUnits.Select("ParentUnitId = '" + primaryUnitId.Replace("'", "''") + "'");
            for (int i = 0; i < childRows.Length; i++)
            {
                decimal ratio = 0m;
                decimal.TryParse(Convert.ToString(childRows[i]["ConversionRatio"]), out ratio);
                if (ratio > 1m)
                {
                    string code = Convert.ToString(childRows[i]["Code"]);
                    string title = Convert.ToString(childRows[i]["Title"]);
                    string shortTitle = dtUnits.Columns.Contains("ShortTitle")
                        ? Convert.ToString(childRows[i]["ShortTitle"])
                        : "";
                    result.Add(new AdditionalUnitConfigDto
                    {
                        UnitId = code,
                        UnitTitle = title,
                        ShortTitle = !string.IsNullOrWhiteSpace(shortTitle) ? shortTitle : title,
                        Factor = ratio
                    });
                }
            }

            return result;
        }

        public static DataTable GetSelectableUnitsTable(DataRow itemRow, DataTable dtUnits)
        {
            var dt = new DataTable();
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Factor", typeof(decimal));

            if (itemRow == null)
                return dt;

            string primaryUnitId = itemRow.Table.Columns.Contains("PrimaryUnit")
                ? Convert.ToString(itemRow["PrimaryUnit"]).Trim()
                : "";
            if (string.IsNullOrEmpty(primaryUnitId))
                return dt;

            string primaryLabel = primaryUnitId;
            if (dtUnits != null)
            {
                var priRows = dtUnits.Select("Code = '" + primaryUnitId.Replace("'", "''") + "'");
                if (priRows.Length > 0)
                {
                    string st = dtUnits.Columns.Contains("ShortTitle") ? Convert.ToString(priRows[0]["ShortTitle"]).Trim() : "";
                    string t = Convert.ToString(priRows[0]["Title"]).Trim();
                    primaryLabel = !string.IsNullOrEmpty(st) ? st : t;
                }
            }

            dt.Rows.Add(primaryUnitId, primaryLabel, 1m);

            string additionalUnitsJson = itemRow.Table.Columns.Contains("AdditionalUnits")
                ? Convert.ToString(itemRow["AdditionalUnits"])
                : null;

            var subUnits = GetEffectiveSubUnits(primaryUnitId, additionalUnitsJson, dtUnits);
            for (int i = 0; i < subUnits.Count; i++)
            {
                var sub = subUnits[i];
                if (sub.UnitId != primaryUnitId && sub.Factor > 1m)
                {
                    string label = !string.IsNullOrWhiteSpace(sub.ShortTitle)
                        ? sub.ShortTitle.Trim()
                        : (!string.IsNullOrWhiteSpace(sub.UnitTitle) ? sub.UnitTitle.Trim() : sub.UnitId);
                    dt.Rows.Add(sub.UnitId, label, sub.Factor);
                }
            }

            return dt;
        }

        public static bool BindRowUnitComboBox(
            DataGridViewRow gridRow,
            int unitColIndex,
            DataRow itemRow,
            DataTable dtUnits,
            string selectedUnitId = null)
        {
            if (gridRow == null || unitColIndex < 0 || unitColIndex >= gridRow.Cells.Count)
                return false;

            var selectableUnits = GetSelectableUnitsTable(itemRow, dtUnits);
            var comboCell = gridRow.Cells[unitColIndex] as DataGridViewComboBoxCell;
            if (comboCell != null && selectableUnits.Rows.Count > 0)
            {
                comboCell.DataSource = selectableUnits;
                comboCell.ValueMember = "Code";
                comboCell.DisplayMember = "Title";
            }

            string targetUnit = selectedUnitId;
            if (string.IsNullOrWhiteSpace(targetUnit) || selectableUnits.Select("Code = '" + targetUnit.Replace("'", "''") + "'").Length == 0)
            {
                targetUnit = selectableUnits.Rows.Count > 0
                    ? Convert.ToString(selectableUnits.Rows[0]["Code"])
                    : (itemRow != null && itemRow.Table.Columns.Contains("PrimaryUnit") ? Convert.ToString(itemRow["PrimaryUnit"]) : "");
            }

            gridRow.Cells[unitColIndex].Value = targetUnit;
            gridRow.Cells[unitColIndex].ReadOnly = selectableUnits.Rows.Count <= 1;
            return selectableUnits.Rows.Count > 1;
        }

        public static void UpdateUnitColumnVisibility(DataGridView dgv, DataGridViewColumn clnUnit, DataGridViewColumn clnQty, DataTable dtItems, DataTable dtUnits)
        {
            if (dgv == null || clnUnit == null)
                return;

            bool hasAnyMultiUnit = false;
            if (dtItems != null && dtUnits != null)
            {
                for (int i = 0; i < dtItems.Rows.Count; i++)
                {
                    var row = dtItems.Rows[i];
                    string pri = dtItems.Columns.Contains("PrimaryUnit") ? Convert.ToString(row["PrimaryUnit"]) : "";
                    string addJson = dtItems.Columns.Contains("AdditionalUnits") ? Convert.ToString(row["AdditionalUnits"]) : null;
                    if (GetEffectiveSubUnits(pri, addJson, dtUnits).Count > 0)
                    {
                        hasAnyMultiUnit = true;
                        break;
                    }
                }
            }

            clnUnit.Visible = hasAnyMultiUnit;
            if (hasAnyMultiUnit)
            {
                clnUnit.HeaderText = "Unit";
                clnUnit.Width = 65;
                clnUnit.ReadOnly = false;
                if (clnQty != null)
                {
                    try
                    {
                        clnUnit.DisplayIndex = clnQty.DisplayIndex + 1;
                    }
                    catch
                    {
                        // Ignore DisplayIndex ordering issues during initialization
                    }
                }
            }
        }

        public static decimal ConvertToBasePrimaryQty(decimal enteredQty, string selectedUnitId, DataRow itemRow, DataTable dtUnits)
        {
            if (enteredQty == 0m || itemRow == null || string.IsNullOrWhiteSpace(selectedUnitId))
                return enteredQty;

            string primaryUnitId = itemRow.Table.Columns.Contains("PrimaryUnit")
                ? Convert.ToString(itemRow["PrimaryUnit"]).Trim()
                : "";
            if (string.IsNullOrEmpty(primaryUnitId) || selectedUnitId.Trim() == primaryUnitId)
                return enteredQty;

            string additionalUnitsJson = itemRow.Table.Columns.Contains("AdditionalUnits")
                ? Convert.ToString(itemRow["AdditionalUnits"])
                : null;
            var subUnits = GetEffectiveSubUnits(primaryUnitId, additionalUnitsJson, dtUnits);
            var matched = subUnits.FirstOrDefault(u => u.UnitId == selectedUnitId.Trim());
            if (matched != null && matched.Factor > 1m)
            {
                return Math.Round(enteredQty / matched.Factor, 4, MidpointRounding.AwayFromZero);
            }

            return enteredQty;
        }

        public static void ResolveDisplayQtyAndUnit(
            decimal baseQty,
            DataRow itemRow,
            DataTable dtUnits,
            out decimal displayQty,
            out string displayUnitId)
        {
            displayQty = baseQty;
            displayUnitId = itemRow != null && itemRow.Table.Columns.Contains("PrimaryUnit")
                ? Convert.ToString(itemRow["PrimaryUnit"]).Trim()
                : "";

            if (baseQty <= 0m || itemRow == null || string.IsNullOrEmpty(displayUnitId))
                return;

            // Any fractional base qty (e.g. 0.050 kg -> 50 g, 1.250 kg -> 1250 g) converts to sub-unit
            bool isFractional = (baseQty % 1m) != 0m;
            if (!isFractional)
                return;

            string additionalUnitsJson = itemRow.Table.Columns.Contains("AdditionalUnits")
                ? Convert.ToString(itemRow["AdditionalUnits"])
                : null;
            var subUnits = GetEffectiveSubUnits(displayUnitId, additionalUnitsJson, dtUnits);
            if (subUnits.Count == 0)
                return;

            for (int i = 0; i < subUnits.Count; i++)
            {
                var sub = subUnits[i];
                if (sub.Factor > 1m)
                {
                    decimal candidate = Math.Round(baseQty * sub.Factor, 4, MidpointRounding.AwayFromZero);
                    if (Math.Abs(candidate - Math.Round(candidate)) < 0.001m)
                    {
                        displayQty = Math.Round(candidate);
                        displayUnitId = sub.UnitId;
                        return;
                    }
                }
            }

            var firstSub = subUnits[0];
            if (firstSub.Factor > 1m)
            {
                displayQty = Math.Round(baseQty * firstSub.Factor, 2, MidpointRounding.AwayFromZero);
                displayUnitId = firstSub.UnitId;
            }
        }

        public static (decimal DisplayQty, string UnitCode) ResolveDisplayQtyAndUnit(
            decimal baseQty,
            DataRow itemRow,
            DataTable dtUnits)
        {
            ResolveDisplayQtyAndUnit(baseQty, itemRow, dtUnits, out decimal dQty, out string dUnit);
            return (dQty, dUnit);
        }

        public static bool BindRowUnitComboBox(
            DataGridView dgv,
            int unitColIndex,
            int rowInd,
            DataRow itemRow,
            DataTable dtUnits,
            string selectedUnitId = null)
        {
            if (dgv == null || rowInd < 0 || rowInd >= dgv.Rows.Count)
                return false;
            return BindRowUnitComboBox(dgv.Rows[rowInd], unitColIndex, itemRow, dtUnits, selectedUnitId);
        }

        public static void UpdateUnitColumnVisibility(
            DataGridView dgv,
            int unitColIndex,
            int itemColIndex,
            DataTable dtItems,
            DataTable dtUnits)
        {
            if (dgv == null || unitColIndex < 0 || unitColIndex >= dgv.Columns.Count)
                return;
            var clnUnit = dgv.Columns[unitColIndex];
            var clnQty = dgv.Columns.Contains("clnQty") ? dgv.Columns["clnQty"] : null;
            UpdateUnitColumnVisibility(dgv, clnUnit, clnQty, dtItems, dtUnits);
        }
    }
}
