﻿using System.Collections.Generic;
using System.Threading;
using MC.Source.Threading;

namespace MC.Source.Searchers
{
    class SearchByPatternInThread : ISearcher
    {

        public ThreadProcess Search(Queue<ISearchble>[] filesQueue, ActionWithThread<ISearchble> searchAndSaveIn)
        {            
            return ((process) =>
            {
                var threads = new ThreadQueue<ISearchble>[filesQueue.Length];
                for (int i = 0; i < filesQueue.Length; i++)
                {
                    threads[i] = new ThreadQueue<ISearchble>(filesQueue[i], searchAndSaveIn);
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
