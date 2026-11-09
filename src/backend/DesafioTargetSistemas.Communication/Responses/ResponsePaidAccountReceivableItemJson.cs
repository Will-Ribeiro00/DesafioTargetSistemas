namespace DesafioTargetSistemas.Communication.Responses;

public class ResponsePaidAccountReceivableItemJson
{
    public int SaleId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly PaymentDate { get; set; }
    public int DaysOverdue { get; set; }
    public decimal Interest { get; set; }
    public decimal PaidAmount { get; set; }
}
