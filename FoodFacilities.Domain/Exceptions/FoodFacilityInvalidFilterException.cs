using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FoodFacilities.Domain.Exceptions
{
    public class FoodFacilityInvalidFilterException : Exception
    {
        public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.BadRequest;

        public FoodFacilityInvalidFilterException(string message) : base(message) { }
    }
}
