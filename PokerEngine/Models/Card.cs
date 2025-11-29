using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PokerEngine.Models
{
    public struct Card
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Rank Rank { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Suit Suit { get; set; }

        public override string ToString() => $"{Rank} of {Suit}";
    }
}