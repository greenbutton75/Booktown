using Identity.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Services;

public static class FacebookAuthServiceExtensions
{
    public static IServiceCollection AddFacebookAuthService(this IServiceCollection services, IConfiguration Configuration)
    {
        var options = new FacebookAuthSettings();
        Configuration.GetSection(nameof(FacebookAuthSettings)).Bind(options);

        services.AddHttpClient(nameof(FacebookAuthService));

        services.AddSingleton<FacebookAuthSettings>(options);
        services.AddSingleton<IFacebookAuthService, FacebookAuthService>();


        return services;
    }
}
