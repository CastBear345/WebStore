using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swagger.Repository;
using System.Text;
using WebStore;

var builder = WebApplication.CreateBuilder(args);

// Регистрация зависимостей
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Настройка Swagger
builder.Services.AddSwaggerGen(c =>
{
    // Добавление определения безопасности для JWT-токенов
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "Заголовок авторизации JWT с использованием схемы Bearer. \r\n\r\n" +
            "Введите 'Bearer' [пробел], а затем свой токен в текстовом поле ниже. \r\n\r\n" +
            "Например: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });

    // Добавление требования безопасности для использования JWT-токенов
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Получение ключа JWT из конфигурации
var key = builder.Configuration.GetValue<string>("AppSettings:Token");

// Настройка аутентификации с использованием JWT-токенов
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer(x => {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
});

// Добавление подключения к базе данных
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Подключение аутентификации и авторизации
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
