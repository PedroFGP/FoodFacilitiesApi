using AutoMapper;
using FoodFacilities.Adapters.Driving.WebApi.Dtos;
using FoodFacilities.Domain.Entities;
using System.Net;

namespace FoodFacilities.Adapters.Driving.WebApi.Mapping
{
    public class FoodFacilityMapping : Profile
    {
        public FoodFacilityMapping()
        {
            CreateMap<FoodFacility, FoodFacilityDto>();
        }
    }
}
