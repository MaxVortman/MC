using MC.Classes;
using MC.Source.Entries;

namespace MC.Source.Visitors
{
    public interface IVisitor
    {
        void Archive(File file);
        void Archive(Directory directory);
        void Search(File file);
        void Search(Directory directory);
    }
}
