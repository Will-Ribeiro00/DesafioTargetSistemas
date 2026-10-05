namespace DesafioTargetSistemas.Domain.Entities
{
    public class AccountReceivable
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly DueDate { get; set; }
        public DateOnly? PaymentDate { get; set; }

        public Sale Sale { get; set; } = null!;
    }
}
