using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source.Entries.Zipped
{
    public class Entry
    {

        public string Path { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public long Size { get; set; }
        public long CompressedLength { get; set; }

        public Entry()
        { }

        public Entry(ZipArchiveEntry entry, string path)
        {
            this.Path = System.IO.Path.Combine(path, entry.FullName);
            this.Name = entry.Name;
            this.Date = entry.LastWriteTime.DateTime.ToString();
            this.Size = entry.Length;
            this.CompressedLength = entry.CompressedLength;
        }
    }
}
