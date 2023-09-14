using FoodFacilities.Domain.Adapters.Driven;
using FoodFacilities.Domain.Adapters.Driving.Handlers;
using FoodFacilities.Domain.Adapters.Driving.Queries;
using FoodFacilities.Domain.Entities;

namespace FoodFacilities.Domain.Services.Handlers
{
    public class FoodFacilityHandler : IFoodFacilityHandler
    {
        private readonly IFoodFacilityRepository _foodFacilityRepository;

        public FoodFacilityHandler(IFoodFacilityRepository foodFacilityRepository) 
        {
            _foodFacilityRepository = foodFacilityRepository;
        }

        public async Task<ICollection<FoodFacility>> GetByApplicant(string applicant, ICollection<string> filterStatus)
        {
            return await _foodFacilityRepository.GetAsync(x =>
                x.Applicant == applicant
                &&
                (
                    !filterStatus.Any()
                    ||
                    (
                        x.Status is not null && filterStatus.Any() && filterStatus.Contains(x.Status)
                    )
                )
            );
        }

        public async Task<ICollection<FoodFacility>> GetByStreet(string street)
        {
            return await _foodFacilityRepository.GetAsync(x =>
                x.Address is not null && x.Address.ToLower().Contains(street.ToLower())
            );
        }
    }
}
