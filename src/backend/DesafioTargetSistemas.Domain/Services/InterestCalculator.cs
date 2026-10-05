using DesafioTargetSistemas.Domain.ValueObjects;

namespace DesafioTargetSistemas.Domain.Services
{
    public class InterestCalculator
    {
        public static int DaysOverdue(DateOnly dueDate, DateOnly today)
            => Math.Max(0, today.DayNumber - dueDate.DayNumber);

        public static decimal Calculate(decimal amount, int daysOverdue)
            => Math.Round(amount * DesafioTargetSistemasRuleConstants.INTEREST_DAILY_RATE * daysOverdue, 2, MidpointRounding.AwayFromZero);
    }
}
