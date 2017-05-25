using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MC.Classes
{
    internal static class Search
    {
        internal static List<string> Files(string directory, string mask)
        {
            _allPath = new List<string>();
            FillList(directory);
            mask = mask.ReplaceAll("*", ".", "?");
            var result = _allPath.FindAll((s) =>
            {
                if (s == null) return false;
                return Regex.IsMatch(s, @mask);
            });
            return result;
        }

        private static string ReplaceAll(this string text, params string[] replace)
        {
            var result = text;
            foreach (var item in replace)
            {
                if (text.Contains(item))
                    switch (item)
                    {
                        case "*":
                            result = result.Replace(item, @"\w*");
                            break;
                        case ".":
                            result = result.Replace(item, @"\.");
                            break;
                        case "?":
                            result = result.Replace(item, @"\w");
                            break;
                        default:
                            break;
                    }
            }
            return result;
        }

        private static List<string> _allPath;
        private static void FillList(string directory)
        {
            try
            {
                foreach (var item in Directory.GetFiles(directory))
                {
                    _allPath.Add(item);
                }
                foreach (var item in Directory.GetDirectories(directory))
                {
                    FillList(item);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}