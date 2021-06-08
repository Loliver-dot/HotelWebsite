using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EigeneHotelWebsiteMVC2.Models.DB
{
    interface IRepositoryBase
    {
        void Open();
        void Close();
    }
}
