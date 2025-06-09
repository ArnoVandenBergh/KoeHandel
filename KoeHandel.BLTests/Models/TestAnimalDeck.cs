namespace KoeHandel.BL.Tests.Models
{
    public class TestAnimalDeck : IAnimalDeck
    {
        internal readonly static AnimalCard _koe = new(9, "Koe", 800);
        internal readonly static AnimalCard _schaap = new(5, "Schaap", 250);
        internal readonly static AnimalCard _gans = new(2, "Gans", 40);
        internal readonly static AnimalCard _kat = new(3, "Kat", 90);
        internal readonly static AnimalCard _paard = new(10, "Paard", 1000);
        internal readonly static AnimalCard _ezel = new(7, "Ezel", 500);
        internal readonly static AnimalCard _hond = new(4, "Hond", 160);
        internal readonly static AnimalCard _kip = new(1, "Kip", 10);
        internal readonly static AnimalCard _varken = new(8, "Varken", 650);
        internal readonly static AnimalCard _geit = new(6, "Geit", 350);

        public Queue<AnimalCard> Animals { get; set; }

        public TestAnimalDeck()
        {
            List<AnimalCard> animals =
            [
                _koe,
                _schaap,
                _gans,
                _kat,
                _paard,
                _ezel,
                _hond,
                _kip,
                _varken,
                _geit
            ];

            List<AnimalCard> animalCards = animals.Aggregate(new List<AnimalCard>(), (acc, animal) =>
            {
                for (int i = 0; i < 4; i++)
                {
                    acc.Add(animal);
                }
                return acc;
            }).ToList();

            Animals = new Queue<AnimalCard>(animalCards);
        }
    }
}
