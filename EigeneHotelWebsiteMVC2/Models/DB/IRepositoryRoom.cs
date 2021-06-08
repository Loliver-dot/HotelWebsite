using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models.DB
{
    interface IRepositoryRoom : IRepositoryBase
    {
        List<Room> GetAllRooms();
        Room GetRoomByID(int? RoomId);
        bool InsertBill(Bill bill, int UserId);
        bool InsertRoomBill(RoomBill roomBill, int billId, int roomId);
        int GetBillLength();
        double GetFullPrice(int billId, int roomId);
        List<RoomFeatures> GetRoomFeaturesByRoomId(int roomId);
    }
}
