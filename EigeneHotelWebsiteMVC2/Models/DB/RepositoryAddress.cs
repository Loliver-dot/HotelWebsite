using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models.DB
{
    public class RepositoryAddress : RepositoryBase, IRepositoryAddress
    {
        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public int getAddressID(AddressResponse addressResponse)
        {
            DbCommand cmdAddressId = _connection.CreateCommand();
            cmdAddressId.CommandText = "select address_id from address where state = @state and city_id = @cityId " +
                "and street = @street and street_number = @streetNumber";

            DbParameter stateParam = cmdAddressId.CreateParameter();
            stateParam.ParameterName = "state";
            stateParam.DbType = DbType.String;
            stateParam.Value = addressResponse.State;

            DbParameter cityIdParam = cmdAddressId.CreateParameter();
            cityIdParam.ParameterName = "cityId";
            cityIdParam.DbType = DbType.String;
            cityIdParam.Value = getCityId(addressResponse.PostalCode);

            DbParameter streetParam = cmdAddressId.CreateParameter();
            streetParam.ParameterName = "street";
            streetParam.DbType = DbType.String;
            streetParam.Value = addressResponse.Street;

            DbParameter streetNumberParam = cmdAddressId.CreateParameter();
            streetNumberParam.ParameterName = "streetNumber";
            streetNumberParam.DbType = DbType.String;
            streetNumberParam.Value = addressResponse.StreetNumber;

            cmdAddressId.Parameters.Add(stateParam);
            cmdAddressId.Parameters.Add(cityIdParam);
            cmdAddressId.Parameters.Add(streetParam);
            cmdAddressId.Parameters.Add(streetNumberParam);

            using(DbDataReader reader = cmdAddressId.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return Convert.ToInt32(reader["address_id"]);
                }
            }
            return -1;
        }

        public List<City> getAllCities()
        {
            List<City> cities = new List<City>();
            DbCommand cmdGetAllCities = _connection.CreateCommand();
            cmdGetAllCities.CommandText = "select * from city";

            using(DbDataReader reader = cmdGetAllCities.ExecuteReader())
            {
                while (reader.Read())
                {
                    cities.Add(new City
                    {
                        CityId = Convert.ToInt32(reader["city_id"]),
                        PostalCode = Convert.ToString(reader["postal_code"]),
                        CityName = Convert.ToString(reader["city"])
                    });
                }
            }
            return cities;
        }

        public int getCityId(string postalCode)
        {
            DbCommand cmdGetCityId = _connection.CreateCommand();
            cmdGetCityId.CommandText = "select city_id from city where postal_code = @pstl";

            DbParameter cityParam = cmdGetCityId.CreateParameter();
            cityParam.ParameterName = "pstl";
            cityParam.DbType = DbType.String;
            cityParam.Value = postalCode;

            cmdGetCityId.Parameters.Add(cityParam);

            using(DbDataReader reader = cmdGetCityId.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return Convert.ToInt32(reader["city_id"]);
                }
            }
            return -1;
        }

        public int getUserID(string email)
        {
            int userId = -1;

            DbCommand cmdUid = _connection.CreateCommand();
            cmdUid.CommandText = "select user_id from user where email = @email";

            DbParameter emailParam = cmdUid.CreateParameter();
            emailParam.ParameterName = "email";
            emailParam.DbType = DbType.String;
            emailParam.Value = email;

            cmdUid.Parameters.Add(emailParam);

            using(DbDataReader reader = cmdUid.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    userId = Convert.ToInt32(reader["user_id"]);
                }
            }
            return userId;
        }

        public bool Insert(AddressResponse AddressResponse, int userId)
        {
            int cityId = -1;
            
            // Überprüft ob eine City bereits existiert
            foreach(City city in getAllCities())
            {
                if(city.CityName == AddressResponse.City)
                {
                    cityId = city.CityId;
                }
            }
            if(cityId == -1)
            {
                City city = new City
                {
                    CityId = -1,
                    PostalCode = AddressResponse.PostalCode,
                    CityName = AddressResponse.City
                };
                InsertCity(city);
                cityId = getCityId(city.PostalCode);
            }
            



            DbCommand cmdInsert = _connection.CreateCommand();
            cmdInsert.CommandText = "insert into address values(null, @city_id, @state, @street, @streetNumber)";

            DbParameter cityIdParam = cmdInsert.CreateParameter();
            cityIdParam.ParameterName = "city_id";
            cityIdParam.DbType = DbType.Int32;
            cityIdParam.Value = cityId;

            DbParameter stateParam = cmdInsert.CreateParameter();
            stateParam.ParameterName = "state";
            stateParam.DbType = DbType.String;
            stateParam.Value = AddressResponse.State;

            DbParameter streetParam = cmdInsert.CreateParameter();
            streetParam.ParameterName = "street";
            streetParam.DbType = DbType.String;
            streetParam.Value = AddressResponse.Street;

            DbParameter streetNumberParam = cmdInsert.CreateParameter();
            streetNumberParam.ParameterName = "streetNumber";
            streetNumberParam.DbType = DbType.String;
            streetNumberParam.Value = AddressResponse.StreetNumber;

            cmdInsert.Parameters.Add(cityIdParam);
            cmdInsert.Parameters.Add(stateParam);
            cmdInsert.Parameters.Add(streetParam);
            cmdInsert.Parameters.Add(streetNumberParam);

            cmdInsert.ExecuteNonQuery();

            InsertUserAddress(getAddressID(AddressResponse), userId);

            return true;
        }

        public void InsertCity(City city)
        {
            DbCommand cmdInsertCity = _connection.CreateCommand();
            cmdInsertCity.CommandText = "insert into city values(null, @pstl, @city)";

            DbParameter postalParam = cmdInsertCity.CreateParameter();
            postalParam.ParameterName = "pstl";
            postalParam.DbType = DbType.String;
            postalParam.Value = city.PostalCode;

            DbParameter cityParam = cmdInsertCity.CreateParameter();
            cityParam.ParameterName = "city";
            cityParam.DbType = DbType.String;
            cityParam.Value = city.CityName;

            cmdInsertCity.Parameters.Add(postalParam);
            cmdInsertCity.Parameters.Add(cityParam);

            cmdInsertCity.ExecuteNonQuery();
        }

        public void InsertUserAddress(int AddressId, int UserId)
        {
            DbCommand cmdInsert = _connection.CreateCommand();
            cmdInsert.CommandText = "insert into user_address values(@uid, @aid)";

            DbParameter uidParam = cmdInsert.CreateParameter();
            uidParam.ParameterName = "uid";
            uidParam.DbType = DbType.Int32;
            uidParam.Value = UserId;

            DbParameter aidParam = cmdInsert.CreateParameter();
            aidParam.ParameterName = "aid";
            aidParam.DbType = DbType.Int32;
            aidParam.Value = AddressId;

            cmdInsert.Parameters.Add(uidParam);
            cmdInsert.Parameters.Add(aidParam);

            cmdInsert.ExecuteNonQuery();
        }

        public bool Update(AddressResponse AddressResponse)
        {
            int cityId = -1;

            // Überprüft ob eine City bereits existiert
            foreach (City city in getAllCities())
            {
                if (city.CityName == AddressResponse.City)
                {
                    cityId = city.CityId;
                    break;
                }
            }
            if (cityId == -1)
            {
                City city = new City {
                    CityId = -1,
                    PostalCode = AddressResponse.PostalCode,
                    CityName = AddressResponse.City
                };
                InsertCity(city);
                cityId = getCityId(city.PostalCode);
            }
            DbCommand cmdUpdateAddress = _connection.CreateCommand();
            cmdUpdateAddress.CommandText = "update address set state=@state, city_id=@cityId, street=@street, " +
                "street_number=@streetNumber where address_id=@addressId";

            DbParameter stateParam = cmdUpdateAddress.CreateParameter();
            stateParam.ParameterName = "state";
            stateParam.DbType = DbType.String;
            stateParam.Value = AddressResponse.State;

            DbParameter cityIdParam = cmdUpdateAddress.CreateParameter();
            cityIdParam.ParameterName = "cityId";
            cityIdParam.DbType = DbType.Int32;
            cityIdParam.Value = cityId;

            DbParameter streetParam = cmdUpdateAddress.CreateParameter();
            streetParam.ParameterName = "street";
            streetParam.DbType = DbType.String;
            streetParam.Value = AddressResponse.Street;

            DbParameter streetNumberParam = cmdUpdateAddress.CreateParameter();
            streetNumberParam.ParameterName = "streetNumber";
            streetNumberParam.DbType = DbType.String;
            streetNumberParam.Value = AddressResponse.StreetNumber;

            DbParameter addressIdParam = cmdUpdateAddress.CreateParameter();
            addressIdParam.ParameterName = "addressId";
            addressIdParam.DbType = DbType.String;
            addressIdParam.Value = AddressResponse.AddressId;

            cmdUpdateAddress.Parameters.Add(stateParam);
            cmdUpdateAddress.Parameters.Add(cityIdParam);
            cmdUpdateAddress.Parameters.Add(streetParam);
            cmdUpdateAddress.Parameters.Add(streetNumberParam);
            cmdUpdateAddress.Parameters.Add(addressIdParam);

            return cmdUpdateAddress.ExecuteNonQuery() == 1;

        }

       
    }
}
