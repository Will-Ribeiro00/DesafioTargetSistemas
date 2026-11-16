using DesafioTargetSistemas.Domain.Services;
using Shouldly;

namespace UseCase.Test.Services;

public class BrazilianDocumentTest
{
    [Theory]
    [InlineData("08765871472")]
    [InlineData("52998224725")]
    public void ValidCpf(string cpf)
    {
        // Act
        var result = BrazilianDocument.IsValidCpf(cpf);

        // Assert
        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData("08765871473")]      // dígito verificador errado
    [InlineData("11111111111")]      // todos os dígitos iguais
    [InlineData("087.658.714-72")]   // com máscara
    [InlineData("0876587147")]       // 10 dígitos
    [InlineData("0876587147A")]      // letra no CPF
    [InlineData("")]                 // string vazia
    [InlineData(null)]              // string nula
    public void InvalidCpf(string? cpf)
    {
        // Act
        var result = BrazilianDocument.IsValidCpf(cpf);

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("11222333000181")]   // numérico
    [InlineData("20250718000143")]   // numérico (cliente do seed)
    [InlineData("12ABC34501DE35")]   // alfanumérico: exemplo oficial da Receita Federal
    [InlineData("4NWRHRLZ000129")]   // alfanumérico (cliente do seed)
    public void ValidCnpj(string cnpj)
    {
        // Act
        var result = BrazilianDocument.IsValidCnpj(cnpj);

        // Assert
        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData("20250718000144")]       // dígito verificador errado (numérico)
    [InlineData("12ABC34501DE36")]       // dígito verificador errado (alfanumérico)
    [InlineData("00000000000000")]       // todos os caracteres iguais
    [InlineData("12abc34501de35")]       // letras minúsculas
    [InlineData("12ABC34501DEAB")]       // letras nos dígitos verificadores
    [InlineData("12.ABC.345/01DE-35")]   // com máscara
    [InlineData("2025071800014")]        // 13 caracteres
    [InlineData("")]                     // string vazia
    [InlineData(null)]                   // string nula
    public void InvalidCnpj(string? cnpj)
    {
        // Act
        var result = BrazilianDocument.IsValidCnpj(cnpj);

        // Assert
        result.ShouldBeFalse();
    }
}
