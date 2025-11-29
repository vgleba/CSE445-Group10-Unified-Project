using System;
using System.Collections.Generic;

namespace PokerEngine.Models
{
    public class Deck
    {
        private readonly List<Card> _cards = new List<Card>(52);
        private int _index;

        public Deck(int seed)
        {
            foreach (Suit s in Enum.GetValues(typeof(Suit)))
                foreach (Rank r in Enum.GetValues(typeof(Rank)))
                    _cards.Add(new Card { Rank = r, Suit = s });

            Shuffle(seed);
        }

        private void Shuffle(int seed)
        {
            Random rng = new Random(seed);
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                Card tmp = _cards[i];
                _cards[i] = _cards[j];
                _cards[j] = tmp;
            }
            _index = 0;
        }

        public Card Draw() => _cards[_index++];
    }
}