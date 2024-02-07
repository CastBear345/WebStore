using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swagger.Repository;
using System.Text;
using WebStore;

var builder = WebApplication.CreateBuilder(args);

// ����������� ������������
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ��������� Swagger
builder.Services.AddSwaggerGen(c =>
{
    // ���������� ����������� ������������ ��� JWT-�������
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "��������� ����������� JWT � �������������� ����� Bearer. \r\n\r\n" +
            "������� 'Bearer' [������], � ����� ���� ����� � ��������� ���� ����. \r\n\r\n" +
            "��������: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });

    // ���������� ���������� ������������ ��� ������������� JWT-�������
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

// ��������� ����� JWT �� ������������
var key = builder.Configuration.GetValue<string>("AppSettings:Token");

// ��������� �������������� � �������������� JWT-�������
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

// ���������� ����������� � ���� ������
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ����������� �������������� � �����������
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
