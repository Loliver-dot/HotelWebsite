using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models.DB
{
    interface IRepositoryUser : IRepositoryBase
    {
        bool Register(RegisterResponse Rr);
        bool Login(LoginResponse Lr);
        string GetNameByEmail(string email);
        int GetIdByEmail(string email);
        List<string> GetAllEmails();
        bool GetIsAdminByEmail(string email);
        User GetUserById(int userIdToFetch);
        List<AddressResponse> GetAddressesByUserId(int userId);
        bool ChangeUser(User newUser);
        AddressResponse GetAddressById(int addressId);
    }

}
