using MC.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source.Watchers
{
    internal class SnapShot
    {

        public string Path { get; }
        public Zip Zip { get; }
        public List<byte[]> Hashes { get; private set; }

        public SnapShot(Zip zip, string path)
        {
            Zip = zip;
            Path = path;
        }



        private void MakeSnapShot()
        {
            Hashes = new List<byte[]>();
            foreach (var entry in Zip.GetFilesPathFromFolder(Path))
            {
                Hashes.Add(CalculateMD5Hash(entry));
            }
        }

        private MD5Cng mD5 = new MD5Cng();
        private byte[] CalculateMD5Hash(string path)
        {
            using (var entryStream = Zip.GetStream(path))
                return mD5.ComputeHash(entryStream);
        }

        public bool IsChanged(SnapShot anotherSnap)
        {
            if (this.Zip != anotherSnap.Zip || this.Path != anotherSnap.Path)
                throw new ArgumentException("Snap shots of different folders");
            foreach (var hashes in this.Hashes.Zip(anotherSnap.Hashes, Tuple.Create))
            {
                foreach (var hash in hashes.Item1.Zip(hashes.Item2, Tuple.Create))
                {
                    if (hash.Item1 != hash.Item2)
                        return true;
                }
            }
            return false;
        }
    }
}
