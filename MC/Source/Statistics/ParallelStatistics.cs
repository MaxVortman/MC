using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MC.Source.Statistics
{
    class ParallelStatistics : Statistics
    {

        public ParallelStatistics(string path) : base(path) { }

        public async override Task<string> GetStatisticsAsync()
        {
            return await Task.Run(() =>
            {
                var time = new Stopwatch();
                time.Start();
                var lines = File.ReadAllLines(path);
                countOfLines = lines.LongLength;
                CountingStatistics(lines);

                allUniqueWordsByTheirCountingInText = (from kv in allUniqueWordsByTheirCountingInText.AsParallel()
                                                       orderby kv.Value descending
                                                       select kv).Take(10).ToDictionary((a) => a.Key, (a) => a.Value);
                time.Stop();
                return WriteReplay(time.Elapsed);
            });
        }

        private string WriteReplay(TimeSpan time)
        {
            var replayStringBuilder = new StringBuilder($"Count of words: {countOfWordsInText}\nCount of lines: {countOfLines}\nTOP TEN:\n");
            foreach (var item in allUniqueWordsByTheirCountingInText)
            {
                replayStringBuilder.AppendLine($"Word \"{item.Key}\" has met a number of time \"{item.Value}\"");
            }
            replayStringBuilder.AppendLine($"The running time of the algorithm parallel {time}\n");
            return replayStringBuilder.ToString();
        }

        private void CountingStatistics(string[] lines)
        {
            string[] words;
            foreach (var line in lines)
            {
                words = SeparateAndCountingWords(line);
                CreateTopTenMostPopular(words);
            }            
        }

        private string[] SeparateAndCountingWords(string text)
        {
            var matchesOfWords = regex.Matches(text);
            var countOfWorldsInLine = matchesOfWords.Count;
            countOfWordsInText += countOfWorldsInLine;
            var words = new string[countOfWorldsInLine];
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
            allUniqueWordsByTheirCountingInText.Add((from w in words.Distinct().AsParallel()
                                                   let keyValuePair = new { Key = w, Value = (long)words.Count((c) => c == w) }                                                   
                                                   select keyValuePair).ToDictionary((a) => a.Key, (a) => a.Value));
        }               
    }
}
