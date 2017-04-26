﻿using System;
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
using System.IO.Compression;

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


        public new string isArchive
        {
            get
            {
                if (System.IO.Path.GetExtension(Path) == "rar")
                {
                    return "Unarchive";
                }
                return "Archive";
            }
        }

        public File(string Path)
        {
            this.Path = Path;
            GetAndSetInfo();
        }
        FileInfo info;
        protected override void GetAndSetInfo()
        {
            Image = IconFromFile(Path);
            info = new FileInfo(Path);
            Name = info.Name;
            Size = FormatSize(info.Length);
            Date = Convert.ToString(info.CreationTime);
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

        public override Buffer Copy()
        {
            int bytesCopied = 0;
            int bytesToCopy = 16384;
            byte[] PartBuffer = new byte[bytesToCopy];
            string tempPath = System.IO.Path.GetTempFileName();
            using (FileStream inStream = System.IO.File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (FileStream outStream = System.IO.File.Open(tempPath, FileMode.Append, FileAccess.Write, FileShare.None))
                {
                    do
                    {
                        bytesCopied = inStream.Read(PartBuffer, 0, bytesToCopy);
                        if (bytesCopied > 0)
                        {
                            outStream.Write(PartBuffer, 0, bytesCopied);
                        }
                    } while (bytesCopied > 0);
                }
            }

            return new FileBuffer(Name, tempPath);
        }

        public override void Paste(string path, Buffer buffer)
        {
            string tempPath = (buffer as FileBuffer).tempPath;
            int bytesCopied = 0;
            int bytesToCopy = 16384;
            byte[] PartBuffer = new byte[bytesToCopy];
            using (FileStream inStream = System.IO.File.Open(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (FileStream outStream = System.IO.File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    do
                    {
                        bytesCopied = inStream.Read(PartBuffer, 0, bytesToCopy);
                        if (bytesCopied > 0)
                        {
                            outStream.Write(PartBuffer, 0, bytesCopied);
                        }
                    } while (bytesCopied > 0);
                }
            }
            // System.IO.File.Delete(tempPath);
        }

        public override void Archive(string pathZip)
        {
            int BufferSize = 16384;
            byte[] buffer = new byte[BufferSize];

            using (Stream inFileStream = System.IO.File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (GZipStream writer = new GZipStream(System.IO.File.Open(pathZip, FileMode.Append, FileAccess.Write, FileShare.None), CompressionMode.Compress))
                {
                    int bytesRead = 0;
                    do
                    {
                        bytesRead = inFileStream.Read(buffer, 0, BufferSize);
                        writer.Write(buffer, 0, bytesRead);
                    } while (bytesRead > 0);
                }
            }
        }
    }
}
