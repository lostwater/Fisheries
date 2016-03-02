using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fisheries.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime? PaymentTime { get; set; }
        public float PaymentPrice { get; set; }
        public string Description { get; set; }

    }
}