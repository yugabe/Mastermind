using System;

namespace Mastermind.Api.Data.Entities
{
    public class Score
    {
        public int Id { get; set; }
        public Guid GameId { get; set; }
        public DateTimeOffset GameStarted { get; set; }
        public int KeyLength { get; set; }
        public double DurationInSeconds { get; set; }
        public Guid PlayerId { get; set; }
        public User Player { get; set; } = null!;
        public int GuessesMade { get; set; }
        public int PossibleValues { get; set; }
        public int MaximumPossibleGuesses { get; set; }
        public bool AllowDuplicates { get; set; }
        public bool Won { get; set; }
    }
}
