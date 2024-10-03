using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RockPaperScissorsAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RockPaperScissorsAPI", Version = "v1" });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins("http://localhost:5173") // Replace with your front-end origin if needed
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RockPaperScissorsAPI v1"));
}

// Serve static files from wwwroot
app.UseStaticFiles();

// Use default files like index.html from wwwroot
app.UseDefaultFiles();

// Use routing
app.UseRouting();

// Use the CORS policy
app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

// Map controllers and SignalR hubs
app.MapControllers();
app.MapHub<GameHub>("/gamehub");

app.Run();
