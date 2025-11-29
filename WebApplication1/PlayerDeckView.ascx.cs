using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace WebApplication1
{
    public partial class PlayerDeckView : UserControl
    {
        public void RenderFromJson(string gameJson)
        {
            pnlError.Visible = false;
            pnlContent.Visible = false;

            if (string.IsNullOrWhiteSpace(gameJson))
            {
                ShowErrorMessage("No game data provided.");
                return;
            }

            JObject parsed;
            try
            {
                parsed = JObject.Parse(gameJson);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Invalid game JSON: {ex.Message}");
                return;
            }

            litGameId.Text = HttpUtility.HtmlEncode(parsed.Value<string>("GameId") ?? "unknown");
            litStage.Text = HttpUtility.HtmlEncode(parsed.Value<string>("Stage") ?? "?");
            litPot.Text = parsed.Value<int?>("Pot")?.ToString() ?? "0";
            litCurrentBet.Text = parsed.Value<int?>("CurrentBet")?.ToString() ?? "0";

            var boardCards = parsed["Board"] as JArray;
            litBoard.Text = (boardCards != null && boardCards.Count > 0)
                ? string.Join("] [", boardCards.Select(FormatCard))
                : "empty";

            var players = parsed["Players"] as JArray;
            int currentIndex = parsed.Value<int?>("CurrentIndex") ?? 0;

            if (players != null && players.Count > 0)
            {
                StringBuilder playersSb = new StringBuilder();
                for (int i = 0; i < players.Count; i++)
                {
                    var player = players[i];
                    string playerId = HttpUtility.HtmlEncode(player.Value<string>("PlayerId") ?? "unknown");
                    bool folded = player.Value<bool?>("Folded") ?? false;
                    int stack = player.Value<int?>("Stack") ?? 0;
                    int currentBet = player.Value<int?>("CurrentBet") ?? 0;

                    var hole = player["Hole"] as JArray;
                    string cardsDisplay = (i == currentIndex && hole != null && hole.Count > 0)
                        ? string.Join(" ", hole.Select(card => $"[{FormatCard(card)}]"))
                        : "[hidden]";

                    playersSb.AppendLine(FrameLine($"{(i == currentIndex ? ">" : " ")} Player: {playerId}{(folded ? " (folded)" : string.Empty)}"));
                    playersSb.AppendLine(FrameLine($"    O   Stack: {stack} | Bet: {currentBet}"));
                    playersSb.AppendLine(FrameLine("   /|\\  Hole cards: " + cardsDisplay));
                    playersSb.AppendLine(FrameLine($"   / \\  {(i == currentIndex ? "To act" : string.Empty)}"));
                    playersSb.AppendLine(FrameLine(string.Empty));
                }

                litPlayers.Text = playersSb.ToString();
            }
            else
            {
                litPlayers.Text = "No players found.";
            }

            pnlContent.Visible = true;
        }

        private static string FormatCard(JToken token)
        {
            if (token == null)
                return "?";

            string rank = HttpUtility.HtmlEncode(token.Value<string>("Rank") ?? "?");
            string suit = token.Value<string>("Suit") ?? "?";
            string suitIcon = SuitIcon(suit);

            return $"{rank}{suitIcon}";
        }

        private static string SuitIcon(string suit)
        {
            if (string.IsNullOrWhiteSpace(suit))
            {
                return "?";
            }

            switch (suit.Trim().ToLowerInvariant())
            {
                case "spades":
                case "spade":
                case "s":
                    return "♠";
                case "hearts":
                case "heart":
                case "h":
                    return "♥";
                case "diamonds":
                case "diamond":
                case "d":
                    return "♦";
                case "clubs":
                case "club":
                case "c":
                    return "♣";
                default:
                    return HttpUtility.HtmlEncode(suit);
            }
        }

        private static string FrameLine(string content)
        {
            const int innerWidth = 63;
            string sanitized = content ?? string.Empty;

            if (sanitized.Length > innerWidth)
            {
                sanitized = sanitized.Substring(0, innerWidth);
            }

            return $"| {sanitized.PadRight(innerWidth - 2)}|";
        }

        public void ShowErrorMessage(string message)
        {
            litError.Text = Server.HtmlEncode(message);
            pnlContent.Visible = false;
            pnlError.Visible = true;
        }
    }
}
