using MC.Abstract_and_Parent_Classes;
using MC.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Classes.Visitors
{
    public interface IVisitor
    {
        void Archive(File file);
        void Archive(Directory directory);
        void Search(File file);
        void Search(Directory directory);
    }
}
