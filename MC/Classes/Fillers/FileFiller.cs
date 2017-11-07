using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using MC.Abstract_and_Parent_Classes;
using MC.Classes.Graphics;

namespace MC.Classes.Fillers
{
    class FileFiller
    {

        public FileFiller(GraphicalApp graphicalApp, FileSystemWatcher systemWatcher)
        {
            this.graphicalApp = graphicalApp;
            this.systemWatcher = systemWatcher;
        }

        private readonly GraphicalApp graphicalApp;
        private FileSystemWatcher systemWatcher;

        public void OpenEntry(Entity entry)
        {
            try
            {
                if (entry is Abstract_and_Parent_Classes.Directory)
                {
                    var dir = entry as Abstract_and_Parent_Classes.Directory;
                    systemWatcher.Path = dir.Path;
                    systemWatcher.EnableRaisingEvents = true;
                    //start fill            

                    graphicalApp.SetCaptionOfPath(dir.Path);
                    var dataList = dir.GetEntry();
                    graphicalApp.DataSource = new ObservableCollection<Entity>(dataList);
                }
                else
                {
                    (entry as File)?.Open();
                }
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                var dataList = (entry as Abstract_and_Parent_Classes.Directory).CreateDataList();
                graphicalApp.DataSource = new ObservableCollection<Entity>(dataList);
            }
        }


    }
}
