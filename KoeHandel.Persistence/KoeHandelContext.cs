using KoeHandel.Domain;
using Microsoft.EntityFrameworkCore;

namespace KoeHandel.Persistence
{
    public class KoeHandelContext(DbContextOptions<KoeHandelContext> options) : DbContext(options)
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<AnimalCard> AnimalCards { get; set; }
    }

    public class Game
    {
        public int Id { get; set; }
        public GameState State { get; set; }
        public required List<Player> Players { get; set; }
        public Player CurrentPlayer { get; set; } = default!;
    }

    public class Player
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        //public required List<MoneyValues> Balance { get; set; }
        //public required List<AnimalCard> AnimalCards { get; set; }
        //public int Score { get; set; }
    }

    public class AnimalCard
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int Value { get; set; }
    }
}
