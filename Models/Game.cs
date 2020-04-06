using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Mastermind.Api.Models
{
    public class Game
    {
        public Game(Guid userId, GameOptions options)
        {
            UserId = userId;
            Options = options;
            GameCreated = DateTimeOffset.UtcNow;

            var random = new Random();
            if (!options.AllowDuplicates && options.KeyLength > options.MaximumKeyValue)
                throw new UserException($"The length of the secret key ({options.KeyLength}) must be at least the maximum key value ({options.MaximumKeyValue}) if duplicates aren't allowed.");
            SecretKeys = options.AllowDuplicates
                ? Enumerable.Range(0, options.KeyLength).Select(_ => random.Next(1, options.MaximumKeyValue + 1)).ToArray()
                : Enumerable.Range(1, options.MaximumKeyValue).OrderBy(_ => random.Next()).Take(options.KeyLength).ToArray();
        }

        [JsonIgnore]
        public TimeSpan ElapsedTime => DateTimeOffset.UtcNow - GameCreated;
        public double ElapsedSeconds => ElapsedTime.TotalSeconds;
        public DateTimeOffset GameCreated { get; }
        public List<Guess> GivenGuesses { get; } = new List<Guess>();
        public Guid Id { get; } = Guid.NewGuid();

        public Guid UserId { get; }
        public GameOptions Options { get; }
        [JsonIgnore]
        public IReadOnlyList<int> SecretKeys { get; }
        public PlayState PlayState => GivenGuesses.Count >= Options.MaximumNumberOfPossibleGuesses
            ? PlayState.Lost
            : GivenGuesses.Count > 0 && GivenGuesses[^1].Numbers.SequenceEqual(SecretKeys) 
                ? PlayState.Won 
                : PlayState.InProgress;
    }
}
