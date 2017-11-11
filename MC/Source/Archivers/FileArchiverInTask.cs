using System;

namespace MC.Source.Archivers
{
    class FileArchiverInTask : FileArchiver
    {
        public FileArchiverInTask(string sourcePathOfFile) : base(sourcePathOfFile)
        {
        }

        public override void DoThread()
        {
            var tasks = new System.Threading.Tasks.Task[filesQueue.Length];
            for (int i = 0; i < filesQueue.Length; i++)
            {
                var queue = filesQueue[i];
                tasks[i] = System.Threading.Tasks.Task.Run(() =>
                {
                    for (int j = 0; j < queue.Count; j++)
                    {
                        ArchiveFileInEntry(queue.Dequeue());
                    }
                });
            }
            System.Threading.Tasks.Task.Run(() =>
            {
                if (tasks.Wait())
                {
                    archive.Dispose();
                    GC.Collect(2);
                    GC.WaitForPendingFinalizers();
                }
            });
        }
    }
}
