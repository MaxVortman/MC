using MC.Abstract_and_Parent_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Classes
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
                Task.Factory.StartNew(() =>
                {
                    ParallelLoopResult result = Parallel.ForEach(filesQueue, currentQueue =>
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
