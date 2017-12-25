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
        private readonly Directory underDir;

        public string FolderPath { get; }

        public Zip Zip => zip;

        public ZippedFolder(Zip zip, string folderPath, Directory underDir)
        {
            this.zip = zip;
            this.FolderPath = folderPath;
            this.underDir = underDir;
            this.FullPath = System.IO.Path.Combine(zip.Path, folderPath);
            this.Name = GetName();
            this.Image = MainWindow.UserPrefs?.Theme.FolderIconPath;
        }

        private string GetName()
        {
            var name = Regex.Match(FullPath, $@"([\w|\W]+\\)?([\w|\W]+)").Groups[2].Value;
            return name;
        }

        public override List<Entity> CreateDataList()
        {
            var dataList = new List<Entity>(50);
            // ... folder
            if (underDir != null)
            {
                underDir.Name = "...";
                dataList.Add(underDir);
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
