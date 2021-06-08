using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models
{
    public class RoomAndAllFeature
    {
        public Room Room { get; set; }
        public List<RoomFeatures> AllRoomFeatures { get; set; } 
    }
}
