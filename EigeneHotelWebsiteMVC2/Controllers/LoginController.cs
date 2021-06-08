using EigeneHotelWebsiteMVC2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EigeneHotelWebsiteMVC2.Models.DB;
using System.Data.Common;

namespace EigeneHotelWebsiteMVC2.Controllers
{
    public class LoginController : Controller
    {
        IRepositoryUser repUser = new RepositoryUser();
        IRepositoryAddress repAddress = new RepositoryAddress();

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(LoginResponse Lr)
        {
            try
            {
                repUser.Open();
                if (repUser.Login(Lr))
                {
                    HttpContext.Session.SetInt32("userId", repUser.GetIdByEmail(Lr.UserName));
                    HttpContext.Session.SetString("name", repUser.GetNameByEmail(Lr.UserName));
                    HttpContext.Session.SetInt32("isLoggedIn", 1);
                    HttpContext.Session.SetInt32("isAdmin", Convert.ToInt32(repUser.GetIsAdminByEmail(Lr.UserName)));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Ihr Konto wurde nicht gefunden";
                    return View();
                }
                
            }
            catch (DbException)
            {
                HttpContext.Session.SetString("name", "DatenbankFehler");
                return RedirectToAction("Index", "Home");
            }
            finally
            {
                repUser.Close();
            }
            
             
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterResponse Rr)
        {
            if (Rr.Password != Rr.PasswordRepeat)
            {
                ViewBag.Error = "Die beiden Passwörter stimmen nicht überein";
                return View();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    repUser.Open();
                    if (repUser.Register(Rr))
                    {
                        HttpContext.Session.SetString("name", Rr.FirstName);
                        HttpContext.Session.SetString("email", Rr.Email);
                        HttpContext.Session.SetInt32("isLoggedIn", 1);
                    }
                    else
                    {
                        ViewBag.Error = "Ein Konto mit dieser email wurde bereits gefunden!";
                        return View();
                    }

                    

                    return View("Address");
                }
                catch (DbException)
                {
                    return View("Error");
                }
                finally
                {
                    repUser.Close();
                }
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Address()
        {
            if (HttpContext.Session.GetInt32("isLoggedIn") == 1)
            {
                return View();
            }
            return View("Register");
        }
        [HttpPost]
        public IActionResult Address(AddressResponse addressResponse)
        {
            try
            {
                string email = HttpContext.Session.GetString("email");
                repAddress.Open();
                repAddress.Insert(addressResponse, (int)HttpContext.Session.GetInt32("userId"));
            }
            catch(DbException e)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "DbError in Address\n" + e
                } );
            }
            finally
            {
                repAddress.Close();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
