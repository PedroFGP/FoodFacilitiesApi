using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodFacilities.Domain.Adapters.Driving.Queries
{
    public interface IFoodFacilityApplicantQuery
    {
        string Applicant { get; set; }

        ICollection<string> Status { get; set; }
    }
}
