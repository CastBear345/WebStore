using Microsoft.EntityFrameworkCore;
using Swagger.Repository;
using WebStore;
using WebStore.Services.Interfacies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "frontend_cors",
                      policy =>
                      {
                          policy.WithOrigins("*");
                      });
});

// ����������� ������������
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ��������� �������������� � �������������� JWT-�������
builder.Services.AddAuthentication().AddCookie("cookie");

// ���������� ����������� � ���� ������
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => 
    builder.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod()
);

// ����������� �������������� � �����������
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
