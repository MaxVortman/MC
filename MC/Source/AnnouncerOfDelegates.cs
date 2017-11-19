using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source
{
    public delegate void ActionWithThread<T>(T file) where T : class;
    public delegate void PackFiles(string sPath);
    public delegate void ThreadProcess(InThreadProcess process);
    public delegate void InThreadProcess();
}
