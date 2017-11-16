using MC.Source.Archivers;
using MC.Source.Entries;
using MC.Source.Searchers;

namespace MC.Source.Visitors.ThreadVisitors
{
    class ParallelVisitor : IThreadsVisitor
    {
        public void Archive(File file)
        {
            var archiver = new FileArchiverParallel(file.FullPath);
            archiver.DoThread();
        }

        public void Archive(Directory directory)
        {
            var archiver = new FileArchiverParallel(directory.FullPath);
            archiver.DoThread();
        }

        public void Search(File file)
        {
            var searcher = new SearchByPatternParallel();
            var searcherBase = new SearchByPattern(file.FullPath, searcher);
        }

        public void Search(Directory directory)
        {
            var searcher = new SearchByPatternParallel();
            var searcherBase = new SearchByPattern(directory.FullPath, searcher);
        }
    }
}
