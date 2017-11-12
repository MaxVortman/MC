using System.Collections.Generic;
using MC.Source.Visitors;
using MC.Source.Visitors.EncryptVisitors;
using MC.Source.Visitors.ThreadVisitors;

namespace MC.Source.Entries
{
    public abstract class Directory : Entity
    {
        
        public List<Entity> GetEntry()
        {
            //must be faster
            var dataList = CreateDataList();
            return GetData(dataList);
        }

        public List<Entity> CreateDataList()
        {
            var dataList = new List<Entity>(500);
            // ... folder
            if (Path.Length > 3)
            {
                var parentPath = System.IO.Directory.GetParent(Path).FullName;
                dataList.Add(new Folder(parentPath) { Name = "...", Date = "", Size = "" });
            }
            return dataList;
        }

        protected List<Entity> GetData(List<Entity> dataList)
        {
            //enumerate folder's path
            foreach (var item in System.IO.Directory.EnumerateDirectories(Path))
            {
                dataList.Add(new Folder(item));
            }
            //enumerate file's path
            foreach (var item in System.IO.Directory.EnumerateFiles(Path))
            {
                dataList.Add(new File(item));
            }

            return dataList;
        }

        public override Buffer Copy()
        {

            var dataList = GetData(new List<Entity>(500));
            int count = dataList.Count;
            Buffer[] buffer = new Buffer[count];
            int i = 0;
            foreach (Entity elem in dataList)
            {
                if (i < count)
                {
                    buffer[i] = elem.Copy();
                    i++;
                }
            }

            return new FolderBuffer(Name, buffer);
        }

        public override void Paste(string path, Buffer buffer)
        {

            System.IO.Directory.CreateDirectory(path);

            Buffer[] filesBuffer = (buffer as FolderBuffer).FoldersBuffer;
            var dataList = GetData(new List<Entity>(500));
            int count = dataList.Count;
            int i = 0;
            foreach (Entity elem in dataList)
            {
                if (i < count)
                {
                    elem.Paste(System.IO.Path.Combine(path, elem.Name), filesBuffer[i]);
                    i++;
                }
            }
        }

        public static bool Exists(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public override void AcceptArchive(IThreadsVisitor visitor)
        {
            visitor.Archive(this);
        }

        public override void AcceptSearch(IThreadsVisitor visitor)
        {
            visitor.Search(this);
        }

        public override void AcceptDecode(IEncryptVisitor visitor)
        {
            visitor.Decode(this);
        }

        public override void AcceptEncode(IEncryptVisitor visitor)
        {
            visitor.Encode(this);
        }
    }
}
