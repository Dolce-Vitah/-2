using Gateway.Web.Services;
using Gateway.Web.Services.Interfaces;
using Microsoft.OpenApi.Models; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient<IFileStorageService, FileStorageService>(client =>
{
    client.BaseAddress = new Uri("http://file-storing:8080");
});

builder.Services.AddHttpClient<IFileAnalysisService, FileAnalysisService>(client =>
{
    client.BaseAddress = new Uri("http://file-analysis:8080");
});
 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gateway API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API v1"));
}

app.UseRouting();
app.MapControllers();

app.Run();
