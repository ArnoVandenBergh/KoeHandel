namespace KoeHandel.BL
{
    public record AnimalCard(Animal Animal);

    public record Animal(string Name, int Value);

    public class AnimalDeck
    {
        public Queue<AnimalCard> Animals { get; set; }

        public AnimalDeck()
        {
            var koe = new Animal("Koe", 800);
            var schaap = new Animal("Schaap", 250);
            var gans = new Animal("Gans", 40);
            var kat = new Animal("Kat", 90);
            var paard = new Animal("Paard", 1000);
            var ezel = new Animal("Ezel", 500);
            var hond = new Animal("Hond", 160);
            var kip = new Animal("Kip", 10);
            var varken = new Animal("Varken", 650);
            var geit = new Animal("Geit", 350);

            List<Animal> animals =
            [
                koe,
                schaap,
                gans,
                kat,
                paard,
                ezel,
                hond,
                kip,
                varken,
                geit
            ];

            List<AnimalCard> animalCards = animals.Aggregate(new List<AnimalCard>(), (acc, animal) =>
            {
                for (int i = 0; i < 4; i++)
                {
                    acc.Add(new AnimalCard(animal));
                }
                return acc;
            }).OrderBy(_ => Guid.NewGuid()).ToList();

            Animals = new Queue<AnimalCard>(animalCards);
        }
    }
}
