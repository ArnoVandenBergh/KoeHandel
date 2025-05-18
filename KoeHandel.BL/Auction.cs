namespace KoeHandel.BL
{
    public class Auction(Player auctioneer, AnimalCard animalCard, Game game) : GameAction()
    {
        public AnimalCard AnimalCard { get; set; } = animalCard;
        public Player? Winner { get; set; }
        public Player Auctioneer { get; set; } = auctioneer;
        public int Bid { get; set; }
        public List<MoneyValues>? MoneyTransfer { get; set; }
        public bool DidActioneerBuyOver { get; set; }
        public bool IsAuctionFinished { get; set; } = false;
        public Game Game { get; set; } = game;
        public List<Player> Bidders { get; set; } = game.Players.Where(p => p.Id != auctioneer.Id).ToList();
        public Player CurrentBidder { get; set; } = game.GetNextPlayer();

        public void EndAuction(bool AuctioneerBuyOver)
        {
            if (Bidders.Count > 0)
            {
                throw new InvalidOperationException("The auction is still in progress.");
            }
        }

        public void PlaceBid(Player bidder, int bid)
        {
            if (Game.CurrentGameAction == null || Game.CurrentGameAction.Id != Id)
            {
                throw new InvalidOperationException("This auction is not currently active.");
            }
            if (IsAuctionFinished)
            {
                throw new InvalidOperationException("The auction is already finished.");
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

            Bid = bid;
            CurrentBidder = _GetNextBidder();
        }

        public void SkipBid(Player bidder)
        {
            if (Game.CurrentGameAction == null || Game.CurrentGameAction.Id != Id)
            {
                throw new InvalidOperationException("This auction is not currently active.");
            }
            if (bidder.Id == Auctioneer.Id)
            {
                throw new InvalidOperationException($"The auctioneer (\"{bidder.Name}\") can't place a bid.");
            }
            if (bidder.Id != CurrentBidder.Id)
            {
                throw new InvalidOperationException($"It's not {bidder.Name}'s turn to bid.");
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
