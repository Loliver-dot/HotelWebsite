using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models.DB
{
    public class RepositoryUser : RepositoryBase, IRepositoryUser
    {
  
        public bool Login(LoginResponse Lr)
        {
            
            DbCommand loginCommand = _connection.CreateCommand();
            loginCommand.CommandText = "select * from user where password = sha2(@pwd, 256) and email = @email";

            DbParameter pwdParam = loginCommand.CreateParameter();
            pwdParam.ParameterName = "pwd";
            pwdParam.DbType = DbType.String;
            pwdParam.Value = Lr.Password;

            DbParameter emailParam = loginCommand.CreateParameter();
            emailParam.ParameterName = "email";
            emailParam.DbType = DbType.String;
            emailParam.Value = Lr.UserName;

            loginCommand.Parameters.Add(pwdParam);
            loginCommand.Parameters.Add(emailParam);
            
            using (DbDataReader reader = loginCommand.ExecuteReader())
            {
                // Überprüft, ob Datensätze gefunden wurden
                if (reader.HasRows)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Register(RegisterResponse Rr)
        {
            foreach(string email in GetAllEmails())
            {
                if(email == Rr.Email)
                {
                    return false;
                }
            }

            DbCommand RegisterCommand = this._connection.CreateCommand();
            RegisterCommand.CommandText = "insert into user values(null, @fname, @lname, @email, @phone, sha2(@password, 256), @isEmployee)";

            DbParameter fnameParam = RegisterCommand.CreateParameter();
            fnameParam.ParameterName = "fname";
            fnameParam.DbType = DbType.String;
            fnameParam.Value = Rr.FirstName;

            DbParameter lnameParam = RegisterCommand.CreateParameter();
            lnameParam.ParameterName = "lname";
            lnameParam.DbType = DbType.String;
            lnameParam.Value = Rr.LastName;

            DbParameter emailParam = RegisterCommand.CreateParameter();
            emailParam.ParameterName = "email";
            emailParam.DbType = DbType.String;
            emailParam.Value = Rr.Email;

            DbParameter phoneParam = RegisterCommand.CreateParameter();
            phoneParam.ParameterName = "phone";
            phoneParam.DbType = DbType.String;
            phoneParam.Value = Rr.PhoneNumber;

            DbParameter pwdParam = RegisterCommand.CreateParameter();
            pwdParam.ParameterName = "password";
            pwdParam.DbType = DbType.String;
            pwdParam.Value = Rr.Password;

            DbParameter employeeParam = RegisterCommand.CreateParameter();
            employeeParam.ParameterName = "isEmployee";
            employeeParam.DbType = DbType.Boolean;
            employeeParam.Value = false;

            RegisterCommand.Parameters.Add(fnameParam);
            RegisterCommand.Parameters.Add(lnameParam);
            RegisterCommand.Parameters.Add(emailParam);
            RegisterCommand.Parameters.Add(phoneParam);
            RegisterCommand.Parameters.Add(pwdParam);
            RegisterCommand.Parameters.Add(employeeParam);

            return RegisterCommand.ExecuteNonQuery() == 1;
        }

        public string GetNameByEmail(string email)
        {
            DbCommand cmdEmail = this._connection.CreateCommand();
            cmdEmail.CommandText = "select firstname from user where email = @email";

            DbParameter emailParam = cmdEmail.CreateParameter();
            emailParam.ParameterName = "email";
            emailParam.DbType = DbType.String;
            emailParam.Value = email;

            cmdEmail.Parameters.Add(emailParam);

            using(DbDataReader reader = cmdEmail.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return Convert.ToString(reader["firstname"]);
                }
            }
            return "NoName";
        }

        public List<string> GetAllEmails()
        {
            List<string> emails = new List<string>();
            DbCommand cmdGetEmails = _connection.CreateCommand();
            cmdGetEmails.CommandText = "select email from user";

            using(DbDataReader reader = cmdGetEmails.ExecuteReader())
            {
                while (reader.Read())
                {
                    emails.Add(Convert.ToString(reader["email"]));
                }
            }

            return emails;
        }

        public int GetIdByEmail(string email)
        {
            DbCommand cmdGetID = _connection.CreateCommand();
            cmdGetID.CommandText = "select user_id from user where email = @email";

            DbParameter emailParam = cmdGetID.CreateParameter();
            emailParam.ParameterName = "email";
            emailParam.DbType = DbType.String;
            emailParam.Value = email;

            cmdGetID.Parameters.Add(emailParam);

            using(DbDataReader reader = cmdGetID.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return Convert.ToInt32(reader["user_id"]);
                }
            }
            return -1;
        }

        public bool GetIsAdminByEmail(string email)
        {
            DbCommand cmdGetAdmin = _connection.CreateCommand();
            cmdGetAdmin.CommandText = "select is_employee from user where email = @email";

            DbParameter emailParam = cmdGetAdmin.CreateParameter();
            emailParam.ParameterName = "email";
            emailParam.DbType = DbType.String;
            emailParam.Value = email;

            cmdGetAdmin.Parameters.Add(emailParam);

            using(DbDataReader reader = cmdGetAdmin.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return Convert.ToBoolean(reader["is_employee"]);
                }
            }
            return false;
        }

        public User GetUserById(int userIdToFetch)
        {
            DbCommand cmdGetUserById = _connection.CreateCommand();
            cmdGetUserById.CommandText = "select * from user where user_id = @userId";

            DbParameter userIdParam = cmdGetUserById.CreateParameter();
            userIdParam.ParameterName = "userId";
            userIdParam.DbType = DbType.Int32;
            userIdParam.Value = userIdToFetch;

            cmdGetUserById.Parameters.Add(userIdParam);

            using(DbDataReader reader = cmdGetUserById.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return new User {
                        UserId = userIdToFetch,
                        FirstName = Convert.ToString(reader["firstname"]),
                        LastName = Convert.ToString(reader["lastname"]),
                        Email = Convert.ToString(reader["email"]),
                        PhoneNumber = Convert.ToString(reader["phone"]),
                        IsAdmin = Convert.ToBoolean(reader["is_employee"])
                    };
                }
            }
            return null;
        }

        public List<AddressResponse> GetAddressesByUserId(int userId)
        {
            List<AddressResponse> foundAddresses = new();

            DbCommand cmdGetAddresses = _connection.CreateCommand();
            cmdGetAddresses.CommandText = "select * from user_address " +
                "join address using(address_id) join city using(city_id) where user_id=@userId";

            DbParameter userIdParam = cmdGetAddresses.CreateParameter();
            userIdParam.ParameterName = "userId";
            userIdParam.DbType = DbType.Int32;
            userIdParam.Value = userId;

            cmdGetAddresses.Parameters.Add(userIdParam);

            using(DbDataReader reader = cmdGetAddresses.ExecuteReader())
            {
                while (reader.Read())
                {
                    foundAddresses.Add(new AddressResponse {
                        AddressId = Convert.ToInt32(reader["address_id"]),
                        State = Convert.ToString(reader["state"]),
                        PostalCode = Convert.ToString(reader["postal_code"]),
                        City = Convert.ToString(reader["city"]),
                        Street = Convert.ToString(reader["street"]),
                        StreetNumber = Convert.ToString(reader["street_number"])
                    });
                }
            }
            return foundAddresses;
        }

        public bool ChangeUser(User newUser)
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
            userIdParam.Value = newUser.UserId;

            cmdChangeUser.Parameters.Add(firstNameParam);
            cmdChangeUser.Parameters.Add(lastNameParam);
            cmdChangeUser.Parameters.Add(emailParam);
            cmdChangeUser.Parameters.Add(phoneParam);
            cmdChangeUser.Parameters.Add(isAdminParam);
            cmdChangeUser.Parameters.Add(userIdParam);

            return cmdChangeUser.ExecuteNonQuery() == 1;
        }

        public AddressResponse GetAddressById(int addressId)
        {
            DbCommand cmdGetAddress = _connection.CreateCommand();
            cmdGetAddress.CommandText = "select * from address join city using(city_id) where address_id=@addressId";

            DbParameter addressIdParam = cmdGetAddress.CreateParameter();
            addressIdParam.ParameterName = "addressId";
            addressIdParam.DbType = DbType.Int32;
            addressIdParam.Value = addressId;

            cmdGetAddress.Parameters.Add(addressIdParam);

            using(DbDataReader reader = cmdGetAddress.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();

                    return new AddressResponse {
                        AddressId = addressId,
                        State = Convert.ToString(reader["state"]),
                        PostalCode = Convert.ToString(reader["postal_code"]),
                        City = Convert.ToString(reader["city"]),
                        Street = Convert.ToString(reader["street"]),
                        StreetNumber = Convert.ToString(reader["street_number"])

                    };
                }
            }
            return null;
        }

        
    }
}
