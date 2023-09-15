using AutoMapper;
using FoodFacilities.Adapters.Driving.WebApi.Controllers;
using FoodFacilities.Adapters.Driving.WebApi.Dtos;
using FoodFacilities.Adapters.Driving.WebApi.Mapping;
using FoodFacilities.Domain.Adapters.Driven.Repositories;
using FoodFacilities.Domain.Entities;
using FoodFacilities.Domain.Exceptions;
using FoodFacilities.Domain.Services;
using FoodFacilities.Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IO;
using static System.Net.WebRequestMethods;

namespace FoodFacilities.Test
{
    public class FoodFacilityControllerTest
    {
        private readonly Mock<IFoodFacilityService> _mockFoodFacilityService;
        private readonly IMapper _mockMapper;

        public FoodFacilityControllerTest()
        {
            _mockFoodFacilityService = new Mock<IFoodFacilityService>();

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
            var foodFacilitiesData = GetFoodFacilitiesData();

            var filteredFacilities = foodFacilitiesData.Where(x => x.Applicant is not null && x.Applicant.ToLower() == applicant).ToList();

            _mockFoodFacilityService.Setup(x => x.GetByApplicantAsync(applicant, status).Result).Returns(filteredFacilities);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            var foodFacilitiesActionResult = foodFacilityController.GetFoodFacilitiesByApplicantAsync(applicant, status).Result as ObjectResult;

            var foodFacilitiesResult = foodFacilitiesActionResult?.Value as List<FoodFacilityDto>;

            Assert.NotNull(foodFacilitiesResult);
            Assert.Equal(filteredFacilities.Count(), foodFacilitiesResult.Count());
            Assert.Equal(filteredFacilities.Select(x => x.Id), foodFacilitiesResult.Select(x => x.Id));
        }

        [Theory]
        [InlineData("Donavan Fletcher Truck", new string[] {"APPROVED"})]
        [InlineData("Break Break", new string[] { "APPROVED" })]
        [InlineData("Break Break", new string[] { "APPROVED", "REQUESTED" })]
        [InlineData("Brazuca Grill", new string[] { "EXPIRED" })]
        [InlineData("Natan's Catering", new string[] { "ISSUED" })]
        [InlineData("Red Lobster", new string[] { "SUSPEND" })]
        public void FoodFacilityGetByApplicantSuccessWithtStatus(string applicant, string[]? status)
        {
            var foodFacilitiesData = GetFoodFacilitiesData();

            var filteredFacilities = foodFacilitiesData.Where(x => x.Applicant is not null && x.Applicant.ToLower() == applicant && status is not null && x.Status is not null && status.Contains(x.Status.ToUpper())).ToList();

            _mockFoodFacilityService.Setup(x => x.GetByApplicantAsync(applicant, status).Result).Returns(filteredFacilities);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            var foodFacilitiesActionResult = foodFacilityController.GetFoodFacilitiesByApplicantAsync(applicant, status).Result as ObjectResult;

            var foodFacilitiesResult = foodFacilitiesActionResult?.Value as List<FoodFacilityDto>;

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
            _mockFoodFacilityService.Setup(x => x.GetByApplicantAsync(applicant, null).Result);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            Assert.ThrowsAsync<FoodFacilityNotFoundException>(() => foodFacilityController.GetFoodFacilitiesByApplicantAsync(applicant, null));
        }

