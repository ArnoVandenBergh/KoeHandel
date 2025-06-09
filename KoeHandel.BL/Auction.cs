using KoeHandel.Domain.Money;

namespace KoeHandel.BL
{
    public enum AuctionState
    {
        BidPhase,
        BuyOverPhase,
        MoneyTransferPhase,
        Finished
    }
    public class Auction(Player auctioneer, AnimalCard animalCard, Game game) : GameAction(game)
    {
        public AnimalCard AnimalCard { get; set; } = animalCard;
        public Player? LastBidder { get; set; }
        public Player Auctioneer { get; set; } = auctioneer;
        public int Bid { get; set; }
        public List<MoneyValues>? MoneyTransfer { get; set; } = null;
        public bool DidActioneerBuyOver { get; set; }
        public AuctionState AuctionState { get; set; } = AuctionState.BidPhase;
        public List<Player> Bidders { get; set; } = game.Players.Where(p => p.Id != auctioneer.Id).ToList();
        public Player CurrentBidder { get; set; } = game.GetNextPlayer();

        internal override void MoveToFinishedState()
        {
            if (MoneyTransfer == null)
            {
                throw new InvalidOperationException("Money transfer must be set before finishing the auction.");
            }
            AuctionState = AuctionState.Finished;
        }

        public void MoveToMoneyTransferPhase(Player auctioneer, bool AuctioneerBuyOver)
        {
            if ((int)AuctionState < (int)AuctionState.BuyOverPhase)
            {
                throw new InvalidOperationException("The auction is still in progress.");
            }
            if ((int)AuctionState > (int)AuctionState.BuyOverPhase)
            {
                throw new InvalidOperationException("The auction is passed the buyover phase.");
            }
            if (auctioneer.Id != Auctioneer.Id)
            {
                throw new InvalidOperationException($"The auctioneer (\"{Auctioneer.Name}\") must be the one to move to the money transfer phase.");
            }

            DidActioneerBuyOver = AuctioneerBuyOver;
            AuctionState = AuctionState.MoneyTransferPhase;
        }

        public void PerformAuctionTransfer(Player payer, Player payee, List<MoneyValues> cash)
        {

            if (AuctionState != AuctionState.MoneyTransferPhase)
            {
                throw new InvalidOperationException("The auction is not in the money transfer phase.");
            }
            if (payer.Id != LastBidder!.Id && payer.Id != Auctioneer.Id)
            {
                throw new InvalidOperationException($"Player \"{payer.Name}\" is not the payer.");
            }
            if (payer.Id == payee.Id)
            {
                throw new InvalidOperationException($"Payer and Payee cannot be the same player.");
            }
            if (payee.Id != LastBidder!.Id && payee.Id != Auctioneer.Id)
            {
                throw new InvalidOperationException($"Player \"{payee.Name}\" is not the payee.");
            }
            if (DidActioneerBuyOver == (payer.Id != Auctioneer.Id))
            {
                throw new InvalidOperationException($"Payer and Payee are reversed.");
            }
            var totalCash = CashExtensions.GetCashValue(cash);
            if (totalCash < Bid)
            {
                throw new InvalidOperationException($"Total cash ({totalCash}) must be at least equal to the bid ({Bid}).");
            }

            payer.ValidatePlayerHasEnoughCash(cash);
            payer.RemoveCash(cash);

            payee.Balance.AddRange(cash);
            payer.AnimalCards.Add(AnimalCard);
            MoneyTransfer = cash;
            Game.EndCurrentGameAction();

        }

        public void PlaceBid(Player bidder, int bid)
        {
            if (Game.CurrentGameAction == null || Game.CurrentGameAction.Id != Id)
            {
                throw new InvalidOperationException("This auction is not currently active.");
            }
            if (AuctionState != AuctionState.BidPhase)
            {
                throw new InvalidOperationException("The auction has passed the bidding phase");
            }
            if (bidder.Id == Auctioneer.Id)
            {
                throw new InvalidOperationException($"The auctioneer (\"{bidder.Name}\") can't place a bid.");
            }
            if (bidder.Id != CurrentBidder.Id)
            {
                throw new InvalidOperationException($"It's not {bidder.Name}'s turn to bid.");
            }
            if (bid <= Bid)
            {
                throw new InvalidOperationException($"Bid must be higher than the current bid of {Bid}.");
            }

            LastBidder = bidder;
            Bid = bid;
            CurrentBidder = GetNextBidder();
            Console.WriteLine($"Player \"{bidder.Name}\" has placed a bid of {Bid}.");

            if (Bidders.Count == 1 && Bid > 0)
            {
                AuctionState = AuctionState.BuyOverPhase;
            }

        }

        public void SkipBid(Player bidder)
        {
            if (Game.CurrentGameAction == null || Game.CurrentGameAction.Id != Id)
            {
                throw new InvalidOperationException("This auction is not currently active.");
            }
            if (AuctionState != AuctionState.BidPhase)
            {
                throw new InvalidOperationException("The auction has passed the bidding phase");
            }
            if (bidder.Id == Auctioneer.Id)
            {
                throw new InvalidOperationException($"The auctioneer (\"{bidder.Name}\") can't place a bid.");
            }
            if (bidder.Id != CurrentBidder.Id)
            {
                throw new InvalidOperationException($"It's not {bidder.Name}'s turn to bid.");
            }

            CurrentBidder = GetNextBidder();
            Bidders.Remove(bidder);
            Console.WriteLine($"Player \"{bidder.Name}\" has skipped their bid.");

            if (Bidders.Count == 1 && Bid > 0)
            {
                AuctionState = AuctionState.BuyOverPhase;
            }
            if (Bidders.Count == 0 && Bid == 0)
            {
                DidActioneerBuyOver = true;
                MoneyTransfer = [];
                Auctioneer.AnimalCards.Add(AnimalCard);
                Game.EndCurrentGameAction();
            }
        }

        private Player GetNextBidder()
        {
            var currentPlayerIndex = Bidders.IndexOf(CurrentBidder);
            int nextIndex = (currentPlayerIndex + 1) % Bidders.Count;
            var nextPlayer = Bidders[nextIndex];
            return nextPlayer;
        }
    }
}
