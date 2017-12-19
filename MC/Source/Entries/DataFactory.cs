using MC.Source.Entries.Zipped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source.Entries
{
    public class DataFactory
    {
        public static IEnumerable<string> GetData(Directory dir)
        {
            if (dir is Folder)
                return System.IO.Directory.EnumerateFileSystemEntries(dir.FullPath);
            if(dir is ZippedFolder)
                return (dir as )
        } 
    }
}
