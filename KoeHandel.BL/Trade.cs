namespace KoeHandel.BL
{
    public class Trade(Player initiator, Player responder, AnimalCard animalCard, Game game) : GameAction(game)
    {
        public Player Initiator { get; set; } = initiator;
        public Player Responder { get; set; } = responder;
        public AnimalCard AnimalCard { get; set; } = animalCard;
        public List<MoneyValues>? Offer { get; set; }
        public List<MoneyValues>? CounterOffer { get; set; }
        public bool IsAnimalPaired { get; set; } = HasCardPaired(initiator, animalCard) && HasCardPaired(responder, animalCard);
        public bool OffersWereEqualBefore { get; set; } = false;

        internal override void MoveToFinishedState()
        {
            // Trades are not finished in the same way as auctions, so no action needed here.
        }

        private static bool HasCardPaired(Player player, AnimalCard animalCard)
        {
            return player.AnimalCards.Count(c => c.Animal.Name == animalCard.Animal.Name) == 2;
        }

        public void SetOffer(Player initiator, List<MoneyValues> offer)
        {
            if (Game.CurrentGameAction == null || Game.CurrentGameAction.Id != Id)
            {
                throw new InvalidOperationException("This trade is not currently active.");
            }
            if (Offer != null)
            {
                throw new InvalidOperationException("Offer already set for this trade.");
            }
            if (initiator.Id != Initiator.Id)
            {
                throw new InvalidOperationException($"\"{initiator.Name}\" is not the trade initiator.");
            }
            if (offer == null || offer.Count == 0)
            {
                throw new InvalidOperationException("Offer cannot be empty.");
            }
            try
            {
                initiator.ValidatePlayerHasEnoughCash(offer);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Player \"{initiator.Name}\" does not have enough money for the proposed trade offer for animal card {AnimalCard.Animal.Name}.");
            }

            Offer = offer;
            Console.WriteLine($"Offer set: {initiator.Name} offers {string.Join(", ", offer)} for {AnimalCard.Animal.Name} to {Responder.Name}.");
        }

        public void SetCounterOffer(Player responder, List<MoneyValues> counterOffer)
        {
            if (Game.CurrentGameAction == null || Game.CurrentGameAction.Id != Id)
            {
                throw new InvalidOperationException("This trade is not currently active.");
            }
            if (Offer == null)
            {
                throw new InvalidOperationException("Counter offer cannot be set when there is no initial offer.");
            }
            if (responder.Id != Responder.Id)
            {
                throw new InvalidOperationException($"\"{responder.Name}\" is not offered the trade.");
            }
            try
            {
                responder.ValidatePlayerHasEnoughCash(counterOffer);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Player \"{responder.Name}\" does not have enough money for the proposed trade offer for animal card {AnimalCard.Animal.Name}.");
            }

            var totalOfferValue = CashExtensions.GetCashValue(Offer);
            var totalCounterOfferValue = CashExtensions.GetCashValue(counterOffer);
            if (totalOfferValue == totalCounterOfferValue && !OffersWereEqualBefore)
            {
                Offer = null;
                CounterOffer = null;
                OffersWereEqualBefore = true;
                return;
            }
            if (totalOfferValue == totalCounterOfferValue && OffersWereEqualBefore)
            {
                CounterOffer = counterOffer;
                TransferCards(responder, Initiator);
                Game.EndCurrentGameAction();
                return;
            }

            Console.WriteLine($"Counter offer set: {responder.Name} offers {string.Join(", ", counterOffer)} for {AnimalCard.Animal.Name} to {Initiator.Name}.");

            CounterOffer = counterOffer;
            responder.RemoveCash(counterOffer);
            Initiator.Balance.AddRange(counterOffer);
            Initiator.RemoveCash(Offer);
            Responder.Balance.AddRange(Offer);

            if (totalOfferValue > totalCounterOfferValue)
            {
                TransferCards(responder, Initiator);
            }
            else
            {
                TransferCards(Initiator, responder);
            }
            Game.EndCurrentGameAction();
        }

        private void TransferCards(Player from, Player to)
        {
            if (IsAnimalPaired)
            {
                to.AnimalCards.AddRange(from.AnimalCards.Where(c => c.Animal.Name == AnimalCard.Animal.Name));
                from.AnimalCards.RemoveAll(c => c.Animal.Name == AnimalCard.Animal.Name);
            }
            else
            {
                to.AnimalCards.Add(AnimalCard);
                from.AnimalCards.Remove(AnimalCard);
            }
        }

        public void AcceptTrade(Player responder)
        {
            if (Game.CurrentGameAction == null || Game.CurrentGameAction.Id != Id)
            {
                throw new InvalidOperationException("This trade is not currently active.");
            }
            if (Offer == null)
            {
                throw new InvalidOperationException("Trade cannot be accepted when there is no initial offer.");
            }
            if (responder.Id != Responder.Id)
            {
                throw new InvalidOperationException($"\"{responder.Name}\" is not offered the trade.");
            }

            Console.WriteLine($"Trade accepted by {responder.Name} for {AnimalCard.Animal.Name} from {Initiator.Name}.");

            TransferCards(Responder, Initiator);
            Initiator.RemoveCash(Offer);
            Responder.Balance.AddRange(Offer);
            Game.EndCurrentGameAction();
            Console.WriteLine($"Trade accepted: {Initiator.Name} trades {AnimalCard.Animal.Name} with {Responder.Name} for {string.Join(", ", Offer)}.");
        }
    }
}
