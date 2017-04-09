using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace MC
{
    class File : List_sElement
    {

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        private static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }
        private ImageSource IconFromFile(string fileName)
        {
            var icon = System.Drawing.Icon.ExtractAssociatedIcon(fileName);
            var bmp = icon.ToBitmap();
            return loadBitmap(bmp);
        }
        private string FormatSize(string size)
        {
            Int64 size_i = Convert.ToInt64(size);
            double d23 = Math.Pow(2, 33);
            if (size_i >= d23)
            {
                size_i /= (long)d23;
                size = size_i.ToString() + "GB";
            }
            else if (size_i >= 1024 * 8 * 1024)
            {
                size_i /= (1024 * 8 * 1024);
                size = size_i.ToString() + "MB";
            }
            else if (size_i >= 1024 * 8)
            {
                size_i /= (1024 * 8);
                size = size_i.ToString() + "KiB";
            }
            else
                size += "B";
            
            return size;
        }

        public File(string Path) : base(Path)
        {
        }

        protected override void GetAndSetInfo()
        {
            Image = IconFromFile(Path);
            FileInfo f = new FileInfo(Path);
            Name = f.Name;
            Size = FormatSize(f.Length.ToString());
            Date = Convert.ToString(f.CreationTime);
        }
    }
}
