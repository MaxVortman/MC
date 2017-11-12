using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MC.Source;

namespace MC.Tests
{
    [TestClass]
    public class AesTest
    {
        private const string SourceFilePath = @"D:/unitTest1.txt";
        private const string DestinationFilePath = @"D:/unitTest2.txt";
        private const string TestText = "I Love You";
        private readonly byte[] Key = new byte[] { 12, 13, 12, 14, 67, 4, 36, 7, 3, 5, 6, 4, 6, 4, 2, 6, 8, 95, 5, 5, 36, 232, 35, 6, 1, 6, 67, 8, 5, 57, 87, 1 };

        [TestMethod]
        public void ShouldEncryptAndDecryptFileCorrectly()
        {
            //arrange
            using (var fileStream = System.IO.File.Open(SourceFilePath, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Write))
            {
                using (var writer = new System.IO.StreamWriter(fileStream))
                {
                    writer.Write(TestText);
                }
            }
            //act
            var aes = new AesEncryptor();
            aes.Encode(SourceFilePath, DestinationFilePath, Key);
            aes.Decode(DestinationFilePath, SourceFilePath, Key);
            //asserts
            using (var fileStream = System.IO.File.Open(SourceFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                using (var reader = new System.IO.StreamReader(fileStream))
                {
                    var plainText = reader.ReadToEnd();
                    Assert.AreEqual(TestText, plainText);
                }
            }
        }

        [TestCleanup]
        public void Clean()
        {
            System.IO.File.Delete(SourceFilePath);
            System.IO.File.Delete(DestinationFilePath);
        }
    }
}
