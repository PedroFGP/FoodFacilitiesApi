using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FoodFacilities.Domain.Exceptions
{
    public class FoodFacilityBaseException : Exception
    {
        public HttpStatusCode StatusCode { get; init; }

        public FoodFacilityBaseException(HttpStatusCode statusCode, string message) : base(message) { StatusCode = statusCode; }
    }
}
