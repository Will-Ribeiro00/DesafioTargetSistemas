using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Services;

namespace CommonTestUtilities.Entities;

public class SaleBuilder
{
    public static Sale Build(Seller seller, DateTime saleDate, decimal totalPrice)
    {
        var commission = CommissionCalculator.Calculate(totalPrice);

        return new Sale
        {
            SellerId = seller.Id,
            SaleDate = saleDate,
            TotalPrice = totalPrice,
            CommissionPercentage = commission.Percentage,
            CommissionAmount = commission.Amount
        };
    }
}