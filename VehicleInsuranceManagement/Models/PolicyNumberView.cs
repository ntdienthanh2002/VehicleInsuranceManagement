using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VehicleInsuranceManagement.Models
{
    public class PolicyNumberView
    {

        public PolicyNumberView(int policyNumber, string description)
        {
            PolicyNumber = policyNumber;
            Description = description;
        }

        public int PolicyNumber { get; set; }
        public string Description { get; set; }
    }
}