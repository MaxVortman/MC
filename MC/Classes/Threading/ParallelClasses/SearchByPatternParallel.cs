using System.Threading.Tasks;
using MC.Abstract_and_Parent_Classes;

namespace MC.Classes.Threading.ParallelClasses
{
    class SearchByPatternParallel : SearchByPattern
    {
        public SearchByPatternParallel(string path) : base(path)
        {
        }

        public override void Search()
        {
            SearchInThread((process) =>
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    ParallelLoopResult result = System.Threading.Tasks.Parallel.ForEach(filesQueue, currentQueue =>
                    {
                        for (int i = 0; i < currentQueue.Count; i++)
                        {
                            SearchAndSaveIn(currentQueue.Dequeue());
                        }
                    });
                    if (result.IsCompleted)
                    {
                        process();
                    }
                });
            });
        }
    }
}
