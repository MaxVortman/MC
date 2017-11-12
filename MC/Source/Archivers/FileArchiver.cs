using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using MC.Source.Threading;
using Microsoft.Win32;

namespace MC.Source.Archivers
{
    abstract class FileArchiver : IThreder
    {
        private readonly object LockToken = new object();
        protected readonly string sourcePathOfFile;
        protected ZipArchive archive;
        private string pZip;
        private FileStream zipToOpen;
        protected Queue<string>[] filesQueue;
        protected FileQueueCreator fileQueueCreator;

        public abstract void DoThread();

        public FileArchiver(string sourcePathOfFile)
        {
            this.sourcePathOfFile = sourcePathOfFile;
            pZip = GetPathOnDialog();
            zipToOpen = new FileStream(pZip, FileMode.Create, FileAccess.ReadWrite, FileShare.Inheritable);
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
                fileEntry = archive.CreateEntry(filePath.Substring(sourcePathOfFile.Length + 1));
            }
            using (var InFileStream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var writer = fileEntry.Open())
                {
                    var bytesRead = 0;
                    do
                    {
                        bytesRead = InFileStream.Read(byteBuffer, 0, BufferSize);
                        writer.Write(byteBuffer, 0, bytesRead);
                    } while (bytesRead > 0);
                }
            }
        }


        private const string Archiveextension = "rar";
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


        protected void Dispose()
        {
            archive.Dispose();
            zipToOpen.Dispose();
        }
    }
}
