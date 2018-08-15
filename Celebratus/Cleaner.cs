using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Celebratus
{
    public static class Cleaner
    {
        public static List<string> CleanArticles(List<string> tagContents)
        {
            var cleanedTagContents = new List<string>();

            foreach (var tagContent in tagContents)
            {
                var cleanedContent = RemoveTagAttributes(tagContent);
                cleanedContent = RemoveTagsWithoutClosingTag(cleanedContent);
                cleanedContent = RemoveTags(cleanedContent);
                cleanedContent = RemovePunctuation(cleanedContent);
                cleanedContent = RemoveCDATA(cleanedContent);
                cleanedTagContents.Add(cleanedContent);
            }

            cleanedTagContents = cleanedTagContents.Select(ctc => ctc.ToLower()).ToList();

            cleanedTagContents = RemoveInvalidWords(cleanedTagContents);

            return cleanedTagContents;
        }

        private static string RemoveCDATA(string text)
        {
            return text.Replace("CDATA", "");
        }

        private static string RemovePunctuation(string text)
        {
            var punctuations = new char[] { '!', '@', '#', '(', ')', '.', ',', ';', ':', '/', '\\', '?', '"', '|', '[', ']', '+', '=', '-', '<', '>', '\n', '\r' };

            foreach (var punctuation in punctuations)
            {
                text = text.Replace(punctuation, ' ');
            }

            return text;
        }

        private static string RemoveTags(string text)
        {
            var tags = new string[] { "figure", "span", "p", "div", "a", "blockquote" };

            foreach (var tag in tags)
            {
                var openningTag = Parser.MakeOpenningTag(tag);
                var closingTag = Parser.MakeClosingTag(tag);
                text = RemoveTag(text, openningTag);
                text = RemoveTag(text, closingTag);
            }

            return text;
        }

        private static string RemoveTag(string text, string tag)
        {
            int index = text.IndexOf(tag);

            while (index != -1)
            {
                text = text.Remove(index, tag.Length);
                index = text.IndexOf(tag);
            }

            return text;
        }

        private static string RemoveTagsWithoutClosingTag(string text)
        {
            var tagsToRemoveWithoutClosingTag = new string[] { "img" };

            foreach (var tag in tagsToRemoveWithoutClosingTag)
            {
                var startTag = '<' + tag;

                var startOfTagIndex = text.IndexOf(startTag);

                var isTagFound = startOfTagIndex != -1;
                if (!isTagFound)
                    break;
                var endOfTagINdex = text.IndexOf('>', startOfTagIndex);

                var count = endOfTagINdex - startOfTagIndex + 1;

                text = text.Remove(startOfTagIndex, count);
            }

            return text;
        }

        private static string RemoveTagWithContent(string text, string tag)
        {
            var openningTag = Parser. MakeOpenningTag(tag);
            var closingTag = Parser.MakeClosingTag(tag);

            var openningTagIndex = text.IndexOf(openningTag);
            var closingTagIndex = text.IndexOf(closingTag) + closingTag.Length;

            return text.Remove(openningTagIndex, closingTagIndex);
        }

        private static string RemoveTagAttributes(string text)
        {
            if (text == null || text.Length == 0)
                return text;

            var indexCount = 0;

            while (text.Length > indexCount)
            {
                var openningTagIndex = NextCharacterAppereance(text, '<', indexCount);

                if (openningTagIndex == -1)
                    break;

                var closingTagIndex = NextCharacterAppereance(text, '>', openningTagIndex);

                if (closingTagIndex == -1)
                    break;

                var tagName = GetTagName(text, openningTagIndex);

                var endOfTagNameIndex = openningTagIndex + tagName.Length;
                var count = closingTagIndex - endOfTagNameIndex;

                text = text.Remove(endOfTagNameIndex, count);

                indexCount = endOfTagNameIndex;
            }

            return text;
        }
        private static int NextCharacterAppereance(string text, char character, int startIndex)
        {
            return text.IndexOf(character, startIndex);
        }

        private static string GetTagName(string text, int openningTagIndex)
        {
            int index;

            for (index = openningTagIndex; !isEndOfTagName(text, index); index++) ;

            var count = index - openningTagIndex;

            return text.Substring(openningTagIndex, count);
        }

        private static bool isEndOfTagName(string text, int index) => text[index] == '>' || text[index] == ' ';

        public static List<string> RemoveInvalidWords(List<string> articles)
        {
            var words = Filter.GetAllWordsByArticle(articles);

            var invalidWords = GetInvalidWords();

            var cleanArticles = RemoveInvalidWordsFromArticles(articles, invalidWords);

            return cleanArticles;
        }

        private static List<string> GetInvalidWords()
        {
            return new List<string>()
            {
                "an", "my", "the", "of", "from", "a", "up", "to", "in", "on", "at",
                "above", "about", "across", "against", "over", "all", "along", "i",
                "around", "among", "far", "as", "behind", "after", "before", "for",
                "by", "since", "out", "do", "did", "was", "were", "am", "pm", "which",
                "instead", "near", "with", "without", "since", "outside", "until", 
                "is", "us", "they", "you", "he", "she", "it", "yours", "own", "our",
                "that", "those", "them", "his", "her", "its", "are", "been", "being",
                "who", "whose", "whether", "wich", "their", "here", "than", "then",
                "so", "don't", "have", "aren't", "doesn't", "isn't", "and", "below",
                "should", "would", "could", "'ve", "or"
            };
        }

        public static List<string> RemoveInvalidWordsFromArticles(List<string> articles, List<string> invalidWords)
        {
            var result = new List<string>();
            foreach (var article in articles)
            {
                var cleanArticle = RemoveInvalidWords(article, invalidWords);
                result.Add(cleanArticle);
            }

            return result;
        }

        public static string RemoveInvalidWords(string text, List<string> invalidWords)
        {
            return RemoveWords(text, ' ', invalidWords);
        }

        public static string RemoveWords(string text, char separator, List<string> words)
        {
            foreach (var word in words)
            {
                text = RemoveWord(text, separator, word);
            }

            return text;
        }

        public static string RemoveWord(string text, char separator, string word)
        {
            for (int i = 0; i < text.Length; i++)
            {
                var nextWordStartIndex = text.IndexOf(word, i);

                if (nextWordStartIndex == -1)
                    break;

                if (isWord(text, separator, nextWordStartIndex, word))
                {
                    text = text.Remove(nextWordStartIndex, word.Length);
                }

                i = nextWordStartIndex;
            }

            return text;
        }

        public static bool isWord(string text, char separator, int startIndex, string word)
        {
            if (startIndex == 0)
                return text[word.Length + startIndex] == separator;
            else
                return text[startIndex - 1] == separator && text[word.Length + startIndex] == separator;
        }

        public static Dictionary<string, List<int>> GetWordsAndIndexes(string text)
        {
            var result = new Dictionary<string, List<int>>();

            var separator = ' ';

            for (int i = 0; i < text.Length; i++)
            {
                var nextWordIndex = NextWordIndex(i, separator, text);
                var nextWord = GetNextWord(i, separator, text);

                if(result.ContainsKey(nextWord))
                {
                    result[nextWord].Add(nextWordIndex);
                } else
                {
                    result.Add(nextWord, new List<int>() { nextWordIndex });
                }
            }

            return result;
        }

        public static int NextWordIndex(int startIndex, char separator, string text)
        {
            while (text[startIndex] == separator) startIndex++;
            return startIndex;
        }

        public static string GetNextWord(int startIndex, char separator, string text)
        {
            var endIndex = startIndex;
            while (text[endIndex] != separator) endIndex++;
            return text.Substring(startIndex, endIndex - startIndex);
        }

        [Obsolete]
        public static List<string> GetValidWords(List<string> words)
        {
            var wordsClient = new WordsApiClient("https://od-api.oxforddictionaries.com/api/v1/entries/en/{word}/lexicalCategory", "c8667c8e", "12bf53bb8f0035514f0bf2b488141c19");

            var validWords = new List<string>();

            foreach (var word in words)
            {
                var lexicalCategories = wordsClient.GetLexicalCategories(word);
                if (isLexicalCategoryValid(lexicalCategories))
                {
                    validWords.Add(word);
                }
            }

            return validWords;
        }

        public static bool isLexicalCategoryValid(List<string> lexicalCategories)
        {
            var validLexicalCategories = new string[] { "verb", "particle", "noun", "adjective", "notavailable" };

            foreach (var lexicalCategory in lexicalCategories)
            {
                var isValid = validLexicalCategories.Contains(lexicalCategory);
                if (!isValid) return false;
            }

            return true;
        }
    }
}
