using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace WebApplication1.TryIt
{
    public partial class GeoResolve_TryIt : System.Web.UI.Page
    {
        private const string baseApiUrl = "https://localhost:7227";
        private const string geoResolvePath = "/api/threat/georesolve";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblEndpoint.Text = baseApiUrl + geoResolvePath;
            }
        }

        protected void btnExampleDir_Click(object sender, EventArgs e)
        {
            txtLocation.Text = "NW of Bila Tserkva";
            txtOrigin.Text = string.Empty;
        }

        protected void btnExampleCity_Click(object sender, EventArgs e)
        {
            txtLocation.Text = "Vinnytsia";
            txtOrigin.Text = string.Empty;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtLocation.Text = string.Empty;
            txtOrigin.Text = string.Empty;
            txtRawJson.Text = string.Empty;
            lblError.Text = string.Empty;
            fvResult.DataSource = null;
            fvResult.DataBind();
        }

        protected void btnInvoke_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            txtRawJson.Text = string.Empty;
            fvResult.DataSource = null;
            fvResult.DataBind();

            var loc = txtLocation.Text;
            if (string.IsNullOrWhiteSpace(loc))
            {
                lblError.Text = "Please enter location_Text.";
                return;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseApiUrl);

                    var payload = new
                    {
                        location_Text = loc,
                        origin = string.IsNullOrWhiteSpace(txtOrigin.Text)
                            ? null
                            : txtOrigin.Text
                    };

                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(geoResolvePath, content).Result;
                    var raw = response.Content.ReadAsStringAsync().Result;

                    txtRawJson.Text =
                        $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}\r\n\r\n{raw}";

                    response.EnsureSuccessStatusCode();

                    var result = JsonConvert.DeserializeObject<GeoResolveResultDto>(raw);
                    if (result != null)
                    {
                        fvResult.DataSource = new[] { result };
                        fvResult.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error calling GeoResolve service: " + ex.Message;
            }
        }

        public class GeoResolveResultDto
        {
            [JsonProperty("lat")]
            public double Lat { get; set; }

            [JsonProperty("lon")]
            public double Lon { get; set; }

            [JsonProperty("r_km")]
            public double RadiusKm { get; set; }
        }
    }
}