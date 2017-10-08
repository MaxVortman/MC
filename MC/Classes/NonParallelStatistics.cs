using System;
using MC.Abstract_and_Parent_Classes;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MC.Classes
{
    internal class NonParallelStatistics : Statistics
    {

        public NonParallelStatistics(string path) : base(path){}

        public override async Task<string> GetStatisticsAsync()
        {
            
           return await Task.Run(() =>
            {
                var time = new Stopwatch();
                time.Start();
                string line;
                using (var txtFile = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(txtFile))
                    {
                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            countOfLines++;
                            CountingStatistics(line);
                        }
                    }
                }
                var topTen = (from d in allUniqueWordsByTheirCountingInText
                              orderby d.Value descending
                              select d).Take(10);
                time.Stop();
                var result = new StringBuilder($"Count of words: {countOfWords}\nCount of lines: {countOfLines}\nTOP TEN:\n");
                foreach (var item in topTen)
                {
                    result.AppendLine($"Word \"{item.Key}\" has met a number of time \"{item.Value}\"");
                }
                result.AppendLine($"The running time of the algorithm non parallel {time.Elapsed}\n");
                return result.ToString();
            });            
        }

        private void CountingStatistics(string line)
        {
            var words = SearchWordsIn(line);
            countOfWords += words.LongLength;
            foreach (var word in words)
            {
                if (allUniqueWordsByTheirCountingInText.Keys.Contains(word))
                {
                    allUniqueWordsByTheirCountingInText[word]++;
                }
                else
                {
                    allUniqueWordsByTheirCountingInText.Add(word, 1);
                }
            }
        }

        private string[] SearchWordsIn(string line)
        {
            var regex = new Regex($@"\b\w+\b");
            var matchesOfWords = regex.Matches(line);
            var arrayOfWords = new string[matchesOfWords.Count];
            int i = 0;
            foreach (Match match in matchesOfWords)
            {
                arrayOfWords[i] = match.Value;
                i++;
            }

            return arrayOfWords;
        }
    }
}