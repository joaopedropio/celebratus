using System.Collections.Generic;

namespace Celebratus
{
    public static class Cleaner
    {
        public static List<string> CleanTagContents(List<string> tagContents)
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

            return cleanedTagContents;
        }

        static string RemoveCDATA(string text)
        {
            return text.Replace("CDATA", "");
        }

        static string RemovePunctuation(string text)
        {
            var punctuations = new char[] { '!', '@', '#', '(', ')', '.', ',', ';', ':', '/', '\\', '?', '"', '|', '[', ']', '+', '=', '-', '<', '>', '\n' };

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
                var openningTag = Parser.MakeOpenningTag(tag);
                var closingTag = Parser.MakeClosingTag(tag);
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

                var isTagFound = startOfTagIndex != -1;
                if (!isTagFound)
                    break;
                var endOfTagINdex = text.IndexOf('>', startOfTagIndex);

                var count = endOfTagINdex - startOfTagIndex + 1;

                text = text.Remove(startOfTagIndex, count);
            }

            return text;
        }

        static string RemoveTagWithContent(string text, string tag)
        {
            var openningTag = Parser. MakeOpenningTag(tag);
            var closingTag = Parser.MakeClosingTag(tag);

            var openningTagIndex = text.IndexOf(openningTag);
            var closingTagIndex = text.IndexOf(closingTag) + closingTag.Length;

            return text.Remove(openningTagIndex, closingTagIndex);
        }

        static string RemoveTagAttributes(string text)
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
        static int NextCharacterAppereance(string text, char character, int startIndex)
        {
            return text.IndexOf(character, startIndex);
        }

        static string GetTagName(string text, int openningTagIndex)
        {
            int index;

            for (index = openningTagIndex; !isEndOfTagName(text, index); index++) ;

            var count = index - openningTagIndex;

            return text.Substring(openningTagIndex, count);
        }

        private static bool isEndOfTagName(string text, int index) => text[index] == '>' || text[index] == ' ';
    }
}
