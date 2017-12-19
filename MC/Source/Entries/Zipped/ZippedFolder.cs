using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MC.Source.Visitors.EncryptVisitors;
using MC.Source.Visitors.ThreadVisitors;
using System.IO;
using MC.Windows;
using System.Text.RegularExpressions;

namespace MC.Source.Entries.Zipped
{
    public class ZippedFolder : Directory
    {
        private readonly Zip zip;

        public string FolderPath { get; }

        public ZippedFolder(Zip zip, string fullPath, string folderPath, string name)
        {
            this.zip = zip;
            this.FolderPath = folderPath;
            this.Name = name;
            this.FullPath = fullPath;
            this.Image = MainWindow.UserPrefs?.Theme.FolderIconPath;
        }

        public override List<Entity> CreateDataList()
        {
            var dataList = new List<Entity>(50);
            // ... folder
            if (FullPath.Length > 3)
            {
                if (string.IsNullOrEmpty(FolderPath))
                    dataList.Add(new Folder(System.IO.Path.GetDirectoryName(zip.Path)){ Name = "..."});
                else
                {
                    var parentPath = System.IO.Path.GetDirectoryName(FolderPath);
                    var fullParentPath = System.IO.Path.GetDirectoryName(FullPath);
                    dataList.Add(new ZippedFolder(zip, fullParentPath, parentPath, "..."));
                }
            }
            return dataList;
        }

        public IEnumerable<string> EnumerateEntry()
        {
            return zip.GetEntity(FolderPath);
        }

        protected override void CreateDirectory(string path)
        {
            //we don't want to create directory
            //entries are creating directories automaticaly
        }

        protected override List<Entity> GetAllSubFiles()
        {
            return zip.GetFolderEntries(FolderPath);
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
