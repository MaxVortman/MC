using MC.Source.Entries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source.Watchers
{
    public interface IWatcherCreator
    {
        void StartWatch(Directory directory);
    }
}
