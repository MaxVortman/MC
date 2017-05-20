using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MC.Abstract_and_Parent_Classes;

namespace MC.Classes
{
    internal class File : ListSElement
    {

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);
        private static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            var ip = source.GetHbitmap();
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

        private static ImageSource IconFromFile(string fileName)
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

        private FileInfo _info;
        protected sealed override void GetAndSetInfo()
        {
            try
            {
                Image = IconFromFile(Path);
                _info = new FileInfo(Path);
                Name = _info.Name;
                Size = FormatSize(_info.Length);
                Date = Convert.ToString(_info.CreationTime);
            }
            catch (FileNotFoundException)
            {
                Image = null;
                Name = null;
                Size = null;
                Date = null;
            }
        }

        public override void UpdateName(string newPath)
        {
            Path = newPath;
            _info = new FileInfo(Path);
            Name = _info.Name;
        }

        public override void UpdateSize()
        {
            _info = new FileInfo(Path);
            Size = FormatSize(_info.Length);
        }

        public override void Open()
        {
            try
            {
                Process.Start(Path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override Buffer Copy()
        {
            const int bytesToCopy = 16384;
            var partBufferFile = new byte[bytesToCopy];
            var tempPath = System.IO.Path.GetTempFileName();
            using (var inStream = System.IO.File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var outStream = System.IO.File.Open(tempPath, FileMode.Append, FileAccess.Write, FileShare.None))
                {
                    var bytesCopied = 0;
                    do
                    {
                        bytesCopied = inStream.Read(partBufferFile, 0, bytesToCopy);
                        if (bytesCopied > 0)
                        {
                            outStream.Write(partBufferFile, 0, bytesCopied);
                        }
                    } while (bytesCopied > 0);
                }
            }

            return new FileBuffer(Name, tempPath);
        }

        public override void Paste(string path, Buffer buffer)
        {
            var tempPath = (buffer as FileBuffer).TempPath;
            const int bytesToCopy = 16384;
            var partBufferFile = new byte[bytesToCopy];
            using (var inStream = System.IO.File.Open(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var outStream = System.IO.File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var bytesCopied = 0;
                    do
                    {
                        bytesCopied = inStream.Read(partBufferFile, 0, bytesToCopy);
                        if (bytesCopied > 0)
                        {
                            outStream.Write(partBufferFile, 0, bytesCopied);
                        }
                    } while (bytesCopied > 0);
                }
            }
        }

        public override void Archive(string pathZip)
        {
            const int bufferSize = 16384;
            var buffer = new byte[bufferSize];

            using (Stream inFileStream = System.IO.File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var writer = new GZipStream(System.IO.File.Open(pathZip, FileMode.Append, FileAccess.Write, FileShare.None), CompressionMode.Compress))
                {
                    int bytesRead = 0;
                    do
                    {
                        bytesRead = inFileStream.Read(buffer, 0, bufferSize);
                        writer.Write(buffer, 0, bytesRead);
                    } while (bytesRead > 0);
                }
            }
        }
    }
}
