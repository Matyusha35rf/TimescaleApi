using DataAccess;
using BusinessLogic;

var builder = WebApplication.CreateBuilder(args);

// Добавляем слой доступа к данным (DataAccess)
// - Регистрирует DbContext для работы с PostgreSQL
// - Регистрирует репозитории (ValueRecordRepository, ResultRepository)
// - Строка подключения берется из appsettings.json
builder.Services.AddData(builder.Configuration);

// Добавляем слой бизнес-логики (BusinessLogic)
// - Регистрирует сервис для обработки CSV файлов (IFileProcessingService)
// - Содержит логику валидации, парсинга и расчета метрик
// - Использует репозитории из DataAccess
builder.Services.AddBusinessLogic();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();

/*// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();*/

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
