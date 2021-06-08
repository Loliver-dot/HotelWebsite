using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models
{
    public class AddressResponse
    {
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie einen Staat ein")]
        public string State { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie eine Postleitzahl an.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie eine Stadt ein")]
        public string City { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie eine Straße ein")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie eine Hausnummer ein")]
        public string StreetNumber { get; set; }
    }
}
