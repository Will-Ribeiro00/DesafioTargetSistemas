namespace DesafioTargetSistemas.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CurrentStock { get; set; }

        public ICollection<SaleItem> SaleItems { get; set; } = [];
        public ICollection<StockMovement> StockMovements { get; set; } = [];
    }
}
