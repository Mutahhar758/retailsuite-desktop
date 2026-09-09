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
            get { return ConfigInfo._ThermalPrinterName; }
            set { ConfigInfo._ThermalPrinterName = value; }
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
    }
}
