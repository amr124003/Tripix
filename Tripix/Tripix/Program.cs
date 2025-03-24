using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Tripix.Context;
using Tripix.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.SetIsOriginAllowed(origin => true) // Ì”„Õ »√Ì Origin
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Access-Control-Allow-Origin")); // ﬂ‘› «·ÂÌœ—
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

//  — Ì» «·‹ Middleware „Â„ Ãœ«° ÷⁄ UseCors ﬁ»· √Ì ‘Ì¡ ¬Œ—
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
