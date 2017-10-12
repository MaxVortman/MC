using MC.Abstract_and_Parent_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Classes
{
    class SearchByPatternInTask : SearchByPattern
    {
        public SearchByPatternInTask(string path) : base(path)
        {
        }

        public override void Search()
        {
            SearchInThread(process =>
            {                
                var tasks = new Task[filesQueue.Length];
                for (int i = 0; i < filesQueue.Length; i++)
                {
                    var queue = filesQueue[i];
                    tasks[i] = Task.Run(() =>
                    {
                        for (int j = 0; j < queue.Count; j++)
                        {
                            SearchAndSaveIn(queue.Dequeue());
                        }
                    });
                }
                Task.Run(() =>
                {
                    if (tasks.IsComplite())
                    {
                        process();
                    }
                });
            });
        }
    }
}
