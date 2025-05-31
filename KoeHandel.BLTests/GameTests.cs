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

            int count = _game.AnimalDeck.Animals.Count;
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
            var notCurrentBidder = _game.Players.First(p => p.Id != auction.CurrentBidder.Id && p.Id != auction.Auctioneer.Id);

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
            // Arrange
            _game.StartGame();
            var oldAuction = _game.StartNewAuction(_game.CurrentPlayer);
            oldAuction.SkipBid(oldAuction.CurrentBidder);
            oldAuction.SkipBid(oldAuction.CurrentBidder);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => oldAuction.PlaceBid(oldAuction.CurrentBidder, 10));
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
            var notCurrentBidder = _game.Players.First(p => p.Id != auction.CurrentBidder.Id && p.Id != auction.Auctioneer.Id);

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
            // Arrange
            _game.StartGame();
            var oldAuction = _game.StartNewAuction(_game.CurrentPlayer);
            oldAuction.SkipBid(oldAuction.CurrentBidder);
            oldAuction.SkipBid(oldAuction.CurrentBidder);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => oldAuction.SkipBid(oldAuction.CurrentBidder));
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
        public void SkipBid_AllBiddersSkip_AuctionFinished()
        {
            // Arrange
            _game.StartGame();
            var auctioneer = _game.CurrentPlayer;
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.SkipBid(auction.CurrentBidder);

            // Act
            auction.SkipBid(auction.CurrentBidder);

            // Assert
            Assert.AreEqual(AuctionState.Finished, auction.AuctionState);
            Assert.AreEqual(0, auction.Bid);
            Assert.IsNull(auction.LastBidder);
            Assert.IsTrue(auctioneer.AnimalCards.Any(c => c == auction.AnimalCard));
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

        [TestMethod]
        public void AuctionTransfer_PayerIsNotLastBidder_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.PlaceBid(auction.CurrentBidder, 30);
            auction.SkipBid(auction.CurrentBidder);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, true);
            var notLastBidder = _game.Players.First(p => p.Id != auction.LastBidder!.Id && p.Id != auction.Auctioneer.Id);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.PerformAuctionTransfer(notLastBidder, auction.Auctioneer, [MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten]));
            Assert.AreEqual($"Player \"{notLastBidder.Name}\" is not the payer.", exception.Message);
        }

        [TestMethod]
        public void AuctionTransfer_PayeeIsNotLastBidder_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.PlaceBid(auction.CurrentBidder, 30);
            auction.SkipBid(auction.CurrentBidder);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, true);
            var notLastBidder = _game.Players.First(p => p.Id != auction.LastBidder!.Id && p.Id != auction.Auctioneer.Id);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.PerformAuctionTransfer(auction.Auctioneer, notLastBidder, [MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten]));
            Assert.AreEqual($"Player \"{notLastBidder.Name}\" is not the payee.", exception.Message);
        }

        [TestMethod]
        public void AuctionTransfer_PayerIsNotAuctioneerAndPayeeIsNotLastBidder_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.PlaceBid(auction.CurrentBidder, 30);
            auction.SkipBid(auction.CurrentBidder);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, false);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.PerformAuctionTransfer(auction.Auctioneer, auction.LastBidder!, [MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten]));
            Assert.AreEqual($"Payer and Payee are reversed.", exception.Message);
        }

        [TestMethod]
        public void AuctionTransfer_PayeeIsNotAuctioneerAndPayerIsNotLastBidder_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.PlaceBid(auction.CurrentBidder, 30);
            auction.SkipBid(auction.CurrentBidder);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, true);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.PerformAuctionTransfer(auction.LastBidder!, auction.Auctioneer, [MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten]));
            Assert.AreEqual($"Payer and Payee are reversed.", exception.Message);
        }

        [TestMethod]
        public void AuctionTransfer_NotInMoneyTransferPhase_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.PerformAuctionTransfer(auction.LastBidder!, auction.Auctioneer, [MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten]));
            Assert.AreEqual($"The auction is not in the money transfer phase.", exception.Message);
        }

        [TestMethod]
        public void AuctionTransfer_PayerIsSamePlayerAsPayee_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.PlaceBid(auction.CurrentBidder, 30);
            auction.SkipBid(auction.CurrentBidder);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, true);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.PerformAuctionTransfer(auction.Auctioneer, auction.Auctioneer, [MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten]));
            Assert.AreEqual($"Payer and Payee cannot be the same player.", exception.Message);
        }

        [TestMethod]
        public void AuctionTransfer_CashIsLowerThanBid_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.PlaceBid(auction.CurrentBidder, 40);
            auction.SkipBid(auction.CurrentBidder);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, true);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.PerformAuctionTransfer(auction.Auctioneer, auction.LastBidder!, [MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten]));
            Assert.AreEqual($"Total cash ({30}) must be at least equal to the bid ({40}).", exception.Message);
        }

        [TestMethod]
        public void AuctionTransfer_BalanceIsToLittle_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.PlaceBid(auction.CurrentBidder, 100);
            auction.SkipBid(auction.CurrentBidder);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, true);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => auction.PerformAuctionTransfer(auction.Auctioneer, auction.LastBidder!, [MoneyValues.Fifty, MoneyValues.Fifty]));
            Assert.AreEqual($"Payer does not have enough of Fifty to transfer.", exception.Message);
            Assert.IsTrue(auction.Auctioneer.Balance.Contains(MoneyValues.Fifty));
        }

        [TestMethod]
        public void AuctionTransfer_HappyFlow_MoneyAndCardTranferHappened()
        {
            // Arrange
            _game.StartGame();
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.PlaceBid(auction.CurrentBidder, 10);
            auction.PlaceBid(auction.CurrentBidder, 20);
            auction.SkipBid(auction.CurrentBidder);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, true);
            List<MoneyValues> expectedPayerBalance = [MoneyValues.Zero, MoneyValues.Zero, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Fifty];
            List<MoneyValues> expectedPayeeBalance = [MoneyValues.Zero, MoneyValues.Zero, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Fifty];

            // Act
            auction.PerformAuctionTransfer(auction.Auctioneer, auction.LastBidder!, [MoneyValues.Ten, MoneyValues.Ten]);

            // Assert
            CollectionAssert.AreEquivalent(expectedPayerBalance, auction.Auctioneer.Balance);
            CollectionAssert.AreEquivalent(expectedPayeeBalance, auction.LastBidder!.Balance);
            Assert.AreEqual(AuctionState.Finished, auction.AuctionState);
            Assert.IsTrue(auction.Auctioneer.AnimalCards.Contains(auction.AnimalCard));
            Assert.IsNull(_game.CurrentGameAction);
        }

        [TestMethod]
        public void StartNewTrade_NotCurrentPlayer_InvalidOperation()
        {
            // Arrange
            _game.StartGame();
            var notCurrentPlayer = _game.Players.First(player => player.Id != _game.CurrentPlayer.Id);
            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => _game.StartNewTrade(notCurrentPlayer, _game.CurrentPlayer, _game.AnimalDeck!.Animals.Peek()));
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
            var animalCard = _game.AnimalDeck!.Animals.Peek();
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
            var animalCard = _game.AnimalDeck!.Animals.Peek();
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
            var animalCard = _game.AnimalDeck!.Animals.Peek();
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
            _game.SortDeck();
            var animalCard = _game.AnimalDeck!.Animals.Peek();
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

        private Trade StartGameWithActiveTrade()
        {
            // Arrange
            _game.StartGame();
            _game.SortDeck();
            var animalCard = _game.AnimalDeck!.Animals.Peek();
            var initiator = _game.CurrentPlayer;
            var responder = _game.GetNextPlayer();

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
            return _game.StartNewTrade(initiator, responder, animalCard);
        }

        [TestMethod]
        public void SetOffer_TradeIsNoLongerCurrentGameAction_InvalidOperation()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Fifty]);
            trade.AcceptTrade(trade.Responder);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => trade.SetOffer(trade.Initiator, []));
            Assert.AreEqual("This trade is not currently active.", exception.Message);
        }

        [TestMethod]
        public void SetOffer_OfferHasAlreadyBeenMade_InvalidOperation()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Fifty]);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => trade.SetOffer(trade.Initiator, []));
            Assert.AreEqual($"Offer already set for this trade.", exception.Message);
        }

        [TestMethod]
        public void SetOffer_NotCurrentPlayer_InvalidOperation()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            var notInitiator = _game.Players.First(p => p.Id != trade.Initiator.Id);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => trade.SetOffer(notInitiator, []));
            Assert.AreEqual($"\"{notInitiator.Name}\" is not the trade initiator.", exception.Message);
        }

        [TestMethod]
        public void SetOffer_OfferCanNotBeEmpty_InvalidOperation()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => trade.SetOffer(trade.Initiator, []));
            Assert.AreEqual("Offer cannot be empty.", exception.Message);
        }

        [TestMethod]
        public void SetOffer_ResponderDoesntHaveEnoughMoney_InvalidOperation()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => trade.SetOffer(trade.Initiator, [MoneyValues.Fifty, MoneyValues.Fifty]));
            Assert.AreEqual($"Player \"{trade.Initiator.Name}\" does not have enough money for the proposed trade offer for animal card {trade.AnimalCard.Animal.Name}.", exception.Message);
        }

        [TestMethod]
        public void SetOffer_HappyFlow_NoException()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();

            // Act
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Fifty]);

            // Assert
            CollectionAssert.AreEquivalent(new List<MoneyValues> { MoneyValues.Ten, MoneyValues.Fifty }, trade.Offer);
        }

        [TestMethod]
        public void AcceptTrade_NotCurrentPlayer_InvalidOperation()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Fifty]);
            var notResponder = _game.Players.First(p => p.Id != trade.Responder.Id);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => trade.AcceptTrade(notResponder));
            Assert.AreEqual($"\"{notResponder.Name}\" is not offered the trade.", exception.Message);
        }

        [TestMethod]
        public void AcceptTrade_TradeIsNoLongerCurrentGameAction_InvalidOperation()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Fifty]);
            trade.AcceptTrade(trade.Responder);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => trade.AcceptTrade(trade.Responder));
            Assert.AreEqual("This trade is not currently active.", exception.Message);
        }

        [TestMethod]
        public void AcceptTrade_NoOfferHasBeenMade_InvalidOperation()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => trade.AcceptTrade(trade.Responder));
            Assert.AreEqual("Trade cannot be accepted when there is no initial offer.", exception.Message);
        }

        [TestMethod]
        public void AcceptTrade_HappyFlow_NoException()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Zero]);

            // Act
            trade.AcceptTrade(trade.Responder);

            // Assert
            Assert.IsTrue(trade.Initiator.AnimalCards.Contains(trade.AnimalCard));
            Assert.IsFalse(trade.Responder.AnimalCards.Contains(trade.AnimalCard));
            CollectionAssert.AreEquivalent(new List<MoneyValues> { MoneyValues.Zero, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Fifty }, trade.Initiator.Balance);
            CollectionAssert.AreEquivalent(new List<MoneyValues> { MoneyValues.Zero, MoneyValues.Zero, MoneyValues.Zero, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Fifty }, trade.Responder.Balance);
        }

        [TestMethod]
        public void Accept_DoubleCards_DoubleCardsAreTraded()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Zero]);
            trade.SetCounterOffer(trade.Responder, [MoneyValues.Ten]);

            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.SkipBid(auction.CurrentBidder);
            auction.PlaceBid(trade.Initiator, 10);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, false);
            auction.PerformAuctionTransfer(trade.Initiator, auction.Auctioneer, [MoneyValues.Ten]);

            var remainingPlayer = _game.Players.Single(p => p.Id != trade.Initiator.Id && p.Id != trade.Responder.Id);

            var secondAuction = _game.StartNewAuction(_game.CurrentPlayer);
            secondAuction.SkipBid(secondAuction.CurrentBidder);
            secondAuction.SkipBid(secondAuction.CurrentBidder);

            var secondTrade = _game.StartNewTrade(trade.Initiator, remainingPlayer, trade.AnimalCard);
            secondTrade.SetOffer(trade.Initiator, [MoneyValues.Ten]);
            secondTrade.SetCounterOffer(remainingPlayer, [MoneyValues.Zero]);

            Console.WriteLine($"Number of Game actions: {_game.Auctions.Count + _game.Trades.Count}");
            var thirdTrade = _game.StartNewTrade(trade.Responder, trade.Initiator, trade.AnimalCard);
            thirdTrade.SetOffer(trade.Responder, [MoneyValues.Ten]);

            // Act
            thirdTrade.AcceptTrade(trade.Initiator);

            // Assert
            Assert.AreEqual(4, trade.Responder.AnimalCards.Count(c => c.Animal.Name == trade.AnimalCard.Animal.Name));
            Assert.AreEqual(0, trade.Initiator.AnimalCards.Count(c => c.Animal.Name == trade.AnimalCard.Animal.Name));
        }

        [TestMethod]
        public void SetCounterOffer_TradeIsNoLongerCurrentGameAction_InvalidOperation()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Fifty]);
            trade.AcceptTrade(trade.Responder);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => trade.SetCounterOffer(trade.Responder, []));
            Assert.AreEqual("This trade is not currently active.", exception.Message);
        }

        [TestMethod]
        public void SetCounterOffer_OfferHasNotBeenMade_InvalidOperation()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => trade.SetCounterOffer(trade.Responder, []));
            Assert.AreEqual("Counter offer cannot be set when there is no initial offer.", exception.Message);
        }

        [TestMethod]
        public void SetCounterOffer_NotCurrentPlayer_InvalidOperation()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Fifty]);
            var notResponder = _game.Players.First(p => p.Id != trade.Responder.Id);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => trade.SetCounterOffer(notResponder, []));
            Assert.AreEqual($"\"{notResponder.Name}\" is not offered the trade.", exception.Message);
        }

        [TestMethod]
        public void SetCounterOffer_ResponderDoesntHaveEnoughMoney_InvalidOperation()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Fifty]);

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() => trade.SetCounterOffer(trade.Responder, [MoneyValues.Fifty, MoneyValues.Fifty]));
            Assert.AreEqual($"Player \"{trade.Responder.Name}\" does not have enough money for the proposed trade offer for animal card {trade.AnimalCard.Animal.Name}.", exception.Message);
        }

        [TestMethod]
        public void SetCounterOffer_HappyFlow_NoException()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Zero]);

            // Act
            trade.SetCounterOffer(trade.Responder, [MoneyValues.Ten, MoneyValues.Ten]);

            // Assert
            Assert.AreEqual(0, trade.Initiator.AnimalCards.Count(c => c.Animal.Name == trade.AnimalCard.Animal.Name));
            Assert.AreEqual(2, trade.Responder.AnimalCards.Count(c => c.Animal.Name == trade.AnimalCard.Animal.Name));
            CollectionAssert.AreEquivalent(new List<MoneyValues> { MoneyValues.Zero, MoneyValues.Zero, MoneyValues.Zero, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Fifty }, trade.Responder.Balance);
            CollectionAssert.AreEquivalent(new List<MoneyValues> { MoneyValues.Zero, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Fifty }, trade.Initiator.Balance);
        }

        [TestMethod]
        public void SetCounterOffer_OfferEqualsCounterOfferForFirstTime_ResetOfferAndCounterOfferAndTradeStillActive()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Zero]);

            // Act
            trade.SetCounterOffer(trade.Responder, [MoneyValues.Ten, MoneyValues.Zero]);

            // Assert
            Assert.AreEqual(1, trade.Initiator.AnimalCards.Count(c => c.Animal.Name == trade.AnimalCard.Animal.Name));
            Assert.AreEqual(1, trade.Responder.AnimalCards.Count(c => c.Animal.Name == trade.AnimalCard.Animal.Name));
            CollectionAssert.AreEquivalent(new List<MoneyValues> { MoneyValues.Zero, MoneyValues.Zero, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Fifty }, trade.Responder.Balance);
            CollectionAssert.AreEquivalent(new List<MoneyValues> { MoneyValues.Zero, MoneyValues.Zero, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Fifty }, trade.Initiator.Balance);
            Assert.AreEqual(_game.CurrentGameAction, trade);
            Assert.IsNull(trade.Offer);
            Assert.IsNull(trade.CounterOffer);
        }

        [TestMethod]
        public void SetCounterOffer_OfferEqualsCounterOfferForSecondTime_InitiatorWinsAnimalsForFree()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Zero]);
            trade.SetCounterOffer(trade.Responder, [MoneyValues.Ten, MoneyValues.Zero]);

            // Act
            trade.SetOffer(trade.Initiator, [MoneyValues.Ten, MoneyValues.Zero]);
            trade.SetCounterOffer(trade.Responder, [MoneyValues.Ten, MoneyValues.Zero]);

            // Assert
            Assert.AreEqual(2, trade.Initiator.AnimalCards.Count(c => c.Animal.Name == trade.AnimalCard.Animal.Name));
            Assert.AreEqual(0, trade.Responder.AnimalCards.Count(c => c.Animal.Name == trade.AnimalCard.Animal.Name));
            CollectionAssert.AreEquivalent(new List<MoneyValues> { MoneyValues.Zero, MoneyValues.Zero, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Fifty }, trade.Responder.Balance);
            CollectionAssert.AreEquivalent(new List<MoneyValues> { MoneyValues.Zero, MoneyValues.Zero, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Fifty }, trade.Initiator.Balance);
            Assert.AreNotEqual(_game.CurrentGameAction, trade);
            CollectionAssert.AreEquivalent(new List<MoneyValues> { MoneyValues.Ten, MoneyValues.Zero }, trade.Offer);
            CollectionAssert.AreEquivalent(new List<MoneyValues> { MoneyValues.Ten, MoneyValues.Zero }, trade.CounterOffer);
        }

        [TestMethod]
        public void SetCounterOffer_DoubleCards_DoubleCardsAreTraded()
        {
            // Arrange
            var trade = StartGameWithActiveTrade();
            trade.SetOffer(trade.Initiator, [MoneyValues.Zero]);
            trade.SetCounterOffer(trade.Responder, [MoneyValues.Ten]);

            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.SkipBid(auction.CurrentBidder);
            auction.PlaceBid(trade.Initiator, 10);
            auction.MoveToMoneyTransferPhase(auction.Auctioneer, false);
            auction.PerformAuctionTransfer(trade.Initiator, auction.Auctioneer, [MoneyValues.Ten]);

            var remainingPlayer = _game.Players.Single(p => p.Id != trade.Initiator.Id && p.Id != trade.Responder.Id);

            var secondAuction = _game.StartNewAuction(_game.CurrentPlayer);
            secondAuction.SkipBid(secondAuction.CurrentBidder);
            secondAuction.SkipBid(secondAuction.CurrentBidder);

            var secondTrade = _game.StartNewTrade(trade.Initiator, remainingPlayer, trade.AnimalCard);
            secondTrade.SetOffer(trade.Initiator, [MoneyValues.Ten]);
            secondTrade.SetCounterOffer(remainingPlayer, [MoneyValues.Zero]);
            var thirdTrade = _game.StartNewTrade(trade.Responder, trade.Initiator, trade.AnimalCard);
            thirdTrade.SetOffer(trade.Responder, [MoneyValues.Ten]);

            // Act
            thirdTrade.SetCounterOffer(trade.Initiator, [MoneyValues.Zero]);

            // Assert
            Assert.AreEqual(4, trade.Responder.AnimalCards.Count(c => c.Animal.Name == trade.AnimalCard.Animal.Name));
            Assert.AreEqual(0, trade.Initiator.AnimalCards.Count(c => c.Animal.Name == trade.AnimalCard.Animal.Name));
        }

        //Schrijf code om het spel te laten eindigen => ik denk dat dat automatisch heft geval is wanneer er geen dieren meer zijn in de deck en er alleen maar kwartetten zijn.

    }
}