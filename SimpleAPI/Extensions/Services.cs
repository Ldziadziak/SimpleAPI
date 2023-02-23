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
        services.AddTransient<ChatGptService, ChatGptService>();
        return services;
    }
}