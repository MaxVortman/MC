using System.Collections.Generic;

namespace MC.Source.Searchers
{
    class SearchByPatternInTask : ISearcher
    {

        public ThreadProcess Search(Queue<string>[] filesQueue, ActionWithThread searchAndSaveIn)
        {
            return (process =>
            {                
                var tasks = new System.Threading.Tasks.Task[filesQueue.Length];
                for (int i = 0; i < filesQueue.Length; i++)
                {
                    var queue = filesQueue[i];
                    tasks[i] = System.Threading.Tasks.Task.Run(() =>
                    {
                        for (int j = 0; j < queue.Count; j++)
                        {
                            searchAndSaveIn(queue.Dequeue());
                        }
                    });
                }
                System.Threading.Tasks.Task.Run(() =>
                {
                    if (tasks.Wait())
                    {
                        process();
                    }
                });
            });
        }
    }
}
