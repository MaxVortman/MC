using MC.Source.Entries;
using MC.Source.Entries.Zipped;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MC.Source
{

    public class Zip : IDisposable
    {
        private ZipArchive archive;
        private readonly string path;
        private List<string> filePaths;
        private List<string> folderPaths;

        public string Path => path;
        public List<ZipArchiveEntry> Entries { get; private set; }
        /// <summary>
        /// paths of file into the archive
        /// </summary>
        public List<string> FilePaths { get { return filePaths ?? GetFilePaths(); } }

        private List<string> GetFilePaths()
        {
            filePaths = (from f in Entries.AsParallel()
                         select GetFullPath(f.FullName)).ToList();
            return filePaths;
        }
        /// <summary>
        /// paths of folder into the archive 
        /// </summary>
        public List<string> FolderPaths { get { return folderPaths ?? GetFolderPaths(); } }

        private List<string> GetFolderPaths()
        {
            var regex = new Regex($@"\\[\w|\W]+?\\");
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

        public Zip(string path)
        {
            this.path = path;
            CreateArchive();
            Entries = new List<ZipArchiveEntry>(archive.Entries);
        }

        public IEnumerable<Entity> GetEntity(string path)
        {
            return GetEntity(new List<Entity>(), path);
        }

        private void CreateArchive()
        {
            var file = System.IO.File.Open(Path, FileMode.Open);
            archive = new ZipArchive(file, ZipArchiveMode.Update, false);
        }

        public void Dispose()
        {
            archive.Dispose();
            GC.Collect();
            GC.SuppressFinalize(this);           
        }  

        public List<Entity> GetEntity(List<Entity> baseEntity, string baseFolderPath = "")
        {
            var baseFileEntity = new List<Entity>();
            var baseFolderPathForRegexp = baseFolderPath.Replace("\\", "\\\\");
            foreach (var entry in Entries)
            {
                if (!entry.FullName.Contains(baseFolderPath))
                    continue;

                if (IsFile(entry.FullName, baseFolderPathForRegexp))
                    baseFileEntity.Add(new ZippedFile(this, new Entry(entry, Path)));
                else
                {
                    var folderPath = GetFolderPath(entry.FullName, baseFolderPathForRegexp);
                    string fullFolderPath = GetFullPath(folderPath);
                    if (!baseEntity.Contains(e => e.FullPath == fullFolderPath))
                        baseEntity.Add(new ZippedFolder(this, fullFolderPath, folderPath, GetFolderName(folderPath)));
                }
            }
            baseEntity.AddRange(baseFileEntity);
            return baseEntity;
        }

        private string GetFullPath(string folderPath)
        {
            return System.IO.Path.Combine(Path, folderPath);
        }

        public ZippedFolder GetRootFolder()
        {
            throw new NotImplementedException();
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

        public static bool IsArchive(string path)
        {
            if (System.IO.Path.GetExtension(path).Equals(".zip"))
            {
                return true;
            }
            return false;
        }

        public Stream GetStream(string path)
        {
            return archive.GetEntry(path).Open();
        }
    }
}
