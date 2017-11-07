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
    class Folder : MC.Abstract_and_Parent_Classes.Directory
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
    }
}
