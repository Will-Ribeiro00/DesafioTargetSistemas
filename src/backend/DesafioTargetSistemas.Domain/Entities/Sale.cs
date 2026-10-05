namespace DesafioTargetSistemas.Domain.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public int SellerId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal CommissionPercentage { get; set; }
        public decimal CommissionAmount { get; set; }

        public Seller Seller { get; set; } = null!;
        public ICollection<SaleItem> Items { get; set; } = [];
        public ICollection<StockMovement> StockMovements { get; set; } = [];
        public ICollection<AccountReceivable> AccountsReceivable { get; set; } = [];
    }
}
