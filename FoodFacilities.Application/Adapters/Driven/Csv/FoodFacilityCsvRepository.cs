using CsvHelper;
using FoodFacilities.Application.Adapters.Driven.Csv1571753.Mapping;
using FoodFacilities.Domain.Adapters.Driven;
using FoodFacilities.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace FoodFacilities.Application.Adapters.Driven.Csv
{
    public class FoodFacilityCsvRepository : IFoodFacilityRepository
    {
        private readonly IConfiguration _configuration;

        public FoodFacilityCsvRepository(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public Task<ICollection<FoodFacility>> GetAsync(Func<FoodFacility, bool> filter)
        {
            var connectionString = _configuration["FoodFacility_ConnectionString"];

            if (connectionString is null)
                throw new ArgumentNullException(nameof(connectionString));

            using (var reader = new StreamReader(connectionString))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<FoodFacilityMapDefinition>();

                var t = csv.GetRecords<dynamic>();

                var records = csv.GetRecords<FoodFacility>().Where(filter).ToList();

                return Task.FromResult<ICollection<FoodFacility>>(records);
            }
        }
    }
}
