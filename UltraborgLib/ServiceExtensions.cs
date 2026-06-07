
using Microsoft.Extensions.DependencyInjection;
namespace Ultraborg.Library;
public static class ServiceExtensions
{
    public static IServiceCollection AddUltraBorgLibrary(this IServiceCollection services)
    {
        services.AddSingleton<IUltraborgAPI,UltraborgAPI>();
        return services;
    }   
}