using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source.Searchers
{
    public interface ISearchble
    {
        string ReadStreamToEnd();

        string FullPath { get; }
    }
}
