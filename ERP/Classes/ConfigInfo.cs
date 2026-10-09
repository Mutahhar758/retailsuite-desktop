using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP
{
    class ConfigInfo
    {
        private static string _ThermalPrinterName;

        public static string ThermalPrinterName
        {
            get
            {
                if (string.IsNullOrEmpty(_ThermalPrinterName))
                {
                    _ThermalPrinterName = LoadThermalPrinter();
                }
                return ConfigInfo._ThermalPrinterName ?? "";
            }
            set { ConfigInfo._ThermalPrinterName = value; }
        }

        public static string LoadThermalPrinter()
        {
            try
            {
                var service = new Services.Legacy.SettingsApiService();
                string val = System.Threading.Tasks.Task.Run(() => service.GetSettingValueAsync("Printer.ThermalPrinter")).GetAwaiter().GetResult();
                if (!string.IsNullOrWhiteSpace(val))
                    return val;
            }
            catch { }

            try
            {
                string iniVal = INIFile.ReadValue("PrinterSetting", "ThermalPrinter");
                if (!string.IsNullOrWhiteSpace(iniVal))
                    return iniVal;
            }
            catch { }

            return "";
        }

        private static string _A4PrinterName;

        public static string A4PrinterName
        {
            get
            {
                if (string.IsNullOrEmpty(_A4PrinterName))
                {
                    _A4PrinterName = LoadA4Printer();
                }
                return ConfigInfo._A4PrinterName ?? "";
            }
            set { ConfigInfo._A4PrinterName = value; }
        }

        public static string LoadA4Printer()
        {
            try
            {
                var service = new Services.Legacy.SettingsApiService();
                string val = System.Threading.Tasks.Task.Run(() => service.GetSettingValueAsync("Printer.A4Printer")).GetAwaiter().GetResult();
                if (!string.IsNullOrWhiteSpace(val))
                    return val;
            }
            catch { }

            try
            {
                string iniVal = INIFile.ReadValue("PrinterSetting", "A4Printer");
                if (!string.IsNullOrWhiteSpace(iniVal))
                    return iniVal;
            }
            catch { }

            return "";
        }

        private static string _DefaultBillFormat;

        public static string DefaultBillFormat
        {
            get
            {
                if (string.IsNullOrEmpty(_DefaultBillFormat))
                {
                    _DefaultBillFormat = LoadDefaultBillFormat();
                }
                return _DefaultBillFormat ?? "A4 Sheet";
            }
            set { ConfigInfo._DefaultBillFormat = value; }
        }

        public static bool IsThermalDefault
        {
            get
            {
                string fmt = DefaultBillFormat;
                if (string.IsNullOrWhiteSpace(fmt))
                    return false;
                return fmt.IndexOf("thermal", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        public static string LoadDefaultBillFormat()
        {
            try
            {
                var service = new Services.Legacy.SettingsApiService();
                string val = System.Threading.Tasks.Task.Run(() => service.GetSettingValueAsync("Bill.DefaultFormat")).GetAwaiter().GetResult();
                if (!string.IsNullOrWhiteSpace(val))
                    return val;
            }
            catch { }

            try
            {
                string iniVal = INIFile.ReadValue("PrinterSetting", "DefaultBillFormat");
                if (!string.IsNullOrWhiteSpace(iniVal))
                    return iniVal;
            }
            catch { }

            return "A4 Sheet";
        }

        private static string _ThankyouLine;

        public static string ThankyouLine
        {
            get
            {
                if (string.IsNullOrEmpty(_ThankyouLine))
                {
                    _ThankyouLine = LoadThankyouLine();
                }
                return _ThankyouLine ?? "";
            }
            set { ConfigInfo._ThankyouLine = value; }
        }

        public static string LoadThankyouLine()
        {
            try
            {
                var service = new Services.Legacy.SettingsApiService();
                string val = System.Threading.Tasks.Task.Run(() => service.GetSettingValueAsync("Bill.ThankYouMessage")).GetAwaiter().GetResult();
                if (!string.IsNullOrWhiteSpace(val))
                    return val;
            }
            catch
            {
            }

            try
            {
                string iniVal = INIFile.ReadValue("BillSetting", "ThankyouLine");
                if (!string.IsNullOrWhiteSpace(iniVal))
                    return iniVal;
            }
            catch
            {
            }

            return "Thank you for shopping with us!";
        }

        public static void Reset()
        {
            _ThermalPrinterName = null;
            _A4PrinterName = null;
            _DefaultBillFormat = null;
            _ThankyouLine = null;
        }
    }
}