        [Theory]
        [InlineData("Hector Tacos", new string[] { "APPROVED" })]
        [InlineData("Hope's Breakfast", new string[] { "REQUESTED" })]
        [InlineData("Henrico's Pizza", new string[] { "SUSPEND" })]
        public void FoodFacilityGetByApplicantFailNotFoundWithStatus(string applicant, string[]? status)
        {
            _mockFoodFacilityService.Setup(x => x.GetByApplicantAsync(applicant, status).Result);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            Assert.ThrowsAsync<FoodFacilityNotFoundException>(() => foodFacilityController.GetFoodFacilitiesByApplicantAsync(applicant, status));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void FoodFacilityGetByApplicantFailInvalidFilterWithoutStatus(string applicant)
        {
            _mockFoodFacilityService.Setup(x => x.GetByApplicantAsync(applicant, null).Result);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            Assert.ThrowsAsync<FoodFacilityInvalidFilterException>(() => foodFacilityController.GetFoodFacilitiesByApplicantAsync(applicant, null));
        }

        [Theory]
        [InlineData("", new string[] { "APPROVED" })]
        [InlineData(null, new string[] { "APPROVED" })]
        public void FoodFacilityGetByApplicantFailInvalidFilterWithStatus(string applicant, string[]? status)
        {
            _mockFoodFacilityService.Setup(x => x.GetByApplicantAsync(applicant, status).Result);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            Assert.ThrowsAsync<FoodFacilityInvalidFilterException>(() => foodFacilityController.GetFoodFacilitiesByApplicantAsync(applicant, status));
        }

        #endregion

        #region GetByStreet Tests

        [Theory]
        [InlineData("MAR")]
        [InlineData("FRANK")]
        [InlineData("GROO")]
        public void FoodFacilityGetByStreetSuccess(string street)
        {
            var foodFacilitiesData = GetFoodFacilitiesData();

            var filteredFacilities = foodFacilitiesData.Where(x => x.Address is not null && x.Address.ToLower().Contains(street.ToLower())).ToList();

            _mockFoodFacilityService.Setup(x => x.GetByStreetAsync(street).Result).Returns(filteredFacilities);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            var foodFacilitiesActionResult = foodFacilityController.GetFoodFacilitiesByStreetAsync(street).Result as ObjectResult;

            var foodFacilitiesResult = foodFacilitiesActionResult?.Value as List<FoodFacilityDto>;

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
            _mockFoodFacilityService.Setup(x => x.GetByStreetAsync(street).Result);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            Assert.ThrowsAsync<FoodFacilityNotFoundException>(() => foodFacilityController.GetFoodFacilitiesByStreetAsync(street));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void FoodFacilityGetByStreetFailInvalidFilter(string street)
        {
            _mockFoodFacilityService.Setup(x => x.GetByStreetAsync(street).Result);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            Assert.ThrowsAsync<FoodFacilityInvalidFilterException>(() => foodFacilityController.GetFoodFacilitiesByStreetAsync(street));
        }

        #endregion

        #region GetNearestFoodFacilitiesByGeolocationAsync Tests
        
        [Theory]
        [InlineData(37.79238985, -122.4012698, null)]
        [InlineData(37.73911140, -122.382469, null)]
        [InlineData(37.78127595, -122.4318404, null)]
        public void FoodFacilityGetNearestFacilitiesSuccessWithoutStatus(double? latitude, double? longitude, string[]? status)
        {
            var foodFacilitiesData = GetFoodFacilitiesData();

            var filteredFacilities = GetNearestFacilities(foodFacilitiesData, latitude, longitude);

            _mockFoodFacilityService.Setup(x => x.GetNearestFoodTruckFacilitiesAsync(latitude, longitude, status).Result).Returns(filteredFacilities);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            var foodFacilitiesActionResult = foodFacilityController.GetNearestFoodTruckFacilitiesByGeolocationAsync(latitude, longitude, status).Result as ObjectResult;

            var foodFacilitiesResult = foodFacilitiesActionResult?.Value as List<FoodFacilityDto>;

            Assert.NotNull(foodFacilitiesResult);
            Assert.Equal(filteredFacilities.Count(), foodFacilitiesResult.Count());
            Assert.Equal(filteredFacilities.Select(x => x.Id), foodFacilitiesResult.Select(x => x.Id));
        }

        [Theory]
        [InlineData(37.79238985, -122.4012698, new string[] { "SUSPEND" })]
        [InlineData(37.73911140, -122.382469, new string[] { "APPROVED", "REQUESTED", "ISSUED" })]
        [InlineData(37.78127595, -122.4318404, new string[] { "APPROVED", "EXPIRED", "ISSUED", "REQUESTED", "SUSPEND" })]
        public void FoodFacilityGetNearestFacilitiesSuccessWithStatus(double? latitude, double? longitude, string[]? status)
        {
            var foodFacilitiesData = GetFoodFacilitiesData();

            var filteredFacilities = GetNearestFacilities(foodFacilitiesData, latitude, longitude);

            _mockFoodFacilityService.Setup(x => x.GetNearestFoodTruckFacilitiesAsync(latitude, longitude, status).Result).Returns(filteredFacilities);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            var foodFacilitiesActionResult = foodFacilityController.GetNearestFoodTruckFacilitiesByGeolocationAsync(latitude, longitude, status).Result as ObjectResult;

            var foodFacilitiesResult = foodFacilitiesActionResult?.Value as List<FoodFacilityDto>;

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
            _mockFoodFacilityService.Setup(x => x.GetNearestFoodTruckFacilitiesAsync(latitude, longitude, status).Result);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            Assert.ThrowsAsync<FoodFacilityNotFoundException>(() => foodFacilityController.GetNearestFoodTruckFacilitiesByGeolocationAsync(latitude, longitude, status));
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
            _mockFoodFacilityService.Setup(x => x.GetNearestFoodTruckFacilitiesAsync(latitude, longitude, status).Result);

            var foodFacilityController = new FoodFacilityController(_mockMapper, _mockFoodFacilityService.Object);

            Assert.ThrowsAsync<FoodFacilityNotFoundException>(() => foodFacilityController.GetNearestFoodTruckFacilitiesByGeolocationAsync(latitude, longitude, status));
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