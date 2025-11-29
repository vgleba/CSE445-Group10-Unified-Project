using PokerEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PokerEngine.Controllers
{
    [RoutePrefix("api/games")]
    public partial class GameController : ApiController
    {
        [HttpPut, Route("")]
        public IHttpActionResult NewGame()
        {
            Game game = new Game();

            game.Log.Push(new LogEntry { Message = $"Game {game.GameId} created." });

            for (int i = 0; i < 5; i++)
            {
                game.Players.Add(new Player()
                {
                    Hole = {
                        game.Deck.Draw(),
                        game.Deck.Draw()
                    }
                });
            }

            game.Log.Push(new LogEntry { Message = $"Player {game.Players[game.DealerIndex].PlayerId} is dealer." });
            foreach (var p in game.Players)
            {
                game.Log.Push(new LogEntry { Message = $"Player {p.PlayerId} served." });
            }

            // blinds
            int sbIdx = (game.DealerIndex + 1) % game.Players.Count;
            int bbIdx = (game.DealerIndex + 2) % game.Players.Count;
            game.CurrentBet = game.BigBlind;
            game.Pot = game.SmallBlind + game.BigBlind;
            game.Players[sbIdx].Stack -= game.SmallBlind;
            game.Players[sbIdx].CurrentBet = game.SmallBlind;
            game.Log.Push(new LogEntry { Message = $"Player {game.Players[bbIdx].PlayerId} has small blind." });
            game.Players[bbIdx].Stack -= game.BigBlind;
            game.Players[bbIdx].CurrentBet = game.BigBlind;
            game.Log.Push(new LogEntry { Message = $"Player {game.Players[bbIdx].PlayerId} has big blind." });

            game.CurrentIndex = (game.DealerIndex + 3) % game.Players.Count;
            game.BettingRoundStartIndex = game.CurrentIndex;

            Player player = game.Players[game.CurrentIndex];
            game.Log.Push(new LogEntry { Message = $"Player {player.PlayerId} is next to play." });
            SetAvailableActions(game, player);

            GameRepository.Games.Add(game.GameId, game);

            return Ok(game);
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult GetGame(Guid id)
        {
            if (GameRepository.Games.TryGetValue(id, out Game game))
            {
                return Ok(game);
            }

            return NotFound();
        }

        [HttpPost, Route("apply")]
        public IHttpActionResult ApplyAction([FromBody] Game.ActionRequest request)
        {
            if (request == null)
                return BadRequest("Action request is required.");

            if (!GameRepository.Games.TryGetValue(request.GameId, out Game game))
                return NotFound();

            if (!Enum.TryParse(request.ActionType, true, out ActionType parsedActionType))
                return BadRequest("Unknown action type.");

            PlayerAction action = game.AvailableActions.Find(a => a.Type == parsedActionType);

            if (action == null)
                return NotFound();

            Player currentPlayer = game.Players[game.CurrentIndex];
            if (action.PlayerId != currentPlayer.PlayerId)
                return BadRequest("It is not this player's turn.");

            switch (action.Type)
            {
                case ActionType.Fold:
                    currentPlayer.Folded = true;
                    game.Log.Push(new LogEntry { Message = $"Player {currentPlayer.PlayerId} folded." });
                    break;
                case ActionType.Check:
                    if (game.CurrentBet > currentPlayer.CurrentBet)
                        return BadRequest("Cannot check while facing a bet.");
                    game.Log.Push(new LogEntry { Message = $"Player {currentPlayer.PlayerId} checked." });
                    break;
                case ActionType.Call:
                    int amountToCall = game.CurrentBet - currentPlayer.CurrentBet;
                    if (amountToCall < 0)
                        return BadRequest("Call amount cannot be negative.");
                    if (request.Amount != amountToCall)
                        return BadRequest("Call amount must match the outstanding bet.");
                    if (request.Amount < 0)
                        return BadRequest("Amount must be non-negative.");
                    if (request.Amount > currentPlayer.Stack)
                        return BadRequest("Player does not have enough chips to call.");

                    currentPlayer.Stack -= request.Amount;
                    game.Pot += request.Amount;
                    currentPlayer.CurrentBet += request.Amount;
                    game.Log.Push(new LogEntry { Message = $"Player {currentPlayer.PlayerId} called with {request.Amount}." });
                    break;
                case ActionType.Raise:
                    if (request.Amount < game.MinRaise)
                        return BadRequest("Raise amount is below minimum.");
                    amountToCall = game.CurrentBet - currentPlayer.CurrentBet;
                    if (amountToCall < 0)
                        return BadRequest("Call amount cannot be negative.");
                    int totalContribution = amountToCall + request.Amount;
                    if (totalContribution > currentPlayer.Stack)
                        return BadRequest("Player does not have enough chips to raise.");

                    currentPlayer.Stack -= totalContribution;
                    game.Pot += totalContribution;
                    currentPlayer.CurrentBet += totalContribution;
                    game.CurrentBet = currentPlayer.CurrentBet;
                    game.Log.Push(new LogEntry { Message = $"Player {currentPlayer.PlayerId} raised by {request.Amount}." });
                    break;
                default:
                    return BadRequest("Unsupported action type.");
            }

            int activePlayers = 0;
            foreach (var p in game.Players)
                if (!p.Folded)
                    activePlayers++;

            if (activePlayers <= 1)
            {
                game.Stage = Stage.Showdown;
                ResolveShowdown(game);
                game.AvailableActions.Clear();
                game.Log.Push(new LogEntry { Message = "Hand ended as only one player remains." });
                return Ok(game);
            }

            do
            {
                game.CurrentIndex = (game.CurrentIndex + 1) % game.Players.Count;
            }
            while (game.Players[game.CurrentIndex].Folded);

            int nextIndex = game.CurrentIndex;
            if (nextIndex == game.BettingRoundStartIndex)
            {
                AdvanceStage(game);

                if (game.Stage == Stage.Showdown)
                {
                    ResolveShowdown(game);
                    game.AvailableActions.Clear();
                    return Ok(game);
                }

                game.CurrentIndex = GetNextActivePlayerIndex(game, (game.DealerIndex + 1) % game.Players.Count);
                game.BettingRoundStartIndex = game.CurrentIndex;
            }

            Player nextPlayer = game.Players[game.CurrentIndex];
            game.Log.Push(new LogEntry { Message = $"Player {nextPlayer.PlayerId} is next to play." });
            SetAvailableActions(game, nextPlayer);

            return Ok(game);
        }

        private static int GetNextActivePlayerIndex(Game game, int startIndex)
        {
            int idx = startIndex;
            do
            {
                idx %= game.Players.Count;
                if (!game.Players[idx].Folded)
                {
                    return idx;
                }

                idx++;
            } while (idx % game.Players.Count != startIndex);

            return startIndex;
        }

        private static void SetAvailableActions(Game game, Player player)
        {
            int outstanding = Math.Max(0, game.CurrentBet - player.CurrentBet);

            var actions = new List<PlayerAction>();

            if (outstanding > 0)
            {
                actions.Add(new PlayerAction
                {
                    PlayerId = player.PlayerId,
                    Type = ActionType.Call
                });
            }
            else
            {
                actions.Add(new PlayerAction
                {
                    PlayerId = player.PlayerId,
                    Type = ActionType.Check
                });

                actions.Add(new PlayerAction
                {
                    PlayerId = player.PlayerId,
                    Type = ActionType.Call
                });
            }

            actions.Add(new PlayerAction
            {
                PlayerId = player.PlayerId,
                Type = ActionType.Raise
            });
            actions.Add(new PlayerAction
            {
                PlayerId = player.PlayerId,
                Type = ActionType.Fold
            });

            game.AvailableActions = actions;
        }

        private void AdvanceStage(Game game)
        {
            switch (game.Stage)
            {
                case Stage.Preflop:
                    game.Stage = Stage.Flop;
                    game.Board.Add(game.Deck.Draw());
                    game.Board.Add(game.Deck.Draw());
                    game.Board.Add(game.Deck.Draw());
                    game.Log.Push(new LogEntry { Message = "Flop dealt." });
                    break;
                case Stage.Flop:
                    game.Stage = Stage.Turn;
                    game.Board.Add(game.Deck.Draw());
                    game.Log.Push(new LogEntry { Message = "Turn dealt." });
                    break;
                case Stage.Turn:
                    game.Stage = Stage.River;
                    game.Board.Add(game.Deck.Draw());
                    game.Log.Push(new LogEntry { Message = "River dealt." });
                    break;
                case Stage.River:
                    game.Stage = Stage.Showdown;
                    game.Log.Push(new LogEntry { Message = "Reached showdown." });
                    break;
            }

            if (game.Stage != Stage.Showdown)
            {
                game.CurrentBet = 0;
                foreach (var player in game.Players)
                {
                    player.CurrentBet = 0;
                }
            }
        }

        private void ResolveShowdown(Game game)
        {
            var activePlayers = game.Players.Where(p => !p.Folded).ToList();
            if (!activePlayers.Any())
            {
                return;
            }

            game.Winners.Clear();

            if (activePlayers.Count == 1)
            {
                game.Winners.Add(activePlayers[0].PlayerId);
            }
            else
            {
                HandStrength bestHand = null;

                foreach (var player in activePlayers)
                {
                    var cards = new List<Card>(player.Hole.Count + game.Board.Count);
                    cards.AddRange(player.Hole);
                    cards.AddRange(game.Board);

                    var strength = EvaluateHand(cards);

                    if (bestHand == null || strength.CompareTo(bestHand) > 0)
                    {
                        bestHand = strength;
                        game.Winners.Clear();
                        game.Winners.Add(player.PlayerId);
                    }
                    else if (strength.CompareTo(bestHand) == 0)
                    {
                        game.Winners.Add(player.PlayerId);
                    }
                }
            }

            if (game.Winners.Count == 0)
            {
                return;
            }

            int share = game.Pot / game.Winners.Count;
            int remainder = game.Pot % game.Winners.Count;

            for (int i = 0; i < game.Winners.Count; i++)
            {
                var winner = game.Players.First(p => p.PlayerId == game.Winners[i]);
                int payout = share + (i == 0 ? remainder : 0);
                winner.Stack += payout;
                game.Log.Push(new LogEntry { Message = $"Player {winner.PlayerId} wins {payout}." });
            }

            game.Pot = 0;
        }

        private static HandStrength EvaluateHand(IEnumerable<Card> cards)
        {
            var cardList = cards.ToList();

            var rankGroups = cardList
                .GroupBy(c => c.Rank)
                .Select(g => new { Rank = (int)g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ThenByDescending(g => g.Rank)
                .ToList();

            var suitGroups = cardList
                .GroupBy(c => c.Suit)
                .ToDictionary(g => g.Key, g => g.Select(c => (int)c.Rank).OrderByDescending(r => r).ToList());

            var distinctRanks = cardList
                .Select(c => (int)c.Rank)
                .Distinct()
                .OrderByDescending(r => r)
                .ToList();

            int? straightHigh = GetStraightHigh(distinctRanks);
            int? flushStraightHigh = null;
            List<int> flushCards = null;

            foreach (var suitGroup in suitGroups.Values.Where(sg => sg.Count >= 5))
            {
                var topFlush = suitGroup.Take(5).ToList();
                var flushStraight = GetStraightHigh(suitGroup.Distinct().ToList());

                if (flushCards == null || CompareKickers(topFlush, flushCards) > 0)
                {
                    flushCards = topFlush;
                }

                if (flushStraight.HasValue && (!flushStraightHigh.HasValue || flushStraight.Value > flushStraightHigh.Value))
                {
                    flushStraightHigh = flushStraight;
                }
            }

            // Straight Flush
            if (flushStraightHigh.HasValue)
            {
                return new HandStrength(8, new List<int> { flushStraightHigh.Value });
            }

            // Four of a kind
            var four = rankGroups.FirstOrDefault(g => g.Count == 4);
            if (four != null)
            {
                var kicker = distinctRanks.First(r => r != four.Rank);
                return new HandStrength(7, new List<int> { four.Rank, kicker });
            }

            // Full house
            var trips = rankGroups.Where(g => g.Count == 3).ToList();
            var pairs = rankGroups.Where(g => g.Count == 2).ToList();
            if (trips.Any() && (pairs.Any() || trips.Count > 1))
            {
                int topTrip = trips.First().Rank;
                int topPair = pairs.Any() ? pairs.First().Rank : trips.Skip(1).First().Rank;
                return new HandStrength(6, new List<int> { topTrip, topPair });
            }

            // Flush
            if (flushCards != null)
            {
                return new HandStrength(5, flushCards);
            }

            // Straight
            if (straightHigh.HasValue)
            {
                return new HandStrength(4, new List<int> { straightHigh.Value });
            }

            // Three of a kind
            if (trips.Any())
            {
                var kickers = distinctRanks.Where(r => r != trips.First().Rank).Take(2).ToList();
                var ranks = new List<int> { trips.First().Rank };
                ranks.AddRange(kickers);
                return new HandStrength(3, ranks);
            }

            // Two pair
            if (pairs.Count >= 2)
            {
                var topTwoPairs = pairs.Take(2).Select(p => p.Rank).ToList();
                var kicker = distinctRanks.First(r => !topTwoPairs.Contains(r));
                topTwoPairs.Add(kicker);
                return new HandStrength(2, topTwoPairs);
            }

            // One pair
            if (pairs.Count == 1)
            {
                var kickers = distinctRanks.Where(r => r != pairs[0].Rank).Take(3).ToList();
                var ranks = new List<int> { pairs[0].Rank };
                ranks.AddRange(kickers);
                return new HandStrength(1, ranks);
            }

            // High card
            return new HandStrength(0, distinctRanks.Take(5).ToList());
        }

        private static int CompareKickers(IReadOnlyList<int> first, IReadOnlyList<int> second)
        {
            int length = Math.Min(first.Count, second.Count);
            for (int i = 0; i < length; i++)
            {
                int cmp = first[i].CompareTo(second[i]);
                if (cmp != 0)
                {
                    return cmp;
                }
            }

            return first.Count.CompareTo(second.Count);
        }

        private static int? GetStraightHigh(List<int> ranks)
        {
            var ordered = ranks.Distinct().OrderByDescending(r => r).ToList();
            if (ordered.Contains((int)Rank.Ace))
            {
                ordered.Add(1); // Ace can be low
            }

            int consecutive = 1;
            for (int i = 1; i < ordered.Count; i++)
            {
                if (ordered[i - 1] - ordered[i] == 1)
                {
                    consecutive++;
                    if (consecutive == 5)
                    {
                        return ordered[i - 4];
                    }
                }
                else
                {
                    consecutive = 1;
                }
            }

            return null;
        }
    }
}