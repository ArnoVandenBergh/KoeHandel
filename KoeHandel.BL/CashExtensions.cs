using KoeHandel.Domain.Money;

namespace KoeHandel.BL
{
    internal static class CashExtensions
    {
        internal static int GetCashValue(List<MoneyValues> cash)
        {
            return cash.Sum(value => (int)value);
        }
    }
}
