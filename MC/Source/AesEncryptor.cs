using MC.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source
{
    public class AesEncryptor
    {
        private readonly byte[] IV = new byte[] { 1, 2, 128, 3, 4, 5, 6, 7, 8, 9, 0, 10, 15, 13, 0, 99};
        private char[] buffer;
        private const int BufferSize = 1028;

        public void Decode(string sourcePath, string destinationPath, byte[] key)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var sourceFileStream = Entries.File.Open(sourcePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
                {
                    using (var destinationFileStream = Entries.File.Open(destinationPath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
                    {
                        using (var csDecrypt = new CryptoStream(sourceFileStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                            {
                                using (var writer = new System.IO.StreamWriter(destinationFileStream))
                                {
                                    var bytesCount = 0;
                                    buffer = new char[BufferSize];
                                    do
                                    {
                                        
                                        bytesCount = srDecrypt.ReadBlock(buffer, 0, BufferSize);
                                        writer.Write(buffer, 0, bytesCount);
                                    } while (bytesCount > 0);
                                }
                            }
                        }
                    }
                }
            }
        }
        

        public void Encode(string sourcePath, string destinationPath, byte[] key)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var sourceFileStream = Entries.File.Open(sourcePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
                {
                    using (var destinationFileStream = Entries.File.Open(destinationPath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
                    {
                        using (var csEncrypt = new CryptoStream(destinationFileStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                            {
                                using (var reader = new System.IO.StreamReader(sourceFileStream))
                                {
                                    var bytesCount = 0;
                                    buffer = new char[BufferSize];
                                    do
                                    {
                                        bytesCount = reader.ReadBlock(buffer, 0, BufferSize);
                                        swEncrypt.Write(buffer, 0, bytesCount);
                                    } while (bytesCount > 0);
                                }
                            }
                        }
                    }
                }
            }   
        }
    }
}
