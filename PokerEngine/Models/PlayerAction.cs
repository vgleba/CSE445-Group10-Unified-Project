using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace PokerEngine.Models
{
    public class PlayerAction
    {
        public Guid PlayerId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionType Type { get; set; }
    }
}