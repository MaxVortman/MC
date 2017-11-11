using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MC.Source.Statistics
{
    abstract class Statistics
    {
        protected Int64 countOfLines;
        protected Int64 countOfWords;
        protected Dictionary<string, Int64> allUniqueWordsByTheirCountingInText;
        protected readonly string path;

        public Statistics(string path)
        {
            this.path = path;
            countOfLines = 0;
            countOfWords = 0;
            allUniqueWordsByTheirCountingInText = new Dictionary<string, long>();
        }
        public abstract Task<string> GetStatisticsAsync();  
        
    }
}
