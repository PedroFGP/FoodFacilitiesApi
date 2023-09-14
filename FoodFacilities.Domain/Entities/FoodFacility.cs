using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FoodFacilities.Domain.Entities
{
    public class FoodFacility
    {
        public long Id { get; set; }
        public string? Applicant { get; set; }
        //Maybe change to custom Enumeration...
        public string?   FacilityType { get; set; }
        public string? Cnn { get; set; }
        public string? LocationDescription { get; set; }
        public string? Address { get; set; }
        public string? BlockLot { get; set; }
        public string? Block { get; set; }
        public string? Lot { get; set; }
        public string? Permit { get; set; }
        //Maybe change to custom Enumeration...
        public string? Status { get; set; }
        public ICollection<string>? FoodItems { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Schedule { get; set; }
        public string? DaysHours { get; set; }
        public DateTime? NOISent { get; set; }
        public DateTime? Approved { get; set; }
        public string? Received { get; set; }
        public bool PriorPermit { get; set; }
        public DateTime? ExpirationDate { get; set; }
        //TODO: change to 2D location
        public Vector2? Location { get; set; }
        public int? FirePreventionDistrictsCount { get; set; }
        public int? PoliceDistrictsCount { get; set; }
        public int? SupervisorDistrictsCount { get; set; }
        public string? ZipCodes { get; set; }
        public int? NeighborhoodsCount { get; set; }
    }
}
