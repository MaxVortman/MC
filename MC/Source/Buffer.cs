namespace MC.Source
{
    public abstract class Buffer
    {
        protected Buffer(string name)
        {
            this.Name = name;

        }

        private string Name { get; set; }
    }

    public class FileBuffer : Buffer
    {
        public FileBuffer(string name, string tempPath) : base(name)
        {
            this.TempPath = tempPath;
        }

        public string TempPath { get; private set; }
    }

    public class FolderBuffer : Buffer
    {
        public FolderBuffer(string name, Buffer[] folderBuffer) : base(name)
        {
            FoldersBuffer = folderBuffer;
        }

        public Buffer[] FoldersBuffer { get; private set; }
    }

}

