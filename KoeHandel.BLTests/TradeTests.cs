using KoeHandel.BL.Tests.Models;
using KoeHandel.Domain.Money;

namespace KoeHandel.BL.Tests
{
    [TestClass()]
    public class TradeTests() : BaseTests()
    {
        private Trade StartGameWithActiveTrade()
        {
            // Arrange
            _game.StartGame(new TestAnimalDeck());
            var animalCard = _game.Deck!.Animals.Peek();
            var initiator = _game.CurrentPlayer;
            var responder = _game.GetNextPlayer();

            // Initiator gets animal card through auction
            var auction = _game.StartNewAuction(_game.CurrentPlayer);
            auction.SkipBid(auction.CurrentBidder);
            auction.SkipBid(auction.CurrentBidder);

            while (!responder.AnimalCards.Any(c => c.Name == animalCard.Name))
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
            Assert.AreEqual($"Player \"{trade.Initiator.Name}\" does not have enough money for the proposed trade offer for animal card {trade.AnimalCard.Name}.", exception.Message);
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
        public void AcceptTrade_DoubleCards_DoubleCardsAreTraded()
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
            Assert.AreEqual(4, trade.Responder.AnimalCards.Count(c => c.Name == trade.AnimalCard.Name));
            Assert.AreEqual(0, trade.Initiator.AnimalCards.Count(c => c.Name == trade.AnimalCard.Name));
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
            Assert.AreEqual($"Player \"{trade.Responder.Name}\" does not have enough money for the proposed trade offer for animal card {trade.AnimalCard.Name}.", exception.Message);
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
            Assert.AreEqual(0, trade.Initiator.AnimalCards.Count(c => c.Name == trade.AnimalCard.Name));
            Assert.AreEqual(2, trade.Responder.AnimalCards.Count(c => c.Name == trade.AnimalCard.Name));
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
            Assert.AreEqual(1, trade.Initiator.AnimalCards.Count(c => c.Name == trade.AnimalCard.Name));
            Assert.AreEqual(1, trade.Responder.AnimalCards.Count(c => c.Name == trade.AnimalCard.Name));
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
            Assert.AreEqual(2, trade.Initiator.AnimalCards.Count(c => c.Name == trade.AnimalCard.Name));
            Assert.AreEqual(0, trade.Responder.AnimalCards.Count(c => c.Name == trade.AnimalCard.Name));
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
            Assert.AreEqual(4, trade.Responder.AnimalCards.Count(c => c.Name == trade.AnimalCard.Name));
            Assert.AreEqual(0, trade.Initiator.AnimalCards.Count(c => c.Name == trade.AnimalCard.Name));
        }
    }
}