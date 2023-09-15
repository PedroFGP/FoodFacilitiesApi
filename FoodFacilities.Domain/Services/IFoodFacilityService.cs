using FoodFacilities.Domain.Adapters.Driven;
using FoodFacilities.Domain.Entities;

namespace FoodFacilities.Domain.Services
{
    public interface IFoodFacilityService
    {
        Task<ICollection<FoodFacility>> GetByApplicantAsync(string applicant, string[]? filterStatus = null);

        Task<ICollection<FoodFacility>> GetByStreetAsync(string street);

        Task<ICollection<FoodFacility>> GetNearestFacilitiesAsync(double? latitude, double? longitude, string[]? filterStatus = null);
    }
}
