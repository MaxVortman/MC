using MC.Source.Entries;
using MC.Source.Entries.Zipped;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MC.Source
{

    public class Zip : IDisposable
    {
        private ZipArchive archive;
        private readonly string path;
        private readonly Stream sourceStream;
        private List<string> filePaths;
        private List<string> folderPaths;

        public string Path => path;
        public List<ZipArchiveEntry> Entries { get; private set; }

        #region Get File Paths

        /// <summary>
        /// paths of file into the archive
        /// </summary>
        public List<string> FilePaths { get { return filePaths ?? GetFilePaths(); } }

        private List<string> GetFilePaths()
        {
            filePaths = (from f in Entries.AsParallel()
                         select f.FullName).ToList();
            return filePaths;
        }

        #endregion

        #region Get Folder Paths 

        /// <summary>
        /// paths of folder into the archive 
        /// </summary>
        public List<string> FolderPaths { get { return folderPaths ?? GetFolderPaths(); } }

        private List<string> GetFolderPaths()
        {
            var regex = new Regex($@"[\w|\W]+?\\");
            folderPaths = new List<string>();
            foreach (var entry in Entries)
            {
                foreach (Match m in regex.Matches(entry.FullName))
                {
                    if(!folderPaths.Contains(m.Value))
                        folderPaths.Add(m.Value);                    
                }
            }
            return folderPaths;
        }

        #endregion

        public Zip(string path, Stream sourceStream)
        {
            this.path = path;            
            this.sourceStream = sourceStream;
            CreateArchive();
            Entries = new List<ZipArchiveEntry>(archive.Entries);
        }


        private void CreateArchive()
        {            
            archive = new ZipArchive(sourceStream, ZipArchiveMode.Update, false);
        }

        #region Dispose

        public void Dispose()
        {
            DisposeZip();
            GC.SuppressFinalize(this);
        }

        private void DisposeZip()
        {
            archive.Dispose();
            sourceStream.Dispose();
        }

        #endregion


        public IEnumerable<string> GetEntityPaths(string baseFolderPath)
        {
            var entity = new List<string>();
            var baseFolderPathForRegexp = baseFolderPath.Replace("\\", "\\\\");
            foreach (var entry in Entries)
            {
                if (!entry.FullName.Contains(baseFolderPath))
                    continue;

                if (IsFile(entry.FullName, baseFolderPathForRegexp))
                    entity.Add(entry.FullName);
                else
                {
                    var folderPath = GetFolderPath(entry.FullName, baseFolderPathForRegexp);
                    if (!entity.Contains(folderPath))
                        entity.Add(folderPath);
                }
            }
            return entity;
        }

        private string GetFullPath(string folderPath)
        {
            return System.IO.Path.Combine(Path, folderPath);
        }

        public ZippedFolder GetRootFolder(MC.Source.Entries.Directory directory)
        {
            return new ZippedFolder(this, "", directory);
        }

        public List<Entity> GetFolderEntries(string folderPath)
        {
            var folderEntries = new List<Entity>();
            foreach (var entry in Entries)
            {
                if (!entry.FullName.Contains(folderPath))
                    continue;
                folderEntries.Add(new ZippedFile(this, new Entry(entry, Path)));
            }
            return folderEntries;
        }

        public IEnumerable<string> GetFilesPathFromFolder(string path)
        {
            var folderEntries = new List<string>();
            foreach (var entry in Entries)
            {
                if (!entry.FullName.Contains(path))
                    continue;
                folderEntries.Add(entry.FullName);
            }
            return folderEntries;
        }

        public ZipArchiveEntry GetArchiveEntry(string path)
        {
            return archive.GetEntry(path);
        }

        private static string GetFolderName(string folderPath)
        {
            if (folderPath.Contains('\\'))
                return folderPath.Substring(folderPath.LastIndexOf('\\') + 1);
            return folderPath;
        }

        private string GetFolderPath(string fullName, string baseFolderPath)
        {
            return Regex.Match(fullName, $@"({baseFolderPath}[\w|\W]+?)\\").Groups[1].Value;
        }

        private bool IsFile(string fullName, string baseFolderPath)
        {
            if (Regex.IsMatch(fullName, $@"{baseFolderPath}[\w|\W]+\\"))
            {
                return false;
            }
            return true;
        }

        public ZipArchiveEntry CreateEntry(string path)
        {
            var newEntry = archive.CreateEntry(path);
            Entries = new List<ZipArchiveEntry>(archive.Entries);
            return newEntry;
        }

        public Stream GetStream(string path)
        {
            return archive.GetEntry(path).Open();
        }        

        public bool IsFile(string path)
        {
            var file = (from e in Entries
                        where e.FullName == path
                        select e).FirstOrDefault();
            return file != null;
        }
    }
}
