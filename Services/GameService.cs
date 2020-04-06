using Mastermind.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mastermind.Api.Services
{
    public class GameService
    {
        private GameRepository GameRepository { get; }
        public ScoreRepository ScoreRepository { get; }
        public UserService UserService { get; }

        public GameService(GameRepository gameRepository, ScoreRepository scoreRepository, UserService userService)
        {
            GameRepository = gameRepository;
            ScoreRepository = scoreRepository;
            UserService = userService;
        }
        public Game AddNewGame(GameOptions options)
        {
            var game = new Game(UserService.GetCurrentUserId(), options);
            if (!GameRepository.TryAddGame(game))
                throw new UserException($"Game with key {game.Id} could not be stored in repository.");
            return game;
        }

        public IEnumerable<Game> GetGames() =>
            GameRepository.GetGamesForUser(UserService.GetCurrentUserId());

        public async Task<IEnumerable<HighScore>> GetHighScores(int entries) =>
            await ScoreRepository.GetHighScoresAsync(entries);

        public Game GetGame(Guid gameId) =>
            GameRepository.TryGetGame(gameId, out var game) && game.UserId == UserService.GetCurrentUserId() 
                ? game
                : throw new UserException($"The game with key {gameId} was not found or was assigned to another user.");

        public async Task<Game> GuessAsync(Guid gameId, IReadOnlyList<int> numbers)
        {
            var game = GetGame(gameId);

            if (game.PlayState != PlayState.InProgress)
                throw new UserException($"The game with key {gameId} is not in progress. The game state is {game.PlayState}.");

            if (numbers?.Count != game.SecretKeys.Count)
                throw new UserException($"The provided number of guessed items ({numbers?.Count}) does not equal the game's secret key length ({game.SecretKeys.Count}).");

            var lessThanOneNumbers = string.Join(", ", numbers.Where(n => n < 1).ToList());
            if (lessThanOneNumbers != "")
                throw new UserException($"The following numbers provided in the guess are lower than the minimum supported (1): {lessThanOneNumbers}.");

            var wrongNumbers = string.Join(", ", numbers.Where(n => n > game.Options.MaximumKeyValue).ToList());
            if (wrongNumbers != "")
                throw new UserException($"The following numbers provided in the guess are higher than the possible maximum for the game ({game.Options.MaximumKeyValue}): {wrongNumbers}.");

            var guessNumberMap = numbers.Select((k, i) => (k, i)).Where(n => n.k != game.SecretKeys[n.i]).GroupBy(n => n.k).ToDictionary(g => g.Key, g => g.Count());

            game.GivenGuesses.Add(new Guess(numbers,
                numbersAtRightPlace: game.SecretKeys.Select((k, i) => (k, i)).Count(n => n.k == numbers[n.i]),
                numbersAtWrongPlace: game.SecretKeys.Select((k, i) => (k, i)).Where(n => n.k != numbers[n.i]).GroupBy(n => n.k).ToDictionary(g => g.Key, g => g.Count())
                    .Sum(n => Math.Min(n.Value, guessNumberMap.TryGetValue(n.Key, out var k) ? k : 0))));

            if (new [] { PlayState.Lost, PlayState.Won }.Contains(game.PlayState))
            {
                await ScoreRepository.AddScoreAsync(game);
                if (!GameRepository.TryRemoveGame(game))
                    throw new UserException($"The finished game ({game.Id}, {game.PlayState}) could not be removed from the repository.");
            }
            return game;
        }
    }
}
