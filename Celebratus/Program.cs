using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Celebratus
{
    class Program
    {
        static void Main(string[] args)
        {
            // Default Values
            var url = "http://feeds.arstechnica.com/arstechnica/technology-lab";
            var numberOfTopWords = 5;
            var contentTag = "content:encoded";

            if(args != null && args.Length == 3)
            {
                url = args[0];
                numberOfTopWords = int.Parse(args[1]);
                contentTag = args[2];
            }

            ShowTopWordsAndTheirFrequenciesByArticleFromWebPage(url, contentTag, numberOfTopWords);

            Console.ReadLine();
        }

        static void ShowTopWordsAndTheirFrequenciesByArticleFromWebPage(string webPageUrl, string tagContainingContent, int numberOfTopWordsByArticle)
        {
            var rssFeedPageText = Requester.GetPage(webPageUrl);

            var tagContents = Parser.GetTagContents(rssFeedPageText, tagContainingContent);

            var articles = Cleaner.CleanArticles(tagContents);

            var frequentWordsByArticle = Selector.RankFrequentWordsByArticle(articles, numberOfTopWordsByArticle);

            Printer.ShowTopWordsByArticle(frequentWordsByArticle);
        }
    }
}
