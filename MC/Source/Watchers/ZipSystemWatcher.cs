using MC.Source;
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
    internal class ZipSystemWatcher
    {
        const int TIME_TO_ELAPSE = 500;
        private Timer timer;

        #region Event and his args

        public event EventHandler<SnapEventArgs> OnChanged;

        public class SnapEventArgs : EventArgs
        {
            public SnapEventArgs(string path)
            {
                Path = path;
            }

            public string Path { get; }
        }

        #endregion

        #region Constructor

        public ZipSystemWatcher(string path, Zip zip)
        {
            Path = path;
            Zip = zip;
            timer = new Timer(TIME_TO_ELAPSE);
            timer.Elapsed += async (sender, e) => await TimerElapsedAsync();
        }

        #endregion

        #region Properties
        private string path;
        public string Path {
            get
            {
                return path;
            }
            set
            {
                Stop();
                path = value;
                Start();
            }
        }

        public Zip Zip { get; }
        private SnapShot lastSnapShot;

        #endregion

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            timer.Start();
        }

        private Task TimerElapsedAsync()
        {
            return Task.Run(() =>
            {
                var snapShot = new SnapShot(Zip, Path);
                if (lastSnapShot != null && lastSnapShot.IsChanged(snapShot))
                    OnChanged.Invoke(this, new SnapEventArgs(Path));
            });            
        }
    }
}
