using KoeHandel.BL;

namespace KoeHandel.BLTests.Models
{
    public class TestAnimalDeck : IAnimalDeck
    {
        internal readonly static Animal _koe = new Animal("Koe", 800);
        internal readonly static Animal _schaap = new Animal("Schaap", 250);
        internal readonly static Animal _gans = new Animal("Gans", 40);
        internal readonly static Animal _kat = new Animal("Kat", 90);
        internal readonly static Animal _paard = new Animal("Paard", 1000);
        internal readonly static Animal _ezel = new Animal("Ezel", 500);
        internal readonly static Animal _hond = new Animal("Hond", 160);
        internal readonly static Animal _kip = new Animal("Kip", 10);
        internal readonly static Animal _varken = new Animal("Varken", 650);
        internal readonly static Animal _geit = new Animal("Geit", 350);

        public Queue<AnimalCard> Animals { get; set; }

        public TestAnimalDeck()
        {
            List<Animal> animals =
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
                    acc.Add(new AnimalCard(animal));
                }
                return acc;
            }).ToList();

            Animals = new Queue<AnimalCard>(animalCards);
        }
    }
}
