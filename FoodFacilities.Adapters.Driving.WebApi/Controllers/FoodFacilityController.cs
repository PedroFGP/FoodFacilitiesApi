using AutoMapper;
using FoodFacilities.Adapters.Driving.WebApi.Dtos;
using FoodFacilities.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FoodFacilities.Adapters.Driving.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class FoodFacilityController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IFoodFacilityService _foodFacilityService;

        public FoodFacilityController(IMapper mapper,
            IFoodFacilityService foodFacilityService) 
        {
            _mapper = mapper;
            _foodFacilityService = foodFacilityService;
        }

        [HttpGet("facilities/applicant")]
        [ProducesResponseType(typeof(FoodFacilityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<List<FoodFacilityDto>> GetFoodFacilitiesByApplicantAsync([FromQuery] string applicant, [FromQuery] string[]? status)
        {
            var result = await _foodFacilityService.GetByApplicantAsync(applicant, status);

            return _mapper.Map<List<FoodFacilityDto>>(result);
        }

        [HttpGet("facilities/street")]
        [ProducesResponseType(typeof(FoodFacilityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<List<FoodFacilityDto>> GetFoodFacilitiesByStreetAsync([FromQuery] string street)
        {
            var result = await _foodFacilityService.GetByStreetAsync(street);

            return _mapper.Map<List<FoodFacilityDto>>(result);
        }

        [HttpGet("facilities/nearest")]
        [ProducesResponseType(typeof(FoodFacilityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<List<FoodFacilityDto>> GetNearestFoodFacilitiesByGeolocationAsync([FromQuery] double? latitude, [FromQuery] double? longitude, [FromQuery] string[]? status)
        {
            var result = await _foodFacilityService.GetNearestFacilitiesAsync(latitude, longitude, status);

            return _mapper.Map<List<FoodFacilityDto>>(result);
        }
    }
}
