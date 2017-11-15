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

        public Zip(string path)
        {
            this.path = path;
            CreateArchive();
        }

        private void CreateArchive()
        {
            var file = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            archive = new ZipArchive(file, ZipArchiveMode.Read, false);
        }

        public void Dispose()
        {
            archive.Dispose();
            GC.SuppressFinalize(this);
        }  

        public List<Entity> GetEntity(List<Entity> baseEntity, string baseFolderPath = "")
        {
            var baseFolderPathForRegexp = baseFolderPath.Replace("\\", "\\\\");
            var entries = archive.Entries;
            foreach (var entry in entries)
            {
                if (!entry.FullName.Contains(baseFolderPath))
                    continue;

                if (IsFile(entry.FullName, baseFolderPathForRegexp))
                    baseEntity.Add(new ZippedFile(this, new Entry(entry)));
                else
                {
                    var folderPath = GetFolderPath(entry.FullName, baseFolderPathForRegexp);
                    if (!baseEntity.Contains(e => e.Path == folderPath))
                        baseEntity.Add(new ZippedFolder(this, folderPath, GetFolderName(folderPath)));
                }
            }
            return baseEntity;
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
    }
}
