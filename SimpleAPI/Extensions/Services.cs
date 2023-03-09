namespace Microsoft.Extensions.DependencyInjection;

using SimpleAPI.Data;
using SimpleAPI.Interfaces;
using SimpleAPI.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLocalServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddSingleton<ICustomerStore, InMemoryCustomerStore>();
        services.AddTransient<IAiChatService, AiChatService>();
#if DEBUG
        services.AddTransient<IMailService, LocalMailService>();
#else 
        services.AddTransient<IMailService, CloudMailService>();
#endif

        return services;
    }
}