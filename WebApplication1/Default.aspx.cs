using LocalComponents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml;
using WebApplication1.ServiceReference1;
using WebApplication1.PokerBotServiceReference;

namespace WebApplication1
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindServiceDirectory();
        }

        // Member / Staff navigation with auth check
        protected void btnMember_Click(object sender, EventArgs e)
        {
            NavigateProtected("~/Member.aspx");
        }

        protected void btnStaff_Click(object sender, EventArgs e)
        {
            NavigateProtected("~/Staff.aspx");
        }

        private void NavigateProtected(string protectedUrl)
        {
            if (Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated)
            {
                Response.Redirect(protectedUrl, endResponse: false);
            }
            else
            {
                string loginUrl = "~/Login.aspx";
                string returnUrl = HttpUtility.UrlEncode(VirtualPathUtility.ToAbsolute(protectedUrl));
                Response.Redirect($"{loginUrl}?returnUrl={returnUrl}", endResponse: false);
            }
        }

        // Directory rows
        private class DirectoryRow
        {
            public string Provider { get; set; }
            public string ComponentType { get; set; }
            public string Operation { get; set; }
            public string Parameters { get; set; }
            public string ReturnType { get; set; }
            public string Description { get; set; }
            public string TryItAnchor { get; set; } // #anchor in this page
        }

        private void BindServiceDirectory()
        {
            List<DirectoryRow> rows = new List<DirectoryRow>
            {
                new DirectoryRow
                {
                    Provider = "Dmytro Ohorodiichuk",
                    ComponentType = "REST",
                    Operation = "Poker new game",
                    Parameters = "none",
                    ReturnType = "string (game JSON)",
                    Description = "Creates a new poker game in the engine",
                    TryItAnchor = "#tryitPokerNewGame"
                },
                new DirectoryRow
                {
                    Provider = "Dmytro Ohorodiichuk",
                    ComponentType = "REST",
                    Operation = "Poker apply action",
                    Parameters = "gameId: guid, actionType: string, amount: int",
                    ReturnType = "string (game JSON)",
                    Description = "Submits a player action to the poker engine",
                    TryItAnchor = "#tryitPokerApplyAction"
                },
                new DirectoryRow
                {
                    Provider = "Dmytro Ohorodiichuk",
                    ComponentType = "WSDL (WCF)",
                    Operation = "Poker bot decision",
                    Parameters = "gameId: guid",
                    ReturnType = "BotDecisionResponse",
                    Description = "Calls WCF bot service to suggest the next poker action",
                    TryItAnchor = "#tryitPokerBot"
                },
                new DirectoryRow
                {
                    Provider = "Dmytro Ohorodiichuk",
                    ComponentType = "User control",
                    Operation = "Poker game state viewer",
                    Parameters = "gameId: guid",
                    ReturnType = "renders PlayerDeckView",
                    Description = "Loads game JSON from REST API and renders it via PlayerDeckView user control",
                    TryItAnchor = "#pokerDeckView"
                },
                new DirectoryRow
                {
                    Provider = "Dmytro Ohorodiichuk",
                    ComponentType = "User control",
                    Operation = "Poker players money view",
                    Parameters = "gameId: guid",
                    ReturnType = "renders PlayerMoneyView list",
                    Description = "Loads game JSON from REST API and renders PlayerMoneyView controls for each player",
                    TryItAnchor = "#pokerPlayersMoneyView"
                },
                new DirectoryRow
                {
                    Provider = "Dmytro Ohorodiichuk",
                    ComponentType = "DLL",
                    Operation = "Password hash",
                    Parameters = "string input",
                    ReturnType = "string",
                    Description = "Hashes a password using the local DLL component",
                    TryItAnchor = "#tryitDllHash"
                },
                new DirectoryRow
                {
                    Provider = "Dmytro Ohorodiichuk",
                    ComponentType = "DLL",
                    Operation = "Password verify",
                    Parameters = "string input, string hash",
                    ReturnType = "bool",
                    Description = "Verifies a password against a hash using the local DLL component",
                    TryItAnchor = "#tryitDllVerify"
                }
            };

            gvDirectory.DataSource = rows;
            gvDirectory.DataBind();
        }

        // TryIt handlers
        private BotDecisionResponse RequestPokerBot(string gameStateJson)
        {
            BotRequest request = new BotRequest { GameStateJson = gameStateJson };
            BotDecisionResponse response = new PokerBotServiceClient().GetBotDecision(request);

            return response;
        }

        private string DoGet(string url)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                try { return wc.DownloadString(url); }
                catch (WebException ex) { return ReadError(ex); }
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

        private string DoPost(string url, string body)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                try { return wc.UploadString(url, "POST", body ?? string.Empty); }
                catch (WebException ex) { return ReadError(ex); }
            }
        }

        private string ReadError(WebException ex)
        {
            try
            {
                using (HttpWebResponse resp = (HttpWebResponse)ex.Response)
                using (System.IO.Stream stream = resp.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    return reader.ReadToEnd();
            }
            catch { return ex.Message; }
        }

        protected void btnNewGame_Click(object sender, EventArgs e)
        {
            string result = DoPut("http://webstrar10.fulton.asu.edu/page1/api/games/", "");
            litPoker.Text = JToken.Parse(result).ToString(Newtonsoft.Json.Formatting.Indented).Replace("\r\n", "<br/>");
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
                litPokerApplyActionResult.Text = JToken.Parse(response).ToString(Newtonsoft.Json.Formatting.Indented).Replace("\r\n", "<br/>");
            }
            catch (JsonReaderException)
            {
                litPokerApplyActionResult.Text = HttpUtility.HtmlEncode(response);
            }
        }

        protected void btnPokerBot_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(txtPokerBotGameId.Text, out Guid gameId))
            {
                litPokerBotResult.Text = "Invalid game id.";
                return;
            }

            string gameState = DoGet($"http://webstrar10.fulton.asu.edu/page1/api/games//{gameId}");

            if (string.IsNullOrWhiteSpace(gameState))
            {
                litPokerBotResult.Text = "Could not load game state.";
                return;
            }

            try
            {
                BotDecisionResponse botResponse = RequestPokerBot(gameState);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Action: {botResponse.ActionType} (Amount: {botResponse.Amount})");
                sb.AppendLine($"Narration: {botResponse.Description}");

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

        protected void btnDllHash_Click(object sender, EventArgs e)
        {
            string data = txtDllHashInput.Text ?? "";
            try
            {
                string result = PasswordHandler.HashPassword(data);
                litDllHashResult.Text = HttpUtility.HtmlEncode(result);
            }
            catch (Exception ex)
            {
                litDllHashResult.Text = HttpUtility.HtmlEncode("DLL hashing error: " + ex.ToString());
            }
        }

        protected void btnDllVerify_Click(object sender, EventArgs e)
        {
            string verifyData = txtDllVerifyInput.Text ?? "";
            string hashedData = txtDllHashedInput.Text ?? "";
            try
            {
                bool result = PasswordHandler.VerifyPassword(verifyData, hashedData);
                litDllVerifyResult.Text = HttpUtility.HtmlEncode(result);
            }
            catch (Exception ex)
            {
                litDllVerifyResult.Text = HttpUtility.HtmlEncode("DLL hashing error: " + ex.ToString());
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

            string gameState = DoGet($"http://webstrar10.fulton.asu.edu/page1/api/games/{gameId}");

            playerDeckView.Visible = true;

            if (string.IsNullOrWhiteSpace(gameState))
            {
                playerDeckView.ShowErrorMessage("Could not load game state.");
                return;
            }

            playerDeckView.RenderFromJson(gameState);
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

            string gameState = DoGet($"http://webstrar10.fulton.asu.edu/page1/api/games/{gameId}");

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
    }
}