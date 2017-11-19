using MC.Source.Entries;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MC.Source
{
    static class MyExtentions
    {
        public static bool Wait(this Task[] tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                var awaiter = tasks[i].GetAwaiter();
                awaiter.GetResult();
            }
            return true;
        }

        public static long Count<T>(this Queue<T>[] filesQueue)
        {
            long count = 0;
            for (int i = 0; i < filesQueue.Length; i++)
            {
                count += filesQueue[i].Count;
            }

            return count;
        }

        public static bool IsArchive(this Entity entity)
        {
            if (Path.GetExtension(entity.FullPath).Equals(".zip"))
            {
                return true;
            }
            return false;
        }

        public static void Add(this Dictionary<string, long> toDictionary, Dictionary<string, long> fromDictionary)
        {
            Parallel.ForEach(fromDictionary.Keys, (k) =>
            {
                if (toDictionary.ContainsKey(k))
                    toDictionary[k] += fromDictionary[k];
                else
                    toDictionary.Add(k, fromDictionary[k]);
            });
        }

        public static bool Contains(this IEnumerable<Entity> entities, Func<Entity, bool> func)
        {
            foreach (var entity in entities)
            {
                if (func(entity))
                    return true;
            }
            return false;
        }
    }
}
