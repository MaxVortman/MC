using System.IO;
using System.Threading;
using MC.Source.Graphics;
using MC.Source.Entries;
using Directory = MC.Source.Entries.Directory;
using File = MC.Source.Entries.File;
using System.Windows.Threading;

namespace MC.Source.Watchers
{
    internal class StandardWatcherCreator : IWatcherCreator
    {

        #region Constructor
        public StandardWatcherCreator(GraphicalApp graphicalApp, Dispatcher dispatcher)
        {
            this.graphicalApp = graphicalApp;
            this.dispatcher = dispatcher;
            CreateWatcher();
        }

        #endregion

        #region Private Properties

        private readonly GraphicalApp graphicalApp;
        private readonly Dispatcher dispatcher;
        private FileSystemWatcher watcher;
        #endregion

        #region Create standart Watcher

        private void CreateWatcher()
        {
            watcher = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.Size | NotifyFilters.FileName |
                               NotifyFilters.DirectoryName | NotifyFilters.CreationTime,
                Filter = "*.*"
            };
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            watcher.Renamed += new RenamedEventHandler(Watcher_Renamed);
        }

        #endregion

        #region Wather's events methods

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            var Data = graphicalApp.DataSource;
            Entity elem = null;
            for (var i = 0; i < Data.Count; i++)
            {
                var item = Data[i];
                if (item.FullPath != e.OldFullPath.Replace("/", "")) continue;
                elem = item;
                break;
            }
            dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
            {               
                if (elem != null)
                {
                    elem.UpdateName(e.FullPath);
                    graphicalApp.Refresh();
                }                
            });
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            //For the dynamics
            var Data = graphicalApp.DataSource;
            Entity elem = null;
            for (var i = 0; i < Data.Count; i++)
            {
                var item = Data[i];
                if (item.FullPath != e.FullPath) continue;
                elem = item;
                break;
            }
            dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
            {
                if (elem != null)
                {
                    elem.UpdateSize();
                    graphicalApp.Refresh();
                }

            });
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            //For the dynamics
            var Data = graphicalApp.DataSource;
            Entity elem = null;
            if (Directory.Exists(e.FullPath))
            {
                elem = new Folder(e.FullPath);
            }
            else if (System.IO.File.Exists(e.FullPath))
            {
                elem = new File(e.FullPath);
            }
            dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
            {                
                if (elem != null)
                {
                    Data.Add(elem);
                }                
            });
        }
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            var Data = graphicalApp.DataSource;
            Entity elem = null;
            foreach (var item in Data)
            {
                if (item.FullPath != e.FullPath) continue;
                elem = item;
                break;
            }
            dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
            {
                Data.Remove(elem);
            });            
        }

        #endregion

        #region IWatcher method
        public void StartWatch(Directory directory)
        {
            watcher.Path = directory.FullPath;
            watcher.EnableRaisingEvents = true;
        } 
        #endregion
    }
}
