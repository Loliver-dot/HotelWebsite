using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models.DB
{
    interface IRepositoryAdmin : IRepositoryBase
    {
        bool AddRoom(Room room);
        List<User> getAllUsers();
        List<User> SearchByName(string name);
        List<User> SearchByPhone(string phone);
        bool ChangeUser(User newUser, int userIdToChange);
        bool ChangeRoom(Room newRoom, int roomIdToChange);
        public Room GetRoomByID(int? RoomId);
        List<RoomFeatures> GetRoomFeatures();
        RoomFeatures GetFeatureById(int FeatureId);

    }
}
