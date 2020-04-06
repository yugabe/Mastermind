using System;
using System.Collections.Generic;

namespace Mastermind.Api.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public ICollection<Score> UserScores { get; set; } = null!;
    }
}
