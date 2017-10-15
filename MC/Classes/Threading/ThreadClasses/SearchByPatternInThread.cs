using System.Threading;
using MC.Abstract_and_Parent_Classes;

namespace MC.Classes.Threading.ThreadClasses
{
    class SearchByPatternInThread : SearchByPattern
    {
        public SearchByPatternInThread(string path) : base(path)
        {
        }

        public override void Search()
        {            
            SearchInThread((process) =>
            {
                var threads = new ThreadQueue[filesQueue.Length];
                for (int i = 0; i < filesQueue.Length; i++)
                {
                    threads[i] = new ThreadQueue(filesQueue[i], SearchAndSaveIn);
                    threads[i].BeginProcessData();
                }

                var waitingThread = new System.Threading.Thread(() =>
                {
                    for (; ; System.Threading.Thread.Sleep(3000))
                    {
                        var count = 0;
                        for (var i = 0; i < threads.Length; i++)
                        {
                            if (threads[i].TheThread.ThreadState == System.Threading.ThreadState.Stopped)
                            {
                                count++;
                            }
                        }
                        if (count == threads.Length)
                        {
                            break;
                        }
                    }

                    var processThread = new System.Threading.Thread(new ThreadStart(process));
                    processThread.Start();
                });
                waitingThread.Start();
            });
        }
    }
}
