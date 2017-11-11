using System.Collections.Generic;

namespace MC.Source.Searchers
{
    public interface ISearcher
    {
        ThreadProcess Search(Queue<string>[] filesQueue, ActionWithThread searchAndSaveIn);
    }
}
