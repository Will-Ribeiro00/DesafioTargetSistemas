using DesafioTargetSistemas.Domain.Enums;

namespace DesafioTargetSistemas.Communication.Requests
{
    public class RequestRegisterStockMovementJson
    {
        public string Code { get; set; } = string.Empty;
        public StockMovementType Type { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
    }
}
