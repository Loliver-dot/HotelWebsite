using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EigeneHotelWebsiteMVC2.Models
{
    public class RoomBill
    {
        public int? RoomId { get; set; }
        public int? BillId { get; set; }

        [Required(ErrorMessage="Bitte geben Sie eine gültige Zahl ein!")]
        public int PeopleCount { get; set; }

        [Required(ErrorMessage ="Bitte geben Sie ein Datum ein")]
        public DateTime StartingDate { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie ein Datum ein")]
        public DateTime EndingDate { get; set; }

        [EnumDataType(typeof(RoomService), ErrorMessage = "Bitte geben Sie einen gültigen Wert ein")]
        public RoomService RoomService { get; set; }
    }
    public enum RoomService
    {
        FullPension, HalfPension, BreakfastOnly
    }
}
