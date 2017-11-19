using System.Collections.Generic;
using System.Threading.Tasks;

namespace MC.Source.Searchers
{
    class SearchByPatternParallel : ISearcher
    {
        public ThreadProcess Search(Queue<ISearchble>[] filesQueue, ActionWithThread<ISearchble> searchAndSaveIn)
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
