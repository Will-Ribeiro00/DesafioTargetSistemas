namespace DesafioTargetSistemas.Communication.Responses
{
    public class ResponseProductJson
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CurrentStock { get; set; }
    }
}
