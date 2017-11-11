using MC.Classes;
using MC.Source.Archivers;
using MC.Source.Entries;
using MC.Source.Searchers;

namespace MC.Source.Visitors
{
    class ThreadVisitor : IVisitor
    {
        public void Archive(File file)
        {
            var archiver = new FileArchiverInThread(file.Path);
            archiver.DoThread();
        }

        public void Archive(Directory directory)
        {
            var archiver = new FileArchiverInThread(directory.Path);
            archiver.DoThread();
        }

        public void Search(File file)
        {
            var searcher = new SearchByPatternInThread();
            var searcherBase = new SearchByPattern(file.Path, searcher);
        }

        public void Search(Directory directory)
        {
            var searcher = new SearchByPatternInThread();
            var searcherBase = new SearchByPattern(directory.Path, searcher);
        }
    }
}
