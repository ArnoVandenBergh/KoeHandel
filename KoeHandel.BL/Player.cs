namespace KoeHandel.BL
{
    public class Player(string name)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = name;
        public List<MoneyValues> Balance { get; set; } = [MoneyValues.Zero, MoneyValues.Zero, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Fifty];
        public List<AnimalCard> AnimalCards { get; set; } = [];
    }
}
