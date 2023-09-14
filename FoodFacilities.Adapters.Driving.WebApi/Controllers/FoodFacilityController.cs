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
        public async Task<List<FoodFacilityDto>> GetFoodFacilitiesByApplicant([FromQuery] string applicant, [FromQuery] string[] status)
        {
            status = status.Where(x => !string.IsNullOrEmpty(x) || x != "null").ToArray();

            var result = await _foodFacilityService.GetByApplicant(applicant, status);

            return _mapper.Map<List<FoodFacilityDto>>(result);
        }

        [HttpGet("facilities/street")]
        [ProducesResponseType(typeof(FoodFacilityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<List<FoodFacilityDto>> GetFoodFacilitiesByStreet([FromQuery] string street)
        {
            var result = await _foodFacilityService.GetByStreet(street);

            return _mapper.Map<List<FoodFacilityDto>>(result);
        }

        [HttpGet("facilities/nearest")]
        [ProducesResponseType(typeof(FoodFacilityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<List<FoodFacilityDto>> GetFoodFacilitiesByStreet([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] string[] status)
        {
            status = status.Where(x => !string.IsNullOrEmpty(x) || x != "null").ToArray();

            var result = await _foodFacilityService.GetNearestFacilities(latitude, longitude, status);

            return _mapper.Map<List<FoodFacilityDto>>(result);
        }
    }
}
