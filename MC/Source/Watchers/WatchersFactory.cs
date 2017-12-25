using MC.Source.Entries;
using MC.Source.Entries.Zipped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MC.Source.Watchers
{
    public class WatchersFactory
    {
        private readonly Graphics.GraphicalApp graphics;
        private readonly Dispatcher dispatcher;
        private StandardWatcherCreator standardWatcherCreator;
        private ZipSystemWatcherCreator zipWatcherCreator;

        public WatchersFactory(Graphics.GraphicalApp graphics, Dispatcher dispatcher)
        {
            this.graphics = graphics;
            this.dispatcher = dispatcher;
        }

        public void StartWatching(Directory directory)
        {
            var watcherCreator = GetWatcherCreator(directory);
            watcherCreator.StartWatch(directory);
        }

        private IWatcherCreator GetWatcherCreator(Directory directory)
        {
            if (directory is Folder)         
                return standardWatcherCreator ?? (standardWatcherCreator = new StandardWatcherCreator(graphics, dispatcher));
            if (directory is ZippedFolder)
                return zipWatcherCreator ?? (zipWatcherCreator = new ZipSystemWatcherCreator(graphics, dispatcher));
            throw new ArgumentException("Wrong directory");
        }
    }
}
