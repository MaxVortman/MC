using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MC.Source.Searchers;

namespace MC.Source.Entries.Zipped
{
    public static class ZipFactory
    {
        private static List<Zip> zips = new List<Zip>();

        public static ZippedFolder GetZippedFolder(string path, DirectoryType dirType, Directory directory)
        {
            var zip = GetZip(path, dirType, directory);
            directory.DisposeZipAction = () => { zip.Dispose(); };
            return zip.GetRootFolder(directory);
        }

        private static Zip GetZip(string path, DirectoryType dirType, Directory directory = null)
        {
            Zip existZip = GetZipFromList(path);
            if (existZip == default(Zip))
            {
                Stream stream;
                switch (dirType)
                {
                    case DirectoryType.System:
                        stream = System.IO.File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        break;
                    case DirectoryType.Zipped:
                        stream = (directory as ZippedFolder).Zip.GetStream(path);
                        break;
                    default:
                        stream = null;
                        throw new ArgumentException("Stream is null!");
                }
                var zip = new Zip(path, stream);
                zips.Add(zip);
                return zip;
            }
            return existZip;
        }

        private static Zip GetZipFromList(string path)
        {
            return (from z in zips
                    where z.Path == path
                    select z).FirstOrDefault();
        }

        public static IEnumerable<ISearchble> GetZipEntries(string fPath)
        {
            var entityPaths = new List<ISearchble>();
            var zip = GetZip(fPath, DirectoryType.System);
            foreach (var entry in zip.Entries)
            {
                entityPaths.Add(new Entries.Zipped.ZippedFile(zip, new Entries.Zipped.Entry(entry, zip.Path)));
            }
            return entityPaths;
        }
    }
}
