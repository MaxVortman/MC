using System.Threading.Tasks;
using MC.Abstract_and_Parent_Classes;
using System.Collections.Generic;
using System;

namespace MC.Classes.Threading.ParallelClasses
{
    class SearchByPatternParallel : ISearcher
    {
        public ThreadProcess Search(Queue<string>[] filesQueue, ActionWithThread searchAndSaveIn)
        {
            return (process) =>
            {
                Task.Factory.StartNew(() =>
                {
                    ParallelLoopResult result = Parallel.ForEach(filesQueue, currentQueue =>
                    {
                        for (int i = 0; i < currentQueue.Count; i++)
                        {
                            searchAndSaveIn(currentQueue.Dequeue());
                        }
                    });
                    if (result.IsCompleted)
                    {
                        process();
                    }
                });
            };
        }
    }
}
