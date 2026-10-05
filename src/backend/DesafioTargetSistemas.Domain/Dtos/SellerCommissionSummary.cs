namespace DesafioTargetSistemas.Domain.Dtos
{
    public record SellerCommissionSummary(
        int SellerId,
        string SellerName,
        int SalesCount,
        decimal TotalSold,
        decimal TotalCommission
    );
}
