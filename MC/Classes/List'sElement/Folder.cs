using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace MC
{
    class Folder : List_sElement
    {
        public Folder(string Path)
        {
            this.Path = Path;
            GetAndSetInfo();
        }
        
        protected override void GetAndSetInfo()
        {
            Image = "/Images/Icons/Folder1.png";
            DirectoryInfo dir = new DirectoryInfo(Path);
            Name = dir.Name;
            try
            {
                int count = dir.GetFileSystemInfos().Count();
                Size = count.ToString() + " item";
                if (count != 1)
                    Size += "s";
            }
            catch (UnauthorizedAccessException)
            {
                Size = "?? items";    
            }
            Date = Convert.ToString(dir.CreationTime);
        }

        public override bool Open()
        {
            return true;
        }

        public override Buffer Copy()
        {

            List<List_sElement> DataList = getData();
            int count = DataList.Count;
            Buffer[] buffer = new Buffer[count];
            int i = 0;
            foreach (List_sElement elem in DataList)
            {
                if (i < count)
                {
                    buffer[i] = elem.Copy();
                    i++;
                }
            }

            return new FolderBuffer(Name, buffer);
        }

        public override void Paste(string path, Buffer buffer)
        {

            Directory.CreateDirectory(path);

            Buffer[] filesBuffer = (buffer as FolderBuffer).FoldersBuffer;
            List<List_sElement> DataList = getData();
            int count = DataList.Count;
            int i = 0;
            foreach (List_sElement elem in DataList)
            {
                if (i < count)
                {
                    elem.Paste(System.IO.Path.Combine(path, elem.Name), filesBuffer[i]);
                    i++;
                }
            }
        }

        private List<List_sElement> getData()
        {
            //must be faster
            List<List_sElement> DataList = new List<List_sElement>(500);
            //enumerate folder's path
            foreach (var item in Directory.EnumerateDirectories(Path))
            {
                DataList.Add(new Folder(item));
                //graphics.AddLine(new Folder(item));
            }
            //enumerate file's path
            foreach (var item in Directory.EnumerateFiles(Path))
            {
                DataList.Add(new File(item));
                //graphics.AddLine(new File(item));
            }

            return DataList;
        }

        public override void Archive(string pathZip)
        {
            using (FileStream zipToOpen = new FileStream(pathZip, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    ArchiveFilesInDirectory(archive, Path);
                }
            }
        }

        private void ArchiveFilesInDirectory(ZipArchive archive, string path)
        {
            foreach (var item in Directory.GetFiles(path))
            {
                int BufferSize = 16384;
                byte[] buffer = new byte[BufferSize];
                ZipArchiveEntry fileEntry = archive.CreateEntry(item.Substring(item.LastIndexOf(Path) + Path.Length + 1));
                using (Stream inFileStream = System.IO.File.Open(item, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (Stream writer = fileEntry.Open())
                    {
                        int bytesRead = 0;
                        do
                        {
                            bytesRead = inFileStream.Read(buffer, 0, BufferSize);
                            writer.Write(buffer, 0, bytesRead);
                        } while (bytesRead > 0);
                    } 
                }

            }
            foreach (var item in Directory.GetDirectories(path))
            {
                ArchiveFilesInDirectory(archive, item);
            }
        }        
    }
}
