namespace KoeHandel.BL.Tests
{
    [TestClass]
    public class GameTests
    {
        private readonly Player _player1;
        private readonly Player _player2;
        private readonly Player _player3;
        private readonly Game _game;

        public GameTests()
        {
            _player1 = new Player("Player 1");
            _player2 = new Player("Player 2");
            _player3 = new Player("Player 3");
            _game = new Game(_player1);
            _game.AddPlayer(_player2);
            _game.AddPlayer(_player3);
        }


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
            var game = new Game(_player1);
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
            var notCurrentPlayer = _game.GetNextPlayer();
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
            var auction = _game.StartNewAuction(_game.CurrentPlayer);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => _game.StartNewAuction(_game.CurrentPlayer));
            Assert.AreEqual("A game action is already in progress.", exception.Message);
        }

        [TestMethod]
        public void StartNewAuction_EmptyDeck_InvalidOperation()
        {
            // Arrange
            _game.StartGame();

            // TODO: as soon the logic is in place to empty the deck, this test should be updated

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => _game.StartNewAuction(_game.CurrentPlayer));
            Assert.AreEqual("No animals left in the deck.", exception.Message);
        }

        [TestMethod]
        public void StartNewAuction_HappyFlow_NoException()
        {
            // Arrange
            _game.StartGame();
            var animalCard = _game.AnimalDeck.Animals.Peek();

            // Act
            var auction = _game.StartNewAuction(_game.CurrentPlayer);

            // Act & Assert
            Assert.AreEqual(_game.CurrentPlayer, auction.Auctioneer);
            Assert.AreEqual(auction, _game.CurrentGameAction);
            Assert.AreEqual(animalCard, auction.AnimalCard);
        }

        [TestMethod]
        public void PlaceBid_NotCurrentBidder_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            var bidder = _game.GetNextPlayer();
            var notCurrentBidder = _game.Players.First(p => p.Id != bidder.Id && p.Id != auction.Auctioneer.Id);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.PlaceBid(notCurrentBidder, 10));
            Assert.AreEqual($"It's not {notCurrentBidder.Name}'s turn to bid.", exception.Message);
        }

        [TestMethod]
        public void PlaceBid_Auctioneer_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            var auctioneer = auction.Auctioneer;

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.PlaceBid(auctioneer, 10));
            Assert.AreEqual($"The auctioneer (\"{auctioneer.Name}\") can't place a bid.", exception.Message);
        }

        [TestMethod]
        public void PlaceBid_AuctionIsNoLongerCurrentGameAction_InvalidOperation()
        {
            //TODO: as soon the logic is in place to start a second gameaction, this test should be updated

            // Arrange
            _game.StartGame();
            var oldAuction = _game.StartNewAuction(_game.CurrentPlayer);
            var newAuction = _game.StartNewAuction(_game.CurrentPlayer);


            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => oldAuction.PlaceBid(_game.GetNextPlayer(), 10));
            Assert.AreEqual($"This auction is not currently active.", exception.Message);
        }

        [TestMethod]
        public void PlaceBid_BidIsToLow_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 20);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.PlaceBid(auction.CurrentBidder, 20));
            Assert.AreEqual($"Bid must be higher than the current bid of {auction.Bid}.", exception.Message);
        }

        [TestMethod]
        public void PlaceBid_HappyFlow_NoException()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            var auctioneer = auction.Auctioneer;

            // Act & Assert
            Assert.AreNotEqual(auctioneer, auction.CurrentBidder);
            auction.PlaceBid(auction.CurrentBidder, 10);
            Assert.AreNotEqual(auctioneer, auction.CurrentBidder);
            auction.PlaceBid(auction.CurrentBidder, 20);
            Assert.AreNotEqual(auctioneer, auction.CurrentBidder);
            auction.PlaceBid(auction.CurrentBidder, 30);
            Assert.AreNotEqual(auctioneer, auction.CurrentBidder);
            auction.PlaceBid(auction.CurrentBidder, 40);
        }

        [TestMethod]
        public void PlaceBid_LastRemainingBidderWhileBidIsLargerThanZero_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.SkipBid(auction.CurrentBidder);
            auction.PlaceBid(auction.CurrentBidder, 10);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.SkipBid(auction.CurrentBidder));
            Assert.AreEqual("The auction has passed the bidding phase", exception.Message);
        }

        [TestMethod]
        public void SkipBid_NotCurrentBidder_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            var bidder = _game.GetNextPlayer();
            var notCurrentBidder = _game.Players.First(p => p.Id != bidder.Id && p.Id != auction.Auctioneer.Id);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.SkipBid(notCurrentBidder));
            Assert.AreEqual($"It's not {notCurrentBidder.Name}'s turn to bid.", exception.Message);
        }

        [TestMethod]
        public void SkipBid_Auctioneer_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            var auctioneer = auction.Auctioneer;

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.SkipBid(auctioneer));
            Assert.AreEqual($"The auctioneer (\"{auctioneer.Name}\") can't place a bid.", exception.Message);
        }

        [TestMethod]
        public void SkipBid_AuctionIsNoLongerCurrentGameAction_InvalidOperation()
        {
            //TODO: as soon the logic is in place to start a second gameaction, this test should be updated

            // Arrange
            _game.StartGame();
            var oldAuction = _game.StartNewAuction(_game.CurrentPlayer);
            var newAuction = _game.StartNewAuction(_game.CurrentPlayer);


            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => oldAuction.SkipBid(_game.GetNextPlayer()));
            Assert.AreEqual($"This auction is not currently active.", exception.Message);
        }

        [TestMethod]
        public void SkipBid_LastRemainingBidderWhileBidIsLargerThanZero_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.SkipBid(auction.CurrentBidder);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.SkipBid(auction.CurrentBidder));
            Assert.AreEqual("The auction has passed the bidding phase", exception.Message);
        }

        [TestMethod]
        public void SkipBid_AllBiddersSkip_NoException()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.SkipBid(auction.CurrentBidder);
            auction.SkipBid(auction.CurrentBidder);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.SkipBid(auction.CurrentBidder));
            Assert.AreEqual("The auction has passed the bidding phase", exception.Message);
        }

        [TestMethod]
        public void BuyOverAuction_StillBidding_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.MoveToMoneyTransferPhase(auction.Auctioneer, true));
            Assert.AreEqual("The auction is still in progress.", exception.Message);
        }

        [TestMethod]
        public void BuyOverAuction_MovedPassedBuyoverPhase_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.SkipBid(auction.CurrentBidder);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, false);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.MoveToMoneyTransferPhase(auction.Auctioneer, true));
            Assert.AreEqual($"The auction is passed the buyover phase.", exception.Message);
        }

        [TestMethod]
        public void BuyOverAuction_PlayerIsNotAuctioneer_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.SkipBid(auction.CurrentBidder);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.MoveToMoneyTransferPhase(auction.LastBidder!, true));
            Assert.AreEqual($"The auctioneer (\"{auction.Auctioneer.Name}\") must be the one to move to the money transfer phase.", exception.Message);
        }

        [TestMethod]
        public void BuyOverAuction_AuctioneerNoBuyOver_NoException()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.SkipBid(auction.CurrentBidder);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, false);

            Assert.AreEqual(AuctionState.MoneyTransferPhase, auction.AuctionState);
            Assert.IsFalse(auction.DidActioneerBuyOver);
        }

        [TestMethod]
        public void BuyOverAuction_AuctioneerBuyOver_NoException()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.SkipBid(auction.CurrentBidder);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, true);

            Assert.AreEqual(AuctionState.MoneyTransferPhase, auction.AuctionState);
            Assert.IsTrue(auction.DidActioneerBuyOver);
        }
    }
}