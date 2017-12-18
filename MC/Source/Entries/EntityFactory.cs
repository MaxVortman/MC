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
        /// <summary>
        /// Factory for Entity
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Entity GetEntity(string path)
        {
            if (System.IO.Directory.Exists(path))
                return new Folder(path);
            if (System.IO.File.Exists(path))
            {
                if (System.IO.Path.GetExtension(path).Equals(".zip"))
                {
                    using (var zip = new Zip(path))
                    {
                        return zip.GetRootFolder();
                    }
                }
                return new File(path);
            }
            throw new ArgumentException("This entity is superfluous.");
        }
    }
}
