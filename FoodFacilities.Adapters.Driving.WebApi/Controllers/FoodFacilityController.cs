using AutoMapper;
using FoodFacilities.Adapters.Driving.WebApi.Dtos;
using FoodFacilities.Domain.Adapters.Driving.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;

namespace FoodFacilities.Adapters.Driving.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class FoodFacilityController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IFoodFacilityHandler _foodFacilityHandler;

        public FoodFacilityController(IMapper mapper,
            IFoodFacilityHandler foodFacilityHandler) 
        {
            _mapper = mapper;
            _foodFacilityHandler = foodFacilityHandler;
        }

        [HttpGet("facilities")]
        [ProducesResponseType(typeof(FoodFacilityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<FoodFacilityDto> GetFoodFacilitiesByApplicant([FromQuery] string ApplicantName, [FromQuery] ICollection<string> status)
        {
            var result = await _foodFacilityHandler.GetByApplicant(ApplicantName, status);

            return _mapper.Map<FoodFacilityDto>(result);
        }

        [HttpGet("facilities/street")]
        [ProducesResponseType(typeof(FoodFacilityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<FoodFacilityDto> GetFoodFacilitiesByStreet([FromQuery] string street)
        {
            var result = await _foodFacilityHandler.GetByStreet(street);

            return _mapper.Map<FoodFacilityDto>(result);
        }
    }
}
