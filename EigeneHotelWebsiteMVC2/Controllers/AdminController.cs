using Microsoft.AspNetCore.Mvc;
using EigeneHotelWebsiteMVC2.Models;
using EigeneHotelWebsiteMVC2.Models.DB;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;

namespace EigeneHotelWebsiteMVC2.Controllers
{
    public class AdminController : Controller
    {
        IRepositoryAdmin repAdmin = new RepositoryAdmin();
        [HttpGet]
        public IActionResult AddRoom()
        {
            if (HttpContext.Session.GetInt32("isAdmin") != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            RoomAndAllFeature Features = new RoomAndAllFeature {
                Room = new(),
                AllRoomFeatures = new()
            };
            try
            {
                repAdmin.Open();
                
                Features.AllRoomFeatures = repAdmin.GetRoomFeatures();
            } catch (DbException e)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "Db Error in AddRoom(Features)" + e
                });
            }
            finally
            {
                repAdmin.Close();
            }
            return View(Features);
        }

        [HttpPost]
        public IActionResult AddRoom(Room room, string FeaturesIDs)
        {
            // FeaturesIDs im Format
            // " 2"
            // oder " 2 3"
            if (HttpContext.Session.GetInt32("isAdmin") != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            string[] featuresString = FeaturesIDs.Trim().Split(" ");
            List<int> features = new();
            foreach (string s in featuresString)
            {
                features.Add(Convert.ToInt32(s));
            }
            room.RoomFeatures = new();
            if (ModelState.IsValid)
            {
                try
                {
                    repAdmin.Open();
                    foreach(int featureId in features)
                    {
                        room.RoomFeatures.Add(repAdmin.GetFeatureById(featureId));
                    }
                    repAdmin.AddRoom(room);
                } catch (DbException e)
                {
                    return View("Error", new ErrorViewModel {
                        RequestId = "DbError in AddRoom " + e
                    });
                }
                finally
                {
                    repAdmin.Close();
                }

                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult ShowUsers()
        {
            if (HttpContext.Session.GetInt32("isAdmin") != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            List<User> AllUsers = new();
            try
            {
                repAdmin.Open();
                AllUsers = repAdmin.getAllUsers();
            } catch (DbException)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "DbErrorInGetAllUsers"
                });
            }
            finally
            {
                repAdmin.Close();
            }
            return View(AllUsers);
        }

        public IActionResult SearchByName(string SearchByName)
        {
            if (HttpContext.Session.GetInt32("isAdmin") != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            List<User> FoundUsers = new();
            try
            {
                repAdmin.Open();
                FoundUsers = repAdmin.SearchByName(SearchByName);
            } catch(DbException)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "DbError in SearchByName"
                });
            }
            finally
            {
                repAdmin.Close();
            }

            return View("ShowUsers", FoundUsers);
        }

        public IActionResult SearchByPhone(string SearchByPhone)
        {
            if (HttpContext.Session.GetInt32("isAdmin") != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            List<User> FoundUsers = new();
            try
            {
                repAdmin.Open();
                FoundUsers = repAdmin.SearchByPhone(SearchByPhone);
            } catch (DbException)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "DbError in SearchByName"
                });
            }
            finally
            {
                repAdmin.Close();
            }

            return View("ShowUsers", FoundUsers);
        }
  
        [HttpPost]
        public IActionResult EditUserInformation(int? UserId)
        {
            if (HttpContext.Session.GetInt32("isAdmin") != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            User user = new();
            try
            {
                repAdmin.Open();
                user = repAdmin.getAllUsers().Where(r => r.UserId == UserId).First();
            } catch (DbException e)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "DbError in EditUserInformation \n" + e
                });
            }
            finally
            {
                repAdmin.Close();
            }
            return View(user);

        }
             
        [HttpPost]
        public IActionResult EditUserInformationResponse(User userToChange, string isAdmin)
        {
            if (HttpContext.Session.GetInt32("isAdmin") != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            if (isAdmin == "on")
            {
                userToChange.IsAdmin = true;
            }
            else{
                userToChange.IsAdmin = false;
            }

            try
            {
                repAdmin.Open();
                if(!repAdmin.ChangeUser(userToChange, userToChange.UserId))
                {
                    return View("Error", new ErrorViewModel {
                        RequestId = "User was not changed"
                    });
                }
            } catch (DbException)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "DbError in EditUserInformation"
                });
            }
            finally
            {
                repAdmin.Close();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ChangeRoomData(int? RoomId)
        {
            if (HttpContext.Session.GetInt32("isAdmin") != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            RoomAndAllFeature r = new();
            try
            {
                repAdmin.Open();
                r.Room = repAdmin.GetRoomByID(RoomId);
                r.AllRoomFeatures = repAdmin.GetRoomFeatures();
            } catch (DbException e)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "DbError in ChangeRoomData \n" + e
                });
            }
            finally
            {
                repAdmin.Close();
            }
            return View(r);
        }

        [HttpPost]
        public IActionResult ChangeRoomDataResponse(RoomAndAllFeature roomRoomFeature, string roomFeatures)
        {
            if (HttpContext.Session.GetInt32("isAdmin") != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            string[] featuresString = roomFeatures.Trim().Split(" ");
            List<int> features = new();
            foreach (string s in featuresString)
            {
                features.Add(Convert.ToInt32(s));
            }
            roomRoomFeature.Room.RoomFeatures = new();
            

            try
            {
                repAdmin.Open();
                
                foreach (int featureId in features)
                {
                    roomRoomFeature.Room.RoomFeatures.Add(repAdmin.GetFeatureById(featureId));
                }
                if (repAdmin.ChangeRoom(roomRoomFeature.Room, roomRoomFeature.Room.RoomID))
                {
                    return RedirectToAction("RoomsIndex", "Room");
                }
            } catch (DbException){
                return View("Error", new ErrorViewModel {
                    RequestId = "DbError in ChangeRoomDataresponse"
                });
            }
            finally
            {
                repAdmin.Close();
            }
            return RedirectToAction("Index", "Home");

        }
    }
}

