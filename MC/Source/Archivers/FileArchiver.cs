using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows;
using MC.Source.QueueCreators;
using MC.Source.Threading;
using Microsoft.Win32;

namespace MC.Source.Archivers
{
    abstract class FileArchiver : IThreder, IDisposable
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
            try
            {
                this.sourcePathOfFile = sourcePathOfFile;
                pZip = GetPathOnDialog();
                zipToOpen = Entries.File.Open(pZip, FileMode.Create, FileAccess.ReadWrite, FileShare.Inheritable);
                archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update, false);
                fileQueueCreator = new FileQueueCreator(sourcePathOfFile);
                filesQueue = fileQueueCreator.GetFilledQueueOfFilesPath();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
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
            using (var InFileStream = MC.Source.Entries.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
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


        private const string Archiveextension = "zip";
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


        public void Dispose()
        {
            archive.Dispose();
            zipToOpen.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
