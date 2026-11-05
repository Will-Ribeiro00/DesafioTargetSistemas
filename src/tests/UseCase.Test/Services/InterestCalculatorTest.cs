using DesafioTargetSistemas.Domain.Services;
using Shouldly;
using System.Globalization;

namespace UseCase.Test.Services;

public class InterestCalculatorTest
{
    [Theory]
    [InlineData("2026-10-10", "2026-10-05", 0)]
    [InlineData("2026-10-05", "2026-10-05", 0)]
    [InlineData("2026-10-04", "2026-10-05", 1)]
    [InlineData("2026-10-01", "2026-10-05", 4)]
    [InlineData("2026-09-06", "2026-10-05", 29)]
    [InlineData("2028-02-28", "2028-03-01", 2)]
    public void DaysOverdue(string dueDate, string today, int expectedDays)
    {
        // Act
        var result = InterestCalculator.DaysOverdue(ParseDate(dueDate), ParseDate(today));

        // Assert
        result.ShouldBe(expectedDays);
    }

    [Theory]
    [InlineData(100.00, 0, 0.00)]
    [InlineData(100.00, 1, 2.50)]
    [InlineData(100.00, 4, 10.00)]
    [InlineData(1200.00, 10, 300.00)]
    [InlineData(33.33, 1, 0.83)]
    [InlineData(1.00, 1, 0.03)]
    public void Calculate(double amount, int daysOverdue, double expectedInterest)
    {
        // Act
        var result = InterestCalculator.Calculate((decimal)amount, daysOverdue);

        // Assert
        result.ShouldBe((decimal)expectedInterest);
    }

    private static DateOnly ParseDate(string date) => DateOnly.Parse(date, CultureInfo.InvariantCulture);
}