using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SureNkap.Models
{
    public class DevisModel
    {
       public int PharmacyId { get; set; }
        public int PatientId { get; set; }
        public string OrdonnanceRef { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public string Equivalent { get; set; }
       
    }
}