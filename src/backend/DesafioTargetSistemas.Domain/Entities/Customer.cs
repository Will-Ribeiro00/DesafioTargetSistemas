using DesafioTargetSistemas.Domain.Enums;

namespace DesafioTargetSistemas.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public PersonType PersonType { get; set; }
    public string Document { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? TradeName { get; set; }
    public string? StateRegistration { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int SellerId { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Seller Seller { get; set; } = null!;
    public ICollection<CustomerAddress> Addresses { get; set; } = [];
}
