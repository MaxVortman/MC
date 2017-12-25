using System;
using System.Text.RegularExpressions;
using System.Windows;
using MC.Source.Entries;

namespace MC.Source
{
    class FileManipulator
    {
        static Buffer _buffer;
        private static Entity _dataToCopy;

        internal static void CopyFile(Entity entity)
        {
            DeleteTemp(_buffer);
            
            try
            {
                _buffer = entity.Copy();
                _dataToCopy = entity;
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
            var sourcePath = elem.FullPath;
            var destinationPath = System.IO.Path.Combine(sourcePath.Remove(sourcePath.LastIndexOf(@"\", StringComparison.Ordinal)), text);

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

        internal static void CutFile(Entity entity)
        {
            try
            {
                CopyFile(entity);
                DeleteFile(entity);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void PasteFileBy(string path)
        {
            if (_buffer == null) return;
            if (path.Contains(".zip"))
                path = ExtractZipRootPath(path);
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

        private static string ExtractZipRootPath(string path)
        {
            return Regex.Match(path, $@"\.zip\\([\w|\W]*)").Groups[1].Value;
        }

        internal static void DeleteFile(object elem)
        {
            try
            {
                var item = elem as Entity;
                var path = item.FullPath;
                if (item is File)
                {
                    File.Delete(path);
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
