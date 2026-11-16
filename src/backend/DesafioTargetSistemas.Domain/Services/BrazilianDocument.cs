using System.Text.RegularExpressions;

namespace DesafioTargetSistemas.Domain.Services;

public static partial class BrazilianDocument
{
    private static readonly int[] CpfFirstWeights = [10, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] CpfSecondWeights = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] CnpjFirstWeights = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] CnpjSecondWeights = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    public static bool IsValidCpf(string? cpf)
    {
        if (cpf is null || !CpfFormat().IsMatch(cpf) || AllSameCharacter(cpf))
            return false;

        var values = ToValues(cpf);

        return CheckDigit(values[..9], CpfFirstWeights) == values[9]
            && CheckDigit(values[..10], CpfSecondWeights) == values[10];
    }

    public static bool IsValidCnpj(string? cnpj)
    {
        if (cnpj is null || !CnpjFormat().IsMatch(cnpj) || AllSameCharacter(cnpj))
            return false;

        var values = ToValues(cnpj);

        return CheckDigit(values[..12], CnpjFirstWeights) == values[12]
            && CheckDigit(values[..13], CnpjSecondWeights) == values[13];
    }

    private static int[] ToValues(string document) => [.. document.Select(c => c - '0')];

    private static int CheckDigit(int[] values, int[] weights)
    {
        var sum = values.Select((value, index) => value * weights[index]).Sum();
        var rest = sum % 11;

        return rest < 2 ? 0 : 11 - rest;
    }

    private static bool AllSameCharacter(string document) => document.All(c => c == document[0]);

    [GeneratedRegex("^[0-9]{11}$")]
    private static partial Regex CpfFormat();

    [GeneratedRegex("^[0-9A-Z]{12}[0-9]{2}$")]
    private static partial Regex CnpjFormat();
}
