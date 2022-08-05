using Identity.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Services;

public static class GoogleAuthServiceExtensions
{
    public static IServiceCollection AddGoogleAuthService(this IServiceCollection services, IConfiguration Configuration)
    {
        var options = new GoogleAuthSettings();
        Configuration.GetSection(nameof(GoogleAuthSettings)).Bind(options);
        //services.Configure<FacebookAuthSettings>(Configuration.GetSection(nameof(FacebookAuthSettings)));

        services.AddHttpClient(nameof(GoogleAuthSettings));

        services.AddSingleton<GoogleAuthSettings>(options);
        services.AddSingleton<IGoogleAuthService, GoogleAuthService>();

        return services;

    }
}
