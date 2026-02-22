using DataAccess;
using BusinessLogic;

var builder = WebApplication.CreateBuilder(args);

// Добавляем слой доступа к данным (DataAccess)
// - Регистрирует DbContext для работы с PostgreSQL
// - Регистрирует репозитории (ValuesRepository, ResultsRepository)
// - Строка подключения берется из appsettings.json
builder.Services.AddData(builder.Configuration);

// Добавляем слой бизнес-логики (BusinessLogic)
// - Регистрирует сервисы: IFileProcessingService, IResultQueryService, IValueQueryService
// - Содержит логику валидации, парсинга, расчета метрик и формирования DTO
builder.Services.AddBusinessLogic();

// Регистрация контроллеров API
builder.Services.AddControllers();

// Регистрация Swagger для документации и тестирования API
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Подключаем маршрутизацию контроллеров
app.MapControllers();

// Включаем Swagger UI (доступен по /swagger)
app.UseSwagger();
app.UseSwaggerUI();

app.Run();