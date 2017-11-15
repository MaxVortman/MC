using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MC.Source.Visitors.EncryptVisitors;
using MC.Source.Visitors.ThreadVisitors;
using System.IO;

namespace MC.Source.Entries.Zipped
{
    class ZippedFolder : Directory
    {
        private readonly Zip zip;

        public ZippedFolder(Zip zip, string path, string name)
        {
            this.zip = zip;
            this.Path = path;
            this.Name = name;
        }

        public override List<Entity> CreateDataList()
        {
            var dataList = new List<Entity>(50);
            // ... folder
            if (Path.Length > 3)
            {
                var parentPath = System.IO.Path.GetDirectoryName(Path);
                dataList.Add(new ZippedFolder(zip, parentPath, "..."));
            }
            return dataList;
        }

        protected override List<Entity> GetData(List<Entity> dataList)
        {
            return zip.GetEntity(dataList, Path);
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
