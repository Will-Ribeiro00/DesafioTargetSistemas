using DesafioTargetSistemas.Domain.Enums;

namespace DesafioTargetSistemas.Communication.Requests;

public class RequestRegisterCustomerJson
{
    public string Name { get; set; } = string.Empty;
    public PersonType PersonType { get; set; }
    public string Document { get; set; } = string.Empty;
    public string? TradeName { get; set; }
    public string? StateRegistration { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int SellerId { get; set; }
    public RequestCustomerAddressJson Address { get; set; } = new();
}
