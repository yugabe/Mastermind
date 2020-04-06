using Mastermind.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mastermind.Api.Data
{
    public class MastermindDbContext : DbContext
    {
        public MastermindDbContext(DbContextOptions<MastermindDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Score> Scores { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Score>(e =>
            {
                e.HasIndex(s => s.Won);
                e.HasIndex(s => new { s.KeyLength, s.PossibleValues, s.MaximumPossibleGuesses, s.GuessesMade, s.DurationInSeconds });
            });

            builder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Username).IsUnique();
            });
        }
    }
}
