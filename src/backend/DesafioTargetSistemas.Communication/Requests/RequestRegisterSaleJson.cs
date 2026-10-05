namespace DesafioTargetSistemas.Communication.Requests
{
    public class RequestRegisterSaleJson
    {
        public int SellerId { get; set; }
        public DateOnly DueDate { get; set; }
        public List<RequestSaleItemJson> Items { get; set; } = [];
    }
}
