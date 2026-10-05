namespace DesafioTargetSistemas.Domain.Entities
{
    public class StockMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? SaleId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int StockBalance { get; set; }
        public DateTime MovementDate { get; set; }

        public Product Product { get; set; } = null!;
        public Sale? Sale { get; set; }
    }
}
