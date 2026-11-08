namespace DesafioTargetSistemas.Communication.Responses;

public class ResponsePaidAccountReceivableJson
{
    public DateOnly PaymentDate { get; set; }
    public decimal PaidAmount { get; set; }
}
