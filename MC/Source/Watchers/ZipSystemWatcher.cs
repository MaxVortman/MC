using MC.Source;
using MC.Source.Entries;
using MC.Source.Entries.Zipped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using System.Threading.Tasks;
using System.Timers;

namespace MC.Source.Watchers
{
    public class SnapEventArgs : EventArgs
    {
        public SnapEventArgs(ZippedFolder directory)
        {
            Directory = directory;
        }
        
        public ZippedFolder Directory { get; }
    }

    public class ZipSystemWatcher
    {
        const int TIME_TO_ELAPSE = 500;
        private Timer timer;

        #region Event

        public event EventHandler<SnapEventArgs> OnChanged;
        
        #endregion

        #region Constructor

        public ZipSystemWatcher()
        {
            timer = new Timer(TIME_TO_ELAPSE);
            timer.Elapsed += async (sender, e) => await TimerElapsedAsync();
        }

        #endregion

        #region Properties
        public ZippedFolder Directory { get; set; }

        private SnapShot lastSnapShot;

        #endregion

        public void Stop()
        {
            timer.Stop();
            lastSnapShot = null;
            Directory = null;
        }

        public void Start()
        {
            if (Directory != null)
                timer.Start();
            else
                throw new ArgumentException("Don't initialize zip and path yet");
        }

        private Task TimerElapsedAsync()
        {
            return Task.Run(() =>
            {
                Directory.Zip.UpdateEntries();
                var snapShot = new SnapShot(Directory.Zip, Directory.FolderPath);
                snapShot.MakeSnapShot();
                if (lastSnapShot != null && lastSnapShot.IsChanged(snapShot))
                    OnChanged.Invoke(this, new SnapEventArgs(Directory));
                lastSnapShot = snapShot;
            });            
        }
    }
}
