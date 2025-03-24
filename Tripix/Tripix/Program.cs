using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Tripix.Context;
using Tripix.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.SetIsOriginAllowed(origin => true) // ���� ��� Origin
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Access-Control-Allow-Origin")); // ��� ������
});

Env.Load();

builder.Configuration.AddEnvironmentVariables();




builder.Services.AddHttpClient<bininfoRepo>();
builder.Services.AddDbContext<ApplicationDbcontext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionString"));
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ����� ��� Middleware ��� ���� �� UseCors ��� �� ��� ���
app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
