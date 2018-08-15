using System.Net.Http;

namespace Celebratus
{
    public static class Requester
    {
        public static string GetPage(string url)
        {
            using (var httpClient = new HttpClient())
            {
                return httpClient.GetStringAsync(url).Result;
            }
        }
    }
}
