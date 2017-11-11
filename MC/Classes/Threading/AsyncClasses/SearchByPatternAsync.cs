using System;
using System.Threading;
using System.Threading.Tasks;
using MC.Abstract_and_Parent_Classes;
using System.Collections.Generic;

namespace MC.Classes.Threading.AsyncClasses
{
    class SearchByPatternAsync : ISearcher
    {
        
        private static object _searchLock = new object();
        private static CancellationTokenSource seachCTS;

        public ThreadProcess Search(Queue<string>[] filesQueue, ActionWithThread searchAndSaveIn)
        {
            return (async (process) =>
            {
                seachCTS = new CancellationTokenSource();
                var seachTC = seachCTS.Token;
                var totalCount = filesQueue.Count();
                var ProgressLayout = new Windows.ProgressWindow("Search") { CTS = seachCTS };
                var progress = new Progress<double>(ProgressLayout.ReportProgress);
                var realProgres = progress as IProgress<double>;
                ProgressLayout.Show();
                await System.Threading.Tasks.Task.Run(() =>
                {

                    var tasks = new Task[filesQueue.Length];
                    var tempCount = 0;
                    for (int i = 0; i < filesQueue.Length; i++)
                    {
                        var queue = filesQueue[i];
                        tasks[i] = Task.Run(() =>
                        {
                            try
                            {
                                for (int j = 0; j < queue.Count; j++)
                                {
                                    seachTC.ThrowIfCancellationRequested();
                                    searchAndSaveIn(queue.Dequeue());
                                    //Interlocked.Increment(ref tempCount);
                                    lock (_searchLock)
                                        realProgres.Report(++tempCount * 100 / (double)totalCount);
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                GC.Collect(2);
                                GC.WaitForPendingFinalizers();
                            }
                        }, seachTC);
                    }
                    tasks.Wait();
                });
                ProgressLayout.Close();
                process();
            });
        }
    }
}
