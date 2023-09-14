using FoodFacilities.Domain.Adapters.Driven;
using FoodFacilities.Domain.Entities;

namespace FoodFacilities.Domain.Services
{
    public interface IFoodFacilityService
    {
        Task<ICollection<FoodFacility>> GetByApplicant(string applicant, string[]? filterStatus = null);

        Task<ICollection<FoodFacility>> GetByStreet(string street);

        Task<ICollection<FoodFacility>> GetNearestFacilities(double latitude, double longitude, string[]? filterStatus = null);
    }
}
