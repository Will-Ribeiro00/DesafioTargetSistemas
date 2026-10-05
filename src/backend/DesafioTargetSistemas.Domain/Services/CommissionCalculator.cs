namespace DesafioTargetSistemas.Domain.Services
{
    public static class CommissionCalculator
    {
        private const decimal NO_COMMISSION_BELOW = 100m;
        private const decimal LOW_COMMISSION_BELOW = 500m;

        public static CommissionResult Calculate(decimal totalPrice)
        {
            var percentage = totalPrice switch
            {
                < NO_COMMISSION_BELOW => 0m,
                < LOW_COMMISSION_BELOW => 1m,
                _ => 5m
            };

            var amount = Math.Round(totalPrice * percentage / 100, 2, MidpointRounding.AwayFromZero);

            return new CommissionResult(percentage, amount);
        }
    }
}
