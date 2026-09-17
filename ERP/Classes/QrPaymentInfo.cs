using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ERP.Services.Legacy;

namespace ERP.Classes
{
    /// <summary>
    /// Holds the tenant-configured receiving bank account for QR payment,
    /// builds the official State Bank of Pakistan (SBP) Raast P2P/P2M QR payload,
    /// and manages a process-level cache.
    /// </summary>
    public class QrPaymentInfo
    {
        public string AccountTitle  { get; set; }
        public string AccountNumber { get; set; }
        public string BankName      { get; set; }
        public bool   IsEnabled     { get; set; }

        // ── Static cache ──────────────────────────────────────────────────────────
        private static QrPaymentInfo _cached;
        private static readonly object _lock = new object();

        public static QrPaymentInfo GetCached()
        {
            lock (_lock)
            {
                if (_cached == null)
                {
                    _cached = LoadFromApi();
                }
                return _cached;
            }
        }

        public static void ClearCache()
        {
            lock (_lock)
            {
                _cached = null;
            }
        }

        private static QrPaymentInfo LoadFromApi()
        {
            var result = new QrPaymentInfo();
            try
            {
                var svc = new SettingsApiService();
                string enabled = Task.Run(() => svc.GetSettingValueAsync("Bill.QrPayment.Enabled")).GetAwaiter().GetResult();
                result.IsEnabled     = string.Equals(enabled, "true", StringComparison.OrdinalIgnoreCase);
                result.AccountTitle  = Task.Run(() => svc.GetSettingValueAsync("Bill.QrPayment.AccountTitle")).GetAwaiter().GetResult()  ?? string.Empty;
                result.AccountNumber = Task.Run(() => svc.GetSettingValueAsync("Bill.QrPayment.AccountNumber")).GetAwaiter().GetResult() ?? string.Empty;
                result.BankName      = Task.Run(() => svc.GetSettingValueAsync("Bill.QrPayment.BankName")).GetAwaiter().GetResult()      ?? string.Empty;
            }
            catch
            {
                result.IsEnabled = false;
            }
            return result;
        }

        // ── Bank code directory & IBAN helper ─────────────────────────────────────

