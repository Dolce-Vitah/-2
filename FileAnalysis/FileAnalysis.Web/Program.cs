using FileAnalysis.Infrastructure;
using FileAnalysis.Infrastructure.Database;
using FileAnalysis.UseCases;
using FileAnalysis.Web.Services;
using FileAnalysis.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();


builder.Services.AddHttpClient<IFileStorageService, FileStorageService>(client =>
{
    client.BaseAddress = new Uri("http://file-storing:8080");
});

builder.Services.AddUseCases();
builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FileAnalysis API", Version = "v1" });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileAnalysis API v1"));
}

app.UseAuthorization();
app.MapControllers();
app.Run();
