using Microsoft.EntityFrameworkCore;

namespace KoeHandel.Persistence
{
    public static class DataSeeder
    {
        public static void Seed(this DbContext context)
        {
            var cards = context.Set<AnimalCard>().ToList();
            if (cards.Count == 0)
            {
                List<AnimalCard> animalCards =
                [
                    CreateAnimalCard(1, "Kip", 10),
                    CreateAnimalCard(2, "Gans", 40),
                    CreateAnimalCard(3, "Kat", 90),
                    CreateAnimalCard(4, "Hond", 160),
                    CreateAnimalCard(5, "Schaap", 250),
                    CreateAnimalCard(6, "Geit", 350),
                    CreateAnimalCard(7, "Ezel", 500),
                    CreateAnimalCard(8, "Varken", 650),
                    CreateAnimalCard(9, "Koe", 800),
                    CreateAnimalCard(10, "Paard", 1000)
                ];
                context.Set<AnimalCard>().AddRange(animalCards);
                context.SaveChanges();
            }
        }

        public static async Task SeedAsync(this DbContext context, CancellationToken token = default)
        {
            var cards = await context.Set<AnimalCard>().ToListAsync(token);
            if (cards.Count == 0)
            {
                List<AnimalCard> animalCards =
                [
                    CreateAnimalCard(1, "Kip", 10),
                    CreateAnimalCard(2, "Gans", 40),
                    CreateAnimalCard(3, "Kat", 90),
                    CreateAnimalCard(4, "Hond", 160),
                    CreateAnimalCard(5, "Schaap", 250),
                    CreateAnimalCard(6, "Geit", 350),
                    CreateAnimalCard(7, "Ezel", 500),
                    CreateAnimalCard(8, "Varken", 650),
                    CreateAnimalCard(9, "Koe", 800),
                    CreateAnimalCard(10, "Paard", 1000)
                ];
                context.Set<AnimalCard>().AddRange(animalCards);
                await context.SaveChangesAsync(token);
            }
        }

        private static AnimalCard CreateAnimalCard(int id, string name, int value)
        {
            return new AnimalCard
            {
                Id = id,
                Name = name,
                Value = value
            };
        }
    }
}
