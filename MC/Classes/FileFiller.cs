using MC.Abstract_and_Parent_Classes;
using MC.Classes.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace MC.Classes
{
    class FileFiller
    {

        public FileFiller(GraphicalApp graphicalApp, FileSystemWatcher systemWatcher)
        {
            this.graphicalApp = graphicalApp;
            this.systemWatcher = systemWatcher;
        }

        private List<ListSElement> _dataList;
        private readonly GraphicalApp graphicalApp;
        private FileSystemWatcher systemWatcher;

        public void OpenElem(object o)
        {
            var elem = o as ListSElement;
            try
            {
                if (elem is Folder || elem is Drive)
                {
                    systemWatcher.Path = elem.Path;
                    systemWatcher.EnableRaisingEvents = true;
                    //start fill            

                    graphicalApp.SetCaptionOfPath(elem.Path);                    
                    FillInList(elem.Path);
                    graphicalApp.DataSource = new ObservableCollection<ListSElement>(_dataList);
                }
                else
                {
                    elem?.Open();
                }
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                CreateDataList(elem?.Path);
            }
        }

        private void FillInList(string path)
        {
            //must be faster
            CreateDataList(path);
            //enumerate folder's path
            foreach (var item in Directory.EnumerateDirectories(path))
            {
                _dataList.Add(new Folder(item));
            }
            //enumerate file's path
            foreach (var item in Directory.EnumerateFiles(path))
            {
                _dataList.Add(new File(item));
            }
        }

        private void CreateDataList(string path)
        {
            _dataList = new List<ListSElement>(500);
            // ... folder
            if (path.Length > 3)
            {
                var parentPath = Directory.GetParent(path).FullName;
                _dataList.Add(new Folder(parentPath) { Name = "...", Date = "", Size = "" });
            }
        }
    }
}
