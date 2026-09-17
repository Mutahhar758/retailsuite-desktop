using System;
using QRCoder;

namespace ERP.Classes
{
    /// <summary>
    /// Static helper that generates QR code PNG bytes from a string payload.
    /// Uses QRCoder (MIT license, no native dependencies) and produces raw PNG
    /// bytes compatible with QuestPDF's .Image(byte[]) API.
    /// </summary>
    public static class QrCodeHelper
    {
        /// <summary>
        /// Generates a QR code PNG as a byte array.
        /// </summary>
        /// <param name="payload">The data to encode (e.g. an EMVCo MPM string).</param>
        /// <param name="pixelSize">Size of each QR module in pixels. Default = 5 (~80pt at 96dpi).</param>
        /// <param name="eccLevel">Error correction level. EMVCo requires at least 'M'. Default = M.</param>
        /// <returns>PNG bytes suitable for QuestPDF .Image() or System.Drawing.Image.</returns>
        public static byte[] GeneratePng(
            string payload,
            int pixelSize = 5,
            QRCodeGenerator.ECCLevel eccLevel = QRCodeGenerator.ECCLevel.M)
        {
            if (string.IsNullOrWhiteSpace(payload))
                return Array.Empty<byte>();

            using (var generator = new QRCodeGenerator())
            using (QRCodeData data = generator.CreateQrCode(payload, eccLevel))
            using (var pngCode = new PngByteQRCode(data))
            {
                return pngCode.GetGraphic(pixelSize);
            }
        }
    }
}
