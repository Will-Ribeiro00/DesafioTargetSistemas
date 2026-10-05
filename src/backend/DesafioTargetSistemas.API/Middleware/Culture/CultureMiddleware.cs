using Microsoft.Extensions.Options;
using System.Globalization;

namespace DesafioTargetSistemas.API.Middleware.Culture
{
    public class CultureMiddleware(RequestDelegate next, IOptions<CultureSettings> cultureSettings)
    {
        private readonly CultureSettings _settings = cultureSettings.Value;

        public async Task Invoke(HttpContext context)
        {
            var requested = context.Request.GetTypedHeaders().AcceptLanguage
                .OrderByDescending(l => l.Quality ?? 1)
                .Select(l => l.Value.Value)
                .FirstOrDefault(v => v is not null &&
                    _settings.SupportedCultures.Contains(v, StringComparer.OrdinalIgnoreCase));

            var cultureInfo = new CultureInfo(requested ?? _settings.DefaultCulture);

            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            await next(context);
        }
    }
}
