using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace WebApplication1.TryIt
{
    public partial class ThreatNLP_TryIt : System.Web.UI.Page
    {
        private const string baseApiUrl = "https://localhost:7227";
        private const string threatNlpPath = "/api/threat/nlp";

        private const string CookieName = "AAI_LastThreatText";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblEndpoint.Text = baseApiUrl + threatNlpPath;

                var cookie = Request.Cookies[CookieName];
                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    lblCookieInfo.Text = "Found saved input in cookie.";
                }
                else
                {
                    lblCookieInfo.Text = "No saved input in cookie yet.";
                }
            }
        }

        protected void btnExampleUA_Click(object sender, EventArgs e)
        {
            txtInput.Text =
                "Зафіксовано БПЛА, рух на пн-зх від Білої Церкви. Можливе прольоти в напрямку Києва.";
        }

        protected void btnExampleEN_Click(object sender, EventArgs e)
        {
            txtInput.Text =
                "Missiles reported moving northwest near Vinnytsia. Possible trajectory towards Kyiv.";
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtInput.Text = string.Empty;
            txtRawJson.Text = string.Empty;
            lblError.Text = string.Empty;
            gvEvents.DataSource = null;
            gvEvents.DataBind();
        }

        protected void btnLoadFromCookie_Click(object sender, EventArgs e)
        {
            var cookie = Request.Cookies[CookieName];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                txtInput.Text = cookie.Value;
                lblCookieInfo.Text = "Loaded input from cookie.";
            }
            else
            {
                lblCookieInfo.Text = "Cookie is empty or missing.";
            }
        }

        protected void btnInvoke_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            txtRawJson.Text = string.Empty;
            gvEvents.DataSource = null;
            gvEvents.DataBind();

            var text = txtInput.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                lblError.Text = "Please enter some text.";
                return;
            }

            var cookie = new System.Web.HttpCookie(CookieName, text)
            {
                Expires = DateTime.Now.AddMinutes(30)
            };
            Response.Cookies.Add(cookie);
            lblCookieInfo.Text = "Saved current input into cookie.";

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseApiUrl);

                    var payload = new { text };
                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(threatNlpPath, content).Result;
                    var raw = response.Content.ReadAsStringAsync().Result;

                    txtRawJson.Text =
                        $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}\r\n\r\n{raw}";

                    response.EnsureSuccessStatusCode();

                    var events = JsonConvert.DeserializeObject<List<ThreatEventDto>>(raw)
                                 ?? new List<ThreatEventDto>();

                    gvEvents.DataSource = events;
                    gvEvents.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error calling ThreatNLP service: " + ex.Message;
            }
        }

        public class ThreatEventDto
        {
            [JsonProperty("threatType")]
            public string ThreatType { get; set; }

            [JsonProperty("directionDeg")]
            public double? DirectionDeg { get; set; }

            [JsonProperty("locationText")]
            public string LocationText { get; set; }

            [JsonProperty("timestamp")]
            public DateTime Timestamp { get; set; }

            [JsonProperty("confidence")]
            public double? Confidence { get; set; }
        }
    }
}