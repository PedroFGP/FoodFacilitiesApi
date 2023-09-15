using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FoodFacilities.Domain.Exceptions
{
    public class FoodFacilityNotFoundException : Exception
    {
        public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.NotFound;

        public FoodFacilityNotFoundException(string message) : base(message) {}
    }
}
