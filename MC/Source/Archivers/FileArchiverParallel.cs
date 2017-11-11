using System;

namespace MC.Source.Archivers
{
    class FileArchiverParallel : FileArchiver
    {
        public FileArchiverParallel(string sourcePathOfFile) : base(sourcePathOfFile)
        {
        }

        public override void DoThread()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var result = System.Threading.Tasks.Parallel.ForEach(filesQueue, currentQueue =>
                {
                    for (int i = 0; i < currentQueue.Count; i++)
                    {
                        ArchiveFileInEntry(currentQueue.Dequeue());
                    }
                });
                if (result.IsCompleted)
                {
                    archive.Dispose();
                    GC.Collect(2);
                    GC.WaitForPendingFinalizers();
                }
            });
        }
    }
}
