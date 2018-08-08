using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Linq;

namespace Celebratus
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            var rssFeedPageText = httpClient.GetStringAsync("http://feeds.arstechnica.com/arstechnica/technology-lab").Result;

            var tagContents = TagContents(rssFeedPageText, "content:encoded");

            var cleanedTagContents = CleanTagContents(tagContents);
        }

        static List<string> CleanTagContents(List<string> tagContents)
        {
            var cleanedTagContents = new List<string>();

            foreach (var tagContent in tagContents)
            {
                var cleanedContent = RemoveTagAttributes(tagContent);
                cleanedContent = RemoveTagsWithoutClosingTag(cleanedContent);
                cleanedContent = RemoveTags(cleanedContent);
                cleanedContent = RemovePunctuation(cleanedContent);
                cleanedTagContents.Add(cleanedContent);
            }

            return cleanedTagContents;
        }

        static string RemovePunctuation(string text)
        {
            var punctuations = new char[] { '!', '@', '#', '(', ')', '.', ',', ';', ':', '/', '\\', '?', '"', '|', '[', ']', '+', '=', '-', '<', '>' };

            foreach (var punctuation in punctuations)
            {
                text = text.Replace(punctuation, ' ');
            }

            return text;
        }

        static string RemoveTags(string text)
        {
            var tags = new string[] { "figure", "span", "p", "div", "a", "blockquote" };

            foreach (var tag in tags)
            {
                var openningTag = MakeOpenningTag(tag);
                var closingTag = MakeClosingTag(tag);
                text = RemoveTag(text, openningTag);
                text = RemoveTag(text, closingTag);
            }

            return text;
        }

        static string RemoveTag(string text, string tag)
        {
            int index = text.IndexOf(tag);

            while (index != -1)
            {
                text = text.Remove(index, tag.Length);
                index = text.IndexOf(tag);
            }

            return text;
        }

        static string RemoveTagsWithoutClosingTag(string text)
        {
            var tagsToRemoveWithoutClosingTag = new string[] { "img" };

            foreach (var tag in tagsToRemoveWithoutClosingTag)
            {
                var startTag = '<' + tag;

                var startOfTagIndex = text.IndexOf(startTag);
                var endOfTagINdex = text.IndexOf('>', startOfTagIndex);

                var count = endOfTagINdex - startOfTagIndex + 1;

                text = text.Remove(startOfTagIndex, count);
            }

            return text;
        }

        static string RemoveTagWithContent(string text, string tag)
        {
            var openningTag = MakeOpenningTag(tag);
            var closingTag = MakeClosingTag(tag);

            var openningTagIndex = text.IndexOf(openningTag);
            var closingTagIndex = text.IndexOf(closingTag) + closingTag.Length;

            return text.Remove(openningTagIndex, closingTagIndex);
        }

        static string RemoveTagAttributes(string text)
        {
            if (text == null || text.Length == 0)
                return text;

            var indexCount = 0;

            while(text.Length > indexCount)
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

        static string GetTagName(string text, int openningTagIndex)
        {
            int index;

            for (index = openningTagIndex; !isEndOfTagName(text, index); index++);

            var count = index - openningTagIndex;

            return text.Substring(openningTagIndex, count);
        }

        static bool isEndOfTagName(string text, int index) => text[index] == '>' || text[index] == ' ';

        static int NextCharacterAppereance(string text, char character, int startIndex)
        {
            return text.IndexOf(character, startIndex);
        }

        static int findFirstTagIndex(string text, string tag)
        {
            return text.IndexOf(tag);
        }

        static List<string> TagContents(string page, string tag)
        {
            var openningTag = MakeOpenningTag(tag);
            var closingTag = MakeClosingTag(tag);

            var tagContents = new List<string>();

            while(page != null || page.Length > 0)
            {
                var openingTagIndex = findFirstTagIndex(page, openningTag);
                var closingTagIndex = findFirstTagIndex(page, closingTag);

                var isTagFound = openingTagIndex != -1 && closingTagIndex != -1;

                if (!isTagFound)
                    break;

                var tagContent = GetTagContent(page, openingTagIndex + openningTag.Length, closingTagIndex);

                tagContents.Add(tagContent);

                page = RemovePassedText(page, closingTagIndex, closingTag);
            }
            return tagContents;
        }

        static string RemovePassedText(string text, int index, string closingTag)
        {
            return text.Substring(index + closingTag.Length);
        }

        static string GetTagContent(string page, int startIndex, int endIndex)
        {
            var length = endIndex - startIndex;
            return page.Substring(startIndex, length);
        }

        static string MakeOpenningTag(string tag)
        {
            return $"<{tag}>";
        }

        static string MakeClosingTag(string tag)
        {
            return $"</{tag}>";
        }
    }
}
