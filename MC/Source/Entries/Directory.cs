using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MC.Source.Visitors;
using MC.Source.Visitors.EncryptVisitors;
using MC.Source.Visitors.ThreadVisitors;

namespace MC.Source.Entries
{
    public abstract class Directory : Entity
    {

        #region Data methods
        public List<Entity> GetEntries()
        {
            //must be faster
            var data = CreateDataList();
            return GetData(data);
        }

        public virtual IEnumerable<string> EnumerateFileSystemEntries()
        {
            return System.IO.Directory.EnumerateFileSystemEntries(this.FullPath);
        }

        public virtual List<Entity> CreateDataList()
        {
            var dataList = new List<Entity>(50);
            // ... folder
            if (FullPath.Length > 3)
            {
                var parentPath = System.IO.Path.GetDirectoryName(FullPath);
                dataList.Add(new Folder(parentPath) { Name = "...", Date = "", Size = "" });
            }
            return dataList;
        }


        public Action DisposeZipAction { get; set; }

        protected List<Entity> GetData(List<Entity> data)
        {
            DisposeZipAction?.Invoke();
            data.AddRange(EntityFactory.GetEntries(this));
            return data;
        } 
        #endregion

        public override Buffer Copy()
        {
            var dataList = GetAllSubFiles();
            int count = dataList.Count;
            Buffer[] buffer = new Buffer[count];
            int i = 0;
            foreach (Entity elem in dataList)
            {
                buffer[i] = elem.Copy();
                i++;                
            }

            return new FolderBuffer(Name, buffer);
        }

        public override void Paste(string path, Buffer buffer)
        {
            CreateDirectoryVirtual(path);

            Buffer[] filesBuffer = (buffer as FolderBuffer).FoldersBuffer;
            List<Entity> dataList = GetAllSubFiles();
            int count = dataList.Count;
            int i = 0;
            foreach (Entity elem in dataList)
            {
                elem.Paste(System.IO.Path.Combine(path, elem.Name), filesBuffer[i]);
                i++;
            }
        }   

        protected virtual List<Entity> GetAllSubFiles()
        {
            return GetData(new List<Entity>(50));
        }

        protected virtual void CreateDirectoryVirtual(string path)
        {
            Directory.CreateDirectory(path);   
        }

        public static System.IO.DirectoryInfo CreateDirectory(string path)
        {
            return System.IO.Directory.CreateDirectory(path);
        }

        public static bool Exists(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        #region Visitors methods
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
        #endregion

        public static IEnumerable<string> GetFiles(string path)
        {
            return System.IO.Directory.GetFiles(path);
        }

        public static IEnumerable<string> GetDirectories(string path)
        {
            return System.IO.Directory.GetDirectories(path);
        }

        public static IEnumerable<string> GetAllFiles(string path)
        {
            filesList = new List<string>();
            FillFilesList(path);
            return filesList;
        }

        private static List<string> filesList;
        private static void FillFilesList(string path)
        {
            Parallel.ForEach(Entries.Directory.GetFiles(path), item =>
            {
                filesList.Add(item);
            });
            Parallel.ForEach(Entries.Directory.GetDirectories(path), item =>
            {
                FillFilesList(item);
            });
        }
    }
}
