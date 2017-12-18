using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source.Entries
{
    public class EntityFactory
    {
        private static Zip zip;

        public static Entity GetEntity(string path)
        {
            if (System.IO.File.Exists(path)) {
                if (System.IO.Path.GetExtension(path).Equals(".zip"))
                {
                    if (zip != null)
                        zip.Dispose();
                    zip = new Zip(path);
                    return zip.GetRootFolder();
                }
                else
                    return new File(path);
            }
            if (System.IO.Directory.Exists(path))
                return new Folder(path);

            if (zip.)
        }
    }
}
