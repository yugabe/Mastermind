using Mastermind.Api.Models;
using Mastermind.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mastermind.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class GamesController : ControllerBase
    {
        public GamesController(GameService gameService) => GameService = gameService;

        public GameService GameService { get; }

        [HttpGet("api/games")]
        public IEnumerable<Game> GetGames() =>
            GameService.GetGames();

        [HttpGet("api/games/{gameId}")]
        public Game GetGame(Guid gameId) =>
            GameService.GetGame(gameId);

        [HttpPost("api/games")]
        public Game AddNewGame([FromBody]GameOptions options) =>
            GameService.AddNewGame(options);

        [HttpPost("api/games/{gameId}/guess")]
        public async Task<ActionResult<Game>> PostGuessAsync([FromRoute]Guid gameId, [FromBody] IReadOnlyList<int> numbers) => 
            await GameService.GuessAsync(gameId, numbers);

        [HttpGet("api/highscores")]
        public async Task<IEnumerable<HighScore>> GetHighScores(int entries = 20) =>
            await GameService.GetHighScores(entries);
    }
}