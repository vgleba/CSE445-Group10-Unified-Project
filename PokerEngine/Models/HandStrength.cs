using System;
using System.Collections.Generic;

namespace PokerEngine.Models
{
    internal class HandStrength : IComparable<HandStrength>
    {
        public int Category { get; }
        public List<int> Kickers { get; }

        public HandStrength(int category, List<int> kickers)
        {
            Category = category;
            Kickers = kickers;
        }

        public int CompareTo(HandStrength other)
        {
            if (other == null)
            {
                return 1;
            }

            int categoryComparison = Category.CompareTo(other.Category);
            if (categoryComparison != 0)
            {
                return categoryComparison;
            }

            int length = Math.Min(Kickers.Count, other.Kickers.Count);
            for (int i = 0; i < length; i++)
            {
                int cmp = Kickers[i].CompareTo(other.Kickers[i]);
                if (cmp != 0)
                {
                    return cmp;
                }
            }

            return Kickers.Count.CompareTo(other.Kickers.Count);
        }
    }
}