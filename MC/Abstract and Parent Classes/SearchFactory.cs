using MC.Classes.Threading.AsyncClasses;
using MC.Classes.Threading.ParallelClasses;
using MC.Classes.Threading.TaskClasses;
using MC.Classes.Threading.ThreadClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Abstract_and_Parent_Classes
{
    public class SearchFactory : IThreadFactory
    {
        public IThreder CreateObject(string type, string filePath)
        {
            switch (type)
            {
                case "Thread":
                    return new SearchByPattern(filePath, new SearchByPatternInThread());
                case "Parralel":
                    return new SearchByPattern(filePath, new SearchByPatternParallel());
                case "Tasks":
                    return new SearchByPattern(filePath, new SearchByPatternInTask());
                case "Async":
                    return new SearchByPattern(filePath, new SearchByPatternAsync());
                default:
                    throw new ArgumentException();
            }
        }
    }
}
