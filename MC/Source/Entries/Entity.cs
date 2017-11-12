using System;
using System.IO.Compression;
using MC.Source.Visitors;
using MC.Source.Visitors.EncryptVisitors;

namespace MC.Source.Entries
{
    public abstract class Entity
    {
        public object Image { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Date { get; set; }
        public string Path { get; protected set; }

        protected abstract void GetAndSetInfo();


        protected static string FormatSize(long size)
        {
            var stringSize = "";
            var sizeI = Convert.ToDouble(size);
            var d23 = Math.Pow(2, 30);
            if (sizeI >= d23)
            {
                sizeI /= d23;
                stringSize = $"{sizeI:f} GB";
            }
            else if (sizeI >= 1024 * 1024)
            {
                sizeI /= (1024 * 1024);
                stringSize = $"{sizeI:f} MB";
            }
            else if (sizeI >= 1024)
            {
                sizeI /= (1024);
                stringSize = $"{sizeI:f} KiB";
            }
            else
                stringSize = $"{size} B";

            return stringSize;
        }

        public abstract void UpdateSize();

        public abstract void UpdateName(string newPath);

        public abstract Buffer Copy();

        public abstract void Paste(string path, Buffer buffer);


        public void Unarchive(string extractPath)
        {
            ZipFile.ExtractToDirectory(Path, extractPath);
        }

        public abstract void AcceptArchive(IThreadsVisitor visitor);
        public abstract void AcceptSearch(IThreadsVisitor visitor);
        public abstract void AcceptDecode(IEncryptVisitor visitor);
        public abstract void AcceptEncode(IEncryptVisitor visitor);
    }
}
