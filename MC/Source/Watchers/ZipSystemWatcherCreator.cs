using MC.Source.Entries;
using MC.Source.Entries.Zipped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MC.Source.Watchers
{
    public class ZipSystemWatcherCreator : IWatcherCreator
    {
        private readonly Graphics.GraphicalApp graphics;
        private readonly Dispatcher dispatcher;
        private ZipSystemWatcher watcher;

        public ZipSystemWatcherCreator(Graphics.GraphicalApp graphics, Dispatcher dispatcher)
        {
            this.graphics = graphics;
            this.dispatcher = dispatcher;
            CreateWatcher();
        }

        private void CreateWatcher()
        {
            watcher = new ZipSystemWatcher();
            watcher.OnChanged += Watcher_OnChanged;
        }

        private void Watcher_OnChanged(object sender, SnapEventArgs e)
        {            
            dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
            {
                var data = e.Directory.CreateDataList();
                data.AddRange(EntityFactory.GetEntries(e.Directory));
                graphics.DataSource = new System.Collections.ObjectModel.ObservableCollection<Entity>(data);
                graphics.Refresh();                
            });
        }

        public void StartWatch(Directory directory)
        {
            watcher.Stop();
            watcher.Directory = (ZippedFolder)directory;
            watcher.Start();
        }
    }
}
