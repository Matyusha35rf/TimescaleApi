using Microsoft.Extensions.DependencyInjection;
using BusinessLogic.Services;
using BusinessLogic.Services.Interfaces;

namespace BusinessLogic
{
    /// <summary>
    /// Класс для регистрации сервисов бизнес-логики в DI контейнере
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Добавление всех сервисов бизнес-логики в DI контейнер
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <returns>Та же коллекция для цепочки вызовов</returns>
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            // Сервис для обработки CSV файлов (МЕТОД 1)
            services.AddScoped<IFileProcessingService, FileProcessingService>();

            // Сервис для запросов к таблице Results (МЕТОД 2)
            services.AddScoped<IResultQueryService, ResultQueryService>();

            // Сервис для запросов к таблице Values (МЕТОД 3)
            services.AddScoped<IValueQueryService, ValueQueryService>();

            return services;
        }
    }
}