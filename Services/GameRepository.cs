using Mastermind.Api.Data;
using Mastermind.Api.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Mastermind.Api.Services
{
    public class GameRepository
    {
        private ConcurrentDictionary<Guid, Game> Games { get; } = new ConcurrentDictionary<Guid, Game>();
        public bool TryAddGame(Game game) => Games.TryAdd(game.Id, game);
        public bool TryRemoveGame(Game game) => Games.TryRemove(game.Id, out _);
        public bool TryGetGame(Guid gameId, [NotNullWhen(true)]out Game game) => Games.TryGetValue(gameId, out game!);
        public IEnumerable<Game> GetGamesForUser(Guid userId) => Games.Values.Where(g => g.UserId == userId).ToList(); 
    }
}