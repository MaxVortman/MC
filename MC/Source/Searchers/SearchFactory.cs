using System;
using MC.Source.Threading;

namespace MC.Source.Searchers
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
