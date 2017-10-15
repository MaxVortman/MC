using MC.Abstract_and_Parent_Classes;

namespace MC.Classes.Threading.TaskClasses
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
                var tasks = new System.Threading.Tasks.Task[filesQueue.Length];
                for (int i = 0; i < filesQueue.Length; i++)
                {
                    var queue = filesQueue[i];
                    tasks[i] = System.Threading.Tasks.Task.Run(() =>
                    {
                        for (int j = 0; j < queue.Count; j++)
                        {
                            SearchAndSaveIn(queue.Dequeue());
                        }
                    });
                }
                System.Threading.Tasks.Task.Run(() =>
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
