using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models.DB
{
    public class RepositoryAdmin : RepositoryBase, IRepositoryAdmin
    {
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
                        RoomID = (int)RoomId,
                        RoomNumber = Convert.ToString(reader["room_number"]),
                        RoomName = Convert.ToString(reader["room_name"]),
                        PricePerNight = Convert.ToDouble(reader["price_per_night"]),
                        Area = Convert.ToDouble(reader["area"]),
                        NumberOfBeds = Convert.ToInt32(reader["number_of_beds"]),
                        Description = Convert.ToString(reader["description"])
                    };
                }
            }
            room.RoomFeatures = getRoomFeaturesByRoomId((int)RoomId);
            return room;

        }
        //TODO error here idk
        public List<RoomFeatures> getRoomFeaturesByRoomId(int roomId)
        {
            DbCommand cmdGetRoomFeatures = _connection.CreateCommand();
            cmdGetRoomFeatures.CommandText = "select * from room_feature join feature using(feature_id) where room_id = @roomId";

            DbParameter roomIdParam = cmdGetRoomFeatures.CreateParameter();
            roomIdParam.ParameterName = "roomId";
            roomIdParam.DbType = DbType.Int32;
            roomIdParam.Value = roomId;

            cmdGetRoomFeatures.Parameters.Add(roomIdParam);

            List<RoomFeatures> foundFeatures = new();

            using(DbDataReader reader = cmdGetRoomFeatures.ExecuteReader())
            {
                while (reader.Read())
                {
                    foundFeatures.Add(new RoomFeatures {
                        FeatureID = Convert.ToInt32(reader["feature_id"]),
                        Name = Convert.ToString(reader["feature_name"]),
                        Description = Convert.ToString(reader["feature_description"])
                    });
                }
            }
            return foundFeatures.Count >= 1 ? foundFeatures : null;

        }

        public bool AddRoom(Room room)
        {
            DbCommand cmdAddRoom = _connection.CreateCommand();
            cmdAddRoom.CommandText = "insert into room values(null, @roomNumber, @roomName, @pricePerNight, @area," +
                "@numberOfBeds, @description)";

            DbParameter roomNumberParam = cmdAddRoom.CreateParameter();
            roomNumberParam.ParameterName = "roomNumber";
            roomNumberParam.DbType = DbType.String;
            roomNumberParam.Value = room.RoomNumber;
            
            DbParameter roomNameParam = cmdAddRoom.CreateParameter();
            roomNameParam.ParameterName = "roomName";
            roomNameParam.DbType = DbType.String;
            roomNameParam.Value = room.RoomName;
            
            DbParameter pricePerNightParam = cmdAddRoom.CreateParameter();
            pricePerNightParam.ParameterName = "pricePerNight";
            pricePerNightParam.DbType = DbType.Decimal;
            pricePerNightParam.Value = room.PricePerNight;

            DbParameter areaParam = cmdAddRoom.CreateParameter();
            areaParam.ParameterName = "area";
            areaParam.DbType = DbType.Double;
            areaParam.Value = room.Area;

            DbParameter numberOfBedsParam = cmdAddRoom.CreateParameter();
            numberOfBedsParam.ParameterName = "numberOfBeds";
            numberOfBedsParam.DbType = DbType.Int32;
            numberOfBedsParam.Value = room.NumberOfBeds;

            DbParameter descriptionParam = cmdAddRoom.CreateParameter();
            descriptionParam.ParameterName = "description";
            descriptionParam.DbType = DbType.String;
            descriptionParam.Value = room.Description;

            cmdAddRoom.Parameters.Add(roomNumberParam);
            cmdAddRoom.Parameters.Add(roomNameParam);
            cmdAddRoom.Parameters.Add(pricePerNightParam);
            cmdAddRoom.Parameters.Add(areaParam);
            cmdAddRoom.Parameters.Add(numberOfBedsParam);
            cmdAddRoom.Parameters.Add(descriptionParam);

            cmdAddRoom.ExecuteNonQuery();
            foreach(RoomFeatures feature in room.RoomFeatures)
            {
                addRoomFeature(feature.FeatureID, GetRoomCount());
            }


            return true;
        }

        private void addRoomFeature(int featureId, int roomID)
        {
            DbCommand cmdAddRoomFeature = _connection.CreateCommand();
            cmdAddRoomFeature.CommandText = "insert into room_feature values(" + roomID + ", " + featureId + ")";

            cmdAddRoomFeature.ExecuteNonQuery();
        }

        public bool ChangeUser(User newUser, int userIdToChange)
        {
            DbCommand cmdChangeUser = _connection.CreateCommand();
            cmdChangeUser.CommandText = "update user set firstname=@firstName, lastname=@lastName," +
                " email=@email, phone=@phone, is_employee=@isAdmin where user_id=@userIdToChange";

            DbParameter firstNameParam = cmdChangeUser.CreateParameter();
            firstNameParam.ParameterName = "firstName";
            firstNameParam.DbType = DbType.String;
            firstNameParam.Value = newUser.FirstName;

            DbParameter lastNameParam = cmdChangeUser.CreateParameter();
            lastNameParam.ParameterName = "lastName";
            lastNameParam.DbType = DbType.String;
            lastNameParam.Value = newUser.LastName;

            DbParameter emailParam = cmdChangeUser.CreateParameter();
            emailParam.ParameterName = "email";
            emailParam.DbType = DbType.String;
            emailParam.Value = newUser.Email;

            DbParameter phoneParam = cmdChangeUser.CreateParameter();
            phoneParam.ParameterName = "phone";
            phoneParam.DbType = DbType.String;
            phoneParam.Value = newUser.PhoneNumber;

            DbParameter isAdminParam = cmdChangeUser.CreateParameter();
            isAdminParam.ParameterName = "isAdmin";
            isAdminParam.DbType = DbType.Boolean;
            isAdminParam.Value = newUser.IsAdmin;

            DbParameter userIdParam = cmdChangeUser.CreateParameter();
            userIdParam.ParameterName = "userIdToChange";
            userIdParam.DbType = DbType.Int32;
            userIdParam.Value = userIdToChange;

            cmdChangeUser.Parameters.Add(firstNameParam);
            cmdChangeUser.Parameters.Add(lastNameParam);
            cmdChangeUser.Parameters.Add(emailParam);
            cmdChangeUser.Parameters.Add(phoneParam);
            cmdChangeUser.Parameters.Add(isAdminParam);
            cmdChangeUser.Parameters.Add(userIdParam);

            return cmdChangeUser.ExecuteNonQuery() == 1;
        }

        public List<User> getAllUsers()
        {
            List<User> foundUsers = new List<User>();

            DbCommand cmdGetAllUsers = _connection.CreateCommand();
            cmdGetAllUsers.CommandText = "select user_id, firstname, lastname, email, phone, is_employee from user";

            using(DbDataReader reader = cmdGetAllUsers.ExecuteReader())
            {
                while (reader.Read())
                {
                    foundUsers.Add(new User {
                        UserId = Convert.ToInt32(reader["user_id"]),
                        FirstName = Convert.ToString(reader["firstname"]),
                        LastName = Convert.ToString(reader["lastname"]),
                        Email = Convert.ToString(reader["email"]),
                        PhoneNumber = Convert.ToString(reader["phone"]),
                        IsAdmin = Convert.ToBoolean(reader["is_employee"])
                    });
                }
            }
            return foundUsers;
        }

        public List<User> SearchByName(string name)
        {
            List<User> foundUsers = new();

            DbCommand cmdSearchUserByName = _connection.CreateCommand();
            cmdSearchUserByName.CommandText = "select user_id, firstname, lastname, email, phone, is_employee from user where" +
                " firstname like @name or lastname like @name";

            DbParameter nameParam = cmdSearchUserByName.CreateParameter();
            nameParam.ParameterName = "name";
            nameParam.DbType = DbType.String;
            nameParam.Value = "%" + name + "%";

            cmdSearchUserByName.Parameters.Add(nameParam);

            using(DbDataReader reader = cmdSearchUserByName.ExecuteReader())
            {
                while (reader.Read())
                {
                    foundUsers.Add(new User {
                        UserId = Convert.ToInt32(reader["user_id"]),
                        FirstName = Convert.ToString(reader["firstname"]),
                        LastName = Convert.ToString(reader["lastname"]),
                        Email = Convert.ToString(reader["email"]),
                        PhoneNumber = Convert.ToString(reader["phone"]),
                        IsAdmin = Convert.ToBoolean(reader["is_employee"])
                    });
                }
            }
            return foundUsers;
        }
        public List<User> SearchByPhone(string phone)
        {
            List<User> foundUsers = new();

            DbCommand cmdSearchUserByName = _connection.CreateCommand();
            cmdSearchUserByName.CommandText = "select user_id, firstname, lastname, email, phone, is_employee from user where" +
                " phone like @phone";

            DbParameter nameParam = cmdSearchUserByName.CreateParameter();
            nameParam.ParameterName = "phone";
            nameParam.DbType = DbType.String;
            nameParam.Value = "%" + phone + "%";

            cmdSearchUserByName.Parameters.Add(nameParam);

            using (DbDataReader reader = cmdSearchUserByName.ExecuteReader())
            {
                while (reader.Read())
                {
                    foundUsers.Add(new User {
                        UserId = Convert.ToInt32(reader["user_id"]),
                        FirstName = Convert.ToString(reader["firstname"]),
                        LastName = Convert.ToString(reader["lastname"]),
                        Email = Convert.ToString(reader["email"]),
                        PhoneNumber = Convert.ToString(reader["phone"]),
                        IsAdmin = Convert.ToBoolean(reader["is_employee"])
                    });
                }
            }
            return foundUsers;
        }

        public bool ChangeRoom(Room newRoom, int roomIdToChange)
        {
            DbCommand cmdChangeRoomData = _connection.CreateCommand();
            cmdChangeRoomData.CommandText = "update room set room_number=@roomNumber, room_name=@roomName, price_per_night=" +
                "@pricePerNight, area=@area, number_of_beds=@numberOfBeds, description=@description where room_id=@roomId";

            DbParameter roomNumberParam = cmdChangeRoomData.CreateParameter();
            roomNumberParam.ParameterName = "roomNumber";
            roomNumberParam.DbType = DbType.String;
            roomNumberParam.Value = newRoom.RoomNumber;

            DbParameter roomNameParam = cmdChangeRoomData.CreateParameter();
            roomNameParam.ParameterName = "roomName";
            roomNameParam.DbType = DbType.String;
            roomNameParam.Value = newRoom.RoomName;

            DbParameter pricePerNightParam = cmdChangeRoomData.CreateParameter();
            pricePerNightParam.ParameterName = "pricePerNight";
            pricePerNightParam.DbType = DbType.Decimal;
            pricePerNightParam.Value = newRoom.PricePerNight;

            DbParameter areaParam = cmdChangeRoomData.CreateParameter();
            areaParam.ParameterName = "area";
            areaParam.DbType = DbType.Double;
            areaParam.Value = newRoom.Area;

            DbParameter numberOfBedsParam = cmdChangeRoomData.CreateParameter();
            numberOfBedsParam.ParameterName = "numberOfBeds";
            numberOfBedsParam.DbType = DbType.Int32;
            numberOfBedsParam.Value = newRoom.NumberOfBeds;

            DbParameter descriptionParam = cmdChangeRoomData.CreateParameter();
            descriptionParam.ParameterName = "description";
            descriptionParam.DbType = DbType.String;
            descriptionParam.Value = newRoom.Description;

            DbParameter roomIdParam = cmdChangeRoomData.CreateParameter();
            roomIdParam.ParameterName = "roomId";
            roomIdParam.DbType = DbType.Int32;
            roomIdParam.Value = newRoom.RoomID;

            cmdChangeRoomData.Parameters.Add(roomNumberParam);
            cmdChangeRoomData.Parameters.Add(roomNameParam);
            cmdChangeRoomData.Parameters.Add(pricePerNightParam);
            cmdChangeRoomData.Parameters.Add(areaParam);
            cmdChangeRoomData.Parameters.Add(numberOfBedsParam);
            cmdChangeRoomData.Parameters.Add(descriptionParam);
            cmdChangeRoomData.Parameters.Add(roomIdParam);

            deleteRoomFeatureByRoomId(roomIdToChange);

            foreach(RoomFeatures feature in newRoom.RoomFeatures)
            {
                addRoomFeature(feature.FeatureID, newRoom.RoomID);
            }
            return cmdChangeRoomData.ExecuteNonQuery() == 1;
        }

        private void deleteRoomFeatureByRoomId(int roomIdToChange)
        {
            DbCommand cmdDeleteRoomFeatures = _connection.CreateCommand();
            cmdDeleteRoomFeatures.CommandText = "delete from room_feature where room_id = @roomId";

            DbParameter roomIdParam = cmdDeleteRoomFeatures.CreateParameter();
            roomIdParam.ParameterName = "roomId";
            roomIdParam.DbType = DbType.Int32;
            roomIdParam.Value = roomIdToChange;

            cmdDeleteRoomFeatures.Parameters.Add(roomIdParam);

            cmdDeleteRoomFeatures.ExecuteNonQuery();
        }

        public List<RoomFeatures> GetRoomFeatures()
        {
            List<RoomFeatures> foundFeatures = new();

            DbCommand cmdGetFeatures = _connection.CreateCommand();
            cmdGetFeatures.CommandText = "select * from feature";

            using (DbDataReader reader = cmdGetFeatures.ExecuteReader())
            {
                while (reader.Read())
                {
                    foundFeatures.Add(new RoomFeatures {
                        FeatureID = Convert.ToInt32(reader["feature_id"]),
                        Name = Convert.ToString(reader["feature_name"]),
                        Description = Convert.ToString(reader["feature_description"])
                    });
                }
            }
            return foundFeatures;
        }

        public RoomFeatures GetFeatureById(int FeatureId)
        {
            DbCommand cmdGetFeature = _connection.CreateCommand();
            cmdGetFeature.CommandText = "select * from feature where feature_id = " + FeatureId;

            using (DbDataReader reader = cmdGetFeature.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return new RoomFeatures {
                        FeatureID = FeatureId,
                        Name = Convert.ToString(reader["feature_name"]),
                        Description = Convert.ToString(reader["feature_description"])
                    };
                }
            }
            return null;
        }

        int GetRoomCount()
        {
            DbCommand cmdGetRoomCount = _connection.CreateCommand();
            cmdGetRoomCount.CommandText = "select room_id from room";
            List<int> ids = new();

            using (DbDataReader reader = cmdGetRoomCount.ExecuteReader())
            {
                while (reader.Read()) { 
                    ids.Add(Convert.ToInt32(reader["room_id"]));    
                }
                return ids.Max();
            }
        }

    }
    
}
