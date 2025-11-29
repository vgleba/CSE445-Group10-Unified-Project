using System;
using System.Collections.Generic;

namespace PokerEngine.Models
{
    public class Player
    {
        public Guid PlayerId { get; set; } = Guid.NewGuid();
        public int Stack { get; set; } = 1000;
        public int CurrentBet { get; set; }
        public List<Card> Hole { get; set; } = new List<Card>(2);
        public bool Folded { get; set; }
    }
}