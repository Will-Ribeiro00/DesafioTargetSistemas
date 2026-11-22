namespace DesafioTargetSistemas.Communication.Requests;

public class RequestCustomerAddressJson
{
    public string ZipCode { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string? Number { get; set; }
    public string? Complement { get; set; }
    public string District { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}
