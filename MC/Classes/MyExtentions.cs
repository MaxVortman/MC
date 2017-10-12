using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
