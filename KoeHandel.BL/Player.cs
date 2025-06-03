namespace KoeHandel.BL
{
    public class Player(string name)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = name;
        public List<MoneyValues> Balance { get; set; } = [MoneyValues.Zero, MoneyValues.Zero, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Ten, MoneyValues.Fifty];
        public List<AnimalCard> AnimalCards { get; set; } = [];
        public int Score { get; internal set; }

        internal void ValidatePlayerHasEnoughCash(List<MoneyValues> cash)
        {
            List<MoneyValues> payerBalance = [.. Balance];
            foreach (var value in cash)
            {
                int index = payerBalance.IndexOf(value);
                if (index != -1)
                {
                    payerBalance.RemoveAt(index);
                }
                else
                {
                    throw new InvalidOperationException($"Payer does not have enough of {value} to transfer.");
                }
            }
        }

        internal void RemoveCash(List<MoneyValues> cash)
        {
            foreach (var value in cash)
            {
                Balance.Remove(value);
            }
        }

    }
}
