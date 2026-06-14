using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;
namespace ERP
{
    static  class  ImageCon
    {
        internal static  byte[] ToByteArray(this Image img)
        {
            if (img != null)
            {
               
                using (MemoryStream ms = new MemoryStream())
                {
                    Bitmap btm = new Bitmap(img); 
                    btm.Save(ms, img.RawFormat);
                    return ms.ToArray();
                }
            }
            else
                return null;
        }
        internal static Image byteArrayToImage(this byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                Image returnImage = Image.FromStream(ms);                
                return returnImage;
            }
        }
    }
}
