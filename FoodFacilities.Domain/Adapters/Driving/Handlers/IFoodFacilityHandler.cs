using FoodFacilities.Domain.Entities;

namespace FoodFacilities.Domain.Adapters.Driving.Handlers
{
    public interface IFoodFacilityHandler
    {
        Task<ICollection<FoodFacility>> GetByApplicant(string applicant, ICollection<string> filterStatus);

        Task<ICollection<FoodFacility>> GetByStreet(string street);
    }
}
