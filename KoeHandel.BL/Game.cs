namespace KoeHandel.BL
{
    public class GameAction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
    public class Game
    {
        public Game(Player firstPlayer)
        {
            Players = [firstPlayer];
            Console.WriteLine($"Player \"{firstPlayer.Name}\" has opened a lobby for a new game.");
        }

        private GameState _state = GameState.NotStarted;
        public List<Player> Players { get; set; }
        public AnimalDeck? AnimalDeck { get; set; }
        public List<Auction> Auctions { get; set; } = [];
        public List<Trade> Trades { get; set; } = [];
        public int Round { get; set; }
        public Player CurrentPlayer { get; set; } = default!;
        public GameAction? CurrentGameAction { get; set; }

        public int AddPlayer(Player player)
        {
            if (_state != GameState.NotStarted)
            {
                throw new InvalidOperationException("Cannot add players after the game has started.");
            }

            if (Players.Count >= 4)
            {
                throw new InvalidOperationException("Cannot add more than 4 players.");
            }

            if (Players.Any(p => p.Id == player.Id))
            {
                throw new InvalidOperationException($"Player \"{player.Name}\" is already part of this game.");
            }

            Players.Add(player);
            Console.WriteLine($"Player \"{player.Name}\" has joined the game.");
            return Players.Count;
        }

        public GameState StartGame()
        {
            if (_state != GameState.NotStarted)
            {
                throw new InvalidOperationException("The game is already in progress.");
            }

            if (Players.Count < 3)
            {
                throw new InvalidOperationException("Cannot start a game with less than 3 players.");
            }

            _state = GameState.InProgress;
            AnimalDeck = new AnimalDeck();
            Console.WriteLine($"The game has started with {Players.Count} players.");

            var random = new Random();
            CurrentPlayer = Players[random.Next(0, Players.Count)];
            Console.WriteLine($"Player \"{CurrentPlayer.Name}\" has been selected as the first player.");

            return _state;
        }

        public Auction StartNewAuction(Player auctioneer)
        {
            if (_state != GameState.InProgress)
            {
                throw new InvalidOperationException("The game is not in progress.");
            }
            if (CurrentPlayer != auctioneer)
            {
                throw new InvalidOperationException($"It's not {auctioneer.Name}'s turn to start an auction.");
            }
            if (CurrentGameAction != null)
            {
                throw new InvalidOperationException("A game action is already in progress.");
            }
            if (!AnimalDeck!.Animals.Any())
            {
                throw new InvalidOperationException("No animals left in the deck.");
            }

            var animalCard = AnimalDeck.Animals.Dequeue();
            var auction = new Auction(auctioneer, animalCard, this);
            Auctions.Add(auction);
            CurrentGameAction = auction;
            Console.WriteLine($"Player \"{auctioneer.Name}\" has started an auction for the {animalCard.Animal.Name}.");
            return auction;
        }

        public Player GetNextPlayer()
        {
            var currentPlayerIndex = Players.IndexOf(CurrentPlayer);
            int nextIndex = (currentPlayerIndex + 1) % Players.Count;
            var nextPlayer = Players[nextIndex];
            return nextPlayer;
        }
    }
}
