namespace KoeHandel.BL
{
    public class Trade(Player buyer, Player seller, AnimalCard animalCard, List<MoneyValues> offer) : GameAction()
    {
        public Player Buyer { get; set; } = buyer;
        public Player Seller { get; set; } = seller;
        public AnimalCard AnimalCard { get; set; } = animalCard;
        public List<MoneyValues> Offer { get; set; } = offer;
        public List<MoneyValues>? CounterOffer { get; set; }
        public bool IsAnimalPaired { get; set; } = HasCardPaired(buyer, animalCard) && HasCardPaired(seller, animalCard);

        internal override void MoveToFinishedState()
        {
            throw new NotImplementedException();
        }

        private static bool HasCardPaired(Player player, AnimalCard animalCard)
        {
            return player.AnimalCards.Count(c => c.Animal.Name == animalCard.Animal.Name) == 2;
        }
    }
}
