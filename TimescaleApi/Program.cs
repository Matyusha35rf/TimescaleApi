using Microsoft.EntityFrameworkCore;
using DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Добавляем слой доступа к данным (DataAccess)
// - Регистрирует DbContext для работы с PostgreSQL
// - Регистрирует репозитории (ValueRecordRepository, ResultRepository)
// - Строка подключения берется из appsettings.json
builder.Services.AddData(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
