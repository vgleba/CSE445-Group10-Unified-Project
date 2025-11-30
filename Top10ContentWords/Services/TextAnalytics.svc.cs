using System;
using System.Configuration;
using System.Net.Http;
using System.ServiceModel.Activation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Top10ContentWords.Logic;

namespace Top10ContentWords.Services
{
    [AspNetCompatibilityRequirements(
    RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TextAnalytics : ITextAnalytics
    {
        private static readonly HttpClient Http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        public string[] Top10ContentWords(string inputOrUrl)
        {
            if (string.IsNullOrWhiteSpace(inputOrUrl)) return new string[0];

            string text = inputOrUrl;

            Uri uri;
            if (Uri.TryCreate(inputOrUrl, UriKind.Absolute, out uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                var baseUrl = (ConfigurationManager.AppSettings["DownloadBaseUrl"] ?? "").TrimEnd('/');
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    var q = HttpUtility.UrlEncode(uri.ToString());
                    var url = baseUrl + "/api/download?url=" + q + "&raw=false";
                    text = SafeGet(url).GetAwaiter().GetResult();
                }
            }

            return TextPipeline.Top10(text);
        }

        private static async Task<string> SafeGet(string url)
        {
            try
            {
                using (var resp = await Http.GetAsync(url))
                {
                    resp.EnsureSuccessStatusCode();
                    return await resp.Content.ReadAsStringAsync();
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
