using System;
using MC.Source.Threading;

namespace MC.Source.Archivers
{
    class FileArchiverInThread : FileArchiver
    {
        public FileArchiverInThread(string pathOfFile) : base(pathOfFile)
        {
        }

        public int CountOfCompliteThread = 0;
        public ThreadQueue[] Threads { get; private set; }

        public override void DoThread()
        {
            Threads = new ThreadQueue[filesQueue.Length];
            ThreadQueue.ThreadingComplite += CompressComplite;
            for (int i = 0; i < filesQueue.Length; i++)
            {
                Threads[i] = new ThreadQueue(filesQueue[i], new ActionWithThread(ArchiveFileInEntry));
                Threads[i].BeginProcessData();
            }
        }

        private static object lockToken = new object();
        private void CompressComplite(object sender, EventArgs e)
        {
            lock (lockToken)
            {
                if (++CountOfCompliteThread == Environment.ProcessorCount)
                {
                    archive.Dispose();
                    Threads = null;
                    CountOfCompliteThread = 0;
                    GC.Collect(2);
                    GC.WaitForPendingFinalizers();
                }
            }            
        }

  
    }
}
