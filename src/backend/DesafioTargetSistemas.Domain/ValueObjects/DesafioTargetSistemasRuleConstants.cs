namespace DesafioTargetSistemas.Domain.ValueObjects
{
    public class DesafioTargetSistemasRuleConstants
    {
        public const int PRODUCT_CODE_MAX_LENGTH = 30;
        public const int STOCK_MOVEMENT_DESCRIPTION_MAX_LENGTH = 200;
        public const decimal COMMISSION_NONE_BELOW_AMOUNT = 100m;
        public const decimal COMMISSION_LOW_BELOW_AMOUNT = 500m;
        public const decimal COMMISSION_NONE_PERCENTAGE = 0m;
        public const decimal COMMISSION_LOW_PERCENTAGE = 1m;
        public const decimal COMMISSION_HIGH_PERCENTAGE = 5m;
        public const int SALE_MAX_DUE_DAYS = 30;
        public const decimal INTEREST_DAILY_RATE = 0.025m;
        public const string STOCK_MOVEMENT_SALE_DESCRIPTION = "Sale registered";
    }
}
