using System;
using System.IO.Compression;
using Buffer = MC.Classes.Buffer;

namespace MC.Abstract_and_Parent_Classes
{
    internal abstract class ListSElement : Entity
    {        

        public abstract void UpdateSize();

        public abstract void UpdateName(string newPath);

        public abstract void Open();

        public abstract Buffer Copy();

        public abstract void Paste(string path, Buffer buffer);

        
        public void Unarchive(string extractPath)
        {
            ZipFile.ExtractToDirectory(Path, extractPath);
        }
    }
}
