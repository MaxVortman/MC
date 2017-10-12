using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC
{
    public delegate void ActionWithThread(string filePath);
    public delegate void PackFiles(string sPath);
    public delegate void ThreadProcess(InThreadProcess process);
    public delegate void InThreadProcess();
}
