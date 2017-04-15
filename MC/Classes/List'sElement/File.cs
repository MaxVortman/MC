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
using System.Diagnostics;

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
        

        public File(string Path)
        {
            this.Path = Path;
            GetAndSetInfo();
        }

        protected override void GetAndSetInfo()
        {
            Image = IconFromFile(Path);
            FileInfo f = new FileInfo(Path);
            Name = f.Name;
            Size = FormatSize(f.Length);
            Date = Convert.ToString(f.CreationTime);
        }

        public override bool Open()
        {
            try
            {
                Process.Start(Path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }
    }
}
