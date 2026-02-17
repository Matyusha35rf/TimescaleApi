using Microsoft.Extensions.DependencyInjection;
using BusinessLogic.Services;

namespace BusinessLogic
{
    public static class Extensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            // Регистрация сервисов бизнес-логики
            services.AddScoped<IFileProcessingService, FileProcessingService>();
            return services;
        }
    }
}
