﻿using System;
using System.IO;
using System.Linq;
using MC.Windows;

namespace MC.Source.Entries
{
    public class Folder : Directory
    {
        public Folder(string Path)
        {
            this.FullPath = Path;
            GetAndSetInfo();
        }

        DirectoryInfo dir;
        private void GetAndSetInfo()
        {
            Image = MainWindow.UserPrefs?.Theme.FolderIconPath;
            dir = new DirectoryInfo(FullPath);
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
            FullPath = newPath;
            dir = new DirectoryInfo(FullPath);
            Name = dir.Name;
        }       
    }
}
