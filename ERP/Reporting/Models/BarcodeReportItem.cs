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

        public static List<BarcodeLabelItem> GetMockData()
        {
            return new List<BarcodeLabelItem>
            {
                new BarcodeLabelItem { ItemId = "ITM-001", Title = "Nestle MilkPak 1000ml", Barcode = "896101400231", Rate = 295.00m, Category = "Dairy", Copies = 12 },
                new BarcodeLabelItem { ItemId = "ITM-002", Title = "Tapal Danedar Tea 950g", Barcode = "896400010452", Rate = 1420.00m, Category = "Beverages", Copies = 6 },
                new BarcodeLabelItem { ItemId = "ITM-003", Title = "Dalda Cooking Oil 5L Tin", Barcode = "896200055104", Rate = 2850.00m, Category = "Edible Oil", Copies = 4 },
                new BarcodeLabelItem { ItemId = "ITM-004", Title = "Ariel Washing Powder 1kg", Barcode = "896300098214", Rate = 680.00m, Category = "Laundry", Copies = 8 },
                new BarcodeLabelItem { ItemId = "ITM-005", Title = "Shan Biryani Masala 50g", Barcode = "896100078129", Rate = 110.00m, Category = "Spices", Copies = 24 },
                new BarcodeLabelItem { ItemId = "ITM-006", Title = "Olpers Milk 1000ml", Barcode = "896100554312", Rate = 290.00m, Category = "Dairy", Copies = 12 }
            };
        }
    }
}
