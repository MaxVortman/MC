using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MC.Source.Statistics
{
    abstract class Statistics
    {
        protected Int64 countOfLines;
        protected Int64 countOfWordsInText;
        protected Dictionary<string, long> allUniqueWordsByTheirCountingInText;
        protected readonly string path;
        protected Regex regex = new Regex($@"\b\w+\b", RegexOptions.Compiled);

        public Statistics(string path)
        {
            this.path = path;
            countOfLines = 0;
            countOfWordsInText = 0;
            allUniqueWordsByTheirCountingInText = new Dictionary<string, long>();
        }
        public abstract Task<string> GetStatisticsAsync();  
        
    }
}
