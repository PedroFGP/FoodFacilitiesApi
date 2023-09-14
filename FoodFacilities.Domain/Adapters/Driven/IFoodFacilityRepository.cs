﻿using FoodFacilities.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FoodFacilities.Domain.Adapters.Driven
{
    public interface IFoodFacilityRepository
    {
        Task<ICollection<FoodFacility>> GetAsync(Func<FoodFacility, bool> filter);
    }
}
