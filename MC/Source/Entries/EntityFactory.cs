using MC.Source.Entries.Zipped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source.Entries
{
    public class EntityFactory
    {

        public static IEnumerable<Entity> GetEntries(Directory directory)
        {
            var data = new List<Entity>();
            foreach (var path in directory.EnumerateFileSystemEntries())
            {
                if (directory is Folder)
                {
                    data.Add(CreateFileSystemEntity(path));
                }
                else
                if (directory is ZippedFolder)
                {
                    data.Add(CreateZippedEntity(directory, path));
                }
            }
            return data;
        }

        private static Entity CreateZippedEntity(Directory directory, string path)
        {
            var zippedDir = directory as ZippedFolder;
            if (ZippedFile.IsFile(path))
            {
                if (System.IO.Path.GetExtension(path).Equals(".zip"))
                {
                    return CreateNewRootZippedFolder(path);
                }
                return new ZippedFile(zippedDir.Zip, zippedDir.GetEntry(path));
            }
            
                return new ZippedFolder(zippedDir.Zip, path);
        }

        private static Entity CreateFileSystemEntity(string path)
        {
            if (System.IO.Directory.Exists(path))
                return new Folder(path);

            if (System.IO.File.Exists(path))
            {
                if (System.IO.Path.GetExtension(path).Equals(".zip"))
                {
                    return CreateNewRootZippedFolder(path);
                }
                return new File(path);
            }
            throw new ArgumentException("This entity is superfluous.");
        }

        private static ZippedFolder CreateNewRootZippedFolder(string path)
        {
            using (var zip = new Zip(path))
            {
                return zip.GetRootFolder();
            }            
        }
    }
}
