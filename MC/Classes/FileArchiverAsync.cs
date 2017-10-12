using MC.Abstract_and_Parent_Classes;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MC.Classes
{
    class FileArchiverAsync : FileArchiver
    {
        public FileArchiverAsync(string sourcePathOfFile) : base(sourcePathOfFile)
        {
        }

        private static object _archiveLock = new object();
        private static CancellationTokenSource archiveCTS;
        private static int tempCount;

        public override void Archive()
        {
            archiveCTS = new CancellationTokenSource();
            var archiveCT = archiveCTS.Token;
            var ProgressLayout = new Windows.ProgressWindow("Archive") { CTS = archiveCTS };
            var progress = new Progress<double>(ProgressLayout.ReportProgress);
            var realProgres = progress as IProgress<double>;
            ProgressLayout.Show();
            Task.Run(async () =>
            {
                var totalCount = fileQueueCreator.GetCountOfFilesInListOfPath();
                tempCount = 0;
                var tasks = new Task[filesQueue.Length];
                await Task.Run(() =>
                {
                    for (int i = 0; i < filesQueue.Length; i++)
                    {
                        var queue = filesQueue[i];
                        tasks[i] = Task.Run(() =>
                        {
                            try
                            {
                                for (int j = 0; j < queue.Count; j++)
                                {
                                    archiveCT.ThrowIfCancellationRequested();
                                    ArchiveFileInEntry(queue.Dequeue());
                                    lock (_archiveLock)
                                        realProgres.Report(++tempCount * 100 / (double)totalCount);
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
                    tasks.IsComplite();
                });
                ProgressLayout.Close();
                archive.Dispose();
                GC.Collect(2);
                GC.WaitForPendingFinalizers();
            });
        }

        internal override void Closing()
        {
            //TO DO: 
        }
    }

}
