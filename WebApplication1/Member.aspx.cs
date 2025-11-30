using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApplication1.Top10Svc;

namespace WebApplication1
{
    public partial class Member : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            if (Session["IsStaff"] == null || Convert.ToBoolean(Session["IsStaff"]))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = "Welcome, " + User.Identity.Name;
                BindMemberServiceDirectory();
            }
        }

        private void BindMemberServiceDirectory()
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
                "catalog get item",
                "category: string, item: string",
                "string (confirmation if found)",
                "Gets a specific category/item pair from JSON catalog",
                "#tryitCatalogGet"
            );
            table.Rows.Add(
                "Vladyslav Saniuk",
                "REST",
                "cart refresh",
                "none",
                "string (cart items)",
                "Displays all items currently in the shopping cart",
                "#tryitCart"
            );
            table.Rows.Add(
                "Vladyslav Saniuk",
                "REST",
                "cart checkout",
                "none",
                "string (thank you message)",
                "Processes checkout, shows thank you message, validate shipping address via 3rd party API and removes items from catalog",
                "#tryitCart"
            );
            table.Rows.Add(
                "Dmytro Ohorodiichuk",
                "REST",
                "Poker new game",
                "none",
                "string (game JSON)",
                "Creates a new poker game in the engine",
                "#tryitPokerNewGame"
            );
            table.Rows.Add(
                "Dmytro Ohorodiichuk",
                "REST",
                "Poker apply action",
                "gameId: guid, actionType: string, amount: int",
                "string (game JSON)",
                "Submits a player action to the poker engine",
                "#tryitPokerApplyAction"
            );
            table.Rows.Add(
                "Dmytro Ohorodiichuk",
                "User control",
                "Poker players money view",
                "gameId: guid",
                "renders PlayerMoneyView list",
                "Loads game JSON from REST API and renders PlayerMoneyView controls for each player",
                "#pokerPlayersMoneyView"
            );
            table.Rows.Add(
                "Volodymyr Gleba",
                "WCF Service",
                "Top10ContentWords",
                "string inputOrUrl",
                "List<string> top10Words",
                "Returns the top 10 content words after stopword removal and stemming.",
                "#tryitTop10"
            );

            gvMemberDirectory.DataSource = table;
            gvMemberDirectory.DataBind();
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
                Session["IsStaff"] == null || Convert.ToBoolean(Session["IsStaff"]))
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

            bool success = AccountStore.ChangeMemberPassword(username, oldPassword, newPassword);

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

        protected void btnCatalogGet_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + string.Format("api/catalog.ashx?action=getitem&category={0}&item={1}", 
                HttpUtility.UrlEncode(txtCategoryGet.Text), HttpUtility.UrlEncode(txtItemGet.Text));
            var result = DoGet(url);
            litCatalogGetResult.Text = HttpUtility.HtmlEncode(result);
            
            if (result.StartsWith("Found:"))
            {
                btnAddToCart.Visible = true;
                ViewState["FoundCategory"] = txtCategoryGet.Text;
                ViewState["FoundItem"] = txtItemGet.Text;
            }
            else
            {
                btnAddToCart.Visible = false;
                ViewState["FoundCategory"] = null;
                ViewState["FoundItem"] = null;
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            var category = ViewState["FoundCategory"] as string;
            var item = ViewState["FoundItem"] as string;
            
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(item))
            {
                var baseUri = GetBaseUri();
                var url = baseUri + string.Format("api/cart.ashx?action=add&category={0}&item={1}",
                    HttpUtility.UrlEncode(category), HttpUtility.UrlEncode(item));
                var result = DoPost(url, "");
                
                litCatalogGetResult.Text += Environment.NewLine + Environment.NewLine + HttpUtility.HtmlEncode(result);
                btnAddToCart.Visible = false;
                
                ViewState["FoundCategory"] = null;
                ViewState["FoundItem"] = null;
            }
            else
            {
                litCatalogGetResult.Text += Environment.NewLine + Environment.NewLine + "Error: No item data available to add to cart.";
                btnAddToCart.Visible = false;
            }
        }

        protected void btnCartRefresh_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + "api/cart.ashx";
            litCartResult.Text = HttpUtility.HtmlEncode(DoGet(url));
        }

        protected void btnCartCheckout_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + "api/cart.ashx?action=checkout";
            var result = DoPost(url, "");
            litCartResult.Text = HttpUtility.HtmlEncode(result);
            
            if (result.Contains("Thank you for shopping with us!"))
            {
                pnlAddress.Visible = true;
                lblAddressError.Text = "";
            }
        }

        protected void btnProceed_Click(object sender, EventArgs e)
        {
            try
            {
                string userState = txtState.Text != null ? txtState.Text.Trim() : "";
                string userZip = txtZip.Text != null ? txtZip.Text.Trim() : "";
                
                if (string.IsNullOrEmpty(userState) || string.IsNullOrEmpty(userZip))
                {
                    lblAddressError.Text = "Please provide both State and ZIP code.";
                    return;
                }

                string apiUrl = string.Format("http://api.zippopotam.us/us/{0}", userZip);
                
                using (var wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    string jsonResponse = wc.DownloadString(apiUrl);
                    
                    string apiState = ExtractStateFromZipResponse(jsonResponse);
                    
                    if (string.IsNullOrEmpty(apiState))
                    {
                        lblAddressError.Text = "Invalid ZIP code. Please provide a valid ZIP code.";
                        return;
                    }
                    
                    if (string.Equals(userState, apiState, StringComparison.OrdinalIgnoreCase) ||
                        IsStateAbbreviationMatch(userState, apiState))
                    {
                        litCartResult.Text += "\n\nOrder processed successfully! Your items will be shipped to " + userState + ", " + userZip + ".";
                        pnlAddress.Visible = false;
                    }
                    else
                    {
                        lblAddressError.Text = "Invalid address. The provided state does not match the ZIP code. Please provide a valid State and ZIP code.";
                    }
                }
            }
            catch (WebException webEx)
            {
                lblAddressError.Text = string.Format("Invalid address. Please provide a valid State and ZIP code. (Error: {0})", webEx.Message);
            }
            catch (Exception ex)
            {
                lblAddressError.Text = string.Format("Error validating address: {0}. Please try again.", ex.Message);
            }
        }

        protected void btnNewGame_Click(object sender, EventArgs e)
        {
            string result = DoPut("http://webstrar10.fulton.asu.edu/page1/api/games/", "");
            litPoker.Text = JToken.Parse(result).ToString(Formatting.Indented).Replace("\r\n", "<br/>");
        }

        protected void btnPokerApplyAction_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(txtPokerGameId.Text, out Guid gameId))
            {
                litPokerApplyActionResult.Text = "Invalid game id.";
                return;
            }

            int amount = 0;
            if (!string.IsNullOrWhiteSpace(txtPokerAmount.Text) && !int.TryParse(txtPokerAmount.Text, out amount))
            {
                litPokerApplyActionResult.Text = "Amount must be a number.";
                return;
            }

            var request = new
            {
                GameId = gameId,
                ActionType = txtPokerActionType.Text,
                Amount = amount
            };

            string response = DoPost("http://webstrar10.fulton.asu.edu/page1/api/games/apply", JsonConvert.SerializeObject(request));

            try
            {
                litPokerApplyActionResult.Text = JToken.Parse(response).ToString(Formatting.Indented).Replace("\r\n", "<br/>");
            }
            catch (JsonReaderException)
            {
                litPokerApplyActionResult.Text = HttpUtility.HtmlEncode(response);
            }
        }

        protected void btnPokerMoneyVisualize_Click(object sender, EventArgs e)
        {
            phPlayersMoney.Controls.Clear();
            litPokerMoneyStatus.Text = string.Empty;

            if (!Guid.TryParse(txtPokerMoneyGameId.Text, out Guid gameId))
            {
                litPokerMoneyStatus.Text = "Invalid game id.";
                return;
            }

            string gameState = DoGet(string.Format("http://webstrar10.fulton.asu.edu/page1/api/games/{0}", gameId));

            if (string.IsNullOrWhiteSpace(gameState))
            {
                litPokerMoneyStatus.Text = "Could not load game state.";
                return;
            }

            try
            {
                JObject game = JObject.Parse(gameState);
                JArray players = (JArray)game["Players"];

                if (players == null || players.Count == 0)
                {
                    litPokerMoneyStatus.Text = "No players found for this game.";
                    return;
                }

                foreach (JToken player in players)
                {
                    string playerId = player.Value<string>("PlayerId");
                    int stack = player.Value<int?>("Stack") ?? 0;
                    int currentBet = player.Value<int?>("CurrentBet") ?? 0;
                    bool folded = player.Value<bool?>("Folded") ?? false;

                    PlayerMoneyView moneyView = (PlayerMoneyView)LoadControl("~/PlayerMoneyView.ascx");
                    moneyView.BindPlayer(Guid.Parse(playerId), stack, currentBet, folded);
                    phPlayersMoney.Controls.Add(moneyView);
                }
            }
            catch (JsonReaderException)
            {
                litPokerMoneyStatus.Text = HttpUtility.HtmlEncode(gameState);
            }
            catch (Exception ex)
            {
                litPokerMoneyStatus.Text = HttpUtility.HtmlEncode("Error loading game: " + ex.Message);
            }
        }

        private string DoPut(string url, string body)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                try { return wc.UploadString(url, "PUT", body ?? string.Empty); }
                catch (WebException ex) { return ReadError(ex); }
            }
        }

        protected void btnTop10_Click(object sender, EventArgs e)
        {
            bltTop10Results.Items.Clear();
            
            try
            {
                var client = new TextAnalyticsClient();
                var input = txtTop10Input.Text;
                var words = client.Top10ContentWords(input);

                foreach (var w in words)
                {
                    bltTop10Results.Items.Add(w);
                }
            }
            catch (Exception ex)
            {
                bltTop10Results.Items.Add("Error: " + ex.Message);
            }
        }

        private string ExtractStateFromZipResponse(string jsonResponse)
        {
            try
            {
                int stateIndex = jsonResponse.IndexOf("\"state\":");
                if (stateIndex > 0)
                {
                    int startQuote = jsonResponse.IndexOf("\"", stateIndex + 8);
                    int endQuote = jsonResponse.IndexOf("\"", startQuote + 1);
                    if (startQuote > 0 && endQuote > startQuote)
                    {
                        return jsonResponse.Substring(startQuote + 1, endQuote - startQuote - 1);
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private bool IsStateAbbreviationMatch(string userInput, string fullStateName)
        {
            var stateAbbreviations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"AL", "Alabama"}, {"AK", "Alaska"}, {"AZ", "Arizona"}, {"AR", "Arkansas"}, {"CA", "California"},
                {"CO", "Colorado"}, {"CT", "Connecticut"}, {"DE", "Delaware"}, {"FL", "Florida"}, {"GA", "Georgia"},
                {"HI", "Hawaii"}, {"ID", "Idaho"}, {"IL", "Illinois"}, {"IN", "Indiana"}, {"IA", "Iowa"},
                {"KS", "Kansas"}, {"KY", "Kentucky"}, {"LA", "Louisiana"}, {"ME", "Maine"}, {"MD", "Maryland"},
                {"MA", "Massachusetts"}, {"MI", "Michigan"}, {"MN", "Minnesota"}, {"MS", "Mississippi"}, {"MO", "Missouri"},
                {"MT", "Montana"}, {"NE", "Nebraska"}, {"NV", "Nevada"}, {"NH", "New Hampshire"}, {"NJ", "New Jersey"},
                {"NM", "New Mexico"}, {"NY", "New York"}, {"NC", "North Carolina"}, {"ND", "North Dakota"}, {"OH", "Ohio"},
                {"OK", "Oklahoma"}, {"OR", "Oregon"}, {"PA", "Pennsylvania"}, {"RI", "Rhode Island"}, {"SC", "South Carolina"},
                {"SD", "South Dakota"}, {"TN", "Tennessee"}, {"TX", "Texas"}, {"UT", "Utah"}, {"VT", "Vermont"},
                {"VA", "Virginia"}, {"WA", "Washington"}, {"WV", "West Virginia"}, {"WI", "Wisconsin"}, {"WY", "Wyoming"}
            };

            if (stateAbbreviations.ContainsKey(userInput) && 
                string.Equals(stateAbbreviations[userInput], fullStateName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            
            foreach (var kvp in stateAbbreviations)
            {
                if (string.Equals(kvp.Value, fullStateName, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(kvp.Key, userInput, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            
            return false;
        }

        private string GetBaseUri()
        {
            var req = HttpContext.Current.Request;
            var appPath = req.ApplicationPath;
            if (!appPath.EndsWith("/")) appPath += "/";
            return string.Format("{0}://{1}{2}", req.Url.Scheme, req.Url.Authority, appPath);
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
    }
}