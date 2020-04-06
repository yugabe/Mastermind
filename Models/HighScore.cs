using System;

namespace Mastermind.Api.Models
{
    public class HighScore
    {
        public HighScore(string user, double playTimeInSeconds, DateTime date, int guessesMade, int possibleValues, int maximumPossibleGuesses, bool allowDuplicates)
        {
            User = user;
            PlayTimeInSeconds = playTimeInSeconds;
            Date = date;
            GuessesMade = guessesMade;
            PossibleValues = possibleValues;
            MaximumPossibleGuesses = maximumPossibleGuesses;
            AllowDuplicates = allowDuplicates;
        }

        public string User { get; }
        public double PlayTimeInSeconds { get; }
        public DateTime Date { get; }
        public int GuessesMade { get; }
        public int PossibleValues { get; }
        public int MaximumPossibleGuesses { get; }
        public bool AllowDuplicates { get; }
    }
}
