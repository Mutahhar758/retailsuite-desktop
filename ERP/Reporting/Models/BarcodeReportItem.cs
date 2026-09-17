using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ERP.Reporting.Models
{
    /// <summary>
    /// Represents a product item for barcode sticker generation.
    /// </summary>
    public class BarcodeLabelItem
    {
        public string ItemId { get; set; }
        public string Title { get; set; }
        public string Barcode { get; set; }
        public decimal Rate { get; set; }
        public string Category { get; set; }
        public int Copies { get; set; } = 1;

        public string FormattedRate => "Rs. " + Rate.ToString("#,##0.00");
    }

    /// <summary>
    /// Generates barcode images and provides barcode sample data.
    /// </summary>
    public static class BarcodeDataService
    {
        /// <summary>
        /// Generates a clean 1D barcode image as PNG byte array.
        /// </summary>
        public static byte[] GenerateBarcodeBytes(string data, int width = 160, int height = 54)
        {
            if (string.IsNullOrWhiteSpace(data)) data = "1000001";

            using (var img = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(img))
            using (var barBrush = new SolidBrush(Color.Black))
            using (var textBrush = new SolidBrush(Color.Black))
            using (var font = new Font("Segoe UI", 7.5f, FontStyle.Regular))
            {
                graphics.Clear(Color.White);

                var bytes = Encoding.ASCII.GetBytes(data);
                var topPadding = 2;
                var bottomPadding = 14;
                var left = 4;
                var drawableWidth = Math.Max(1, width - (left * 2));
                var barHeight = Math.Max(1, height - topPadding - bottomPadding);
                var totalUnits = Math.Max(1, (bytes.Length * 8) + 6);
                var unitWidth = Math.Max(1, drawableWidth / totalUnits);

                // Start guard
                graphics.FillRectangle(barBrush, left, topPadding, unitWidth, barHeight);
                left += unitWidth * 2;

                foreach (var value in bytes)
                {
                    for (var bit = 7; bit >= 0; bit--)
                    {
                        if (((value >> bit) & 1) == 1)
                        {
                            graphics.FillRectangle(barBrush, left, topPadding, unitWidth, barHeight);
                        }
                        left += unitWidth;
                    }
                }

                // Stop guard
                graphics.FillRectangle(barBrush, Math.Min(left, width - unitWidth - 1), topPadding, unitWidth, barHeight);

                // Human readable text underneath
                var textRectangle = new RectangleF(0, height - 13, width, 12);
                var textFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                graphics.DrawString(data, font, textBrush, textRectangle, textFormat);

                using (var ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
    }
}
