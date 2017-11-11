using System;
using System.IO;
using System.Windows;
using MC.Source.Entries;
using File = MC.Source.Entries.File;

namespace MC.Source
{
    class FileManipulator
    {
        static Buffer _buffer;
        private static Entity _dataToCopy;

        internal static void CopyFile(object elem)
        {
            DeleteTemp(_buffer);

            var item = elem as Entity;
            try
            {
                _buffer = item.Copy();
                _dataToCopy = item;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void DeleteTemp(Buffer buffer)
        {
            if (buffer == null) return;
            if (buffer is FileBuffer)
            {
                var fileBuffer = buffer as FileBuffer;
                System.IO.File.Delete(fileBuffer.TempPath);
            }
            else
            {
                var folderBuffer = buffer as FolderBuffer;
                foreach (var item in folderBuffer.FoldersBuffer)
                {
                    DeleteTemp(item);
                }
            }
        }

        internal static void RenameFile(object v, string text)
        {
            var elem = v as Entity;
            var sourcePath = elem.Path;
            var destinationPath = Path.Combine(sourcePath.Remove(sourcePath.LastIndexOf(@"\", StringComparison.Ordinal)), text);

            try
            {
                if (elem is File)
                {
                    System.IO.File.Move(sourcePath, destinationPath);
                }
                else
                       if (elem is Folder)
                {
                    System.IO.Directory.Move(sourcePath, destinationPath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void CutFile(object elem)
        {
            try
            {
                CopyFile(elem);
                DeleteFile(elem);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void PasteFileBy(string path)
        {
            if (_buffer == null) return;
            var fullPath = System.IO.Path.Combine(path, _dataToCopy.Name);
            try
            {
                _dataToCopy.Paste(fullPath, _buffer);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void DeleteFile(object elem)
        {
            try
            {
                var item = elem as Entity;
                var path = item.Path;
                if (item is File)
                {
                    System.IO.File.Delete(path);
                }
                else
                {
                    System.IO.Directory.Delete(path, true);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
