using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models
{
    public class Bill
    {
        public int? BillNumber { get; set; }
        public DateTime? BillingDate { get; set; }
        public PayingMethod PayingMethod { get; set; }
        public DateTime? LastBillingDate { get; set; }
        public bool? IsPaid { get; set; }
    }
    public enum PayingMethod
    {
        Bar, Visa, Mastercard
    }
}
