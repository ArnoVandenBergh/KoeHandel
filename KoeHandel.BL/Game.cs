namespace KoeHandel.BL
{
    public abstract class GameAction(Game Game)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Game Game { get; set; } = Game;

        internal abstract void MoveToFinishedState();
        internal static void ValidatePlayerHasEnoughCash(Player player, List<MoneyValues> cash)
        {
            List<MoneyValues> payerBalance = [.. player.Balance];
            foreach (var value in cash)
            {
                int index = payerBalance.IndexOf(value);
                if (index != -1)
                {
                    payerBalance.RemoveAt(index);
                }
                else
                {
                    throw new InvalidOperationException($"Payer does not have enough of {value} to transfer.");
                }
            }
        }

        internal static void RemoveCashFromPlayer(Player player, List<MoneyValues> cash)
        {
            foreach (var value in cash)
            {
                player.Balance.Remove(value);
            }
        }

        internal static int GetCashValue(List<MoneyValues> cash)
        {
            return cash.Sum(value => (int)value);
        }
    }
    public class Game
    {
        public Game(Player firstPlayer, IAnimalDeck deck)
        {
            Players = [firstPlayer];
            Console.WriteLine($"Player \"{firstPlayer.Name}\" has opened a lobby for a new game.");
            Deck = deck;
        }

        private GameState _state = GameState.NotStarted;
        public List<Player> Players { get; set; }
        internal IAnimalDeck? Deck { get; set; }
        internal List<Auction> Auctions { get; set; } = [];
        internal List<Trade> Trades { get; set; } = [];
        internal int Round { get; set; }
        internal Player CurrentPlayer { get; set; } = default!;
        public GameAction? CurrentGameAction { get; set; }
        private int _numberOfDonkeyDrops = 0;

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
            Console.WriteLine($"The game has started with {Players.Count} players.");

            var random = new Random();
            CurrentPlayer = Players[random.Next(0, Players.Count)];
            Console.WriteLine($"Player \"{CurrentPlayer.Name}\" has been selected as the first player.");

            return _state;
        }

        private static MoneyValues GetMoneyValueBasedOnDonkeyDropCount(int count)
        {
            return count switch
            {
                0 => throw new ArgumentOutOfRangeException(nameof(count), "Invalid donkey drop count."),
                1 => MoneyValues.Fifty,
                2 => MoneyValues.Hundred,
                3 => MoneyValues.TwoHundred,
                4 => MoneyValues.FiveHundred,
                _ => throw new ArgumentOutOfRangeException(nameof(count), "Invalid donkey drop count.")
            };
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
            if (!Deck!.Animals.Any())
            {
                throw new InvalidOperationException("No animals left in the deck.");
            }

            var animalCard = Deck.Animals.Dequeue();

            if (animalCard.Animal.Name == AnimalDeck._ezel.Name)
            {
                _numberOfDonkeyDrops++;
                Console.WriteLine($"Player \"{auctioneer.Name}\" has dropped a donkey. Current donkey drop count: {_numberOfDonkeyDrops}.");
                if (_numberOfDonkeyDrops > 4)
                {
                    throw new InvalidOperationException("Cannot drop more than 4 donkeys.");
                }
                foreach (var player in Players)
                {
                    var moneyValue = GetMoneyValueBasedOnDonkeyDropCount(_numberOfDonkeyDrops);
                    player.Balance.Add(moneyValue);
                    Console.WriteLine($"Player \"{player.Name}\" has received {moneyValue} for the donkey drop.");
                }
            }

            var auction = new Auction(auctioneer, animalCard, this);
            Auctions.Add(auction);
            CurrentGameAction = auction;
            Console.WriteLine($"Player \"{auctioneer.Name}\" has started an auction for the {animalCard.Animal.Name}.");
            return auction;
        }

        public Trade StartNewTrade(Player buyer, Player seller, AnimalCard animalCard)
        {
            if (_state != GameState.InProgress)
            {
                throw new InvalidOperationException("The game is not in progress.");
            }
            if (CurrentPlayer != buyer)
            {
                throw new InvalidOperationException($"It's not {buyer.Name}'s turn to start a trade.");
            }
            if (CurrentGameAction != null)
            {
                throw new InvalidOperationException("A game action is already in progress.");
            }
            if (!buyer.AnimalCards.Any(c => c.Animal.Name == animalCard.Animal.Name))
            {
                throw new InvalidOperationException($"Player \"{buyer.Name}\" does not have the animal card {animalCard.Animal.Name}.");
            }
            if (!seller.AnimalCards.Any(c => c.Animal.Name == animalCard.Animal.Name))
            {
                throw new InvalidOperationException($"Player \"{seller.Name}\" does not have the animal card {animalCard.Animal.Name}.");
            }

            var trade = new Trade(buyer, seller, animalCard, this);
            Trades.Add(trade);
            CurrentGameAction = trade;
            Console.WriteLine($"Player \"{buyer.Name}\" has started a trade with {seller.Name} for the {animalCard.Animal.Name}.");
            return trade;
        }

        internal void SortDeck()
        {
            var orderedDeck = Deck!.Animals.OrderBy(c => c.Animal.Name);
            Deck!.Animals = new Queue<AnimalCard>(orderedDeck);
        }

        internal void EndCurrentGameAction()
        {
            if (CurrentGameAction == null)
            {
                throw new InvalidOperationException("No game action is currently in progress.");
            }
            Console.WriteLine($"Ending game action: {CurrentGameAction.GetType().Name}");
            CurrentGameAction.MoveToFinishedState();
            CurrentGameAction = null;
            CurrentPlayer = GetNextPlayer();
        }

        internal Player GetNextPlayer()
        {
            var currentPlayerIndex = Players.IndexOf(CurrentPlayer);
            int nextIndex = (currentPlayerIndex + 1) % Players.Count;
            var nextPlayer = Players[nextIndex];
            return nextPlayer;
        }
    }
}
