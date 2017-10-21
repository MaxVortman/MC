using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Abstract_and_Parent_Classes
{
    public interface IThreadFactory
    {
        IThreder CreateObject(string type, string filePath);
    }
}
