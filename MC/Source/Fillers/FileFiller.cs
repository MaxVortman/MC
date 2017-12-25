using System;
using System.Collections.ObjectModel;
using System.Windows;
using MC.Source.Graphics;
using MC.Source.Entries;

namespace MC.Source.Fillers
{
    class FileFiller
    {

        public FileFiller(GraphicalApp graphicalApp, System.IO.FileSystemWatcher systemWatcher)
        {
            this.graphicalApp = graphicalApp;
            this.systemWatcher = systemWatcher;
        }

        private readonly GraphicalApp graphicalApp;
        private System.IO.FileSystemWatcher systemWatcher;

        public void OpenEntry(Entity entity)
        {            
            try
            {                
                if (entity is Directory)
                {
                    var dir = entity as Directory;
                    //systemWatcher.Path = dir.Path;
                    //systemWatcher.EnableRaisingEvents = true;
                    //start fill            

                    graphicalApp.SetCaptionOfPath(dir.FullPath);
                    var dataList = dir.GetEntries();
                    graphicalApp.DataSource = new ObservableCollection<Entity>(dataList);
                }
                else if(entity is File)
                {                
                    (entity as File)?.Open();
                }
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.IO.FileNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                var dataList = (entity as Directory).CreateDataList();
                graphicalApp.DataSource = new ObservableCollection<Entity>(dataList);
            }
        }
    }
}
