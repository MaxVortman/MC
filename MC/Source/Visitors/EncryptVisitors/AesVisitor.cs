using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MC.Source.Entries;
using MC.Windows;
using Microsoft.Win32;

namespace MC.Source.Visitors.EncryptVisitors
{
    public class AesVisitor : IEncryptVisitor
    {

        public void Decode(File file)
        {
            var aes = new AesEncryptor();
            aes.Decode(file.FullPath, GetDestinationFilePath(System.IO.Path.GetExtension(file.FullPath)), GetKeyFromDialog());
        }

        public void Decode(Directory directory)
        {
            var aes = new AesEncryptor();
            var entryList = (new FileQueueCreator(directory.FullPath)).ListOfFilesInDirectory;
            var destinationPath = GetDestinationFolderPath();
            var key = GetKeyFromDialog();
            foreach (var entry in entryList)
            {
                aes.Decode(entry, destinationPath + entry.Substring(directory.FullPath.Length), key);
            }
        }

        public void Encode(File file)
        {
            var aes = new AesEncryptor();
            aes.Encode(file.FullPath, GetDestinationFilePath(System.IO.Path.GetExtension(file.FullPath)), GetKeyFromDialog());
        }

        public void Encode(Directory directory)
        {
            var aes = new AesEncryptor();
            var entryList = (new FileQueueCreator(directory.FullPath)).ListOfFilesInDirectory;
            var destinationPath = GetDestinationFolderPath();
            var key = GetKeyFromDialog();
            foreach (var entry in entryList)
            {
                aes.Encode(entry, destinationPath + entry.Substring(directory.FullPath.Length), key);
            }
        }

        private byte[] GetKeyFromDialog()
        {
            var keyDialog = new KeyDialog();
            string key;
            keyDialog.ShowDialog();
            key = keyDialog.KeyString;
            var keyArr = new byte[32];
            if (key != string.Empty)
            {
                var bytes = Encoding.UTF8.GetBytes(key);
                bytes.CopyTo(keyArr, 0);
                return keyArr;
            }
            throw new Exception();
        }

        private string GetDestinationFolderPath()
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                return folderDialog.SelectedPath;
            }
            throw new ArgumentException("Choose Path pls.");
        }

        private string GetDestinationFilePath(string extension)
        {
            var fileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "All Files | *.* ",
                AddExtension = true,
                DefaultExt = extension
            };
            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            throw new ArgumentException("Pick folder pls.");
        }
    }
}
