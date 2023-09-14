using CsvHelper;
using CsvHelper.Configuration;
using FoodFacilities.Application.Adapters.Driven.Csv.Mapping;
using FoodFacilities.Domain.Adapters.Driven.Repositories;
using FoodFacilities.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace FoodFacilities.Application.Adapters.Driven.Csv
{
    public class FoodFacilityCsvRepository : IFoodFacilityRepository
    {
        private readonly IConfiguration _configuration;

        public FoodFacilityCsvRepository(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public async Task<ICollection<FoodFacility>> GetAsync(Func<FoodFacility, bool>? filter = null)
        {
            var connectionString = _configuration["FoodFacility_ConnectionString"];

            if (connectionString is null)
                throw new ArgumentNullException(nameof(connectionString));

            using (var reader = new StreamReader(connectionString))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<FoodFacilityMapDefinition>();

                var facilities = new List<FoodFacility>();
                while (await csv.ReadAsync())
                {
                    var record = csv.GetRecord<FoodFacility>();

                    if(record is null)
                        continue;

                    if (filter is null || filter(record))
                    {
                        facilities.Add(record);
                    }
                }

                return facilities;
            }
        }
    }
}
