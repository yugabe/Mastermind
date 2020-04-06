using System.Collections.Generic;

namespace Mastermind.Api.Models
{
    public class Guess
    {
        public Guess(IReadOnlyList<int> numbers, int numbersAtRightPlace, int numbersAtWrongPlace)
        {
            Numbers = numbers;
            NumbersAtRightPlace = numbersAtRightPlace;
            NumbersAtWrongPlace = numbersAtWrongPlace;
        }

        public IReadOnlyList<int> Numbers { get; }
        public int NumbersAtRightPlace { get; }
        public int NumbersAtWrongPlace { get; }
    }
}
