using MC.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Abstract_and_Parent_Classes
{
    abstract class FileArchiver
    {
        private readonly object LockToken = new object();
        protected readonly string sourcePathOfFile;
        protected ZipArchive archive;
        private string pZip;
        protected Queue<string>[] filesQueue;
        protected FileQueueCreator fileQueueCreator;


        internal abstract void Closing();
        public abstract void Archive();

        public FileArchiver(string sourcePathOfFile)
        {
            this.sourcePathOfFile = sourcePathOfFile;
            pZip = GetPathOnDialog();
            var zipToOpen = new FileStream(pZip, FileMode.Create, FileAccess.ReadWrite, FileShare.Inheritable);
            archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
            fileQueueCreator = new FileQueueCreator(sourcePathOfFile);
            filesQueue = fileQueueCreator.GetFilledQueueOfFilesPath();
        }

        protected void ArchiveFileInEntry(string filePath)
        {
            var BufferSize = 16384;
            var byteBuffer = new byte[BufferSize];
            ZipArchiveEntry fileEntry;
            lock (LockToken)
            {
                fileEntry = archive.CreateEntry(pZip.Substring(filePath.Length + 1));
            }
            using (var inFileStream = System.IO.File.Open(pZip, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var writer = fileEntry.Open())
                {
                    var bytesRead = 0;
                    do
                    {
                        bytesRead = inFileStream.Read(byteBuffer, 0, BufferSize);
                        writer.Write(byteBuffer, 0, bytesRead);
                    } while (bytesRead > 0);
                }
            }
        }


        private const string Archiveextension = "RAR";
        protected string GetPathOnDialog()
        {
            var fileDialog = new SaveFileDialog
            {
                Filter = "All Files | *.* ",
                AddExtension = true,
                DefaultExt = Archiveextension
            };
            //getting full file name, where we'll save the archive
            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            throw new ArgumentException("Pick folder pls.");
        }

    }
}
