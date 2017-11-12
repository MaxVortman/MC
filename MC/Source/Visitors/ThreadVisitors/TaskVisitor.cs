using MC.Source.Archivers;
using MC.Source.Entries;
using MC.Source.Searchers;

namespace MC.Source.Visitors.ThreadVisitors
{
    class TaskVisitor : IThreadsVisitor
    {
        public void Archive(File file)
        {
            var archiver = new FileArchiverInTask(file.Path);
            archiver.DoThread();
        }

        public void Archive(Directory directory)
        {
            var archiver = new FileArchiverInTask(directory.Path);
            archiver.DoThread();
        }

        public void Search(File file)
        {
            var searcherBase = new SearchByPattern(file.Path, new SearchByPatternInTask());
            searcherBase.DoThread();
        }

        public void Search(Directory directory)
        {
            var searcherBase = new SearchByPattern(directory.Path, new SearchByPatternInTask());
            searcherBase.DoThread();
        }
    }
}
