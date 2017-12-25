using System;
using System.Collections.ObjectModel;
using System.Windows;
using MC.Source.Graphics;
using MC.Source.Entries;
using MC.Source.Watchers;

namespace MC.Source.Fillers
{
    class FileFiller
    {

        public FileFiller(GraphicalApp graphicalApp, WatchersFactory systemWatcherFactory)
        {
            this.graphicalApp = graphicalApp;
            this.systemWatcherFactory = systemWatcherFactory;
        }

        private readonly GraphicalApp graphicalApp;
        private WatchersFactory systemWatcherFactory;

        public void OpenEntry(Entity entity)
        {            
            try
            {                
                if (entity is Directory)
                {
                    var dir = entity as Directory;
                   // systemWatcherFactory.StartWatching(dir);           
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
