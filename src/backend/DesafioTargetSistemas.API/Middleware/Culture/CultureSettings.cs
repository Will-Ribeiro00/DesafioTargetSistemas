namespace DesafioTargetSistemas.API.Middleware.Culture
{
    public class CultureSettings
    {
        public string DefaultCulture { get; set; } = "en";
        public List<string> SupportedCultures { get; set; } = [];
    }
}
