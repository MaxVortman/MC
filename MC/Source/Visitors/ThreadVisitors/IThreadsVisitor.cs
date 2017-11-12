using MC.Source.Entries;

namespace MC.Source.Visitors.ThreadVisitors
{
    public interface IThreadsVisitor
    {
        void Archive(File file);
        void Archive(Directory directory);
        void Search(File file);
        void Search(Directory directory);
    }
}
