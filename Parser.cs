using System.Collections.Generic;

namespace Celebratus
{
    public static class Parser
    {
        public static List<string> TagContents(string page, string tag)
        {
            var openningTag = MakeOpenningTag(tag);
            var closingTag = MakeClosingTag(tag);

            var tagContents = new List<string>();

            while (page != null || page.Length > 0)
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

        private static int findFirstTagIndex(string text, string tag)
        {
            return text.IndexOf(tag);
        }

        private static string RemovePassedText(string text, int index, string closingTag)
        {
            return text.Substring(index + closingTag.Length);
        }

        private static string GetTagContent(string page, int startIndex, int endIndex)
        {
            var length = endIndex - startIndex;
            return page.Substring(startIndex, length);
        }

        public static string MakeOpenningTag(string tag)
        {
            return $"<{tag}>";
        }

        public static string MakeClosingTag(string tag)
        {
            return $"</{tag}>";
        }
    }
}