        private static readonly Dictionary<string, string> BankCodes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "meezan", "MEZN" },
            { "hbl", "HABB" },
            { "habib", "HABB" },
            { "ubl", "UNIL" },
            { "united", "UNIL" },
            { "mcb", "MUCB" },
            { "alfalah", "ALFH" },
            { "allied", "ABPA" },
            { "abl", "ABPA" },
            { "askari", "ASCM" },
            { "faysal", "FAYS" },
            { "bop", "BPUN" },
            { "punjab", "BPUN" },
            { "bankislami", "BKIP" },
            { "islami", "BKIP" },
            { "soneri", "SONE" },
            { "scb", "SCBL" },
            { "standard", "SCBL" },
            { "easypaisa", "TMFB" },
            { "telenor", "TMFB" },
            { "jazzcash", "MMBL" },
            { "mobilink", "MMBL" },
            { "sadapay", "SADA" },
            { "nayapay", "NPAY" },
            { "js", "JSBL" },
            { "albaraka", "BARK" },
            { "habibmetro", "HMBP" },
            { "dubai", "DIBP" }
        };

        /// <summary>
        /// Normalizes an input to a 24-character Pakistani IBAN.
        /// If already a 24-char IBAN starting with PK, returns it clean.
        /// If an account number is supplied with a known bank name, synthesizes
        /// a valid IBAN with correct ISO 7064 mod-97 check digits.
        /// </summary>
        public static string NormalizeToIban(string input, string bankName)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            string clean = Regex.Replace(input, @"[^A-Za-z0-9]", string.Empty).ToUpperInvariant();
            if (clean.Length == 24 && clean.StartsWith("PK", StringComparison.OrdinalIgnoreCase))
                return clean;
            if (clean.StartsWith("PK", StringComparison.OrdinalIgnoreCase))
                return clean;

            string bankCode = null;
            if (!string.IsNullOrWhiteSpace(bankName))
            {
                string bLower = Regex.Replace(bankName, @"[^A-Za-z0-9]", string.Empty).ToLowerInvariant();
                foreach (var kvp in BankCodes)
                {
                    if (bLower.Contains(kvp.Key))
                    {
                        bankCode = kvp.Value;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(bankCode))
                return clean;

            string account16 = clean.Length > 16 ? clean.Substring(clean.Length - 16) : clean.PadLeft(16, '0');

            string numStr = string.Empty;
            foreach (char c in bankCode)
            {
                numStr += (c - 55).ToString();
            }
            numStr += account16 + "252000";

            int remainder = 0;
            for (int i = 0; i < numStr.Length; i++)
            {
                remainder = (remainder * 10 + (numStr[i] - '0')) % 97;
            }
            int check = 98 - remainder;

            return "PK" + check.ToString("D2") + bankCode + account16;
        }

        /// <summary>
        /// Formats an IBAN with spaces every 4 characters for readability (e.g. "PK36 MEZN 0001 ...").
        /// </summary>
        public static string FormatIban(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban)) return string.Empty;
            string clean = Regex.Replace(iban, @"[^A-Za-z0-9]", string.Empty).ToUpperInvariant();
            return Regex.Replace(clean, ".{4}", "$0 ").Trim();
        }

        // ── SBP Raast QR payload builder ──────────────────────────────────────────

        /// <summary>
        /// Builds the official State Bank of Pakistan (SBP) Raast QR payload string.
        /// Scanned natively by Meezan Bank, HBL, UBL, Alfalah, MCB, Easypaisa, JazzCash, etc.
        ///
        /// Format layout:
        ///   00 02 "02"             Format Version (02)
        ///   01 02 "11" | "12"      Point of Initiation (11=Static, 12=Dynamic with amount)
        ///   02 02 "00"             Payload Type (00)
        ///   04 24 <IBAN>           24-character Pakistani IBAN
        ///   05 nn <amount>         Amount in PKR (optional)
        ///   10 04 <CRC-16>         CRC-16/CCITT-FALSE (tag 10, len 04)
        /// </summary>
        private static string FormatRaastExpiry(int daysFromNow = 30)
        {
            DateTime d = DateTime.Now.AddDays(daysFromNow);
            return d.ToString("ddMMyyyy") + "2359";
        }

        public string BuildEmvCoPayload(decimal amount)
        {
            string iban = NormalizeToIban(AccountNumber, BankName);
            bool hasAmount = amount > 0;
            string poi = hasAmount ? "12" : "11";

            string payload =
                Tlv("00", "02") +
                Tlv("01", poi) +
                Tlv("02", "00") +
                Tlv("04", iban);

            if (hasAmount)
            {
                // SBP Raast QR — amount encoded as integer with trailing dot, e.g. "23261."
                // Easypaisa has an off-by-one TLV parser: it reads (length + 1) characters for Tag 05,
                // picking up the first byte of the following tag. Appending a trailing "." makes that
                // extra byte a dot, which both Easypaisa and Meezan Bank silently strip when parsing
                // the numeric value — so both apps receive the correct whole-Rupee amount.
                decimal rounded = Math.Round(amount, MidpointRounding.AwayFromZero);
                string amtStr = ((long)rounded).ToString(System.Globalization.CultureInfo.InvariantCulture) + ".";
                payload += Tlv("05", amtStr);
                payload += Tlv("07", FormatRaastExpiry(30)); // Tag 07: Expiry (mandatory for dynamic SBP Raast)
            }

            payload += "1004";
            return payload + Crc16Hex(payload);
        }

        private static string Tlv(string tag, string value)
        {
            return tag + value.Length.ToString("D2") + value;
        }

        /// <summary>CRC-16/CCITT-FALSE (poly 0x1021, init 0xFFFF, no reflect, no final XOR).</summary>
        public static string Crc16Hex(string data)
        {
            ushort crc = 0xFFFF;
            foreach (char c in data)
            {
                crc ^= (ushort)(c << 8);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc = (ushort)(crc << 1);
                }
            }
            return crc.ToString("X4");
        }
    }
}
