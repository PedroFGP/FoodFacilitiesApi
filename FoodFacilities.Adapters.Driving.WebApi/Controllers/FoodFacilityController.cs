using AutoMapper;
using FoodFacilities.Adapters.Driving.WebApi.Dtos;
using FoodFacilities.Domain.Exceptions;
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
        public async Task<IActionResult> GetFoodFacilitiesByApplicantAsync([FromQuery] string applicant, [FromQuery] string[]? status)
        {
            try
            {
                var result = await _foodFacilityService.GetByApplicantAsync(applicant, status);

                return StatusCode(StatusCodes.Status200OK, _mapper.Map<List<FoodFacilityDto>>(result));
            }
            catch(FoodFacilityBaseException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }

        [HttpGet("facilities/street")]
        [ProducesResponseType(typeof(FoodFacilityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFoodFacilitiesByStreetAsync([FromQuery] string street)
        {
            try
            {
                var result = await _foodFacilityService.GetByStreetAsync(street);

                return StatusCode(StatusCodes.Status200OK, _mapper.Map<List<FoodFacilityDto>>(result));
            }
            catch (FoodFacilityBaseException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }

        [HttpGet("facilities/nearest-food-trucks")]
        [ProducesResponseType(typeof(FoodFacilityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNearestFoodTruckFacilitiesByGeolocationAsync([FromQuery] double? latitude, [FromQuery] double? longitude, [FromQuery] string[]? status)
        {
            try
            {
                var result = await _foodFacilityService.GetNearestFoodTruckFacilitiesAsync(latitude, longitude, status);

                return StatusCode(StatusCodes.Status200OK, _mapper.Map<List<FoodFacilityDto>>(result));
            }
            catch (FoodFacilityBaseException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }
    }
}
