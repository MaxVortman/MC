using System.IO;
using System.Threading;
using System.Windows.Threading;
using MC.Source.Graphics;
using MC.Source.Entries;
using Directory = MC.Source.Entries.Directory;
using File = MC.Source.Entries.File;

namespace MC.Source
{
    class WatcherCreator
    {

        public WatcherCreator(GraphicalApp graphicalApp, Dispatcher dispatcher)
        {
            this.graphicalApp = graphicalApp;
            this._dispatcher = dispatcher;
        }        

        private readonly GraphicalApp graphicalApp;
        private readonly Dispatcher _dispatcher;

        public FileSystemWatcher CreateWatcher()
        {
            var watcher = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.Size | NotifyFilters.FileName |
                               NotifyFilters.DirectoryName | NotifyFilters.CreationTime,
                Filter = "*.*"
            };
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            watcher.Renamed += new RenamedEventHandler(Watcher_Renamed);
            return watcher;
        }

        
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            var Data = graphicalApp.DataSource;
            Entity elem = null;
            for (var i = 0; i < Data.Count; i++)
            {
                var item = Data[i];
                if (item.Path != e.OldFullPath.Replace("/", "")) continue;
                elem = item;
                break;
            }
            _dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
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
                if (item.Path != e.FullPath) continue;
                elem = item;
                break;
            }
            _dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
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
            _dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
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
                if (item.Path != e.FullPath) continue;
                elem = item;
                break;
            }
            _dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
            {
                Data.Remove(elem);
            });            
        }
    }
}
