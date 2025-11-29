using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace PokerEngine.Models
{
    public class Game
    {
        public Guid GameId { get; set; } = Guid.NewGuid();
        public Stack<LogEntry> Log { get; set; } = new Stack<LogEntry>();

        public int DealerIndex { get; set; }
        public int CurrentIndex { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Stage Stage { get; set; } = Stage.Preflop;
        public List<PlayerAction> AvailableActions { get; set; } = new List<PlayerAction>();

        public int SmallBlind { get; set; } = 10;
        public int BigBlind { get; set; } = 20;
        public int Pot { get; set; } = 0;
        public int CurrentBet { get; set; } = 0;
        public int MinRaise { get; set; } = 10;
        public int BettingRoundStartIndex { get; set; }
        public List<Guid> Winners { get; set; } = new List<Guid>();

        public List<Card> Board { get; set; } = new List<Card>(5);
        public List<Player> Players { get; set; } = new List<Player>(5);

        // Transient
        internal Deck Deck { get; set; } = new Deck(0);

        public class ActionRequest
        {
            public Guid GameId { get; set; }
            public string ActionType { get; set; }
            public int Amount { get; set; }
        }
    }
}