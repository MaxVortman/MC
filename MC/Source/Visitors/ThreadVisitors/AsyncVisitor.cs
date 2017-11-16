using MC.Source.Archivers;
using MC.Source.Entries;
using MC.Source.Searchers;

namespace MC.Source.Visitors.ThreadVisitors
{
    class AsyncVisitor : IThreadsVisitor
    {
        public void Archive(File file)
        {
            var archiver = new FileArchiverAsync(file.FullPath);
            archiver.DoThread();
        }

        public void Archive(Directory directory)
        {
            var archiver = new FileArchiverAsync(directory.FullPath);
            archiver.DoThread();
        }

        public void Search(File file)
        {
            var searcherBase = new SearchByPattern(file.FullPath, new SearchByPatternAsync());
            searcherBase.DoThread();
        }

        public void Search(Directory directory)
        {
            var searcherBase = new SearchByPattern(directory.FullPath, new SearchByPatternAsync());
            searcherBase.DoThread();
        }
    }
}
