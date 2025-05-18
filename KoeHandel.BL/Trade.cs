namespace KoeHandel.BL
{
    public class Trade(Player buyer, Player seller, AnimalCard animalCard, List<MoneyValues> offer, bool isAnimalPaired)
    {
        public Player Buyer { get; set; } = buyer;
        public Player Seller { get; set; } = seller;
        public AnimalCard AnimalCard { get; set; } = animalCard;
        public List<MoneyValues> Offer { get; set; } = offer;
        public List<MoneyValues>? CounterOffer { get; set; }
        public bool IsAnimalPaired { get; set; } = isAnimalPaired;
    }
}
