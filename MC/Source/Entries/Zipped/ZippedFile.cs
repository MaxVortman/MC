using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MC.Source.Searchers;
using MC.Source.Visitors.EncryptVisitors;
using MC.Source.Visitors.ThreadVisitors;

namespace MC.Source.Entries.Zipped
{
    class ZippedFile : Entity, ISearchble
    {
        private Zip zip;
        private readonly Entry entry;

        public string CompressedLength { get; private set; }
        public string Path { get; private set; }

        public ZippedFile(Zip zip, Entry entry)
        {
            this.zip = zip;
            this.entry = entry;
            GetAndSetInfo();
        }

        private void GetAndSetInfo()
        {
            this.Path = entry.Path;
            FullPath = entry.FullPath;
            Name = entry.Name;
            Image = Etier.IconHelper.IconReader.IconFromFile(Name);
            Size = FormatSize(entry.Size);
            CompressedLength = FormatSize(entry.CompressedLength);
            Date = entry.Date;
        }

        public static bool IsFile(string path)
        {
            return !Regex.Match(path, $@"[\w|\W]+\\").Value.Equals(path);
        }

        public override void AcceptArchive(IThreadsVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override void AcceptDecode(IEncryptVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override void AcceptEncode(IEncryptVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override void AcceptSearch(IThreadsVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override Buffer Copy()
        {
            const int bytesToCopy = 16384;
            var partBufferFile = new byte[bytesToCopy];
            var tempPath = System.IO.Path.GetTempFileName();
            using (var inStream = zip.GetArchiveEntry(Path).Open())
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
                var newEntry = zip.CreateEntry(path);
                using (var outStream = newEntry.Open())
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

        public override void UpdateName(string newPath)
        {
            throw new NotImplementedException();
        }

        public override void UpdateSize()
        {
            throw new NotImplementedException();
        }

        public string ReadStreamToEnd()
        {
            using (var reader = new StreamReader(zip.GetArchiveEntry(Path).Open()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
