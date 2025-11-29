using PokerEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokerEngine
{
    public class GameRepository
    {
        public static Dictionary<Guid, Game> Games { get; } = new Dictionary<Guid, Game>();
    }
}