using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models
{
    public class UserAddresses
    {
        public User User { get; set; }
        public List<AddressResponse> Addresses { get; set; }
    }
}
