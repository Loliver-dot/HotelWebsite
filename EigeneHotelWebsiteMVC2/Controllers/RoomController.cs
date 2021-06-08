using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EigeneHotelWebsiteMVC2.Models.DB;
using EigeneHotelWebsiteMVC2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;




namespace EigeneHotelWebsiteMVC2.Controllers
{
    public class RoomController : Controller
    {
        IRepositoryRoom repRoom = new RepositroyRoom();
        List<int> BookedRoomsIds = new List<int>();
        List<RoomBill> BookedRoomBills = new();

        [HttpGet]
        public IActionResult RoomsIndex()
        {
            List<Room> allRooms = new();
            try
            {
                repRoom.Open();
                allRooms = repRoom.GetAllRooms();
            } catch (DbException)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "Some Db Error. I don´t know myself."
                });
            }
            finally
            {
                repRoom.Close();
            }

            return View(allRooms);
        }

        [HttpGet]
        public IActionResult RoomInfo()
        {

            return View("RoomsIndex");
        }
        [HttpPost]
        public IActionResult RoomInfo(int? LookUpId)
        {
            Room room;
            TempData["LookUpId"] = LookUpId;
            LookUpId = Convert.ToInt32(TempData["LookUpId"]);

            if (LookUpId == null || LookUpId == 0)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "id ist null / 0"
                });
            }

            try
            {
                repRoom.Open();
                room = repRoom.GetRoomByID(LookUpId);
            } catch (DbException e)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "DB Error in RoomInfo" + e
                });
            }
            finally
            {
                repRoom.Close();
            }
            return View(room);
        }

        [HttpGet]
        public IActionResult Booking()
        {

            return View("RoomsIndex");
        }

        [HttpPost]
        public IActionResult Booking1(int? BookingId)
        {
            HttpContext.Session.SetInt32("roomId", Convert.ToInt32(BookingId));
            BookingId = BookingId == null || BookingId == 0 ? HttpContext.Session.GetInt32("roomId") : BookingId;


            Room room;
            RoomBillAndNumberOfBeds numberOfBeds = new RoomBillAndNumberOfBeds {
                NumberOfBeds = 0,
                RoomBill = new()
            };
            try
            {
                repRoom.Open();
                room = repRoom.GetRoomByID(BookingId);
            } catch (DbException)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "Some Db Error"
                });
            }
            finally
            {
                repRoom.Close();
            }
            numberOfBeds.NumberOfBeds = room.NumberOfBeds;
            return View("Booking", numberOfBeds);
        }

        [HttpPost]
        public IActionResult Booking(int? BookingId, RoomBillAndNumberOfBeds roomBillAndNumberOfBeds)
        {
            BookingId = BookingId == null || BookingId == 0 ? HttpContext.Session.GetInt32("roomId") : BookingId;

            roomBillAndNumberOfBeds.RoomBill.RoomId = BookingId;
            Room room;

            if (ModelState.IsValid)
            {
                try
                {

                    repRoom.Open();
                    room = repRoom.GetRoomByID(BookingId);
                    if (SessionHelper.GetObjectFromJson<List<int>>(HttpContext.Session, "bookingIds") != null)
                    {
                        BookedRoomsIds = SessionHelper.GetObjectFromJson<List<int>>(HttpContext.Session, "bookingIds");
                    }

                    BookedRoomsIds.Add((int)BookingId);

                    if (SessionHelper.GetObjectFromJson<List<RoomBill>>(HttpContext.Session, "roomBill") != null)
                    {
                        BookedRoomBills = SessionHelper.GetObjectFromJson<List<RoomBill>>(HttpContext.Session, "roomBill");
                    }
                    BookedRoomBills.Add(roomBillAndNumberOfBeds.RoomBill);

                    SessionHelper.SetObjectAsJson(HttpContext.Session, "roomBill", BookedRoomBills);
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "bookingIds", BookedRoomsIds);
                } catch (DbException)
                {
                    return View("Error", new ErrorViewModel {
                        RequestId = "Some Db Error"
                    });
                }
                finally
                {
                    repRoom.Close();
                }
                return View("BillingInformation");
            }
            else
            {
                try
                {
                    repRoom.Open();
                    room = repRoom.GetRoomByID(BookingId);
                } catch (DbException)
                {
                    return View("Error", new ErrorViewModel {
                        RequestId = "Some Db Error"
                    });
                }
                finally
                {
                    repRoom.Close();
                }
                roomBillAndNumberOfBeds.NumberOfBeds = room.NumberOfBeds;
                return View(roomBillAndNumberOfBeds);
            }


        }
        [HttpGet]
        public IActionResult BillingInformation()
        {

            return View();
        }
        [HttpPost]
        public IActionResult BillingInformation(Bill bill, string paid)
        {
            int? BookingId = HttpContext.Session.GetInt32("roomId");
            int? UserId = HttpContext.Session.GetInt32("userId");
            double sum = 0;

            Room room = null;

            bill.IsPaid = paid == "on" ? true : false;

            if (ModelState.IsValid)
            {
                bill.LastBillingDate = DateTime.UtcNow.AddDays(30);
                if ((bool)bill.IsPaid)
                {
                    bill.BillingDate = DateTime.UtcNow;
                }


                try
                {
                    repRoom.Open();
                    room = repRoom.GetRoomByID(BookingId);
                    repRoom.InsertBill(bill, (int)UserId);
                    BookedRoomsIds = SessionHelper.GetObjectFromJson<List<int>>(HttpContext.Session, "bookingIds");
                    BookedRoomBills = SessionHelper.GetObjectFromJson<List<RoomBill>>(HttpContext.Session, "roomBill");

                    for (int i = 0; i < BookedRoomsIds.Count; i++)
                    {
                        repRoom.InsertRoomBill(BookedRoomBills[i], repRoom.GetBillLength(), BookedRoomsIds[i]);
                        sum += repRoom.GetFullPrice(repRoom.GetBillLength(), BookedRoomsIds[i]);
                    }

                } catch (DbException)
                {
                    return View("Error", new ErrorViewModel {
                        RequestId = "DbError in Bill"
                    });
                }
                finally
                {
                    repRoom.Close();
                }

                HttpContext.Session.Remove("bookingIds");
                HttpContext.Session.Remove("roomBill");
                ViewBag.Price = Math.Round(sum, 2);
                return View("Thanks", bill);
            }
            else
            {
                return View();
            }

        }
        public IActionResult Thanks()
        {
            return View();
        }

        public IActionResult SearchByName(string SearchByName)
        {
            List<Room> foundRooms = new();
            try
            {
                repRoom.Open();
                if (SearchByName == null)
                {
                    foundRooms = repRoom.GetAllRooms();
                }
                else
                {

                    foundRooms = repRoom.GetAllRooms().Where(r => r.RoomName.ToLower().Contains(SearchByName.ToLower())).ToList().Count > 0 ? repRoom.GetAllRooms().Where(r => r.RoomName.ToLower().Contains(SearchByName.ToLower())).ToList() : null;
                }
            } catch (DbException)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "Some Db Error. I don´t know myself."
                });
            }
            finally
            {
                repRoom.Close();
            }

            return View("RoomsIndex", foundRooms);
        }
        public IActionResult SearchByPriceRange(double? bottom, double? top)
        {
            List<Room> foundRooms = new();
            try
            {
                repRoom.Open();
                if (bottom == null || top == null)
                {
                    foundRooms = repRoom.GetAllRooms();
                }
                else
                {
                    foundRooms = repRoom.GetAllRooms().Where(r => r.PricePerNight > bottom && r.PricePerNight < top).ToList().Count > 0 ? repRoom.GetAllRooms().Where(r => r.PricePerNight > bottom && r.PricePerNight < top).ToList() : null ;
                }
            } catch (DbException)
            {
                return View("Error", new ErrorViewModel {
                    RequestId = "Some Db Error. I don´t know myself."
                });
            }
            finally
            {
                repRoom.Close();
            }

            return View("RoomsIndex", foundRooms);
        }
    }
}