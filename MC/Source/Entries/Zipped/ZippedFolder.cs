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

        public Zip Zip => zip;

        public ZippedFolder(Zip zip, string folderPath)
        {
            this.zip = zip;
            this.FolderPath = folderPath;
            this.FullPath = System.IO.Path.Combine(zip.Path, folderPath);
            this.Name = GetName();
            this.Image = MainWindow.UserPrefs?.Theme.FolderIconPath;
        }

        private string GetName()
        {
            var name = Regex.Match(FullPath, $@"[\w|\W]+\\([\w|\W]+)").Groups[1].Value;
            if (name[name.Length - 1] == '\\')
                name = name.Remove(name.Length - 1);
            return name;
        }

        public override List<Entity> CreateDataList()
        {
            var dataList = new List<Entity>(50);
            // ... folder
            if (FullPath.Length > 3)
            {
                if (string.IsNullOrEmpty(FolderPath))
                    dataList.Add(new Folder(System.IO.Path.GetDirectoryName(Zip.Path)){ Name = "..."});
                else
                {
                    var parentPath = System.IO.Path.GetDirectoryName(FolderPath);
                    var fullParentPath = System.IO.Path.GetDirectoryName(FullPath);
                    dataList.Add(new ZippedFolder(Zip, parentPath) { Name = "..."});
                }
            }
            return dataList;
        }



        public Entry GetEntry(string path)
        {
            return new Entry(zip.GetArchiveEntry(path), path);
        }

        public override IEnumerable<string> EnumerateFileSystemEntries()
        {
            return Zip.GetEntityPaths(FolderPath);
        }

        protected override void CreateDirectory(string path)
        {
            //we don't want to create directory
            //entries are creating directories automaticaly
        }

        protected override List<Entity> GetAllSubFiles()
        {
            return Zip.GetFolderEntries(FolderPath);
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
