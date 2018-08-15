using System;
using System.Collections.Generic;
using System.Linq;

namespace Celebratus
{
    public static class Printer
    {
        public static void ShowTopWordsByArticle(List<Dictionary<string, int>> frequentWordsByArticle)
        {
            Console.WriteLine("######### Most Frequent Words By Article #########\n");
            for (int i = 0; i < frequentWordsByArticle.Count(); i++)
            {
                Console.WriteLine($"  Article #{i}");
                PrintTopWords(frequentWordsByArticle[i]);
                Console.Write("\n");
            }
        }

        public static void PrintTopWords(Dictionary<string, int> topWords)
        {
            foreach (var topWord in topWords)
            {
                Console.WriteLine($"    Word: {topWord.Key} - Frequency: {topWord.Value}");
            }
        }
    }
}
