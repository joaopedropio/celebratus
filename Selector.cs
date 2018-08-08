using System.Collections.Generic;
using System.Linq;

namespace Celebratus
{
    public static class Selector
    {
        public static List<Dictionary<string, int>> RankFrequentWordsByArticle(List<string> texts, int frequentWordsNumber)
        {
            var result = new List<Dictionary<string, int>>();
            foreach (var text in texts)
            {
                result.Add(RankFrequentWords(text, 5));
            }
            return result;
        }

        private static Dictionary<string, int> RankFrequentWords(string text, int frequentWordsNumber)
        {
            var words = GetWords(text).Select(w => w.ToLower()).ToList();
            var wordsAndFrequency = words.GroupBy(w => w).ToDictionary(w => w.Key, w => w.Count());
            var wordsAndFrequencySorted = wordsAndFrequency.OrderByDescending(w => w.Value);
            var topWordsByFrequency = wordsAndFrequencySorted.Take(frequentWordsNumber);

            return topWordsByFrequency.ToDictionary(w => w.Key, w => w.Value);
        }

        private static List<string> GetWords(string text)
        {
            var words = text.Split(' ').ToList();
            words = RemoveEmptyWords(words);
            return words;
        }

        private static List<string> RemoveEmptyWords(List<string> words)
        {
            words.RemoveAll(w => w.Equals(""));
            return words;
        }
    }
}
