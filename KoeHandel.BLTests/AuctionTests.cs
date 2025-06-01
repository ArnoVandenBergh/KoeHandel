namespace KoeHandel.BL.Tests
{
    [TestClass()]
    public class AuctionTests() : BaseTests()
    {
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
    }
}