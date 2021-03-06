﻿using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MC.Source.Statistics
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
                var lines = File.ReadAllLines(path);
                countOfLines = lines.LongLength;
                CountingStatistics(lines);     
                var topTen = (from d in allUniqueWordsByTheirCountingInText
                              orderby d.Value descending
                              select d).Take(10);
                time.Stop();
                var result = new StringBuilder($"Count of words: {countOfWordsInText}\nCount of lines: {countOfLines}\nTOP TEN:\n");
                foreach (var item in topTen)
                {
                    result.AppendLine($"Word \"{item.Key}\" has met a number of time \"{item.Value}\"");
                }
                result.AppendLine($"The running time of the algorithm non parallel {time.Elapsed}\n");
                return result.ToString();
            });            
        }

        private void CountingStatistics(string[] lines)
        {
            string[] words;
            foreach (var line in lines)
            {
                words = SearchWordsIn(line);
                countOfWordsInText += words.LongLength;
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
        }

        private string[] SearchWordsIn(string line)
        {
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