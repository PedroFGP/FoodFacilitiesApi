using FoodFacilities.Application.Adapters.Driven.Csv;
using FoodFacilities.Application.Services;
using FoodFacilities.Domain.Adapters.Driven.Repositories;
using FoodFacilities.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(app => app.AddEnvironmentVariables("FoodFacility_"));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddTransient<IFoodFacilityService, FoodFacilityService>();
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
