using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MC.Classes.Threading.ParallelClasses
{
    class ParallelStatistics : MC.Abstract_and_Parent_Classes.Statistics
    {

        public ParallelStatistics(string path) : base(path) { }

        public async override Task<string> GetStatisticsAsync()
        {
           return await System.Threading.Tasks.Task.Run(async () => {
                var time = new Stopwatch();
                time.Start();
                using (var txtFile = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(txtFile))
                    {
                        var text = await reader.ReadToEndAsync();
                        CountingStatistics(text);
                    }
                }
               allUniqueWordsByTheirCountingInText = (from kv in allUniqueWordsByTheirCountingInText
                                                      orderby kv.Value descending
                                                      select kv).Take(10).ToDictionary((a) => a.Key, (a) => a.Value);
                time.Stop();
                return WriteReplay(time.Elapsed);
            });            
        }

        private string WriteReplay(TimeSpan time)
        {
            var replayStringBuilder = new StringBuilder($"Count of words: {countOfWords}\nCount of lines: {countOfLines}\nTOP TEN:\n");
            foreach (var item in allUniqueWordsByTheirCountingInText)
            {
                replayStringBuilder.AppendLine($"Word \"{item.Key}\" has met a number of time \"{item.Value}\"");
            }
            replayStringBuilder.AppendLine($"The running time of the algorithm parallel {time}\n");
            return replayStringBuilder.ToString();
        }

        private void CountingStatistics(string text)
        {
            CountingLinesInText(text);
            CountingAndCreateTopOfWords(text);
        }

        private void CountingLinesInText(string text)
        {
            countOfLines = text.Count((c) => { return c == '\n'; });
        }

        private void CountingAndCreateTopOfWords(string text)
        {
            string[] words = SeparateAndCountingWords(text);
            CreateTopTenMostPopular(words);
        }

        private string[] SeparateAndCountingWords(string text)
        {
            var matchesOfWords = Regex.Matches(text, $@"\b\w+\b");
            countOfWords = matchesOfWords.Count;
            var words = new string[countOfWords];
            var i = 0;
            foreach (Match match in matchesOfWords)
            {
                words[i] = match.Value;
                i++;
            }
            return words;
        }

        private void CreateTopTenMostPopular(string[] words)
        {
            var uniqueWords = words.Distinct().ToArray();
            allUniqueWordsByTheirCountingInText = (from w in uniqueWords.AsParallel()
                                                   let keyValuePair = new { Key = w, Value = (long)words.Count((word) => word == w) }                                                   
                                                   select keyValuePair).ToDictionary((a) => a.Key, (a) => a.Value);
        }               
    }
}
