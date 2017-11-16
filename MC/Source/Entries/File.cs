using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MC.Source.Visitors;
using MC.Source.Visitors.EncryptVisitors;
using MC.Source.Visitors.ThreadVisitors;

namespace MC.Source.Entries
{
    public class File : Entity
    {

        

        public File(string Path)
        {
            this.FullPath = Path;
            GetAndSetInfo();
        }

        private FileInfo _info;
        private void GetAndSetInfo()
        {            
            _info = new FileInfo(FullPath);
            Name = _info.Name;
            Image = Etier.IconHelper.IconReader.IconFromFile(Name);
            Size = FormatSize(_info.Length);
            Date = Convert.ToString(_info.CreationTime);
        }

        public override void UpdateName(string newPath)
        {
            FullPath = newPath;
            _info = new FileInfo(FullPath);
            Name = _info.Name;
        }

        public override void UpdateSize()
        {
            _info = new FileInfo(FullPath);
            Size = FormatSize(_info.Length);
        }

        public void Open()
        {
            try
            {
                Process.Start(FullPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override Buffer Copy()
        {
            const int bytesToCopy = 16384;
            var partBufferFile = new byte[bytesToCopy];
            var tempPath = System.IO.Path.GetTempFileName();
            using (var inStream = System.IO.File.Open(FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var outStream = System.IO.File.Open(tempPath, FileMode.Append, FileAccess.Write, FileShare.None))
                {
                    var bytesCopied = 0;
                    do
                    {
                        bytesCopied = inStream.Read(partBufferFile, 0, bytesToCopy);
                        if (bytesCopied > 0)
                        {
                            outStream.Write(partBufferFile, 0, bytesCopied);
                        }
                    } while (bytesCopied > 0);
                }
            }

            return new FileBuffer(Name, tempPath);
        }

        public override void Paste(string path, Buffer buffer)
        {
            var tempPath = (buffer as FileBuffer).TempPath;
            const int bytesToCopy = 16384;
            var partBufferFile = new byte[bytesToCopy];
            using (var inStream = System.IO.File.Open(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var outStream = System.IO.File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var bytesCopied = 0;
                    do
                    {
                        bytesCopied = inStream.Read(partBufferFile, 0, bytesToCopy);
                        if (bytesCopied > 0)
                        {
                            outStream.Write(partBufferFile, 0, bytesCopied);
                        }
                    } while (bytesCopied > 0);
                }
            }
        }

        public override void AcceptArchive(IThreadsVisitor visitor)
        {
            visitor.Archive(this);
        }

        public override void AcceptSearch(IThreadsVisitor visitor)
        {
            visitor.Search(this);
        }

        public override void AcceptDecode(IEncryptVisitor visitor)
        {
            visitor.Decode(this);
        }

        public override void AcceptEncode(IEncryptVisitor visitor)
        {
            visitor.Encode(this);
        }
    }
}
