using FoodFacilities.Domain.Adapters.Driven.Repositories;
using FoodFacilities.Domain.Entities;
using FoodFacilities.Domain.Exceptions;
using FoodFacilities.Domain.Services;
using FoodFacilities.Domain.Utils;
using System.IO;

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

        public async Task<ICollection<FoodFacility>> GetByApplicantAsync(string applicant, string[]? filterStatus = null)
        {
            if (filterStatus is not null && !filterStatus.All(x => ValidStatus.Contains(x)))
                throw new FoodFacilityInvalidFilterException($"Invalid food facility status. Allowed values are: {string.Join(',', ValidStatus)}.");

            if (string.IsNullOrEmpty(applicant))
                throw new FoodFacilityInvalidFilterException($"Food facility applicant filter cannot be null nor empty.");

            var facilities = await _foodFacilityRepository.GetAsync(x =>
                x.Applicant is not null 
                && 
                x.Applicant.ToLower() == applicant.ToLower()
                &&
                (
                    filterStatus is null 
                    || 
                    !filterStatus.Any()
                    ||
                    x.Status is not null && filterStatus.Contains(x.Status.ToUpper())
                )
            );

            if (!facilities.Any())
                throw new FoodFacilityNotFoundException("No food facility found for the given applicant.");

            return facilities;
        }

        public async Task<ICollection<FoodFacility>> GetByStreetAsync(string street)
        {
            if (string.IsNullOrEmpty(street))
                throw new FoodFacilityInvalidFilterException($"Food facility street filter cannot be null nor empty.");

            var facilities = await _foodFacilityRepository.GetAsync(x =>
                x.Address is not null && x.Address.ToLower().Contains(street.ToLower())
            );

            if (!facilities.Any())
                throw new FoodFacilityNotFoundException("No food facility found for the given street.");

            return facilities;
        }

        public async Task<ICollection<FoodFacility>> GetNearestFacilitiesAsync(double? latitude, double? longitude, string[]? filterStatus = null)
        {
            if (!latitude.HasValue || !longitude.HasValue)
                throw new FoodFacilityInvalidFilterException($"Food facility latitude/longitude filter cannot be null.");

            Func<FoodFacility, bool>? filter;

            if (filterStatus is not null && !filterStatus.All(x => ValidStatus.Contains(x)))
                throw new FoodFacilityInvalidFilterException($"Invalid food facility status. Allowed values are: {string.Join(',', ValidStatus)}");

            if (filterStatus is not null && filterStatus.Any())
            {
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

                facilitiesDistance[facility.Id] = LocationUtils.CalculateDistance(latitude.Value, longitude.Value, facility.Latitude.Value, facility.Longitude.Value);
            }

            var nearestFailicitesIds = facilitiesDistance.OrderBy(x => x.Value).Take(5).Select(x => x.Key);

            var nearestFailicites = facilities.Where(x => nearestFailicitesIds.Contains(x.Id)).ToList();

            if (!nearestFailicites.Any())
                throw new FoodFacilityNotFoundException("No food facility found for the given latitude/longitude.");

            return nearestFailicites;
        }
    }
}
