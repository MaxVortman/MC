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
        private List<ZipArchiveEntry> zipArchiveEntries;

        public string Path => path;

        public List<ZipArchiveEntry> ZipArchiveEntries { get => zipArchiveEntries; private set => zipArchiveEntries = value; }

        public Zip(string path)
        {
            this.path = path;
            CreateArchive();
            ZipArchiveEntries = new List<ZipArchiveEntry>(archive.Entries);
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
            foreach (var entry in ZipArchiveEntries)
            {
                if (!entry.FullName.Contains(baseFolderPath))
                    continue;

                if (IsFile(entry.FullName, baseFolderPathForRegexp))
                    baseFileEntity.Add(new ZippedFile(this, new Entry(entry, Path)));
                else
                {
                    var folderPath = GetFolderPath(entry.FullName, baseFolderPathForRegexp);
                    var fullFolderPath = System.IO.Path.Combine(Path, folderPath);
                    if (!baseEntity.Contains(e => e.FullPath == fullFolderPath))
                        baseEntity.Add(new ZippedFolder(this, fullFolderPath, folderPath, GetFolderName(folderPath)));
                }
            }
            baseEntity.AddRange(baseFileEntity);
            return baseEntity;
        }

        public List<Entity> GetFolderEntries(string folderPath)
        {
            var folderEntries = new List<Entity>();
            foreach (var entry in ZipArchiveEntries)
            {
                if (!entry.FullName.Contains(folderPath))
                    continue;
                folderEntries.Add(new ZippedFile(this, new Entry(entry, Path)));
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
            ZipArchiveEntries = new List<ZipArchiveEntry>(archive.Entries);
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
    }
}
