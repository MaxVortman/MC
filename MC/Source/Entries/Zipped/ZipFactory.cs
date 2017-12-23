using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source.Entries.Zipped
{
    public static class ZipFactory
    {
        private static List<Zip> zips = new List<Zip>();

        public static ZippedFolder GetZippedFolder(string path, DirectoryType dirType , Directory directory)
        {
            var existZip = (from z in zips
                            where z.Path == path
                            select z).FirstOrDefault();
            if(existZip == default(Zip))
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
                directory.DisposeZipAction = () => { zip.Dispose(); };
                return zip.GetRootFolder(directory);
            }
            return existZip.GetRootFolder(directory);
        }
    }
}
