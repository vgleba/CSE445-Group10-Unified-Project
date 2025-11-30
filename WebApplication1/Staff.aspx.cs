using System;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using WebApplication1.PokerBotServiceReference;
using Newtonsoft.Json;

namespace WebApplication1
{
    public partial class Staff : System.Web.UI.Page
    {
        private const string geoResolveApiUrl = "https://localhost:7227/api/threat/georesolve";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            if (Session["IsStaff"] == null || !Convert.ToBoolean(Session["IsStaff"]))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = "Welcome, " + User.Identity.Name;
                BindStaffServiceDirectory();
            }
        }

        private void BindStaffServiceDirectory()
        {
            var table = new DataTable();
            table.Columns.Add("Provider");
            table.Columns.Add("Type");
            table.Columns.Add("Name");
            table.Columns.Add("Parameters");
            table.Columns.Add("Return");
            table.Columns.Add("Description");
            table.Columns.Add("TryItUrl");

            table.Rows.Add(
                "Vladyslav Saniuk",
                "REST",
                "catalog add",
                "category: string, item: string",
                "string",
                "Adds key/value to JSON catalog",
                "#tryitCatalogAdd"
            );
            table.Rows.Add(
                "Vladyslav Saniuk",
                "REST",
                "catalog delete",
                "category: string, item: string",
                "string",
                "Deletes key/value from JSON catalog",
                "#tryitCatalogDelete"
            );
            table.Rows.Add(
                "Dmytro Ohorodiichuk",
                "WSDL (WCF)",
                "Poker bot decision",
                "gameId: guid",
                "BotDecisionResponse",
                "Calls WCF bot service to suggest the next poker action",
                "#tryitPokerBot"
            );
            table.Rows.Add(
                "Dmytro Ohorodiichuk",
                "User control",
                "Poker game state viewer",
                "gameId: guid",
                "renders PlayerDeckView",
                "Loads game JSON from REST API and renders it via PlayerDeckView user control",
                "#pokerDeckView"
            );
            table.Rows.Add(
                "Volodymyr Gleba",
                "REST Service",
                "GeoResolve",
                "string locationText",
                "GeoPoint + radius",
                "Maps place mentions & directions to coordinates using gazetteer.",
                "#tryitGeoResolve"
            );

            gvStaffDirectory.DataSource = table;
            gvStaffDirectory.DataBind();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Response.Redirect("~/Default.aspx");
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated ||
                Session["IsStaff"] == null || !Convert.ToBoolean(Session["IsStaff"]))
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            string username = User.Identity.Name;
            string oldPassword = txtOldPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmNewPassword = txtConfirmNewPassword.Text;

            lblChangePasswordMessage.Text = "";
            
            if (string.IsNullOrWhiteSpace(oldPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmNewPassword))
            {
                lblChangePasswordMessage.ForeColor = Color.Red;
                lblChangePasswordMessage.Text = "All password fields are required.";
                return;
            }

            if (!string.Equals(newPassword, confirmNewPassword))
            {
                lblChangePasswordMessage.ForeColor = Color.Red;
                lblChangePasswordMessage.Text = "New password and confirmation do not match.";
                return;
            }

            if (newPassword.Length < 6)
            {
                lblChangePasswordMessage.ForeColor = Color.Red;
                lblChangePasswordMessage.Text = "New password must be at least 6 characters long.";
                return;
            }

            bool success = AccountStore.ChangeStaffPassword(username, oldPassword, newPassword);

            if (success)
            {
                lblChangePasswordMessage.ForeColor = Color.Green;
                lblChangePasswordMessage.Text = "Password changed successfully.";
                txtOldPassword.Text = "";
                txtNewPassword.Text = "";
                txtConfirmNewPassword.Text = "";
            }
            else
            {
                lblChangePasswordMessage.ForeColor = Color.Red;
                lblChangePasswordMessage.Text = "Password change failed. Current password may be incorrect.";
            }
        }

        protected void btnCatalogAdd_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + $"api/catalog.ashx?category={HttpUtility.UrlEncode(txtCategoryAdd.Text)}&item={HttpUtility.UrlEncode(txtItemAdd.Text)}";
            litCatalogAddResult.Text = HttpUtility.HtmlEncode(DoPost(url, ""));
        }

        protected void btnCatalogDelete_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + $"api/catalog.ashx?category={HttpUtility.UrlEncode(txtCategoryDel.Text)}&item={HttpUtility.UrlEncode(txtItemDel.Text)}";
            litCatalogDeleteResult.Text = HttpUtility.HtmlEncode(DoDelete(url));
        }

        protected void btnPokerBot_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(txtPokerBotGameId.Text, out Guid gameId))
            {
                litPokerBotResult.Text = "Invalid game id.";
                return;
            }

            string gameState = DoGet(string.Format("http://webstrar10.fulton.asu.edu/page1/api/games/{0}", gameId));

            if (string.IsNullOrWhiteSpace(gameState))
            {
                litPokerBotResult.Text = "Could not load game state.";
                return;
            }

            try
            {
                BotDecisionResponse botResponse = RequestPokerBot(gameState);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("Action: {0} (Amount: {1})", botResponse.ActionType, botResponse.Amount));
                sb.AppendLine(string.Format("Narration: {0}", botResponse.Description));

                if (!string.IsNullOrWhiteSpace(botResponse.RawModelResponse))
                {
                    sb.AppendLine();
                    sb.AppendLine("Model response:");
                    sb.AppendLine(botResponse.RawModelResponse);
                }

                litPokerBotResult.Text = HttpUtility.HtmlEncode(sb.ToString());
            }
            catch (Exception ex)
            {
                litPokerBotResult.Text = HttpUtility.HtmlEncode("Error calling PokerBot service: " + ex.Message);
            }
        }

        protected void btnPokerDeckVisualize_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(txtPokerVisualizeGameId.Text, out Guid gameId))
            {
                playerDeckView.Visible = true;
                playerDeckView.ShowErrorMessage("Invalid game id.");
                return;
            }

            string gameState = DoGet(string.Format("http://webstrar10.fulton.asu.edu/page1/api/games/{0}", gameId));

            playerDeckView.Visible = true;

            if (string.IsNullOrWhiteSpace(gameState))
            {
                playerDeckView.ShowErrorMessage("Could not load game state.");
                return;
            }

            playerDeckView.RenderFromJson(gameState);
        }

        protected void btnGeoResolve_Click(object sender, EventArgs e)
        {
            lblGeoError.Text = string.Empty;
            litGeoResult.Text = string.Empty;

            var loc = txtGeoLocation.Text;
            if (string.IsNullOrWhiteSpace(loc))
            {
                lblGeoError.Text = "Please enter location text.";
                return;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var payload = new
                    {
                        location_Text = loc,
                        origin = string.IsNullOrWhiteSpace(txtGeoOrigin.Text) ? null : txtGeoOrigin.Text
                    };

                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(geoResolveApiUrl, content).Result;
                    var raw = response.Content.ReadAsStringAsync().Result;

                    litGeoResult.Text = string.Format("HTTP {0} {1}\n\n{2}", 
                        (int)response.StatusCode, 
                        response.ReasonPhrase, 
                        raw);

                    if (!response.IsSuccessStatusCode)
                    {
                        lblGeoError.Text = "Service returned an error. See response above.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblGeoError.Text = "Error calling GeoResolve service: " + ex.Message;
            }
        }

        private string GetBaseUri()
        {
            var req = HttpContext.Current.Request;
            var appPath = req.ApplicationPath;
            if (!appPath.EndsWith("/")) appPath += "/";
            return $"{req.Url.Scheme}://{req.Url.Authority}{appPath}";
        }

        private string DoPost(string url, string body)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                try { return wc.UploadString(url, "POST", body ?? string.Empty); }
                catch (WebException ex) { return ReadError(ex); }
            }
        }

        private string DoGet(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                try { return wc.DownloadString(url); }
                catch (WebException ex) { return ReadError(ex); }
            }
        }

        private string DoDelete(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "DELETE";
            try
            {
                using (var resp = (HttpWebResponse)request.GetResponse())
                using (var stream = resp.GetResponseStream())
                using (var reader = new System.IO.StreamReader(stream))
                    return reader.ReadToEnd();
            }
            catch (WebException ex) { return ReadError(ex); }
        }

        private string ReadError(WebException ex)
        {
            try
            {
                using (var resp = (HttpWebResponse)ex.Response)
                using (var stream = resp.GetResponseStream())
                using (var reader = new System.IO.StreamReader(stream))
                    return reader.ReadToEnd();
            }
            catch { return ex.Message; }
        }

        private BotDecisionResponse RequestPokerBot(string gameStateJson)
        {
            BotRequest request = new BotRequest { GameStateJson = gameStateJson };
            BotDecisionResponse response = new PokerBotServiceClient().GetBotDecision(request);
            return response;
        }
    }
}