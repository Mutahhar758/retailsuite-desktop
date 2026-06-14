using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ERP.Classes
{
   public class Barcode
    {
        public static void GetBarcode(int height, int width, string data, string fileSaveUrl)
        {
            using (var img = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(img))
            using (var barBrush = new SolidBrush(Color.Black))
            using (var textBrush = new SolidBrush(Color.Black))
            using (var font = new Font("Arial", 8f))
            {
                graphics.Clear(Color.White);

                if (!string.IsNullOrWhiteSpace(data))
                {
                    var bytes = Encoding.ASCII.GetBytes(data);
                    var topPadding = 4;
                    var bottomPadding = 18;
                    var left = 4;
                    var drawableWidth = Math.Max(1, width - (left * 2));
                    var barHeight = Math.Max(1, height - topPadding - bottomPadding);
                    var totalUnits = Math.Max(1, (bytes.Length * 8) + 6);
                    var unitWidth = Math.Max(1, drawableWidth / totalUnits);

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

                    graphics.FillRectangle(barBrush, Math.Min(left, width - unitWidth - 1), topPadding, unitWidth, barHeight);
                }

                var textRectangle = new RectangleF(0, height - 16, width, 14);
                var textFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                graphics.DrawString(data ?? string.Empty, font, textBrush, textRectangle, textFormat);

                var directory = Path.GetDirectoryName(fileSaveUrl);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                img.Save(fileSaveUrl);
            }
        }
    }
}
