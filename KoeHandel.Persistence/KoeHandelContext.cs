using KoeHandel.Domain;
using Microsoft.EntityFrameworkCore;

namespace KoeHandel.Persistence
{
    public class KoeHandelContext(DbContextOptions<KoeHandelContext> options) : DbContext(options)
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<AnimalCard> AnimalCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Players)
                .WithOne()
                .HasForeignKey(p => p.GameId)
                .IsRequired(true);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.CurrentPlayer)
                .WithOne()
                .HasForeignKey<Game>(g => g.CurrentPlayerId);

            base.OnModelCreating(modelBuilder);
        }
    }

    public class Game
    {
        public int Id { get; set; }
        public GameState State { get; set; }
        public required List<Player> Players { get; set; }
        public Player CurrentPlayer { get; set; } = default!;
        public Guid CurrentPlayerId { get; set; }
    }

    public class Player
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public int GameId { get; set; }
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
