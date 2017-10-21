using System.Collections.Generic;
using System.Threading.Tasks;

namespace MC.Classes
{
    static class MyExtentions
    {
        public static bool IsComplite(this Task[] tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                var awaiter = tasks[i].GetAwaiter();
                awaiter.GetResult();
            }
            return true;
        }

        public static long Count(this Queue<string>[] filesQueue)
        {
            long count = 0;
            for (int i = 0; i < filesQueue.Length; i++)
            {
                count += filesQueue[i].Count;
            }

            return count;
        }
    }
}
