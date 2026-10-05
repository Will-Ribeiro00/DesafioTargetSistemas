namespace DesafioTargetSistemas.Domain.Entities
{
    public class Seller
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Sale> Sales { get; set; } = [];
    }
}
