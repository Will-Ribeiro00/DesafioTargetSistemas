namespace DesafioTargetSistemas.Communication.Responses
{
    public class ResponseAccountReceivableJson
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly DueDate { get; set; }
        public int DaysOverdue { get; set; }
        public decimal Interest { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
