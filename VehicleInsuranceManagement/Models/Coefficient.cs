//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VehicleInsuranceManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Coefficient
    {
        [Display(Name = "Coefficient ID")]
        public int CoefficientID { get; set; }

        [Display(Name = "Seat Number")]
        public int SeatNumber { get; set; }

        [Display(Name = "Coefficient")]
        public decimal Coefficient1 { get; set; }
    }
}
