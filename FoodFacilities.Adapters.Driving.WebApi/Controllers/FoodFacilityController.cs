using AutoMapper;
using FoodFacilities.Adapters.Driving.WebApi.Dtos;
using FoodFacilities.Domain.Exceptions;
using FoodFacilities.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace FoodFacilities.Adapters.Driving.WebApi.Controllers
{
    [Route("api/food-facility")]
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
        [SwaggerOperation(Summary = "Retrieves the list of food facilities that match the given applicant", Description = "Optional status filter should be used to filter out desired status, being: (APPROVED, EXPIRED, ISSUED, REQUESTED, SUSPEND) the supported values.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request successful.", typeof(List<FoodFacilityDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid filter/query parameters.", typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No food facilities found given the parameters.", typeof(string))]
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
        [SwaggerOperation(Summary = "Retrieves the list of food facilities that match the given street", Description = "This can be a part of the Address and it's case insensitive.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request successful.", typeof(List<FoodFacilityDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No food facilities found given the street.", typeof(string))]
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
        [SwaggerOperation(Summary = "Retrieves the top 5 food trucks facilities that are closest to the given latitude and longitude", Description = "Optional status filter should be used to filter out desired status, being: (APPROVED, EXPIRED, ISSUED, REQUESTED, SUSPEND) the supported values. If no status is sent APPROVED status is enforced!")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request successful.", typeof(List<FoodFacilityDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid filter/query parameters.", typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No food facilities found given the parameters.", typeof(string))]
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
