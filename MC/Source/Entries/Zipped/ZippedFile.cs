using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MC.Source.Visitors.EncryptVisitors;
using MC.Source.Visitors.ThreadVisitors;

namespace MC.Source.Entries.Zipped
{
    class ZippedFile : Entity
    {
        private Zip zip;
        private readonly Entry entry;

        public string CompressedLength { get; private set; }

        public ZippedFile(Zip zip, Entry entry)
        {
            this.zip = zip;
            this.entry = entry;
            GetAndSetInfo();
        }

        private void GetAndSetInfo()
        {
            FullPath = entry.Path;
            Name = entry.Name;
            Image = Etier.IconHelper.IconReader.IconFromFile(Name);
            Size = FormatSize(entry.Size);
            CompressedLength = FormatSize(entry.CompressedLength);
            Date = entry.Date;
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
            throw new NotImplementedException();
        }

        public override void Paste(string path, Buffer buffer)
        {
            throw new NotImplementedException();
        }

        public override void UpdateName(string newPath)
        {
            throw new NotImplementedException();
        }

        public override void UpdateSize()
        {
            throw new NotImplementedException();
        }
    }
}
