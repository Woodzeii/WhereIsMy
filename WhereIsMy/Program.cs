using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using WhereIsMy;

// БЛОК 1: Инициализация строителя приложения
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");




// БЛОК 2: Регистрация зависимостей в DI-контейнере
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString)); 
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddOpenApi(); // Регистрируем сервисы для генерации спецификации OpenAPI (Swagger)          

// БЛОК 3: Построение экземпляра приложения
var app = builder.Build();

// БЛОК 4: Настройка конвейера Middleware и запуск эндпоинтов
// Включаем OpenAPI только в режиме разработки (для безопасности production)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();   // Генерирует спецификацию в формате JSON
    
}

app.UseHttpsRedirection();


app.MapGet("/weather", () => new[] { "Sunny", "Cloudy", "Rainy" })
   .WithName("GetWeatherForecast")
   .WithDescription("Retrieves a list of weather forecasts."); 

app.Run(); 
