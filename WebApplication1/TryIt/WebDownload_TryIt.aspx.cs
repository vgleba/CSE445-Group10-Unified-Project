using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.TryIt
{
    public partial class WebDownload_TryIt : System.Web.UI.Page
    {
        private const string WebDownloadBaseUrl = "https://localhost:7152/";
        private const string WebDownloadRoute = "/api/download";

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            _ = CallWebDownloadAsync();
        }

        private async Task CallWebDownloadAsync()
        {
            lblError.Text = string.Empty;
            txtResult.Text = string.Empty;

            var url = txtUrl.Text;
            if (string.IsNullOrWhiteSpace(url))
            {
                lblError.Text = "Please enter a URL.";
                return;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebDownloadBaseUrl);

                    var payload = new { url };
                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(WebDownloadRoute, content);
                    response.EnsureSuccessStatusCode();

                    var responseJson = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var obj = JsonConvert.DeserializeObject<WebDownloadResultDto>(responseJson);
                        txtResult.Text = obj?.Text ?? responseJson;
                    }
                    catch
                    {
                        txtResult.Text = responseJson;
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error calling WebDownload service: " + ex.Message;
            }
        }

        public class WebDownloadResultDto
        {
            [JsonProperty("text")]
            public string Text { get; set; }
        }
    }
}