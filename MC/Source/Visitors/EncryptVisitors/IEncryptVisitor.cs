using MC.Source.Entries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source.Visitors.EncryptVisitors
{
    public interface IEncryptVisitor
    {
        void Encode(File file);
        void Encode(Directory directory);
        void Decode(File file);
        void Decode(Directory directory);
    }
}
