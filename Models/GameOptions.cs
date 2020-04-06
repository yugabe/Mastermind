using System.ComponentModel.DataAnnotations;

namespace Mastermind.Api.Models
{
    public class GameOptions
    {
        [Range(2, 10)]
        public int MaximumKeyValue { get; set; }
        [Range(4, 20)]
        public int MaximumNumberOfPossibleGuesses { get; set; }
        [Range(2, 8)]
        public int KeyLength { get; set; }
        public bool AllowDuplicates { get; set; }
    }
}
