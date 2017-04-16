using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC
{
    public abstract class Buffer
    {
        public Buffer(string name)
        {
            this.name = name;

        }

        public string name { get; private set; }
    }

    public class FileBuffer : Buffer
    {
        public FileBuffer(string name, string tempPath) : base(name)
        {
            this.tempPath = tempPath;
        }

        public string tempPath { get; private set; }
    }

    public class FolderBuffer : Buffer
    {
        public FolderBuffer(string name, Buffer[] FolderBuffer) : base(name)
        {
            FoldersBuffer = FolderBuffer;
        }

        public Buffer[] FoldersBuffer { get; private set; }
    }

}

