namespace KoeHandel.BL
{
    public enum AuctionState
    {
        BidPhase,
        BuyOverPhase,
        MoneyTransferPhase,
        Finished
    }
    public class Auction(Player auctioneer, AnimalCard animalCard, Game game) : GameAction()
    {
        public AnimalCard AnimalCard { get; set; } = animalCard;
        public Player? LastBidder { get; set; }
        public Player Auctioneer { get; set; } = auctioneer;
        public int Bid { get; set; }
        public List<MoneyValues>? MoneyTransfer { get; set; }
        public bool DidActioneerBuyOver { get; set; }
        public AuctionState AuctionState { get; set; } = AuctionState.BidPhase;
        public Game Game { get; set; } = game;
        public List<Player> Bidders { get; set; } = game.Players.Where(p => p.Id != auctioneer.Id).ToList();
        public Player CurrentBidder { get; set; } = game.GetNextPlayer();

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

            if (Bidders.Count == 1 && Bid > 0)
            {
                AuctionState = AuctionState.BuyOverPhase;
                return;
            }

            CurrentBidder = _GetNextBidder();
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

            if (Bidders.Count == 2 && Bid > 0)
            {
                AuctionState = AuctionState.BuyOverPhase;
                return;
            }
            if (Bidders.Count == 1 && Bid == 0)
            {
                AuctionState = AuctionState.Finished;
                DidActioneerBuyOver = true;
                MoneyTransfer = [];
                return;
            }
            CurrentBidder = _GetNextBidder();
            Bidders.Remove(bidder);
        }

        private Player _GetNextBidder()
        {
            var currentPlayerIndex = Bidders.IndexOf(CurrentBidder);
            int nextIndex = (currentPlayerIndex + 1) % Bidders.Count;
            var nextPlayer = Bidders[nextIndex];
            return nextPlayer;
        }
    }
}
