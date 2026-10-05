using DesafioTargetSistemas.Domain.ValueObjects;

namespace DesafioTargetSistemas.Domain.Services
{
    public static class CommissionCalculator
    {
        public static CommissionResult Calculate(decimal totalPrice)
        {
            var percentage = totalPrice switch
            {
                < DesafioTargetSistemasRuleConstants.COMMISSION_NONE_BELOW_AMOUNT => DesafioTargetSistemasRuleConstants.COMMISSION_NONE_PERCENTAGE,
                < DesafioTargetSistemasRuleConstants.COMMISSION_LOW_BELOW_AMOUNT => DesafioTargetSistemasRuleConstants.COMMISSION_LOW_PERCENTAGE,
                _ => DesafioTargetSistemasRuleConstants.COMMISSION_HIGH_PERCENTAGE
            };

            var amount = Math.Round(totalPrice * percentage / 100, 2, MidpointRounding.AwayFromZero);

            return new CommissionResult(percentage, amount);
        }
    }
}
