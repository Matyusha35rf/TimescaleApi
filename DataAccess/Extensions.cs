using DataAccess.Interfaces;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess
{
    /// <summary>
    /// Класс для регистрации сервисов слоя доступа к данным в DI контейнере
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Добавление всех сервисов DataAccess в DI контейнер
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <param name="configuration">Конфигурация приложения (для строки подключения)</param>
        /// <returns>Та же коллекция для цепочки вызовов</returns>
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            // Регистрация DbContext с PostgreSQL
            // Строка подключения берется из appsettings.json
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Регистрация репозиториев для работы с таблицами
            services.AddScoped<IValuesRepository, ValuesRepository>();   // для таблицы Values
            services.AddScoped<IResultsRepository, ResultsRepository>(); // для таблицы Results

            return services;
        }
    }
}