using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models
{
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public double Area { get; set; }
        public int NumberOfBeds { get; set; }
        public string? RoomName { get; set; }
        public double PricePerNight { get; set; }
        public string Description { get; set; }
        public List<RoomFeatures> RoomFeatures { get; set; }
        
    }
}
