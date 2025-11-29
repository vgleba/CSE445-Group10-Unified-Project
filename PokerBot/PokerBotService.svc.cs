using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.ServiceModel.Activation;
using System.Text;

namespace PokerBot
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PokerBotService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PokerBotService.svc or PokerBotService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(
        RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PokerBotService : IPokerBotService
    {
        public BotDecisionResponse GetBotDecision(BotRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.GameStateJson))
            {
                throw new ArgumentException("Game data is required for the bot to make a decision.", nameof(request));
            }

            string apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "AIzaSyAsBv0Jf-rpI8YjfRTJ2ujaDOV5P9SZHII";
            string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return new BotDecisionResponse
                {
                    ActionType = "Fold",
                    Amount = 0,
                    Description = "GEMINI_API_KEY is not configured on the server.",
                    RawModelResponse = string.Empty
                };
            }

            var prompt = new StringBuilder();
            prompt.AppendLine("You are a poker coach bot. Analyze the provided JSON game state and suggest one action.");
            prompt.AppendLine("Pick the action from AvailableActions[].Type.");
            prompt.AppendLine("If the recommended action is raise, provide an integer amount that is at least the minimum raise and within the current player's stack.");
            prompt.AppendLine("Respond strictly as JSON with fields: actionType (string), amount (integer), description (string must be a single sentence (no more than 40 words) of literary narration that mentions: - the chosen action, and - the player's visible emotions while taking this action.).");
            prompt.AppendLine("Game state JSON:");
            prompt.Append(request.GameStateJson);

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt.ToString() }
                        }
                    }
                },
                generationConfig = new
                {
                    responseMimeType = "application/json",
                    responseSchema = new
                    {
                        type = "OBJECT",
                        properties = new
                        {
                            actionType = new { type = "STRING" },
                            amount = new { type = "INTEGER" },
                            description = new { type = "STRING" }
                        },
                        required = new[] { "actionType", "amount", "description" }
                    }
                }
            };

            string jsonBody = JsonConvert.SerializeObject(payload);

            using (HttpClient http = new HttpClient())
            using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url))
            {
                req.Headers.Add("x-goog-api-key", apiKey);
                req.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage res = http.SendAsync(req).GetAwaiter().GetResult();
                string responseText = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!res.IsSuccessStatusCode)
                {
                    return new BotDecisionResponse
                    {
                        ActionType = "Fold",
                        Amount = 0,
                        Description = $"Gemini API call failed: {res.StatusCode}",
                        RawModelResponse = responseText
                    };
                }

                string structuredText = ExtractStructuredJson(responseText);
                BotDecisionResponse parsedResponse = null;
                if (!string.IsNullOrWhiteSpace(structuredText))
                {
                    try
                    {
                        parsedResponse = JsonConvert.DeserializeObject<BotDecisionResponse>(structuredText);
                    }
                    catch
                    {
                        // fall back below
                    }
                }

                if (parsedResponse == null)
                {
                    parsedResponse = new BotDecisionResponse
                    {
                        ActionType = "Call",
                        Amount = 0,
                        Description = "Unable to parse Gemini response; defaulted to call.",
                        RawModelResponse = responseText
                    };
                }
                else
                {
                    parsedResponse.RawModelResponse = responseText;
                }

                return parsedResponse;
            }
        }

        private static string ExtractStructuredJson(string responseText)
        {
            try
            {
                var obj = JObject.Parse(responseText);
                return obj["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
