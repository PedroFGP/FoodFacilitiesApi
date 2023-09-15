using AutoMapper;
using Castle.Core.Configuration;
using FoodFacilities.Adapters.Driving.WebApi.Controllers;
using FoodFacilities.Adapters.Driving.WebApi.Dtos;
using FoodFacilities.Adapters.Driving.WebApi.Mapping;
using FoodFacilities.Application.Adapters.Driven.Csv;
using FoodFacilities.Application.Services;
using FoodFacilities.Domain.Adapters.Driven.Repositories;
using FoodFacilities.Domain.Entities;
using FoodFacilities.Domain.Exceptions;
using FoodFacilities.Domain.Services;
using FoodFacilities.Domain.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IO;
using static System.Net.WebRequestMethods;

namespace FoodFacilities.Test
{
    public class FoodFacilityControllerTest
    {
        private readonly IMapper _mockMapper;
        private readonly FoodFacilityCsvRepository _foodFacilityCsvRepository;
        private readonly FoodFacilityService _foodFacilityService;

        public FoodFacilityControllerTest()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"FoodFacility_ConnectionString", "./Database/MockDatabase.csv"}
            };

            Microsoft.Extensions.Configuration.IConfiguration mockConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _foodFacilityCsvRepository = new FoodFacilityCsvRepository(mockConfiguration);

            _foodFacilityService = new FoodFacilityService(_foodFacilityCsvRepository);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new FoodFacilityMapping());
            });

            _mockMapper = mockMapper.CreateMapper();
        }

        #region GetByApplicant Tests

        [Theory]
        [InlineData("Donavan Fletcher Truck", null)]
        [InlineData("Break Break", null)]
        [InlineData("Brazuca Grill", null)]
        public void FoodFacilityGetByApplicantSuccessWithoutStatus(string applicant, string[]? status)
        {
            var filteredFacilities = GetFoodFacilitiesData().Where(x => x.Applicant is not null && x.Applicant.ToLower() == applicant.ToLower()).ToList();

            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetFoodFacilitiesByApplicantAsync(applicant, status).Result as ObjectResult;

            var foodFacilitiesResult = foodFacilitiesActionResult?.Value as List<FoodFacilityDto>;

            Assert.Equal(StatusCodes.Status200OK, foodFacilitiesActionResult?.StatusCode);
            Assert.NotNull(foodFacilitiesResult);
            Assert.Equal(filteredFacilities.Count(), foodFacilitiesResult.Count());
            Assert.Equal(filteredFacilities.Select(x => x.Id), foodFacilitiesResult.Select(x => x.Id));
        }

        [Theory]
        [InlineData("Donavan Fletcher Truck", new string[] { "APPROVED" })]
        [InlineData("Break Break", new string[] { "APPROVED" })]
        [InlineData("Break Break", new string[] { "APPROVED", "REQUESTED" })]
        [InlineData("Brazuca Grill", new string[] { "EXPIRED" })]
        [InlineData("Natan's Catering", new string[] { "ISSUED" })]
        [InlineData("Red Lobster", new string[] { "ISSUED" })]
        public void FoodFacilityGetByApplicantSuccessWithtStatus(string applicant, string[]? status)
        {
            var filteredFacilities = GetFoodFacilitiesData().Where(x => x.Applicant is not null && x.Applicant.ToLower() == applicant.ToLower() && status is not null && x.Status is not null && status.Contains(x.Status.ToUpper())).ToList();

            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetFoodFacilitiesByApplicantAsync(applicant, status).Result as ObjectResult;

            var foodFacilitiesResult = foodFacilitiesActionResult?.Value as List<FoodFacilityDto>;

            Assert.Equal(StatusCodes.Status200OK, foodFacilitiesActionResult?.StatusCode);
            Assert.NotNull(foodFacilitiesResult);
            Assert.Equal(filteredFacilities.Count(), foodFacilitiesResult.Count());
            Assert.Equal(filteredFacilities.Select(x => x.Id), foodFacilitiesResult.Select(x => x.Id));
        }

        [Theory]
        [InlineData("Hector Tacos")]
        [InlineData("Hope's Breakfast")]
        [InlineData("Henrico's Pizza")]
        public void FoodFacilityGetByApplicantFailNotFoundWithoutStatus(string applicant)
        {
            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetFoodFacilitiesByApplicantAsync(applicant, null).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status404NotFound, foodFacilitiesActionResult?.StatusCode);
        }

        [Theory]
        [InlineData("Hector Tacos", new string[] { "APPROVED" })]
        [InlineData("Hope's Breakfast", new string[] { "REQUESTED" })]
        [InlineData("Henrico's Pizza", new string[] { "SUSPEND" })]
        public void FoodFacilityGetByApplicantFailNotFoundWithStatus(string applicant, string[]? status)
        {
            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetFoodFacilitiesByApplicantAsync(applicant, status).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status404NotFound, foodFacilitiesActionResult?.StatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void FoodFacilityGetByApplicantFailInvalidFilterWithoutStatus(string applicant)
        {
            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetFoodFacilitiesByApplicantAsync(applicant, null).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status400BadRequest, foodFacilitiesActionResult?.StatusCode);
        }

        [Theory]
        [InlineData("", new string[] { "APPROVED" })]
        [InlineData(null, new string[] { "APPROVED" })]
        public void FoodFacilityGetByApplicantFailInvalidFilterWithStatus(string applicant, string[]? status)
        {
            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetFoodFacilitiesByApplicantAsync(applicant, status).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status400BadRequest, foodFacilitiesActionResult?.StatusCode);
        }

        #endregion

        #region GetByStreet Tests

        [Theory]
        [InlineData("MAR")]
        [InlineData("FRANK")]
        [InlineData("GROV")]
        public void FoodFacilityGetByStreetSuccess(string street)
        {
            var filteredFacilities = GetFoodFacilitiesData().Where(x => x.Address is not null && x.Address.ToLower().Contains(street.ToLower())).ToList();

            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetFoodFacilitiesByStreetAsync(street).Result as ObjectResult;

            var foodFacilitiesResult = foodFacilitiesActionResult?.Value as List<FoodFacilityDto>;

            Assert.Equal(StatusCodes.Status200OK, foodFacilitiesActionResult?.StatusCode);
            Assert.NotNull(foodFacilitiesResult);
            Assert.Equal(filteredFacilities.Count(), foodFacilitiesResult.Count());
            Assert.Equal(filteredFacilities.Select(x => x.Id), foodFacilitiesResult.Select(x => x.Id));
        }

        [Theory]
        [InlineData("CECILIA")]
        [InlineData("HOPE")]
        [InlineData("CONTOR")]
        public void FoodFacilityGetByStreetFailNotFound(string street)
        {
            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetFoodFacilitiesByStreetAsync(street).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status404NotFound, foodFacilitiesActionResult?.StatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void FoodFacilityGetByStreetFailInvalidFilter(string street)
        {
            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetFoodFacilitiesByStreetAsync(street).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status400BadRequest, foodFacilitiesActionResult?.StatusCode);
        }

        #endregion

        #region GetNearestFoodFacilitiesByGeolocationAsync Tests

        [Theory]
        [InlineData(37.79238985, -122.4012698, null)]
        [InlineData(37.73911140, -122.382469, null)]
        [InlineData(37.78127595, -122.4318404, null)]
        public void FoodFacilityGetNearestFacilitiesSuccessWithoutStatus(double? latitude, double? longitude, string[]? status)
        {
            var validaFacilities = GetFoodFacilitiesData().Where(x => x.FacilityType == "Truck" && x.Status is not null && x.Status == "APPROVED").ToList();

            var filteredFacilities = GetNearestFacilities(validaFacilities, latitude, longitude);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetNearestFoodTruckFacilitiesByGeolocationAsync(latitude, longitude, status).Result as ObjectResult;

            var foodFacilitiesResult = foodFacilitiesActionResult?.Value as List<FoodFacilityDto>;

            Assert.Equal(StatusCodes.Status200OK, foodFacilitiesActionResult?.StatusCode);
            Assert.NotNull(foodFacilitiesResult);
            Assert.Equal(filteredFacilities.Count(), foodFacilitiesResult.Count());
            Assert.Equal(filteredFacilities.Select(x => x.Id), foodFacilitiesResult.Select(x => x.Id));
        }

        [Theory]
        [InlineData(37.79238985, -122.4012698, new string[] { "EXPIRED" })]
        [InlineData(37.73911140, -122.382469, new string[] { "APPROVED", "REQUESTED", "ISSUED" })]
        [InlineData(37.78127595, -122.4318404, new string[] { "APPROVED", "EXPIRED", "ISSUED", "REQUESTED", "SUSPEND" })]
        public void FoodFacilityGetNearestFacilitiesSuccessWithStatus(double? latitude, double? longitude, string[]? status)
        {
            var validaFacilities = GetFoodFacilitiesData().Where(x => x.FacilityType == "Truck" && x.Status is not null && status is not null && status.Contains(x.Status)).ToList();

            var filteredFacilities = GetNearestFacilities(validaFacilities, latitude, longitude);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetNearestFoodTruckFacilitiesByGeolocationAsync(latitude, longitude, status).Result as ObjectResult;

            var foodFacilitiesResult = foodFacilitiesActionResult?.Value as List<FoodFacilityDto>;

            Assert.Equal(StatusCodes.Status200OK, foodFacilitiesActionResult?.StatusCode);
            Assert.NotNull(foodFacilitiesResult);
            Assert.Equal(filteredFacilities.Count(), foodFacilitiesResult.Count());
            Assert.Equal(filteredFacilities.Select(x => x.Id), foodFacilitiesResult.Select(x => x.Id));
        }

        [Theory]
        [InlineData(37.79238985, -122.4012698, new string[] { "SUSPEND" })]
        [InlineData(37.73911140, -122.382469, new string[] { "SUSPEND" })]
        [InlineData(37.78127595, -122.4318404, new string[] { "SUSPEND" })]
        public void FoodFacilityGetNearestFacilitiesFailNotFound(double? latitude, double? longitude, string[]? status)
        {
            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetNearestFoodTruckFacilitiesByGeolocationAsync(latitude, longitude, status).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status404NotFound, foodFacilitiesActionResult?.StatusCode);
        }

        [Theory]
        [InlineData(37.79238985, -122.4012698, new string[] { "STATUS1" })]
        [InlineData(37.73911140, -122.382469, new string[] { "STATUS1", "STATUS2", "STATUS3" })]
        [InlineData(37.78127595, -122.4318404, new string[] { "APPROVED", "EXPIRED", "STATUS4", "REQUESTED", "SUSPEND" })]
        [InlineData(null, null, new string[] { "APPROVED", "EXPIRED", "REQUESTED", "SUSPEND" })]
        [InlineData(null, null, new string[] { "APPROVED", "EXPIRED", "SUSPEND" })]
        [InlineData(null, null, new string[] { "APPROVED", "EXPIRED" })]
        public void FoodFacilityGetNearestFacilitiesFailInvalidFilter(double? latitude, double? longitude, string[]? status)
        {
            var foodFacilityController = new FoodFacilityController(_mockMapper, _foodFacilityService);

            var foodFacilitiesActionResult = foodFacilityController.GetNearestFoodTruckFacilitiesByGeolocationAsync(latitude, longitude, status).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status400BadRequest, foodFacilitiesActionResult?.StatusCode);
        }

        #endregion

        #region MockData

        private List<FoodFacility> GetFoodFacilitiesData()
        {
            var productsData = new List<FoodFacility>
            {
                new FoodFacility
                {
                    Id = 1,
                    Applicant = "Lester Miles Lunch",
                    Address = "211 SANFORD ST",
                    Status = "APPROVED",
                    Latitude = 37.79238986,
                    Longitude = -122.4012697,
                    FacilityType = "Truck"
                },
                new FoodFacility
                {
                    Id = 2,
                    Applicant = "Donavan Fletcher Truck",
                    Address = "1265 GROVE ST",
                    Status = "APPROVED",
                    Latitude = 37.794,
                    Longitude = -122.4013,
                    FacilityType = "Truck"
                },
                new FoodFacility
                {
                    Id = 3,
                    Applicant = "Sun & Moon Delicacy",
                    Address = "15 MARINA BLVD",
                    Status = "REQUESTED",
                    Latitude = 37.78484603,
                    Longitude = -122.4225681,
                    FacilityType = "Truck"
                },
                new FoodFacility
                {
                    Id = 4,
                    Applicant = "Break Break",
                    Address = "1188 FRANKLIN ST",
                    Status = "APPROVED",
                    Latitude = 37.73911143,
                    Longitude = -122.382465,
                    FacilityType = "Truck"
                },
                new FoodFacility
                {
                    Id = 5,
                    Applicant = "Break Break",
                    Address = "2111 FRANKLIN ST",
                    Status = "REQUESTED",
                    Latitude = 37.73911148,
                    Longitude = -122.382466,
                    FacilityType = "Truck"
                },
                new FoodFacility
                {
                    Id = 6,
                    Applicant = "Brazuca Grill",
                    Address = "90 BROADWAY",
                    Status = "EXPIRED",
                    Latitude = 37.77522831,
                    Longitude = -122.4174661,
                    FacilityType = "Truck"
                },
                new FoodFacility
                {
                    Id = 7,
                    Applicant = "Natan's Catering",
                    Address = "251 GEARY ST",
                    Status = "ISSUED",
                    Latitude = 37.78127595,
                    Longitude = -122.4318404,
                    FacilityType = "Push Cart"
                },
                new FoodFacility
                {
                    Id = 8,
                    Applicant = "Red Lobster",
                    Address = "1420 YOSEMITE AVE",
                    Status = "ISSUED",
                    Latitude = 37.78127575,
                    Longitude = -122.4318414,
                    FacilityType = "Push Cart"
                }
            };

            return productsData;
        }

        #endregion

        #region Helpers

        private List<FoodFacility> GetNearestFacilities(List<FoodFacility> facilities, double? latitude, double? longitude)
        {
            if (!latitude.HasValue || !longitude.HasValue)
                return new List<FoodFacility>();

            var facilitiesDistance = new SortedDictionary<long, double>();

            foreach (var facility in facilities)
            {
                if (!facility.Latitude.HasValue || !facility.Longitude.HasValue || facility.Latitude.Value == 0.0f || facility.Longitude.Value == 0.0f)
                    continue;

                facilitiesDistance[facility.Id] = LocationUtils.CalculateDistance(latitude.Value, longitude.Value, facility.Latitude.Value, facility.Longitude.Value);
            }

            var nearestFailicitesIds = facilitiesDistance.OrderBy(x => x.Value).Take(5).Select(x => x.Key);

            return facilities.Where(x => nearestFailicitesIds.Contains(x.Id)).ToList();
        }

        #endregion
    }
}