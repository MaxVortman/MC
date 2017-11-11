using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MC.Source.Archivers
{
    class FileArchiverAsync : FileArchiver
    {
        public FileArchiverAsync(string sourcePathOfFile) : base(sourcePathOfFile)
        {
        }

        private static object _archiveLock = new object();
        private static CancellationTokenSource archiveCTS;
        private static int tempCount;

        public override void DoThread()
        {
            archiveCTS = new CancellationTokenSource();
            var archiveCT = archiveCTS.Token;
            var ProgressLayout = new Windows.ProgressWindow("Archive") { CTS = archiveCTS };
            var progress = new Progress<double>(ProgressLayout.ReportProgress);
            var realProgres = progress as IProgress<double>;
            ProgressLayout.Show();
            Task.Run(async () =>
            {
                var totalCount = filesQueue.Count();
                tempCount = 0;
                var tasks = new Task[filesQueue.Length];
                await Task.Run(() =>
                {
                    for (int i = 0; i < filesQueue.Length; i++)
                    {
                        var queue = filesQueue[i];
                        var queueLength = queue.Count;
                        tasks[i] = Task.Run(() =>
                        {
                            try
                            {
                                for (int j = 0; j < queueLength; j++)
                                {
                                    archiveCT.ThrowIfCancellationRequested();
                                    ArchiveFileInEntry(queue.Dequeue());
                                    Interlocked.Increment(ref tempCount);
                                    lock (_archiveLock)
                                    {                                        
                                        realProgres.Report(tempCount * 100 / (double)totalCount);
                                    }
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                MessageBox.Show("Archive canceled.");
                                GC.Collect(2);
                                GC.WaitForPendingFinalizers();
                            }
                        }, archiveCT);
                    }
                    tasks.Wait();
                });
                ProgressLayout.Close();
                Dispose();
                GC.Collect(2);
                GC.WaitForPendingFinalizers();
            });
        }
    }

}
