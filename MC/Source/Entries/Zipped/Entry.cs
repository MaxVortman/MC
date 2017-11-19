using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MC.Source.Entries.Zipped
{
    public class Entry
    {

        public string FullPath { get; }
        public string Path { get; }
        public string Name { get; }
        public string Date { get; }
        public long? Size { get; }
        public long? CompressedLength { get; }

        public Entry(ZipArchiveEntry entry, string path)
        {
            this.Path = entry.FullName;
            this.FullPath = System.IO.Path.Combine(path, entry.FullName);
            this.Name = entry.Name;
            this.Date = entry.LastWriteTime.DateTime.ToString();
            try
            {
                this.Size = entry.Length;
                this.CompressedLength = entry.CompressedLength;
            }
            catch (InvalidOperationException)
            {
                //to much exception
                //MessageBox.Show(e.Message, "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);                
            }
        }
    }
}
