using FoodFacilities.Application.Adapters.Driven.Csv;
using FoodFacilities.Domain.Adapters.Driven;
using FoodFacilities.Domain.Adapters.Driving.Handlers;
using FoodFacilities.Domain.Services.Handlers;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(app => app.AddEnvironmentVariables("FoodFacility_"));

// Add services to the container.
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddTransient<IFoodFacilityHandler, FoodFacilityHandler>();
builder.Services.AddTransient<IFoodFacilityRepository, FoodFacilityCsvRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
