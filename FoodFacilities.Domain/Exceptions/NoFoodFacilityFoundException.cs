using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodFacilities.Domain.Exceptions
{
    public class NoFoodFacilityFoundException : Exception
    {
        public NoFoodFacilityFoundException() : base() { }
        public NoFoodFacilityFoundException(string message) : base(message) { }
        public NoFoodFacilityFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
