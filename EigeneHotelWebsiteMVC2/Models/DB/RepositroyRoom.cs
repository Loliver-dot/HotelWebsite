using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models.DB
{
    public class RepositroyRoom : RepositoryBase, IRepositoryRoom
    {
        public List<Room> GetAllRooms()
        {
            List<Room> Rooms = new(); 

            DbCommand cmdGetRooms = _connection.CreateCommand();
            cmdGetRooms.CommandText = "select * from room";

            using(DbDataReader reader = cmdGetRooms.ExecuteReader())
            {
                while (reader.Read())
                {
                    Rooms.Add(new Room
                    {
                        RoomID = Convert.ToInt32(reader["room_id"]),
                        RoomName = Convert.ToString(reader["room_name"]),
                        RoomNumber = Convert.ToString(reader["room_number"]),
                        PricePerNight = Convert.ToDouble(reader["price_per_night"]),
                        Area = Convert.ToDouble(reader["area"]),
                        NumberOfBeds = Convert.ToInt32(reader["number_of_beds"]),
                        Description = Convert.ToString(reader["description"])
                    });
                }
            }
            return Rooms;
        }

        public int GetBillLength()
        {
            DbCommand cmdGetBillLength = _connection.CreateCommand();
            cmdGetBillLength.CommandText = "select count(*) from bill";

            using(DbDataReader reader = cmdGetBillLength.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return Convert.ToInt32(reader["count(*)"]);
                }
            }
            return -1;
        }

        public double GetFullPrice(int billId, int roomId)
        {
            double multiplier = 0;
            DbCommand cmdGetFullPrice = _connection.CreateCommand();
            cmdGetFullPrice.CommandText = "select people_count * price_per_night * datediff(ending_date, starting_date), service from room join room_bill using(room_id) where bill_id = @billId and room_id = @roomId";

            DbParameter billIdParam = cmdGetFullPrice.CreateParameter();
            billIdParam.ParameterName = "billId";
            billIdParam.DbType = DbType.Int32;
            billIdParam.Value = billId;

            DbParameter roomIdParam = cmdGetFullPrice.CreateParameter();
            roomIdParam.ParameterName = "roomId";
            roomIdParam.DbType = DbType.Int32;
            roomIdParam.Value = roomId;

            cmdGetFullPrice.Parameters.Add(billIdParam);
            cmdGetFullPrice.Parameters.Add(roomIdParam);

            using(DbDataReader reader = cmdGetFullPrice.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    switch (Convert.ToInt32(reader["service"])){
                        case 0:
                            multiplier = 1.15;
                            break;
                        case 1:
                            multiplier = 1.07;
                            break;
                        case 2:
                            multiplier = 1;
                            break;
                    }
                    return multiplier * Convert.ToDouble(reader["people_count * price_per_night * datediff(ending_date, starting_date)"]);
                }
            }
            return 0;
        }

        public Room GetRoomByID(int? RoomId)
        {
            DbCommand cmdGetRoom = _connection.CreateCommand();
            cmdGetRoom.CommandText = "select * from room where room_id = @id";

            DbParameter idParam = cmdGetRoom.CreateParameter();
            idParam.ParameterName = "id";
            idParam.DbType = DbType.Int32;
            idParam.Value = RoomId;

            cmdGetRoom.Parameters.Add(idParam);
            Room room = new();

            using (DbDataReader reader = cmdGetRoom.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    room = new Room {
                        RoomID = Convert.ToInt32(reader["room_id"]),
                        RoomNumber = Convert.ToString(reader["room_number"]),
                        RoomName = Convert.ToString(reader["room_name"]),
                        PricePerNight = Convert.ToDouble(reader["price_per_night"]),
                        Area = Convert.ToDouble(reader["area"]),
                        NumberOfBeds = Convert.ToInt32(reader["number_of_beds"]),
                        Description = Convert.ToString(reader["description"]),
                        RoomFeatures = new()
                    };
                    
                }
                
            }
            List<RoomFeatures> features = new();
            if ((features = GetRoomFeaturesByRoomId((int)RoomId)) == null)
            {
                return room;
            }
            foreach (RoomFeatures rF in features)
            {
                room.RoomFeatures.Add(rF);
            }
            return room;

        }

        public List<RoomFeatures> GetRoomFeaturesByRoomId(int roomId)
        {
            List<RoomFeatures> foundRoomFeatures = new();
            DbCommand cmdGetRoomFeaturesByRoomId = _connection.CreateCommand();
            cmdGetRoomFeaturesByRoomId.CommandText = "select * from feature join room_feature using(feature_id) where room_id = @roomId";

            DbParameter roomIdParam = cmdGetRoomFeaturesByRoomId.CreateParameter();
            roomIdParam.ParameterName = "roomId";
            roomIdParam.DbType = DbType.Int32;
            roomIdParam.Value = roomId;

            cmdGetRoomFeaturesByRoomId.Parameters.Add(roomIdParam);

            using(DbDataReader reader = cmdGetRoomFeaturesByRoomId.ExecuteReader())
            {
                while (reader.Read())
                {
                    foundRoomFeatures.Add(new RoomFeatures {
                        FeatureID = Convert.ToInt32(reader["feature_id"]),
                        Name = Convert.ToString(reader["feature_name"]),
                        Description = Convert.ToString(reader["feature_description"])
                    });
                }
            }
            return foundRoomFeatures.Count > 0 ? foundRoomFeatures : null;
        }

        public bool InsertBill(Bill bill, int UserId)
        {
            DbCommand cmdInsertBill = _connection.CreateCommand();
            cmdInsertBill.CommandText = "insert into bill values(null, @userId, @billingDate, @payingMethod, @lastBillingDate, @isPaid)";

            DbParameter userIdParam = cmdInsertBill.CreateParameter();
            userIdParam.ParameterName = "userId";
            userIdParam.DbType = DbType.Int32;
            userIdParam.Value = UserId;

            DbParameter billingDateParam = cmdInsertBill.CreateParameter();
            billingDateParam.ParameterName = "billingDate";
            billingDateParam.DbType = DbType.Date;
            billingDateParam.Value = bill.BillingDate;

            DbParameter payingMethodParam = cmdInsertBill.CreateParameter();
            payingMethodParam.ParameterName = "payingMethod";
            payingMethodParam.DbType = DbType.Int32;
            payingMethodParam.Value = bill.PayingMethod;

            DbParameter lastBillingDateParam = cmdInsertBill.CreateParameter();
            lastBillingDateParam.ParameterName = "lastBillingDate";
            lastBillingDateParam.DbType = DbType.Date;
            lastBillingDateParam.Value = bill.LastBillingDate;

            DbParameter isPaidParam = cmdInsertBill.CreateParameter();
            isPaidParam.ParameterName = "isPaid";
            isPaidParam.DbType = DbType.Boolean;
            isPaidParam.Value = bill.IsPaid;

            cmdInsertBill.Parameters.Add(userIdParam);
            cmdInsertBill.Parameters.Add(billingDateParam);
            cmdInsertBill.Parameters.Add(payingMethodParam);
            cmdInsertBill.Parameters.Add(lastBillingDateParam);
            cmdInsertBill.Parameters.Add(isPaidParam);

            return cmdInsertBill.ExecuteNonQuery() == 1;
        }

        public bool InsertRoomBill(RoomBill roomBill, int billId, int roomId)
        {
            DbCommand cmdInsertRoomBill = _connection.CreateCommand();
            cmdInsertRoomBill.CommandText = "insert into room_bill values(@roomId, @billId, @startingDate, @endingDate, @service, @peopleCount)";

            DbParameter roomIdParam = cmdInsertRoomBill.CreateParameter();
            roomIdParam.ParameterName = "roomId";
            roomIdParam.DbType = DbType.Int32;
            roomIdParam.Value = roomId;

            DbParameter billIdParam = cmdInsertRoomBill.CreateParameter();
            billIdParam.ParameterName = "billId";
            billIdParam.DbType = DbType.Int32;
            billIdParam.Value = billId;

            DbParameter startingDateParam = cmdInsertRoomBill.CreateParameter();
            startingDateParam.ParameterName = "startingDate";
            startingDateParam.DbType = DbType.Date;
            startingDateParam.Value = roomBill.StartingDate;

            DbParameter endingDateParam = cmdInsertRoomBill.CreateParameter();
            endingDateParam.ParameterName = "endingDate";
            endingDateParam.DbType = DbType.Date;
            endingDateParam.Value = roomBill.EndingDate;

            DbParameter serviceParam = cmdInsertRoomBill.CreateParameter();
            serviceParam.ParameterName = "service";
            serviceParam.DbType = DbType.Int32;
            serviceParam.Value = roomBill.RoomService;

            DbParameter peopleCountParam = cmdInsertRoomBill.CreateParameter();
            peopleCountParam.ParameterName = "peopleCount";
            peopleCountParam.DbType = DbType.Int32;
            peopleCountParam.Value = roomBill.PeopleCount;

            cmdInsertRoomBill.Parameters.Add(roomIdParam);
            cmdInsertRoomBill.Parameters.Add(billIdParam);
            cmdInsertRoomBill.Parameters.Add(startingDateParam);
            cmdInsertRoomBill.Parameters.Add(endingDateParam);
            cmdInsertRoomBill.Parameters.Add(serviceParam);
            cmdInsertRoomBill.Parameters.Add(peopleCountParam);

            return cmdInsertRoomBill.ExecuteNonQuery() == 1;
        }


    }
}
