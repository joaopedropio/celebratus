using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace Celebratus
{
    public class WordsApiClient
    {
        private HttpClient httpClient;
        private string uriTemplate;

        public WordsApiClient(string uriTemplate, string appId, string appKey)
        {
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Add("app_id", appId);
            this.httpClient.DefaultRequestHeaders.Add("app_key", appKey);
            this.uriTemplate = uriTemplate;
        }

        public List<string> GetLexicalCategories(string word)
        {
            var url = GetOxfordApiUrl(this.uriTemplate, word);

            try
            {
                var responseJson = this.httpClient.GetStringAsync(url).Result;
                return GetLexicalCategoriesFromJson(responseJson);
            }
            catch (Exception)
            {
                return new List<string>() { "notavailable" };
            }
        }

        public List<string> GetLexicalCategoriesFromJson(string json)
        {
            try
            {
                var lexicalCategory = JsonConvert.DeserializeObject<LexicalCategory>(json);
                return GetLexicalCategories(lexicalCategory.Results);
            }
            catch (Exception)
            {
                if(json.Contains("No entry available for"))
                {
                    return new List<string>() { "notavailable" };
                }
                throw;
            }
        }

        public static List<string> GetLexicalCategories(List<Result> results)
        {
            var lexicalCategories = new List<string>();
            foreach (var result in results)
            {
                lexicalCategories.AddRange(result.LexicalEntries.Select(lc => lc.LexicalCategory.ToLower()));
            }
            return lexicalCategories;
        }

        public static string GetOxfordApiUrl(string uriTemplate, string word)
        {
            return uriTemplate.Replace("{word}", word);
        }
    }
}
