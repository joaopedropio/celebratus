using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Celebratus
{
    public class Filter
    {
        public static List<string> GetAllWordsByArticle(List<string> articles)
        {
            var words = new List<string>();
            foreach (var article in articles)
            {
                words.AddRange(GetWords(article));
            }
            return words.Select(w => w.ToLower()).Distinct().ToList();
        }
        public static List<string> GetWords(string text)
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
