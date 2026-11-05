using DesafioTargetSistemas.Domain.Services;
using Shouldly;

namespace UseCase.Test.Services;

public class CommissionCalculatorTest
{
    [Theory]
    [InlineData(0.01, 0, 0.00)]
    [InlineData(99.99, 0, 0.00)]
    [InlineData(100.00, 1, 1.00)]
    [InlineData(499.99, 1, 5.00)]
    [InlineData(500.00, 5, 25.00)]
    [InlineData(1200.50, 5, 60.03)]
    public void Calculate(double totalPrice, double expectedPercentage, double expectedAmount)
    {
        // Act
        var result = CommissionCalculator.Calculate((decimal)totalPrice);

        // Assert
        result.Percentage.ShouldBe((decimal)expectedPercentage);
        result.Amount.ShouldBe((decimal)expectedAmount);
    }

    [Fact]
    public void ChallengeExampleJoaoSilva()
    {
        // Arrange: as vendas do João Silva no JSON do desafio
        decimal[] sales = [1200.50m, 950.75m, 1800.00m, 1400.30m, 1100.90m, 1550.00m, 1700.80m, 250.30m, 480.75m, 320.40m];

        // Act
        var totalCommission = sales.Sum(sale => CommissionCalculator.Calculate(sale).Amount);

        // Assert
        totalCommission.ShouldBe(495.69m);
    }
}
