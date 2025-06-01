using KoeHandel.BL.Tests.Models;

namespace KoeHandel.BL.Tests
{
    [TestClass]
    public class GameTests() : BaseTests()
    {
        [TestMethod]
        public void AddPlayer_AlreadyPartOfTheGame_InvalidOperation()
        {
            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => _game.AddPlayer(_player1));
            Assert.AreEqual($"Player \"{_player1.Name}\" is already part of this game.", exception.Message);
        }

        [TestMethod]
        public void AddPlayer_GameAlreadyInProgress_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var player4 = new Player("Player 4");

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => _game.AddPlayer(player4));
            Assert.AreEqual("Cannot add players after the game has started.", exception.Message);
        }

        [TestMethod]
        public void AddPlayer_GameDidntStartYet_NoException()
        {
            // Arrange
            var player4 = new Player("Player 4");

            // Act
            var playerCount = _game.AddPlayer(player4);

            // Assert
            Assert.AreEqual(4, playerCount);
        }

        [TestMethod]
        public void StartGame_NotEnoughPlayers_InvalidOperation()
        {
            // Arrange
            var game = new Game(_player1, new TestAnimalDeck());
            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => game.StartGame());
            Assert.AreEqual("Cannot start a game with less than 3 players.", exception.Message);
        }

        [TestMethod]
        public void StartGame_EnoughPlayers_NoException()
        {
            // Act
            var gameState = _game.StartGame();

            // Assert
            Assert.AreEqual(GameState.InProgress, gameState);
        }

        [TestMethod]
        public void StartNewAuction_NotCurrentPlayer_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var notCurrentPlayer = _game.Players.First(player => player.Id != _game.CurrentPlayer.Id);
            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => _game.StartNewAuction(notCurrentPlayer));
            Assert.AreEqual($"It's not {notCurrentPlayer.Name}'s turn to start an auction.", exception.Message);
        }

        [TestMethod]
        public void StartNewAuction_GameDidNotStartYet_InvalidOperation()
        {
            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => _game.StartNewAuction(_player3));
            Assert.AreEqual("The game is not in progress.", exception.Message);
        }

        [TestMethod]
        public void StartNewAuction_ActionAlreadyInProgress_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            _game.StartNewAuction(_game.CurrentPlayer);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => _game.StartNewAuction(_game.CurrentPlayer));
            Assert.AreEqual("A game action is already in progress.", exception.Message);
        }

        [TestMethod]
        public void StartNewAuction_EmptyDeck_InvalidOperation()
        {
            // Arrange
            _game.StartGame();

            int count = _game.Deck!.Animals.Count;
            for (int i = 0; i < count; i++)
            {
                var auction = _game.StartNewAuction(_game.CurrentPlayer);
                auction.SkipBid(auction.CurrentBidder);
                auction.SkipBid(auction.CurrentBidder);
            }

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => _game.StartNewAuction(_game.CurrentPlayer));
            Assert.AreEqual("No animals left in the deck.", exception.Message);
        }

        [TestMethod]
        public void StartNewAuction_HappyFlow_NoException()
        {
            // Arrange
            _game.StartGame();
            var animalCard = _game.Deck!.Animals.Peek();

            // Act
            var auction = _game.StartNewAuction(_game.CurrentPlayer);

            // Act & Assert
            Assert.AreEqual(_game.CurrentPlayer, auction.Auctioneer);
            Assert.AreEqual(auction, _game.CurrentGameAction);
            Assert.AreEqual(animalCard, auction.AnimalCard);
        }

        [TestMethod]
        public void StartNewTrade_DonkeyDropNr1_AllPlayersGain50()
        {
            // Arrange
            _game.StartGame();
            _game.SortDeck();

            // Act
            _game.StartNewAuction(_game.CurrentPlayer);

            // Act & Assert
            Assert.AreEqual(2, _player1.Balance.Count(m => m == MoneyValues.Fifty));
            Assert.AreEqual(2, _player2.Balance.Count(m => m == MoneyValues.Fifty));
            Assert.AreEqual(2, _player3.Balance.Count(m => m == MoneyValues.Fifty));
        }

        [TestMethod]
        public void StartNewTrade_DonkeyDropNr2_AllPlayersGain100()
        {
            // Arrange
            _game.StartGame();
            _game.SortDeck();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.SkipBid(auction.CurrentBidder);
            auction.SkipBid(auction.CurrentBidder);

            // Act
            _game.StartNewAuction(_game.CurrentPlayer);

            // Act & Assert
            Assert.AreEqual(1, _player1.Balance.Count(m => m == MoneyValues.Hundred));
            Assert.AreEqual(1, _player2.Balance.Count(m => m == MoneyValues.Hundred));
            Assert.AreEqual(1, _player3.Balance.Count(m => m == MoneyValues.Hundred));
        }

        [TestMethod]
        public void StartNewTrade_DonkeyDropNr3_AllPlayersGain200()
        {
            // Arrange
            _game.StartGame();
            _game.SortDeck();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.SkipBid(auction.CurrentBidder);
            auction.SkipBid(auction.CurrentBidder);
            var secondAuction = _game.StartNewAuction(_game.CurrentPlayer);
            secondAuction.SkipBid(secondAuction.CurrentBidder);
            secondAuction.SkipBid(secondAuction.CurrentBidder);

            // Act
            _game.StartNewAuction(_game.CurrentPlayer);

            // Act & Assert
            Assert.AreEqual(1, _player1.Balance.Count(m => m == MoneyValues.TwoHundred));
            Assert.AreEqual(1, _player2.Balance.Count(m => m == MoneyValues.TwoHundred));
            Assert.AreEqual(1, _player3.Balance.Count(m => m == MoneyValues.TwoHundred));
        }

        [TestMethod]
        public void StartNewTrade_DonkeyDropNr4_AllPlayersGain500()
        {
            // Arrange
            _game.StartGame();
            _game.SortDeck();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.SkipBid(auction.CurrentBidder);
            auction.SkipBid(auction.CurrentBidder);
            var secondAuction = _game.StartNewAuction(_game.CurrentPlayer);
            secondAuction.SkipBid(secondAuction.CurrentBidder);
            secondAuction.SkipBid(secondAuction.CurrentBidder);
            var thirdAuction = _game.StartNewAuction(_game.CurrentPlayer);
            thirdAuction.SkipBid(thirdAuction.CurrentBidder);
            thirdAuction.SkipBid(thirdAuction.CurrentBidder);

            // Act
            _game.StartNewAuction(_game.CurrentPlayer);

            // Act & Assert
            Assert.AreEqual(1, _player1.Balance.Count(m => m == MoneyValues.FiveHundred));
            Assert.AreEqual(1, _player2.Balance.Count(m => m == MoneyValues.FiveHundred));
            Assert.AreEqual(1, _player3.Balance.Count(m => m == MoneyValues.FiveHundred));
        }

        [TestMethod]
        public void StartNewTrade_NotCurrentPlayer_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var notCurrentPlayer = _game.Players.First(player => player.Id != _game.CurrentPlayer.Id);
            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => _game.StartNewTrade(notCurrentPlayer, _game.CurrentPlayer, _game.Deck!.Animals.Peek()));
            Assert.AreEqual($"It's not {notCurrentPlayer.Name}'s turn to start a trade.", exception.Message);
        }

        [TestMethod]
        public void StartNewTrade_GameDidNotStartYet_InvalidOperation()
        {
            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() =>
                _game.StartNewTrade(_player1, _player2, new AnimalCard(new Animal("Schaap", 1))));
            Assert.AreEqual("The game is not in progress.", exception.Message);
        }

        [TestMethod]
        public void StartNewTrade_ActionAlreadyInProgress_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var animalCard = _game.Deck!.Animals.Peek();
            _game.StartNewAuction(_game.CurrentPlayer);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() =>
                _game.StartNewTrade(_game.CurrentPlayer, _game.Players.First(p => p.Id != _game.CurrentPlayer.Id), animalCard));
            Assert.AreEqual("A game action is already in progress.", exception.Message);
        }

        [TestMethod]
        public void StartNewTrade_InitiatorDoesntHaveTheAnimalCard_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var animalCard = _game.Deck!.Animals.Peek();
            var initiator = _game.CurrentPlayer;
            var responder = _game.Players.First(p => p.Id != initiator.Id);
            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() =>
                _game.StartNewTrade(initiator, responder, animalCard));
            Assert.AreEqual($"Player \"{initiator.Name}\" does not have the animal card {animalCard.Animal.Name}.", exception.Message);
        }

        [TestMethod]
        public void StartNewTrade_ResponderDoesntHaveTheAnimalCard_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var animalCard = _game.Deck!.Animals.Peek();
            var initiator = _game.CurrentPlayer;

            // initiator gets animal card through auction
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.SkipBid(auction.CurrentBidder);
            auction.SkipBid(auction.CurrentBidder);

            // initiator places a bid and gets card
            var secondAuction = _game.StartNewAuction(_game.CurrentPlayer);
            secondAuction.SkipBid(secondAuction.CurrentBidder);
            secondAuction.PlaceBid(initiator, 10);
            secondAuction.MoveToMoneyTransferPhase(secondAuction.Auctioneer, false);
            secondAuction.PerformAuctionTransfer(initiator, secondAuction.Auctioneer, [MoneyValues.Ten]);

            // initiator places a bid and gets card
            var thirdAuction = _game.StartNewAuction(_game.CurrentPlayer);
            thirdAuction.PlaceBid(initiator, 10);
            thirdAuction.SkipBid(thirdAuction.CurrentBidder);
            thirdAuction.MoveToMoneyTransferPhase(thirdAuction.Auctioneer, false);
            thirdAuction.PerformAuctionTransfer(initiator, thirdAuction.Auctioneer, [MoneyValues.Ten]);

            var responder = _game.Players.First(p => p.Id != initiator.Id);
            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() =>
                _game.StartNewTrade(initiator, responder, animalCard));
            Assert.AreEqual($"Player \"{responder.Name}\" does not have the animal card {animalCard.Animal.Name}.", exception.Message);
        }

        [TestMethod]
        public void StartNewTrade_HappyFlow_NoException()
        {
            // Arrange
            _game.StartGame();
            var animalCard = _game.Deck!.Animals.Peek();
            var initiator = _game.CurrentPlayer;
            var responder = _game.Players.First(p => p.Id != initiator.Id);

            // Initiator gets animal card through auction
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.SkipBid(auction.CurrentBidder);
            auction.SkipBid(auction.CurrentBidder);

            while (!responder.AnimalCards.Any(c => c.Animal.Name == animalCard.Animal.Name))
            {
                // Ensure responder has the animal card
                var secondAuction = _game.StartNewAuction(_game.CurrentPlayer);
                secondAuction.SkipBid(secondAuction.CurrentBidder);
                secondAuction.SkipBid(secondAuction.CurrentBidder);
            }

            while (_game.CurrentPlayer != initiator)
            {
                var thirdAuction = _game.StartNewAuction(_game.CurrentPlayer);
                thirdAuction.SkipBid(thirdAuction.CurrentBidder);
                thirdAuction.SkipBid(thirdAuction.CurrentBidder);
            }

            // Act
            var trade = _game.StartNewTrade(initiator, responder, animalCard);

            // Assert
            Assert.AreEqual(initiator, trade.Initiator);
            Assert.AreEqual(responder, trade.Responder);
            Assert.AreEqual(animalCard, trade.AnimalCard);
            Assert.IsNull(trade.Offer);
        }

        private void MakePlayerWinCard(Player player)
        {
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            if (player.Id == _game.CurrentPlayer.Id)
            {
                auction.SkipBid(auction.CurrentBidder);
                auction.SkipBid(auction.CurrentBidder);
                return;
            }
            while (auction.CurrentBidder.Id != player.Id)
            {
                auction.SkipBid(auction.CurrentBidder);
            }
            auction.PlaceBid(player, 10);
            while (auction.CurrentBidder.Id != player.Id)
            {
                auction.SkipBid(auction.CurrentBidder);
            }
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, false);
            var lowestValueOfPlayer = player.Balance.Where(m => m != MoneyValues.Zero).Min();
            auction.PerformAuctionTransfer(player, auction.Auctioneer, [lowestValueOfPlayer]);
        }

        private void MakePlayerWinQuartet(Player player)
        {
            MakePlayerWinCard(player);
            MakePlayerWinCard(player);
            MakePlayerWinCard(player);
            MakePlayerWinCard(player);
        }

        [TestMethod]
        public void EndGame_NoAnimalsLeftInDeck_AllPlayersHaveQuartets_GameEnds()
        {
            // Arrange & Act
            _game.StartGame();
            MakePlayerWinQuartet(_player1);
            MakePlayerWinQuartet(_player2);
            MakePlayerWinQuartet(_player3);
            MakePlayerWinQuartet(_player1);
            MakePlayerWinQuartet(_player2);
            MakePlayerWinQuartet(_player3);
            MakePlayerWinQuartet(_player1);
            MakePlayerWinQuartet(_player2);
            MakePlayerWinQuartet(_player3);
            MakePlayerWinQuartet(_player1);

            // Assert
            Assert.AreEqual(GameState.Finished, _game.State);
            Assert.AreEqual(5600, _player1.Score);
            Assert.AreEqual(3780, _player2.Score);
            Assert.AreEqual(3570, _player3.Score);
        }
    }
}