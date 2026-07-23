using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using WhereIsMy;
using RabbitMQ.Client;
using MassTransit;

// БЛОК 1: Инициализация строителя приложения
var builder = WebApplication.CreateBuilder(args);






// БЛОК 2: Регистрация зависимостей в DI-контейнере

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Настройка MassTransit
builder.Services.AddMassTransit(x =>
{
    // 1. Регистрируем потребителя (Consumer), который будет обрабатывать сообщения
    x.AddConsumer<MoveItemConsumer>();

    // 2. Настраиваем транспорт RabbitMQ
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Автоматически создает очереди для всех зарегистрированных Consumer'ов
        cfg.ConfigureEndpoints(context);
    });
});
//builder.Services.AddHostedService<MoveItemConsumer>();

builder.Services.AddControllers();

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

app.Run(); 
