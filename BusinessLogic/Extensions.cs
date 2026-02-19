using Microsoft.Extensions.DependencyInjection;
using BusinessLogic.Services;
using BusinessLogic.Services.Interfaces;

namespace BusinessLogic
{
    public static class Extensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            // Регистрация сервисов бизнес-логики
            services.AddScoped<IFileProcessingService, FileProcessingService>();
            services.AddScoped<IResultQueryService, ResultQueryService>();
            services.AddScoped<IValueQueryService, ValueQueryService>();
            return services;
        }
    }
}
