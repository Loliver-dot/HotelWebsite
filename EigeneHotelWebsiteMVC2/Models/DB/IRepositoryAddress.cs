using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models.DB
{
    interface IRepositoryAddress : IRepositoryBase
    {
        bool Insert(AddressResponse AddressResponse, int userId);
        bool Update(AddressResponse AddressResponse);
        bool Delete(int id);
        List<City> getAllCities();
        void InsertCity(City city);
        void InsertUserAddress(int AddressId, int UserId);
        int getCityId(string postalCode);
        int getUserID(string email);
        int getAddressID(AddressResponse addressResponse);
    }
}
