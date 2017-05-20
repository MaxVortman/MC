using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using MC.Abstract_and_Parent_Classes;
using MC.Classes;
using MC.Windows;
using Buffer = MC.Classes.Buffer;
using File = MC.Classes.File;

namespace MC
{
    class Folder : ListSElement
    {
        public Folder(string Path)
        {
            this.Path = Path;
            GetAndSetInfo();
        }
        DirectoryInfo dir;
        protected override void GetAndSetInfo()
        {
            Image = MainWindow.UserPrefs?.Theme.FolderIconPath;
            dir = new DirectoryInfo(Path);
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

        public override void UpdateSize()
        {
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
        }

        public override void UpdateName(string newPath)
        {
            Path = newPath;
            dir = new DirectoryInfo(Path);
            Name = dir.Name;
        }

        public override void Open()
        {
        }

        public override Buffer Copy()
        {

            List<ListSElement> DataList = getData();
            int count = DataList.Count;
            Buffer[] buffer = new Buffer[count];
            int i = 0;
            foreach (ListSElement elem in DataList)
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
            List<ListSElement> DataList = getData();
            int count = DataList.Count;
            int i = 0;
            foreach (ListSElement elem in DataList)
            {
                if (i < count)
                {
                    elem.Paste(System.IO.Path.Combine(path, elem.Name), filesBuffer[i]);
                    i++;
                }
            }
        }

        private List<ListSElement> getData()
        {
            //must be faster
            List<ListSElement> DataList = new List<ListSElement>(500);
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
