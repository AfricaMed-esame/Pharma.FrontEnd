using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SureNkap.Models
{
    public class ProformaViewModel
    {
        public string Product { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public string Equivalent { get; set; }
        public decimal Total { get; set; }
    }
}