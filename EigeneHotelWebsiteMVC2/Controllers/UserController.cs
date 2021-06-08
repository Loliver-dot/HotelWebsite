using Microsoft.AspNetCore.Mvc;
using EigeneHotelWebsiteMVC2.Models.DB;
using Microsoft.AspNetCore.Http;
using EigeneHotelWebsiteMVC2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;

namespace EigeneHotelWebsiteMVC2.Controllers
{
    public class UserController : Controller
    {
        IRepositoryUser repUser = new RepositoryUser();
        IRepositoryAddress repAddress = new RepositoryAddress();
        public IActionResult ShowProfile()
        {
            if(HttpContext.Session.GetInt32("userId") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            UserAddresses userAddresses = new UserAddresses {
                User = new(),
                Addresses = new()
            };
            try
            {
                repUser.Open();
                userAddresses.User = repUser.GetUserById((int)HttpContext.Session.GetInt32("userId"));
                userAddresses.Addresses = repUser.GetAddressesByUserId((int)HttpContext.Session.GetInt32("userId"));
                // TODO
                // -----------------------------
                // Get all addresses from user and write it to ShowProfile
            } catch (DbException)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "DbError in ShowProfile"
                });
            }
            finally
            {
                repUser.Close();
            }
            return View(userAddresses);
        }
        [HttpPost]
        public IActionResult ShowProfile(int? UserId)
        {
            User user = new();
            try
            {
                repUser.Open();
                user = repUser.GetUserById((int)UserId);
            } catch (DbException){
                return View("Error", new ErrorViewModel {
                    RequestId = "DbError in ChangeUserData"
                });
            }
            finally
            {
                repUser.Close();
            }
            return View("ChangeUserData", user);
        }
        public IActionResult ChangeUserDataResponse(User newUser, string IsAdmin) { 

            if (IsAdmin == "on")
            {
                newUser.IsAdmin = true;
            }
            else{
                newUser.IsAdmin = false;
            }
            try
            {
                repUser.Open();
                repUser.ChangeUser(newUser);
            } catch (DbException)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "DbError in ChangeUserDataResponse"
                });
            }
            finally
            {
                repUser.Close();
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult ChangeAddressData(int? addressId)
        {
            AddressResponse address = new();
            if(HttpContext.Session.GetInt32("isLoggedIn") != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                repUser.Open();
                address = repUser.GetAddressById((int)addressId);

            } catch (DbException)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "DbError in ChangeAddressData"
                });
            }
            finally
            {
                repUser.Close();
            }
            return View(address);
        }
        public IActionResult ChangeAddressDataResponse(AddressResponse newAddress)
        {
            if (HttpContext.Session.GetInt32("isLoggedIn") != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                repAddress.Open();
                if (!repAddress.Update(newAddress))
                {
                    return View("Error", new ErrorViewModel {
                        RequestId = "Addressdaten konnten nicht geändert werden"
                    });
                }
            } catch (DbException)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "DbError in ChangeAddressDataResponse"
                });
            }
            finally
            {
                repAddress.Close();
            }
            return RedirectToAction("Index", "Home");
        }

    }
}
