using Mastermind.Api.Data;
using Mastermind.Api.Data.Entities;
using Mastermind.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mastermind.Api.Services
{
    public class ScoreRepository
    {
        public ScoreRepository(MastermindDbContext dbContext) => DbContext = dbContext;

        public MastermindDbContext DbContext { get; }

        public async Task AddScoreAsync(Game game)
        {
            DbContext.Add(new Score
            {
                DurationInSeconds = game.ElapsedTime.TotalSeconds,
                GameStarted = game.GameCreated,
                GuessesMade = game.GivenGuesses.Count,
                MaximumPossibleGuesses = game.Options.MaximumNumberOfPossibleGuesses,
                PlayerId = game.UserId,
                GameId = game.Id,
                PossibleValues = game.Options.MaximumKeyValue - 1,
                KeyLength = game.Options.KeyLength,
                Won = game.PlayState == PlayState.Won,
                AllowDuplicates = game.Options.AllowDuplicates
            });
            await DbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<HighScore>> GetHighScoresAsync(int entries) =>
            await DbContext.Scores.Where(s => s.Won).OrderByDescending(s => 10 * s.KeyLength * s.PossibleValues * (-5 * s.MaximumPossibleGuesses) * s.GuessesMade * s.DurationInSeconds)
                .Take(entries)
                .Select(s => new HighScore(s.Player.Username, s.DurationInSeconds, s.GameStarted.Date, s.GuessesMade, s.PossibleValues, s.MaximumPossibleGuesses, s.AllowDuplicates))
                .ToListAsync();
    }
}
