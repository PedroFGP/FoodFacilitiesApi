using FoodFacilities.Application.Adapters.Driven.Csv;
using FoodFacilities.Application.Services;
using FoodFacilities.Domain.Adapters.Driven.Repositories;
using FoodFacilities.Domain.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(app => app.AddEnvironmentVariables("FoodFacility_"));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddTransient<IFoodFacilityService, FoodFacilityService>();
builder.Services.AddTransient<IFoodFacilityRepository, FoodFacilityCsvRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var swaggerDocName = "FoodFacilityApi";

builder.Services.AddSwaggerGen(options => 
{
    options.EnableAnnotations();
    options.SwaggerDoc(swaggerDocName, new OpenApiInfo 
    { 
        Title = "Food Facility API Challenge Swagger Documentation",
        Description = "API built to retrieve food facilities with different filters and parameters.",
        Version = "v1"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => 
    { 
        options.SwaggerEndpoint($"/swagger/{swaggerDocName}/swagger.json", "Food Facility API Challenge v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
