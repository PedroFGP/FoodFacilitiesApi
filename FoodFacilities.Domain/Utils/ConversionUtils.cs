using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FoodFacilities.Domain.Utils
{
    public static class ConversionUtils
    {
        public static Vector2 ParseVector2(string value)
        {
            if(string.IsNullOrEmpty(value)) 
                return Vector2.Zero;

            var strings = value.Replace("(", string.Empty).Replace(")", string.Empty).Trim().Split(',');

            if(strings.Length == 2)
            {
                var xValid = float.TryParse(strings.First(), out float xParse);
                var yValid = float.TryParse(strings.Last(), out float yParse);

                if(xValid && yValid)
                    return new Vector2(xParse, yParse);
            }

            return Vector2.Zero;
        }

    }
}
