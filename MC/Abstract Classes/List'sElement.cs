using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.IO.Compression;

namespace MC
{
    abstract class List_sElement
    {
        public object Image { get; protected set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Date { get; set; }
        public string Path { get; protected set; }
        public string isArchive { get { return "Archive"; } }


        protected abstract void GetAndSetInfo();

        protected string FormatSize(long size)
        {
            string stringSize = "";
            double size_i = Convert.ToDouble(size);
            double d23 = Math.Pow(2, 30);
            if (size_i >= d23)
            {
                size_i /= d23;
                stringSize = String.Format("{0:f} GB", size_i);
            }
            else if (size_i >= 1024 * 1024)
            {
                size_i /= (1024 * 1024);
                stringSize = String.Format("{0:f} MB", size_i);
            }
            else if (size_i >= 1024)
            {
                size_i /= (1024);
                stringSize = String.Format("{0:f} KiB", size_i);
            }
            else
                stringSize = size + " B";

            return stringSize;
        }

        public abstract bool Open();

        public abstract Buffer Copy();

        public abstract void Paste(string path, Buffer buffer);

        public abstract void Archive(string pathZip);
        public void Unarchive(string extractPath)
        {
            //using (FileStream zipToOpen = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            //{
            //    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
            //    {
            //        archive.ExtractToDirectory(extractPath);
            //    }
            //}
            ZipFile.ExtractToDirectory(Path, extractPath);
        }
    }
}
