using FoodFacilities.Domain.Adapters.Driven.Repositories;
using FoodFacilities.Domain.Entities;
using FoodFacilities.Domain.Exceptions;
using FoodFacilities.Domain.Services;
using FoodFacilities.Domain.Utils;

namespace FoodFacilities.Application.Services
{
    public class FoodFacilityService : IFoodFacilityService
    {
        private readonly IFoodFacilityRepository _foodFacilityRepository;

        private readonly string[] ValidStatus = { "APPROVED", "EXPIRED", "ISSUED", "REQUESTED", "SUSPEND" };

        public FoodFacilityService(IFoodFacilityRepository foodFacilityRepository)
        {
            _foodFacilityRepository = foodFacilityRepository;
        }

        public async Task<ICollection<FoodFacility>> GetByApplicant(string applicant, string[]? filterStatus = null)
        {
            string[]? sanitizedStatus = null;

            if (filterStatus is not null && filterStatus.Any())
                sanitizedStatus = SanitizeStatus(filterStatus);

            var facilities = await _foodFacilityRepository.GetAsync(x =>
                x.Applicant == applicant
                &&
                (
                    sanitizedStatus is null 
                    || 
                    !sanitizedStatus.Any()
                    ||
                    x.Status is not null && sanitizedStatus.Contains(x.Status.ToUpper())
                )
            );

            if (!facilities.Any())
                throw new NoFoodFacilityFoundException();

            return facilities;
        }

        public async Task<ICollection<FoodFacility>> GetByStreet(string street)
        {
            var facilities = await _foodFacilityRepository.GetAsync(x =>
                x.Address is not null && x.Address.ToLower().Contains(street.ToLower())
            );

            if (!facilities.Any())
                throw new NoFoodFacilityFoundException();

            return facilities;
        }

        public async Task<ICollection<FoodFacility>> GetNearestFacilities(double latitude, double longitude, string[]? filterStatus = null)
        {
            Func<FoodFacility, bool>? filter;

            if(filterStatus is not null && filterStatus.Any())
            {
                filterStatus = SanitizeStatus(filterStatus);

                filter = x => x.Status is not null && filterStatus.Contains(x.Status.ToUpper());
            }
            else
            {
                filter = x => x.Status is not null && x.Status.ToUpper() == "APPROVED";
            }

            var facilities = await _foodFacilityRepository.GetAsync(filter);

            var facilitiesDistance = new SortedDictionary<long, double>();

            foreach ( var facility in facilities)
            {
                if (!facility.Latitude.HasValue || !facility.Longitude.HasValue || facility.Latitude.Value == 0.0f || facility.Longitude.Value == 0.0f)
                    continue;

                facilitiesDistance[facility.Id] = LocationUtils.CalculateDistance(latitude, longitude, facility.Latitude.Value, facility.Longitude.Value);
            }

            var nearestFailicitesIds = facilitiesDistance.OrderBy(x => x.Value).Take(5).Select(x => x.Key);

            var nearestFailicites = facilities.Where(x => nearestFailicitesIds.Contains(x.Id)).ToList();

            if (!nearestFailicites.Any())
                throw new NoFoodFacilityFoundException();

            return nearestFailicites;
        }

        private string[] SanitizeStatus(string[] status)
        {
            return status.Select(s => s.ToUpper()).Where(s => ValidStatus.Contains(s)).ToArray();
        }
    }
}
