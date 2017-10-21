using System.Threading;
using MC.Abstract_and_Parent_Classes;
using System;
using System.Collections.Generic;

namespace MC.Classes.Threading.ThreadClasses
{
    class SearchByPatternInThread : ISearcher
    {

        public ThreadProcess Search(Queue<string>[] filesQueue, ActionWithThread searchAndSaveIn)
        {            
            return ((process) =>
            {
                var threads = new ThreadQueue[filesQueue.Length];
                for (int i = 0; i < filesQueue.Length; i++)
                {
                    threads[i] = new ThreadQueue(filesQueue[i], searchAndSaveIn);
                    threads[i].BeginProcessData();
                }

                var waitingThread = new Thread(() =>
                {
                    for (; ; Thread.Sleep(3000))
                    {
                        var count = 0;
                        for (var i = 0; i < threads.Length; i++)
                        {
                            if (threads[i].TheThread.ThreadState == ThreadState.Stopped)
                            {
                                count++;
                            }
                        }
                        if (count == threads.Length)
                        {
                            break;
                        }
                    }

                    var processThread = new Thread(new ThreadStart(process));
                    processThread.Start();
                });
                waitingThread.Start();
            });
        }
    }
}
