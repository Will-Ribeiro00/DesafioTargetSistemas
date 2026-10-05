namespace DesafioTargetSistemas.Communication.Responses
{
    public class ResponseSaleJson
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public decimal CommissionPercentage { get; set; }
        public decimal CommissionAmount { get; set; }
    }
}
