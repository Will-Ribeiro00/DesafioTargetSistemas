namespace DesafioTargetSistemas.Communication.Responses
{
    public class ResponseSellerCommissionJson
    {
        public int SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public int SalesCount { get; set; }
        public decimal TotalSold { get; set; }
        public decimal TotalCommission { get; set; }
    }
}
